#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Messaging.Data;

namespace DotNetNuke.Security.Roles
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Roles
    /// Class:      RoleController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The RoleController class provides Business Layer methods for Roles
    /// </summary>
    /// <history>
    ///     [cnurse]    05/23/2005  made compatible with .NET 2.0
    /// </history>
    /// -----------------------------------------------------------------------------
    public class RoleController
    {
        #region Private Nested Type: UserRoleActions

        private enum UserRoleActions
        {
            add = 0,
            update = 1,
            delete = 2
        }

        #endregion

		#region Private Shared Members
		
        private static readonly string[] UserRoleActionsCaption = {"ASSIGNMENT", "UPDATE", "UNASSIGNMENT"};

        private static readonly RoleProvider provider = RoleProvider.Instance();

        #endregion
		
		#region Private Methods

        private void AutoAssignUsers(RoleInfo objRoleInfo)
        {
            if (objRoleInfo.AutoAssignment)
            {
                //loop through users for portal and add to role
                ArrayList arrUsers = UserController.GetUsers(objRoleInfo.PortalID);
                foreach (UserInfo objUser in arrUsers)
                {
                    try
                    {
                        AddUserRole(objRoleInfo.PortalID, objUser.UserID, objRoleInfo.RoleID, Null.NullDate, Null.NullDate);
                    }
                    catch (Exception exc)
                    {
                        //user already belongs to role
                        DnnLog.Error(exc);

                    }
                }
            }
        }

        private static bool DeleteUserRoleInternal(int portalId, int userId, int roleId)
        {
            var roleController = new RoleController();
            var user = UserController.GetUserById(portalId, userId);
            var userRole = roleController.GetUserRole(portalId, userId, roleId);
            var portalController = new PortalController();
            bool delete = true;
            var portal = portalController.GetPortal(portalId);
            if (portal != null && userRole != null)
            {
                if (CanRemoveUserFromRole(portal, userId, roleId))
                {
                    provider.RemoveUserFromRole(portalId, user, userRole);
                    var objEventLog = new EventLogController();
                    objEventLog.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.ROLE_UPDATED);

                    //Remove the UserInfo from the Cache, as it has been modified
                    DataCache.ClearUserCache(portalId, user.Username);
                }
                else
                {
                    delete = false;
                }
            }
            return delete;
        }

        private static void SendNotification(UserInfo objUser, RoleInfo objRole, PortalSettings PortalSettings, UserRoleActions Action)
        {
            var objRoles = new RoleController();
            var Custom = new ArrayList {objRole.RoleName, objRole.Description};
            switch (Action)
            {
                case UserRoleActions.add:
                case UserRoleActions.update:
                    string preferredLocale = objUser.Profile.PreferredLocale;
                    if (string.IsNullOrEmpty(preferredLocale))
                    {
                        preferredLocale = PortalSettings.DefaultLanguage;
                    }
                    var ci = new CultureInfo(preferredLocale);
                    UserRoleInfo objUserRole = objRoles.GetUserRole(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
                    Custom.Add(Null.IsNull(objUserRole.EffectiveDate)
                                   ? DateTime.Today.ToString("g", ci)
                                   : objUserRole.EffectiveDate.ToString("g", ci));
                    Custom.Add(Null.IsNull(objUserRole.ExpiryDate) ? "-" : objUserRole.ExpiryDate.ToString("g", ci));
                    break;
                case UserRoleActions.delete:
                    Custom.Add("");
                    break;
            }
            var _message = new Message
                               {
                                   FromUserID = PortalSettings.AdministratorId,
                                   ToUserID = objUser.UserID,
                                   Subject =
                                       Localization.GetSystemMessage(objUser.Profile.PreferredLocale, PortalSettings,
                                                                     "EMAIL_ROLE_" +
                                                                     UserRoleActionsCaption[(int) Action] +
                                                                     "_SUBJECT", objUser),
                                   Body = Localization.GetSystemMessage(objUser.Profile.PreferredLocale,
                                                                        PortalSettings,
                                                                        "EMAIL_ROLE_" +
                                                                        UserRoleActionsCaption[(int) Action] + "_BODY",
                                                                        objUser,
                                                                        Localization.GlobalResourceFile,
                                                                        Custom),
                                   Status = MessageStatusType.Unread
                               };

            //_messagingController.SaveMessage(_message);
            Mail.SendEmail(PortalSettings.Email, objUser.Email, _message.Subject, _message.Body);
        }

        #endregion

        #region RoleInfo Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This overload adds a role and optionally adds the info to the AspNet Roles
        /// </summary>
        /// <param name="roleInfo">The Role to Add</param>
        /// <returns>The Id of the new role</returns>
        /// <history>
        /// 	[cnurse]	05/23/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddRole(RoleInfo roleInfo)
        {
            var roleId = -1;
            var success = provider.CreateRole(roleInfo.PortalID, ref roleInfo);
            if (success)
            {
                var eventLogController = new EventLogController();
                eventLogController.AddLog(roleInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.ROLE_CREATED);
                AutoAssignUsers(roleInfo);
                roleId = roleInfo.RoleID;
            }

            return roleId;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Delete a Role
        /// </summary>
        /// <param name="RoleId">The Id of the Role to delete</param>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public void DeleteRole(int RoleId, int PortalId)
        {
            RoleInfo objRole = GetRole(RoleId, PortalId);
            if (objRole != null)
            {
                provider.DeleteRole(PortalId, ref objRole);
                var objEventLog = new EventLogController();
                objEventLog.AddLog("RoleID", RoleId.ToString(CultureInfo.InvariantCulture), PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.ROLE_DELETED);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get a list of roles for the Portal
        /// </summary>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList GetPortalRoles(int PortalId)
        {
            return provider.GetRoles(PortalId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fetch a single Role
        /// </summary>
        /// <param name="RoleID">The Id of the Role</param>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public RoleInfo GetRole(int RoleID, int PortalID)
        {
            return provider.GetRole(PortalID, RoleID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Obtains a role given the role name
        /// </summary>
        /// <param name="PortalId">Portal indentifier</param>
        /// <param name="RoleName">Name of the role to be found</param>
        /// <returns>A RoleInfo object is the role is found</returns>
        /// <history>
        /// 	[VMasanas]	27/08/2004	Created
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public RoleInfo GetRoleByName(int PortalId, string RoleName)
        {
            return provider.GetRole(PortalId, RoleName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Returns an array of rolenames for a Portal
        /// </summary>
		/// <param name="portalId">The Id of the Portal</param>
        /// <returns>A String Array of Role Names</returns>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public string[] GetRoleNames(int portalId)
        {
			return provider.GetRoleNames(portalId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an ArrayList of Roles
        /// </summary>
        /// <returns>An ArrayList of Roles</returns>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList GetRoles()
        {
            return provider.GetRoles(Null.NullInteger);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the roles for a Role Group
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="roleGroupId">Id of the Role Group (If -1 all roles for the portal are
        /// retrieved).</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        /// <history>
        ///     [cnurse]	01/03/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            return provider.GetRolesByGroup(portalId, roleGroupId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a List of Roles for a given User
        /// </summary>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="PortalId">The Id of the Portal</param>
        /// <returns>A String Array of Role Names</returns>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public string[] GetRolesByUser(int UserId, int PortalId)
        {
            return provider.GetRoleNames(PortalId, UserId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Serializes the roles
        /// </summary>
        /// <param name="writer">An XmlWriter</param>
        /// <param name="portalId">The Id of the Portal</param>
        /// <history>
        /// 	[cnurse]	03/17/2008  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void SerializeRoles(XmlWriter writer, int portalId)
        {
            //Serialize Global Roles
            writer.WriteStartElement("roles");
            foreach (RoleInfo objRole in new RoleController().GetRolesByGroup(portalId, Null.NullInteger))
            {
                CBO.SerializeObject(objRole, writer);
            }
            writer.WriteEndElement();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Persists a role to the Data Store
        /// </summary>
        /// <param name="objRoleInfo">The role to persist</param>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateRole(RoleInfo objRoleInfo)
        {
            provider.UpdateRole(objRoleInfo);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(objRoleInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.ROLE_UPDATED);
            AutoAssignUsers(objRoleInfo);
        }

        #endregion

        #region UserRoleInfo Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a User to a Role
        /// </summary>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role</param>
        /// <param name="expiryDate">The expiry Date of the Role membership</param>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        ///     [cnurse]    05/12/2006  Now calls new overload with EffectiveDate = Now()
        /// </history>
        /// -----------------------------------------------------------------------------
        public void AddUserRole(int portalId, int userId, int roleId, DateTime expiryDate)
        {
            AddUserRole(portalId, userId, roleId, Null.NullDate, expiryDate);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a User to a Role
        /// </summary>
        /// <remarks>Overload adds Effective Date</remarks>
        /// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role</param>
        /// <param name="effectiveDate">The expiry Date of the Role membership</param>
        /// <param name="expiryDate">The expiry Date of the Role membership</param>
        /// <history>
        ///     [cnurse]    05/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void AddUserRole(int portalId, int userId, int roleId, DateTime effectiveDate, DateTime expiryDate)
        {
            UserInfo user = UserController.GetUserById(portalId, userId);
            UserRoleInfo userRole = GetUserRole(portalId, userId, roleId);
            var eventLogController = new EventLogController();
            if (userRole == null)
            {
				//Create new UserRole
                userRole = new UserRoleInfo
                               {
                                   UserID = userId, 
                                   RoleID = roleId, 
                                   PortalID = portalId, 
                                   EffectiveDate = effectiveDate, 
                                   ExpiryDate = expiryDate
                               };
                provider.AddUserToRole(portalId, user, userRole);
                eventLogController.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_CREATED);
            }
            else
            {
                userRole.EffectiveDate = effectiveDate;
                userRole.ExpiryDate = expiryDate;
                provider.UpdateUserRole(userRole);
                eventLogController.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_UPDATED);
            }

            //Remove the UserInfo from the Cache, as it has been modified
            DataCache.ClearUserCache(portalId, user.Username);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a User to a Role
        /// </summary>
        /// <param name="user">The user to assign</param>
        /// <param name="role">The role to add</param>
        /// <param name="portalSettings">The PortalSettings of the Portal</param>
        /// <param name="effectiveDate">The expiry Date of the Role membership</param>
        /// <param name="expiryDate">The expiry Date of the Role membership</param>
        /// <param name="userId">The Id of the User assigning the role</param>
        /// <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        /// <history>
        ///     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void AddUserRole(UserInfo user, RoleInfo role, PortalSettings portalSettings, DateTime effectiveDate, DateTime expiryDate, int userId, bool notifyUser)
        {
            var roleController = new RoleController();
            UserRoleInfo userRole = roleController.GetUserRole(portalSettings.PortalId, user.UserID, role.RoleID);
            var eventLogController = new EventLogController();

            //update assignment
            roleController.AddUserRole(portalSettings.PortalId, user.UserID, role.RoleID, effectiveDate, expiryDate);

            UserController.UpdateUser(portalSettings.PortalId, user);
            if (userRole == null)
            {
                eventLogController.AddLog("Role", role.RoleName, portalSettings, userId, EventLogController.EventLogType.USER_ROLE_CREATED);

                //send notification
                if (notifyUser)
                {
                    SendNotification(user, role, portalSettings, UserRoleActions.@add);
                }
            }
            else
            {
                eventLogController.AddLog("Role", role.RoleName, portalSettings, userId, EventLogController.EventLogType.USER_ROLE_UPDATED);
                if (notifyUser)
                {
                    roleController.GetUserRole(portalSettings.PortalId, user.UserID, role.RoleID);
                    SendNotification(user, role, portalSettings, UserRoleActions.update);
                }
            }

            //Remove the UserInfo from the Cache, as it has been modified
            DataCache.ClearUserCache(portalSettings.PortalId, user.Username);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Determines if the specified user can be removed from a role
        /// </summary>
        /// <remarks>
        /// Roles such as "Registered Users" and "Administrators" can only
        /// be removed in certain circumstances
        /// </remarks>
        /// <param name="PortalSettings">A <see cref="PortalSettings">PortalSettings</see> structure representing the current portal settings</param>
        /// <param name="UserId">The Id of the User that should be checked for role removability</param>
        /// <param name="RoleId">The Id of the Role that should be checked for removability</param>
        /// <returns></returns>
        /// <history>
        /// 	[anurse]	01/12/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool CanRemoveUserFromRole(PortalSettings PortalSettings, int UserId, int RoleId)
        {
            //[DNN-4285] Refactored this check into a method for use in SecurityRoles.ascx.vb
            //HACK: Duplicated in CanRemoveUserFromRole(PortalInfo, Integer, Integer) method below
            //changes to this method should be reflected in the other method as well
            return !((PortalSettings.AdministratorId == UserId && PortalSettings.AdministratorRoleId == RoleId) || PortalSettings.RegisteredRoleId == RoleId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Determines if the specified user can be removed from a role
        /// </summary>
        /// <remarks>
        /// Roles such as "Registered Users" and "Administrators" can only
        /// be removed in certain circumstances
        /// </remarks>
        /// <param name="PortalInfo">A <see cref="PortalInfo">PortalInfo</see> structure representing the current portal</param>
        /// <param name="UserId">The Id of the User</param>
        /// <param name="RoleId">The Id of the Role that should be checked for removability</param>
        /// <returns></returns>
        /// <history>
        /// 	[anurse]	01/12/2007	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool CanRemoveUserFromRole(PortalInfo PortalInfo, int UserId, int RoleId)
        {
            //[DNN-4285] Refactored this check into a method for use in SecurityRoles.ascx.vb
            //HACK: Duplicated in CanRemoveUserFromRole(PortalSettings, Integer, Integer) method above
            //changes to this method should be reflected in the other method as well

            return !((PortalInfo.AdministratorId == UserId && PortalInfo.AdministratorRoleId == RoleId) || PortalInfo.RegisteredRoleId == RoleId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Removes a User from a Role
        /// </summary>
        /// <param name="objUser">The user to remove</param>
        /// <param name="objRole">The role to remove the use from</param>
        /// <param name="PortalSettings">The PortalSettings of the Portal</param>
        /// <param name="notifyUser">A flag that indicates whether the user should be notified</param>
        /// <history>
        ///     [cnurse]    10/17/2007  Created  (Refactored code from Security Roles user control)
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool DeleteUserRole(UserInfo objUser, RoleInfo objRole, PortalSettings PortalSettings, bool notifyUser)
        {
            bool canDelete = DeleteUserRoleInternal(PortalSettings.PortalId, objUser.UserID, objRole.RoleID);
            if (canDelete)
            {
                if (notifyUser)
                {
                    SendNotification(objUser, objRole, PortalSettings, UserRoleActions.delete);
                }
            }
            return canDelete;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a User/Role
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <param name="UserId">The Id of the user</param>
        /// <param name="RoleId">The Id of the Role</param>
        /// <returns>A UserRoleInfo object</returns>
        /// <history>
        /// 	[cnurse]	05/24/2005	Documented
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public UserRoleInfo GetUserRole(int PortalID, int UserId, int RoleId)
        {
            return provider.GetUserRole(PortalID, UserId, RoleId);
        }

        /// <summary>
        /// Gets a list of UserRoles for the user
        /// </summary>
        /// <param name="user">A UserInfo object representaing the user</param>
        /// <param name="includePrivate">Include private roles.</param>
        /// <returns>A list of UserRoleInfo objects</returns>
        public IList<UserRoleInfo> GetUserRoles(UserInfo user, bool includePrivate)
        {
            return provider.GetUserRoles(user, includePrivate);
        }

        public IList<UserRoleInfo> GetUserRoles(int portalID, string userName, string roleName)
        {
            return provider.GetUserRoles(portalID, userName, roleName).Cast<UserRoleInfo>().ToList();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the users in a role (as User objects)
        /// </summary>
        /// <param name="portalId">Id of the portal (If -1 all roles for all portals are
        /// retrieved.</param>
        /// <param name="roleName">The role to fetch users for</param>
        /// <returns>An ArrayList of UserInfo objects</returns>
        /// <history>
        ///     [cnurse]	01/27/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
		public ArrayList GetUsersByRoleName(int portalId, string roleName)
        {
			return provider.GetUsersByRoleName(portalId, roleName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Service (UserRole)
        /// </summary>
		/// <param name="portalId">The Id of the Portal</param>
		/// <param name="userId">The Id of the User</param>
		/// <param name="roleId">The Id of the Role</param>
        /// <history>
        /// 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateUserRole(int portalId, int userId, int roleId)
        {
			UpdateUserRole(portalId, userId, roleId, false);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Service (UserRole)
        /// </summary>
		/// <param name="portalId">The Id of the Portal</param>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role</param>
        /// <param name="cancel">A flag that indicates whether to cancel (delete) the userrole</param>
        /// <history>
        /// 	[Nik Kalyani]	10/15/2004	Created multiple signatures to eliminate Optional parameters
        ///     [cnurse]    12/15/2005  Abstracted to MembershipProvider
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateUserRole(int portalId, int userId, int roleId, bool cancel)
        {
            UserInfo user = UserController.GetUserById(portalId, userId);
            UserRoleInfo userRole = GetUserRole(portalId, userId, roleId);
            var eventLogController = new EventLogController();
            if (cancel)
            {
                if (userRole != null && userRole.ServiceFee > 0.0 && userRole.IsTrialUsed)
                {
					//Expire Role so we retain trial used data
                    userRole.ExpiryDate = DateTime.Now.AddDays(-1);
                    provider.UpdateUserRole(userRole);
                    eventLogController.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_UPDATED);
                }
                else
                {
					//Delete Role
                    DeleteUserRoleInternal(portalId, userId, roleId);
                    eventLogController.AddLog("UserId",
                                       userId.ToString(CultureInfo.InvariantCulture),
                                       PortalController.GetCurrentPortalSettings(),
                                       UserController.GetCurrentUserInfo().UserID,
                                       EventLogController.EventLogType.USER_ROLE_DELETED);
                }
            }
            else
            {
                int UserRoleId = -1;
                DateTime ExpiryDate = DateTime.Now;
                DateTime EffectiveDate = Null.NullDate;
                bool IsTrialUsed = false;
                int Period = 0;
                string Frequency = "";
                if (userRole != null)
                {
                    UserRoleId = userRole.UserRoleID;
                    EffectiveDate = userRole.EffectiveDate;
                    ExpiryDate = userRole.ExpiryDate;
                    IsTrialUsed = userRole.IsTrialUsed;
                }
                RoleInfo role = GetRole(roleId, portalId);
                if (role != null)
                {
                    if (IsTrialUsed == false && role.TrialFrequency != "N")
                    {
                        Period = role.TrialPeriod;
                        Frequency = role.TrialFrequency;
                    }
                    else
                    {
                        Period = role.BillingPeriod;
                        Frequency = role.BillingFrequency;
                    }
                }
                if (EffectiveDate < DateTime.Now)
                {
                    EffectiveDate = Null.NullDate;
                }
                if (ExpiryDate < DateTime.Now)
                {
                    ExpiryDate = DateTime.Now;
                }
                if (Period == Null.NullInteger)
                {
                    ExpiryDate = Null.NullDate;
                }
                else
                {
                    switch (Frequency)
                    {
                        case "N":
                            ExpiryDate = Null.NullDate;
                            break;
                        case "O":
                            ExpiryDate = new DateTime(9999, 12, 31);
                            break;
                        case "D":
                            ExpiryDate = ExpiryDate.AddDays(Period);
                            break;
                        case "W":
                            ExpiryDate = ExpiryDate.AddDays(Period*7);
                            break;
                        case "M":
                            ExpiryDate = ExpiryDate.AddMonths(Period);
                            break;
                        case "Y":
                            ExpiryDate = ExpiryDate.AddYears(Period);
                            break;
                    }
                }
                if (UserRoleId != -1)
                {
                    userRole.ExpiryDate = ExpiryDate;
                    provider.UpdateUserRole(userRole);
                    eventLogController.AddLog(userRole, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_UPDATED);
                }
                else
                {
                    AddUserRole(portalId, userId, roleId, EffectiveDate, ExpiryDate);
                }
            }

            //Remove the UserInfo from the Cache, as it has been modified
            DataCache.ClearUserCache(portalId, user.Username);
        }

        #endregion

        #region RoleGroupInfo Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a Role Group
        /// </summary>
        /// <param name="objRoleGroupInfo">The RoleGroup to Add</param>
        /// <returns>The Id of the new role</returns>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            var objEventLog = new EventLogController();
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_CREATED);
            return provider.CreateRoleGroup(objRoleGroupInfo);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a Role Group
        /// </summary>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteRoleGroup(int PortalID, int RoleGroupId)
        {
            DeleteRoleGroup(GetRoleGroup(PortalID, RoleGroupId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a Role Group
        /// </summary>
        /// <param name="objRoleGroupInfo">The RoleGroup to Delete</param>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteRoleGroup(RoleGroupInfo objRoleGroupInfo)
        {
            provider.DeleteRoleGroup(objRoleGroupInfo);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(objRoleGroupInfo, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_DELETED);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fetch a single RoleGroup
        /// </summary>
		/// <param name="portalId">The Id of the Portal</param>
		/// <param name="roleGroupId">Role Group ID</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId)
        {
			return provider.GetRoleGroup(portalId, roleGroupId);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fetch a single RoleGroup by Name
        /// </summary>
		/// <param name="portalId">The Id of the Portal</param>
        /// <param name="roleGroupName">Role Group Name</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
		public static RoleGroupInfo GetRoleGroupByName(int portalId, string roleGroupName)
        {
			return provider.GetRoleGroupByName(portalId, roleGroupName);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets an ArrayList of RoleGroups
        /// </summary>
        /// <param name="PortalID">The Id of the Portal</param>
        /// <returns>An ArrayList of RoleGroups</returns>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static ArrayList GetRoleGroups(int PortalID)
        {
            return provider.GetRoleGroups(PortalID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Serializes the role groups
        /// </summary>
        /// <param name="writer">An XmlWriter</param>
		/// <param name="portalID">The Id of the Portal</param>
        /// <history>
        /// 	[cnurse]	03/18/2008  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void SerializeRoleGroups(XmlWriter writer, int portalID)
        {
			//Serialize Role Groups
            writer.WriteStartElement("rolegroups");
            foreach (RoleGroupInfo objRoleGroup in GetRoleGroups(portalID))
            {
                CBO.SerializeObject(objRoleGroup, writer);
            }
			
            //Serialize Global Roles
            var globalRoleGroup = new RoleGroupInfo(Null.NullInteger, portalID, true)
                                      {
                                          RoleGroupName = "GlobalRoles",
                                          Description = "A dummy role group that represents the Global roles"
                                      };
            CBO.SerializeObject(globalRoleGroup, writer);
            writer.WriteEndElement();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Role Group
        /// </summary>
        /// <param name="roleGroup">The RoleGroup to Update</param>
        /// <history>
        /// 	[cnurse]	01/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdateRoleGroup(RoleGroupInfo roleGroup)
        {
            UpdateRoleGroup(roleGroup, false);
        }

        public static void UpdateRoleGroup(RoleGroupInfo roleGroup, bool includeRoles)
        {
            provider.UpdateRoleGroup(roleGroup);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(roleGroup, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.USER_ROLE_UPDATED);
            if (includeRoles)
            {
                var controller = new RoleController();
                foreach (RoleInfo role in roleGroup.Roles.Values)
                {
                    controller.UpdateRole(role);
                    objEventLog.AddLog(role, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.ROLE_UPDATED);
                }
            }
        }
		
		#endregion

        #region Obsoleted Methods, retained for Binary Compatability

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by AddRole(objRoleInfo)")]
        public int AddRole(RoleInfo roleInfo, bool SynchronizationMode)
        {
            return AddRole(roleInfo);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2.")]
        public static bool DeleteUserRole(int userId, RoleInfo objRole, PortalSettings PortalSettings, bool notifyUser)
        {
            UserInfo objUser = UserController.GetUserById(PortalSettings.PortalId, userId);
            return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2.")]
        public static bool DeleteUserRole(int roleId, UserInfo objUser, PortalSettings PortalSettings, bool notifyUser)
        {
            var objRoleController = new RoleController();
            RoleInfo objRole = objRoleController.GetRole(roleId, PortalSettings.PortalId);
            return DeleteUserRole(objUser, objRole, PortalSettings, notifyUser);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2.")]
        public bool DeleteUserRole(int PortalId, int UserId, int RoleId)
        {
            return DeleteUserRoleInternal(PortalId, UserId, RoleId);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by GetPortalRoles(PortalId)")]
        public ArrayList GetPortalRoles(int PortalId, bool SynchronizeRoles)
        {
            return GetPortalRoles(PortalId);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by GetRolesByUser")]
        public string[] GetPortalRolesByUser(int UserId, int PortalId)
        {
            return GetRolesByUser(UserId, PortalId);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by GetUserRoles")]
        public ArrayList GetServices(int PortalId)
        {
            return GetServices(PortalId, -1);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by GetUserRoles")]
        public ArrayList GetServices(int PortalId, int UserId)
        {
            return provider.GetUserRoles(PortalId, UserId, false);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2.")]
        public ArrayList GetUserRoles(int PortalId)
        {
            return GetUserRoles(PortalId, -1);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2. Replaced by overload that returns IList")]
        public ArrayList GetUserRoles(int PortalId, int UserId)
        {
            return provider.GetUserRoles(PortalId, UserId, true);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2. Replaced by overload that returns IList")]
        public ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate)
        {
            return provider.GetUserRoles(PortalId, UserId, includePrivate);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2. Replaced by overload of GetUserRoles that returns IList")]
        public ArrayList GetUserRolesByUsername(int PortalID, string Username, string Rolename)
        {
            return provider.GetUserRoles(PortalID, Username, Rolename);
        }

        [Obsolete("Deprecated in DotNetNuke 6.2.")]
        public ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            return provider.GetUserRoles(portalId, null, roleName);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by GetUserRolesByRoleName")]
        public ArrayList GetUsersInRole(int PortalID, string RoleName)
        {
            return provider.GetUserRolesByRoleName(PortalID, RoleName);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by UpdateUserRole")]
        public void UpdateService(int PortalId, int UserId, int RoleId)
        {
            UpdateUserRole(PortalId, UserId, RoleId, false);
        }

        [Obsolete("Deprecated in DotNetNuke 5.0. This function has been replaced by UpdateUserRole")]
        public void UpdateService(int PortalId, int UserId, int RoleId, bool Cancel)
        {
            UpdateUserRole(PortalId, UserId, RoleId, Cancel);
        }

        #endregion

     }
}
