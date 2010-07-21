'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Containers
Imports DotNetNuke.Entities.Modules.Actions
Imports System.Collections.Generic

Namespace DotNetNuke.UI
    Public Class Navigation

#Region "Enums"

        Public Enum ToolTipSource
            TabName
            Title
            Description
            None
        End Enum

        Public Enum NavNodeOptions
            IncludeSelf = CInt(2 ^ 0)
            IncludeParent = CInt(2 ^ 1)
            IncludeSiblings = CInt(2 ^ 2)
            MarkPendingNodes = CInt(2 ^ 3)
        End Enum

#End Region

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Recursive function to add module's actions to the DNNNodeCollection based off of passed in ModuleActions
        ''' </summary>
        ''' <param name="objParentAction">Parent action</param>
        ''' <param name="objParentNode">Parent node</param>
        ''' <param name="objActionControl">ActionControl to base actions off of</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddChildActions(ByVal objParentAction As ModuleAction, ByVal objParentNode As DNNNode, ByVal objActionControl As IActionControl)
            AddChildActions(objParentAction, objParentNode, objParentNode, objActionControl, -1)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Recursive function to add module's actions to the DNNNodeCollection based off of passed in ModuleActions
        ''' </summary>
        ''' <param name="objParentAction">Parent action</param>
        ''' <param name="objParentNode">Parent node</param>
        ''' <param name="objActionControl">ActionControl to base actions off of</param>
        ''' <param name="intDepth">How many levels deep should be populated</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	5/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddChildActions(ByVal objParentAction As ModuleAction, ByVal objParentNode As DNNNode, ByVal objRootNode As DNNNode, ByVal objActionControl As IActionControl, ByVal intDepth As Integer)
            ' Add Menu Items

            Dim objAction As DotNetNuke.Entities.Modules.Actions.ModuleAction
            Dim blnPending As Boolean
            For Each objAction In objParentAction.Actions
                blnPending = IsActionPending(objParentNode, objRootNode, intDepth)
                If objAction.Title = "~" Then
                    If blnPending = False Then
                        'A title (text) of ~ denotes a break
                        objParentNode.DNNNodes.AddBreak()
                    End If
                Else
                    'if action is visible and user has permission 
                    If objAction.Visible = True AndAlso _
                            (objAction.Secure <> DotNetNuke.Security.SecurityAccessLevel.Anonymous OrElse _
                            ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", objActionControl.ModuleControl.ModuleContext.Configuration)) AndAlso _
                            ModulePermissionController.HasModuleAccess(objAction.Secure, Null.NullString, objActionControl.ModuleControl.ModuleContext.Configuration) Then
                        If blnPending Then
                            objParentNode.HasNodes = True
                        Else
                            Dim objNode As DNNNode
                            Dim i As Integer = objParentNode.DNNNodes.Add()
                            objNode = objParentNode.DNNNodes(i)
                            objNode.ID = objAction.ID.ToString
                            objNode.Key = objAction.ID.ToString
                            objNode.Text = objAction.Title                                 'no longer including SPACE in generic node collection, each control must handle how they want to display
                            If Len(objAction.ClientScript) > 0 Then
                                objNode.JSFunction = objAction.ClientScript
                                objNode.ClickAction = eClickAction.None
                            Else
                                objNode.NavigateURL = objAction.Url
                                If objAction.UseActionEvent = False AndAlso Len(objNode.NavigateURL) > 0 Then
                                    objNode.ClickAction = eClickAction.Navigate
                                    If objAction.NewWindow Then
                                        objNode.Target = "_blank"
                                    End If
                                Else
                                    objNode.ClickAction = eClickAction.PostBack
                                End If
                            End If
                            objNode.Image = objAction.Icon

                            If objAction.HasChildren Then                                 'if action has children then call function recursively
                                AddChildActions(objAction, objNode, objRootNode, objActionControl, intDepth)
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns common properties from passed in tab to newly created DNNNode that is added to the passed in DNNNodeCollection
        ''' </summary>
        ''' <param name="objTab">Tab to base DNNNode off of</param>
        ''' <param name="objNodes">Node collection to append new node to</param>
        ''' <param name="objBreadCrumbs">Hashtable of breadcrumb IDs to efficiently determine node's BreadCrumb property</param>
        ''' <param name="objPortalSettings">Portal settings object to determine if node is selected</param>
        ''' <remarks>
        ''' Logic moved to separate sub to make GetNavigationNodes cleaner
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddNode(ByVal objTab As TabInfo, ByVal objNodes As DNNNodeCollection, ByVal objBreadCrumbs As Hashtable, ByVal objPortalSettings As PortalSettings, ByVal eToolTips As ToolTipSource)
            Dim objNode As DNNNode = New DNNNode

            If objTab.Title = "~" Then    ' NEW!
                'A title (text) of ~ denotes a break
                objNodes.AddBreak()
            Else
                'assign breadcrumb and selected properties
                If objBreadCrumbs.Contains(objTab.TabID) Then
                    objNode.BreadCrumb = True
                    If objTab.TabID = objPortalSettings.ActiveTab.TabID Then
                        objNode.Selected = True
                    End If
                End If

                If objTab.DisableLink Then objNode.Enabled = False

                objNode.ID = objTab.TabID.ToString
                objNode.Key = objNode.ID
                objNode.Text = objTab.LocalizedTabName
                objNode.NavigateURL = objTab.FullUrl
                objNode.ClickAction = eClickAction.Navigate
                objNode.Image = objTab.IconFile

                Select Case eToolTips
                    Case ToolTipSource.TabName
                        objNode.ToolTip = objTab.LocalizedTabName
                    Case ToolTipSource.Title
                        objNode.ToolTip = objTab.Title
                    Case ToolTipSource.Description
                        objNode.ToolTip = objTab.Description
                End Select

                objNodes.Add(objNode)
            End If
        End Sub

        Private Shared Function IsActionPending(ByVal objParentNode As DNNNode, ByVal objRootNode As DNNNode, ByVal intDepth As Integer) As Boolean
            'if we aren't restricting depth then its never pending
            If intDepth = -1 Then Return False

            'parents level + 1 = current node level
            'if current node level - (roots node level) <= the desired depth then not pending
            If objParentNode.Level + 1 - objRootNode.Level <= intDepth Then Return False
            Return True
        End Function

        Private Shared Function IsTabPending(ByVal objTab As TabInfo, ByVal objParentNode As DNNNode, ByVal objRootNode As DNNNode, ByVal intDepth As Integer, ByVal objBreadCrumbs As Hashtable, ByVal intLastBreadCrumbId As Integer, ByVal blnPOD As Boolean) As Boolean
            '
            ' A
            ' |
            ' --B
            ' | |
            ' | --B-1
            ' | | |
            ' | | --B-1-1
            ' | | |
            ' | | --B-1-2
            ' | |
            ' | --B-2
            ' |   |
            ' |   --B-2-1
            ' |   |
            ' |   --B-2-2
            ' |
            ' --C
            '   |
            '   --C-1
            '   | |
            '   | --C-1-1
            '   | |
            '   | --C-1-2
            '   |
            '   --C-2
            '     |
            '     --C-2-1
            '     |
            '     --C-2-2

            'if we aren't restricting depth then its never pending
            If intDepth = -1 Then Return False

            'parents level + 1 = current node level
            'if current node level - (roots node level) <= the desired depth then not pending
            If objParentNode.Level + 1 - objRootNode.Level <= intDepth Then Return False


            '--- These checks below are here so tree becomes expands to selected node ---'
            If blnPOD Then
                'really only applies to controls with POD enabled, since the root passed in may be some node buried down in the chain
                'and the depth something like 1.  We need to include the appropriate parent's and parent siblings
                'Why is the check for POD required?  Well to allow for functionality like RootOnly requests.  We do not want any children
                'regardless if they are a breadcrumb

                'if tab is in the breadcrumbs then obviously not pending
                If objBreadCrumbs.Contains(objTab.TabID) Then Return False

                'if parent is in the breadcrumb and it is not the last breadcrumb then not pending
                'in tree above say we our breadcrumb is (A, B, B-2) we want our tree containing A, B, B-2 AND B-1 AND C since A and B are expanded
                'we do NOT want B-2-1 and B-2-2, thus the check for Last Bread Crumb
                If objBreadCrumbs.Contains(objTab.ParentId) AndAlso intLastBreadCrumbId <> objTab.ParentId Then Return False
            End If

            Return True
            'if depth matters and if parents level + 1 (current node level) - (roots node level) > the desired depth and not a breadcrumb tab and parent tabid not a breadcrumb (or parent breadcrumb is last in chain, thus we mark as pending)
            'If intDepth <> -1 AndAlso objParentNode.Level + 1 - objRootNode.Level > intDepth AndAlso objBreadCrumbs.Contains(objTab.TabID) = False AndAlso (objBreadCrumbs.Contains(objTab.ParentId) = False OrElse intLastBreadCrumbId = objTab.ParentId) Then
            '	Return True
            'Else
            '	Return False
            'End If
        End Function

        Private Shared Function IsTabSibling(ByVal objTab As TabInfo, ByVal intStartTabId As Integer, ByVal objTabLookup As Hashtable) As Boolean
            If intStartTabId = -1 Then
                If objTab.ParentId = -1 Then
                    Return True
                Else
                    Return False
                End If
            ElseIf objTab.ParentId = CType(objTabLookup(intStartTabId), TabInfo).ParentId Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Shared Sub ProcessTab(ByVal objRootNode As DNNNode, ByVal objTab As TabInfo, ByVal objTabLookup As Hashtable, ByVal objBreadCrumbs As Hashtable, ByVal intLastBreadCrumbId As Integer, ByVal eToolTips As ToolTipSource, ByVal intStartTabId As Integer, ByVal intDepth As Integer, ByVal intNavNodeOptions As Integer)
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Dim objRootNodes As DNNNodeCollection = objRootNode.DNNNodes

            If CanShowTab(objTab, TabPermissionController.CanAdminPage(), True) Then       'based off of tab properties, is it shown
                Dim objParentNodes As DNNNodeCollection
                Dim objParentNode As DNNNode = objRootNodes.FindNode(objTab.ParentId.ToString)
                Dim blnParentFound As Boolean = Not objParentNode Is Nothing
                If objParentNode Is Nothing Then objParentNode = objRootNode
                objParentNodes = objParentNode.DNNNodes

                If objTab.TabID = intStartTabId Then
                    'is this the starting tab
                    If (intNavNodeOptions And NavNodeOptions.IncludeParent) <> 0 Then
                        'if we are including parent, make sure there is one, then add
                        If Not objTabLookup(objTab.ParentId) Is Nothing Then
                            AddNode(CType(objTabLookup(objTab.ParentId), TabInfo), objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips)
                            objParentNode = objRootNodes.FindNode(objTab.ParentId.ToString)
                            objParentNodes = objParentNode.DNNNodes
                        End If
                    End If
                    If (intNavNodeOptions And NavNodeOptions.IncludeSelf) <> 0 Then
                        'if we are including our self (starting tab) then add
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips)
                    End If
                ElseIf ((intNavNodeOptions And NavNodeOptions.IncludeSiblings) <> 0) AndAlso IsTabSibling(objTab, intStartTabId, objTabLookup) Then
                    'is this a sibling of the starting node, and we are including siblings, then add it
                    AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips)
                Else
                    If blnParentFound Then          'if tabs parent already in hierarchy (as is the case when we are sending down more than 1 level)
                        'parent will be found for siblings.  Check to see if we want them, if we don't make sure tab is not a sibling
                        If ((intNavNodeOptions And NavNodeOptions.IncludeSiblings) <> 0) OrElse IsTabSibling(objTab, intStartTabId, objTabLookup) = False Then
                            'determine if tab should be included or marked as pending
                            Dim blnPOD As Boolean = (intNavNodeOptions And NavNodeOptions.MarkPendingNodes) <> 0
                            If IsTabPending(objTab, objParentNode, objRootNode, intDepth, objBreadCrumbs, intLastBreadCrumbId, blnPOD) Then
                                If blnPOD Then
                                    objParentNode.HasNodes = True             'mark it as a pending node
                                End If
                            Else
                                AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips)
                            End If
                        End If
                    ElseIf (intNavNodeOptions And NavNodeOptions.IncludeSelf) = 0 AndAlso objTab.ParentId = intStartTabId Then
                        'if not including self and parent is the start id then add 
                        AddNode(objTab, objParentNodes, objBreadCrumbs, objPortalSettings, eToolTips)
                    End If
                End If
            End If
        End Sub

#End Region

#Region "Public Shared Methods"

        Public Shared Function CanShowTab(ByVal objTab As TabInfo, ByVal isAdminMode As Boolean, ByVal showDisabled As Boolean) As Boolean
            'if tab is visible, not deleted, not expired (or admin), and user has permission to see it...
            If objTab.IsVisible AndAlso Not objTab.IsDeleted AndAlso _
                    (Not objTab.DisableLink OrElse showDisabled) AndAlso _
                    (((objTab.StartDate < Now OrElse objTab.StartDate = Null.NullDate) AndAlso _
                            (objTab.EndDate > Now OrElse objTab.EndDate = Null.NullDate)) OrElse _
                        isAdminMode) AndAlso TabPermissionController.CanNavigateToPage(objTab) Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows for DNNNode object to be easily obtained based off of passed in ID
        ''' </summary>
        ''' <param name="strID">NodeID to retrieve</param>
        ''' <param name="strNamespace">Namespace for node collection (usually control's ClientID)</param>
        ''' <param name="objActionRoot">Root Action object used in searching</param>
        ''' <param name="objControl">ActionControl to base actions off of</param>
        ''' <returns>DNNNode</returns>
        ''' <remarks>
        ''' Primary purpose of this is to obtain the DNNNode needed for the events exposed by the NavigationProvider
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	5/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetActionNode(ByVal strID As String, ByVal strNamespace As String, ByVal objActionRoot As ModuleAction, ByVal objControl As Control) As DNNNode
            Dim objNodes As DNNNodeCollection = GetActionNodes(objActionRoot, objControl, -1)
            Dim objNode As DNNNode = objNodes.FindNode(strID)
            Dim objReturnNodes As DNNNodeCollection = New DNNNodeCollection(strNamespace)
            objReturnNodes.Import(objNode)
            objReturnNodes(0).ID = strID
            Return objReturnNodes(0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the actions associated 
        ''' to a module based off of the current user's context
        ''' </summary>
        ''' <param name="objActionRoot">Root module action</param>
        ''' <param name="objControl">ActionControl to base actions off of</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetActionNodes(ByVal objActionRoot As ModuleAction, ByVal objControl As Control) As DNNNodeCollection
            Return GetActionNodes(objActionRoot, objControl, -1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the actions associated 
        ''' to a module based off of the current user's context
        ''' </summary>
        ''' <param name="objActionRoot">Root module action</param>
        ''' <param name="objControl">ActionControl to base actions off of</param>
        ''' <param name="intDepth">How many levels deep should be populated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	5/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetActionNodes(ByVal objActionRoot As ModuleAction, ByVal objControl As Control, ByVal intDepth As Integer) As DNNNodeCollection
            Dim objCol As DNNNodeCollection = New DNNNodeCollection(objControl.ClientID)

            Dim objActionControl As IActionControl = TryCast(objControl, IActionControl)
            If objActionControl IsNot Nothing Then
                If objActionRoot.Visible Then
                    objCol.Add()
                    Dim objRoot As DNNNode = objCol.Item(0)
                    objRoot.ID = objActionRoot.ID.ToString
                    objRoot.Key = objActionRoot.ID.ToString
                    objRoot.Text = objActionRoot.Title
                    objRoot.NavigateURL = objActionRoot.Url
                    objRoot.Image = objActionRoot.Icon
                    objRoot.Enabled = False
                    AddChildActions(objActionRoot, objRoot, objRoot.ParentNode, objActionControl, intDepth)
                End If
            End If
            Return objCol
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the actions associated 
        ''' to a module based off of the current user's context
        ''' </summary>
        ''' <param name="objActionRoot">Root module action</param>
        ''' <param name="objRootNode">Root node on which to populate children</param>
        ''' <param name="objControl">ActionControl to base actions off of</param>
        ''' <param name="intDepth">How many levels deep should be populated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	5/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetActionNodes(ByVal objActionRoot As ModuleAction, ByVal objRootNode As DNNNode, ByVal objControl As Control, ByVal intDepth As Integer) As DNNNodeCollection
            Dim objCol As DNNNodeCollection = objRootNode.ParentNode.DNNNodes
            Dim objActionControl As IActionControl = TryCast(objControl, IActionControl)
            If objActionControl IsNot Nothing Then
                AddChildActions(objActionRoot, objRootNode, objRootNode, objActionControl, intDepth)
            End If
            Return objCol
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows for DNNNode object to be easily obtained based off of passed in ID
        ''' </summary>
        ''' <param name="strID">NodeID to retrieve</param>
        ''' <param name="strNamespace">Namespace for node collection (usually control's ClientID)</param>
        ''' <returns>DNNNode</returns>
        ''' <remarks>
        ''' Primary purpose of this is to obtain the DNNNode needed for the events exposed by the NavigationProvider
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNavigationNode(ByVal strID As String, ByVal strNamespace As String) As DNNNode
            'TODO:  FIX THIS MESS!
            Dim objNodes As DNNNodeCollection = GetNavigationNodes(strNamespace)
            Dim objNode As DNNNode = objNodes.FindNode(strID)
            Dim objReturnNodes As DNNNodeCollection = New DNNNodeCollection(strNamespace)
            objReturnNodes.Import(objNode)
            objReturnNodes(0).ID = strID
            Return objReturnNodes(0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the pages/tabs included in
        ''' the current context's (user) navigation hierarchy
        ''' </summary>
        ''' <param name="strNamespace">Namespace (typically control's ClientID) of node collection to create</param>
        ''' <returns>Collection of DNNNodes</returns>
        ''' <remarks>
        ''' Returns all navigation nodes for a given user 
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNavigationNodes(ByVal strNamespace As String) As DNNNodeCollection
            Return GetNavigationNodes(strNamespace, ToolTipSource.None, -1, -1, 0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the pages/tabs included in
        ''' the current context's (user) navigation hierarchy
        ''' </summary>
        ''' <param name="strNamespace">Namespace (typically control's ClientID) of node collection to create</param>
        ''' <param name="eToolTips">Enumerator to determine what text to display in the tooltips</param>
        ''' <param name="intStartTabId">If using Populate On Demand, then this is the tab id of the root element to retrieve (-1 for no POD)</param>
        ''' <param name="intDepth">If Populate On Demand is enabled, then this parameter determines the number of nodes to retrieve beneath the starting tab passed in (intStartTabId) (-1 for no POD)</param>
        ''' <param name="intNavNodeOptions">Bitwise integer containing values to determine what nodes to display (self, siblings, parent)</param>
        ''' <returns>Collection of DNNNodes</returns>
        ''' <remarks>
        ''' Returns a subset of navigation nodes based off of passed in starting node id and depth
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNavigationNodes(ByVal strNamespace As String, ByVal eToolTips As ToolTipSource, ByVal intStartTabId As Integer, ByVal intDepth As Integer, ByVal intNavNodeOptions As Integer) As DNNNodeCollection
            Dim objCol As DNNNodeCollection = New DNNNodeCollection(strNamespace)
            Return GetNavigationNodes(New DNNNode(objCol.XMLNode), eToolTips, intStartTabId, intDepth, intNavNodeOptions)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This function provides a central location to obtain a generic node collection of the pages/tabs included in
        ''' the current context's (user) navigation hierarchy
        ''' </summary>
        ''' <param name="objRootNode">Node in which to add children to</param>
        ''' <param name="eToolTips">Enumerator to determine what text to display in the tooltips</param>
        ''' <param name="intStartTabId">If using Populate On Demand, then this is the tab id of the root element to retrieve (-1 for no POD)</param>
        ''' <param name="intDepth">If Populate On Demand is enabled, then this parameter determines the number of nodes to retrieve beneath the starting tab passed in (intStartTabId) (-1 for no POD)</param>
        ''' <param name="intNavNodeOptions">Bitwise integer containing values to determine what nodes to display (self, siblings, parent)</param>
        ''' <returns>Collection of DNNNodes</returns>
        ''' <remarks>
        ''' Returns a subset of navigation nodes based off of passed in starting node id and depth
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	8/9/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetNavigationNodes(ByVal objRootNode As DNNNode, ByVal eToolTips As ToolTipSource, ByVal intStartTabId As Integer, ByVal intDepth As Integer, ByVal intNavNodeOptions As Integer) As DNNNodeCollection
            Dim i As Integer
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim blnFoundStart As Boolean = intStartTabId = -1    'if -1 then we want all

            Dim objBreadCrumbs As Hashtable = New Hashtable
            Dim objTabLookup As Hashtable = New Hashtable
            Dim objRootNodes As DNNNodeCollection = objRootNode.DNNNodes
            Dim intLastBreadCrumbId As Integer

            '--- cache breadcrumbs in hashtable so we can easily set flag on node denoting it as a breadcrumb node (without looping multiple times) ---'
            For i = 0 To (objPortalSettings.ActiveTab.BreadCrumbs.Count - 1)
                objBreadCrumbs.Add(CType(objPortalSettings.ActiveTab.BreadCrumbs(i), TabInfo).TabID, 1)
                intLastBreadCrumbId = CType(objPortalSettings.ActiveTab.BreadCrumbs(i), TabInfo).TabID
            Next

            Dim objTabController As New TabController()
            Dim portalTabs As List(Of TabInfo) = TabController.GetTabsBySortOrder(objPortalSettings.PortalId, objPortalSettings.CultureCode, True)
            Dim hostTabs As List(Of TabInfo) = TabController.GetTabsBySortOrder(Null.NullInteger, Localization.SystemLocale, True)
            For Each objTab As TabInfo In portalTabs
                objTabLookup.Add(objTab.TabID, objTab)
            Next
            For Each objTab As TabInfo In hostTabs
                objTabLookup.Add(objTab.TabID, objTab)
            Next

            For Each objTab As TabInfo In portalTabs
                Try
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions)
                Catch ex As Exception
                    Throw ex
                End Try
            Next
            For Each objTab As TabInfo In hostTabs
                Try
                    ProcessTab(objRootNode, objTab, objTabLookup, objBreadCrumbs, intLastBreadCrumbId, eToolTips, intStartTabId, intDepth, intNavNodeOptions)
                Catch ex As Exception
                    Throw ex
                End Try
            Next

            Return objRootNodes
        End Function


#End Region

    End Class
End Namespace