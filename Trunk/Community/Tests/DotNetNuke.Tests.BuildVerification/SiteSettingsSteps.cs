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
using DotNetNuke.Tests.UI.WatiN.Utilities;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class SiteSettingsSteps : WatiNTest
    {
        public SiteSettingsPage SettingsPage
        {
            get
            {
                return GetPage<SiteSettingsPage>();
            }
        }

        /// <summary>
        /// Clicks the Advanced Settings tab on the Site Settings page.
        /// </summary>
        [Given(@"I am on the Advanced Settings Tab")]
        public void GivenIAmOnTheAdvancedSettingsTab()
        {
            Thread.Sleep(2500);
            SettingsPage.AdvancedSettingsTab.Click();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Clicks the Site Aliases section link on the Site Settings page.
        /// </summary>
        [Given(@"I am on the Site Aliases Section")]
        public void GivenIAmOnTheSiteAliasesSection()
        {
            Thread.Sleep(1500);
            SettingsPage.SiteAliasesSectionLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Creats an IIS site in the application pool specified with the alias specified.
        /// </summary>
        /// <param name="appPool">The application pool.</param>
        /// <param name="portalAlias">The portal alias.</param>
        [Given(@"I have created an (.*) site in IIS with the alias (.*)")]
        public void GivenIHaveCreatedASiteInIISWithTheAlias(string appPool, string portalAlias)
        {
            string physicalPath = Directory.GetCurrentDirectory();
            physicalPath = physicalPath.Replace("\\Tests\\Fixtures", "\\Website");
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultPhysicalAppPath"]))
            {
                physicalPath = ConfigurationManager.AppSettings["DefaultPhysicalAppPath"];
            }
            IISManager.CreateIISApplication(portalAlias, physicalPath);
            IISManager.SetApplicationPool(portalAlias, appPool);
        }

        /// <summary>
        /// Clicks the Add New Alias link on the Site Settings page.
        /// This link will display the New Portal Alias field. To add a new portal alias to the site you will need to click this link first.
        /// </summary>
        [When(@"I click Add New Alias")]
        public void WhenIClickAddNewAlias()
        {
            SettingsPage.AddNewAliasLink.ClickNoWait();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Enters the portal alias specified into the portal alias field. 
        /// This step will enter exactly what is specified in the steps, you may need to append localhost/ to the alias you wish to use.
        /// </summary>
        /// <param name="fullPortalAlias">The portal alias.</param>
        [When(@"I enter (.*) in the portal alias field")]
        public void WhenIEnterInThePortalAliasField(string fullPortalAlias)
        {
            SettingsPage.AliasTextBox.Value = fullPortalAlias;
        }

        /// <summary>
        /// Clicks the save icon next to the portal alias field. 
        /// </summary>
        [When(@"I click Save from the site alias table")]
        public void WhenIClickSaveFromTheSiteAliasTable()
        {
            SettingsPage.PortalAliasSaveButtonPortal.ClickNoWait();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Checks that the current page is the Site Settings page.
        /// Checks that the Site Settings module is on the page.
        /// </summary>
        [Then(@"I should be on the Site Settings Page")]
        public void ThenIShouldBeOnTheSiteSettingsPage()
        {
            WatiNAssert.AssertIsTrue(SettingsPage.PageTitle.InnerHtml.Contains("Site Settings"), "NotOnSiteSettingsPage.jpg");
            var menu = new SiteMenu(HomePage);
            WatiNAssert.AssertIsTrue(menu.BreadCrumbSpan.InnerHtml.Contains("Site Settings"), "NotOnSiteSettingsPage2.jpg");

        }

        /// <summary>
        /// Checks that the Site Settings page does not contain a SSL Section.
        /// </summary>
        [Then(@"I should not see the SSL section on the page")]
        public void ThenIShouldNotSeeTheSSLSectionOnThePage()
        {
            WatiNAssert.AssertIsFalse(SettingsPage.SSLSectionLink.Exists, "SSLSectionOnPage.jpg");
        }

        /// <summary>
        /// Checks that the Site Settings page does not contain a Host Settings section.
        /// </summary>
        [Then(@"I should not see the Host section on the page")]
        public void ThenIShouldNotSeeTheHostSectionOnThePage()
        {
            WatiNAssert.AssertIsFalse(SettingsPage.HostSectionLink.Exists, "HostSettingsSectionOnPage.jpg");
        }

        /// <summary>
        /// Checks that the Site Settings page does contain a SSL Section.
        /// </summary>
        [Then(@"I should see the SSL section on the page")]
        public void ThenIShouldSeeTheSSLSectionOnThePage()
        {
            WatiNAssert.AssertIsTrue(SettingsPage.SSLSectionLink.Exists, "SSLSectionNotOnPage.jpg");
        }

        /// <summary>
        /// Checks that the Site Settings page does contain a Host Settings section.
        /// </summary>
        [Then(@"I should see the Host section on the page")]
        public void ThenIShouldSeeTheHostSectionOnThePage()
        {
            WatiNAssert.AssertIsTrue(SettingsPage.HostSectionLink.Exists, "HostSettingsSectionNotOnPage.jpg");
        }

    }
}
