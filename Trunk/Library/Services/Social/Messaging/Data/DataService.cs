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
using System.Globalization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Social.Messaging.Views;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService
    {
        private readonly DataProvider _provider = DataProvider.Instance();

        #region Messages CRUD

        public int SaveSocialMessage(Message message, int createUpdateUserId)
        {
            //need to fix groupmail
            return _provider.ExecuteScalar<int>("SaveSocialMessage", message.MessageID, message.To, message.From, message.Subject, message.Body, message.ConversationId, message.ReplyAllAllowed, message.SenderUserID, createUpdateUserId);
        }

        public IDataReader GetSocialMessage()
        {
            return _provider.ExecuteReader("GetSocialMessage");
        }

        public IDataReader GetSocialMessagesBySender()
        {
            return _provider.ExecuteReader("GetSocialMessagesBySender");
        }

        public void DeleteSocialMessage(int messageId)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessage", messageId);
        }

        public int CreateMessageReply(int conversationId, string body, int senderUserId, string from, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("CreateSocialMessageReply", conversationId, body, senderUserId, from, createUpdateUserId);
        }

        public MessageBoxView GetMessageBoxView(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, MessageSentStatus sentStatus)
        {
            object read = null;
            object archived = null;
            object sent = null;

            var messageBoxView = new MessageBoxView();

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

            var dr = _provider.ExecuteReader("GetMessageConversations", userId, pageIndex, pageSize, sortColumn, sortAscending, read, archived, sent);

            try
            {
                while (dr.Read())
                {
                    messageBoxView.TotalNewThreads = Convert.ToInt32(dr["TotalNewThreads"]);
                }
                dr.NextResult();

                while (dr.Read())
                {
                    messageBoxView.TotalConversations = Convert.ToInt32(dr["TotalRecords"]);
                }
                dr.NextResult();
                messageBoxView.Conversations = CBO.FillCollection<MessageConversationView>(dr);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }

            return messageBoxView;
        }

        public MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, string sortColumn, bool @sortAscending, ref int totalRecords)
        {
            var messageThreadsView = new MessageThreadsView();

            var dr = _provider.ExecuteReader("GetMessageThread", conversationId, userId, pageIndex, pageSize, sortColumn, sortAscending);

            try
            {
                while (dr.Read())
                {
                    messageThreadsView.TotalNewThreads = Convert.ToInt32(dr["TotalNewThreads"]);
                }
                dr.NextResult();

                while (dr.Read())
                {
                    messageThreadsView.TotalThreads = Convert.ToInt32(dr["TotalRecords"]);
                }
                dr.NextResult();

                while (dr.Read())
                {
                    messageThreadsView.TotalArchivedThreads = Convert.ToInt32(dr["TotalArchivedThreads"]);
                }
                dr.NextResult();

                while (dr.Read())
                {
                    var messageThreadView = new MessageThreadView { Conversation = new MessageConversationView() };
                    messageThreadView.Conversation.Fill(dr);

                    if (messageThreadView.Conversation.AttachmentCount > 0)
                    {
                        messageThreadView.Attachments = GetSocialMessageAttachmentsByMessage(messageThreadView.Conversation.MessageID);
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

        public void UpdateSocialMessageReadStatus(int conversationId, int userId, bool read)
        {
            _provider.ExecuteNonQuery("UpdateSocialMessageReadStatus", conversationId, userId, read);
        }

        public void UpdateSocialMessageArchivedStatus(int conversationId, int userId, bool archived)
        {
            _provider.ExecuteNonQuery("UpdateSocialMessageArchivedStatus", conversationId, userId, archived);
        }


        #endregion

        #region Message_Recipients CRUD

        public int SaveSocialMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageRecipient", messageRecipient.RecipientID, messageRecipient.MessageID, messageRecipient.UserID, messageRecipient.Read, messageRecipient.Archived, createUpdateUserId);
        }

        public void CreateSocialMessageRecipientsForRole(int messageId, string roleIds, int createUpdateUserId)
        {
            _provider.ExecuteNonQuery("CreateSocialMessageRecipientsForRole", messageId, roleIds, createUpdateUserId);
        }

        public IDataReader GetSocialMessageRecipient(int messageRecipientId)
        {
            return _provider.ExecuteReader("GetSocialMessageRecipient", messageRecipientId);
        }

        public IDataReader GetSocialMessageRecipientsByUser(int userId)
        {
            return _provider.ExecuteReader("GetSocialMessageRecipientsByUser", userId);
        }

        public IDataReader GetSocialMessageRecipientsByMessage(int messageId)
        {
            return _provider.ExecuteReader("GetSocialMessageRecipientsByMessage", messageId);
        }

        public IDataReader GetSocialMessageRecipientByMessageAndUser(int messageId, int userId)
        {
            return _provider.ExecuteReader("GetSocialMessageRecipientsByMessageAndUser", messageId, userId);
        }

        public void DeleteSocialMessageRecipient(int messageRecipientId)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessageRecipient", messageRecipientId);
        }

        #endregion

        #region Message_Attachments CRUD

        public int SaveSocialMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageAttachment", messageAttachment.MessageAttachmentID, messageAttachment.MessageID, messageAttachment.FileID, createUpdateUserId);
        }

        public IDataReader GetSocialMessageAttachment(int messageAttachmentId)
        {
            return _provider.ExecuteReader("GetSocialMessageAttachment", messageAttachmentId);
        }

        public IList<MessageFileView> GetSocialMessageAttachmentsByMessage(int messageId)
        {
            var attachments = new List<MessageFileView>();
            var dr = _provider.ExecuteReader("GetSocialMessageAttachmentsByMessage", messageId);

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

        public void DeleteSocialMessageAttachment(int messageAttachmentId)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessageAttachment", messageAttachmentId);
        }

        #endregion

        #region MessageTypes CRUD
        
        public int SaveMessageType(int messageTypeId, string name, string description, int timeToLive, bool isNotification)
        {
            return _provider.ExecuteScalar<int>("SaveMessageType", messageTypeId, name, _provider.GetNull(description), _provider.GetNull(timeToLive), isNotification);
        }

        public void DeleteMessageType(int messageTypeId)
        {
            _provider.ExecuteNonQuery("DeleteMessageType", messageTypeId);
        }

        public IDataReader GetMessageType(int messageTypeId)
        {
            return _provider.ExecuteReader("GetMessageType", messageTypeId);
        }

        public IDataReader GetMessageTypeByName(string name)
        {
            return _provider.ExecuteReader("GetMessageTypeByName", name);
        }

        #endregion
    }
}