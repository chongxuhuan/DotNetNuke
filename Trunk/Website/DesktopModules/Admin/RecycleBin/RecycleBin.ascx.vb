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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Utilities
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.RecycleBin

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The RecycleBin PortalModuleBase allows Tabs and Modules to be recovered or
    ''' prmanentl deleted
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/15/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class RecycleBin
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads deleted tabs and modules into the lists 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	
        '''   [VMasanas]  20/08/2004  Update display information for deleted modules to:
        '''               ModuleFriendlyName: ModuleTitle - Tab: TabName
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()
            Dim objModules As New ModuleController
            Dim objTabs As New TabController
            Dim objModule As ModuleInfo
            Dim objTab As TabInfo
            Dim intModule As Integer

            lstModules.Items.Clear()
            lstTabs.Items.Clear()

            Dim arrDeletedTabs As New ArrayList
            For Each objTab In TabController.GetTabsBySortOrder(PortalId)
                If objTab.IsDeleted = True Then
                    arrDeletedTabs.Add(objTab)
                End If
            Next

            Dim arrModules As ArrayList = objModules.GetModules(PortalId)
            For intModule = 0 To arrModules.Count - 1
                objModule = CType(arrModules(intModule), ModuleInfo)
                If objModule.IsDeleted = True Then
                    If objModule.ModuleTitle = "" Then
                        objModule.ModuleTitle = objModule.DesktopModule.FriendlyName
                    End If
                    objTab = objTabs.GetTab(objModule.TabID, PortalId, False)
                    If objTab IsNot Nothing Then
                        lstModules.Items.Add(New ListItem(objTab.TabName & " - " & objModule.ModuleTitle, objModule.TabID.ToString & "-" & objModule.ModuleID.ToString))
                    Else
                        lstModules.Items.Add(New ListItem(objModule.ModuleTitle, objModule.TabID.ToString & "-" & objModule.ModuleID.ToString))
                    End If
                End If
            Next

            lstTabs.DataSource = arrDeletedTabs
            lstTabs.DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a module
        ''' </summary>
        ''' <param name="intModuleId">ModuleId of the module to be deleted</param>
        ''' <remarks>
        ''' Adds a log entry for the action to the EvenLog
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteModule(ByVal intModuleId As Integer)
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            ' delete module
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(intModuleId, Null.NullInteger, False)
            If Not objModule Is Nothing Then
                'hard-delete Tab Module INstance
                objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, False)
                objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_DELETED)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a tab
        ''' </summary>
        ''' <param name="objTab">The tab to be deleted</param>
        ''' <remarks>
        ''' Adds a log entry for the action to the EventLog
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Created
        '''                 19/09/2004  Remove skin deassignment. BLL takes care of this.
        '''                 30/09/2004  Change logic so log is only added when tab is actually deleted
        '''                 28/02/2005  Remove modules when deleting pages
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteTab(ByVal objTab As TabInfo, ByVal deleteDescendants As Boolean)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objTabs As New TabController
            Dim objModules As New ModuleController

            'get tab modules before deleting page
            Dim dicTabModules As Dictionary(Of Integer, ModuleInfo) = objModules.GetTabModules(objTab.TabID)

            ' hard delete the tab
            objTabs.DeleteTab(objTab.TabID, objTab.PortalID, deleteDescendants)

            'delete modules that do not have other instances
            For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In dicTabModules
                ' check if all modules instances have been deleted
                Dim objDelModule As ModuleInfo = objModules.GetModule(kvp.Value.ModuleID, Null.NullInteger, False)
                If objDelModule Is Nothing OrElse objDelModule.TabID = Null.NullInteger Then
                    objModules.DeleteModule(kvp.Value.ModuleID)
                End If
            Next
            objEventLog.AddLog(objTab, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_DELETED)

        End Sub

        Private Function RestoreTab(ByVal objTab As TabInfo) As Boolean
            Dim success As Boolean = True
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objTabs As New TabController

            If Not objTab Is Nothing Then
                If Not Null.IsNull(objTab.ParentId) AndAlso Not lstTabs.Items.FindByValue(objTab.ParentId.ToString) Is Nothing Then
                    UI.Skins.Skin.AddModuleMessage(Me, String.Format(Services.Localization.Localization.GetString("ChildTab.ErrorMessage", Me.LocalResourceFile()), objTab.TabName), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    success = False
                Else
                    TabController.RestoreTab(objTab, PortalSettings, UserId)
                End If
            End If

            Return success
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Add confirmation for Empty Recycle Bin button
        ''' 	[cnurse]	15/09/2004	Localized Confirm text
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim ResourceFileRoot As String = Me.TemplateSourceDirectory + "/" + Services.Localization.Localization.LocalResourceDirectory + "/" + Me.ID
            ' If this is the first visit to the page
            If (Page.IsPostBack = False) Then
                ClientAPI.AddButtonConfirm(cmdDeleteTab, Services.Localization.Localization.GetString("DeleteTab", ResourceFileRoot))
                ClientAPI.AddButtonConfirm(cmdDeleteModule, Services.Localization.Localization.GetString("DeleteModule", ResourceFileRoot))
                ClientAPI.AddButtonConfirm(cmdEmpty, Services.Localization.Localization.GetString("DeleteAll", ResourceFileRoot))

                tblModuleButtons.Visible = Me.IsEditable
                tblTabButtons.Visible = Me.IsEditable
                cmdEmpty.Visible = Me.IsEditable

                BindData()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Restores selected tabs in the listbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Adds a log entry for each restored tab to the EventLog
        ''' Redirects to same page after restoring so the menu can be refreshed with restored tabs.
        ''' This will not restore deleted modules for selected tabs, only the tabs are restored.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        '''                 30/09/2004  Child tabs cannot be restored until their parent is restored first.
        '''                             Change logic so log is only added when tab is actually restored
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRestoreTab_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdRestoreTab.Click
            Dim item As ListItem
            Dim errors As Boolean = False

            For Each item In lstTabs.Items
                If item.Selected Then
                    Dim objTabs As New TabController
                    Dim objTab As TabInfo = objTabs.GetTab(Integer.Parse(item.Value), PortalId, False)

                    If Not RestoreTab(objTab) Then
                        errors = True
                    End If
                End If
            Next
            If Not errors Then
                Response.Redirect(NavigateURL())
            Else
                BindData()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes selected tabs in the listbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Parent tabs will not be deleted. To delete a parent tab all child tabs need to be deleted before.
        ''' Reloads data to refresh deleted modules and tabs listboxes
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDeleteTab_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDeleteTab.Click
            Dim item As ListItem

            For Each item In lstTabs.Items
                If item.Selected Then
                    Dim intTabId As Integer = Integer.Parse(item.Value)
                    Dim objTabs As New TabController
                    Dim objTab As TabInfo = objTabs.GetTab(intTabId, PortalId, False)
                    If objTab IsNot Nothing Then
                        If objTab.HasChildren Then
                            UI.Skins.Skin.AddModuleMessage(Me, String.Format(DotNetNuke.Services.Localization.Localization.GetString("ParentTab.ErrorMessage", Me.LocalResourceFile()), objTab.TabName), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        Else
                            DeleteTab(objTab, False)
                        End If
                    End If
                End If
            Next
            BindData()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Restores selected modules in the listbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Adds a log entry for each restored module to the EventLog
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRestoreModule_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdRestoreModule.Click
            Dim item As ListItem
            Dim errors As Boolean = False
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objModules As New ModuleController

            For Each item In lstModules.Items
                If item.Selected Then
                    Dim values As String() = item.Value.Split("-")
                    Dim tabId As Integer = Integer.Parse(values(0))
                    Dim moduleId As Integer = Integer.Parse(values(1))

                    ' restore module
                    Dim objModule As ModuleInfo = objModules.GetModule(moduleId, tabId, False)
                    If Not objModule Is Nothing Then
                        objModules.RestoreModule(objModule)
                        objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_RESTORED)
                    End If
                End If
            Next
            BindData()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes selected modules in the listbox
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Added support for multiselect listbox
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDeleteModule_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDeleteModule.Click
            Dim item As ListItem
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim objModules As New ModuleController

            For Each item In lstModules.Items
                If item.Selected Then
                    Dim values As String() = item.Value.Split("-")
                    Dim tabId As Integer = Integer.Parse(values(0))
                    Dim moduleId As Integer = Integer.Parse(values(1))

                    ' delete module
                    Dim objModule As ModuleInfo = objModules.GetModule(moduleId, tabId, False)
                    If Not objModule Is Nothing Then
                        'hard-delete Tab Module Instance
                        objModules.DeleteTabModule(tabId, moduleId, False)
                        objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_DELETED)
                    End If
                End If
            Next
            BindData()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Permanently removes all deleted tabs and modules
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Parent tabs will not be deleted. To delete a parent tab all child tabs need to be deleted before.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	18/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdEmpty_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEmpty.Click
            Dim item As ListItem
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            For Each item In lstModules.Items
                Dim objModules As New ModuleController
                Dim values As String() = item.Value.Split("-")
                Dim tabId As Integer = Integer.Parse(values(0))
                Dim moduleId As Integer = Integer.Parse(values(1))

                ' delete module
                Dim objModule As ModuleInfo = objModules.GetModule(moduleId, tabId, False)
                If Not objModule Is Nothing Then
                    'hard-delete Tab Module Instance
                    objModules.DeleteTabModule(tabId, moduleId, False)
                    objEventLog.AddLog(objModule, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_DELETED)
                End If
            Next
            For Each item In lstTabs.Items
                Dim intTabId As Integer = Integer.Parse(item.Value)
                Dim objTabs As New TabController
                Dim objTab As TabInfo = objTabs.GetTab(intTabId, PortalId, False)

                If objTab IsNot Nothing Then
                    DeleteTab(objTab, True)
                End If
            Next
            BindData()

        End Sub

#End Region

    End Class

End Namespace
