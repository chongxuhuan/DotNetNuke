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
    /// Summary description for HTMLModuleTests
    /// </summary>
    [TestClass]
    public class HTMLModuleTests
    {
        public static IE ie;
        public TestContext TestContext { get; set; }

        public HTMLModuleTests()
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
        public void Direct_Publish_Add_New_Module()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            //Act
            AddTextModule("HTML Module");
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("content_pad")).InnerHtml.Contains("HTML Module"));
            ie.RadioButton(Find.ById("dnn_IconBar.ascx_optMode_0")).Click();
            Assert.IsFalse(ie.Element(Find.ByClass("content_pad")).InnerHtml.Contains("HTML Module"));
        }

        [TestMethod]
        public void Direct_Publish_Edit_Content_From_Edit_Page()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This module has been edited.");
            ie.Link(Find.ByTitle("Save")).Click();
            //Assert
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("HTML Module"));
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This module has been edited."));
            ie.RadioButton(Find.ById("dnn_IconBar.ascx_optMode_0")).Click();
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("HTML Module"));
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This module has been edited."));

        }

        [TestMethod]
        public void Direct_Publish_Preview_Current_Content()
        {
            //This method passes when it shouldn't
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This module has been edited.");
            ie.Link(Find.ByTitle("Save")).Click();
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            //ie.Link(Find.ByTitle("Preview")).Click();
            //Assert
            Assert.IsTrue(ie.Table(Find.ById("dnn_ctr375_EditHTML_tblPreview")).Enabled);
        }

        [TestMethod]
        public void Direct_Pulish_Preview_From_Version_History()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            for (int i = 0; i < 3; i++)
            {
                ie.Link(Find.ByTitle("Edit Content")).Click();
                ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
                ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is version " + (i+1));
                ie.Link(Find.ByTitle("Save")).Click();
            }
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.Button(Find.ById("dnn_ctr375_EditHTML_dshVersions_imgIcon")).Click();
            ie.Button(Find.ByName("dnn$ctr375$EditHTML$grdVersions$ctl03$ctl00")).Click();
            //Asssert
            Assert.AreEqual("This is version 2", ie.Element(Find.ById("dnn_ctr375_EditHTML_lblPreview")).Text);

        }

        [TestMethod]
        public void Content_Staging_Edit_From_Edit_Page_Draft()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is the first version.");
            ie.Link(Find.ByTitle("Save")).Click();
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.SelectList(Find.ById("dnn_ctr375_ModuleSettings_Settings_cboWorkflow")).Select("Content Staging");
            ie.Link(Find.ByTitle("Update")).Click();
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is the second version.");
            ie.Link(Find.ByTitle("Save")).Click();
            //Assert
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This is the second version."));
            ie.RadioButton(Find.ById("dnn_IconBar.ascx_optMode_0")).Click();
            ie.WaitForComplete();
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This is the first version."));

        }

        [TestMethod]
        public void Content_Staging_Publish_From_Edit_Page()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is the first version.");
            ie.Link(Find.ByTitle("Save")).Click();
            ie.Button(Find.ByTitle("Settings")).Click();
            ie.SelectList(Find.ById("dnn_ctr375_ModuleSettings_Settings_cboWorkflow")).Select("Content Staging");
            ie.Link(Find.ByTitle("Update")).Click();
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is the second version.");
            ie.Link(Find.ByTitle("Save")).Click();
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.CheckBox(Find.ById("dnn_ctr375_EditHTML_chkPublish")).Click();
            ie.Link(Find.ByTitle("Save")).Click();
            //Assert
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This is the second version."));
            ie.RadioButton(Find.ById("dnn_IconBar.ascx_optMode_0")).Click();
            Assert.IsTrue(ie.TableCell(Find.ById("dnn_ContentPane")).InnerHtml.Contains("This is the second version."));
        }

        [TestMethod]
        public void Content_Staging_Rollback_to_Previous_Version()
        {
            //Arrange
            AddParentPageFromCP("HTML Page");
            AddTextModule("HTML Module");
            for (int i = 0; i < 3; i++)
            {
                ie.Link(Find.ByTitle("Edit Content")).Click();
                ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
                ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("This is version " + (i + 1));
                ie.Link(Find.ByTitle("Save")).Click();
            }
            //Act
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.Button(Find.ById("dnn_ctr375_EditHTML_dshVersions_imgIcon")).Click();
            ie.Button(Find.ByName("dnn$ctr375$EditHTML$grdVersions$ctl03$ctl01")).Click();
            //Asssert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr375_HtmlModule_lblContent")).InnerHtml.Contains("This is version 2"));
        }

       
        private void AddParentPageFromCP(String pageName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddTextModule(String moduleName)
        {
            ie.SelectList(Find.ById("dnn_IconBar.ascx_cboDesktopModules")).Select("HTML");
            ie.TextField(Find.ById("dnn_IconBar.ascx_txtTitle")).TypeText(moduleName);
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddModule")).Click();
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
