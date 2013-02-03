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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.UI.Skins.Controls;
using NUnit.Framework;
using DotNetNuke.Tests.Instance.Utilities;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class LoginSteps : WatiNTest
    {
        public LoginPage LoginPage
        {
            get
            {
                return GetPage<LoginPage>();
            }
        }

        /// <summary>
        /// Enters the admins username and password that are stored in the tests.
        /// This test requires that the admins password has been updated to the default stored by the tests.
        /// Tests using this step must include either the @MustBeDefaultAdminCredentialsForceUpdate tag or the @MustBeHostDefaultCredentials tag.
        /// </summary>
        [Given(@"I have entered the default Admin Username and the default password")]
        public void GivenIHaveEnteredTheDefaultAdminUsernameAndTheDefaultPassword()
        {
            LoginPage.EnterUserLogin(TestUsers.Admin.UserName, TestUsers.Admin.Password);
        }

        /// <summary>
        /// Clicks either the login link displayed on every page, or the login button on the user login page.
        /// </summary>
       [Given(@"I have pressed Login")]
       public void GivenIHavePressedLogin()
       {
           LoginPage.LoginLink.Click();
           Thread.Sleep(2500);
       }

        /// <summary>
        /// Clicks the login button on the user login page.
        /// </summary>
        [When(@"I press Login")]
        [Given(@"I press Login")]
        public void WhenIPressLogin()
        {
            LoginPage.LoginButton.Click();
        }

        /// <summary>
        /// Enters an invalid username (badUser) that is predefined into the username field in the login module.
        /// A user with this username must not already exist on the site.
        /// </summary>
        [When(@"I enter an Invalid username")]
        public void WhenIEnterAnInvalidUsername()
        {
            Thread.Sleep(1500);
            LoginPage.UserNameField.Value = TestUsers.Invalid.UserName;
        }

        /// <summary>
        /// Enters an invalid password (badUser) that is predefined into the password field in the login module.
        /// </summary>
        [When(@"I enter an Invalid Password")]
        public void WhenIEnterAnInvalidPassword()
        {
            LoginPage.PasswordField.Value = TestUsers.Invalid.Password;
        }

        /// <summary>
        /// This will enter the default host username (host) into the username field in the login module.
        /// </summary>
        [When(@"I enter the default host username")]
        public void WhenIEnterTheDefaultHostUsername()
        {
            Thread.Sleep(1500);
            LoginPage.UserNameField.Value = TestUsers.Host.UserName;
        }

        /// <summary>
        /// This will enter the default host password (dnnhost) into the username field in the login module. 
        /// </summary>
        [When(@"I enter the default host password")]
        public void WhenIEnterTheDefaultHostPassword()
        {
            LoginPage.PasswordField.Value = TestUsers.Host.Password;
        }
        
        /// <summary>
        /// This will enter the admins new password (password) into the new password and confirm password fields, then click the update link and navigate to the home page.
        /// </summary>
        [When(@"I enter and confirm my new password")]
        public void WhenIEnterAndConfirmMyNewPassword()
        {
            LoginPage.UpdatePassword(TestUsers.Admin.Password, TestUsers.Admin.UpdatedPassword, TestUsers.Admin.UpdatedPassword);
        }

        /// <summary>
        /// Checks that the user is required to update their password before they are able to login. 
        /// </summary>
        [Then(@"I should be forced to enter a new password to proceed")]
        public void ThenIShouldBeForcedToEnterANewPasswordToProceed()
        {
            Thread.Sleep(2500);
            WatiNAssert.AssertIsTrue(LoginPage.OldPasswordField.Exists, "UserNotForcedToChangePassword.jpg");
        }

    }
}
