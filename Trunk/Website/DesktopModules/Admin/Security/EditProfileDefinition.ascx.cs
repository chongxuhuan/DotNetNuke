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
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class EditProfileDefinition : PortalModuleBase
    {
        private string ResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx";
        private string _Message = Null.NullString;
        private ProfilePropertyDefinition _PropertyDefinition;

        protected bool IsAddMode
        {
            get
            {
                return (PropertyDefinitionID == Null.NullInteger);
            }
        }

        protected bool IsList
        {
            get
            {
                bool _IsList = false;
                var objListController = new ListController();
                ListEntryInfo dataType = objListController.GetListEntryInfo(PropertyDefinition.DataType);
                if ((dataType != null) && (dataType.ListName == "DataType") && (dataType.Value == "List"))
                {
                    _IsList = true;
                }
                return _IsList;
            }
        }

        protected bool IsSuperUser
        {
            get
            {
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected ProfilePropertyDefinition PropertyDefinition
        {
            get
            {
                if (_PropertyDefinition == null)
                {
                    if (IsAddMode)
                    {
                        _PropertyDefinition = new ProfilePropertyDefinition();
                        _PropertyDefinition.PortalId = UsersPortalId;
                    }
                    else
                    {
                        _PropertyDefinition = ProfileController.GetPropertyDefinition(PropertyDefinitionID, UsersPortalId);
                    }
                }
                return _PropertyDefinition;
            }
        }

        protected int PropertyDefinitionID
        {
            get
            {
                int _DefinitionID = Null.NullInteger;
                if (ViewState["PropertyDefinitionID"] != null)
                {
                    _DefinitionID = Int32.Parse(ViewState["PropertyDefinitionID"].ToString());
                }
                return _DefinitionID;
            }
            set
            {
                ViewState["PropertyDefinitionID"] = value;
            }
        }

        protected int UsersPortalId
        {
            get
            {
                int intPortalId = PortalId;
                if (IsSuperUser)
                {
                    intPortalId = Null.NullInteger;
                }
                return intPortalId;
            }
        }

        private void UpdateResourceFileNode(XmlDocument xmlDoc, string key, string text)
        {
            XmlNode node;
            XmlNode nodeData;
            XmlAttribute attr;
            node = xmlDoc.SelectSingleNode("//root/data[@name='" + key + "']/value");
            if (node == null)
            {
                nodeData = xmlDoc.CreateElement("data");
                attr = xmlDoc.CreateAttribute("name");
                attr.Value = key;
                nodeData.Attributes.Append(attr);
                xmlDoc.SelectSingleNode("//root").AppendChild(nodeData);
                node = nodeData.AppendChild(xmlDoc.CreateElement("value"));
            }
            node.InnerXml = Server.HtmlEncode(text);
        }

        private void BindLanguages()
        {
            txtPropertyName.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName, ResourceFile, cboLocales.SelectedValue);
            txtPropertyHelp.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Help", ResourceFile, cboLocales.SelectedValue);
            txtPropertyRequired.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Required", ResourceFile, cboLocales.SelectedValue);
            txtPropertyValidation.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Validation", ResourceFile, cboLocales.SelectedValue);
            txtCategoryName.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyCategory + ".Header", ResourceFile, cboLocales.SelectedValue);
        }

        private void BindList()
        {
            if (IsList)
            {
                lstEntries.Mode = "ListEntries";
                lstEntries.SelectedKey = PropertyDefinition.PropertyName;
                lstEntries.ListPortalID = UsersPortalId;
                lstEntries.ShowDelete = false;
                lstEntries.DataBind();
            }
        }

        private string GetResourceFile(string type, string language)
        {
            string resourcefilename = ResourceFile;
            if (language != Localization.SystemLocale)
            {
                resourcefilename = resourcefilename + "." + language;
            }
            if (type == "Portal")
            {
                resourcefilename = resourcefilename + "." + "Portal-" + PortalId;
            }
            else if (type == "Host")
            {
                resourcefilename = resourcefilename + "." + "Host";
            }
            return HttpContext.Current.Server.MapPath(resourcefilename + ".resx");
        }

        private bool ValidateProperty(ProfilePropertyDefinition definition)
        {
            bool isValid = true;
            var objListController = new ListController();
            string strDataType = objListController.GetListEntryInfo(definition.DataType).Value;
            switch (strDataType)
            {
                case "Text":
                    if (definition.Required && definition.Length == 0)
                    {
                        _Message = "RequiredTextBox";
                        isValid = Null.NullBoolean;
                    }
                    break;
            }
            return isValid;
        }

        public string GetText(string type)
        {
            string text = Null.NullString;
            if (IsAddMode && Wizard.ActiveStepIndex == 0)
            {
                if (type == "Title")
                {
                    text = Localization.GetString(Wizard.ActiveStep.Title + "_Add.Title", LocalResourceFile);
                }
                else if (type == "Help")
                {
                    text = Localization.GetString(Wizard.ActiveStep.Title + "_Add.Help", LocalResourceFile);
                }
            }
            else
            {
                if (type == "Title")
                {
                    text = Localization.GetString(Wizard.ActiveStep.Title + ".Title", LocalResourceFile);
                }
                else if (type == "Help")
                {
                    text = Localization.GetString(Wizard.ActiveStep.Title + ".Help", LocalResourceFile);
                }
            }
            return text;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lstEntries.ID = "ListEntries";
            if (PropertyDefinitionID == Null.NullInteger)
            {
                if ((Request.QueryString["PropertyDefinitionId"] != null))
                {
                    PropertyDefinitionID = Int32.Parse(Request.QueryString["PropertyDefinitionId"]);
                }
            }
            if (IsAddMode)
            {
                ModuleConfiguration.ModuleTitle = Localization.GetString("AddProperty", LocalResourceFile);
            }
            else
            {
                ModuleConfiguration.ModuleTitle = Localization.GetString("EditProperty", LocalResourceFile);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cboLocales.SelectedIndexChanged += cboLocales_SelectedIndexChanged;
            cmdSaveKeys.Click += cmdSaveKeys_Click;
            Wizard.ActiveStepChanged += Wizard_ActiveStepChanged;
            Wizard.CancelButtonClick += Wizard_CancelButtonClick;
            Wizard.FinishButtonClick += Wizard_FinishButtonClick;
            Wizard.NextButtonClick += Wizard_NextButtonClick;

            try
            {
                Wizard.CancelButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + Localization.GetString("cmdCancel", LocalResourceFile);
                Wizard.StartNextButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + Localization.GetString("Next", LocalResourceFile);
                Wizard.StepNextButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/rt.gif\" border=\"0\" /> " + Localization.GetString("Next", LocalResourceFile);
                Wizard.FinishCompleteButtonText = "<img src=\"" + Globals.ApplicationPath + "/images/lt.gif\" border=\"0\" /> " + Localization.GetString("cmdCancel", LocalResourceFile);
                if (!Page.IsPostBack)
                {
                    Localization.LoadCultureDropDownList(cboLocales, CultureDropDownTypes.NativeName, ((PageBase) Page).PageCulture.Name);
                    if (cboLocales.SelectedItem != null)
                    {
                        lblLocales.Text = cboLocales.SelectedItem.Text;
                    }
                    cboLocales.Visible = cboLocales.Items.Count != 1;
                    lblLocales.Visible = cboLocales.Items.Count == 1;
                }
                Properties.LocalResourceFile = LocalResourceFile;
                Properties.DataSource = PropertyDefinition;
                Properties.DataBind();
                foreach (FieldEditorControl editor in Properties.Fields)
                {
                    if (editor.DataField == "Required")
                    {
                        editor.Visible = UsersPortalId != Null.NullInteger;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cboLocales_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindLanguages();
        }

        protected void cmdSaveKeys_Click(object sender, EventArgs e)
        {
            var portalResources = new XmlDocument();
            var defaultResources = new XmlDocument();
            XmlNode parent;
            string filename;
            try
            {
                defaultResources.Load(GetResourceFile("", Localization.SystemLocale));
                if (IsHostMenu)
                {
                    filename = GetResourceFile("Host", cboLocales.SelectedValue);
                }
                else
                {
                    filename = GetResourceFile("Portal", cboLocales.SelectedValue);
                }
                if (File.Exists(filename))
                {
                    portalResources.Load(filename);
                }
                else
                {
                    portalResources.Load(GetResourceFile("", Localization.SystemLocale));
                }
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Text", txtPropertyName.Text);
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Help", txtPropertyHelp.Text);
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Required", txtPropertyRequired.Text);
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Validation", txtPropertyValidation.Text);
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyCategory + ".Header", txtCategoryName.Text);
                foreach (XmlNode node in portalResources.SelectNodes("//root/data"))
                {
                    XmlNode defaultNode = defaultResources.SelectSingleNode("//root/data[@name='" + node.Attributes["name"].Value + "']");
                    if (defaultNode != null && defaultNode.InnerXml == node.InnerXml)
                    {
                        parent = node.ParentNode;
                        parent.RemoveChild(node);
                    }
                }
                foreach (XmlNode node in portalResources.SelectNodes("//root/data"))
                {
                    if (portalResources.SelectNodes("//root/data[@name='" + node.Attributes["name"].Value + "']").Count > 1)
                    {
                        parent = node.ParentNode;
                        parent.RemoveChild(node);
                    }
                }
                if (portalResources.SelectNodes("//root/data").Count > 0)
                {
                    portalResources.Save(filename);
                }
                else
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("Save.ErrorMessage", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
            }
        }

        protected void Wizard_ActiveStepChanged(object sender, EventArgs e)
        {
            switch (Wizard.ActiveStepIndex)
            {
                case 1:
                    if (!IsList)
                    {
                        Wizard.ActiveStepIndex = 2;
                    }
                    else
                    {
                        BindList();
                    }
                    break;
                case 2:
                    BindLanguages();
                    Wizard.DisplayCancelButton = false;
                    break;
            }
        }

        protected void Wizard_CancelButtonClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(TabId, "ManageProfile", "mid=" + ModuleId), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void Wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(TabId, "ManageProfile", "mid=" + ModuleId), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void Wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            switch (e.CurrentStepIndex)
            {
                case 0:
                    try
                    {
                        if (Properties.IsDirty && Properties.IsValid)
                        {
                            ProfilePropertyDefinition propertyDefinition;
                            propertyDefinition = (ProfilePropertyDefinition) Properties.DataSource;
                            if (UsersPortalId == Null.NullInteger)
                            {
                                propertyDefinition.Required = false;
                            }
                            if (ValidateProperty(propertyDefinition))
                            {
                                if (PropertyDefinitionID == Null.NullInteger)
                                {
                                    PropertyDefinitionID = ProfileController.AddPropertyDefinition(propertyDefinition);
                                    if (PropertyDefinitionID < Null.NullInteger)
                                    {
                                        UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DuplicateName", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                                        e.Cancel = true;
                                    }
                                }
                                else
                                {
                                    ProfileController.UpdatePropertyDefinition(propertyDefinition);
                                }
                            }
                            else
                            {
                                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString(_Message, LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                                e.Cancel = true;
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                    break;
            }
        }
    }
}