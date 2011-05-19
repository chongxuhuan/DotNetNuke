#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
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
    public class ModulePermissionsGrid : PermissionsGrid
    {
        #region "Private Members"
		
		private bool _InheritViewPermissionsFromTab;
        private int _ModuleID = -1;
        private ModulePermissionCollection _ModulePermissions;
        private List<PermissionInfoBase> _PermissionsList;
        private int _TabId = -1;
        private int _ViewColumnIndex;
		
		#endregion
		
		#region "Protected Properties"

        protected override List<PermissionInfoBase> PermissionsList
        {
            get
            {
                if (_PermissionsList == null && _ModulePermissions != null)
                {
                    _PermissionsList = _ModulePermissions.ToList();
                }
                return _PermissionsList;
            }
        }
		
		#endregion
		
		#region "Public Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets whether the Module inherits the Page's(Tab's) permissions
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool InheritViewPermissionsFromTab
        {
            get
            {
                return _InheritViewPermissionsFromTab;
            }
            set
            {
                _InheritViewPermissionsFromTab = value;
                _PermissionsList = null;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Id of the Module
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int ModuleID
        {
            get
            {
                return _ModuleID;
            }
            set
            {
                _ModuleID = value;
                if (!Page.IsPostBack)
                {
                    GetModulePermissions();
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Id of the Tab associated with this module
        /// </summary>
        /// <history>
        ///     [cnurse]    24/11/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int TabId
        {
            get
            {
                return _TabId;
            }
            set
            {
                _TabId = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ModulePermission Collection
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModulePermissionCollection Permissions
        {
            get
            {
				//First Update Permissions in case they have been changed
                UpdatePermissions();

                //Return the ModulePermissions
                return _ModulePermissions;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the ModulePermissions from the Data Store
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void GetModulePermissions()
        {
            _ModulePermissions = new ModulePermissionCollection(ModulePermissionController.GetModulePermissions(ModuleID, TabId));
            _PermissionsList = null;
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
        private ModulePermissionInfo ParseKeys(string[] Settings)
        {
            var objModulePermission = new ModulePermissionInfo();

            //Call base class to load base properties
            base.ParsePermissionKeys(objModulePermission, Settings);
            if (String.IsNullOrEmpty(Settings[2]))
            {
                objModulePermission.ModulePermissionID = -1;
            }
            else
            {
                objModulePermission.ModulePermissionID = Convert.ToInt32(Settings[2]);
            }
            objModulePermission.ModuleID = ModuleID;
            return objModulePermission;
        }
		
		#endregion
		
		#region "Protected Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permissions">The permissions collection</param>
        /// <param name="user">The user to add</param>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void AddPermission(ArrayList permissions, UserInfo user)
        {
            bool isMatch = false;
            foreach (ModulePermissionInfo objModulePermission in _ModulePermissions)
            {
                if (objModulePermission.UserID == user.UserID)
                {
                    isMatch = true;
                    break;
                }
            }
			
			//user not found so add new
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

        protected override void AddPermission(PermissionInfo permission, int roleId, string roleName, int userId, string displayName, bool allowAccess)
        {
            var objPermission = new ModulePermissionInfo(permission);
            objPermission.ModuleID = ModuleID;
            objPermission.RoleID = roleId;
            objPermission.RoleName = roleName;
            objPermission.AllowAccess = allowAccess;
            objPermission.UserID = userId;
            objPermission.DisplayName = displayName;
            _ModulePermissions.Add(objPermission, true);

            //Clear Permission List
            _PermissionsList = null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <history>
        ///     [cnurse]    01/13/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
        {
            bool enabled;
            if (InheritViewPermissionsFromTab && column == _ViewColumnIndex)
            {
                enabled = false;
            }
            else
            {
                if (role.RoleID == AdministratorRoleId)
                {
                    enabled = false;
                }
                else
                {
                    enabled = true;
                }
            }
            return enabled;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="user">The user</param>
        /// <param name="column">The column of the Grid</param>
        /// <history>
        ///     [cnurse]    01/13/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override bool GetEnabled(PermissionInfo objPerm, UserInfo user, int column)
        {
            return InheritViewPermissionsFromTab && column != _ViewColumnIndex;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Value of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <param name="defaultState">Default State.</param>
        /// <returns>A Boolean (True or False)</returns>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override string GetPermission(PermissionInfo objPerm, RoleInfo role, int column, string defaultState)
        {
            string permission;
            if (InheritViewPermissionsFromTab && column == _ViewColumnIndex)
            {
                permission = PermissionTypeNull;
            }
            else
            {
                if (role.RoleID == AdministratorRoleId)
                {
                    permission = PermissionTypeGrant;
                }
                else
                {
					//Call base class method to handle standard permissions
                    permission = base.GetPermission(objPerm, role, column, defaultState);
                }
            }
            return permission;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Value of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="user">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <param name="defaultState">Default State.</param>
        /// <returns>A Boolean (True or False)</returns>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override string GetPermission(PermissionInfo objPerm, UserInfo user, int column, string defaultState)
        {
            string permission;
            if (InheritViewPermissionsFromTab && column == _ViewColumnIndex)
            {
                permission = PermissionTypeNull;
            }
            else
            {
				//Call base class method to handle standard permissions
                permission = base.GetPermission(objPerm, user, column, defaultState);
            }
            return permission;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Permissions from the Data Store
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override ArrayList GetPermissions()
        {
            var objPermissionController = new PermissionController();
            ArrayList arrPermissions = objPermissionController.GetPermissionsByModuleID(ModuleID);
            int i;
            for (i = 0; i <= arrPermissions.Count - 1; i++)
            {
                PermissionInfo objPermission;
                objPermission = (PermissionInfo) arrPermissions[i];
                if (objPermission.PermissionKey == "VIEW")
                {
                    _ViewColumnIndex = i + 1;
                }
            }
            return arrPermissions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Load the ViewState
        /// </summary>
        /// <param name="savedState">The saved state</param>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
				//Load State from the array of objects that was saved with SaveViewState.
                var myState = (object[]) savedState;
                
				//Load Base Controls ViewState
				if (myState[0] != null)
                {
                    base.LoadViewState(myState[0]);
                }
				
				//Load ModuleID
                if (myState[1] != null)
                {
                    ModuleID = Convert.ToInt32(myState[1]);
                }
				
				//Load InheritViewPermissionsFromTab
                if (myState[2] != null)
                {
                    InheritViewPermissionsFromTab = Convert.ToBoolean(myState[2]);
                }
				
				//Load ModulePermissions
                if (myState[3] != null)
                {
                    _ModulePermissions = new ModulePermissionCollection();
                    string state = Convert.ToString(myState[3]);
                    if (!String.IsNullOrEmpty(state))
                    {
						//First Break the String into individual Keys
                        string[] permissionKeys = state.Split(new[] {"##"}, StringSplitOptions.None);
                        foreach (string key in permissionKeys)
                        {
                            string[] Settings = key.Split('|');
                            _ModulePermissions.Add(ParseKeys(Settings));
                        }
                    }
                }
            }
        }

        protected override void RemovePermission(int permissionID, int roleID, int userID)
        {
            _ModulePermissions.Remove(permissionID, roleID, userID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Saves the ViewState
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override object SaveViewState()
        {
            var allStates = new object[4];
			
			//Save the Base Controls ViewState
            allStates[0] = base.SaveViewState();

            //Save the ModuleID
            allStates[1] = ModuleID;

            //Save the InheritViewPermissionsFromTab
            allStates[2] = InheritViewPermissionsFromTab;

            //Persist the ModulePermissions
            var sb = new StringBuilder();
            if (_ModulePermissions != null)
            {
                bool addDelimiter = false;
                foreach (ModulePermissionInfo objModulePermission in _ModulePermissions)
                {
                    if (addDelimiter)
                    {
                        sb.Append("##");
                    }
                    else
                    {
                        addDelimiter = true;
                    }
                    sb.Append(BuildKey(objModulePermission.AllowAccess,
                                       objModulePermission.PermissionID,
                                       objModulePermission.ModulePermissionID,
                                       objModulePermission.RoleID,
                                       objModulePermission.RoleName,
                                       objModulePermission.UserID,
                                       objModulePermission.DisplayName));
                }
            }
            allStates[3] = sb.ToString();
            return allStates;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// returns whether or not the derived grid supports Deny permissions
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override bool SupportsDenyPermissions()
        {
            return true;
        }
		
		#endregion
		
		#region "Public Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Overrides the Base method to Generate the Data Grid
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void GenerateDataGrid()
        {
        }
		
		#endregion
    }
}
