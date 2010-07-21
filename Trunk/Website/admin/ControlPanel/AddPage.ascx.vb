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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Web.UI

Imports Telerik.Web.UI

Namespace DotNetNuke.UI.ControlPanel

	Partial Class AddPage
		Inherits System.Web.UI.UserControl
		Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool

#Region "Event Handlers"

		Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

		End Sub

		Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				If (Not IsPostBack()) Then
					If (Visible) Then
						LoadAllLists()
					End If
				End If
			Catch exc As Exception	  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		End Sub

		Protected Sub cmdAddPage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddPage.Click
			Dim tabCtrl As New TabController()

			Dim selectedTabID As Integer = Int32.Parse(PageLst.SelectedValue)
			Dim selectedTab As DotNetNuke.Entities.Tabs.TabInfo = tabCtrl.GetTab(selectedTabID, PortalSettings.ActiveTab.PortalID, False)
			Dim tabLocation As TabRelativeLocation = CType([Enum].Parse(GetType(TabRelativeLocation), LocationLst.SelectedValue), TabRelativeLocation)
			Dim newTab As DotNetNuke.Entities.Tabs.TabInfo = RibbonBarManager.InitTabInfoObject(selectedTab, tabLocation)

			Dim templateFile As String = ""
			If (Not String.IsNullOrEmpty(TemplateLst.SelectedValue)) Then
				templateFile = System.IO.Path.Combine(PortalSettings.HomeDirectoryMapPath, "Templates\" + TemplateLst.SelectedValue)
			End If

			newTab.TabName = Name.Text
			newTab.IsVisible = IncludeInMenu.Checked

			Dim errMsg As String = ""
			Try
				RibbonBarManager.SaveTabInfoObject(newTab, selectedTab, tabLocation, templateFile)
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
			If New TabController().GetTabsByPortal(PortalSettings.ActiveTab.PortalID).TryGetValue(newTab.TabID, tempTab) Then
				tempTab.TabPath = newTab.TabPath
			End If

			If (String.IsNullOrEmpty(errMsg)) Then
				Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(newTab.TabID))
			Else
				'Show error
				errMsg = GetString("Err.Header") + "<br /><br />" + errMsg
				DotNetNuke.Web.UI.Utilities.RegisterAlertOnPageLoad(Me, errMsg)
			End If
		End Sub

#End Region

#Region "Properties"

		Public Property ToolName() As String Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool.ToolName
			Get
				Return "QuickAddPage"
			End Get
			Set(ByVal value As String)
				Throw New NotSupportedException("Set ToolName not supported")
			End Set
		End Property

		Public Overrides Property Visible() As Boolean
			Get
				Return MyBase.Visible = True AndAlso TabPermissionController.CanAddPage
			End Get
			Set(ByVal value As Boolean)
				MyBase.Visible = value
			End Set
		End Property

#End Region

#Region "Methods"

		Private _NewTabObject As DotNetNuke.Entities.Tabs.TabInfo = Nothing
		Protected ReadOnly Property NewTabObject() As DotNetNuke.Entities.Tabs.TabInfo
			Get
				If (IsNothing(_NewTabObject)) Then
					_NewTabObject = RibbonBarManager.InitTabInfoObject(PortalSettings.ActiveTab)
				End If
				Return _NewTabObject
			End Get
		End Property

		Private Sub LoadAllLists()
			LoadLocationList()
			LoadTemplateList()
			LoadPageList()
		End Sub

		Private Sub LoadTemplateList()
			TemplateLst.ClearSelection()
			TemplateLst.Items.Clear()

			'Get Templates Folder
			Dim templateFiles As ArrayList = Common.Globals.GetFileList(PortalSettings.PortalId, "page.template", False, "Templates/")
			Dim dnnFile As FileItem
			For Each dnnFile In templateFiles
				Dim item As New RadComboBoxItem(dnnFile.Text.Replace(".page.template", ""), dnnFile.Text)
				TemplateLst.Items.Add(item)
				If (item.Text = "Default") Then
					item.Selected = True
				End If
			Next

			TemplateLst.Items.Insert(0, New RadComboBoxItem(GetString("NoTemplate"), ""))
		End Sub

		Private Sub LoadLocationList()
			LocationLst.ClearSelection()
			LocationLst.Items.Clear()

			LocationLst.Items.Add(New RadComboBoxItem(GetString("Before"), "BEFORE"))
			LocationLst.Items.Add(New RadComboBoxItem(GetString("After"), "AFTER"))
			LocationLst.Items.Add(New RadComboBoxItem(GetString("Child"), "CHILD"))

			If (Not PortalSecurity.IsInRole("Administrators")) Then
				LocationLst.SelectedIndex = 2
			Else
				LocationLst.SelectedIndex = 1
			End If
		End Sub

		Private Sub LoadPageList()
			PageLst.ClearSelection()
			PageLst.Items.Clear()

			PageLst.DataTextField = "IndentedTabName"
			PageLst.DataValueField = "TabID"
			PageLst.DataSource = RibbonBarManager.GetPagesList()
			PageLst.DataBind()

			Dim item As RadComboBoxItem = PageLst.FindItemByValue(PortalSettings.ActiveTab.TabID)
			If (Not IsNothing(item)) Then
				item.Selected = True
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

		Private ReadOnly Property PortalSettings() As PortalSettings
			Get
				Return DotNetNuke.Entities.Portals.PortalSettings.Current
			End Get
		End Property

#End Region

	End Class

End Namespace


