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

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Security.Roles;

#endregion

// ReSharper disable CheckNamespace
namespace DotNetNuke.Security.Membership
// ReSharper restore CheckNamespace
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Membership
    /// Class:      DNNRoleProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DNNRoleProvider overrides the default MembershipProvider to provide
    /// a purely DNN Membership Component implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	03/28/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class DNNRoleProvider : RoleProvider
    {
        private readonly DataProvider dataProvider;

        public DNNRoleProvider()
        {
            dataProvider = DataProvider.Instance();
            if (dataProvider == null)
            {
				//get the provider configuration based on the type
                string defaultprovider = DotNetNuke.Data.DataProvider.Instance().DefaultProviderName;
                const string dataProviderNamespace = "DotNetNuke.Security.Membership.Data";
                if (defaultprovider == "SqlDataProvider")
                {
                    dataProvider = new SqlDataProvider();
                }
                else
                {
                    string providerType = dataProviderNamespace + "." + defaultprovider;
                    dataProvider = (DataProvider) Reflection.CreateObject(providerType, providerType, true);
                }
                ComponentFactory.RegisterComponentInstance<DataProvider>(dataProvider);
            }
        }

        #region Private Methods

        private void AddDNNUserRole(UserRoleInfo userRole)
        {
            //Add UserRole to DNN
            userRole.UserRoleID = Convert.ToInt32(dataProvider.AddUserRole(userRole.PortalID, userRole.UserID, userRole.RoleID, 
                                                                userRole.EffectiveDate, userRole.ExpiryDate, 
                                                                UserController.GetCurrentUserInfo().UserID));
        }

        #endregion

        #region Role Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CreateRole persists a Role to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="role">The role to persist to the Data Store.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// -----------------------------------------------------------------------------
        public override bool CreateRole(RoleInfo role)
        {
            try
            {
                role.RoleID =
                    Convert.ToInt32(dataProvider.AddRole(role.PortalID,
                                                         role.RoleGroupID,
                                                         role.RoleName,
                                                         role.Description,
                                                         role.ServiceFee,
                                                         role.BillingPeriod.ToString(CultureInfo.InvariantCulture),
                                                         role.BillingFrequency,
                                                         role.TrialFee,
                                                         role.TrialPeriod,
                                                         role.TrialFrequency,
                                                         role.IsPublic,
                                                         role.AutoAssignment,
                                                         role.RSVPCode,
                                                         role.IconFile,
                                                         UserController.GetCurrentUserInfo().UserID,
                                                         (int)role.Status,
                                                         role.IsSecurityRole));
            }
            catch (SqlException e)
            {
                throw new ArgumentException(e.ToString());
            }

            return true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteRole deletes a Role from the Data Store
        /// </summary>
        /// <param name="role">The role to delete from the Data Store.</param>
        /// -----------------------------------------------------------------------------
        public override void DeleteRole(RoleInfo role)
        {
            dataProvider.DeleteRole(role.RoleID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the roles for a portal
        /// </summary>
        /// <param name="portalId">Id of the portal (If -1 all roles for all portals are 
        /// retrieved.</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetRoles(int portalId)
        {
            var arrRoles = CBO.FillCollection(portalId == Null.NullInteger 
                                        ? dataProvider.GetRoles() 
                                        : dataProvider.GetPortalRoles(portalId), typeof (RoleInfo));
            return arrRoles;
        }

        public override IDictionary<string, string> GetRoleSettings(int roleId)
        {
            return CBO.FillDictionary<string, string>("SettingName", dataProvider.GetRoleSettings(roleId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="role">The role to update</param>
        /// -----------------------------------------------------------------------------
        public override void UpdateRole(RoleInfo role)
        {
            dataProvider.UpdateRole(role.RoleID,
                                    role.RoleGroupID,
                                    role.Description,
                                    role.ServiceFee,
                                    role.BillingPeriod.ToString(CultureInfo.InvariantCulture),
                                    role.BillingFrequency,
                                    role.TrialFee,
                                    role.TrialPeriod,
                                    role.TrialFrequency,
                                    role.IsPublic,
                                    role.AutoAssignment,
                                    role.RSVPCode,
                                    role.IconFile,
                                    UserController.GetCurrentUserInfo().UserID,
                                    (int)role.Status,
                                    role.IsSecurityRole);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Update the role settings for a role
        /// </summary>
        /// <param name="role">The role to update</param>
        /// -----------------------------------------------------------------------------
        public override void UpdateRoleSettings(RoleInfo role)
        {
            foreach (var setting in role.Settings)
            {
                dataProvider.UpdateRoleSetting(role.RoleID, setting.Key, setting.Value, UserController.GetCurrentUserInfo().UserID);
            }
        }

		#endregion
		
		#region User Role Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddUserToRole adds a User to a Role
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="user">The user to add.</param>
        /// <param name="userRole">The role to add the user to.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override bool AddUserToRole(int portalId, UserInfo user, UserRoleInfo userRole)
        {
            bool createStatus = true;
            try
            {
				//Add UserRole to DNN
                AddDNNUserRole(userRole);
            }
            catch (Exception exc)
            {
				//Clear User (duplicate User information)
                DnnLog.Error(exc);

                createStatus = false;
            }
            return createStatus;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserRole gets a User/Role object from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="userId">The Id of the User</param>
        /// <param name="roleId">The Id of the Role.</param>
        /// <returns>The UserRoleInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override UserRoleInfo GetUserRole(int portalId, int userId, int roleId)
        {
            return CBO.FillObject<UserRoleInfo>(dataProvider.GetUserRole(portalId, userId, roleId));
        }

        /// <summary>
        /// Gets a list of UserRoles for the user
        /// </summary>
        /// <param name="user">A UserInfo object representaing the user</param>
        /// <param name="includePrivate">Include private roles.</param>
        /// <returns>A list of UserRoleInfo objects</returns>
        public override IList<UserRoleInfo> GetUserRoles(UserInfo user, bool includePrivate)
        {
            return CBO.FillCollection<UserRoleInfo>(includePrivate 
                    ? dataProvider.GetUserRoles(user.PortalID, user.UserID) 
                    : dataProvider.GetServices(user.PortalID, user.UserID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserRoles gets a collection of User/Role objects from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="userName">The user to fetch roles for</param>
        /// <param name="roleName">The role to fetch users for</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetUserRoles(int portalId, string userName, string roleName)
        {
            return CBO.FillCollection(dataProvider.GetUserRolesByUsername(portalId, userName, roleName), typeof (UserRoleInfo));
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
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetUsersByRoleName(int portalId, string roleName)
        {
            return AspNetMembershipProvider.FillUserCollection(portalId, dataProvider.GetUsersByRolename(portalId, roleName));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Remove a User from a Role
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="user">The user to remove.</param>
        /// <param name="userRole">The role to remove the user from.</param>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void RemoveUserFromRole(int portalId, UserInfo user, UserRoleInfo userRole)
        {
            dataProvider.DeleteUserRole(userRole.UserID, userRole.RoleID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a User/Role
        /// </summary>
        /// <param name="userRole">The User/Role to update</param>
        /// <history>
        ///     [cnurse]	12/15/2005	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UpdateUserRole(UserRoleInfo userRole)
        {
            dataProvider.UpdateUserRole(userRole.UserRoleID, userRole.EffectiveDate, userRole.ExpiryDate, UserController.GetCurrentUserInfo().UserID);
		}

		#endregion

        #region RoleGroup Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CreateRoleGroup persists a RoleGroup to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="roleGroup">The RoleGroup to persist to the Data Store.</param>
        /// <returns>The Id of the new role.</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        ///     [jlucarino]	02/26/2009	added CreatedByUserID parameter
        /// </history>
        /// -----------------------------------------------------------------------------
        public override int CreateRoleGroup(RoleGroupInfo roleGroup)
        {
            return Convert.ToInt32(dataProvider.AddRoleGroup(roleGroup.PortalID, roleGroup.RoleGroupName, roleGroup.Description, UserController.GetCurrentUserInfo().UserID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteRoleGroup deletes a RoleGroup from the Data Store
        /// </summary>
        /// <param name="roleGroup">The RoleGroup to delete from the Data Store.</param>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void DeleteRoleGroup(RoleGroupInfo roleGroup)
        {
            dataProvider.DeleteRoleGroup(roleGroup.RoleGroupID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetRoleGroup gets a RoleGroup from the Data Store
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="roleGroupId">The Id of the RoleGroup to retrieve.</param>
        /// <returns>A RoleGroupInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId)
        {
            return CBO.FillObject<RoleGroupInfo>(dataProvider.GetRoleGroup(portalId, roleGroupId));
        }

        public override RoleGroupInfo GetRoleGroupByName(int PortalID, string RoleGroupName)
        {
            return CBO.FillObject<RoleGroupInfo>(dataProvider.GetRoleGroupByName(PortalID, RoleGroupName));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the RoleGroups for a portal
        /// </summary>
        /// <param name="portalId">Id of the portal.</param>
        /// <returns>An ArrayList of RoleGroupInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetRoleGroups(int portalId)
        {
            return CBO.FillCollection(dataProvider.GetRoleGroups(portalId), typeof(RoleGroupInfo));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Update a RoleGroup
        /// </summary>
        /// <param name="roleGroup">The RoleGroup to update</param>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        ///     [jlucarino]	02/26/2009	added LastModifiedByUserID parameter
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UpdateRoleGroup(RoleGroupInfo roleGroup)
        {
            dataProvider.UpdateRoleGroup(roleGroup.RoleGroupID, roleGroup.RoleGroupName, roleGroup.Description, UserController.GetCurrentUserInfo().UserID);
        }
		

        #endregion
    }
}
