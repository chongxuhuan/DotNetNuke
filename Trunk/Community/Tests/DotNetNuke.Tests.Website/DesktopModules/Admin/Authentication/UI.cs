using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Website.DesktopModules.Admin.Authentication
{
    public class UI
    {
        public static IWebElement RegisterLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_userLogin_registerLink']"); }
        public static IWebElement LoginLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_userLogin_loginLink']"); }
        public static IWebElement UserNameTextBox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_ctr_Login_Login_DNN_txtUsername']"); }
        public static IWebElement PasswordTextBox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_ctr_Login_Login_DNN_txtPassword']"); }
        public static IWebElement LoginButton(IWebDriver driver){return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_ctr_Login_Login_DNN_cmdLogin']"); }
        public static IWebElement UserNameLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_dnnUser_userNameLink']"); }
    }
}
