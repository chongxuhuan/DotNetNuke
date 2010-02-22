using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetNuke.Tests.Utilities;
using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Tests.UI
{
    /// <summary>
    /// Summary description for AdminSiteSettingsTests
    /// </summary>
    [TestClass]
    public class AdminSiteSettingsTests
    {

        public static IE ie;
        public TestContext TestContext { get; set; }

        public AdminSiteSettingsTests()
        {
            
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
        public void Access_Site_Settings()
        {
            //Act
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("Site Settings"));

        }

        [TestMethod]
        public void Changing_User_Registration_To_None()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr358_SiteSettings_optUserRegistration_0")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Logout();
            ie.GoToDNNUrl("Home.aspx");
            Assert.IsFalse(ie.Element(Find.ByClass("bread_bg")).InnerHtml.Contains("Register"));
        }

        [TestMethod]
        public void Changing_Login_Page()
        {
            //Arrange
            AddParentPageFromCP("New Login Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl03_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            AddModule("Account Login");
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.SelectList(Find.ById("dnn_ctr358_SiteSettings_cboLoginTabId")).Select("New Login Page");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Logout();
            Assert.AreEqual("New Login Page", ie.Link(Find.ByClass("Breadcrumb")).Text);
            Assert.AreEqual("Account Login", ie.Element(Find.ById("dnn_ctr375_dnnTITLE_lblTitle")).Text);
        }



        [TestMethod]
        public void Changing_Page_Quota()
        {
            //Arrange
            Login("host", "dnnhost", "SuperUser Account");
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshHost_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr358_SiteSettings_txtPageQuota")).TypeText("19");
            ie.Link(Find.ByTitle("Update")).Click();
            Login("admin", "password", "Administrator Account");
            //Assert
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);

        }

        [TestMethod]
        public void Changing_User_Quota()
        {
            //Arrange
            Login("host", "dnnhost", "SuperUser Account");
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshHost_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr358_SiteSettings_txtUserQuota")).TypeText("1");
            ie.Link(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Home.aspx");
            Logout();
            //Click the Login/Logout button
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            //Enter the users information
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText("admin");
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText("password");
            //Click the Login
            ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();
            Assert.AreEqual("Administrator Account", ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);
            
            //Assert
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.Link(Find.ByTitle("Add New User")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);
        }

        [TestMethod]
        public void Changing_Splash_Page()
        {
            //Arrange
            AddParentPageFromCP("New Splash Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl03_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.SelectList(Find.ById("dnn_ctr358_SiteSettings_cboSplashTabId")).Select("New Splash Page");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Logout();
            ie.Close();
            ie = new IE();
            ie.GoTo(TestEnvironment.PortalUrl);
            Assert.AreEqual("New Splash Page", ie.Link(Find.ByClass("Breadcrumb")).Text);
        }

        [TestMethod]
        public void Changing_Home_Page()
        {
            //Arrange
            AddParentPageFromCP("New Home Page");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl03_ctl00")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Admin/SiteSettings.aspx");
            //Act
            ie.Button(Find.ById("dnn_ctr358_SiteSettings_dshAdvanced_imgIcon")).Click();
            ie.SelectList(Find.ById("dnn_ctr358_SiteSettings_cboHomeTabId")).Select("New Home Page");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Logout();
            Assert.AreEqual("New Home Page", ie.Link(Find.ByClass("Breadcrumb")).Text);
        }

        private void AddParentPageFromCP(String pageName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddModule(String moduleName)
        {
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")).Select(moduleName);
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
        }

        private void AddTextModule(String moduleName)
        {
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")).Select("HTML");
            ie.TextField(Find.ById("dnn_IconBar.ascx_txtTitle")).TypeText(moduleName);
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
        }

        private static void Logout()
        {
            if (ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text != "Register")
            {
                ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            }
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
    }
}
