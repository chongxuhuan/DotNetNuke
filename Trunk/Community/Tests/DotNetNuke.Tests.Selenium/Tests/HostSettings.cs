using System;
using System.Threading;

using NUnit.Framework;

using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class HostSettings : Base
    {
        public static IWebElement AdvancedTabButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnnHostSettings']/ul[1]/li[2]")); }
        public static IWebElement AdvancedTabSmtpServerSettingsButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='Panel-SMTP']/a")); }
        public static IWebElement AdvancedTabSmtpServerSettingsServerTextbox(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_HostSettings_txtSMTPServer']"));}
        public static IWebElement AdvancedTabSmtpServerSettingsTestButton(IWebDriver driver){return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_HostSettings_cmdEmail']"));}
        public static IWebElement AdvancedTabSmtpServerSettingsTestResponse(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_HostSettings_valSummary']/ul/li")); }
        public static IWebElement AdvancedTabSmtpServerSettingsTestResponse2(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_ctl01_lblMessage']/text()[1]")); }
        public static IWebElement UpdateButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_HostSettings_cmdUpdate']")); }
        public static IWebElement HostDetailsButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='Panel-HostDetails']/a")); }
        public static IWebElement HostEmailTextbox(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr327_HostSettings_txtHostEmail']")); }
    }

    [TestFixture]
    public class HostSettingsTests : Base
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

        #region VerifyConfigureEmail

        [Test]
        public void VerifyConfigureEmailFirefox() { VerifyConfigureEmail(_firefox); }
        [Test]
        public void VerifyConfigureEmailChrome() { VerifyConfigureEmail(_chrome); }
        [Test]
        public void VerifyConfigureEmailIe() { VerifyConfigureEmail(_ie); }

        private static void VerifyConfigureEmail(IWebDriver driver)
        {
            Logoff(driver);

            Login.AsHost(driver);

            driver.Navigate().GoToUrl(HostSettingsPage);

            HostSettings.HostDetailsButton(driver).Click();
            HostSettings.HostEmailTextbox(driver).Clear();
            HostSettings.HostEmailTextbox(driver).SendKeys("james.coles-nash@dnncorp.com");

            HostSettings.UpdateButton(driver).Click();
            
            Thread.Sleep(2000);

            HostSettings.AdvancedTabButton(driver).Click();

            Thread.Sleep(2000);
            HostSettings.AdvancedTabSmtpServerSettingsButton(driver).Click();

            HostSettings.AdvancedTabSmtpServerSettingsServerTextbox(driver).Clear();
            HostSettings.AdvancedTabSmtpServerSettingsServerTextbox(driver).SendKeys("smtp.dnncorp.com");

            HostSettings.AdvancedTabSmtpServerSettingsTestButton(driver).Click();

            Thread.Sleep(2000);

            Assert.That(HostSettings.AdvancedTabSmtpServerSettingsTestResponse(driver).Text, !Is.StringContaining("error"));
            Assert.That(HostSettings.AdvancedTabSmtpServerSettingsTestResponse2(driver).Text, !Is.StringContaining("error"));
        }

        #endregion
    }
}
