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
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class ExtensionsSteps : WatiNTest
    {
        public ExtensionsPage ExtensionsPage
        {
            get
            {
                return GetPage<ExtensionsPage>();
            }
        }
        
        /// <summary>
        /// Clicks the Install Extension Wizard link on the Extensions page.
        /// </summary>
        [When(@"I press Install Extension Wizard")]
        public void WhenIPressInstallExtensionWizard()
        {
            Thread.Sleep(1000);
            ExtensionsPage.ContentPaneDiv.Link(Find.ByText(s => s.Contains("Install Extension Wizard"))).Click();
        }

        /// <summary>
        /// Selects the language pack specified in the file upload on the Upload New Extension page.
        /// </summary>
        /// <param name="languagePack">The language pack that will be installed. This must match the name of the zip file exactly.
        /// The language pack must be in the Tests\Packages folder.
        /// </param>
        [When(@"I upload the language pack (.*)")]
        public void WhenIUploadTheLanguagePack(string languagePack)
        {
            string solutionPath = Directory.GetCurrentDirectory();
            if(!languagePack.EndsWith(".zip"))
            {
                languagePack = languagePack + ".zip";
            }
            string languagePath = solutionPath.Replace("\\Fixtures", "\\Community\\Tests\\Packages\\" + languagePack);
            ExtensionsPage.UploadExtensionsPackage(languagePath);
        }

        /// <summary>
        /// Completes the install extension wizard after selecting an extensions package.
        /// The step will click through each page in the wizard, and accept the extension license.
        /// </summary>
        [When(@"I complete the install extension wizard")]
        public void WhenICompleteTheInstallExtensionWizard()
        {
            Thread.Sleep(1500);
            //Upload Package page
            ExtensionsPage.WizardStartLink.ClickNoWait();
            //if package already installed
            IEInstance.WaitForComplete();
            if (ExtensionsPage.RepaireInstallCheckBox.Exists)
            {
                ExtensionsPage.RepaireInstallCheckBox.Checked = true;
                ExtensionsPage.WizardNextLink.ClickNoWait();
            }
            //Package Info page
            IEInstance.WaitForComplete();
            ExtensionsPage.WizardNextLink.ClickNoWait();
            //Release Notes Page
            Thread.Sleep(1500);
            IEInstance.WaitForComplete();
            ExtensionsPage.WizardNextLink.ClickNoWait();
            //License Page
            Thread.Sleep(1500);
            IEInstance.WaitForComplete();
            ExtensionsPage.AcceptLicenseCheckBox.ClickNoWait();
            ExtensionsPage.WizardNextLink.ClickNoWait();
            //Installation Report Page
            Thread.Sleep(1500);
            IEInstance.WaitForComplete();
            ExtensionsPage.WizardFinishLink.Click();
            Thread.Sleep(2500);
        }
    }
}
