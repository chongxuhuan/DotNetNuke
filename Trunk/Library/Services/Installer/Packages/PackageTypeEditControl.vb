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

Imports DNNLocalization = DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic

Namespace DotNetNuke.Services.Installer.Packages.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Services.Installer.Packages.WebControls
    ''' Class:      PackageTypeEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageTypeEditControl control provides a standard UI component for editing 
    ''' package types.
    ''' </summary>
    ''' <history>
    '''     [cnurse]	01/25/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:PackageTypeEditControl runat=server></{0}:PackageTypeEditControl>")> _
    Public Class PackageTypeEditControl
        Inherits TextEditControl

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderEditMode renders the Edit mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderEditMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            Dim packageTypes As List(Of PackageType) = PackageController.GetPackageTypes()

            'Render the Select Tag
            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            'Add the Not Specified Option
            writer.AddAttribute(HtmlTextWriterAttribute.Value, Null.NullString)

            If StringValue = Null.NullString Then
                'Add the Selected Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
            End If

            'Render Option Tag
            writer.RenderBeginTag(HtmlTextWriterTag.Option)
            writer.Write("<" & DNNLocalization.GetString("Not_Specified", DNNLocalization.SharedResourceFile) & ">")
            writer.RenderEndTag()

            For Each type As PackageType In packageTypes
                'Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, type.PackageType)

                If type.PackageType = StringValue Then
                    'Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If

                'Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write(type.PackageType)
                writer.RenderEndTag()
            Next

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

#End Region

    End Class

End Namespace

