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
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using DotNetNuke.Tests.Instance.Utilities;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.Exceptions;

namespace DotNetNuke.Tests.Steps
{
    [Binding]
    public partial class BaseSteps : WatiNTest
    {
        public BaseSteps()
            : base(0)
        {
        }

        protected void ExecuteSQL(string content)
        {
            var connectionstring = string.Empty;
            var colConnections = ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings objConnection in colConnections)
            {
                if (objConnection.Name.ToLower() != "localmysqlserver" && objConnection.Name.ToLower() != "localsqlserver")
                {
                    connectionstring = objConnection.Name;
                }
            }
#pragma warning disable 612,618
            string exceptions = DataProvider.Instance().ExecuteScript(Config.GetConnectionString(connectionstring), content);
#pragma warning restore 612,618
            if (!String.IsNullOrEmpty(exceptions))
            {
                //An error occured executing one of more of the scripts. Output the error. 
                Assert.IsTrue(false, "The following exception(s) occured executing the script: " + exceptions);
            }
            
        }

        protected void ExecuteScript(string script)
        {
            var content = File.ReadAllText(script); 

            ExecuteSQL(content);
        }

        /// <summary>
        /// This after scenario method will be run after every test.
        /// It finds the ie instance that was opened for the test, ensures any users are logged off, and closes the window. 
        /// </summary>
        [AfterScenario]
        public void CloseWindowAfterScenario()
        {
            var closeWindow = Convert.ToBoolean(ConfigurationManager.AppSettings["CloseWindow"]);
            if (closeWindow)
            {
                if (IEInstance != null)
                {
                    IEInstance.GoTo(SiteUrl + "/logoff.aspx");
                    IEInstance.Close();
                }
            }
        }

        /// <summary>
        /// Clears the dotnetnuke cache by clicking on the clear cache link in the ribbon bar. 
        /// The host must be logged in to perform this step.
        /// Use this step if a page has been added behind the scenes (using the @MustHaveTestPageAdded tag). 
        /// </summary>
        [Given(@"I have cleared the dotnetnuke cache")]
        [When(@"I have cleared the dotnetnuke cache")]
        public void GivenIHaveClearedTheDotnetnukeCache()
        {
            _ribbonBar = GetPage<RibbonBar>();
            _ribbonBar.ClearCacheLink.Click();
            Thread.Sleep(3500);
        }

        /// <summary>
        /// Clicks the update button/link on almost any form on the site. 
        /// </summary>
        [Given(@"I click Update")]
        [When(@"I click Update")]
        public void WhenIClickUpdate()
        {
            HomePage.UpdateLink.Click();
            HomePage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        /// <summary>
        /// Browse to the url specified. 
        /// The test will add "http://" to any url entered.
        /// </summary>
        /// <param name="url"></param>
        [When(@"I browse to (.*)")]
        public void WhenIBrowseTo(string url)
        {
            Thread.Sleep(2500);
            IEInstance.GoTo("http://" + url);
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Checks that the site logo exists on the current page. 
        /// </summary>
        [Then(@"I should see my site")]
        public void ThenIShouldSeeMySite()
        {
            Thread.Sleep(2500);
            WatiNAssert.AssertIsTrue(HomePage.Title.Text.Contains("My Website"), "CantFindSite.jpg");
        }
    }
}
