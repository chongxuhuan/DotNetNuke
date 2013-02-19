using System.Diagnostics;
using DotNetNuke.Tests.NewInstaller.BaseClasses;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.NewInstaller.LoginPage
{
	public class LoginPage : BasePage
	{
		public LoginPage (IWebDriver driver) : base (driver) {}

		public static string RegisterLink = "dnn_dnnUser_enhancedRegisterLink";
		public static string LoginLink = "dnn_dnnLogin_enhancedLoginLink";

		public static string LoginUserNameTextbox = "dnn_ctr_Login_Login_DNN_txtUsername";
		public static string LoginPasswordTextbox = "dnn_ctr_Login_Login_DNN_txtPassword";

		public static string LoginButton = "dnn_ctr_Login_Login_DNN_cmdLogin";
		public static string RememberLoginCheckBox = "dnn_ctr_Login_Login_DNN_chkCookie";
		public static string RegisterButton = "dnn_ctr_Login_Login_DNN_registerLink";
		public static string RetrieveButton = "dnn_ctr_Login_Login_DNN_passwordLink";

		public static string LoginPageUrl = "/login.aspx";


		//navigation to Login page
		public void OpenLoginPage(string baseUrl)
		{
			Trace.WriteLine(BasePage.TraceLevelPage + "Open 'Login' page:");
			GoToUrl(baseUrl + LoginPageUrl);
		}

		public void DoLoginUsingLoginLink(string userName, string password)
		{
			WaitForElement(By.XPath("//*[@id='" + LoginLink + "' and not(contains(@href, 'Logoff'))]"), 20).WaitTillVisible(20).Click();

			WaitAndSwitchToFrame(60);

			Trace.WriteLine(BasePage.TraceLevelPage + "Login using user credentials:");
			
			Trace.WriteLine(BasePage.TraceLevelElement + "Set '" + userName + "' in input field [id: " + LoginUserNameTextbox +"]");
			WaitForElement(By.Id(LoginUserNameTextbox), 60).WaitTillVisible().SendKeys(userName);

			Trace.WriteLine(BasePage.TraceLevelElement + "Set '" + password + "' in input field [id: " + LoginPasswordTextbox + "]");
			WaitForElement(By.Id(LoginPasswordTextbox), 60).WaitTillVisible().SendKeys(password);

			Trace.WriteLine(BasePage.TraceLevelElement + "Click on button [id: " + LoginButton + "]");			
			WaitForElement(By.Id(LoginButton)).Click();

			WaitAndSwitchToWindow(60);
		}

		public void LoginUsingLoginLink(string userName, string password)
		{
			Trace.WriteLine(BasePage.TraceLevelComposite + "Login using 'Login' link:");
			LetMeOut();
			DoLoginUsingLoginLink(userName, password);
		}

		public void DoLoginUsingUrl(string username, string password)
		{
			Trace.WriteLine(BasePage.TraceLevelPage + "Login using user credentials:");

			WaitAndType(By.Id(LoginUserNameTextbox), username);
			WaitAndType(By.Id(LoginPasswordTextbox), password);

			Click(By.Id(LoginButton));

			WaitForElement(By.Id(CopyrightNotice), 60).WaitTillVisible();
		}

		public void LoginUsingUrl(string baseUrl, string username, string password)
		{
			Trace.WriteLine(BasePage.TraceLevelComposite + "Login using url:");
			LetMeOut();
			OpenLoginPage(baseUrl);
			DoLoginUsingUrl(username, password);
		}

		public void LoginAsHost(string baseUrl)
		{
			Trace.WriteLine(BasePage.TraceLevelComposite + "Login as default host:");
			LetMeOut();
			OpenLoginPage(baseUrl);
			DoLoginUsingUrl("host", "dnnhost");

		}

		public void LoginAsAdmin(string baseUrl)
		{
			Trace.WriteLine(BasePage.TraceLevelComposite + "Login as default admin:");
			LetMeOut();
			OpenLoginPage(baseUrl);
			DoLoginUsingUrl("admin", "dnnadmin");
		}

		public void LoginAsDefaultUser(string baseUrl)
		{
			Trace.WriteLine(BasePage.TraceLevelComposite + "Login as default user:");
			LetMeOut();
			OpenLoginPage(baseUrl);
			DoLoginUsingUrl("user", "dnnuser");
		}

		public void LetMeOut()
		{
			Trace.WriteLine(BasePage.TraceLevelPage + "Logout");
			Trace.WriteLine(BasePage.TraceLevelElement + "Click on button [id: " + LogoutLink + "]");
			WaitForElement(By.Id(LogoutLink), 20).Click();
		}

	}
}
