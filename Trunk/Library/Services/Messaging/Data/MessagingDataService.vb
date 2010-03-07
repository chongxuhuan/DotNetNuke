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


Namespace DotNetNuke.Services.Messaging.Data

    Public Class MessagingDataService
        Implements IMessagingDataService

        Private provider As DataProvider = DataProvider.Instance()

        Public Function GetMessageByID(ByVal messageId As Integer) As System.Data.IDataReader Implements IMessagingDataService.GetMessageByID
            Return CType(provider.ExecuteReader("Messaging_GetMessage", messageId), IDataReader)

        End Function

        Public Function GetUserInbox(ByVal PortalID As Integer, ByVal UserID As Integer, ByVal PageNumber As Integer, ByVal PageSize As Integer) As System.Data.IDataReader Implements IMessagingDataService.GetUserInbox
            Return CType(provider.ExecuteReader("Messaging_GetInbox", PortalID, UserID, PageNumber, PageSize), IDataReader)

        End Function

        Public Function GetInboxCount(ByVal PortalID As Integer, ByVal UserID As Integer) As Integer Implements IMessagingDataService.GetInboxCount
            Return DirectCast(provider.ExecuteScalar("Messaging_GetInboxCount", PortalID, UserID), Integer)
        End Function

        Public Function SaveMessage(ByVal objMessaging As Message) As Long Implements IMessagingDataService.SaveMessage
            Dim messageId As Object = provider.ExecuteScalar("Messaging_Save_Message", _
                                        objMessaging.PortalID, _
                                        objMessaging.FromUserID, _
                                        objMessaging.ToUserID, _
                                        objMessaging.ToRoleID, _
                                        CType(objMessaging.Status, Integer), _
                                        objMessaging.Subject, _
                                        objMessaging.Body, _
                                        objMessaging.MessageDate, _
                                        objMessaging.Conversation, _
                                        objMessaging.ReplyTo, _
                                        objMessaging.AllowReply, _
                                        objMessaging.SkipInbox)

            Return CType(messageId, Long)
        End Function

        Public Function GetNewMessageCount(ByVal PortalID As Integer, ByVal UserID As Integer) As Integer Implements IMessagingDataService.GetNewMessageCount
            Return DirectCast(provider.ExecuteScalar("Messaging_GetNewMessageCount", PortalID, UserID), Integer)
        End Function

        Public Function GetNextMessageForDispatch(ByVal SchedulerInstance As Guid) As IDataReader Implements IMessagingDataService.GetNextMessageForDispatch
            Return CType(provider.ExecuteReader("Messaging_GetNextMessageForDispatch", SchedulerInstance), IDataReader)

        End Function

        Public Sub MarkMessageAsDispatched(ByVal MessageID As Integer) Implements IMessagingDataService.MarkMessageAsDispatched
            provider.ExecuteNonQuery("Messaging_MarkMessageAsDispatched", MessageID)
        End Sub

        Public Sub UpdateMessage(ByVal message As Message) Implements IMessagingDataService.UpdateMessage
            provider.ExecuteNonQuery("Messaging_UpdateMessage", message.MessageID, message.ToUserID, message.ToRoleID, CType(message.Status, Integer), message.Subject, message.Body, message.MessageDate, message.ReplyTo, message.AllowReply, message.SkipInbox)
        End Sub
    End Class
End Namespace