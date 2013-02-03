using System;
using System.Net;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace DotNetNuke.Tests.Selenium
{
    public class Base
    {
        public static IWebDriver Firefox;
        public static IWebDriver Chrome;
        public static IWebDriver Ie;

        public static string StartPage = "http://localhost/DotNetNuke_Enterprise/";
        public static string UserAccountsPage = StartPage + "Admin/UserAccounts.aspx";
        public static string SiteSettingsPage = StartPage + "Admin/SiteSettings.aspx";
        public static string HostSettingsPage = StartPage + "Host/HostSettings.aspx";
        public static string RecycleBinPage = StartPage + "Admin/RecycleBin.aspx";
        public static string SiteManagementPage = StartPage + "Host/SiteManagement.aspx";
        public static string SecurityRolesPage = StartPage + "Admin/SecurityRoles.aspx";
        public static string SuperUserAccountsPage = StartPage + "Host/SuperuserAccounts.aspx";
        public static string LogoffPage = StartPage + "Logoff.aspx";

        public static By CurrentUserLink = By.Id("dnn_dnnUser_enhancedRegisterLink");
        public static By LetMeAtItButton = By.Id("btnLetMeAtIn");

        private FirefoxProfile _ffp;
        private IWebDriver _driver;

        [TestFixtureSetUp]
        public void SetUp()
        {
            Common.DriverTimeout = 10000;

            Firefox = StartBrowser(Common.BrowserType.firefox);
            Firefox.Manage().Window.Maximize();
            Firefox.Navigate().GoToUrl(StartPage);

            Chrome = StartBrowser(Common.BrowserType.chrome);
            Chrome.Manage().Window.Maximize();
            Chrome.Navigate().GoToUrl(StartPage);

            Ie = StartBrowser(Common.BrowserType.ie);
            Ie.Manage().Window.Maximize();
            Ie.Navigate().GoToUrl(StartPage);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Common.TestSite = null;
            Firefox.Quit();
            Chrome.Quit();
            Ie.Quit();
        }

        public IWebDriver StartBrowser(string browserType)
        {
            switch (browserType)
            {
                case Common.BrowserType.firefox:
                    _ffp = new FirefoxProfile();
                    _ffp.AcceptUntrustedCertificates = true;
                    _driver = new FirefoxDriver(_ffp);
                    break;
                case Common.BrowserType.ie:
                    _driver = new InternetExplorerDriver();
                    break;
                case Common.BrowserType.chrome:
                    _driver = new ChromeDriver();
                    break;
            }

            return _driver;
        }

        public static void Logoff(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(LogoffPage);
        }

        public static string GetBrowser(IWebDriver driver)
        {
            var js = driver as IJavaScriptExecutor;

            if (js == null)
                throw new Exception("Could not get browser type");

            return (string)js.ExecuteScript("return navigator.userAgent;");
        }

        public static void VerifyTextboxEditable(IWebDriver driver, By byIn)
        {

            string origText = driver.FindDnnElement(byIn).Text;
            driver.FindDnnElement(byIn).Clear();
            driver.FindDnnElement(byIn).SendKeys("TestString");
            Assert.That(driver.FindDnnElement(byIn).GetAttribute("value"), Is.StringMatching("TestString"));

            driver.FindDnnElement(byIn).Clear();
            driver.FindDnnElement(byIn).SendKeys(origText);
        }

        public static bool VerifyImageExists(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;
            }
            catch
            {
                exists = false;
            }
            return exists;
        }
    }

    public class Wait
    {
        public const int DefaultTimeout = 5000;
        public static IWebElement ForElement(ISearchContext searchContext,
      By by, int millisecondsToWait = DefaultTimeout)
        {
            var endTime = DateTime.Now.AddMilliseconds(millisecondsToWait);
            while (DateTime.Now < endTime)
            {
                try
                {
                    var element = searchContext.FindElement(by);
                    return element;
                }
                catch (NoSuchElementException) { }
            }
            throw new NoSuchElementException(by + " not found in " +
        millisecondsToWait + "ms");
        }
    }

    public static class WebElementExtensions
    {
        public static void WaitClick(this IWebDriver driver, By byIn)
        {
            var endTime = DateTime.Now.AddMilliseconds(Common.DriverTimeout);

            while (DateTime.Now < endTime)
            {
                try
                {
                    driver.FindDnnElement(byIn).Click();
                    break;
                }
                catch { }
            }
        }

        public static void WaitSendKeys(this IWebDriver driver, By byIn, string keys)
        {
            var endTime = DateTime.Now.AddMilliseconds(Common.DriverTimeout);

            while (DateTime.Now < endTime)
            {
                try
                {
                    driver.FindDnnElement(byIn).SendKeys(keys);
                    break;
                }
                catch { }
            }
        }
    }

    public static class WebDriverExtensions
    {
        public static IWebElement FindDnnElement(this IWebDriver driver, By by)
        {
            driver.SwitchTo().DefaultContent();

            try
            {
                driver.SwitchTo().Frame(driver.FindElement(By.Id("iPopUp")));
            }
            catch (NoSuchElementException) { }

            return Wait.ForElement(driver, by, Common.DriverTimeout);
        }
    }
}
