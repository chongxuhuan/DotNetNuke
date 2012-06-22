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
using System.Configuration;
using System.IO;
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
    public class PageSteps : WatiNTest
    {
        public RibbonBar RibbonBar
        {
            get
            {
                return GetPage<RibbonBar>();
            }
        }

        public PageSettingsPage PageSettings
        {
            get
            {
                return GetPage<PageSettingsPage>();
            }
        }

        public IconBar IconBar
        {
            get
            {
                return GetPage<IconBar>();
            }
        }
        public AdminPagesPage PagesPage
        {
            get
            {
                return GetPage<AdminPagesPage>();
            }
        }
      
        [BeforeScenario("MustUseRibbonBar")]
        public void MustUseRibbonBar()
        {
            //TODO: Set the control panel type to Ribbon Bar
        }
        
        /// <summary>
        /// Gives the user specified view permissions for the page.
        /// This step assumes that both the page and the user already exist on the site.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="userName">The username for the user that will be given view permissions.</param>
        [Given(@"The page (.*) has view permissions set for the user (.*)")]
        public void GivenThePageHasViewPermissionsSetForTheUserUser1(string pageName, string userName)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl + "/admin/pages.aspx");
            Thread.Sleep(1500);
            PagesPage.EditPageGiveUserViewPermissions(pageName, userName);
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives the user specified edit permissions for the page.
        /// This step assumes that both the page and the user already exist on the site.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="userName">The username for the user that will be given edit permisions.</param>
        /// <param name="displayName">The display name of the user that will be given edit permissions.</param>
        [Given(@"The page (.*) has edit permissions set for the user (.*) with display name (.*)")]
        public void GivenThePageHasEditPermissionsSetForTheUserUser1(string pageName, string userName, string displayName)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl + "/admin/pages.aspx");
            Thread.Sleep(1500);
            PagesPage.EditPageGiveUserEditPermissions(pageName, userName, displayName);
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Clicks the Delete Page link from the ribbon bar.
        /// The site must be using the Ribbon bar, to ensure this add the "@MustUseRibbonBar" tag to your scenario.
        /// </summary>
        [When("I click Delete Page from the Ribbon Bar")]
        public void IClickDeletePageFromTheRibbonBar()
        {
            Thread.Sleep(1500);
            RibbonBar.DeletePageLink.Click();

        }

        /// <summary>
        /// Clicks the confirmation (Yes) button in the DotNetNuke pop up.
        /// </summary>
        [When("I click confirm in the pop up")]
        public void IClickConfirmInThePopUp()
        {
            Thread.Sleep(1500);
            RibbonBar.PopUpConfirmation.ClickNoWait();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Clicks the Add Page link from the ribbon bar, which will open a new page form.
        /// The site must be using the Ribbon bar, to ensure this add the "@MustUseRibbonBar" tag to your scenario.
        /// </summary>
        [When("I press Add from the Ribbon Bar")]
        public void WhenIPressAddFromTheRibbonBar()
        {
            RibbonBar.NewPageLink.Click();
        }

        
        //[When("I press Add from the Icon Bar")]
        //public void WhenIPressAddFromTheIconBar()
        //{
        //    _iconBar = new ControlPanel(_homePage);
        //    _iconBar.AddPageLink.Click();
        //}

        /// <summary>
        /// Enters the page name into the page name field on a page settings/new page form.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        [When("I enter the page name (.*) into the page settings")]
        public void WhenIEnterThePageNameIntoThePageSettings(string pageName)
        {
            PageSettings.PageNameField.Value = pageName;
        }

        /// <summary>
        /// Clicks the Add Page link on the new page form.
        /// </summary>
        [When("I click Add Page")]
        public void WhenIClickAddPage()
        {
            PageSettings.AddPageLink.Click();
        }

        /// <summary>
        /// Checks that a page with the name specified appears in the menu and is in the bread crumb of the current page. 
        /// </summary>
        /// <param name="pageName">The page name.</param>
        [Then("A page called (.*) should be created")]
        public void APageShouldBeCreated(string pageName)
        {
            Thread.Sleep(1500);
            var menu = GetPage<SiteMenu>();
            WatiNAssert.AssertStringsAreEqual(pageName, menu.BreadCrumbSpan.Text, pageName + "NotInBreadcrumb.jpg");
            WatiNAssert.AssertIsTrue(menu.MainMenu.InnerHtml.Contains(pageName), pageName + "NotInMenu.jpg");
        }

        /// <summary>
        /// Checks that a page with the name specified doesn't appear in the menu, and is in the recycle bin. 
        /// </summary>
        /// <param name="pageName">The page name.</param>
        [Then(@"The page called (.*) is deleted")]
        public void ThenThePageCalledTestPageIsDeleted(string pageName)
        {
            Thread.Sleep(2500);
            IEInstance.Refresh();
            var menu = GetPage<SiteMenu>();
            WatiNAssert.AssertIsFalse(menu.MainMenu.Link(Find.ByText(s => s.Contains(pageName))).Exists, pageName + "FoundInMenu.jpg", pageName + " found in main menu: " + menu.MainMenu.InnerHtml);
            IEInstance.GoTo(SiteUrl + "/Admin/RecycleBin.aspx");
            var recycleBin = GetPage<RecycleBinPage>();
            Thread.Sleep(1000);
            WatiNAssert.AssertIsTrue(recycleBin.PagesRecycleBin.InnerHtml.Contains(pageName), pageName + "NotFoundInRecycleBin.jpg", pageName + " not found in recycle bin: " + recycleBin.PagesRecycleBin.InnerHtml);
        }
    }
}
