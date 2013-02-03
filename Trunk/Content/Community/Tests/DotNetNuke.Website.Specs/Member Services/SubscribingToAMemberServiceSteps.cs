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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.Steps
{
    [Binding]
    public class SubscribingToAMemberServiceSteps : WatiNTest
    {
        public UserProfilePage UserProfilePage
        {
            get
            {
                return GetPage<UserProfilePage>();
            }
        }
        [When(@"I click Manage Services")]
        public void WhenIClickManageServices()
        {
            UserProfilePage.ManageServicesTabLink.Click();
            UserProfilePage.ManageServicesTabLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

        }

        [When(@"I Subscribe to a service with a fee")]
        public void WhenISubscribeToAServiceWithAFee()
        {
            foreach (var link in UserProfilePage.ServicesTable.Links)
            {
                if (link.Text == "Subscribe")
                {
                    link.Click();
                    break;
                }
            }
            IEInstance.WaitForComplete();
        }

        [Then(@"I should be on the PayPal site with all the fields filled out")]
        public void ThenIShouldBeOnThePayPalSiteWithAllTheFieldsFilledOut()
        {
            var payPalPage = IE.AttachTo<IE>(Find.ByUrl(new Regex(".*sandbox.paypal.com.*", RegexOptions.IgnoreCase)));
            Assert.IsNotNull(payPalPage);
        }

    }
}
