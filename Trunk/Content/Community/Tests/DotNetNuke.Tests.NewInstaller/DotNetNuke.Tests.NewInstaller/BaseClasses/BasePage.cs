using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using DotNetNuke.Tests.NewInstaller.Properties;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DotNetNuke.Tests.NewInstaller.BaseClasses
{
	public class BasePage
	{
		protected readonly IWebDriver _driver;

		//public static string PageTitle = "dnnTITLE_titleLabel";

		public static string LogoutLink = "dnn_dnnLogin_enhancedLoginLink";

				/*		public static string MessageLink = "dnn_dnnUser_messageLink";
						public static string NotificationLink = "dnn_dnnUser_notificationLink";
						public static string RegisteredUserLink = "dnn_dnnUser_enhancedRegisterLink";
						public static string UserAvatar = "dnn_dnnUser_avatar";
				*/
		public static string SearchBox = "dnn_dnnSearch_txtSearch";
		public static string SearchButton = "dnn_dnnSearch_cmdSearch";
				
		public static string CopyrightNotice = "dnn_dnnCopyright_lblCopyright";
		public static string CopyrightText = "Copyright DotNetNuke Corporation, 2013";

		public const string TraceLevelLow = "\t\t\t[D] ";
		public const string TraceLevelElement = "\t\t[E] ";
		public const string TraceLevelPage = "\t[P] ";
		public const string TraceLevelComposite = "[C] ";

		private BasePage() {}
		public BasePage (IWebDriver driver)
		{
			_driver = driver;
		}

		public void GoToUrl (string url)
		{
			Trace.WriteLine(TraceLevelLow + "Open url: http://" + url);
			if (_driver != null) _driver.Navigate().GoToUrl("http://" + url);

			WaitTillPageIsLoaded(2 * 60);
		}

		protected Dictionary<string, string> _translate = null;
		public string Translate(string key)
		{
			string localized = _translate[key];
			Trace.WriteLine(BasePage.TraceLevelElement + "Translating '" + key + "'" + " to  '" + localized + "'");

			return localized;
		}

		/*
			var js = (IJavaScriptExecutor)driver;
			js.ExecuteScript
			(
					"var scriptElement=document.createElement('script');" +
					"scriptElement.setAttribute('type','text/javascript');" +
					"document.getElementsByTagName('head')[0].appendChild(scriptElement);" +
					"scriptElement.appendChild(document.createTextNode('function myScript(){ alert(\\'my alert\\'); }'));"
			);

			js.ExecuteAsyncScript("myScript()");

			return;
*/
		#region Waitings
		
		public const int FindElementTimeOut = 5;
		public static IWebElement WaitForElement(IWebDriver driver, By locator, int timeOutSeconds = FindElementTimeOut)
		{
			timeOutSeconds *= Settings.Default.WaitFactor;

			Trace.Write(TraceLevelLow + "Wait for element '" + locator + "' is present. ");
			Trace.WriteLine("Max waiting time: " + timeOutSeconds + " sec");

			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutSeconds));
			return wait.Until(d => d.FindElement(locator));
		}

		public IWebElement WaitForElement(By locator, int timeOutSeconds = FindElementTimeOut)
		{
			return WaitForElement(_driver, locator, timeOutSeconds);
		}

		public static void WaitForElementNotPresent(IWebDriver driver, By locator, int timeOutSeconds = FindElementTimeOut)
		{
			timeOutSeconds *= Settings.Default.WaitFactor;

			Trace.Write(TraceLevelLow + "Wait for element '" + locator + "' is NOT present. ");
			Trace.WriteLine("Max waiting time: " + timeOutSeconds + " sec");

			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOutSeconds));
			wait.Until(d =>
				           {
					           try
					           {
								   return d.FindElement(locator) == null;
							   }
							   catch (NoSuchElementException)
							   {
								   return true;
							   }
						   });
		}

		public void WaitForElementNotPresent(By locator, int timeOutSeconds = FindElementTimeOut)
		{
			WaitForElementNotPresent(_driver, locator, timeOutSeconds);
		}

		public static void WaitTillStopMoving (IWebDriver driver, IWebElement element, Point startPos, int timeOutSeconds = FindElementTimeOut)
		{
			timeOutSeconds *= Settings.Default.WaitFactor;

			DateTime startTime = DateTime.UtcNow;

			while (element.Location == startPos)
			{
				if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(timeOutSeconds))
					throw new TimeoutException("Timout while waitng for element to START moving.");

				//Trace.WriteLine("Waiting for move to start.");

				Thread.Sleep(100);
			}

			Point lastPos = startPos;
			do
			{
				if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(timeOutSeconds))
					throw new TimeoutException("Timout while waitng for element to STOP moving.");

				Point curPos = element.Location;
				//Trace.WriteLine("curPos = " + curPos);

				if (curPos == lastPos)
				{
					break;
				}

				lastPos = curPos;
				Thread.Sleep(100);
			}
			while (true);
		}

		public void WaitTillStopMoving(IWebElement element, Point startPos, int timeOutSeconds = FindElementTimeOut)
		{
			WaitTillStopMoving(_driver, element, startPos, timeOutSeconds);
		}

		public void WaitTillPageIsLoaded(int timeOutSeconds)
		{
			timeOutSeconds *= Settings.Default.WaitFactor;

			// Cycle for 5 minutes or until
			// FindElement stops throwing exceptions
			Trace.WriteLine(TraceLevelLow + "Max waiting time for page to load: " + timeOutSeconds + " sec");

			DateTime startTime = DateTime.UtcNow;
			do
			{
				try
				{
					//Trace.Write("Looking for //body... ");
					_driver.FindElement(By.XPath("//body"));
					break;
				}
				catch (Exception) { }

				Thread.Sleep(TimeSpan.FromSeconds(0.3));
			}
			while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(timeOutSeconds));
		}

		#endregion

		public IWebElement FindElement(By locator)
		{
			Trace.WriteLine(TraceLevelLow + "Looking for element: '" + locator + "'");
			return _driver.FindElement(locator);
		}

		public void Type(By locator, string value)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Type '" + value + "' in input field: '" + locator + "'");
			FindElement(locator).SendKeys(value);
		}

		public void WaitAndType(By locator, int timeOutSeconds, string value)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Wait and Type '" + value + "' in input field: '" + locator + "'");
			WaitForElement(locator, timeOutSeconds).SendKeys(value);
		}

		public void WaitAndType(By locator, string value)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Wait and Type '" + value + "' in input field: '" + locator + "'");
			WaitForElement(locator).SendKeys(value);
		}

		public void WaitAndSwitchToFrame(int timeOutSeconds)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Wait for frame is present and switch to frame");
			_driver.SwitchTo().Frame(WaitForElement(By.Id("iPopUp"), timeOutSeconds));
		}

		public void WaitAndSwitchToWindow(int timeOutSeconds)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Wait for frame is NOT present and switch to window");
			WaitForElementNotPresent(By.Id("iPopUp"), timeOutSeconds);
			_driver.SwitchTo().DefaultContent();

			WaitForElement(By.Id(CopyrightNotice), timeOutSeconds).WaitTillVisible();
		}

		public string CurrentFrameTitle()
		{
			string _title = WaitForElement(By.XPath("//div/span[contains(@id, '-dialog-title-iPopUp')]"), 20).Text;

			Trace.WriteLine(BasePage.TraceLevelElement + "The current frame title is: '" + _title + "'");

			return _title;
		}

		public void Clear(By locator)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Clear input field: '" + locator + "'");
			FindElement(locator).Clear();
		}

		public void Click(By locator)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Click on: '" + locator + "'");
			FindElement(locator).Click();
		}

		public string CurrentWindowTitle()
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "The current window title is: '" + _driver.Title + "'");
			return _driver.Title;
		}

		public ReadOnlyCollection<IWebElement> FindElements(By locator)
		{
			Trace.WriteLine(TraceLevelLow + "Looking for elements: '" + locator + "'");
			return _driver.FindElements(locator);
		}
	}

	public static class WebElementExtensions
	{
		private static IWebElement WaitTill(this IWebElement element, int timeOutSeconds, Func<IWebElement, bool> condition, string desc)
		{
			timeOutSeconds *= Settings.Default.WaitFactor;

			Trace.Write(BasePage.TraceLevelLow + desc);
			Trace.WriteLine("Max waiting time: " + timeOutSeconds + " sec");

			var wait = new DefaultWait<IWebElement>(element);
			wait.Timeout = TimeSpan.FromSeconds(timeOutSeconds);

			wait.Until(condition);

			return element;
		}

		public static IWebElement WaitTillVisible(this IWebElement element, int timeOutSeconds = BasePage.FindElementTimeOut)
		{
			return WaitTill(element, timeOutSeconds, e => e.Displayed, "Wait for element is visible.");
		}

		public static IWebElement WaitTillNotVisible(this IWebElement element, int timeOutSeconds = BasePage.FindElementTimeOut)
		{
			return WaitTill(element, timeOutSeconds, e => !e.Displayed, "Wait for element is NOT visible.");
		}

		public static IWebElement WaitTillEnabled(this IWebElement element, int timeOutSeconds = BasePage.FindElementTimeOut)
		{
			return WaitTill(element, timeOutSeconds, e => e.Enabled, "Wait for current element is enabled.");
		}

		public static IWebElement WaitTillDisabled(this IWebElement element, int timeOutSeconds = BasePage.FindElementTimeOut)
		{
			return WaitTill(element, timeOutSeconds, e => !e.Enabled, "Wait for current element is disabled.");
		}
	}
}
