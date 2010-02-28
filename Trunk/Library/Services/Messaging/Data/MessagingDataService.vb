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

Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Messaging.Providers

Namespace DotNetNuke.Services.Messaging.Data

    Public Class MessagingDataService
        Implements IMessagingDataService

        Private provider As DataProvider = DataProvider.Instance()

        Public Sub DeleteMessage(ByVal PortalID As Integer, ByVal IndexID As Integer) Implements IMessagingDataService.DeleteMessage
            Dim pm As New Message
            pm.PortalID = PortalID
            pm.IndexID = IndexID
            pm.Status = "Deleted"
            SaveMessage(pm)
        End Sub

        Public Function GetMessageByID(ByVal PortalID As Integer, ByVal UserID As Integer, ByVal IndexID As Integer) As System.Data.IDataReader Implements IMessagingDataService.GetMessageByID
            Return CType(provider.ExecuteReader("Messaging_Get_Message_ByIndexID", PortalID, UserID, IndexID), IDataReader)

        End Function

        Public Function GetMessagesForUser(ByVal PortalID As Integer, ByVal UserID As Integer) As System.Data.IDataReader Implements IMessagingDataService.GetMessagesForUser
            Return CType(provider.ExecuteReader("Messaging_Get_Messages_ByUser", PortalID, UserID), IDataReader)

        End Function

        Public Function GetMessagesPendingSend(ByVal ExecutionCycleGuid As System.Guid) As System.Data.IDataReader Implements IMessagingDataService.GetMessagesPendingSend
            Return CType(provider.ExecuteReader("Messaging_Get_Messages_ForSend", ExecutionCycleGuid), IDataReader)

        End Function

        Public Sub SaveMessage(ByVal objMessaging As Providers.Message) Implements IMessagingDataService.SaveMessage
            Dim rdr As IDataReader = provider.ExecuteReader("Messaging_Save_Message", _
                                     objMessaging.LongBody, _
                                     objMessaging.Subject, _
                                     objMessaging.PortalID, _
                                     objMessaging.FromUserID, _
                                     objMessaging.ToUserID, _
                                     objMessaging.PendingSend, _
                                     (IIf(objMessaging.SendDate = DateTime.MinValue, Nothing, objMessaging.SendDate)), _
                                     objMessaging.ReplyTo, _
                                     objMessaging.Status, _
                                     objMessaging.MessageDate, _
                                     objMessaging.MessageGroup, _
                                     objMessaging.MessageID, _
                                     objMessaging.IndexID _
                                     )
            rdr.NextResult()
            rdr.NextResult()
            rdr.Read()
            objMessaging.MessageID = DirectCast(rdr(0), Integer)
            objMessaging.IndexID = DirectCast(rdr(1), Integer)

        End Sub

    End Class
End Namespace