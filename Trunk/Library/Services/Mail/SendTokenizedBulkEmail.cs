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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Messaging.Data;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.Services.Mail
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// SendTokenizedBulkEmail Class is a class to manage the sending of bulk mails
    /// that contains tokens, which might be replaced with individual user properties
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [sleupold]	8/15/2007	created to support tokens and localisation
    ///     [sleupold]  9/09/2007   refactored interface for enhanced type safety
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SendTokenizedBulkEmail
    {
        #region AddressMethods enum

        /// <summary>
        /// Addressing Methods (personalized or hidden)
        /// </summary>
        public enum AddressMethods
        {
            Send_TO = 1,
            Send_BCC = 2,
            Send_Relay = 3
        }

        #endregion

        private readonly List<string> _addressedRoles = new List<string>();
        private readonly List<UserInfo> _addressedUsers = new List<UserInfo>();
        private readonly List<string> _attachments = new List<string>();
        private UserInfo _replyToUser;
        private bool _smtpEnableSSL;
        private TokenReplace _tokenReplace;
        private PortalSettings _portalSettings;
        private UserInfo _sendingUser;
        private string _body = "";
        private string _confirmBodyHTML;
        private string _confirmBodyText;
        private string _confirmSubject;
        private string _noError;
        private string _relayEmail;
        private string _smtpAuthenticationMethod = "";
        private string _smtpPassword = "";
        private string _smtpServer = "";
        private string _smtpUsername = "";
        private string _strSenderLanguage;

        public SendTokenizedBulkEmail()
        {
            ReportRecipients = true;
            AddressMethod = AddressMethods.Send_TO;
            BodyFormat = MailFormat.Text;
            Subject = "";
            Priority = MailPriority.Normal;
            Initialize();
        }

        public SendTokenizedBulkEmail(List<string> addressedRoles, List<UserInfo> addressedUsers, bool removeDuplicates, string subject, string body)
        {
            ReportRecipients = true;
            AddressMethod = AddressMethods.Send_TO;
            BodyFormat = MailFormat.Text;
            Priority = MailPriority.Normal;
            _addressedRoles = addressedRoles;
            _addressedUsers = addressedUsers;
            RemoveDuplicates = removeDuplicates;
            Subject = subject;
            Body = body;
            SuppressTokenReplace = SuppressTokenReplace;
            Initialize();
        }

		/// <summary>
		/// Priority of emails to be sent
		/// </summary>
        public MailPriority Priority { get; set; }

		/// <summary>
		/// Subject of the emails to be sent
		/// </summary>
		/// <remarks>may contain tokens</remarks>
        public string Subject { get; set; }

		/// <summary>
		/// body text of the email to be sent
		/// </summary>
		/// <remarks>may contain HTML tags and tokens. Side effect: sets BodyFormat autmatically</remarks>
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
                BodyFormat = HtmlUtils.IsHtml(_body) ? MailFormat.Html : MailFormat.Text;
            }
        }

		/// <summary>format of body text for the email to be sent.</summary>
		/// <remarks>by default activated, if tokens are found in Body and subject.</remarks>
        public MailFormat BodyFormat { get; set; }

		/// <summary>address method for the email to be sent (TO or BCC)</summary>
		/// <remarks>TO is default value</remarks>
        public AddressMethods AddressMethod { get; set; }

		/// <summary>portal alias http path to be used for links to images, ...</summary>
        public string PortalAlias { get; set; }

        /// <summary>UserInfo of the user sending the mail</summary>
        /// <remarks>if not set explicitely, currentuser will be used</remarks>
        public UserInfo SendingUser
        {
            get
            {
                return _sendingUser;
            }
            set
            {
                _sendingUser = value;
                if (_sendingUser.Profile.PreferredLocale != null)
                {
                    _strSenderLanguage = _sendingUser.Profile.PreferredLocale;
                }
                else
                {
                    PortalSettings portalSettings = PortalController.GetCurrentPortalSettings();
                    _strSenderLanguage = portalSettings.DefaultLanguage;
                }
            }
        }

        /// <summary>email of the user to be shown in the mail as replyTo address</summary>
        /// <remarks>if not set explicitely, sendingUser will be used</remarks>
        public UserInfo ReplyTo
        {
            get
            {
                return _replyToUser ?? SendingUser;
            }
            set
            {
                _replyToUser = value;
            }
        }

		 /// <summary>shall duplicate email addresses be ignored? (default value: false)</summary>
		 /// <remarks>Duplicate Users (e.g. from multiple role selections) will always be ignored.</remarks>
        public bool RemoveDuplicates { get; set; }

		 /// <summary>Shall automatic TokenReplace be prohibited?</summary>
		 /// <remarks>default value: false</remarks>
        public bool SuppressTokenReplace { get; set; }

		 /// <summary>Shall List of recipients appended to confirmation report?</summary>
		 /// <remarks>enabled by default.</remarks>
        public bool ReportRecipients { get; set; }

        public string RelayEmailAddress
        {
            get
            {
                return AddressMethod == AddressMethods.Send_Relay ? _relayEmail : string.Empty;
            }
            set
            {
                _relayEmail = value;
            }
        }

        public string[] LanguageFilter { get; set; }

        private void Initialize()
        {
            _portalSettings = PortalController.GetCurrentPortalSettings();
            PortalAlias = _portalSettings.PortalAlias.HTTPAlias;
            SendingUser = (UserInfo) HttpContext.Current.Items["UserInfo"];
            _tokenReplace = new TokenReplace();
            _confirmBodyHTML = Localization.Localization.GetString("EMAIL_BulkMailConf_Html_Body", Localization.Localization.GlobalResourceFile, _strSenderLanguage);
            _confirmBodyText = Localization.Localization.GetString("EMAIL_BulkMailConf_Text_Body", Localization.Localization.GlobalResourceFile, _strSenderLanguage);
            _confirmSubject = Localization.Localization.GetString("EMAIL_BulkMailConf_Subject", Localization.Localization.GlobalResourceFile, _strSenderLanguage);
            _noError = Localization.Localization.GetString("NoErrorsSending", Localization.Localization.GlobalResourceFile, _strSenderLanguage);
            _smtpEnableSSL = Host.EnableSMTPSSL;
        }

        private void SendConfirmationMail(int numRecipients, int numMessages, int numErrors, string subject, string startedAt, string mailErrors, string recipientList)
        {
            var parameters = new ArrayList
                                 {
                                     startedAt,
                                     numRecipients.ToString(),
                                     numMessages >= 0 ? numMessages.ToString() : "***",
                                     DateTime.Now.ToString(),
                                     numErrors > 0 ? numErrors.ToString() : "",
                                     mailErrors != string.Empty ? mailErrors : _noError,
                                     ReportRecipients ? recipientList : ""
                                 };
            _tokenReplace.User = _sendingUser;
            string body = _tokenReplace.ReplaceEnvironmentTokens(BodyFormat == MailFormat.Html ? _confirmBodyHTML : _confirmBodyText, parameters, "Custom");
            string strSubject = string.Format(_confirmSubject, subject);
            if (!SuppressTokenReplace)
            {
                strSubject = _tokenReplace.ReplaceEnvironmentTokens(strSubject);
            }
            var message = new Message {FromUserID = _sendingUser.UserID, ToUserID = _sendingUser.UserID, Subject = strSubject, Body = body, Status = MessageStatusType.Unread};

            Mail.SendEmail(_sendingUser.Email, _sendingUser.Email, message.Subject, message.Body);
        }

        private bool MatchLanguageFilter(string userLanguage)
        {
            if (String.IsNullOrEmpty(userLanguage))
            {
                return true;
            }
            if (LanguageFilter == null || LanguageFilter.Length == 0)
            {
                return true;
            }
            return LanguageFilter.Any(s => userLanguage.ToLowerInvariant().StartsWith(s.ToLowerInvariant() + "*"));
        }

        private void ConditionallyAddUser(UserInfo user, ref List<string> keyList, ref List<UserInfo> userList)
        {
            if (((user.UserID <= 0 || user.Membership.Approved) && user.Email != string.Empty) && MatchLanguageFilter(user.Profile.PreferredLocale))
            {
                string key;
                if (RemoveDuplicates || user.UserID == Null.NullInteger)
                {
                    key = user.Email;
                }
                else
                {
                    key = user.UserID.ToString();
                }
                if (key != string.Empty && !keyList.Contains(key))
                {
                    userList.Add(user);
                    keyList.Add(key);
                }
            }
        }

        public bool SetSMTPServer(string smtpServer, string smtpAuthentication, string smtpUsername, string smtpPassword, bool smtpEnableSSL)
        {
            _smtpServer = smtpServer;
            _smtpAuthenticationMethod = smtpAuthentication;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _smtpEnableSSL = smtpEnableSSL;
            return true;
        }

        public void AddAttachment(string pAttachment)
        {
            _attachments.Add(pAttachment);
        }

        public void AddAddressedUser(UserInfo recipient)
        {
            _addressedUsers.Add(recipient);
        }

        public void AddAddressedRole(string roleName)
        {
            _addressedRoles.Add(roleName);
        }

        public List<UserInfo> Recipients()
        {
            var userList = new List<UserInfo>();
            var keyList = new List<string>();
            var roleController = new RoleController();
            foreach (string strRolename in _addressedRoles)
            {
                var roleInfo = roleController.GetRoleByName(_portalSettings.PortalId, strRolename);
                foreach (UserInfo objUser in roleController.GetUsersByRoleName(_portalSettings.PortalId, strRolename))
                {
                    UserInfo user = objUser;
                    ProfileController.GetUserProfile(ref user);
                    var userRole = roleController.GetUserRole(_portalSettings.PortalId, objUser.UserID, roleInfo.RoleID);
                    if ((userRole.EffectiveDate <= DateTime.Now || Null.IsNull(userRole.EffectiveDate)) && (userRole.ExpiryDate >= DateTime.Now || Null.IsNull(userRole.ExpiryDate)))
                    {
                        ConditionallyAddUser(objUser, ref keyList, ref userList);
                    }
                }
            }
            foreach (UserInfo objUser in _addressedUsers)
            {
                ConditionallyAddUser(objUser, ref keyList, ref userList);
            }
            return userList;
        }

        public int SendMails()
        {
            int recipients = 0;
            int messagesSent = 0;
            int errors = 0;
            try
            {
                string body = _body;
                if (BodyFormat == MailFormat.Html)
                {
                    body = "<Base Href='" + PortalAlias + "'>" + body;
                }
                string subject = Subject;
                string startedAt = DateTime.Now.ToString();
                bool replaceTokens = !SuppressTokenReplace && (_tokenReplace.ContainsTokens(Subject) || _tokenReplace.ContainsTokens(_body));
                bool individualSubj = false;
                bool individualBody = false;
                var mailErrors = new StringBuilder();
                var mailRecipients = new StringBuilder();
                switch (AddressMethod)
                {
                    case AddressMethods.Send_TO:
                    case AddressMethods.Send_Relay:
                        if (replaceTokens)
                        {
                            individualBody = (_tokenReplace.Cacheability(_body) == CacheLevel.notCacheable);
                            individualSubj = (_tokenReplace.Cacheability(Subject) == CacheLevel.notCacheable);
                            if (!individualBody)
                            {
                                body = _tokenReplace.ReplaceEnvironmentTokens(body);
                            }
                            if (!individualSubj)
                            {
                                subject = _tokenReplace.ReplaceEnvironmentTokens(subject);
                            }
                        }
                        foreach (UserInfo user in Recipients())
                        {
                            recipients += 1;
                            if (individualBody || individualSubj)
                            {
                                _tokenReplace.User = user;
                                _tokenReplace.AccessingUser = user;
                                if (individualBody)
                                {
                                    body = _tokenReplace.ReplaceEnvironmentTokens(_body);
                                }
                                if (individualSubj)
                                {
                                    subject = _tokenReplace.ReplaceEnvironmentTokens(Subject);
                                }
                            }
                            string recipient = AddressMethod == AddressMethods.Send_TO ? user.Email : RelayEmailAddress;
                            string mailError = Mail.SendMail(_sendingUser.Email,
                                                                recipient,
                                                                "",
                                                                "",
                                                                ReplyTo.Email,
                                                                Priority,
                                                                subject,
                                                                BodyFormat,
                                                                Encoding.UTF8,
                                                                body,
                                                                _attachments.ToArray(),
                                                                _smtpServer,
                                                                _smtpAuthenticationMethod,
                                                                _smtpUsername,
                                                                _smtpPassword,
                                                                _smtpEnableSSL);
                            if (!string.IsNullOrEmpty(mailError))
                            {
                                mailErrors.Append(mailError);
                                mailErrors.AppendLine();
                                errors += 1;
                            }
                            else
                            {
                                mailRecipients.Append(user.Email);
                                mailRecipients.Append(BodyFormat == MailFormat.Html ? "<br />" : Environment.NewLine);
                                messagesSent += 1;
                            }
                        }

                        break;
                    case AddressMethods.Send_BCC:
                        var distributionList = new StringBuilder();
                        messagesSent = Null.NullInteger;
                        foreach (UserInfo user in Recipients())
                        {
                            recipients += 1;
                            distributionList.Append(user.Email + "; ");
                            mailRecipients.Append(user.Email);
                            mailRecipients.Append(BodyFormat == MailFormat.Html ? "<br />" : Environment.NewLine);
                        }

                        if (distributionList.Length > 2)
                        {
                            if (replaceTokens)
                            {
                                var tr = new TokenReplace(Scope.Configuration);
                                body = tr.ReplaceEnvironmentTokens(_body);
                                subject = tr.ReplaceEnvironmentTokens(Subject);
                            }
                            else
                            {
                                body = _body;
                                subject = Subject;
                            }
                            string mailError = Mail.SendMail(_sendingUser.Email,
                                                       _sendingUser.Email,
                                                       "",
                                                       distributionList.ToString(0, distributionList.Length - 2),
                                                       ReplyTo.Email,
                                                       Priority,
                                                       subject,
                                                       BodyFormat,
                                                       Encoding.UTF8,
                                                       body,
                                                       _attachments.ToArray(),
                                                       _smtpServer,
                                                       _smtpAuthenticationMethod,
                                                       _smtpUsername,
                                                       _smtpPassword,
                                                       _smtpEnableSSL);
                            if (mailError == string.Empty)
                            {
                                messagesSent = 1;
                            }
                            else
                            {
                                mailErrors.Append(mailError);
                                errors += 1;
                            }
                        }
                        break;
                }
                if (mailErrors.Length > 0)
                {
                    mailRecipients = new StringBuilder();
                }
                SendConfirmationMail(recipients, messagesSent, errors, subject, startedAt, mailErrors.ToString(), mailRecipients.ToString());
            }
            catch (Exception exc)
            {
                Instrumentation.DnnLog.Error(exc);

                Debug.Write(exc.Message);
            }
            return messagesSent;
        }

        public void Send()
        {
            SendMails();
        }
    }
}
