using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DotNetNuke.Tests.Selenium.Tests
{
    public class SecurityRoles : Base
    {
        public static By AddNewRoleButton = By.CssSelector("*[id$='_Roles_cmdAddRole']");
        public static By AddNewRoleGroupButton = By.CssSelector("*[id$='_Roles_cmdAddRoleGroup']");

        public static By RoleTableItem(string roleName){return By.XPath("//td[contains(text(),'" + roleName + "')]");}

        public static By RoleTableManageUsersButton(string roleName){return By.XPath("//td[contains(text(),'" + roleName + "')]/preceding-sibling::*[1]");}
        public static By RoleTableEditButton(string roleName) { return By.XPath("//td[contains(text(),'" + roleName + "')]/preceding-sibling::*[2]/a"); }

        public static By EditSecurityRoleRoleNameTextbox = By.CssSelector("*[id$='_EditRoles_txtRoleName']");
        public static By EditSecurityRoleUpdateButton = By.CssSelector("*[id$='_EditRoles_cmdUpdate']");
        public static By EditSecurityRoleDeleteButton = By.CssSelector("*[id$='_EditRoles_cmdDelete']");
        public static By EditSecurityRoleDeleteButtonConfirm = By.XPath("//span[contains(text(),'Yes')]"); 
        
        public static By UserRolesAddUserToRoleButton = By.CssSelector("*[id$='_SecurityRoles_cmdAdd']");
        public static By UserRolesUserDropdown = By.CssSelector("*[id$='_SecurityRoles_cboUsers_Input']");
        public static By UserRolesUserItem(string userName) { return By.XPath("//li[contains(text(),'" + userName + "')]"); }
        public static By UserRolesUserTableUserItem(string userName) { return By.XPath("//a[contains(text(),'" + userName + "')]"); }

        public static void CreateNewRole(IWebDriver driver, string roleName)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SecurityRolesPage);

            driver.WaitClick(AddNewRoleButton);
            driver.WaitSendKeys(EditSecurityRoleRoleNameTextbox, roleName);
            driver.WaitClick(EditSecurityRoleUpdateButton);
        }

        public static void DeleteRole(IWebDriver driver, string roleName)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SecurityRolesPage);

            driver.WaitClick(RoleTableEditButton(roleName));

            driver.WaitClick(EditSecurityRoleDeleteButton);

            driver.WaitClick(EditSecurityRoleDeleteButtonConfirm);
        }

        public static void AddUserToRole(IWebDriver driver, string userName, string role)
        {
            Login.AsHost(driver);

            driver.Navigate().GoToUrl(SecurityRolesPage);
            driver.WaitClick(RoleTableManageUsersButton(role));

            driver.WaitClick(UserRolesUserDropdown);
            driver.WaitClick(UserRolesUserItem(userName));

            driver.WaitClick(UserRolesAddUserToRoleButton);
        }

        #region VerifyCreatingNewRole

        [Test]
        public void VerifyCreatingNewRoleFirefox() { VerifyCreatingNewRole(Firefox); }
        [Test]
        public void VerifyCreatingNewRoleChrome() { VerifyCreatingNewRole(Chrome); }
        [Test]
        public void VerifyCreatingNewRoleIe() { VerifyCreatingNewRole(Ie); }

        private static void VerifyCreatingNewRole(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            CreateNewRole(driver, "TestRole" + rndNum);

            Assert.That(driver.FindDnnElement(RoleTableItem("TestRole" + rndNum)).Text, Is.StringContaining("TestRole" + rndNum));
        }

        #endregion

        #region VerifyAddingUserToRole

        [Test]
        public void VerifyAddingUserToRoleFirefox() { VerifyAddingUserToRole(Firefox); }
        [Test]
        public void VerifyAddingUserToRoleChrome() { VerifyAddingUserToRole(Chrome); }
        [Test]
        public void VerifyAddingUserToRoleIe() { VerifyAddingUserToRole(Ie); }

        private static void VerifyAddingUserToRole(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            UserAccounts.AddNewUser(driver, "UserRole" + rndNum, "UserRole", rndNum.ToString(), "UserRole" + rndNum, "UserRole" + rndNum + "@testuser.com", "testpassword");
            CreateNewRole(driver, "TestRole" + rndNum);

            AddUserToRole(driver, "UserRole" + rndNum, "TestRole" + rndNum);

            Assert.That(driver.FindDnnElement(UserRolesUserTableUserItem("UserRole" + rndNum)).Text, Is.StringContaining("UserRole" + rndNum));
        }

        #endregion

        #region VerifyDeletingRole

        [Test]
        public void VerifyDeletingRoleFirefox() { VerifyDeletingRole(Firefox); }
        [Test]
        public void VerifyDeletingRoleChrome() { VerifyDeletingRole(Chrome); }
        [Test]
        public void VerifyDeletingRoleIe() { VerifyDeletingRole(Ie); }

        private static void VerifyDeletingRole(IWebDriver driver)
        {
            var rnd = new Random();
            int rndNum = rnd.Next(9999);

            CreateNewRole(driver, "TestRole" + rndNum);

            DeleteRole(driver, "TestRole" + rndNum);

            try
            {
                Console.WriteLine(driver.FindDnnElement(RoleTableItem("TestRole" + rndNum)));
                Assert.Fail("Role not Deleted");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass();
            }
        }

        #endregion
    }
}
