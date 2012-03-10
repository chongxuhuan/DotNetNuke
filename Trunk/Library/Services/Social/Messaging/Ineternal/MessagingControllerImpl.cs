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
using System.Linq;
using System.Text;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Security;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging.Exceptions;
using DotNetNuke.Services.Social.Messaging.Views;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Ineternal
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
    public class MessagingControllerImpl : IMessagingController
    {
        #region Constants

        internal const int ConstMaxTo = 2000;
        internal const int ConstMaxSubject = 400;
        internal const int ConstDefaultPageIndex = 0;
        internal const int ConstDefaultPageSize = 10;
        internal const string ConstSortColumnDate = "CreatedOnDate";
        internal const string ConstSortColumnFrom = "From";
        internal const string ConstSortColumnSubject = "Subject";
        internal const bool ConstAscending = true;

        #endregion

        #region Private Variables

        private readonly IDataService _dataService;

        #endregion

        #region Constructors

        public MessagingControllerImpl()
            : this(DataService.Instance)
        {
        }

        public MessagingControllerImpl(IDataService dataService)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);

            _dataService = dataService;
        }

        #endregion

        #region Public Methods

        ///<summary>Are attachments allowed</summary>        
        ///<returns>True or False</returns>
        /// <param name="portalId">Portal Id</param>               
        public virtual bool AttachmentsAllowed(int portalId)
        {
            return GetPortalSetting("MessagingAllowAttachments", portalId, "YES") == "YES";
        }

        public virtual Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs)
        {
            return CreateMessage(subject, body, roles, users, fileIDs, UserController.GetCurrentUserInfo());
        }

        public virtual Message CreateMessage(string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, IList<int> fileIDs, UserInfo sender)
        {
            if (sender == null || sender.UserID <= 0)
            {
                throw new ArgumentException(Localization.Localization.GetString("MsgSenderRequiredError", Localization.Localization.ExceptionsResourceFile));
            }

            if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body))
            {
                throw new ArgumentException(Localization.Localization.GetString("MsgSubjectOrBodyRequiredError", Localization.Localization.ExceptionsResourceFile));
            }

            if (roles == null && users == null)
            {
                throw new ArgumentException(Localization.Localization.GetString("MsgRolesOrUsersRequiredError", Localization.Localization.ExceptionsResourceFile));
            }

            if (!string.IsNullOrEmpty(subject) && subject.Length > ConstMaxSubject)
            {
                throw new ArgumentException(string.Format(Localization.Localization.GetString("MsgSubjectTooBigError", Localization.Localization.ExceptionsResourceFile), ConstMaxSubject, subject.Length));
            }

            if (roles != null && roles.Count > 0)
            {
                if (!IsAdminOrHost(sender))
                {
                    throw new ArgumentException(Localization.Localization.GetString("MsgOnlyHostOrAdminCanSendToRoleError", Localization.Localization.ExceptionsResourceFile));
                }
            }

            var sbTo = new StringBuilder();
            var replyAllAllowed = true;
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
                throw new ArgumentException(Localization.Localization.GetString("MsgEmptyToListFoundError", Localization.Localization.ExceptionsResourceFile));
            }

            if (sbTo.Length > ConstMaxTo)
            {
                throw new ArgumentException(string.Format(Localization.Localization.GetString("MsgToListTooBigError", Localization.Localization.ExceptionsResourceFile), ConstMaxTo, sbTo.Length));
            }

            //Cannot send message if within ThrottlingInterval
            if (WaitTimeForNextMessage(sender) > 0)
            {
                throw new ThrottlingIntervalNotMetException(Localization.Localization.GetString("MsgThrottlingIntervalNotMet", Localization.Localization.ExceptionsResourceFile));
            }

            //Cannot have attachments if it's not enabled
            if (fileIDs != null && !AttachmentsAllowed(sender.PortalID))
            {
                throw new AttachmentsNotAllowed(Localization.Localization.GetString("MsgAttachmentsNotAllowed", Localization.Localization.ExceptionsResourceFile));
            }

            //Cannot exceed RecipientLimit
            var recipientCount = 0;
            if (users != null) recipientCount += users.Count;
            if (roles != null) recipientCount += roles.Count;
            if (recipientCount > RecipientLimit(sender.PortalID))
            {
                throw new RecipientLimitExceeded(Localization.Localization.GetString("MsgRecipientLimitExceeded", Localization.Localization.ExceptionsResourceFile));
            }

            //Profanity Filter
            var profanityFilterSetting = GetPortalSetting("MessagingProfanityFilters", sender.PortalID, "NO");
            if (profanityFilterSetting.Equals("YES", StringComparison.InvariantCultureIgnoreCase))
            {
                subject = InputFilter(subject);
                body = InputFilter(body);
            }

            var message = new Message { Body = body, Subject = subject, To = sbTo.ToString().Trim(','), MessageID = Null.NullInteger, ReplyAllAllowed = replyAllAllowed, SenderUserID = sender.UserID, From = sender.DisplayName };

            message.MessageID = _dataService.SaveSocialMessage(message, UserController.GetCurrentUserInfo().UserID);

            //associate attachments
            if (fileIDs != null)
            {
                foreach (var attachment in fileIDs.Select(fileId => new MessageAttachment { MessageAttachmentID = Null.NullInteger, FileID = fileId, MessageID = message.MessageID }))
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
                foreach (var recipient in from user in users where GetSocialMessageRecipient(message.MessageID, user.UserID) == null select new MessageRecipient { MessageID = message.MessageID, UserID = user.UserID, Read = false, RecipientID = Null.NullInteger })
                {
                    _dataService.SaveSocialMessageRecipient(recipient, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return message;
        }

        public virtual MessageBoxView GetArchivedMessages(int userId, int pageIndex, int pageSize)
        {
            return GetInbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, MessageReadStatus.Any, MessageArchivedStatus.Archived);
        }

        public virtual MessageBoxView GetInbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending)
        {
            return GetInbox(userId, pageIndex, pageSize, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
        }

        public virtual MessageBoxView GetInbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus)
        {
            return _dataService.GetMessageBoxView(userId, pageIndex, pageSize, sortColumn, sortAscending, readStatus, archivedStatus, MessageSentStatus.Received);
        }

        public virtual MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetMessageThread(conversationId, userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public virtual MessageThreadsView GetMessageThread(int conversationId, int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            return _dataService.GetMessageThread(conversationId, userId, pageIndex, pageSize, sortColumn, sortAscending, ref totalRecords);
        }

        public virtual MessageBoxView GetRecentInbox(int userId)
        {
            return GetRecentInbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize);
        }

        public virtual MessageBoxView GetRecentInbox(int userId, int pageIndex, int pageSize)
        {
            return GetInbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending);
        }

        public virtual MessageBoxView GetRecentSentbox(int userId)
        {
            return GetRecentSentbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize);
        }

        public virtual MessageBoxView GetRecentSentbox(int userId, int pageIndex, int pageSize)
        {
            return GetSentbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending);
        }

        public virtual MessageBoxView GetSentbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending)
        {
            return GetSentbox(userId, pageIndex, pageSize, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
        }

        public virtual MessageBoxView GetSentbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus)
        {
            return _dataService.GetMessageBoxView(userId, pageIndex, pageSize, sortColumn, sortAscending, readStatus, archivedStatus, MessageSentStatus.Sent);
        }

        public virtual MessageRecipient GetSocialMessageRecipient(int messageRecipientId, int userId)
        {
            return CBO.FillObject<MessageRecipient>(_dataService.GetSocialMessageRecipientByMessageAndUser(messageRecipientId, userId));
        }

        public virtual void MarkArchived(int conversationId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(conversationId, userId, true);
        }

        public virtual void MarkRead(int conversationId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(conversationId, userId, true);
        }

        public virtual void MarkUnArchived(int conversationId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(conversationId, userId, false);
        }

        public virtual void MarkUnRead(int conversationId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(conversationId, userId, false);
        }

        ///<summary>Maximum number of Recipients allowed</summary>        
        ///<returns>Count. Message to a Role is considered a single Recipient. Each User in the To list is counted as one User each.</returns>
        /// <param name="portalId">Portal Id</param>        
        public virtual int RecipientLimit(int portalId)
        {
            return GetPortalSettingAsInteger("MessagingRecipientLimit", portalId, 5);
        }

        public virtual int ReplyMessage(int conversationId, string body, IList<int> fileIDs)
        {
            return ReplyMessage(conversationId, body, fileIDs, UserController.GetCurrentUserInfo());
        }

        public virtual int ReplyMessage(int conversationId, string body, IList<int> fileIDs, UserInfo sender)
        {
            if (sender == null || sender.UserID <= 0)
            {
                throw new ArgumentException(Localization.Localization.GetString("MsgSenderRequiredError", Localization.Localization.ExceptionsResourceFile));
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentException(Localization.Localization.GetString("MsgBodyRequiredError", Localization.Localization.ExceptionsResourceFile));
            }

            //Cannot have attachments if it's not enabled
            if (fileIDs != null && !AttachmentsAllowed(sender.PortalID))
            {
                throw new AttachmentsNotAllowed(Localization.Localization.GetString("MsgAttachmentsNotAllowed", Localization.Localization.ExceptionsResourceFile));
            }

            //Profanity Filter
            var profanityFilterSetting = GetPortalSetting("MessagingProfanityFilters", sender.PortalID, "NO");
            if (profanityFilterSetting.Equals("YES", StringComparison.InvariantCultureIgnoreCase))
            {
                body = InputFilter(body);
            }

            //call ReplyMessage
            var messageId = _dataService.CreateMessageReply(conversationId, body, sender.UserID, sender.DisplayName, GetCurrentUserInfo().UserID);
            if (messageId == -1) //Parent message was not found or Recipient was not found in the message
            {
                throw new MessageOrRecipientNotFound(Localization.Localization.GetString("MsgMessageOrRecipientNotFound", Localization.Localization.ExceptionsResourceFile));
            }

            //associate attachments
            if (fileIDs != null)
            {
                foreach (var attachment in fileIDs.Select(fileId => new MessageAttachment { MessageAttachmentID = Null.NullInteger, FileID = fileId, MessageID = messageId }))
                {
                    _dataService.SaveSocialMessageAttachment(attachment, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return messageId;
        }

        /// <summary>How long a user needs to wait before sending the next message.</summary>
        /// <returns>Time in seconds. Returns zero if user is Host, Admin or has never sent a message.</returns>
        /// <param name="sender">Sender's UserInfo</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual int WaitTimeForNextMessage(UserInfo sender)
        {
            Requires.NotNull("sender", sender);

            var waitTime = 0;
            // MessagingThrottlingInterval contains the number of MINUTES to wait before sending the next message
            var interval = GetPortalSettingAsInteger("MessagingThrottlingInterval", sender.PortalID, Null.NullInteger) * 60;
            if (interval > 0 && !IsAdminOrHost(sender))
            {
                var messageBoxView = GetRecentSentbox(sender.UserID, 0, 1);
                if (messageBoxView != null && messageBoxView.TotalConversations > 0)
                {
                    waitTime = (int)(interval - GetDateTimeNow().Subtract(messageBoxView.Conversations.First().CreatedOnDate).TotalSeconds);
                }
            }
            return waitTime < 0 ? 0 : waitTime;
        }

        #endregion

        #region Internal Methods

        internal virtual UserInfo GetCurrentUserInfo()
        {
            return UserController.GetCurrentUserInfo();
        }

        internal virtual DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        internal virtual string GetPortalSetting(string settingName, int portalId, string defaultValue)
        {
            return PortalController.GetPortalSetting(settingName, portalId, defaultValue);
        }

        internal virtual int GetPortalSettingAsInteger(string key, int portalId, int defaultValue)
        {
            return PortalController.GetPortalSettingAsInteger(key, portalId, defaultValue);
        }

        internal virtual string InputFilter(string input)
        {
            var ps = new PortalSecurity();
            return ps.InputFilter(input, PortalSecurity.FilterFlag.NoProfanity);
        }

        internal virtual bool IsAdminOrHost(UserInfo userInfo)
        {
            return userInfo.IsSuperUser || userInfo.IsInRole(TestablePortalSettings.Instance.AdministratorRoleName);
        }

        #endregion
    }
}
