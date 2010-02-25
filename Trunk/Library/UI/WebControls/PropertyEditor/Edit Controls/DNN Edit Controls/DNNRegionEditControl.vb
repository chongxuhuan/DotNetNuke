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

Imports System.Collections.Specialized

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNRegionEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNRegionEditControl control provides a standard UI component for editing
    ''' Regions
    ''' </summary>
    ''' <history>
    '''     [cnurse]	05/04/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DNNRegionEditControl runat=server></{0}:DNNRegionEditControl>")> _
    Public Class DNNRegionEditControl
        Inherits DNNListEditControl

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a DNNRegionEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            AutoPostBack = False
            TextField = ListBoundField.Text
            ValueField = ListBoundField.Text
        End Sub

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	05/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            If (List Is Nothing) OrElse List.Count = 0 Then
                'No List so use a Text Box
                Dim propValue As String = CType(Me.Value, String)

                ControlStyle.AddAttributesToRender(writer)
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
                writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue)
                writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
                writer.RenderBeginTag(HtmlTextWriterTag.Input)
                writer.RenderEndTag()
            Else
                'Render the standard List
                MyBase.RenderEditMode(writer)
            End If

        End Sub

    End Class

End Namespace

