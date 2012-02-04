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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging.Data;

#endregion

namespace DotNetNuke.Services.Social.Messaging
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The Controller class for social Messaging
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public class MessagingController : IMessagingController
    {
        #region constants

        internal const int ConstMaxTo = 2000;
        internal const int ConstMaxSubject = 400;
        internal const int ConstDefaultPageIndex = 0;
        internal const int ConstDefaultPageSize = 10;
        internal const string ConstSortColumnDate = "CreatedOnDate";
        internal const string ConstSortColumnFrom = "From";
        internal const string ConstSortColumnSubject = "Subject";
        internal const bool ConstAscending = true;        

        #endregion


        private readonly IDataService _dataService;

        #region "Constructors"

        public MessagingController()
            : this(GetDataService())
        {
        }

        public MessagingController(IDataService dataService)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);

            _dataService = dataService;
        }

        #endregion

        #region Private Shared Methods

        private static IDataService GetDataService()
        {
            var ds = ComponentFactory.GetComponent<IDataService>();

            if (ds == null)
            {
                ds = new DataService();
                ComponentFactory.RegisterComponentInstance<IDataService>(ds);
            }
            return ds;
        }

        #endregion


        #region Public Methods

        #region Messaging Business APIs

        public MessageRecipient GetSocialMessageRecipient(int messageRecipientId, int userId)
        {
            return CBO.FillObject<MessageRecipient>(_dataService.GetSocialMessageRecipientByMessageAndUser(messageRecipientId, userId));
        }

        #endregion

        #region Easy Wrapper APIs

        public void MarkRead(int messageRecipientId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(messageRecipientId, userId, true);
        }

        public void MarkUnRead(int messageRecipientId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(messageRecipientId, userId, false);
        }

        public void MarkArchived(int messageRecipientId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(messageRecipientId, userId, true);
        }

        public void MarkUnArchived(int messageRecipientId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(messageRecipientId, userId, false);
        }

        public IList<MessageItem> GetInbox(int userId, int pageIndex, int pageSize, ref int totalRecords, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus)
        {
            return _dataService.GetInbox(userId, pageIndex, pageSize, ref totalRecords, sortColumn, sortAscending, readStatus, archivedStatus);
        }

        public IList<Message> GetSentbox(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return _dataService.GetSentbox(userId, pageIndex, pageSize, ref totalRecords);
        }

        public IList<MessageItem> GetRecentMessages(int userId, ref int totalRecords)
        {
            var messages = GetInbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize, ref totalRecords, ConstSortColumnDate, !ConstAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
            return messages;
        }

        public IList<MessageItem> GetArchivedMessages(int userId, ref int totalRecords)
        {
            var messages = GetInbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize, ref totalRecords, ConstSortColumnDate, !ConstAscending, MessageReadStatus.Any, MessageArchivedStatus.Archived);
            return messages;
        }


        public Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs)
        {
            return CreateMessage(subject, body, roles, users, fileIDs, UserController.GetCurrentUserInfo());
        }

        public Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs, UserInfo sender)
        {
            if (sender == null || sender.UserID <= 0)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("SenderRequiredError", "Either Sender is null or Sender.UserID is negative."));
            }

            if(string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body))
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("SubjectOrBodyRequiredError", "Both Subject and Body cannot be null or empty."));
            }

            if (roles == null && users == null)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("RolesOrUsersRequiredError", "Both Roles and Users cannot be null or empty-lists."));
            }

            if (!string.IsNullOrEmpty(subject) && subject.Length > ConstMaxSubject)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("SubjectTooBigError", "Subject supplied is too big. Maximum {0}, Actual {1}.", ConstMaxSubject, subject.Length));
            }

            var sbTo = new StringBuilder();
            bool replyAllAllowed = true;
            if (roles != null)
            {
                foreach (var role in roles.Where(role => !string.IsNullOrEmpty(role.RoleName)))
                {
                    sbTo.Append(role.RoleName + ",");
                    replyAllAllowed = false;
                }
            }

            if (users != null)
            {
                foreach (var user in users.Where(user => !string.IsNullOrEmpty(user.DisplayName))) sbTo.Append(user.DisplayName + ",");                        
            }
            
            if (sbTo.Length == 0)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("EmptyToListFoundError", "Empty To List found while analyzing User and Roles List."));
            }
            
            if (sbTo.Length > ConstMaxTo)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("ToListTooBigError", "To List supplied is too big. Maximum {0}, Actual {1}.", ConstMaxTo, sbTo.Length));
            }

            var message = new Message { Body = body, Subject = subject, To = sbTo.ToString().Trim(','), MessageID = Null.NullInteger, ReplyAllAllowed = replyAllAllowed, SenderUserID = sender.UserID, From = sender.DisplayName};

            message.MessageID = _dataService.SaveSocialMessage(message, UserController.GetCurrentUserInfo().UserID);

            //associate attachments
            if (fileIDs != null)
            {
                foreach (var attachment in fileIDs.Select(fileId => new MessageAttachment {FileID = fileId, MessageID = message.MessageID}))
                {
                    _dataService.SaveSocialMessageAttachment(attachment, UserController.GetCurrentUserInfo().UserID);
                }
            }

            //send message to Roles
            if (roles != null)
            {
                var roleIds = string.Empty;
                roleIds = roles
                    .Select(r => r.RoleID)
                    .Aggregate(roleIds, (current, roleId) => current + (roleId + ","))
                    .Trim(',');

                _dataService.CreateSocialMessageRecipientsForRole(message.MessageID, roleIds, UserController.GetCurrentUserInfo().UserID);
            }

            //send message to each User - this should be called after CreateSocialMessageRecipientsForRole.
            if (users != null)
            {
                foreach (var recipient in from user in users where GetSocialMessageRecipient(message.MessageID, user.UserID) == null select new MessageRecipient {MessageID = message.MessageID, UserID = user.UserID, Read = false, RecipientID = Null.NullInteger})
                {
                    _dataService.SaveSocialMessageRecipient(recipient, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return message;
        }

        #endregion

        #endregion

    }
}
