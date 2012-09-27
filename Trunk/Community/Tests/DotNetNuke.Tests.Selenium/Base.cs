using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace DotNetNuke.Tests.Selenium
{
    public class Base
    {
        public static string StartPage = "http://localhost/DotNetNuke_Enterprise/";
        public static string UserAccountsPage = StartPage + "Admin/UserAccounts.aspx";
        public static string SiteSettingsPage = StartPage + "Admin/SiteSettings.aspx";
        public static string HostSettingsPage = StartPage + "Host/HostSettings.aspx";
        public static string RecycleBinPage = StartPage + "Admin/RecycleBin.aspx";
        public static string LogoffPage = StartPage + "Logoff.aspx";

        public static IWebElement CurrentUserLink(IWebDriver driver) { return driver.FindDnnElement(By.Id("dnn_dnnUser_userNameLink")); }

        private FirefoxProfile _ffp;
        private IWebDriver _driver;

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
                    if (element.Displayed)
                        return element;
                }
                catch (NoSuchElementException) { }
                catch (ElementNotVisibleException) { }
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
                var popUp = driver.FindElement(By.Id("iPopUp"));
                driver.SwitchTo().Frame(popUp);
            }
            catch (NoSuchElementException) { }

            return Wait.ForElement(driver, by, Common.DriverTimeout);
        }
    }

    [SetUpFixture]
    public class Setup
    {
        // Runs before all tests

        [SetUp]
        public void SetUp()
        {
            Common.DriverTimeout = 10000;
        }

        [TearDown]
        public void TearDown()
        {
            Common.TestSite = null;
            
        }
    }
}
