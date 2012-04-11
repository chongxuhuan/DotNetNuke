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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Exceptions;
using DotNetNuke.Services.Social.Notifications.Data;

namespace DotNetNuke.Services.Social.Notifications.Internal
{
    public class NotificationsControllerImpl : INotificationsController
    {
        #region Constants

        internal const int ConstMaxSubject = 400;
        internal const int ConstMaxTo = 2000;

        internal const string FriendRquest = "FriendRequest";
        internal const string FollowerRquest = "FollowerRequest";

        #endregion

        #region Private Variables

        private readonly IDataService _dataService;
        private readonly Messaging.Data.IDataService _messagingDataService;

        #endregion

        #region Constructors

        public NotificationsControllerImpl()
            : this(DataService.Instance, Messaging.Data.DataService.Instance)
        {
        }

        public NotificationsControllerImpl(IDataService dataService, Messaging.Data.IDataService messagingDataService)
        {
            Requires.NotNull("dataService", dataService);
            Requires.NotNull("messagingDataService", messagingDataService);

            _dataService = dataService;
            _messagingDataService = messagingDataService;
        }

        #endregion

        #region Public API

        public virtual NotificationTypeAction AddNotificationTypeActionAfter(int afterNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionAfter(afterNotificationTypeActionId, notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        public virtual NotificationTypeAction AddNotificationTypeActionBefore(int beforeNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionBefore(beforeNotificationTypeActionId, notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        public virtual NotificationTypeAction AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionToEnd(notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        public virtual int CountNotifications(int userId,int portalId)
        {
            return _dataService.CountNotifications(userId, portalId);
        }

        public virtual Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users)
        {
            var sender = GetAdminUser();
            return CreateNotification(notificationTypeId,portalId, subject, body, roles, users, sender);
        }

        public virtual Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, UserInfo sender)
        {
            Requires.NotNull("sender", sender);

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

            var sbTo = new StringBuilder();
            if (roles != null)
            {
                foreach (var role in roles.Where(role => !string.IsNullOrEmpty(role.RoleName)))
                {
                    sbTo.Append(role.RoleName + ",");
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

            //Cannot exceed RecipientLimit
            var recipientCount = 0;
            if (users != null) recipientCount += users.Count;
            if (roles != null) recipientCount += roles.Count;
            if (recipientCount > MessagingController.Instance.RecipientLimit(sender.PortalID))
            {
                throw new RecipientLimitExceededException(Localization.Localization.GetString("MsgRecipientLimitExceeded", Localization.Localization.ExceptionsResourceFile));
            }

            //Profanity Filter
            var profanityFilterSetting = GetPortalSetting("MessagingProfanityFilters", sender.PortalID, "NO");
            if (profanityFilterSetting.Equals("YES", StringComparison.InvariantCultureIgnoreCase))
            {
                subject = InputFilter(subject);
                body = InputFilter(body);
            }

            var notification = new Notification
                                   {
                                       NotificationTypeID = notificationTypeId,
                                       Body = body,
                                       Subject = subject,
                                       To = sbTo.ToString().Trim(','),
                                       SenderUserID = sender.UserID,
                                       From = sender.DisplayName
                                   };

            notification.NotificationID = _dataService.CreateNotification(
                notification.NotificationTypeID,
                portalId,
                notification.To,
                notification.From,
                notification.Subject,
                notification.Body,
                notification.SenderUserID,
                UserController.GetCurrentUserInfo().UserID,
                DateTime.MinValue);

            //send message to Roles
            if (roles != null)
            {
                var roleIds = string.Empty;
                roleIds = roles
                    .Select(r => r.RoleID)
                    .Aggregate(roleIds, (current, roleId) => current + (roleId + ","))
                    .Trim(',');

                _messagingDataService.CreateMessageRecipientsForRole(
                    notification.NotificationID,
                    roleIds,
                    UserController.GetCurrentUserInfo().UserID,
                    DateTime.Now);
            }

            //send message to each User - this should be called after CreateMessageRecipientsForRole.
            if (users == null)
            {
                users = new List<UserInfo>();
            }

            var recipients = from user in users
                             where MessagingController.Instance.GetMessageRecipient(notification.NotificationID, user.UserID) == null
                             select new MessageRecipient
                                        {
                                            MessageID = notification.NotificationID,
                                            UserID = user.UserID,
                                            Read = false,
                                            RecipientID = Null.NullInteger
                                        };

            foreach (var recipient in recipients)
            {
                _messagingDataService.SaveMessageRecipient(
                    recipient,
                    UserController.GetCurrentUserInfo().UserID,
                    DateTime.Now);
            }

            return notification;
        }

        public virtual NotificationAction CreateNotificationAction(int notificationId, int notificationTypeActionId, string key)
        {
            Requires.NotNullOrEmpty("key", key);

            var notificationAction = new NotificationAction
                                         {
                                             NotificationActionId = _dataService.SaveNotificationAction(Null.NullInteger, notificationId, notificationTypeActionId, key, GetCurrentUserId()),
                                             NotificationId = notificationId,
                                             NotificationTypeActionId = notificationTypeActionId,
                                             Key = key
                                         };

            return notificationAction;
        }

        public virtual NotificationType CreateNotificationType(string name, string description, TimeSpan timeToLive, int desktopModuleId)
        {
            Requires.NotNullOrEmpty("name", name);

            var totalMinutes = (int)timeToLive.TotalMinutes;
            var timeToLiveMinutes = totalMinutes == 0 ? Null.NullInteger : totalMinutes;

            if (desktopModuleId <= 0)
            {
                desktopModuleId = Null.NullInteger;
            }

            var messageType = new NotificationType
            {
                NotificationTypeId = _dataService.SaveNotificationType(Null.NullInteger, name, description, timeToLiveMinutes, desktopModuleId, GetCurrentUserId()),
                Name = name,
                Description = description,
                TimeToLive = timeToLive,
                DesktopModuleId = desktopModuleId
            };

            return messageType;
        }

        public virtual void DeleteNotification(int notificationId)
        {
            _dataService.DeleteNotification(notificationId);
        }

        public virtual void DeleteNotificationAction(int notificationActionId)
        {
            _dataService.DeleteNotificationAction(notificationActionId);
        }

        public virtual void DeleteNotificationRecipient(int notificationId, int userId)
        {
            MessagingController.Instance.DeleteMessageRecipient(notificationId, userId);
            var recipients = MessagingController.Instance.GetMessageRecipients(notificationId);
            if (recipients.Count == 0)
            {
                DeleteNotification(notificationId);
            }
        }

        public virtual void DeleteNotificationType(int notificationTypeId)
        {
            _dataService.DeleteNotificationType(notificationTypeId);

            RemoveNotificationTypeCache();
        }

        public virtual void DeleteNotificationTypeAction(int notificationTypeActionId)
        {
            _dataService.DeleteNotificationTypeAction(notificationTypeActionId);

            RemoveNotificationTypeActionCache();
        }

        public virtual NotificationAction GetNotificationAction(int notificationActionId)
        {
            return CBO.FillObject<NotificationAction>(_dataService.GetNotificationAction(notificationActionId));
        }

        public virtual NotificationAction GetNotificationAction(int notificationId, int notificationTypeActionId)
        {
            return CBO.FillObject<NotificationAction>(_dataService.GetNotificationActionByMessageAndNotificationTypeAction(notificationId, notificationTypeActionId));
        }

        public virtual IList<NotificationAction> GetNotificationActionsByNotificationId(int notificationId)
        {
            return CBO.FillCollection<NotificationAction>(_dataService.GetNotificationActionsByMessageId(notificationId));
        }

        public virtual IList<Notification> GetNotifications(int userId, int portalId,int pageIndex, int pageSize)
        {
            return CBO.FillCollection<Notification>(_dataService.GetNotifications(userId, portalId, pageIndex, pageSize));
        }

        public virtual NotificationType GetNotificationType(int notificationTypeId)
        {
            var notificationTypeCacheKey = string.Format(DataCache.NotificationTypesCacheKey, notificationTypeId);
            var cacheItemArgs = new CacheItemArgs(notificationTypeCacheKey, DataCache.NotificationTypesTimeOut, DataCache.NotificationTypesCachePriority, notificationTypeId);
            return CBO.GetCachedObject<NotificationType>(cacheItemArgs, GetNotificationTypeCallBack);
        }

        public virtual NotificationType GetNotificationType(string name)
        {
            Requires.NotNullOrEmpty("name", name);

            var notificationTypeCacheKey = string.Format(DataCache.NotificationTypesCacheKey, name);
            var cacheItemArgs = new CacheItemArgs(notificationTypeCacheKey, DataCache.NotificationTypesTimeOut, DataCache.NotificationTypesCachePriority, name);
            return CBO.GetCachedObject<NotificationType>(cacheItemArgs, GetNotificationTypeByNameCallBack);
        }

        public virtual NotificationTypeAction GetNotificationTypeAction(int notificationTypeActionId)
        {
            var notificationTypeActionCacheKey = string.Format(DataCache.NotificationTypeActionsCacheKey, notificationTypeActionId);
            var cacheItemArgs = new CacheItemArgs(notificationTypeActionCacheKey, DataCache.NotificationTypeActionsTimeOut, DataCache.NotificationTypeActionsPriority, notificationTypeActionId);
            return CBO.GetCachedObject<NotificationTypeAction>(cacheItemArgs, GetNotificationTypeActionCallBack);
        }

        public virtual NotificationTypeAction GetNotificationTypeAction(int notificationTypeId, string name)
        {
            Requires.NotNullOrEmpty("name", name);

            var notificationTypeActionCacheKey = string.Format(DataCache.NotificationTypeActionsByNameCacheKey, notificationTypeId, name);
            var cacheItemArgs = new CacheItemArgs(notificationTypeActionCacheKey, DataCache.NotificationTypeActionsTimeOut, DataCache.NotificationTypeActionsPriority, notificationTypeId, name);
            return CBO.GetCachedObject<NotificationTypeAction>(cacheItemArgs, GetNotificationTypeActionByNameCallBack);
        }

        public virtual IList<NotificationTypeAction> GetNotificationTypeActions(int notificationTypeId)
        {
            return CBO.FillCollection<NotificationTypeAction>(_dataService.GetNotificationTypeActions(notificationTypeId));
        }

        public virtual void UpdateNotificationAction(NotificationAction notificationAction)
        {
            Requires.NotNull("notificationAction", notificationAction);

            _dataService.SaveNotificationAction(
                notificationAction.NotificationActionId,
                notificationAction.NotificationId,
                notificationAction.NotificationTypeActionId,
                notificationAction.Key,
                GetCurrentUserId());
        }

        public virtual void UpdateNotificationType(NotificationType notificationType)
        {
            Requires.NotNull("NotificationType", notificationType);

            var totalMinutes = (int)notificationType.TimeToLive.TotalMinutes;
            var timeToLiveMinutes = totalMinutes == 0 ? Null.NullInteger : totalMinutes;

            _dataService.SaveNotificationType(
                notificationType.NotificationTypeId,
                notificationType.Name,
                notificationType.Description,
                timeToLiveMinutes,
                notificationType.DesktopModuleId,
                GetCurrentUserId());

            RemoveNotificationTypeCache();
        }

        public virtual void UpdateNotificationTypeAction(NotificationTypeAction notificationTypeAction)
        {
            Requires.NotNull("notificationTypeAction", notificationTypeAction);

            _dataService.UpdateNotificationTypeAction(
                notificationTypeAction.NotificationTypeActionId,
                notificationTypeAction.NameResourceKey,
                notificationTypeAction.DescriptionResourceKey,
                notificationTypeAction.ConfirmResourceKey,
                notificationTypeAction.APICall,
                GetCurrentUserId());

            RemoveNotificationTypeActionCache();
        }

        #endregion

        #region Internal Methods

        internal virtual UserInfo GetAdminUser()
        {
            return UserController.GetUserById(PortalSettings.Current.PortalId, PortalSettings.Current.AdministratorId);
        }

        internal virtual int GetCurrentUserId()
        {
            return UserController.GetCurrentUserInfo().UserID;
        }

        internal virtual object GetNotificationTypeActionCallBack(CacheItemArgs cacheItemArgs)
        {
            var notificationTypeActionId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillObject<NotificationTypeAction>(_dataService.GetNotificationTypeAction(notificationTypeActionId));
        }

        internal virtual object GetNotificationTypeActionByNameCallBack(CacheItemArgs cacheItemArgs)
        {
            var notificationTypeId = (int)cacheItemArgs.ParamList[0];
            var name = cacheItemArgs.ParamList[1].ToString();
            return CBO.FillObject<NotificationTypeAction>(_dataService.GetNotificationTypeActionByName(notificationTypeId, name));
        }

        internal virtual object GetNotificationTypeByNameCallBack(CacheItemArgs cacheItemArgs)
        {
            var notificationName = cacheItemArgs.ParamList[0].ToString();
            return CBO.FillObject<NotificationType>(_dataService.GetNotificationTypeByName(notificationName));
        }

        internal virtual object GetNotificationTypeCallBack(CacheItemArgs cacheItemArgs)
        {
            var notificationTypeId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillObject<NotificationType>(_dataService.GetNotificationType(notificationTypeId));
        }

        internal virtual string GetPortalSetting(string settingName, int portalId, string defaultValue)
        {
            return PortalController.GetPortalSetting(settingName, portalId, defaultValue);
        }

        internal virtual string InputFilter(string input)
        {
            var ps = new PortalSecurity();
            return ps.InputFilter(input, PortalSecurity.FilterFlag.NoProfanity);
        }

        internal virtual void RemoveNotificationTypeActionCache()
        {
            DataCache.ClearCache("NotificationTypeActions:");
        }

        internal virtual void RemoveNotificationTypeCache()
        {
            DataCache.ClearCache("NotificationTypes:");
        }

        #endregion
    }
}
