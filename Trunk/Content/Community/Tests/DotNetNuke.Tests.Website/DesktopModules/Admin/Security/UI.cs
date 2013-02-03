using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace DotNetNuke.Tests.Website.DesktopModules.Admin.Security
{
    public class UI
    {
        public static IWebElement AddNewUserActionMenuItem(IWebDriver driver)
        {
            var userMenuUserLink = driver.FindDnnElementByXpath(driver, "//*[@id='ControlActionMenu']/li[3]/a");
            var builder = new Actions(driver);
            var hoverOverUserMenuUserLink = builder.MoveToElement(userMenuUserLink).ClickAndHold();
            //hoverOverUserMenuUserLink.Perform();
            return driver.FindDnnElementByXpath(driver, "//*[@id='ControlActionMenu']/li[3]/ul/li[1]/a");
        }

        public static IWebElement AddNewUserButton(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_Users_addUser"); }
        public static IWebElement UserNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_userForm_userName_userName_TextBox"); }
        public static IWebElement FirstNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_userForm_firstName_firstName_TextBox"); }
        public static IWebElement LastNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_userForm_lastName_lastName_TextBox"); }
        public static IWebElement DisplayNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_userForm_displayName_displayName_TextBox"); }
        public static IWebElement EmailTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_userForm_email_email_TextBox"); }
        public static IWebElement PasswordTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_txtPassword"); }
        public static IWebElement ConfirmPasswordTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_txtConfirm"); }
        public static IWebElement AuthorizeCheckbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_chkAuthorize"); }
        public static IWebElement NotifyCheckbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "ddnn_ctr449_ManageUsers_User_chkNotify"); }
        public static IWebElement RandomPasswordCheckbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_User_chkRandom"); }
        public static IWebElement RegisterButton(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr449_ManageUsers_cmdRegister"); }
    }
}

