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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class DeleteUserSteps : WatiNTest
    {
        public UserPage UserPage
        {
            get
            {
                return GetPage<UserPage>();
            }
        }

        public HostUserPage HostUserPage
        {
            get
            {
                return GetPage<HostUserPage>();
            }
        }

        public LoginPage LoginPage
        {
            get
            {
                return GetPage<LoginPage>();
            }
        }

        /// <summary>
        /// Clicks the delete icon from the user table for the user with the display name specified, and confirms the deletion in the pop up window.
        /// This statement will contain what looks like two staements, because of the nature of handling Internet Explorer pop ups with WatiN.
        /// To handle the pop up the initial delete click and the confirmation must occur together, so these statements have been combined into one statement.
        /// In cases where there will be a DNN pop up, this pattern does not need to be followed.
        /// </summary>
        /// <param name="displayName">The display name of the user that will be deleted.</param>
        [When(@"I click Delete in the user table for user (.*) and confirm the deletion")]
        public void WhenIClickDeleteInTheUserTableAndConfirmTheDeletion(string displayName)
        {
            Thread.Sleep(1500);
            UserPage.AllDisplayLink.ClickNoWait();
            Thread.Sleep(1000);
            var dialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(IEInstance.DialogWatcher, dialog))
            {
                UserPage.GetUserDeleteButton(displayName).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Clicks the delete icon from the Superuser table for the Superuser with the display name specified.
        /// Confirms the deletion in the pop up window.
        /// This statement will contain what looks like two staements, because of the nature of handling Internet Explorer pop ups with WatiN.
        /// To handle the pop up the initial delete click and the confirmation must occur together, so these statements have been combined into one statement.
        /// In cases where there will be a DNN pop up, this pattern does not need to be followed.
        /// </summary>
        /// <param name="displayName">The display name of the user that will be deleted.</param>
        [When(@"I click Delete in the Super User table for (.*) and confirm the deletion")]
        public void WhenIClickDeleteInTheSuperUserTableAndConfirmTheDeletion(string displayName)
        {
            Thread.Sleep(1500);
            HostUserPage.AllDisplayLink.ClickNoWait();
            Thread.Sleep(1000);
            var dialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(IEInstance.DialogWatcher, dialog))
            {
                HostUserPage.GetUserDeleteButton(displayName).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Checks that the user is displayed as being soft deleted in the user table.
        /// Checks that the user will not be able to login anymore. 
        /// </summary>
        /// <param name="userName">The username of the deleted user.</param>
        /// <param name="password">The password of the deleted user.</param>
        /// <param name="displayName">The display name of the deleted user.</param>
        [Then(@"The User Account (.*) with password (.*) and display name (.*) is deleted from the site")]
        public void ThenUserIsDeletedFromTheSite(string userName, string password, string displayName)
        {
            WatiNAssert.AssertIsTrue(UserPage.CheckUserDisplaysAsDeleted(displayName), displayName + "InUserTable.jpg");
            LoginPage.LogoffUser();
            Thread.Sleep(1500);
            LoginPage.LoginUser(userName, password);
            WatiNAssert.AssertIsTrue(LoginPage.LoginMessage.InnerHtml.Contains("Login Failed."), userName + "AbleToLogin.jpg");
        }
    }
}
