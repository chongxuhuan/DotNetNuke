using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    [TestFixture]
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

        #region VerifyConfigureEmail

        [Test]
        public void VerifyConfigureEmailFirefox() { VerifyConfigureEmail(Firefox); }
        [Test]
        public void VerifyConfigureEmailChrome() { VerifyConfigureEmail(Chrome); }
        [Test]
        public void VerifyConfigureEmailIe() { VerifyConfigureEmail(Ie); }

        private static void VerifyConfigureEmail(IWebDriver driver)
        {
            Logoff(driver);

            Login.AsHost(driver);

            driver.Navigate().GoToUrl(HostSettingsPage);

            HostDetailsButton(driver).Click();
            HostEmailTextbox(driver).Clear();
            HostEmailTextbox(driver).SendKeys("james.coles-nash@dnncorp.com");

            UpdateButton(driver).Click();

            Thread.Sleep(2000);

            AdvancedTabButton(driver).Click();

            Thread.Sleep(2000);
            AdvancedTabSmtpServerSettingsButton(driver).Click();

            AdvancedTabSmtpServerSettingsServerTextbox(driver).Clear();
            AdvancedTabSmtpServerSettingsServerTextbox(driver).SendKeys("smtp.dnncorp.com");

            AdvancedTabSmtpServerSettingsTestButton(driver).Click();

            Thread.Sleep(2000);

            Assert.That(AdvancedTabSmtpServerSettingsTestResponse(driver).Text, !Is.StringContaining("error"));
            Assert.That(AdvancedTabSmtpServerSettingsTestResponse2(driver).Text, !Is.StringContaining("error"));
        }

        #endregion
    }
}
