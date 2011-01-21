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

Imports DotNetNuke.UI.WebControls
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnTimeZoneEditControl
        Inherits TextEditControl

        Private TimeZones As DnnTimeZoneComboBox

#Region "Constructors"

        Public Sub New()
        End Sub

#End Region

        Protected Overrides Sub CreateChildControls()
            TimeZones = New DnnTimeZoneComboBox

            Controls.Clear()
            Controls.Add(TimeZones)

            MyBase.CreateChildControls()
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
            Dim dataChanged As Boolean = False
            Dim presentValue As String = StringValue
            Dim postedValue As String = TimeZones.SelectedValue
            If Not presentValue.Equals(postedValue) Then
                Value = postedValue
                dataChanged = True
            End If

            Return dataChanged
        End Function

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Me.EnsureChildControls()
            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            TimeZones.Width = New Unit(450)
            TimeZones.DataBind()
            If TimeZones.FindItemByValue(StringValue) IsNot Nothing Then
                TimeZones.FindItemByValue(StringValue).Selected = True
            End If

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Me.RenderChildren(writer)
        End Sub

        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim propValue As String = Me.Page.Server.HtmlDecode(CType(Me.Value, String))
            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(propValue)
            writer.RenderEndTag()
        End Sub

    End Class

End Namespace
