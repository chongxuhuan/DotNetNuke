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
using System.Threading;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.UI.WatiN.Utilities;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        public AdminPagesPage _pagesPage;
        public RibbonBar _ribbonBar;


        /// <summary>
        /// Adds a page called Test Page to portal 0 without using the UI. 
        /// The page will be given edit a view permissions for the host and admin. 
        /// All other settings will be left blank.
        /// To access this page correctly during a test the dotnetnuke cache will need to be cleared. Be sure to use the statement "Given I have cleared the dotnetnuke cache" during your test.
        /// </summary>
        [BeforeScenario("MustHaveTestPageAdded")]
        public void MustHaveTestPageAdded()
        {
            //Create a tabInfo obj, then use TabController AddTab Method?
            TabInfo newPage = new TabInfo();
            newPage.TabName = "Test Page";
            newPage.PortalID = 0;
            newPage.SkinSrc = "[G]Skins/DarkKnight/Home-Mega-Menu.ascx";
            newPage.ContainerSrc = "[G]Containers/DarkKnight/SubTitle_Grey.ascx";
            TabController tabController = new TabController();
            var tab = tabController.GetTabByName("Test Page", 0);
            if (tab == null)
            {
                try
                {
                    tabController.AddTab(newPage);
                }
                catch (Exception exc)
                {
                    Assert.IsTrue(false, "Add Tab result: " + exc.Message);
                }
                //tabController.AddTab(newPage);
                newPage = tabController.GetTabByName("Test Page", 0);
                TabPermissionInfo tabPermission = new TabPermissionInfo();
                tabPermission.PermissionID = 3;
                tabPermission.TabID = newPage.TabID;
                tabPermission.AllowAccess = true;
                tabPermission.RoleID = 0;
                newPage.TabPermissions.Add(tabPermission);
                try
                {
                    tabController.UpdateTab(newPage);
                }
                catch (Exception exc)
                {
                    Assert.IsTrue(false, "Update Tab result: " + exc.Message);
                }
                newPage = tabController.GetTabByName("Test Page", 0);
                tabPermission.RoleID = 0;
                tabPermission.PermissionID = 4;
                tabPermission.TabID = newPage.TabID;
                tabPermission.AllowAccess = true;
                newPage.TabPermissions.Add(tabPermission);
                try
                {
                    tabController.UpdateTab(newPage);
                }
                catch (Exception exc)
                {
                    Assert.IsTrue(false, "Update Tab Permissions result: " + exc.Message);
                }
                Thread.Sleep(1500);
            }
        }

        /// <summary>
        /// Recycles the application pool and opens an ie instance to the site home page.
        /// This step will be the initial step to almost any scenario.
        /// </summary>
        [When("I am on the site home page (.*)")]
        [Given("I am on the site home page (.*)")]
        public void GivenIAmOnTheSiteHomePageWithTimeOut(string timeOutValue)
        {
            LoadHomePage(timeOutValue);
        }

        /// <summary>
        /// Recycles the application pool and opens an ie instance to the site home page.
        /// This step will be the initial step to almost any scenario.
        /// </summary>
        [When("I am on the site home page")]
        [Given("I am on the site home page")]
        public void GivenIAmOnTheSiteHomePage()
        {
            LoadHomePage("");
        }

        private void LoadHomePage(string timeOutValue)
        {
            int timeOut = 180;
            if (timeOutValue == null)
            {
                int.TryParse(timeOutValue, out timeOut);
            }
            Thread.Sleep(1500);
            IEInstance = WatiNUtil.OpenIEInstance(SiteUrl, false, timeOut, true, false);
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Navigates to the root level page specified.
        /// Entering Test Page as a pageName would browse to "(mysite)/testpage.aspx"
        /// </summary>
        /// <param name="pageName"></param>
        [Given("I am viewing the page called (.*)")]
        [When("I am viewing the page called (.*)")]
        public void GivenIAmViewingThePage(string pageName)
        {
            Thread.Sleep(1500);
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/" + pageName + ".aspx");
        }

        /// <summary>
        /// Navigates to the admin page specified.
        /// Entering Languages as a adminPageName would brose to "(mysite)/admin/languages.aspx"
        /// </summary>
        /// <param name="adminPageName"></param>
        [Given(@"I am on the admin page (.*)")]
        public void GivenIAmOnTheAdminPage(string adminPageName)
        {
            Thread.Sleep(2500);
            adminPageName = adminPageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/admin/" + adminPageName + ".aspx");
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Navigates to the host page specified.
        /// Entering Superuser Accounts as a hostPageName would brose to "(mysite)/host/superuser%20accounts.aspx"
        /// </summary>
        /// <param name="hostPageName"></param>
        [Given(@"I am on the Host Page (.*)")]
        [When(@"I am on the Host Page (.*)")]
        public void IAmOnTheHostPage(string hostPageName)
        {
            Thread.Sleep(2500);
            hostPageName = hostPageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/host/" + hostPageName + ".aspx");
            Thread.Sleep(1500);
        }


        /// <summary>
        /// Navigates to the host professional feature page specified.
        /// Entering Application Integrity as a hostPEPageName would browse to "(mysite)/host/professionalfeatures/application%20integrity.aspx"
        /// </summary>
        /// <param name="hostPEPageName"></param>
        [Given(@"I am on the Host Professional Feature Page (.*)")]
        [When(@"I navigate to the Host professional feature page (.*)")]
        public void GivenIAmOnTheHostProFeaturePagePage(string hostPEPageName)
        {
            Thread.Sleep(2500);
            hostPEPageName = hostPEPageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/host/professionalfeatures/" + hostPEPageName + ".aspx");
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Navigates to the admin page specified.
        /// Entering Languages as a adminPageName would brose to "(mysite)/admin/languages.aspx"
        /// </summary>
        /// <param name="adminPageName"></param>
        [Given(@"I navigate to the admin page (.*)")]
        [When(@"I navigate to the admin page (.*)")]
        public void WhenINavigateToTheAdminPage(string adminPageName)
        {
            Thread.Sleep(2500);
            adminPageName = adminPageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/admin/" + adminPageName + ".aspx");
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Creates a page with the name specified with the default settings for a page. 
        /// If the page already exists, the step will not create the page. 
        /// </summary>
        /// <param name="pageName"></param>
        [When("I have created the page called (.*)")]
        [Given("I have created the page called (.*)")]
        public void GivenIHaveCreatedThePage(string pageName)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl + "/admin/pages.aspx");
            Thread.Sleep(1500);
            _pagesPage = GetPage<AdminPagesPage>();
            if (!_pagesPage.PagesPanel.InnerHtml.Contains(pageName))
            {
                Thread.Sleep(2500);
                _pagesPage.AddPage(pageName);
                Thread.Sleep(1500);
            }
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Creates a page with the name specified and no template. 
        /// The page will be added from the Ribbon Bar, if a test will be using this statement you may want to add the @MustUseRibbonBar tag to make sure that the site uses the ribbon bar.
        /// The page will be added at the root level, no matter what page the test was on before executing this step. To add a child or grandchild page use a different statement. 
        /// </summary>
        /// <param name="pageName"></param>
        [When(@"I have created the blank page called (.*) from the Ribbon Bar")]
        [Given(@"I have created the blank page called (.*) from the Ribbon Bar")]
        public void GivenIHaveCreatedTheBlankPageFromTheRibbonBar(string pageName)
        {
            AddPage(pageName, "None Specified");
        }

        /// <summary>
        /// Creates a page with the name specified and the default template. 
        /// The page will be added from the Ribbon Bar, if a test will be using this statement you may want to add the @MustUseRibbonBar tag to make sure that the site uses the ribbon bar.
        /// The page will be added at the root level, no matter what page the test was on before executing this step. To add a child or grandchild page use a different statement. 
        /// </summary>
        /// <param name="pageName"></param>
        [When(@"I have created the default page called (.*) from the Ribbon Bar")]
        [Given(@"I have created the default page called (.*) from the Ribbon Bar")]
        public void GivenIHaveCreatedThePageFromTheRibbonBar(string pageName)
        {
            AddPage(pageName, "Default");
        }

        /// <summary>
        /// Creates a page with the name specified and template specified. 
        /// The page will be added from the Ribbon Bar, if a test will be using this statement you may want to add the @MustUseRibbonBar tag to make sure that the site uses the ribbon bar.
        /// The page will be added at the root level, no matter what page the test was on before executing this step. To add a child or grandchild page use a different statement. 
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="template"></param>
        [When(@"I have created page called (.*) with template (.*) from the Ribbon Bar")]
        [Given(@"I have created page called (.*) with template (.*) from the Ribbon Bar")]
        public void GivenIHaveCreatedThePageWithTemplateFromTheRibbonBar(string pageName, string template)
        {
            AddPage(pageName, template);
        }

        private void AddPage(string pageName, string pageTemplate)
        {
            IEInstance.GoTo(SiteUrl);
            Thread.Sleep(2500);
            _ribbonBar = GetPage<RibbonBar>();
            _ribbonBar.NewPageLink.Click();
            var pageSettings = GetPage<PageSettingsPage>();
            Thread.Sleep(1500);
            pageSettings.AddPageSelectTemplate(pageName, pageTemplate);
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives the role specified the specified permissions for the page.
        /// This step assumes that both the page and the role already exist on the site.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="roleName">The role that will be given view permissions. 
        /// <param name="permission">The permission. 
        /// <param name="setting">Allowed, Denied, Not Allowed.</param>
        /// This field must be entered exactly as the role name will appear in the permissions grid.</param>
        [Given(@"The page (.*) has (.*) permission set to (.*) for the role (.*)")]
        [When(@"The page (.*) has (.*) permission set to (.*) for the role (.*)")]
        public void GivenThePageHasPermissionSetForRole(string pageName, string permission, string setting, string roleName)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl + "/admin/pages.aspx");
            Thread.Sleep(1500);
            var adminPagesPage = GetPage<AdminPagesPage>();
            adminPagesPage.SetPagePermissionForRole(setting, permission, pageName, roleName);
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Navigates to the admin page specified.
        /// Entering Languages as a adminPageName would brose to "(mysite)/admin/languages.aspx"
        /// </summary>
        [When(@"I click the link (.*)")]
        public void WhenIClickTheLink(string linkAnchorText)
        {
            var link = IEInstance.Link(Find.ByText(s => s.Contains(linkAnchorText)));
            link.Click();
        }


        /// <summary>
        /// Checks the current page for indications that an error has occurred. 
        /// If the statement finds an error image, or the text error in a message span on the page it will return false. 
        /// </summary>
        [Then(@"I should not see any errors on the page")]
        public void ThenIShouldNotSeeAnyErrorsOnThePage()
        {
            Assert.IsFalse(HomePage.PageContainsErrors());
        }

        [Then(@"I should go to the site (.*)")]
        public void ThenIShouldGoToTheSite(string site)
        {
            var page = IE.AttachTo<IE>(Find.ByUrl(new Regex(string.Format(".*{0}.*", site), RegexOptions.IgnoreCase)));
            Assert.IsNotNull(page);

            //close the browser if its a new window
            if (IE.AttachTo<IE>(Find.ByUrl(new Regex(".*" + SiteUrl + ".*", RegexOptions.IgnoreCase))) != null)
            {
                page.Close();
            }
        }

    }
}
