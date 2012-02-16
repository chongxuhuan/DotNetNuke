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

using System;
using System.Collections.Generic;
using System.Linq;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Log.EventLog;

namespace DotNetNuke.Security.Roles.Internal
{
    internal class RoleControllerImpl : IRoleController
    {
        #region Private Members

        private static readonly RoleProvider provider = RoleProvider.Instance();

        #endregion

        #region Private Methods

        private void AddMessage(RoleInfo roleInfo, EventLogController.EventLogType logType)
        {
            var eventLogController = new EventLogController();
            eventLogController.AddLog(roleInfo,
                                PortalController.GetCurrentPortalSettings(),
                                UserController.GetCurrentUserInfo().UserID,
                                "",
                                logType);

        }

        private void AutoAssignUsers(RoleInfo objRoleInfo)
        {
            if (objRoleInfo.AutoAssignment)
            {
                //loop through users for portal and add to role
                var arrUsers = UserController.GetUsers(objRoleInfo.PortalID);
                foreach (UserInfo objUser in arrUsers)
                {
                    try
                    {
                        var legacyRoleController = new RoleController();
                        legacyRoleController.AddUserRole(objRoleInfo.PortalID, objUser.UserID, objRoleInfo.RoleID, Null.NullDate, Null.NullDate);
                    }
                    catch (Exception exc)
                    {
                        //user already belongs to role
                        DnnLog.Error(exc);

                    }
                }
            }
        }

        private void ClearRoleCache(int portalId)
        {
            DataCache.RemoveCache(String.Format(DataCache.RolesCacheKey, portalId));
        }

        #endregion

        #region IRoleController Implementation

        public int AddRole(RoleInfo role)
        {
            Requires.NotNull("role", role);

            var roleId = -1;
            if (provider.CreateRole(role))
            {
                AddMessage(role, EventLogController.EventLogType.ROLE_CREATED);
                AutoAssignUsers(role);
                roleId = role.RoleID;

                ClearRoleCache(role.PortalID);
            }

            return roleId;
        }

        public void DeleteRole(RoleInfo role)
        {
            Requires.NotNull("role", role);

            AddMessage(role, EventLogController.EventLogType.ROLE_DELETED);
            provider.DeleteRole(role);

            ClearRoleCache(role.PortalID);
        }

        public RoleInfo GetRole(int portalId, Func<RoleInfo, bool> predicate)
        {
            return GetRoles(portalId).Where(predicate).SingleOrDefault();
        }

        public IList<RoleInfo> GetRoles(int portalId)
        {
            var cacheKey = String.Format(DataCache.RolesCacheKey, portalId);
            return CBO.GetCachedObject<IList<RoleInfo>>(new CacheItemArgs(cacheKey, DataCache.RolesCacheTimeOut, DataCache.RolesCachePriority, portalId),
                                                            c =>
                                                            {
                                                                var id = Convert.ToInt32(c.Params[0]);
                                                                return provider.GetRoles(id).Cast<RoleInfo>().ToList();
                                                            });
        }

        public IList<RoleInfo> GetRoles(int portalId, Func<RoleInfo, bool> predicate)
        {
            return GetRoles(portalId).Where(predicate).ToList();
        }

        public IDictionary<string, string> GetRoleSettings(int roleId)
        {
            return provider.GetRoleSettings(roleId);
        }

        public void UpdateRole(RoleInfo role)
        {
            Requires.NotNull("role", role);

            provider.UpdateRole(role);
            AddMessage(role, EventLogController.EventLogType.ROLE_UPDATED);
            AutoAssignUsers(role);

            ClearRoleCache(role.PortalID);
        }

        public void UpdateRoleSettings(RoleInfo role, bool clearCache)
        {
            provider.UpdateRoleSettings(role);

            if (clearCache)
            {
                ClearRoleCache(role.PortalID);
            }
        }

        #endregion
    }
}
