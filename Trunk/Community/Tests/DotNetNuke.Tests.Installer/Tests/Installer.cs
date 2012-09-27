using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using DotNetNuke.Tests.Selenium;

namespace DotNetNuke.Tests.Installer.Tests
{
    public class Installer : Base
    {
        #region Ids

        public static IWebElement Username(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtUsername")); }
        public static IWebElement UsernameLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblUsername_lblLabel")); }

        public static IWebElement Password(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtPassword")); }
        public static IWebElement PasswordLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblPassword_lblLabel")); }

        public static IWebElement ConfirmPassword(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtConfirmPassword")); }
        public static IWebElement ConfirmPasswordLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblConfirmPassword_lblLabel")); }

        public static IWebElement WebsiteName(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtWebsiteName")); }
        public static IWebElement WebsiteNameLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblWebsiteName_lblLabel")); }

        public static IWebElement Template(IWebDriver driver) { return driver.FindDnnElement(By.Id("templateList_Input")); }
        public static IWebElement TemplateLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblTemplate_lblLabel")); }

        public static IWebElement Language(IWebDriver driver) { return driver.FindDnnElement(By.Id("languageList_Input")); }
        public static IWebElement LanguageLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblLanguage_lblLabel")); }

        public static IWebElement DatabaseServer(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabaseServerName")); }
        public static IWebElement DatabaseServerLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabaseServerName_lblLabel")); }

        public static IWebElement DatabaseName(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabaseName")); }
        public static IWebElement DatabaseNameLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabaseName_lblLabel")); }

        public static IWebElement DatabaseObjectQualifier(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabaseObjectQualifier")); }
        public static IWebElement DatabaseObjectQualifierLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabaseObjectQualifier_lblLabel")); }

        public static IWebElement DatabaseFilename(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabaseFilename")); }
        public static IWebElement DatabaseFilenameLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabaseFilename_lblLabel")); }

        public static IWebElement DatabaseUsername(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabaseUsername")); }
        public static IWebElement DatabaseUsernameLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabaseUsername_lblLabel")); }

        public static IWebElement DatabasePassword(IWebDriver driver) { return driver.FindDnnElement(By.Id("txtDatabasePassword")); }
        public static IWebElement DatabasePasswordLabel(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDatabasePassword_lblLabel")); }

        public static IWebElement AdminError(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblAdminInfoError")); }
        public static IWebElement AccountInfoError(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblAccountInfoError")); }
        public static IWebElement ContinueButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("continueLink")); }
        public static IWebElement DotNetNukeInstallHeader(IWebDriver driver) { return driver.FindDnnElement(By.Id("lblDotNetNukeInstalltion")); }

        public static IWebElement InstallStatus(IWebDriver driver) { return driver.FindDnnElement(By.Id("percentage")); }
        public static IWebElement VisitSiteButton(IWebDriver driver) { return driver.FindDnnElement(By.Id("visitSite")); }

        public static IWebElement ClosePopupButton(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='Body']/div[1]/div[1]/a/a[2]")); }

        #endregion

        #region Xpaths

        public static IWebElement DatabaseSetupType(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//label[@for='databaseSetupType_1']")); }
        public static IWebElement DatabaseType(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//label[@for='databaseType_0']")); }
        public static IWebElement DatabaseSecurity(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//label[@for='databaseSecurityType_1']")); }
        public static IWebElement DotNetNukeLogo(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//img[@alt='DotNetNuke']")); }
        public static IWebElement TabSteps1(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='tabs']/ul/li[1]/a/div/span[2]")); }
        public static IWebElement TabSteps2(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='tabs']/ul/li[2]/a/div/span[2]")); }
        public static IWebElement TabSteps3(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='tabs']/ul/li[3]/a/div/span[2]")); }
        public static IWebElement TemplateDropdownList1(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[1]")); }
        public static IWebElement TemplateDropdownList2(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[2]")); }
        public static IWebElement TemplateDropdownList3(IWebDriver driver) { return driver.FindDnnElement(By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[3]")); }
        #endregion
    }

    [TestFixture]
    public class InstallerTests : Base
    {
        private IWebDriver _firefox;
        private IWebDriver _chrome;
        private IWebDriver _ie;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Common.DriverTimeout = 10000;
            _firefox = StartBrowser(Common.BrowserType.firefox);
            _chrome = StartBrowser(Common.BrowserType.chrome);
            _ie = StartBrowser(Common.BrowserType.ie);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _firefox.Quit();
            _chrome.Quit();
            _ie.Quit();
        }

        #region Tests

        #region VerifyBasicFields

        [Test]
        public void VerifyBasicFieldsFirefox() { VerifyBasicFields(_firefox); }
        [Test]
        public void VerifyBasicFieldsChrome() { VerifyBasicFields(_chrome); }
        [Test]
        public void VerifyBasicFieldsIe() { VerifyBasicFields(_ie); }

        private static void VerifyBasicFields(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);
            VerifyTextboxEditable(Installer.Username(driver));
            VerifyTextboxEditable(Installer.Password(driver));
            VerifyTextboxEditable(Installer.ConfirmPassword(driver));
            VerifyTextboxEditable(Installer.WebsiteName(driver));
        }

        #endregion

        #region VerifyAdvancedFields

        [Test]
        public void VerifyAdvancedFieldsFirefox() { VerifyAdvancedFields(_firefox); }
        [Test]
        public void VerifyAdvancedFieldsChrome() { VerifyAdvancedFields(_chrome); }
        [Test]
        public void VerifyAdvancedFieldsIe() { VerifyAdvancedFields(_ie); }

        private static void VerifyAdvancedFields(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);
            Installer.DatabaseSetupType(driver).Click();

            VerifyTextboxEditable(Installer.DatabaseServer(driver));
            VerifyTextboxEditable(Installer.DatabaseName(driver));
            VerifyTextboxEditable(Installer.DatabaseObjectQualifier(driver));

            Installer.DatabaseType(driver).Click();

            VerifyTextboxEditable(Installer.DatabaseFilename(driver));

            Installer.DatabaseSecurity(driver).Click();

            VerifyTextboxEditable(Installer.DatabaseUsername(driver));
            VerifyTextboxEditable(Installer.DatabasePassword(driver));
        }

        #endregion

        #region VerifyPasswordConfirmNoMatch

        [Test]
        public void VerifyPasswordConfirmNoMatchFirefox() { VerifyPasswordConfirmNoMatch(_firefox); }
        [Test]
        public void VerifyPasswordConfirmNoMatchChrome() { VerifyPasswordConfirmNoMatch(_chrome); }
        [Test]
        public void VerifyPasswordConfirmNoMatchIe() { VerifyPasswordConfirmNoMatch(_ie); }

        private static void VerifyPasswordConfirmNoMatch(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.Password(driver).SendKeys("first");
            Installer.ConfirmPassword(driver).SendKeys("second");
            Installer.Username(driver).Click();

            Assert.That(Installer.AdminError(driver).Text, Is.StringContaining("Passwords do not match"), "Mismatched password error message missing");
        }

        #endregion

        #region VerifyPasswordConfirmMatch

        [Test]
        public void VerifyPasswordConfirmMatchFirefox() { VerifyPasswordConfirmMatch(_firefox); }
        [Test]
        public void VerifyPasswordConfirmMatchChrome() { VerifyPasswordConfirmMatch(_chrome); }
        [Test]
        public void VerifyPasswordConfirmMatchIe() { VerifyPasswordConfirmMatch(_ie); }

        private static void VerifyPasswordConfirmMatch(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.Password(driver).SendKeys("first");
            Installer.ConfirmPassword(driver).SendKeys("first");
            Installer.Username(driver).Click();

            Assert.That(Installer.AdminError(driver), !Is.StringContaining("Passwords do not match"), "Error message on matching passwords");
        }

        #endregion

        #region VerifyPasswordTooShortError

        [Test]
        public void VerifyPasswordTooShortErrorFirefox() { VerifyPasswordTooShortError(_firefox); }
        [Test]
        public void VerifyPasswordTooShortErrorChrome() { VerifyPasswordTooShortError(_chrome); }
        [Test]
        public void VerifyPasswordTooShortErrorIe() { VerifyPasswordTooShortError(_ie); }

        private static void VerifyPasswordTooShortError(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.Password(driver).SendKeys("abcdef");
            Installer.ConfirmPassword(driver).SendKeys("abcdef");
            Installer.ContinueButton(driver).Click();

            Assert.That(Installer.AccountInfoError(driver), !Is.StringContaining("Invalid Password"), "Error message on matching passwords");
        }

        #endregion

        #region VerifyLogoIsVisible

        [Test]
        public void VerifyLogoIsVisibleFirefox() { VerifyLogoIsVisible(_firefox); }
        [Test]
        public void VerifyLogoIsVisibleChrome() { VerifyLogoIsVisible(_chrome); }
        [Test]
        public void VerifyLogoIsVisibleIe() { VerifyLogoIsVisible(_ie); }

        private static void VerifyLogoIsVisible(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(VerifyImageExists(Installer.DotNetNukeLogo(driver).GetAttribute("src")), "DotNetNuke Logo not present");
        }

        #endregion

        #region VerifyInstallHeaderIsVisible

        [Test]
        public void VerifyInstallHeaderIsVisibleFirefox() { VerifyInstallHeaderIsVisible(_firefox); }
        [Test]
        public void VerifyInstallHeaderIsVisibleChrome() { VerifyInstallHeaderIsVisible(_chrome); }
        [Test]
        public void VerifyInstallHeaderIsVisibleIe() { VerifyInstallHeaderIsVisible(_ie); }

        private static void VerifyInstallHeaderIsVisible(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(Installer.DotNetNukeInstallHeader(driver).Text, Is.StringMatching("DotNetNuke Installation"), "Install Header missing or not matching control");
        }

        #endregion

        #region VerifyTabsAreInCorrectOrder

        [Test]
        public void VerifyTabsAreInCorrectOrderFirefox() { VerifyTabsAreInCorrectOrder(_firefox); }
        [Test]
        public void VerifyTabsAreInCorrectOrderChrome() { VerifyTabsAreInCorrectOrder(_chrome); }
        [Test]
        public void VerifyTabsAreInCorrectOrderIe() { VerifyTabsAreInCorrectOrder(_ie); }

        private static void VerifyTabsAreInCorrectOrder(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(Installer.TabSteps1(driver).Text, Is.StringMatching("Account Info"), "First tabs text is not \"Account Info\" ");
            Assert.That(Installer.TabSteps2(driver).Text, Is.StringMatching("Installation"), "Second tabs text is not \"Installation\" ");
            Assert.That(Installer.TabSteps3(driver).Text, Is.StringMatching("View Website"), "First tabs text is not \"View Website\" ");
        }

        #endregion

        #region VerifyRequiredFieldsBasic

        [Test]
        public void VerifyRequiredFieldsBasicFirefox() { VerifyRequiredFieldsBasic(_firefox); }
        [Test]
        public void VerifyRequiredFieldsBasicChrome() { VerifyRequiredFieldsBasic(_chrome); }
        [Test]
        public void VerifyRequiredFieldsBasicIe() { VerifyRequiredFieldsBasic(_ie); }

        private static void VerifyRequiredFieldsBasic(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.UsernameLabel(driver).Click();

            Assert.That(Installer.UsernameLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Username field does not have \"required\" class");
            Assert.That(Installer.PasswordLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Password field does not have \"required\" class");
            Assert.That(Installer.ConfirmPasswordLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Confirm Password field does not have \"required\" class");

            Assert.That(Installer.WebsiteNameLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "WebsiteName field does not have \"required\" class");
            Assert.That(Installer.TemplateLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Template field does not have \"required\" class");
            Assert.That(Installer.LanguageLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Language field does not have \"required\" class");
        }

        #endregion

        #region VerifyRequiredFieldsAdvanced

        [Test]
        public void VerifyRequiredFieldsAdvancedFirefox() { VerifyRequiredFieldsAdvanced(_firefox); }
        [Test]
        public void VerifyRequiredFieldsAdvancedChrome() { VerifyRequiredFieldsAdvanced(_chrome); }
        [Test]
        public void VerifyRequiredFieldsAdvancedIe() { VerifyRequiredFieldsAdvanced(_ie); }

        private static void VerifyRequiredFieldsAdvanced(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.DatabaseSetupType(driver).Click();

            Assert.That(Installer.DatabaseServerLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Server Name field does not have \"required\" class");
            Assert.That(Installer.DatabaseServerLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Name field does not have \"required\" class");
            Assert.That(Installer.DatabaseObjectQualifierLabel(driver).GetAttribute("class"), !Is.StringMatching("dnnFormRequired"), "Object Qualifier field has the \"required\" class and it shouldnt");

            Installer.DatabaseType(driver).Click();

            Assert.That(Installer.DatabaseFilenameLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database File Name field does not have \"required\" class");

            Installer.DatabaseSecurity(driver).Click();

            Assert.That(Installer.DatabaseUsernameLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Username field does not have \"required\" class");
            Assert.That(Installer.DatabasePasswordLabel(driver).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Password field does not have \"required\" class");
        }

        #endregion

        #region VerifyTemplateDropdownContents

        [Test]
        public void VerifyTemplateDropdownContentsFirefox() { VerifyTemplateDropdownContents(_firefox); }
        [Test]
        public void VerifyTemplateDropdownContentsChrome() { VerifyTemplateDropdownContents(_chrome); }
        [Test]
        public void VerifyTemplateDropdownContentsIe() { VerifyTemplateDropdownContents(_ie); }

        private static void VerifyTemplateDropdownContents(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.Template(driver).Click();

            Assert.That(Installer.TemplateDropdownList1(driver).Text, Is.StringMatching("Default Template"), "First Item in template dropdown is not Default Template");
            Assert.That(Installer.TemplateDropdownList2(driver).Text, Is.StringMatching("Mobile Template"), "Second Item in template dropdown is not Default Template");
            Assert.That(Installer.TemplateDropdownList3(driver).Text, Is.StringMatching("Blank Template"), "Third Item in template dropdown is not Default Template");
        }

        #endregion

        #region RunTypicalInstall

        [Test]
        [Category("FullInstall")]
        public void RunTypicalInstallChrome() { RunTypicalInstall(_chrome); }

        private static void RunTypicalInstall(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Installer.Password(driver).SendKeys("dnnhost");
            Installer.ConfirmPassword(driver).SendKeys("dnnhost");

            Installer.ContinueButton(driver).Click();

            DateTime beginWait = DateTime.Now;

            while (true)
            {

                if (Installer.InstallStatus(driver).Text.Contains("ERROR"))
                    Assert.Fail("Error found on install");

                if (DateTime.Now.Subtract(beginWait).Seconds > 300)
                    Assert.Fail("Install Took longer than 5 minutes");

                if (Installer.VisitSiteButton(driver).GetAttribute("disabled") != "true")
                    break;

                Thread.Sleep(1000);
            }

            Installer.VisitSiteButton(driver).Click();

            driver.Navigate().GoToUrl(StartPage);

            try
            {
                Installer.ClosePopupButton(driver).Click();
            }
            catch(NoSuchElementException){}

            Assert.That(CurrentUserLink(driver).Text, Is.StringContaining("SuperUser"));
        }

        #endregion

        #endregion

        private static void VerifyTextboxEditable(IWebElement element)
        {
            string origText = element.Text;
            element.Clear();
            element.SendKeys("TestString");
            Assert.That(element.GetAttribute("value"), Is.StringMatching("TestString"));

            element.Clear();
            element.SendKeys(origText);
        }

        private static bool VerifyImageExists(string url)
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
}
