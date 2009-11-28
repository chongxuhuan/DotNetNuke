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
Imports System
Imports System.Threading
Imports DotNetNuke.ComponentModel
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Scheduling.DNNScheduling

    Public Class DNNScheduler
        Inherits Services.Scheduling.SchedulingProvider

#Region "Constructors"

        Public Sub New()
            If DataProvider.Instance Is Nothing Then
                ' get the provider configuration based on the type
                Dim objProvider As DataProvider = Nothing
                Dim defaultprovider As String = DotNetNuke.Data.DataProvider.Instance.DefaultProviderName
                Dim dataProviderNamespace As String = "DotNetNuke.Services.Scheduling.DNNScheduling"
                If defaultprovider = "SqlDataProvider" Then
                    objProvider = New SqlDataProvider
                Else
                    Dim providerType As String = dataProviderNamespace + "." + defaultprovider
                    objProvider = CType(Framework.Reflection.CreateObject(providerType, providerType, True), DataProvider)
                End If
                ComponentFactory.RegisterComponentInstance(Of DataProvider)(objProvider)
            End If
        End Sub

#End Region

#Region "Private Methods"

        Private Function CanRunOnThisServer(ByVal Servers As String) As Boolean
            Dim lwrServers As String = ""
            If Not lwrServers Is Nothing Then
                lwrServers = Servers.ToLower()
            End If

            If lwrServers.Length = 0 OrElse lwrServers.Contains(Common.Globals.ServerName.ToLower()) Then
                Return True
            Else
                Return False
            End If

        End Function

#End Region

#Region "Public Methods"

        Public Overrides Function AddSchedule(ByVal objScheduleItem As ScheduleItem) As Integer
            'Remove item from queue
            CoreScheduler.RemoveFromScheduleQueue(objScheduleItem)

            'save item
            objScheduleItem.ScheduleID = SchedulingController.AddSchedule(objScheduleItem.TypeFullName, objScheduleItem.TimeLapse, objScheduleItem.TimeLapseMeasurement, objScheduleItem.RetryTimeLapse, objScheduleItem.RetryTimeLapseMeasurement, objScheduleItem.RetainHistoryNum, objScheduleItem.AttachToEvent, objScheduleItem.CatchUpEnabled, objScheduleItem.Enabled, objScheduleItem.ObjectDependencies, objScheduleItem.Servers)

            'Add schedule to queue
            RunScheduleItemNow(objScheduleItem)

            'Return Id
            Return objScheduleItem.ScheduleID
        End Function

        Public Overrides Sub AddScheduleItemSetting(ByVal ScheduleID As Integer, ByVal Name As String, ByVal Value As String)
            SchedulingController.AddScheduleItemSetting(ScheduleID, Name, Value)
        End Sub

        Public Overrides Sub DeleteSchedule(ByVal objScheduleItem As ScheduleItem)
            SchedulingController.DeleteSchedule(objScheduleItem.ScheduleID)
            CoreScheduler.RemoveFromScheduleQueue(objScheduleItem)
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("ScheduleLastPolled")
        End Sub

        Public Overrides Sub ExecuteTasks()
            If Enabled Then
                Dim s As New CoreScheduler(Debug, MaxThreads)
                CoreScheduler.KeepRunning = True
                CoreScheduler.KeepThreadAlive = False
                CoreScheduler.Start()
            End If
        End Sub

        Public Overrides Function GetActiveThreadCount() As Integer
            Return SchedulingController.GetActiveThreadCount
        End Function

        Public Overrides Function GetFreeThreadCount() As Integer
            Return SchedulingController.GetFreeThreadCount
        End Function

        Public Overrides Function GetMaxThreadCount() As Integer
            Return SchedulingController.GetMaxThreadCount
        End Function

        Public Overrides Function GetNextScheduledTask(ByVal Server As String) As ScheduleItem
            Return SchedulingController.GetNextScheduledTask(Server)
        End Function

        Public Overloads Overrides Function GetSchedule() As ArrayList
            Return New ArrayList(SchedulingController.GetSchedule().ToArray())
        End Function

        Public Overloads Overrides Function GetSchedule(ByVal Server As String) As ArrayList
            Return New ArrayList(SchedulingController.GetSchedule(Server).ToArray())
        End Function

        Public Overloads Overrides Function GetSchedule(ByVal ScheduleID As Integer) As ScheduleItem
            Return SchedulingController.GetSchedule(ScheduleID)
        End Function

        Public Overloads Overrides Function GetSchedule(ByVal TypeFullName As String, ByVal Server As String) As ScheduleItem
            Return SchedulingController.GetSchedule(TypeFullName, Server)
        End Function

        Public Overrides Function GetScheduleHistory(ByVal ScheduleID As Integer) As ArrayList
            Return New ArrayList(SchedulingController.GetScheduleHistory(ScheduleID).ToArray())
        End Function

        Public Overrides Function GetScheduleItemSettings(ByVal ScheduleID As Integer) As Hashtable
            Return SchedulingController.GetScheduleItemSettings(ScheduleID)
        End Function

        Public Overrides Function GetScheduleProcessing() As Collection
            Return SchedulingController.GetScheduleProcessing()
        End Function

        Public Overrides Function GetScheduleQueue() As Collection
            Return SchedulingController.GetScheduleQueue()
        End Function

        Public Overrides Function GetScheduleStatus() As ScheduleStatus
            Return SchedulingController.GetScheduleStatus
        End Function

        Public Overrides Sub Halt(ByVal SourceOfHalt As String)
            Dim s As New CoreScheduler(Debug, MaxThreads)
            CoreScheduler.Halt(SourceOfHalt)
            CoreScheduler.KeepRunning = False
        End Sub

        Public Overrides Sub PurgeScheduleHistory()
            Dim s As New CoreScheduler(MaxThreads)
            CoreScheduler.PurgeScheduleHistory()
        End Sub

        Public Overrides Sub ReStart(ByVal SourceOfRestart As String)
            Halt(SourceOfRestart)
            StartAndWaitForResponse()
        End Sub

        Public Overrides Sub RunEventSchedule(ByVal objEventName As Scheduling.EventName)
            If Enabled Then
                Dim s As New CoreScheduler(Debug, MaxThreads)
                CoreScheduler.RunEventSchedule(objEventName)
            End If
        End Sub

        Public Overrides Sub RunScheduleItemNow(ByVal objScheduleItem As ScheduleItem)
            'Remove item from queue
            CoreScheduler.RemoveFromScheduleQueue(objScheduleItem)

            Dim objScheduleHistoryItem As New ScheduleHistoryItem(objScheduleItem)
            objScheduleHistoryItem.NextStart = Now

            If Not objScheduleHistoryItem.TimeLapse = DotNetNuke.Common.Utilities.Null.NullInteger _
             AndAlso Not objScheduleHistoryItem.TimeLapseMeasurement = DotNetNuke.Common.Utilities.Null.NullString _
             AndAlso objScheduleHistoryItem.Enabled _
             AndAlso CanRunOnThisServer(objScheduleItem.Servers) Then
                objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_SCHEDULE_CHANGE
                CoreScheduler.AddToScheduleQueue(objScheduleHistoryItem)
            End If
            DotNetNuke.Common.Utilities.DataCache.RemoveCache("ScheduleLastPolled")
        End Sub

        Public Overrides ReadOnly Property Settings() As Dictionary(Of String, String)
            Get
                Return TryCast(DotNetNuke.ComponentModel.ComponentFactory.GetComponentSettings(Of DNNScheduler)(), Dictionary(Of String, String))
            End Get
        End Property

        Public Overrides Sub Start()
            If Enabled Then
                Dim s As New CoreScheduler(Debug, MaxThreads)
                CoreScheduler.KeepRunning = True
                CoreScheduler.KeepThreadAlive = True
                CoreScheduler.Start()
            End If
        End Sub

        Public Overrides Sub StartAndWaitForResponse()
            If Enabled Then
                Dim newThread As New Threading.Thread(AddressOf Start)
                newThread.IsBackground = True
                newThread.Start()

                'wait for up to 30 seconds for thread
                'to start up
                Dim i As Integer
                For i = 0 To 30
                    If GetScheduleStatus() <> ScheduleStatus.STOPPED Then Exit Sub
                    Thread.Sleep(1000)
                Next
            End If
        End Sub

        Public Overrides Sub UpdateSchedule(ByVal objScheduleItem As ScheduleItem)
            'Remove item from queue
            CoreScheduler.RemoveFromScheduleQueue(objScheduleItem)

            'save item
            SchedulingController.UpdateSchedule(objScheduleItem.ScheduleID, objScheduleItem.TypeFullName, objScheduleItem.TimeLapse, objScheduleItem.TimeLapseMeasurement, objScheduleItem.RetryTimeLapse, objScheduleItem.RetryTimeLapseMeasurement, objScheduleItem.RetainHistoryNum, objScheduleItem.AttachToEvent, objScheduleItem.CatchUpEnabled, objScheduleItem.Enabled, objScheduleItem.ObjectDependencies, objScheduleItem.Servers)

            'Add schedule to queue
            RunScheduleItemNow(objScheduleItem)
        End Sub

#End Region

    End Class


End Namespace
