#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

        public override void DeleteRole(int portalId, ref RoleInfo role)
        {
            dataProvider.DeleteRole(role.RoleID);
        }

        public override RoleInfo GetRole(int portalId, int roleId)
        {
            return CBO.FillObject<RoleInfo>(dataProvider.GetRole(roleId, portalId));
        }

        public override RoleInfo GetRole(int portalId, string roleName)
        {
            return CBO.FillObject<RoleInfo>(dataProvider.GetRoleByName(portalId, roleName));
        }

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

        public override ArrayList GetRolesByGroup(int portalId, int roleGroupId)
        {
            return CBO.FillCollection(dataProvider.GetRolesByGroup(roleGroupId, portalId), typeof (RoleInfo));
        }

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

        public override int CreateRoleGroup(RoleGroupInfo roleGroup)
        {
            return Convert.ToInt32(dataProvider.AddRoleGroup(roleGroup.PortalID, roleGroup.RoleGroupName, roleGroup.Description, UserController.GetCurrentUserInfo().UserID));
        }

        public override void DeleteRoleGroup(RoleGroupInfo roleGroup)
        {
            dataProvider.DeleteRoleGroup(roleGroup.RoleGroupID);
        }

        public override RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId)
        {
            return CBO.FillObject<RoleGroupInfo>(dataProvider.GetRoleGroup(portalId, roleGroupId));
        }

        public override RoleGroupInfo GetRoleGroupByName(int PortalID, string RoleGroupName)
        {
            return CBO.FillObject<RoleGroupInfo>(dataProvider.GetRoleGroupByName(PortalID, RoleGroupName));
        }

        public override ArrayList GetRoleGroups(int portalId)
        {
            return CBO.FillCollection(dataProvider.GetRoleGroups(portalId), typeof (RoleGroupInfo));
        }

        public override void UpdateRoleGroup(RoleGroupInfo roleGroup)
        {
            dataProvider.UpdateRoleGroup(roleGroup.RoleGroupID, roleGroup.RoleGroupName, roleGroup.Description, UserController.GetCurrentUserInfo().UserID);
        }

        private void AddDNNUserRole(UserRoleInfo userRole)
        {
            userRole.UserRoleID =
                Convert.ToInt32(dataProvider.AddUserRole(userRole.PortalID, userRole.UserID, userRole.RoleID, userRole.EffectiveDate, userRole.ExpiryDate, UserController.GetCurrentUserInfo().UserID));
        }

        public override bool AddUserToRole(int portalId, UserInfo user, UserRoleInfo userRole)
        {
            bool createStatus = true;
            try
            {
                AddDNNUserRole(userRole);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

                createStatus = false;
            }
            return createStatus;
        }

        public override UserRoleInfo GetUserRole(int portalId, int userId, int roleId)
        {
            return CBO.FillObject<UserRoleInfo>(dataProvider.GetUserRole(portalId, userId, roleId));
        }

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

        public override ArrayList GetUserRoles(int portalId, string userName, string roleName)
        {
            return CBO.FillCollection(dataProvider.GetUserRolesByUsername(portalId, userName, roleName), typeof (UserRoleInfo));
        }

        public override ArrayList GetUserRolesByRoleName(int portalId, string roleName)
        {
            return GetUserRoles(portalId, null, roleName);
        }

        public override ArrayList GetUsersByRoleName(int portalId, string roleName)
        {
            return UserController.FillUserCollection(portalId, dataProvider.GetUsersByRolename(portalId, roleName));
        }

        public override void RemoveUserFromRole(int portalId, UserInfo user, UserRoleInfo userRole)
        {
            dataProvider.DeleteUserRole(userRole.UserID, userRole.RoleID);
        }

        public override void UpdateUserRole(UserRoleInfo userRole)
        {
            dataProvider.UpdateUserRole(userRole.UserRoleID, userRole.EffectiveDate, userRole.ExpiryDate, UserController.GetCurrentUserInfo().UserID);
        }
    }
}
