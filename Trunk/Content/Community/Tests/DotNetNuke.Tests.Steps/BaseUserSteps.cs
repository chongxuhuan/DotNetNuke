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

using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Profile;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.UI.WatiN.Utilities;

using NUnit.Framework;
using TechTalk.SpecFlow;

using WatiN.Core;

using Table = TechTalk.SpecFlow.Table;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        public UserProfilePage _userProfilePage;
        private UserPage _userPage;

        /// <summary>
        /// Makes the host user a member of portal 0. 
        /// </summary>
        [BeforeScenario("HostUserMustBeMemberOfPortal")]
        public void HostUserMustBeMemberOfPortal()
        {
            ExecuteScript("Scripts/AddSuperUserToPortal.sql");
        }

        /// <summary>
        /// Sets the admin's credentials to the default value that is stored within the tests.
        /// The admin will also be forced to update their password when they log in to the site. 
        /// </summary>
        [BeforeScenario("MustBeDefaultAdminCredentialsForceUpdate")]
        public void DefaultAdminCredentialsForceUpdate()
        {
            ExecuteScript("Scripts/ResetPassword.sql");
            ExecuteScript("Scripts/SetUpdatePassword.sql");
        }

        /// <summary>
        /// This will set the admin's credentials to the default value that is stored within the tests. 
        /// The admin will not be forced to update their password when they log in. 
        /// Use this tag anytime the admin will be logging in to the site.
        /// </summary>
        [BeforeScenario("MustBeDefaultAdminCredentials")]
        public void MustBeDefaultAdminCredentials()
        {
            ExecuteScript("Scripts/ResetPassword.sql");
        }

        /// <summary>
        /// Creates a test superuser. 
        /// Use this for tests that will need an existing superuser to complete the tests.
        /// The superuser will have the following credentials:
        /// username: 'TestSuperUser', emails: 'TestSuperUser@dnn.com', first name: 'TestSuperUserFN', last name: 'TestSuperUserLN', display name: 'TestSuperUser DN', password: 'dnnhost'
        /// </summary>
        [BeforeScenario("TestSuperUserMustExist")]
        public void TestSuperUserMustExist()
        {
            var user = UserController.GetUserByName(-1, "TestSuperUser");
            if (user == null)
            {
                user = new UserInfo
                           {
                               Username = "TestSuperUser",
                               IsSuperUser = true,
                               Email = "TestSuperUser@dnn.com",
                               FirstName = "TestSuperUserFN",
                               LastName = "TestSuperUserLN",
                               DisplayName = "TestSuperUser DN"
                           };
                user.Membership.Password = "Password";

                UserController.CreateUser(ref user);
            }
            else if (user.IsDeleted)
            {
                UserController.RestoreUser(ref user);
            }
        }

        /// <summary>
        /// Adds a user to the site without using the UI. 
        /// Use this for tests that will need an exisitn user to complete the test, for example when testing the delete user feature, a user must exist so it can be delete. 
        /// Instead of adding a user within the tests (in a given statement), use this tab. 
        /// The user will have the following values:
        /// username: 'atestuser', email: 'testuser@dnn.com', first name: 'testuserFN', last name: 'testuserLN', display name: 'Testuser DN', password: 'dnnhost'
        /// </summary>
        [BeforeScenario("MustHaveAUserWithFullProfile")]
        public void MustHaveUserMustHaveAUserWithFullProfile()
        {
            WebConfigManager.SyncConfig(PhysicalPath);

            var user = UserController.GetUserByName(0, "MichaelWoods");
            if (user == null)
            {
                user = new UserInfo
                {
                    PortalID = 0,
                    Username = "MichaelWoods",
                    IsSuperUser = false,
                    Email = "MichaelWoods@dnncorp.com",
                    FirstName = "Michael",
                    LastName = "Woods",
                    DisplayName = "Michael Woods"
                };
                user.Membership.Password = "password1234";
                UserController.CreateUser(ref user);

                user.Profile.City = "Vancouver";
                user.Profile.Country = "Canada";
                user.Profile.PostalCode = "V1M 4A6";
                user.Profile.Region = "British Columbia";
                user.Profile.Street = "211 – 9440 202nd Street Langley";
                var provider = ProfileProvider.Instance();
                provider.UpdateUserProfile(user);

            }
            else if (user.IsDeleted)
            {
                UserController.RestoreUser(ref user);
            }
        }

        /// <summary>
        /// Adds a user to the site without using the UI. 
        /// Use this for tests that will need an exisitng user to complete the test, for example when testing the delete user feature, a user must exist so it can be delete. 
        /// Instead of adding a user within the tests (in a given statement), use this tag. 
        /// The user will have the following values:
        /// username: 'atestuser', email: 'testuser@dnn.com', first name: 'testuserFN', last name: 'testuserLN', display name: 'Testuser DN', password: 'dnnhost'
        /// </summary>
        [BeforeScenario("MustHaveUser1Added")]
        public void MustHaveUser1Added()
        {
            var user = UserController.GetUserByName(0, "atestuser");
            if (user == null)
            {
                ExecuteScript("Scripts/AddTestUser.sql");
            }
        }

        [BeforeScenario("CustomRegistration")]
        public void CustomRegistration()
        {
            PortalController.UpdatePortalSetting(PortalId, "Registration_RegistrationFormType", "1");
            PortalController.UpdatePortalSetting(PortalId, "Registration_RegistrationFields", "Email,Username,DisplayName,Password,PasswordConfirm,Country");

        }

        /// <summary>
        /// Clicks on the users display name link. 
        /// </summary>
        [Given(@"I have clicked on my name")]
        [When(@"I have clicked on my name")]
        public void GivenIHaveClickedOnMyName()
        {
            Thread.Sleep(2000);
            HomePage.RegisterLink.ClickNoWait();
        }

        /// <summary>
        /// Clicks on the Edit Profile link from a users profile page. 
        /// </summary>
        [When(@"I click Edit Profile")]
        public void WhenIClickEditProfile()
        {
            _userProfilePage = GetPage<UserProfilePage>();
            _userProfilePage.EditProfileLink.Click();
        }

        /// <summary>
        /// Clicks on the Manage Profile link from a users profile page. 
        /// </summary>
        [When(@"I click the Manage Profile Tab")]
        public void WhenIClickManageProfileTab()
        {
            _userProfilePage = GetPage<UserProfilePage>();
            _userProfilePage.ManageProfileTabLink.Click();
        }

        /// <summary>
        /// Creates a Superuser with the credentials in the table.
        /// </summary>
        /// <param name="table">A table containing the superusers credentials.</param>
        [Given(@"The Super User exists")]
        public void GivenTheSuperUserExists(Table table)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I click the Edit User Link for (.*)")]
        public void WhenIClickTheEditUserLinkForUser(string userName)
        {
            _userPage = GetPage<UserPage>();
            _userPage.GetUserEditButton(userName).Click();
        }

        [When(@"I log off")]
        [Given(@"I log off")]
        public void ILogOff()
        {
            if (IEInstance != null)
            {
                IEInstance.GoTo(SiteUrl + "/logoff.aspx");
            }
            Thread.Sleep(2000);
        }

    }
}
