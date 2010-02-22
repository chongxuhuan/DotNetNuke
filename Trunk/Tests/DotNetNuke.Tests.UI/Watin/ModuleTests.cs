using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatiN.Core;

using System.IO;
using WatiN.Core.DialogHandlers;
using DotNetNuke.Tests.Utilities;
using System.Data.SqlClient;
using DotNetNuke.Tests.UI;

namespace DotNetNuke.Tests.UI
{
    /// <summary>
    /// Summary description for ModuleTests
    /// </summary>
    [TestClass]
    public class ModuleTests
    {
        public static IE ie;
        public TestContext TestContext { get; set; }

        public ModuleTests()
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
         public static void MyClassInitialize(TestContext testContext) 
         { 
            
         }
        
         
         [ClassCleanup()]
         public static void MyClassCleanup() 
         { 
            
         }
        
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             IISManager.KillW3WP();
             DatabaseManager.EnsureDatabaseExists(TestEnvironment.DatabaseName);
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
        public void Add_Text_Module_To_Page()
        {
            //Arrange 
            AddParentPageFromCP("HTML Module Page");

            //Act
            AddTextModule("HTML module");

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("HTML module"));
            
        }

        [TestMethod]
        public void Move_Module_To_Another_Page()
        {
            //Arrange
            AddParentPageFromCP("HTML Module Page");
            AddParentPageFromCP("Move Page");

            //Act
            ie.GoToDNNUrl("HTMLModulePage.aspx");
            AddTextModule("Move Module");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Move Module"));
            ie.Div(Find.ByClass("c_footer")).Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshPage_imgIcon")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshOther_imgIcon")).Click();
            ie.SelectList(Find.ById("dnn_ctr375_ModuleSettings_cboTab")).Select("Move Page");
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            ie.GoToDNNUrl("MovePage.aspx");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Move Module"));
        }

        //These are not working
        [TestMethod]
        public void Modify_Module_Start_Date_Future()
        {
            //Arrange
            AddParentPageFromCP("HTML Module Page");
            //Act
            AddTextModule("Start Date Module");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Start Date Module"));
            ie.Div(Find.ByClass("c_footer")).Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshSecurity_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr375_ModuleSettings_txtStartDate")).TypeText("7/15/2012");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Effective - 7/15/2011"));
        }

        [TestMethod]
        public void Modify_Module_End_Date_Future()
        {
            //Arrange
            AddParentPageFromCP("HTML Module Page");
            //Act
            AddTextModule("End Date Module");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("End Date Module"));
            ie.Div(Find.ByClass("c_footer")).Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshSecurity_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr375_ModuleSettings_txtEndDate")).TypeText("7/15/2012");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsFalse(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Expired"));
        }

        [TestMethod]
        public void Modify_Module_Start_Date_Past()
        {
            //Arrange
            AddParentPageFromCP("HTML Module Page");
            //Act
            AddTextModule("Start Date Module");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Start Date Module"));
            ie.Div(Find.ByClass("c_footer")).Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshSecurity_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr375_ModuleSettings_txtStartDate")).TypeText("7/15/2008");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsFalse(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Effective"));
        }

        [TestMethod]
        public void Modify_Module_End_Date_Past()
        {
            //Arrange
            AddParentPageFromCP("HTML Module Page");
            //Act
            AddTextModule("End Date Module");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("End Date Module"));
            ie.Div(Find.ByClass("c_footer")).Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshSecurity_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr375_ModuleSettings_txtEndDate")).TypeText("7/15/2008");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Expired"));
        }

        [TestMethod]
        public void Add_Existing_Module_To_Page()
        {
            //Arrange
            AddParentPageFromCP("First Page");
            AddTextModule("Existing Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddParentPageFromCP("Second Page");
            //Act
            ie.RadioButton(Find.ById("dnn_IconBar.ascx_optModuleType_1")).Click();
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboTabs")).Select("First Page");
            ie.WaitForComplete();
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Existing Module"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Unique Text"));

        }

        [TestMethod]
        public void Modify_Module_View_Permissions()
        {
            //Arrange
            AddUser("perm", "permFN", "permLN", "perm DN", "email@address.com", "password");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Module Page");
            AddTextModule("Permissions Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            //Act
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.CheckBox(Find.ById("dnn_ctr375_ModuleSettings_chkInheritPermissions")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageTabs$dgPermissions$ctl04")).TypeText("perm");
            ie.Link(Find.ByTitle("Add")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Login("perm", "password", "perm DN");
            ie.GoToDNNUrl("ModulePage.aspx");
            Assert.IsFalse(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Text Module"));

        }

        [TestMethod]
        public void Modify_Module_Display_On_All_Pages()
        {
            //Arrange
            AddParentPageFromCP("Page One");
            AddTextModule("Modify Module");
            ie.Button(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            AddParentPageFromCP("Page Two");
            AddParentPageFromCP("Page Three");
            //Act
            ie.GoToDNNUrl("PageOne.aspx");
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshSecurity_imgIcon")).Click();
            ie.CheckBox(Find.ById("dnn_ctr375_ModuleSettings_chkAllTabs")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.GoToDNNUrl("PageOne.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));
            ie.GoToDNNUrl("PageTwo.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));
            ie.GoToDNNUrl("PageThree.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));
            ie.GoToDNNUrl("Admin/LogViewer.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Modify Module"));

        }

        [TestMethod]
        public void Modify_Module_Change_Container()
        {
            //Arrange
            AddParentPageFromCP("Module Page");
            AddTextModule("Container Module");
            //Act
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshPage_imgIcon")).Click();
            ie.SelectList(Find.ById("dnn_ctr375_ModuleSettings_ctlModuleContainer_cboSkin")).Select("MinimalExtropy - Title_Blue");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_head c_head_blue")).Exists);
        }

        [TestMethod]
        public void Modify_Module_Print_And_Syndicate()
        {
            //Arrange
            AddParentPageFromCP("Module Page");
            AddTextModule("Icon Module");
            //Act
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.Button(Find.ById("dnn_ctr375_ModuleSettings_dshPage_imgIcon")).Click();
            ie.CheckBox(Find.ById("dnn_ctr375_ModuleSettings_chkDisplayPrint")).Click();
            ie.CheckBox(Find.ById("dnn_ctr375_ModuleSettings_chkDisplaySyndicate")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsFalse(ie.Image(Find.ByTitle("Print")).Exists);
            Assert.IsTrue(ie.Image(Find.ByTitle("Syndicate")).Exists);
        }

        [TestMethod]
        public void Add_New_Module_Visibility()
        {
            //Arrange
            AddUser("perm", "permFN", "permLN", "perm DN", "email@address.com", "password");
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.GoToDNNUrl("Home.aspx");
            AddParentPageFromCP("Module Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl04_ctl00")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageTabs$dgPermissions$ctl04")).TypeText("perm");
            ie.Link(Find.ByTitle("Add")).Click();
            ie.WaitForComplete();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl02_ctl02_ctl01")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Act
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")).Select("HTML");
            ie.TextField(Find.ById("dnn_IconBar.ascx_txtTitle")).TypeText("Visibility Module");
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboPermission")).Select("Page Editors Only");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
            ie.Button(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            //Assert
            Login("user1", "password", "user1 DN");
            ie.GoToDNNUrl("ModulePage.aspx");
            Assert.IsFalse(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Visibility Module"));
            Login("perm", "password", "perm DN");
            ie.GoToDNNUrl("ModulePage.aspx");
            Assert.IsTrue(ie.Element(Find.ById("s_wrap_main")).InnerHtml.Contains("Visibility Module"));

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

        private void AddGlobalRole(String roleName)
        {
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.Link(Find.ByTitle("Add New Role")).Click();
            ie.TextField(Find.ById("dnn_ctr364_EditRoles_txtRoleName")).TypeText(roleName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AssignUserToRole(String userName, String roleName)
        {
            ie.GoToDNNUrl("Admin/SecurityRoles.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText(userName);
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Manage Roles")).Click();
            ie.SelectList(Find.ById("dnn_ctr365_SecurityRoles_cboRoles")).Select(roleName);
            ie.Link(Find.ByTitle("Add Role To User")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr365_SecurityRoles_pnlUserRoles")).InnerHtml.Contains(roleName));

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

    }
}
