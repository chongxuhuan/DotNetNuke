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

Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Collections
Imports System.Reflection

Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Navigation
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Modules.Console.Components

Namespace DotNetNuke.Modules.Admin.Console

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The ViewConsole class displays the content
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial Class ViewConsole
		Inherits Entities.Modules.PortalModuleBase

#Region "Event Handlers"

		Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				jQuery.RequestRegistration()

				Dim consoleJs As String = ResolveUrl("~/desktopmodules/admin/console/jquery.console.js")

				Page.ClientScript.RegisterClientScriptInclude("ConsoleJS", consoleJs)

				'Save User Preferences
				SavePersonalizedSettings()
			Catch exc As Exception
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				If (Not IsPostBack) Then
					IconSize.Visible = AllowSizeChange
					View.Visible = AllowViewChange

					For Each val As String In ConsoleController.GetSizeValues()
						IconSize.Items.Add(New ListItem(Localization.GetString(val + ".Text", MyBase.LocalResourceFile), val))
					Next

					For Each val As String In ConsoleController.GetViewValues()
						View.Items.Add(New ListItem(Localization.GetString(val + ".Text", MyBase.LocalResourceFile), val))
					Next

					IconSize.SelectedValue = DefaultSize
					View.SelectedValue = DefaultView

					SettingsBreak.Visible = (IconSize.Visible And View.Visible)

					Dim tempTabs As List(Of TabInfo) = Nothing
					If (IsHostTab()) Then
						tempTabs = TabController.GetTabsBySortOrder(Null.NullInteger)
					Else
						tempTabs = TabController.GetTabsBySortOrder(PortalId)
					End If

					Dim tabs As IList(Of Entities.Tabs.TabInfo) = New List(Of Entities.Tabs.TabInfo)()

					Dim parentIDList As IList(Of Integer) = New List(Of Integer)()
					parentIDList.Add(ConsoleTabID)

					For Each tab As TabInfo In tempTabs
						If (Not CanShowTab(tab)) Then
							Continue For
						End If

						If (parentIDList.Contains(tab.ParentId)) Then
							If (Not parentIDList.Contains(tab.TabID)) Then
								parentIDList.Add(tab.TabID)
							End If
							tabs.Add(tab)
						End If
					Next

					DetailView.DataSource = tabs
					DetailView.DataBind()
				End If

				If (ConsoleWidth <> String.Empty) Then
					Console.Attributes.Add("style", "width:" & ConsoleWidth)
				End If
			Catch exc As Exception		  'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

#End Region

#Region "Methods"

		Private _groupTabID As Integer = -1

		Protected Function GetHtml(ByVal dataItem As Object) As String
			Dim tab As TabInfo = CType(dataItem, TabInfo)
			Dim returnValue As String = String.Empty

			If (_groupTabID > -1 AndAlso _groupTabID <> tab.ParentId) Then
				_groupTabID = -1
				If (Not tab.DisableLink) Then
					returnValue = "<br style=""clear:both;"" /><br />"
				End If
			End If

			If (tab.DisableLink) Then
				Dim headerHtml As String = "<br style=""clear:both;"" /><br /><h1><span class=""TitleHead"">{0}</span></h1><br style=""clear:both"" />"
				returnValue += String.Format(headerHtml, _
				   DataBinder.Eval(dataItem, "TabName"))

				_groupTabID = Integer.Parse(DataBinder.Eval(dataItem, "TabID"))
			Else
				Dim contentHtml As String = "<div>" _
				  + "<a href=""{0}""><img src=""{1}"" /><img src=""{2}"" /></a>" _
				  + "<h3 class=""SubHead"">{3}</h3>" _
				  + "<div>{4}</div>" _
				  + "</div>"

                returnValue += String.Format(contentHtml, _
                  DataBinder.Eval(dataItem, "FullUrl"), _
                  GetIconUrl(dataItem, "IconFile"), _
                  GetIconUrl(dataItem, "IconFileLarge"), _
                  DataBinder.Eval(dataItem, "LocalizedTabName"), _
                  DataBinder.Eval(dataItem, "Description"))
			End If

			Return returnValue
		End Function

		Private Function CanShowTab(ByVal tab As TabInfo) As Boolean
            Return (Not tab.IsDeleted AndAlso (tab.StartDate < DateTime.Now OrElse tab.StartDate = Null.NullDate)) _
                AndAlso TabPermissionController.CanViewPage(tab)
        End Function

		Protected Function GetIconUrl(ByVal dataItem As Object, ByVal size As String) As String
			Dim iconURL As String = Convert.ToString(DataBinder.Eval(dataItem, size))
			If (iconURL = String.Empty) Then
				If (size = "IconFile") Then
					iconURL = "~/images/icon_unknown_16px.gif"
				Else
					iconURL = "~/images/icon_unknown_32px.gif"
				End If
			End If

            If iconURL.Contains("~") = False Then
                iconURL = System.IO.Path.Combine(PortalSettings.HomeDirectory, iconURL)
            End If

			Return ResolveUrl(iconURL)
		End Function

		Protected Function GetClientSideSettings() As String
			Dim tmid As String = "-1"
			If (UserId > -1) Then
				tmid = TabModuleId.ToString()
			End If

			Return String.Format("allowIconSizeChange: {0}, allowDetailChange: {1}, selectedSize: '{2}', showDetails: '{3}', tabModuleID: {4}, showTooltip: {5}" _
			 , AllowSizeChange.ToString().ToLower() _
			 , AllowViewChange.ToString().ToLower() _
			 , DefaultSize _
			 , DefaultView _
			 , tmid _
			 , ShowTooltip.ToString().ToLower())
		End Function

		Protected Sub SavePersonalizedSettings()
			If (UserId > -1) Then
				Dim consoleModuleID As Integer = -1

				Try
					If Not Request.QueryString("CTMID") Is Nothing Then
						consoleModuleID = CInt(Request.QueryString("CTMID"))
					End If
				Catch ex As Exception
					consoleModuleID = -1
				End Try

				If (consoleModuleID = TabModuleId) Then

					Dim consoleSize As String = String.Empty
					If Not Request.QueryString("CS") Is Nothing Then
						consoleSize = Request.QueryString("CS").ToString()
					End If

					Dim consoleView As String = String.Empty
					If Not Request.QueryString("CV") Is Nothing Then
						consoleView = Request.QueryString("CV").ToString()
					End If

					If (Not consoleSize = String.Empty AndAlso ConsoleController.GetSizeValues().Contains(consoleSize)) Then
						SaveUserSetting("DefaultSize", consoleSize)
					End If

					If (Not consoleView = String.Empty AndAlso ConsoleController.GetViewValues().Contains(consoleView)) Then
						SaveUserSetting("DefaultView", consoleView)
					End If
				End If
			End If
		End Sub

		Public Function GetUserSetting(ByVal key As String) As Object
			Return Personalization.Personalization.GetProfile(ModuleConfiguration.ModuleDefinition.FriendlyName, PersonalizationKey(key))
		End Function

		Public Sub SaveUserSetting(ByVal key As String, ByVal val As Object)
			Personalization.Personalization.SetProfile(ModuleConfiguration.ModuleDefinition.FriendlyName, PersonalizationKey(key), val)
		End Sub

		Public Function PersonalizationKey(ByVal key As String) As String
			Return String.Format("{0}_{1}_{2}", PortalId.ToString(), TabModuleId.ToString(), key)
		End Function

#End Region

#Region "Properties"
		Private _ConsoleCtrl As ConsoleController
		Public ReadOnly Property ConsoleCtrl() As ConsoleController
			Get
				If (_ConsoleCtrl Is Nothing) Then
					_ConsoleCtrl = New ConsoleController()
				End If
				Return _ConsoleCtrl
			End Get
		End Property

		Private _ConsoleTabID As Integer = Null.NullInteger
		Public ReadOnly Property ConsoleTabID() As Integer
			Get
				If (_ConsoleTabID = Null.NullInteger) Then
					If Settings.ContainsKey("ParentTabID") Then
						_ConsoleTabID = Integer.Parse(Settings("ParentTabID").ToString())
					Else
						_ConsoleTabID = TabId
					End If
				End If
				Return _ConsoleTabID
			End Get
		End Property

		Public Function IsHostTab() As Boolean
			Dim returnValue As Boolean = False

			If ConsoleTabID <> TabId Then
				If Not MyBase.UserInfo Is Nothing And MyBase.UserInfo.IsSuperUser Then
					Dim hostTabs As TabCollection = New TabController().GetTabsByPortal(Null.NullInteger)
					For Each key As Integer In hostTabs.Keys
						If key = ConsoleTabID Then
							returnValue = True
							Exit For
						End If
					Next
				End If
			Else
				returnValue = PortalSettings.ActiveTab.IsSuperTab
			End If

			Return returnValue
		End Function

		Private _AllowSizeChange As Object
		Public ReadOnly Property AllowSizeChange() As Boolean
			Get
				If (_AllowSizeChange Is Nothing) Then
					If Settings.ContainsKey("AllowSizeChange") Then
						Try
							_AllowSizeChange = Boolean.Parse(Settings("AllowSizeChange"))
						Catch ex As Exception
							_AllowSizeChange = True
						End Try
					Else
						_AllowSizeChange = True
					End If
				End If

				Return CBool(_AllowSizeChange)
			End Get
		End Property

		Private _AllowViewChange As Object
		Public ReadOnly Property AllowViewChange() As Boolean
			Get
				If (_AllowViewChange Is Nothing) Then
					If Settings.ContainsKey("AllowViewChange") Then
						Try
							_AllowViewChange = Boolean.Parse(Settings("AllowViewChange"))
						Catch ex As Exception
							_AllowViewChange = True
						End Try
					Else
						_AllowViewChange = True
					End If
				End If

				Return CBool(_AllowViewChange)
			End Get
		End Property

		Private _ShowTooltip As Object
		Public ReadOnly Property ShowTooltip() As Boolean
			Get
				If (_ShowTooltip Is Nothing) Then
					If Settings.ContainsKey("ShowTooltip") Then
						Try
							_ShowTooltip = Boolean.Parse(Settings("ShowTooltip"))
						Catch ex As Exception
							_ShowTooltip = True
						End Try
					Else
						_ShowTooltip = True
					End If
				End If

				Return CBool(_ShowTooltip)
			End Get
		End Property

		Private _DefaultSize As String = String.Empty
		Public ReadOnly Property DefaultSize() As String
			Get
				If (_DefaultSize = String.Empty AndAlso AllowSizeChange AndAlso UserId > Null.NullInteger) Then
					Dim personalizedValue As Object = GetUserSetting("DefaultSize")

					If (Not personalizedValue Is Nothing) Then
						_DefaultSize = CStr(personalizedValue)
					End If
				End If

				If (_DefaultSize = String.Empty) Then
					If Settings.ContainsKey("DefaultSize") Then
						_DefaultSize = CStr(Settings("DefaultSize"))
					Else
						_DefaultSize = "IconFile"
					End If
				End If

				Return _DefaultSize
			End Get
		End Property

		Private _DefaultView As String = String.Empty
		Public ReadOnly Property DefaultView() As String
			Get
				If (_DefaultView = String.Empty AndAlso AllowViewChange AndAlso UserId > Null.NullInteger) Then
					Dim personalizedValue As Object = GetUserSetting("DefaultView")

					If (Not personalizedValue Is Nothing) Then
						_DefaultView = CStr(personalizedValue)
					End If
				End If

				If (_DefaultView = String.Empty) Then
					If Settings.ContainsKey("DefaultView") Then
						_DefaultView = CStr(Settings("DefaultView"))
					Else
						_DefaultView = "Hide"
					End If
				End If

				Return _DefaultView
			End Get
		End Property

		Private _ConsoleWidth As Object = Nothing
		Public ReadOnly Property ConsoleWidth() As String
			Get
				If (_ConsoleWidth = Nothing) Then
					If Settings.ContainsKey("ConsoleWidth") Then
						Try
							_ConsoleWidth = Unit.Parse(Settings("ConsoleWidth")).ToString()
						Catch ex As Exception
							_ConsoleWidth = ""
						End Try
					Else
						_ConsoleWidth = ""
					End If
				End If

				Return CStr(_ConsoleWidth)
			End Get
		End Property

#End Region

	End Class

End Namespace
