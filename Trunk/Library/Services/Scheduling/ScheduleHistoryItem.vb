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
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Services.Scheduling
    <Serializable()> Public Class ScheduleHistoryItem
        Inherits ScheduleItem

#Region "Private Members"

        Private _ScheduleHistoryID As Integer
        Private _StartDate As Date
        Private _EndDate As Date
        Private _Succeeded As Boolean
        Private _LogNotes As System.Text.StringBuilder
        Private _Server As String

#End Region

#Region "Constructors"

        Public Sub New()
            _ScheduleHistoryID = Null.NullInteger
            _StartDate = Null.NullDate
            _EndDate = Null.NullDate
            _Succeeded = Null.NullBoolean
            _LogNotes = New System.Text.StringBuilder
            _Server = Null.NullString
        End Sub

        Public Sub New(ByVal objScheduleItem As ScheduleItem)
            Me.AttachToEvent = objScheduleItem.AttachToEvent
            Me.CatchUpEnabled = objScheduleItem.CatchUpEnabled
            Me.Enabled = objScheduleItem.Enabled
            Me.NextStart = objScheduleItem.NextStart
            Me.ObjectDependencies = objScheduleItem.ObjectDependencies
            Me.ProcessGroup = objScheduleItem.ProcessGroup
            Me.RetainHistoryNum = objScheduleItem.RetainHistoryNum
            Me.RetryTimeLapse = objScheduleItem.RetryTimeLapse
            Me.RetryTimeLapseMeasurement = objScheduleItem.RetryTimeLapseMeasurement
            Me.ScheduleID = objScheduleItem.ScheduleID
            Me.ScheduleSource = objScheduleItem.ScheduleSource
            Me.ThreadID = objScheduleItem.ThreadID
            Me.TimeLapse = objScheduleItem.TimeLapse
            Me.TimeLapseMeasurement = objScheduleItem.TimeLapseMeasurement
            Me.TypeFullName = objScheduleItem.TypeFullName
            Me.Servers = objScheduleItem.Servers
            Me.FriendlyName = objScheduleItem.FriendlyName
            _ScheduleHistoryID = Null.NullInteger
            _StartDate = Null.NullDate
            _EndDate = Null.NullDate
            _Succeeded = Null.NullBoolean
            _LogNotes = New System.Text.StringBuilder
            _Server = Null.NullString
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property ElapsedTime() As Double
            Get
                Try
                    If _EndDate = Null.NullDate And _StartDate <> Null.NullDate Then
                        Return Now.Subtract(_StartDate).TotalSeconds()
                    ElseIf _StartDate <> Null.NullDate Then
                        Return _EndDate.Subtract(_StartDate).TotalSeconds()
                    Else
                        Return 0
                    End If
                Catch
                    ElapsedTime = 0
                End Try
            End Get
        End Property

        Public Property EndDate() As Date
            Get
                Return _EndDate
            End Get
            Set(ByVal Value As Date)
                _EndDate = Value
            End Set
        End Property

        Public Property LogNotes() As String
            Get
                Return _LogNotes.ToString
            End Get
            Set(ByVal Value As String)
                _LogNotes = New System.Text.StringBuilder(Value)
            End Set
        End Property

        Public ReadOnly Property Overdue() As Boolean
            Get
                If NextStart < Now And EndDate = Null.NullDate Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property OverdueBy() As Double
            Get
                Try
                    If NextStart <= Now And EndDate = Null.NullDate Then
                        Return Now.Subtract(NextStart).TotalSeconds
                    Else
                        Return 0
                    End If
                Catch
                    OverdueBy = 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RemainingTime() As Double
            Get
                Try
                    If NextStart > Now And EndDate = Null.NullDate Then
                        Return NextStart.Subtract(Now).TotalSeconds
                    Else
                        Return 0
                    End If
                Catch
                    RemainingTime = 0
                End Try
            End Get
        End Property

        Public Property ScheduleHistoryID() As Integer
            Get
                Return _ScheduleHistoryID
            End Get
            Set(ByVal Value As Integer)
                _ScheduleHistoryID = Value
            End Set
        End Property

        Public Property Server() As String
            Get
                Return _Server
            End Get
            Set(ByVal Value As String)
                _Server = Value
            End Set
        End Property

        Public Property StartDate() As Date
            Get
                Return _StartDate
            End Get
            Set(ByVal Value As Date)
                _StartDate = Value
            End Set
        End Property

        Public Property Succeeded() As Boolean
            Get
                Return _Succeeded
            End Get
            Set(ByVal Value As Boolean)
                _Succeeded = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub AddLogNote(ByVal Notes As String)
            _LogNotes.Append(Notes + vbCrLf)
        End Sub

        Public Overrides Sub Fill(ByVal dr As System.Data.IDataReader)
            ScheduleHistoryID = Null.SetNullInteger(dr("ScheduleHistoryID"))
            StartDate = Null.SetNullDateTime(dr("StartDate"))
            EndDate = Null.SetNullDateTime(dr("EndDate"))
            Succeeded = Null.SetNullBoolean(dr("Succeeded"))
            LogNotes = Null.SetNullString(dr("LogNotes"))
            Server = Null.SetNullString(dr("Server"))
            MyBase.FillInternal(dr)
        End Sub

#End Region

    End Class


End Namespace
