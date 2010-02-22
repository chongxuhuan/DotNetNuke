using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetNuke.Tests.Utilities;
using WatiN.Core;

namespace DotNetNuke.Tests.UI
{
    /// <summary>
    /// Summary description for RoleTests
    /// </summary>
    [TestClass]
    public class RoleTests
    {
        public static IE ie;
        public TestContext TestContext { get; set; }

        public RoleTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>


        #region Additional test attributes
        
         
         [ClassInitialize()]
         public static void MyClassInitialize(TestContext testContext) { }
        
         
         [ClassCleanup()]
         public static void MyClassCleanup() { }
        
        
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             IISManager.KillW3WP();
             DatabaseManager.DropDatabase(TestEnvironment.DatabaseName);
             DatabaseManager.CopyAndAttachDatabase(TestContext, TestEnvironment.DatabaseName);

             // Open a new Internet Explorer window and navigate to the website
             ie = new IE();
             ie.GoTo(TestEnvironment.PortalUrl);

             Login(TestUsers.Admin.UserName, TestUsers.Admin.Password, TestUsers.Admin.DisplayName);
         }
        
         
         [TestCleanup()]
         public void MyTestCleanup()
         {
             DatabaseManager.DropDatabase(TestEnvironment.DatabaseName);
             ie.Close();
         }
        
        #endregion

        [TestMethod]
        public void Add_Global_Role()
        {
            //Arrange & Act
            AddGlobalRole("Test Role");
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_content")).InnerHtml.Contains("Test Role"));
        }

        [TestMethod]
        public void Add_New_Role_Group()
        {
            //Arrange & Act
            AddRoleGroup("Test Role Group");
            //Assert
            Assert.IsTrue(ie.SelectList(Find.ById("dnn_ctr364_Roles_cboRoleGroups")).InnerHtml.Contains("Test Role Group"));

        }

        [TestMethod]
        public void Add_Duplicate_Global_Role_Fail()
        {
            //Arrange & Act
            AddGlobalRole("Administrators");
            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Error")).Exists);
        }

        [TestMethod]
        public void Edit_Role_Group()
        {
            //Arrange
            AddRoleGroup("Test Role Group");
            //Act
            ie.SelectList(Find.ById("dnn_ctr364_Roles_cboRoleGroups")).Select("Test Role Group");
            ie.Link(Find.ById("dnn_ctr364_Roles_lnkEditGroup")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditGroups_txtRoleGroupName")).TypeText("New Role Group");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsFalse(ie.SelectList(Find.ById("dnn_ctr364_Roles_cboRoleGroups")).InnerHtml.Contains("Test Role Group"));
            Assert.IsTrue(ie.SelectList(Find.ById("dnn_ctr364_Roles_cboRoleGroups")).InnerHtml.Contains("New Role Group"));

        }

        [TestMethod]
        public void Add_Role_To_Group()
        {
            //Arrange
            AddRoleGroup("Test Role Group");
            //Act
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.Link(Find.ByTitle("Add New Role")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditRoles_txtRoleName")).TypeText("Group Role");
            ie.SelectList(Find.ById("dnn_ctr364_EditRoles_cboRoleGroups")).Select("Test Role Group");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsFalse(ie.Element(Find.ByClass("c_content")).InnerHtml.Contains("Group Role"));
            ie.SelectList(Find.ById("dnn_ctr364_Roles_cboRoleGroups")).Select("Test Role Group");
            Assert.IsTrue(ie.Element(Find.ByClass("c_content")).InnerHtml.Contains("Group Role"));
        }

        [TestMethod]
        public void Add_Duplicate_Role_Group_Fail()
        {
            //Arrange
            AddRoleGroup("Original");
            //Act
            AddRoleGroup("Original");
            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Error")).Exists);
        }

        [TestMethod]
        public void Add_Auto_Assign_Public_Role()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            //Act
            ie.Link(Find.ByTitle("Add New Role")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditRoles_txtRoleName")).TypeText("Auto Public Role");
            ie.CheckBox(Find.ById("dnn_ctr364_EditRoles_chkIsPublic")).Click();
            ie.CheckBox(Find.ById("dnn_ctr364_EditRoles_chkAutoAssignment")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText("user1");
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Manage Roles")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr365_SecurityRoles_pnlUserRoles")).InnerHtml.Contains("Auto Public Role"));

        }

        [TestMethod]
        public void Assign_User_To_Role()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            AddGlobalRole("Test Role");
            //Act
            AssignUserToRole("user1", "Test Role");
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr365_SecurityRoles_pnlUserRoles")).InnerHtml.Contains("Test Role"));

        }


        [TestMethod]
        public void Test_Role_Effective_Date_In_Future()
        {
            //Arrange
            AddGlobalRole("Date Role");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Date Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl04_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AddTextModule("Text Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            
            //Act
            AssignUserToRoleEffectiveDate("user1", "Date Role", "7/15/2012");
            Login("user1", "password", "user1 DN");
            ie.GoToDNNUrl("DatePage.aspx");
            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);
            Assert.IsFalse(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Text Module"));
        }

        [TestMethod]
        public void Test_Role_Effective_Date_In_Past()
        {
            //This method is passing but it is not running properly
            //Arrange
            AddGlobalRole("Date Role");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Date Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl04_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AddTextModule("Text Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            //Act
            AssignUserToRoleEffectiveDate("user1", "Date Role", "7/15/2008");
            Login("user1", "password", "user1 DN");
            ie.GoToDNNUrl("DatePage.aspx");
            //Assert
            Assert.IsFalse(ie.Image(Find.ByTitle("Warning")).Exists);
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Text Module"));

        }

        [TestMethod]
        public void Test_Role_Expiry_Date_In_Future()
        {
            //Passed but did not run properly...
            //Arrange
            AddGlobalRole("Date Role");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Date Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl04_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AddTextModule("Text Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            //Act
            AssignUserToRoleExpiryDate("user1", "Date Role", "7/15/2012");
            Login("user1", "password", "user1 DN");
            ie.GoToDNNUrl("DatePage.aspx");
            //Assert
            Assert.IsFalse(ie.Image(Find.ByTitle("Warning")).Exists);
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Text Module"));

        }

        [TestMethod]
        public void Test_Role_Expiry_Date_In_Past()
        {
            //Arrange
            AddGlobalRole("Date Role");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Date Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl04_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AddTextModule("Text Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");

            //Act
            AssignUserToRoleExpiryDate("user1", "Date Role", "7/15/2008");
            Login("user1", "password", "user1 DN");
            ie.GoToDNNUrl("DatePage.aspx");
            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);
            Assert.IsFalse(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Text Module"));
        }

        [TestMethod]
        public void Test_Public_Role()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.Link(Find.ByTitle("Add New Role")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditRoles_txtRoleName")).TypeText("Test Role");
            ie.CheckBox(Find.ById("dnn_ctr364_EditRoles_chkIsPublic")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AssignUserToRole("user1", "Test Role");
            //Act
            Login("user1", "password", "user1 DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Services")).Click();
            //Assert
            Assert.IsTrue(ie.TableRow(Find.ById("dnn_ctr_ManageUsers_MemberServices_ServicesRow")).InnerHtml.Contains("Test Role"));
        }


        private void AddRoleGroup(String groupName)
        {
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.Link(Find.ByTitle("Add New Role Group")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditGroups_txtRoleGroupName")).TypeText(groupName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddGlobalRole(String roleName)
        {
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.Link(Find.ByTitle("Add New Role")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditRoles_txtRoleName")).TypeText(roleName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AssignUserToRole(String userName, String roleName)
        {
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText(userName);
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Manage Roles")).Click();
            ie.SelectList(Find.ById("dnn_ctr365_SecurityRoles_cboRoles")).Select(roleName);
            ie.Link(Find.ByTitle("Add Role To User")).Click();

        }

        private void AssignUserToRoleEffectiveDate(String userName, String roleName, String Date)
        {
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText(userName);
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Manage Roles")).Click();
            ie.SelectList(Find.ById("dnn_ctr365_SecurityRoles_cboRoles")).Select(roleName);
            ie.TextField(Find.ById("dnn_ctr365_SecurityRoles_txtEffectiveDate")).Value = Date;
            ie.Link(Find.ByTitle("Add Role To User")).ClickNoWait();
        }

        private void AssignUserToRoleExpiryDate(String userName, String roleName, String Date)
        {
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText(userName);
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Manage Roles")).Click();
            ie.SelectList(Find.ById("dnn_ctr365_SecurityRoles_cboRoles")).Select(roleName);
            ie.TextField(Find.ById("dnn_ctr365_SecurityRoles_txtExpiryDate")).TypeText(Date);
            ie.Link(Find.ByTitle("Add Role To User")).ClickNoWait();
        }

        private static void Login(string userName, string password, string expectedDisplayName)
        {
            // If we are logged in (say, from a previous test), log out
            if (ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text != "Register")
            {
                ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            }
            Assert.AreEqual("Register", ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);

            //Click the Login/Logout button
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            //Enter the users information
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText(userName);
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText(password);
            //Click the Login
            ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();

            Assert.AreEqual(expectedDisplayName, ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);
        }

        private static void Logout()
        {
            if (ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text != "Register")
            {
                ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            }
        }

        private void AddUser(String userName, String fName, String lName, String dName, String email, String password)
        {
            //Arrange
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");

            //Act
            ie.Link(Find.ByTitle("Add New User")).Click();
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$UserEditor$ctl00$Username")).TypeText(userName);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$UserEditor$ctl01$FirstName")).TypeText(fName);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$UserEditor$ctl02$LastName")).TypeText(lName);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$UserEditor$ctl03$DisplayName")).TypeText(dName);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$UserEditor$ctl04$Email")).TypeText(email);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$txtPassword")).TypeText(password);
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$User$txtConfirm")).TypeText(password);
            ie.Link(Find.ByTitle("Add New User")).Click();
        }

        private void AddTextModule(String moduleName)
        {
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")).Select("HTML");
            ie.TextField(Find.ById("dnn_IconBar.ascx_txtTitle")).TypeText(moduleName);
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
        }

        private void AddParentPageFromCP(String pageName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.Link(Find.ByTitle("Update")).Click();
        }
    }
}
