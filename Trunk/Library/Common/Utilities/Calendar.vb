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

Imports System
Imports System.Configuration
Imports System.Data
Imports System.Globalization

Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Common.Utilities

    Public Class Calendar
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Opens a popup Calendar
        ''' </summary>
        ''' <param name="Field">TextBox to return the date value</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	12/09/2004	Added localized parameter strings: today, close, calendar
        '''                             Use AbbreviatedDayName property instead of first 3 chars of day name
        ''' 	[VMasanas]	14/10/2004	Added support for First Day Of Week
        '''     [VMasanas]  14/11/2004  Register client script to work with FriendlyURLs
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InvokePopupCal(ByVal Field As System.Web.UI.WebControls.TextBox) As String

            ' Define character array to trim from language strings
            Dim TrimChars As Char() = {","c, " "c}

            ' Get culture array of month names and convert to string for
            ' passing to the popup calendar
            Dim MonthNameString As String = ""
            Dim Month As String
            For Each Month In DateTimeFormatInfo.CurrentInfo.MonthNames
                MonthNameString += Month & ","
            Next
            MonthNameString = MonthNameString.TrimEnd(TrimChars)

            ' Get culture array of day names and convert to string for
            ' passing to the popup calendar
            Dim DayNameString As String = ""
            Dim Day As String
            For Each Day In DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames
                DayNameString += Day & ","
            Next
            DayNameString = DayNameString.TrimEnd(TrimChars)

            ' Get the short date pattern for the culture
            Dim FormatString As String = DateTimeFormatInfo.CurrentInfo.ShortDatePattern.ToString
            If Not ClientAPI.IsClientScriptBlockRegistered(Field.Page, "PopupCalendar.js") Then
                ClientAPI.RegisterClientScriptBlock(Field.Page, "PopupCalendar.js", "<script type=""text/javascript"" src=""" & UI.Utilities.ClientAPI.ScriptPath & "PopupCalendar.js""></script>")
            End If

            Dim strToday As String = DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Services.Localization.Localization.GetString("Today"))
            Dim strClose As String = DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Services.Localization.Localization.GetString("Close"))
            Dim strCalendar As String = DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(Services.Localization.Localization.GetString("Calendar"))
            Return "javascript:popupCal('Cal','" & Field.ClientID & "','" & FormatString & "','" & MonthNameString & "','" & DayNameString & "','" & strToday & "','" & strClose & "','" & strCalendar & "'," & DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek & ");"

        End Function



    End Class

End Namespace
