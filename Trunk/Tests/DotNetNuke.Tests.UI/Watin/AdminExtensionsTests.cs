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
    /// Summary description for AdminExtensionsTests
    /// </summary>
    [TestClass]
    public class AdminExtensionsTests
    {

        public static IE ie;
        public TestContext TestContext { get; set; }

        public AdminExtensionsTests()
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
        public void Editing_Authentication_Systems_Extensions()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/Extensions.aspx");
            //Act
            ie.SelectList(Find.ById("dnn_ctr360_Extensions_cboPackageTypes")).Select("Container");
            ie.Link(Find.ByTitle("Edit this Extension")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_dnnTITLE_lblTitle")).InnerHtml.Contains("Edit Extension"));
        }

        [TestMethod]
        public void Editing_Container_Extensions()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/Extensions.aspx");
            //Act
            ie.SelectList(Find.ById("dnn_ctr360_Extensions_cboPackageTypes")).Select("");
            ie.Link(Find.ByTitle("Edit this Extension")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_dnnTITLE_lblTitle")).InnerHtml.Contains("Edit Extension"));
        }

        [TestMethod]
        public void Editing_Core_Language_Pack_Extensions()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/Extensions.aspx");
            //Act
            ie.SelectList(Find.ById("dnn_ctr360_Extensions_cboPackageTypes")).Select("Core Language Pack");
            ie.Link(Find.ByTitle("Edit this Extension")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_dnnTITLE_lblTitle")).InnerHtml.Contains("Edit Extension"));
        }

        [TestMethod]
        public void Editing_Module_Extensions()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/Extensions.aspx");
            //Act
            ie.SelectList(Find.ById("dnn_ctr360_Extensions_cboPackageTypes")).Select("Module");
            ie.WaitForComplete();
            ie.Link(Find.ByTitle("Edit this Extension")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_dnnTITLE_lblTitle")).InnerHtml.Contains("Edit Extension"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_EditExtension_lblTitle")).InnerHtml.Contains("Configure Authentication Extension Settings for this Portal"));
        }

        [TestMethod]
        public void Editing_Skin_Extensions()
        {
            //Arrange
            ie.GoToDNNUrl("Admin/Extensions.aspx");
            //Act
            ie.SelectList(Find.ById("dnn_ctr360_Extensions_cboPackageTypes")).Select("Skin");
            ie.Link(Find.ByTitle("Edit this Extension")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr360_dnnTITLE_lblTitle")).InnerHtml.Contains("Edit Extension"));
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
