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
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Modules.Admin.Security
{
    public partial class MemberServices : UserModuleBase
    {
        #region Delegates

        public delegate void SubscriptionUpdatedEventHandler(object sender, SubscriptionUpdatedEventArgs e);

        #endregion

       public event SubscriptionUpdatedEventHandler SubscriptionUpdated;

        private string FormatPrice(float price)
        {
            string formatPrice = Null.NullString;
            try
            {
                if (price != Null.NullSingle)
                {
                    formatPrice = price.ToString("##0.00");
                }
                else
                {
                    formatPrice = "";
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatPrice;
        }

        private ArrayList GetRoles(int portalId, int userId)
        {
            var objRoles = new RoleController();
            return objRoles.GetUserRoles(portalId, userId, false);
        }

        private void Subscribe(int roleID, bool cancel)
        {
            var objRoles = new RoleController();
            RoleInfo objRole = objRoles.GetRole(roleID, PortalSettings.PortalId);
            if (objRole.IsPublic && objRole.ServiceFee == 0.0)
            {
                objRoles.UpdateUserRole(PortalId, UserInfo.UserID, roleID, cancel);
                OnSubscriptionUpdated(new SubscriptionUpdatedEventArgs(cancel, objRole.RoleName));
            }
            else
            {
                if (!cancel)
                {
                    Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&RoleID=" + roleID, true);
                }
                else
                {
                    Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&RoleID=" + roleID + "&cancel=1", true);
                }
            }
        }

        private void UseTrial(int roleID)
        {
            var objRoles = new RoleController();
            RoleInfo objRole = objRoles.GetRole(roleID, PortalSettings.PortalId);
            if (objRole.IsPublic && objRole.TrialFee == 0.0)
            {
                objRoles.UpdateUserRole(PortalId, UserInfo.UserID, roleID, false);
                OnSubscriptionUpdated(new SubscriptionUpdatedEventArgs(false, objRole.RoleName));
            }
            else
            {
                Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" + TabId + "&RoleID=" + roleID, true);
            }
        }

        protected string FormatExpiryDate(DateTime expiryDate)
        {
            string formatExpiryDate = Null.NullString;
            try
            {
                if (!Null.IsNull(expiryDate))
                {
                    if (expiryDate > DateTime.Today)
                    {
                        formatExpiryDate = expiryDate.ToShortDateString();
                    }
                    else
                    {
                        formatExpiryDate = Localization.GetString("Expired", LocalResourceFile);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatExpiryDate;
        }

        protected string FormatPrice(float price, int period, string frequency)
        {
            string formatPrice = Null.NullString;
            try
            {
                switch (frequency)
                {
                    case "N":
                    case "":
                        formatPrice = Localization.GetString("NoFee", LocalResourceFile);
                        break;
                    case "O":
                        formatPrice = FormatPrice(price);
                        break;
                    default:
                        formatPrice = string.Format(Localization.GetString("Fee", LocalResourceFile), FormatPrice(price), period, Localization.GetString("Frequency_" + frequency, LocalResourceFile));
                        break;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatPrice;
        }

        protected string FormatTrial(float price, int period, string frequency)
        {
            string formatTrial = Null.NullString;
            try
            {
                switch (frequency)
                {
                    case "N":
                    case "":
                        formatTrial = Localization.GetString("NoFee", LocalResourceFile);
                        break;
                    case "O":
                        formatTrial = FormatPrice(price);
                        break;
                    default:
                        formatTrial = string.Format(Localization.GetString("TrialFee", LocalResourceFile),
                                                     FormatPrice(price),
                                                     period,
                                                     Localization.GetString("Frequency_" + frequency, LocalResourceFile));
                        break;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatTrial;
        }

        protected string FormatURL()
        {
            string formatURL = Null.NullString;
            try
            {
                string serverPath = Request.ApplicationPath;
                if (!serverPath.EndsWith("/"))
                {
                    serverPath += "/";
                }
                formatURL = serverPath + "Register.aspx?tabid=" + TabId;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return formatURL;
        }

        protected string ServiceText(bool subscribed, DateTime expiryDate)
        {
            string serviceText = Null.NullString;
            try
            {
                if (!subscribed)
                {
                    serviceText = Localization.GetString("Subscribe", LocalResourceFile);
                }
                else
                {
                    serviceText = Localization.GetString("Unsubscribe", LocalResourceFile);
                    if (!Null.IsNull(expiryDate))
                    {
                        if (expiryDate < DateTime.Today)
                        {
                            serviceText = Localization.GetString("Renew", LocalResourceFile);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return serviceText;
        }

        protected bool ShowSubscribe(int roleID)
        {
            var objRoles = new RoleController();
            bool showSubscribe = Null.NullBoolean;
            RoleInfo objRole = objRoles.GetRole(roleID, PortalSettings.PortalId);
            if (objRole.IsPublic)
            {
                var objPortals = new PortalController();
                PortalInfo objPortal = objPortals.GetPortal(PortalSettings.PortalId);
                if (objRole.ServiceFee == 0.0)
                {
                    showSubscribe = true;
                }
                else if (objPortal != null && !string.IsNullOrEmpty(objPortal.ProcessorUserId))
                {
                    showSubscribe = true;
                }
            }
            return showSubscribe;
        }

        protected bool ShowTrial(int roleID)
        {
            var objRoles = new RoleController();
            bool showTrial = Null.NullBoolean;
            RoleInfo objRole = objRoles.GetRole(roleID, PortalSettings.PortalId);
            if (objRole.TrialFrequency == "N" || (objRole.IsPublic && objRole.ServiceFee == 0.0))
            {
                showTrial = Null.NullBoolean;
            }
            else if (objRole.IsPublic && objRole.TrialFee == 0.0)
            {
                UserRoleInfo objUserRole = objRoles.GetUserRole(PortalId, UserInfo.UserID, roleID);
                if ((objUserRole == null) || (!objUserRole.IsTrialUsed))
                {
                    showTrial = true;
                }
            }
            return showTrial;
        }

        public override void DataBind()
        {
            if (Request.IsAuthenticated)
            {
                grdServices.DataSource = GetRoles(PortalId, UserInfo.UserID);
                grdServices.DataBind();
                ServicesRow.Visible = (grdServices.Items.Count > 0);
            }
        }

        public void OnSubscriptionUpdated(SubscriptionUpdatedEventArgs e)
        {
            if (SubscriptionUpdated != null)
            {
                SubscriptionUpdated(this, e);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdRSVP.Click += cmdRSVP_Click;
            grdServices.ItemCommand += grdServices_ItemCommand;

            try
            {
                lblRSVP.Text = "";
                if (Page.IsPostBack == false)
                {
                    Localization.LocalizeDataGrid(ref grdServices, LocalResourceFile);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdRSVP_Click(object sender, EventArgs e)
        {
            string code = txtRSVPCode.Text;
            bool rsvpCodeExists = false;
            if (!String.IsNullOrEmpty(code))
            {
                var objRoles = new RoleController();
                ArrayList arrRoles = objRoles.GetPortalRoles(PortalSettings.PortalId);
                foreach (RoleInfo objRole in arrRoles)
                {
                    if (objRole.RSVPCode == code)
                    {
                        objRoles.UpdateUserRole(PortalId, UserInfo.UserID, objRole.RoleID);
                        rsvpCodeExists = true;
                        OnSubscriptionUpdated(new SubscriptionUpdatedEventArgs(false, objRole.RoleName));
                    }
                }
                if (rsvpCodeExists)
                {
                    lblRSVP.Text = Localization.GetString("RSVPSuccess", LocalResourceFile);
                    txtRSVPCode.Text = "";
                }
                else
                {
                    lblRSVP.Text = Localization.GetString("RSVPFailure", LocalResourceFile);
                }
            }
            DataBind();
        }

        protected void grdServices_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
            int roleID = Convert.ToInt32(e.CommandArgument);
            if (commandName == Localization.GetString("Subscribe", LocalResourceFile) || commandName == Localization.GetString("Renew", LocalResourceFile))
            {
                Subscribe(roleID, false);
            }
            else if (commandName == Localization.GetString("Unsubscribe", LocalResourceFile))
            {
                Subscribe(roleID, true);
            }
            else if (commandName == Localization.GetString("Unsubscribe", LocalResourceFile))
            {
                Subscribe(roleID, true);
            }
            else if (commandName == "UseTrial")
            {
                UseTrial(roleID);
            }
            DataBind();
        }

        #region Nested type: SubscriptionUpdatedEventArgs

        public class SubscriptionUpdatedEventArgs
        {
            public SubscriptionUpdatedEventArgs(bool cancel, string roleName)
            {
                Cancel = cancel;
                RoleName = roleName;
            }

            public bool Cancel { get; set; }

            public string RoleName { get; set; }
        }

        #endregion
    }
}