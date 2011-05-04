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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.Newsletters
{
    public partial class Newsletter : PortalModuleBase
    {
        private string FormatUrls(Match m)
        {
            string match = m.Value;
            string url = m.Groups["url"].Value;
            string result = match;
            if (url.StartsWith("/"))
            {
                return result.Replace(url, Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + url);
            }
            else if (url.Contains("://"))
            {
                return result;
            }
            else
            {
                return result.Replace(url, Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + Globals.ApplicationPath + "/" + url);
            }
        }

        private string ManageDirectoryBase(string source)
        {
            string pattern = "<(a|link|img|script|object|table|td|body).[^>]*(href|src|action|background)=(\\\"|'|)(?<url>(.[^\\\"']*))(\\\"|'|)[^>]*>";
            return Regex.Replace(source, pattern, FormatUrls);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdPreview.Click += cmdPreview_Click;
            cmdSend.Click += cmdSend_Click;

            try
            {
                if (!Page.IsPostBack)
                {
                    txtFrom.Text = UserInfo.Email;
                }
                plLanguages.Visible = (LocaleController.Instance.GetLocales(PortalId).Count > 1);
                pnlRelayAddress.Visible = (cboSendMethod.SelectedValue == "RELAY");
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdSend_Click(Object sender, EventArgs e)
        {
            string strResult = "";
            ModuleMessage.ModuleMessageType msgResult = ModuleMessage.ModuleMessageType.GreenSuccess;
            int intMailsSent = -1;
            bool isValid = true;
            try
            {
                if (String.IsNullOrEmpty(txtSubject.Text) || String.IsNullOrEmpty(teMessage.Text))
                {
                    strResult = Localization.GetString("MessageValidation", LocalResourceFile);
                    msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                    isValid = false;
                }
                else
                {
                    var objRoleNames = new List<string>();
                    foreach (string strRoleName in dgSelectedRoles.SelectedRoleNames)
                    {
                        objRoleNames.Add(strRoleName);
                    }
                    var objUsers = new List<UserInfo>();
                    if (!String.IsNullOrEmpty(txtEmail.Text))
                    {
                        Array arrEmail = txtEmail.Text.Split(';');
                        foreach (string strEmail in arrEmail)
                        {
                            var objUser = new UserInfo();
                            objUser.UserID = Null.NullInteger;
                            objUser.Email = strEmail;
                            objUser.DisplayName = strEmail;
                            objUsers.Add(objUser);
                        }
                    }
                    if (objUsers.Count == 0 && objRoleNames.Count == 0)
                    {
                        strResult = string.Format(Localization.GetString("NoMessagesSent", LocalResourceFile), intMailsSent);
                        msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                        isValid = false;
                    }
                    else
                    {
                        var objSendBulkEMail = new SendTokenizedBulkEmail(objRoleNames, objUsers, false, txtSubject.Text, teMessage.Text);
                        switch (teMessage.Mode)
                        {
                            case "RICH":
                                objSendBulkEMail.BodyFormat = MailFormat.Html;
                                break;
                            default:
                                objSendBulkEMail.BodyFormat = MailFormat.Text;
                                break;
                        }
                        if (objSendBulkEMail.SuppressTokenReplace != !chkReplaceTokens.Checked)
                        {
                            objSendBulkEMail.SuppressTokenReplace = !chkReplaceTokens.Checked;
                        }
                        switch (cboPriority.SelectedItem.Value)
                        {
                            case "1":
                                objSendBulkEMail.Priority = MailPriority.High;
                                break;
                            case "2":
                                objSendBulkEMail.Priority = MailPriority.Normal;
                                break;
                            case "3":
                                objSendBulkEMail.Priority = MailPriority.Low;
                                break;
                            default:
                                isValid = false;
                                break;
                        }
                        if (txtFrom.Text != string.Empty && objSendBulkEMail.SendingUser.Email != txtFrom.Text)
                        {
                            UserInfo myUser = objSendBulkEMail.SendingUser;
                            if (myUser == null)
                            {
                                myUser = new UserInfo();
                            }
                            myUser.Email = txtFrom.Text;
                            objSendBulkEMail.SendingUser = myUser;
                        }
                        if (txtReplyTo.Text != string.Empty && objSendBulkEMail.ReplyTo.Email != txtReplyTo.Text)
                        {
                            var myUser = new UserInfo();
                            myUser.Email = txtReplyTo.Text;
                            objSendBulkEMail.ReplyTo = myUser;
                        }
                        if (selLanguage.Visible && selLanguage.SelectedLanguages != null)
                        {
                            objSendBulkEMail.LanguageFilter = selLanguage.SelectedLanguages;
                        }
                        if (ctlAttachment.Url.StartsWith("FileID="))
                        {
                            int fileId = int.Parse(ctlAttachment.Url.Substring(7));
                            var objFileInfo = FileManager.Instance.GetFile(fileId);
                            objSendBulkEMail.AddAttachment(PortalSettings.HomeDirectoryMapPath + objFileInfo.Folder + objFileInfo.FileName);
                        }
                        switch (cboSendMethod.SelectedItem.Value)
                        {
                            case "TO":
                                objSendBulkEMail.AddressMethod = SendTokenizedBulkEmail.AddressMethods.Send_TO;
                                break;
                            case "BCC":
                                objSendBulkEMail.AddressMethod = SendTokenizedBulkEmail.AddressMethods.Send_BCC;
                                break;
                            case "RELAY":
                                objSendBulkEMail.AddressMethod = SendTokenizedBulkEmail.AddressMethods.Send_Relay;
                                if (string.IsNullOrEmpty(txtRelayAddress.Text))
                                {
                                    strResult = string.Format(Localization.GetString("MessagesSentCount", LocalResourceFile), intMailsSent);
                                    msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                                    isValid = false;
                                }
                                else
                                {
                                    objSendBulkEMail.RelayEmailAddress = txtRelayAddress.Text;
                                }
                                break;
                        }
                        if (!chkReplaceTokens.Checked)
                        {
                            objSendBulkEMail.SuppressTokenReplace = true;
                        }
                        objSendBulkEMail.RemoveDuplicates = true;
                        if (!isValid)
                        {
                        }
                        else if (optSendAction.SelectedItem.Value == "S")
                        {
                            intMailsSent = objSendBulkEMail.SendMails();
                            if (intMailsSent > 0)
                            {
                                strResult = string.Format(Localization.GetString("MessagesSentCount", LocalResourceFile), intMailsSent);
                                msgResult = ModuleMessage.ModuleMessageType.GreenSuccess;
                            }
                            else
                            {
                                strResult = Localization.GetString("NoMessagesSent", LocalResourceFile);
                                msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                            }
                        }
                        else
                        {
                            string strMessage = Mail.SendMail(Host.HostEmail,
                                                              Host.HostEmail,
                                                              "",
                                                              "",
                                                              MailPriority.Normal,
                                                              Localization.GetSystemMessage(PortalSettings, "EMAIL_SMTP_TEST_SUBJECT"),
                                                              MailFormat.Text,
                                                              Encoding.UTF8,
                                                              "",
                                                              "",
                                                              Host.SMTPServer,
                                                              Host.SMTPAuthentication,
                                                              Host.SMTPUsername,
                                                              Host.SMTPPassword,
                                                              Host.EnableSMTPSSL);
                            if (string.IsNullOrEmpty(strMessage))
                            {
                                var objThread = new Thread(objSendBulkEMail.Send);
                                objThread.Start();
                                strResult = Localization.GetString("MessageSent", LocalResourceFile);
                                msgResult = ModuleMessage.ModuleMessageType.GreenSuccess;
                            }
                            else
                            {
                                strResult = Localization.GetString("NoMessagesSent", LocalResourceFile);
                                msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                            }
                        }
                    }
                }
                UI.Skins.Skin.AddModuleMessage(this, strResult, msgResult);
                ((CDefault) Page).ScrollToControl(Page.Form);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdPreview_Click(object sender, EventArgs e)
        {
            string strResult = "";
            ModuleMessage.ModuleMessageType msgResult;
            try
            {
                if (String.IsNullOrEmpty(txtSubject.Text) || String.IsNullOrEmpty(teMessage.Text))
                {
                    strResult = Localization.GetString("MessageValidation", LocalResourceFile);
                    msgResult = ModuleMessage.ModuleMessageType.YellowWarning;
                    UI.Skins.Skin.AddModuleMessage(this, strResult, msgResult);
                    ((CDefault) Page).ScrollToControl(Page.Form);
                    return;
                }
                string strBody = teMessage.Text;
                string pattern = "<(a|link|img|script|object).[^>]*(href|src|action)=(\\\"|'|)(?<url>(.[^\\\"']*))(\\\"|'|)[^>]*>";
                strBody = Regex.Replace(strBody, pattern, FormatUrls);
                if (chkReplaceTokens.Checked)
                {
                    var objTR = new TokenReplace();
                    if (cboSendMethod.SelectedItem.Value == "TO")
                    {
                        objTR.User = UserInfo;
                        objTR.AccessingUser = UserInfo;
                        objTR.DebugMessages = true;
                    }
                    lblPreviewSubject.Text = objTR.ReplaceEnvironmentTokens(txtSubject.Text);
                    lblPreviewBody.Text = objTR.ReplaceEnvironmentTokens(strBody);
                }
                else
                {
                    lblPreviewSubject.Text = txtSubject.Text;
                    lblPreviewBody.Text = strBody;
                }
                pnlPreview.Visible = true;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}