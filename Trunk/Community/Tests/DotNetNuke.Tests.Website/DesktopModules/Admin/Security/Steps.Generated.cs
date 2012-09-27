using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.DesktopModules.Admin.Security
{
    [Binding]
    public partial class Steps : AutomationBase
    {
        private string userName;
        private string firstName;
        private string lastName;

        [When(@"I set Add User User Name to (.*)")]
        public void WhenISetUserNameToUserName(string UserName)
        {
            UI.UserNameTextbox(Driver).SendKeys(UserName);
            userName = UserName;
        }

        [When(@"I set Add User First Name to (.*)")]
        public void WhenISetFirstNameTo(string FirstName)
        {
            firstName = FirstName;
        }

        [When(@"I set Add User Last Name to (.*)")]
        public void WhenISetLastNameTo(string LastName)
        {
            lastName = LastName;
        }

        [When(@"I set Add User Display Name to (.*)")]
        public void WhenISetDisplayNameTo(string DisplayName)
        {
        }

        [When(@"I set Add User Email to (.*)")]
        public void WhenISetEmailTo(string Email)
        {
        }

        [When(@"I set Add User Password to (.*)")]
        public void WhenISetPasswordTo(string Password)
        {
        }

        [When(@"I set Add User Confirm Password to (.*)")]
        public void WhenISetConfirmPasswordTo(string Password)
        {
        }

        [When(@"I set Add User Authorize to (.*)")]
        public void WhenISetAuthorizeTo(string Authorize)
        {
        }

        [When(@"I set Add User Notify to (.*)")]
        public void WhenISetNotifyTo(string Notify)
        {
        }

        [When(@"I set Add User Random Password to (.*)")]
        public void WhenISetRandomPasswordTo(string RandomPassword)
        {
        }

        [When(@"I click Add New User")]
        public void WhenIClickAddNewUser()
        {

        }

        [Then(@"The newly added user account can now be viewed and modified using the User Accounts module")]
        public void ThenIfTheFormIsValidNewUserIsCreatedTheNewlyAddedUserAccountCanNowBeViewedAndModifiedUsingTheUserAccountsModule()
        {
        }

        [Then(@"If Authorize is checked the new user will automatically gain access to the Registered User role and any roles set for Auto Assignment")]
        public void ThenIfAuthorizeIsCheckedTheNewUserWillAutomaticallyGainAccessToTheRegisteredUserRoleAndAnyRolesSetForAutoAssignment()
        {
        }

        [Then(@"If Authorize is unchecked the new user will be created but will not be able to access the restricted areas of the site")]
        public void ThenIfAuthorizeIsUncheckedTheNewUserWillBeCreatedButWillNotBeAbleToAccessTheRestrictedAreasOfTheSite()
        {
        }

        [Then(@"If Notify is checked the new user will be sent a notification email")]
        public void ThenIfNotifyIsCheckedANotificationEmailWillBeSentToTheNewUser()
        {
        }

        [Then(@"If Notify is unchecked the new user will not be sent a notification email")]
        public void ThenIfNotifyIsUncheckedTheNewUserWillNotBeSentANotificationEmail()
        {
        }
    }
}
