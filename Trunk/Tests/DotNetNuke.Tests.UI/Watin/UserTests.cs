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

namespace WatinPageTests
{
    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestClass]
    public class UserTest
    {
        public static IE ie;

        

        public TestContext TestContext { get; set; }

        #region Additional test attributes

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            
        }


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
        public void Add_User()
        {
            //Add users to the website
            //Navigate to the users page
            //Arrange & Act
            AddUser("user1", "user1FN", "user1LN", "User1 DN", "email@address.com", "password");

            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr365_Users_UP")).InnerHtml.Contains("User1 DN"));
        
        }

        [TestMethod]
        public void Register_User()
        {
            //Arrange
            Logout();
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            
            //Act
            Register("register", "register", "account", "register account", "email@address.com", "password");

            //Assert
            Assert.AreEqual("register account", ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);
        }

      
        
        [TestMethod]
        public void Add_User_Fail()
        {
           //Arrange & Act
            AddUser(TestUsers.Admin.UserName, "admin", "admin", TestUsers.Admin.DisplayName, "admin@account.com", "dnnadmin");
            ie.WaitForComplete();

            //Assert
            Assert.IsTrue(ie.Image(Find.ByTitle("Error")).Exists);
        }

        [TestMethod]
        public void Delete_User()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "User1 DN", "email@address.com", "password");
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            //Act
            using (new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Button(Find.ByTitle("Delete")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            ie.WaitForComplete();
            //Assert
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr365_Users_UP")).InnerHtml.Contains("user1"));

        }

        [TestMethod]
        public void Unauthorize_User()
        {
            //Arrange
            AddUser("user1", "userFN", "userLN", "user DN", "email@address.com", "password1");
            
            //Act
            Table table = ie.Table(Find.ById("dnn_ctr365_Users_grdUsers"));
            Link editLink = null;
            Span span;
            foreach(TableRow row in table.TableRows)
            {
                span = row.TableCell(Find.ByIndex(4)).Span(Find.ByLabelText("Span"));
                if (span != null && span.Text == "user1")
                {
                    editLink = row.TableCell(Find.ByIndex(0)).Link(Find.ByTitle("Edit"));
                    break;
                }
            }
            editLink.Click();
            ie.Link(Find.ByTitle("UnAuthorize User")).Click();
            //Assert
            Assert.AreEqual("User successfully Un-Authorized", ie.Span(Find.ById("dnn_ctr365_ctl01_lblMessage")).Text);
        }

        [TestMethod]
        public void Delete_Unauthorised_Users()
        {
            String userName;
            //Arrange
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            for (int i = 0; i < 3; i++)
            {
                userName = "user" + (i+1);
                AddUser(userName, "Fname", "Lname", "Display Name", "email@address.com", "password");
            }
            //Act
            UnauthoriseUser("user1");
            UnauthoriseUser("user2");
            using (new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Link(Find.ByTitle("Delete Unauthorized Users")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            ie.WaitForComplete();
            //Assert
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr365_Users_UP")).InnerHtml.Contains("user1"));
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr365_Users_UP")).InnerHtml.Contains("user2"));

        }

        [TestMethod]
        public void User_Update_Password()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password1");
            Login("user1", "password1", "user1 DN");

            //Act
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Password")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtOldPassword")).TypeText("password1");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtNewPassword")).TypeText("newpassword");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtNewConfirm")).TypeText("newpassword");
            ie.Link(Find.ByTitle("Change Password")).Click();

            //Assert
            Assert.AreEqual("The password has been reset.", ie.Span(Find.ById("dnn_ctr_ctl01_lblMessage")).Text);
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            Login("user1", "newpassword", "user1 DN");
        }

        [TestMethod]
        public void Add_New_Visible_Profile_Field()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password1");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.CheckBox(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl07$Visible")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            Login("user1", "password1", "user1 DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.IsTrue(ie.Table(Find.ById("dnn_ctr_ManageUsers_Profile_ProfileProperties_tbl")).InnerHtml.Contains("Property1"));

        }

        [TestMethod]
        public void Add_New_Invisible_Profile_Field()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password1");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            Login("user1", "password1", "user1 DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.IsFalse(ie.Table(Find.ById("dnn_ctr_ManageUsers_Profile_ProfileProperties_tbl")).InnerHtml.Contains("Property1"));

        }

        [TestMethod]
        public void Add_Required_Profile_Field_Valid_Profile_Required()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password1");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.CheckBox(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl06$Required")).Click();
            ie.CheckBox(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl07$Visible")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            //Click the Login/Logout button
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            //Enter the users information
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText("user1");
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText("password1");
            //Click the Login
            ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();
            Assert.AreEqual("Please update your profile before continuing.", ie.Span(Find.ById("dnn_ctr_ctl00_lblMessage")).Text);
        }

        [TestMethod]
        public void Add_Required_Profile_Field_Valid_Profile_Not_Required()
        {
            //This is not working properly!!!!
            //Arrange
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.Link(Find.ByTitle("User Settings")).Click();
            ie.RadioButton(Find.ByName("dnn$ctr365$UserSettings$UserSettings$ctl23$Security_RequireValidProfileAtLogin")).Checked = false;
            ie.Link(Find.ByTitle("Update")).Click();
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password1");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.CheckBox(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl06$Required")).Click();
            ie.CheckBox(Find.ByName("dnn$ctr365$EditProfileDefinition$Wizard$Properties$ctl07$Visible")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr365_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            Login("user1", "password1", "user1 DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            ie.Link(Find.ByTitle("Update")).Click();
            Assert.AreEqual("Property1 is Required", ie.Span(Find.ById("dnn_ctr_ManageUsers_Profile_ProfileProperties_ctl01_required_Req")).Text);

        }

        [TestMethod]
        public void Force_Password_Change()
        {
            AddUser("newUser1", "newFN", "newLN", "new DN", "email@address.com", "newpassword");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText("newUser1");
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.WaitForComplete();
            //This link is not working right 
            ie.Div(Find.ById("dnn_ctr365_Users_UP")).Link(Find.ByTitle("Edit")).Click();
            ie.Link(Find.ByTitle("Force Password Change")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Success")).Exists);
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText("newUser1");
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText("newpassword");
            //Click the Login
            ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtOldPassword")).TypeText("newpassword");
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtNewPassword")).TypeText("password1");
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtNewConfirm")).TypeText("password1");
            ie.Link(Find.ByTitle("Change Password")).Click();
            Assert.AreEqual("new DN", ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);
        }

        [TestMethod]
        public void Redirect_On_Login()
        {
            //Arrange
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("New Login Page");
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select("<None Specified>");
            CheckBox cb = ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl03_ctl00"));
            cb.Checked = true;
            ie.Link(Find.ByTitle("Update")).Click();
            Assert.AreEqual("New Login Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

            //Act
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.Link(Find.ByTitle("User Settings")).Click();
            ie.SelectList(Find.ByName("dnn$ctr365$UserSettings$UserSettings$ctl15$Redirect_AfterLogin")).Select("New Login Page");
            ie.Link(Find.ByTitle("Update")).Click();
            
            //Assert
            Login("user1", "password", "user1 DN");
            Assert.AreEqual("New Login Page", ie.Link(Find.ByClass("Breadcrumb")).Text);
            
        }

        [TestMethod]
        public void Redirect_On_Registration()
        {
            //Arrange
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText("New Registration Page");
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select("<None Specified>");
            CheckBox cb = ie.CheckBox(Find.ById("dnn_ctr_ManageTabs_dgPermissions_ctl01_ctl03_ctl00"));
            cb.Checked = true;
            ie.Link(Find.ByTitle("Update")).Click();
            Assert.AreEqual("New Registration Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

            //Act
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.Link(Find.ByTitle("User Settings")).Click();
            ie.SelectList(Find.ByName("dnn$ctr365$UserSettings$UserSettings$ctl17$Redirect_AfterRegistration")).Select("New Registration Page");
            ie.Link(Find.ByTitle("Update")).Click();

            //Assert
            Logout();
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl00$Username")).TypeText("user1");
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl01$FirstName")).TypeText("user1FN");
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl02$LastName")).TypeText("user1LN");
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl03$DisplayName")).TypeText("user1 DN");
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl04$Email")).TypeText("email@address.com");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_User_txtPassword")).TypeText("password");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_User_txtConfirm")).TypeText("password");
            ie.Link(Find.ByTitle("Register")).Click();
            Assert.AreEqual("New Registration Page", ie.Link(Find.ByClass("Breadcrumb")).Text);

        }

        [TestMethod]
        public void Update_Profile_As_User()
        {
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            Login("user1", "password", "user1 DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$Profile$ProfileProperties$ctl06$Street")).TypeText("123 street");
            ie.Link(Find.ByTitle("Update")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.AreEqual("123 street", ie.TextField(Find.ByName("dnn$ctr$ManageUsers$Profile$ProfileProperties$ctl06$Street")).Text);

        }

        [TestMethod]
        public void Update_Profile_As_Admin()
        {
            AddUser("user1", "user1FN", "user1LN", "user1 DN", "email@address.com", "password");
            ie.GoToDNNUrl("admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText("user1");
            ie.Button(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.Div(Find.ById("dnn_ctr365_Users_UP")).Link(Find.ByTitle("Edit")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            ie.TextField(Find.ByName("dnn$ctr365$ManageUsers$Profile$ProfileProperties$ctl06$Street")).TypeText("123 Street");
            ie.Link(Find.ByTitle("Update")).Click();
            ie.Link(Find.ByTitle("Cancel")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr365_Users_UP")).InnerHtml.Contains("123 Street"));

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

        private static void UnauthoriseUser(String userName)
        {
            ie.GoToDNNUrl("Admin/UserAccounts.aspx");
            ie.TextField(Find.ById("dnn_ctr365_Users_txtSearch")).TypeText(userName);
            ie.Image(Find.ById("dnn_ctr365_Users_btnSearch")).Click();
            ie.WaitForComplete();
            ie.Div(Find.ById("dnn_ctr365_Users_UP")).Link(Find.ByTitle("Edit")).Click();
            ie.Link(Find.ByTitle("UnAuthorize User")).Click();
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

        private static void Register(String userName, String fName, String lName, String dName, String email, String password)
        {
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl00$Username")).TypeText(userName);
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl01$FirstName")).TypeText(fName);
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl02$LastName")).TypeText(lName);
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl03$DisplayName")).TypeText(dName);
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$User$UserEditor$ctl04$Email")).TypeText(email);
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_User_txtPassword")).TypeText(password);
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_User_txtConfirm")).TypeText(password);
            ie.Link(Find.ByTitle("Register")).Click();
            ie.Link(Find.ByTitle("Proceed to Login")).Click();
        }

        private void AddParentPageFromCP(String pageName)
        {
            //Arrange & Act
            ie.Link(Find.ById("dnn_IconBar.ascx_cmdAddTab")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageTabs_txtTabName")).TypeText(pageName);
            ie.SelectList(Find.ById("dnn_ctr_ManageTabs_cboParentTab")).Select("<None Specified>");
            ie.Link(Find.ByTitle("Update")).Click();
        }
    }
}

