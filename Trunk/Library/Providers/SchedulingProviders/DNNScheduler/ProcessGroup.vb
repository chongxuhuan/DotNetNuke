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
Imports System.Threading
Imports System.Xml
Imports DotNetNuke.Services.Scheduling
Imports DotNetNuke.Services.Exceptions
Imports System.Web.Compilation

Namespace DotNetNuke.Services.Scheduling.DNNScheduling

    Public Class ProcessGroup
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        'This class represents a process group for
        'our threads to run in.
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        Private Shared numberOfProcessesInQueue As Integer = 0
        Private Shared numberOfProcesses As Integer = 0
        Private Shared processesCompleted As Integer
        Private Shared ticksElapsed As Integer

        Shared ReadOnly Property GetTicksElapsed() As Integer
            Get
                Return ticksElapsed
            End Get
        End Property

        Shared ReadOnly Property GetProcessesCompleted() As Integer
            Get
                Return processesCompleted
            End Get
        End Property

        Shared ReadOnly Property GetProcessesInQueue() As Integer
            Get
                Return numberOfProcessesInQueue
            End Get
        End Property

        Public Event Completed()

        Public Sub Run(ByVal objScheduleHistoryItem As ScheduleHistoryItem)
            Dim Process As SchedulerClient = Nothing
            Try
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'This is called from RunPooledThread()
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                ticksElapsed = Environment.TickCount - ticksElapsed
                Process = GetSchedulerClient(objScheduleHistoryItem.TypeFullName, objScheduleHistoryItem)

                Process.ScheduleHistoryItem = objScheduleHistoryItem

                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'Set up the handlers for the CoreScheduler
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                AddHandler Process.ProcessStarted, New WorkStarted(AddressOf CoreScheduler.WorkStarted)
                AddHandler Process.ProcessProgressing, New WorkProgressing(AddressOf CoreScheduler.WorkProgressing)
                AddHandler Process.ProcessCompleted, New WorkCompleted(AddressOf CoreScheduler.WorkCompleted)
                AddHandler Process.ProcessErrored, New WorkErrored(AddressOf CoreScheduler.WorkErrored)

                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'This kicks off the DoWork method of the class
                'type specified in the configuration.
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                Process.Started()
                Try
                    Process.ScheduleHistoryItem.Succeeded = False
                    Process.DoWork()
                Catch exc As Exception
                    'in case the scheduler client
                    'didn't have proper exception handling
                    'make sure we fire the Errored event
                    If Not Process Is Nothing Then
                        If Not Process.ScheduleHistoryItem Is Nothing Then
                            Process.ScheduleHistoryItem.Succeeded = False
                        End If
                        Process.Errored(exc)
                    End If
                End Try
                If Process.ScheduleHistoryItem.Succeeded = True Then
                    Process.Completed()
                End If
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'If all processes in this ProcessGroup have
                'completed, set the ticksElapsed and raise
                'the Completed event.
                'I don't think this is necessary with the
                'other events.  I'll leave it for now and
                'will probably take it out later.
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                If processesCompleted = numberOfProcesses Then
                    If processesCompleted = numberOfProcesses Then
                        ticksElapsed = Environment.TickCount - ticksElapsed
                        RaiseEvent Completed()
                    End If
                End If
            Catch exc As Exception
                'in case the scheduler client
                'didn't have proper exception handling
                'make sure we fire the Errored event
                If Not Process Is Nothing Then
                    If Not Process.ScheduleHistoryItem Is Nothing Then
                        Process.ScheduleHistoryItem.Succeeded = False
                    End If
                    Process.Errored(exc)
                End If
            Finally
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                'Track how many processes have completed for
                'this instanciation of the ProcessGroup
                '''''''''''''''''''''''''''''''''''''''''''''''''''
                numberOfProcessesInQueue -= 1
                processesCompleted += 1
            End Try
        End Sub

        Private Function GetSchedulerClient(ByVal strProcess As String, ByVal objScheduleHistoryItem As ScheduleHistoryItem) As SchedulerClient
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'This is a method to encapsulate returning
            'an object whose class inherits SchedulerClient.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim t As Type = BuildManager.GetType(strProcess, True, True)
            Dim param(0) As ScheduleHistoryItem
            param(0) = objScheduleHistoryItem

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the constructor for the Class
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim types(0) As Type
            types(0) = GetType(ScheduleHistoryItem)
            Dim objConstructor As System.Reflection.ConstructorInfo
            objConstructor = t.GetConstructor(types)

            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'Return an instance of the class as an object
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            Return CType(objConstructor.Invoke(param), SchedulerClient)
        End Function

        '''''''''''''''''''''''''''''''''''''''''''''''''''
        ' This subroutine is callback for Threadpool.QueueWorkItem.  This is the necessary
        ' subroutine signature for QueueWorkItem, and Run() is proper for creating a Thread
        ' so the two subroutines cannot be combined, so instead just call Run from here.
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        Private Sub RunPooledThread(ByVal objScheduleHistoryItem As Object)
            Run(CType(objScheduleHistoryItem, ScheduleHistoryItem))
        End Sub

        '''''''''''''''''''''''''''''''''''''''''''''''''''
        'Add a queue request to Threadpool with a 
        'callback to RunPooledThread which calls Run()
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        Public Sub AddQueueUserWorkItem(ByVal s As ScheduleItem)
            numberOfProcessesInQueue += 1
            numberOfProcesses += 1
            Dim obj As New ScheduleHistoryItem
            obj.TypeFullName = s.TypeFullName
            obj.ScheduleID = s.ScheduleID
            obj.TimeLapse = s.TimeLapse
            obj.TimeLapseMeasurement = s.TimeLapseMeasurement
            obj.RetryTimeLapse = s.RetryTimeLapse
            obj.RetryTimeLapseMeasurement = s.RetryTimeLapseMeasurement
            obj.ObjectDependencies = s.ObjectDependencies
            obj.CatchUpEnabled = s.CatchUpEnabled
            obj.Enabled = s.Enabled
            obj.NextStart = s.NextStart
            obj.ScheduleSource = s.ScheduleSource
            obj.ThreadID = s.ThreadID
            obj.ProcessGroup = s.ProcessGroup
            obj.RetainHistoryNum = s.RetainHistoryNum

            Try
                ' Create a callback to subroutine RunPooledThread
                Dim callback As New Threading.WaitCallback(AddressOf RunPooledThread)
                ' And put in a request to ThreadPool to run the process.
                Threading.ThreadPool.QueueUserWorkItem(callback, CType(obj, Object))
                Thread.Sleep(1000)
            Catch exc As Exception
                ProcessSchedulerException(exc)
            End Try
        End Sub

        Public Sub RunSingleTask(ByVal s As ScheduleItem)
            numberOfProcessesInQueue += 1
            numberOfProcesses += 1
            Dim obj As New ScheduleHistoryItem
            obj.TypeFullName = s.TypeFullName
            obj.ScheduleID = s.ScheduleID
            obj.TimeLapse = s.TimeLapse
            obj.TimeLapseMeasurement = s.TimeLapseMeasurement
            obj.RetryTimeLapse = s.RetryTimeLapse
            obj.RetryTimeLapseMeasurement = s.RetryTimeLapseMeasurement
            obj.ObjectDependencies = s.ObjectDependencies
            obj.CatchUpEnabled = s.CatchUpEnabled
            obj.Enabled = s.Enabled
            obj.NextStart = s.NextStart
            obj.ScheduleSource = s.ScheduleSource
            obj.ThreadID = s.ThreadID
            obj.ProcessGroup = s.ProcessGroup

            Try
                Run(obj)
                Thread.Sleep(1000)
            Catch exc As Exception
                ProcessSchedulerException(exc)
            End Try
        End Sub

    End Class
End Namespace