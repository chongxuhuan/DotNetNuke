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
Imports System.IO
Imports System.Web
Imports System.Xml
Imports System.Xml.XPath
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.EventQueue.Config
Imports DotNetNuke.Security

Namespace DotNetNuke.Services.EventQueue

    Public Class EventQueueController

#Region "Private Shared Methods"

        Private Shared Function FillMessage(ByVal dr As IDataReader, ByVal CheckForOpenDataReader As Boolean) As EventMessage
            Dim message As EventMessage

            ' read datareader
            Dim canContinue As Boolean = True
            If CheckForOpenDataReader Then
                canContinue = False
                If dr.Read Then
                    canContinue = True
                End If
            End If

            If canContinue Then
                message = New EventMessage()
                message.EventMessageID = Convert.ToInt32(Null.SetNull(dr("EventMessageID"), message.EventMessageID))
                message.Priority = CType([Enum].Parse(GetType(MessagePriority), Convert.ToString(Null.SetNull(dr("Priority"), message.Priority))), MessagePriority)
                message.ProcessorType = Convert.ToString(Null.SetNull(dr("ProcessorType"), message.ProcessorType))
                message.ProcessorCommand = Convert.ToString(Null.SetNull(dr("ProcessorCommand"), message.ProcessorCommand))
                message.Body = Convert.ToString(Null.SetNull(dr("Body"), message.Body))
                message.Sender = Convert.ToString(Null.SetNull(dr("Sender"), message.Sender))
                message.Subscribers = Convert.ToString(Null.SetNull(dr("Subscriber"), message.Subscribers))
                message.AuthorizedRoles = Convert.ToString(Null.SetNull(dr("AuthorizedRoles"), message.AuthorizedRoles))
                message.ExceptionMessage = Convert.ToString(Null.SetNull(dr("ExceptionMessage"), message.ExceptionMessage))
                message.SentDate = Convert.ToDateTime(Null.SetNull(dr("SentDate"), message.SentDate))
                message.ExpirationDate = Convert.ToDateTime(Null.SetNull(dr("ExpirationDate"), message.ExpirationDate))

                'Deserialize Attributes
                Dim xmlAttributes As String = Null.NullString
                xmlAttributes = Convert.ToString(Null.SetNull(dr("Attributes"), xmlAttributes))
                message.DeserializeAttributes(xmlAttributes)
            Else
                message = Nothing
            End If

            Return message

        End Function

        Private Shared Function FillMessageCollection(ByVal dr As IDataReader) As EventMessageCollection
            Dim arr As New EventMessageCollection()
            Try
                Dim obj As EventMessage
                While dr.Read
                    ' fill business object
                    obj = FillMessage(dr, False)
                    ' add to collection
                    arr.Add(obj)
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try
            Return arr
        End Function

        Private Shared Function GetSubscribers(ByVal eventName As String) As String()
            'Get the subscribers to this event
            Dim subscribers(-1) As String
            Dim publishedEvent As PublishedEvent
            If Config.EventQueueConfiguration.GetConfig().PublishedEvents.TryGetValue(eventName, publishedEvent) Then
                subscribers = publishedEvent.Subscribers().Split(";".ToCharArray())
            Else
                subscribers(0) = ""
            End If

            Return subscribers
        End Function

#End Region

        'Public Function GetMessage(ByVal eventName As String, ByVal subscriberId As String, ByVal messageId As String) As EventMessage
        '    Return DeserializeMessage(m_messagePath & MessageName(eventName, subscriberId, messageId), subscriberId)
        'End Function

        Public Shared Function GetMessages(ByVal eventName As String) As EventMessageCollection
            Return FillMessageCollection(DataProvider.Instance.GetEventMessages(eventName))
        End Function

        Public Shared Function GetMessages(ByVal eventName As String, ByVal subscriberId As String) As EventMessageCollection
            Return FillMessageCollection(DataProvider.Instance.GetEventMessagesBySubscriber(eventName, subscriberId))
        End Function

        Public Shared Function ProcessMessages(ByVal eventName As String) As Boolean
            Return ProcessMessages(GetMessages(eventName))
        End Function

        Public Shared Function ProcessMessages(ByVal eventName As String, ByVal subscriberId As String) As Boolean
            Return ProcessMessages(GetMessages(eventName, subscriberId))
        End Function

        Public Shared Function ProcessMessages(ByVal eventMessages As EventMessageCollection) As Boolean

            Dim message As EventMessage
            For messageNo As Integer = 0 To eventMessages.Count - 1
                message = eventMessages(messageNo)
                Try
                    Dim oMessageProcessor As Object = Framework.Reflection.CreateObject(message.ProcessorType, message.ProcessorType)
                    If Not CType(oMessageProcessor, EventMessageProcessorBase).ProcessMessage(message) Then
                        Throw New Exception
                    End If

                    'Set Message comlete so it is not run a second time
                    DataProvider.Instance.SetEventMessageComplete(message.EventMessageID)
                Catch
                    'log if message could not be processed
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                    objEventLogInfo.AddProperty("EventQueue.ProcessMessage", "Message Processing Failed")
                    objEventLogInfo.AddProperty("ProcessorType", message.ProcessorType)
                    objEventLogInfo.AddProperty("Body", message.Body)
                    objEventLogInfo.AddProperty("Sender", message.Sender)
                    For Each key As String In message.Attributes.Keys
                        objEventLogInfo.AddProperty(key, message.Attributes(key))
                    Next
                    If message.ExceptionMessage.Length > 0 Then
                        objEventLogInfo.AddProperty("ExceptionMessage", message.ExceptionMessage)
                    End If
                    objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                    objEventLog.AddLog(objEventLogInfo)

                    If message.ExpirationDate < DateTime.Now() Then
                        'Set Message comlete so it is not run a second time
                        DataProvider.Instance.SetEventMessageComplete(message.EventMessageID)
                    End If
                End Try
            Next
        End Function

        Public Shared Function SendMessage(ByVal message As EventMessage, ByVal eventName As String) As Boolean
            'set the sent date if it wasn't set by the sender
            If IsNothing(message.SentDate) Then
                message.SentDate = DateTime.Now
            End If

            'Get the subscribers to this event
            Dim subscribers() As String = GetSubscribers(eventName)

            'send a message for each subscriber of the specified event
            Dim intMessageID As Integer = Null.NullInteger
            Dim success As Boolean = True
            Try
                For indx As Integer = 0 To subscribers.Length - 1
                    intMessageID = DataProvider.Instance.AddEventMessage(eventName, message.Priority, message.ProcessorType, message.ProcessorCommand, message.Body, message.Sender, subscribers(indx), message.AuthorizedRoles, message.ExceptionMessage, message.SentDate, message.ExpirationDate, message.SerializeAttributes())
                Next
            Catch ex As Exception
                LogException(ex)
                success = Null.NullBoolean
            End Try

            Return success
        End Function

#Region "Obsolete Methods"

        <Obsolete("This method is obsolete. Use Sendmessage(message, eventName) instead")> _
        Public Function SendMessage(ByVal message As EventMessage, ByVal eventName As String, ByVal encryptMessage As Boolean) As Boolean
            Return SendMessage(message, eventName)
        End Function

#End Region

    End Class


End Namespace
