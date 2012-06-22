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
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using TechTalk.SpecFlow;
using WatiN.Core;
using NUnit.Framework;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class LanguageSteps :WatiNTest
    {
        public LanguagePage LanguagePage
        {
            get
            {
                return GetPage<LanguagePage>();
            }
        }
        
        /// <summary>
        /// Clicks the Add New Language button on the Admin Languages page.
        /// Make sure you have navigated to this page in a previous step before using this.
        /// </summary>
        [When(@"I press Add New Language")]
        public void WhenIPressAddNewLanguage()
        {
            Thread.Sleep(1500);
            LanguagePage.AddNewLanguageLink.Click();
        }

        /// <summary>
        /// Selects the language specified when the language drop down is present. 
        /// </summary>
        /// <param name="languageName">The language that will be selected.
        /// Must match the text in the drop down exactly.</param>
        [When(@"I select the language (.*)")]
        public void WhenISelectTheLanguage(string languageName)
        {
            LanguagePage.SelectLanguage("language", "rcbItem ", languageName);
        }

        /// <summary>
        /// Selects the language specified when the fallback language drop down is present. 
        /// </summary>
        /// <param name="languageName">The language that will be selected as the fallback language.
        /// Must match the text in the drop down exactly.</param>
        [When(@"I select the fallback language (.*)")]
        public void WhenISelectTheFallbackLanguage(string languageName)
        {
            LanguagePage.SelectLanguage("fallback", "rcbItem ", languageName);
        }

        /// <summary>
        /// Checks that the corresponding language flag is displayed.
        /// </summary>
        /// <param name="languageName">The name of the language.</param>
        [Then(@"The language (.*) is added to the language page")]
        public void ThenTheLanguageIsAddedToTheLanguagePage(string languageName)
        {
            Span flagSpan = LanguagePage.GetLanguageFlagSpanByLocaleName(languageName);
            Thread.Sleep(5000);
            IEInstance.Refresh();
            Thread.Sleep(1500);
            WatiNAssert.AssertIsTrue(flagSpan.Exists, "FlagSpanMissing.jpg");
            WatiNAssert.AssertIsTrue(flagSpan.ClassName.Equals("Language"), "FlagSpanError.jpg");
        }

        /// <summary>
        /// Checks that the corresponding language flag is displayed.
        /// For certain languages, it will also check that after clicking on the flag some text is correctly translated.
        /// Currently translation is only checked when using French. 
        /// </summary>
        /// <param name="languageName">The name of the language.</param>
        [Then(@"The language (.*) is added to the site")]
        public void ThenTheLanguageFrancaisCanadaIsAddedToTheSite(string languageName)
        {
            Span flagSpan = LanguagePage.GetLanguageFlagSpanByLocaleName(languageName);
            Thread.Sleep(5000);
            IEInstance.Refresh();
            WatiNAssert.AssertIsTrue(flagSpan.Exists, "FlagSpanMissing2.jpg");
            WatiNAssert.AssertIsTrue(flagSpan.ClassName.Equals("Language"), "FlagSpanError2.jpg");
            IEInstance.GoTo(SiteUrl + "/logoff.aspx");
            Thread.Sleep(1500);
            flagSpan.Link(Find.Any).ClickNoWait();
            Thread.Sleep(1500);
            if(languageName.Contains("français"))
            {
                WatiNAssert.AssertIsTrue(HomePage.LoginLink.Text.ToLower().Contains("connexion"), "LoginLinkTextIncorrect.jpg");
                WatiNAssert.AssertIsTrue(HomePage.RegisterLink.Text.ToLower().Contains("inscription"), "RegisterLinkTextIncorrect.jpg");
            }
        }
    }
}
