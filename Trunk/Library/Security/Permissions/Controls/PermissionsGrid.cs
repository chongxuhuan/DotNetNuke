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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Security.Permissions.Controls
{
    public abstract class PermissionsGrid : Control, INamingContainer
    {
        protected const string PermissionTypeGrant = "True";
        protected const string PermissionTypeDeny = "False";
        protected const string PermissionTypeNull = "Null";
        private readonly DataTable _dtRolePermissions = new DataTable();
        private readonly DataTable _dtUserPermissions = new DataTable();
        private ArrayList _permissions;
        private ArrayList _roles;
        private ArrayList _users;
        private DropDownList cboRoleGroups;
        private CommandButton cmdUser;
        private DataGrid dgRolePermissions;
        private DataGrid dgUserPermissions;
        private Label lblGroups;
        private Label lblUser;
        private Panel pnlPermissions;
        private TextBox txtUser;

        protected virtual List<PermissionInfoBase> PermissionsList
        {
            get
            {
                return null;
            }
        }

        protected virtual bool RefreshGrid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Registers the scripts neccesary to make the tri-state controls work inside a RadAjaxPanel
        /// </summary>
        /// <remarks>
        /// No need to call this unless using the PermissionGrid inside an ajax control that omits scripts on postback
        /// See DesktopModules/Admin/Tabs.ascx.cs for an example of usage
        /// </remarks>
        public void RegisterScriptsForAjaxPanel()
        {
            PermissionTriState.RegisterScripts(Page, this);
        }

        public TableItemStyle AlternatingItemStyle
        {
            get
            {
                return dgRolePermissions.AlternatingItemStyle;
            }
        }

        public bool AutoGenerateColumns
        {
            get
            {
                return dgRolePermissions.AutoGenerateColumns;
            }
            set
            {
                dgRolePermissions.AutoGenerateColumns = value;
                dgUserPermissions.AutoGenerateColumns = value;
            }
        }

        public int CellSpacing
        {
            get
            {
                return dgRolePermissions.CellSpacing;
            }
            set
            {
                dgRolePermissions.CellSpacing = value;
                dgUserPermissions.CellSpacing = value;
            }
        }

        public DataGridColumnCollection Columns
        {
            get
            {
                return dgRolePermissions.Columns;
            }
        }

        public TableItemStyle FooterStyle
        {
            get
            {
                return dgRolePermissions.FooterStyle;
            }
        }

        public GridLines GridLines
        {
            get
            {
                return dgRolePermissions.GridLines;
            }
            set
            {
                dgRolePermissions.GridLines = value;
                dgUserPermissions.GridLines = value;
            }
        }

        public TableItemStyle HeaderStyle
        {
            get
            {
                return dgRolePermissions.HeaderStyle;
            }
        }

        public TableItemStyle ItemStyle
        {
            get
            {
                return dgRolePermissions.ItemStyle;
            }
        }

        public DataGridItemCollection Items
        {
            get
            {
                return dgRolePermissions.Items;
            }
        }

        public TableItemStyle SelectedItemStyle
        {
            get
            {
                return dgRolePermissions.SelectedItemStyle;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Id of the Administrator Role
        /// </summary>
        /// <history>
        ///     [cnurse]    01/16/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AdministratorRoleId
        {
            get
            {
                return PortalController.GetCurrentPortalSettings().AdministratorRoleId;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Id of the Registered Users Role
        /// </summary>
        /// <history>
        ///     [cnurse]    01/16/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int RegisteredUsersRoleId
        {
            get
            {
                return PortalController.GetCurrentPortalSettings().RegisteredRoleId;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets whether a Dynamic Column has been added
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool DynamicColumnAdded
        {
            get
            {
                if (ViewState["ColumnAdded"] == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                ViewState["ColumnAdded"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the underlying Permissions Data Table
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public DataTable dtRolePermissions
        {
            get
            {
                return _dtRolePermissions;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the underlying Permissions Data Table
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public DataTable dtUserPermissions
        {
            get
            {
                return _dtUserPermissions;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Id of the Portal
        /// </summary>
        /// <history>
        ///     [cnurse]    01/16/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int PortalId
        {
            get
            {
                PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
                int intPortalID;
                if (_portalSettings.ActiveTab.ParentId == _portalSettings.SuperTabId)
                {
                    intPortalID = Null.NullInteger;
                }
                else
                {
                    intPortalID = _portalSettings.PortalId;
                }
                return intPortalID;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the collection of Roles to display
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ArrayList Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets and Sets the ResourceFile to localize permissions
 /// </summary>
 /// <history>
 ///     [vmasanas]    02/24/2006  Created
 /// </history>
 /// -----------------------------------------------------------------------------
        public string ResourceFile { get; set; }

        public abstract void GenerateDataGrid();

        private void BindData()
        {
            EnsureChildControls();
            BindRolesGrid();
            BindUsersGrid();
        }

        private void BindRolesGrid()
        {
            dtRolePermissions.Columns.Clear();
            dtRolePermissions.Rows.Clear();
            DataColumn col;
            col = new DataColumn("RoleId");
            dtRolePermissions.Columns.Add(col);
            col = new DataColumn("RoleName");
            dtRolePermissions.Columns.Add(col);
            int i;
            for (i = 0; i <= _permissions.Count - 1; i++)
            {
                PermissionInfo objPerm;
                objPerm = (PermissionInfo) _permissions[i];
                col = new DataColumn(objPerm.PermissionName + "_Enabled");
                dtRolePermissions.Columns.Add(col);
                col = new DataColumn(objPerm.PermissionName);
                dtRolePermissions.Columns.Add(col);
            }
            GetRoles();
            UpdateRolePermissions();
            DataRow row;
            for (i = 0; i <= Roles.Count - 1; i++)
            {
                var role = (RoleInfo) Roles[i];
                row = dtRolePermissions.NewRow();
                row["RoleId"] = role.RoleID;
                row["RoleName"] = Localization.LocalizeRole(role.RoleName);
                int j;
                for (j = 0; j <= _permissions.Count - 1; j++)
                {
                    PermissionInfo objPerm;
                    objPerm = (PermissionInfo) _permissions[j];
                    row[objPerm.PermissionName + "_Enabled"] = GetEnabled(objPerm, role, j + 1);
                    if (SupportsDenyPermissions())
                    {
                        row[objPerm.PermissionName] = GetPermission(objPerm, role, j + 1, PermissionTypeNull);
                    }
                    else
                    {
                        if (GetPermission(objPerm, role, j + 1))
                        {
                            row[objPerm.PermissionName] = PermissionTypeGrant;
                        }
                        else
                        {
                            row[objPerm.PermissionName] = PermissionTypeNull;
                        }
                    }
                }
                dtRolePermissions.Rows.Add(row);
            }
            dgRolePermissions.DataSource = dtRolePermissions;
            dgRolePermissions.DataBind();
        }

        private void BindUsersGrid()
        {
            dtUserPermissions.Columns.Clear();
            dtUserPermissions.Rows.Clear();
            DataColumn col;
            col = new DataColumn("UserId");
            dtUserPermissions.Columns.Add(col);
            col = new DataColumn("DisplayName");
            dtUserPermissions.Columns.Add(col);
            int i;
            for (i = 0; i <= _permissions.Count - 1; i++)
            {
                PermissionInfo objPerm;
                objPerm = (PermissionInfo) _permissions[i];
                col = new DataColumn(objPerm.PermissionName + "_Enabled");
                dtUserPermissions.Columns.Add(col);
                col = new DataColumn(objPerm.PermissionName);
                dtUserPermissions.Columns.Add(col);
            }
            if (dgUserPermissions != null)
            {
                _users = GetUsers();
                if (_users.Count != 0)
                {
                    dgUserPermissions.Visible = true;
                    UpdateUserPermissions();
                    DataRow row;
                    for (i = 0; i <= _users.Count - 1; i++)
                    {
                        var user = (UserInfo) _users[i];
                        row = dtUserPermissions.NewRow();
                        row["UserId"] = user.UserID;
                        row["DisplayName"] = user.DisplayName;
                        int j;
                        for (j = 0; j <= _permissions.Count - 1; j++)
                        {
                            PermissionInfo objPerm;
                            objPerm = (PermissionInfo) _permissions[j];
                            row[objPerm.PermissionName + "_Enabled"] = GetEnabled(objPerm, user, j + 1);
                            if (SupportsDenyPermissions())
                            {
                                row[objPerm.PermissionName] = GetPermission(objPerm, user, j + 1, PermissionTypeNull);
                            }
                            else
                            {
                                if (GetPermission(objPerm, user, j + 1))
                                {
                                    row[objPerm.PermissionName] = PermissionTypeGrant;
                                }
                                else
                                {
                                    row[objPerm.PermissionName] = PermissionTypeNull;
                                }
                            }
                        }
                        dtUserPermissions.Rows.Add(row);
                    }
                    dgUserPermissions.DataSource = dtUserPermissions;
                    dgUserPermissions.DataBind();
                }
                else
                {
                    dgUserPermissions.Visible = false;
                }
            }
        }

        private void GetRoles()
        {
            var objRoleController = new RoleController();
            int RoleGroupId = -2;
            if ((cboRoleGroups != null) && (cboRoleGroups.SelectedValue != null))
            {
                RoleGroupId = int.Parse(cboRoleGroups.SelectedValue);
            }
            if (RoleGroupId > -2)
            {
                _roles = objRoleController.GetRolesByGroup(PortalController.GetCurrentPortalSettings().PortalId, RoleGroupId);
            }
            else
            {
                _roles = objRoleController.GetPortalRoles(PortalController.GetCurrentPortalSettings().PortalId);
            }
            if (RoleGroupId < 0)
            {
                var r = new RoleInfo();
                r.RoleID = int.Parse(Globals.glbRoleUnauthUser);
                r.RoleName = Globals.glbRoleUnauthUserName;
                _roles.Add(r);
                r = new RoleInfo();
                r.RoleID = int.Parse(Globals.glbRoleAllUsers);
                r.RoleName = Globals.glbRoleAllUsersName;
                _roles.Add(r);
            }
            _roles.Reverse();
            _roles.Sort(new RoleComparer());
        }

        private void SetUpRolesGrid()
        {
            dgRolePermissions.Columns.Clear();
            var textCol = new BoundColumn();
            textCol.HeaderText = "&nbsp;";
            textCol.DataField = "RoleName";
            textCol.ItemStyle.Width = Unit.Parse("150px");
            textCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
            dgRolePermissions.Columns.Add(textCol);
            var idCol = new BoundColumn();
            idCol.HeaderText = "";
            idCol.DataField = "roleid";
            idCol.Visible = false;
            dgRolePermissions.Columns.Add(idCol);
            TemplateColumn templateCol;
            foreach (PermissionInfo objPermission in _permissions)
            {
                templateCol = new TemplateColumn();
                var columnTemplate = new PermissionTriStateTemplate();
                columnTemplate.DataField = objPermission.PermissionName;
                columnTemplate.EnabledField = objPermission.PermissionName + "_Enabled";
                columnTemplate.SupportDenyMode = SupportsDenyPermissions();
                templateCol.ItemTemplate = columnTemplate;

                string locName = "";
                if (objPermission.ModuleDefID > 0)
                {
                    if (!String.IsNullOrEmpty(ResourceFile))
                    {
                        locName = Localization.GetString(objPermission.PermissionName + ".Permission", ResourceFile);
                    }
                }
                else
                {
                    locName = Localization.GetString(objPermission.PermissionName + ".Permission", PermissionProvider.Instance().LocalResourceFile);
                }
                templateCol.HeaderText = !String.IsNullOrEmpty(locName) ? locName : objPermission.PermissionName;
                templateCol.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                templateCol.HeaderStyle.VerticalAlign = VerticalAlign.Bottom;
                templateCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                templateCol.ItemStyle.Width = Unit.Parse("70px");
                templateCol.HeaderStyle.Wrap = true;
                dgRolePermissions.Columns.Add(templateCol);
            }
        }

        private void SetUpUsersGrid()
        {
            if (dgUserPermissions != null)
            {
                dgUserPermissions.Columns.Clear();
                var textCol = new BoundColumn();
                textCol.HeaderText = "&nbsp;";
                textCol.DataField = "DisplayName";
                textCol.ItemStyle.Width = Unit.Parse("150px");
                textCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                dgUserPermissions.Columns.Add(textCol);
                var idCol = new BoundColumn();
                idCol.HeaderText = "";
                idCol.DataField = "userid";
                idCol.Visible = false;
                dgUserPermissions.Columns.Add(idCol);
                TemplateColumn templateCol;
                foreach (PermissionInfo objPermission in _permissions)
                {
                    templateCol = new TemplateColumn();
                    var columnTemplate = new PermissionTriStateTemplate();
                    columnTemplate.DataField = objPermission.PermissionName;
                    columnTemplate.EnabledField = objPermission.PermissionName + "_Enabled";
                    columnTemplate.SupportDenyMode = SupportsDenyPermissions();
                    templateCol.ItemTemplate = columnTemplate;


                    string locName = "";
                    if (objPermission.ModuleDefID > 0)
                    {
                        if (!String.IsNullOrEmpty(ResourceFile))
                        {
                            locName = Localization.GetString(objPermission.PermissionName + ".Permission", ResourceFile);
                        }
                    }
                    else
                    {
                        locName = Localization.GetString(objPermission.PermissionName + ".Permission", PermissionProvider.Instance().LocalResourceFile);
                    }
                    templateCol.HeaderText = !String.IsNullOrEmpty(locName) ? locName : objPermission.PermissionName;
                    templateCol.HeaderStyle.VerticalAlign = VerticalAlign.Bottom;
                    templateCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                    templateCol.ItemStyle.Width = Unit.Parse("70px");
                    templateCol.HeaderStyle.Wrap = true;
                    dgUserPermissions.Columns.Add(templateCol);
                }
            }
        }

        protected virtual void AddPermission(PermissionInfo permission, int roleId, string roleName, int userId, string displayName, bool allowAccess)
        {
        }

        protected virtual void AddPermission(ArrayList permissions, UserInfo user)
        {
        }

        protected string BuildKey(bool allowAccess, int permissionId, int objectPermissionId, int roleId, string roleName)
        {
            return BuildKey(allowAccess, permissionId, objectPermissionId, roleId, roleName, Null.NullInteger, Null.NullString);
        }

        protected string BuildKey(bool allowAccess, int permissionId, int objectPermissionId, int roleId, string roleName, int userID, string displayName)
        {
            string key;
            if (allowAccess)
            {
                key = "True";
            }
            else
            {
                key = "False";
            }
            key += "|" + Convert.ToString(permissionId);
            key += "|";
            if (objectPermissionId > -1)
            {
                key += Convert.ToString(objectPermissionId);
            }
            key += "|" + roleName;
            key += "|" + roleId;
            key += "|" + userID;
            key += "|" + displayName;
            return key;
        }

        protected override void CreateChildControls()
        {
            _permissions = GetPermissions();
            pnlPermissions = new Panel();
            pnlPermissions.CssClass = "DataGrid_Container";
            PortalSettings _portalSettings = PortalController.GetCurrentPortalSettings();
            ArrayList arrGroups = RoleController.GetRoleGroups(_portalSettings.PortalId);
            if (arrGroups.Count > 0)
            {
                lblGroups = new Label();
                lblGroups.Text = Localization.GetString("RoleGroupFilter");
                lblGroups.CssClass = "SubHead";
                pnlPermissions.Controls.Add(lblGroups);
                pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                cboRoleGroups = new DropDownList();
                cboRoleGroups.AutoPostBack = true;
                cboRoleGroups.Items.Add(new ListItem(Localization.GetString("AllRoles"), "-2"));
                var liItem = new ListItem(Localization.GetString("GlobalRoles"), "-1");
                liItem.Selected = true;
                cboRoleGroups.Items.Add(liItem);
                foreach (RoleGroupInfo roleGroup in arrGroups)
                {
                    cboRoleGroups.Items.Add(new ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString()));
                }
                pnlPermissions.Controls.Add(cboRoleGroups);
                pnlPermissions.Controls.Add(new LiteralControl("<br/><br/>"));
            }
            dgRolePermissions = new DataGrid();
            dgRolePermissions.AutoGenerateColumns = false;
            dgRolePermissions.CellSpacing = 0;
            dgRolePermissions.CellPadding = 2;
            dgRolePermissions.GridLines = GridLines.None;
            dgRolePermissions.FooterStyle.CssClass = "DataGrid_Footer";
            dgRolePermissions.HeaderStyle.CssClass = "DataGrid_Header";
            dgRolePermissions.ItemStyle.CssClass = "DataGrid_Item";
            dgRolePermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem";
            SetUpRolesGrid();
            pnlPermissions.Controls.Add(dgRolePermissions);
            _users = GetUsers();
            if (_users != null)
            {
                dgUserPermissions = new DataGrid();
                dgUserPermissions.AutoGenerateColumns = false;
                dgUserPermissions.CellSpacing = 0;
                dgUserPermissions.GridLines = GridLines.None;
                dgUserPermissions.FooterStyle.CssClass = "DataGrid_Footer";
                dgUserPermissions.HeaderStyle.CssClass = "DataGrid_Header";
                dgUserPermissions.ItemStyle.CssClass = "DataGrid_Item";
                dgUserPermissions.AlternatingItemStyle.CssClass = "DataGrid_AlternatingItem";
                SetUpUsersGrid();
                pnlPermissions.Controls.Add(dgUserPermissions);
                pnlPermissions.Controls.Add(new LiteralControl("<br/>"));
                lblUser = new Label();
                lblUser.Text = Localization.GetString("User");
                lblUser.CssClass = "SubHead";
                pnlPermissions.Controls.Add(lblUser);
                pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                txtUser = new TextBox();
                txtUser.CssClass = "NormalTextBox";
                pnlPermissions.Controls.Add(txtUser);
                pnlPermissions.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                cmdUser = new CommandButton();
                cmdUser.Text = Localization.GetString("Add");
                cmdUser.CssClass = "CommandButton";
                cmdUser.ImageUrl = "~/images/add.gif";
                pnlPermissions.Controls.Add(cmdUser);
                cmdUser.Click += AddUser;
            }
            Controls.Add(pnlPermissions);
        }

        protected virtual bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
        {
            return true;
        }

        protected virtual bool GetEnabled(PermissionInfo objPerm, UserInfo user, int column)
        {
            return true;
        }

        protected virtual bool GetPermission(PermissionInfo objPerm, RoleInfo role, int column)
        {
            return Convert.ToBoolean(GetPermission(objPerm, role, column, PermissionTypeDeny));
        }

        protected virtual string GetPermission(PermissionInfo objPerm, RoleInfo role, int column, string defaultState)
        {
            string stateKey = defaultState;
            if (PermissionsList != null)
            {
                foreach (PermissionInfoBase permission in PermissionsList)
                {
                    if (permission.PermissionID == objPerm.PermissionID && permission.RoleID == role.RoleID)
                    {
                        if (permission.AllowAccess)
                        {
                            stateKey = PermissionTypeGrant;
                        }
                        else
                        {
                            stateKey = PermissionTypeDeny;
                        }
                        break;
                    }
                }
            }
            return stateKey;
        }

        protected virtual bool GetPermission(PermissionInfo objPerm, UserInfo user, int column)
        {
            return Convert.ToBoolean(GetPermission(objPerm, user, column, PermissionTypeDeny));
        }

        protected virtual string GetPermission(PermissionInfo objPerm, UserInfo user, int column, string defaultState)
        {
            string stateKey = defaultState;
            if (PermissionsList != null)
            {
                foreach (PermissionInfoBase permission in PermissionsList)
                {
                    if (permission.PermissionID == objPerm.PermissionID && permission.UserID == user.UserID)
                    {
                        if (permission.AllowAccess)
                        {
                            stateKey = PermissionTypeGrant;
                        }
                        else
                        {
                            stateKey = PermissionTypeDeny;
                        }
                        break;
                    }
                }
            }
            return stateKey;
        }

        protected virtual ArrayList GetPermissions()
        {
            return null;
        }

        protected virtual ArrayList GetUsers()
        {
            var arrUsers = new ArrayList();
            UserInfo objUser;
            bool blnExists;
            if (PermissionsList != null)
            {
                foreach (PermissionInfoBase permission in PermissionsList)
                {
                    if (!Null.IsNull(permission.UserID))
                    {
                        blnExists = false;
                        foreach (UserInfo user in arrUsers)
                        {
                            if (permission.UserID == user.UserID)
                            {
                                blnExists = true;
                            }
                        }
                        if (!blnExists)
                        {
                            objUser = new UserInfo();
                            objUser.UserID = permission.UserID;
                            objUser.Username = permission.Username;
                            objUser.DisplayName = permission.DisplayName;
                            arrUsers.Add(objUser);
                        }
                    }
                }
            }
            return arrUsers;
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindData();
        }

        protected virtual void ParsePermissionKeys(PermissionInfoBase permission, string[] Settings)
        {
            permission.PermissionID = Convert.ToInt32(Settings[1]);
            permission.RoleID = Convert.ToInt32(Settings[4]);
            permission.RoleName = Settings[3];
            permission.AllowAccess = Convert.ToBoolean(Settings[0]);
            permission.UserID = Convert.ToInt32(Settings[5]);
            permission.DisplayName = Settings[6];
        }

        protected virtual void RemovePermission(int permissionID, int roleID, int userID)
        {
        }

        protected virtual void UpdatePermission(PermissionInfo permission, int roleId, string roleName, bool allowAccess)
        {
            if (allowAccess)
            {
                UpdatePermission(permission, roleId, roleName, PermissionTypeGrant);
            }
            else
            {
                UpdatePermission(permission, roleId, roleName, PermissionTypeNull);
            }
        }

        protected virtual void UpdatePermission(PermissionInfo permission, int roleId, string roleName, string stateKey)
        {
            RemovePermission(permission.PermissionID, roleId, Null.NullInteger);
            switch (stateKey)
            {
                case PermissionTypeGrant:
                    AddPermission(permission, roleId, roleName, Null.NullInteger, Null.NullString, true);
                    break;
                case PermissionTypeDeny:
                    AddPermission(permission, roleId, roleName, Null.NullInteger, Null.NullString, false);
                    break;
            }
        }

        protected virtual void UpdatePermission(PermissionInfo permission, string displayName, int userId, bool allowAccess)
        {
            if (allowAccess)
            {
                UpdatePermission(permission, displayName, userId, PermissionTypeGrant);
            }
            else
            {
                UpdatePermission(permission, displayName, userId, PermissionTypeNull);
            }
        }

        protected virtual void UpdatePermission(PermissionInfo permission, string displayName, int userId, string stateKey)
        {
            RemovePermission(permission.PermissionID, int.Parse(Globals.glbRoleNothing), userId);
            switch (stateKey)
            {
                case PermissionTypeGrant:
                    AddPermission(permission, int.Parse(Globals.glbRoleNothing), Null.NullString, userId, displayName, true);
                    break;
                case PermissionTypeDeny:
                    AddPermission(permission, int.Parse(Globals.glbRoleNothing), Null.NullString, userId, displayName, false);
                    break;
            }
        }

        protected virtual bool SupportsDenyPermissions()
        {
            return false;
        }

        protected void UpdatePermissions()
        {
            EnsureChildControls();
            UpdateRolePermissions();
            UpdateUserPermissions();
        }

        protected void UpdateRolePermissions()
        {
            if (dgRolePermissions != null && !RefreshGrid)
            {
                foreach (DataGridItem dgi in dgRolePermissions.Items)
                {
                    int i;
                    for (i = 2; i <= dgi.Cells.Count - 1; i++)
                    {
                        if (dgi.Cells[i].Controls.Count > 0)
                        {
                            var triState = (PermissionTriState) dgi.Cells[i].Controls[0];
                            if (SupportsDenyPermissions())
                            {
                                UpdatePermission((PermissionInfo) _permissions[i - 2], int.Parse(dgi.Cells[1].Text), dgi.Cells[0].Text, triState.Value);
                            }
                            else
                            {
                                UpdatePermission((PermissionInfo) _permissions[i - 2], int.Parse(dgi.Cells[1].Text), dgi.Cells[0].Text, triState.Value == PermissionTypeGrant);
                            }
                        }
                    }
                }
            }
        }

        protected void UpdateUserPermissions()
        {
            if (dgUserPermissions != null && !RefreshGrid)
            {
                foreach (DataGridItem dgi in dgUserPermissions.Items)
                {
                    int i;
                    for (i = 2; i <= dgi.Cells.Count - 1; i++)
                    {
                        if (dgi.Cells[i].Controls.Count > 0)
                        {
                            var triState = (PermissionTriState) dgi.Cells[i].Controls[0];
                            if (SupportsDenyPermissions())
                            {
                                UpdatePermission((PermissionInfo) _permissions[i - 2], dgi.Cells[0].Text, int.Parse(dgi.Cells[1].Text), triState.Value);
                            }
                            else
                            {
                                UpdatePermission((PermissionInfo) _permissions[i - 2], dgi.Cells[0].Text, int.Parse(dgi.Cells[1].Text), triState.Value == PermissionTypeGrant);
                            }
                        }
                    }
                }
            }
        }

        protected void RoleGroupsSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePermissions();
        }

        protected void AddUser(object sender, EventArgs e)
        {
            UpdatePermissions();
            if (!String.IsNullOrEmpty(txtUser.Text))
            {
                UserInfo objUser = UserController.GetCachedUser(PortalId, txtUser.Text);
                if (objUser != null)
                {
                    AddPermission(_permissions, objUser);
                    BindData();
                }
                else
                {
                    lblUser = new Label();
                    lblUser.Text = "<br>" + Localization.GetString("InvalidUserName");
                    lblUser.CssClass = "NormalRed";
                    pnlPermissions.Controls.Add(lblUser);
                }
            }
        }
    }
}
