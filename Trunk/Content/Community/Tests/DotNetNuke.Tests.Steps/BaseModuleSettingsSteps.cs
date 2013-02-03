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
using System.Threading;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        public ModulePage ModuleSettingsPage
        {
            get { return GetPage<ModulePage>(); }
        }

        /// <summary>
        /// Sets the HelpURL field for the HTML Module to
        /// hive.dotnetnuke.com/Default.aspx?tabid=265
        /// </summary>
        [BeforeScenario("MustHaveHelpUrlFilledOutForHtmlModule")]
        public void MustHaveHelpUrlFilledOutForHtmlModule()
        {
            var moduleDefId = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Text/HTML").ModuleDefID;
            var control = ModuleControlController.GetModuleControlByControlKey("", moduleDefId);
            control.HelpURL = "http://hive.dotnetnuke.com/Default.aspx?tabid=283";
            ModuleControlController.UpdateModuleControl(control);
        }

        /// <summary>
        /// Sets the HelpURL field for the HTML Module to
        /// ""
        /// </summary>
        [BeforeScenario("MustHaveEmptyHelpUrlForHtmlModule")]
        public void MustHaveEmptyHelpUrlForHtmlModule()
        {
            var control = ModuleControlController.GetModuleControlByControlKey("", 111);
            control.HelpURL = "";
            ModuleControlController.UpdateModuleControl(control);
        }

        /// <summary>
        /// Clicks on the module settings link for the module in the postion specified. 
        /// Entering a moduleId of 0, will pick the first module on the page. 
        /// </summary>
        /// <param name="moduleId"></param>
        [When(@"I go to the Module (.*) Settings page")]
        public void WhenIGoToTheModuleSettingsPage(int moduleId)
        {
            ModuleSettingsPage.GetModuleSettingsImage(moduleId).Click();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Clicks on the module settings Permissions Tab for the module in the postion specified. 
        /// Entering a moduleId of 0, will pick the first module on the page. 
        /// </summary>
        /// <param name="moduleId"></param>
        [When(@"I go to the Module (.*) Permissions tab")]
        public void WhenIGoToTheModulePermissionsTab(int moduleId)
        {
            ModuleSettingsPage.GetModuleSettingsImage(moduleId).Click();
            ModuleSettingsPage.PermissionsLink.Click();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Clicks on the module settings link for the module in the postion specified. 
        /// Entering a moduleId of 0, will pick the first module on the page. 
        /// </summary>
        /// <param name="moduleId"></param>
        [When(@"I click the Module (.*) Online Help")]
        public void WhenIGoToTheModuleOnlineHelp(int moduleId)
        {
            ModuleSettingsPage.GetModuleOnlineHelpImage(moduleId).Click();
        }

        /// <summary>
        /// Unchecks the Inherit View Permissions checkbox
        /// </summary>
        [Given(@"I Uncheck Inherit View Permissions")]
        [When(@"I Uncheck Inherit View Permissions")]
        public void WhenIUncheckInheritViewPermissions()
        {
            if (!ModuleSettingsPage.InheritPagePermissionsCheckBox.Checked)
            {
                return;
            }
            ModuleSettingsPage.InheritPagePermissionsCheckBox.Click();
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Sets the permissions for the module
        /// </summary>
        [Given(@"The (.*) permission is set to (.*) for the role (.*)")]
        [When(@"The (.*) permission is set to (.*) for the role (.*)")]
        public void WhenISetPermissionForRole(string permission, string setting, string roleName)
        {
            ModuleSettingsPage.SetPermissionForRole(setting, permission, roleName);
            Thread.Sleep(2000);
        }
    }
}
