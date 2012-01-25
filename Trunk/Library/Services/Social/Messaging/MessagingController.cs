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

        internal const int MESSAGING_MAX_TO = 2000;
        internal const int MESSAGING_MAX_SUBJECT = 400;

        #endregion


        private readonly IDataService _DataService;

        #region "Constructors"

        public MessagingController()
            : this(GetDataService())
        {
        }

        public MessagingController(IDataService dataService)
        {
            //Argument Contract
            Requires.NotNull("dataService", dataService);

            _DataService = dataService;
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



        #region Easy Wrapper APIs

        public void MarkRead(int messageRecipientID)
        {
            _DataService.UpdateSocialMessageStatus(messageRecipientID,(int)Messaging.MessageStatus.Read);
        }

        public void MarkUnRead(int messageRecipientID)
        {
            _DataService.UpdateSocialMessageStatus(messageRecipientID, (int)Messaging.MessageStatus.Unread);
        }

        public void MarkArchived(int messageRecipientID)
        {
            _DataService.UpdateSocialMessageStatus(messageRecipientID, (int)Messaging.MessageStatus.Archived);
        }

        public IList<Message> GetInbox(int userID, int pageIndex, int pageSize, ref int totalRecords)
        {
            var messages= _DataService.GetInbox(userID, pageIndex, pageSize, totalRecords);
            totalRecords = messages.Count;
            return messages;
        }

        public IList<Message> GetRecentMessages(int userID, ref int totalRecords)
        {
            var messages = GetInbox(userID, 1, 10, ref totalRecords);
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

            if (!string.IsNullOrEmpty(subject) && subject.Length > MESSAGING_MAX_SUBJECT)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("SubjectTooBigError", "Subject supplied is too big. Maximum {0}, Actual {1}.", MESSAGING_MAX_SUBJECT, subject.Length));
            }

            var sbTo = new StringBuilder();
            bool replyAllAllowed = true;
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (!string.IsNullOrEmpty(role.RoleName))
                    {
                        sbTo.Append(role.RoleName + ",");
                        replyAllAllowed = false;
                    }
                }
            }

            if (users != null)
            {
                foreach (var user in users)
                    if (!string.IsNullOrEmpty(user.DisplayName)) sbTo.Append(user.DisplayName + ",");                        
            }
            
            if (sbTo.Length == 0)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("EmptyToListFoundError", "Empty To List found while analyzing User and Roles List."));
            }
            
            if (sbTo.Length > MESSAGING_MAX_TO)
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("ToListTooBigError", "To List supplied is too big. Maximum {0}, Actual {1}.", MESSAGING_MAX_TO, sbTo.Length));
            }

            var message = new Message { Body = body, Subject = subject, To = sbTo.ToString(0, sbTo.Length - 1), MessageID = Null.NullInteger, ReplyAllAllowed = replyAllAllowed, SenderUserID = sender.UserID, From = sender.DisplayName};

            message.MessageID = _DataService.SaveSocialMessage(message, UserController.GetCurrentUserInfo().UserID);

            //associate attachments
            if (fileIDs != null)
            {
                foreach (var fileID in fileIDs)
                {
                    var attachment = new MessageAttachment {FileID = fileID, MessageID = message.MessageID};
                    _DataService.SaveSocialMessageAttachment(attachment, UserController.GetCurrentUserInfo().UserID);
                }
            }

            //send message to each Role
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    _DataService.CreateSocialMessageRecipientsForRole(message.MessageID, role.RoleID, (int)MessageStatus.Unread, UserController.GetCurrentUserInfo().UserID);
                }
            }

            //send message to each User
            if (users != null)
            {
                foreach (var user in users)
                {
                    var recipient = new MessageRecipient {MessageID = message.MessageID, UserID = user.UserID, Status = (int)MessageStatus.Unread, RecipientID = Null.NullInteger};
                    _DataService.SaveSocialMessageRecipient(recipient, UserController.GetCurrentUserInfo().UserID);
                }
            }

            return message;
        }

        #endregion

        #endregion

    }
}
