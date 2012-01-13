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

using System.Collections;

using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Security.Roles
{
    public abstract class RoleProvider
    {
		#region "Shared/Static Methods"

        //return the provider
        public static RoleProvider Instance()
        {
            return ComponentFactory.GetComponent<RoleProvider>();
        }
		
		#endregion

		#region "Abstract Methods"

        //Roles
        public abstract bool CreateRole(int portalId, ref RoleInfo role);

        public abstract void DeleteRole(int portalId, ref RoleInfo role);

        public abstract RoleInfo GetRole(int portalId, int roleId);

        public abstract RoleInfo GetRole(int portalId, string roleName);

        public abstract string[] GetRoleNames(int portalId);

        public abstract string[] GetRoleNames(int portalId, int userId);

        public abstract ArrayList GetRoles(int portalId);

        public abstract ArrayList GetRolesByGroup(int portalId, int roleGroupId);

        public abstract void UpdateRole(RoleInfo role);

        //Role Groups
        public abstract int CreateRoleGroup(RoleGroupInfo roleGroup);

        public abstract void DeleteRoleGroup(RoleGroupInfo roleGroup);

        public abstract RoleGroupInfo GetRoleGroup(int portalId, int roleGroupId);

        public abstract ArrayList GetRoleGroups(int portalId);

        public abstract void UpdateRoleGroup(RoleGroupInfo roleGroup);

        //User Roles
        public abstract bool AddUserToRole(int portalId, UserInfo user, UserRoleInfo userRole);

        public abstract UserRoleInfo GetUserRole(int PortalId, int UserId, int RoleId);

        public abstract ArrayList GetUserRoles(int PortalId, int UserId, bool includePrivate);

        public abstract ArrayList GetUserRoles(int PortalId, string Username, string Rolename);

        public abstract ArrayList GetUsersByRoleName(int portalId, string roleName);

        public abstract ArrayList GetUserRolesByRoleName(int portalId, string roleName);

        public abstract void RemoveUserFromRole(int portalId, UserInfo user, UserRoleInfo userRole);

        public abstract void UpdateUserRole(UserRoleInfo userRole);

        public virtual RoleGroupInfo GetRoleGroupByName(int PortalID, string RoleGroupName)
        {
            return null;
        }
		
		#endregion
    }
}