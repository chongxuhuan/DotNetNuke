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

Imports System.Web.UI

Namespace DotNetNuke.Web.UI.WebControls

    <ParseChildren(True)> _
    Public Class DnnTab
        Inherits System.Web.UI.WebControls.WebControl
        Public Sub New()
            MyBase.New("div")
        End Sub

        Protected Overloads Overrides Sub CreateChildControls()
            Controls.Clear()

            If Content IsNot Nothing Then
                Content.InstantiateIn(Me)
            End If
        End Sub

        Public Overloads Overrides ReadOnly Property Controls() As ControlCollection
            Get
                EnsureChildControls()
                Return MyBase.Controls
            End Get
        End Property

        Private _Header As ITemplate
        <TemplateInstance(TemplateInstance.[Single])> _
        Public Overridable Property Header() As ITemplate
            Get
                Return _Header
            End Get
            Set(ByVal value As ITemplate)
                _Header = value
            End Set
        End Property

        Private _Content As ITemplate
        <TemplateInstance(TemplateInstance.[Single])> _
        Public Overridable Property Content() As ITemplate
            Get
                Return _Content
            End Get
            Set(ByVal value As ITemplate)
                _Content = value
            End Set
        End Property

        Public Overrides Function FindControl(ByVal id As String) As Control
            EnsureChildControls()
            Return MyBase.FindControl(id)
        End Function

        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            MyBase.RenderBeginTag(writer)
            MyBase.RenderChildren(writer)
            MyBase.RenderEndTag(writer)
        End Sub

    End Class

End Namespace