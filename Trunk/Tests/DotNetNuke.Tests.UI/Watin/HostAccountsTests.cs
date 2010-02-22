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
    /// Summary description for HostAccountsTests
    /// </summary>
    [TestClass]
    public class HostAccountsTests
    {
        public static IE ie;
        public TestContext TestContext { get; set; }

        public HostAccountsTests()
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
        public void Add_Host_User()
        {
            //Arrange & Act
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            //Assert
            Assert.IsTrue(ie.Element(Find.ByClass("c_content")).InnerHtml.Contains("host2"));

        }

        [TestMethod]
        public void Delete_Host_User()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            ConfirmDialogHandler dialog = new ConfirmDialogHandler();
            //Act
            using (new UseDialogOnce(ie.DialogWatcher, dialog))
            {
                ie.Button(Find.ByTitle("Delete")).ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }
            ie.WaitForComplete();
            //Assert
            Assert.IsFalse(ie.Element(Find.ById("dnn_ctr343_Users_UP")).InnerHtml.Contains("host2"));

        }

        [TestMethod]
        public void Unauthorise_Host_User()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@addresss.com", "password");
            ie.TextField(Find.ById("dnn_ctr343_Users_txtSearch")).TypeText("host2");
            ie.Button(Find.ById("dnn_ctr343_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Edit")).Click();
            //Act
            ie.Link(Find.ByTitle("UnAuthorize User")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr343_ManageUsers_UP")).InnerHtml.Contains("User successfully Un-Authorized"));
            ie.Link(Find.ByTitle("Cancel")).Click();
            ie.Link(Find.ByText("Unauthorized")).Click();
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr343_Users_UP")).InnerHtml.Contains("host2"));
        }

        [TestMethod]
        public void Update_Password()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            Login("host2", "password", "Host DN");
            //Act
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Password")).Click();
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtOldPassword")).TypeText("password");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtNewPassword")).TypeText("newpassword");
            ie.TextField(Find.ById("dnn_ctr_ManageUsers_Password_txtNewConfirm")).TypeText("newpassword");
            ie.Link(Find.ByTitle("Change Password")).Click();
            //Assert
            Assert.AreEqual("The password has been reset.", ie.Span(Find.ById("dnn_ctr_ctl01_lblMessage")).Text);
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            Login("host2", "newpassword", "Host DN");
        }

        [TestMethod]
        public void Force_Password_Change()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            ie.TextField(Find.ById("dnn_ctr343_Users_txtSearch")).TypeText("host2");
            ie.Button(Find.ById("dnn_ctr343_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Edit")).Click();
            //Act
            ie.Link(Find.ByTitle("Force Password Change")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Success")).Exists);
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            ie.Link(Find.ById("dnn_dnnLOGIN_cmdLogin")).Click();
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtUsername")).TypeText("host2");
            ie.TextField(Find.ById("dnn_ctr_Login_Login_DNN_txtPassword")).TypeText("password");
            //Click the Login
            ie.Button(Find.ById("dnn_ctr_Login_Login_DNN_cmdLogin")).Click();
            Assert.IsTrue(ie.Image(Find.ByTitle("Warning")).Exists);
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtOldPassword")).TypeText("password");
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtNewPassword")).TypeText("newpassword");
            ie.TextField(Find.ById("dnn_ctr_Login_Password_txtNewConfirm")).TypeText("newpassword");
            ie.Link(Find.ByTitle("Change Password")).Click();
            Assert.AreEqual("Host DN", ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Text);
        }

        [TestMethod]
        public void Update_Profile_As_Host_User()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            Login("host2", "password", "Host DN");
            //Act
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$Profile$ProfileProperties$ctl06$Street")).TypeText("123 street");
            ie.Link(Find.ByTitle("Update")).Click();
            //Assert
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.AreEqual("123 street", ie.TextField(Find.ByName("dnn$ctr$ManageUsers$Profile$ProfileProperties$ctl06$Street")).Text);
        }

        [TestMethod]
        public void Update_Profile_As_Host()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            ie.TextField(Find.ById("dnn_ctr343_Users_txtSearch")).TypeText("host2");
            ie.Button(Find.ById("dnn_ctr343_Users_btnSearch")).Click();
            ie.Link(Find.ByTitle("Edit")).Click();
            //Act
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            ie.TextField(Find.ByName("dnn$ctr$ManageUsers$Profile$ProfileProperties$ctl06$Street")).TypeText("123 Street");
            ie.Link(Find.ByTitle("Update")).Click();
            ie.Link(Find.ByTitle("Cancel")).Click();
            //Assert
            Assert.IsTrue(ie.Element(Find.ById("dnn_ctr343_Users_UP")).InnerHtml.Contains("123 Street"));
        }

        [TestMethod]
        public void Add_New_Visible_Profile_Field()
        {
             //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.CheckBox(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl07$Visible")).Click();
            ie.Link(Find.ById("dnn_ctr343_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr343_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            Login("host2", "password", "Host DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.IsTrue(ie.Table(Find.ById("dnn_ctr_ManageUsers_Profile_ProfileProperties_tbl")).InnerHtml.Contains("Property1"));

        }

        [TestMethod]
        public void Add_New_Invisible_Profile_Field()
        {
            //Arrange
            AddHostUser("host2", "hostFN", "hostLN", "Host DN", "email@address.com", "password");
            ie.Link(Find.ByTitle("Manage Profile Properties")).Click();
            ie.Link(Find.ByTitle("Add New Profile Property")).Click();

            //Act
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl00$PropertyName")).TypeText("Property1");
            ie.SelectList(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl01$DataType")).Select("Text");
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl02$PropertyCategory")).TypeText("Category");
            ie.TextField(Find.ByName("dnn$ctr343$EditProfileDefinition$Wizard$Properties$ctl03$Length")).TypeText("15");
            ie.Link(Find.ById("dnn_ctr343_EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_StartNextLinkButton")).Click();
            ie.Link(Find.ById("dnn_ctr343_EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_FinishLinkButton")).Click();
            Logout();

            //Assert
            Login("host2", "password", "Host DN");
            ie.Link(Find.ById("dnn_dnnUSER_cmdRegister")).Click();
            ie.Link(Find.ByTitle("Manage Profile")).Click();
            Assert.IsFalse(ie.Table(Find.ById("dnn_ctr_ManageUsers_Profile_ProfileProperties_tbl")).InnerHtml.Contains("Property1"));

        }

        private void AddHostUser(String userName, String fName, String lName, String dName, String email, String password)
        {
            ie.GoToDNNUrl("tabid/34/portalid/0/Default.aspx");
            ie.Link(Find.ByTitle("Add New User")).Click();
            //Act
            ie.TextField(Find.ByName("dnn$ctr343$ManageUsers$User$UserEditor$ctl00$Username")).TypeText(userName);
            ie.TextField(Find.ByName("dnn$ctr343$ManageUsers$User$UserEditor$ctl01$FirstName")).TypeText(fName);
            ie.TextField(Find.ByName("dnn$ctr343$ManageUsers$User$UserEditor$ctl02$LastName")).TypeText(lName);
            ie.TextField(Find.ByName("dnn$ctr343$ManageUsers$User$UserEditor$ctl03$DisplayName")).TypeText(dName);
            ie.TextField(Find.ByName("dnn$ctr343$ManageUsers$User$UserEditor$ctl04$Email")).TypeText(email);
            ie.TextField(Find.ById("dnn_ctr343_ManageUsers_User_txtPassword")).TypeText(password);
            ie.TextField(Find.ById("dnn_ctr343_ManageUsers_User_txtConfirm")).TypeText(password);
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
