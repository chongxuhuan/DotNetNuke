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
    /// Summary description for ControlPanelTests
    /// </summary>
    [TestClass]
    public class ControlPanelTests
    {
        public static IE ie;
        public TestContext TestContext { get; set; }

        public ControlPanelTests()
        {
            
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
        public void Test_Site_Link()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdSite")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("Site Settings"));
        }

        [TestMethod]
        public void Test_Users_Link()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdUsers")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("User Accounts"));
        }

        [TestMethod]
        public void Test_Roles_Link()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdRoles")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("Security Roles"));
        }

        [TestMethod]
        public void Test_Files_Link()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdFiles")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("File Manager"));
        }

        [TestMethod]
        public void Test_Extensions_Link()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdExtensions")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_icon")).InnerHtml.Contains("Extensions"));
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
