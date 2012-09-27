using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class UserAccounts : Base
    {
        public static IWebElement AccountManagerAddNewUserButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_Users_AddUserLink")); }
        public static IWebElement AccountManagerDeleteButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr487_Users_grdUsers_ctl00__0']/td[2]/input")); }
        public static IWebElement AccountManagerRemoveButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr487_Users_grdUsers_ctl00__0']/td[5]/input")); }

        public static IWebElement AddNewUserUsernameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_userForm_userName_userName_TextBox")); }
        public static IWebElement AddNewUserFirstNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_userForm_firstName_firstName_TextBox")); }
        public static IWebElement AddNewUserLastNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_userForm_lastName_lastName_TextBox")); }
        public static IWebElement AddNewUserDisplayNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_userForm_displayName_displayName_TextBox")); }
        public static IWebElement AddNewUserEmailTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_userForm_email_email_TextBox")); }
        public static IWebElement AddNewUserAuthorizeCheckbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_chkAuthorize")); }
        public static IWebElement AddNewUserNotifyCheckbox(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='dnn_ctr487_ManageUsers_User_AuthorizeNotify']/div[2]/span/span/img")); }
        public static IWebElement AddNewUserPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_User_txtPassword")); }
        public static IWebElement AddNewUserConfirmPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.XPath("dnn_ctr487_ManageUsers_User_txtConfirm")); }
        public static IWebElement AddNewUserAddNewUserButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr487_ManageUsers_cmdRegister")); }

        public static void DeleteFirstUser(IWebDriver driver)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(UserAccountsPage);

            AccountManagerDeleteButton(driver).Click();

            driver.SwitchTo().Alert().Accept();
            driver.SwitchTo().DefaultContent();

            AccountManagerRemoveButton(driver).Click();

            driver.SwitchTo().Alert().Accept();
            driver.SwitchTo().DefaultContent();
        }
    }

    [TestFixture]
    public class UserAccountsTests : Base
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

        #region VerifyAddUser

        [Test]
        public void VerifyAddUserFirefox() { VerifyAddUser(_firefox); }
        [Test]
        public void VerifyAddUserChrome() { VerifyAddUser(_chrome); }
        [Test]
        public void VerifyAddUserIe() { VerifyAddUser(_ie); }

        private static void VerifyAddUser(IWebDriver driver)
        {
            Logoff(driver);

            Login.AsHost(driver);

            driver.Navigate().GoToUrl(UserAccountsPage);

            UserAccounts.AccountManagerAddNewUserButton(driver).Click();

            UserAccounts.AddNewUserUsernameTextbox(driver).SendKeys("TestAddUser");
            UserAccounts.AddNewUserFirstNameTextbox(driver).SendKeys("TestAdd");
            UserAccounts.AddNewUserLastNameTextbox(driver).SendKeys("User");
            UserAccounts.AddNewUserDisplayNameTextbox(driver).SendKeys("TestAddUser");
            UserAccounts.AddNewUserEmailTextbox(driver).SendKeys("TestAddUser@dnncorp.com");
            UserAccounts.AddNewUserNotifyCheckbox(driver).Click();
            UserAccounts.AddNewUserPasswordTextbox(driver).SendKeys("testadduserpass");
            UserAccounts.AddNewUserConfirmPasswordTextbox(driver).SendKeys("testadduserpass");

            UserAccounts.AddNewUserAddNewUserButton(driver).Click();

            Logoff(driver);

            Login.AsUser(driver, "TestAddUser", "testadduserpass");

            Assert.That(CurrentUserLink(driver).Text, Is.StringContaining("TestAddUser"), "User Not Added Correctly or not logged in");

            UserAccounts.DeleteFirstUser(driver);
        }

        #endregion
    }
}
