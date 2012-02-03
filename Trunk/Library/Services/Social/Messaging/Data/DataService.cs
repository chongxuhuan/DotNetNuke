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

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;

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
            return _provider.ExecuteScalar<int>("SaveSocialMessage", message.MessageID, message.To, message.From, message.Subject, message.Body, message.ParentMessageID, message.ReplyAllAllowed, message.SenderUserID, createUpdateUserId);
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

        public IList<MessageItem> GetInbox(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            IDataReader dr = _provider.ExecuteReader("GetInbox", userId, pageIndex, pageSize);

            try
            {
                while (dr.Read())
                {
                    totalRecords = Convert.ToInt32(dr["TotalRecords"]);
                }
                dr.NextResult();
                return CBO.FillCollection<MessageItem>(dr);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
        }

        public IList<Message> GetSentbox(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            IDataReader dr = _provider.ExecuteReader("GetSentbox", userId, pageIndex, pageSize);

            try
            {
                while (dr.Read())
                {
                    totalRecords = Convert.ToInt32(dr["TotalRecords"]);
                }
                dr.NextResult();
                return CBO.FillCollection<Message>(dr);
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
        }

        public void UpdateSocialMessageReadStatus(int recipientId, int userId, bool read)
        {
            _provider.ExecuteNonQuery("UpdateSocialMessageReadStatus", recipientId, userId, read);
        }

        public void UpdateSocialMessageArchivedStatus(int recipientId, int userId, bool archived)
        {
            _provider.ExecuteNonQuery("UpdateSocialMessageArchivedStatus", recipientId, userId, archived);
        }


        #endregion

        #region Message_Recipients CRUD

        public int SaveSocialMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageRecipient", messageRecipient.RecipientID, messageRecipient.MessageID, messageRecipient.UserID, messageRecipient.Read, messageRecipient.Archived, createUpdateUserId);
        }

        public void CreateSocialMessageRecipientsForRole(int messageId, int roleId, int createUpdateUserId)
        {
            _provider.ExecuteNonQuery("CreateSocialMessageRecipientsForRole", messageId, roleId, createUpdateUserId);
        }

        public IDataReader GetSocialMessageRecipient()
        {
            return _provider.ExecuteReader("GetSocialMessageRecipient");
        }

        public IDataReader GetSocialMessageRecipientsByUser()
        {
            return _provider.ExecuteReader("GetSocialMessageRecipientsByUser");
        }

        public IDataReader GetSocialMessageRecipientsByMessage()
        {
            return _provider.ExecuteReader("GetSocialMessageRecipientsByMessage");
        }

        public void DeleteSocialMessageRecipient(int messageRecipientId)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessageRecipient", messageRecipientId);
        }

        #endregion

        #region Message_Attachments CRUD

        public int SaveSocialMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageAttachment", messageAttachment.MessageID, messageAttachment.FileID,createUpdateUserId);
        }

        public IDataReader GetSocialMessageAttachment()
        {
            return _provider.ExecuteReader("GetSocialMessageAttachment");
        }

        public IDataReader GetSocialMessageAttachmentsByMessage()
        {
            return _provider.ExecuteReader("GetSocialMessageAttachmentsByMessage");
        }

        public void DeleteSocialMessageAttachment(int messageAttachmentID)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessageAttachment", messageAttachmentID);
        }

        #endregion
       
    }
}