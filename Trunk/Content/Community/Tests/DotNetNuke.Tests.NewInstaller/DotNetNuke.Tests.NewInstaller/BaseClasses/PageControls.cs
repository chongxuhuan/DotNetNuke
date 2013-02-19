using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.NewInstaller.BaseClasses
{
	public class NonStandardSelect
	{
		private readonly By _arrowId, _listDropDownId;
		private readonly IWebDriver _driver;

		private NonStandardSelect(){}

		public NonStandardSelect(IWebDriver driver, By arrowId, By listDropDownId)
		{
			_driver = driver;

			_arrowId = arrowId;
			_listDropDownId = listDropDownId;
		}

		public void SelectByValue (string value)
		{
			IWebElement listDropDown = _driver.FindElement(_listDropDownId);

			IWebElement firstElement = _driver.FindElement(By.XPath("//*[@id='" + listDropDown.GetAttribute("id") + "']//li[1]"));
			IWebElement lastElement = _driver.FindElement(By.XPath("//*[@id='" + listDropDown.GetAttribute("id") + "']//li[last()]"));

			Point fistElementStart = firstElement.Location;
			Point lastElementStart = lastElement.Location;
			//_driver.FindElement(_arrowId).FindElement(By.XPath("./parent::*")).Click();
			_driver.FindElement(_arrowId).Click();

			BasePage.WaitTillStopMoving(_driver, firstElement, fistElementStart);
			BasePage.WaitTillStopMoving(_driver, lastElement, lastElementStart);

			fistElementStart = firstElement.Location;
			lastElementStart = lastElement.Location;
			_driver.FindElement(By.XPath("//*[@id='" + listDropDown.GetAttribute("id") + "']//li[contains(text(), '" + value + "')]")).Click();

			BasePage.WaitTillStopMoving(_driver, firstElement, fistElementStart);
			BasePage.WaitTillStopMoving(_driver, lastElement, lastElementStart);
		}
			
		public int CountAllOptions ()
		{
			IWebElement listDropDown = _driver.FindElement(_listDropDownId);
			int xpathCount = _driver.FindElements(By.XPath("//*[@id='" + listDropDown.GetAttribute("id") + "']//li")).Count();
			return xpathCount;
		}

		public static void SelectByValue (IWebDriver driver, By arrowId, By listDropDownId, string value)
		{
			new NonStandardSelect(driver, arrowId, listDropDownId).SelectByValue(value);
			Trace.WriteLine(BasePage.TraceLevelElement + "Select '" + value + "' in the list '" + listDropDownId + "'");
		}

		public static int CountAllOptions (IWebDriver driver, By arrowId, By listDropDownId)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Count options in the list '" + listDropDownId + "'");
			return new NonStandardSelect(driver, arrowId, listDropDownId).CountAllOptions();
		}
	}



	public class RadioButton
	{
		private readonly By _buttonId;
		private readonly IWebDriver _driver;

		private RadioButton() {}

		public RadioButton (IWebDriver driver, By buttonId)
		{
			_driver = driver;

			_buttonId = buttonId;
		}	

		public void Select ()
		{
			_driver.FindElement(_buttonId).FindElement(By.XPath("./following-sibling::*")).Click();
		}

		public static void Select(IWebDriver driver, By locator)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Select radio button '" + locator + "'");
			new RadioButton(driver, locator).Select();
		}
	}



	public class CheckBox
	{
		private readonly By _checkBoxId;
		private readonly IWebDriver _driver;

		private CheckBox() {}

		public CheckBox(IWebDriver driver, By checkBoxId)
		{
			_driver = driver;

			_checkBoxId = checkBoxId;
		}

		public void Check()
		{
			_driver.FindElement(_checkBoxId).FindElement(By.XPath("./following-sibling::*")).Click();
		}

		public static void Check (IWebDriver driver, By locator)
		{
			Trace.WriteLine(BasePage.TraceLevelElement + "Select check box '" + locator + "'");
			new CheckBox(driver, locator).Check();
		}
	}


}