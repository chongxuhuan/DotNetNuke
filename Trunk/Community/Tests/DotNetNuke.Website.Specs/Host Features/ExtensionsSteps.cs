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

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.Host_Features
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
        [Given(@"I click Install Extension Wizard from action menu")]
        public void GivenIClickInstallExtensionWizardFromActionMenu()
        {
            ExtensionsPage.InstallExtensionsWizardLink.Click();
        }

        [Given(@"I select the invalid package")]
        public void GivenISelectTheInvalidPackage()
        {
            ExtensionsPage.SelectPackageFileUpload.Set(Path.Combine(Environment.CurrentDirectory, "Resources\\InvalidPackage.zip"));
        }

        [Then(@"I Should not see next button")]
        public void ThenIShouldNotSeeNextButton()
        {
            Assert.IsFalse(ExtensionsPage.WizardNextLink.Exists);
        }

        [Given(@"I click edit icon for module (.*)")]
        public void GivenIClickEditIconForModule(string moduleName)
        {
            var table = ExtensionsPage.InstalledExtensionDiv.Table(Find.ByClass("dnnGrid"));
            ExtensionsPage.GetExtensionEditButton(moduleName, table).Click();
        }

        [Given(@"I click (.*) Button")]
        public void GivenIClickButton(string type)
        {
            switch (type)
            {
                case "Next":
                    ExtensionsPage.WizardNextLink.Click();
                    break;
                case "Return":
                    ExtensionsPage.WizardReturnLink.Click();
                    break;
                case "Create Package":
                    ExtensionsPage.CreatePackageLink.Click();
                    break;
            }

            ExtensionsPage.ContentPaneDiv.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }


        [Then(@"Icon file of module (.*) should be available")]
        public void ThenIconFileOfModuleJournalShouldBeAvailable(string moduleName)
        {
            var table = ExtensionsPage.InstalledExtensionDiv.Table(Find.ByClass("dnnGrid"));
            Assert.IsTrue(ExtensionsPage.GetExtensionIcon(moduleName, table).Src.Contains("DesktopModules/Journal/"));
        }


	}
}
