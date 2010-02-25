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

Imports System.Data.SqlTypes

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DateEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DateEditControl control provides a standard UI component for editing 
    ''' date properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/10/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DateEditControl runat=server></{0}:DateEditControl>")> _
Public Class DateEditControl
        Inherits EditControl

#Region "Controls"

        Private dateField As TextBox
        Private linkCalendar As HyperLink

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DateValue returns the Date representation of the Value
        ''' </summary>
        ''' <value>A Date representing the Value</value>
        ''' <history>
        '''     [cnurse]	05/22/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property DateValue() As DateTime
            Get
                Dim dteValue As DateTime = Null.NullDate
                Try
                    'Try and cast the value to an DateTime
                    dteValue = CType(Value, DateTime)
                Catch ex As Exception
                End Try
                Return dteValue
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DefaultDateFormat is a string that will be used to format the date in the absence of a 
        ''' FormatAttribute
        ''' </summary>
        ''' <value>A String representing the default format to use to render the date</value>
        ''' <returns>A Format String</returns>
        ''' <history>
        '''     [cnurse]	10/29/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property DefaultFormat() As String
            Get
                Return "d"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Format is a string that will be used to format the date in View mode
        ''' </summary>
        ''' <value>A String representing the format to use to render the date</value>
        ''' <returns>A Format String</returns>
        ''' <history>
        '''     [cnurse]	06/11/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property Format() As String
            Get
                Dim _Format As String = DefaultFormat
                If CustomAttributes IsNot Nothing Then
                    For Each attribute As System.Attribute In CustomAttributes
                        If TypeOf attribute Is FormatAttribute Then
                            Dim formatAtt As FormatAttribute = CType(attribute, FormatAttribute)
                            _Format = formatAtt.Format
                            Exit For
                        End If
                    Next
                End If
                Return _Format
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OldDateValue returns the Date representation of the OldValue
        ''' </summary>
        ''' <value>A Date representing the OldValue</value>
        ''' <history>
        '''     [cnurse]	05/22/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property OldDateValue() As DateTime
            Get
                Dim dteValue As DateTime = Null.NullDate
                Try
                    'Try and cast the value to an DateTime
                    dteValue = CType(OldValue, DateTime)
                Catch ex As Exception
                End Try
                Return dteValue
            End Get
        End Property

        ''' <summary>
        ''' The Value expressed as a String
        ''' </summary>
        Protected Overrides Property StringValue() As String
            Get
                Dim _StringValue As String = Null.NullString
                If (DateValue.ToUniversalTime.Date <> DateTime.Parse("1754/01/01") AndAlso DateValue <> Null.NullDate) Then
                    _StringValue = DateValue.ToString(Format)
                End If
                Return _StringValue
            End Get
            Set(ByVal value As String)
                Me.Value = DateTime.Parse(value)
            End Set
        End Property

#End Region

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            dateField = New TextBox()
            dateField.ControlStyle.CopyFrom(Me.ControlStyle)
            dateField.ID = Me.ID + "date"
            Controls.Add(dateField)

            Controls.Add(New LiteralControl("&nbsp;"))

            linkCalendar = New HyperLink()
            linkCalendar.CssClass = "CommandButton"
            linkCalendar.Text = "<img src=""" + DotNetNuke.Common.Globals.ApplicationPath + "/images/calendar.png"" border=""0"" />&nbsp;&nbsp;" + Localization.GetString("Calendar")
            linkCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(dateField)
            Controls.Add(linkCalendar)
        End Sub

        Protected Overridable Sub LoadDateControls()
            If Not DateValue = Null.NullDate Then
                dateField.Text = DateValue.Date.ToString("d")
            End If
        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean

            Me.EnsureChildControls()
            Return MyBase.LoadPostData(postDataKey + "date", postCollection)

        End Function

        ''' <summary>
        ''' OnDataChanged is called by the PostBack Handler when the Data has changed
        ''' </summary>
        ''' <param name="e">An EventArgs object</param>
        Protected Overrides Sub OnDataChanged(ByVal e As EventArgs)
            Dim args As New PropertyEditorEventArgs(Name)
            args.Value = DateValue
            args.OldValue = OldDateValue
            args.StringValue = StringValue
            MyBase.OnValueChanged(args)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As EventArgs)
            MyBase.OnPreRender(e)

            LoadDateControls()

            If Not Page Is Nothing And Me.EditMode = PropertyEditorMode.Edit Then
                Me.Page.RegisterRequiresPostBack(Me)
            End If
        End Sub

        ''' <summary>
        ''' RenderEditMode is called by the base control to render the control in Edit Mode
        ''' </summary>
        ''' <param name="writer"></param>
        Protected Overrides Sub RenderEditMode(ByVal writer As HtmlTextWriter)
            RenderChildren(writer)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderViewMode renders the View (readonly) mode of the control
        ''' </summary>
        ''' <param name="writer">A HtmlTextWriter.</param>
        ''' <history>
        '''     [cnurse]	06/11/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderViewMode(ByVal writer As System.Web.UI.HtmlTextWriter)
            ControlStyle.AddAttributesToRender(writer)
            writer.RenderBeginTag(HtmlTextWriterTag.Span)
            writer.Write(StringValue)
            writer.RenderEndTag()
        End Sub

    End Class

End Namespace

