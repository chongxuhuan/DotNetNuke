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
using System.Data.SqlClient;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Security.Membership
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
                string dataProviderNamespace = "DotNetNuke.Security.Membership.Data";
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
            userRole.UserRoleID =
                Convert.ToInt32(dataProvider.AddUserRole(userRole.PortalID, userRole.UserID, userRole.RoleID, userRole.EffectiveDate, userRole.ExpiryDate, UserController.GetCurrentUserInfo().UserID));
        }

        #endregion

        #region Role Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CreateRole persists a Role to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="role">The role to persist to the Data Store.</param>
        /// <returns>A Boolean indicating success or failure.</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        ///     [jlucarino]	02/23/2006	added CreatedByUserID parameter
        /// </history>
        /// -----------------------------------------------------------------------------
        public override bool CreateRole(int portalId, ref RoleInfo role)
        {
            try
            {
                role.RoleID =
                    Convert.ToInt32(dataProvider.AddRole(role.PortalID,
                                                         role.RoleGroupID,
                                                         role.RoleName,
                                                         role.Description,
                                                         role.ServiceFee,
                                                         role.BillingPeriod.ToString(),
                                                         role.BillingFrequency,
                                                         role.TrialFee,
                                                         role.TrialPeriod,
                                                         role.TrialFrequency,
                                                         role.IsPublic,
                                                         role.AutoAssignment,
                                                         role.RSVPCode,
                                                         role.IconFile,
                                                         UserController.GetCurrentUserInfo().UserID));
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
        /// <param name="portalId">Id of the portal</param>
        /// <param name="role">The role to delete from the Data Store.</param>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void DeleteRole(int portalId, ref RoleInfo role)
        {
            dataProvider.DeleteRole(role.RoleID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetRole gets a role from the Data Store
        /// </summary>
        /// <remarks>This overload gets the role by its ID</remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="roleId">The Id of the role to retrieve.</param>
        /// <returns>A RoleInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override RoleInfo GetRole(int portalId, int roleId)
        {
            return CBO.FillObject<RoleInfo>(dataProvider.GetRole(roleId, portalId));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetRole gets a role from the Data Store
        /// </summary>
        /// <remarks>This overload gets the role by its name</remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="roleName">The name of the role to retrieve.</param>
        /// <returns>A RoleInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override RoleInfo GetRole(int portalId, string roleName)
        {
            return CBO.FillObject<RoleInfo>(dataProvider.GetRoleByName(portalId, roleName));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetRoleNames gets an array of roles for a portal
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <returns>A RoleInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override string[] GetRoleNames(int portalId)
        {
            string[] roles = {};
            string strRoles = "";
            ArrayList arrRoles = GetRoles(portalId);
            foreach (RoleInfo role in arrRoles)
            {
                strRoles += role.RoleName + "|";
            }
            if (strRoles.IndexOf("|") > 0)
            {
                roles = strRoles.Substring(0, strRoles.Length - 1).Split('|');
            }
            return roles;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetRoleNames gets an array of roles
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="userId">The Id of the user whose roles are required. (If -1 then all
        /// rolenames in a portal are retrieved.</param>
        /// <returns>A RoleInfo object</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override string[] GetRoleNames(int portalId, int userId)
        {
            string[] roles = {};
            string strRoles = "";
            IDataReader dr = dataProvider.GetRolesByUser(userId, portalId);
            try
            {
                while (dr.Read())
                {
                    strRoles += Convert.ToString(dr["RoleName"]) + "|";
                }
            }
            finally
            {
                CBO.CloseDataReader(dr, true);
            }
            if (strRoles.IndexOf("|") > 0)
            {
                roles = strRoles.Substring(0, strRoles.Length - 1).Split('|');
            }
            return roles;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the roles for a portal
        /// </summary>
        /// <param name="portalId">Id of the portal (If -1 all roles for all portals are 
        /// retrieved.</param>
        /// <returns>An ArrayList of RoleInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetRoles(int portalId)
        {
            ArrayList arrRoles;
            if (portalId == Null.NullInteger)
            {
                arrRoles = CBO.FillCollection(dataProvider.GetRoles(), typeof (RoleInfo));
            }
            else
            {
                arrRoles = CBO.FillCollection(dataProvider.GetPortalRoles(portalId), typeof (RoleInfo));
            }
            return arrRoles;
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
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            return CBO.FillCollection(dataProvider.GetRolesByGroup(roleGroupId, portalId), typeof (RoleInfo));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="role">The role to update</param>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        ///     [jlucarino]	02/23/2006	added LastModifiedByUserID parameter
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UpdateRole(RoleInfo role)
        {
            dataProvider.UpdateRole(role.RoleID,
                                    role.RoleGroupID,
                                    role.Description,
                                    role.ServiceFee,
                                    role.BillingPeriod.ToString(),
                                    role.BillingFrequency,
                                    role.TrialFee,
                                    role.TrialPeriod,
                                    role.TrialFrequency,
                                    role.IsPublic,
                                    role.AutoAssignment,
                                    role.RSVPCode,
                                    role.IconFile,
                                    UserController.GetCurrentUserInfo().UserID);
        }

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
            return CBO.FillCollection(dataProvider.GetRoleGroups(portalId), typeof (RoleGroupInfo));
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserRoles gets a collection of User/Role objects from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="userId">The user to fetch roles for</param>
        /// <param name="includePrivate">Include private roles.</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetUserRoles(int portalId, int userId, bool includePrivate)
        {
            if (includePrivate)
            {
                return CBO.FillCollection(dataProvider.GetUserRoles(portalId, userId), typeof (UserRoleInfo));
            }
            else
            {
                return CBO.FillCollection(dataProvider.GetServices(portalId, userId), typeof (UserRoleInfo));
            }
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
        /// Get the users in a role (as UserRole objects)
        /// </summary>
        /// <param name="portalId">Id of the portal (If -1 all roles for all portals are 
        /// retrieved.</param>
        /// <param name="roleName">The role to fetch users for</param>
        /// <returns>An ArrayList of UserRoleInfo objects</returns>
        /// <history>
        ///     [cnurse]	03/28/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            return GetUserRoles(portalId, null, roleName);
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
	}
}
