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
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.ControlPanels

	Partial Class RibbonBar
		Inherits ControlPanelBase

#Region "Private Methods"

		Private Sub Localize()
			RibbonBarTabs.Tabs(0).Text = Localization.GetString("Tab_CommonTasks", LocalResourceFile)
			RibbonBarTabs.Tabs(1).Text = Localization.GetString("Tab_CurrentPage", LocalResourceFile)
			RibbonBarTabs.Tabs(2).Text = Localization.GetString("Tab_Site", LocalResourceFile)
			RibbonBarTabs.Tabs(3).Text = Localization.GetString("Tab_Host", LocalResourceFile)

			cmdAdmin.Text = Localization.GetString("AdminConsole", LocalResourceFile)
			cmdAdmin.ToolTip = Localization.GetString("AdminConsole.ToolTip", LocalResourceFile)
			cmdHost.Text = Localization.GetString("HostConsole", LocalResourceFile)
			cmdHost.ToolTip = Localization.GetString("HostConsole.ToolTip", LocalResourceFile)

			Dim ctrl As Control = G101.FindControl("SiteNewPage")
			If (Not IsNothing(ctrl) AndAlso TypeOf ctrl Is DotNetNuke.Web.UI.WebControls.DnnRibbonBarTool) Then
				Dim toolCtrl As DotNetNuke.Web.UI.WebControls.DnnRibbonBarTool = DirectCast(ctrl, DotNetNuke.Web.UI.WebControls.DnnRibbonBarTool)
				toolCtrl.Text = Localization.GetString("SiteNewPage", LocalResourceFile)
				toolCtrl.ToolTip = Localization.GetString("SiteNewPage.ToolTip", LocalResourceFile)
			End If

			Dim lstItem As ListItem = optMode.Items.FindByValue("VIEW")
			If (Not IsNothing(lstItem)) Then
				lstItem.Text = Localization.GetString("ModeView", LocalResourceFile)
			End If
			lstItem = optMode.Items.FindByValue("EDIT")
			If (Not IsNothing(lstItem)) Then
				lstItem.Text = Localization.GetString("ModeEdit", LocalResourceFile)
			End If
			lstItem = optMode.Items.FindByValue("LAYOUT")
			If (Not IsNothing(lstItem)) Then
				lstItem.Text = Localization.GetString("ModeLayout", LocalResourceFile)
			End If
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

#End Region

#Region "Event Handlers"

		Protected Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
			Me.ID = "RibbonBar.ascx"
		End Sub

		Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try

				RibbonBarTabs.Tabs(2).Visible = False
				Pages.PageViews(2).Visible = False
				RibbonBarTabs.Tabs(3).Visible = False
				Pages.PageViews(3).Visible = False
				cmdHost.Visible = False
				cmdAdmin.Visible = False

				'AddModule group
				'CommonTabAddModuleGroup.Visible = TabPermissionController.CanAddContentToPage()

				'AddPage groups
				'CommonTabAddPageGroup.Visible = TabPermissionController.CanAddPage()

				If (Request.IsAuthenticated) Then
					Dim user As UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()
					If (Not IsNothing(user)) Then
						RibbonBarTabs.Tabs(2).Visible = user.IsInRole(PortalSettings.Current.AdministratorRoleName)
						Pages.PageViews(2).Visible = user.IsInRole(PortalSettings.Current.AdministratorRoleName)
						RibbonBarTabs.Tabs(3).Visible = user.IsSuperUser
						Pages.PageViews(3).Visible = user.IsSuperUser
						cmdHost.Visible = user.IsSuperUser
						cmdAdmin.Visible = user.IsInRole(PortalSettings.Current.AdministratorRoleName)
					End If
				End If

				If IsPageAdmin() Then
                    DotNetNuke.Web.UI.Utilities.ApplySkin(Me.RibbonBarTabs, "", "", "RibbonBar")

                    RB.Visible = True
					cmdVisibility.Visible = True
                    RB_RibbonBar.Visible = True

                    'Hide Support icon in CE
                    If (DotNetNukeContext.Current.Application.Name = "DNNCORP.CE") Then
                        G16.FindControl("SupportTickets").Visible = False
                    End If

					Localize()

					If Not Page.IsPostBack Then
						Dim objUser As UserInfo = UserController.GetCurrentUserInfo
						If Not objUser Is Nothing Then
							If objUser.IsSuperUser Then
								hypMessage.ImageUrl = Upgrade.Upgrade.UpgradeIndicator(DotNetNukeContext.Current.Application.Version, Request.IsLocal, Request.IsSecureConnection)
								If hypMessage.ImageUrl <> "" Then
									hypMessage.ToolTip = Localization.GetString("hypUpgrade.Text", LocalResourceFile)
									hypMessage.NavigateUrl = Upgrade.Upgrade.UpgradeRedirect()
								End If
							Else ' branding
								If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) AndAlso Host.DisplayCopyright Then
                                    hypMessage.ImageUrl = "~/images/branding/iconbar_logo.png"
                                    hypMessage.ToolTip = DotNetNukeContext.Current.Application.Description
                                    hypMessage.NavigateUrl = Localization.GetString("hypMessageUrl.Text", LocalResourceFile)
								Else
									hypMessage.Visible = False
								End If
							End If
						End If
						SetMode(False)
						SetVisibility(False)
					End If
				ElseIf IsModuleAdmin() Then
					RB.Visible = True
					cmdVisibility.Visible = False
					RB_RibbonBar.Visible = False
					If Not Page.IsPostBack Then
						SetMode(False)
						SetVisibility(False)
					End If
				Else
					RB.Visible = False
				End If
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			'Set initial value
			DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(imgVisibility, RB_RibbonBar, PortalSettings.DefaultControlPanelVisibility, Common.Globals.ApplicationPath & "/images/collapse.gif", _
			Common.Globals.ApplicationPath & "/images/expand.gif", DNNClientAPI.MinMaxPersistanceType.Personalization, "Usability", "ControlPanelVisible" & Me.PortalSettings.PortalId.ToString)

			'Dim rbTool As New DotNetNuke.Web.UI.WebControls.DnnRibbonBarTool
			'Dim webServersTool As New DotNetNuke.Web.UI.WebControls.RibbonBarToolInfo("WebServerManager", True, False, "", "WebServerManager", "")
			'Dim healthMonitorTool As New DotNetNuke.Web.UI.WebControls.RibbonBarToolInfo("HealthMonitoring", True, False, "", "HealthMonitoring", "")
			'webServersTool.

			'rbTool.AllTools.Add(
			'Dim profileSelectedTab As Object = DotNetNuke.Services.Personalization.Personalization.GetProfile("RibbonBar", "SelectedTabIndex")
			'If (Not IsNothing(profileSelectedTab)) Then
			'	If (TypeOf profileSelectedTab Is Integer) Then
			'		Dim selectedTabIndex As Integer = DirectCast(profileSelectedTab, Integer)
			'		If (RibbonBarTabs.Tabs.Count > selectedTabIndex) Then
			'			RibbonBarTabs.SelectedIndex = selectedTabIndex
			'			Pages.SelectedIndex = selectedTabIndex
			'		End If
			'	End If
			'End If
		End Sub

		Protected Sub cmdVisibility_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdVisibility.Click
			SetVisibility(True)
			Response.Redirect(Request.RawUrl, True)
		End Sub

		'Make async Ajax request, don't interrupt the flow of using the tabs
		'Protected Sub OnClick_RibbonBarTabs(ByVal sender As Object, ByVal e As System.EventArgs) Handles RibbonBarTabs.TabClick
		'	DotNetNuke.Services.Personalization.Personalization.SetProfile("RibbonBar", "SelectedTabIndex", RibbonBarTabs.SelectedIndex)
		'End Sub

		Protected Sub optMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optMode.SelectedIndexChanged
			If Not Page.IsCallback Then
				SetMode(True)
				Response.Redirect(Request.RawUrl, True)
			End If
		End Sub

#End Region

	End Class

End Namespace
