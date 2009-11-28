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

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      TextEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The TextEditControl control provides a standard UI component for editing 
    ''' string/text properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/21/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:TextEditControl runat=server></{0}:TextEditControl>")> _
    Public Class TextEditControl
        Inherits EditControl

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a TextEditControl
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a TextEditControl
        ''' </summary>
        ''' <param name="type">The type of the property</param>
        ''' <history>
        '''     [cnurse]	02/22/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal type As String)
            Me.SystemType = type
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldStringValue returns the Boolean representation of the OldValue
        ''' </summary>
        ''' <value>A String representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	06/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldStringValue() As String
            Get
                Return CType(OldValue, String)
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
                Dim strValue As String = Null.NullString
                If Not Value Is Nothing Then
                    strValue = CType(Value, String)
                End If
                Return strValue
            End Get
            Set(ByVal Value As String)
                Me.Value = Value
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
            args.Value = StringValue
            args.OldValue = OldStringValue
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
            Dim length As Integer = Null.NullInteger
            If (Not CustomAttributes Is Nothing) Then
                For Each attribute As System.Attribute In CustomAttributes
                    If TypeOf attribute Is MaxLengthAttribute Then
                        Dim lengthAtt As MaxLengthAttribute = CType(attribute, MaxLengthAttribute)
                        length = lengthAtt.Length
                        Exit For
                    End If
                Next
            End If

            ControlStyle.AddAttributesToRender(writer)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text")
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue)
            If length > Null.NullInteger Then
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString)
            End If
            writer.AddAttribute(HtmlTextWriterAttribute.Name, Me.UniqueID)
            writer.RenderBeginTag(HtmlTextWriterTag.Input)
            writer.RenderEndTag()
        End Sub

#End Region

    End Class

End Namespace

