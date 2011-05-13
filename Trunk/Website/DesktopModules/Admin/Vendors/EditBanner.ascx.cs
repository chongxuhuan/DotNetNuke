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
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using Calendar = DotNetNuke.Common.Utilities.Calendar;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Vendors
{
    public partial class EditBanner : PortalModuleBase
    {

        #region Members

        private int BannerId = Null.NullInteger;
        private int VendorId = Null.NullInteger;
        protected Label lblBannerGroup;

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

        #endregion

        #region Public Methods

        public string FormatItem(int VendorId, int BannerId, int BannerTypeId, string BannerName, string ImageFile, string Description, string URL, int Width, int Height)
        {
            var objBanners = new BannerController();
            return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, PortalId == -1 ? "G" : "L", PortalSettings.HomeDirectory);
        }

        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += OnCancelClick;
            cmdCopy.Click += OnCopyClick;
            cmdDelete.Click += OnDeleteClick;
            cmdEmail.Click += OnEmailClick;
            cmdUpdate.Click += OnUpdateClick;
            DNNTxtBannerGroup.PopulateOnDemand += PopulateBannersOnDemand;

            try
            {
                if ((Request.QueryString["VendorId"] != null))
                {
                    VendorId = Int32.Parse(Request.QueryString["VendorId"]);
                }
                if ((Request.QueryString["BannerId"] != null))
                {
                    BannerId = Int32.Parse(Request.QueryString["BannerId"]);
                }
                cmdStartCalendar.NavigateUrl = Calendar.InvokePopupCal(txtStartDate);
                cmdEndCalendar.NavigateUrl = Calendar.InvokePopupCal(txtEndDate);
                if (Page.IsPostBack == false)
                {
                    ctlImage.FileFilter = Globals.glbImageFileTypes;
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));
                    var objBannerTypes = new BannerTypeController();
                    cboBannerType.DataSource = objBannerTypes.GetBannerTypes();
                    cboBannerType.DataBind();
                    var objBanners = new BannerController();
                    if (BannerId != Null.NullInteger)
                    {
                        BannerInfo objBanner = objBanners.GetBanner(BannerId);
                        if (objBanner != null)
                        {
                            txtBannerName.Text = objBanner.BannerName;
                            cboBannerType.Items.FindByValue(objBanner.BannerTypeId.ToString()).Selected = true;
                            DNNTxtBannerGroup.Text = objBanner.GroupName;
                            ctlImage.Url = objBanner.ImageFile;
                            if (objBanner.Width != 0)
                            {
                                txtWidth.Text = objBanner.Width.ToString();
                            }
                            if (objBanner.Height != 0)
                            {
                                txtHeight.Text = objBanner.Height.ToString();
                            }
                            txtDescription.Text = objBanner.Description;
                            if (!String.IsNullOrEmpty(objBanner.URL))
                            {
                                ctlURL.Url = objBanner.URL;
                            }
                            txtImpressions.Text = objBanner.Impressions.ToString();
                            txtCPM.Text = objBanner.CPM.ToString();
                            if (!Null.IsNull(objBanner.StartDate))
                            {
                                txtStartDate.Text = objBanner.StartDate.ToShortDateString();
                            }
                            if (!Null.IsNull(objBanner.EndDate))
                            {
                                txtEndDate.Text = objBanner.EndDate.ToShortDateString();
                            }
                            optCriteria.Items.FindByValue(objBanner.Criteria.ToString()).Selected = true;
                            ctlAudit.CreatedByUser = objBanner.CreatedByUser;
                            ctlAudit.CreatedDate = objBanner.CreatedDate.ToString();
                            var arrBanners = new ArrayList();

                            arrBanners.Add(objBanner);
                            bannersRow.Visible = true;
                            lstBanners.DataSource = arrBanners;
                            lstBanners.DataBind();       
                        }
                        else
                        {
                            Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
                        }
                    }
                    else
                    {
                        txtImpressions.Text = "0";
                        txtCPM.Text = "0";
                        optCriteria.Items.FindByValue("1").Selected = true;
                        cmdDelete.Visible = false;
                        cmdCopy.Visible = false;
                        cmdEmail.Visible = false;
                        ctlAudit.Visible = false;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnCancelClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnDeleteClick(object sender, EventArgs e)
        {
            try
            {
                if (BannerId != -1)
                {
                    var objBanner = new BannerController();
                    objBanner.DeleteBanner(BannerId);
                    Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnUpdateClick(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    if (!cmdCopy.Visible)
                    {
                        BannerId = -1;
                    }
                    DateTime StartDate = Null.NullDate;
                    if (!String.IsNullOrEmpty(txtStartDate.Text))
                    {
                        StartDate = Convert.ToDateTime(txtStartDate.Text);
                    }
                    DateTime EndDate = Null.NullDate;
                    if (!String.IsNullOrEmpty(txtEndDate.Text))
                    {
                        EndDate = Convert.ToDateTime(txtEndDate.Text);
                    }
                    var objBanner = new BannerInfo();
                    objBanner.BannerId = BannerId;
                    objBanner.VendorId = VendorId;
                    objBanner.BannerName = txtBannerName.Text;
                    objBanner.BannerTypeId = Convert.ToInt32(cboBannerType.SelectedItem.Value);
                    objBanner.GroupName = DNNTxtBannerGroup.Text;
                    objBanner.ImageFile = ctlImage.Url;
                    if (!String.IsNullOrEmpty(txtWidth.Text))
                    {
                        objBanner.Width = int.Parse(txtWidth.Text);
                    }
                    else
                    {
                        objBanner.Width = 0;
                    }
                    if (!String.IsNullOrEmpty(txtHeight.Text))
                    {
                        objBanner.Height = int.Parse(txtHeight.Text);
                    }
                    else
                    {
                        objBanner.Height = 0;
                    }
                    objBanner.Description = txtDescription.Text;
                    objBanner.URL = ctlURL.Url;
                    objBanner.Impressions = int.Parse(txtImpressions.Text);
                    objBanner.CPM = double.Parse(txtCPM.Text);
                    objBanner.StartDate = StartDate;
                    objBanner.EndDate = EndDate;
                    objBanner.Criteria = int.Parse(optCriteria.SelectedItem.Value);
                    objBanner.CreatedByUser = UserInfo.UserID.ToString();
                    var objBanners = new BannerController();
                    if (BannerId == Null.NullInteger)
                    {
                        objBanners.AddBanner(objBanner);
                    }
                    else
                    {
                        objBanners.UpdateBanner(objBanner);
                    }
                    Response.Redirect(EditUrl("VendorId", VendorId.ToString()), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnCopyClick(object sender, EventArgs e)
        {
            try
            {
                txtStartDate.Text = "";
                txtEndDate.Text = "";
                cmdDelete.Visible = false;
                cmdCopy.Visible = false;
                cmdEmail.Visible = false;
                ctlAudit.Visible = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnEmailClick(object sender, EventArgs e)
        {
            var objBanners = new BannerController();
            var objBanner = objBanners.GetBanner(BannerId);
            if (objBanner != null)
            {
                var objVendors = new VendorController();
                var objVendor = objVendors.GetVendor(objBanner.VendorId, PortalId);
                if (objVendor != null)
                {
                    if (!Null.IsNull(objVendor.Email))
                    {
                        var Custom = new ArrayList();
                        Custom.Add(objBanner.BannerName);
                        Custom.Add(objBanner.Description);
                        Custom.Add(objBanner.ImageFile);
                        Custom.Add(objBanner.CPM.ToString("#0.#####"));
                        Custom.Add(objBanner.Impressions.ToString());
                        Custom.Add(objBanner.StartDate.ToShortDateString());
                        Custom.Add(objBanner.EndDate.ToShortDateString());
                        Custom.Add(objBanner.Views.ToString());
                        Custom.Add(objBanner.ClickThroughs.ToString());
                        var errorMsg = Mail.SendMail(PortalSettings.Email,
                                                        objVendor.Email,
                                                        "",
                                                        Localization.GetSystemMessage(PortalSettings, "EMAIL_BANNER_NOTIFICATION_SUBJECT", Localization.GlobalResourceFile, Custom),
                                                        Localization.GetSystemMessage(PortalSettings, "EMAIL_BANNER_NOTIFICATION_BODY", Localization.GlobalResourceFile, Custom),
                                                        "",
                                                        "",
                                                        "",
                                                        "",
                                                        "",
                                                        "");
                        string strMessage;
                        if (String.IsNullOrEmpty(errorMsg))
                        {
                            strMessage = Localization.GetString("EmailSuccess", LocalResourceFile);
                            UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
                        }
                        else
                        {
                            strMessage = Localization.GetString("EmailFailure", LocalResourceFile);
                            strMessage = string.Format(strMessage, errorMsg);
                            UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                        }
                    }
                }
            }
        }

        protected void PopulateBannersOnDemand(object source, DNNTextSuggestEventArgs e)
        {
            var objBanners = new BannerController();
            var dt = objBanners.GetBannerGroups(PortalId);
            dt.CaseSensitive = false;
            var dr = dt.Select("GroupName like '" + e.Text + "%'");
            foreach (var d in dr)
            {
                var objNode = new DNNNode(d["GroupName"].ToString()) {ID = e.Nodes.Count.ToString()};
                e.Nodes.Add(objNode);
            }
        }

        #endregion

    }
}