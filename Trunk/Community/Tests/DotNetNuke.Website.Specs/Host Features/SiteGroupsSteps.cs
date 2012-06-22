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

namespace DotNetNuke.Website.Specs.Host_Features
{
    [Binding]
    public class SiteGroupsSteps : WatiNTest
    {
        public HostSiteManagementPage SiteManagementPage
        {
            get
            {
                return GetPage<HostSiteManagementPage>();
            }
        }

        public HostProSiteGroupPage SiteGroupPage
        {
            get
            {
                return GetPage<HostProSiteGroupPage>();
            }
        }

        public UserPage UserPage
        {
            get
            {
                return GetPage<UserPage>();
            }
        }

        [When(@"I create a new child portal")]
        public void WhenICreateANewChildPortal()
        {
            if (SiteManagementPage.GetPortalAliasCellByPortalName("Child") != null)
            {
                return;
            }

            SiteManagementPage.AddNewSiteLink.Click();
            SiteManagementPage.ChildRadioButton.Checked = true;
            Thread.Sleep(1000);
            SiteManagementPage.SiteAliasField.Value += "Child";
            SiteManagementPage.SiteNameField.Value = "Child";
            SiteManagementPage.CreateSiteLink.Click();
            SiteManagementPage.CreateSiteLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [When(@"I create a new site group")]
        public void WhenICreateANewSiteGroup()
        {
            if (!SiteGroupPage.CreateSiteGroupButton.Exists)
            {
                return;
            }

            SiteGroupPage.CreateSiteGroupButton.Click();
            SiteGroupPage.GroupNameField.Value = "Main Group";
            SiteGroupPage.GroupDescriptionField.Value = "Main Group";
            SiteGroupPage.PortalSelectList.SelectByValue("0");
            SiteGroupPage.DoCreateButton.Click();
            while (!SiteGroupPage.AvailablePortalSelectList.Exists)
            {
                Thread.Sleep(100);
            }
        }

        [When(@"I add the child portal to the group")]
        public void WhenIAddTheChildPortalToTheGroup()
        {
            if (!SiteGroupPage.AvailablePortalSelectList.Exists)
            {
                return;
            }

            if (SiteGroupPage.AvailablePortalSelectList.Options.Count==0)
            {
                throw new Exception("No portals available in Portal Select list");
            }

            SiteGroupPage.AvailablePortalSelectList.Options[0].Select();
            SiteGroupPage.AddSiteToGroupButton.Click();

            SiteGroupPage.AddSiteToGroupButton.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [When(@"I visit to the child portal")]
        public void WhenIVisitToTheChildPortal()
        {
            IEInstance.GoTo(string.Format("http://{0}/Child/Admin/UserAccounts.aspx", SiteUrl));
        }

        [Then(@"I shouldn't see delete button on administrator")]
        public void ThenIShouldnTSeeDeleteButtonOnAdministrator()
        {
            Assert.IsFalse(UserPage.GetUserDeleteButton("Administrator Account").Exists);
        }
    }
}
