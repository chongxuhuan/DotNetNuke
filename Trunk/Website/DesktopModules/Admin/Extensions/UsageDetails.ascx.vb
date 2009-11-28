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

Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Collections
Imports System.Reflection
Imports System.Text

Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Navigation
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Extensions

	''' <summary>
	''' Add and Edit Servers for a Web Farm
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' </history>
	Partial Class UsageDetails
		Inherits Entities.Modules.PortalModuleBase

#Region "Event Handlers"

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			Try
				lblTitle.Text = Localization.GetString("Usage", LocalResourceFile) + Package.FriendlyName

				UsageList.PagerSettings.FirstPageText = Localization.GetString("grd.PagerSettings.FirstPageText", LocalResourceFile)
				UsageList.PagerSettings.LastPageText = Localization.GetString("grd.PagerSettings.LastPageText", LocalResourceFile)
				UsageList.PagerSettings.NextPageText = Localization.GetString("grd.PagerSettings.NextPageText", LocalResourceFile)
				UsageList.PagerSettings.PreviousPageText = Localization.GetString("grd.PagerSettings.PreviousPageText", LocalResourceFile)
			Catch ex As Exception
				ProcessModuleLoadException(Me, ex)
			End Try
		End Sub

		Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
			Try
				BindFilterList()

				If (FilterUsageList.Visible) Then
					BindUsageList(Integer.Parse(FilterUsageList.SelectedValue), FilterUsageList.SelectedItem.Text)
				Else
					BindUsageList(PortalId, PortalController.GetCurrentPortalSettings().PortalName)
				End If
			Catch ex As Exception
				ProcessModuleLoadException(Me, ex)
			End Try
		End Sub

		Protected Sub FilterUsageList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles FilterUsageList.SelectedIndexChanged
			Try
				If (Not FilterUsageList.SelectedValue Is Nothing) Then
					UsageList.PageIndex = 0
					BindUsageList(Integer.Parse(FilterUsageList.SelectedValue), FilterUsageList.SelectedItem.Text)
				End If
			Catch ex As Exception
				ProcessModuleLoadException(Me, ex)
			End Try
		End Sub

		Protected Sub UsageList_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles UsageList.PageIndexChanging
			Try
				UsageList.PageIndex = e.NewPageIndex
				BindUsageList(Integer.Parse(FilterUsageList.SelectedValue), FilterUsageList.SelectedItem.Text)
			Catch ex As Exception
				ProcessModuleLoadException(Me, ex)
			End Try
		End Sub

#End Region

#Region "Properties"

		Dim _PackageID As Integer = Null.NullInteger
		Protected ReadOnly Property PackageID() As Integer
			Get
				If (_PackageID = Null.NullInteger AndAlso Not Request.QueryString("PackageID") Is Nothing) Then
					_PackageID = Int32.Parse(Request.QueryString("PackageID"))
				End If
				Return _PackageID
			End Get
		End Property

		Dim _Package As PackageInfo = Nothing
		Protected ReadOnly Property Package() As PackageInfo
			Get
				If _Package Is Nothing Then
					If PackageID = Null.NullInteger Then
						_Package = New PackageInfo()
					Else
						_Package = PackageController.GetPackage(PackageID)
					End If
				End If
				Return _Package
			End Get
		End Property

		Private _TabCtrl As TabController = Nothing
		Protected ReadOnly Property TabCtrl() As TabController
			Get
				If _TabCtrl Is Nothing Then
					_TabCtrl = New TabController()
				End If
				Return _TabCtrl
			End Get
		End Property

		Private _Portals As IDictionary(Of Integer, PortalInfo) = Nothing
		Protected ReadOnly Property Portals() As IDictionary(Of Integer, PortalInfo)
			Get
				If _Portals Is Nothing Then
					_Portals = New Dictionary(Of Integer, PortalInfo)

					Dim items As ArrayList = New PortalController().GetPortals()
					For Each item As PortalInfo In items
						_Portals.Add(item.PortalID, item)
					Next
				End If
				Return _Portals
			End Get
		End Property

		Protected ReadOnly Property IsSuperTab() As Boolean
			Get
				Return (Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab)
			End Get
		End Property

#End Region

#Region "Methods"

        Private Function FormatPortalAliases(ByVal PortalID As Integer, ByVal TabId As Integer) As String
            Dim str As New System.Text.StringBuilder
            Try
                Dim objPortalAliasController As New PortalAliasController
                Dim arr As ArrayList = objPortalAliasController.GetPortalAliasArrayByPortalID(PortalID)
                Dim objPortalAliasInfo As PortalAliasInfo
                Dim i As Integer
                For i = 0 To arr.Count - 1
                    objPortalAliasInfo = CType(arr(i), PortalAliasInfo)
                    str.Append("<a href=""" + AddHTTP(objPortalAliasInfo.HTTPAlias) + """>" + objPortalAliasInfo.HTTPAlias + "</a>")
                Next
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return str.ToString
        End Function

		Protected Function GetFormattedLink(ByVal dataItem As Object) As String
			Dim returnValue As StringBuilder = New StringBuilder()
			If (TypeOf dataItem Is Entities.Tabs.TabInfo) Then
				Dim tab As Entities.Tabs.TabInfo = DirectCast(dataItem, Entities.Tabs.TabInfo)

				If (Not tab Is Nothing) Then
					Dim index As Integer = 0
					TabCtrl.PopulateBreadCrumbs(tab)

					For Each t As Entities.Tabs.TabInfo In tab.BreadCrumbs
						If (index > 0) Then
							returnValue.Append(" > ")
						End If

                        If (tab.BreadCrumbs.Count - 1 = index) Then
                            Dim url As String
                            Dim objPortalAliasController As New PortalAliasController
                            Dim arr As ArrayList = objPortalAliasController.GetPortalAliasArrayByPortalID(t.PortalID)
                            Dim objPortalAliasInfo As PortalAliasInfo = CType(arr(0), PortalAliasInfo)
                            url = AddHTTP(objPortalAliasInfo.HTTPAlias) & "/Default.aspx?tabId=" & t.TabID

                            returnValue.AppendFormat("<a href=""{0}"">{1}</a>", url, t.LocalizedTabName)
                        Else
                            returnValue.AppendFormat("{0}", t.LocalizedTabName)
                        End If

						index = index + 1
					Next
				End If
			End If
			Return returnValue.ToString()
		End Function

		Private Sub BindFilterList()
			If (Not PackageID = Null.NullInteger AndAlso Not Package Is Nothing AndAlso Package.PackageType.ToUpper() = "MODULE") Then
				tblFilterUsage.Visible = IsSuperTab
				If Not IsPostBack Then
					If (FilterUsageList.Visible) Then
						FilterUsageList.DataSource = Portals.Values
						FilterUsageList.DataTextField = "PortalName"
						FilterUsageList.DataValueField = "PortalID"
						FilterUsageList.DataBind()
						FilterUsageList.Items.Insert(0, New ListItem(Localization.GetString("FilterOptionHost", LocalResourceFile), Null.NullInteger.ToString()))
						FilterUsageList.Items.Insert(0, New ListItem(Localization.GetString("FilterOptionSelect", LocalResourceFile), "-2"))
						FilterUsageList.Items(0).Selected = True
					End If
				End If
			End If
		End Sub

		Private _IsListBound As Boolean = False
		Private Sub BindUsageList(ByVal selectedPortalID As Integer, ByVal selectedPortalName As String)
			If (_IsListBound) Then
				Return
			End If
			_IsListBound = True

			Dim tabs As IDictionary(Of Integer, Entities.Tabs.TabInfo) = Nothing
			Dim portalName As String = String.Empty

			If Not PackageID = Null.NullInteger AndAlso Not Package Is Nothing Then
				If IsSuperTab Then
					If selectedPortalID = -2 Then
						portalName = String.Empty
					Else
						tabs = BuildData(selectedPortalID)
						portalName = selectedPortalName
					End If
				Else
					tabs = BuildData(PortalId)
					portalName = String.Empty
				End If
			End If

			If (Not tabs Is Nothing AndAlso tabs.Count > 0) Then
				UsageList.Visible = True
				UsageList.DataSource = tabs.Values
				UsageList.DataBind()

				UsageListMsg.Text = String.Format(Localization.GetString("Msg.InUseBy", LocalResourceFile), tabs.Count.ToString(), portalName)
			ElseIf (portalName <> String.Empty) Then
				UsageList.Visible = False
				UsageListMsg.Text = String.Format(Localization.GetString("Msg.NotUsedBy", LocalResourceFile), portalName)
			Else
				UsageList.Visible = False
				UsageListMsg.Text = String.Empty
			End If
		End Sub

		Private Function BuildData(ByVal portalID As Integer) As IDictionary(Of Integer, Entities.Tabs.TabInfo)
			Dim tabsWithModule As IDictionary(Of Integer, Entities.Tabs.TabInfo) = TabCtrl.GetTabsByPackageID(portalID, PackageID, False)
			Dim allPortalTabs As TabCollection = TabCtrl.GetTabsByPortal(PortalId)
			Dim tabsInOrder As IDictionary(Of Integer, Entities.Tabs.TabInfo) = New Dictionary(Of Integer, Entities.Tabs.TabInfo)

			'must get each tab, they parent may not exist
			For Each tab As Entities.Tabs.TabInfo In allPortalTabs.Values
				AddChildTabsToList(tab, allPortalTabs, tabsWithModule, tabsInOrder)
			Next

			Return tabsInOrder
		End Function

		Private Sub AddChildTabsToList(ByVal currentTab As Entities.Tabs.TabInfo, ByRef allPortalTabs As TabCollection, ByRef tabsWithModule As IDictionary(Of Integer, Entities.Tabs.TabInfo), ByRef tabsInOrder As IDictionary(Of Integer, Entities.Tabs.TabInfo))
			If (tabsWithModule.ContainsKey(currentTab.TabID) AndAlso Not tabsInOrder.ContainsKey(currentTab.TabID)) Then
				'add current tab
				tabsInOrder.Add(currentTab.TabID, currentTab)
				'add children of current tab
				For Each tab As Entities.Tabs.TabInfo In allPortalTabs.WithParentId(currentTab.TabID)
					AddChildTabsToList(tab, allPortalTabs, tabsWithModule, tabsInOrder)
				Next
			End If
		End Sub

#End Region

	End Class

End Namespace
