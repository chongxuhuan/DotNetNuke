using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetNuke.Tests.Utilities;
using WatiN.Core;
using System.Timers;


namespace DotNetNuke.Tests.UI
{
    /// <summary>
    /// Summary description for HostSettingsTests
    /// </summary>
    [TestClass]
    public class HostSettingsTests
    {

        public static IE ie;
        public TestContext TestContext { get; set; }
        public Timer timer1;

        public HostSettingsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

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
             DatabaseManager.DropDatabase(TestEnvironment.DatabaseName);
             DatabaseManager.CopyAndAttachDatabase(TestContext, TestEnvironment.DatabaseName);

             // Open a new Internet Explorer window and navigate to the website
             ie = new IE();
             ie.GoTo(TestEnvironment.PortalUrl);

             Login(TestUsers.Host.UserName, TestUsers.Host.Password, TestUsers.Host.DisplayName);
         }
         
        
        [TestCleanup()]
        public void MyTestCleanup() 
        {
             DatabaseManager.DropDatabase(TestEnvironment.DatabaseName);
             ie.Close();
        }
        
        #endregion

        [TestMethod]
        public void Access_The_Settings_Page()
        {
            //Act
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            //Assert
            Assert.AreEqual("Host > Host Settings", ie.Span(Find.ById("dnn_dnnBREADCRUMB_lblBreadCrumb")).Text);
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("Host Settings"));

        }

        [TestMethod]
        public void Enabling_and_Disabling_Remember_Me()
        {
            Logout();
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            Assert.IsTrue(ie.Element(Find.ByClass("Content")).InnerHtml.Contains("Remember Login"));
            Login(TestUsers.Host.UserName, TestUsers.Host.Password, TestUsers.Host.DisplayName);
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.CheckBox(Find.ById("dnn_ctr327_HostSettings_chkRemember")).Checked = false;
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            Logout();
            Assert.IsFalse(ie.Element(Find.ByClass("Content")).InnerHtml.Contains("Remember Login"));
        }

        [TestMethod]
        public void Entering_An_Auto_Unlock_Accounts_Time()
        {
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.Button(Find.ById("dnn_ctr327_HostSettings_dshOther_imgIcon")).Click();
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtAutoAccountUnlock")).TypeText("2");
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Home.aspx");
            Logout();
            for(int i = 0; i < 6; i++)
            {
                //Click the Login/Logout button
                ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
                //Enter the users information
                ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText("user1");
                ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText("psswrd");
                //Click the Login
                ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();
            }
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(LoginForAutoUnlock);
            timer1.Interval = 120;
            timer1.AutoReset = false;
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This account has been locked out after too many unsuccessful login attempts. Please wait 2 minutes "));
            timer1.Start();
        }

        [TestMethod]
        public void Enabling_Disabling_Custom_Copyright_Credits()
        {
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            Assert.IsTrue(ie.Element(Find.ById("Head")).InnerHtml.Contains("DotNetNuke� - http://www.dotnetnuke.com"));
            Assert.IsTrue(ie.Element(Find.ById("Head")).InnerHtml.Contains("Copyright (c) 2002-2009 "));
            Assert.IsTrue(ie.Element(Find.ById("Head")).InnerHtml.Contains("by DotNetNuke Corporation   "));
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.CheckBox(Find.ById("dnn_ctr327_HostSettings_chkCopyright")).Click();
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            Assert.IsFalse(ie.Element(Find.ById("Head")).InnerHtml.Contains("DotNetNuke� - http://www.dotnetnuke.com"));
            Assert.IsFalse(ie.Element(Find.ById("Head")).InnerHtml.Contains("Copyright (c) 2002-2009 "));
            Assert.IsFalse(ie.Element(Find.ById("Head")).InnerHtml.Contains("by DotNetNuke Corporation   "));
        }

        [TestMethod]
        public void Changing_The_Host_Title()
        {
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostTitle")).TypeText("New Title");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Admin.aspx");
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            Assert.AreEqual("New Title", ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostTitle")).Text);
        }

        [TestMethod]
        public void Changing_The_Host_URL()
        {
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostURL")).TypeText("http://testwebsite.com");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Admin.aspx");
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            Assert.AreEqual("http://testwebsite.com", ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostURL")).Text);
        }

        [TestMethod]
        public void Updating_The_Host_Email()
        {
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).TypeText("support@localhost.com");
            ie.Span(Find.ById("dnn_ctr327_HostSettings_cmdUpdate")).Button(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("Admin.aspx");
            ie.GoToDNNUrl("tabid/16/portalid/0/Default.aspx");
            Assert.AreEqual("support@localhost.com", ie.TextField(Find.ById("dnn_ctr327_HostSettings_txtHostEmail")).Text);
        }


        public static void LoginForAutoUnlock(object source, ElapsedEventArgs e)
        {
            Login("user1", "password", "user1 DN");
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
    }
}
