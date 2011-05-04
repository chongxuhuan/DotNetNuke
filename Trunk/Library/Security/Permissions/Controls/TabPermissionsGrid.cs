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
using System.Collections.Generic;
using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

#endregion

namespace DotNetNuke.Security.Permissions.Controls
{
    public class TabPermissionsGrid : PermissionsGrid
    {
        private List<PermissionInfoBase> _PermissionsList;
        private int _TabID = -1;
        private TabPermissionCollection _TabPermissions;

        protected override List<PermissionInfoBase> PermissionsList
        {
            get
            {
                if (_PermissionsList == null && _TabPermissions != null)
                {
                    _PermissionsList = _TabPermissions.ToList();
                }
                return _PermissionsList;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Permissions Collection
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public TabPermissionCollection Permissions
        {
            get
            {
                UpdatePermissions();
                return _TabPermissions;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Id of the Tab
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int TabID
        {
            get
            {
                return _TabID;
            }
            set
            {
                _TabID = value;
                if (!Page.IsPostBack)
                {
                    GetTabPermissions();
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the TabPermissions from the Data Store
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void GetTabPermissions()
        {
            _TabPermissions = new TabPermissionCollection(TabPermissionController.GetTabPermissions(TabID, PortalId));
            _PermissionsList = null;
        }

        public override void DataBind()
        {
            GetTabPermissions();
            base.DataBind();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Parse the Permission Keys used to persist the Permissions in the ViewState
        /// </summary>
        /// <param name="Settings">A string array of settings</param>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private TabPermissionInfo ParseKeys(string[] Settings)
        {
            var objTabPermission = new TabPermissionInfo();
            base.ParsePermissionKeys(objTabPermission, Settings);
            if (String.IsNullOrEmpty(Settings[2]))
            {
                objTabPermission.TabPermissionID = -1;
            }
            else
            {
                objTabPermission.TabPermissionID = Convert.ToInt32(Settings[2]);
            }
            objTabPermission.TabID = TabID;
            return objTabPermission;
        }

        protected override void AddPermission(PermissionInfo permission, int roleId, string roleName, int userId, string displayName, bool allowAccess)
        {
            var objPermission = new TabPermissionInfo(permission);
            objPermission.TabID = TabID;
            objPermission.RoleID = roleId;
            objPermission.RoleName = roleName;
            objPermission.AllowAccess = allowAccess;
            objPermission.UserID = userId;
            objPermission.DisplayName = displayName;
            _TabPermissions.Add(objPermission, true);
            _PermissionsList = null;
        }

        protected override void AddPermission(ArrayList permissions, UserInfo user)
        {
            bool isMatch = false;
            foreach (TabPermissionInfo objTabPermission in _TabPermissions)
            {
                if (objTabPermission.UserID == user.UserID)
                {
                    isMatch = true;
                    break;
                }
            }
            if (!isMatch)
            {
                foreach (PermissionInfo objPermission in permissions)
                {
                    if (objPermission.PermissionKey == "VIEW")
                    {
                        AddPermission(objPermission, int.Parse(Globals.glbRoleNothing), Null.NullString, user.UserID, user.DisplayName, true);
                    }
                }
            }
        }

        protected override bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
        {
            return role.RoleID != AdministratorRoleId;
        }

        protected override string GetPermission(PermissionInfo objPerm, RoleInfo role, int column, string defaultState)
        {
            string permission;
            if (role.RoleID == AdministratorRoleId)
            {
                permission = PermissionTypeGrant;
            }
            else
            {
                permission = base.GetPermission(objPerm, role, column, PermissionTypeNull);
            }
            return permission;
        }

        protected override ArrayList GetPermissions()
        {
            return PermissionController.GetPermissionsByTab();
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                var myState = (object[]) savedState;
                if (myState[0] != null)
                {
                    base.LoadViewState(myState[0]);
                }
                if (myState[1] != null)
                {
                    TabID = Convert.ToInt32(myState[1]);
                }
                if (myState[2] != null)
                {
                    _TabPermissions = new TabPermissionCollection();
                    string state = Convert.ToString(myState[2]);
                    if (!String.IsNullOrEmpty(state))
                    {
                        string[] permissionKeys = state.Split(new[] {"##"}, StringSplitOptions.None);
                        foreach (string key in permissionKeys)
                        {
                            string[] Settings = key.Split('|');
                            _TabPermissions.Add(ParseKeys(Settings));
                        }
                    }
                }
            }
        }

        protected override void RemovePermission(int permissionID, int roleID, int userID)
        {
            _TabPermissions.Remove(permissionID, roleID, userID);
        }

        protected override object SaveViewState()
        {
            var allStates = new object[3];
            allStates[0] = base.SaveViewState();
            allStates[1] = TabID;
            var sb = new StringBuilder();
            if (_TabPermissions != null)
            {
                bool addDelimiter = false;
                foreach (TabPermissionInfo objTabPermission in _TabPermissions)
                {
                    if (addDelimiter)
                    {
                        sb.Append("##");
                    }
                    else
                    {
                        addDelimiter = true;
                    }
                    sb.Append(BuildKey(objTabPermission.AllowAccess,
                                       objTabPermission.PermissionID,
                                       objTabPermission.TabPermissionID,
                                       objTabPermission.RoleID,
                                       objTabPermission.RoleName,
                                       objTabPermission.UserID,
                                       objTabPermission.DisplayName));
                }
            }
            allStates[2] = sb.ToString();
            return allStates;
        }

        protected override bool SupportsDenyPermissions()
        {
            return true;
        }

        public override void GenerateDataGrid()
        {
        }
    }
}
