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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Website.Specs
{
	[Binding]
    public class SuperAccountsSteps : WatiNTest
	{
		public HostUserPage HostUserPage
		{
			get
			{
                return GetPage<HostUserPage>();
			}
		}

		public FileManagerPage FileManagerPage
		{
			get
			{
                return GetPage<FileManagerPage>();
			}
		}

        [When(@"I click Add User")]
        public void WhenIClickAddUser()
        {
            if (HostUserPage.PopUpFrame != null || HostUserPage.ModuleTitleSpan.InnerHtml.Contains("Edit User Accounts"))
            {
                Assert.IsTrue(HostUserPage.AddNewUserLink.Exists);
                HostUserPage.AddNewUserLink.Click();
            }
            else
            {
                Assert.IsTrue(HostUserPage.AddNewUserStartLink.Exists);
                HostUserPage.AddNewUserStartLink.Click();
            }
            Thread.Sleep(1500);
        }

        [When(@"I fill in the user form")]
        public void WhenIFillInTheUserForm(TechTalk.SpecFlow.Table table)
        {
            Thread.Sleep(2500);
            HostUserPage.UserNameField.Value = table.Rows[0]["Value"];
            HostUserPage.FirstNameField.Value = table.Rows[1]["Value"];
            HostUserPage.LastNameField.Value = table.Rows[2]["Value"];
            HostUserPage.DisplayNameField.Value = table.Rows[3]["Value"];
            HostUserPage.EmailField.Value = table.Rows[4]["Value"];
            HostUserPage.PasswordField.Value = table.Rows[5]["Value"];
            HostUserPage.ConfirmPasswordField.Value = table.Rows[5]["Value"];
        }

        [When(@"I Apply a folder permission on user (.*)")]
        public void WhenIApplyAFolderPermissionOnUserTestuser(string user)
		{
			var usernameBox = FileManagerPage.ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("dgPermissions_txtUser")));
            usernameBox.Value = user;
			usernameBox.NextSibling.Click();
			Thread.Sleep(2500);
			FileManagerPage.ContentPaneDiv.Link(Find.ById(s => s.EndsWith("FileManager_cmdUpdate"))).Click();
			Thread.Sleep(2500);
		}

        [When(@"I delete (.*)")]
        public void WhenIDeleteTestuser(string user)
		{
			var approveConfirmDialog = new ConfirmDialogHandler();
            var deleteButton = HostUserPage.GetUserDeleteButton(user);
			//remove the confirm window because it can't handle by WatIn in IE9, will update this code
			//after update the confirm window with dnn jquery confirm plugin.
			deleteButton.SetAttributeValue("onclick", "return true");
			deleteButton.Click();
			Thread.Sleep(2500);
            var removeButton = HostUserPage.GetUserRemoveButton(user);
			removeButton.SetAttributeValue("onclick", "return true");
			removeButton.Click();

			Thread.Sleep(2500);
		}

		[Then(@"(.*) should delete successful without exceptions")]
		public void ThenTestuserShouldDeleteSuccessfulWithoutExceptions(string user)
		{
			Assert.AreEqual(null, HostUserPage.GetUserRow(user));
		}
	}
}
