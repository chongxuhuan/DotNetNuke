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
using System.Data;

using DotNetNuke.ComponentModel;

#endregion

namespace DotNetNuke.Security.Membership.Data
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      DataProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DataProvider provides the abstract Data Access Layer for the project
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/28/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public abstract class DataProvider
    {
        //return the provider
        public static DataProvider Instance()
        {
            return ComponentFactory.GetComponent<DataProvider>();
        }

        #region Abstract Methods

        // Login/Security
        public abstract IDataReader UserLogin(string username, string password);
        public abstract IDataReader GetAuthRoles(int portalId, int moduleId);

        // Users
        public abstract int AddUser(int portalID, string username, string firstName, string lastName, int affiliateId, bool isSuperUser, string email, string displayName, bool updatePassword, bool isApproved, int createdByUserID);
        public abstract void DeleteUserPortal(int userId, int portalId);
        public abstract void RestoreUser(int userId, int portalId);
        public abstract void RemoveUser(int userId, int portalId);
        public abstract IDataReader GetAllUsers(int portalID, int pageIndex, int pageSize);

        public abstract IDataReader GetAllUsers(int portalID, int pageIndex, int pageSize, bool includeDeleted,
                                                bool superUsersOnly);
        public abstract IDataReader GetUnAuthorizedUsers(int portalId);
        public abstract IDataReader GetUnAuthorizedUsers(int portalId, bool includeDeleted, bool superUsersOnly);
        public abstract IDataReader GetDeletedUsers(int portalId);
        public abstract IDataReader GetUser(int portalId, int userId);
        public abstract IDataReader GetUserByAuthToken(int portalID, string userToken, string authType);
        public abstract IDataReader GetUserByUsername(int portalID, string username);
        public abstract int GetUserCountByPortal(int portalId);
        public abstract IDataReader GetUsersByEmail(int portalID, string email, int pageIndex, int pageSize);

        public abstract IDataReader GetUsersByEmail(int portalID, string email, int pageIndex, int pageSize,
                                                    bool includeDeleted, bool superUsersOnly);
        public abstract IDataReader GetUsersByProfileProperty(int portalID, string propertyName, string propertyValue, int pageIndex, int pageSize);

        public abstract IDataReader GetUsersByProfileProperty(int portalID, string propertyName, string propertyValue,
                                                              int pageIndex, int pageSize, bool includeDeleted,
                                                              bool superUsersOnly);
        public abstract IDataReader GetUsersByRolename(int portalID, string rolename);
        public abstract IDataReader GetUsersByUsername(int portalID, string username, int pageIndex, int pageSize);

        public abstract IDataReader GetUsersByUsername(int portalID, string username, int pageIndex, int pageSize,
                                                       bool includeDeleted, bool superUsersOnly);
        public abstract IDataReader GetSuperUsers();
        public abstract void UpdateUser(int userId, int portalID, string firstName, string lastName, bool isSuperUser, string email, string displayName, bool updatePassword, bool isApproved, bool refreshRoles, string lastIpAddress, bool isDeleted, int lastModifiedByUserID);

        // Roles
        public abstract int AddRole(int portalId, int roleGroupId, string roleName, string description, float serviceFee, string billingPeriod, string billingFrequency, float trialFee, int trialPeriod,
                                    string trialFrequency, bool isPublic, bool autoAssignment, string rsvpCode, string iconFile, int createdByUserID);
        public abstract void DeleteRole(int roleId);
        public abstract IDataReader GetPortalRoles(int portalId);
        public abstract IDataReader GetRoles();
        public abstract IDataReader GetRoleSettings(int roleId);
        public abstract void UpdateRole(int roleId, int roleGroupId, string description, float serviceFee, string billingPeriod, string billingFrequency, float trialFee, int trialPeriod,
                                        string trialFrequency, bool isPublic, bool autoAssignment, string rsvpCode, string iconFile, int lastModifiedByUserID);
        public abstract void UpdateRoleSetting(int roleId, string settingName, string settingValue, int lastModifiedByUserID);

        // RoleGroups
        public abstract int AddRoleGroup(int portalId, string groupName, string description, int createdByUserID);
        public abstract void DeleteRoleGroup(int roleGroupId);
        public abstract IDataReader GetRoleGroup(int portalId, int roleGroupId);
        public abstract IDataReader GetRoleGroupByName(int portalID, string roleGroupName);
        public abstract IDataReader GetRoleGroups(int portalId);
        public abstract void UpdateRoleGroup(int roleGroupId, string groupName, string description, int lastModifiedByUserID);

        // User Roles
        public abstract IDataReader GetUserRole(int portalID, int userId, int roleId);
        public abstract IDataReader GetUserRoles(int portalID, int userId);
        public abstract IDataReader GetUserRolesByUsername(int portalID, string username, string rolename);
        public abstract int AddUserRole(int portalID, int userId, int roleId, DateTime effectiveDate, DateTime expiryDate, int createdByUserID);
        public abstract void UpdateUserRole(int userRoleId, DateTime effectiveDate, DateTime expiryDate, int lastModifiedByUserID);
        public abstract void DeleteUserRole(int userId, int roleId);

        // Services
        public abstract IDataReader GetServices(int portalId, int userId);

        // Profile
        public abstract IDataReader GetUserProfile(int userId);
        public abstract void UpdateProfileProperty(int profileId, int userId, int propertyDefinitionID, string propertyValue, int visibility, 
                                                    string extendedVisibility, DateTime lastUpdatedDate);

        // Users Online
        public abstract void UpdateUsersOnline(Hashtable userList);
        public abstract void DeleteUsersOnline(int timeWindow);
        public abstract IDataReader GetOnlineUser(int userId);
        public abstract IDataReader GetOnlineUsers(int portalId);

        // Legacy
        public abstract IDataReader GetUsers(int portalId);

        #endregion
    }
}
