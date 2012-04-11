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

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Services.Social.Notifications.Internal
{
    public interface INotificationsController
    {
        #region NotificationTypes Methods

        NotificationType CreateNotificationType(string name, string description, TimeSpan timeToLive, int desktopModuleId);
        void UpdateNotificationType(NotificationType notificationType);
        void DeleteNotificationType(int notificationTypeId);
        NotificationType GetNotificationType(int notificationTypeId);
        NotificationType GetNotificationType(string name);

        #endregion

        #region NotificationTypeActions Methods

        NotificationTypeAction AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        NotificationTypeAction AddNotificationTypeActionAfter(int afterNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        NotificationTypeAction AddNotificationTypeActionBefore(int beforeNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        void UpdateNotificationTypeAction(NotificationTypeAction notificationTypeAction);
        void DeleteNotificationTypeAction(int notificationTypeActionId);
        NotificationTypeAction GetNotificationTypeAction(int notificationTypeActionId);
        NotificationTypeAction GetNotificationTypeAction(int notificationTypeId, string name);
        IList<NotificationTypeAction> GetNotificationTypeActions(int notificationTypeId);

        #endregion

        #region NotificationActions Methods

        NotificationAction CreateNotificationAction(int notificationId, int notificationTypeActionId, string key);
        void UpdateNotificationAction(NotificationAction notificationAction);
        void DeleteNotificationAction(int notificationActionId);
        NotificationAction GetNotificationAction(int notificationActionId);
        NotificationAction GetNotificationAction(int notificationId, int notificationTypeActionId);
        IList<NotificationAction> GetNotificationActionsByNotificationId(int notificationId);

        #endregion

        #region Notifications Methods

        Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users);
        Notification CreateNotification(int notificationTypeId, int portalId, string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, UserInfo sender);
        int CountNotifications(int userId, int portalId);
        IList<Notification> GetNotifications(int userId, int portalId, int pageIndex, int pageSize);
        void DeleteNotification(int notificationId);
        void DeleteNotificationRecipient(int notificationId, int userId);

        #endregion

        #region Business Helper APIs

        #endregion
    }
}
