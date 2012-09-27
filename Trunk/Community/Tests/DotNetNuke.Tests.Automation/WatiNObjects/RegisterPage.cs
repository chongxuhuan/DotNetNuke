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
using System;
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The register page object.
    /// </summary>
    public class RegisterPage : WatiNBase
    {
        #region Constructors

        public RegisterPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public RegisterPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }
       
        #endregion

        #region Public Properties

        #region Links

        public Link CancelLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("cancelButton")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("cancelButton")));
            }
        }

        public Link RegisterButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("registerButton")));
                }
                return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("registerButton")));
            }
        }

        #endregion

        #region TextFields

        public TextField UserNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Username")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Username_TextBox")));
                }
               // return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Username")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Username_TextBox")));
            }
        }

        public TextField DisplayNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$DisplayName")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("DisplayName_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$DisplayName")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("DisplayName_TextBox")));
            }
        }

        public TextField EmailField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Email")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Email_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Email")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Email_TextBox")));
            }
        }

        public TextField PasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$txtPassword")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Password_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$txtPassword")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Password_TextBox")));
            }
        }

        public TextField ConfirmPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                   // return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$txtConfirm")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("PasswordConfirm_TextBox")));
                } 
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$txtConfirm")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("PasswordConfirm_TextBox")));
            }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a user.
        /// Clicks the register link
        /// Fills in the Register User form
        /// Clicks the register button
        /// </summary>
        /// <param name="userName">The users username.</param>
        /// <param name="password">The users password.</param>
        /// <param name="firstName">The users first name.</param>
        /// <param name="lastName">The users last name.</param>
        /// <param name="email">The users email address.</param>
        /// <param name="displayName">The users display name.</param>
        public void RegisterUser(string userName, string password, string firstName, string lastName, string email, string displayName)
        {
            RegisterLink.Click();
            UserNameField.Value = userName;
            DisplayNameField.Value = displayName;
            EmailField.Value = email;
            PasswordField.Value = password;
            ConfirmPasswordField.Value = password;
            RegisterButton.Click();
        }

        
        #endregion
    }
}