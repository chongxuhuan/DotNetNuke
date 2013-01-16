using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class SuperuserAccounts : Base
    {
        public static By AddNewSuperuserButton = By.CssSelector("*[id$='_Users_addUser']");

        public static By EditSuperuserUsernameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_userName_userName_TextBox']");
        public static By EditSuperuserFirstnameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_firstName_firstName_TextBox']");
        public static By EditSuperuserLastnameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_lastName_lastName_TextBox']");
        public static By EditSuperuserDisplayNameTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_displayName_displayName_TextBox']");
        public static By EditSuperuserEmailTextbox = By.CssSelector("*[id$='_ManageUsers_User_userForm_email_email_TextBox']");

        public static By EditSuperuserPasswordTextbox = By.CssSelector("*[id$='_ManageUsers_User_txtPassword']");
        public static By EditSuperuserPasswordConfirmTextbox = By.CssSelector("*[id$='_ManageUsers_User_txtConfirm']");

        public static By EditSuperuserAddNewUserButton = By.CssSelector("*[id$='_ManageUsers_cmdRegister']");

        public static void AddNewSuperuser(IWebDriver driver, string userName, string firstName, string lastName, string displayName, string email, string password)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SuperUserAccountsPage);

            driver.WaitClick(AddNewSuperuserButton);

            driver.WaitClick(EditSuperuserUsernameTextbox);
            driver.WaitSendKeys(EditSuperuserUsernameTextbox, userName);

            driver.WaitSendKeys(EditSuperuserFirstnameTextbox, firstName);
            driver.WaitSendKeys(EditSuperuserLastnameTextbox, lastName);
            driver.WaitSendKeys(EditSuperuserDisplayNameTextbox, displayName);
            driver.WaitSendKeys(EditSuperuserEmailTextbox, email);
            driver.WaitSendKeys(EditSuperuserPasswordTextbox, password);
            driver.WaitSendKeys(EditSuperuserPasswordConfirmTextbox, password);

            driver.WaitClick(EditSuperuserAddNewUserButton);
        }


        #region VerifyAddNewSuperuser

        [Test]
        public void VerifyAddNewSuperuserFirefox() { VerifyAddNewSuperuser(Firefox); }
        [Test]
        public void VerifyAddNewSuperuserChrome() { VerifyAddNewSuperuser(Chrome); }
        [Test]
        public void VerifyAddNewSuperuserIe() { VerifyAddNewSuperuser(Ie); }

        private static void VerifyAddNewSuperuser(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            var userName = "SuperMan" + rndNum;
            var pass = "supermanPass";

            AddNewSuperuser(driver, userName, "Super", "Man", userName, userName + "@dnncorp.com", pass);

            Login.AsUser(driver, userName, pass);

            Assert.That(driver.FindDnnElement(CurrentUserLink).Text, Is.StringContaining(userName));
        }

        #endregion

    }
}
