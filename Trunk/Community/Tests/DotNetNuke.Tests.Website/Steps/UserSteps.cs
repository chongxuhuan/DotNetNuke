using System;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using NUnit.Framework;
using TechTalk.SpecFlow;
using System.Xml.Linq;

namespace DotNetNuke.Tests.Website.Steps
{
    [Binding]
    public class UserSteps : AutomationBase
    {
        /// <summary>
        /// Ensures that a Page called Designers is not present.  A page called Designers may then
        /// be added by the test.
        /// </summary>
        [BeforeScenario("CreateUserPhilipBeadleInAdminRole")]
        public void CreateUserPhilipBeadleInAdminRole()
        {
            var user = UserController.GetUserByName(PortalId, "host");
            var password = UserController.GetPassword(ref user, "");
            UserController.ChangePassword(user, password, "password123");
        }
        /// <summary>
        /// Deletes the user deadmau5
        /// </summary>
        [BeforeScenario("Userdeadmau5MustNotExist")]
        public void Userdeadmau5MustNotExist()
        {
            DeleteUser(PortalId, "deadmau5");
        }

        /// <summary>
        /// Deletes the user deadmau5
        /// </summary>
        [BeforeScenario("UserChristian.SmithMustNotExist")]
        public void UserChristianSmithMustNotExist()
        {
            DeleteUser(PortalId, "Christian.Smith");
        }

        /// <summary>
        /// Deletes the user deadmau5
        /// </summary>
        [BeforeScenario("SuperUsertigg_sMustNotExist")]
        public void SuperUsertigg_sMustNotExist()
        {
            DeleteUser(-1, "tigg_s");
        }

        private static void DeleteUser(int portalId, string userName)
        {
            var user = UserController.GetUserByName(portalId, userName);
            if (user != null)
            {
                UserController.DeleteUser(ref user, false, false);
                UserController.RemoveUser(user);
            }
            Config.Touch();
        }

        [Given(@"I am Logged In As a User in the Administrators Role")]
        public void GivenIAmLoggedInAsAUserInTheAdministratorsRole()
        {

        }

        [Given(@"There is a user (.*) (.*) with these roles")]
        public void GivenThereIsAUserWithTheseRoles(string userName, string password, Table table)
        {
            for (var row = 0; row <= table.RowCount - 1; row++)
            {
                GivenThereIsAUserWithThisRole(userName, password, table.Rows[row][0]);
            }
        }

        [Given(@"Login as UID=(.*) PWD=(.*) Role=(.*)")]
        public void LoginAs(string userName, string password, string role)
        {
            GivenThereIsAUserWithThisRole(userName, password, role);
            GivenIHaveClickedTheLoginLink();
            WhenIEnterTheUsername(userName);
            WhenIEnterThePassword(password);
            WhenIClickTheLoginButton();
            var url = SiteUrl;
            if (Page != null)
            {
                url = string.Format("{0}{1}.aspx", SiteUrl, Page.TabPath.Replace("//", "/"));
                Driver.Navigate().GoToUrl(url);
            }
        }

        private void GivenThereIsAUserWithThisRole(string userName, string password, string roleName)
        {
            var reset = false;
            var isSuperUser = roleName == "Super User";
            var portalId = PortalId;
            var user = UserController.GetUserByName(PortalId, userName);
            if (user == null)
            {
                user = new UserInfo
                           {
                               PortalID = PortalId,
                               Username = userName,
                               IsSuperUser = isSuperUser,
                               Email = string.Format("{0}@dnn.com", userName),
                               FirstName = string.Format("{0} FN", userName),
                               LastName = string.Format("{0} LN", userName),
                               DisplayName = string.Format("{0} DN", userName),
                               Membership = { Password = password }
                           };
                var res = UserController.CreateUser(ref user);
                reset = true;
            }
            else if (user.IsDeleted)
            {
                UserController.RestoreUser(ref user);
                reset = true;
            }
            user.Membership.Password = password;
            if (!isSuperUser)
            {
                var roles = user.Roles.Where(r => r == roleName);
                if (!roles.Any())
                {
                    var roleController = new RoleController();
                    var role = roleController.GetRoleByName(PortalId, roleName);
                    if (role == null)
                    {
                        role = new RoleInfo { RoleName = roleName, PortalID = PortalId, RoleGroupID = -1 };
                        role.RoleID = roleController.AddRole(role);
                    }
                    RoleController.AddUserRole(user, role, PortalSettings.Current, RoleStatus.Approved, DateTime.Now, Null.NullDate, false, false);
                    reset = true;
                }
            }

            User = user;
            if (reset)
            {
                Config.Touch();
            }
        }

        [Given(@"Click the Login link \(typically located in the top right corner of each page\) to open the User Login pop-up window")]
        public void GivenIHaveClickedTheLoginLink()
        {
            Driver.Navigate().GoToUrl(SiteUrl + "/Logoff.aspx");
            DesktopModules.Admin.Authentication.UI.LoginLink(Driver).Click();
        }

        [When(@"I enter the Login username (.*)")]
        public void WhenIEnterTheUsername(string userName)
        {
            DesktopModules.Admin.Authentication.UI.UserNameTextBox(Driver).SendKeys(userName);
        }

        [When(@"I enter the Login password (.*)")]
        public void WhenIEnterThePassword(string password)
        {
            DesktopModules.Admin.Authentication.UI.PasswordTextBox(Driver).SendKeys(password);
        }

        [When(@"I click the Login button")]
        public void WhenIClickTheLoginButton()
        {
            DesktopModules.Admin.Authentication.UI.LoginButton(Driver).Click();
        }

        [Then(@"I should be logged in as the user")]
        public void ThenIShouldBeLoggedInAsTheUser()
        {
            Assert.IsTrue(DesktopModules.Admin.Authentication.UI.UserNameLink(Driver).Text == User.DisplayName);
        }

    }
}
