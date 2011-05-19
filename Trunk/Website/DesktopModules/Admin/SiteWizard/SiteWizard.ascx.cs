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
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;

#endregion

namespace DotNetNuke.Modules.Admin.Portals
{

    public partial class SiteWizard : PortalModuleBase
    {

        #region ContainerType enum

        public enum ContainerType
        {
            Host = 0,
            Portal = 1,
            Folder = 2,
            All = 3
        }

        #endregion

        #region Private Methods

        private void BindContainers()
        {
            ctlPortalContainer.Clear();
            if (chkIncludeAll.Checked)
            {
                GetContainers(ContainerType.All, "", "");
            }
            else
            {
                if (!String.IsNullOrEmpty(ctlPortalSkin.SkinSrc))
                {
                    string strFolder;
                    string strContainerFolder = ctlPortalSkin.SkinSrc.Substring(0, ctlPortalSkin.SkinSrc.LastIndexOf("/"));
                    if (strContainerFolder.StartsWith("[G]"))
                    {
                        strContainerFolder = strContainerFolder.Replace("[G]Skins/", "Containers\\");
                        strFolder = Globals.HostMapPath + strContainerFolder;
                        GetContainers(ContainerType.Folder, "[G]", strFolder);
                    }
                    else
                    {
                        strContainerFolder = strContainerFolder.Replace("[L]Skins/", "Containers\\");
                        strFolder = PortalSettings.HomeDirectoryMapPath + strContainerFolder;
                        GetContainers(ContainerType.Folder, "[L]", strFolder);
                    }
                }
                else
                {
                    GetContainers(ContainerType.Portal, "", "");
                }
            }
        }

        private void GetContainers(ContainerType type, string skinType, string strFolder)
        {
            ctlPortalContainer.Columns = 3;
            ctlPortalContainer.SkinRoot = SkinController.RootContainer;
            switch (type)
            {
                case ContainerType.Folder:
                    ctlPortalContainer.LoadSkins(strFolder, skinType, false);
                    break;
                case ContainerType.Portal:
                    ctlPortalContainer.LoadPortalSkins(false);
                    break;
                case ContainerType.Host:
                    ctlPortalContainer.LoadHostSkins(false);
                    break;
                case ContainerType.All:
                    ctlPortalContainer.LoadAllSkins(false);
                    break;
            }
        }

        private void GetSkins()
        {
            ctlPortalSkin.Columns = 3;
            ctlPortalSkin.SkinRoot = SkinController.RootSkin;
            ctlPortalSkin.LoadAllSkins(false);
        }

        private void GetTemplates()
        {
            string strFolder;
            strFolder = Globals.HostMapPath;
            if (Directory.Exists(strFolder))
            {
                string[] fileEntries = Directory.GetFiles(strFolder, "*.template");
                foreach (string strFileName in fileEntries)
                {
                    if (Path.GetFileNameWithoutExtension(strFileName) == "admin")
                    {
                    }
                    else
                    {
                        lstTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName));
                    }
                }
                if (lstTemplate.Items.Count == 0)
                {
                }
            }
        }

        private void UseTemplate()
        {
            lstTemplate.Enabled = chkTemplate.Checked;
            optMerge.Enabled = chkTemplate.Checked;
            lblMergeTitle.Enabled = chkTemplate.Checked;
            lblMergeWarning.Enabled = chkTemplate.Checked;
            lblTemplateMessage.Text = "";
        }

        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chkIncludeAll.CheckedChanged += OnIncludeAllCheckChanged;
            chkTemplate.CheckedChanged += OnTemplateCheckChanged;
            lstTemplate.SelectedIndexChanged += OnTemplateSelectedIndexChanged;
            Wizard.ActiveStepChanged += OnWizardActiveStepChanged;
            Wizard.FinishButtonClick += OnWizardFinishedClick;
            Wizard.NextButtonClick += OnWizardNextClick;

            try
            {
                Wizard.StartNextButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + Localization.GetString("Next", LocalResourceFile);
                Wizard.StepNextButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + Localization.GetString("Next", LocalResourceFile);
                Wizard.StepPreviousButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + Localization.GetString("Previous", LocalResourceFile);
                Wizard.FinishPreviousButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + Localization.GetString("Previous", LocalResourceFile);
                Wizard.FinishCompleteButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/save.gif\" border=\"0\" /> " + Localization.GetString("Finish", LocalResourceFile);
                if (!Page.IsPostBack)
                {
                    GetTemplates();
                    chkTemplate.Checked = false;
                    lstTemplate.Enabled = false;
                    GetSkins();
                    var objPortalController = new PortalController();
                    var objPortal = objPortalController.GetPortal(PortalId);
                    txtPortalName.Text = objPortal.PortalName;
                    txtDescription.Text = objPortal.Description;
                    txtKeyWords.Text = objPortal.KeyWords;
                    urlLogo.Url = objPortal.LogoFile;
                    urlLogo.FileFilter = Globals.glbImageFileTypes;
                    UseTemplate();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnIncludeAllCheckChanged(object sender, EventArgs e)
        {
            BindContainers();
        }

        protected void OnTemplateCheckChanged(object sender, EventArgs e)
        {
            if (chkTemplate.Checked)
            {
                lstTemplate.SelectedIndex = -1;
            }
            UseTemplate();
        }

        protected void OnTemplateSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTemplate.SelectedIndex > -1)
            {
                var xmlDoc = new XmlDocument();
                XmlNode node;
                var strTemplatePath = Globals.HostMapPath;
                var strTemplateFile = lstTemplate.SelectedItem.Text + ".template";
                try
                {
                    xmlDoc.Load(strTemplatePath + strTemplateFile);
                    node = xmlDoc.SelectSingleNode("//portal/description");
                    if (node != null)
                    {
                        lblTemplateMessage.Text = node.InnerText;
                    }
                    else
                    {
                        lblTemplateMessage.Text = "";
                    }
                    //Check that all modules in template are isntalled
                    // parse portal desktop modules (version 5.0 templates)
                    node = xmlDoc.SelectSingleNode("//portal/portalDesktopModules");
                    if (node != null)
                    {
                        var message = PortalController.CheckDesktopModulesInstalled(node.CreateNavigator());
                        if (!string.IsNullOrEmpty(message))
                        {
                            lblTemplateMessage.Text = string.Format("<p>This template has the following modules that are not installed.</p><p>{0}</p>", message);
                        }
                    }
                }
                catch (Exception exc)
                {
                    
                    DnnLog.Error(exc);

                    lblTemplateMessage.Text = "Error Loading Template description";
                }
            }
            else
            {
                lblTemplateMessage.Text = "";
            }
        }

        protected void OnWizardActiveStepChanged(object sender, EventArgs e)
        {
            switch (Wizard.ActiveStepIndex)
            {
                case 3:
                    BindContainers();
                    break;
            }
        }

        protected void OnWizardFinishedClick(object sender, WizardNavigationEventArgs e)
        {
            var objPortalController = new PortalController();
            if (lstTemplate.SelectedIndex != -1)
            {
                string strTemplateFile = lstTemplate.SelectedItem.Text + ".template";
                objPortalController.ProcessResourceFile(PortalSettings.HomeDirectoryMapPath, Globals.HostMapPath + strTemplateFile);
                switch (optMerge.SelectedValue)
                {
                    case "Ignore":
                        objPortalController.ParseTemplate(PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Ignore, false);
                        break;
                    case "Replace":
                        objPortalController.ParseTemplate(PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Replace, false);
                        break;
                    case "Merge":
                        objPortalController.ParseTemplate(PortalId, Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Merge, false);
                        break;
                }
            }
            PortalInfo objPortal = objPortalController.GetPortal(PortalId);
            objPortal.Description = txtDescription.Text;
            objPortal.KeyWords = txtKeyWords.Text;
            objPortal.PortalName = txtPortalName.Text;
            objPortal.LogoFile = urlLogo.Url;
            objPortalController.UpdatePortalInfo(objPortal);
            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, ctlPortalSkin.SkinSrc);
            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, ctlPortalSkin.SkinSrc);
            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, ctlPortalContainer.SkinSrc);
            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, ctlPortalContainer.SkinSrc);
        }

        protected void OnWizardNextClick(object sender, WizardNavigationEventArgs e)
        {
            string strMessage;
            switch (e.CurrentStepIndex)
            {
                case 1:
                    if (lstTemplate.SelectedIndex == -1)
                    {
                        if (chkTemplate.Checked)
                        {
                            e.Cancel = true;
                            lblTemplateMessage.Text = Localization.GetString("TemplateRequired", LocalResourceFile);
                        }
                    }
                    else
                    {
                        string schemaFilename = Server.MapPath("DesktopModules/Admin/Portals/portal.template.xsd");
                        string xmlFilename = Globals.HostMapPath + lstTemplate.SelectedItem.Text + ".template";
                        var xval = new PortalTemplateValidator();
                        if (!xval.Validate(xmlFilename, schemaFilename))
                        {
                            strMessage = Localization.GetString("InvalidTemplate", LocalResourceFile);
                            lblTemplateMessage.Text = string.Format(strMessage, lstTemplate.SelectedItem.Text + ".template");
                            e.Cancel = true;
                        }
                    }
                    break;
            }

        }

        #endregion

    }
}