﻿#region Copyright
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
using System.Data;

namespace DotNetNuke.Services.Social.Notifications.Data
{
    public interface IDataService
    {
        #region NotificationTypes CRUD

        int SaveNotificationType(int notificationTypeId, string name, string description, int timeToLive, int desktopModuleId, int createUpdateUserId);
        void DeleteNotificationType(int notificationTypeId);
        IDataReader GetNotificationType(int notificationTypeId);
        IDataReader GetNotificationTypeByName(string name);

        #endregion

        #region NotificationTypeActions CRUD

        int AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId);
        int AddNotificationTypeActionAfter(int afterNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId);
        int AddNotificationTypeActionBefore(int beforeNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId);
        void UpdateNotificationTypeAction(int notificationTypeActionId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int lastModifiedByUserId);
        void DeleteNotificationTypeAction(int notificationTypeActionId);
        IDataReader GetNotificationTypeAction(int notificationTypeActionId);
        IDataReader GetNotificationTypeActionByName(int notificationTypeId, string name);
        IDataReader GetNotificationTypeActions(int notificationTypeId);

        #endregion

        #region NotificationActions CRUD

        int SaveNotificationAction(int notificationActionId, int messageId, int notificationTypeActionId, string key, int createUpdateUserId);
        void DeleteNotificationAction(int notificationActionId);
        IDataReader GetNotificationAction(int notificationActionId);
        IDataReader GetNotificationActionsByMessageId(int messageId);
        IDataReader GetNotificationActionByMessageAndNotificationTypeAction(int messageId, int notificationTypeActionId);

        #endregion

        #region Notifications

        int CreateNotification(int notificationTypeId, int portalId,string to, string from, string subject, string body, int senderUserId, int createUpdateUserId, DateTime expirationDate);
        void DeleteNotification(int notificationId);
        int CountNotifications(int userId, int portalId);
        IDataReader GetNotifications(int userId, int portalId, int pageIndex, int pageSize);

        #endregion
    }
}