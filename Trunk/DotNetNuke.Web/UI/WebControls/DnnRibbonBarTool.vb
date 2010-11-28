'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System
Imports System.Collections.Generic
Imports System.Web.UI
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Web.UI.WebControls

	<ParseChildren(True)> _
	Public Class DnnRibbonBarTool
		Inherits System.Web.UI.Control
		Implements IDnnRibbonBarTool

#Region "Properties"

		Private _ToolInfo As RibbonBarToolInfo = New RibbonBarToolInfo()
		Public Overridable Property ToolInfo() As RibbonBarToolInfo
			Get
				If (ViewState("ToolInfo") Is Nothing) Then
					ViewState.Add("ToolInfo", New RibbonBarToolInfo())
				End If
				Return DirectCast(ViewState("ToolInfo"), RibbonBarToolInfo)
			End Get
			Set(ByVal value As RibbonBarToolInfo)
				ViewState("ToolInfo") = value
			End Set
		End Property

		Public Overridable Property ToolName() As String Implements IDnnRibbonBarTool.ToolName
			Get
				Return ToolInfo.ToolName
			End Get
			Set(ByVal value As String)
				If (AllTools.ContainsKey(value)) Then
					ToolInfo = AllTools(value)
				Else
					Throw New NotSupportedException("Tool not found [" + value + "]")
				End If
			End Set
		End Property

		Public Overridable Property NavigateUrl() As String
			Get
                Return Utilities.GetViewStateAsString(ViewState("NavigateUrl"), Null.NullString)
            End Get
			Set(ByVal value As String)
				ViewState("NavigateUrl") = value
			End Set
		End Property

		Public Overridable Property ToolCssClass() As String
            Get
                Return Utilities.GetViewStateAsString(ViewState("ToolCssClass"), Null.NullString)
            End Get
			Set(ByVal value As String)
				ViewState("ToolCssClass") = value
			End Set
		End Property

		Public Overridable Property ImageUrl() As String
			Get
                Return Utilities.GetViewStateAsString(ViewState("ImageUrl"), Null.NullString)
            End Get
			Set(ByVal value As String)
				ViewState("ImageUrl") = value
			End Set
		End Property

		Public Overridable Property Text() As String
			Get
                Return Utilities.GetViewStateAsString(ViewState("Text"), Null.NullString)
            End Get
			Set(ByVal value As String)
				ViewState("Text") = value
			End Set
		End Property

        Public Overridable Property ToolTip() As String
            Get
                Return Utilities.GetViewStateAsString(ViewState("ToolTip"), Null.NullString)
            End Get
            Set(ByVal value As String)
                ViewState("ToolTip") = value
            End Set
        End Property

		Private _DnnLinkButton As DnnImageTextButton = Nothing
		Protected Overridable ReadOnly Property DnnLinkButton() As DnnImageTextButton
			Get
				If (_DnnLinkButton Is Nothing) Then
					_DnnLinkButton = New DnnImageTextButton()
					_DnnLinkButton.ID = Me.ID + "_CPCommandBtn"
				End If
				Return _DnnLinkButton
			End Get
		End Property

		Private _DnnLink As DnnImageTextLink = Nothing
		Protected Overridable ReadOnly Property DnnLink() As DnnImageTextLink
			Get
				If (_DnnLink Is Nothing) Then
					_DnnLink = New DnnImageTextLink()
				End If
				Return _DnnLink
			End Get
		End Property

		Private _AllTools As IDictionary(Of String, RibbonBarToolInfo)
		Protected Overridable ReadOnly Property AllTools() As IDictionary(Of String, RibbonBarToolInfo)
			Get
				If (IsNothing(_AllTools)) Then
					_AllTools = New Dictionary(Of String, RibbonBarToolInfo)

					'Framework
					_AllTools.Add("PageSettings", New RibbonBarToolInfo("PageSettings", False, False, "", "", ""))
					_AllTools.Add("CopyPage", New RibbonBarToolInfo("CopyPage", False, False, "", "", ""))
					_AllTools.Add("DeletePage", New RibbonBarToolInfo("DeletePage", False, True, "", "", ""))
					_AllTools.Add("ImportPage", New RibbonBarToolInfo("ImportPage", False, False, "", "", ""))
					_AllTools.Add("ExportPage", New RibbonBarToolInfo("ExportPage", False, False, "", "", ""))
					_AllTools.Add("NewPage", New RibbonBarToolInfo("NewPage", False, False, "", "", ""))
					_AllTools.Add("CopyPermissionsToChildren", New RibbonBarToolInfo("CopyPermissionsToChildren", False, True, "", "", ""))
					_AllTools.Add("CopyDesignToChildren", New RibbonBarToolInfo("CopyDesignToChildren", False, True, "", "", ""))
					_AllTools.Add("Help", New RibbonBarToolInfo("Help", False, False, "_Blank", "", ""))

					'Modules on Tabs
					_AllTools.Add("Console", New RibbonBarToolInfo("Console", False, False, "", "Console", ""))
					_AllTools.Add("HostConsole", New RibbonBarToolInfo("HostConsole", True, False, "", "Console", ""))
					_AllTools.Add("Dashboard", New RibbonBarToolInfo("Dashboard", True, False, "", "Dashboard", ""))
					_AllTools.Add("Extensions", New RibbonBarToolInfo("Extensions", False, False, "", "Extensions", ""))
					_AllTools.Add("HostExtensions", New RibbonBarToolInfo("HostExtensions", True, False, "", "Extensions", ""))
					_AllTools.Add("FileManager", New RibbonBarToolInfo("FileManager", False, False, "", "File Manager", ""))
					_AllTools.Add("UploadFile", New RibbonBarToolInfo("UploadFile", False, False, "", "File Manager", "Edit"))
					_AllTools.Add("HostFileManager", New RibbonBarToolInfo("HostFileManager", True, False, "", "File Manager", ""))
					_AllTools.Add("HostUploadFile", New RibbonBarToolInfo("HostUploadFile", True, False, "", "File Manager", "Edit"))
					_AllTools.Add("GoogleAnalytics", New RibbonBarToolInfo("GoogleAnalytics", False, False, "", "GoogleAnalytics", ""))
					_AllTools.Add("HostSettings", New RibbonBarToolInfo("HostSettings", True, False, "", "Host Settings", ""))
					_AllTools.Add("Languages", New RibbonBarToolInfo("Languages", False, False, "", "Languages", ""))
					_AllTools.Add("Lists", New RibbonBarToolInfo("Lists", True, False, "", "Lists", ""))
					_AllTools.Add("EventLog", New RibbonBarToolInfo("EventLog", False, False, "", "Log Viewer", ""))
					_AllTools.Add("Marketplace", New RibbonBarToolInfo("Marketplace", True, False, "", "Marketplace", ""))
					_AllTools.Add("Newsletters", New RibbonBarToolInfo("Newsletters", False, False, "", "Newsletters", ""))
					_AllTools.Add("Sites", New RibbonBarToolInfo("Sites", True, False, "", "Portals", ""))
					_AllTools.Add("RecycleBin", New RibbonBarToolInfo("RecycleBin", False, False, "", "Recycle Bin", ""))
					_AllTools.Add("Scheduler", New RibbonBarToolInfo("Scheduler", True, False, "", "Scheduler", ""))
					_AllTools.Add("SearchAdmin", New RibbonBarToolInfo("SearchAdmin", True, False, "", "Search Admin", ""))
					_AllTools.Add("UserRoles", New RibbonBarToolInfo("UserRoles", False, False, "", "Security Roles", ""))
                    _AllTools.Add("NewRole", New RibbonBarToolInfo("NewRole", False, False, "", "Security Roles", "Edit"))
					_AllTools.Add("SiteLog", New RibbonBarToolInfo("SiteLog", False, False, "", "Site Log", ""))
					_AllTools.Add("SiteSettings", New RibbonBarToolInfo("SiteSettings", False, False, "", "Site Settings", ""))
					_AllTools.Add("SiteWizard", New RibbonBarToolInfo("SiteWizard", False, False, "", "Site Wizard", ""))
					_AllTools.Add("Skins", New RibbonBarToolInfo("Skins", False, False, "", "Skins", ""))
					_AllTools.Add("SQL", New RibbonBarToolInfo("SQL", True, False, "", "SQL", ""))
					_AllTools.Add("ManagePages", New RibbonBarToolInfo("ManagePages", False, False, "", "Tabs", ""))
					_AllTools.Add("Users", New RibbonBarToolInfo("Users", False, False, "", "User Accounts", ""))
					_AllTools.Add("NewUser", New RibbonBarToolInfo("NewUser", False, False, "", "User Accounts", "Edit"))
					_AllTools.Add("SuperUsers", New RibbonBarToolInfo("SuperUsers", True, False, "", "User Accounts", ""))
					_AllTools.Add("Vendors", New RibbonBarToolInfo("Vendors", False, False, "", "Vendors", ""))
					_AllTools.Add("HostVendors", New RibbonBarToolInfo("HostVendors", True, False, "", "Vendors", ""))
					_AllTools.Add("WhatsNew", New RibbonBarToolInfo("WhatsNew", True, False, "", "WhatsNew", ""))
				End If

				Return _AllTools
			End Get
		End Property

		Private ReadOnly Property PortalSettings() As DotNetNuke.Entities.Portals.PortalSettings
			Get
				Return DotNetNuke.Entities.Portals.PortalSettings.Current
			End Get
		End Property

#End Region

#Region "Events"

		Protected Overrides Sub CreateChildControls()
			Controls.Clear()
			Controls.Add(DnnLinkButton)
			Controls.Add(DnnLink)
		End Sub

		Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
			EnsureChildControls()
			AddHandler DnnLinkButton.Click, AddressOf Me.ControlPanelTool_OnClick
		End Sub

		Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
			ProcessTool()
			Visible = (DnnLink.Visible = True OrElse DnnLinkButton.Visible = True)
			MyBase.OnPreRender(e)
		End Sub

		Public Overridable Sub ControlPanelTool_OnClick(ByVal sender As Object, ByVal e As EventArgs)
			Select Case ToolInfo.ToolName
				Case "DeletePage"
					If (HasToolPermissions("DeletePage")) Then
                        Dim url As String = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete")
						Page.Response.Redirect(url, True)
					End If
				Case "CopyPermissionsToChildren"
					If (HasToolPermissions("CopyPermissionsToChildren")) Then
						TabController.CopyPermissionsToChildren(PortalSettings.ActiveTab, PortalSettings.ActiveTab.TabPermissions)
						Page.Response.Redirect(Page.Request.RawUrl)
					End If
				Case "CopyDesignToChildren"
					If (HasToolPermissions("CopyDesignToChildren")) Then
						TabController.CopyDesignToChildren(PortalSettings.ActiveTab, PortalSettings.ActiveTab.SkinSrc, PortalSettings.ActiveTab.ContainerSrc)
						Page.Response.Redirect(Page.Request.RawUrl)
					End If
			End Select
		End Sub

#End Region

#Region "Methods"

		Protected Overridable Sub ProcessTool()
			DnnLink.Visible = False
			DnnLinkButton.Visible = False

			If (Not String.IsNullOrEmpty(ToolInfo.ToolName)) Then
				If (ToolInfo.UseButton) Then
					DnnLinkButton.Visible = HasToolPermissions(ToolInfo.ToolName)
					DnnLinkButton.Enabled = EnableTool()
					DnnLinkButton.Localize = False

					DnnLinkButton.ImageUrl = GetImageUrl()
					DnnLinkButton.CssClass = ToolCssClass
					DnnLinkButton.DisabledCssClass = ToolCssClass + " rgIconDisabled"

					DnnLinkButton.Text = GetText()
					DnnLinkButton.ToolTip = GetToolTip()

					DnnLinkButton.ConfirmMessage = GetButtonConfirmMessage()
				Else
					DnnLink.Visible = HasToolPermissions(ToolInfo.ToolName)
					DnnLink.Enabled = EnableTool()
					DnnLink.Localize = False

					If (DnnLink.Enabled) Then
						DnnLink.NavigateUrl = BuildToolUrl()

						'can't find the page, disable it?
						If (DnnLink.NavigateUrl = "") Then
							DnnLink.Enabled = False
						End If
					End If

					DnnLink.ImageUrl = GetImageUrl()
					DnnLink.CssClass = ToolCssClass
					DnnLink.DisabledCssClass = ToolCssClass + " rgIconDisabled"

					DnnLink.Text = GetText()
					DnnLink.ToolTip = GetToolTip()
					DnnLink.Target = ToolInfo.LinkWindowTarget
				End If
			End If
		End Sub

		Protected Overridable Function EnableTool() As Boolean
			Dim returnValue As Boolean = True

			Select Case ToolInfo.ToolName
				Case "DeletePage"
					If (TabController.IsSpecialTab(TabController.CurrentPage.TabID, PortalSettings)) Then
						returnValue = False
					End If
				Case "CopyDesignToChildren", "CopyPermissionsToChildren"
					returnValue = ActiveTabHasChildren()
					If (returnValue = True AndAlso ToolInfo.ToolName = "CopyPermissionsToChildren") Then
						If (PortalSettings.ActiveTab.IsSuperTab) Then
							returnValue = False
						End If
					End If
					'Case "Help"
					'	returnValue = Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.HelpURL)
					'Case Else
					'	If (AllTools.ContainsKey(toolName)) Then
					'		Dim friendlyName As String = AllTools(toolName).ModuleFriendlyName

					'		If (Not String.IsNullOrEmpty(friendlyName)) Then
					'			returnValue = True
					'			Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo = Nothing

					'			If (IsHostTool(toolName)) Then
					'				moduleInfo = GetInstalledModule(Null.NullInteger, friendlyName)
					'			Else
					'				moduleInfo = GetInstalledModule(PortalSettings.PortalId, friendlyName)
					'			End If

					'			If moduleInfo Is Nothing Then
					'				returnValue = False
					'			End If
					'		End If
					'	End If
			End Select

			Return returnValue
		End Function

		Protected Overridable Function HasToolPermissions(ByVal toolName As String) As Boolean
			Dim isHostTool As Boolean = False
			If (ToolInfo.ToolName = toolName) Then
				isHostTool = ToolInfo.IsHostTool
			ElseIf (AllTools.ContainsKey(toolName)) Then
				isHostTool = AllTools(toolName).IsHostTool
			End If

			If (isHostTool AndAlso Not DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsSuperUser) Then
				Return False
			End If

			Dim returnValue As Boolean = True
			Select Case toolName
				Case "PageSettings", "CopyDesignToChildren", "CopyPermissionsToChildren"
					returnValue = TabPermissionController.CanManagePage

					If (returnValue = True AndAlso toolName = "CopyPermissionsToChildren") Then
						If (Not DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
							returnValue = False
						End If
					End If
				Case "CopyPage"
					returnValue = TabPermissionController.CanCopyPage
				Case "DeletePage"
					returnValue = (TabPermissionController.CanDeletePage)
				Case "ImportPage"
					returnValue = TabPermissionController.CanImportPage
				Case "ExportPage"
					returnValue = TabPermissionController.CanExportPage
				Case "NewPage"
					returnValue = TabPermissionController.CanAddPage
				Case "Help"
					returnValue = Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.HelpURL)
				Case Else
					'if it has a module definition, look it up and check permissions
					'if it doesn't exist, assume no permission
					Dim friendlyName As String = ""
					If (ToolInfo.ToolName = toolName) Then
						friendlyName = ToolInfo.ModuleFriendlyName
					ElseIf (AllTools.ContainsKey(toolName)) Then
						friendlyName = AllTools(toolName).ModuleFriendlyName
					End If

					If (Not String.IsNullOrEmpty(friendlyName)) Then
						returnValue = False
						Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo = Nothing

						If (isHostTool) Then
							moduleInfo = GetInstalledModule(Null.NullInteger, friendlyName)
						Else
							moduleInfo = GetInstalledModule(PortalSettings.PortalId, friendlyName)
						End If

						If Not moduleInfo Is Nothing Then
							returnValue = ModulePermissionController.CanViewModule(moduleInfo)

							'If (toolName = "UploadFile") Then
							'	If (Not DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
							'		returnValue = False
							'	End If
							'End If
						End If
					End If
			End Select

			Return returnValue
		End Function

		Protected Overridable Function BuildToolUrl() As String
			If (ToolInfo.IsHostTool AndAlso Not DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsSuperUser) Then
				Return "javascript:void(0);"
			End If

			If (Not String.IsNullOrEmpty(NavigateUrl)) Then
				Return NavigateUrl
			End If

			Dim returnValue As String = "javascript:void(0);"
			Select Case ToolInfo.ToolName
				Case "PageSettings"
					returnValue = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=edit")
					Exit Select
				Case "CopyPage"
					returnValue = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=copy")
					Exit Select
				Case "DeletePage"
					returnValue = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete")
					Exit Select
				Case "ImportPage"
					returnValue = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ImportTab")
					Exit Select
				Case "ExportPage"
					returnValue = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ExportTab")
					Exit Select
				Case "NewPage"
					returnValue = DotNetNuke.Common.Globals.NavigateURL("Tab")
					Exit Select
				Case "Help"
					If Not String.IsNullOrEmpty(DotNetNuke.Entities.Host.Host.HelpURL) Then
						returnValue = FormatHelpUrl(DotNetNuke.Entities.Host.Host.HelpURL, PortalSettings, "")
					End If
					Exit Select
				Case Else
					'if it has a module definition, look it up and check permissions
					Dim friendlyName As String = ToolInfo.ModuleFriendlyName
					Dim controlKey As String = ToolInfo.ControlKey

					If (Not String.IsNullOrEmpty(friendlyName)) Then
						Dim additionalParams As List(Of String) = New List(Of String)()
						If (ToolInfo.ToolName = "UploadFile" Or ToolInfo.ToolName = "HostUploadFile") Then
							additionalParams.Add("ftype=File")
							additionalParams.Add("rtab=" + PortalSettings.ActiveTab.TabID.ToString())
						End If

						If (ToolInfo.IsHostTool) Then
							returnValue = GetTabURL(Null.NullInteger, friendlyName, controlKey, additionalParams)
						Else
							returnValue = GetTabURL(PortalSettings.PortalId, friendlyName, controlKey, additionalParams)
						End If
					End If
					Exit Select
			End Select

			Return returnValue
		End Function

		Protected Overridable Function GetText() As String
			If (String.IsNullOrEmpty(Text)) Then
				Return GetString(String.Format("Tool.{0}.Text", ToolInfo.ToolName))
			Else
				Return Text
			End If
		End Function

		Protected Overridable Function GetToolTip() As String
			If (ToolInfo.ToolName = "DeletePage") Then
				If (TabController.IsSpecialTab(TabController.CurrentPage.TabID, PortalSettings)) Then
					Return GetString("Tool.DeletePage.Special.ToolTip")
				End If
			End If

			If (String.IsNullOrEmpty(Text)) Then
				Dim tip As String = GetString(String.Format("Tool.{0}.ToolTip", ToolInfo.ToolName))
				If (String.IsNullOrEmpty(tip)) Then
					tip = GetString(String.Format("Tool.{0}.Text", ToolInfo.ToolName))
				End If
				Return tip
			Else
				Return ToolTip
			End If
		End Function

		Protected Overridable Function GetImageUrl() As String
			If (Not String.IsNullOrEmpty(ImageUrl)) Then
				Return ImageUrl
			End If

			Return Page.ResolveUrl(String.Format("~/admin/controlpanel/ribbonimages/{0}.gif", ToolInfo.ToolName))
		End Function

		Protected Overridable Function GetTabURL(ByVal portalID As Integer, ByVal friendlyName As String, ByVal controlKey As String, ByVal additionalParams As List(Of String)) As String
			Dim strURL As String = String.Empty

			If (IsNothing(additionalParams)) Then
				additionalParams = New List(Of String)
			End If

			Dim moduleCtrl As New DotNetNuke.Entities.Modules.ModuleController()
			Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo = moduleCtrl.GetModuleByDefinition(portalID, friendlyName)

			If (Not IsNothing(moduleInfo)) Then
				Dim isHostPage As Boolean = (portalID = Null.NullInteger)
				If (Not String.IsNullOrEmpty(controlKey)) Then
					additionalParams.Insert(0, "mid=" + moduleInfo.ModuleID.ToString())
                End If

                Dim currentCulture As String = Threading.Thread.CurrentThread.CurrentCulture.Name

                strURL = DotNetNuke.Common.Globals.NavigateURL(moduleInfo.TabID, isHostPage, PortalSettings, controlKey, currentCulture, additionalParams.ToArray())
				'If (portalID = Null.NullInteger) Then
				'	If (String.IsNullOrEmpty(controlKey)) Then
				'		strURL = NavigateURL(moduleInfo.TabID, True, PortalSettings, "", additionalParams.ToArray())
				'	Else
				'		additionalParams.Add("mid=" + moduleInfo.ModuleID.ToString())
				'		strURL = NavigateURL(moduleInfo.TabID, True, PortalSettings, controlKey, additionalParams.ToArray())
				'	End If
				'Else
				'	If (String.IsNullOrEmpty(controlKey)) Then
				'		strURL = NavigateURL(moduleInfo.TabID)
				'	Else
				'		additionalParams.Add("mid=" + moduleInfo.ModuleID.ToString())
				'		strURL = NavigateURL(moduleInfo.TabID, controlKey, additionalParams.ToArray())
				'	End If
				'End If
			End If

			Return strURL
		End Function

		Protected Overridable Function ActiveTabHasChildren() As Boolean
			Dim children As List(Of TabInfo) = DotNetNuke.Entities.Tabs.TabController.GetTabsByParent(PortalSettings.ActiveTab.TabID, PortalSettings.ActiveTab.PortalID)

			If (IsNothing(children) OrElse children.Count < 1) Then
				Return False
			End If

			Return True
		End Function

		Protected Overridable Function GetButtonConfirmMessage() As String
			If (ToolInfo.ToolName = "DeletePage") Then
				Return GetString("Tool.DeletePage.Confirm")
			ElseIf (ToolInfo.ToolName = "CopyPermissionsToChildren") Then
				If (DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
					Return GetString("Tool.CopyPermissionsToChildren.Confirm")
				Else
					Return GetString("Tool.CopyPermissionsToChildrenPageEditor.Confirm")
				End If
			ElseIf (ToolInfo.ToolName = "CopyDesignToChildren") Then
				If (DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
					Return GetString("Tool.CopyDesignToChildren.Confirm")
				Else
					Return GetString("Tool.CopyDesignToChildrenPageEditor.Confirm")
				End If
			End If

			Return String.Empty
		End Function

		Protected Overridable Function GetString(ByVal key As String) As String
			Return Utilities.GetLocalizedStringFromParent(key, Me)
		End Function

		Private Function GetInstalledModule(ByVal portalID As Integer, ByVal friendlyName As String) As DotNetNuke.Entities.Modules.ModuleInfo
			Dim moduleCtrl As New DotNetNuke.Entities.Modules.ModuleController()
			Return moduleCtrl.GetModuleByDefinition(portalID, friendlyName)
		End Function

#End Region

	End Class

End Namespace
