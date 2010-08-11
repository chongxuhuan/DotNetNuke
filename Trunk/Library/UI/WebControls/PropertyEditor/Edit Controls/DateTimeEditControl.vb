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

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DateTimeEditControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DateTimeEditControl control provides a standard UI component for editing 
    ''' date and time properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/14/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:DateTimeEditControl runat=server></{0}:DateTimeEditControl>")> _
    Public Class DateTimeEditControl
        Inherits DateEditControl

#Region "Private Members"

        Private is24HourClock As Boolean = False

#End Region

#Region "Controls"

        Private hourField As DropDownList
        Private minutesField As DropDownList
        Private ampmField As DropDownList

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DefaultFormat is a string that will be used to format the date in the absence of a 
        ''' FormatAttribute
        ''' </summary>
        ''' <value>A String representing the default format to use to render the date</value>
        ''' <returns>A Format String</returns>
        ''' <history>
        '''     [cnurse]	06/11/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property DefaultFormat() As String
            Get
                Return "g"
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)

            If String.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator) Then
                is24HourClock = True
            End If

        End Sub

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            Controls.Add(New LiteralControl("<br/>"))

            hourField = New DropDownList()
            Dim maxHour As Integer = 12
            Dim minHour As Integer = 1
            If is24HourClock Then
                minHour = 0
                maxHour = 23
            End If
            For i As Integer = minHour To maxHour
                hourField.Items.Add(New ListItem(i.ToString("00"), i.ToString()))
            Next i

            hourField.ControlStyle.CopyFrom(Me.ControlStyle)
            hourField.ID = Me.ID + "hours"
            Controls.Add(hourField)

            Controls.Add(New LiteralControl("&nbsp"))

            minutesField = New DropDownList()
            For i As Integer = 0 To 59
                minutesField.Items.Add(New ListItem(i.ToString("00"), i.ToString()))
            Next i
            minutesField.ControlStyle.CopyFrom(Me.ControlStyle)
            minutesField.ID = Me.ID + "minutes"
            Controls.Add(minutesField)

            If Not is24HourClock Then
                Controls.Add(New LiteralControl("&nbsp"))

                ampmField = New DropDownList()
                ampmField.Items.Add(New ListItem("AM", "AM"))
                ampmField.Items.Add(New ListItem("PM", "PM"))
                ampmField.ControlStyle.CopyFrom(Me.ControlStyle)
                ampmField.ID = Me.ID + "ampm"
                Controls.Add(ampmField)
            End If
        End Sub

        Protected Overrides Sub LoadDateControls()
            MyBase.LoadDateControls()

            Dim hour As Integer = DateValue.Hour
            Dim minute As Integer = DateValue.Minute
            Dim isAM As Boolean = True

            If Not is24HourClock Then
                If hour >= 12 Then
                    hour -= 12
                    isAM = False
                End If
                If hour = 0 Then
                    hour = 12
                End If
            End If

            If hourField.Items.FindByValue(hour.ToString()) IsNot Nothing Then
                hourField.Items.FindByValue(hour.ToString()).Selected = True
            End If

            If minutesField.Items.FindByValue(minute.ToString()) IsNot Nothing Then
                minutesField.Items.FindByValue(minute.ToString()).Selected = True
            End If
            If Not is24HourClock Then
                If isAM Then
                    ampmField.SelectedIndex = 0
                Else
                    ampmField.SelectedIndex = 1
                End If
            End If

        End Sub

        Public Overrides Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean
            Dim dataChanged As Boolean = False
            Dim presentValue As DateTime = OldDateValue
            Dim postedDate As String = postCollection(postDataKey + "date")
            Dim postedHours As String = postCollection(postDataKey + "hours")
            Dim postedMinutes As String = postCollection(postDataKey + "minutes")
            Dim postedAMPM As String = postCollection(postDataKey + "ampm")
            Dim postedValue As DateTime = Null.NullDate
            If Not String.IsNullOrEmpty(postedDate) Then
                postedValue = DateTime.Parse(postedDate)
            End If
            If postedHours <> "12" OrElse is24HourClock Then
                postedValue = postedValue.AddHours(Int32.Parse(postedHours))
            End If
            postedValue = postedValue.AddMinutes(Int32.Parse(postedMinutes))

            If Not is24HourClock AndAlso postedAMPM.Equals("PM") Then
                postedValue = postedValue.AddHours(12)
            End If

            If Not presentValue.Equals(postedValue) Then
                Value = postedValue.ToString(CultureInfo.InvariantCulture)
                dataChanged = True
            End If
            Return dataChanged
        End Function

#End Region

    End Class

End Namespace

