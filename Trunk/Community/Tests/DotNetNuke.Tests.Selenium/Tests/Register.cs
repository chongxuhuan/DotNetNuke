using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class Register
    {
        public static IWebElement RegisterButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_userLogin_registerLink")); }

        public static IWebElement RegisterUsernameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Username_Username_TextBox")); }
        public static IWebElement RegisterPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Password_Password_TextBox")); }
        public static IWebElement RegisterConfirmPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_PasswordConfirm_PasswordConfirm_TextBox")); }
        public static IWebElement RegisterDisplayNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_DisplayName_DisplayName_TextBox")); }
        public static IWebElement RegisterEmailTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Email_Email_TextBox")); }
        public static IWebElement RegisterContinueButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_registerButton")); }
    }

    [TestFixture]
    public class RegisterTests : Base
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

        #region VerifyRegisterUser

        [Test]
        public void VerifyRegisterUserFirefox() { VerifyRegisterUser(_firefox); }
        [Test]
        public void VerifyRegisterUserChrome() { VerifyRegisterUser(_chrome); }
        [Test]
        public void VerifyRegisterUserIe() { VerifyRegisterUser(_ie); }

        private static void VerifyRegisterUser(IWebDriver driver)
        {
            Logoff(driver);

            Login.RegisterButton(driver).Click();

            Register.RegisterUsernameTextbox(driver).SendKeys("TestUser");
            Register.RegisterPasswordTextbox(driver).SendKeys("password#1");
            Register.RegisterConfirmPasswordTextbox(driver).SendKeys("password#1");
            Register.RegisterDisplayNameTextbox(driver).SendKeys("TestUser");
            Register.RegisterEmailTextbox(driver).SendKeys("TestUser@dnncorp.com");
            Register.RegisterContinueButton(driver).Click();

            Assert.That(CurrentUserLink(driver).Text, Is.StringContaining("TestUser"), "User Not registered or not logged in");

            UserAccounts.DeleteFirstUser(driver);
        }

        #endregion
    }
}
