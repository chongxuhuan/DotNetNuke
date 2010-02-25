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

Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      EnumEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EnumEditControl control provides a standard UI component for editing 
    ''' enumerated properties.
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/23/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:EnumEditControl runat=server></{0}:EnumEditControl>")> _
    Public Class EnumEditControl
        Inherits EditControl

        Private EnumType As Type

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs an EnumEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs an EnumEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal type As String)
            Me.SystemType = type
            EnumType = System.Type.GetType(type)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StringValue is the value of the control expressed as a String
        ''' </summary>
        ''' <value>A string representing the Value</value>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Property StringValue() As String
            Get
                Dim retValue As Integer = CType(Value, Integer)
                Return retValue.ToString
            End Get
            Set(ByVal Value As String)
                Dim setValue As Integer = Int32.Parse(Value)
                Me.Value = setValue
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataChanged runs when the PostbackData has changed.  It raises the ValueChanged
        ''' Event
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim intValue As Integer = CType(Value, Integer)
            Dim intOldValue As Integer = CType(OldValue, Integer)

            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = System.Enum.ToObject(EnumType, intValue)
            args.OldValue = System.Enum.ToObject(EnumType, intOldValue)

            MyBase.OnValueChanged(args)
        End Sub

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

            Dim propValue As Int32 = Convert.ToInt32(Value)
            Dim enumValues As Array = System.Enum.GetValues(EnumType)

            'Render the Select Tag
            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Select)

            For I As Integer = 0 To enumValues.Length - 1
                Dim enumValue As Integer = Convert.ToInt32(enumValues.GetValue(I))
                Dim enumName As String = System.Enum.GetName(EnumType, enumValue)
                enumName = Localization.GetString(enumName, LocalResourceFile)

                'Add the Value Attribute
                writer.AddAttribute(HtmlTextWriterAttribute.Value, enumValue.ToString)

                If enumValue = propValue Then
                    'Add the Selected Attribute
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected")
                End If

                'Render Option Tag
                writer.RenderBeginTag(HtmlTextWriterTag.Option)
                writer.Write(enumName)
                writer.RenderEndTag()
            Next

            'Close Select Tag
            writer.RenderEndTag()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)

            Dim propValue As Int32 = Convert.ToInt32(Value)
            Dim enumValue As String = System.Enum.Format(EnumType, propValue, "G")

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(enumValue)
            writer.RenderEndTag()

        End Sub

#End Region

    End Class

End Namespace

