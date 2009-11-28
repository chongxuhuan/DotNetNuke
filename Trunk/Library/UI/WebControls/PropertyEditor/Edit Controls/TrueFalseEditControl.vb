'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports System.ComponentModel
Imports System.Reflection
Imports System.Web.UI

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      TrueFalseEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The TrueFalseEditControl control provides a standard UI component for editing 
    ''' true/false (boolean) properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/21/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:TrueFalseEditControl runat=server></{0}:TrueFalseEditControl>")> _
    Public Class TrueFalseEditControl
        Inherits EditControl

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a TrueFalseEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            Me.SystemType = "System.Boolean"
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BooleanValue returns the Boolean representation of the Value
        ''' </summary>
        ''' <value>A Boolean representing the Value</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property BooleanValue() As Boolean
            Get
                Dim boolValue As Boolean = Null.NullBoolean
                Try
                    'Try and cast the value to an Boolean
                    boolValue = CType(Value, Boolean)
                Catch ex As Exception
                End Try
                Return boolValue
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldBooleanValue returns the Boolean representation of the OldValue
        ''' </summary>
        ''' <value>A Boolean representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldBooleanValue() As Boolean
            Get
                Dim boolValue As Boolean = Null.NullBoolean
                Try
                    'Try and cast the value to an Boolean
                    boolValue = CType(OldValue, Boolean)
                Catch ex As Exception
                End Try
                Return boolValue
            End Get
        End Property

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
                Return BooleanValue.ToString
            End Get
            Set(ByVal Value As String)
                Dim setValue As Boolean = Boolean.Parse(Value)
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
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = BooleanValue
            args.OldValue = OldBooleanValue
            args.StringValue = StringValue
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

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If (BooleanValue) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "True")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(Localization.GetString("True", Localization.SharedResourceFile))
            writer.RenderEndTag()

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio")
            If (Not BooleanValue) Then
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked")
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "False")
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()

            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(Localization.GetString("False", Localization.SharedResourceFile))
            writer.RenderEndTag()

        End Sub

#End Region

    End Class

End Namespace

