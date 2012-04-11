#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Services.Social.Messaging.Views;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    public interface IDataService
    {
        #region Messages CRUD

        int SaveMessage(Message message,int portalId, int createUpdateUserId, DateTime messageDateTime);
        IDataReader GetMessage(int messageId);
        IDataReader GetMessagesBySender(int messageId, int portalId);
        void DeleteMessage(int messageId);

        MessageBoxView GetMessageBoxView(int userId, int portalId,int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, MessageSentStatus sentStatus);
        MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords);        
        void UpdateMessageReadStatus(int conversationId, int userId, bool read);
        void UpdateMessageArchivedStatus(int conversationId, int userId, bool archived);
        int CreateMessageReply(int conversationId, int portalId,string body, int senderUserId, string from, int createUpdateUserId);
        int CountNewThreads(int userId, int portalId);
        int CountTotalConversations(int userId, int portalId, bool? read, bool? archived, bool? sentOnly);
        int CountMessagesByConversation(int conversationId);
        int CountArchivedMessagesByConversation(int conversationId);
        
        #endregion

        #region Message_Recipients CRUD

        int SaveMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserId, DateTime messageDateTime);
        void CreateMessageRecipientsForRole(int messageId, string roleIds, int createUpdateUserId, DateTime messageDateTime);
        IDataReader GetMessageRecipient(int messageRecipientId);
        IDataReader GetMessageRecipientsByUser(int userId);
        IDataReader GetMessageRecipientsByMessage(int messageId);
        IDataReader GetMessageRecipientByMessageAndUser(int messageId, int userId);
        void DeleteMessageRecipient(int messageRecipientId);
        void DeleteMessageRecipientByMessageAndUser(int messageId, int userId);

        #endregion

        #region Message_Attachments CRUD

        int SaveMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserId);
        IDataReader GetMessageAttachment(int messageAttachmentId);
        IList<MessageFileView> GetMessageAttachmentsByMessage(int messageId);
        void DeleteMessageAttachment(int messageAttachmentId);

        #endregion

        #region Upgrade APIs
        
        void ConvertLegacyMessages(int pageIndex, int pageSize);

        IDataReader CountLegacyMessages();        

        #endregion

        #region Queued email API's

        IDataReader GetNextMessageForDispatch(Guid schedulerInstance);
        void MarkMessageAsDispatched(int messageId,int recipientId);

        #endregion
    }
}