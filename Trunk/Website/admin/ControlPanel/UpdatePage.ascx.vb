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
Imports System.IO

Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Portals.PortalSettings
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Web.UI

Imports Telerik.Web.UI

Namespace DotNetNuke.UI.ControlPanel

	Partial Class UpdatePage
		Inherits System.Web.UI.UserControl
		Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool

#Region "Event Handlers"

		Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

		End Sub

		Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				If (Visible AndAlso Not IsPostBack()) Then
					Name.Text = CurrentTab.TabName
					IncludeInMenu.Checked = CurrentTab.IsVisible
					IsDisabled.Checked = CurrentTab.DisableLink
					TRSSL.Visible = PortalSettings.SSLEnabled
					IsSecure.Enabled = PortalSettings.SSLEnabled
					IsSecure.Checked = CurrentTab.IsSecure
					LoadAllLists()
				End If
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		End Sub

		Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
			If (TabPermissionController.CanManagePage()) Then
				Dim tabCtrl As New TabController()

				Dim selectedTabID As Integer = Null.NullInteger
				Dim selectedTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
				If (Not String.IsNullOrEmpty(PageLst.SelectedValue)) Then
					selectedTabID = Int32.Parse(PageLst.SelectedValue)
					selectedTab = tabCtrl.GetTab(selectedTabID, PortalSettings.ActiveTab.PortalID, False)
				End If

				Dim tabLocation As TabRelativeLocation = TabRelativeLocation.NOTSET
				If (Not String.IsNullOrEmpty(LocationLst.SelectedValue)) Then
					tabLocation = CType([Enum].Parse(GetType(TabRelativeLocation), LocationLst.SelectedValue), TabRelativeLocation)
				End If

				Dim tab As DotNetNuke.Entities.Tabs.TabInfo = CurrentTab

				tab.TabName = Name.Text
				tab.IsVisible = IncludeInMenu.Checked
				tab.DisableLink = IsDisabled.Checked
				tab.IsSecure = IsSecure.Checked
				tab.SkinSrc = SkinLst.SelectedValue

				Dim errMsg As String = ""
				Try
					RibbonBarManager.SaveTabInfoObject(tab, selectedTab, tabLocation, Nothing)
				Catch ex As DotNetNukeException
					LogException(ex)
					If (ex.ErrorCode <> DotNetNukeErrorCode.NotSet) Then
						errMsg = GetString("Err." + ex.ErrorCode.ToString())
					Else
						errMsg = ex.Message
					End If
				Catch ex As Exception
					LogException(ex)
					errMsg = ex.Message
				End Try

				'Clear the Tab's Cached modules
				DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(PortalSettings.ActiveTab.TabID)

				'Update Cached Tabs as TabPath may be needed before cache is cleared
				Dim tempTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
				If New TabController().GetTabsByPortal(PortalSettings.ActiveTab.PortalID).TryGetValue(tab.TabID, tempTab) Then
					tempTab.TabPath = tab.TabPath
				End If

				If (String.IsNullOrEmpty(errMsg)) Then
					Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(tab.TabID))
				Else
					'Show error
					errMsg = GetString("Err.Header") + "<br /><br />" + errMsg
					errMsg = errMsg.Replace("""", "'")
					Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "AddTabError", "function pageLoad() { radalert(""" + errMsg + """); }", True)
					'DotNetNuke.Framework.jQuery.RequestRegistration()
					'Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "AddTabError", "jQuery(document).ready(function() { alert(""" + errMsg.Replace("""", "'") + """); });", True)
				End If
			End If
		End Sub

#End Region

#Region "Properties"

		Public Property ToolName() As String Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool.ToolName
			Get
				Return "QuickUpdatePage"
			End Get
			Set(ByVal value As String)
				Throw New NotSupportedException("Set ToolName not supported")
			End Set
		End Property

		Public Overrides Property Visible() As Boolean
			Get
				Return MyBase.Visible = True AndAlso TabPermissionController.CanManagePage()
			End Get
			Set(ByVal value As Boolean)
				MyBase.Visible = value
			End Set
		End Property

#End Region

#Region "Methods"

		Private Sub LoadAllLists()
			LocationLst.Enabled = RibbonBarManager.CanMovePage()
			PageLst.Enabled = RibbonBarManager.CanMovePage()
			If (LocationLst.Enabled) Then
				LoadLocationList()
				LoadPageList()
			End If

			LoadSkinList()
		End Sub

		Private Sub LoadSkinList()
			SkinLst.ClearSelection()
			SkinLst.Items.Clear()
			SkinLst.Items.Add(New RadComboBoxItem(GetString("DefaultSkin"), ""))

			' load portal skins
			Dim portalSkinsHeader As New RadComboBoxItem(GetString("PortalSkins"), "")
			portalSkinsHeader.Enabled = False
			portalSkinsHeader.CssClass = "SkinListHeader"
			SkinLst.Items.Add(portalSkinsHeader)

			Dim arrFolders As String()
			Dim arrFiles As String()
			Dim strLastFolder = ""
			Dim strRoot = PortalSettings.HomeDirectoryMapPath & SkinController.RootSkin
			If Directory.Exists(strRoot) Then
				arrFolders = Directory.GetDirectories(strRoot)
				For Each strFolder In arrFolders
					arrFiles = Directory.GetFiles(strFolder, "*.ascx")
					For Each strFile In arrFiles
						strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
						If strLastFolder <> strFolder Then
							If strLastFolder <> "" Then
								SkinLst.Items.Add(GetSeparatorItem())
								'cboSkin.Items.Add(New ListItem(separatorText, ""))
							End If
							strLastFolder = strFolder
						End If
						'cboSkin.Items.Add(New ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[L]" & SkinRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))
						SkinLst.Items.Add(New RadComboBoxItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[L]" & SkinController.RootSkin & "/" & strFolder & "/" & Path.GetFileName(strFile)))
					Next
				Next
			End If

			'No portal skins added, remove the header
			If (SkinLst.Items.Count = 2) Then
				SkinLst.Items.Remove(1)
			End If

			'load host skins
			Dim hostSkinsHeader As New RadComboBoxItem(GetString("HostSkins"), "")
			hostSkinsHeader.Enabled = False
			hostSkinsHeader.CssClass = "SkinListHeader"
			SkinLst.Items.Add(hostSkinsHeader)

			strRoot = Common.Globals.HostMapPath & SkinController.RootSkin
			If Directory.Exists(strRoot) Then
				arrFolders = Directory.GetDirectories(strRoot)
				For Each strFolder In arrFolders
					If Not strFolder.EndsWith(glbHostSkinFolder) Then
						arrFiles = Directory.GetFiles(strFolder, "*.ascx")
						For Each strFile In arrFiles
							strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
							If strLastFolder <> strFolder Then
								If strLastFolder <> "" Then
									'cboSkin.Items.Add(New ListItem(strSeparator, ""))
									SkinLst.Items.Add(GetSeparatorItem())
								End If
								strLastFolder = strFolder
							End If
							'cboSkin.Items.Add(New ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[G]" & SkinRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))
							SkinLst.Items.Add(New RadComboBoxItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[G]" & SkinController.RootSkin & "/" & strFolder & "/" & Path.GetFileName(strFile)))
						Next
					End If
				Next
			End If

			'Set the selected item
			SkinLst.SelectedIndex = 0
			If (Not String.IsNullOrEmpty(CurrentTab.SkinSrc)) Then
				Dim selectItem As RadComboBoxItem = SkinLst.FindItemByValue(CurrentTab.SkinSrc)
				If (Not IsNothing(selectItem)) Then
					selectItem.Selected = True
				End If
			End If
		End Sub

		Private Function GetSeparatorItem() As RadComboBoxItem
			Dim item As New RadComboBoxItem(GetString("SkinLstSeparator"), "")
			item.CssClass = "SkinLstSeparator"
			item.Enabled = False
			Return item
		End Function

		Private Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
			If strSkinFolder.ToLower = "_default" Then
				' host folder
				Return strSkinFile
			Else ' portal folder
				Select Case strSkinFile.ToLower
					Case "skin", "container", "default"
						Return strSkinFolder
					Case Else
						Return strSkinFolder & " - " & strSkinFile
				End Select
			End If
		End Function

		Private Sub LoadLocationList()
			LocationLst.ClearSelection()
			LocationLst.Items.Clear()

			LocationLst.Items.Add(New RadComboBoxItem(GetString("NoLocationSelection"), ""))
			LocationLst.Items.Add(New RadComboBoxItem(GetString("Before"), "BEFORE"))
			LocationLst.Items.Add(New RadComboBoxItem(GetString("After"), "AFTER"))
			LocationLst.Items.Add(New RadComboBoxItem(GetString("Child"), "CHILD"))

			LocationLst.SelectedIndex = 0
		End Sub

		Private Sub LoadPageList()
			PageLst.ClearSelection()
			PageLst.Items.Clear()

			PageLst.DataTextField = "IndentedTabName"
			PageLst.DataValueField = "TabID"
			PageLst.DataSource = RibbonBarManager.GetPagesList()
			PageLst.DataBind()

			Dim disableCurrentTab As RadComboBoxItem = PageLst.FindItemByValue(CurrentTab.TabID)
			If (Not IsNothing(disableCurrentTab)) Then
				disableCurrentTab.Enabled = False
			End If

			PageLst.Items.Insert(0, New RadComboBoxItem(GetString("NoPageSelection"), ""))
			PageLst.SelectedIndex = 0
		End Sub

		Private _CurrentTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
		Private ReadOnly Property CurrentTab() As DotNetNuke.Entities.Tabs.TabInfo
			Get
                If (IsNothing(_CurrentTab)) Then
                    _CurrentTab = New TabController().GetTab(PortalSettings.ActiveTab.TabID, PortalSettings.ActiveTab.PortalID, False)
                End If
				Return _CurrentTab
			End Get
		End Property

		Private ReadOnly Property LocalResourceFile() As String
			Get
				Return String.Format("{0}/{1}/{2}.ascx.resx", Me.TemplateSourceDirectory, Localization.LocalResourceDirectory, Me.GetType().BaseType().Name)
			End Get
		End Property

		Private Function GetString(ByVal key As String) As String
			Return Localization.GetString(key, LocalResourceFile)
		End Function

		Private ReadOnly Property PortalSettings() As PortalSettings
			Get
				Return DotNetNuke.Entities.Portals.PortalSettings.Current
			End Get
		End Property

#End Region

	End Class

End Namespace


