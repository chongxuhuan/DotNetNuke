using System;
using System.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Common.Lists;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.User_Accounts
{
    [Binding]
    public class ProfilePropertiesSteps : WatiNTest
    {
        private UserPage UserPage
        {
            get
            {
                return GetPage<UserPage>();
            }
        }

        private UserProfilePage UserProfilePage
        {
            get
            {
                return GetPage<UserProfilePage>();
            }
        }
        private ProfilePropertiesPage ProfilePropertiesPage
        {
            get
            {
                return GetPage<ProfilePropertiesPage>();
            }
        }

        [Then("A list of Profile Properties is displayed")]
        public void ThenAListOfProfilePropertiesIsDisplayed()
        {
            Assert.IsTrue(HomePage.ContentPaneDiv.Table(Find.ById(s => s.Contains("profileDefinitions_grdProfileProperties"))).Exists);
        }

        [When(@"I click Add New Profile Property")]
        [Given(@"I click Add New Profile Property")]
        public void WhenIClickAddNewProfileProperty()
        {
            ProfilePropertiesPage.AddNewProfilePropertyLink.Click();
        }

        [When(@"I fill in the profile property form")]
        [Given(@"I fill in the profile property form")]
        public void WhenIFillInTheProfilePropertyForm(TechTalk.SpecFlow.Table table)
        {
            ProfilePropertiesPage.PropertyNameField.Value = table.Rows[0]["Value"];

            ListEntryInfo dataType = new ListController().GetListEntryInfo("DataType", table.Rows[1]["Value"]);
            ProfilePropertiesPage.DataTypeSelectList.SelectByValue(dataType.EntryID.ToString(CultureInfo.InvariantCulture));

            ProfilePropertiesPage.PropertyCategoryField.Value = table.Rows[2]["Value"];
            ProfilePropertiesPage.LengthTextField.Value = table.Rows[3]["Value"];
            ProfilePropertiesPage.RequiredCheckBox.Checked = Boolean.Parse(table.Rows[4]["Value"]);
            ProfilePropertiesPage.VisibleCheckBox.Checked = Boolean.Parse(table.Rows[5]["Value"]);
            ProfilePropertiesPage.ReadOnlyCheckBox.Checked = Boolean.Parse(table.Rows[6]["Value"]);
        }

        [When(@"I click Next")]
        [Given(@"I click Next")]
        public void WhenIClickNext()
        {
            ProfilePropertiesPage.NextButton.Click();
        }

        [When(@"I click Return")]
        [Given(@"I click Return")]
        public void WhenIClickReturn()
        {
            ProfilePropertiesPage.ReturnToProfilePropertiesLink.Click();
        }

        [Then(@"Profile Property (.*) is created correctly")]
        public void ThenProfilePropertyIsCreatedCorrectly(string propertyName)
        {
            Thread.Sleep(1500);
            IEInstance.WaitForComplete();
            var propertyRow = ProfilePropertiesPage.GetPropertyRow(propertyName);
            WatiNAssert.AssertIsTrue(propertyRow != null, "Property_" + propertyName + "_CreatedError.jpg");
        }

        [When(@"I click Edit (.*) Link")]
        public void WhenIClickEditLink(string propertyName)
        {
            ProfilePropertiesPage.GetPropertyEditButton(propertyName).Click();
        }

        [Then(@"Edit Profile Page is displayed for (.*)")]
        public void ThenEditProfilePageIsDisplayed(string propertyName)
        {
            Div profilePropertyName = ProfilePropertiesPage.GetPropertyEditorRow("PropertyName");

            Assert.IsTrue(profilePropertyName.InnerHtml.Contains(propertyName));
        }

        [Then(@"Profile Property (.*) is visible")]
        public void ThenProfilePropertyIsVisible(string propertyName)
        {
            Thread.Sleep(1500);
            Assert.IsTrue(UserProfilePage.GetProfilePropertyDiv(propertyName).Exists);
        }

        [Then(@"Profile Property (.*) is not visible")]
        public void ThenProfilePropertyIsNotVisible(string propertyName)
        {
            Thread.Sleep(1500);
            Assert.IsFalse(UserProfilePage.GetProfilePropertyDiv(propertyName).Exists);
        }

        [Then(@"Profile Property (.*) is editable")]
        public void ThenProfilePropertyIsEditable(string propertyName)
        {
            Assert.IsTrue(UserProfilePage.GetProfilePropertyTextField(propertyName).Exists);
        }

        [Then(@"Profile Property (.*) is not editable")]
        public void ThenProfilePropertyIsNotEditable(string propertyName)
        {
            Assert.IsFalse(UserProfilePage.GetProfilePropertyTextField(propertyName).Exists);
        }

        [Then(@"the Edit Profile link should be visible")]
        public void EditProfileLinkVisible()
        {
            IEInstance.WaitForComplete();
            Assert.IsTrue(UserProfilePage.EditProfileLink.Exists);
        }
    }
}
