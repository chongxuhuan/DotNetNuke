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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The login module object.
    /// </summary>
    public class LoginPage : WatiNBase
    {
        #region Constructors

        public LoginPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName){}

        public LoginPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Divs
        public Div LoginPanelDiv
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById("dnn_ctr_Login_DNN"));
                }
                return IEInstance.Div(Find.ById("dnn_ctr_Login_DNN"));
            }
        }
        #endregion
        
        #region CheckBoxes
        public CheckBox RememberMeCheckBox { get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("ctr_Login_chkCookie"))); } }
        #endregion

        #region TextFields
        public TextField ForgotPasswordUserNameField { get { return IEInstance.TextField(Find.ById(s => s.EndsWith("ctr_SendPassword_txtUsername"))); } }
        public TextField UserNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("txtUsername")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("txtUsername")));
            }
        }
        public TextField PasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword"));
                }
                return IEInstance.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword"));
            }
        }

        public TextField OldPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("txtOldPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("txtOldPassword")));
            }
        }

        public TextField NewPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("txtNewPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("txtNewPassword")));
            }
        }

        public TextField ConfirmPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("txtNewConfirm")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("txtNewConfirm")));
            }
        }

        #endregion

        #region Links
        public Link SendPassworkLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("ctr_SendPassword_cmdSendPassword")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("ctr_SendPassword_cmdSendPassword")));
            }
        }
        public Link ForgotPasswordLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("ctr_Login_passwordLink")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("ctr_Login_passwordLink")));
            }
        }
        public Link LoginButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("ctr_Login_Login_DNN_cmdLogin")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("ctr_Login_Login_DNN_cmdLogin")));
            }
        }
        public Link UpdateButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("cmdUpdate")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("cmdUpdate")));
            }
        }

        #endregion

        #region Spans
        public Span LoginMessage
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Span(Find.ById(s => s.EndsWith("ctr_ctl00_lblMessage")));
                }
                return IEInstance.Span(Find.ById(s => s.EndsWith("ctr_ctl00_lblMessage")));
            }
        }
        public Span ErrorMsg
        {
            get
            {
                if (PopUpFrame != null)
                {
                    if (IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage"))).Count > 1)
                    {
                        return PopUpFrame.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage")))[2];
                    }
                    return PopUpFrame.Span(Find.ById(s => s.EndsWith("lblMessage")));
                }
                if (IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage"))).Count > 1)
                {
                    return IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("lblMessage")))[2];
                }
                return IEInstance.Span(Find.ById(s => s.EndsWith("lblMessage")));
            }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Enters a username and password into the login module.
        /// Doesn't login the user after entering these values.
        /// </summary>
        /// <param name="name">The username.</param>
        /// <param name="password">The password.</param>
        public void EnterUserLogin(string name, string password)
        {
            IEInstance.GoTo(SiteUrl);
            System.Threading.Thread.Sleep(1500);
            if (LoginLink.Text != "Login")
            {
                return;
            }

            LoginLink.Click();
            System.Threading.Thread.Sleep(1500);

            UserNameField.Value = name;
            PasswordField.Value = password;
        }

        /// <summary>
        /// Logs in a user.
        /// First navigates to the site home page, clicks on the login link, then enters the users username and password and clicks login.
        /// </summary>
        /// <param name="name">The username for the user.</param>
        /// <param name="password">The password for the user.</param>
        public void LoginUser(string name, string password)
        {
            IEInstance.GoTo(SiteUrl);
            System.Threading.Thread.Sleep(1500);
            if (LoginLink.Text != "Login")
            {
                return;
            }

            LoginLink.Click();
            System.Threading.Thread.Sleep(1500);

            UserNameField.Value = name;
            PasswordField.Value = password;

            LoginButton.ClickNoWait();
            System.Threading.Thread.Sleep(2500);
        }

        /// <summary>
        /// Logs in a user.
        /// Clicks on the login link, then enters the users username and password and clicks login.
        /// This method doesn't navigate to the site home page first.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public void LoginUserNoGoTo(string name, string password)
        {
            LoginLink.Click();

            UserNameField.Value = name;
            PasswordField.Value = password;

            LoginButton.Click();
        }
        
        /// <summary>
        /// Logs out a user by clicking on the login link.
        /// Won't check if a user is logged in or not. 
        /// </summary>
        public void LogoffUser()
        {
            LoginLink.ClickNoWait();
        }

        /// <summary>
        /// Enters the old password, new password and  confirmation password on the update password form.
        /// </summary>
        /// <param name="oldPassword">The users old password.</param>
        /// <param name="newPassword">The users new password.</param>
        /// <param name="confirmPassword">The users new password.</param>
        public void UpdatePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            OldPasswordField.Value = oldPassword;
            NewPasswordField.Value = newPassword;
            ConfirmPasswordField.Value = confirmPassword;
            UpdateButton.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            IEInstance.GoTo(SiteUrl);
        }

        #endregion
    }
}
