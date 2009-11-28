'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.ControlPanels

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The IconBar ControlPanel provides an icon bar based Page/Module manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class IconBar
        Inherits ControlPanelBase

#Region "Private Methods"

        Private Sub BindData()
            Select Case optModuleType.SelectedItem.Value
                Case "0" ' new module
                    cboTabs.Visible = False
                    cboModules.Visible = False
                    cboDesktopModules.Visible = True
                    txtTitle.Visible = True
                    lblModule.Text = Localization.GetString("Module", LocalResourceFile)
                    lblTitle.Text = Localization.GetString("Title", LocalResourceFile)
                    cboPermission.Enabled = True

                    ' get list of modules
                    cboDesktopModules.DataSource = DesktopModuleController.GetPortalDesktopModules(PortalSettings.PortalId).Values
                    cboDesktopModules.DataBind()
                    cboDesktopModules.Items.Insert(0, New ListItem("<" + Localization.GetString("SelectModule", LocalResourceFile) + ">", "-1"))

                    ' select default module
                    Dim intDesktopModuleID As Integer = -1
                    If Localization.GetString("DefaultModule", LocalResourceFile) <> "" Then
                        Dim objDesktopModule As DesktopModuleInfo
                        objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(Localization.GetString("DefaultModule", LocalResourceFile, PortalSettings, Nothing, True), PortalSettings.PortalId)
                        If Not objDesktopModule Is Nothing Then
                            intDesktopModuleID = objDesktopModule.DesktopModuleID
                        End If
                    End If
                    If intDesktopModuleID <> -1 AndAlso (Not cboDesktopModules.Items.FindByValue(intDesktopModuleID.ToString()) Is Nothing) Then
                        cboDesktopModules.Items.FindByValue(intDesktopModuleID.ToString()).Selected = True
                    Else
                        cboDesktopModules.SelectedIndex = 0
                    End If
                Case "1" ' existing module
                    cboTabs.Visible = True
                    cboModules.Visible = True
                    cboDesktopModules.Visible = False
                    txtTitle.Visible = False
                    lblModule.Text = Localization.GetString("Tab", LocalResourceFile)
                    lblTitle.Text = Localization.GetString("Module", LocalResourceFile)
                    cboPermission.Enabled = False

					cboTabs.DataSource = TabController.GetPortalTabs(PortalSettings.PortalId, PortalSettings.ActiveTab.TabID, True, "<" & Localization.GetString("SelectPage", LocalResourceFile) & ">", True, False, False, False, True)
                    cboTabs.DataBind()
            End Select

        End Sub

        Private Sub DisableAction(ByVal image As System.Web.UI.WebControls.Image, ByVal imageUrl As String, ByVal imageButton As LinkButton, ByVal button As LinkButton)
            image.ImageUrl = "~/Admin/ControlPanel/images/" + imageUrl
            imageButton.Enabled = False
            button.Enabled = False
        End Sub

        Private Sub Localize()
			lblMode.Text = Localization.GetString("Mode", LocalResourceFile)
			imgAdmin.AlternateText = Localization.GetString("imgAdmin.AlternateText", LocalResourceFile)
			cmdAdmin.Text = Localization.GetString("cmdAdmin", LocalResourceFile)
			imgHost.AlternateText = Localization.GetString("imgHost.AlternateText", LocalResourceFile)
			cmdHost.Text = Localization.GetString("cmdHost", LocalResourceFile)
            lblPageFunctions.Text = Localization.GetString("PageFunctions", LocalResourceFile)
            lblCommonTasks.Text = Localization.GetString("CommonTasks", LocalResourceFile)
            lblModule.Text = Localization.GetString("Module", LocalResourceFile)
            lblPane.Text = Localization.GetString("Pane", LocalResourceFile)
            lblTitle.Text = Localization.GetString("Title", LocalResourceFile)
            lblInstance.Text = Localization.GetString("Instance", LocalResourceFile)

            imgAddTabIcon.AlternateText = Localization.GetString("AddTab.AlternateText", LocalResourceFile)
			cmdAddTab.Text = Localization.GetString("AddTab", LocalResourceFile)

            imgEditTabIcon.AlternateText = Localization.GetString("EditTab.AlternateText", LocalResourceFile)
            cmdEditTab.Text = Localization.GetString("EditTab", LocalResourceFile)

            imgDeleteTabIcon.AlternateText = Localization.GetString("DeleteTab.AlternateText", LocalResourceFile)
            cmdDeleteTab.Text = Localization.GetString("DeleteTab", LocalResourceFile)

            imgCopyTabIcon.AlternateText = Localization.GetString("CopyTab.AlternateText", LocalResourceFile)
            cmdCopyTab.Text = Localization.GetString("CopyTab", LocalResourceFile)

            imgExportTabIcon.AlternateText = Localization.GetString("ExportTab.AlternateText", LocalResourceFile)
            cmdExportTab.Text = Localization.GetString("ExportTab", LocalResourceFile)

            imgImportTabIcon.AlternateText = Localization.GetString("ImportTab.AlternateText", LocalResourceFile)
            cmdImportTab.Text = Localization.GetString("ImportTab", LocalResourceFile)

			imgAddModule.AlternateText = Localization.GetString("AddModule.AlternateText", LocalResourceFile)
            cmdAddModule.Text = Localization.GetString("AddModule", LocalResourceFile)

            imgSiteIcon.AlternateText = Localization.GetString("Site.AlternateText", LocalResourceFile)
            cmdSite.Text = Localization.GetString("Site", LocalResourceFile)

            imgUsersIcon.AlternateText = Localization.GetString("Users.AlternateText", LocalResourceFile)
            cmdUsers.Text = Localization.GetString("Users", LocalResourceFile)

            imgRolesIcon.AlternateText = Localization.GetString("Roles.AlternateText", LocalResourceFile)
            cmdRoles.Text = Localization.GetString("Roles", LocalResourceFile)

            imgFilesIcon.AlternateText = Localization.GetString("Files.AlternateText", LocalResourceFile)
            cmdFiles.Text = Localization.GetString("Files", LocalResourceFile)

            imgHelpIcon.AlternateText = Localization.GetString("Help.AlternateText", LocalResourceFile)
            cmdHelp.Text = Localization.GetString("Help", LocalResourceFile)

			imgExtensionsIcon.AlternateText = Localization.GetString("Extensions.AlternateText", LocalResourceFile)
			cmdExtensions.Text = Localization.GetString("Extensions", LocalResourceFile)
        End Sub

        Private Sub SetMode(ByVal Update As Boolean)
            If Update Then
                SetUserMode(optMode.SelectedValue)
            End If

            If Not TabPermissionController.CanAddContentToPage() Then
                optMode.Items.Remove(optMode.Items.FindByValue("LAYOUT"))
            End If

            Select Case UserMode
                Case Mode.View
                    optMode.Items.FindByValue("VIEW").Selected = True
                Case Mode.Edit
                    optMode.Items.FindByValue("EDIT").Selected = True
                Case Mode.Layout
                    optMode.Items.FindByValue("LAYOUT").Selected = True
            End Select
        End Sub

        Private Sub SetVisibility(ByVal Toggle As Boolean)
            If Toggle Then
                SetVisibleMode(Not IsVisible)
            End If
        End Sub

        Private Sub LoadPositions()
            LoadInstances()
            cboPosition.Items.Clear()
            If cboInstances.Items.Count > 1 Then
                cboPosition.Items.Add(New ListItem(Localization.GetString("Top", LocalResourceFile),"TOP"))
                cboPosition.Items.Add(New ListItem(Localization.GetString("Above", LocalResourceFile),"ABOVE"))
                cboPosition.Items.Add(New ListItem(Localization.GetString("Below", LocalResourceFile),"BELOW"))
            End If
            cboPosition.Items.Add(New ListItem(Localization.GetString("Bottom", LocalResourceFile),"BOTTOM"))
            cboPosition.SelectedIndex = cboPosition.Items.Count - 1
            DisplayInstances()
        End Sub

        Private Sub DisplayInstances()
            If Not cboPosition.SelectedItem Is Nothing Then
                Select Case cboPosition.SelectedItem.Value
                    Case "TOP", "BOTTOM"
                        cboInstances.Visible = False
                    Case "ABOVE", "BELOW"
                        cboInstances.Visible = True
                End Select
            End If
            lblInstance.Visible = cboInstances.Visible
        End Sub

        Private Sub LoadInstances()
            cboInstances.Items.Clear()
            Dim objModule As ModuleInfo = Nothing
            For Each objModule In PortalSettings.ActiveTab.Modules
                ' if user is allowed to view module and module is not deleted
				If ModulePermissionController.CanViewModule(objModule) = True And objModule.IsDeleted = False Then
					' modules which are displayed on all tabs should not be displayed on the Admin or Super tabs
					If objModule.AllTabs = False Or PortalSettings.ActiveTab.IsSuperTab = False Then
						If objModule.PaneName = cboPanes.SelectedItem.Value Then
							cboInstances.Items.Add(New ListItem(objModule.ModuleTitle, objModule.ModuleOrder.ToString()))
						End If
					End If
				End If
            Next
            cboInstances.Items.Insert(0,New ListItem("",""))
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Me.ID = "IconBar.ascx"
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If IsPageAdmin() Then
                    tblControlPanel.Visible = True
                    cmdVisibility.Visible = True
                    rowControlPanel.Visible = True

                    Localize()

                    If IsAdminControl() Then
                        cmdAddModule.Enabled = False
                    End If

                    If Not Page.IsPostBack Then

                        optModuleType.Items.FindByValue("0").Selected = True

                        If Not TabPermissionController.CanAddPage Then
                            DisableAction(imgAddTabIcon, "iconbar_addtab_bw.gif", cmdAddTabIcon, cmdAddTab)
                        End If

                        If Not TabPermissionController.CanManagePage Then
                            DisableAction(imgEditTabIcon, "iconbar_edittab_bw.gif", cmdEditTabIcon, cmdEditTab)
                        End If

                        If Not TabPermissionController.CanDeletePage OrElse TabController.IsSpecialTab(TabController.CurrentPage.TabID, PortalSettings) Then
                            DisableAction(imgDeleteTabIcon, "iconbar_deletetab_bw.gif", cmdDeleteTabIcon, cmdDeleteTab)
                        Else
                            ClientAPI.AddButtonConfirm(cmdDeleteTab, Localization.GetString("DeleteTabConfirm", LocalResourceFile))
                            ClientAPI.AddButtonConfirm(cmdDeleteTabIcon, Localization.GetString("DeleteTabConfirm", LocalResourceFile))
                        End If

                        If Not TabPermissionController.CanCopyPage Then
                            DisableAction(imgCopyTabIcon, "iconbar_copytab_bw.gif", cmdCopyTabIcon, cmdCopyTab)
                        End If

                        If Not TabPermissionController.CanExportPage Then
                            DisableAction(imgExportTabIcon, "iconbar_exporttab_bw.gif", cmdExportTabIcon, cmdExportTab)
                        End If

                        If Not TabPermissionController.CanImportPage Then
                            DisableAction(imgImportTabIcon, "iconbar_importtab_bw.gif", cmdImportTabIcon, cmdImportTab)
                        End If

                        If Not TabPermissionController.CanAddContentToPage Then
                            pnlModules.Visible = False
                        End If

                        If Not GetModulePermission(PortalSettings.PortalId, "Site Settings") Then
                            DisableAction(imgSiteIcon, "iconbar_site_bw.gif", cmdSiteIcon, cmdSite)
                        End If
                        If GetModulePermission(PortalSettings.PortalId, "User Accounts") = False Then
                            DisableAction(imgUsersIcon, "iconbar_users_bw.gif", cmdUsersIcon, cmdUsers)
                        End If
                        If GetModulePermission(PortalSettings.PortalId, "Security Roles") = False Then
                            DisableAction(imgRolesIcon, "iconbar_roles_bw.gif", cmdRolesIcon, cmdRoles)
                        End If
                        If GetModulePermission(PortalSettings.PortalId, "File Manager") = False Then
                            DisableAction(imgFilesIcon, "iconbar_files_bw.gif", cmdFilesIcon, cmdFiles)
                        End If
						If GetModulePermission(PortalSettings.PortalId, "Extensions") = False Then
							DisableAction(imgExtensionsIcon, "iconbar_extensions_bw.gif", cmdExtensionsIcon, cmdExtensions)
						End If

                        Dim objUser As UserInfo = UserController.GetCurrentUserInfo
						If Not objUser Is Nothing Then
							If objUser.IsSuperUser Then
								hypMessage.ImageUrl = Upgrade.Upgrade.UpgradeIndicator(DotNetNukeContext.Current.Application.Version, Request.IsLocal, Request.IsSecureConnection)
								If hypMessage.ImageUrl <> "" Then
									hypMessage.ToolTip = Localization.GetString("hypUpgrade.Text", LocalResourceFile)
									hypMessage.NavigateUrl = Upgrade.Upgrade.UpgradeRedirect()
								End If
								cmdHost.Visible = True
							Else ' branding
                                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) AndAlso Host.DisplayCopyright Then
                                    hypMessage.ImageUrl = Localization.GetString("hypMessageImage.Text", LocalResourceFile)
                                    hypMessage.ToolTip = Localization.GetString("hypMessageTooltip.Text", LocalResourceFile)
                                    hypMessage.NavigateUrl = Localization.GetString("hypMessageUrl.Text", LocalResourceFile)
                                Else
                                    hypMessage.Visible = False
                                End If
								cmdHost.Visible = False
								cmdAdmin.Visible = GetModulePermission(PortalSettings.PortalId, "Console")
							End If
							imgHost.Visible = cmdHost.Visible
							imgAdmin.Visible = cmdAdmin.Visible
						End If

                        BindData()

                        Dim intItem As Integer
                        For intItem = 0 To PortalSettings.ActiveTab.Panes.Count - 1
                            cboPanes.Items.Add(Convert.ToString(PortalSettings.ActiveTab.Panes(intItem)))
                        Next intItem
                        If Not cboPanes.Items.FindByValue(glbDefaultPane) Is Nothing Then
                            cboPanes.Items.FindByValue(glbDefaultPane).Selected = True
                        End If

                        If cboPermission.Items.Count > 0 Then
                            cboPermission.SelectedIndex = 0 ' view
                        End If

                        LoadPositions()

                        If Not String.IsNullOrEmpty(Host.HelpURL) Then
                            cmdHelp.NavigateUrl = FormatHelpUrl(Host.HelpURL, PortalSettings, "")
                            cmdHelpIcon.NavigateUrl = cmdHelp.NavigateUrl
                            cmdHelp.Enabled = True
                            cmdHelpIcon.Enabled = True
                        Else
                            cmdHelp.Enabled = False
                            cmdHelpIcon.Enabled = False
                        End If

                        SetMode(False)
                        SetVisibility(False)
                    End If

                    'Register jQuery
                    jQuery.RequestRegistration()
                ElseIf IsModuleAdmin() Then
                    tblControlPanel.Visible = True
                    cmdVisibility.Visible = False
                    rowControlPanel.Visible = False
                    If Not Page.IsPostBack Then
                        SetMode(False)
                        SetVisibility(False)
                    End If
                Else
                    tblControlPanel.Visible = False
                End If


            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'Set initial value
            DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(imgVisibility, rowControlPanel, PortalSettings.DefaultControlPanelVisibility, Common.Globals.ApplicationPath & "/images/collapse.gif", _
                Common.Globals.ApplicationPath & "/images/expand.gif", DNNClientAPI.MinMaxPersistanceType.Personalization, "Usability", "ControlPanelVisible" & Me.PortalSettings.PortalId.ToString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' PageFunctions_Click runs when any button in the Page toolbar is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageFunctions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddTab.Click, cmdAddTabIcon.Click, cmdEditTab.Click, cmdEditTabIcon.Click, cmdDeleteTab.Click, cmdDeleteTabIcon.Click, cmdCopyTab.Click, cmdCopyTabIcon.Click, cmdExportTab.Click, cmdExportTabIcon.Click, cmdImportTab.Click, cmdImportTabIcon.Click
            Try
                Dim URL As String = Request.RawUrl
                Select Case CType(sender, LinkButton).ID
                    Case "cmdAddTab", "cmdAddTabIcon"
                        URL = NavigateURL("Tab")
                    Case "cmdEditTab", "cmdEditTabIcon"
                        URL = NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit")
                    Case "cmdDeleteTab", "cmdDeleteTabIcon"
                        URL = NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete")
                    Case "cmdCopyTab", "cmdCopyTabIcon"
                        URL = NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=copy")
                    Case "cmdExportTab", "cmdExportTabIcon"
                        URL = NavigateURL(PortalSettings.ActiveTab.TabID, "ExportTab")
                    Case "cmdImportTab", "cmdImportTabIcon"
                        URL = NavigateURL(PortalSettings.ActiveTab.TabID, "ImportTab")
                End Select
                Response.Redirect(URL, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CommonTasks_Click runs when any button in the Common Tasks toolbar is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
		Private Sub CommonTasks_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSite.Click, cmdSiteIcon.Click, cmdUsers.Click, cmdUsersIcon.Click, cmdRoles.Click, cmdRolesIcon.Click, cmdFiles.Click, cmdFilesIcon.Click, cmdExtensions.Click, cmdExtensionsIcon.Click
			Try
				Dim URL As String = Request.RawUrl
				Select Case CType(sender, LinkButton).ID
					Case "cmdSite", "cmdSiteIcon"
						URL = BuildURL(PortalSettings.PortalId, "Site Settings")
					Case "cmdUsers", "cmdUsersIcon"
						URL = BuildURL(PortalSettings.PortalId, "User Accounts")
					Case "cmdRoles", "cmdRolesIcon"
						URL = BuildURL(PortalSettings.PortalId, "Security Roles")
					Case "cmdFiles", "cmdFilesIcon"
						URL = BuildURL(PortalSettings.PortalId, "File Manager")
					Case "cmdExtensions", "cmdExtensionsIcon"
						URL = BuildURL(PortalSettings.PortalId, "Extensions")
				End Select

				Response.Redirect(URL, True)
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub imgAddModule_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAddModule.Click
			AddModule_Click(sender, e)
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' AddModule_Click runs when the Add Module Icon or text button is clicked
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	10/06/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		'''     [vmasanas]  01/07/2005  Modified to add view perm. to all roles with edit perm.
		''' </history>
		''' -----------------------------------------------------------------------------
		Protected Sub AddModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddModule.Click
			Try
                If TabPermissionController.CanAddContentToPage() Then
                    Dim title As String = txtTitle.Text

                    Dim permissionType As ViewPermissionType = ViewPermissionType.View
                    If Not cboPermission.SelectedItem Is Nothing Then
                        permissionType = CType(cboPermission.SelectedItem.Value, ViewPermissionType)
                    End If

                    Dim position As Integer = -1
                    If Not cboPosition.SelectedItem Is Nothing Then
                        Select Case cboPosition.SelectedItem.Value
                            Case "TOP"
                                position = 0
                            Case "ABOVE"
                                If Not String.IsNullOrEmpty(cboInstances.SelectedValue) Then
                                    position = Integer.Parse(cboInstances.SelectedItem.Value) - 1
                                Else
                                    position = 0
                                End If
                            Case "BELOW"
                                If Not String.IsNullOrEmpty(cboInstances.SelectedValue) Then
                                    position = Integer.Parse(cboInstances.SelectedItem.Value) + 1
                                Else
                                    position = -1
                                End If
                            Case "BOTTOM"
                                position = -1
                        End Select
                    End If

                    Select Case optModuleType.SelectedItem.Value
                        Case "0" ' new module
                            If cboDesktopModules.SelectedIndex > 0 Then
                                AddNewModule(title, Integer.Parse(cboDesktopModules.SelectedItem.Value), cboPanes.SelectedItem.Text, position, permissionType, "")

                                ' Redirect to the same page to pick up changes
                                Response.Redirect(Request.RawUrl, True)
                            End If
                        Case "1" ' existing module
                            If Not cboModules.SelectedItem Is Nothing Then
                                AddExistingModule(Integer.Parse(cboModules.SelectedItem.Value), Integer.Parse(cboTabs.SelectedItem.Value), cboPanes.SelectedItem.Text, position, "")

                                ' Redirect to the same page to pick up changes
                                Response.Redirect(Request.RawUrl, True)
                            End If
                    End Select
                End If

			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Private Sub optModuleType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optModuleType.SelectedIndexChanged
			BindData()
		End Sub

		Private Sub cboTabs_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTabs.SelectedIndexChanged
			Dim objModules As New ModuleController
			Dim arrModules As New ArrayList

			Dim objModule As ModuleInfo
			Dim arrPortalModules As Dictionary(Of Integer, ModuleInfo) = objModules.GetTabModules(Integer.Parse(cboTabs.SelectedItem.Value))
			For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In arrPortalModules
				objModule = kvp.Value
				If ModulePermissionController.CanAdminModule(objModule) = True And objModule.IsDeleted = False Then
					arrModules.Add(objModule)
				End If
			Next

			lblModule.Text = Localization.GetString("Tab", LocalResourceFile)
			lblTitle.Text = Localization.GetString("Module", LocalResourceFile)

			cboModules.DataSource = arrModules
			cboModules.DataBind()
		End Sub

		Protected Sub cmdVisibility_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdVisibility.Click
			SetVisibility(True)
			Response.Redirect(Request.RawUrl, True)
		End Sub

		Protected Sub optMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optMode.SelectedIndexChanged
			If Not Page.IsCallback Then
				SetMode(True)
				Response.Redirect(Request.RawUrl, True)
			End If
		End Sub

		Private Sub cboPanes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPanes.SelectedIndexChanged
            LoadPositions()
            Dim script As String = String.Format(glbScriptFormat, ResolveUrl("~/Resources/ControlPanel/ControlPanel.js"))
            ClientAPI.RegisterStartUpScript(Page, "controlPanel", script)
		End Sub

        Protected Sub cboPosition_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPosition.SelectedIndexChanged
            DisplayInstances()
        End Sub

		Protected Sub imgAdmin_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgAdmin.Click
			cmdAdmin_Click(sender, e)
		End Sub

		Private Sub cmdAdmin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAdmin.Click
			Try
				Response.Redirect(NavigateURL(PortalSettings.AdminTabId), True)
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub imgHost_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgHost.Click
			cmdHost_Click(sender, e)
		End Sub

		Private Sub cmdHost_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdHost.Click
			Try
                Response.Redirect(NavigateURL(PortalSettings.SuperTabId, True), True)
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

#End Region

    End Class

End Namespace
