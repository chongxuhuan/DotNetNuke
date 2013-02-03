using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Website.DesktopModules.Sketch
{
    public class UI
    {
        public static IWebElement TitleTextbox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "(//input[@type='text'])[position()=2]"); }
        public static IWebElement DescriptionTextbox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//textarea"); }
        public static IWebElement AddSketchMenuItem(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_ctr520_Sketches_treeSketches_MainContextMenu_detached']/ul/li/a/span"); }
    }
}
