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
Imports System.Web.UI.HtmlControls

Namespace DotNetNuke.Web.UI.WebControls

    <ParseChildren(True)> _
    Public Class DnnRibbonBarGroup
        Inherits System.Web.UI.WebControls.WebControl

        Public Sub New()
            MyBase.New("div")
            CssClass = "dnnRibbonGroup"
        End Sub

		Private _contentContainer As HtmlGenericControl = Nothing

        Protected Overloads Overrides Sub CreateChildControls()
            Controls.Clear()

            Dim topLeft As New HtmlGenericControl("div")
            topLeft.Attributes.Add("class", "topLeft")
            Dim topRight As New HtmlGenericControl("div")
            topRight.Attributes.Add("class", "topRight")

            Dim bottomLeft As New HtmlGenericControl("div")
            bottomLeft.Attributes.Add("class", "bottomLeft")
            Dim bottomRight As New HtmlGenericControl("div")
            bottomRight.Attributes.Add("class", "bottomRight")

			_contentContainer = New HtmlGenericControl("div")
			_contentContainer.Attributes.Add("class", "content")

            Dim footerContainer As New HtmlGenericControl("div")
            footerContainer.Attributes.Add("class", "footer")

            Controls.Add(topLeft)
            Controls.Add(topRight)
			Controls.Add(_contentContainer)
            Controls.Add(footerContainer)
            Controls.Add(bottomLeft)
            Controls.Add(bottomRight)

            If Content IsNot Nothing Then
				Content.InstantiateIn(_contentContainer)
            End If

            If Footer IsNot Nothing Then
                Footer.InstantiateIn(footerContainer)
            End If
        End Sub

        Public Overloads Overrides ReadOnly Property Controls() As ControlCollection
            Get
                EnsureChildControls()
                Return MyBase.Controls
            End Get
        End Property

		Private Function CheckVisibility() As Boolean
			Dim returnValue As Boolean = True
			If (Visible AndAlso CheckToolVisibility) Then
				'Hide group if all tools are invisible
				Dim foundTool As Boolean = False
				returnValue = AreChildToolsVisible(_contentContainer.Controls, foundTool)
			End If
			Return returnValue
		End Function

		Private Function AreChildToolsVisible(ByRef children As ControlCollection, ByRef foundTool As Boolean) As Boolean
			Dim returnValue As Boolean = False

			For Each ctrl As Control In children
				If (TypeOf ctrl Is DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool) Then
					foundTool = True
					If (ctrl.Visible) Then
						returnValue = True
						Exit For
					End If
				Else
					If (AreChildToolsVisible(ctrl.Controls, foundTool)) Then
						If (foundTool) Then
							returnValue = True
							Exit For
						End If
					End If
				End If
			Next

			If (Not foundTool) Then
				Return True
			End If

			Return returnValue
		End Function

		Private _Footer As ITemplate
		<TemplateInstance(TemplateInstance.[Single])> _
		Public Overridable Property Footer() As ITemplate
			Get
				Return _Footer
			End Get
			Set(ByVal value As ITemplate)
				_Footer = value
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

		Private _CheckToolVisibility As Boolean = True
		Public Overridable Property CheckToolVisibility() As Boolean
			Get
				Return _CheckToolVisibility
			End Get
			Set(ByVal value As Boolean)
				_CheckToolVisibility = value
			End Set
		End Property

		Public Overloads Overrides Function FindControl(ByVal id As String) As Control
			EnsureChildControls()
			Return MyBase.FindControl(id)
		End Function

		Public Overrides Sub RenderControl(ByVal writer As System.Web.UI.HtmlTextWriter)
			If (CheckVisibility()) Then
				MyBase.RenderBeginTag(writer)
				MyBase.RenderChildren(writer)
				MyBase.RenderEndTag(writer)
			End If
		End Sub

	End Class

End Namespace
