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
using System.Linq;
using System.Text;
using System.Threading;

using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        public LoginPage LoginPage
        {
            get
            {
                return GetPage<LoginPage>();
            }
        }
        /// <summary>
        /// Logs in as the admin.
        /// Uses the default values stored by the tests.
        /// username: 'admin' password: 'dnnhost'
        /// </summary>
        [Given("I have logged in as the admin")]
        [When("I have logged in as the admin")]
        public void GivenIHaveLoggedInAsTheAdmin()
        {
            LoginPage.LoginUser(TestUsers.Admin.UserName, TestUsers.Admin.Password);
        }

        /// <summary>
        /// Logs in as the admin.
        /// Uses the default values stored by the tests.
        /// username: 'host' password: 'dnnhost'
        /// </summary>
        [Given(@"I have logged in as the host")]
        [When("I have logged in as the host")]
        public void GivenIHaveLoggedInAsTheHost()
        {
            LoginPage.LoginUser(TestUsers.Host.UserName, TestUsers.Host.Password);
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Logs in as the user with the username and password specified.
        /// The user must already exist on the site.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        [Given("I have logged in as the user (.*) (.*)")]
        [When("I have logged in as the user (.*) (.*)")]
        public void GivenIHaveLoggedInAsTheUser(string userName, string password)
        {
            LoginPage.LoginUser(userName, password);
        }

        /// <summary>
        /// Asserts that the Login control is visible
        /// </summary>
        [Then("I should see the Login screen")]
        public void IShouldSeeTheLoginScreen()
        {
            var loginLink = LoginPage.LoginButton;
            WatiNAssert.AssertIsTrue(loginLink.Exists, "LoginScreenDoesNotShow.jpg");
        }

        /// <summary>
        /// Checks that a user has been logged in.
        /// </summary>
        [Then(@"I should be logged in")]
        public void ThenIShouldBeLoggedIn()
        {
            Thread.Sleep(1500);
            WatiNAssert.AssertIsTrue(LoginPage.LoginLink.Text.ToLower() == "logout", "UserNotLoggedIn.jpg");
        }

        /// <summary>
        /// Checks that a user has been logged in to the site and that the users display name is used in the profile link, instead of the text Register.
        /// Currently this step can only be used for the admin and the host.
        /// </summary>
        /// <param name="userName">The users username.</param>
        [Then(@"I should be logged in as the (.*) user")]
        public void ThenIShouldBeLoggedInAsTheUser(string userName)
        {
            Thread.Sleep(2500);
            WatiNAssert.AssertIsTrue(LoginPage.LoginLink.Text.ToLower() == "logout", userName + "NotLoggedIn.jpg", LoginPage.LoginLink.Text.ToLower());
            if (userName.Equals(TestUsers.Host.UserName))
            {
                WatiNAssert.AssertStringsAreEqual(TestUsers.Host.DisplayName, HomePage.RegisterLink.Text, "TestHostNotLoggedIn.jpg");
            }
            else if (userName.Equals(TestUsers.Admin.UserName))
            {
                WatiNAssert.AssertStringsAreEqual(TestUsers.Admin.DisplayName, HomePage.RegisterLink.Text, "AdminNotLoggedIn.jpg");
            }
        }

        /// <summary>
        /// Checks that a login error is displayed in the login module.
        /// </summary>
        [Then(@"I should see a login error")]
        public void ThenIShouldSeeALoginError()
        {
            WatiNAssert.AssertIsTrue(LoginPage.LoginMessage.Text.Contains("Login Failed"), "NoLoginError.jpg");
        }

        /// <summary>
        /// Checks that no user is currently logged in.
        /// </summary>
        [Then(@"I should not be logged in")]
        public void ThenIShouldNotBeLoggedIn()
        {
            IEInstance.GoTo(SiteUrl);
            WatiNAssert.AssertIsTrue(HomePage.LoginLink.Text.Equals("Login"), "UserLoggedIn.jpg");
        }
    }
}
