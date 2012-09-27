#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer.Packages;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;

using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace DotNetNuke.Modules.Admin.AdvancedSettings
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class AdvancedSettings : PortalModuleBase
    {
        #region Private Members

        private string ReturnUrl
        {
            get
            {
                return Request.Params["ReturnUrl"] ?? Globals.NavigateURL();
            }
        }

        #endregion

        #region Public Properties

        #endregion

        #region Private Methods

        private bool InstallPackage(string packageType, string fileName, string errorKey)
        {
            var success = false;
            var errorMessage = Null.NullString;

            try
            {
                //Set Script timeout to MAX value
                Server.ScriptTimeout = int.MaxValue;

                var installPath = Globals.ApplicationMapPath + "\\Install\\" + packageType;

                if (File.Exists(installPath + "\\" + fileName))
                {
                    success = Upgrade.InstallPackage(installPath + "\\" + fileName, packageType, false);
                    if (!success)
                    {
                        errorMessage = string.Format(LocalizeString(errorKey), fileName); // TODO: Use package FriendlyName
                    }
                }

                success = string.IsNullOrEmpty(errorMessage);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                errorMessage = ex.StackTrace;
            }

            if (!success)
            {
                Skin.AddModuleMessage(this, errorMessage, ModuleMessage.ModuleMessageType.RedError);
            }

            return success;
        }

        #endregion

        #region Protected Methods

        protected bool SmtpServerSettingsEnabled
        {
            get
            {
                return UserInfo.IsSuperUser;
            }            
        }

        #endregion

        #region Event Handlers

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdSkinsUpdate.Click += OnUpdateSkinsClick;
            cmdSmtpUpdate.Click += OnUpdateSmtpClick;                      
            cmdSkinsCancel.Click += OnCancelClick;
            cmdSmtpCancel.Click += OnCancelClick;
            cmdLangCancel.Click += OnCancelClick;
            cmdAuthCancel.Click += OnCancelClick;
            cmdProvidersCancel.Click += OnCancelClick;
            cmdModulesCancel.Click += OnCancelClick;
            authSystemsGrid.ItemCommand += OnAuthSystemsGridItemCommand;
            providersGrid.ItemCommand += OnProvidersGridItemCommand;
            modulesGrid.ItemCommand += OnModulesGridItemCommand;

            //Hide Update button in SkinDesigner
            HideUpdateButtonInSkinDesigner();

            //Hide Cancel Button when not originating from Getting Started
            if(!HttpContext.Current.Request.Url.AbsoluteUri.Contains("gettingStarted"))
            {
                cmdSkinsCancel.Visible = false;
                cmdSmtpCancel.Visible = false;
                cmdLangCancel.Visible = false;
                cmdAuthCancel.Visible = false;
                cmdProvidersCancel.Visible = false;
                cmdModulesCancel.Visible = false;
            }

            try
            {
                if (!Page.IsPostBack)
                {
                    BindGrid("AuthSystem", authSystemsGrid, divNoAuthSystems);
                    BindGrid("Provider", providersGrid, divNoProviders);
                    BindGrid("Module", modulesGrid, divNoModules);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void HideUpdateButtonInSkinDesigner()
        {
            this.Attributes.HideUpdateButton = true;
        }

        protected void OnUpdateSmtpClick(object sender, EventArgs e)
        {
            try
            {
                var returnUrl = ReturnUrl;

                if (SmtpServerSettingsEnabled)
                {
                    smtpServerSettings.Update(ref returnUrl);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                DataCache.ClearHostCache(false);
            }
        }

        protected void OnCancelClick(object sender, EventArgs e)
        {
            Response.Redirect(UrlUtils.PopUpUrl("~/GettingStarted.aspx", this, PortalSettings, false, true));            
        }
        
        protected void OnUpdateSkinsClick(object sender, EventArgs e)
        {
            try
            {
                //Update skin changes
                Attributes.Update();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            finally
            {
                DataCache.ClearHostCache(false);
            }
        }
        #endregion

        #region Packages table

        protected void OnAuthSystemsGridItemCommand(object source, DataGridCommandEventArgs e)
        {
            var fileName = ((LinkButton)e.CommandSource).CommandArgument;
            if (InstallPackage("AuthSystem", fileName, "InstallAuthSystemError"))
            {
                BindGrid("AuthSystem", authSystemsGrid, divNoAuthSystems);
            }
        }

        protected void OnModulesGridItemCommand(object source, DataGridCommandEventArgs e)
        {
            var fileName = ((LinkButton)e.CommandSource).CommandArgument;
            if (InstallPackage("Module", fileName, "InstallModuleError"))
            {
                BindGrid("Module", modulesGrid, divNoModules);
            }
        }

        protected void OnProvidersGridItemCommand(object source, DataGridCommandEventArgs e)
        {
            var fileName = ((LinkButton)e.CommandSource).CommandArgument;
            if (InstallPackage("Provider", fileName, "InstallProviderError"))
            {
                BindGrid("Provider", providersGrid, divNoProviders);
            }
        }

        private void BindGrid(string type, DataGrid grid, HtmlGenericControl noItemsControl)
        {
            var installPath = Globals.ApplicationMapPath + "\\Install\\" + type;
            var packages = new List<PackageInfo>();
            var invalidPackages = new List<string>();

            foreach (string file in Directory.GetFiles(installPath))
            {
                if (file.ToLower().EndsWith(".zip") || file.ToLower().EndsWith(".resources"))
                {
                    Stream inputStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var unzip = new ZipInputStream(inputStream);

                    try
                    {
                        ZipEntry entry = unzip.GetNextEntry();

                        while (entry != null)
                        {
                            if (!entry.IsDirectory)
                            {
                                var fileName = entry.Name;
                                string extension = Path.GetExtension(fileName);
                                if (extension.ToLower() == ".dnn" || extension.ToLower() == ".dnn5")
                                {
                                    //Manifest
                                    var manifestReader = new StreamReader(unzip);
                                    var manifest = manifestReader.ReadToEnd();

                                    var package = new PackageInfo();
                                    package.Manifest = manifest;
                                    if (!string.IsNullOrEmpty(manifest))
                                    {
                                        var doc = new XPathDocument(new StringReader(manifest));
                                        XPathNavigator rootNav = doc.CreateNavigator().SelectSingleNode("dotnetnuke");
                                        string packageType = String.Empty;
                                        if (rootNav.Name == "dotnetnuke")
                                        {
                                            packageType = XmlUtils.GetAttributeValue(rootNav, "type");
                                        }
                                        else if (rootNav.Name.ToLower() == "languagepack")
                                        {
                                            packageType = "LanguagePack";
                                        }
                                        XPathNavigator nav = null;
                                        switch (packageType.ToLower())
                                        {
                                            case "package":
                                                nav = rootNav.SelectSingleNode("packages/package");
                                                break;

                                            case "languagepack":

                                                //nav = Installer.ConvertLegacyNavigator(rootNav, new InstallerInfo()).SelectSingleNode("packages/package");
                                                break;
                                        }

                                        if (nav != null)
                                        {
                                            package.Name = XmlUtils.GetAttributeValue(nav, "name");
                                            package.PackageType = XmlUtils.GetAttributeValue(nav, "type");
                                            package.IsSystemPackage = XmlUtils.GetAttributeValueAsBoolean(nav, "isSystem", false);
                                            package.Version = new Version(XmlUtils.GetAttributeValue(nav, "version"));
                                            package.FriendlyName = XmlUtils.GetNodeValue(nav, "friendlyName");
                                            if (String.IsNullOrEmpty(package.FriendlyName))
                                            {
                                                package.FriendlyName = package.Name;
                                            }

                                            package.Description = XmlUtils.GetNodeValue(nav, "description");
                                            package.FileName = file.Replace(installPath + "\\", "");

                                            packages.Add(package);
                                        }
                                    }

                                    break;
                                }
                            }
                            entry = unzip.GetNextEntry();
                        }
                    }
                    catch (Exception)
                    {
                        invalidPackages.Add(file);
                    }
                    finally
                    {
                        unzip.Close();
                        unzip.Dispose();
                    }
                }
            }

            if (invalidPackages.Count > 0)
            {
                var pkgErrorsMsg = invalidPackages.Aggregate(string.Empty, (current, pkg) => current + (pkg + "<br />"));
                Skin.AddModuleMessage(this, Localization.GetString("PackageErrors.Text", LocalResourceFile) + pkgErrorsMsg, ModuleMessage.ModuleMessageType.RedError);
            }

            if (packages.Count == 0)
            {
                noItemsControl.Visible = true;
                grid.Visible = false;    
            }
            else
            {
                noItemsControl.Visible = false;
                grid.DataSource = packages;
                grid.DataBind();
            }            
        }

        protected string FormatVersion(object version)
        {
            var package = version as PackageInfo;
            string retValue = Null.NullString;
            if (package != null)
            {
                retValue = package.Version.ToString(3);
            }
            return retValue;
        }

        protected string GetPackageDescription(object dataItem)
        {
            var package = dataItem as PackageInfo;
            string retValue = Null.NullString;
            if (package != null)
            {
                retValue = package.Description;
            }
            return retValue;
        }
        
        protected string GetIconUrl(string icon)
        {
            return Globals.ImagePath + icon;
        }

        protected string GetFileName(object dataItem)
        {
            var package = dataItem as PackageInfo;
            return package.FileName;
        }
      
        #endregion
    }
}