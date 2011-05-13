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
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Security
{

    public partial class Roles : PortalModuleBase, IActionable
    {

        #region Private Members

        private int _roleGroupId = -1;

        #endregion

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "add.gif",
                            EditUrl(),
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                Actions.Add(GetNextActionID(),
                            Localization.GetString("AddGroup.Action", LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "add.gif",
                            EditUrl("EditGroup"),
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                Actions.Add(GetNextActionID(),
                            Localization.GetString("UserSettings.Action", LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "settings.gif",
                            EditUrl("UserSettings"),
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                return Actions;
            }
        }

        #endregion

        #region Private Methods

        private void BindData()
        {
            var objRoles = new RoleController();
            var arrRoles = _roleGroupId < -1 ? objRoles.GetPortalRoles(PortalId) : objRoles.GetRolesByGroup(PortalId, _roleGroupId);
            grdRoles.DataSource = arrRoles;
            if (_roleGroupId < 0)
            {
                lnkEditGroup.Visible = false;
                cmdDelete.Visible = false;
            }
            else
            {
                lnkEditGroup.Visible = true;
                lnkEditGroup.NavigateUrl = EditUrl("RoleGroupId", _roleGroupId.ToString(), "EditGroup");
                cmdDelete.Visible = arrRoles.Count == 0;
            }
            Localization.LocalizeDataGrid(ref grdRoles, LocalResourceFile);
            grdRoles.DataBind();
        }

        private void BindGroups()
        {
            ListItem liItem;
            ArrayList arrGroups = RoleController.GetRoleGroups(PortalId);
            if (arrGroups.Count > 0)
            {
                cboRoleGroups.Items.Clear();
                cboRoleGroups.Items.Add(new ListItem(Localization.GetString("AllRoles"), "-2"));
                liItem = new ListItem(Localization.GetString("GlobalRoles"), "-1");
                if (_roleGroupId < 0)
                {
                    liItem.Selected = true;
                }
                cboRoleGroups.Items.Add(liItem);
                foreach (RoleGroupInfo roleGroup in arrGroups)
                {
                    liItem = new ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString());
                    if (_roleGroupId == roleGroup.RoleGroupID)
                    {
                        liItem.Selected = true;
                    }
                    cboRoleGroups.Items.Add(liItem);
                }
                divGroups.Visible = true;
            }
            else
            {
                _roleGroupId = -2;
                divGroups.Visible = false;
            }
            BindData();
        }

        #endregion

        public string FormatPeriod(int period)
        {
            var formatPeriod = Null.NullString;
            try
            {
                if (period != Null.NullInteger)
                {
                    formatPeriod = period.ToString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatPeriod;
        }

        public string FormatPrice(float price)
        {
            var formatPrice = Null.NullString;
            try
            {
                if (price != Null.NullSingle)
                {
                    formatPrice = price.ToString("##0.00");
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatPrice;
        }

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            jQuery.RequestDnnPluginsRegistration();

            foreach (DataGridColumn column in grdRoles.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (ImageCommandColumn)))
                {
                    var imageColumn = (ImageCommandColumn) column;
                    imageColumn.Visible = IsEditable;
                    if (imageColumn.CommandName == "Delete")
                    {
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem");
                    }
                    if (imageColumn.CommandName == "Edit")
                    {
                        string formatString = EditUrl("RoleID", "KEYFIELD", "Edit");
                        formatString = formatString.Replace("KEYFIELD", "{0}");
                        imageColumn.NavigateURLFormatString = formatString;
                    }
                    if (imageColumn.CommandName == "UserRoles")
                    {
                        string formatString = Globals.NavigateURL(TabId, "User Roles", "RoleId=KEYFIELD", "mid=" + ModuleId);
                        formatString = formatString.Replace("KEYFIELD", "{0}");
                        imageColumn.NavigateURLFormatString = formatString;
                    }
                    if (!String.IsNullOrEmpty(imageColumn.CommandName))
                    {
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, LocalResourceFile);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cboRoleGroups.SelectedIndexChanged += OnRoleGroupIndexChanged;
            cmdDelete.Click += OnDeleteClick;
            grdRoles.ItemDataBound += OnRolesGridItemDataBound;

            try
            {
                if (!Page.IsPostBack)
                {
                    if ((Request.QueryString["RoleGroupID"] != null))
                    {
                        _roleGroupId = Int32.Parse(Request.QueryString["RoleGroupID"]);
                    }
                    BindGroups();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnRoleGroupIndexChanged(object sender, EventArgs e)
        {
            _roleGroupId = Int32.Parse(cboRoleGroups.SelectedValue);
            BindData();
        }

        protected void OnDeleteClick(object sender, ImageClickEventArgs e)
        {
            _roleGroupId = Int32.Parse(cboRoleGroups.SelectedValue);
            if (_roleGroupId > -1)
            {
                RoleController.DeleteRoleGroup(PortalId, _roleGroupId);
                _roleGroupId = -1;
            }
            BindGroups();
        }

        protected void OnRolesGridItemDataBound(object sender, DataGridItemEventArgs e)
        {
            var item = e.Item;
            switch (item.ItemType)
            {
                case ListItemType.SelectedItem:
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    {
                        var imgColumnControl = item.Controls[0].Controls[0];
                        if (imgColumnControl is HyperLink)
                        {
                            var editLink = (HyperLink) imgColumnControl;
                            var role = (RoleInfo) item.DataItem;
                            editLink.Visible = role.RoleName != PortalSettings.AdministratorRoleName || (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName));
                        }
                        imgColumnControl = item.Controls[1].Controls[0];
                        if (imgColumnControl is HyperLink)
                        {
                            var rolesLink = (HyperLink) imgColumnControl;
                            var role = (RoleInfo) item.DataItem;
                            rolesLink.Visible = role.RoleName != PortalSettings.AdministratorRoleName || (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName));
                        }
                    }
                    break;
            }
        }

        #endregion

    }
}