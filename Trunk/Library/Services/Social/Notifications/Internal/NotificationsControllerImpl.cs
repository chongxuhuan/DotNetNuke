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
    /// <summary>
    /// Provides the methods to work with Notifications, NotificationTypes, NotificationTypeActions and NotificationActions.
    /// </summary>
    public class NotificationsControllerImpl : INotificationsController
    {
        #region Constants

        internal const int ConstMaxSubject = 400;
        internal const int ConstMaxTo = 2000;

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

        /// <summary>
        /// Creates a new notification type action and adds it after the specified notification type action in the actions list for the specified notification type.
        /// </summary>
        /// <param name="afterNotificationTypeActionId">The notification type action identifier after which the new action will be placed.</param>
        /// <param name="notificationTypeId">The notification type action identifier.</param>
        /// <param name="nameResourceKey">The resource key used to localize the action name.</param>
        /// <param name="descriptionResourceKey">The resource key used to localize the action description.</param>
        /// <param name="confirmResourceKey">The resource key used to localize the action confirmation message. Leave it empty if no confirmation is needed.</param>
        /// <param name="apiCall">The service framework url to be called to perform the notification action.</param>
        /// <returns>The new notification type action.</returns>
        /// <exception cref="System.ArgumentException">Thrown when nameResourceKey or apiCall are null or empty.</exception>
        public virtual NotificationTypeAction AddNotificationTypeActionAfter(int afterNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionAfter(afterNotificationTypeActionId, notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        /// <summary>
        /// Creates a new notification type action and adds it before the specified notification type action in the actions list for the specified notification type.
        /// </summary>
        /// <param name="beforeNotificationTypeActionId">The notification type action identifier before which the new action will be placed.</param>
        /// <param name="notificationTypeId">The notification type action identifier.</param>
        /// <param name="nameResourceKey">The resource key used to localize the action name.</param>
        /// <param name="descriptionResourceKey">The resource key used to localize the description name.</param>
        /// <param name="confirmResourceKey">The resource key used to localize the action confirmation message. Leave it empty if no confirmation is needed.</param>
        /// <param name="apiCall">The service framework url to be called to perform the notification action.</param>
        /// <returns>The new notification type action.</returns>
        /// <exception cref="System.ArgumentException">Thrown when nameResourceKey or apiCall are null or empty.</exception>
        public virtual NotificationTypeAction AddNotificationTypeActionBefore(int beforeNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionBefore(beforeNotificationTypeActionId, notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        /// <summary>
        /// Creates a new notification type action and adds it to the end of the actions list for the specified notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="nameResourceKey">The resource key used to localize the action name.</param>
        /// <param name="descriptionResourceKey">The resource key used to localize the action description.</param>
        /// <param name="confirmResourceKey">The resource key used to localize the action confirmation message. Leave it empty if no confirmation is needed.</param>
        /// <param name="apiCall">The service framework url to be called to perform the notification action.</param>
        /// <returns>The new notification type action.</returns>
        /// <exception cref="System.ArgumentException">Thrown when nameResourceKey or apiCall are null or empty.</exception>
        public virtual NotificationTypeAction AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall)
        {
            Requires.NotNullOrEmpty("nameResourceKey", nameResourceKey);
            Requires.NotNullOrEmpty("apiCall", apiCall);

            var notificationTypeActionId = _dataService.AddNotificationTypeActionToEnd(notificationTypeId, nameResourceKey, descriptionResourceKey, confirmResourceKey, apiCall, GetCurrentUserId());
            return GetNotificationTypeAction(notificationTypeActionId);
        }

        /// <summary>
        /// Counts the notifications sent to the provided user in the specified portal.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <returns>The number of notifications sent to the provided user in the specified portal.</returns>
        public virtual int CountNotifications(int userId,int portalId)
        {
            return _dataService.CountNotifications(userId, portalId);
        }

        /// <summary>
        /// Creates a new notification and sets is sender as the portal administrator.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="subject">The notification subject.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="includeDismissAction">Include a dismiss action?</param>
        /// <param name="roles">The list of roles to send the notification to. Leave it as null to send only to individual users.</param>
        /// <param name="users">The list of users to send the notification to. Leave it as null to send only to roles.</param>
        /// <returns>The new notification.</returns>
        public virtual Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, bool includeDismissAction, IList<RoleInfo> roles, IList<UserInfo> users)
        {
            var sender = GetAdminUser();
            return CreateNotification(notificationTypeId,portalId, subject, body, includeDismissAction, roles, users, sender);
        }

        /// <summary>
        /// Creates a new notification.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="subject">The notification subject.</param>
        /// <param name="body">The notification body.</param>
        /// /// <param name="includeDismissAction">Include a dismiss action?</param>
        /// <param name="roles">The list of roles to send the notification to. Leave it as null to send only to individual users.</param>
        /// <param name="users">The list of users to send the notification to. Leave it as null to send only to roles.</param>
        /// <param name="sender">The notification sender.</param>
        /// <returns>The new notification.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when sender is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when subject and body are null or empty, or when roles and users are both null, or when the subject length is too large, or when there are no recipients.</exception>
        /// <exception cref="DotNetNuke.Services.Social.Messaging.Exceptions.RecipientLimitExceededException">Thrown when there are too much recipients.</exception>
        public virtual Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, bool includeDismissAction, IList<RoleInfo> roles, IList<UserInfo> users, UserInfo sender)
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
                                       From = sender.DisplayName,
                                       ExpirationDate = GetExpirationDate(notificationTypeId),
                                       IncludeDismissAction = includeDismissAction
                                   };

            notification.NotificationID = _dataService.CreateNotification(
                notification.NotificationTypeID,
                portalId,
                notification.To,
                notification.From,
                notification.Subject,
                notification.Body,
                notification.IncludeDismissAction, 
                notification.SenderUserID, 
                UserController.GetCurrentUserInfo().UserID, 
                notification.ExpirationDate);

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
                    UserController.GetCurrentUserInfo().UserID);
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
                    UserController.GetCurrentUserInfo().UserID);
            }

            return notification;
        }

        /// <summary>
        /// Creates a new notification action.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <param name="key">The custom information to be stored for this particular action.</param>
        /// <returns>The new notification action.</returns>
        /// <exception cref="System.ArgumentException">Thrown when key is null or empty.</exception>
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

        /// <summary>
        /// Creates a new notification type.
        /// </summary>
        /// <param name="name">The name of the notification type.</param>
        /// <param name="description">The description of the notification type.</param>
        /// <param name="timeToLive">The number of minutes to be added to the creation time of new notifications of this type to calculate the expiration date.</param>
        /// <param name="desktopModuleId">Indicates the module that will include the resource file (named SharedResources.resx) used localize NotificationTypeActions. If the value is 0 or negative, the resource file used will be ~/App_GlobalResources/SharedResources.resx.</param>
        /// <returns>The new notification type.</returns>
        /// <exception cref="System.ArgumentException">Thrown when name is null or empty.</exception>
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

        /// <summary>
        /// Deletes an existing notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        public virtual void DeleteNotification(int notificationId)
        {
            _dataService.DeleteNotification(notificationId);
        }

        /// <summary>
        /// Deletes an existing notification action.
        /// </summary>
        /// <param name="notificationActionId">The notification action identifier.</param>
        public virtual void DeleteNotificationAction(int notificationActionId)
        {
            _dataService.DeleteNotificationAction(notificationActionId);
        }

        /// <summary>
        /// Deletes an individual notification recipient.
        /// </summary>
        /// <remarks>It also deletes the notification if there are no more recipients.</remarks>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public virtual void DeleteNotificationRecipient(int notificationId, int userId)
        {
            MessagingController.Instance.DeleteMessageRecipient(notificationId, userId);
            var recipients = MessagingController.Instance.GetMessageRecipients(notificationId);
            if (recipients.Count == 0)
            {
                DeleteNotification(notificationId);
            }
        }

        /// <summary>
        /// Deletes an existing notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        public virtual void DeleteNotificationType(int notificationTypeId)
        {
            _dataService.DeleteNotificationType(notificationTypeId);

            RemoveNotificationTypeCache();
        }

        /// <summary>
        /// Deletes an existing notification type action.
        /// </summary>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        public virtual void DeleteNotificationTypeAction(int notificationTypeActionId)
        {
            _dataService.DeleteNotificationTypeAction(notificationTypeActionId);

            RemoveNotificationTypeActionCache();
        }

        /// <summary>
        /// Gets a notification action by identifier.
        /// </summary>
        /// <param name="notificationActionId">The notification action identifier.</param>
        /// <returns>The notification action with the provided identifier.</returns>
        public virtual NotificationAction GetNotificationAction(int notificationActionId)
        {
            return CBO.FillObject<NotificationAction>(_dataService.GetNotificationAction(notificationActionId));
        }

        /// <summary>
        /// Gets a notification action by notification and notification type action.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <returns>The notification action with the provided notification and notification type action.</returns>
        public virtual NotificationAction GetNotificationAction(int notificationId, int notificationTypeActionId)
        {
            return CBO.FillObject<NotificationAction>(_dataService.GetNotificationActionByMessageAndNotificationTypeAction(notificationId, notificationTypeActionId));
        }

        /// <summary>
        /// Gets a notification action by notification and notification type action name.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeActionName">The notification type action name.</param>
        /// <returns>The notification action with the provided notification and notification type action name.</returns>
        public virtual NotificationAction GetNotificationAction(int notificationId, string notificationTypeActionName)
        {
            return CBO.FillObject<NotificationAction>(_dataService.GetNotificationActionByMessageAndNotificationTypeActionName(notificationId, notificationTypeActionName));
        }

        /// <summary>
        /// Gets the list of notification actions for the provided notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>A list of notification actions for the provided notification.</returns>
        public virtual IList<NotificationAction> GetNotificationActionsByNotificationId(int notificationId)
        {
            return CBO.FillCollection<NotificationAction>(_dataService.GetNotificationActionsByMessageId(notificationId));
        }

        /// <summary>
        /// Gets a list of notifications sent to the provided user in the specified portal.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="pageIndex">The page index (starting at 0) used to filter the list to a particular number of results.</param>
        /// <param name="pageSize">The page size used to filter the list to a particular number of results.</param>
        /// <returns>The filtered list of notifications sent to the provided user in the specified portal.</returns>
        public virtual IList<Notification> GetNotifications(int userId, int portalId,int pageIndex, int pageSize)
        {
            return CBO.FillCollection<Notification>(_dataService.GetNotifications(userId, portalId, pageIndex, pageSize));
        }

        /// <summary>
        /// Gets a notification type by identifier.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <returns>The notification type with the provided identifier.</returns>
        public virtual NotificationType GetNotificationType(int notificationTypeId)
        {
            var notificationTypeCacheKey = string.Format(DataCache.NotificationTypesCacheKey, notificationTypeId);
            var cacheItemArgs = new CacheItemArgs(notificationTypeCacheKey, DataCache.NotificationTypesTimeOut, DataCache.NotificationTypesCachePriority, notificationTypeId);
            return CBO.GetCachedObject<NotificationType>(cacheItemArgs, GetNotificationTypeCallBack);
        }

        /// <summary>
        /// Gets a notification type by name.
        /// </summary>
        /// <param name="name">The notification type name.</param>
        /// <returns>The notification type with the provided name.</returns>
        /// <exception cref="System.ArgumentException">Thrown when name is null or empty.</exception>
        public virtual NotificationType GetNotificationType(string name)
        {
            Requires.NotNullOrEmpty("name", name);

            var notificationTypeCacheKey = string.Format(DataCache.NotificationTypesCacheKey, name);
            var cacheItemArgs = new CacheItemArgs(notificationTypeCacheKey, DataCache.NotificationTypesTimeOut, DataCache.NotificationTypesCachePriority, name);
            return CBO.GetCachedObject<NotificationType>(cacheItemArgs, GetNotificationTypeByNameCallBack);
        }

        /// <summary>
        /// Gets a notification type action by identifier.
        /// </summary>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <returns>The notification type action with the provided identifier.</returns>
        public virtual NotificationTypeAction GetNotificationTypeAction(int notificationTypeActionId)
        {
            var notificationTypeActionCacheKey = string.Format(DataCache.NotificationTypeActionsCacheKey, notificationTypeActionId);
            var cacheItemArgs = new CacheItemArgs(notificationTypeActionCacheKey, DataCache.NotificationTypeActionsTimeOut, DataCache.NotificationTypeActionsPriority, notificationTypeActionId);
            return CBO.GetCachedObject<NotificationTypeAction>(cacheItemArgs, GetNotificationTypeActionCallBack);
        }

        /// <summary>
        /// Gets a notification type action by notification type and name.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="name">The notification type action name.</param>
        /// <returns>The notification type action with the provided notification type and name.</returns>
        /// <exception cref="System.ArgumentException">Thrown when name is null or empty.</exception>
        public virtual NotificationTypeAction GetNotificationTypeAction(int notificationTypeId, string name)
        {
            Requires.NotNullOrEmpty("name", name);

            var notificationTypeActionCacheKey = string.Format(DataCache.NotificationTypeActionsByNameCacheKey, notificationTypeId, name);
            var cacheItemArgs = new CacheItemArgs(notificationTypeActionCacheKey, DataCache.NotificationTypeActionsTimeOut, DataCache.NotificationTypeActionsPriority, notificationTypeId, name);
            return CBO.GetCachedObject<NotificationTypeAction>(cacheItemArgs, GetNotificationTypeActionByNameCallBack);
        }

        /// <summary>
        /// Gets the list of notification type actions for the provided notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <returns>An ordered list of notification type actions for the provided notification type.</returns>
        public virtual IList<NotificationTypeAction> GetNotificationTypeActions(int notificationTypeId)
        {
            return CBO.FillCollection<NotificationTypeAction>(_dataService.GetNotificationTypeActions(notificationTypeId));
        }

        /// <summary>
        /// Updates an existing notification action.
        /// </summary>
        /// <param name="notificationAction">The existing notification action.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when notificationAction is null.</exception>
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

        /// <summary>
        /// Updates an existing notification type.
        /// </summary>
        /// <param name="notificationType">The existing notification type</param>
        /// <exception cref="System.ArgumentNullException">Thrown when notificationType is null.</exception>
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

        /// <summary>
        /// Updates an existing notificaiton type action.
        /// </summary>
        /// <param name="notificationTypeAction">The existing notification type action.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when notificationTypeAction is null.</exception>
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

        internal virtual DateTime GetExpirationDate(int notificationTypeId)
        {
            var notificationType = GetNotificationType(notificationTypeId);
            
            return notificationType.TimeToLive.TotalMinutes > 0 
                ? DateTime.UtcNow.AddMinutes(notificationType.TimeToLive.TotalMinutes) 
                : DateTime.MinValue;
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
