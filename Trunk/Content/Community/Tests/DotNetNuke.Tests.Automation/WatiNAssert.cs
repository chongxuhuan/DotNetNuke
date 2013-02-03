#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using NUnit.Framework;
using System;
using WatiN.Core;
using System.IO;
using System.Configuration;

namespace DotNetNuke.Tests.UI.WatiN.Common
{
    public class WatiNAssert
    {
        private IE _ieInstance;
        private string _screenShotPartialPath;
        private string _screenShotPath = ConfigurationManager.AppSettings["ScreenShotPath"];

        public WatiNAssert(IE ieInstance, string screenShotPartialPath)
        {
            IeInstance = ieInstance;
            ScreenShotPartialPath = screenShotPartialPath;
            if(!Directory.Exists(ScreenShotPartialPath))
            {
                Directory.CreateDirectory(ScreenShotPartialPath);
            }
        }

        public IE IeInstance
        {
            get { return _ieInstance; }
            set { _ieInstance = value;}
        }

        public string ScreenShotPartialPath
        {
            get { return _screenShotPartialPath; }
            set { _screenShotPartialPath = value; }
        }

        private void AppendToHtml(string screenShotFileName, string partialPath)
        {
            using (StreamWriter x = File.AppendText(_screenShotPath + "\\index.html"))
            {
                x.Write("<h2>" + screenShotFileName + "</h2>" + 
                    Environment.NewLine + "<img src=\"" + partialPath.Split('\\')[partialPath.Split('\\').Length - 1] + "\\" + screenShotFileName + "\" width=\"800\" height=\"600\" />" + 
                    Environment.NewLine + "<br/>" +
                    Environment.NewLine);
            }
        }

        /// <summary>
        /// Creates a screen shot with the given file name if check evaluates to false.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        public void AssertIsTrue(bool check, string screenShotFileName)
        {
            if (!check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                System.Threading.Thread.Sleep(1500);
                IeInstance.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsTrue(check);
        }

        /// <summary>
        /// Creates a screen shot of the window given with the file name if check evaluates to false.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        /// <param name="screenShotWindow">The window to take a screen shot of.</param>
        public void AssertIsTrue(bool check, string screenShotFileName, IE screenShotWindow)
        {
            if (!check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                screenShotWindow.CaptureWebPageToFile(ScreenShotPartialPath + screenShotFileName);
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsTrue(check);
        }

        /// <summary>
        /// Creates a screen shot with the file name if check evaluates to false.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        /// <param name="message">The error message that will be displayed in the test report.</param>
        public void AssertIsTrue(bool check, string screenShotFileName, string message)
        {
            if (!check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                IeInstance.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsTrue(check, message);
        }

        /// <summary>
        /// Creates a screen shot with the given file name if check evaluates to true.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        public void AssertIsFalse(bool check, string screenShotFileName)
        {
            if (check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                IeInstance.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsFalse(check);
        }

        /// <summary>
        /// Creates a screen shot of the window given with the file name if check evaluates to true.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        /// <param name="screenShotWindow">The window to take a screen shot of.</param>
        public void AssertIsFalse(bool check, string screenShotFileName, IE screenShotWindow)
        {
            if (check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                screenShotWindow.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsFalse(check);
        }

        /// <summary>
        /// Creates a screen shot with the file name if check evaluates to true.
        /// </summary>
        /// <param name="check">The condition to assert.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        /// <param name="message">The error message that will be displayed in the test report.</param>
        public void AssertIsFalse(bool check, string screenShotFileName, string message)
        {
            if (check)
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                IeInstance.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.IsFalse(check, message);
        }

        /// <summary>
        /// Creates a screen shot with the file name if the two strings are not equal.
        /// </summary>
        /// <param name="expected">The expected string.</param>
        /// <param name="actual">The actual string.</param>
        /// <param name="screenShotFileName">The file name for the screen shot.</param>
        public void AssertStringsAreEqual(string expected, string actual, string screenShotFileName)
        {
            if (!expected.Equals(actual))
            {
                if (File.Exists(Path.Combine(ScreenShotPartialPath, screenShotFileName)))
                {
                    File.Delete(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                }
                IeInstance.CaptureWebPageToFile(Path.Combine(ScreenShotPartialPath, screenShotFileName));
                AppendToHtml(screenShotFileName, ScreenShotPartialPath);
            }
            Assert.AreEqual(expected, actual);
        }

        //public static void ContentContains<TElement>(string expectedSubString, TElement element) where TElement : global::WatiN.Core.Element
        //{
        //    string message = String.Format("Element - {0} did not contain expected substring - {1}.  Actual Value of element's InnerHtml - {2}",
        //                                    element.Id, expectedSubString, element.InnerHtml);
        //    ContentContains<TElement>(expectedSubString, element,message);
        //}

        //public static void ContentContains<TElement>(string expectedSubString, TElement element, string message) where TElement : global::WatiN.Core.Element
        //{
        //    Assert.Contains(element.InnerHtml, expectedSubString, message);
        //}

        //public static void ContentEquals<TElement>(string expectedValue, TElement element) where TElement : global::WatiN.Core.Element
        //{
        //    string message = String.Format("Element - {0} did not equal the string - {1}.  Actual Value of element's InnerHtml - {2}",
        //                                    element.Id, expectedValue, element.InnerHtml);
        //    ContentEquals<TElement>(expectedValue, element , message);
        //}

        //public static void ContentEquals<TElement>(string expectedValue, TElement element, string message) where TElement : global::WatiN.Core.Element
        //{
        //    Assert.Contains(element.InnerHtml, expectedValue, message);
        //}

        public static void ElementExists<TElement>(TElement element) where TElement : global::WatiN.Core.Element
        {
            string message = String.Format("Could not find the element that was being searched for.");
            ElementExists<TElement>(element, message);
        }

        public static void ElementExists<TElement>(TElement element, string message) where TElement : global::WatiN.Core.Element
        {
            Assert.IsTrue(element.Exists, message);
        }

        public static void ElementDoesNotExists<TElement>(TElement element) where TElement : global::WatiN.Core.Element
        {
            string message = String.Format("The following element existed when it shouldn't have:{0}", element.Id);
            ElementExists<TElement>(element, message);
        }

        public static void ElementDoesNotExists<TElement>(TElement element, string message) where TElement : global::WatiN.Core.Element
        {
            Assert.IsFalse(element.Exists, message);
        }

    }
}
