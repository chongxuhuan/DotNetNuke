﻿#region Copyright
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
using System.Globalization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Social.Messaging.Internal.Views;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService
    {
        private readonly DataProvider _provider = DataProvider.Instance();

        #region Messages CRUD

        public int SaveMessage(Message message, int portalId, int createUpdateUserId)
        {
            //need to fix groupmail
            return _provider.ExecuteScalar<int>("CoreMessaging_SaveMessage", message.MessageID, portalId ,message.To, message.From, message.Subject, message.Body, message.ConversationId, message.ReplyAllAllowed, message.SenderUserID, createUpdateUserId);
        }

        public IDataReader GetMessage(int messageId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessage", messageId);
        }

        public IDataReader GetLastSentMessage(int userId, int portalId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetLastSentMessage", userId, portalId);
        }

        public IDataReader GetMessagesBySender(int messageId, int portalId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessagesBySender", messageId, portalId);
        }

        public void DeleteMessage(int messageId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_DeleteMessage", messageId);
        }

        public int CreateMessageReply(int conversationId, int portalId,string body, int senderUserId, string from, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_CreateMessageReply", conversationId, portalId,body, senderUserId, from, createUpdateUserId);
        }

        public MessageBoxView GetMessageBoxView(int userId, int portalId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, MessageSentStatus sentStatus)
        {
            object read = null;
            object archived = null;
            object sent = null;

            switch (readStatus)
            {
                case MessageReadStatus.Read:
                    read = true;
                    break;
                case MessageReadStatus.UnRead:
                    read = false;
                    break;
            }

            switch (archivedStatus)
            {
                case MessageArchivedStatus.Archived:
                    archived = true;
                    break;
                case MessageArchivedStatus.UnArchived:
                    archived = false;
                    break;
            }

            switch (sentStatus)
            {
                case MessageSentStatus.Received:
                    sent = false;
                    break;
                case MessageSentStatus.Sent:
                    sent = true;
                    break;
            }

            var dr = _provider.ExecuteReader("CoreMessaging_GetMessageConversations", userId, portalId , pageIndex, pageSize, sortColumn, sortAscending, read, archived, sent);
            var messageBoxView = new MessageBoxView();

            try
            {
                messageBoxView.Conversations = CBO.FillCollection<MessageConversationView>(dr);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return messageBoxView;
        }

        public IDataReader GetSentBoxView(int userId, int portalId, int pageIndex, int pageSize, string sortColumn, bool sortAscending)
        {
            return _provider.ExecuteReader("CoreMessaging_GetSentBox", userId, portalId, pageIndex, pageSize, sortColumn, sortAscending);            
        }

        public IDataReader GetArchiveBoxView(int userId, int portalId, int pageIndex, int pageSize, string sortColumn, bool sortAscending)
        {
            return _provider.ExecuteReader("CoreMessaging_GetArchiveBox", userId, portalId, pageIndex, pageSize, sortColumn, sortAscending);
        }

        public MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, string sortColumn, bool @sortAscending, ref int totalRecords)
        {
            var messageThreadsView = new MessageThreadsView();

            var dr = _provider.ExecuteReader("CoreMessaging_GetMessageThread", conversationId, userId, pageIndex, pageSize, sortColumn, sortAscending);

            try
            {
                while (dr.Read())
                {
                    var messageThreadView = new MessageThreadView { Conversation = new MessageConversationView() };
                    messageThreadView.Conversation.Fill(dr);

                    if (messageThreadView.Conversation.AttachmentCount > 0)
                    {
                        messageThreadView.Attachments = GetMessageAttachmentsByMessage(messageThreadView.Conversation.MessageID);
                    }

                    if (messageThreadsView.Conversations == null) messageThreadsView.Conversations = new List<MessageThreadView>();

                    messageThreadsView.Conversations.Add(messageThreadView);
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return messageThreadsView;
        }

        public void UpdateMessageReadStatus(int conversationId, int userId, bool read)
        {
            _provider.ExecuteNonQuery("CoreMessaging_UpdateMessageReadStatus", conversationId, userId, read);
        }

        public void UpdateMessageArchivedStatus(int conversationId, int userId, bool archived)
        {
            _provider.ExecuteNonQuery("CoreMessaging_UpdateMessageArchivedStatus", conversationId, userId, archived);
        }

        public int CountNewThreads(int userId, int portalId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_CountNewThreads", userId, portalId);
        }

        public int CountTotalConversations(int userId, int portalId, bool? read, bool? archived, bool? sentOnly)
        {
            object objRead = DBNull.Value;
            object objArchived = DBNull.Value;
            object objSent = DBNull.Value;

            if (read != null) objRead = read.Value;
            if (archived != null) objArchived = archived.Value;
            if (sentOnly != null) objSent = sentOnly.Value;

            return _provider.ExecuteScalar<int>("CoreMessaging_CountTotalConversations", userId, portalId,objRead, objArchived, objSent);
        }

        public int CountMessagesByConversation(int conversationId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_CountMessagesByConversation", conversationId);
        }

        public int CountArchivedMessagesByConversation(int conversationId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_CountArchivedMessagesByConversation", conversationId);
        }

        #endregion

        #region Message_Recipients CRUD

        public int SaveMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_SaveMessageRecipient", messageRecipient.RecipientID, messageRecipient.MessageID, messageRecipient.UserID, messageRecipient.Read, messageRecipient.Archived, createUpdateUserId);
        }

        public void CreateMessageRecipientsForRole(int messageId, string roleIds, int createUpdateUserId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_CreateMessageRecipientsForRole", messageId, roleIds, createUpdateUserId);
        }

        public IDataReader GetMessageRecipient(int messageRecipientId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessageRecipient", messageRecipientId);
        }

        public IDataReader GetMessageRecipientsByUser(int userId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessageRecipientsByUser", userId);
        }

        public IDataReader GetMessageRecipientsByMessage(int messageId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessageRecipientsByMessage", messageId);
        }

        public IDataReader GetMessageRecipientByMessageAndUser(int messageId, int userId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessageRecipientsByMessageAndUser", messageId, userId);
        }

        public void DeleteMessageRecipient(int messageRecipientId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_DeleteMessageRecipient", messageRecipientId);
        }

        public void DeleteMessageRecipientByMessageAndUser(int messageId, int userId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_DeleteMessageRecipientByMessageAndUser", messageId, userId);
        }

        #endregion

        #region Message_Attachments CRUD

        public int SaveMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("CoreMessaging_SaveMessageAttachment", messageAttachment.MessageAttachmentID, messageAttachment.MessageID, messageAttachment.FileID, createUpdateUserId);
        }

        public IDataReader GetMessageAttachment(int messageAttachmentId)
        {
            return _provider.ExecuteReader("CoreMessaging_GetMessageAttachment", messageAttachmentId);
        }

        public IList<MessageFileView> GetMessageAttachmentsByMessage(int messageId)
        {
            var attachments = new List<MessageFileView>();
            var dr = _provider.ExecuteReader("CoreMessaging_GetMessageAttachmentsByMessage", messageId);

            try
            {
                while (dr.Read())
                {
                    var fileId = Convert.ToInt32(dr["FileID"]);
                    var file = FileManager.Instance.GetFile(fileId);

                    if (file == null) continue;

                    var attachment = new MessageFileView
                                         {
                                             Name = file.FileName,
                                             Size = file.Size.ToString(CultureInfo.InvariantCulture),
                                             Url = FileManager.Instance.GetUrl(file)
                                         };

                    attachments.Add(attachment);
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return attachments;
        }

        public void DeleteMessageAttachment(int messageAttachmentId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_DeleteMessageAttachment", messageAttachmentId);
        }

        #endregion

        #region Upgrade APIs
        
        public void ConvertLegacyMessages(int pageIndex, int pageSize)
        {
            _provider.ExecuteNonQuery("CoreMessaging_ConvertLegacyMessages", pageIndex, pageSize);
        }

        public IDataReader CountLegacyMessages()
        {
            return _provider.ExecuteReader("CoreMessaging_CountLegacyMessages");
        }

        #endregion    

        #region Queued email API's

        public IDataReader GetNextMessagesForDispatch(Guid schedulerInstance,int batchSize)
        {
            return _provider.ExecuteReader("CoreMessaging_GetNextMessagesForDispatch", schedulerInstance,batchSize);
        }

        public void MarkMessageAsDispatched(int messageId,int recipientId)
        {
            _provider.ExecuteNonQuery("CoreMessaging_MarkMessageAsDispatched", messageId, recipientId);
        }


        #endregion
    }
}