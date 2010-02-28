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
Imports DotNetNuke
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Threading
Imports System.ComponentModel
Imports DotNetNuke.Services.Messaging.Providers
Imports DotNetNuke.Services.Messaging

Namespace DotNetNuke.Services.Messaging.Scheduler

    Public Class MessagingScheduler
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

        Dim asyncCompletedEventArgs As AsyncCompletedEventArgs
        Dim waitHandle As AutoResetEvent

        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = objScheduleHistoryItem
            waitHandle = New AutoResetEvent(False)
        End Sub

        Public Overrides Sub DoWork()
            Try

                Dim ExecutionCycleGuid As Guid = Guid.NewGuid()
                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler DoWork Starting")

                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler DoWork Querying Database for list of pending messages that need to be sent")
                Dim c As New MessagingController

                Dim settings As Hashtable = Me.ScheduleHistoryItem.GetSettings()

                Dim newStatus As String = "Sent"
                If (Not (settings Is Nothing) And settings.ContainsKey("NewStatus")) Then
                    newStatus = settings("NewStatus").ToString()
                End If

                Dim sendList As List(Of Message) = c.GetMessagesPendingSend(ExecutionCycleGuid)
                Me.ScheduleHistoryItem.AddLogNote(String.Format("MessagingScheduler DoWork Found:{0} records", sendList.Count))
                For Each msg As Message In sendList
                    Dim mail As New System.Net.Mail.MailMessage(New System.Net.Mail.MailAddress(msg.FromEmail, msg.FromDisplayName), New System.Net.Mail.MailAddress(msg.ToEmail, msg.ToUsername))
                    mail.Subject = msg.Subject
                    mail.Body = msg.LongBody
                    mail.IsBodyHtml = True
                    Dim client As New System.Net.Mail.SmtpClient()
                    Dim userToken As Object() = New Object() {c, msg, newStatus, Me}

                    AddHandler client.SendCompleted, AddressOf SendCompletedCallback

                    client.SendAsync(mail, userToken)
                    waitHandle.WaitOne()
                Next

                Me.ScheduleHistoryItem.Succeeded = True
                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler DoWork Succeeded")
            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler Failed: " & ex.ToString)
                Me.Errored(ex)
            End Try
        End Sub

        Private Sub SendCompletedCallback(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
            asyncCompletedEventArgs = e

            Dim smtpClient As System.Net.Mail.SmtpClient = TryCast(sender, SmtpClient)

            Dim userToken As Object() = DirectCast(e.UserState, Object())
            Dim c As MessagingController = TryCast(userToken(0), MessagingController)
            Dim msg As Message = TryCast(userToken(1), Message)
            Dim newStatus As String = TryCast(userToken(2), String)
            Dim client As MessagingScheduler = TryCast(userToken(3), MessagingScheduler)

            If (e.Cancelled) Then
                client.ScheduleHistoryItem.AddLogNote("Cancelled")
            ElseIf Not (e.Error Is Nothing) Then
                client.ScheduleHistoryItem.AddLogNote("Errored:" & e.Error.ToString)
            Else
                msg.PendingSend = False
                msg.SendDate = DateTime.Now
                msg.Status = newStatus
                c.SaveMessage(msg)
            End If
            waitHandle.Set()
        End Sub

    End Class
End Namespace
