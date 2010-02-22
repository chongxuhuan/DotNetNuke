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
namespace DotNetNuke.Tests.UI
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    //

    [TestClass]
    public class PageTests
    {
        public static IE ie;

        #region Additional test attributes

        public TestContext TestContext { get; set; }

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
        public void Add_Page_From_Control_Panel()
        {
            //Arrange
            String pageName = "Page1";

            //Act
            AddParentPageFromCP(pageName);

            //Assert
            Assert.AreEqual(pageName, ie.Link(Find.ByClass("Breadcrumb")).Text);
        }

        [TestMethod]
        public void Add_Page_From_Pages_Module()
        {
            //Arrange
            String pageName = "Page 2";

            //Act
            AddParentPageFromPM(pageName);

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr359_Tabs_lstTabs")).InnerHtml.Contains(pageName));
        }


        [TestMethod]
        public void Add_Duplicate_Page_Fail()
        {
            //Act
            AddParentPageFromCP("Admin");
            ie.WaitForComplete();

            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Error")).Exists);

        }

        [TestMethod]
        public void Add_Child_Page_From_Control_Panel()
        {
            //Arrange & Act
            AddParentPageFromCP("Parent Page");
            AddChildPageFromCP("Child Page", "Parent Page");
            ie.GoToDNNUrl("ParentPage/ChildPage.aspx");

            //Assert
            Assert.AreEqual("Parent Page > Child Page", ie.Span(Find.ById("dnn_dnnBREADCRUMB_lblBreadCrumb")).Text);
        }

        [TestMethod]
        public void Add_Child_Page_From_Pages_Module()
        {
            //Arrange & Act
            AddParentPageFromPM("Parent Page");
            AddChildPageFromPM("Child Page", "Parent Page");

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr359_Tabs_lstTabs")).InnerHtml.Contains("...Child Page"));
        }

        [TestMethod]
        public void Edit_Page_Parent_To_Child()
        {
            //Arrange
            //Add a page that is currently a parent.
            AddParentPageFromCP("Parent Page");
            Assert.AreEqual("Parent Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

            //Add a page that is currently a parent and will become a child of the above page
            AddParentPageFromCP("Child Page");
            Assert.AreEqual("Child Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

            //Act
            ie.GoToDNNUrl("ChildPage.aspx");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select("Parent Page");
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Assert.AreEqual("Parent Page > Child Page", ie.Span(Find.ById("dnn_dnnBREADCRUMB_lblBreadCrumb")).Text);
        }

        [TestMethod]
        public void Edit_Page_Child_To_Parent()
        {
            AddParentPageFromCP("Parent Page");
            AddChildPageFromCP("Child Page", "Parent Page");
            ie.GoToDNNUrl("ParentPage/ChildPage.aspx");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.Link(Find.ByTitle("Update")).Click();
            Assert.AreEqual("Child Page", ie.Span(Find.ById("dnn_dnnBREADCRUMB_lblBreadCrumb")).Text);
        }

        [TestMethod]
        public void Edit_Page_Change_Skin()
        {
            //Arrange
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            //Act
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_ctlSkin_cboSkin")).Select("MinimalExtropy - index full");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("ADNN_510_Test_Portals__default_Skins_MinimalExtropy_index_full_css")).Exists);
        }

        [TestMethod]
        public void Delete_Page_From_Control_Panel()
        {
            //Arrange
            AddParentPageFromCP("Delete Page");
            ie.GoToDNNUrl("DeletePage.aspx");
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            //Act
            using(new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Link(Find.ById("dnn_IconBar.ascx_cmdDeleteTab")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            ie.WaitForComplete();
            //Assert
            Assert.IsFalse(ie.Element(Find.ByClass("menu_style")).InnerHtml.Contains("Delete Page"));
            ie.GoToDNNUrl("Admin/RecycleBin.aspx");
            Assert.IsTrue(ie.SelectList(Find.ById("dnn_ctr370_RecycleBin_lstTabs")).InnerHtml.Contains("Delete Page"));
        }

        [TestMethod]
        public void Add_Hidden_Page_From_Control_Panel()
        {
            //Add a hidden page through the control panel
            //Arrange
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();

            //Act
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Hidden Page 1");
            ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_chkMenu")).Click();
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Assert.AreEqual("Hidden Page 1", ie.Link(Find.ByClass("Breadcrumb")).Text);
            Assert.IsFalse(ie.Span(Find.ByClass("mainMenu")).InnerHtml.Contains("Hidden Page 1"));

        }

        [TestMethod]
        public void Add_Hidden_Page_From_Pages_Module()
        {
            //Add a hidden page from the pages module
            //Arrange
            ie.GoToDNNUrl("Admin/Pages.aspx");
            ie.Link(Find.ByTitle("Add New Page")).Click();

            //Act
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_txtTabName")).TypeText("Hidden Page 2");
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.CheckBox(Find.ById("dnn_ctr359_ManageTabs_chkMenu")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr359_Tabs_lstTabs")).InnerHtml.Contains("Hidden Page 2"));
            Assert.IsFalse(ie.Span(Find.ByClass("mainMenu")).InnerHtml.Contains("Hidden Page 2"));
        }

        [TestMethod]
        public void Add_Grandchild_Page_From_Control_Panel()
        {
            //Arrange & Act
            AddParentPageFromCP("Parent Page");
            AddChildPageFromCP("Child Page", "Parent Page");
            AddChildPageFromCP("Grandchild Page", "...Child Page");

            //Assert
            Assert.AreEqual("Parent Page > Child Page > Grandchild Page", ie.Span(Find.ById("dnn_dnnBREADCRUMB_lblBreadCrumb")).Text);
        }

        [TestMethod]
        public void Add_Grandchild_Page_From_Pages_Module()
        {
            //Arrange & Act
            AddParentPageFromPM("Parent Page");
            AddChildPageFromPM("Child Page", "Parent Page");
            AddChildPageFromPM("Grandchild Page", "...Child Page");

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr359_Tabs_lstTabs")).InnerHtml.Contains("......Grandchild Page"));
        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Permissions_Restricted()
        {
            AddUser("perm", "permFN", "permLN", "Perm DN", "email@address.com", "dnnperm");
            ie.GoToDNNUrl("Home.aspx");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Permissions Page");
            ie.TextField(Find.ByName("dnn$ctr$ManageTabs$dgPermissions$ctl04")).TypeText("perm");
            ie.Link(Find.ByTitle("Add")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            Login("perm", "dnnperm", "Perm DN");
            ie.GoToDNNUrl("PermissionsPage.aspx");
            Assert.AreEqual("Permissions Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

            Assert.IsFalse(ie.Image(Find.ById("dnn_ctr_ctl00_imgIcon")).Exists);
        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Linked_To_File_Already_Uploaded()
        {
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("File Page");
            ie.Button(Find.ById("dnn_ctr_ManageTabs_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_ctlURL_optType_3")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            ie.GoToDNNUrl("FilePage.aspx");

        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Linked_To_Another_Page()
        {
            //Arrange
            AddParentPageFromCP("Other Page");
            ie.GoToDNNUrl("default.aspx");
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Link Page");
            ie.Button(Find.ById("dnn_ctr_ManageTabs_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_ctlURL_optType_2")).Click();
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_ctlURL_cboTabs")).Select("Other Page");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.GoToDNNUrl("LinkPage.aspx");
            Assert.AreEqual("Other Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

        }

        [TestMethod]
        public void Add_Page_From_Pages_Module_Linked_To_Another_Page()
        {
            //Arrange
            AddParentPageFromPM("Other Page");
            //Act
            ie.Link(Find.ByTitle("Add New Page")).Click();
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_txtTabName")).TypeText("Link Page");
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.Button(Find.ById("dnn_ctr359_ManageTabs_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr359_ManageTabs_ctlURL_optType_2")).Click();
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_ctlURL_cboTabs")).Select("Other Page");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.GoToDNNUrl("LinkPage.aspx");
            Assert.AreEqual("Other Page", ie.Link(Find.ByClass("Breadcrumb")).Text);
        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Linked_To_URL()
        {
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Link Page");
            ie.Button(Find.ById("dnn_ctr_ManageTabs_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_ctlURL_optType_1")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_ctlURL_txtUrl")).TypeText("www.dotnetnuke.com");
            ie.Link(Find.ByTitle("Update")).Click();
            
            //Assert
            ie.GoToDNNUrl("LinkPage.aspx");
            ie.WaitForComplete();
            Assert.AreEqual("http://www.dotnetnuke.com/", ie.Url);

        }

        [TestMethod]
        public void Add_Page_From_Pages_Module_Linked_To_URL()
        {
            //Arrange
            ie.GoToDNNUrl("admin/Pages.aspx");
            ie.Link(Find.ByTitle("Add New Page")).Click();
            //Act
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_txtTabName")).TypeText("Link Page");
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.Button(Find.ById("dnn_ctr359_ManageTabs_dshAdvanced_imgIcon")).Click();
            ie.RadioButton(Find.ById("dnn_ctr359_ManageTabs_ctlURL_optType_1")).Click();
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_ctlURL_txtUrl")).TypeText("www.dotnetnuke.com");
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            ie.GoToDNNUrl("LinkPage.aspx");
            ie.WaitForComplete();
            Assert.AreEqual("http://www.dotnetnuke.com/", ie.Url);

        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_New_Copy_From_Page()
        {
            //Arrange
            AddParentPageFromCP("Original Page");
            ie.GoToDNNUrl("OriginalPage.aspx");
            AddTextModule("Module One");
            //Add text to the first module
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            //Add two more modules
            AddTextModule("Module Two");
            AddTextModule("Module Three");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Copy Page");
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboCopyPage")).Select("Original Page");
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module One"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Two"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Three"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Unique Text"));
        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Copy_From_Page()
        {
            //Arrange
            AddParentPageFromCP("Original Page");
            ie.GoToDNNUrl("OriginalPage.aspx");
            AddTextModule("Module One");
            //Add text to the first module
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            //Add two more modules
            AddTextModule("Module Two");
            AddTextModule("Module Three");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Copy Page");
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboCopyPage")).Select("Original Page");
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_grdModules_ctl02_optCopy")).Click();
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_grdModules_ctl03_optCopy")).Click();
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_grdModules_ctl04_optCopy")).Click();
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module One"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Two"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module Three"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Unique Text"));
        }

        [TestMethod]
        public void Add_Page_From_Control_Panel_Reference_Copy_From_Page()
        {
            //Arrange
            AddParentPageFromCP("Original Page");
            ie.GoToDNNUrl("OriginalPage.aspx");
            AddTextModule("Module One");
            //Add text to the first module
            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.RadioButton(Find.ById("dnn_ctr375_EditHTML_txtContent_optView_0")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Unique Text");
            ie.Link(Find.ByTitle("Save")).Click();
            
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdEditTab")).Click();
            //Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("Copy Page");
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboCopyPage")).Select("Original Page");
            ie.RadioButton(Find.ById("dnn_ctr_ManageTabs_grdModules_ctl02_optReference")).Click();

            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Module One"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Unique Text"));

            ie.Link(Find.ByTitle("Edit Content")).Click();
            ie.TextField(Find.ById("dnn_ctr375_EditHTML_txtContent_txtDesktopHTML")).TypeText("Different Text");
            ie.Link(Find.ByTitle("Save")).Click();
            ie.GoToDNNUrl("OriginalPage.aspx");
            Assert.IsTrue(ie.Element(Find.ById("dnn_ContentPane")).InnerHtml.Contains("Different Text"));
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

        private void AddParentPageFromPM(String pageName)
        {
            ie.GoToDNNUrl("admin/Pages.aspx");
            ie.Link(Find.ByTitle("Add New Page")).Click();
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_txtTabName")).TypeText(pageName);
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddChildPageFromPM(String pageName, String parentName)
        {
            ie.GoToDNNUrl("admin/Pages.aspx");
            ie.Link(Find.ByTitle("Add New Page")).Click();
            ie.TextField(Find.ById("dnn_ctr359_ManageTabs_txtTabName")).TypeText(pageName);
            ie.SelectList(Find.ById("dnn_ctr359_ManageTabs_cboParentTab")).Select(parentName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddParentPageFromCP(String pageName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddChildPageFromCP(String pageName, String parentName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select(parentName);
            ie.Link(Find.ByTitle("Update")).Click();
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
    }

}