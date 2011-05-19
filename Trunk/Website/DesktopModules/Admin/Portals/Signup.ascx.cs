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
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Portals
{
    public partial class Signup : PortalModuleBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsHostMenu)
            {
                ModuleConfiguration.ModuleTitle = Localization.GetString("AddPortal", LocalResourceFile);
            }

            jQuery.RequestDnnPluginsRegistration();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            optType.SelectedIndexChanged += optType_SelectedIndexChanged;
            btnCustomizeHomeDir.Click += btnCustomizeHomeDir_Click;
            cboTemplate.SelectedIndexChanged += cboTemplate_SelectedIndexChanged;

            try
            {
                if ((!IsHostMenu || UserInfo.IsSuperUser == false) && !Host.DemoSignup)
                {
                    Response.Redirect(Globals.NavigateURL("Access Denied"), true);
                }
                valEmail2.ValidationExpression = Globals.glbEmailRegEx;
                if (!Page.IsPostBack)
                {
                    string strFolder = Globals.HostMapPath;
                    if (Directory.Exists(strFolder))
                    {
                        string[] fileEntries = Directory.GetFiles(strFolder, "*.template");
                        foreach (string strFileName in fileEntries)
                        {
                            if (Path.GetFileNameWithoutExtension(strFileName) != "admin")
                            {
                                cboTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName));
                            }
                        }
                        if (cboTemplate.Items.Count == 0)
                        {
                            UI.Skins.Skin.AddModuleMessage(this, "", Localization.GetString("PortalMissing", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                            cmdUpdate.Enabled = false;
                        }
                        cboTemplate.Items.Insert(0, new ListItem(Localization.GetString("None_Specified"), "-1"));
                        cboTemplate.SelectedIndex = 0;
                    }
                    if (UserInfo.IsSuperUser)
                    {
                        rowType.Visible = true;
                        optType.SelectedValue = "P";
                    }
                    else
                    {
                        optType.SelectedValue = "C";
                        txtPortalName.Text = Globals.GetDomainName(Request) + @"/";
                        rowType.Visible = false;
                        string strMessage = string.Format(Localization.GetString("DemoMessage", LocalResourceFile),
                                                          Host.DemoPeriod != Null.NullInteger ? " for " + Host.DemoPeriod + " days" : "",
                                                          Globals.GetDomainName(Request));
                        lblInstructions.Text = strMessage;
                        btnCustomizeHomeDir.Visible = false;
                    }
                    txtHomeDirectory.Text = @"Portals/[PortalID]";
                    txtHomeDirectory.Enabled = false;
                    if (MembershipProviderConfig.RequiresQuestionAndAnswer)
                    {
                        questionRow.Visible = true;
                        answerRow.Visible = true;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            txtPassword.Attributes["value"] = txtPassword.Text;
            txtConfirm.Attributes["value"] = txtConfirm.Text;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(IsHostMenu ? Globals.NavigateURL() : Globals.GetPortalDomainName(PortalAlias.HTTPAlias, Request, true), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    bool blnChild;
                    string strPortalAlias;
                    string strChildPath = string.Empty;
                    var objPortalController = new PortalController();
                    var messages = new ArrayList();
                    string schemaFilename = Server.MapPath(string.Concat(AppRelativeTemplateSourceDirectory, "portal.template.xsd"));
                    string xmlFilename = Globals.HostMapPath + cboTemplate.SelectedItem.Text + ".template";
                    var xval = new PortalTemplateValidator();
                    if (!xval.Validate(xmlFilename, schemaFilename))
                    {
                        UI.Skins.Skin.AddModuleMessage(this, "", String.Format(Localization.GetString("InvalidTemplate", LocalResourceFile), cboTemplate.SelectedItem.Text + ".template"), ModuleMessage.ModuleMessageType.RedError);
                        messages.AddRange(xval.Errors);
                        lstResults.Visible = true;
                        lstResults.DataSource = messages;
                        lstResults.DataBind();
                        return;
                    }
                    txtPortalName.Text = txtPortalName.Text.ToLowerInvariant();
                    txtPortalName.Text = txtPortalName.Text.Replace("http://", "");
                    if (PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                    {
                        blnChild = true;
                        strPortalAlias = txtPortalName.Text;
                    }
                    else
                    {
                        blnChild = (optType.SelectedValue == "C");
                        strPortalAlias = blnChild ? txtPortalName.Text.Substring(txtPortalName.Text.LastIndexOf("/") + 1) : txtPortalName.Text;
                    }

                    string message = String.Empty;
                    ModuleMessage.ModuleMessageType messageType = ModuleMessage.ModuleMessageType.RedError;
                    if (!PortalAliasController.ValidateAlias(strPortalAlias, blnChild))
                    {
                        message = Localization.GetString("InvalidName", LocalResourceFile);
                    }
                    if (txtPassword.Text != txtConfirm.Text)
                    {
                        if (!String.IsNullOrEmpty(message)) message += "<br/>";
                        message += Localization.GetString("InvalidPassword", LocalResourceFile);
                    }
                    string strServerPath = Globals.GetAbsoluteServerPath(Request);
                    if (String.IsNullOrEmpty(message))
                    {
                        if (blnChild)
                        {
                            strChildPath = strServerPath + strPortalAlias;
                            if (Directory.Exists(strChildPath))
                            {
                                message = Localization.GetString("ChildExists", LocalResourceFile);
                            }
                            else
                            {
                                if (PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                                {
                                    strPortalAlias = Globals.GetDomainName(Request, true) + "/" + strPortalAlias;
                                }
                                else
                                {
                                    strPortalAlias = txtPortalName.Text;
                                }
                            }
                        }
                    }
                    string homeDir = txtHomeDirectory.Text != @"Portals/[PortalID]" ? txtHomeDirectory.Text : "";
                    if (!string.IsNullOrEmpty(homeDir))
                    {
                        if (string.IsNullOrEmpty(String.Format("{0}\\{1}\\}", Globals.ApplicationMapPath, homeDir).Replace("/", "\\")))
                        {
                            message = Localization.GetString("InvalidHomeFolder", LocalResourceFile);
                        }
                        if (homeDir.Contains("admin") || homeDir.Contains("DesktopModules") || homeDir.ToLowerInvariant() == "portals/")
                        {
                            message = Localization.GetString("InvalidHomeFolder", LocalResourceFile);
                        }
                    }
                    if (!string.IsNullOrEmpty(strPortalAlias))
                    {
                        PortalAliasInfo portalAlias = PortalAliasController.GetPortalAliasLookup(strPortalAlias.ToLower());
                        if (portalAlias != null)
                        {
                            message = Localization.GetString("DuplicatePortalAlias", LocalResourceFile);
                        }
                    }
                    if (String.IsNullOrEmpty(message))
                    {
                        string strTemplateFile = cboTemplate.SelectedItem.Text + ".template";
                        var objAdminUser = new UserInfo();
                        int intPortalId;
                        try
                        {
                            objAdminUser.FirstName = txtFirstName.Text;
                            objAdminUser.LastName = txtLastName.Text;
                            objAdminUser.Username = txtUsername.Text;
                            objAdminUser.DisplayName = txtFirstName.Text + " " + txtLastName.Text;
                            objAdminUser.Email = txtEmail.Text;
                            objAdminUser.IsSuperUser = false;
                            objAdminUser.Membership.Approved = true;
                            objAdminUser.Membership.Password = txtPassword.Text;
                            objAdminUser.Membership.PasswordQuestion = txtQuestion.Text;
                            objAdminUser.Membership.PasswordAnswer = txtAnswer.Text;
                            objAdminUser.Profile.FirstName = txtFirstName.Text;
                            objAdminUser.Profile.LastName = txtLastName.Text;
                            intPortalId = objPortalController.CreatePortal(txtTitle.Text,
                                                                           objAdminUser,
                                                                           txtDescription.Text,
                                                                           txtKeyWords.Text,
                                                                           Globals.HostMapPath,
                                                                           strTemplateFile,
                                                                           homeDir,
                                                                           strPortalAlias,
                                                                           strServerPath,
                                                                           strChildPath,
                                                                           blnChild);
                        }
                        catch (Exception ex)
                        {
                            intPortalId = Null.NullInteger;
                            message = ex.Message;
                        }
                        if (intPortalId != -1)
                        {
                            PortalInfo objPortal = objPortalController.GetPortal(intPortalId);
                            var newSettings = new PortalSettings {PortalAlias = new PortalAliasInfo {HTTPAlias = strPortalAlias}, PortalId = intPortalId, DefaultLanguage = objPortal.DefaultLanguage};
                            string webUrl = Globals.AddHTTP(strPortalAlias);
                            try
                            {
                                if (PortalSettings.ActiveTab.ParentId != PortalSettings.SuperTabId)
                                {
                                    message = Mail.SendMail(PortalSettings.Email,
                                                               txtEmail.Text,
                                                               PortalSettings.Email + ";" + Host.HostEmail,
                                                               Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_SUBJECT", objAdminUser),
                                                               Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_BODY", objAdminUser),
                                                               "",
                                                               "",
                                                               "",
                                                               "",
                                                               "",
                                                               "");
                                }
                                else
                                {
                                    message = Mail.SendMail(Host.HostEmail,
                                                               txtEmail.Text,
                                                               Host.HostEmail,
                                                               Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_SUBJECT", objAdminUser),
                                                               Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_BODY", objAdminUser),
                                                               "",
                                                               "",
                                                               "",
                                                               "",
                                                               "",
                                                               "");
                                }
                            }
                            catch (Exception exc)
                            {
                                DnnLog.Error(exc);

                                message = string.Format(Localization.GetString("UnknownSendMail.Error", LocalResourceFile), webUrl);
                            }
                            var objEventLog = new EventLogController();
                            objEventLog.AddLog(objPortalController.GetPortal(intPortalId), PortalSettings, UserId, "", EventLogController.EventLogType.PORTAL_CREATED);
                            if (message == Null.NullString)
                            {
                                Response.Redirect(webUrl, true);
                            }
                            else
                            {
                                message = string.Format(Localization.GetString("SendMail.Error", LocalResourceFile), message, webUrl);
                                messageType = ModuleMessage.ModuleMessageType.YellowWarning;
                            }
                        }
                    }
                    UI.Skins.Skin.AddModuleMessage(this, "", message, messageType);
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        private void optType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtPortalName.Text = optType.SelectedValue == "C" ? Globals.GetDomainName(Request) + @"/" : "";
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void btnCustomizeHomeDir_Click(Object sender, EventArgs e)
        {
            try
            {
                if (txtHomeDirectory.Enabled)
                {
                    btnCustomizeHomeDir.Text = Localization.GetString("Customize", LocalResourceFile);
                    txtHomeDirectory.Text = @"Portals/[PortalID]";
                    txtHomeDirectory.Enabled = false;
                }
                else
                {
                    btnCustomizeHomeDir.Text = Localization.GetString("AutoGenerate", LocalResourceFile);
                    txtHomeDirectory.Text = "";
                    txtHomeDirectory.Enabled = true;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cboTemplate_SelectedIndexChanged(Object sender, EventArgs e)
        {
            try
            {
                if (cboTemplate.SelectedIndex > 0)
                {
                    string filename = Globals.HostMapPath + cboTemplate.SelectedItem.Text + ".template";
                    var xmldoc = new XmlDocument();
                    xmldoc.Load(filename);
                    XmlNode node = xmldoc.SelectSingleNode("//portal/description");
                    if (node != null && !String.IsNullOrEmpty(node.InnerXml))
                    {
                        lblTemplateDescription.Visible = true;
                        lblTemplateDescription.Text = Server.HtmlDecode(node.InnerXml);
                    }
                    else
                    {
                        lblTemplateDescription.Visible = false;
                    }
                }
                else
                {
                    lblTemplateDescription.Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}