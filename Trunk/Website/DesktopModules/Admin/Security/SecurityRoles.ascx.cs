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
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;

using Calendar = DotNetNuke.Common.Utilities.Calendar;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Security
{
    public partial class SecurityRoles : PortalModuleBase, IActionable
    {
        private int RoleId = Null.NullInteger;
        private new int UserId = Null.NullInteger;
        private RoleInfo _Role;
        private int _SelectedUserID = Null.NullInteger;
        private UserInfo _User;

        protected string ReturnUrl
        {
            get
            {
                string _ReturnURL;
                var FilterParams = new string[String.IsNullOrEmpty(Request.QueryString["filterproperty"]) ? 2 : 3];
                if (String.IsNullOrEmpty(Request.QueryString["filterProperty"]))
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                    FilterParams.SetValue("currentpage=" + Request.QueryString["currentpage"], 1);
                }
                else
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                    FilterParams.SetValue("filterProperty=" + Request.QueryString["filterProperty"], 1);
                    FilterParams.SetValue("currentpage=" + Request.QueryString["currentpage"], 2);
                }
                if (string.IsNullOrEmpty(Request.QueryString["filter"]))
                {
                    _ReturnURL = Globals.NavigateURL(TabId);
                }
                else
                {
                    _ReturnURL = Globals.NavigateURL(TabId, "", FilterParams);
                }
                return _ReturnURL;
            }
        }

        protected RoleInfo Role
        {
            get
            {
                if (_Role == null)
                {
                    var objRoleController = new RoleController();
                    if (RoleId != Null.NullInteger)
                    {
                        _Role = objRoleController.GetRole(RoleId, PortalId);
                    }
                    else if (cboRoles.SelectedItem != null)
                    {
                        _Role = objRoleController.GetRole(Convert.ToInt32(cboRoles.SelectedItem.Value), PortalId);
                    }
                }
                return _Role;
            }
        }

        protected UserInfo User
        {
            get
            {
                if (_User == null)
                {
                    if (UserId != Null.NullInteger)
                    {
                        _User = UserController.GetUserById(PortalId, UserId);
                    }
                    else if (UsersControl == UsersControl.TextBox && !String.IsNullOrEmpty(txtUsers.Text))
                    {
                        _User = UserController.GetUserByName(PortalId, txtUsers.Text);
                    }
                    else if (UsersControl == UsersControl.Combo && (cboUsers.SelectedItem != null))
                    {
                        _User = UserController.GetUserById(PortalId, Convert.ToInt32(cboUsers.SelectedItem.Value));
                    }
                }
                return _User;
            }
        }

        protected int SelectedUserID
        {
            get
            {
                return _SelectedUserID;
            }
            set
            {
                _SelectedUserID = value;
            }
        }

        protected UsersControl UsersControl
        {
            get
            {
                object setting = UserModuleBase.GetSetting(PortalId, "Security_UsersControl");
                return (UsersControl) setting;
            }
        }

        public PortalModuleBase ParentModule { get; set; }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString("Cancel.Action", LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "lt.gif",
                            ReturnUrl,
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                return Actions;
            }
        }

        #endregion

        private void BindData()
        {
            var objRoles = new RoleController();
            if (RoleId == Null.NullInteger)
            {
                if (cboRoles.Items.Count == 0)
                {
                    ArrayList arrRoles = objRoles.GetPortalRoles(PortalId);
                    int roleIndex = Null.NullInteger;
                    foreach (RoleInfo tmpRole in arrRoles)
                    {
                        if (tmpRole.RoleName == PortalSettings.AdministratorRoleName)
                        {
                            if (!PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                            {
                                roleIndex = arrRoles.IndexOf(tmpRole);
                            }
                        }
                        break;
                    }
                    if (roleIndex > Null.NullInteger)
                    {
                        arrRoles.RemoveAt(roleIndex);
                    }
                    cboRoles.DataSource = arrRoles;
                    cboRoles.DataBind();
                }
            }
            else
            {
                if (!Page.IsPostBack)
                {
                    if (Role != null)
                    {
                        cboRoles.Items.Add(new ListItem(Role.RoleName, Role.RoleID.ToString()));
                        cboRoles.Items[0].Selected = true;
                        lblTitle.Text = string.Format(Localization.GetString("RoleTitle.Text", LocalResourceFile), Role.RoleName, Role.RoleID);
                    }
                    cboRoles.Visible = false;
                    plRoles.Visible = false;
                }
            }
            if (UserId == -1)
            {
                if (Role.RoleName == PortalSettings.AdministratorRoleName && !PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("NotAuthorized", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                    pnlRoles.Visible = false;
                    pnlUserRoles.Visible = false;
                    chkNotify.Visible = false;
                    return;
                }
                if (UsersControl == UsersControl.Combo)
                {
                    if (cboUsers.Items.Count == 0)
                    {
                        foreach (UserInfo objUser in UserController.GetUsers(PortalId))
                        {
                            cboUsers.Items.Add(new ListItem(objUser.DisplayName + " (" + objUser.Username + ")", objUser.UserID.ToString()));
                        }
                    }
                    txtUsers.Visible = false;
                    cboUsers.Visible = true;
                    cmdValidate.Visible = false;
                }
                else
                {
                    txtUsers.Visible = true;
                    cboUsers.Visible = false;
                    cmdValidate.Visible = true;
                }
            }
            else
            {
                if (User != null)
                {
                    txtUsers.Text = User.UserID.ToString();
                    lblTitle.Text = string.Format(Localization.GetString("UserTitle.Text", LocalResourceFile), User.Username, User.UserID);
                }
                txtUsers.Visible = false;
                cboUsers.Visible = false;
                cmdValidate.Visible = false;
                plUsers.Visible = false;
            }
        }

        private void BindGrid()
        {
            var objRoleController = new RoleController();
            if (RoleId != Null.NullInteger)
            {
                cmdAdd.Text = Localization.GetString("AddUser.Text", LocalResourceFile);
                grdUserRoles.DataKeyField = "UserId";
                grdUserRoles.Columns[2].Visible = false;
                grdUserRoles.DataSource = objRoleController.GetUserRolesByRoleName(PortalId, Role.RoleName);
                grdUserRoles.DataBind();
            }
            if (UserId != -1)
            {
                cmdAdd.Text = Localization.GetString("AddRole.Text", LocalResourceFile);
                grdUserRoles.DataKeyField = "RoleId";
                grdUserRoles.Columns[1].Visible = false;
                grdUserRoles.DataSource = objRoleController.GetUserRolesByUsername(PortalId, User.Username, Null.NullString);
                grdUserRoles.DataBind();
            }
        }

        private void GetDates(int UserId, int RoleId)
        {
            string strExpiryDate = "";
            string strEffectiveDate = "";
            var objRoles = new RoleController();
            UserRoleInfo objUserRole = objRoles.GetUserRole(PortalId, UserId, RoleId);
            if (objUserRole != null)
            {
                if (Null.IsNull(objUserRole.EffectiveDate) == false)
                {
                    strEffectiveDate = objUserRole.EffectiveDate.ToShortDateString();
                }
                if (Null.IsNull(objUserRole.ExpiryDate) == false)
                {
                    strExpiryDate = objUserRole.ExpiryDate.ToShortDateString();
                }
            }
            else
            {
                RoleInfo objRole = objRoles.GetRole(RoleId, PortalId);
                if (objRole.BillingPeriod > 0)
                {
                    switch (objRole.BillingFrequency)
                    {
                        case "D":
                            strExpiryDate = DateTime.Now.AddDays(objRole.BillingPeriod).ToShortDateString();
                            break;
                        case "W":
                            strExpiryDate = DateTime.Now.AddDays(objRole.BillingPeriod*7).ToShortDateString();
                            break;
                        case "M":
                            strExpiryDate = DateTime.Now.AddMonths(objRole.BillingPeriod).ToShortDateString();
                            break;
                        case "Y":
                            strExpiryDate = DateTime.Now.AddYears(objRole.BillingPeriod).ToShortDateString();
                            break;
                    }
                }
            }
            txtEffectiveDate.Text = strEffectiveDate;
            txtExpiryDate.Text = strExpiryDate;
        }

        public override void DataBind()
        {
            if (!ModulePermissionController.CanEditModuleContent(ModuleConfiguration))
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
            base.DataBind();
            cmdEffectiveCalendar.NavigateUrl = Calendar.InvokePopupCal(txtEffectiveDate);
            cmdExpiryCalendar.NavigateUrl = Calendar.InvokePopupCal(txtExpiryDate);
            string localizedCalendarText = Localization.GetString("Calendar");
            string calendarText = "<img src='" + ResolveUrl("~/images/calendar.png") + "' border='0' alt='" + localizedCalendarText + "'>";
            cmdExpiryCalendar.Text = calendarText;
            cmdEffectiveCalendar.Text = calendarText;
            Localization.LocalizeDataGrid(ref grdUserRoles, LocalResourceFile);
            BindData();
            BindGrid();
        }

        public bool DeleteButtonVisible(int UserID, int RoleID)
        {
            bool canDelete = RoleController.CanRemoveUserFromRole(PortalSettings, UserID, RoleID);
            if (RoleID == PortalSettings.AdministratorRoleId && canDelete)
            {
                canDelete = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
            }
            return canDelete;
        }

        public string FormatDate(DateTime DateTime)
        {
            if (!Null.IsNull(DateTime))
            {
                return DateTime.ToShortDateString();
            }
            else
            {
                return "";
            }
        }

        public string FormatUser(int UserID, string DisplayName)
        {
            return "<a href=\"" + Globals.LinkClick("userid=" + UserID, TabId, ModuleId) + "\" class=\"CommandButton\">" + DisplayName + "</a>";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if ((Request.QueryString["RoleId"] != null))
            {
                RoleId = Int32.Parse(Request.QueryString["RoleId"]);
            }
            if ((Request.QueryString["UserId"] != null))
            {
                UserId = Int32.Parse(Request.QueryString["UserId"]);
            }

            cboRoles.SelectedIndexChanged += cboRoles_SelectedIndexChanged;
            cboUsers.SelectedIndexChanged += cboUsers_SelectedIndexChanged;
            cmdAdd.Click += cmdAdd_Click;
            cmdValidate.Click += cmdValidate_Click;
            grdUserRoles.ItemCreated += grdUserRoles_ItemCreated;
            grdUserRoles.ItemDataBound += grdUserRoles_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (ParentModule == null)
                {
                    DataBind();
                }
            }
            catch (ThreadAbortException exc)
            {
                DnnLog.Debug(exc);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cboUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((cboUsers.SelectedItem != null) && (cboRoles.SelectedItem != null))
            {
                SelectedUserID = Int32.Parse(cboUsers.SelectedItem.Value);
                GetDates(SelectedUserID, Int32.Parse(cboRoles.SelectedItem.Value));
            }
            BindGrid();
        }

        private void cmdValidate_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtUsers.Text))
            {
                UserInfo objUser = UserController.GetUserByName(PortalId, txtUsers.Text);
                if (objUser != null)
                {
                    GetDates(objUser.UserID, RoleId);
                    SelectedUserID = objUser.UserID;
                }
                else
                {
                    txtUsers.Text = "";
                }
            }
            BindGrid();
        }

        private void cboRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDates(UserId, Int32.Parse(cboRoles.SelectedItem.Value));
            BindGrid();
        }

        private void cmdAdd_Click(Object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    if ((Role != null) && (User != null))
                    {
                        if (User.UserID == PortalSettings.AdministratorId && Role.RoleID == PortalSettings.AdministratorRoleId)
                        {
                            txtEffectiveDate.Text = "";
                            txtExpiryDate.Text = "";
                        }
                        DateTime datEffectiveDate;
                        if (!String.IsNullOrEmpty(txtEffectiveDate.Text))
                        {
                            datEffectiveDate = DateTime.Parse(txtEffectiveDate.Text);
                        }
                        else
                        {
                            datEffectiveDate = Null.NullDate;
                        }
                        DateTime datExpiryDate;
                        if (!String.IsNullOrEmpty(txtExpiryDate.Text))
                        {
                            datExpiryDate = DateTime.Parse(txtExpiryDate.Text);
                        }
                        else
                        {
                            datExpiryDate = Null.NullDate;
                        }
                        RoleController.AddUserRole(User, Role, PortalSettings, datEffectiveDate, datExpiryDate, UserId, chkNotify.Checked);
                    }
                }
                BindGrid();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public void cmdDeleteUserRole_click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var cmdDeleteUserRole = (ImageButton) sender;
                int roleId = Convert.ToInt32(cmdDeleteUserRole.Attributes["roleId"]);
                int userId = Convert.ToInt32(cmdDeleteUserRole.Attributes["userId"]);

                if (!RoleController.DeleteUserRole(roleId, UserController.GetUserById(PortalId, userId), PortalSettings, chkNotify.Checked))
                {
                    UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("RoleRemoveError", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                }
                BindGrid();
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("RoleRemoveError", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
            }
        }

        private void grdUserRoles_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                DataGridItem item = e.Item;
                var cmdDeleteUserRole = e.Item.FindControl("cmdDeleteUserRole") as ImageButton;
                var role = e.Item.DataItem as UserRoleInfo;

                if (cmdDeleteUserRole != null)
                {
                    ClientAPI.AddButtonConfirm(cmdDeleteUserRole, String.Format(Localization.GetString("DeleteUsersFromRole.Text", LocalResourceFile), role.FullName, role.RoleName));
                    cmdDeleteUserRole.Attributes.Add("roleId", role.RoleID.ToString());
                    cmdDeleteUserRole.Attributes.Add("userId", role.UserID.ToString());
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void grdUserRoles_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DataGridItem item = e.Item;
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
            {
                var userRole = (UserRoleInfo) item.DataItem;
                if (RoleId == Null.NullInteger)
                {
                    if (userRole.RoleID == Convert.ToInt32(cboRoles.SelectedValue))
                    {
                        cmdAdd.Text = Localization.GetString("UpdateRole.Text", LocalResourceFile);
                    }
                }
                if (UserId == Null.NullInteger)
                {
                    if (userRole.UserID == SelectedUserID)
                    {
                        cmdAdd.Text = Localization.GetString("UpdateRole.Text", LocalResourceFile);
                    }
                }
            }
        }
    }
}