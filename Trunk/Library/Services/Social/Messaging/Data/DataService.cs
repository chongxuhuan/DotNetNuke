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

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService
        
    {
        private readonly DataProvider _provider = DataProvider.Instance();

        #region Messages CRUD

        public int SaveSocialMessage(Message message, int createUpdateUserID)
        {
            //need to fix groupmail
            return _provider.ExecuteScalar<int>("SaveSocialMessage", message.Subject,message.Body,message.ParentMessageID,message.SenderUserID, createUpdateUserID);
        }

        public IDataReader GetSocialMessage()
        {
            return _provider.ExecuteReader("GetSocialMessage");
        }

        public IDataReader GetSocialMessagesBySender()
        {
            return _provider.ExecuteReader("GetSocialMessagesBySender");
        }

        public void DeleteSocialMessage(int messageID)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessage", messageID);
        }

        public IList<Message> GetInbox(int userID, int pageIndex, int pageSize, int totalRecords)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Message_Recipients CRUD

        public int SaveSocialMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageRecipient", messageRecipient.UserID, messageRecipient.Status, createUpdateUserID);
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

        public void DeleteSocialMessageRecipient(int messageRecipientID)
        {
            _provider.ExecuteNonQuery("DeleteSocialMessageRecipient", messageRecipientID);
        }

        #endregion

        #region Message_Attachments CRUD

        public int SaveSocialMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserID)
        {
            return _provider.ExecuteScalar<int>("SaveSocialMessageAttachment", messageAttachment.MessageID, messageAttachment.FileID,createUpdateUserID);
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