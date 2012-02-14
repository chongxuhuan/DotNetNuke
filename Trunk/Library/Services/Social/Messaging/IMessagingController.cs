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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Services.Social.Messaging
{
    public interface IMessagingController
    {
        #region Messaging Business APIs

        MessageRecipient GetSocialMessageRecipient(int messageRecipientId, int userId);

        ///<summary> How long a user needs to wait before user is allowed sending the next message</summary>
        ///<returns>Time in seconds. Returns zero if user has never sent a message</returns>
        /// <param name="sender">Sender's UserInfo</param>
        /// <param name="portalId">PortalId</param>
        int WaitTimeForNextMessage(UserInfo sender, int portalId);

        #endregion

        #region Easy Wrapper APIs

        void MarkRead(int messageRecipientId, int userId);
        void MarkUnRead(int messageRecipientId, int userId);
        void MarkArchived(int messageRecipientId, int userId);
        void MarkUnArchived(int messageRecipientId, int userId);

        IList<MessageItem> GetInbox(int userId, int pageIndex, int pageSize, ref int totalRecords, string sortColumn, bool ascending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus);
        IList<Message> GetSentbox(int userId, int pageIndex, int pageSize, ref int totalRecords);
        IList<MessageItem> GetArchivedMessages(int userId, int pageIndex, int pageSize, ref int totalRecords);


        //Gets the latest 10 messages
        IList<MessageItem> GetRecentMessages(int userId, ref int totalRecords);

        IList<MessageItem> GetRecentMessages(int userId, int pageIndex, int pageSize, ref int totalRecords);

        Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs);

        Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs, UserInfo sender);

        #endregion
    }
}