using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class Login : Base
    {
        public static IWebElement LoginButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_userLogin_loginLink")); }
        public static IWebElement LoginUserNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Login_Login_DNN_txtUsername")); }
        public static IWebElement LoginPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Login_Login_DNN_txtPassword")); }
        public static IWebElement LoginLoginButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Login_Login_DNN_cmdLogin")); }
        public static IWebElement LoginErrorMessage(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_ctl00_lblMessage")); }
        public static IWebElement RegisterButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_userLogin_registerLink")); }

        public static void AsUser(IWebDriver driver, string user, string pass)
        {
            Logoff(driver);
            driver.Navigate().Refresh();
            LoginButton(driver).Click();

            LoginUserNameTextbox(driver).SendKeys(user);
            LoginPasswordTextbox(driver).SendKeys(pass);
            LoginLoginButton(driver).Click();

            var currentUser = CurrentUserLink(driver).Text;
            Console.WriteLine(currentUser);
        }

        public static void AsHost(IWebDriver driver)
        {
            AsUser(driver, "host", "dnnhost");
        }

        public static void AsAdmin(IWebDriver driver)
        {
            AsUser(driver, "admin", "dnnadmin");
        }

    }

    [TestFixture]
    public class LoginTests : Base
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

        #region VerifyHostLogin

        [Test]
        public void VerifyHostLoginFirefox() { VerifyHostLogin(_firefox); }
        [Test]
        public void VerifyHostLoginChrome() { VerifyHostLogin(_chrome); }
        [Test]
        public void VerifyHostLoginIe() { VerifyHostLogin(_ie); }

        private static void VerifyHostLogin(IWebDriver driver)
        {
            Login.AsHost(driver);

            Assert.That(CurrentUserLink(driver).Text, Is.StringContaining("SuperUser"), "SuperUser account not logged in");
        }

        #endregion

        #region VerifyLoginFailsWithBadPassword

        [Test]
        public void VerifyLoginFailsWithBadPasswordFirefox() { VerifyLoginFailsWithBadPassword(_firefox); }
        [Test]
        public void VerifyLoginFailsWithBadPasswordChrome() { VerifyLoginFailsWithBadPassword(_chrome); }
        [Test]
        public void VerifyLoginFailsWithBadPasswordIe() { VerifyLoginFailsWithBadPassword(_ie); }

        private static void VerifyLoginFailsWithBadPassword(IWebDriver driver)
        {
            Login.AsUser(driver, "host", "wrong");

            Assert.That(Login.LoginErrorMessage(driver).Text, Is.StringContaining("Login Failed"), "Able to log in with incorrect password");
        }

        #endregion
    }
}
