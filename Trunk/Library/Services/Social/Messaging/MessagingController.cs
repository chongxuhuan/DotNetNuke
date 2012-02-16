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
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging.Exceptions;

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
        private readonly IEventLogController _eventLogController;

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

        public MessageRecipient GetSocialMessageRecipient(int messageRecipientId, int userId)
        {
            return CBO.FillObject<MessageRecipient>(_dataService.GetSocialMessageRecipientByMessageAndUser(messageRecipientId, userId));
        }

        ///<summary> How long a user needs to wait before user is allowed sending the next message</summary>
        ///<returns>Time in seconds. Returns zero if user has never sent a message</returns>
        /// <param name="sender">Sender's UserInfo</param>        
        public int WaitTimeForNextMessage(UserInfo sender)
        {
            int waitTime = 0;
            var interval = PortalController.GetPortalSettingAsInteger("MessagingThrottlingInterval", sender.PortalID, Null.NullInteger);
            if (interval > 0 && (!IsAdminOrHost(sender)))
            {
                int totalRecords = 0;
                waitTime = DateTime.Now.Subtract(GetSentbox(sender.UserID, 0, 1, ref totalRecords).First().CreatedOnDate).Seconds;
                //var messages = GetSentbox(sender.UserID, 0, 1, ref totalRecords);
                //if(messages != null )
                {
                    waitTime = DateTime.Now.Subtract(GetSentbox(sender.UserID, 0, 1, ref totalRecords).First().CreatedOnDate).Seconds;
                }

            }

            return waitTime;
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
            return GetRecentMessages(userId, ConstDefaultPageIndex, ConstDefaultPageSize, ref totalRecords);
        }

        public IList<MessageItem> GetRecentMessages(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            return GetRecentMessages(userId, pageIndex, pageSize, ConstSortColumnDate, !ConstAscending, ref totalRecords);
        }

        public IList<MessageItem> GetRecentMessages(int userId, int pageIndex, int pageSize, string sortColumn, bool sortAscending, ref int totalRecords)
        {
            var messages = GetInbox(userId, pageIndex, pageSize, ref totalRecords, sortColumn, sortAscending, MessageReadStatus.Any, MessageArchivedStatus.UnArchived);
            return messages;
        }

        public IList<MessageItem> GetArchivedMessages(int userId, int pageIndex, int pageSize, ref int totalRecords)
        {
            var messages = GetInbox(userId, pageIndex, pageSize, ref totalRecords, ConstSortColumnDate, !ConstAscending, MessageReadStatus.Any, MessageArchivedStatus.Archived);
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
                foreach (var recipient in from user in users where GetSocialMessageRecipient(message.MessageID, user.UserID) == null select new MessageRecipient {MessageID = message.MessageID, UserID = user.UserID, Read = false, RecipientID = Null.NullInteger})
                {
                    _dataService.SaveSocialMessageRecipient(recipient, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return message;
        }

        public Message ReplyMessage(int parentMessageId, string body, IList<int> fileIDs)
        {
            return ReplyMessage(parentMessageId, body, fileIDs, UserController.GetCurrentUserInfo());
        }


        public Message ReplyMessage(int parentMessageId, string body, IList<int> fileIDs, UserInfo sender)
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


            return null;
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
