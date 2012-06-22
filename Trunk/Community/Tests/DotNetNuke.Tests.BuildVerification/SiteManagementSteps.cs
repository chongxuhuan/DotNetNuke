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

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class SiteManagementSteps : WatiNTest
    {
        public HostSiteManagementPage SiteManagementPage
        {
            get
            {
                return GetPage<HostSiteManagementPage>();
            }
        }

        /// <summary>
        /// Clicks the Add New Site link on the Site Management page.
        /// </summary>
        [When(@"I click the Add New Site Link")]
        public void WhenIClickTheAddNewSiteLink()
        {
            Thread.Sleep(1500);
            SiteManagementPage.AddNewSiteLink.ClickNoWait();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Fills in the Add Site form with the alias and title specified. 
        /// Selects the Default Website template.
        /// </summary>
        /// <param name="portalAlias">The site alias. This should not include the site URL, the step will append the site URL to the alias entered.</param>
        /// <param name="portalTitle">The portal title.</param>
        [When(@"I fill in the portal form for a child portal with the alias (.*) the title (.*)")]
        public void WhenIFillInThePortalFormForAChildPortal(string portalAlias, string portalTitle)
        {
            SiteManagementPage.ChildRadioButton.Click();
            Thread.Sleep(4000);
            SiteManagementPage.SiteAliasField.Value = SiteUrl.Replace("http://", "") + "/" + portalAlias;
            SiteManagementPage.SiteNameField.Value = portalTitle;
            SiteManagementPage.PortalTemplateSelectList.Refresh();
            SiteManagementPage.PortalTemplateSelectList.Select("Default Website - en-US");
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Checks the Use Current User as Admin checkbox on the Add Site form.
        /// </summary>
        [When(@"I select the current user for the site admin")]
        public void WhenISelectTheCurrentUserForTheSiteAdmin()
        {
            Thread.Sleep(1500);
            SiteManagementPage.UseCurrentUserCheckbox.Checked = true;
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Clicks the Create Site Link on the Add Site form
        /// </summary>
        [When(@"I click the Create Site Link")]
        public void WhenIClickTheCreateSiteLink()
        {
            SiteManagementPage.CreateSiteLink.Click();
            SiteManagementPage.CreateSiteLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        /// <summary>
        /// Checks that the site table on the site management page contains the title.
        /// Browses to the new site and looks for a DNN logo, login link and the register link.
        /// </summary>
        /// <param name="portalAlias">The site alias. This should not include the site URL.</param>
        /// <param name="portalTitle">The portal title.</param>
        [Then(@"The child portal with the alias (.*) and the title (.*) should be created correctly")]
        public void ThenAChildPortalShouldBeCreatedCorrectly(string portalAlias, string portalTitle)
        {
            IEInstance.NativeBrowser.NavigateTo(new Uri("http://" + SiteUrl + "/host/sitemanagement.aspx"));
            IEInstance.WaitForComplete();

            WatiNAssert.AssertIsTrue(SiteManagementPage.PortalsTable.InnerHtml.Contains(portalTitle), portalTitle + "NotInSitesTable.jpg");

            IEInstance.GoTo(SiteUrl + "/" + portalAlias);
            IEInstance.WaitForComplete();
            WatiNAssert.AssertIsTrue(IEInstance.Image(Find.ById("dnn_dnnLogo_imgLogo")).Exists, "LogoNotFoundOnChildPortal.jpg");
            WatiNAssert.AssertIsTrue(HomePage.LogoLink.Exists, "LogoLinkNotFoundOnChildPortal.jpg");
            WatiNAssert.AssertIsTrue(HomePage.LoginLink.Exists, "LoginLinkNotFoundOnChildPortal.jpg");
            WatiNAssert.AssertIsTrue(HomePage.RegisterLink.Exists, "RegisterLinkNotFoundOnChildPortal.jpg");

        }
    }
}
