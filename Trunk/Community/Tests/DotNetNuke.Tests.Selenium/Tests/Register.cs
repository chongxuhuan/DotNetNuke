using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    [TestFixture]
    public class Register : Base
    {
        public static IWebElement RegisterButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_dnnUser_enhancedRegisterLink")); }

        public static IWebElement UsernameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Username_Username_TextBox")); }
        public static IWebElement PasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Password_Password_TextBox")); }
        public static IWebElement ConfirmPasswordTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_PasswordConfirm_PasswordConfirm_TextBox")); }
        public static IWebElement DisplayNameTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_DisplayName_DisplayName_TextBox")); }
        public static IWebElement EmailTextbox(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_userForm_Email_Email_TextBox")); }
        public static IWebElement ContinueButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_ctr_Register_registerButton")); }

        #region VerifyRegisterUser

        [Test]
        public void VerifyRegisterUserFirefox() { VerifyRegisterUser(Firefox); }
        [Test]
        public void VerifyRegisterUserChrome() { VerifyRegisterUser(Chrome); }
        [Test]
        public void VerifyRegisterUserIe() { VerifyRegisterUser(Ie); }

        private static void VerifyRegisterUser(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            Logoff(driver);

            RegisterButton(driver).Click();

            UsernameTextbox(driver).SendKeys("TestUser" + rndNum);
            PasswordTextbox(driver).SendKeys("password#1");
            ConfirmPasswordTextbox(driver).SendKeys("password#1");
            DisplayNameTextbox(driver).SendKeys("TestUser" + rndNum);
            EmailTextbox(driver).SendKeys("TestUser" + rndNum + "@dnncorp.com");
            ContinueButton(driver).Click();

            Assert.That(driver.FindDnnElement(CurrentUserLink).Text, Is.StringContaining("TestUser" + rndNum), "User Not registered or not logged in");
        }

        #endregion
    }
}
