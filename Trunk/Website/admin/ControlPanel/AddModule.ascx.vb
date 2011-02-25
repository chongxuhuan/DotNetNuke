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

Imports System.Collections.Generic

Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Portals.PortalSettings
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.ControlPanel

    Partial Class AddModule
        Inherits System.Web.UI.UserControl
        Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool

#Region "Event Handlers"

        Protected Sub AddNewOrExisting_OnClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddExistingModule.CheckedChanged, AddNewModule.CheckedChanged
            LoadAllLists()
        End Sub

        Protected Sub PaneLst_OnSelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PaneLst.SelectedIndexChanged
            LoadPositionList()
            LoadPaneModulesList()
            'Dim script As String = String.Format(glbScriptFormat, ResolveUrl("~/Resources/ControlPanel/ControlPanel.js"))
            'ClientAPI.RegisterStartUpScript(Page, "controlPanel", script)
        End Sub

        Protected Sub PageLst_OnSelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PageLst.SelectedIndexChanged
            LoadModuleList()
        End Sub

        Protected Sub PositionLst_OnSelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PositionLst.SelectedIndexChanged
            PaneModulesLst.Enabled = PositionLst.SelectedValue = "ABOVE" Or PositionLst.SelectedValue = "BELOW"
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'For Pane Highlighting: Resources/ControlPanel/ControlPanel.js
            'jQuery.RequestRegistration()
            'Dim scriptUrl As String = ResolveUrl("~/Resources/ControlPanel/ControlPanel.js")
            'Page.ClientScript.RegisterClientScriptInclude("ControlPanel_AddModule", scriptUrl)
        End Sub

        Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If (Visible) Then
                    cmdAddModule.Enabled = Enabled
                    AddExistingModule.Enabled = Enabled
                    AddNewModule.Enabled = Enabled
                    Title.Enabled = Enabled
                    PageLst.Enabled = Enabled
                    ModuleLst.Enabled = Enabled
                    VisibilityLst.Enabled = Enabled
                    PaneLst.Enabled = Enabled
                    PositionLst.Enabled = Enabled
                    PaneModulesLst.Enabled = Enabled
                End If

                If (Not IsPostBack() AndAlso Visible AndAlso Enabled) Then
                    LoadAllLists()
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        End Sub

        Protected Sub cmdAddModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddModule.Click
            If TabPermissionController.CanAddContentToPage() AndAlso CanAddModuleToPage() Then
                Dim permissionType As Integer = 0
                Try
                    permissionType = Integer.Parse(VisibilityLst.SelectedValue)
                Catch ex As Exception
                    permissionType = 0
                End Try

                Dim position As Integer = -1
                Select Case PositionLst.SelectedValue
                    Case "TOP"
                        position = 0
                    Case "ABOVE"
                        If Not String.IsNullOrEmpty(PaneModulesLst.SelectedValue) Then
                            Try
                                position = Integer.Parse(PaneModulesLst.SelectedValue) - 1
                            Catch ex As Exception
                                position = -1
                            End Try
                        Else
                            position = 0
                        End If
                    Case "BELOW"
                        If Not String.IsNullOrEmpty(PaneModulesLst.SelectedValue) Then
                            Try
                                position = Integer.Parse(PaneModulesLst.SelectedValue) + 1
                            Catch ex As Exception
                                position = -1
                            End Try
                        Else
                            position = -1
                        End If
                    Case "BOTTOM"
                        position = -1
                End Select

                Dim moduleLstID As Integer = -1
                Try
                    moduleLstID = Integer.Parse(ModuleLst.SelectedValue)
                Catch ex As Exception
                    moduleLstID = -1
                End Try

                If (moduleLstID > -1) Then
                    If (AddExistingModule.Checked) Then
                        Dim pageID As Integer = -1
                        Try
                            pageID = Integer.Parse(PageLst.SelectedValue)
                        Catch ex As Exception
                            pageID = -1
                        End Try

                        If (pageID > -1) Then
                            DoAddExistingModule(moduleLstID, pageID, PaneLst.SelectedValue, position, "", chkCopyModule.Checked)
                        End If
                    Else
                        DoAddNewModule(Title.Text, moduleLstID, PaneLst.SelectedValue, position, permissionType, "")
                    End If
                End If

                Response.Redirect(Request.RawUrl, True)
            End If
        End Sub

        Protected Sub ModuleLst_OnSelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ModuleLst.SelectedIndexChanged

            Dim moduleCtrl As New ModuleController
            SetCopyModuleMessage(GetIsPortable(moduleCtrl, ModuleLst.SelectedValue, PageLst.SelectedValue))

        End Sub

#End Region

#Region "Properties"

        Public Property ToolName() As String Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool.ToolName
            Get
                Return "QuickAddModule"
            End Get
            Set(ByVal value As String)
                Throw New NotSupportedException("Set ToolName not supported")
            End Set
        End Property

        Public Overrides Property Visible() As Boolean
            Get
                Return MyBase.Visible = True AndAlso TabPermissionController.CanAddContentToPage
            End Get
            Set(ByVal value As Boolean)
                MyBase.Visible = value
            End Set
        End Property

        Private _Enabled As Boolean = True
        Public Property Enabled() As Boolean
            Get
                Return _Enabled AndAlso CanAddModuleToPage()
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Sub SetCopyModuleMessage(ByVal isPortable As Boolean)
            If (isPortable) Then
                chkCopyModule.Text = Localization.GetString("CopyModuleWcontent", Me.LocalResourceFile)
                chkCopyModule.ToolTip = Localization.GetString("CopyModuleWcontent.ToolTip", Me.LocalResourceFile)
            Else
                chkCopyModule.Text = Localization.GetString("CopyModuleWOcontent", Me.LocalResourceFile)
                chkCopyModule.ToolTip = Localization.GetString("CopyModuleWOcontent.ToolTip", Me.LocalResourceFile)
            End If
        End Sub

        Private Sub LoadAllLists()
            LoadPageList()
            LoadModuleList()
            LoadVisibilityList()
            LoadPaneList()
            LoadPositionList()
            LoadPaneModulesList()
        End Sub

        Private Sub LoadPageList()
            PageListTR.Visible = AddExistingModule.Checked
            TitleTR.Visible = Not AddExistingModule.Checked
            chkCopyModule.Visible = AddExistingModule.Checked

            If (AddExistingModule.Checked) Then
                chkCopyModule.Text = Localization.GetString("CopyModuleDefault.Text", Me.LocalResourceFile)
            End If


            PageLst.Items.Clear()
            If (PageListTR.Visible) Then
                PageLst.DataValueField = "TabID"
                PageLst.DataTextField = "IndentedTabName"
                PageLst.DataSource = TabController.GetPortalTabs(PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, True, "", True, False, False, False, True)
                PageLst.DataBind()
            End If
        End Sub

        Private Sub LoadModuleList()
            If (AddExistingModule.Checked) Then
                'Get list of modules for the selected tab
                ModuleLst.Items.Clear()
                If (Not String.IsNullOrEmpty(PageLst.SelectedValue)) Then
                    Dim moduleCtrl As New ModuleController
                    Dim pageModules As New ArrayList

                    Dim portalModules As Dictionary(Of Integer, ModuleInfo) = moduleCtrl.GetTabModules(Integer.Parse(PageLst.SelectedValue))
                    For Each m As ModuleInfo In portalModules.Values
                        If ModulePermissionController.CanAdminModule(m) = True And m.IsDeleted = False Then
                            pageModules.Add(m)
                        End If
                    Next
                    ModuleLst.DataValueField = "ModuleID"
                    ModuleLst.DataTextField = "ModuleTitle"
                    ModuleLst.DataSource = pageModules
                    ModuleLst.DataBind()

                    If (ModuleLst.Items.Count > 0) Then
                        chkCopyModule.Visible = True
                        SetCopyModuleMessage(GetIsPortable(moduleCtrl, ModuleLst.Items(0).Value, PageLst.SelectedValue))
                    End If

                End If
            Else
                ModuleLst.Items.Clear()
                ModuleLst.DataValueField = "DesktopModuleID"
                ModuleLst.DataTextField = "FriendlyName"
                ModuleLst.DataSource = DesktopModuleController.GetPortalDesktopModules(PortalSettings.Current.PortalId).Values
                ModuleLst.DataBind()

                'Select default module
                Dim defaultModuleID As Integer = -1
                Dim defaultModuleName As String = Localization.GetString("DefaultModule", LocalResourceFile, PortalSettings.Current, Nothing, True)
                If (Not String.IsNullOrEmpty(defaultModuleName)) Then
                    Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(defaultModuleName, PortalSettings.Current.PortalId)
                    If (Not IsNothing(desktopModule) AndAlso Not IsNothing(ModuleLst.Items.FindItemByValue(desktopModule.DesktopModuleID))) Then
                        ModuleLst.SelectedValue = desktopModule.DesktopModuleID.ToString()
                    End If
                End If
            End If

            ModuleLst.Enabled = ModuleLst.Items.Count > 0
        End Sub

        Private Function GetIsPortable(ByVal moduleCtrl As ModuleController, ByVal ModuleID As String, ByVal TabID As String) As Boolean

            Dim IsPortable As Boolean = False
            Dim parsedModuleID As Integer = 0
            Dim parsedTabID As Integer = 0

            Dim validModuleID As Boolean = Integer.TryParse(ModuleID, parsedModuleID)
            Dim validTabID As Boolean = Integer.TryParse(TabID, parsedTabID)

            If (validModuleID AndAlso validTabID) Then
                Dim moduleInfo As ModuleInfo = moduleCtrl.GetModule(ModuleID, TabID)
                If (Not moduleInfo Is Nothing) Then
                    Dim moduleDesktopInfo As DesktopModuleInfo = moduleInfo.DesktopModule
                    If (Not moduleDesktopInfo Is Nothing) Then
                        IsPortable = moduleDesktopInfo.IsPortable
                    End If
                End If
            End If

            Return IsPortable

        End Function

        Private Sub LoadVisibilityList()
            VisibilityLst.Enabled = Not AddExistingModule.Checked
            If (VisibilityLst.Enabled) Then
                Dim items As New Dictionary(Of String, String)
                items.Add("0", GetString("PermissionView"))
                items.Add("1", GetString("PermissionEdit"))

                VisibilityLst.Items.Clear()
                VisibilityLst.DataValueField = "key"
                VisibilityLst.DataTextField = "value"
                VisibilityLst.DataSource = items
                VisibilityLst.DataBind()
            End If
        End Sub

        Private Sub LoadPaneList()
            PaneLst.Items.Clear()
            PaneLst.DataSource = PortalSettings.Current.ActiveTab.Panes
            PaneLst.DataBind()
            If (PortalSettings.Current.ActiveTab.Panes.Contains(glbDefaultPane)) Then
                PaneLst.SelectedValue = glbDefaultPane
            End If
        End Sub

        Private Sub LoadPositionList()
            Dim items As New Dictionary(Of String, String)
            items.Add("TOP", GetString("Top"))
            items.Add("ABOVE", GetString("Above"))
            items.Add("BELOW", GetString("Below"))
            items.Add("BOTTOM", GetString("Bottom"))

            PositionLst.Items.Clear()
            PositionLst.DataValueField = "key"
            PositionLst.DataTextField = "value"
            PositionLst.DataSource = items
            PositionLst.DataBind()
            PositionLst.SelectedValue = "BOTTOM"
        End Sub

        Private Sub LoadPaneModulesList()
            Dim items As New Dictionary(Of String, String)
            items.Add("", "")

            For Each m As ModuleInfo In PortalSettings.Current.ActiveTab.Modules
                'if user is allowed to view module and module is not deleted
                If ModulePermissionController.CanViewModule(m) = True And m.IsDeleted = False Then
                    'modules which are displayed on all tabs should not be displayed on the Admin or Super tabs
                    If m.AllTabs = False Or PortalSettings.Current.ActiveTab.IsSuperTab = False Then
                        If m.PaneName = PaneLst.SelectedValue Then

                            Dim moduleOrder As Integer = m.ModuleOrder

                            While (items.ContainsKey(moduleOrder.ToString()) Or moduleOrder = 0)
                                moduleOrder += 1
                            End While

                            items.Add(moduleOrder.ToString(), m.ModuleTitle)

                        End If
                    End If
                End If
            Next

            PaneModulesLst.Enabled = True
            PaneModulesLst.Items.Clear()
            PaneModulesLst.DataValueField = "key"
            PaneModulesLst.DataTextField = "value"
            PaneModulesLst.DataSource = items
            PaneModulesLst.DataBind()

            If (PaneModulesLst.Items.Count <= 1) Then
                Dim listItem As Telerik.Web.UI.RadComboBoxItem = PositionLst.Items.FindItemByValue("ABOVE")
                If (Not IsNothing(listItem)) Then
                    PositionLst.Items.Remove(listItem)
                End If
                listItem = PositionLst.Items.FindItemByValue("BELOW")
                If (Not IsNothing(listItem)) Then
                    PositionLst.Items.Remove(listItem)
                End If
                PaneModulesLst.Enabled = False
            End If

            If (PositionLst.SelectedValue = "TOP" Or PositionLst.SelectedValue = "BOTTOM") Then
                PaneModulesLst.Enabled = False
            End If
        End Sub

        Private ReadOnly Property LocalResourceFile() As String
            Get
                Return String.Format("{0}/{1}/{2}.ascx.resx", Me.TemplateSourceDirectory, Localization.LocalResourceDirectory, Me.GetType().BaseType().Name)
            End Get
        End Property

        Private Function GetString(ByVal key As String) As String
            Return Localization.GetString(key, LocalResourceFile)
        End Function

        Public Function CanAddModuleToPage() As Boolean
            If HttpContext.Current Is Nothing Then
                Return False
            End If
            'If we are not in an edit page
            Return (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("mid"))) AndAlso (String.IsNullOrEmpty(HttpContext.Current.Request.QueryString("ctl")))
        End Function

        'todo: Move to business layer
        'todo: Return message when we try to add an existing module that has already been added to the page (we can't add the module twice)
        Private Sub DoAddExistingModule(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal paneName As String, ByVal position As Integer, ByVal align As String)
            Dim moduleCtrl As New ModuleController
            Dim moduleInfo As ModuleInfo = moduleCtrl.GetModule(moduleId, tabId, False)

            If Not moduleInfo Is Nothing Then
                ' clone the module object ( to avoid creating an object reference to the data cache )
                Dim newModule As ModuleInfo = moduleInfo.Clone()

                newModule.TabID = PortalSettings.Current.ActiveTab.TabID
                newModule.ModuleOrder = position
                newModule.PaneName = paneName
                newModule.Alignment = align
                moduleCtrl.AddModule(newModule)

                'Add Event Log
                Dim userID As Integer = -1
                If Request.IsAuthenticated Then
                    Dim user As UserInfo = UserController.GetCurrentUserInfo
                    If (Not IsNothing(user)) Then
                        userID = user.UserID
                    End If
                End If

                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(newModule, PortalSettings.Current, userID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED)
            End If
        End Sub

        'this has an overload of whether or not to create a cloned module
        Private Sub DoAddExistingModule(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal paneName As String, ByVal position As Integer, ByVal align As String, ByVal cloneModule As Boolean)
            Dim moduleCtrl As New ModuleController
            Dim moduleInfo As ModuleInfo = moduleCtrl.GetModule(moduleId, tabId, False)

            Dim userID As Integer = -1
            If Request.IsAuthenticated Then
                Dim user As UserInfo = UserController.GetCurrentUserInfo
                If (Not IsNothing(user)) Then
                    userID = user.UserID
                End If
            End If


            If Not moduleInfo Is Nothing Then
                ' clone the module object ( to avoid creating an object reference to the data cache )
                Dim newModule As ModuleInfo = moduleInfo.Clone()

                newModule.UniqueId = GUID.NewGuid() ' Cloned Module requires a different uniqueID

                newModule.TabID = PortalSettings.Current.ActiveTab.TabID
                newModule.ModuleOrder = position
                newModule.PaneName = paneName
                newModule.Alignment = align

                If (cloneModule) Then
                    newModule.ModuleID = Null.NullInteger 'reset the module id
                    newModule.ModuleID = moduleCtrl.AddModule(newModule)

                    If newModule.DesktopModule.BusinessControllerClass <> "" Then
                        Dim objObject As Object = Framework.Reflection.CreateObject(newModule.DesktopModule.BusinessControllerClass, newModule.DesktopModule.BusinessControllerClass)
                        If TypeOf objObject Is IPortable Then
                            Dim Content As String = CType(CType(objObject, IPortable).ExportModule(moduleId), String)
                            If Content <> "" Then
                                CType(objObject, IPortable).ImportModule(newModule.ModuleID, Content, newModule.DesktopModule.Version, userID)
                            End If
                        End If
                    End If

                Else
                    moduleCtrl.AddModule(newModule)
                End If

                'Add Event Log
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(newModule, PortalSettings.Current, userID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED)


            End If
        End Sub

        Private Sub DoAddNewModule(ByVal title As String, ByVal desktopModuleId As Integer, ByVal paneName As String, ByVal position As Integer, ByVal permissionType As Integer, ByVal align As String)

            Dim objTabPermissions As TabPermissionCollection = PortalSettings.Current.ActiveTab.TabPermissions
            Dim objPermissionController As New PermissionController
            Dim objModules As New ModuleController
            Dim objModuleDefinition As ModuleDefinitionInfo
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim j As Integer

            Try
                Dim desktopModule As DesktopModuleInfo = Nothing
                If Not DesktopModuleController.GetDesktopModules(PortalSettings.Current.PortalId).TryGetValue(desktopModuleId, desktopModule) Then
                    Throw New ArgumentException("desktopModuleId")
                End If
            Catch ex As Exception
                LogException(ex)
            End Try

            Dim UserId As Integer = -1
            If Request.IsAuthenticated Then
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                UserId = objUserInfo.UserID
            End If

            For Each objModuleDefinition In ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(desktopModuleId).Values
                Dim objModule As New ModuleInfo
                objModule.Initialize(PortalSettings.Current.PortalId)

                objModule.PortalID = PortalSettings.Current.PortalId
                objModule.TabID = PortalSettings.Current.ActiveTab.TabID
                objModule.ModuleOrder = position
                If title = "" Then
                    objModule.ModuleTitle = objModuleDefinition.FriendlyName
                Else
                    objModule.ModuleTitle = title
                End If
                objModule.PaneName = paneName
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID
                If objModuleDefinition.DefaultCacheTime > 0 Then
                    objModule.CacheTime = objModuleDefinition.DefaultCacheTime
                    If PortalSettings.Current.DefaultModuleId > Null.NullInteger AndAlso PortalSettings.Current.DefaultTabId > Null.NullInteger Then
                        Dim defaultModule As ModuleInfo = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, True)
                        If Not defaultModule Is Nothing Then
                            objModule.CacheTime = defaultModule.CacheTime
                        End If
                    End If
                End If

                Select Case permissionType
                    Case 0
                        objModule.InheritViewPermissions = True
                    Case 1
                        objModule.InheritViewPermissions = False
                End Select

                ' get the default module view permissions
                Dim arrSystemModuleViewPermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW")

                ' get the permissions from the page
                For Each objTabPermission As TabPermissionInfo In objTabPermissions
                    If objTabPermission.PermissionKey = "VIEW" AndAlso permissionType = 0 Then
                        'Don't need to explicitly add View permisisons if "Same As Page"
                        Continue For
                    End If

                    ' get the system module permissions for the permissionkey
                    Dim arrSystemModulePermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey)
                    ' loop through the system module permissions
                    For j = 0 To arrSystemModulePermissions.Count - 1
                        ' create the module permission
                        Dim objSystemModulePermission As PermissionInfo
                        objSystemModulePermission = CType(arrSystemModulePermissions(j), PermissionInfo)
                        If objSystemModulePermission.PermissionKey = "VIEW" AndAlso permissionType = 1 AndAlso _
                          objTabPermission.PermissionKey <> "EDIT" Then
                            'Only Page Editors get View permissions if "Page Editors Only"
                            Continue For
                        End If

                        Dim objModulePermission As ModulePermissionInfo = AddModulePermission(objModule, _
                          objSystemModulePermission, _
                          objTabPermission.RoleID, objTabPermission.UserID, _
                          objTabPermission.AllowAccess)

                        ' ensure that every EDIT permission which allows access also provides VIEW permission
                        If objModulePermission.PermissionKey = "EDIT" And objModulePermission.AllowAccess Then
                            Dim objModuleViewperm As ModulePermissionInfo = AddModulePermission(objModule, _
                             CType(arrSystemModuleViewPermissions(0), PermissionInfo), _
                             objModulePermission.RoleID, objModulePermission.UserID, _
                             True)
                        End If
                    Next

                    'Get the custom Module Permissions,  Assume that roles with Edit Tab Permissions
                    'are automatically assigned to the Custom Module Permissions
                    If objTabPermission.PermissionKey = "EDIT" Then
                        Dim arrCustomModulePermissions As ArrayList = objPermissionController.GetPermissionsByModuleDefID(objModule.ModuleDefID)

                        ' loop through the custom module permissions
                        For j = 0 To arrCustomModulePermissions.Count - 1
                            ' create the module permission
                            Dim objCustomModulePermission As PermissionInfo
                            objCustomModulePermission = CType(arrCustomModulePermissions(j), PermissionInfo)

                            AddModulePermission(objModule, objCustomModulePermission, _
                             objTabPermission.RoleID, objTabPermission.UserID, _
                             objTabPermission.AllowAccess)
                        Next
                    End If
                Next

                If PortalSettings.Current.ContentLocalizationEnabled Then
                    Dim defaultLocale As Locale = LocaleController.Instance.GetDefaultLocale(PortalSettings.Current.PortalId)
                    objModule.CultureCode = defaultLocale.Code
                Else
                    objModule.CultureCode = Null.NullString
                End If

                objModule.AllTabs = False
                objModule.Alignment = align

                objModules.AddModule(objModule)
            Next

        End Sub

        Private Function AddModulePermission(ByVal objModule As ModuleInfo, ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal userId As Integer, ByVal allowAccess As Boolean) As ModulePermissionInfo
            Dim objModulePermission As New ModulePermissionInfo
            objModulePermission.ModuleID = objModule.ModuleID
            objModulePermission.PermissionID = permission.PermissionID
            objModulePermission.RoleID = roleId
            objModulePermission.UserID = userId
            objModulePermission.PermissionKey = permission.PermissionKey
            objModulePermission.AllowAccess = allowAccess

            ' add the permission to the collection
            If Not objModule.ModulePermissions.Contains(objModulePermission) Then
                objModule.ModulePermissions.Add(objModulePermission)
            End If

            Return objModulePermission
        End Function

#End Region

    End Class

End Namespace


