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

Imports System.Data
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Host
Imports System.Collections.Generic

Namespace DotNetNuke.Services.Scheduling.DNNScheduling

	Public Class SchedulingController

        <Obsolete("Obsoleted in 5.2.1 - use overload that pass's a FriendlyName")> _
        Public Shared Function AddSchedule(ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String) As Integer
            AddSchedule(TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers, TypeFullName)
        End Function

        Public Shared Function AddSchedule(ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String, ByVal FriendlyName As String) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("TypeFullName", TypeFullName, Entities.Portals.PortalController.GetCurrentPortalSettings, Entities.Users.UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.SCHEDULE_CREATED)
            Return DataProvider.Instance.AddSchedule(TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers, Entities.Users.UserController.GetCurrentUserInfo.UserID, FriendlyName)
        End Function

        Public Shared Function AddScheduleHistory(ByVal objScheduleHistoryItem As ScheduleHistoryItem) As Integer
            Return DataProvider.Instance.AddScheduleHistory(objScheduleHistoryItem.ScheduleID, objScheduleHistoryItem.StartDate, ServerController.GetExecutingServerName())
        End Function

        Public Shared Sub AddScheduleItemSetting(ByVal ScheduleID As Integer, ByVal Name As String, ByVal Value As String)
            DataProvider.Instance.AddScheduleItemSetting(ScheduleID, Name, Value)
        End Sub

        Public Shared Sub DeleteSchedule(ByVal ScheduleID As Integer)
            DataProvider.Instance.DeleteSchedule(ScheduleID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("ScheduleID", ScheduleID.ToString, Entities.Portals.PortalController.GetCurrentPortalSettings, Entities.Users.UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.SCHEDULE_DELETED)
        End Sub

        Public Shared Function GetActiveThreadCount() As Integer
            Return CoreScheduler.GetActiveThreadCount()
        End Function

        Public Shared Function GetFreeThreadCount() As Integer
            Return CoreScheduler.GetFreeThreadCount()
        End Function

        Public Shared Function GetMaxThreadCount() As Integer
            Return CoreScheduler.GetMaxThreadCount
        End Function

        Public Shared Function GetNextScheduledTask(ByVal Server As String) As ScheduleItem
            Return CBO.FillObject(Of ScheduleItem)(DataProvider.Instance.GetNextScheduledTask(Server))
        End Function

        Public Shared Function GetSchedule() As List(Of ScheduleItem)
            Return CBO.FillCollection(Of ScheduleItem)(DataProvider.Instance.GetSchedule())
        End Function

        Public Shared Function GetSchedule(ByVal Server As String) As List(Of ScheduleItem)
            Return CBO.FillCollection(Of ScheduleItem)(DataProvider.Instance.GetSchedule(Server))
        End Function

        Public Shared Function GetSchedule(ByVal TypeFullName As String, ByVal Server As String) As ScheduleItem
            Return CBO.FillObject(Of ScheduleItem)(DataProvider.Instance.GetSchedule(TypeFullName, Server))
        End Function

        Public Shared Function GetSchedule(ByVal ScheduleID As Integer) As ScheduleItem
            Return CBO.FillObject(Of ScheduleItem)(DataProvider.Instance.GetSchedule(ScheduleID))
        End Function

        Public Shared Function GetScheduleByEvent(ByVal EventName As String, ByVal Server As String) As List(Of ScheduleItem)
            Return CBO.FillCollection(Of ScheduleItem)(DataProvider.Instance.GetScheduleByEvent(EventName, Server))
        End Function

        Public Shared Function GetScheduleHistory(ByVal ScheduleID As Integer) As List(Of ScheduleHistoryItem)
            Return CBO.FillCollection(Of ScheduleHistoryItem)(DataProvider.Instance.GetScheduleHistory(ScheduleID))
        End Function

        Public Shared Function GetScheduleItemSettings(ByVal ScheduleID As Integer) As Hashtable
            Dim h As New Hashtable
            Dim r As IDataReader = DataProvider.Instance.GetScheduleItemSettings(ScheduleID)
            While r.Read
                h.Add(r("SettingName"), r("SettingValue"))
            End While

            ' close datareader
            If Not r Is Nothing Then
                r.Close()
            End If

            Return h
        End Function

        Public Shared Function GetScheduleProcessing() As Collection
            Return CoreScheduler.GetScheduleInProgress
        End Function

        Public Shared Function GetScheduleQueue() As Collection
            Return CoreScheduler.GetScheduleQueue
        End Function

        Public Shared Function GetScheduleStatus() As ScheduleStatus
            Return CoreScheduler.GetScheduleStatus
        End Function

        Public Shared Sub PurgeScheduleHistory()
            DataProvider.Instance.PurgeScheduleHistory()
        End Sub

        Public Shared Sub ReloadSchedule()
            CoreScheduler.ReloadSchedule()
        End Sub

        <Obsolete("Obsoleted in 5.2.1 - use overload that pass's a FriendlyName")> _
        Public Shared Sub UpdateSchedule(ByVal ScheduleID As Integer, ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String)
            UpdateSchedule(ScheduleID, TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers, TypeFullName)
        End Sub

        Public Shared Sub UpdateSchedule(ByVal ScheduleID As Integer, ByVal TypeFullName As String, ByVal TimeLapse As Integer, ByVal TimeLapseMeasurement As String, ByVal RetryTimeLapse As Integer, ByVal RetryTimeLapseMeasurement As String, ByVal RetainHistoryNum As Integer, ByVal AttachToEvent As String, ByVal CatchUpEnabled As Boolean, ByVal Enabled As Boolean, ByVal ObjectDependencies As String, ByVal Servers As String, ByVal FriendlyName As String)
            DataProvider.Instance.UpdateSchedule(ScheduleID, TypeFullName, TimeLapse, TimeLapseMeasurement, RetryTimeLapse, RetryTimeLapseMeasurement, RetainHistoryNum, AttachToEvent, CatchUpEnabled, Enabled, ObjectDependencies, Servers, Entities.Users.UserController.GetCurrentUserInfo.UserID, FriendlyName)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("TypeFullName", TypeFullName, Entities.Portals.PortalController.GetCurrentPortalSettings, Entities.Users.UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.SCHEDULE_UPDATED)
        End Sub

        Public Shared Sub UpdateScheduleHistory(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
            DataProvider.Instance.UpdateScheduleHistory(objScheduleHistoryItem.ScheduleHistoryID, objScheduleHistoryItem.EndDate, objScheduleHistoryItem.Succeeded, objScheduleHistoryItem.LogNotes, objScheduleHistoryItem.NextStart)
        End Sub

    End Class

End Namespace
