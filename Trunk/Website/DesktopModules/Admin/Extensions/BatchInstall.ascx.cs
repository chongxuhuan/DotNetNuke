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
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.UI.Modules;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Extensions
{
    public partial class BatchInstall : ModuleUserControlBase
    {
        private void BindAuthSystems()
        {
            BindPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "NoAuthSystems", lblAuthSystemsError);
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
                foreach (string strFile in arrFiles)
                {
                    string strResource = strFile.Replace(InstallPath + "\\", "");
                    if (strResource.ToLower().EndsWith(".zip") || strResource.ToLower().EndsWith(".resources"))
                    {
                        var packageItem = new ListItem();
                        packageItem.Value = strResource;
                        strResource = strResource.Replace(".zip", "");
                        strResource = strResource.Replace(".resources", "");
                        strResource = strResource.Replace("_Install", ")");
                        strResource = strResource.Replace("_install", ")");
                        strResource = strResource.Replace("_Source", ")");
                        strResource = strResource.Replace("_source", ")");
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
                noItemsLabel.Text = Localization.GetString(noItemsKey, LocalResourceFile);
            }
            if (errorLabel != null)
            {
                errorLabel.Text = Null.NullString;
            }
        }

        private void BindSkins()
        {
            BindPackageItems("Skin", lstSkins, lblNoSkins, "NoSkins", lblSkinsError);
            BindPackageItems("Container", lstContainers, lblNoContainers, "NoContainers", null);
        }

        private bool InstallAuthSystems()
        {
            return InstallPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "InstallAuthSystemError");
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
                        success = Upgrade.InstallPackage(InstallPath + "\\" + packageItem.Value, packageType, false);
                        if (!success)
                        {
                            strErrorMessage += string.Format(Localization.GetString(errorKey, LocalResourceFile), packageItem.Text);
                        }
                    }
                }
                success = string.IsNullOrEmpty(strErrorMessage);
            }
            catch (Exception ex)
            {
                DnnLog.Debug(ex);
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

        private bool InstallSkins()
        {
            bool skinSuccess = InstallPackageItems("Skin", lstSkins, lblSkinsError, "InstallSkinError");
            bool containerSuccess = InstallPackageItems("Container", lstContainers, lblSkinsError, "InstallContainerError");
            return skinSuccess && containerSuccess;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdInstall.Click += cmdInstall_Click;

            if (!Page.IsPostBack)
            {
                BindModules();
                BindSkins();
                BindLanguages();
                BindAuthSystems();
            }
        }

        protected void cmdInstall_Click(object sender, EventArgs e)
        {
            bool moduleSuccess;
            bool skinSuccess;
            bool languagesSuccess;
            bool AuthSystemSuccess;
            if (lstAuthSystems.SelectedIndex == Null.NullInteger && lstContainers.SelectedIndex == Null.NullInteger && lstSkins.SelectedIndex == Null.NullInteger &&
                lstModules.SelectedIndex == Null.NullInteger && lstLanguages.SelectedIndex == Null.NullInteger)
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("NoneSelected", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                return;
            }
            moduleSuccess = InstallModules();
            skinSuccess = InstallSkins();
            languagesSuccess = InstallLanguages();
            AuthSystemSuccess = InstallAuthSystems();
            if (moduleSuccess && skinSuccess && languagesSuccess && AuthSystemSuccess)
            {
                Response.Redirect(Request.RawUrl, true);
            }
        }
    }
}