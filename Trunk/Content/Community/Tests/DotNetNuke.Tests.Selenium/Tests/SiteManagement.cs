using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class SiteManagement : Base
    {
        public static By CreatePortalButton = By.CssSelector("*[id$='_Portals_createSite']");
        public static By AddSiteSiteTypeChildRadio = By.XPath("//*[@id='dnn_ctr321_Signup_optType']/span[2]/span/img"); 
        public static By AddSitePortalAliasTextbox = By.CssSelector("*[id$='_Signup_txtPortalAlias']"); 
        public static By AddSitePortalNameTextbox = By.CssSelector("*[id$='_Signup_txtPortalName']");

        public static By AddSiteTemplateInput = By.CssSelector("*[id$='_Signup_cboTemplate_Input']");
        public static By AddSiteTemplateBlankItem = By.XPath("//*[@id='dnn_ctr321_Signup_cboTemplate_DropDown']/div/div/div/ul/li[text() = 'Blank Website - English (United States)']");
        public static By AddSiteTemplateDefaultItem = By.XPath("//*[@id='dnn_ctr321_Signup_cboTemplate_DropDown']/div/div/div/ul/li[text() = 'Default Website - English (United States)']");
        public static By AddSiteTemplateMobileItem = By.XPath("//*[@id='dnn_ctr321_Signup_cboTemplate_DropDown']/div/div/div/ul/li[text() = 'Mobile Website - English (United States)']");
        

        public static By AddSiteCreateSiteButton = By.CssSelector("*[id$='_Signup_cmdUpdate']");
        public static By AddSiteVisitNewPortalLink = By.CssSelector("*[id$='_lblMessage'] a");

        public static void CreateNewPortal(IWebDriver driver, string portalName, By template)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SiteManagementPage);

            driver.WaitClick(CreatePortalButton);

            driver.WaitClick(AddSiteSiteTypeChildRadio);

            driver.FindDnnElement(AddSitePortalAliasTextbox).Clear();

            driver.WaitSendKeys(AddSitePortalAliasTextbox, "localhost/DotNetNuke_Enterprise/" + portalName + "/");

            driver.FindDnnElement(AddSitePortalAliasTextbox).Clear();

            driver.WaitSendKeys(AddSitePortalAliasTextbox, "localhost/DotNetNuke_Enterprise/" + portalName + "/");

            driver.WaitSendKeys(AddSitePortalNameTextbox, portalName);

            driver.WaitClick(AddSiteTemplateInput);
            driver.WaitClick(template);

            driver.WaitClick(AddSiteCreateSiteButton);

            int origTimeout = Common.DriverTimeout;
            Common.DriverTimeout = 30000;
            driver.WaitClick(AddSiteVisitNewPortalLink);
            Common.DriverTimeout = origTimeout;
        }

        public static void SwitchToNewPortal(IWebDriver driver, string portalName)
        {
            CreateNewPortal(driver, portalName, AddSiteTemplateDefaultItem);

            StartPage = "http://localhost/DotNetNuke_Enterprise/" + portalName + "/";

            driver.Navigate().GoToUrl(StartPage);
        }

        #region VerifyCreatingChildSite

        [Test]
        public void VerifyCreatingChildSiteFirefox() { VerifyCreatingChildSite(Firefox); }
        [Test]
        public void VerifyCreatingChildSiteChrome() { VerifyCreatingChildSite(Chrome); }
        [Test]
        public void VerifyCreatingChildSiteIe() { VerifyCreatingChildSite(Ie); }

        private static void VerifyCreatingChildSite(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            CreateNewPortal(driver, "ChildSite" + rndNum, AddSiteTemplateDefaultItem);

            driver.Navigate().GoToUrl(StartPage + "ChildSite" + rndNum + "/");

            Assert.That(driver.FindDnnElement(LetMeAtItButton).Text, Is.StringContaining("Let me at it"));
        }

        #endregion
    }
}
