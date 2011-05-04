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
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Internals;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls.Extensions;

using Calendar = DotNetNuke.Common.Utilities.Calendar;
using DataCache = DotNetNuke.Common.Utilities.DataCache;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Portals
{
    public partial class SiteSettings : PortalModuleBase
    {

        #region Private Members

        private int _portalId = -1;

        private string SelectedCultureCode
        {
            get
            {
                return LocaleController.Instance.GetCurrentLocale(PortalId).Code;
            }
        } 

        #endregion

        #region Private Methods

        private void BindAliases(PortalInfo portal)
        {
            var portalSettings = new PortalSettings(portal);
            var portalAliasController = new PortalAliasController();
            ArrayList aliases = portalAliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
            
            string portalAliasMapping = portalSettings.PortalAliasMappingMode.ToString().ToUpper();
            if (String.IsNullOrEmpty(portalAliasMapping))
            {
                portalAliasMapping = "CANONICALURL";
            }
            portalAliasModeButtonList.Select(portalAliasMapping, false);

            if (portalAliasMapping.ToUpperInvariant() == "NONE")
            {
                defaultAliasRow.Visible = false;
            }
            else
            {
                defaultAliasRow.Visible = true;
                defaultAliasDropDown.DataSource = aliases;
                defaultAliasDropDown.DataBind();

                string defaultAlias = PortalController.GetPortalSetting("DefaultPortalAlias", portal.PortalID, "");
                if (defaultAliasDropDown.Items.FindByValue(defaultAlias) != null)
                {
                    defaultAliasDropDown.Items.FindByValue(defaultAlias).Selected = true;
                }
            }

            //Auto Add Portal Alias
            if (new PortalController().GetPortals().Count > 1)
            {
                chkAutoAddPortalAlias.Enabled = false;
                chkAutoAddPortalAlias.Checked = false;
            }
            else
            {
                chkAutoAddPortalAlias.Checked = HostController.Instance.GetBoolean("AutoAddPortalAlias");
            }           
        }

        private void BindDesktopModules()
        {
            Dictionary<int, DesktopModuleInfo> dicModules = DesktopModuleController.GetDesktopModules(Null.NullInteger);
            Dictionary<int, PortalDesktopModuleInfo> dicPortalDesktopModules = DesktopModuleController.GetPortalDesktopModulesByPortalID(_portalId);
            foreach (PortalDesktopModuleInfo objPortalDesktopModule in dicPortalDesktopModules.Values)
            {
                dicModules.Remove(objPortalDesktopModule.DesktopModuleID);
            }
            ctlDesktopModules.AvailableDataSource = dicModules.Values;
            ctlDesktopModules.SelectedDataSource = dicPortalDesktopModules.Values;
        }

        private void BindDetails(PortalInfo portal)
        {
            txtPortalName.Text = portal.PortalName;
            txtDescription.Text = portal.Description;
            txtKeyWords.Text = portal.KeyWords;
            lblGUID.Text = portal.GUID.ToString().ToUpper();
            txtFooterText.Text = portal.FooterText;
        }

        private void BindHostSettings(PortalInfo portal)
        {
            if (!Null.IsNull(portal.ExpiryDate))
            {
                txtExpiryDate.Text = portal.ExpiryDate.ToShortDateString();
            }
            txtHostFee.Text = portal.HostFee.ToString();
            txtHostSpace.Text = portal.HostSpace.ToString();
            txtPageQuota.Text = portal.PageQuota.ToString();
            txtUserQuota.Text = portal.UserQuota.ToString();
            if (portal.SiteLogHistory != Null.NullInteger)
            {
                txtSiteLogHistory.Text = portal.SiteLogHistory.ToString();
            }           
        }

        private void BindMarketing(PortalInfo portal)
        {
            //Load DocTypes
            var searchEngines = new Dictionary<string, string>
                               {
                                   { "Google", "http://www.google.com/addurl?q=" + Globals.HTTPPOSTEncode(Globals.AddHTTP(Globals.GetDomainName(Request))) }, 
                                   { "Yahoo", "http://siteexplorer.search.yahoo.com/submit" }, 
                                   { "Microsoft", "http://search.msn.com.sg/docs/submit.aspx" }
                               };

            cboSearchEngine.DataSource = searchEngines;
            cboSearchEngine.DataBind();

            var portalAliasController = new PortalAliasController();
            ArrayList aliases = portalAliasController.GetPortalAliasArrayByPortalID(portal.PortalID);
            if (PortalController.IsChildPortal(portal, Globals.GetAbsoluteServerPath(Request)))
            {
                txtSiteMap.Text = Globals.AddHTTP(Globals.GetDomainName(Request)) + "/SiteMap.aspx?portalid=" + portal.PortalID;
            }
            else
            {
                if (aliases.Count > 0)
                {
                    var objPortalAliasInfo = (PortalAliasInfo)aliases[0];
                    txtSiteMap.Text = Globals.AddHTTP(objPortalAliasInfo.HTTPAlias) + "/SiteMap.aspx";
                }
                else
                {
                    txtSiteMap.Text = Globals.AddHTTP(Globals.GetDomainName(Request)) + "/SiteMap.aspx";
                }
            }
            optBanners.SelectedIndex = portal.BannerAdvertising;
            if (UserInfo.IsSuperUser)
            {
                lblBanners.Visible = false;
            }
            else
            {
                optBanners.Enabled = portal.BannerAdvertising != 2;
                lblBanners.Visible = portal.BannerAdvertising == 2;
            }
            
        }

        private void BindPages(PortalInfo portal, string activeLanguage)
        {
            List<TabInfo> listTabs = TabController.GetPortalTabs(TabController.GetTabsBySortOrder(portal.PortalID, activeLanguage, true),
                                                                 Null.NullInteger,
                                                                 true,
                                                                 "<" + Localization.GetString("None_Specified") + ">",
                                                                 true,
                                                                 false,
                                                                 false,
                                                                 false,
                                                                 false);
            cboSplashTabId.DataSource = listTabs;
            cboSplashTabId.DataBind(portal.SplashTabId.ToString());

            cboHomeTabId.DataSource = listTabs;
            cboHomeTabId.DataBind(portal.HomeTabId.ToString());

            cboLoginTabId.DataSource = listTabs;
            cboLoginTabId.DataBind(portal.LoginTabId.ToString());

            cboRegisterTabId.DataSource = listTabs;
            cboRegisterTabId.DataBind(portal.RegisterTabId.ToString());

            cboSearchTabId.DataSource = listTabs;
            cboSearchTabId.DataBind(portal.SearchTabId.ToString());

            listTabs = TabController.GetPortalTabs(portal.PortalID, Null.NullInteger, false, true);

            cboUserTabId.DataSource = listTabs;
            cboUserTabId.DataBind(portal.UserTabId.ToString());
        }

        private void BindPaymentProcessor(PortalInfo portal)
        {
            var listController = new ListController();
            currencyCombo.DataSource = listController.GetListEntryInfoCollection("Currency", "");
            var currency = portal.Currency;
            if (String.IsNullOrEmpty(currency))
            {
                currency = "USD";
            }
            currencyCombo.DataBind(currency);

            processorCombo.DataSource = listController.GetListEntryInfoCollection("Processor", "");
            processorCombo.DataBind();
            processorCombo.InsertItem(0, "<" + Localization.GetString("None_Specified") + ">", "");
            processorCombo.Select(Entities.Host.Host.PaymentProcessor, true);

            processorLink.NavigateUrl = Globals.AddHTTP(processorCombo.SelectedItem.Value);

            txtUserId.Text = portal.ProcessorUserId;
            txtPassword.Attributes.Add("value", portal.ProcessorPassword);

            // use sandbox?
            bool bolPayPalSandbox = Boolean.Parse(PortalController.GetPortalSetting("paypalsandbox", portal.PortalID, "false"));
            chkPayPalSandboxEnabled.Checked = bolPayPalSandbox;

            // return url after payment or on cancel
            string strPayPalReturnURL = PortalController.GetPortalSetting("paypalsubscriptionreturn", portal.PortalID, Null.NullString);
            txtPayPalReturnURL.Text = strPayPalReturnURL;
            string strPayPalCancelURL = PortalController.GetPortalSetting("paypalsubscriptioncancelreturn", portal.PortalID, Null.NullString);
            txtPayPalCancelURL.Text = strPayPalCancelURL;
        }

        private void BindPortal(int portalId, string activeLanguage)
        {
            var portalController = new PortalController();
            var portal = portalController.GetPortal(portalId, activeLanguage);

            BindDetails(portal);

            BindMarketing(portal);

            ctlLogo.FilePath = portal.LogoFile;
            ctlLogo.FileFilter = Globals.glbImageFileTypes;
            ctlBackground.FilePath = portal.BackgroundFile;
            ctlBackground.FileFilter = Globals.glbImageFileTypes;
            ctlFavIcon.FilePath = new FavIcon(portal.PortalID).GetFilePath();
            chkSkinWidgestEnabled.Checked = PortalController.GetPortalSettingAsBoolean("EnableSkinWidgets", portalId, true);

            BindSkins(portal);

            BindPages(portal, activeLanguage);

            lblHomeDirectory.Text = portal.HomeDirectory;

            optUserRegistration.SelectedIndex = portal.UserRegistration;

            BindPaymentProcessor(portal);

            BindUsability(portal);

            var roleController = new RoleController();
            cboAdministratorId.DataSource = roleController.GetUserRolesByRoleName(portalId, portal.AdministratorRoleName);
            cboAdministratorId.DataBind(portal.AdministratorId.ToString());

            //PortalSettings for portal being edited
            var portalSettings = new PortalSettings(portal);

            cboTimeZone.DataBind(portalSettings.TimeZone.ToString());

            if (UserInfo.IsSuperUser)
            {
                BindAliases(portal);

                BindSSLSettings(portal);

                BindHostSettings(portal);
            }

            LoadStyleSheet(portal);

            ctlAudit.Entity = portal;
        }

        private void BindSkins(PortalInfo portal)
        {
            var skins = SkinController.GetSkins(portal, SkinController.RootSkin, SkinScope.All)
                                                     .ToDictionary(skin => skin.Key, skin => skin.Value);
            var containers = SkinController.GetSkins(portal, SkinController.RootContainer, SkinScope.All)
                                                    .ToDictionary(skin => skin.Key, skin => skin.Value);
            portalSkinCombo.DataSource = skins;
            portalSkinCombo.DataBind(PortalController.GetPortalSetting("DefaultPortalSkin", portal.PortalID, Host.DefaultPortalSkin));

            portalContainerCombo.DataSource = containers;
            portalContainerCombo.DataBind(PortalController.GetPortalSetting("DefaultPortalContainer", portal.PortalID, Host.DefaultPortalContainer));

            editSkinCombo.DataSource = skins;
            editSkinCombo.DataBind(PortalController.GetPortalSetting("DefaultAdminSkin", portal.PortalID, Host.DefaultAdminSkin));

            editContainerCombo.DataSource = containers;
            editContainerCombo.DataBind(PortalController.GetPortalSetting("DefaultAdminContainer", portal.PortalID, Host.DefaultAdminContainer));

            uploadSkinLink.NavigateUrl = Util.InstallURL(ModuleContext.TabId, "");

            if (PortalSettings.EnablePopUps)
            {
                uploadSkinLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(uploadSkinLink.NavigateUrl, this, PortalSettings));
            }
        }

        private void BindSSLSettings(PortalInfo portal)
        {
            chkSSLEnabled.Checked = PortalController.GetPortalSettingAsBoolean("SSLEnabled", portal.PortalID, false);
            chkSSLEnforced.Checked = PortalController.GetPortalSettingAsBoolean("SSLEnforced", portal.PortalID, false);
            txtSSLURL.Text = PortalController.GetPortalSetting("SSLURL", portal.PortalID, Null.NullString);
            txtSTDURL.Text = PortalController.GetPortalSetting("STDURL", portal.PortalID, Null.NullString);
        }

        private void BindUsability(PortalInfo portal)
        {
            //PortalSettings for portal being edited
            var portalSettings = new PortalSettings(portal);
            chkInlineEditor.Checked = portalSettings.InlineEditorEnabled;
            enablePopUpsCheckBox.Checked = portalSettings.EnablePopUps;
            chkHideSystemFolders.Checked = portalSettings.HideFoldersEnabled;

            var mode = (portalSettings.DefaultControlPanelMode== PortalSettings.Mode.Edit) ? "EDIT" : "VIEW";
            optControlPanelMode.Select(mode, false);

            optControlPanelVisibility.Select(PortalController.GetPortalSetting("ControlPanelVisibility", portal.PortalID, "MAX"), false);

            optControlPanelSecurity.Select(PortalController.GetPortalSetting("ControlPanelSecurity", portal.PortalID, "MODULE"), false);
        }

        private void LoadStyleSheet(PortalInfo portalInfo)
        {
            string uploadDirectory = "";
            if (portalInfo != null)
            {
                uploadDirectory = portalInfo.HomeDirectoryMapPath;
            }
            if (File.Exists(uploadDirectory + "portal.css"))
            {
                using (var text = File.OpenText(uploadDirectory + "portal.css"))
                {
                    txtStyleSheet.Text = text.ReadToEnd();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected string FormatCurrency()
        {
            string retValue = "";
            try
            {
                retValue = Host.HostCurrency + " / " + Localization.GetString("Month");
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return retValue;
        }

        protected string FormatFee(object objHostFee)
        {
            string retValue = "";
            try
            {
                if (objHostFee != DBNull.Value)
                {
                    retValue = ((float)objHostFee).ToString("#,##0.00");
                }
                else
                {
                    retValue = "0";
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return retValue;
        }

        protected bool IsSubscribed(int portalModuleDefinitionId)
        {
            try
            {
                return Null.IsNull(portalModuleDefinitionId) == false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                return false;
            }
        }

        protected bool IsSuperUser()
        {
            return UserInfo.IsSuperUser;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            jQuery.RequestDnnPluginsRegistration();

            ctlDesktopModules.LocalResourceFile = LocalResourceFile;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdDelete.Click += DeletePortal;
            cmdRestore.Click += cmdRestore_Click;
            cmdSave.Click += cmdSave_Click;
            cmdUpdate.Click += UpdatePortal;
            cmdVerification.Click += cmdVerification_Click;
            ctlDesktopModules.AddAllButtonClick += ctlDesktopModules_AddAllButtonClick;
            ctlDesktopModules.AddButtonClick += ctlDesktopModules_AddButtonClick;
            ctlDesktopModules.RemoveAllButtonClick += ctlDesktopModules_RemoveAllButtonClick;
            ctlDesktopModules.RemoveButtonClick += ctlDesktopModules_RemoveButtonClick;

            try
            {
                if ((Request.QueryString["pid"] != null) && (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId || UserInfo.IsSuperUser))
                {
                    _portalId = Int32.Parse(Request.QueryString["pid"]);
                    ctlLogo.ShowUpLoad = false;
                    ctlBackground.ShowUpLoad = false;
                    ctlFavIcon.ShowUpLoad = false;
                    cancelHyperLink.Visible = true;
                    cancelHyperLink.NavigateUrl = Globals.NavigateURL();
                }
                else
                {
                    _portalId = PortalId;
                    ctlLogo.ShowUpLoad = true;
                    ctlBackground.ShowUpLoad = true;
                    ctlFavIcon.ShowUpLoad = true;
                    cancelHyperLink.Visible = false;
                }

                cmdExpiryCalendar.NavigateUrl = Calendar.InvokePopupCal(txtExpiryDate);
                ClientAPI.AddButtonConfirm(cmdRestore, Localization.GetString("RestoreCCSMessage", LocalResourceFile));
                BindDesktopModules();
                if (Page.IsPostBack == false)
                {
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteMessage", LocalResourceFile));

                    BindPortal(_portalId, SelectedCultureCode);
                }

                if (UserInfo.IsSuperUser)
                {
                    hostSections.Visible = true;
                    cmdDelete.Visible = (_portalId != PortalId);
                }
                else
                {
                    hostSections.Visible = false;
                    cmdDelete.Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        private void DeletePortal(object sender, EventArgs e)
        {
            try
            {
                var objPortalController = new PortalController();
                PortalInfo objPortalInfo = objPortalController.GetPortal(_portalId);
                if (objPortalInfo != null)
                {
                    string strMessage = PortalController.DeletePortal(objPortalInfo, Globals.GetAbsoluteServerPath(Request));
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        var objEventLog = new EventLogController();
                        objEventLog.AddLog("PortalName", objPortalInfo.PortalName, PortalSettings, UserId, EventLogController.EventLogType.PORTAL_DELETED);
                        if (_portalId == PortalId)
                        {
                            if (!string.IsNullOrEmpty(Host.HostURL))
                            {
                                Response.Redirect(Globals.AddHTTP(Host.HostURL));
                            }
                            else
                            {
                                Response.End();
                            }
                        }
                        else
                        {
                            Response.Redirect(Convert.ToString(ViewState["UrlReferrer"]), true);
                        }
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdRestore_Click(object sender, EventArgs e)
        {
            try
            {
                var portalController = new PortalController();
                PortalInfo portal = portalController.GetPortal(_portalId);
                if (portal != null)
                {
                    if (File.Exists(portal.HomeDirectoryMapPath + "portal.css"))
                    {
                        File.Delete(portal.HomeDirectoryMapPath + "portal.css");
                    }
                    if (File.Exists(Globals.HostMapPath + "portal.css"))
                    {
                        File.Copy(Globals.HostMapPath + "portal.css", portal.HomeDirectoryMapPath + "portal.css");
                    }
                }
                LoadStyleSheet(portal);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strUploadDirectory = "";
                var objPortalController = new PortalController();
                PortalInfo objPortal = objPortalController.GetPortal(_portalId);
                if (objPortal != null)
                {
                    strUploadDirectory = objPortal.HomeDirectoryMapPath;
                }
                if (File.Exists(strUploadDirectory + "portal.css"))
                {
                    File.SetAttributes(strUploadDirectory + "portal.css", FileAttributes.Normal);
                }
                using(var writer = File.CreateText(strUploadDirectory + "portal.css"))
                {
                    writer.WriteLine(txtStyleSheet.Text);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void UpdatePortal(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var objPortalController = new PortalController();
                    PortalInfo objPortal = objPortalController.GetPortal(_portalId);
                    string strLogo = String.Format("FileID={0}", ctlLogo.FileID);
                    string strBackground = String.Format("FileID={0}", ctlBackground.FileID);
                    bool refreshPage = (strBackground == objPortal.BackgroundFile || strLogo == objPortal.LogoFile);
                    double dblHostFee = 0;
                    if (!String.IsNullOrEmpty(txtHostFee.Text))
                    {
                        dblHostFee = double.Parse(txtHostFee.Text);
                    }
                    double dblHostSpace = 0;
                    if (!String.IsNullOrEmpty(txtHostSpace.Text))
                    {
                        dblHostSpace = double.Parse(txtHostSpace.Text);
                    }
                    int intPageQuota = 0;
                    if (!String.IsNullOrEmpty(txtPageQuota.Text))
                    {
                        intPageQuota = int.Parse(txtPageQuota.Text);
                    }
                    double intUserQuota = 0;
                    if (!String.IsNullOrEmpty(txtUserQuota.Text))
                    {
                        intUserQuota = int.Parse(txtUserQuota.Text);
                    }
                    int intSiteLogHistory = -1;
                    if (!String.IsNullOrEmpty(txtSiteLogHistory.Text))
                    {
                        intSiteLogHistory = int.Parse(txtSiteLogHistory.Text);
                    }
                    DateTime datExpiryDate = Null.NullDate;
                    if (!String.IsNullOrEmpty(txtExpiryDate.Text))
                    {
                        datExpiryDate = Convert.ToDateTime(txtExpiryDate.Text);
                    }
                    int intSplashTabId = Null.NullInteger;
                    if (cboSplashTabId.SelectedItem != null)
                    {
                        intSplashTabId = int.Parse(cboSplashTabId.SelectedItem.Value);
                    }
                    int intHomeTabId = Null.NullInteger;
                    if (cboHomeTabId.SelectedItem != null)
                    {
                        intHomeTabId = int.Parse(cboHomeTabId.SelectedItem.Value);
                    }
                    int intLoginTabId = Null.NullInteger;
                    if (cboLoginTabId.SelectedItem != null)
                    {
                        intLoginTabId = int.Parse(cboLoginTabId.SelectedItem.Value);
                    }
                    int intRegisterTabId = Null.NullInteger;
                    if (cboRegisterTabId.SelectedItem != null)
                    {
                        intRegisterTabId = int.Parse(cboRegisterTabId.SelectedItem.Value);
                    }
                    int intUserTabId = Null.NullInteger;
                    if (cboUserTabId.SelectedItem != null)
                    {
                        intUserTabId = int.Parse(cboUserTabId.SelectedItem.Value);
                    }
                    int intSearchTabId = Null.NullInteger;
                    if (cboSearchTabId.SelectedItem != null)
                    {
                        intSearchTabId = int.Parse(cboSearchTabId.SelectedItem.Value);
                    }

                    if (txtPassword.Attributes["value"] != null)
                    {
                        txtPassword.Attributes["value"] = txtPassword.Text;
                    }
                    if (!UserInfo.IsSuperUser)
                    {
                        bool hostChanged = false;
                        if (dblHostFee != objPortal.HostFee)
                        {
                            hostChanged = true;
                        }
                        if (dblHostSpace != objPortal.HostSpace)
                        {
                            hostChanged = true;
                        }
                        if (intPageQuota != objPortal.PageQuota)
                        {
                            hostChanged = true;
                        }
                        if (intUserQuota != objPortal.UserQuota)
                        {
                            hostChanged = true;
                        }
                        if (intSiteLogHistory != objPortal.SiteLogHistory)
                        {
                            hostChanged = true;
                        }
                        if (datExpiryDate != objPortal.ExpiryDate)
                        {
                            hostChanged = true;
                        }
                        if (hostChanged)
                        {
                            throw new Exception();
                        }
                    }

                    objPortalController.UpdatePortalInfo(_portalId,
                                                         txtPortalName.Text,
                                                         strLogo,
                                                         txtFooterText.Text,
                                                         datExpiryDate,
                                                         optUserRegistration.SelectedIndex,
                                                         optBanners.SelectedIndex,
                                                         currencyCombo.SelectedItem.Value,
                                                         Convert.ToInt32(cboAdministratorId.SelectedItem.Value),
                                                         dblHostFee,
                                                         dblHostSpace,
                                                         intPageQuota,
                                                         (int) intUserQuota,
                                                         String.IsNullOrEmpty(processorCombo.SelectedValue) ? "" : processorCombo.SelectedItem.Text,
                                                         txtUserId.Text,
                                                         txtPassword.Text,
                                                         txtDescription.Text,
                                                         txtKeyWords.Text,
                                                         strBackground,
                                                         intSiteLogHistory,
                                                         intSplashTabId,
                                                         intHomeTabId,
                                                         intLoginTabId,
                                                         intRegisterTabId,
                                                         intUserTabId,
                                                         intSearchTabId,
                                                         objPortal.DefaultLanguage,
                                                         lblHomeDirectory.Text,
                                                         SelectedCultureCode);
                    if (!refreshPage)
                    {
                        refreshPage = (PortalSettings.DefaultAdminSkin == editSkinCombo.SelectedValue) || 
                                        (PortalSettings.DefaultAdminContainer == editContainerCombo.SelectedValue);
                    }
                    PortalController.UpdatePortalSetting(_portalId, "EnableSkinWidgets", chkSkinWidgestEnabled.Checked.ToString(), false);
                    PortalController.UpdatePortalSetting(_portalId, "DefaultAdminSkin", editSkinCombo.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "DefaultPortalSkin", portalSkinCombo.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "DefaultAdminContainer", editContainerCombo.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "DefaultPortalContainer", portalContainerCombo.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "EnablePopups", enablePopUpsCheckBox.Checked.ToString(), false);
                    PortalController.UpdatePortalSetting(_portalId, "InlineEditorEnabled", chkInlineEditor.Checked.ToString(), false);
                    PortalController.UpdatePortalSetting(_portalId, "HideFoldersEnabled", chkHideSystemFolders.Checked.ToString(), false);
                    PortalController.UpdatePortalSetting(_portalId, "ControlPanelMode", optControlPanelMode.SelectedItem.Value, false);
                    PortalController.UpdatePortalSetting(_portalId, "ControlPanelVisibility", optControlPanelVisibility.SelectedItem.Value, false);
                    PortalController.UpdatePortalSetting(_portalId, "ControlPanelSecurity", optControlPanelSecurity.SelectedItem.Value, false);
                    PortalController.UpdatePortalSetting(_portalId, "paypalsandbox", chkPayPalSandboxEnabled.Checked.ToString(), false);
                    PortalController.UpdatePortalSetting(_portalId, "paypalsubscriptionreturn", txtPayPalReturnURL.Text, false);
                    PortalController.UpdatePortalSetting(_portalId, "paypalsubscriptioncancelreturn", txtPayPalCancelURL.Text, false);
                    PortalController.UpdatePortalSetting(_portalId, "TimeZone", cboTimeZone.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "PortalAliasMapping", portalAliasModeButtonList.SelectedValue, false);
                    PortalController.UpdatePortalSetting(_portalId, "DefaultPortalAlias", defaultAliasDropDown.SelectedValue, false);
                    HostController.Instance.Update("AutoAddPortalAlias", chkAutoAddPortalAlias.Checked ? "Y" : "N", true);

                    new FavIcon(_portalId).Update(ctlFavIcon.FileID);

                    if (IsSuperUser())
                    {
                        PortalController.UpdatePortalSetting(_portalId, "SSLEnabled", chkSSLEnabled.Checked.ToString(), false);
                        PortalController.UpdatePortalSetting(_portalId, "SSLEnforced", chkSSLEnforced.Checked.ToString(), false);
                        PortalController.UpdatePortalSetting(_portalId, "SSLURL", AddPortalAlias(txtSSLURL.Text, _portalId), false);
                        PortalController.UpdatePortalSetting(_portalId, "STDURL", AddPortalAlias(txtSTDURL.Text, _portalId), false);
                    }
                    if (refreshPage)
                    {
                        Response.Redirect(Request.RawUrl, true);
                    }
                    BindPortal(_portalId, SelectedCultureCode);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
                finally
                {
                    DataCache.ClearPortalCache(_portalId, false);
                }
            }
        }

        private string AddPortalAlias(string portalAlias, int portalID)
        {
            if (!String.IsNullOrEmpty(portalAlias))
            {
                if (portalAlias.IndexOf("://") != -1)
                {
                    portalAlias = portalAlias.Remove(0, portalAlias.IndexOf("://") + 3);
                }
                var objPortalAliasController = new PortalAliasController();
                PortalAliasInfo objPortalAlias = objPortalAliasController.GetPortalAlias(portalAlias, portalID);
                if (objPortalAlias == null)
                {
                    objPortalAlias = new PortalAliasInfo();
                    objPortalAlias.PortalID = portalID;
                    objPortalAlias.HTTPAlias = portalAlias;
                    objPortalAliasController.AddPortalAlias(objPortalAlias);
                }
            }
            return portalAlias;
        }

        protected void cmdVerification_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtVerification.Text) && txtVerification.Text.EndsWith(".html"))
            {
                if (!File.Exists(Globals.ApplicationMapPath + "\\" + txtVerification.Text))
                {
                    using(var writer = File.CreateText(Globals.ApplicationMapPath + "\\" + txtVerification.Text))
                    {
                        writer.WriteLine("google-site-verification: " + txtVerification.Text);
                    }
                }
            }
        }

        protected void ctlDesktopModules_AddAllButtonClick(object sender, EventArgs e)
        {
            foreach (DesktopModuleInfo desktopModule in DesktopModuleController.GetDesktopModules(Null.NullInteger).Values)
            {
                DesktopModuleController.AddDesktopModuleToPortal(_portalId, desktopModule.DesktopModuleID, true, false);
            }
            DataCache.ClearPortalCache(_portalId, false);
            BindDesktopModules();
        }

        protected void ctlDesktopModules_AddButtonClick(object sender, DualListBoxEventArgs e)
        {
            if (e.Items != null)
            {
                foreach (string desktopModule in e.Items)
                {
                    DesktopModuleController.AddDesktopModuleToPortal(_portalId, int.Parse(desktopModule), true, false);
                }
            }
            DataCache.ClearPortalCache(_portalId, false);
            BindDesktopModules();
        }

        protected void ctlDesktopModules_RemoveAllButtonClick(object sender, EventArgs e)
        {
            foreach (DesktopModuleInfo desktopModule in DesktopModuleController.GetDesktopModules(Null.NullInteger).Values)
            {
                DesktopModuleController.RemoveDesktopModuleFromPortal(_portalId, desktopModule.DesktopModuleID, false);
            }
            DataCache.ClearPortalCache(_portalId, false);
            BindDesktopModules();
        }

        protected void ctlDesktopModules_RemoveButtonClick(object sender, DualListBoxEventArgs e)
        {
            if (e.Items != null)
            {
                foreach (string desktopModule in e.Items)
                {
                    DesktopModuleController.RemoveDesktopModuleFromPortal(_portalId, int.Parse(desktopModule), false);
                }
            }
            DataCache.ClearPortalCache(_portalId, false);
            BindDesktopModules();
        }
    }
}