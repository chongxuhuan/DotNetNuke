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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using DotNetNuke.Tests.Steps;

using System.Text.RegularExpressions;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class ModuleSteps : WatiNTest
    {
        public LoginPage LoginPage
        {
            get
            {
                return GetPage<LoginPage>();
            }
        }


        /// <summary>
        /// Logs in as the admin (unless they are already logged in).
        /// Navigates to the root level page specified.
        /// Then checks that the module title and content specified exist on the page. 
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <param name="moduleName">The module title.</param>
        /// <param name="moduleContent">The content in the module.</param>
        [Then(@"As an admin on the page (.*) I can see the module (.*) and its content (.*)")]
        public void ThenAsAnAdminOnThePageICanSeeTheModuleAndItsContent(string pageName, string moduleName, string moduleContent)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl);
            Thread.Sleep(1500);
            LoginPage.LogoffUser();
            Thread.Sleep(1500);
            LoginPage.LoginUser(TestUsers.Admin.UserName, TestUsers.Admin.Password);
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/"+pageName+".aspx");
            WatiNAssert.AssertIsTrue(HomePage.HtmlModuleExistsOnPage(moduleName, moduleContent), "MissingModuleForAdmin.jpg");
        }

        /// <summary>
        /// Logs in as the user specified.
        /// Navigates to the root level page specified.
        /// Then checks that the module title and content specified exist on the page. 
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="moduleName">The module title.</param>
        /// <param name="moduleContent">The content in the module.</param>
        [Then(@"As (.*) with the password (.*) on the page (.*) I can see the module (.*) and its content (.*)")]
        public void ThenAsUserOnThePageICanSeeTheModuleAndItsContent(string userName, string password, string pageName, string moduleName, string moduleContent)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl);
            Thread.Sleep(1500);
            LoginPage.LogoffUser();
            Thread.Sleep(1500);
            LoginPage.LoginUser(userName, password);
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/" + pageName + ".aspx");
            Thread.Sleep(1500);
            WatiNAssert.AssertIsTrue(HomePage.HtmlModuleExistsOnPage(moduleName, moduleContent), "MissingModuleFor" + userName + ".jpg");
        }

        /// <summary>
        /// Browses to the page specified as an anonymous user.
        /// Then checks that the module title and content specified does not exist on the page. 
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="moduleName"></param>
        /// <param name="moduleContent"></param>
        [Then(@"As an anonymous user on the page (.*) I can not see the module (.*) and its content (.*)")]
        public void ThenAsAnAnonymousUserOnThePageICanNotSeeTheModuleAndItsContent(string pageName, string moduleName, string moduleContent)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl);
            Thread.Sleep(1500);
            LoginPage.LogoffUser();
            Thread.Sleep(1500);
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/" + pageName + ".aspx");
            WatiNAssert.AssertIsFalse(HomePage.HtmlModuleExistsOnPage(moduleName, moduleContent), "AnonUserCanSeeModule.jpg");
        }

        /// <summary>
        /// Browses to the page specified as an anonymous user.
        /// Then checks that the module name and content specified exists on the page. 
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="moduleName"></param>
        /// <param name="moduleContent"></param>
        [Then(@"As an anonymous user on the page (.*) I can see the module (.*) and its content (.*)")]
        public void ThenAsAnAnonymousUserOnThePageICanSeeTheModuleAndItsContent(string pageName, string moduleName, string moduleContent)
        {
            Thread.Sleep(1500);
            IEInstance.GoTo(SiteUrl);
            Thread.Sleep(1500);
            LoginPage.LogoffUser();
            Thread.Sleep(1500);
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/" + pageName + ".aspx");
            WatiNAssert.AssertIsTrue(HomePage.HtmlModuleExistsOnPage(moduleName, moduleContent), "AnonUserCannotSeeModule.jpg");
        }

    }
}
