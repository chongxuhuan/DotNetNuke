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
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Messaging;
using DotNetNuke.Services.Messaging.Data;

using Localize = DotNetNuke.Services.Localization.Localization;

#endregion

namespace DotNetNuke.Services.Mail
{
    public class Mail
    {
        public static string ConvertToText(string sHTML)
        {
            string sContent = sHTML;
            sContent = sContent.Replace("<br />", Environment.NewLine);
            sContent = sContent.Replace("<br>", Environment.NewLine);
            sContent = HtmlUtils.FormatText(sContent, true);
            return HtmlUtils.StripTags(sContent, true);
        }

        [Obsolete("Obsoleted in DotNetNuke 5.5. Use DotNetNuke.Common.Utilities.HtmlUtils.IsHtml()")]
        public static bool IsHTMLMail(string Body)
        {
            return HtmlUtils.IsHtml(Body);
        }

        public static bool IsValidEmailAddress(string Email, int portalid)
        {
            string pattern = Null.NullString;
            if (portalid != Null.NullInteger)
            {
                pattern = Convert.ToString(UserController.GetUserSettings(portalid)["Security_EmailValidation"]);
            }
            pattern = string.IsNullOrEmpty(pattern) ? Globals.glbEmailRegEx : pattern;
            return Regex.Match(Email, pattern).Success;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// <summary>Send an email notification</summary>
        /// </summary>
        /// <param name="user">The user to whom the message is being sent</param>
        /// <param name="msgType">The type of message being sent</param>
        /// <param name="settings">Portal Settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <history>
        ///     [cnurse]        09/29/2005  Moved to Mail class
        ///     [sLeupold]      02/07/2008 language used for admin mails corrected
        /// </history>
        /// -----------------------------------------------------------------------------
        public static string SendMail(UserInfo user, MessageType msgType, PortalSettings settings)
        {
            int toUser = user.UserID;
            string locale = user.Profile.PreferredLocale;
            string subject = "";
            string body = "";
            ArrayList custom = null;
            switch (msgType)
            {
                case MessageType.UserRegistrationAdmin:
                    subject = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY";
                    toUser = settings.AdministratorId;
                    UserInfo admin = UserController.GetUserById(settings.PortalId, settings.AdministratorId);
                    locale = admin.Profile.PreferredLocale;
                    break;
                case MessageType.UserRegistrationPrivate:
                    subject = "EMAIL_USER_REGISTRATION_PRIVATE_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_PRIVATE_BODY";
                    break;
                case MessageType.UserRegistrationPublic:
                    subject = "EMAIL_USER_REGISTRATION_PUBLIC_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_PUBLIC_BODY";
                    break;
                case MessageType.UserRegistrationVerified:
                    subject = "EMAIL_USER_REGISTRATION_VERIFIED_SUBJECT";
                    body = "EMAIL_USER_REGISTRATION_VERIFIED_BODY";
                    if (HttpContext.Current != null)
                    {
                        custom = new ArrayList();
                        custom.Add(HttpContext.Current.Server.UrlEncode(user.Username));
                    }
                    break;
                case MessageType.PasswordReminder:
                    subject = "EMAIL_PASSWORD_REMINDER_SUBJECT";
                    body = "EMAIL_PASSWORD_REMINDER_BODY";
                    break;
                case MessageType.ProfileUpdated:
                    subject = "EMAIL_PROFILE_UPDATED_SUBJECT";
                    body = "EMAIL_PROFILE_UPDATED_BODY";
                    break;
                default:
                    subject = "EMAIL_USER_UPDATED_OWN_PASSWORD_SUBJECT";
                    body = "EMAIL_USER_UPDATED_OWN_PASSWORD_BODY";
                    break;
            }
            subject = Localize.GetSystemMessage(locale, settings, subject, user, Localize.GlobalResourceFile, custom, "", settings.AdministratorId);
            body = Localize.GetSystemMessage(locale, settings, body, user, Localize.GlobalResourceFile, custom, "", settings.AdministratorId);
            SendEmail(settings.Email, UserController.GetUserById(settings.PortalId, toUser).Email, subject, body);
            return Null.NullString;
        }

        public static string SendMail(string MailFrom, string MailTo, string Bcc, string Subject, string Body, string Attachment, string BodyType, string SMTPServer, string SMTPAuthentication,
                                      string SMTPUsername, string SMTPPassword)
        {
            MailFormat objBodyFormat = MailFormat.Text;
            if (!String.IsNullOrEmpty(BodyType))
            {
                switch (BodyType.ToLower())
                {
                    case "html":
                        objBodyFormat = MailFormat.Html;
                        break;
                    case "text":
                        objBodyFormat = MailFormat.Text;
                        break;
                }
            }
            return SendMail(MailFrom, MailTo, "", Bcc, MailPriority.Normal, Subject, objBodyFormat, Encoding.UTF8, Body, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword);
        }

        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body,
                                      string Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword)
        {
            bool SMTPEnableSSL = Host.EnableSMTPSSL;
            return SendMail(MailFrom, MailTo, Cc, Bcc, Priority, Subject, BodyFormat, BodyEncoding, Body, Attachment, SMTPServer, SMTPAuthentication, SMTPUsername, SMTPPassword, SMTPEnableSSL);
        }

        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body,
                                      string Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            return SendMail(MailFrom,
                            MailTo,
                            Cc,
                            Bcc,
                            MailFrom,
                            Priority,
                            Subject,
                            BodyFormat,
                            BodyEncoding,
                            Body,
                            Attachment.Split('|'),
                            SMTPServer,
                            SMTPAuthentication,
                            SMTPUsername,
                            SMTPPassword,
                            SMTPEnableSSL);
        }

        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding, string Body,
                                      string[] Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            return SendMail(MailFrom,
                            MailTo,
                            Cc,
                            Bcc,
                            MailFrom,
                            Priority,
                            Subject,
                            BodyFormat,
                            BodyEncoding,
                            Body,
                            Attachment,
                            SMTPServer,
                            SMTPAuthentication,
                            SMTPUsername,
                            SMTPPassword,
                            SMTPEnableSSL);
        }

        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, string ReplyTo, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding,
                                      string Body, string[] Attachment, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            var attachments = new List<Attachment>();
            foreach (string myAtt in Attachment)
            {
                if (!String.IsNullOrEmpty(myAtt))
                {
                    attachments.Add(new Attachment(myAtt));
                }
            }
            return SendMail(MailFrom,
                            MailTo,
                            Cc,
                            Bcc,
                            ReplyTo,
                            Priority,
                            Subject,
                            BodyFormat,
                            BodyEncoding,
                            Body,
                            attachments,
                            SMTPServer,
                            SMTPAuthentication,
                            SMTPUsername,
                            SMTPPassword,
                            SMTPEnableSSL);
        }

        public static string SendMail(string MailFrom, string MailTo, string Cc, string Bcc, string ReplyTo, MailPriority Priority, string Subject, MailFormat BodyFormat, Encoding BodyEncoding,
                                      string Body, List<Attachment> Attachments, string SMTPServer, string SMTPAuthentication, string SMTPUsername, string SMTPPassword, bool SMTPEnableSSL)
        {
            string retValue = "";
            if (!IsValidEmailAddress(MailFrom, PortalSettings.Current != null ? PortalSettings.Current.PortalId : Null.NullInteger))
            {
                var ex = new ArgumentException(string.Format(Localize.GetString("EXCEPTION_InvalidEmailAddress", PortalSettings.Current), MailFrom));
                Exceptions.Exceptions.LogException(ex);
                return ex.Message;
            }

            if (string.IsNullOrEmpty(SMTPServer) && !string.IsNullOrEmpty(Host.SMTPServer))
            {
                SMTPServer = Host.SMTPServer;
            }
            if (string.IsNullOrEmpty(SMTPAuthentication) && !string.IsNullOrEmpty(Host.SMTPAuthentication))
            {
                SMTPAuthentication = Host.SMTPAuthentication;
            }
            if (string.IsNullOrEmpty(SMTPUsername) && !string.IsNullOrEmpty(Host.SMTPUsername))
            {
                SMTPUsername = Host.SMTPUsername;
            }
            if (string.IsNullOrEmpty(SMTPPassword) && !string.IsNullOrEmpty(Host.SMTPPassword))
            {
                SMTPPassword = Host.SMTPPassword;
            }
            MailTo = MailTo.Replace(";", ",");
            Cc = Cc.Replace(";", ",");
            Bcc = Bcc.Replace(";", ",");
            MailMessage objMail = null;
            try
            {
                objMail = new MailMessage();
                objMail.From = new MailAddress(MailFrom);
                if (!String.IsNullOrEmpty(MailTo))
                {
                    objMail.To.Add(MailTo);
                }
                if (!String.IsNullOrEmpty(Cc))
                {
                    objMail.CC.Add(Cc);
                }
                if (!String.IsNullOrEmpty(Bcc))
                {
                    objMail.Bcc.Add(Bcc);
                }
                if (ReplyTo != string.Empty)
                {
                    objMail.ReplyTo = new MailAddress(ReplyTo);
                }
                objMail.Priority = (System.Net.Mail.MailPriority) Priority;
                objMail.IsBodyHtml = BodyFormat == MailFormat.Html;
                foreach (Attachment myAtt in Attachments)
                {
                    objMail.Attachments.Add(myAtt);
                }
                objMail.SubjectEncoding = BodyEncoding;
                objMail.Subject = HtmlUtils.StripWhiteSpace(Subject, true);
                objMail.BodyEncoding = BodyEncoding;
                AlternateView PlainView = AlternateView.CreateAlternateViewFromString(ConvertToText(Body), null, "text/plain");
                objMail.AlternateViews.Add(PlainView);
                if (objMail.IsBodyHtml)
                {
                    AlternateView HTMLView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                    objMail.AlternateViews.Add(HTMLView);
                }
            }
            catch (Exception objException)
            {
                retValue = MailTo + ": " + objException.Message;
                Exceptions.Exceptions.LogException(objException);
            }
            if (objMail != null)
            {
                int SmtpPort = Null.NullInteger;
                int portPos = SMTPServer.IndexOf(":");
                if (portPos > -1)
                {
                    SmtpPort = Int32.Parse(SMTPServer.Substring(portPos + 1, SMTPServer.Length - portPos - 1));
                    SMTPServer = SMTPServer.Substring(0, portPos);
                }
                var smtpClient = new SmtpClient();
                try
                {
                    if (!String.IsNullOrEmpty(SMTPServer))
                    {
                        smtpClient.Host = SMTPServer;
                        if (SmtpPort > Null.NullInteger)
                        {
                            smtpClient.Port = SmtpPort;
                        }
                        switch (SMTPAuthentication)
                        {
                            case "":
                            case "0":
                                break;
                            case "1":
                                if (!String.IsNullOrEmpty(SMTPUsername) && !String.IsNullOrEmpty(SMTPPassword))
                                {
                                    smtpClient.UseDefaultCredentials = false;
                                    smtpClient.Credentials = new NetworkCredential(SMTPUsername, SMTPPassword);
                                }
                                break;
                            case "2":
                                smtpClient.UseDefaultCredentials = true;
                                break;
                        }
                    }
                    smtpClient.EnableSsl = SMTPEnableSSL;
                    smtpClient.Send(objMail);
                    retValue = "";
                }
                catch (SmtpFailedRecipientException exc)
                {
                    retValue = string.Format(Localize.GetString("FailedRecipient"), exc.FailedRecipient);
                    Exceptions.Exceptions.LogException(exc);
                }
                catch (SmtpException exc)
                {
                    retValue = Localize.GetString("SMTPConfigurationProblem");
                    Exceptions.Exceptions.LogException(exc);
                }
                catch (Exception objException)
                {
                    if (objException.InnerException != null)
                    {
                        retValue = string.Concat(objException.Message, Environment.NewLine, objException.InnerException.Message);
                        Exceptions.Exceptions.LogException(objException.InnerException);
                    }
                    else
                    {
                        retValue = objException.Message;
                        Exceptions.Exceptions.LogException(objException);
                    }
                }
                finally
                {
                    objMail.Dispose();
                }
            }
            return retValue;
        }

        public static void SendEmail(string fromAddress, string toAddress, string subject, string body)
        {
            SendEmail(fromAddress, fromAddress, toAddress, subject, body);
        }

        public static void SendEmail(string fromAddress, string senderAddress, string toAddress, string subject, string body)
        {
            if ((string.IsNullOrEmpty(Host.SMTPServer)))
            {
                //throw new InvalidOperationException("SMTP Server not configured");
                return;
            }


            var emailMessage = new MailMessage(fromAddress, toAddress);
            emailMessage.SubjectEncoding = Encoding.UTF8;
            emailMessage.BodyEncoding = Encoding.UTF8;
            emailMessage.Subject = subject;
            emailMessage.Body = body;
            emailMessage.Sender = new MailAddress(senderAddress);

            if (HtmlUtils.IsHtml(body))
            {
                emailMessage.IsBodyHtml = true;
            }

            var smtpClient = new SmtpClient(Host.SMTPServer);

            string[] smtpHostParts = Host.SMTPServer.Split(':');
            if (smtpHostParts.Length > 1)
            {
                smtpClient.Host = smtpHostParts[0];
                smtpClient.Port = Convert.ToInt32(smtpHostParts[1]);
            }


            switch (Host.SMTPAuthentication)
            {
                case "":
                case "0":
                    // anonymous
                    break;
                case "1":
                    // basic
                    if (!string.IsNullOrEmpty(Host.SMTPUsername) && !string.IsNullOrEmpty(Host.SMTPPassword))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(Host.SMTPUsername, Host.SMTPPassword);
                    }
                    break;
                case "2":
                    // NTLM
                    smtpClient.UseDefaultCredentials = true;
                    break;
            }

            smtpClient.EnableSsl = Host.EnableSMTPSSL;

            //Retry up to 5 times to send the message
            for (int index = 0; index < 5; index++)
            {
                try
                {
                    smtpClient.Send(emailMessage);
                    return;
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);
                    if (index == 5)
                    {
                        throw;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        public static string SendEmail(string fromAddress, string senderAddress, string toAddress, string subject, string body, List<Attachment> Attachments)
        {
            if ((string.IsNullOrEmpty(Host.SMTPServer)))
            {
                return "SMTP Server not configured";
            }

            var emailMessage = new MailMessage(fromAddress, toAddress, subject, body);
            emailMessage.Sender = new MailAddress(senderAddress);

            if ((HtmlUtils.IsHtml(body)))
            {
                emailMessage.IsBodyHtml = true;
            }

            foreach (Attachment myAtt in Attachments)
            {
                emailMessage.Attachments.Add(myAtt);
            }

            var smtpClient = new SmtpClient(Host.SMTPServer);

            string[] smtpHostParts = Host.SMTPServer.Split(':');
            if (smtpHostParts.Length > 1)
            {
                smtpClient.Host = smtpHostParts[0];
                smtpClient.Port = Convert.ToInt32(smtpHostParts[1]);
            }


            switch (Host.SMTPAuthentication)
            {
                case "":
                case "0":
                    // anonymous
                    break;
                case "1":
                    // basic
                    if (!string.IsNullOrEmpty(Host.SMTPUsername) && !string.IsNullOrEmpty(Host.SMTPPassword))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(Host.SMTPUsername, Host.SMTPPassword);
                    }
                    break;
                case "2":
                    // NTLM
                    smtpClient.UseDefaultCredentials = true;
                    break;
            }

            smtpClient.EnableSsl = Host.EnableSMTPSSL;

            //'Retry up to 5 times to send the message
            for (int index = 1; index <= 5; index++)
            {
                try
                {
                    smtpClient.Send(emailMessage);
                    return "";
                }
                catch (Exception exc)
                {
                    Instrumentation.DnnLog.Error(exc);

                    if ((index == 5))
                    {
                        throw;
                    }
                    Thread.Sleep(1000);
                }
            }

            return "";
        }

        internal static bool RouteToUserMessaging(string MailFrom, string MailTo, string Cc, string Bcc, string Subject, string Body, List<Attachment> Attachments)
        {
            int totalRecords = -1;
            ArrayList fromUsersList = UserController.GetUsersByEmail(PortalSettings.Current.PortalId, MailFrom, -1, -1, ref totalRecords);
            UserInfo fromUser = default(UserInfo);
            if ((fromUsersList.Count != 0))
            {
                fromUser = (UserInfo) fromUsersList[0];
            }
            else
            {
                return false;
            }

            var ToEmails = new List<string>();
            var ToUsers = new List<UserInfo>();

            if ((!string.IsNullOrEmpty(MailTo)))
            {
                ToEmails.AddRange(MailTo.Split(';', ','));
            }

            if ((!string.IsNullOrEmpty(Cc)))
            {
                ToEmails.AddRange(Cc.Split(';', ','));
            }

            if ((!string.IsNullOrEmpty(Bcc)))
            {
                ToEmails.AddRange(Bcc.Split(';', ','));
            }

            foreach (string email in ToEmails)
            {
                if ((!string.IsNullOrEmpty(email)))
                {
                    ArrayList toUsersList = UserController.GetUsersByEmail(PortalSettings.Current.PortalId, email, -1, -1, ref totalRecords);
                    if ((toUsersList.Count != 0))
                    {
                        ToUsers.Add((UserInfo) toUsersList[0]);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var messageController = new MessagingController();

            foreach (UserInfo recepient in ToUsers)
            {
                var message = new Message();
                message.FromUserID = fromUser.UserID;
                message.Subject = Subject;
                message.Body = Body;
                message.ToUserID = recepient.UserID;
                message.Status = MessageStatusType.Unread;

                messageController.SaveMessage(message);
            }
            return true;
        }
    }
}
