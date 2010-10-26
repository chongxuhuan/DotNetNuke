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

Namespace DotNetNuke.Web.UI.WebControls

    <ParseChildren(True)> _
    Public Class DnnRibbonBar
        Inherits System.Web.UI.WebControls.WebControl

        Public Sub New()
            MyBase.New("div")
			CssClass = "dnnRibbon"
        End Sub

        Protected Overloads Overrides Sub AddParsedSubObject(ByVal obj As Object)
            If TypeOf obj Is DnnRibbonBarGroup Then
                MyBase.AddParsedSubObject(obj)
            Else
                Throw New NotSupportedException("DnnRibbonBarGroupCollection must contain controls of type DnnRibbonBarGroup")
            End If
        End Sub

        Protected Overloads Overrides Function CreateControlCollection() As ControlCollection
            Return New DnnRibbonBarGroupCollection(Me)
        End Function

        Public ReadOnly Property Groups() As DnnRibbonBarGroupCollection
            Get
                Return DirectCast(Controls, DnnRibbonBarGroupCollection)
            End Get
        End Property

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
            Utilities.ApplySkin(Me, "", "RibbonBar", "RibbonBar")
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			If (Groups.Count > 0) Then
				Groups(0).CssClass = Groups(0).CssClass + " " + Groups(0).CssClass.Trim() + "First"
				Groups(Groups.Count - 1).CssClass = Groups(Groups.Count - 1).CssClass + " " + Groups(Groups.Count - 1).CssClass.Trim() + "Last"
			End If

			MyBase.RenderBeginTag(writer)

			writer.AddAttribute("class", "barContent")
			writer.RenderBeginTag("div")

			writer.AddAttribute("cellpadding", "0")
			writer.AddAttribute("cellspacing", "0")
			writer.AddAttribute("border", "0")
			writer.RenderBeginTag("table")
			writer.RenderBeginTag("tr")

			For Each grp As DnnRibbonBarGroup In Groups
				If (grp.Visible) Then
					writer.RenderBeginTag("td")
					grp.RenderControl(writer)
					writer.RenderEndTag()
				End If
			Next
			'MyBase.RenderChildren(writer)

			writer.RenderEndTag() 'tr
			writer.RenderEndTag() 'table
			writer.RenderEndTag() 'div

			writer.AddAttribute("class", "barBottomLeft")
            writer.RenderBeginTag("div")
            writer.RenderEndTag()

			writer.AddAttribute("class", "barBottomRight")
            writer.RenderBeginTag("div")
            writer.RenderEndTag()

            MyBase.RenderEndTag(writer)
        End Sub

    End Class

End Namespace
