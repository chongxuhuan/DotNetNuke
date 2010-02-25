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
Imports System.Web
Imports System.Web.UI

Imports DotNetNuke.Security

Namespace DotNetNuke.Common.Controls

	Public Class Form

		Inherits System.Web.UI.HtmlControls.HtmlForm

		Protected Overrides Sub RenderAttributes(ByVal writer As HtmlTextWriter)

            Dim stringWriter As System.IO.StringWriter = New System.IO.StringWriter
            Dim htmlWriter As HtmlTextWriter = New HtmlTextWriter(stringWriter)
            MyBase.RenderAttributes(htmlWriter)
            Dim html As String = stringWriter.ToString()

            ' Locate and replace action attribute
            Dim StartPoint As Integer = html.IndexOf("action=""")
            If StartPoint >= 0 Then 'does action exist?
                Dim EndPoint As Integer = html.IndexOf("""", StartPoint + 8) + 1
                html = html.Remove(StartPoint, EndPoint - StartPoint)
                html = html.Insert(StartPoint, "action=""" & HttpUtility.HtmlEncode(HttpContext.Current.Request.RawUrl) & """")
            End If

            '' Locate and replace id attribute
            If Not (MyBase.ID Is Nothing) Then
                StartPoint = html.IndexOf("id=""")
                If StartPoint >= 0 Then 'does id exist?
                    Dim EndPoint As Integer = html.IndexOf("""", StartPoint + 4) + 1
                    html = html.Remove(StartPoint, EndPoint - StartPoint)
                    html = html.Insert(StartPoint, "id=""" & MyBase.ClientID & """")
                End If
            End If

            writer.Write(html)
        End Sub

    End Class

End Namespace

