using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class SiteSettings : Base
    {
        public static IWebElement Header(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr480_dnnTITLE_titleLabel")); }
    }

    [TestFixture]
    public class SiteSettingsTests : Base
    {
        private IWebDriver _firefox;
        private IWebDriver _chrome;
        private IWebDriver _ie;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _firefox = StartBrowser(Common.BrowserType.firefox);
            _firefox.Manage().Window.Maximize();
            _firefox.Navigate().GoToUrl(StartPage);

            _chrome = StartBrowser(Common.BrowserType.chrome);
            _chrome.Manage().Window.Maximize();
            _chrome.Navigate().GoToUrl(StartPage);

            _ie = StartBrowser(Common.BrowserType.ie);
            _ie.Manage().Window.Maximize();
            _ie.Navigate().GoToUrl(StartPage);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _firefox.Quit();
            _chrome.Quit();
            _ie.Quit();
        }

        #region VerifySiteSettingsHost

        [Test]
        public void VerifySiteSettingsHostFirefox() { VerifySiteSettingsHost(_firefox); }
        [Test]
        public void VerifySiteSettingsHosthChrome() { VerifySiteSettingsHost(_chrome); }
        [Test]
        public void VerifySiteSettingsHostIe() { VerifySiteSettingsHost(_ie); }

        private static void VerifySiteSettingsHost(IWebDriver driver)
        {
            Logoff(driver);

            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SiteSettingsPage);

            Assert.That(SiteSettings.Header(driver).Text, Is.StringContaining("Site Settings"), "Site Settings page not accessible");
        }

        #endregion

        #region VerifySiteSettingsAdmin

        [Test]
        public void VerifySiteSettingsAdminFirefox() { VerifySiteSettingsAdmin(_firefox); }
        [Test]
        public void VerifySiteSettingsAdminChrome() { VerifySiteSettingsAdmin(_chrome); }
        [Test]
        public void VerifySiteSettingsAdminIe() { VerifySiteSettingsAdmin(_ie); }

        private static void VerifySiteSettingsAdmin(IWebDriver driver)
        {
            Logoff(driver);

            Login.AsUser(driver, "admin", "dnnadmin");

            driver.Navigate().GoToUrl(SiteSettingsPage);

            Assert.That(SiteSettings.Header(driver).Text, Is.StringContaining("Site Settings"), "Site Settings page not accessible");
        }

        #endregion

    }
}
