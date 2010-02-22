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
    /// Summary description for AdminVendorsTests
    /// </summary>
    [TestClass]
    public class AdminVendorsTests
    {

        public static IE ie;
        public TestContext TestContext { get; set; }

        public AdminVendorsTests()
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
        public void Add_Vendor()
        {
            //Arrange & Act
            AddVendor("Company 1", "Fname", "Lname");
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));
        }

        [TestMethod]
        public void Add_Duplicate_Vendor_Fail()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            //Act
            ie.Link(Find.ByTitle("Add New Vendor")).Click();
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtVendorName")).TypeText("Company 1");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtFirstName")).TypeText("Fname");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtLastName")).TypeText("Lname");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtEmail")).TypeText("email@address.com");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_addresssVendor_txtStreet")).TypeText("123 street");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_addresssVendor_txtCity")).TypeText("My City");
            ie.SelectList(Find.ById("dnn_ctr366_EditVendors_addresssVendor_cboCountry")).Select("Canada");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Error")).Exists);
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_ctl01_lblMessage")).InnerHtml.Contains("There was an error adding the Vendor. Check the EventViewer for more information."));
        }

        [TestMethod]
        public void Edit_Vendor()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            //Act
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtVendorName")).TypeText("New Company");
            ie.Link(Find.ById("dnn_ctr366_EditVendors_cmdUpdate")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("New Company"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));
        }

        [TestMethod]
        public void Delete_Vendor()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            //Act
            using (new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Link(Find.ByTitle("Delete")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            //Assert
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));
        }

        [TestMethod]
        public void Edit_Vendor_Adding_Text_Banner()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            //Act
            ie.Button(Find.ById("dnn_ctr366_EditVendors_dshBanners_imgIcon")).Click();
            ie.Link(Find.ById("dnn_ctr366_EditVendors_/Banners.ascx_cmdAdd")).Click();
            ie.TextField(Find.ById("dnn_ctr366_EditBanner_txtBannerName")).TypeText("Banner 1");
            ie.SelectList(Find.ById("dnn_ctr366_EditBanner_cboBannerType")).Select("Text");
            ie.TextField(Find.ById("dnn_ctr366_EditBanner_txtDescription")).TypeText("This banner is for testing.");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.Button(Find.ById("dnn_ctr366_EditVendors_dshBanners_imgIcon")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_EditVendors_divBanners")).InnerHtml.Contains("Banner 1"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_EditVendors_divBanners")).InnerHtml.Contains("Text"));
        }

        [TestMethod]
        public void Editing_Vendors_Unauthorize_Vendor()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            //Act
            UnauthorizeVendor();
            //Assert
            ie.Link(Find.ById("dnn_ctr366_Vendors_rptLetterSearch_ctl27_Hyperlink2")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));

        }


        [TestMethod]
        public void Delete_Unauthorized_Vendors()
        {
            //Arrange
            AddVendor("Company 1", "fname", "lname");
            AddVendor("Company 2", "fname", "lname");
            UnauthorizeVendor();
            AddVendor("Company 3", "fname", "lname");
            UnauthorizeVendor();
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            //Act
            using (new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Link(Find.ByTitle("Delete Unauthorized Vendors")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            //Assert
            ie.Link(Find.ById("dnn_ctr366_Vendors_rptLetterSearch_ctl26_Hyperlink2")).Click();
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 2"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 3"));
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));

        }


        [TestMethod]
        public void Editing_Vendors_Authorize_Vendor()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            UnauthorizeVendor();
            //Act
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_chkAuthorized")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.Link(Find.ById("dnn_ctr366_Vendors_rptLetterSearch_ctl27_Hyperlink2")).Click();
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));
        }

        [TestMethod]
        public void Editing_Vendors_Add_Image_Banner()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            ie.SelectList(Find.ById("dnn_ctr366_EditVendors_ctlLogo_cboFiles")).Select("365-Button.jpg");
            ie.Link(Find.ById("dnn_ctr366_EditVendors_cmdUpdate")).Click();
            //Act
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            ie.Button(Find.ById("dnn_ctr366_EditVendors_dshBanners_imgIcon")).Click();
            ie.Link(Find.ById("dnn_ctr366_EditVendors_/Banners.ascx_cmdAdd")).Click();
            ie.TextField(Find.ById("dnn_ctr366_EditBanner_txtBannerName")).TypeText("Image Banner 1");
            ie.SelectList(Find.ById("dnn_ctr366_EditBanner_cboBannerType")).Select("Banner");
            ie.SelectList(Find.ById("dnn_ctr366_EditBanner_ctlImage_cboFiles")).Select("365-Button.jpg");
            ie.TextField(Find.ById("dnn_ctr366_EditBanner_txtDescription")).TypeText("This banner is for testing.");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.Button(Find.ById("dnn_ctr366_EditVendors_dshBanners_imgIcon")).Click();
            Assert.IsTrue(ie.Table(Find.ById("dnn_ctr366_EditVendors_/Banners.ascx_grdBanners")).InnerHtml.Contains("Image Banner 1"));
            Assert.IsTrue(ie.Table(Find.ById("dnn_ctr366_EditVendors_/Banners.ascx_grdBanners")).InnerHtml.Contains("Banner"));
        }

        [TestMethod]
        public void Search_Vendors()
        {
            //Arrange
            AddVendor("Company 1", "Fname", "Lname");
            AddVendor("Company 2", "Fname", "Lname");
            AddVendor("Company 3", "Fname", "Lname");
            //Act
            ie.TextField(Find.ById("dnn_ctr366_Vendors_txtSearch")).TypeText("Company 2");
            ie.Button(Find.ById("dnn_ctr366_Vendors_btnSearch")).Click();
            //ie.WaitForComplete();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 2"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 1"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr366_Vendors_UP")).InnerHtml.Contains("Company 3"));

        }

        private void UnauthorizeVendor()
        {
            ie.Link(Find.ById("dnn_ctr366_Vendors_grdVendors_ctl02_Hyperlink1")).Click();
            ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_chkAuthorized")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
        }

        private void AddVendor(String company, String fName, String lName)
        {
            ie.GoToDNNUrl("Admin/Vendors.aspx");
            ie.Link(Find.ByTitle("Add New Vendor")).Click();
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtVendorName")).TypeText(company);
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtFirstName")).TypeText(fName);
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtLastName")).TypeText(lName);
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtEmail")).TypeText("email@address.com");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_addresssVendor_txtStreet")).TypeText("123 street");
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_addresssVendor_txtCity")).TypeText("My City");
            ie.SelectList(Find.ById("dnn_ctr366_EditVendors_addresssVendor_cboCountry")).Select("Canada");
            if (ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkRegion")).Checked == true)
            {
                ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkRegion")).Click();
            }
            if (ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkPostal")).Checked == true)
            {
                ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkPostal")).Click();
            }
            if (ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkTelephone")).Checked == true)
            {
                ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkTelephone")).Click();
            }
            if (ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkCell")).Checked == true)
            {
                ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkCell")).Click();
            }
            if (ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkFax")).Checked == true)
            {
                ie.CheckBox(Find.ById("dnn_ctr366_EditVendors_addresssVendor_chkFax")).Click();
            }
            ie.TextField(Find.ById("dnn_ctr366_EditVendors_txtWebsite")).TypeText("www.dotnetnuke.com");
            ie.Link(Find.ByTitle("Update")).Click();
            ie.Link(Find.ByTitle("Cancel")).Click();

        }

        private static void Logout()
        {
            if (ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text != "Register")
            {
                ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            }
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
