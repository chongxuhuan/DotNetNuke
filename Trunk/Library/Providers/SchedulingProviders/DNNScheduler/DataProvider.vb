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

Namespace DotNetNuke.Services.Scheduling.DNNScheduling
    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of DataProvider)()
        End Function

#End Region

#Region "Abstract methods"

        Public MustOverride Function AddSchedule(ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String, ByVal CreatedByUserID As Integer, ByVal FriendlyName As String) As Integer
        Public MustOverride Function AddScheduleHistory(ByVal ScheduleID As Integer, ByVal StartDate As Date, ByVal Server As String) As Integer
        Public MustOverride Sub AddScheduleItemSetting(ByVal ScheduleID As Integer, ByVal Name As String, ByVal Value As String)

        Public MustOverride Sub DeleteSchedule(ByVal ScheduleID As Integer)

        Public MustOverride Function GetNextScheduledTask(ByVal Server As String) As IDataReader

        Public MustOverride Function GetSchedule() As IDataReader
        Public MustOverride Function GetSchedule(ByVal Server As String) As IDataReader
        Public MustOverride Function GetSchedule(ByVal ScheduleID As Integer) As IDataReader
        Public MustOverride Function GetSchedule(ByVal TypeFullName As String, ByVal Server As String) As IDataReader

        Public MustOverride Function GetScheduleByEvent(ByVal EventName As String, ByVal Server As String) As IDataReader
        Public MustOverride Function GetScheduleHistory(ByVal ScheduleID As Integer) As IDataReader
        Public MustOverride Function GetScheduleItemSettings(ByVal ScheduleID As Integer) As IDataReader

        Public MustOverride Sub PurgeScheduleHistory()

        Public MustOverride Sub UpdateSchedule(ByVal ScheduleID As Integer, ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String, ByVal LastModifiedByUserID As Integer, ByVal FriendlyName As String)
        Public MustOverride Sub UpdateScheduleHistory(ByVal ScheduleHistoryID As Integer, ByVal EndDate As Date, ByVal Succeeded As Boolean, ByVal LogNotes As String, ByVal NextStart As Date)

#End Region

    End Class

End Namespace
