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

Imports Telerik.Web.UI
Imports DotNetNuke.Entities.Tabs
Imports System.Collections.Generic
Imports System.Linq
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Admin.Modules
Imports DotNetNuke.Modules.Admin.Tabs

Namespace DotNetNuke.Modules.Admin.Languages

    Partial Class TranslationStatus
        Inherits DotNetNuke.UI.Modules.ModuleUserControlBase

#Region "Public Properties"

        Protected ReadOnly Property Language() As Locale
            Get
                Return LocaleController.Instance().GetLocale(Request.QueryString("locale"))
            End Get
        End Property

        Protected ReadOnly Property Tabs() As List(Of TabInfo)
            Get
                Dim tabList As New List(Of TabInfo)
                For Each t As TabInfo In TabController.GetPortalTabs(TabController.GetTabsBySortOrder(Me.ModuleContext.PortalId, Language.Code, False), Null.NullInteger, False, "", False, False, False, False, False)
                    Dim newTab As TabInfo = t.Clone()
                    If newTab.ParentId = Null.NullInteger Then newTab.ParentId = 0
                    tabList.Add(newTab)
                Next
                Return tabList
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub LocalizeSelectedItems(ByVal localize As Boolean, ByVal nodes As RadTreeNodeCollection)
            For Each node As RadTreeNode In nodes
                Dim moduleLocalization As ModuleLocalization = DirectCast(node.FindControl("moduleLocalization"), ModuleLocalization)
                If moduleLocalization IsNot Nothing Then
                    moduleLocalization.LocalizeSelectedItems(localize)

                    'Recursively call for child nodes
                    LocalizeSelectedItems(localize, node.Nodes)
                End If
            Next
        End Sub

        Private Sub MarkTranslatedSelectedItems(ByVal translated As Boolean, ByVal nodes As RadTreeNodeCollection)
            For Each node As RadTreeNode In nodes
                Dim moduleLocalization As ModuleLocalization = DirectCast(node.FindControl("moduleLocalization"), ModuleLocalization)
                Dim tabLocalization As TabLocalization = DirectCast(node.FindControl("tabLocalization"), TabLocalization)
                If moduleLocalization IsNot Nothing Then
                    moduleLocalization.MarkTranslatedSelectedItems(translated)
                End If
                If tabLocalization IsNot Nothing Then
                    tabLocalization.MarkTranslatedSelectedItems(translated)
                End If

                'Recursively call for child nodes
                MarkTranslatedSelectedItems(translated, node.Nodes)
            Next
        End Sub

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            pagesTreeView.DataSource = Tabs
            If Not Page.IsPostBack Then
                pagesTreeView.DataBind()
            End If
        End Sub

        Protected Sub delocalizeModuleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles delocalizeModuleButton.Click
            LocalizeSelectedItems(False, pagesTreeView.Nodes)
        End Sub

        Protected Sub localizeModuleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles localizeModuleButton.Click
            LocalizeSelectedItems(True, pagesTreeView.Nodes)
        End Sub

        Protected Sub markModuleTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markModuleTranslatedButton.Click
            MarkTranslatedSelectedItems(True, pagesTreeView.Nodes)
        End Sub

        Protected Sub markModuleUnTranslatedButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles markModuleUnTranslatedButton.Click
            MarkTranslatedSelectedItems(False, pagesTreeView.Nodes)
        End Sub

        Protected Sub pagesTreeView_NodeDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles pagesTreeView.NodeDataBound
            Dim moduleLocalization As ModuleLocalization = DirectCast(e.Node.FindControl("moduleLocalization"), ModuleLocalization)
            Dim tabLocalization As TabLocalization = DirectCast(e.Node.FindControl("tabLocalization"), TabLocalization)
            Dim boundTab As TabInfo = TryCast(e.Node.DataItem, TabInfo)
            If boundTab IsNot Nothing Then
                moduleLocalization.TabId = boundTab.TabID
                tabLocalization.TabId = boundTab.TabID

                If Not Page.IsPostBack Then
                    moduleLocalization.DataBind()
                    tabLocalization.DataBind()
                End If
            End If
        End Sub

    End Class

End Namespace
