using DotNetNuke.Tests.NewInstaller.BaseClasses;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.NewInstaller
{
	class NavigationTests : NavigationTestBase
	{
		
		//[TestCase("chrome")]
		//[TestCase("firefox")]
		//[TestCase("ie")]
		public void Navigation(IWebDriver driver, string baseUrl)
		{
			LoginPage.LoginPage loginPage = new LoginPage.LoginPage(driver);
			loginPage.OpenLoginPage(baseUrl);

			//Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.MessageLink)).GetAttribute("title"), "The Message Link or Message Link bubble-help is missing."));
			//Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.NotificationLink)).GetAttribute("title"), "The Notification Link or Notification Link bubble-help is missing."));
			//Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.RegisteredUserLink)).GetAttribute("title"), "The Registered User Link or Registered User Link bubble-help is missing."));
			//Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.UserAvatar)).GetAttribute("title"), "The User Avatar or User Avatar bubble-help is missing."));
			//Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.LogoutLink)).GetAttribute("title"), "The Logout Link or Logout Link bubble-help is missing."));

			Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(LoginPage.LoginPage.RegisterLink)).GetAttribute("title"), "The Register Link or Register Link bubble-help is missing."));
			Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(LoginPage.LoginPage.LoginLink)).GetAttribute("title"), "The Login Link or Login Link bubble-help is missing."));

			//Utilities.SoftAssert(() => StringAssert.Contains("Basic Features", driver.FindElement(By.XPath("//span/contains(@id, '" + BasePage.PageTitle + "')")).Text, "The page title is missing."));

			Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.SearchBox)).GetAttribute("title"), "The Search Box or Search Box bubble-help is missing."));
			Utilities.SoftAssert(() => Assert.IsNotEmpty(driver.FindElement(By.Id(BasePage.SearchButton)).GetAttribute("title"), "The Search Button or Search Button bubble-help is missing."));

			Utilities.SoftAssert(() => StringAssert.Contains(BasePage.CopyrightText, driver.FindElement(By.Id(BasePage.CopyrightNotice)).Text,
				"Copyright notice is not present or contains wrong text message"));
		}

	}
}
