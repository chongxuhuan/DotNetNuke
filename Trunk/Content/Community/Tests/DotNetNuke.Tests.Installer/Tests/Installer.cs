using System;
using System.Threading;

using DotNetNuke.Tests.Selenium.Tests;

using NUnit.Framework;
using OpenQA.Selenium;
using DotNetNuke.Tests.Selenium;

namespace DotNetNuke.Tests.Installer.Tests
{
    [TestFixture]
    public class Installer : Base
    {
        #region Ids

        public static By Username = By.Id("txtUsername");
        public static By UsernameLabel = By.Id("lblUsername_lblLabel");

        public static By Password = By.Id("txtPassword");
        public static By PasswordLabel = By.Id("lblPassword_lblLabel");

        public static By ConfirmPassword = By.Id("txtConfirmPassword");
        public static By ConfirmPasswordLabel = By.Id("lblConfirmPassword_lblLabel");

        public static By WebsiteName = By.Id("txtWebsiteName");
        public static By WebsiteNameLabel = By.Id("lblWebsiteName_lblLabel");

        public static By Template = By.Id("templateList_Input");
        public static By TemplateLabel = By.Id("lblTemplate_lblLabel");

        public static By Language = By.Id("languageList_Input");
        public static By LanguageLabel = By.Id("lblLanguage_lblLabel");

        public static By DatabaseServer = By.Id("txtDatabaseServerName");
        public static By DatabaseServerLabel = By.Id("lblDatabaseServerName_lblLabel");

        public static By DatabaseName = By.Id("txtDatabaseName");
        public static By DatabaseNameLabel = By.Id("lblDatabaseName_lblLabel");

        public static By DatabaseObjectQualifier = By.Id("txtDatabaseObjectQualifier");
        public static By DatabaseObjectQualifierLabel = By.Id("lblDatabaseObjectQualifier_lblLabel");

        public static By DatabaseFilename = By.Id("txtDatabaseFilename");
        public static By DatabaseFilenameLabel = By.Id("lblDatabaseFilename_lblLabel");

        public static By DatabaseUsername = By.Id("txtDatabaseUsername");
        public static By DatabaseUsernameLabel = By.Id("lblDatabaseUsername_lblLabel");

        public static By DatabasePassword = By.Id("txtDatabasePassword");
        public static By DatabasePasswordLabel = By.Id("lblDatabasePassword_lblLabel");

        public static By AdminError = By.Id("lblAdminInfoError");
        public static By AccountInfoError = By.Id("lblAccountInfoError");
        public static By ContinueButton = By.Id("continueLink");
        public static By DotNetNukeInstallHeader = By.Id("lblDotNetNukeInstalltion");

        public static By InstallStatus = By.Id("percentage");
        public static By VisitSiteButton = By.Id("visitSite");

        #endregion

        #region Xpaths

        public static By DatabaseSetupType = By.XPath("//label[@for='databaseSetupType_1']");
        public static By DatabaseType = By.XPath("//label[@for='databaseType_0']");
        public static By DatabaseSecurity = By.XPath("//label[@for='databaseSecurityType_1']");
        public static By DotNetNukeLogo = By.XPath("//img[@alt='DotNetNuke']");
        public static By TabSteps1 = By.XPath("//*[@id='tabs']/ul/li[1]/a/div/span[2]");
        public static By TabSteps2 = By.XPath("//*[@id='tabs']/ul/li[2]/a/div/span[2]");
        public static By TabSteps3 = By.XPath("//*[@id='tabs']/ul/li[3]/a/div/span[2]");
        public static By TemplateDropdownList1 = By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[1]");
        public static By TemplateDropdownList2 = By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[2]");
        public static By TemplateDropdownList3 = By.XPath("//*[@id='templateList_DropDown']/div/div/div/ul/li[3]");

        public static By TemplateDropdownList1Ie = By.XPath("//*[@id='templateList_DropDown']/div/ul/li[1]");
        public static By TemplateDropdownList2Ie = By.XPath("//*[@id='templateList_DropDown']/div/ul/li[2]");
        public static By TemplateDropdownList3Ie = By.XPath("//*[@id='templateList_DropDown']/div/ul/li[3]");

        #endregion

        #region Tests

        #region VerifyBasicFields

        [Test]
        public void VerifyBasicFieldsFirefox() { VerifyBasicFields(Firefox); }
        [Test]
        public void VerifyBasicFieldsChrome() { VerifyBasicFields(Chrome); }
        [Test]
        public void VerifyBasicFieldsIe() { VerifyBasicFields(Ie); }

        private static void VerifyBasicFields(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);
            VerifyTextboxEditable(driver, Username);
            VerifyTextboxEditable(driver, Password);
            VerifyTextboxEditable(driver, ConfirmPassword);
            VerifyTextboxEditable(driver, WebsiteName);
        }

        #endregion

        #region VerifyAdvancedFields

        [Test]
        public void VerifyAdvancedFieldsFirefox() { VerifyAdvancedFields(Firefox); }
        [Test]
        public void VerifyAdvancedFieldsChrome() { VerifyAdvancedFields(Chrome); }
        [Test]
        public void VerifyAdvancedFieldsIe() { VerifyAdvancedFields(Ie); }

        private static void VerifyAdvancedFields(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);
            driver.WaitClick(DatabaseSetupType);

            VerifyTextboxEditable(driver, DatabaseServer);
            VerifyTextboxEditable(driver, DatabaseName);
            VerifyTextboxEditable(driver, DatabaseObjectQualifier);

            driver.WaitClick(DatabaseType);

            VerifyTextboxEditable(driver, DatabaseFilename);

            driver.WaitClick(DatabaseSecurity);

            VerifyTextboxEditable(driver, DatabaseUsername);
            VerifyTextboxEditable(driver, DatabasePassword);
        }

        #endregion

        #region VerifyPasswordConfirmNoMatch

        [Test]
        public void VerifyPasswordConfirmNoMatchFirefox() { VerifyPasswordConfirmNoMatch(Firefox); }
        [Test]
        public void VerifyPasswordConfirmNoMatchChrome() { VerifyPasswordConfirmNoMatch(Chrome); }
        [Test]
        public void VerifyPasswordConfirmNoMatchIe() { VerifyPasswordConfirmNoMatch(Ie); }

        private static void VerifyPasswordConfirmNoMatch(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitSendKeys(Password, "first");
            driver.WaitSendKeys(ConfirmPassword, "second");
            driver.WaitClick(Username);

            //Wait for ajax call
            if (!driver.FindDnnElement(AdminError).Text.Contains("passwords do not match"))
                Thread.Sleep(1000);

            Assert.That(driver.FindDnnElement(AdminError).Text, Is.StringContaining("passwords do not match"), "Mismatched password error message missing");
        }

        #endregion

        #region VerifyPasswordConfirmMatch

        [Test]
        public void VerifyPasswordConfirmMatchFirefox() { VerifyPasswordConfirmMatch(Firefox); }
        [Test]
        public void VerifyPasswordConfirmMatchChrome() { VerifyPasswordConfirmMatch(Chrome); }
        [Test]
        public void VerifyPasswordConfirmMatchIe() { VerifyPasswordConfirmMatch(Ie); }

        private static void VerifyPasswordConfirmMatch(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitSendKeys(Password, "first");
            driver.WaitSendKeys(ConfirmPassword, "first");
            driver.WaitClick(Username);

            try
            {
                Assert.That(driver.FindDnnElement(AdminError), !Is.StringContaining("Passwords do not match"), "Error message on matching passwords");
            }
            catch (NoSuchElementException) { }

            Assert.Pass();
        }

        #endregion

        #region VerifyPasswordTooShortError

        [Test]
        public void VerifyPasswordTooShortErrorFirefox() { VerifyPasswordTooShortError(Firefox); }
        [Test]
        public void VerifyPasswordTooShortErrorChrome() { VerifyPasswordTooShortError(Chrome); }
        [Test]
        public void VerifyPasswordTooShortErrorIe() { VerifyPasswordTooShortError(Ie); }

        private static void VerifyPasswordTooShortError(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitSendKeys(Password, "abcdef");
            driver.WaitSendKeys(ConfirmPassword, "abcdef");
            driver.WaitClick(Username);

            Assert.That(driver.FindDnnElement(AdminError), !Is.StringContaining("Invalid Password"), "No error message on too short passwords");
        }

        #endregion

        #region VerifyLogoIsVisible

        [Test]
        public void VerifyLogoIsVisibleFirefox() { VerifyLogoIsVisible(Firefox); }
        [Test]
        public void VerifyLogoIsVisibleChrome() { VerifyLogoIsVisible(Chrome); }
        [Test]
        public void VerifyLogoIsVisibleIe() { VerifyLogoIsVisible(Ie); }

        private static void VerifyLogoIsVisible(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(VerifyImageExists(driver.FindDnnElement(DotNetNukeLogo).GetAttribute("src")), "DotNetNuke Logo not present");
        }

        #endregion

        #region VerifyInstallHeaderIsVisible

        [Test]
        public void VerifyInstallHeaderIsVisibleFirefox() { VerifyInstallHeaderIsVisible(Firefox); }
        [Test]
        public void VerifyInstallHeaderIsVisibleChrome() { VerifyInstallHeaderIsVisible(Chrome); }
        [Test]
        public void VerifyInstallHeaderIsVisibleIe() { VerifyInstallHeaderIsVisible(Ie); }

        private static void VerifyInstallHeaderIsVisible(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(driver.FindDnnElement(DotNetNukeInstallHeader).Text, Is.StringMatching("DotNetNuke Installation"), "Install Header missing or not matching control");
        }

        #endregion

        #region VerifyTabsAreInCorrectOrder

        [Test]
        public void VerifyTabsAreInCorrectOrderFirefox() { VerifyTabsAreInCorrectOrder(Firefox); }
        [Test]
        public void VerifyTabsAreInCorrectOrderChrome() { VerifyTabsAreInCorrectOrder(Chrome); }
        [Test]
        public void VerifyTabsAreInCorrectOrderIe() { VerifyTabsAreInCorrectOrder(Ie); }

        private static void VerifyTabsAreInCorrectOrder(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            Assert.That(driver.FindDnnElement(TabSteps1).Text, Is.StringMatching("Account Info"), "First tabs text is not \"Account Info\" ");
            Assert.That(driver.FindDnnElement(TabSteps2).Text, Is.StringMatching("Installation"), "Second tabs text is not \"Installation\" ");
            Assert.That(driver.FindDnnElement(TabSteps3).Text, Is.StringMatching("View Website"), "First tabs text is not \"View Website\" ");
        }

        #endregion

        #region VerifyRequiredFieldsBasic

        [Test]
        public void VerifyRequiredFieldsBasicFirefox() { VerifyRequiredFieldsBasic(Firefox); }
        [Test]
        public void VerifyRequiredFieldsBasicChrome() { VerifyRequiredFieldsBasic(Chrome); }
        [Test]
        public void VerifyRequiredFieldsBasicIe() { VerifyRequiredFieldsBasic(Ie); }

        private static void VerifyRequiredFieldsBasic(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitClick(UsernameLabel);

            Assert.That(driver.FindDnnElement(UsernameLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Username field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(PasswordLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Password field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(ConfirmPasswordLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Confirm Password field does not have \"required\" class");

            Assert.That(driver.FindDnnElement(WebsiteNameLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "WebsiteName field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(TemplateLabel).GetAttribute("class"), Is.Not.StringMatching("dnnFormRequired"), "Template field has \"required\" class when it shouldn't");
            Assert.That(driver.FindDnnElement(LanguageLabel).GetAttribute("class"), Is.Not.StringMatching("dnnFormRequired"), "Language field has \"required\" class when it shouldn't");
        }

        #endregion

        #region VerifyRequiredFieldsAdvanced

        [Test]
        public void VerifyRequiredFieldsAdvancedFirefox() { VerifyRequiredFieldsAdvanced(Firefox); }
        [Test]
        public void VerifyRequiredFieldsAdvancedChrome() { VerifyRequiredFieldsAdvanced(Chrome); }
        [Test]
        public void VerifyRequiredFieldsAdvancedIe() { VerifyRequiredFieldsAdvanced(Ie); }

        private static void VerifyRequiredFieldsAdvanced(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitClick(DatabaseSetupType);

            Assert.That(driver.FindDnnElement(DatabaseServerLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Server Name field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(DatabaseServerLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Name field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(DatabaseObjectQualifierLabel).GetAttribute("class"), !Is.StringMatching("dnnFormRequired"), "Object Qualifier field has the \"required\" class and it shouldnt");

            driver.WaitClick(DatabaseType);

            Assert.That(driver.FindDnnElement(DatabaseFilenameLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database File Name field does not have \"required\" class");

            driver.WaitClick(DatabaseSecurity);

            Assert.That(driver.FindDnnElement(DatabaseUsernameLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Username field does not have \"required\" class");
            Assert.That(driver.FindDnnElement(DatabasePasswordLabel).GetAttribute("class"), Is.StringMatching("dnnFormRequired"), "Database Password field does not have \"required\" class");
        }

        #endregion

        #region VerifyTemplateDropdownContents

        [Test]
        public void VerifyTemplateDropdownContentsFirefox() { VerifyTemplateDropdownContents(Firefox); }
        [Test]
        public void VerifyTemplateDropdownContentsChrome() { VerifyTemplateDropdownContents(Chrome); }
        [Test]
        public void VerifyTemplateDropdownContentsIe()
        {
            var driver = Ie;

            driver.Navigate().GoToUrl(StartPage);

            driver.WaitClick(Template);

            Assert.That(driver.FindDnnElement(TemplateDropdownList1Ie).Text, Is.StringMatching("Default Template"), "First Item in template dropdown is not Default Template");
            Assert.That(driver.FindDnnElement(TemplateDropdownList2Ie).Text, Is.StringMatching("Mobile Template"), "Second Item in template dropdown is not Default Template");
            Assert.That(driver.FindDnnElement(TemplateDropdownList3Ie).Text, Is.StringMatching("Blank Template"), "Third Item in template dropdown is not Default Template");
        }

        private static void VerifyTemplateDropdownContents(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitClick(Template);

            Assert.That(driver.FindDnnElement(TemplateDropdownList1).Text, Is.StringMatching("Default Template"), "First Item in template dropdown is not Default Template");
            Assert.That(driver.FindDnnElement(TemplateDropdownList2).Text, Is.StringMatching("Mobile Template"), "Second Item in template dropdown is not Default Template");
            Assert.That(driver.FindDnnElement(TemplateDropdownList3).Text, Is.StringMatching("Blank Template"), "Third Item in template dropdown is not Default Template");
        }

        #endregion

        #region RunTypicalInstall

        [Test]
        [Category("FullInstall")]
        public void RunTypicalInstallFirefox() { RunTypicalInstall(Firefox); }

        private static void RunTypicalInstall(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(StartPage);

            driver.WaitSendKeys(Password, "dnnhost");
            driver.WaitSendKeys(ConfirmPassword, "dnnhost");

            driver.WaitClick(ContinueButton);

            for (int i = 0; i < 201; i++)
            {
                try
                {

                
                if (driver.FindElement(InstallStatus).Text.Contains("ERROR"))
                    Assert.Fail("Error found on install");

                if (i > 200)
                    Assert.Fail("Install Took longer than 5 minutes");

                if (!driver.FindElement(VisitSiteButton).GetAttribute("Class").Contains("dnnDisabledAction"))
                    break;

                }
                catch (NoSuchElementException){}

                Thread.Sleep(1000);
            }

            driver.WaitClick(VisitSiteButton);

            int oldTimeout = Common.DriverTimeout;
            Common.DriverTimeout = 120000;

            driver.WaitClick(LetMeAtItButton);

            SiteSettings.DisablePopups(driver);

            SiteManagement.SwitchToNewPortal(driver, "TestPortal");

            driver.WaitClick(LetMeAtItButton);

            Common.DriverTimeout = oldTimeout;

            Login.AsHost(driver);

            Assert.That(driver.FindDnnElement(CurrentUserLink).Text, Is.StringContaining("SuperUser"));
        }

        #endregion

        #endregion
    }
}
