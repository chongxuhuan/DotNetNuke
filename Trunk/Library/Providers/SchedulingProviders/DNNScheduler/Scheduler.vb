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
Imports System.Threading
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Host
Imports System.Collections.Generic

Namespace DotNetNuke.Services.Scheduling.DNNScheduling

    Module Scheduler

        Public Class CoreScheduler

#Region "Private Shared Members"

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is the heart of the scheduler mechanism.
            'This class manages running new events according
            'to the schedule.
            '
            'This class can also react to the three
            'scheduler events (Started, Progressing and Completed)
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared ThreadPoolInitialized As Boolean = False
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'The MaxThreadCount establishes the maximum
            'threads you want running simultaneously
            'for spawning SchedulerClient processes
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared MaxThreadCount As Integer
            Private Shared ActiveThreadCount As Integer
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'If KeepRunning gets switched to false, 
            'the scheduler stops running.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared ForceReloadSchedule As Boolean = False
            Private Shared Debug As Boolean = False

            Private Shared NumberOfProcessGroups As Integer
            Private Shared _ScheduleQueue As Collection
            Private Shared _ScheduleInProgress As Collection

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'The ScheduleQueue collection contains the current
            'queue of scheduler clients that need to run
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared Property ScheduleQueue() As Collection
                Get
                    Try
                        objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                        Try
                            ' It is safe for this thread to read or write
                            ' from the shared resource.
                            If _ScheduleQueue Is Nothing Then
                                _ScheduleQueue = New Collection
                            End If
                            Interlocked.Increment(Reads)
                        Finally
                            ' Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseReaderLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(ReadTimeout)
                        LogException(ex)
                    End Try
                    Return _ScheduleQueue
                End Get
                Set(ByVal Value As Collection)
                    Try
                        objQueueReadWriteLock.AcquireWriterLock(WriteTimeout)
                        Try
                            ' It is safe for this thread to read or write
                            ' from the shared resource.
                            DataCache.SetCache("ScheduleQueue", Value)
                            _ScheduleQueue = Value
                            Interlocked.Increment(Writes)
                        Finally
                            ' Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(WriterTimeouts)
                        LogException(ex)
                    End Try
                End Set
            End Property

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'The ScheduleInProgress collection contains the 
            'collection of tasks that are currently in progress
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared Property ScheduleInProgress() As Collection
                Get
                    Try
                        objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                        Try
                            ' It is safe for this thread to read or write
                            ' from the shared resource.
                            If _ScheduleInProgress Is Nothing Then
                                _ScheduleInProgress = New Collection
                            End If
                            Interlocked.Increment(Reads)
                        Finally
                            ' Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseReaderLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(ReadTimeout)
                        LogException(ex)
                    End Try
                    Return _ScheduleInProgress
                End Get
                Set(ByVal Value As Collection)
                    Try
                        objQueueReadWriteLock.AcquireWriterLock(WriteTimeout)
                        Try
                            ' It is safe for this thread to read or write
                            ' from the shared resource.
                            DataCache.SetCache("ScheduleQueue", Value)
                            _ScheduleInProgress = Value
                            Interlocked.Increment(Writes)
                        Finally
                            ' Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(WriterTimeouts)
                        LogException(ex)
                    End Try
                End Set
            End Property

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is our array that holds the process group
            'where our threads will be kicked off.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared arrProcessGroup() As ProcessGroup

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'A ReaderWriterLock will protect our objects
            'in memory from being corrupted by simultaneous
            'thread operations.  This block of code below
            'establishes variables to help keep track
            'of the ReaderWriter locks.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared objInProgressReadWriteLock As New ReaderWriterLock
            Private Shared objQueueReadWriteLock As New ReaderWriterLock
            Private Shared ReaderTimeouts As Integer = 0
            Private Shared WriterTimeouts As Integer = 0
            Private Shared Reads As Integer = 0
            Private Shared Writes As Integer = 0
            Private Shared ReadTimeout As Integer = 45000    'wait 45 seconds
            Private Shared WriteTimeout As Integer = 45000    'wait 45 seconds
            Private Shared objStatusReadWriteLock As New ReaderWriterLock
            Private Shared Status As ScheduleStatus = ScheduleStatus.STOPPED

#End Region

#Region "Public Shared Members"

            Public Shared KeepThreadAlive As Boolean = True
            Public Shared KeepRunning As Boolean = True

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'FreeThreads tracks how many threads we have
            'free to work with at any given time.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared ReadOnly Property FreeThreads() As Integer
                Get
                    Return MaxThreadCount - ActiveThreadCount
                End Get
            End Property

#End Region

#Region "Private Shared Methods"

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that adds
            'an item to the collection of schedule items in 
            'progress.  It first obtains a write lock
            'on the ScheduleInProgress object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared Sub AddToScheduleInProgress(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
                If Not ScheduleInProgressContains(objScheduleHistoryItem) Then
                    Try
                        objInProgressReadWriteLock.AcquireWriterLock(WriteTimeout)
                        Try
                            If Not ScheduleInProgressContains(objScheduleHistoryItem) Then
                                ' It is safe for this thread to read or write
                                ' from the shared resource.
                                ScheduleInProgress.Add(objScheduleHistoryItem, objScheduleHistoryItem.ScheduleID.ToString)
                                Interlocked.Increment(Writes)
                            End If
                        Finally
                            ' Ensure that the lock is released.
                            objInProgressReadWriteLock.ReleaseWriterLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(WriterTimeouts)
                        LogException(ex)
                    End Try
                End If
            End Sub

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that clears
            'the collection of schedule items in 
            'queue.  It first obtains a write lock
            'on the ScheduleQueue object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared Sub ClearScheduleQueue()
                Try
                    Dim intCount As Integer = GetScheduleQueueCount()
                    If intCount = 0 Then Exit Sub
                    objQueueReadWriteLock.AcquireWriterLock(WriteTimeout)
                    Try
                        Dim i As Integer
                        For i = 1 To intCount
                            ScheduleQueue.Remove(i)
                        Next
                        Interlocked.Increment(Writes)
                    Finally
                        ' Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseWriterLock()
                    End Try
                Catch ex As ApplicationException
                    ' The writer lock request timed out.
                    Interlocked.Increment(WriterTimeouts)
                    LogException(ex)
                End Try
            End Sub

            Private Shared Function GetProcessGroup() As Integer
                'return a random process group
                Dim r As New Random
                Return r.Next(0, NumberOfProcessGroups - 1)
            End Function

            Private Shared Function IsInProgress(ByVal objScheduleItem As ScheduleItem) As Boolean
                Dim i As Integer
                Dim objReturn As Boolean = False
                Try
                    objInProgressReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        If ScheduleInProgress.Count() > 0 Then
                            For i = 1 To ScheduleInProgress.Count()
                                Dim obj As ScheduleItem
                                obj = CType(ScheduleInProgress(i), ScheduleItem)
                                If obj.ScheduleID = objScheduleItem.ScheduleID Then
                                    objReturn = True
                                End If
                            Next
                        End If
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return objReturn
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that removes
            'an item from the collection of schedule items in 
            'progress.  It first obtains a write lock
            'on the ScheduleInProgress object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Private Shared Sub RemoveFromScheduleInProgress(ByVal objScheduleItem As ScheduleItem)
                Try
                    objInProgressReadWriteLock.AcquireWriterLock(WriteTimeout)
                    Try
                        ' It is safe for this thread to read or write
                        ' from the shared resource.
                        ScheduleInProgress.Remove(objScheduleItem.ScheduleID.ToString)
                        Interlocked.Increment(Writes)
                    Catch exc As System.ArgumentException
                    Finally
                        ' Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseWriterLock()
                    End Try
                Catch ex As ApplicationException
                    ' The writer lock request timed out.
                    Interlocked.Increment(WriterTimeouts)
                    LogException(ex)
                End Try
            End Sub

            Private Shared Function ScheduleInProgressContains(ByVal objScheduleHistoryItem As ScheduleHistoryItem) As Boolean
                Dim i As Integer

                For i = 1 To GetScheduleInProgressCount()
                    Dim objScheduleHistoryItem2 As ScheduleHistoryItem = CType(ScheduleInProgress.Item(i), ScheduleHistoryItem)
                    If objScheduleHistoryItem2.ScheduleID = objScheduleHistoryItem.ScheduleID Then
                        Return True
                    End If
                Next
                Return False
            End Function

            Private Shared Function ScheduleQueueContains(ByVal objScheduleItem As ScheduleItem) As Boolean
                Dim i As Integer

                For i = 1 To GetScheduleQueueCount()
                    Dim objScheduleItem2 As ScheduleItem = CType(ScheduleQueue.Item(i), ScheduleItem)
                    If objScheduleItem2.ScheduleID = objScheduleItem.ScheduleID Then
                        Return True
                    End If
                Next
                Return False
            End Function

#End Region

#Region "Friend Shared Methods"

            Friend Shared Function IsInQueue(ByVal objScheduleItem As ScheduleItem) As Boolean
                Dim i As Integer
                Dim objReturn As Boolean = False
                Try
                    objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        If GetScheduleQueueCount() > 0 Then
                            For i = 1 To GetScheduleQueueCount()
                                Dim obj As ScheduleItem
                                obj = CType(ScheduleQueue(i), ScheduleItem)
                                If obj.ScheduleID = objScheduleItem.ScheduleID Then
                                    objReturn = True
                                End If
                            Next
                        End If
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return objReturn
            End Function

#End Region

#Region "Public Shared Methods"

            Public Shared Function AddScheduleHistory(ByVal objScheduleHistoryItem As ScheduleHistoryItem) As ScheduleHistoryItem
                Try
                    Dim intScheduleHistoryID As Integer
                    intScheduleHistoryID = SchedulingController.AddScheduleHistory(objScheduleHistoryItem)

                    objScheduleHistoryItem.ScheduleHistoryID = intScheduleHistoryID

                Catch exc As Exception
                    ProcessSchedulerException(exc)
                End Try
                Return objScheduleHistoryItem
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that adds
            'an item to the collection of schedule items in 
            'queue.  It first obtains a write lock
            'on the ScheduleQueue object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Sub AddToScheduleQueue(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
                If Not ScheduleQueueContains(objScheduleHistoryItem) Then
                    Try
                        objQueueReadWriteLock.AcquireWriterLock(WriteTimeout)
                        Try
                            'Do a second check just in case
                            If Not ScheduleQueueContains(objScheduleHistoryItem) AndAlso Not IsInProgress(objScheduleHistoryItem) Then
                                ' It is safe for this thread to read or write
                                ' from the shared resource.
                                ScheduleQueue.Add(objScheduleHistoryItem, objScheduleHistoryItem.ScheduleID.ToString)
                                Interlocked.Increment(Writes)
                            End If
                        Finally
                            ' Ensure that the lock is released.
                            objQueueReadWriteLock.ReleaseWriterLock()
                        End Try
                    Catch ex As ApplicationException
                        ' The writer lock request timed out.
                        Interlocked.Increment(WriterTimeouts)
                        LogException(ex)
                    End Try
                End If
            End Sub

            Public Shared Sub FireEvents(ByVal Asynchronous As Boolean)
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'This method uses a thread pool to
                'call the SchedulerClient methods that need
                'to be called.
                '''''''''''''''''''''''''''''''''''''''''''''''''''

                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'For each item in the queue that there
                'is an open thread for, set the object
                'in the array to a new ProcessGroup object.
                'Pass in the ScheduleItem to the ProcessGroup
                'so the ProcessGroup can pass it around for
                'logging and notifications.
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim intScheduleQueueCount As Integer = GetScheduleQueueCount()
                Dim numToRun As Integer = intScheduleQueueCount
                Dim numRun As Integer = 0
                'If numToRun > FreeThreads Then
                '	numToRun = FreeThreads
                'End If

                Dim i As Integer
                For i = 0 To intScheduleQueueCount - 1
                    If KeepRunning = False Then Exit Sub
                    Dim ProcessGroup As Integer = GetProcessGroup()
                    Dim objScheduleItem As ScheduleItem = CType(ScheduleQueue(i + 1), ScheduleItem)

                    If objScheduleItem.NextStart <= Now _
                     AndAlso objScheduleItem.Enabled _
                     AndAlso Not IsInProgress(objScheduleItem) _
                     AndAlso Not HasDependenciesConflict(objScheduleItem) _
                     AndAlso numRun < numToRun Then
                        objScheduleItem.ProcessGroup = ProcessGroup
                        If Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Then
                            objScheduleItem.ScheduleSource = ScheduleSource.STARTED_FROM_TIMER
                        ElseIf Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.REQUEST_METHOD Then
                            objScheduleItem.ScheduleSource = ScheduleSource.STARTED_FROM_BEGIN_REQUEST
                        End If
                        If Asynchronous = True Then
                            arrProcessGroup(ProcessGroup).AddQueueUserWorkItem(objScheduleItem)
                        Else
                            arrProcessGroup(ProcessGroup).RunSingleTask(objScheduleItem)
                        End If
                        If Debug = True Then
                            Dim objEventLog As New EventLogController
                            Dim objEventLogInfo As New LogInfo
                            objEventLogInfo.AddProperty("EVENT ADDED TO PROCESS GROUP " + objScheduleItem.ProcessGroup.ToString, objScheduleItem.TypeFullName)
                            objEventLogInfo.AddProperty("SCHEDULE ID", objScheduleItem.ScheduleID.ToString)
                            objEventLogInfo.LogTypeKey = "DEBUG"
                            objEventLog.AddLog(objEventLogInfo)
                        End If
                        numRun += 1
                    Else
                        If Debug = True Then
                            Dim appended As Boolean = False
                            Dim strDebug As New System.Text.StringBuilder("Task not run because ")
                            If Not objScheduleItem.NextStart <= Now Then
                                strDebug.Append(" task is scheduled for " + objScheduleItem.NextStart.ToString)
                                appended = True
                            End If
                            'If Not (objScheduleItem.NextStart <> Null.NullDate And objScheduleItem.ScheduleSource <> ScheduleSource.STARTED_FROM_EVENT) Then
                            '    If appended Then strDebug.Append(" and")
                            '    strDebug.Append(" task's NextStart <> NullDate and it's wasn't started from an EVENT")
                            '    appended = True
                            'End If
                            If Not objScheduleItem.Enabled Then
                                If appended Then strDebug.Append(" and")
                                strDebug.Append(" task is not enabled")
                                appended = True
                            End If
                            If IsInProgress(objScheduleItem) Then
                                If appended Then strDebug.Append(" and")
                                strDebug.Append(" task is already in progress")
                                appended = True
                            End If
                            If HasDependenciesConflict(objScheduleItem) Then
                                If appended Then strDebug.Append(" and")
                                strDebug.Append(" task has conflicting dependency")
                                appended = True
                            End If
                            Dim objEventLog As New EventLogController
                            Dim objEventLogInfo As New LogInfo
                            objEventLogInfo.AddProperty("EVENT NOT RUN REASON", strDebug.ToString)
                            objEventLogInfo.AddProperty("SCHEDULE ID", objScheduleItem.ScheduleID.ToString)
                            objEventLogInfo.AddProperty("TYPE FULL NAME", objScheduleItem.TypeFullName)
                            objEventLogInfo.LogTypeKey = "DEBUG"
                            objEventLog.AddLog(objEventLogInfo)
                        End If
                    End If
                Next
            End Sub

            Public Shared Function GetActiveThreadCount() As Integer
                Return ActiveThreadCount
            End Function

            Public Shared Function GetFreeThreadCount() As Integer
                Return FreeThreads
            End Function

            Public Shared Function GetMaxThreadCount() As Integer
                Return MaxThreadCount
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that returns
            'the number of items in the collection of 
            'schedule items in progress.  It first obtains a
            'read lock on the ScheduleProgress object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Function GetScheduleInProgress() As Collection
                Dim c As Collection = Nothing
                Try
                    objInProgressReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        c = ScheduleInProgress
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return c
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that returns
            'the number of items in the collection of 
            'schedule items in progress.  It first obtains a
            'read lock on the ScheduleProgress object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Function GetScheduleInProgressCount() As Integer
                Dim intCount As Integer
                Try
                    objInProgressReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        intCount = ScheduleInProgress.Count
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return intCount
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that returns
            'the number of items in the collection of 
            'schedule items in queue.  It first obtains a
            'read lock on the ScheduleQueue object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Function GetScheduleQueue() As Collection
                Dim c As Collection = Nothing
                Try
                    objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        c = ScheduleQueue
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return c
            End Function

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that returns
            'the number of items in the collection of 
            'schedule items in queue.  It first obtains a
            'read lock on the ScheduleQueue object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Function GetScheduleQueueCount() As Integer
                Dim intCount As Integer
                Try
                    objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        intCount = ScheduleQueue.Count
                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return intCount
            End Function

            Public Shared Function GetScheduleStatus() As ScheduleStatus
                Dim s As ScheduleStatus
                Try
                    objStatusReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        s = Status
                    Finally
                        ' Ensure that the lock is released.
                        objStatusReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try
                Return s
            End Function

            Public Shared Sub Halt(ByVal SourceOfHalt As String)
                If Services.Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Then
                    Dim objEventLog As New EventLogController
                    Dim objEventLogInfo As New LogInfo
                    SetScheduleStatus(ScheduleStatus.SHUTTING_DOWN)
                    objEventLogInfo = New LogInfo
                    objEventLogInfo.AddProperty("Initiator", SourceOfHalt)
                    objEventLogInfo.LogTypeKey = "SCHEDULER_SHUTTING_DOWN"
                    objEventLog.AddLog(objEventLogInfo)

                    KeepRunning = False

                    'wait for up to 120 seconds for thread
                    'to shut down
                    Dim i As Integer
                    For i = 0 To 120
                        If GetScheduleStatus() = ScheduleStatus.STOPPED Then Exit Sub
                        Thread.Sleep(1000)
                    Next
                End If
                ActiveThreadCount = 0
            End Sub

            Public Shared Function HasDependenciesConflict(ByVal objScheduleItem As ScheduleItem) As Boolean
                Dim i As Integer
                Dim objReturn As Boolean = False
                Try
                    objInProgressReadWriteLock.AcquireReaderLock(ReadTimeout)
                    Try
                        ' It is safe for this thread to read from
                        ' the shared resource.
                        If Not ScheduleInProgress Is Nothing And objScheduleItem.ObjectDependencies.Length > 0 Then
                            For i = 1 To ScheduleInProgress.Count()
                                Dim obj As ScheduleItem
                                obj = CType(ScheduleInProgress(i), ScheduleItem)
                                If obj.ObjectDependencies.Length > 0 Then
                                    If obj.HasObjectDependencies(objScheduleItem.ObjectDependencies) Then
                                        objReturn = True
                                    End If
                                End If
                            Next
                        End If

                        Interlocked.Increment(Reads)
                    Finally
                        ' Ensure that the lock is released.
                        objInProgressReadWriteLock.ReleaseReaderLock()
                    End Try
                Catch ex As ApplicationException
                    ' The reader lock request timed out.
                    Interlocked.Increment(ReaderTimeouts)
                End Try

                Return objReturn
            End Function

            Public Shared Sub LoadQueueFromEvent(ByVal EventName As EventName)
                Dim schedule As List(Of ScheduleItem) = SchedulingController.GetScheduleByEvent(EventName.ToString, ServerController.GetExecutingServerName())

                For i As Integer = 0 To schedule.Count - 1
                    Dim objScheduleHistoryItem As New ScheduleHistoryItem(schedule(i))

                    If Not IsInQueue(objScheduleHistoryItem) _
                     AndAlso Not IsInProgress(objScheduleHistoryItem) _
                     AndAlso Not HasDependenciesConflict(objScheduleHistoryItem) _
                     AndAlso objScheduleHistoryItem.Enabled Then
                        objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_EVENT
                        AddToScheduleQueue(objScheduleHistoryItem)
                    End If
                Next

            End Sub

            Public Shared Sub LoadQueueFromTimer()
                ForceReloadSchedule = False

                Dim schedule As List(Of ScheduleItem) = SchedulingController.GetSchedule(ServerController.GetExecutingServerName())

                For i As Integer = 0 To schedule.Count - 1
                    Dim objScheduleHistoryItem As New ScheduleHistoryItem(schedule(i))

                    If Not IsInQueue(objScheduleHistoryItem) _
                     AndAlso Not objScheduleHistoryItem.TimeLapse = Null.NullInteger _
                     AndAlso Not objScheduleHistoryItem.TimeLapseMeasurement = Null.NullString _
                     AndAlso objScheduleHistoryItem.Enabled Then
                        If Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Then
                            objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_TIMER
                        ElseIf Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.REQUEST_METHOD Then
                            objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_BEGIN_REQUEST
                        End If
                        AddToScheduleQueue(objScheduleHistoryItem)
                    End If
                Next

            End Sub

            Public Shared Sub PurgeScheduleHistory()
                SchedulingController.PurgeScheduleHistory()
            End Sub

            Public Shared Sub ReloadSchedule()
                ForceReloadSchedule = True
            End Sub

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a multi-thread safe method that removes
            'an item from the collection of schedule items in 
            'queue.  It first obtains a write lock
            'on the ScheduleQueue object.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Public Shared Sub RemoveFromScheduleQueue(ByVal objScheduleItem As ScheduleItem)
                Try
                    objQueueReadWriteLock.AcquireWriterLock(WriteTimeout)
                    Try
                        ' It is safe for this thread to read or write
                        ' from the shared resource.
                        ScheduleQueue.Remove(objScheduleItem.ScheduleID.ToString)
                        Interlocked.Increment(Writes)
                    Catch exc As System.ArgumentException
                    Finally
                        ' Ensure that the lock is released.
                        objQueueReadWriteLock.ReleaseWriterLock()
                    End Try
                Catch ex As ApplicationException
                    ' The writer lock request timed out.
                    Interlocked.Increment(WriterTimeouts)
                    LogException(ex)
                End Try

            End Sub

            Public Shared Sub RunEventSchedule(ByVal EventName As EventName)
                Try
                    Dim objEventLog As New EventLogController
                    Dim objEventLogInfo As New LogInfo
                    objEventLogInfo.AddProperty("EVENT", EventName.ToString)
                    objEventLogInfo.LogTypeKey = "SCHEDULE_FIRED_FROM_EVENT"
                    objEventLog.AddLog(objEventLogInfo)

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'We allow for three threads to run simultaneously.
                    'As long as we have an open thread, continue.
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Load the queue to determine which schedule
                    'items need to be run. 
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    LoadQueueFromEvent(EventName)

                    While GetScheduleQueueCount() > 0
                        SetScheduleStatus(ScheduleStatus.RUNNING_EVENT_SCHEDULE)

                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Fire off the events that need running.
                        '''''''''''''''''''''''''''''''''''''''''''''''''''

                        Try
                            objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                            Try
                                ' It is safe for this thread to read from
                                ' the shared resource.
                                If GetScheduleQueueCount() > 0 Then
                                    'FireEvents(False)
                                    FireEvents(True)
                                End If
                                Interlocked.Increment(Reads)
                            Finally
                                ' Ensure that the lock is released.
                                objQueueReadWriteLock.ReleaseReaderLock()
                            End Try
                        Catch ex As ApplicationException
                            ' The reader lock request timed out.
                            Interlocked.Increment(ReaderTimeouts)
                        End Try


                        If WriterTimeouts > 20 Or ReaderTimeouts > 20 Then
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Wait for 10 minutes so we don't fill up the logs
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep(600000)       'sleep for 10 seconds
                        Else
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Wait for 10 seconds to avoid cpu overutilization
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep(10000)       'sleep for 10 seconds
                        End If

                        If GetScheduleQueueCount() = 0 Then Exit Sub

                    End While

                Catch exc As Exception
                    ProcessSchedulerException(exc)

                End Try

            End Sub

            Public Shared Sub SetScheduleStatus(ByVal objScheduleStatus As ScheduleStatus)
                Try
                    objStatusReadWriteLock.AcquireWriterLock(WriteTimeout)
                    Try
                        ' It is safe for this thread to read or write
                        ' from the shared resource.
                        Status = objScheduleStatus
                        Interlocked.Increment(Writes)
                    Finally
                        ' Ensure that the lock is released.
                        objStatusReadWriteLock.ReleaseWriterLock()
                    End Try
                Catch ex As ApplicationException
                    ' The writer lock request timed out.
                    Interlocked.Increment(WriterTimeouts)
                    LogException(ex)
                End Try

            End Sub

            Public Shared Sub Start()
                Try
                    ActiveThreadCount = 0

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'This is where the action begins.
                    'Loop until KeepRunning = false
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    If Services.Scheduling.SchedulingProvider.SchedulerMode <> SchedulerMode.REQUEST_METHOD Or Debug = True Then
                        Dim objEventLog As New EventLogController
                        Dim objEventLogInfo As New LogInfo
                        objEventLogInfo.LogTypeKey = "SCHEDULER_STARTED"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                    While KeepRunning = True
                        Try
                            If Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Then
                                SetScheduleStatus(ScheduleStatus.RUNNING_TIMER_SCHEDULE)
                            Else
                                SetScheduleStatus(ScheduleStatus.RUNNING_REQUEST_SCHEDULE)
                            End If
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Load the queue to determine which schedule
                            'items need to be run. 
                            '''''''''''''''''''''''''''''''''''''''''''''''''''

                            LoadQueueFromTimer()

                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'Keep track of when the queue was last refreshed
                            'so we can perform a refresh periodically
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            Dim LastQueueRefresh As Date = Now

                            Dim RefreshQueueSchedule As Boolean = False

                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'We allow for [MaxThreadCount] threads to run 
                            'simultaneously.  As long as we have an open thread
                            'and we don't have to refresh the queue, continue
                            'to loop.
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            While FreeThreads > 0 And RefreshQueueSchedule = False And KeepRunning = True And ForceReloadSchedule = False
                                '''''''''''''''''''''''''''''''''''''''''''''''''''
                                'Fire off the events that need running.
                                '''''''''''''''''''''''''''''''''''''''''''''''''''

                                Try
                                    If Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Then
                                        SetScheduleStatus(ScheduleStatus.RUNNING_TIMER_SCHEDULE)
                                    Else
                                        SetScheduleStatus(ScheduleStatus.RUNNING_REQUEST_SCHEDULE)
                                    End If
                                    objQueueReadWriteLock.AcquireReaderLock(ReadTimeout)
                                    Try
                                        ' It is safe for this thread to read from
                                        ' the shared resource.
                                        If GetScheduleQueueCount() > 0 Then
                                            FireEvents(True)
                                        End If
                                        If KeepThreadAlive = False Then
                                            Exit Sub
                                        End If
                                        Interlocked.Increment(Reads)
                                    Finally
                                        ' Ensure that the lock is released.
                                        objQueueReadWriteLock.ReleaseReaderLock()
                                    End Try
                                Catch ex As ApplicationException
                                    ' The reader lock request timed out.
                                    Interlocked.Increment(ReaderTimeouts)
                                End Try


                                If WriterTimeouts > 20 Or ReaderTimeouts > 20 Then
                                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                                    'Some kind of deadlock on a resource.
                                    'Wait for 10 minutes so we don't fill up the logs
                                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                                    If KeepRunning = True Then
                                        Thread.Sleep(600000)             'sleep for 10 minutes
                                    Else
                                        Exit Sub
                                    End If
                                Else
                                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                                    'Wait for 10 seconds to avoid cpu overutilization
                                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                                    If KeepRunning = True Then
                                        Thread.Sleep(10000)             'sleep for 10 seconds
                                    Else
                                        Exit Sub
                                    End If

                                    'Refresh queue from database every 10 minutes
                                    'if there are no items currently in progress
                                    If (LastQueueRefresh.AddMinutes(10) <= Now Or ForceReloadSchedule) And FreeThreads = MaxThreadCount Then
                                        RefreshQueueSchedule = True
                                        Exit While
                                    End If
                                End If

                            End While
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'There are no available threads, all threads are being
                            'used.  Wait 10 seconds until one is available
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            If KeepRunning = True Then
                                If RefreshQueueSchedule = False Then
                                    SetScheduleStatus(ScheduleStatus.WAITING_FOR_OPEN_THREAD)
                                    Thread.Sleep(10000)          'sleep for 10 seconds
                                End If
                            Else
                                Exit Sub
                            End If
                        Catch exc As Exception
                            ProcessSchedulerException(exc)
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            'sleep for 10 minutes
                            '''''''''''''''''''''''''''''''''''''''''''''''''''
                            Thread.Sleep(600000)
                        End Try
                    End While
                Finally
                    If Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.TIMER_METHOD Or Scheduling.SchedulingProvider.SchedulerMode = SchedulerMode.DISABLED Then
                        SetScheduleStatus(ScheduleStatus.STOPPED)
                    Else
                        SetScheduleStatus(ScheduleStatus.WAITING_FOR_REQUEST)
                    End If
                    If Services.Scheduling.SchedulingProvider.SchedulerMode <> SchedulerMode.REQUEST_METHOD Or Debug = True Then
                        Dim objEventLog As New EventLogController
                        Dim objEventLogInfo As New LogInfo
                        objEventLogInfo.LogTypeKey = "SCHEDULER_STOPPED"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                End Try
            End Sub

            Public Shared Sub UpdateScheduleHistory(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
                Try
                    SchedulingController.UpdateScheduleHistory(objScheduleHistoryItem)
                Catch exc As Exception
                    ProcessSchedulerException(exc)
                End Try
            End Sub

            Public Shared Sub WorkCompleted(ByRef objSchedulerClient As SchedulerClient)
                Try

                    Dim objScheduleHistoryItem As ScheduleHistoryItem
                    objScheduleHistoryItem = objSchedulerClient.ScheduleHistoryItem

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Remove the object in the ScheduleInProgress collection
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleInProgress(objScheduleHistoryItem)

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'A SchedulerClient is notifying us that their
                    'process has completed.  Decrease our ActiveThreadCount
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Decrement(ActiveThreadCount)

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Update the schedule item object property
                    'to note the end time and next start
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    objScheduleHistoryItem.EndDate = Now

                    If objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_EVENT Then
                        objScheduleHistoryItem.NextStart = Null.NullDate
                    Else
                        If objScheduleHistoryItem.CatchUpEnabled = True Then
                            Select Case objScheduleHistoryItem.TimeLapseMeasurement
                                Case "s"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddSeconds(objScheduleHistoryItem.TimeLapse)
                                Case "m"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddMinutes(objScheduleHistoryItem.TimeLapse)
                                Case "h"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddHours(objScheduleHistoryItem.TimeLapse)
                                Case "d"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.NextStart.AddDays(objScheduleHistoryItem.TimeLapse)
                            End Select
                        Else
                            Select Case objScheduleHistoryItem.TimeLapseMeasurement
                                Case "s"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddSeconds(objScheduleHistoryItem.TimeLapse)
                                Case "m"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddMinutes(objScheduleHistoryItem.TimeLapse)
                                Case "h"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddHours(objScheduleHistoryItem.TimeLapse)
                                Case "d"
                                    objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddDays(objScheduleHistoryItem.TimeLapse)
                            End Select
                        End If
                    End If

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Update the ScheduleHistory in the database
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    CoreScheduler.UpdateScheduleHistory(objScheduleHistoryItem)
                    Dim objEventLogInfo As New LogInfo

                    If objScheduleHistoryItem.NextStart <> Null.NullDate Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Put the object back into the ScheduleQueue
                        'collection with the new NextStart date.
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        objScheduleHistoryItem.StartDate = Null.NullDate
                        objScheduleHistoryItem.EndDate = Null.NullDate
                        objScheduleHistoryItem.LogNotes = ""
                        objScheduleHistoryItem.ProcessGroup = -1
                        AddToScheduleQueue(objScheduleHistoryItem)
                    End If


                    If objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Write out the log entry for this event
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        Dim objEventLog As New EventLogController

                        objEventLogInfo.AddProperty("TYPE", objSchedulerClient.GetType().FullName)
                        objEventLogInfo.AddProperty("THREAD ID", Thread.CurrentThread.GetHashCode().ToString)
                        objEventLogInfo.AddProperty("NEXT START", Convert.ToString(objScheduleHistoryItem.NextStart))
                        objEventLogInfo.AddProperty("SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString)
                        objEventLogInfo.AddProperty("ACTIVE THREADS", ActiveThreadCount.ToString)
                        objEventLogInfo.AddProperty("FREE THREADS", FreeThreads.ToString)
                        objEventLogInfo.AddProperty("READER TIMEOUTS", ReaderTimeouts.ToString)
                        objEventLogInfo.AddProperty("WRITER TIMEOUTS", WriterTimeouts.ToString)
                        objEventLogInfo.AddProperty("IN PROGRESS", GetScheduleInProgressCount().ToString)
                        objEventLogInfo.AddProperty("IN QUEUE", GetScheduleQueueCount().ToString)
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_COMPLETED"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                Catch exc As Exception
                    ProcessSchedulerException(exc)
                End Try
            End Sub

            Public Shared Sub WorkErrored(ByRef objSchedulerClient As SchedulerClient, ByRef objException As Exception)
                Try

                    Dim objScheduleHistoryItem As ScheduleHistoryItem
                    objScheduleHistoryItem = objSchedulerClient.ScheduleHistoryItem
                    ''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Remove the object in the ScheduleInProgress collection
                    ''''''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleInProgress(objScheduleHistoryItem)

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'A SchedulerClient is notifying us that their
                    'process has errored.  Decrease our ActiveThreadCount
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Decrement(ActiveThreadCount)


                    ProcessSchedulerException(objException)

                    ''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Update the schedule item object property
                    'to note the end time and next start
                    ''''''''''''''''''''''''''''''''''''''''''''''''''
                    objScheduleHistoryItem.EndDate = Now
                    If objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_EVENT Then
                        objScheduleHistoryItem.NextStart = Null.NullDate
                    ElseIf objScheduleHistoryItem.RetryTimeLapse <> Null.NullInteger Then

                        Select Case objScheduleHistoryItem.RetryTimeLapseMeasurement
                            Case "s"
                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddSeconds(objScheduleHistoryItem.RetryTimeLapse)
                            Case "m"
                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddMinutes(objScheduleHistoryItem.RetryTimeLapse)
                            Case "h"
                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddHours(objScheduleHistoryItem.RetryTimeLapse)
                            Case "d"
                                objScheduleHistoryItem.NextStart = objScheduleHistoryItem.StartDate.AddDays(objScheduleHistoryItem.RetryTimeLapse)
                        End Select

                    End If
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Update the ScheduleHistory in the database
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    CoreScheduler.UpdateScheduleHistory(objScheduleHistoryItem)

                    If objScheduleHistoryItem.NextStart <> Null.NullDate _
                    AndAlso objScheduleHistoryItem.RetryTimeLapse <> Null.NullInteger Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Put the object back into the ScheduleQueue
                        'collection with the new NextStart date.
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        objScheduleHistoryItem.StartDate = Null.NullDate
                        objScheduleHistoryItem.EndDate = Null.NullDate
                        objScheduleHistoryItem.LogNotes = ""
                        objScheduleHistoryItem.ProcessGroup = -1
                        AddToScheduleQueue(objScheduleHistoryItem)
                    End If

                    If objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Write out the log entry for this event
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        Dim objEventLog As New EventLogController
                        Dim objEventLogInfo As New LogInfo
                        objEventLogInfo.AddProperty("THREAD ID", Thread.CurrentThread.GetHashCode().ToString)
                        objEventLogInfo.AddProperty("TYPE", objSchedulerClient.GetType().FullName)
                        If Not objException Is Nothing Then
                            objEventLogInfo.AddProperty("EXCEPTION", objException.Message)
                        End If
                        objEventLogInfo.AddProperty("RESCHEDULED FOR", Convert.ToString(objScheduleHistoryItem.NextStart))
                        objEventLogInfo.AddProperty("SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString)
                        objEventLogInfo.AddProperty("ACTIVE THREADS", ActiveThreadCount.ToString)
                        objEventLogInfo.AddProperty("FREE THREADS", FreeThreads.ToString)
                        objEventLogInfo.AddProperty("READER TIMEOUTS", ReaderTimeouts.ToString)
                        objEventLogInfo.AddProperty("WRITER TIMEOUTS", WriterTimeouts.ToString)
                        objEventLogInfo.AddProperty("IN PROGRESS", GetScheduleInProgressCount().ToString)
                        objEventLogInfo.AddProperty("IN QUEUE", GetScheduleQueueCount().ToString)
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_FAILURE"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                Catch exc As Exception
                    ProcessSchedulerException(exc)
                End Try
            End Sub

            Public Shared Sub WorkProgressing(ByRef objSchedulerClient As SchedulerClient)
                Try
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'A SchedulerClient is notifying us that their
                    'process is in progress.  Informational only.
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    If objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Write out the log entry for this event
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        Dim objEventLog As New EventLogController
                        Dim objEventLogInfo As New LogInfo
                        objEventLogInfo.AddProperty("THREAD ID", Thread.CurrentThread.GetHashCode().ToString)
                        objEventLogInfo.AddProperty("TYPE", objSchedulerClient.GetType().FullName)
                        objEventLogInfo.AddProperty("SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString)
                        objEventLogInfo.AddProperty("ACTIVE THREADS", ActiveThreadCount.ToString)
                        objEventLogInfo.AddProperty("FREE THREADS", FreeThreads.ToString)
                        objEventLogInfo.AddProperty("READER TIMEOUTS", ReaderTimeouts.ToString)
                        objEventLogInfo.AddProperty("WRITER TIMEOUTS", WriterTimeouts.ToString)
                        objEventLogInfo.AddProperty("IN PROGRESS", GetScheduleInProgressCount().ToString)
                        objEventLogInfo.AddProperty("IN QUEUE", GetScheduleQueueCount().ToString)
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_PROGRESSING"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                Catch exc As Exception
                    ProcessSchedulerException(exc)
                End Try
            End Sub

            Public Shared Sub WorkStarted(ByRef objSchedulerClient As SchedulerClient)
                Dim ActiveThreadCountIncremented As Boolean = False
                Try

                    objSchedulerClient.ScheduleHistoryItem.ThreadID = Thread.CurrentThread.GetHashCode()

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Put the object in the ScheduleInProgress collection
                    'and remove it from the ScheduleQueue
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    RemoveFromScheduleQueue(objSchedulerClient.ScheduleHistoryItem)
                    AddToScheduleInProgress(objSchedulerClient.ScheduleHistoryItem)

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'A SchedulerClient is notifying us that their
                    'process has started.  Increase our ActiveThreadCount
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    Interlocked.Increment(ActiveThreadCount)
                    ActiveThreadCountIncremented = True

                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Update the schedule item
                    'object property to note the start time.
                    '''''''''''''''''''''''''''''''''''''''''''''''''''
                    objSchedulerClient.ScheduleHistoryItem.StartDate = Now
                    CoreScheduler.AddScheduleHistory(objSchedulerClient.ScheduleHistoryItem)


                    If objSchedulerClient.ScheduleHistoryItem.RetainHistoryNum > 0 Then
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        'Write out the log entry for this event
                        '''''''''''''''''''''''''''''''''''''''''''''''''''
                        Dim objEventLog As New EventLogController
                        Dim objEventLogInfo As New LogInfo
                        objEventLogInfo.AddProperty("THREAD ID", Thread.CurrentThread.GetHashCode().ToString)
                        objEventLogInfo.AddProperty("TYPE", objSchedulerClient.GetType().FullName)
                        objEventLogInfo.AddProperty("SOURCE", objSchedulerClient.ScheduleHistoryItem.ScheduleSource.ToString)
                        objEventLogInfo.AddProperty("ACTIVE THREADS", ActiveThreadCount.ToString)
                        objEventLogInfo.AddProperty("FREE THREADS", FreeThreads.ToString)
                        objEventLogInfo.AddProperty("READER TIMEOUTS", ReaderTimeouts.ToString)
                        objEventLogInfo.AddProperty("WRITER TIMEOUTS", WriterTimeouts.ToString)
                        objEventLogInfo.AddProperty("IN PROGRESS", GetScheduleInProgressCount().ToString)
                        objEventLogInfo.AddProperty("IN QUEUE", GetScheduleQueueCount().ToString)
                        objEventLogInfo.LogTypeKey = "SCHEDULER_EVENT_STARTED"
                        objEventLog.AddLog(objEventLogInfo)
                    End If
                Catch exc As Exception
                    'Decrement the ActiveThreadCount because
                    'otherwise the number of active threads
                    'will appear to be climbing when in fact
                    'no tasks are being executed.
                    If ActiveThreadCountIncremented Then
                        Interlocked.Decrement(ActiveThreadCount)
                    End If
                    ProcessSchedulerException(exc)
                End Try
            End Sub

#End Region

            Public Sub New(ByVal MaxThreads As Integer)
                MyBase.New()
                If Not ThreadPoolInitialized Then
                    InitializeThreadPool(MaxThreads)
                End If
            End Sub

            Public Sub New(ByVal boolDebug As Boolean, ByVal MaxThreads As Integer)
                MyBase.New()
                Debug = boolDebug
                If Not ThreadPoolInitialized Then
                    InitializeThreadPool(MaxThreads)
                End If
            End Sub

            Private Sub InitializeThreadPool(ByVal MaxThreads As Integer)
                If MaxThreads = -1 Then MaxThreads = 1
                NumberOfProcessGroups = MaxThreads
                MaxThreadCount = MaxThreads
                Dim i As Integer
                For i = 0 To NumberOfProcessGroups - 1
                    ReDim Preserve arrProcessGroup(i)
                    arrProcessGroup(i) = New ProcessGroup
                Next
                ThreadPoolInitialized = True
            End Sub

            Protected Overrides Sub Finalize()
                MyBase.Finalize()
            End Sub

        End Class



    End Module
End Namespace
