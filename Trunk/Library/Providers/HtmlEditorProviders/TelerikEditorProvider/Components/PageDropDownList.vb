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

Imports System.Web.UI.WebControls

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Portals
Imports Telerik.Web.UI

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider

    Public Class PageDropDownList
        Inherits Telerik.Web.UI.RadComboBox

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            Dim userInfo As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()
            If (Not Page.IsPostBack AndAlso Not IsNothing(userInfo) AndAlso userInfo.UserID <> Null.NullInteger) Then
                'check view permissions - Yes?
                Dim _PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
                Dim pageCulture As String = _PortalSettings.ActiveTab.CultureCode
                If String.IsNullOrEmpty(pageCulture) Then
                    pageCulture = PortalController.GetActivePortalLanguage(_PortalSettings.PortalId)
                End If

                Dim tabs As List(Of TabInfo) = TabController.GetTabsBySortOrder(_PortalSettings.PortalId, pageCulture, True)
                Dim sortedTabList = TabController.GetPortalTabs(tabs, Null.NullInteger, False, Null.NullString, True, False, True, True, True)

                Items.Clear()
                For Each _tab In sortedTabList
                    Dim tabItem As New RadComboBoxItem(_tab.IndentedTabName, _tab.FullUrl)
                    tabItem.Enabled = Not _tab.DisableLink

                    Items.Add(tabItem)
                Next

                Items.Insert(0, New Telerik.Web.UI.RadComboBoxItem("", ""))
            End If

            Width = Unit.Pixel(245)

        End Sub

    End Class

End Namespace

