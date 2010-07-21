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
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports System.Linq
Imports Telerik.Web.UI
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.Modules.Admin.Tabs

    Partial Class TabLocalization
        Inherits PortalModuleBase

#Region "Private Members"

        Private _IsSelf As Boolean = Null.NullBoolean
        Private _ShowFooter As Boolean = True
        Private _ShowEditColumn As Boolean = True
        Private _ShowLanguageColumn As Boolean = True
        Private _ShowViewColumn As Boolean = True
        Private _Tab As TabInfo

#End Region

#Region "Contructors"

        Public Sub New()
            TabId = Null.NullInteger
        End Sub

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property Tab() As TabInfo
            Get
                If _Tab Is Nothing Then
                    _Tab = New TabController().GetTab(TabId, PortalSettings.PortalId, False)
                End If
                Return _Tab
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public Property IsSelf As Boolean
            Get
                Return _IsSelf
            End Get
            Set(ByVal value As Boolean)
                _IsSelf = value
            End Set
        End Property

        Public Property ShowEditColumn As Boolean
            Get
                Return _ShowEditColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowEditColumn = value
            End Set
        End Property

        Public Property ShowFooter As Boolean
            Get
                Return _ShowFooter
            End Get
            Set(ByVal value As Boolean)
                _ShowFooter = value
            End Set
        End Property

        Public Property ShowLanguageColumn As Boolean
            Get
                Return _ShowLanguageColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowLanguageColumn = value
            End Set
        End Property

        Public Property ShowViewColumn As Boolean
            Get
                Return _ShowViewColumn
            End Get
            Set(ByVal value As Boolean)
                _ShowViewColumn = value
            End Set
        End Property

        Public Property TabId As Integer
            Get
                Return DirectCast(ViewState("TabId"), Integer)
            End Get
            Set(ByVal value As Integer)
                ViewState("TabId") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Function GetChildModules(ByVal tabId As Integer, ByVal cultureCode As String) As List(Of ModuleInfo)
            Dim modules As New List(Of ModuleInfo)
            Dim tabCtrl As New TabController
            Dim locale As Locale = LocaleController.Instance().GetLocale(cultureCode)
            If locale IsNot Nothing Then
                modules = (From kvp As KeyValuePair(Of Integer, ModuleInfo) In tabCtrl.GetTabByCulture(tabId, PortalSettings.PortalId, locale).ChildModules _
                            Where Not kvp.Value.IsDeleted _
                            Select kvp.Value).ToList()
            End If
            Return modules
        End Function

        Private Function GetLocalizedModulesList(ByVal tabId As Integer, ByVal cultureCode As String) As List(Of ModuleInfo)
            Return (From m As ModuleInfo In GetChildModules(tabId, cultureCode) _
                    Where m.CultureCode = cultureCode _
                    AndAlso m.IsLocalized _
                    Select m).ToList()
        End Function

        Private Function GetSharedModulesList(ByVal tabId As Integer, ByVal cultureCode As String) As List(Of ModuleInfo)
            Return (From m As ModuleInfo In GetChildModules(tabId, cultureCode) _
                    Where m.CultureCode = cultureCode _
                    Select m).ToList()
        End Function

        Private Function GetTranslatedModulesList(ByVal tabId As Integer, ByVal cultureCode As String) As List(Of ModuleInfo)
            Return (From m As ModuleInfo In GetChildModules(tabId, cultureCode) _
                    Where m.CultureCode = cultureCode _
                    AndAlso m.IsTranslated _
                    Select m).ToList()
        End Function

#End Region

#Region "Protected Methods"

        Protected Function CanEdit(ByVal editTabId As Integer, ByVal cultureCode As String) As Boolean
            Dim locale As Locale = LocaleController.Instance.GetLocale(cultureCode)
            Return TabPermissionController.CanManagePage(New TabController().GetTabByCulture(editTabId, PortalSettings.PortalId, locale))
        End Function

        Protected Function CanView(ByVal viewTabId As Integer, ByVal cultureCode As String) As Boolean
            Dim locale As Locale = LocaleController.Instance.GetLocale(cultureCode)
            Return TabPermissionController.CanViewPage(New TabController().GetTabByCulture(viewTabId, PortalSettings.PortalId, locale))
        End Function

        Protected Function GetLocalizedModules(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Return GetLocalizedModulesList(tabId, cultureCode).Count.ToString
        End Function

        Protected Function GetLocalizedStatus(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Dim localizedStatus As Single = 0
            If GetSharedModulesList(tabId, cultureCode).Count > 0 Then
                localizedStatus = GetLocalizedModulesList(tabId, cultureCode).Count / GetSharedModulesList(tabId, cultureCode).Count
            End If
            Return String.Format("{0:#0%}", localizedStatus)
        End Function

        Protected Function GetSharedModules(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Return GetSharedModulesList(tabId, cultureCode).Count.ToString
        End Function

        Protected Function GetTotalModules(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Return GetChildModules(tabId, cultureCode).Count.ToString()
        End Function

        Protected Function GetTranslatedModules(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Return GetTranslatedModulesList(tabId, cultureCode).Count.ToString()
        End Function

        Protected Function GetTranslatedStatus(ByVal tabId As Integer, ByVal cultureCode As String) As String
            Dim translatedStatus As Single = 0
            If GetLocalizedModulesList(tabId, cultureCode).Count > 0 Then
                translatedStatus = GetTranslatedModulesList(tabId, cultureCode).Count / GetLocalizedModulesList(tabId, cultureCode).Count
            End If
            Return String.Format("{0:#0%}", translatedStatus)
        End Function

#End Region

#Region "Public Methods"

        Public Overrides Sub DataBind()
            If TabId <> Null.NullInteger Then
                If IsSelf Then
                    Dim tabs = New List(Of TabInfo)
                    tabs.Add(Tab)
                    localizedTabsGrid.DataSource = tabs
                Else
                    localizedTabsGrid.DataSource = Tab.LocalizedTabs.Values
                End If
            End If
            localizedTabsGrid.DataBind()
        End Sub

        Public Sub MarkTranslatedSelectedItems(ByVal translated As Boolean)
            For Each row As GridDataItem In localizedTabsGrid.SelectedItems
                Dim language As String = DirectCast(row.OwnerTableView.DataKeyValues(row.ItemIndex)("CultureCode"), String)
                Dim tabCtrl As New TabController()
                Dim localizedTab As TabInfo = Nothing
                If Tab.LocalizedTabs.TryGetValue(language, localizedTab) Then
                    tabCtrl.UpdateTranslationStatus(localizedTab, translated)
                End If
            Next

            DataBind()
        End Sub

#End Region

#Region "EventHandlers"

        Protected Sub localizedModulesGrid_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles localizedTabsGrid.PreRender
            For Each column As GridColumn In localizedTabsGrid.Columns
                If (column.UniqueName = "Edit") Then
                    column.Visible = ShowEditColumn
                End If
                If (column.UniqueName = "Language") Then
                    column.Visible = ShowLanguageColumn
                End If
                If (column.UniqueName = "View") Then
                    column.Visible = ShowViewColumn
                End If
            Next
            localizedTabsGrid.Rebind()

            footerPlaceHolder.Visible = ShowFooter
        End Sub

        Protected Sub markTabTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markTabTranslatedButton.Click
            MarkTranslatedSelectedItems(True)
        End Sub

        Protected Sub markTabUnTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markTabUnTranslatedButton.Click
            MarkTranslatedSelectedItems(False)
        End Sub

#End Region

    End Class

End Namespace
