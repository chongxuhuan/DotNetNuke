using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Website.DesktopModules.Admin.Security
{
    public class UI
    {
        public static IWebElement LoginButton(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_userLogin_loginLink"); }
        public static IWebElement UserNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr_Login_Login_DNN_txtUsername"); }
        public static IWebElement FirstNameTextbox(IWebDriver driver) { return driver.FindDnnElementById(driver, "dnn_ctr_Login_Login_DNN_txtUsername"); }

    }
}
