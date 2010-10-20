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

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.Modules

    ''' <summary>
    ''' Provides context data for a particular instance of a module
    ''' </summary>
    Public Class ModuleInstanceContext

#Region "Private Fields"

        Private _actions As ModuleActionCollection
        Private _nextActionId As Integer = -1
        Private _configuration As ModuleInfo
        Private _isEditable As Nullable(Of Boolean) = Nothing
        Private _settings As Hashtable
        Private _helpurl As String
        Private _moduleControl As IModuleControl

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Actions for this module context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Actions() As ModuleActionCollection
            Get
                If _actions Is Nothing Then
                    LoadActions(HttpContext.Current.Request)
                End If
                Return _actions
            End Get
            Set(ByVal Value As ModuleActionCollection)
                _actions = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Module Configuration (ModuleInfo) for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Configuration() As ModuleInfo
            Get
                Return _configuration
            End Get
            Set(ByVal value As ModuleInfo)
                _configuration = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The EditMode property is used to determine whether the user is in the 
        ''' Administrator role
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property EditMode() As Boolean
            Get
                Return TabPermissionController.CanAdminPage()
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the HelpUrl for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HelpURL() As String
            Get
                Return _helpurl
            End Get
            Set(ByVal Value As String)
                _helpurl = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the module is Editable (in Admin mode)
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsEditable() As Boolean
            Get
                ' Perform tri-state switch check to avoid having to perform a security
                ' role lookup on every property access (instead caching the result)
                If Not _isEditable.HasValue Then

                    Dim blnPreview As Boolean = (PortalSettings.UserMode = PortalSettings.Mode.View)
                    If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                        blnPreview = False
                    End If

                    Dim blnHasModuleEditPermissions As Boolean = False
                    If Not _configuration Is Nothing Then
                        blnHasModuleEditPermissions = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "CONTENT", Configuration)
                    End If

                    If blnPreview = False And blnHasModuleEditPermissions = True Then
                        _isEditable = True
                    Else
                        _isEditable = False
                    End If
                End If

                Return _isEditable.Value
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the module ID for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleId() As Integer
            Get
                If Not _configuration Is Nothing Then
                    Return _configuration.ModuleID
                Else
                    Return Null.NullInteger
                End If
            End Get
            Set(ByVal value As Integer)
                If Not _configuration Is Nothing Then
                    _configuration.ModuleID = value
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the settings for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Settings() As Hashtable
            Get
                Dim controller As New ModuleController()
                If _settings Is Nothing Then
                    ' we need to make sure we don't directly modify the ModuleSettings so create new HashTable DNN-8715
                    _settings = New Hashtable(controller.GetModuleSettings(ModuleId))

                    ' add the TabModuleSettings to the ModuleSettings
                    Dim tabModuleSettings As Hashtable = controller.GetTabModuleSettings(TabModuleId)
                    For Each strKey As String In tabModuleSettings.Keys
                        _settings(strKey) = tabModuleSettings(strKey)
                    Next

                End If
                Return _settings
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the tab ID for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property TabId() As Integer
            Get
                If Not _configuration Is Nothing Then
                    Return Convert.ToInt32(_configuration.TabID)
                Else
                    Return Null.NullInteger
                End If
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the tabnmodule ID for this context
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property TabModuleId() As Integer
            Get
                If Not _configuration Is Nothing Then
                    Return Convert.ToInt32(_configuration.TabModuleID)
                Else
                    Return Null.NullInteger
                End If
            End Get
            Set(ByVal value As Integer)
                If Not _configuration Is Nothing Then
                    _configuration.TabModuleID = value
                End If
            End Set
        End Property


#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ModuleInstanceContext
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ModuleInstanceContext
        ''' </summary>
        ''' <param name="moduleControl">The Module Control for this context</param>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal moduleControl As IModuleControl)
            _moduleControl = moduleControl
        End Sub

#End Region

#Region "To be moved to global objects"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether this module is on a Host Menu
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsHostMenu() As Boolean
            Get
                Dim _IsHost As Boolean = False
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    _IsHost = True
                End If
                Return _IsHost
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the portal Settings for this module
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the portal Alias for this module
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PortalAlias() As PortalAliasInfo
            Get
                Return PortalSettings.PortalAlias
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the portal Id for this module
        ''' </summary>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PortalId() As Integer
            Get
                Return PortalSettings.PortalId
            End Get
        End Property

#End Region

#Region "Public Methods"

#Region "Public Function EditUrl(...) As String"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the URL for an "Edit" control
        ''' </summary>
        ''' <history>
        '''   [cnurse]   03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function EditUrl() As String
            Return EditUrl("", "", "Edit")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the URL for an "Edit" control
        ''' </summary>
        ''' <param name="ControlKey">The key for the Control (ctl=key)</param>
        ''' <history>
        '''   [cnurse]   03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function EditUrl(ByVal ControlKey As String) As String
            Return EditUrl("", "", ControlKey)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the URL for an "Edit" control
        ''' </summary>
        ''' <param name="KeyName">The name of Querysstring Paramater</param>
        ''' <param name="KeyValue">The value of a Querystring Parameter</param>
        ''' <history>
        '''   [cnurse]   03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String) As String
            Return EditUrl(KeyName, KeyValue, "Edit")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the URL for an "Edit" control
        ''' </summary>
        ''' <param name="ControlKey">The key for the Control (ctl=key)</param>
        ''' <param name="KeyName">The name of Querysstring Paramater</param>
        ''' <param name="KeyValue">The value of a Querystring Parameter</param>
        ''' <history>
        '''   [cnurse]   03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String, ByVal ControlKey As String) As String
            Dim params(0) As String
            Return EditUrl(KeyName, KeyValue, ControlKey, params)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the URL for an "Edit" control
        ''' </summary>
        ''' <param name="ControlKey">The key for the Control (ctl=key)</param>
        ''' <param name="KeyName">The name of Querysstring Paramater</param>
        ''' <param name="KeyValue">The value of a Querystring Parameter</param>
        ''' <param name="AdditionalParameters">A collection of extra Parameters</param>
        ''' <history>
        '''   [cnurse]   03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String, ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String

            Dim key As String = ControlKey

            If String.IsNullOrEmpty(key) Then
                key = "Edit"
            End If

            Dim ModuleIdParam As String = String.Empty
            If Configuration IsNot Nothing Then
                ModuleIdParam = String.Format("mid={0}", Configuration.ModuleID)
            End If

            Dim params() As String
            If Not String.IsNullOrEmpty(KeyName) And Not String.IsNullOrEmpty(KeyValue) Then
                ReDim params(AdditionalParameters.Length + 2)

                params(0) = ModuleIdParam
                params(1) = String.Format("{0}={1}", KeyName, KeyValue)

                Array.Copy(AdditionalParameters, 0, params, 2, AdditionalParameters.Length)
            Else
                ReDim params(AdditionalParameters.Length)

                params(0) = ModuleIdParam

                Array.Copy(AdditionalParameters, 0, params, 1, AdditionalParameters.Length)
            End If
            Return NavigateURL(PortalSettings.ActiveTab.TabID, key, params)

        End Function

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Next Action ID
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetNextActionID() As Integer
            _nextActionId += 1
            Return _nextActionId
        End Function

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddHelpActions Adds the Help actions to the Action Menu
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/12/2005	Documented
        '''     [cnurse]    01/19/2006  Moved from ActionBase
        '''     [cnurse]    12/24/2007  Renamed (from SetHelpVisibility)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddHelpActions()
            Dim helpAction As New ModuleAction(GetNextActionID)

            'Add Help Menu Action
            helpAction.Title = Localization.GetString(ModuleActionType.ModuleHelp, Localization.GlobalResourceFile)
            helpAction.CommandName = ModuleActionType.ModuleHelp
            helpAction.CommandArgument = ""
            helpAction.Icon = "action_help.gif"
            helpAction.Url = NavigateURL(TabId, "Help", "ctlid=" & Configuration.ModuleControlId.ToString, "moduleid=" & ModuleId)
            helpAction.Secure = SecurityAccessLevel.Edit
            helpAction.Visible = True
            helpAction.NewWindow = False
            helpAction.UseActionEvent = True
            _actions.Add(helpAction)

            'Add OnLine Help Action
            Dim helpURL As String = GetOnLineHelp(Configuration.ModuleControl.HelpURL, Configuration)
            If Not String.IsNullOrEmpty(helpURL) Then
                'Add OnLine Help menu action
                helpAction = New ModuleAction(GetNextActionID())
                helpAction.Title = Localization.GetString(ModuleActionType.OnlineHelp, Localization.GlobalResourceFile)
                helpAction.CommandName = ModuleActionType.OnlineHelp
                helpAction.CommandArgument = ""
                helpAction.Icon = "action_help.gif"
                helpAction.Url = FormatHelpUrl(helpURL, PortalSettings, Configuration.DesktopModule.FriendlyName)
                helpAction.Secure = SecurityAccessLevel.Edit
                helpAction.UseActionEvent = True
                helpAction.Visible = True
                helpAction.NewWindow = True
                _actions.Add(helpAction)
            End If
        End Sub

        Private Sub AddPrintAction()
            Dim action As New ModuleAction(GetNextActionID())
            action.Title = Localization.GetString(ModuleActionType.PrintModule, Localization.GlobalResourceFile)
            action.CommandName = ModuleActionType.PrintModule
            action.CommandArgument = ""
            action.Icon = "action_print.gif"
            action.Url = NavigateURL(TabId, "", "mid=" & ModuleId.ToString, "SkinSrc=" & QueryStringEncode("[G]" & SkinController.RootSkin & "/" & glbHostSkinFolder & "/" & "No Skin"), "ContainerSrc=" & QueryStringEncode("[G]" & SkinController.RootContainer & "/" & glbHostSkinFolder & "/" & "No Container"), "dnnprintmode=true")
            action.Secure = SecurityAccessLevel.Anonymous
            action.UseActionEvent = True
            action.Visible = True
            action.NewWindow = True
            _actions.Add(action)
        End Sub

        Private Sub AddSyndicateAction()
            Dim action As New ModuleAction(GetNextActionID())
            action.Title = Localization.GetString(ModuleActionType.SyndicateModule, Localization.GlobalResourceFile)
            action.CommandName = ModuleActionType.SyndicateModule
            action.CommandArgument = ""
            action.Icon = "action_rss.gif"
            action.Url = NavigateURL(PortalSettings.ActiveTab.TabID, "", "moduleid=" & ModuleId.ToString).Replace(glbDefaultPage, "RSS.aspx")
            action.Secure = SecurityAccessLevel.Anonymous
            action.UseActionEvent = True
            action.Visible = True
            action.NewWindow = True
            _actions.Add(action)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddMenuMoveActions Adds the Move actions to the Action Menu
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    01/04/2008  Refactored from LoadActions
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddMenuMoveActions()
            ' module movement
            _actions.Add(GetNextActionID, "~", "")
            Dim MoveActionRoot As New ModuleAction(GetNextActionID, Localization.GetString(ModuleActionType.MoveRoot, Localization.GlobalResourceFile), "", "", "", "", "", False, SecurityAccessLevel.View, True, False)

            ' move module up/down
            If Not Configuration Is Nothing Then
                If (Configuration.ModuleOrder <> 0) And (Configuration.PaneModuleIndex > 0) Then
                    MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveTop, Localization.GlobalResourceFile), ModuleActionType.MoveTop, Configuration.PaneName, "action_top.gif", "", False, SecurityAccessLevel.View, True, False)
                    MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveUp, Localization.GlobalResourceFile), ModuleActionType.MoveUp, Configuration.PaneName, "action_up.gif", "", False, SecurityAccessLevel.View, True, False)
                End If
                If (Configuration.ModuleOrder <> 0) And (Configuration.PaneModuleIndex < (Configuration.PaneModuleCount - 1)) Then
                    MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveDown, Localization.GlobalResourceFile), ModuleActionType.MoveDown, Configuration.PaneName, "action_down.gif", "", False, SecurityAccessLevel.View, True, False)
                    MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MoveBottom, Localization.GlobalResourceFile), ModuleActionType.MoveBottom, Configuration.PaneName, "action_bottom.gif", "", False, SecurityAccessLevel.View, True, False)
                End If

            End If

            ' move module to pane
            For Each obj As Object In PortalSettings.ActiveTab.Panes
                Dim pane As String = TryCast(obj, String)
                If Not String.IsNullOrEmpty(pane) AndAlso Not Configuration.PaneName.Equals(pane, StringComparison.InvariantCultureIgnoreCase) Then
                    MoveActionRoot.Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.MovePane, Localization.GlobalResourceFile) & " " & pane, ModuleActionType.MovePane, pane, "action_move.gif", "", False, SecurityAccessLevel.View, True, False)
                End If
            Next

            If MoveActionRoot.Actions.Count > 0 Then
                _actions.Add(MoveActionRoot)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetActionsCount gets the current number of actions
        ''' </summary>
        ''' <param name="actions">The actions collection to count.</param>
        ''' <param name="count">The current count</param>
        ''' <history>
        '''     [cnurse]    01/04/2008  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetActionsCount(ByVal count As Integer, ByVal actions As ModuleActionCollection) As Integer

            For Each action As ModuleAction In actions
                If action.HasChildren Then
                    count += action.Actions.Count

                    'Recursively call to see if this collection has any child actions that would affect the count
                    count = GetActionsCount(count, action.Actions)
                End If
            Next

            Return count

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadActions loads the Actions collections
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    01/19/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadActions(ByVal Request As HttpRequest)

            _actions = New ModuleActionCollection
            Dim maxActionId As Integer = Null.NullInteger

            'check if module Implements Entities.Modules.IActionable interface
            Dim actionable As IActionable = TryCast(_moduleControl, IActionable)
            If actionable IsNot Nothing Then
                ' load module actions
                Dim ModuleActions As ModuleActionCollection = actionable.ModuleActions

                For Each action As ModuleAction In ModuleActions
                    If ModulePermissionController.HasModuleAccess(action.Secure, "CONTENT", Configuration) Then
                        If action.Icon = "" Then
                            action.Icon = "edit.gif"
                        End If
                        If action.ID > maxActionId Then
                            maxActionId = action.ID
                        End If
                        _actions.Add(action)
                    End If
                Next
            End If

            'Make sure the Next Action Id counter is correct
            Dim actionCount As Integer = GetActionsCount(_actions.Count(), _actions)
            If _nextActionId < maxActionId Then
                _nextActionId = maxActionId
            End If
            If _nextActionId < actionCount Then
                _nextActionId = actionCount
            End If

            If Not String.IsNullOrEmpty(Configuration.DesktopModule.BusinessControllerClass) Then
                ' check if module implements IPortable interface, and user has Admin permissions
                If Configuration.DesktopModule.IsPortable Then
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "EXPORT", Configuration) Then
                        _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ExportModule, Localization.GlobalResourceFile), "", "", "action_export.gif", NavigateURL(PortalSettings.ActiveTab.TabID, "ExportModule", "moduleid=" & ModuleId.ToString), "", False, SecurityAccessLevel.View, True, False)
                    End If
                    If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "IMPORT", Configuration) Then
                        _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ImportModule, Localization.GlobalResourceFile), "", "", "action_import.gif", NavigateURL(PortalSettings.ActiveTab.TabID, "ImportModule", "moduleid=" & ModuleId.ToString), "", False, SecurityAccessLevel.View, True, False)
                    End If
                End If

                If Configuration.DesktopModule.IsSearchable AndAlso Configuration.DisplaySyndicate Then
                    AddSyndicateAction()
                End If
            End If

            ' help module actions available to content editors and administrators
            Dim permisisonList As String = "CONTENT,DELETE,EDIT,EXPORT,IMPORT,MANAGE"
            If Configuration.ModuleID > Null.NullInteger AndAlso ModulePermissionController.HasModulePermission(Configuration.ModulePermissions, permisisonList) AndAlso _
                            Request.QueryString("ctl") <> "Help" Then
                AddHelpActions()
            End If

            'Add Print Action
            If Configuration.DisplayPrint Then
                ' print module action available to everyone
                AddPrintAction()
            End If

            If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Host, "MANAGE", Configuration) Then
                _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ViewSource, Localization.GlobalResourceFile), ModuleActionType.ViewSource, "", "action_source.gif", NavigateURL(TabId, "ViewSource", "ctlid=" & Configuration.ModuleControlId.ToString), False, SecurityAccessLevel.Host, True, False)
            End If

            If Not IsAdminControl() AndAlso ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "DELETE,MANAGE", Configuration) Then
                _actions.Add(GetNextActionID, "~", "")
                If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "MANAGE", Configuration) Then
                    _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ModuleSettings, Localization.GlobalResourceFile), ModuleActionType.ModuleSettings, "", "action_settings.gif", NavigateURL(TabId, "Module", "ModuleId=" & ModuleId.ToString), False, SecurityAccessLevel.Edit, True, False)
                End If
                If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "DELETE", Configuration) Then
                    _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.DeleteModule, Localization.GlobalResourceFile), ModuleActionType.DeleteModule, Configuration.ModuleID.ToString, "action_delete.gif", "", "confirm('" + DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Localization.GetString("DeleteModule.Confirm")) + "')", False, SecurityAccessLevel.View, True, False)
                End If
                If ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Admin, "MANAGE", Configuration) Then
                    _actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.ClearCache, Localization.GlobalResourceFile), ModuleActionType.ClearCache, Configuration.ModuleID.ToString, "action_refresh.gif", "", False, SecurityAccessLevel.View, True, False)

                    ' module movement
                    AddMenuMoveActions()
                End If
            End If
        End Sub

#End Region

    End Class


End Namespace