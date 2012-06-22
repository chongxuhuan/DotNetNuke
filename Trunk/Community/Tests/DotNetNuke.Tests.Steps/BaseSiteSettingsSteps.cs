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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        private SiteSettingsPage _siteSettingsPage;

        [BeforeScenario("MustHaveSandboxedPaymentSettings")]
        public void MustHaveSandboxedPaymentSettings()
        {
            var portalController = new PortalController();
            var site = portalController.GetPortal(0);
            if (site.ProcessorPassword == string.Empty)
            {
                PortalController.UpdatePortalSetting(0, "paypalsandbox", "true");
                //uses PayPal sandbox account.
                site.ProcessorUserId = "PayPal";
                site.ProcessorPassword = "320122999";
                site.ProcessorUserId = "philip_1320123085_biz@dnncorp.com";
                portalController.UpdatePortalInfo(site);
            }
        }

        /// <summary>
        /// Clicks the advanced settings tab on the site settings page.
        /// </summary>
        [When(@"I click Advanced Settings Tab")]
        public void WhenIClickAdvancedSettingsTab()
        {
            _siteSettingsPage = GetPage<SiteSettingsPage>();
            Thread.Sleep(1000);
            _siteSettingsPage.AdvancedSettingsTab.Click();
        }

        /// <summary>
        /// Clicks the payment settings section link on the site settings page. 
        /// </summary>
        [When(@"I click Payment Settings Section")]
        public void WhenIClickPaymentSettingsSectionLink()
        {
            _siteSettingsPage = GetPage<SiteSettingsPage>();
            Thread.Sleep(1000);
            _siteSettingsPage.PaymentSettingsSectionLink.Click();
        }

    }
}
