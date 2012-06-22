using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Entities.Users;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

using TableRow = WatiN.Core.TableRow;

namespace DotNetNuke.Website.Specs.Steps
{
    [Binding]
    public class UserAccountsSteps : WatiNTest
    {
        public UserPage UserPage
        {
            get
            {
                return GetPage<UserPage>();
            }
        }

        public UserSettingsPage UserSettingsPage
        {
            get
            {
                return GetPage<UserSettingsPage>();
            }
        }

        public UserProfilePage UserProfilePage
        {
            get
            {
                return GetPage<UserProfilePage>();
            }
        }

        public SiteSettingsPage SiteSettingsPage
        {
            get
            {
                return GetPage<SiteSettingsPage>();
            }
        }

        [Then(@"The user (.*) should have a link to edit the User")]
        public void ThenTheUserShouldHaveALinkToEditTheUser(string displayName)
        {
            Assert.IsTrue(UserPage.GetUserEditButton(displayName).Exists);
        }

        [Then(@"The user (.*) should have a link to delete the User")]
        public void ThenTheUserShouldHaveALinkToDeleteTheUser(string displayName)
        {
            Assert.IsTrue(UserPage.GetUserDeleteButton(displayName).Exists);
        }

        [Then(@"The user (.*) should have a link to edit the Security Roles")]
        public void ThenTheUserShouldHaveALinkToEditTheSecurityRoles(string displayName)
        {
            Assert.IsTrue(UserPage.GetUserRolesButton(displayName).Exists);
        }

        [Then(@"The user (.*) should not have a link to delete the User")]
        public void ThenTheUserShouldNotHaveALinkToDeleteTheUser(string displayName)
        {
            Assert.IsFalse(UserPage.GetUserDeleteButton(displayName).Exists);
        }

        [Then(@"The user (.*) should not have a link to edit the User")]
        public void ThenTheUserShouldNotHaveALinkToEditTheUser(string displayName)
        {
            Assert.IsFalse(UserPage.GetUserEditButton(displayName).Exists);
        }

        [Then(@"The user (.*) should not have a link to edit the Security Roles")]
        public void ThenTheUserShouldNotHaveALinkToEditTheSecurityRoles(string displayName)
        {
            Assert.IsFalse(UserPage.GetUserRolesButton(displayName).Exists);
        }

        [Then(@"The user (.*) should not display")]
        public void ThenTheUserShouldNotDisplay(string displayName)
        {
            Assert.IsNull(UserPage.GetUserRow(displayName));
        }

        [When(@"I click User Settings from action menu")]
        public void WhenIClickFromActionMenu()
        {
            UserPage.UserSettingsLink.Click();
        }

        [When(@"I input (.*) in (.*) field")]
        public void WhenIInputInField(string value, string fieldName)
        {
            SiteSettingsPage.ContentPaneDiv
                .Div(Find.ById(s => s.EndsWith("SiteSettings_validationRegistrationSettings")))
                .TextFields[0].Value = value;
        }

        [When(@"I Update Site Settings")]
        public void WhenIUpdateSiteSettings()
        {
            SiteSettingsPage.UpdateLink.Click();
            SiteSettingsPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

        }


        [Then(@"I should see error message")]
        public void ThenIShouldSeeErrorMessage()
        {
            if(HomePage.PopUpFrame != null)
            {
                Assert.IsTrue(HomePage.PopUpFrame.Div(Find.ByClass(c => c.Contains("dnnFormValidationSummary"))).Exists);
            }
            else
            {
                Assert.IsTrue(IEInstance.Div(Find.ByClass(c => c.Contains("dnnFormValidationSummary"))).Exists);
            }
        }

        #region Scenario ""

        [When(@"I click my name to edit profile")]
        public void WhenIClickMyNameToEditProfile()
        {
            HomePage.RegisterLink.Click();
        }

        [When(@"I click Edit Profile button")]
        public void WhenIClickEditProfileButton()
        {
            UserProfilePage.EditProfileLink.Click();
        }

        [When(@"I upload a picture for photo")]
        public void WhenIUploadAPictureForPhoto()
        {
            UserProfilePage.UploadFileLink.Click();
            UserProfilePage.FileUpload.Set(Path.Combine(Environment.CurrentDirectory, "Resources\\Desert.jpg"));
            UserProfilePage.SaveFileLink.Click();
        }

        [When(@"I click Profile Update button")]
        public void WhenIClickUpdateButton()
        {
            UserProfilePage.ProfileUpdateLink.Click();
            UserProfilePage.ProfileUpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

        }

        [Then(@"I should see the picture just upload")]
        public void ThenIShouldSeeThePictureJustUpload()
        {
            Assert.IsTrue(UserProfilePage.PhotoFileSelectList.Option(Find.ByText("Desert.jpg")).Exists);
        }

        #endregion
    }
}
