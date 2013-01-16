using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class UserAccounts : Base
    {

        public static By AccountManagerAddNewUserButton = By.CssSelector("*[id$='_Users_cmdAddUser']");
        public static By AccountManagerDeleteButton = By.CssSelector("tr[id$='_Users_grdUsers_ctl00__0'] input[onclick*='Delete This Item']"); 
        public static By AccountManagerRemoveButton = By.CssSelector("tr[id$='_Users_grdUsers_ctl00__0'] input[onclick*='Remove This Item']");
        public static By AddNewUserUsernameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_userName_userName_TextBox']");
        public static By AddNewUserFirstNameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_firstName_firstName_TextBox']"); 
        public static By AddNewUserLastNameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_lastName_lastName_TextBox']"); 
        public static By AddNewUserDisplayNameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_displayName_displayName_TextBox']"); 
        public static By AddNewUserEmailTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_email_email_TextBox']"); 
        public static By AddNewUserAuthorizeCheckbox = By.CssSelector("*[id$='_ManageUsers_User_chkAuthorize']"); 
        public static By AddNewUserNotifyCheckbox = By.CssSelector("*[id$='_ManageUsers_User_chkNotify']"); 
        public static By AddNewUserPasswordTextbox = By.CssSelector("*[id$='_ManageUsers_User_txtPassword']"); 
        public static By AddNewUserConfirmPasswordTextbox = By.CssSelector("*[id$='_ManageUsers_User_txtConfirm']"); 
        public static By AddNewUserAddNewUserButton = By.CssSelector("*[id$='_ManageUsers_cmdRegister']");

        public static void DeleteFirstUser(IWebDriver driver)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(UserAccountsPage);

            driver.WaitClick(AccountManagerDeleteButton);

            driver.SwitchTo().Alert().Accept();
            driver.SwitchTo().DefaultContent();

            driver.WaitClick(AccountManagerRemoveButton);

            driver.SwitchTo().Alert().Accept();
            driver.SwitchTo().DefaultContent();
        }

        public static void AddNewUser(IWebDriver driver, string username, string firstName, string lastName, string displayName, string email, string password)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(UserAccountsPage);

            driver.WaitClick(AccountManagerAddNewUserButton);

            driver.WaitSendKeys(AddNewUserUsernameTextbox, username);
            driver.WaitSendKeys(AddNewUserFirstNameTextbox, firstName);
            driver.WaitSendKeys(AddNewUserLastNameTextbox, lastName);
            driver.WaitSendKeys(AddNewUserDisplayNameTextbox, displayName);
            driver.WaitSendKeys(AddNewUserEmailTextbox, email);
            driver.WaitClick(AddNewUserNotifyCheckbox);
            driver.WaitSendKeys(AddNewUserPasswordTextbox, password);
            driver.WaitSendKeys(AddNewUserConfirmPasswordTextbox, password);

            driver.WaitClick(AddNewUserAddNewUserButton);
        }

        #region VerifyAddUser

        [Test]
        public void VerifyAddUserFirefox() { VerifyAddUser(Firefox); }
        [Test]
        public void VerifyAddUserChrome() { VerifyAddUser(Chrome); }
        [Test]
        public void VerifyAddUserIe() { VerifyAddUser(Ie); }

        private static void VerifyAddUser(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            AddNewUser(driver, "TestAddUser" + rndNum, "TestAdd", "User", "TestAddUser" + rndNum, "TestAddUser" + rndNum + "@dnncorp.com", "testadduserpass");

            Login.AsUser(driver, "TestAddUser" + rndNum, "testadduserpass");

            Assert.That(driver.FindDnnElement(CurrentUserLink).Text, Is.StringContaining("TestAddUser" + rndNum), "User Not Added Correctly or not logged in");
        }

        #endregion
    }
}
