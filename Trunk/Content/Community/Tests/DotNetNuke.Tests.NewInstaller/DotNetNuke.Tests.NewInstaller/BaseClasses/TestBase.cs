using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace DotNetNuke.Tests.NewInstaller.BaseClasses
{
	/*	internal enum BrowserType
		{
			Ie,
			FireFox,
			Chrome,
		};
	*/
	public class TestBase
	{
		private IWebDriver _driver;
		//private const int LoadPageTimeOut = 30;

		internal IWebDriver StartBrowser(string browserType)
		{
			Trace.WriteLine("Start browser: '" + browserType + "'");

			if (_driver != null)
			{
				_driver.Quit();
			}

			switch (browserType)
			{
				case "ie":
					{
						 _driver = new InternetExplorerDriver("Drivers");
						break;
					}
					case "firefox":
					{
						FirefoxProfile firefoxProfile = new FirefoxProfile();
						firefoxProfile.AcceptUntrustedCertificates = true;

						_driver = new FirefoxDriver(firefoxProfile);
						break;
					}
					case "chrome":
					{
						_driver = new ChromeDriver("Drivers");
						break;
					}
			}

			_driver.Manage().Window.Maximize();
			

			return _driver;
		}

		private const string LogFile = "DnnTest.log";
		private const string ExFile = "DnnSoftAssert.log";

		protected void TryTest (Action<XElement> test, XElement settings)
		{
			try
			{
				test(settings);
			}
			catch (Exception e)
			{
				Trace.WriteLine("EXCEPTION =>" + e.Message + e.StackTrace);
				throw;
			}
		}

		[TestFixtureSetUp]
		public void SetUp()
		{
			File.Delete(LogFile);
			File.Delete(ExFile);

			Trace.Listeners.Add(new TextWriterTraceListener(LogFile));

			Trace.AutoFlush = true;
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			Trace.WriteLine("Stop browser");

			if (_driver != null)
			{
				_driver.Quit();
				_driver = null;
			}

			if (Utilities.ExceptionList.Count > 0)
			{
				Trace.Listeners.Add(new TextWriterTraceListener(ExFile));

				Trace.WriteLine("");
				Trace.Write("========> EXCEPTION SUMMARY:");
				Trace.WriteLine("");

				foreach (Exception e in Utilities.ExceptionList)
				{
					Trace.WriteLine("Assert Failed =>" + e.Message + e.StackTrace);
					Trace.WriteLine("");
				}
			}

			Trace.Flush();
		}
	}
}
