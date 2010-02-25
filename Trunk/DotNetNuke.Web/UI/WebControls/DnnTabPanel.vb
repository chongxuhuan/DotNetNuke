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
Imports System.Web.UI
Imports Telerik.Web.UI

Namespace DotNetNuke.Web.UI.WebControls

    <ParseChildrenAttribute(True)> _
    Public Class DnnTabPanel
        Inherits System.Web.UI.WebControls.WebControl

        Protected Overloads Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.EnsureChildControls()
        End Sub

        Protected Overloads Overrides Sub CreateChildControls()
            Controls.Clear()

            TelerikTabs.ID = Me.ID + "_Tabs"
            TelerikTabs.Skin = "Office2007"
            TelerikTabs.EnableEmbeddedSkins = True

            TelerikPages.ID = Me.ID + "_Pages"

            TelerikTabs.MultiPageID = TelerikPages.ID

            Controls.Add(TelerikTabs)
            Controls.Add(TelerikPages)
        End Sub

        Private _TelerikTabs As RadTabStrip = Nothing
        Private ReadOnly Property TelerikTabs() As RadTabStrip
            Get
                If _TelerikTabs Is Nothing Then
                    _TelerikTabs = New RadTabStrip()
                End If

                Return _TelerikTabs
            End Get
        End Property

        Private _TelerikPages As RadMultiPage = Nothing
        Private ReadOnly Property TelerikPages() As RadMultiPage
            Get
                If _TelerikPages Is Nothing Then
                    _TelerikPages = New RadMultiPage()
                End If

                Return _TelerikPages
            End Get
        End Property

        Private _Tabs As DnnTabCollection = Nothing
        Public ReadOnly Property Tabs() As DnnTabCollection
            Get
                If _Tabs Is Nothing Then
                    _Tabs = New DnnTabCollection(Me)
                End If

                Return _Tabs
            End Get
        End Property

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            If (Not Page.IsPostBack) Then
                TelerikTabs.SelectedIndex = 0
                TelerikPages.SelectedIndex = 0

                Dim idIndex As Integer = 0

                For Each t As DnnTab In Tabs
                    Dim tab As New RadTab()
                    tab.TabTemplate = t.Header
                    Dim pageView As New RadPageView()
                    pageView.Controls.Add(t)

                    tab.PageViewID = "PV_" + idIndex.ToString()
                    pageView.ID = "PV_" + idIndex.ToString()

                    TelerikTabs.Tabs.Add(tab)
                    TelerikPages.PageViews.Add(pageView)

                    idIndex = idIndex + 1
                Next

            End If

            MyBase.OnPreRender(e)
        End Sub

        Protected Overloads Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)
        End Sub
    End Class

End Namespace
