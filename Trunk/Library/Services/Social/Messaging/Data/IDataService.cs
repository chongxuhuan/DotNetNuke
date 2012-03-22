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

using System.Collections.Generic;
using System.Data;
using DotNetNuke.Services.Social.Messaging.Views;

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

        MessageBoxView GetMessageBoxView(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, MessageSentStatus sentStatus);
        MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords);        
        void UpdateSocialMessageReadStatus(int conversationId, int userId, bool read);
        void UpdateSocialMessageArchivedStatus(int conversationId, int userId, bool archived);
        int CreateMessageReply(int conversationId, string body, int senderUserId, string from, int createUpdateUserId);
        
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
        IDataReader GetSocialMessageAttachment(int messageAttachmentId);
        IList<MessageFileView> GetSocialMessageAttachmentsByMessage(int messageId);
        void DeleteSocialMessageAttachment(int messageAttachmentId);

        #endregion

        #region MessageTypes CRUD

        int SaveMessageType(int messageTypeId, string name, string description, int timeToLive, bool isNotification);
        void DeleteMessageType(int messageTypeId);
        IDataReader GetMessageType(int messageTypeId);
        IDataReader GetMessageTypeByName(string name);

        #endregion
    }
}