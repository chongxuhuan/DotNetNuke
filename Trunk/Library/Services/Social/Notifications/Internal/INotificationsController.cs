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
using System.Collections.Generic;

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Services.Social.Notifications.Internal
{
    /// <summary>
    /// Defines the methods to work with Notifications, NotificationTypes, NotificationTypeActions and NotificationActions.
    /// </summary>
    public interface INotificationsController
    {
        #region NotificationTypes Methods

        /// <summary>
        /// Creates a new notification type.
        /// </summary>
        /// <param name="name">The name of the notification type.</param>
        /// <param name="description">The description of the notification type.</param>
        /// <param name="timeToLive">The number of minutes to be added to the creation time of new notifications of this type to calculate the expiration date.</param>
        /// <param name="desktopModuleId">Indicates the module that will include the resource file (named SharedResources.resx) used localize NotificationTypeActions. If the value is 0 or negative, the resource file used will be ~/App_GlobalResources/SharedResources.resx.</param>
        /// <returns>The new notification type.</returns>
        NotificationType CreateNotificationType(string name, string description, TimeSpan timeToLive, int desktopModuleId);
        
        /// <summary>
        /// Updates an existing notification type.
        /// </summary>
        /// <param name="notificationType">The existing notification type</param>
        void UpdateNotificationType(NotificationType notificationType);
        
        /// <summary>
        /// Deletes an existing notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        void DeleteNotificationType(int notificationTypeId);
        
        /// <summary>
        /// Gets a notification type by identifier.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <returns>The notification type with the provided identifier.</returns>
        NotificationType GetNotificationType(int notificationTypeId);
        
        /// <summary>
        /// Gets a notification type by name.
        /// </summary>
        /// <param name="name">The notification type name.</param>
        /// <returns>The notification type with the provided name.</returns>
        NotificationType GetNotificationType(string name);

        #endregion

        #region NotificationTypeActions Methods

        /// <summary>
        /// Creates a new notification type action and adds it to the end of the actions list for the specified notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="nameResourceKey">The resource key used to localize the action name.</param>
        /// <param name="descriptionResourceKey">The resource key used to localize the action description.</param>
        /// <param name="confirmResourceKey">The resource key used to localize the action confirmation message. Leave it empty if no confirmation is needed.</param>
        /// <param name="apiCall">The service framework url to be called to perform the notification action.</param>
        /// <returns>The new notification type action.</returns>
        NotificationTypeAction AddNotificationTypeActionToEnd(int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        
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
        NotificationTypeAction AddNotificationTypeActionAfter(int afterNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        
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
        NotificationTypeAction AddNotificationTypeActionBefore(int beforeNotificationTypeActionId, int notificationTypeId, string nameResourceKey, string descriptionResourceKey, string confirmResourceKey, string apiCall);
        
        /// <summary>
        /// Updates an existing notificaiton type action.
        /// </summary>
        /// <param name="notificationTypeAction">The existing notification type action.</param>
        void UpdateNotificationTypeAction(NotificationTypeAction notificationTypeAction);
        
        /// <summary>
        /// Deletes an existing notification type action.
        /// </summary>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        void DeleteNotificationTypeAction(int notificationTypeActionId);
        
        /// <summary>
        /// Gets a notification type action by identifier.
        /// </summary>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <returns>The notification type action with the provided identifier.</returns>
        NotificationTypeAction GetNotificationTypeAction(int notificationTypeActionId);
        
        /// <summary>
        /// Gets a notification type action by notification type and name.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="name">The notification type action name.</param>
        /// <returns>The notification type action with the provided notification type and name.</returns>
        NotificationTypeAction GetNotificationTypeAction(int notificationTypeId, string name);
        
        /// <summary>
        /// Gets the list of notification type actions for the provided notification type.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <returns>An ordered list of notification type actions for the provided notification type.</returns>
        IList<NotificationTypeAction> GetNotificationTypeActions(int notificationTypeId);

        #endregion

        #region NotificationActions Methods

        /// <summary>
        /// Creates a new notification action.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <param name="key">The custom information to be stored for this particular action.</param>
        /// <returns>The new notification action.</returns>
        NotificationAction CreateNotificationAction(int notificationId, int notificationTypeActionId, string key);
        
        /// <summary>
        /// Updates an existing notification action.
        /// </summary>
        /// <param name="notificationAction">The existing notification action.</param>
        void UpdateNotificationAction(NotificationAction notificationAction);
        
        /// <summary>
        /// Deletes an existing notification action.
        /// </summary>
        /// <param name="notificationActionId">The notification action identifier.</param>
        void DeleteNotificationAction(int notificationActionId);
        
        /// <summary>
        /// Gets a notification action by identifier.
        /// </summary>
        /// <param name="notificationActionId">The notification action identifier.</param>
        /// <returns>The notification action with the provided identifier.</returns>
        NotificationAction GetNotificationAction(int notificationActionId);
        
        /// <summary>
        /// Gets a notification action by notification and notification type action.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="notificationTypeActionId">The notification type action identifier.</param>
        /// <returns>The notification action with the provided notification and notification type action.</returns>
        NotificationAction GetNotificationAction(int notificationId, int notificationTypeActionId);
        
        /// <summary>
        /// Gets the list of notification actions for the provided notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>A list of notification actions for the provided notification.</returns>
        IList<NotificationAction> GetNotificationActionsByNotificationId(int notificationId);

        #endregion

        #region Notifications Methods

        /// <summary>
        /// Creates a new notification and sets is sender as the portal administrator.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="subject">The notification subject.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="roles">The list of roles to send the notification to. Leave it as null to send only to individual users.</param>
        /// <param name="users">The list of users to send the notification to. Leave it as null to send only to roles.</param>
        /// <returns>The new notification.</returns>
        Notification CreateNotification(int notificationTypeId, int portalId,string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users);
        
        /// <summary>
        /// Creates a new notification.
        /// </summary>
        /// <param name="notificationTypeId">The notification type identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="subject">The notification subject.</param>
        /// <param name="body">The notification body.</param>
        /// <param name="roles">The list of roles to send the notification to. Leave it as null to send only to individual users.</param>
        /// <param name="users">The list of users to send the notification to. Leave it as null to send only to roles.</param>
        /// <param name="sender">The notification sender.</param>
        /// <returns>The new notification.</returns>
        Notification CreateNotification(int notificationTypeId, int portalId, string subject, string body, IList<RoleInfo> roles, IList<UserInfo> users, UserInfo sender);
        
        /// <summary>
        /// Counts the notifications sent to the provided user in the specified portal.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <returns>The number of notifications sent to the provided user in the specified portal.</returns>
        int CountNotifications(int userId, int portalId);
        
        /// <summary>
        /// Gets a list of notifications sent to the provided user in the specified portal.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="portalId">The portal identifier.</param>
        /// <param name="pageIndex">The page index (starting at 0) used to filter the list to a particular number of results.</param>
        /// <param name="pageSize">The page size used to filter the list to a particular number of results.</param>
        /// <returns>The filtered list of notifications sent to the provided user in the specified portal.</returns>
        IList<Notification> GetNotifications(int userId, int portalId, int pageIndex, int pageSize);
        
        /// <summary>
        /// Deletes an existing notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        void DeleteNotification(int notificationId);
        
        /// <summary>
        /// Deletes an individual notification recipient.
        /// </summary>
        /// <remarks>It also deletes the notification if there are no more recipients.</remarks>
        /// <param name="notificationId">The notification identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void DeleteNotificationRecipient(int notificationId, int userId);

        #endregion
    }
}