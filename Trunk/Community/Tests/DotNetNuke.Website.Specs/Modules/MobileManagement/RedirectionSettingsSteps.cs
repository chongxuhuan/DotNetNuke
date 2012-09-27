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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Website.Specs.Modules.MobileManagement
{
    [Binding]
    public class RedirectionSettingsSteps : WatiNTest
    {
        #region Properties

        public RedirectionSettingsPage RedirectionSettingsPage
        {
            get
            {
                return GetPage<RedirectionSettingsPage>();
            }
        }

        #endregion

        #region Scenario "User Profile page should not appear in target drop down list"

        [When(@"I click Create button")]
        public void WhenIClickButton()
        {
            RedirectionSettingsPage.CreateButton.Click();
        }

        [Then(@"User Profile page should not appear in target drop down list")]
        public void ThenUserProfilePageShouldNotAppearInTargetDropDownList()
        {
            var dropDown = RedirectionSettingsPage.TargetPageSelectList;
            WatiNAssert.AssertIsTrue(!dropDown.Option(Find.ByText("Activity Feed")).Exists, "UserProfileExistsIndropDown.png");
        }


        #endregion
    }
}
