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

Imports System.Reflection
Imports System.Threading

Namespace DotNetNuke.Services.Scheduling
    Public MustInherit Class SchedulerClient

        '''''''''''''''''''''''''''''''''''''''''''''''''''
        'This class is inherited by any class that wants
        'to run tasks in the scheduler.
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        Public Event ProcessStarted As WorkStarted
        Public Event ProcessProgressing As WorkProgressing
        Public Event ProcessCompleted As WorkCompleted
        Public Event ProcessErrored As WorkErrored

        Public Sub Started()
            RaiseEvent ProcessStarted(Me)
        End Sub
        Public Sub Progressing()
            RaiseEvent ProcessProgressing(Me)
        End Sub
        Public Sub Completed()
            RaiseEvent ProcessCompleted(Me)
        End Sub
        Public Sub Errored(ByRef objException As Exception)
            RaiseEvent ProcessErrored(Me, objException)
        End Sub

        '''''''''''''''''''''''''''''''''''''''''''''''''''
        'This is the sub that kicks off the actual
        'work within the SchedulerClient's subclass
        '''''''''''''''''''''''''''''''''''''''''''''''''''
        Public MustOverride Sub DoWork()

        Private _SchedulerEventGUID As String
        Private _ProcessMethod As String
        Private _Status As String
        Private _ScheduleHistoryItem As ScheduleHistoryItem

        Public Sub New()
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            'Assign the event a unique ID for tracking purposes.
            '''''''''''''''''''''''''''''''''''''''''''''''''''
            _SchedulerEventGUID = Null.NullString
            _ProcessMethod = Null.NullString
            _Status = Null.NullString
            _ScheduleHistoryItem = New ScheduleHistoryItem

        End Sub



        Public Property ScheduleHistoryItem() As ScheduleHistoryItem
            Get
                Return _ScheduleHistoryItem
            End Get
            Set(ByVal Value As ScheduleHistoryItem)
                _ScheduleHistoryItem = Value
            End Set
        End Property
        Public Property SchedulerEventGUID() As String
            Get
                Return _SchedulerEventGUID
            End Get
            Set(ByVal Value As String)
                _SchedulerEventGUID = Value
            End Set
        End Property
        Public Property aProcessMethod() As String
            Get
                Return _ProcessMethod
            End Get
            Set(ByVal Value As String)
                _ProcessMethod = Value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return _Status
            End Get
            Set(ByVal Value As String)
                _Status = Value
            End Set
        End Property

        Public ReadOnly Property ThreadID() As Integer
            Get
                Return Thread.CurrentThread.ManagedThreadId()
            End Get
        End Property

    End Class

End Namespace
