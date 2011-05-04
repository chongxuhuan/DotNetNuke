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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Vendors
{
    public partial class EditAffiliate : PortalModuleBase
    {
        private int AffiliateId = -1;
        private int VendorId = -1;

        public new int PortalId
        {
            get
            {
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    return -1;
                }
                else
                {
                    return PortalSettings.PortalId;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdDelete.Click += cmdDelete_Click;
            cmdSend.Click += cmdSend_Click;
            cmdUpdate.Click += cmdUpdate_Click;

            if ((Request.QueryString["VendorId"] != null))
            {
                VendorId = Int32.Parse(Request.QueryString["VendorId"]);
            }
            if ((Request.QueryString["AffilId"] != null))
            {
                AffiliateId = Int32.Parse(Request.QueryString["AffilId"]);
            }
            cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal(txtStartDate);
            cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal(txtEndDate);
            if (Page.IsPostBack == false)
            {
                ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));
                var objAffiliates = new AffiliateController();
                if (AffiliateId != Null.NullInteger)
                {
                    AffiliateInfo objAffiliate = objAffiliates.GetAffiliate(AffiliateId, VendorId, PortalId);
                    if (objAffiliate != null)
                    {
                        if (!Null.IsNull(objAffiliate.StartDate))
                        {
                            txtStartDate.Text = objAffiliate.StartDate.ToShortDateString();
                        }
                        if (!Null.IsNull(objAffiliate.EndDate))
                        {
                            txtEndDate.Text = objAffiliate.EndDate.ToShortDateString();
                        }
                        txtCPC.Text = objAffiliate.CPC.ToString("#0.0####");
                        txtCPA.Text = objAffiliate.CPA.ToString("#0.0####");
                    }
                    else
                    {
                        Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
                    }
                }
                else
                {
                    txtCPC.Text = 0.ToString("#0.0####");
                    txtCPA.Text = 0.ToString("#0.0####");
                    cmdDelete.Visible = false;
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (AffiliateId != -1)
            {
                var objAffiliates = new AffiliateController();
                objAffiliates.DeleteAffiliate(AffiliateId);
                Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
            }
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            var objVendors = new VendorController();
            VendorInfo objVendor;
            objVendor = objVendors.GetVendor(VendorId, PortalId);
            if (objVendor != null)
            {
                if (!Null.IsNull(objVendor.Email))
                {
                    var Custom = new ArrayList();
                    Custom.Add(objVendor.VendorName);
                    Custom.Add(Globals.GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request, true) + "/" + Globals.glbDefaultPage + "?AffiliateId=" + VendorId);
                    string errorMsg = Mail.SendMail(PortalSettings.Email,
                                                    objVendor.Email,
                                                    "",
                                                    Localization.GetSystemMessage(PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_SUBJECT"),
                                                    Localization.GetSystemMessage(PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_BODY", Localization.GlobalResourceFile, Custom),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "");
                    string strMessage;
                    if (String.IsNullOrEmpty(errorMsg))
                    {
                        strMessage = Localization.GetString("NotificationSuccess", LocalResourceFile);
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
                    }
                    else
                    {
                        strMessage = Localization.GetString("NotificationFailure", LocalResourceFile);
                        strMessage = string.Format(strMessage, errorMsg);
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                    }
                }
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var objAffiliate = new AffiliateInfo();
                objAffiliate.AffiliateId = AffiliateId;
                objAffiliate.VendorId = VendorId;
                if (!String.IsNullOrEmpty(txtStartDate.Text))
                {
                    objAffiliate.StartDate = DateTime.Parse(txtStartDate.Text);
                }
                else
                {
                    objAffiliate.StartDate = Null.NullDate;
                }
                if (!String.IsNullOrEmpty(txtEndDate.Text))
                {
                    objAffiliate.EndDate = DateTime.Parse(txtEndDate.Text);
                }
                else
                {
                    objAffiliate.EndDate = Null.NullDate;
                }
                objAffiliate.CPC = double.Parse(txtCPC.Text);
                objAffiliate.CPA = double.Parse(txtCPA.Text);
                var objAffiliates = new AffiliateController();
                if (AffiliateId == -1)
                {
                    objAffiliates.AddAffiliate(objAffiliate);
                }
                else
                {
                    objAffiliates.UpdateAffiliate(objAffiliate);
                }
                Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
            }
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitializeComponent();
        }
    }
}