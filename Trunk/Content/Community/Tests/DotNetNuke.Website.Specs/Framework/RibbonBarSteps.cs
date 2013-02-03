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
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.Framework
{
    [Binding]
    public class RibbonBarSteps : WatiNTest
    {
        #region Properties

        public ExtensionsPage ExtensionsPage
        {
            get
            {
                return GetPage<ExtensionsPage>();
            }
        }

        public RibbonBar RibbonBar
        {
            get
            {
                return GetPage<RibbonBar>();
            }
        }

        #endregion

        #region Scenario "Modules is shown in correct catagory"

        [Given(@"I update module (.*) to catagory (.*)")]
        public void GivenIUpdateModuleToCatagory(string moduleName, string catagory)
        {
            var table = ExtensionsPage.InstalledExtensionDiv.Table(Find.ByClass("dnnGrid"));
            var editButton = ExtensionsPage.GetExtensionEditButton(moduleName, table);
            editButton.Click();
            ExtensionsPage.ModuleCatagorySelect.SelectByValue(catagory);
            ExtensionsPage.UpdateExtensionLink.Click();

            IEInstance.Refresh();
        }
        [Then(@"module (.*) should display after change module catagory to (.*)")]
        public void ThenModuleShouldDisplayAfterChangeModuleCatagory(string moduleName, string catagory)
        {
            RibbonBar.ModuleCategorySelectList.SelectByValue(catagory);
            RibbonBar.ModuleCategorySelectList.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            Assert.IsTrue(RibbonBar.ModuleSelectDiv.ListItem(Find.ByText(t => t.Contains(moduleName))).Exists);
        }

        #endregion

        #region Scenario "Should not create page which name conflict with site alias"

        [When(@"I try to create page named Child")]
        public void WhenITryToCreatePageNamedChild()
        {
            Thread.Sleep(10000);
            RibbonBar.NewPageNameField.Value = "Child";
            RibbonBar.AddPageLink.Click();
        }

        [Then(@"The page should not create")]
        public void ThenThePageShouldNotCreate()
        {
            Assert.IsTrue(IEInstance.Span(Find.ByClass(c => c.Contains("ui-dialog-title"))).Exists);
        }

        #endregion
    }
}
