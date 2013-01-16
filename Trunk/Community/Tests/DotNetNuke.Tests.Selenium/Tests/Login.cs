using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    [TestFixture]
    public class Login : Base
    {
        public static By LoginButton = By.Id("dnn_dnnLogin_enhancedLoginLink");
        public static By LoginUserNameTextbox = By.Id("dnn_ctr_Login_Login_DNN_txtUsername");
        public static By LoginPasswordTextbox = By.Id("dnn_ctr_Login_Login_DNN_txtPassword");
        public static By LoginLoginButton = By.Id("dnn_ctr_Login_Login_DNN_cmdLogin");
        public static By LoginErrorMessage = By.CssSelector("*[id$='_lblMessage']");
        public static By RegisterButton = By.Id("dnn_userLogin_registerLink");

        public static void AsUser(IWebDriver driver, string user, string pass)
        {
            Logoff(driver);

            driver.Navigate().Refresh();
            driver.WaitClick(LoginButton);

            driver.WaitSendKeys(LoginUserNameTextbox, user);
            driver.WaitSendKeys(LoginPasswordTextbox, pass);
            driver.WaitClick(LoginLoginButton);

            try
            {
                var currentUser = driver.FindDnnElement(CurrentUserLink).Text;
                Console.WriteLine(currentUser);
            }
            catch (NoSuchElementException)
            {
                if (driver.FindDnnElement(LoginErrorMessage).Text.Contains("Login Failed") || driver.FindDnnElement(LoginErrorMessage).Text.Contains("Locked"))
                {
                    Console.WriteLine("Failed to Log in: " + driver.FindDnnElement(LoginErrorMessage).Text);
                }
            }
        }

        public static void AsUser(IWebDriver driver, string user, string pass, string displayName)
        {
            try
            {
                if (driver.FindDnnElement(CurrentUserLink).Text.Contains(displayName))
                    Console.WriteLine("Already Logged in as:" + displayName);
                else
                    throw new Exception("Not logged in yet.");
            }
            catch (Exception)
            {
                Logoff(driver);

                driver.Navigate().Refresh();
                driver.WaitClick(LoginButton);

                driver.WaitSendKeys(LoginUserNameTextbox, user);
                driver.WaitSendKeys(LoginPasswordTextbox, pass);
                driver.WaitClick(LoginLoginButton);

                try
                {
                    var currentUser = driver.FindDnnElement(CurrentUserLink).Text;
                    Console.WriteLine(currentUser);
                }
                catch (NoSuchElementException)
                {
                    if (driver.FindDnnElement(LoginErrorMessage).Text.Contains("Login Failed") || driver.FindDnnElement(LoginErrorMessage).Text.Contains("Locked"))
                    {
                        Console.WriteLine("Failed to Log in: " + driver.FindDnnElement(LoginErrorMessage).Text);
                    }
                }
            }
        }

        public static void AsHost(IWebDriver driver)
        {
            AsUser(driver, "host", "dnnhost", "SuperUser");
        }

        #region VerifyHostLogin

        [Test]
        public void VerifyHostLoginFirefox() { VerifyHostLogin(Firefox); }
        [Test]
        public void VerifyHostLoginChrome() { VerifyHostLogin(Chrome); }
        [Test]
        public void VerifyHostLoginIe() { VerifyHostLogin(Ie); }

        private static void VerifyHostLogin(IWebDriver driver)
        {
            AsHost(driver);

            Assert.That(driver.FindDnnElement(CurrentUserLink).Text, Is.StringContaining("SuperUser"), "SuperUser account not logged in");
        }

        #endregion

        #region VerifyLoginFailsWithBadPassword

        [Test]
        public void VerifyLoginFailsWithBadPasswordFirefox() { VerifyLoginFailsWithBadPassword(Firefox); }
        [Test]
        public void VerifyLoginFailsWithBadPasswordChrome() { VerifyLoginFailsWithBadPassword(Chrome); }
        [Test]
        public void VerifyLoginFailsWithBadPasswordIe() { VerifyLoginFailsWithBadPassword(Ie); }

        private static void VerifyLoginFailsWithBadPassword(IWebDriver driver)
        {
            AsUser(driver, "host", "wrong");

            var nth = driver.FindElements(LoginErrorMessage);

            Assert.That(nth[nth.Count-1].Text, Is.StringContaining("Login Failed"), "Able to log in with incorrect password");
        }

        #endregion
    }
}
