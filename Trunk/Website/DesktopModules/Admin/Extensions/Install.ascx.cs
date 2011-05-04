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
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Extensions
{
    partial class Install : ModuleUserControlBase
    {
        private Installer _Installer;
        private PackageInfo _Package;
        private PackageType _PackageType;

        protected string FileName
        {
            get
            {
                return Convert.ToString(ViewState["FileName"]);
            }
            set
            {
                ViewState["FileName"] = value;
            }
        }

        protected Installer Installer
        {
            get
            {
                return _Installer;
            }
        }

        protected string ManifestFile
        {
            get
            {
                return Convert.ToString(ViewState["ManifestFile"]);
            }
            set
            {
                ViewState["ManifestFile"] = value;
            }
        }

        protected PackageInfo Package
        {
            get
            {
                return _Package;
            }
        }

        protected PackageType PackageType
        {
            get
            {
                if (_PackageType == null)
                {
                    string pType = Null.NullString;
                    if (!string.IsNullOrEmpty(Request.QueryString["ptype"]))
                    {
                        pType = Request.QueryString["ptype"];
                    }
                    _PackageType = PackageController.GetPackageType(pType);
                }
                return _PackageType;
            }
        }

        protected int InstallPortalId
        {
            get
            {
                int _PortalId = ModuleContext.PortalId;
                if (ModuleContext.IsHostMenu)
                {
                    _PortalId = Null.NullInteger;
                }
                return _PortalId;
            }
        }

        protected string ReturnURL
        {
            get
            {
                string _ReturnUrl = Server.UrlDecode(Request.Params["returnUrl"]);
                if (string.IsNullOrEmpty(_ReturnUrl))
                {
                    int TabID = ModuleContext.PortalSettings.HomeTabId;
                    if (Request.Params["rtab"] != null)
                    {
                        TabID = int.Parse(Request.Params["rtab"]);
                    }
                    _ReturnUrl = Globals.NavigateURL(TabID);
                }
                return _ReturnUrl;
            }
        }

        protected string TempInstallFolder
        {
            get
            {
                return Convert.ToString(ViewState["TempInstallFolder"]);
            }
            set
            {
                ViewState["TempInstallFolder"] = value;
            }
        }

        private void BindPackage()
        {
            CreateInstaller();
            if (Installer.IsValid)
            {
                if (_Installer.Packages.Count > 0)
                {
                    _Package = _Installer.Packages[0].Package;
                }
                packageForm.DataSource = _Package;
                packageForm.DataBind();
                licenseForm.DataSource = _Package;
                licenseForm.DataBind();
                releaseNotesForm.DataSource = _Package;
                releaseNotesForm.DataBind();
            }
            else
            {
                switch (wizInstall.ActiveStepIndex)
                {
                    case 0:
                        lblLoadMessage.Text = Localization.GetString("InstallError", LocalResourceFile);
                        phLoadLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
                        break;
                    case 3:
                        lblAcceptMessage.Text = Localization.GetString("InstallError", LocalResourceFile);
                        lblAcceptMessage.Visible = true;
                        phAcceptLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
                        break;
                }
            }
        }

        private void CheckSecurity()
        {
            if (!ModuleContext.PortalSettings.UserInfo.IsSuperUser)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
        }

        private void CreateInstaller()
        {
            CheckSecurity();
            _Installer = new Installer(TempInstallFolder, ManifestFile, Request.MapPath("."), false);
            if (!ModuleContext.PortalSettings.UserInfo.IsSuperUser)
            {
                if (ModuleContext.PortalSettings.UserInfo.IsInRole(ModuleContext.PortalSettings.AdministratorRoleName))
                {
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Admin;
                }
                else if (ModulePermissionController.CanAdminModule(ModuleContext.Configuration))
                {
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Edit;
                }
                else if (ModulePermissionController.CanViewModule(ModuleContext.Configuration))
                {
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.View;
                }
                else
                {
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Anonymous;
                }
            }
            Installer.InstallerInfo.PortalID = InstallPortalId;
            if (Installer.InstallerInfo.ManifestFile != null)
            {
                Installer.ReadManifest(true);
            }
        }

        private void CreateManifest()
        {
            ManifestFile = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(FileName) + ".dnn");
            StreamWriter manifestWriter = new StreamWriter(ManifestFile);
            manifestWriter.Write(LegacyUtil.CreateSkinManifest(FileName, rblLegacySkin.SelectedValue, TempInstallFolder));
            manifestWriter.Close();
        }

        private void InstallPackage(WizardNavigationEventArgs e)
        {
            CreateInstaller();
            if (Installer.IsValid)
            {
                Installer.InstallerInfo.Log.Logs.Clear();
                Installer.InstallerInfo.IgnoreWhiteList = chkIgnoreWhiteList.Checked;
                Installer.InstallerInfo.RepairInstall = chkRepairInstall.Checked;
                Installer.Install();
                if (!Installer.IsValid)
                {
                    lblInstallMessage.Text = Localization.GetString("InstallError", LocalResourceFile);
                }
                phInstallLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
            }
            else
            {
                switch (e.CurrentStepIndex)
                {
                    case 3:
                        lblAcceptMessage.Text = Localization.GetString("InstallError", LocalResourceFile);
                        lblAcceptMessage.Visible = true;
                        phAcceptLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
                        break;
                    case 4:
                        lblInstallMessage.Text = Localization.GetString("InstallError", LocalResourceFile);
                        phInstallLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
                        break;
                }
                e.Cancel = true;
            }
        }

        private bool ValidatePackage()
        {
            bool isValid = Null.NullBoolean;
            CreateInstaller();
            if (Installer.InstallerInfo.ManifestFile != null)
            {
                ManifestFile = Path.GetFileName(Installer.InstallerInfo.ManifestFile.TempFileName);
            }
            if (string.IsNullOrEmpty(ManifestFile))
            {
                if (rblLegacySkin.SelectedValue != "None")
                {
                    CreateManifest();
                    isValid = ValidatePackage();
                }
                else
                {
                    lblWarningMessage.Visible = true;
                    pnlRepair.Visible = false;
                    pnlWhitelist.Visible = false;
                    pnlLegacy.Visible = true;
                    lblWarningMessage.Text = Localization.GetString("NoManifest", LocalResourceFile);
                }
            }
            else if (Installer == null)
            {
                lblWarningMessage.Visible = true;
                pnlRepair.Visible = false;
                pnlWhitelist.Visible = false;
                pnlLegacy.Visible = false;
                lblWarningMessage.Text = Localization.GetString("ZipCriticalError", LocalResourceFile);
            }
            else if (!Installer.IsValid)
            {
                lblWarningMessage.Visible = true;
                pnlRepair.Visible = false;
                pnlWhitelist.Visible = false;
                pnlLegacy.Visible = false;
                lblWarningMessage.Text = Localization.GetString("ZipError", LocalResourceFile);
                phLoadLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable());
            }
            else if (!string.IsNullOrEmpty(Installer.InstallerInfo.LegacyError))
            {
                lblWarningMessage.Visible = true;
                pnlRepair.Visible = false;
                pnlWhitelist.Visible = false;
                pnlLegacy.Visible = false;
                lblWarningMessage.Text = Localization.GetString(Installer.InstallerInfo.LegacyError, LocalResourceFile);
            }
            else if (!Installer.InstallerInfo.HasValidFiles && !chkIgnoreWhiteList.Checked)
            {
                lblWarningMessage.Visible = true;
                pnlRepair.Visible = false;
                pnlWhitelist.Visible = true;
                pnlLegacy.Visible = false;
                lblWarningMessage.Text = string.Format(Localization.GetString("InvalidFiles", LocalResourceFile), Installer.InstallerInfo.InvalidFileExtensions);
            }
            else if (Installer.InstallerInfo.Installed && !chkRepairInstall.Checked)
            {
                lblWarningMessage.Visible = true;
                if (Installer.InstallerInfo.PortalID == InstallPortalId)
                {
                    pnlRepair.Visible = true;
                }
                pnlWhitelist.Visible = false;
                pnlLegacy.Visible = false;
                lblWarningMessage.Text = Localization.GetString("PackageInstalled", LocalResourceFile);
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }

        protected string GetText(string type)
        {
            string text = Null.NullString;
            if (type == "Title")
            {
                text = Localization.GetString(wizInstall.ActiveStep.Title + ".Title", LocalResourceFile);
            }
            else if (type == "Help")
            {
                text = Localization.GetString(wizInstall.ActiveStep.Title + ".Help", LocalResourceFile);
            }
            return text;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chkIgnoreWhiteList.CheckedChanged += chkIgnoreRestrictedFiles_CheckedChanged;
            chkRepairInstall.CheckedChanged += chkRepairInstall_CheckedChanged;
            wizInstall.ActiveStepChanged += wizInstall_ActiveStepChanged;
            wizInstall.CancelButtonClick += wizInstall_CancelButtonClick;
            wizInstall.NextButtonClick += wizInstall_NextButtonClick;
            wizInstall.FinishButtonClick += wizInstall_FinishButtonClick;

            try
            {
                CheckSecurity();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkIgnoreRestrictedFiles_CheckedChanged(object sender, EventArgs e)
        {
            if ((chkIgnoreWhiteList.Checked))
            {
                lblWarningMessage.Text = Localization.GetString("IgnoreRestrictedFilesWarning", LocalResourceFile);
            }
            else
            {
                lblWarningMessage.Text = "";
            }
        }

        protected void chkRepairInstall_CheckedChanged(object sender, EventArgs e)
        {
            if ((chkRepairInstall.Checked))
            {
                lblWarningMessage.Text = Localization.GetString("RepairInstallWarning", LocalResourceFile);
            }
            else
            {
                lblWarningMessage.Text = Localization.GetString("PackageInstalled", LocalResourceFile);
            }
        }

        protected void wizInstall_ActiveStepChanged(object sender, EventArgs e)
        {
            switch (wizInstall.ActiveStepIndex)
            {
                case 1:
                    if (ValidatePackage())
                    {
                        wizInstall.ActiveStepIndex = 2;
                    }
                    break;
                case 2:
                case 3:
                case 4:
                    BindPackage();
                    break;
                case 5:
                    wizInstall.DisplayCancelButton = false;
                    break;
            }
        }

        protected void wizInstall_CancelButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(TempInstallFolder) && Directory.Exists(TempInstallFolder))
                {
                    Directory.Delete(TempInstallFolder, true);
                }
                Response.Redirect(ReturnURL, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void wizInstall_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                if (Installer != null)
                {
                    Installer.DeleteTempFolder();
                }
                Response.Redirect(ReturnURL, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void wizInstall_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            switch (e.CurrentStepIndex)
            {
                case 0:
                    HttpPostedFile postedFile = cmdBrowse.PostedFile;
                    string strMessage = "";
                    FileName = Path.GetFileName(postedFile.FileName);
                    string strExtension = Path.GetExtension(FileName);
                    if (string.IsNullOrEmpty(postedFile.FileName))
                    {
                        strMessage = Localization.GetString("NoFile", LocalResourceFile);
                    }
                    else if (strExtension.ToLower() != ".zip")
                    {
                        strMessage += string.Format(Localization.GetString("InvalidExt", LocalResourceFile), FileName);
                    }
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        _Installer = new Installer(postedFile.InputStream, Request.MapPath("."), true, false);
                        TempInstallFolder = Installer.TempInstallFolder;
                        if (Installer.InstallerInfo.ManifestFile != null)
                        {
                            ManifestFile = Path.GetFileName(Installer.InstallerInfo.ManifestFile.TempFileName);
                        }
                    }
                    else
                    {
                        lblLoadMessage.Text = strMessage;
                        lblLoadMessage.Visible = true;
                        e.Cancel = true;
                    }
                    break;
                case 1:
                    e.Cancel = !ValidatePackage();
                    break;
                case 4:
                    if (chkAcceptLicense.Checked)
                    {
                        InstallPackage(e);
                    }
                    else
                    {
                        lblAcceptMessage.Text = Localization.GetString("AcceptTerms", LocalResourceFile);
                        lblAcceptMessage.Visible = true;
                        e.Cancel = true;
                        BindPackage();
                    }
                    break;
            }
        }
    }
}