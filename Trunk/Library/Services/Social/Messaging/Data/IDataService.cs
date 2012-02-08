#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
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

#endregion

namespace DotNetNuke.Services.Social.Messaging.Data
{
    public interface IDataService
    {
        #region Messages CRUD
        
        int SaveSocialMessage(Message message, int createUpdateUserId);
        IDataReader GetSocialMessage();
        IDataReader GetSocialMessagesBySender();
        void DeleteSocialMessage(int messageId);

        IList<MessageItem> GetInbox(int userId, int pageIndex, int pageSize, ref int totalRecords, string sortColumn, bool ascending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus);
        IList<Message> GetSentbox(int userId, int pageIndex, int pageSize, ref int totalRecords);
        void UpdateSocialMessageReadStatus(int recipientId, int userId, bool read);
        void UpdateSocialMessageArchivedStatus(int recipientId, int userId, bool archived);
        
        #endregion

        #region Message_Recipients CRUD
        
        int SaveSocialMessageRecipient(MessageRecipient messageRecipient, int createUpdateUserId);
        void CreateSocialMessageRecipientsForRole(int messageId, string roleIds, int createUpdateUserId);
        IDataReader GetSocialMessageRecipient(int messageRecipientId);
        IDataReader GetSocialMessageRecipientsByUser(int userId);
        IDataReader GetSocialMessageRecipientsByMessage(int messageId);
        IDataReader GetSocialMessageRecipientByMessageAndUser(int messageId, int userId);
        void DeleteSocialMessageRecipient(int messageRecipientId);

        #endregion

        #region Message_Attachments CRUD

        int SaveSocialMessageAttachment(MessageAttachment messageAttachment, int createUpdateUserId);
        IDataReader GetSocialMessageAttachment();
        IDataReader GetSocialMessageAttachmentsByMessage();
        void DeleteSocialMessageAttachment(int messageAttachmentID);

        #endregion

        

        
        
    }
}