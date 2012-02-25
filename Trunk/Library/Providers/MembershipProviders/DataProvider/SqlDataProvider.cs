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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Security.Membership.Data
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      SqlDataProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SqlDataProvider provides a concrete SQL Server implementation of the
    /// Data Access Layer for the project
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/28/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SqlDataProvider : DataProvider
    {
        public string ConnectionString
        {
            get
            {
                return DotNetNuke.Data.DataProvider.Instance().ConnectionString;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return DotNetNuke.Data.DataProvider.Instance().DatabaseOwner;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return DotNetNuke.Data.DataProvider.Instance().ObjectQualifier;
            }
        }

        private object GetNull(object field)
        {
            return Null.GetNull(field, DBNull.Value);
        }

        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + name;
        }

        //Security
        public override IDataReader UserLogin(string username, string password)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("UserLogin"), username, password);
        }

        public override IDataReader GetAuthRoles(int portalId, int moduleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetAuthRoles"), portalId, moduleId);
        }

        //Users
        public int AddUser(int portalID, string username, string firstName, string lastName, int affiliateId, bool isSuperUser, string email, string displayName, bool updatePassword, int createdByUserID)
        {
            return AddUser(portalID, username, firstName, lastName, affiliateId, isSuperUser, email, displayName, updatePassword, false, createdByUserID);
        }

        public override int AddUser(int portalID, string username, string firstName, string lastName, int affiliateId, bool isSuperUser, string email, string displayName, bool updatePassword,
                                    bool isApproved, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddUser",
                                                        portalID,
                                                        username,
                                                        firstName,
                                                        lastName,
                                                        GetNull(affiliateId),
                                                        isSuperUser,
                                                        email,
                                                        displayName,
                                                        updatePassword,
                                                        isApproved,
                                                        createdByUserID));
        }

        public override void DeleteUserPortal(int userId, int portalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteUserPortal"), userId, GetNull(portalId));
        }

        public override void RestoreUser(int userId, int portalId)
        {
	        SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("RestoreUser"), userId, GetNull(portalId));
        }

        public override void RemoveUser(int userId, int portalId)
        {
	        SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("RemoveUser"), userId, GetNull(portalId));
        }

        public override void UpdateUser(int userId, int portalID, string firstName, string lastName, bool isSuperUser, string email, string displayName, bool updatePassword, bool isApproved, bool refreshRoles,
                                        string lastIpAddress, bool isDeleted, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      GetFullyQualifiedName("UpdateUser"),
                                      userId,
                                      GetNull(portalID),
                                      firstName,
                                      lastName,
                                      isSuperUser,
                                      email,
                                      displayName,
                                      updatePassword,
                                      isApproved,
                                      refreshRoles,
                                      lastIpAddress,
                                      isDeleted,
                                      lastModifiedByUserID);
        }

        public override IDataReader GetAllUsers(int portalID, int pageIndex, int pageSize, bool includeDeleted, bool superUsersOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetAllUsers"), GetNull(portalID), pageIndex, pageSize, includeDeleted, superUsersOnly);
        }
        
        public override IDataReader GetAllUsers(int portalID, int pageIndex, int pageSize)
        {
            return GetAllUsers(portalID, pageIndex, pageSize, false, false);
        }

        public override IDataReader GetUnAuthorizedUsers(int portalId, bool includeDeleted, bool superUsersOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUnAuthorizedUsers"), GetNull(portalId), includeDeleted, superUsersOnly);
        }

        public override IDataReader GetUnAuthorizedUsers(int portalId)
        {
            return GetUnAuthorizedUsers(portalId, false, false);
        }

        public override IDataReader GetDeletedUsers(int portalId)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetDeletedUsers"), GetNull(portalId));
        }
        
        public override IDataReader GetUser(int portalId, int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUser"), portalId, userId);
        }

        public override IDataReader GetUserByAuthToken(int portalID, string userToken, string authType)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserByAuthToken"), portalID, userToken, authType);
        }

        public override IDataReader GetUserByUsername(int portalId, string username)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserByUsername"), GetNull(portalId), username);
        }

        public override int GetUserCountByPortal(int portalId)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("GetUserCountByPortal"), portalId));
        }

        public override IDataReader GetUsersByEmail(int portalID, string email, int pageIndex, int pageSize, bool includeDeleted, bool superUsersOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUsersByEmail"), GetNull(portalID), email, pageIndex, pageSize, includeDeleted, superUsersOnly);
        }
        
        public override IDataReader GetUsersByEmail(int portalID, string email, int pageIndex, int pageSize)
        {
            return GetUsersByEmail(portalID, email, pageIndex, pageSize, false, false);
        }

        public override IDataReader GetUsersByProfileProperty(int portalID, string propertyName, string propertyValue, int pageIndex, int pageSize)
        {
            return GetUsersByProfileProperty(portalID, propertyName, propertyValue, pageIndex, pageSize, false, false);
        }

        public override IDataReader GetUsersByProfileProperty(int portalID, string propertyName, string propertyValue, int pageIndex, int pageSize, bool includeDeleted, bool superUsersOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUsersByProfileProperty"), GetNull(portalID), propertyName, propertyValue, pageIndex, pageSize, includeDeleted, superUsersOnly);
        }

        public override IDataReader GetUsersByRolename(int portalID, string rolename)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUsersByRolename"), GetNull(portalID), rolename);
        }

        public override IDataReader GetUsersByUsername(int portalID, string username, int pageIndex, int pageSize, bool includeDeleted, bool superUsersOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUsersByUsername"), GetNull(portalID), username, pageIndex, pageSize, includeDeleted, superUsersOnly);
        }

        public override IDataReader GetUsersByUsername(int portalID, string username, int pageIndex, int pageSize)
        {
            return GetUsersByUsername(portalID, username, pageIndex, pageSize, false, false);
        }

        public override IDataReader GetSuperUsers()
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetSuperUsers"));
        }

        //Roles
        public override int AddRole(int portalId, int roleGroupId, string roleName, string description, float serviceFee, string billingPeriod, string billingFrequency, float trialFee, int trialPeriod,
                                    string trialFrequency, bool isPublic, bool autoAssignment, string rsvpCode, string iconFile, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        GetFullyQualifiedName("AddRole"),
                                                        portalId,
                                                        GetNull(roleGroupId),
                                                        roleName,
                                                        description,
                                                        serviceFee,
                                                        billingPeriod,
                                                        GetNull(billingFrequency),
                                                        trialFee,
                                                        trialPeriod,
                                                        GetNull(trialFrequency),
                                                        isPublic,
                                                        autoAssignment,
                                                        rsvpCode,
                                                        iconFile,
                                                        createdByUserID));
        }

        public override void DeleteRole(int roleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteRole"), roleId);
        }

        public override IDataReader GetPortalRoles(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetPortalRoles"), portalId);
        }

        public override IDataReader GetRoles()
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoles"));
        }

        public override IDataReader GetRoleSettings(int roleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleSettings"), roleId);
        }

        public override void UpdateRole(int roleId, int roleGroupId, string description, float serviceFee, string billingPeriod, string billingFrequency, float trialFee, int trialPeriod,
                                        string trialFrequency, bool isPublic, bool autoAssignment, string rsvpCode, string iconFile, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      GetFullyQualifiedName("UpdateRole"),
                                      roleId,
                                      GetNull(roleGroupId),
                                      description,
                                      serviceFee,
                                      billingPeriod,
                                      GetNull(billingFrequency),
                                      trialFee,
                                      trialPeriod,
                                      GetNull(trialFrequency),
                                      isPublic,
                                      autoAssignment,
                                      rsvpCode,
                                      iconFile,
                                      lastModifiedByUserID);
        }

        public override void UpdateRoleSetting(int roleId, string settingName, string settingValue, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      GetFullyQualifiedName("UpdateRoleSetting"),
                                      roleId,
                                      settingName,
                                      settingValue,
                                      lastModifiedByUserID);
        }

        //Role Groups
        public override int AddRoleGroup(int portalId, string groupName, string description, int createdByUserID)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("AddRoleGroup"), portalId, groupName, description, createdByUserID));
        }

        public override void DeleteRoleGroup(int roleGroupId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteRoleGroup"), roleGroupId);
        }

        public override IDataReader GetRoleGroup(int portalId, int roleGroupId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleGroup"), portalId, roleGroupId);
        }

        public override IDataReader GetRoleGroupByName(int portalID, string roleGroupName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleGroupByName"), portalID, roleGroupName);
        }

        public override IDataReader GetRoleGroups(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetRoleGroups"), portalId);
        }

        public override void UpdateRoleGroup(int roleGroupId, string groupName, string description, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateRoleGroup"), roleGroupId, groupName, description, lastModifiedByUserID);
        }

        //User Roles
        public override IDataReader GetUserRole(int portalID, int userId, int roleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserRole"), portalID, userId, roleId);
        }

        public override IDataReader GetUserRoles(int portalID, int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserRoles"), portalID, userId);
        }

        public override IDataReader GetUserRolesByUsername(int portalID, string username, string rolename)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserRolesByUsername"), portalID, GetNull(username), GetNull(rolename));
        }

        public int AddUserRole(int portalId, int roleId, DateTime effectiveDate, DateTime expiryDate, int createdByUserID)
        {
            return AddUserRole(portalId, 0, roleId, effectiveDate, expiryDate, createdByUserID);
        }

        public override int AddUserRole(int portalId, int userId, int roleId, DateTime effectiveDate, DateTime expiryDate, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("AddUserRole"), portalId, userId, roleId, GetNull(effectiveDate), GetNull(expiryDate), createdByUserID));
        }

        public override void UpdateUserRole(int userRoleId, DateTime effectiveDate, DateTime expiryDate, int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateUserRole"), userRoleId, GetNull(effectiveDate), GetNull(expiryDate), lastModifiedByUserID);
        }

        public override void DeleteUserRole(int userId, int roleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteUserRole"), userId, roleId);
        }

        public override IDataReader GetServices(int portalId, int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetServices"), portalId, GetNull(userId));
        }

        public override IDataReader GetUsers(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUsers"), GetNull(portalId));
        }

        //Profile
        public override IDataReader GetUserProfile(int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetUserProfile"), userId);
        }

        public override void UpdateProfileProperty(int profileId, int userId, int propertyDefinitionID, string propertyValue, int visibility,
                                                    string extendedVisibility, DateTime lastUpdatedDate)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateUserProfileProperty"), GetNull(profileId), userId, 
                                                                        propertyDefinitionID, propertyValue, visibility, 
                                                                        extendedVisibility, lastUpdatedDate);
        }

        //users online
        public override void DeleteUsersOnline(int timeWindow)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteUsersOnline", timeWindow);
        }

        public override IDataReader GetOnlineUser(int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUser", userId);
        }

        public override IDataReader GetOnlineUsers(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetOnlineUsers", portalId);
        }

        public override void UpdateUsersOnline(Hashtable userList)
        {
            if ((userList.Count == 0))
            {
				//No users to process, quit method
                return;
            }
            foreach (string key in userList.Keys)
            {
                if (userList[key] is AnonymousUserInfo)
                {
                    var user = (AnonymousUserInfo) userList[key];
                    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateAnonymousUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate);
                }
                else if (userList[key] is OnlineUserInfo)
                {
                    var user = (OnlineUserInfo) userList[key];
                    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateOnlineUser", user.UserID, user.PortalID, user.TabID, user.LastActiveDate);
                }
            }
        }
    }
}
