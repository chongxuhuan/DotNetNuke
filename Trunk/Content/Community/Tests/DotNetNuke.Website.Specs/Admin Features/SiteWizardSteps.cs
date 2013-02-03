using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace DotNetNuke.Website.Specs.Admin_Features
{
    [Binding]
    public class SiteWizardSteps : WatiNTest
    {
        #region Properties

        public SiteWizardPage SiteWizardPage
        {
            get
            {
                return GetPage<SiteWizardPage>();
            }
        }

        public SiteSettingsPage SiteSettingsPage
        {
            get
            {
                return GetPage<SiteSettingsPage>();
            }
        }

        #endregion
        [When(@"I click the Site Wizard (.*) button")]
        public void WhenIClickButton(string type)
        {
            switch (type)
            {
                case "Next":
                    SiteWizardPage.NextButtonLink.Click();
                    break;
                case "Finish":
                    SiteWizardPage.FinishButtonLink.Click();
                    break;
            }
            SiteWizardPage.ContentPaneDiv.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [When(@"I select the skin or container (.*)")]
        public void WhenISelectSkinOrContainer(string value)
        {
            SiteWizardPage.SelectRadioButton(value);
        }

        [When(@"I set site name to (.*)")]
        public void WhenISetSiteNameToNewSite(string name)
        {
            SiteWizardPage.PortalNameField.Value = name;
        }

        [Then(@"Site's default skin should be (.*)")]
        public void ThenSiteSDefaultSkinShouldBe(string value)
        {
            Assert.IsTrue(SiteSettingsPage.SiteSkinSelectList.SelectedItem.Contains(value));
        }

        [Then(@"Site's default container should be (.*)")]
        public void ThenSiteSDefaultContainerShouldBe(string value)
        {
            Assert.IsTrue(SiteSettingsPage.SiteContainerSelectList.SelectedItem.Contains(value));
        }

        [Then(@"Site's name should be (.*)")]
        public void ThenSiteSNameShouldBeNewSite(string name)
        {
            Assert.IsTrue(SiteSettingsPage.SiteTitleField.Value.Equals(name));
        }


    }
}
