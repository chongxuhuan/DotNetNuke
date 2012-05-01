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

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;

namespace DotNetNuke.Services.Social.Notifications.Data
{
    internal class DataService : ComponentBase<IDataService, DataService>, IDataService
    {
        private readonly DataProvider _provider = DataProvider.Instance();
        private const string Prefix = "CoreMessaging_";

        #region Private Methods

        private static string GetFullyQualifiedName(string procedureName)
        {
            return Prefix + procedureName;
        }

        #endregion

        #region NotificationTypes CRUD

        public int SaveNotificationType(int notificationTypeId, string name, string description, int timeToLive, int desktopModuleId, int createUpdateUserId)
        {
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("SaveNotificationType"), notificationTypeId, name, _provider.GetNull(description), _provider.GetNull(timeToLive), _provider.GetNull(desktopModuleId), createUpdateUserId);
        }

        public void DeleteNotificationType(int notificationTypeId)
        {
            _provider.ExecuteNonQuery(GetFullyQualifiedName("DeleteNotificationType"), notificationTypeId);
        }

        public IDataReader GetNotificationType(int notificationTypeId)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotificationType"), notificationTypeId);
        }

        public IDataReader GetNotificationTypeByName(string name)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotificationTypeByName"), name);
        }

        #endregion

        #region NotificationTypeActions CRUD

        public int AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId)
        {
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("AddNotificationTypeActionToEnd"), notificationTypeId, nameResourceKey, _provider.GetNull(descriptionResourceKey), _provider.GetNull(confirmResourceKey), apiCall, createdByUserId);
        }

        public int AddNotificationTypeActionAfter(int afternotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId)
        {
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("AddNotificationTypeActionAfter"), afternotificationTypeActionId, notificationTypeId, nameResourceKey, _provider.GetNull(descriptionResourceKey), _provider.GetNull(confirmResourceKey), apiCall, createdByUserId);
        }

        public int AddNotificationTypeActionBefore(int beforenotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int createdByUserId)
        {
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("AddNotificationTypeActionBefore"), beforenotificationTypeActionId, notificationTypeId, nameResourceKey, _provider.GetNull(descriptionResourceKey), _provider.GetNull(confirmResourceKey), apiCall, createdByUserId);
        }

        public void UpdateNotificationTypeAction(int notificationTypeActionId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall, int lastModifiedByUserId)
        {
            _provider.ExecuteNonQuery(GetFullyQualifiedName("UpdateNotificationTypeAction"), notificationTypeActionId, nameResourceKey, _provider.GetNull(descriptionResourceKey), _provider.GetNull(confirmResourceKey), apiCall, lastModifiedByUserId);
        }

        public void DeleteNotificationTypeAction(int notificationTypeActionId)
        {
            _provider.ExecuteNonQuery(GetFullyQualifiedName("DeleteNotificationTypeAction"), notificationTypeActionId);
        }

        public IDataReader GetNotificationTypeAction(int notificationTypeActionId)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotificationTypeAction"), notificationTypeActionId);
        }

        public IDataReader GetNotificationTypeActionByName(int notificationTypeId, string name)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotificationTypeActionByName"), notificationTypeId, name);
        }

        public IDataReader GetNotificationTypeActions(int notificationTypeId)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotificationTypeActions"), notificationTypeId);
        }

        #endregion

        #region Notifications Public Methods

        public int CreateNotification(Notification notification, int portalId)
        {
            int createdByUserId = UserController.GetCurrentUserInfo().UserID;
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("CreateNotification"),
                                                notification.NotificationTypeID,
                                                portalId,
                                                notification.To,
                                                notification.From,
                                                notification.Subject,
                                                notification.Body,
                                                notification.SenderUserID,
                                                createdByUserId,
                                                _provider.GetNull(notification.ExpirationDate),
                                                notification.IncludeDismissAction,
                                                notification.Context);
        }

        public void DeleteNotification(int notificationId)
        {
            _provider.ExecuteNonQuery(GetFullyQualifiedName("DeleteNotification"), notificationId);
        }

        public int CountNotifications(int userId, int portalId)
        {
            return _provider.ExecuteScalar<int>(GetFullyQualifiedName("CountNotifications"), userId, portalId);
        }

        public IDataReader GetNotifications(int userId, int portalId,int pageIndex, int pageSize)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotifications"), userId, portalId, pageIndex, pageSize);
        }

        public IDataReader GetNotification(int notificationId)
        {
            return _provider.ExecuteReader(GetFullyQualifiedName("GetNotification"), notificationId);
        }

        #endregion
    }
}
