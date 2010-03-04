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
Imports DotNetNuke.Services.Messaging.Data
Imports System.Net.Mail
Imports System.Threading
Imports System.ComponentModel
Imports DotNetNuke.Entities.Host


Namespace DotNetNuke.Services.Messaging.Scheduler

    Public Class MessagingScheduler
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

        Dim asyncCompletedEventArgs As AsyncCompletedEventArgs
        Dim waitHandle As AutoResetEvent
        Dim _pController As New PortalController()
        Dim _mController As New MessagingController()
        Dim _uController As New UserController()


        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.New()
            Me.ScheduleHistoryItem = objScheduleHistoryItem
            waitHandle = New AutoResetEvent(False)
        End Sub

        Public Overrides Sub DoWork()
            Try

                Dim _schedulerInstance As Guid = Guid.NewGuid()
                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler DoWork Starting " + _schedulerInstance.ToString())

                If (String.IsNullOrEmpty(Host.SMTPServer)) Then
                    Me.ScheduleHistoryItem.AddLogNote("No SMTP Servers have been configured for this host. Terminating task.")
                    Me.ScheduleHistoryItem.Succeeded = True
                    ''Return
                Else
                    Dim settings As Hashtable = Me.ScheduleHistoryItem.GetSettings()

                    Dim _messageLeft As Boolean = True
                    Dim _messagesSent As Integer = 0

                    While _messageLeft

                        Dim currentMessage As Message = _mController.GetNextMessageForDispatch(_schedulerInstance)

                        If (currentMessage IsNot Nothing) Then
                            Try
                                SendMessage(currentMessage)
                                _messagesSent = _messagesSent + 1
                            Catch e As Exception
                                Me.Errored(e)
                            End Try
                        Else
                            _messageLeft = False
                        End If

                    End While

                    Me.ScheduleHistoryItem.AddLogNote(String.Format("Message Scheduler '{0}' sent a total of {1} message(s)", _schedulerInstance, _messagesSent))
                    Me.ScheduleHistoryItem.Succeeded = True

                End If

            Catch ex As Exception
                Me.ScheduleHistoryItem.Succeeded = False
                Me.ScheduleHistoryItem.AddLogNote("MessagingScheduler Failed: " & ex.ToString)
                Me.Errored(ex)
            End Try
        End Sub

        Private Sub SendMessage(ByVal objMessage As Message)
            Dim ToEmailAddress As String = UserController.GetUserById(objMessage.PortalID, objMessage.ToUserID).Email

            Dim emailMessage As New MailMessage(_uController.GetUser(objMessage.PortalID, objMessage.ToUserID).Email, _pController.GetPortal(objMessage.PortalID).Email)
            emailMessage.Body = objMessage.Body
            emailMessage.Subject = objMessage.Subject
            emailMessage.Sender = New MailAddress(UserController.GetUserById(objMessage.PortalID, objMessage.FromUserID).Email)

            Dim smtpClient As New SmtpClient(Host.SMTPServer)

            smtpClient.Send(emailMessage)

            _mController.MarkMessageAsDispatched(objMessage.MessageID)

        End Sub
    End Class



End Namespace
