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
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

using DataCache = DotNetNuke.Common.Utilities.DataCache;
using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Services.Install
{
    public partial class InstallWizard : PageBase, IClientAPICallbackEventHandler
    {
        private readonly DataProvider dataProvider = DataProvider.Instance();
        protected new string LocalResourceFile = "~/Install/App_LocalResources/InstallWizard.aspx.resx";
        private Version _DataBaseVersion;
        private XmlDocument _installTemplate;
        private string _localesFile = "/Install/App_LocalResources/Locales.xml";
        private string connectionString = Null.NullString;

        protected Version ApplicationVersion
        {
            get
            {
                return DotNetNukeContext.Current.Application.Version;
            }
        }

        protected Version DatabaseVersion
        {
            get
            {
                if (_DataBaseVersion == null)
                {
                    _DataBaseVersion = DataProvider.Instance().GetVersion();
                }
                return _DataBaseVersion;
            }
        }

        protected Version BaseVersion
        {
            get
            {
                return Upgrade.Upgrade.GetInstallVersion(InstallTemplate);
            }
        }

        protected XmlDocument InstallTemplate
        {
            get
            {
                if (_installTemplate == null)
                {
                    _installTemplate = new XmlDocument();
                    Upgrade.Upgrade.GetInstallTemplate(_installTemplate);
                }
                return _installTemplate;
            }
        }

        protected bool PermissionsValid
        {
            get
            {
                bool _Valid = false;
                if (ViewState["PermissionsValid"] != null)
                {
                    _Valid = Convert.ToBoolean(ViewState["PermissionsValid"]);
                }
                return _Valid;
            }
            set
            {
                ViewState["PermissionsValid"] = value;
            }
        }

        protected int PortalId
        {
            get
            {
                int _PortalId = Null.NullInteger;
                if (ViewState["PortalId"] != null)
                {
                    _PortalId = Convert.ToInt32(ViewState["PortalId"]);
                }
                return _PortalId;
            }
            set
            {
                ViewState["PortalId"] = value;
            }
        }

        protected string Versions
        {
            get
            {
                string _Versions = Null.NullString;
                if (ViewState["Versions"] != null)
                {
                    _Versions = Convert.ToString(ViewState["Versions"]);
                }
                return _Versions;
            }
            set
            {
                ViewState["Versions"] = value;
            }
        }

        #region IClientAPICallbackEventHandler Members

        public string RaiseClientAPICallbackEvent(string eventArgument)
        {
            return ProcessAction(eventArgument);
        }

        #endregion

        private void BindAuthSystems()
        {
            BindPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "NoAuthSystems", lblAuthSystemsError);
        }

        private void BindConnectionString()
        {
            string connection = Config.GetConnectionString();
            string[] connectionParams = connection.Split(';');
            foreach (string connectionParam in connection.Split(';'))
            {
                int index = connectionParam.IndexOf("=");
                if (index > 0)
                {
                    string key = connectionParam.Substring(0, index);
                    string value = connectionParam.Substring(index + 1);
                    switch (key.ToLower())
                    {
                        case "server":
                        case "data source":
                        case "address":
                        case "addr":
                        case "network address":
                            txtServer.Text = value;
                            break;
                        case "database":
                        case "initial catalog":
                            txtDatabase.Text = value;
                            break;
                        case "uid":
                        case "user id":
                        case "user":
                            txtUserId.Text = value;
                            break;
                        case "pwd":
                        case "password":
                            txtPassword.Text = value;
                            break;
                        case "integrated security":
                            chkIntegrated.Checked = (value.ToLower() == "true");
                            break;
                        case "attachdbfilename":
                            txtFile.Text = value.Replace("|DataDirectory|", "");
                            break;
                    }
                }
            }
            if (chkIntegrated.Checked)
            {
                chkOwner.Checked = true;
            }
            chkOwner.Enabled = !chkIntegrated.Checked;
        }

        private string GetUpgradeConnectionStringUserID()
        {
            string dbUser = "";
            string connection = Config.GetUpgradeConnectionString();
            string[] connectionParams;
            if (connection.ToLower().Contains("user id") || connection.ToLower().Contains("uid") || connection.ToLower().Contains("user"))
            {
                connectionParams = connection.Split(';');
                foreach (string connectionParam in connectionParams)
                {
                    int index = connectionParam.IndexOf("=");
                    if (index > 0)
                    {
                        string key = connectionParam.Substring(0, index);
                        string value = connectionParam.Substring(index + 1);
                        if ("user id|uuid|user".Contains(key.Trim().ToLower()))
                        {
                            dbUser = value.Trim();
                        }
                    }
                }
            }
            return dbUser;
        }

        private void BindDatabases()
        {
            if ((Config.GetDefaultProvider("data").Name == "SqlDataProvider"))
            {
                string connection = Config.GetConnectionString();
                if (connection.ToLower().Contains("attachdbfilename"))
                {
                    rblDatabases.Items.FindByValue("SQLFile").Selected = true;
                }
                else
                {
                    rblDatabases.Items.FindByValue("SQLDatabase").Selected = true;
                }
            }
            if ((Config.GetDefaultProvider("data").Name == "OracleDataProvider"))
            {
                rblDatabases.Items.Add(new ListItem(LocalizeString("Oracle"), "Oracle"));
                rblDatabases.SelectedIndex = 2;
            }
        }

        private void BindLanguages()
        {
            BindPackageItems("Language", lstLanguages, lblNoLanguages, "NoLanguages", lblLanguagesError);
        }

        private void BindModules()
        {
            BindPackageItems("Module", lstModules, lblNoModules, "NoModules", lblModulesError);
        }

        private void BindPackageItems(string packageType, CheckBoxList list, Label noItemsLabel, string noItemsKey, Label errorLabel)
        {
            string[] arrFiles;
            string InstallPath = Globals.ApplicationMapPath + "\\Install\\" + packageType;
            list.Items.Clear();
            if (Directory.Exists(InstallPath))
            {
                arrFiles = Directory.GetFiles(InstallPath);
                int iFile = 0;
                foreach (string strFile in arrFiles)
                {
                    string strResource = strFile.Replace(InstallPath + "\\", "");
                    if (strResource.ToLower().EndsWith(".zip") || strResource.ToLower().EndsWith(".resources"))
                    {
                        var packageItem = new ListItem();
                        if (strResource.ToLower().EndsWith(".zip"))
                        {
                            packageItem.Selected = true;
                            packageItem.Enabled = false;
                        }
                        else
                        {
                            packageItem.Selected = false;
                            packageItem.Enabled = true;
                        }
                        packageItem.Value = strResource;
                        strResource = strResource.Replace(".zip", "");
                        strResource = strResource.Replace(".resources", "");
                        strResource = strResource.Replace("_Install", ")");
                        strResource = strResource.Replace("_Source", ")");
                        strResource = strResource.Replace("_", " (");
                        packageItem.Text = strResource;
                        list.Items.Add(packageItem);
                    }
                }
            }
            if (list.Items.Count > 0)
            {
                noItemsLabel.Visible = false;
            }
            else
            {
                noItemsLabel.Visible = true;
                noItemsLabel.Text = LocalizeString(noItemsKey);
            }
            if (errorLabel != null)
            {
                errorLabel.Text = Null.NullString;
            }
        }

        private void BindPermissions(bool test)
        {
            PermissionsValid = true;
            lstPermissions.Items.Clear();
            var permissionItem = new ListItem();
            var verifier = new FileSystemPermissionVerifier(Server.MapPath("~"));
            if (test)
            {
                permissionItem.Selected = verifier.VerifyFolderCreate();
                PermissionsValid = PermissionsValid && permissionItem.Selected;
            }
            permissionItem.Enabled = false;
            permissionItem.Text = LocalizeString("FolderCreate");
            lstPermissions.Items.Add(permissionItem);
            permissionItem = new ListItem();
            if (test)
            {
                permissionItem.Selected = verifier.VerifyFileCreate();
                PermissionsValid = PermissionsValid && permissionItem.Selected;
            }
            permissionItem.Enabled = false;
            permissionItem.Text = LocalizeString("FileCreate");
            lstPermissions.Items.Add(permissionItem);
            permissionItem = new ListItem();
            if (test)
            {
                permissionItem.Selected = verifier.VerifyFileDelete();
                PermissionsValid = PermissionsValid && permissionItem.Selected;
            }
            permissionItem.Enabled = false;
            permissionItem.Text = LocalizeString("FileDelete");
            lstPermissions.Items.Add(permissionItem);
            permissionItem = new ListItem();
            if (test)
            {
                permissionItem.Selected = verifier.VerifyFolderDelete();
                PermissionsValid = PermissionsValid && permissionItem.Selected;
            }
            permissionItem.Enabled = false;
            permissionItem.Text = LocalizeString("FolderDelete");
            lstPermissions.Items.Add(permissionItem);
            if (test)
            {
                if (PermissionsValid)
                {
                    lblPermissionsError.Text = LocalizeString("PermissionsOk");
                }
                else
                {
                    lblPermissionsError.Text = LocalizeString("PermissionsError").Replace("{0}", Globals.ApplicationMapPath);
                }
            }
        }

        private void BindPortal()
        {
            XmlNode node = InstallTemplate.SelectSingleNode("//dotnetnuke/portals/portal");
            if (node != null)
            {
                XmlNode adminNode = node.SelectSingleNode("administrator");
                usrAdmin.FirstName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "firstname");
                usrAdmin.LastName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "lastname");
                usrAdmin.UserName = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "username");
                usrAdmin.Email = XmlUtils.GetNodeValue(adminNode.CreateNavigator(), "email");
                txtPortalTitle.Text = XmlUtils.GetNodeValue(node.CreateNavigator(), "portalname");
                string strTemplate = XmlUtils.GetNodeValue(node.CreateNavigator(), "templatefile");
                string strFolder = Globals.HostMapPath;
                if (Directory.Exists(strFolder))
                {
                    cboPortalTemplate.Items.Clear();
                    string[] fileEntries = Directory.GetFiles(strFolder, "*.template");
                    foreach (string strFileName in fileEntries)
                    {
                        if (Path.GetFileNameWithoutExtension(strFileName) == "admin")
                        {
                        }
                        else
                        {
                            cboPortalTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName));
                        }
                    }
                    if (cboPortalTemplate.Items.Count == 0)
                    {
                    }
                    if (cboPortalTemplate.Items.FindByValue(strTemplate.Replace(".template", "")) != null)
                    {
                        cboPortalTemplate.Items.FindByValue(strTemplate.Replace(".template", "")).Selected = true;
                    }
                    else
                    {
                        cboPortalTemplate.SelectedIndex = 0;
                    }
                }
            }
            lblPortalError.Text = Null.NullString;
        }

        private void BindProviders()
        {
            BindPackageItems("Provider", lstProviders, lblNoProviders, "NoProviders", lblProvidersError);
        }

        private void BindSkins()
        {
            BindPackageItems("Skin", lstSkins, lblNoSkins, "NoSkins", lblSkinsError);
            BindPackageItems("Container", lstContainers, lblNoContainers, "NoContainers", null);
        }

        private void BindSuperUser(LinkButton customButton)
        {
            UserInfo superUser = Upgrade.Upgrade.GetSuperUser(InstallTemplate, false);
            if (superUser != null)
            {
                usrHost.FirstName = superUser.FirstName;
                usrHost.LastName = superUser.LastName;
                usrHost.UserName = superUser.Username;
                usrHost.Email = superUser.Email;
            }
            //ShowButton(customButton, true);
        }

        private void EnableButton(LinkButton button, bool enabled)
        {
            if (button != null)
            {
                button.OnClientClick = "return !checkDisabled(this);";
                if (enabled)
                {
                    button.CssClass = "WizardButton";
                }
                else
                {
                    button.CssClass = "WizardButtonDisabled";
                }
            }
        }

        private ArrayList GetInstallerLocales()
        {
            var supportedLocales = new ArrayList();
            string filePath = Globals.ApplicationMapPath + _localesFile.Replace("/", "\\");
            if (File.Exists(filePath))
            {
                var doc = new XPathDocument(filePath);
                foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/language"))
                {
                    if (nav.NodeType != XPathNodeType.Comment)
                    {
                        var objLocale = new Locale();
                        objLocale.Text = nav.GetAttribute("name", "");
                        objLocale.Code = nav.GetAttribute("key", "");
                        objLocale.Fallback = nav.GetAttribute("fallback", "");
                        supportedLocales.Add(objLocale);
                    }
                }
            }
            else
            {
                var objLocale = new Locale();
                objLocale.Text = "English";
                objLocale.Code = "en-US";
                objLocale.Fallback = "";
                supportedLocales.Add(objLocale);
            }
            return supportedLocales;
        }

        private string GetNextScriptVersion(string strProviderPath, Version currentVersion)
        {
            string strNextVersion = "Done";
            if (currentVersion == null)
            {
                strNextVersion = GetBaseDatabaseVersion();
            }
            else
            {
                string strScriptVersion = Null.NullString;
                ArrayList arrScripts = Upgrade.Upgrade.GetUpgradeScripts(strProviderPath, currentVersion);
                if (arrScripts.Count > 0)
                {
                    strScriptVersion = Path.GetFileNameWithoutExtension(Convert.ToString(arrScripts[0]));
                }
                if (!string.IsNullOrEmpty(strScriptVersion))
                {
                    strNextVersion = strScriptVersion;
                }
            }
            return strNextVersion;
        }

        private Version GetVersion(string scriptFile)
        {
            return new Version(Path.GetFileNameWithoutExtension(scriptFile));
        }

        private LinkButton GetWizardButton(string containerID, string buttonID)
        {
            Control navContainer = wizInstall.FindControl(containerID);
            LinkButton button = null;
            if (navContainer != null)
            {
                button = navContainer.FindControl(buttonID) as LinkButton;
            }
            return button;
        }

        private void Initialise()
        {
            if (TestDataBaseInstalled())
            {
                Response.Redirect("~/Default.aspx", true);
            }
            else
            {
                if (DatabaseVersion > new Version(0, 0, 0))
                {
                    tblLanguage.Visible = false;
                    lblStep0Title.Text = string.Format(LocalizeString("UpgradeTitle"), ApplicationVersion.ToString(3));
                    lblStep0Detail.Text = string.Format(LocalizeString("Upgrade"), Upgrade.Upgrade.GetStringVersion(DatabaseVersion));
                }
                else
                {
                    UpdateMachineKey();
                }
            }
        }

        private bool InstallAuthSystems()
        {
            return InstallPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "InstallAuthSystemError");
        }

        private string InstallDatabase()
        {
            string strErrorMessage = Null.NullString;
            string strProviderPath = dataProvider.GetProviderPath();
            if (!strProviderPath.StartsWith("ERROR:"))
            {
                strErrorMessage = Upgrade.Upgrade.InstallDatabase(BaseVersion, strProviderPath, InstallTemplate, false);
            }
            else
            {
                strErrorMessage = strProviderPath;
            }
            if (string.IsNullOrEmpty(strErrorMessage))
            {
                strErrorMessage = GetNextScriptVersion(strProviderPath, BaseVersion);
            }
            else if (!strErrorMessage.StartsWith("ERROR:"))
            {
                strErrorMessage = "ERROR: " + string.Format(LocalizeString("ScriptError"), Upgrade.Upgrade.GetLogFile(strProviderPath, BaseVersion));
            }
            return strErrorMessage;
        }

        private bool InstallHost()
        {
            bool success = false;
            string strErrorMessage = usrHost.Validate();
            if (!string.IsNullOrEmpty(strErrorMessage))
            {
                string strError = LocalizeString(strErrorMessage);
                if (strErrorMessage == "PasswordLength")
                {
                    strError = string.Format(strError, MembershipProviderConfig.MinPasswordLength);
                }
                lblHostUserError.Text = string.Format(LocalizeString("HostUserError"), strError);
            }
            else
            {
                try
                {
                    Upgrade.Upgrade.InitialiseHostSettings(InstallTemplate, false);
                    UserInfo objSuperUserInfo = Upgrade.Upgrade.GetSuperUser(InstallTemplate, false);
                    objSuperUserInfo.FirstName = usrHost.FirstName;
                    objSuperUserInfo.LastName = usrHost.LastName;
                    objSuperUserInfo.Username = usrHost.UserName;
                    objSuperUserInfo.DisplayName = usrHost.FirstName + " " + usrHost.LastName;
                    objSuperUserInfo.Membership.Password = usrHost.Password;
                    objSuperUserInfo.Email = usrHost.Email;
                    UserController.CreateUser(ref objSuperUserInfo);
                    Upgrade.Upgrade.InstallFiles(InstallTemplate, false);
                    if (!string.IsNullOrEmpty(txtSMTPServer.Text))
                    {
                        HostController.Instance.Update("SMTPServer", txtSMTPServer.Text);
                        HostController.Instance.Update("SMTPAuthentication", optSMTPAuthentication.SelectedItem.Value);
                        HostController.Instance.Update("SMTPUsername", txtSMTPUsername.Text, true);
                        HostController.Instance.Update("SMTPPassword", txtSMTPPassword.Text, true);
                        HostController.Instance.Update("SMTPEnableSSL", chkSMTPEnableSSL.Checked ? "Y" : "N");
                    }
                    DataCache.ClearHostCache(false);
                    success = true;
                }
                catch (Exception ex)
                {
                    Instrumentation.DnnLog.Error(ex);
                    lblHostUserError.Text = string.Format(LocalizeString("HostUserError"), ex.Message);
                }
            }
            return success;
        }

        private bool InstallLanguages()
        {
            return InstallPackageItems("Language", lstLanguages, lblLanguagesError, "InstallLanguageError");
        }

        private bool InstallModules()
        {
            return InstallPackageItems("Module", lstModules, lblModulesError, "InstallModuleError");
        }

        private bool InstallPackageItems(string packageType, CheckBoxList list, Label errorLabel, string errorKey)
        {
            bool success = false;
            string strErrorMessage = Null.NullString;
            int scriptTimeOut = Server.ScriptTimeout;
            try
            {
                Server.ScriptTimeout = int.MaxValue;
                string InstallPath = Globals.ApplicationMapPath + "\\Install\\" + packageType;
                foreach (ListItem packageItem in list.Items)
                {
                    if (packageItem.Selected)
                    {
                        if ((File.Exists(InstallPath + "\\" + packageItem.Value)))
                        {
                            success = Upgrade.Upgrade.InstallPackage(InstallPath + "\\" + packageItem.Value, packageType, false);
                            if (!success)
                            {
                                strErrorMessage += string.Format(LocalizeString(errorKey), packageItem.Text);
                            }
                        }
                    }
                }
                success = string.IsNullOrEmpty(strErrorMessage);
            }
            catch (Exception ex)
            {
                Instrumentation.DnnLog.Error(ex);
                strErrorMessage = ex.StackTrace;
            }
            finally
            {
                Server.ScriptTimeout = scriptTimeOut;
            }
            if (!success)
            {
                errorLabel.Text += strErrorMessage;
            }
            return success;
        }

        private bool InstallPortal()
        {
            bool success = false;
            string strErrorMessage = usrAdmin.Validate();
            if (!string.IsNullOrEmpty(strErrorMessage))
            {
                string strError = LocalizeString(strErrorMessage);
                if (strErrorMessage == "PasswordLength")
                {
                    strError = string.Format(strError, MembershipProviderConfig.MinPasswordLength);
                }
                lblPortalError.Text = string.Format(LocalizeString("AdminUserError"), strError);
            }
            else
            {
                try
                {
                    var objPortalController = new PortalController();
                    string strServerPath = Globals.ApplicationMapPath + "\\";
                    string strPortalAlias = Globals.GetDomainName(HttpContext.Current.Request, true).Replace("/Install", "");
                    string strTemplate = cboPortalTemplate.SelectedValue + ".template";
                    PortalId = objPortalController.CreatePortal(txtPortalTitle.Text,
                                                                usrAdmin.FirstName,
                                                                usrAdmin.LastName,
                                                                usrAdmin.UserName,
                                                                usrAdmin.Password,
                                                                usrAdmin.Email,
                                                                "",
                                                                "",
                                                                Globals.HostMapPath,
                                                                strTemplate,
                                                                "",
                                                                strPortalAlias,
                                                                strServerPath,
                                                                "",
                                                                false);
                    success = (PortalId > Null.NullInteger);
                }
                catch (Exception ex)
                {
                    Instrumentation.DnnLog.Error(ex);
                    success = false;
                    strErrorMessage = ex.Message;
                }
                if (!success)
                {
                    lblPortalError.Text = string.Format(LocalizeString("InstallPortalError"), strErrorMessage);
                }
            }
            return success;
        }

        private bool InstallProviders()
        {
            return InstallPackageItems("Provider", lstProviders, lblProvidersError, "InstallProviderError");
        }

        private bool InstallSkins()
        {
            bool skinSuccess = InstallPackageItems("Skin", lstSkins, lblSkinsError, "InstallSkinError");
            bool containerSuccess = InstallPackageItems("Container", lstContainers, lblSkinsError, "InstallContainerError");
            return skinSuccess && containerSuccess;
        }

        private string InstallVersion(string strVersion)
        {
            string strErrorMessage = Null.NullString;
            var version = new Version(strVersion);
            string strScriptFile = Null.NullString;
            string strProviderPath = dataProvider.GetProviderPath();
            if (!strProviderPath.StartsWith("ERROR:"))
            {
                strScriptFile = Upgrade.Upgrade.GetScriptFile(strProviderPath, version);
                strErrorMessage += Upgrade.Upgrade.UpgradeVersion(strScriptFile, false);
                Versions += "," + strVersion;
            }
            else
            {
                strErrorMessage = strProviderPath;
            }
            if (string.IsNullOrEmpty(strErrorMessage))
            {
                strErrorMessage = GetNextScriptVersion(strProviderPath, version);
            }
            else
            {
                strErrorMessage = "ERROR: (see " + Path.GetFileName(strScriptFile).Replace("." + Upgrade.Upgrade.DefaultProvider, ".log") + " for more information)";
            }
            return strErrorMessage;
        }

        private void LocalizePage()
        {
            Title = LocalizeString("Title") + " - " + LocalizeString("Page" + wizInstall.ActiveStepIndex + ".Title");
            for (int i = 0; i <= wizInstall.WizardSteps.Count - 1; i++)
            {
                wizInstall.WizardSteps[i].Title = LocalizeString("Page" + i + ".Title");
            }
            wizInstall.StartNextButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + LocalizeString("Next");
            wizInstall.FinishPreviousButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + LocalizeString("Previous");
            wizInstall.FinishCompleteButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + LocalizeString("Finished");
            lblStep0Title.Text = string.Format(LocalizeString("IntroTitle"), Globals.FormatVersion(ApplicationVersion));
            lblStep0Detail.Text = LocalizeString("IntroDetail");
            lblChooseInstall.Text = LocalizeString("ChooseInstall");
            lblChooseLanguage.Text = LocalizeString("ChooseLanguage");
            rblInstall.Items[0].Text = LocalizeString("Full");
            rblInstall.Items[1].Text = LocalizeString("Typical");
            rblInstall.Items[2].Text = LocalizeString("Auto");
            lblStep1Title.Text = LocalizeString("PermissionsTitle");
            lblStep1Detail.Text = LocalizeString("PermissionsDetail");
            lblPermissions.Text = LocalizeString("Permissions");
            BindPermissions(false);
            lblStep2Title.Text = LocalizeString("DatabaseConfigTitle");
            lblStep2Detail.Text = LocalizeString("DatabaseConfigDetail");
            lblChooseDatabase.Text = LocalizeString("ChooseDatabase");
            lblServerHelp.Text = LocalizeString("ServerHelp");
            lblServer.Text = LocalizeString("Server");
            lblFile.Text = LocalizeString("DatabaseFile");
            lblDatabaseFileHelp.Text = LocalizeString("DatabaseFileHelp");
            lblDataBase.Text = LocalizeString("Database");
            lblDatabaseHelp.Text = LocalizeString("DatabaseHelp");
            lblIntegrated.Text = LocalizeString("Integrated");
            lblIntegratedHelp.Text = LocalizeString("IntegratedHelp");
            lblUserId.Text = LocalizeString("UserId");
            lblUserHelp.Text = LocalizeString("UserHelp");
            lblPassword.Text = LocalizeString("Password");
            lblPasswordHelp.Text = LocalizeString("PasswordHelp");
            lblOwner.Text = LocalizeString("Owner");
            lblOwnerHelp.Text = LocalizeString("OwnerHelp");
            lblQualifier.Text = LocalizeString("Qualifier");
            lblQualifierHelp.Text = LocalizeString("QualifierHelp");
            rblDatabases.Items[0].Text = LocalizeString("SQLServerXPress");
            rblDatabases.Items[1].Text = LocalizeString("SQLServer");
            lblStep3Title.Text = LocalizeString("DatabaseInstallTitle");
            lblStep3Detail.Text = LocalizeString("DatabaseInstallDetail");
            lblStep4Title.Text = LocalizeString("HostUserTitle");
            lblStep4Detail.Text = LocalizeString("HostUserDetail");
            usrHost.FirstNameLabel = LocalizeString("FirstName");
            usrHost.LastNameLabel = LocalizeString("LastName");
            usrHost.UserNameLabel = LocalizeString("UserName");
            usrHost.PasswordLabel = LocalizeString("Password");
            usrHost.ConfirmLabel = LocalizeString("Confirm");
            usrHost.EmailLabel = LocalizeString("Email");
            lblSMTPSettings.Text = LocalizeString("SMTPSettings");
            lblSMTPSettingsHelp.Text = LocalizeString("SMTPSettingsHelp");
            lblSMTPServer.Text = LocalizeString("SMTPServer");
            lblSMTPAuthentication.Text = LocalizeString("SMTPAuthentication");
            lblSMTPEnableSSL.Text = LocalizeString("SMTPEnableSSL");
            lblSMTPUsername.Text = LocalizeString("SMTPUsername");
            lblSMTPPassword.Text = LocalizeString("SMTPPassword");
            lblStep5Title.Text = LocalizeString("ModulesTitle");
            lblStep5Detail.Text = LocalizeString("ModulesDetail");
            lblModules.Text = LocalizeString("Modules");
            lblStep6Title.Text = LocalizeString("SkinsTitle");
            lblStep6Detail.Text = LocalizeString("SkinsDetail");
            lblSkins.Text = LocalizeString("Skins");
            lblContainers.Text = LocalizeString("Containers");
            lblStep7Title.Text = LocalizeString("LanguagesTitle");
            lblStep7Detail.Text = LocalizeString("LanguagesDetail");
            lblLanguages.Text = LocalizeString("Languages");
            lblStep8Title.Text = LocalizeString("AuthSystemsTitle");
            lblStep8Detail.Text = LocalizeString("AuthSystemsDetail");
            lblAuthSystems.Text = LocalizeString("AuthSystems");
            lblStep9Title.Text = LocalizeString("ProvidersTitle");
            lblStep9Detail.Text = LocalizeString("ProvidersDetail");
            lblProviders.Text = LocalizeString("Providers");
            lblStep10Title.Text = LocalizeString("PortalTitle");
            lblStep10Detail.Text = LocalizeString("PortalDetail");
            usrAdmin.FirstNameLabel = LocalizeString("FirstName");
            usrAdmin.LastNameLabel = LocalizeString("LastName");
            usrAdmin.UserNameLabel = LocalizeString("UserName");
            usrAdmin.PasswordLabel = LocalizeString("Password");
            usrAdmin.ConfirmLabel = LocalizeString("Confirm");
            usrAdmin.EmailLabel = LocalizeString("Email");
            lblAdminUser.Text = LocalizeString("AdminUser");
            lblPortal.Text = LocalizeString("Portal");
            lblPortalTitle.Text = LocalizeString("PortalTitle");
            lblPortalTemplate.Text = LocalizeString("PortalTemplate");
            lblCompleteTitle.Text = LocalizeString("CompleteTitle");
            lblCompleteDetail.Text = LocalizeString("CompleteDetail");
        }

        private void SetupDatabasePage()
        {
            LinkButton nextButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton");
            LinkButton customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
            SetupDatabasePage(customButton, nextButton);
        }

        private void SetupDatabasePage(LinkButton customButton, LinkButton nextButton)
        {
            if (rblDatabases.SelectedIndex > Null.NullInteger)
            {
                bool isSQLFile = (rblDatabases.SelectedValue == "SQLFile");
                bool isSQLDb = (rblDatabases.SelectedValue == "SQLDatabase");
                bool isOracle = (rblDatabases.SelectedValue == "Oracle");
                tblDatabase.Visible = true;
                trFile.Visible = isSQLFile;
                trDatabase.Visible = isSQLDb;
                trIntegrated.Visible = !isOracle;
                trUser.Visible = !chkIntegrated.Checked || isOracle;
                trPassword.Visible = !chkIntegrated.Checked || isOracle;
                if (isSQLDb)
                {
                    chkOwner.Enabled = true;
                }
                else
                {
                    chkOwner.Enabled = false;
                }
                chkOwner.Checked = (Config.GetDataBaseOwner() == "dbo.");
                txtqualifier.Text = Config.GetObjectQualifer();
            }
            else
            {
                tblDatabase.Visible = false;
            }
        }

        private void SetupPage()
        {
            LinkButton nextButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton");
            LinkButton prevButton = GetWizardButton("StepNavigationTemplateContainerID", "StepPreviousButton");
            LinkButton customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
            EnableButton(nextButton, true);
            EnableButton(prevButton, true);
            ShowButton(customButton, false);
            switch (wizInstall.ActiveStepIndex)
            {
                case 0:
                    lblPermissionsError.Text = "";
                    break;
                case 1:
                    lblDataBaseError.Text = "";
                    break;
                case 2:
                    lblPermissionsError.Text = "";
                    SetupDatabasePage(customButton, nextButton);
                    break;
                case 3:
                    lblDataBaseError.Text = "";
                    lblInstallError.Text = "";
                    lblInstallError.Visible = false;
                    EnableButton(nextButton, false);
                    ShowButton(prevButton, false);
                    break;
                case 4:
                    BindSuperUser(customButton);
                    ShowButton(prevButton, false);
                    break;
                case 5:
                    BindModules();
                    ShowButton(prevButton, false);
                    break;
                case 6:
                    BindSkins();
                    ShowButton(prevButton, false);
                    break;
                case 7:
                    BindLanguages();
                    ShowButton(prevButton, false);
                    break;
                case 8:
                    BindAuthSystems();
                    ShowButton(prevButton, false);
                    break;
                case 9:
                    BindProviders();
                    ShowButton(prevButton, false);
                    break;
                case 10:
                    BindPortal();
                    ShowButton(prevButton, false);
                    break;
            }
        }

        private void ShowButton(LinkButton button, bool enabled)
        {
            if (button != null)
            {
                button.Visible = enabled;
            }
        }

        private void ShowHideSMTPCredentials()
        {
            if (optSMTPAuthentication.SelectedValue == "1")
            {
                trSMTPPassword.Visible = true;
                trSMTPUserName.Visible = true;
            }
            else
            {
                trSMTPPassword.Visible = false;
                trSMTPUserName.Visible = false;
            }
        }

        private bool TestDatabaseConnection()
        {
            bool success = false;
            if (string.IsNullOrEmpty(rblDatabases.SelectedValue))
            {
                connectionString = "ERROR:" + LocalizeString("ChooseDbError");
            }
            else
            {
                bool isSQLFile = (rblDatabases.SelectedValue == "SQLFile");
                DbConnectionStringBuilder builder = dataProvider.GetConnectionStringBuilder();
                if (!string.IsNullOrEmpty(txtServer.Text))
                {
                    builder["Data Source"] = txtServer.Text;
                }
                if (!string.IsNullOrEmpty(txtDatabase.Text) && !isSQLFile)
                {
                    builder["Initial Catalog"] = txtDatabase.Text;
                }
                if (String.IsNullOrEmpty(txtDatabase.Text) && !isSQLFile)
                {
                    lblDataBaseError.Text = LocalizeString("DbNameError");
                    return false;
                }
                if (!string.IsNullOrEmpty(txtFile.Text) && isSQLFile)
                {
                    builder["attachdbfilename"] = "|DataDirectory|" + txtFile.Text;
                }
                if (chkIntegrated.Checked)
                {
                    builder["integrated security"] = "true";
                }
                if (!string.IsNullOrEmpty(txtUserId.Text))
                {
                    builder["uid"] = txtUserId.Text;
                }
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    builder["pwd"] = txtPassword.Text;
                }
                if (isSQLFile)
                {
                    builder["user instance"] = "true";
                }
                string owner = txtUserId.Text + ".";
                if (chkOwner.Checked)
                {
                    owner = "dbo.";
                }
                connectionString = DataProvider.Instance().TestDatabaseConnection(builder, owner, txtqualifier.Text);
            }
            if (connectionString.StartsWith("ERROR:"))
            {
                lblDataBaseError.Text = string.Format(LocalizeString("ConnectError"), connectionString.Replace("ERROR:", ""));
            }
            else
            {
                success = true;
                lblDataBaseError.Text = LocalizeString("ConnectSuccess");
            }
            return success;
        }

        private bool TestDataBaseInstalled()
        {
            bool success = true;
            if (DatabaseVersion == null || DatabaseVersion.Major != ApplicationVersion.Major || DatabaseVersion.Minor != ApplicationVersion.Minor || DatabaseVersion.Build != ApplicationVersion.Build)
            {
                success = false;
            }
            if (!success)
            {
                lblInstallError.Text = LocalizeString("Install.Error");
            }
            return success;
        }

        private bool TestSMTPSettings()
        {
            if (!string.IsNullOrEmpty(usrHost.Email))
            {
                try
                {
                    var emailMessage = new MailMessage(usrHost.Email, usrHost.Email, LocalizeString("EmailTestMessageSubject"), string.Empty);

                    var smtpClient = new SmtpClient(txtSMTPServer.Text);

                    string[] smtpHostParts = txtSMTPServer.Text.Split(':');
                    if (smtpHostParts.Length > 1)
                    {
                        smtpClient.Host = smtpHostParts[0];
                        smtpClient.Port = Convert.ToInt32(smtpHostParts[1]);
                    }


                    switch (optSMTPAuthentication.SelectedItem.Value)
                    {
                        case "":
                        case "0":
                            // anonymous
                            break;
                        case "1":
                            // basic
                            if (!string.IsNullOrEmpty(txtSMTPUsername.Text) && !string.IsNullOrEmpty(txtSMTPPassword.Text))
                            {
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = new NetworkCredential(txtSMTPUsername.Text, txtSMTPPassword.Text);
                            }
                            break;
                        case "2":
                            // NTLM
                            smtpClient.UseDefaultCredentials = true;
                            break;
                    }

                    smtpClient.EnableSsl = chkSMTPEnableSSL.Checked;

                    smtpClient.Send(emailMessage);

                    lblHostUserError.Text = string.Format(LocalizeString("SMTPSuccess"), LocalizeString("EmailSentMessage"));

                    return true;
                }
                catch (Exception ex)
                {
                    Instrumentation.DnnLog.Error(ex);
                    lblHostUserError.Text = string.Format(LocalizeString("SMTPError"), string.Format(LocalizeString("EmailErrorMessage"), ex.Message));

                    return false;
                }
            }
            else
            {
                lblHostUserError.Text = string.Format(LocalizeString("SMTPError"), LocalizeString("SpecifyHostEmailMessage"));
                return false;
            }
        }

        private void UpdateMachineKey()
        {
            string installationDate = Config.GetSetting("InstallationDate");
            if (installationDate == null || String.IsNullOrEmpty(installationDate))
            {
                string strError = Config.UpdateMachineKey();
                if (String.IsNullOrEmpty(strError))
                {
                    Response.Redirect(HttpContext.Current.Request.RawUrl, true);
                }
                else
                {
                    string strURL = "~/ErrorPage.aspx?status=403_3&error=" + strError;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Server.Transfer(strURL);
                }
            }
        }

        protected string GetBaseDatabaseVersion()
        {
            return Upgrade.Upgrade.GetStringVersion(BaseVersion);
        }

        protected string LocalizeString(string key)
        {
            return Localization.Localization.GetString(key, LocalResourceFile, cboLanguages.SelectedValue.ToLower());
        }

        protected override void OnError(EventArgs e)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Server.Transfer("~/ErrorPage.aspx");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboLanguages.SelectedIndexChanged += cboLanguages_SelectedIndexChanged;
            chkIntegrated.CheckedChanged += chkIntegrated_CheckedChanged;
            optSMTPAuthentication.SelectedIndexChanged += optSMTPAuthentication_SelectedIndexChanged;
            rblDatabases.SelectedIndexChanged += rblDatabases_SelectedIndexChanged;
            wizInstall.ActiveStepChanged += wizInstall_ActiveStepChanged;
            wizInstall.FinishButtonClick += wizInstall_FinishButtonClick;
            wizInstall.NextButtonClick += wizInstall_NextButtonClick;
            LinkButton customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
            customButton.Click += wizInstall_CustomButtonClick;

            ClientAPI.HandleClientAPICallbackEvent(this);

            LinkButton button = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
            if (button != null)
            {
                button.Click += wizInstall_CustomButtonClick;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientAPI.RegisterClientVariable(Page, "ActionCallback", ClientAPI.GetCallbackEventReference(this, "[ACTIONTOKEN]", "successFunc", "this", "errorFunc"), true);
            lblHostWarning.Visible = !Regex.IsMatch(Request.Url.Host, "^([a-zA-Z0-9.-]+)$", RegexOptions.IgnoreCase);
            if (!Page.IsPostBack)
            {
                rblInstall.Items.Clear();
                rblInstall.Items.Add(new ListItem(LocalizeString("Full"), "Full"));
                rblInstall.Items.Add(new ListItem(LocalizeString("Typical"), "Typical"));
                rblInstall.Items.Add(new ListItem(LocalizeString("Auto"), "Auto"));
                rblInstall.SelectedIndex = 1;
                rblDatabases.Items.Clear();
                rblDatabases.Items.Add(new ListItem(LocalizeString("SQLServerXPress"), "SQLFile"));
                rblDatabases.Items.Add(new ListItem(LocalizeString("SQLServer"), "SQLDatabase"));
                BindConnectionString();
                BindDatabases();
                if (TestDatabaseConnection())
                {
                    Initialise();
                    rblInstall.Items[2].Enabled = true;
                    lblDataBaseWarning.Visible = false;
                }
                else
                {
                    UpdateMachineKey();
                    rblInstall.Items[2].Enabled = false;
                    lblDataBaseWarning.Visible = true;
                }
                cboLanguages.DataSource = GetInstallerLocales();
                cboLanguages.DataBind();
                LocalizePage();
                wizInstall.ActiveStepIndex = 0;
                SetupPage();
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            txtPassword.Attributes["value"] = txtPassword.Text;
            txtSMTPPassword.Attributes["value"] = txtSMTPPassword.Text;
        }

        protected void cboLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocalizePage();
        }

        protected void chkIntegrated_CheckedChanged(object sender, EventArgs e)
        {
            trUser.Visible = !chkIntegrated.Checked;
            trPassword.Visible = !chkIntegrated.Checked;
            if (chkIntegrated.Checked)
            {
                chkOwner.Checked = true;
            }
            chkOwner.Enabled = !chkIntegrated.Checked;
        }

        protected void optSMTPAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideSMTPCredentials();
        }

        public string ProcessAction(string someAction)
        {
            string strProviderPath = dataProvider.GetProviderPath();
            string nextVersion = GetNextScriptVersion(strProviderPath, DatabaseVersion);
            if (someAction != nextVersion)
            {
                Exceptions.Exceptions.LogException(new Exception("Attempt made to run a Database Script - Possible Attack"));
                return "Error: Possible Attack";
            }
            if (someAction == GetBaseDatabaseVersion())
            {
                string result = InstallDatabase();
                if (result == "Done")
                {
                    Upgrade.Upgrade.UpgradeApplication();
                }
                return result;
            }
            else if (someAction.Contains("."))
            {
                string result = InstallVersion(someAction);
                if (result == "Done")
                {
                    ArrayList arrVersions = Upgrade.Upgrade.GetUpgradeScripts(strProviderPath, BaseVersion);
                    string strErrorMessage = Null.NullString;
                    for (int i = 0; i <= arrVersions.Count - 1; i++)
                    {
                        string strVersion = Path.GetFileNameWithoutExtension(Convert.ToString(arrVersions[i]));
                        var version = new Version(strVersion);
                        if (version != null)
                        {
                            strErrorMessage += Upgrade.Upgrade.UpgradeApplication(strProviderPath, version, false);
                            strErrorMessage += Upgrade.Upgrade.DeleteFiles(strProviderPath, version, false);
                            strErrorMessage += Upgrade.Upgrade.UpdateConfig(strProviderPath, version, false);
                        }
                    }
                    Upgrade.Upgrade.UpgradeApplication();
                    if (!string.IsNullOrEmpty(strErrorMessage))
                    {
                        result = "ERROR: " + strErrorMessage;
                    }
                }
                return result;
            }
            else
            {
                return "Done";
            }
        }

        protected void rblDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindConnectionString();
            SetupDatabasePage();
        }

        protected void wizInstall_ActiveStepChanged(object sender, EventArgs e)
        {
            Title = LocalizeString("Title") + " - " + LocalizeString("Page" + wizInstall.ActiveStepIndex + ".Title");
            if (wizInstall.ActiveStepIndex > 0)
            {
                LinkButton nextButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton");
                LinkButton prevButton = GetWizardButton("StepNavigationTemplateContainerID", "StepPreviousButton");
                nextButton.Text = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + LocalizeString("Next");
                prevButton.Text = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + LocalizeString("Previous");
            }
            LinkButton customButton;
            switch (wizInstall.ActiveStepIndex)
            {
                case 1:
                    //Page 1 - File Permissions
                    BindPermissions(true);
                    break;
                case 2:
                    //customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
                    //customButton.Text = "<img src=\"" + Common.Globals.ApplicationPath + "/images/icon_sql_16px.gif\" border=\"0\" /> " + LocalizeString("TestDB");
                    break;
                case 4:
                    customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
                    customButton.Text = "<img src=\"" + Globals.ApplicationPath + "/images/icon_bulkmail_16px.gif\" border=\"0\" /> " + LocalizeString("TestSMTP");
                    ShowHideSMTPCredentials();
                    break;
                case 5:
                    if (rblInstall.SelectedValue == "Typical")
                    {
                        BindModules();
                        if (InstallModules())
                        {
                            wizInstall.ActiveStepIndex = 6;
                        }
                    }
                    break;
                case 6:
                    if (rblInstall.SelectedValue == "Typical")
                    {
                        BindSkins();
                        if (InstallSkins())
                        {
                            wizInstall.ActiveStepIndex = 7;
                        }
                    }
                    break;
                case 7:
                    if (rblInstall.SelectedValue == "Typical")
                    {
                        BindLanguages();
                        if (InstallLanguages())
                        {
                            wizInstall.ActiveStepIndex = 8;
                        }
                    }
                    break;
                case 8:
                    if (rblInstall.SelectedValue == "Typical")
                    {
                        BindAuthSystems();
                        if (InstallAuthSystems())
                        {
                            wizInstall.ActiveStepIndex = 9;
                        }
                    }
                    break;
                case 9:
                    if (rblInstall.SelectedValue == "Typical")
                    {
                        BindProviders();
                        if (InstallProviders())
                        {
                            wizInstall.ActiveStepIndex = 10;
                        }
                    }
                    break;
            }
            SetupPage();
        }

        protected void wizInstall_CustomButtonClick(object sender, EventArgs e)
        {
            //switch (wizInstall.ActiveStepIndex)
            //{
            //    case 1:
            //        BindPermissions(true);
            //        if (!PermissionsValid)
            //        {
            //            LinkButton customButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton");
            //            customButton.Text = "<img src=\"" + DotNetNuke.Common.Globals.ApplicationPath + "/images/icon_filemanager_16px.gif\" border=\"0\" /> " + LocalizeString("TestPerm");
            //            ShowButton(GetWizardButton("StepNavigationTemplateContainerID", "CustomButton"), true);
            //            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), false);
            //        }
            //        else
            //        {
            //            ShowButton(GetWizardButton("StepNavigationTemplateContainerID", "CustomButton"), false);
            //            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), true);
            //        }
            //        break;
            //    case 2:
            //        //Page 2 - Database Connection String
            //        if (TestDatabaseConnection())
            //        {
            //            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), true);
            //        }
            //        break;
            //    case 4:
            //        //Page 4 - SMTP Server
            //        TestSMTPSettings();
            //        break;
            //}
        }

        protected void wizInstall_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            Config.Touch();
            Response.Redirect("~/Default.aspx", true);
        }

        protected void wizInstall_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            var nextStep = wizInstall.WizardSteps[e.NextStepIndex] as WizardStep;
            switch (e.CurrentStepIndex)
            {
                case 0:
                    if (rblInstall.SelectedValue == "Auto")
                    {
                        Response.Redirect("~/Install/Install.aspx?mode=install");
                    }
                    break;
                case 1:
                    BindPermissions(true);
                    e.Cancel = !PermissionsValid;
                    break;
                case 2:
                    bool canConnect = TestDatabaseConnection();
                    if (canConnect)
                    {
                        Config.UpdateConnectionString(connectionString);
                        string dbOwner = string.Empty;
                        if (chkOwner.Checked)
                        {
                            dbOwner = "dbo";
                        }
                        else
                        {
                            if ((string.IsNullOrEmpty(GetUpgradeConnectionStringUserID())))
                            {
                                dbOwner = txtUserId.Text;
                            }
                            else
                            {
                                dbOwner = GetUpgradeConnectionStringUserID();
                            }
                        }
                        if (rblDatabases.SelectedValue == "Oracle")
                        {
                            Config.UpdateDataProvider("OracleDataProvider", "", txtqualifier.Text);
                        }
                        else
                        {
                            Config.UpdateDataProvider("SqlDataProvider", dbOwner, txtqualifier.Text);
                        }
                        GetBaseDatabaseVersion();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                    break;
                case 3:
                    e.Cancel = !TestDataBaseInstalled();
                    break;
                case 4:
                    //Page 4 - Host User
                    //Check if SMTP needs to be tested
                    if (!String.IsNullOrEmpty(txtSMTPServer.Text))
                    {
                        if (!TestSMTPSettings())
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    e.Cancel = !InstallHost();
                    break;
                case 5:
                    e.Cancel = !InstallModules();
                    break;
                case 6:
                    e.Cancel = !InstallSkins();
                    break;
                case 7:
                    e.Cancel = !InstallLanguages();
                    break;
                case 8:
                    e.Cancel = !InstallAuthSystems();
                    break;
                case 9:
                    e.Cancel = !InstallProviders();
                    break;
                case 10:
                    e.Cancel = !InstallPortal();
                    break;
            }
        }
    }
}