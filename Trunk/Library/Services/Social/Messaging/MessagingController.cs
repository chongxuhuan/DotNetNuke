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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging.Exceptions;
using DotNetNuke.Services.Social.Messaging.Views;

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
    public class MessagingController : ComponentBase<IMessagingController, MessagingController>, IMessagingController
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

        #region Constructors

        public MessagingController() : this(DataService.Instance)
        {
        }

        public MessagingController(IDataService dataService)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);

            _dataService = dataService;            
        }

        #endregion


        #region Public Methods

        #region Messaging Business APIs

        public MessageRecipient GetSocialMessageRecipient(int messageRecipientId, int userId, MessageSentStatus sentStatus)
        {
            return CBO.FillObject<MessageRecipient>(_dataService.GetSocialMessageRecipientByMessageAndUser(messageRecipientId, userId, sentStatus));
        }

        ///<summary> How long a user needs to wait before user is allowed sending the next message</summary>
        ///<returns>Time in seconds. Returns zero if user has never sent a message</returns>
        /// <param name="sender">Sender's UserInfo</param>        
        public int WaitTimeForNextMessage(UserInfo sender)
        {
            var waitTime = 0;
            var interval = PortalController.GetPortalSettingAsInteger("MessagingThrottlingInterval", sender.PortalID, Null.NullInteger);
            if (interval > 0 && !IsAdminOrHost(sender))
            {
                var totalRecords = 0;
                var messages = GetRecentSentbox(sender.UserID, 0, 1, ref totalRecords);
                if (totalRecords > 0)
                {
                    waitTime = interval - DateTime.Now.Subtract(messages.First().CreatedOnDate).Seconds;
                }
            }
            return waitTime < 0 ? 0 : waitTime;
        }

        #endregion

        #region Easy Wrapper APIs

        public void MarkRead(int parentMessageId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(parentMessageId, userId, true);
        }

        public void MarkUnRead(int parentMessageId, int userId)
        {
            _dataService.UpdateSocialMessageReadStatus(parentMessageId, userId, false);
        }

        public void MarkArchived(int parentMessageId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(parentMessageId, userId, true);
        }

        public void MarkUnArchived(int parentMessageId, int userId)
        {
            _dataService.UpdateSocialMessageArchivedStatus(parentMessageId, userId, false);
        }

        public IList<MessageItemView> GetInbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, ref int totalRecords)
        {
            return _dataService.GetMessageItems(userId, pageIndex, pageSize, sortColumn, sortAscending, readStatus, archivedStatus, MessageSentStatus.Received, ref totalRecords);
        }

        public IList<MessageItemView> GetInbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            var messages = GetInbox(userId, pageIndex, pageSize, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived, ref totalRecords);
            return messages;
        }

        public IList<MessageItemView> GetRecentInbox(int userId, ref int totalRecords)
        {
            return GetRecentInbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize, ref totalRecords);
        }

        public IList<MessageItemView> GetRecentInbox(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetInbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public IList<MessageItemView> GetSentbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, MessageReadStatus readStatus, MessageArchivedStatus archivedStatus, ref int totalRecords)
        {
            return _dataService.GetMessageItems(userId, pageIndex, pageSize, sortColumn, sortAscending, readStatus, archivedStatus, MessageSentStatus.Sent, ref totalRecords);
        }

        public IList<MessageItemView> GetSentbox(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            var messages = GetSentbox(userId, pageIndex, pageSize, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived, ref totalRecords);
            return messages;
        }

        public IList<MessageItemView> GetRecentSentbox(int userId, ref int totalRecords)
        {
            return GetRecentSentbox(userId, ConstDefaultPageIndex, ConstDefaultPageSize, ref totalRecords);
        }

        public IList<MessageItemView> GetRecentSentbox(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetSentbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public IList<MessageThreadView> GetMessageThread(int messageId, int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            var threads = new List<MessageThreadView>();

            var messages = _dataService.GetMessageThread(messageId, userId, pageIndex, pageSize, sortColumn, sortAscending, ref totalRecords);
            foreach (var messageItemView in messages)
            {
                var thread = new MessageThreadView();
                thread.MessageItem = messageItemView;
                if (messageItemView.AttachmentCount > 0)
                {
                    //TODO Add Attachment details from File Providers                    
                }                
            }

            return threads;
        }

        public IList<MessageThreadView> GetMessageThread(int messageId, int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetMessageThread(messageId, userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public IList<MessageItemView> GetArchivedMessages(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            var messages = GetInbox(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, MessageReadStatus.Any, MessageArchivedStatus.Archived, ref totalRecords);
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
                throw new ArgumentException(Localization.Localization.GetString("MsgSenderRequiredError", Localization.Localization.ExceptionsResourceFile));                
            }

            if(string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body))
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
            if(recipientCount > RecipientLimit(sender.PortalID))
            {
                throw new RecipientLimitExceeded(Localization.Localization.GetString("MsgRecipientLimitExceeded", Localization.Localization.ExceptionsResourceFile));
            }

            var message = new Message { Body = body, Subject = subject, To = sbTo.ToString().Trim(','), MessageID = Null.NullInteger, ReplyAllAllowed = replyAllAllowed, SenderUserID = sender.UserID, From = sender.DisplayName};

            message.MessageID = _dataService.SaveSocialMessage(message, UserController.GetCurrentUserInfo().UserID);

            /*
            //create sent record
            var sendRecord = new MessageRecipient
                                 {
                                     MessageID = message.MessageID,
                                     UserID = sender.UserID,
                                     Read = false,
                                     Sent = true,
                                     RecipientID = Null.NullInteger
                                 };

            _dataService.SaveSocialMessageRecipient(sendRecord, UserController.GetCurrentUserInfo().UserID);
             */ 

            //associate attachments
            if (fileIDs != null)
            {
                foreach (var attachment in fileIDs.Select(fileId => new MessageAttachment {MessageAttachmentID = Null.NullInteger, FileID = fileId, MessageID = message.MessageID}))
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
                foreach (var recipient in from user in users where GetSocialMessageRecipient(message.MessageID, user.UserID, MessageSentStatus.Received) == null select new MessageRecipient {MessageID = message.MessageID, UserID = user.UserID, Read = false, Sent = false, RecipientID = Null.NullInteger})
                {
                    _dataService.SaveSocialMessageRecipient(recipient, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return message;
        }

        public int ReplyMessage(int parentMessageId, string body, IList<int> fileIDs)
        {
            return ReplyMessage(parentMessageId, body, fileIDs, UserController.GetCurrentUserInfo());
        }


        public int ReplyMessage(int parentMessageId, string body, IList<int> fileIDs, UserInfo sender)
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
            
            //call ReplyMessage
            var messageId = _dataService.CreateMessageReply(parentMessageId, body, sender.UserID, sender.DisplayName, UserController.GetCurrentUserInfo().UserID);
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


        ///<summary>Are attachments allowed</summary>        
        ///<returns>True or False</returns>
        /// <param name="portalId">Portal Id</param>               
        public bool AttachmentsAllowed(int portalId)
        {
            return PortalController.GetPortalSetting("MessagingAllowAttachments", portalId, "YES") == "YES";
        }

        ///<summary>Maximum number of Recipients allowed</summary>        
        ///<returns>Count. Message to a Role is considered a single Recipient. Each User in the To list is counted as one User each.</returns>
        /// <param name="portalId">Portal Id</param>        
        public int RecipientLimit(int portalId)
        {
            return PortalController.GetPortalSettingAsInteger("MessagingRecipientLimit", portalId, 5);
        }

        #endregion

        #region Private Methods

        private bool IsAdminOrHost(UserInfo userInfo)
        {
            return userInfo.IsSuperUser || userInfo.IsInRole(TestablePortalSettings.Instance.AdministratorRoleName);
        }

        #endregion

        #endregion

    }
}
