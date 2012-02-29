#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Exceptions;
using DotNetNuke.Services.Social.Messaging.Ineternal;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using Moq;

using NUnit.Framework;

namespace DotNetNuke.Tests.Core.Controllers
{
	/// <summary>
    ///  Testing various aspects of RelationshipController
	/// </summary>
	[TestFixture]
	public class MessagingControllerTests
	{
		#region "Private Properties"

        private Mock<IDataService> _mockDataService;		
        private MessagingControllerImpl _messagingController;
        private Mock<DataProvider> _dataProvider;
        private Mock<IPortalSettings> _portalSettingsWrapper;
        private Mock<RoleProvider> _mockRoleProvider;
	    private Mock<CachingProvider> _mockCacheProvider;

        private DataTable _dtMessages;
        private DataTable _dtMessageAttachment;
        private DataTable _dtMessageRecipients;
        private DataTable _dtPortalSettings;
        private UserInfo _adminUserInfo;
        private UserInfo _hostUserInfo;
        private UserInfo _user12UserInfo;

		#endregion

		#region "Set Up"

		[SetUp]
		public void SetUp()
		{
			ComponentFactory.Container = new SimpleContainer();
            _mockDataService = new Mock<IDataService>();
            _dataProvider = MockComponentProvider.CreateDataProvider();
		    _mockRoleProvider = MockComponentProvider.CreateRoleProvider();
            _mockCacheProvider = MockComponentProvider.CreateDataCacheProvider();
			MockComponentProvider.CreateEventLogController();

            _messagingController = new MessagingControllerImpl(_mockDataService.Object);            

            _portalSettingsWrapper = new Mock<IPortalSettings>();
            TestablePortalSettings.RegisterInstance(_portalSettingsWrapper.Object);

			SetupDataProvider();
            SetupRoleProvider();
		    SetupDataTables();
            SetupUsers();
		    SetupPortalSettingsWrapper();
		}


        private void SetupDataProvider()
        {
            //Standard DataProvider Path for Logging
            _dataProvider.Setup(d => d.GetProviderPath()).Returns("");                    
        }

        private void SetupUsers()
        {
            _adminUserInfo = new UserInfo { DisplayName = Constants.UserDisplayName_Admin, UserID = Constants.UserID_Admin };
            _hostUserInfo = new UserInfo { DisplayName = Constants.UserDisplayName_Host, UserID = Constants.UserID_Host, IsSuperUser = true};
            _user12UserInfo = new UserInfo { DisplayName = Constants.UserDisplayName_User12, UserID = Constants.UserID_User12 };
        }

        private void SetupPortalSettingsWrapper()
        {            
            _portalSettingsWrapper.Setup(ps => ps.AdministratorRoleName).Returns(Constants.RoleName_Administrators);
        }

        private void SetupRoleProvider()
        {
            var adminRoleInfo = new UserRoleInfo { RoleName = Constants.RoleName_Administrators, RoleID = Constants.RoleID_Administrators, UserID = Constants.UserID_Admin};
            var user12RoleInfo = new UserRoleInfo { RoleName = Constants.RoleName_RegisteredUsers, RoleID = Constants.RoleID_RegisteredUsers, UserID = Constants.UserID_User12 };

            _mockRoleProvider.Setup(rp => rp.GetUserRoles(It.Is<UserInfo>(u => u.UserID == Constants.UserID_Admin), It.IsAny<bool>())).Returns(new List<UserRoleInfo> {adminRoleInfo});
            _mockRoleProvider.Setup(rp => rp.GetUserRoles(It.Is<UserInfo>(u => u.UserID == Constants.UserID_User12), It.IsAny<bool>())).Returns(new List<UserRoleInfo> { user12RoleInfo });            
        }


		#endregion

        #region Constructor Tests

        [Test]
        public void MessagingController_Constructor_Throws_On_Null_DataService()
        {
            //Arrange            

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new MessagingControllerImpl(null));
        }

        #endregion

        #region Easy Wrapper APIs Tests

        #region CreateMessageTests

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Null_Body_And_Subject()
        {
            //Act, Assert
            _messagingController.CreateMessage(null, null, null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Null_Roles_And_Users()
        {
            //Act, Assert
            _messagingController.CreateMessage("subject", "body", null, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Empty_Roles_And_Users_Lists()
        {
            //Act, Assert
            _messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo>(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Roles_And_Users_With_No_DisplayNames()
        {
            //Act, Assert
            _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { new RoleInfo() }, new List<UserInfo> { new UserInfo() }, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Large_Subject()
        {
            //Arrange
            StringBuilder subject = new StringBuilder();
            for (int i = 0; i <= 40; i++)
            {
                subject.Append("1234567890");
            }

            //Act, Assert
            _messagingController.CreateMessage(subject.ToString(), "body", new List<RoleInfo> { new RoleInfo() }, new List<UserInfo> { new UserInfo() }, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Large_To()
        {
            //Arrange
            StringBuilder subject = new StringBuilder();
            for (int i = 0; i <= 40; i++)
            {
                subject.Append("1234567890");
            }
            
            var roles = new List<RoleInfo>();
            for (int i = 0; i <= 100; i++)
            {                
                roles.Add(new RoleInfo() { RoleName = "1234567890" });
            }

            var users = new List<UserInfo>();
            for (int i = 0; i <= 100; i++)
            {
                users.Add(new UserInfo() { DisplayName = "1234567890" });
            }

            //Act, Assert
            _messagingController.CreateMessage("subject", "body", roles, users, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Null_Sender()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };

            //Act
            _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Negative_SenderID()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };
            var sender = new UserInfo { DisplayName = "user11"};

            //Act
            _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, sender);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_CreateMessage_Thorws_On_SendingToRole_ByNonAdmin()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };
            var role = new RoleInfo { RoleName = Constants.RoleName_RegisteredUsers, RoleID = Constants.RoleID_RegisteredUsers };

            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _user12UserInfo);
        }

        //[Test]
        [ExpectedException(typeof(AttachmentsNotAllowed))]
        public void MessagingController_CreateMessage_Thorws_On_Passing_Attachments_When_Its_Not_Enabled()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };            

            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            //disable caching
            _mockCacheProvider.Setup(mc => mc.GetItem(It.IsAny<string>())).Returns(null);

            _dtPortalSettings.Clear();
            _dtPortalSettings.Rows.Add(Constants.PORTALSETTING_MessagingAllowAttachments_Name, Constants.PORTALSETTING_MessagingAllowAttachments_Value_NO, Constants.CULTURE_EN_US);
            _dataProvider.Setup(d => d.GetPortalSettings(It.IsAny<int>(), It.IsAny<string>())).Returns(_dtPortalSettings.CreateDataReader());

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", null, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _user12UserInfo);
        }


        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessage_On_Valid_Message()
        {   
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };            
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, _adminUserInfo);

            //Assert
            mockDataService.Verify(ds => ds.SaveSocialMessage(It.Is<Message>(v => v.Subject == "subject"
                                                                && v.Body == "body"
                                                                && v.To == "role1,user1"
                                                                && v.SenderUserID == _adminUserInfo.UserID)
                                                               , It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_For_CommonUser_Calls_DataService_SaveSocialMessageRecipient_Then_CreateSocialMessageRecipientsForRole()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };            
            
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //this pattern is based on: http://dpwhelan.com/blog/software-development/moq-sequences/
            var callingSequence = 0;

            //Arrange for Assert
            mockDataService.Setup(ds => ds.CreateSocialMessageRecipientsForRole(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Callback(() => Assert.That(callingSequence++, Is.EqualTo(0)));
            mockDataService.Setup(ds => ds.SaveSocialMessageRecipient(It.IsAny<MessageRecipient>(), It.IsAny<int>())).Callback(() => Assert.That(callingSequence++, Is.EqualTo(1)));            

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, _adminUserInfo);

            //SaveSocialMessageRecipient is called twice, one for sent message and second for receive                        
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_One_User()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.USER_TenName, UserID = Constants.USER_TenId };
            var sender = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user }, null, sender);

            //Assert
            Assert.AreEqual(message.To, Constants.USER_TenName);            
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_Two_Users()
        {
            //Arrange
            var user10 = new UserInfo { DisplayName = Constants.USER_TenName, UserID = Constants.USER_TenId };
            var user11 = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };
            var sender = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            var recipientId = 0;
            //_dtMessageRecipients.Rows.Add(Constants.SocialMessaging_RecipientId_2, Constants.USER_Null, Constants.SocialMessaging_UnReadMessage, Constants.SocialMessaging_UnArchivedMessage);

            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>()))                 
                .Callback(() => _dtMessageRecipients.Rows.Add(recipientId++, Constants.USER_Null, Constants.SocialMessaging_UnReadMessage, Constants.SocialMessaging_UnArchivedMessage))
                .Returns(() => _dtMessageRecipients.CreateDataReader());            

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user10, user11 }, null, sender);

            //Assert
            Assert.AreEqual(message.To, Constants.USER_TenName + "," + Constants.USER_ElevenName);
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_One_Role()
        {
            //Arrange
            var role = new RoleInfo { RoleName = Constants.RoleName_Administrators };            

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo>(), null, _adminUserInfo);

            //Assert
            Assert.AreEqual(message.To, Constants.RoleName_Administrators);
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_Two_Roles()
        {
            //Arrange
            var role1 = new RoleInfo { RoleName = Constants.RoleName_Administrators };
            var role2 = new RoleInfo { RoleName = Constants.RoleName_Subscribers };            

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role1, role2 }, new List<UserInfo>(), null, _adminUserInfo);

            //Assert
            Assert.AreEqual(message.To, Constants.RoleName_Administrators + "," + Constants.RoleName_Subscribers);
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessageAttachment_On_Passing_Attachments()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId};
            var role = new RoleInfo { RoleName = "role1" };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());


            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _adminUserInfo);

            //Assert
            mockDataService.Verify(ds => ds.SaveSocialMessageAttachment(It.Is<MessageAttachment>(v => v.MessageID == message.MessageID), It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_CreateSocialMessageRecipientsForRole_On_Passing_Role_ByAdmin()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.UserDisplayName_Admin, UserID = Constants.UserID_Admin };
            var role = new RoleInfo { RoleName = Constants.RoleName_RegisteredUsers, RoleID = Constants.RoleID_RegisteredUsers };
            
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _adminUserInfo);

            //Assert
            mockDataService.Verify(ds => ds.CreateSocialMessageRecipientsForRole(message.MessageID, Constants.RoleID_RegisteredUsers.ToString(), It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_CreateSocialMessageRecipientsForRole_On_Passing_Role_ByHost()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.UserDisplayName_Admin, UserID = Constants.UserID_Admin };
            var role = new RoleInfo { RoleName = Constants.RoleName_RegisteredUsers, RoleID = Constants.RoleID_RegisteredUsers };

            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _hostUserInfo);

            //Assert
            mockDataService.Verify(ds => ds.CreateSocialMessageRecipientsForRole(message.MessageID, Constants.RoleID_RegisteredUsers.ToString(), It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_CreateSocialMessageRecipientsForRole_On_Passing_Roles()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role1 = new RoleInfo { RoleName = "role1", RoleID = Constants.RoleID_RegisteredUsers };
            var role2 = new RoleInfo { RoleName = "role2", RoleID = Constants.RoleID_Administrators };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role1, role2 }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _adminUserInfo);

            //Assert
            mockDataService.Verify(ds => ds.CreateSocialMessageRecipientsForRole(message.MessageID, Constants.RoleID_RegisteredUsers + "," + Constants.RoleID_Administrators, It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessageRecipient_On_Passing_Users()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();

            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, _adminUserInfo);         

            //Assert
            mockDataService.Verify(ds => ds.SaveSocialMessageRecipient(It.Is<MessageRecipient>(v => v.MessageID == message.MessageID && v.UserID == user.UserID), It.IsAny<int>()));            
        }

        [Test]
        public void MessagingController_CreateMessage_Sets_ReplyAll_To_False_On_Passing_Roles()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1", RoleID = Constants.RoleID_RegisteredUsers };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, _adminUserInfo);

            //Assert
            Assert.AreEqual(message.ReplyAllAllowed, false);            
        }

        [Test]
        public void MessagingController_CreateMessage_Sets_ReplyAll_To_True_On_Passing_User()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var mockDataService = new Mock<IDataService>();
            var messagingController = new MessagingControllerImpl(mockDataService.Object);

            _dtMessageRecipients.Clear();
            mockDataService.Setup(md => md.GetSocialMessageRecipientByMessageAndUser(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtMessageRecipients.CreateDataReader());

            //Act
            var message = messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user }, new List<int> { Constants.FOLDER_ValidFileId }, user);
            
            //Assert
            Assert.AreEqual(message.ReplyAllAllowed, true);
        }

        #endregion

        #region ReplyMessageTests

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_ReplyMessage_Throws_On_Null_Sender()
        {
            //Arrange

            //Act
            _messagingController.ReplyMessage(Constants.SocialMessaging_MessageId_1, "body", null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_ReplyMessage_Throws_On_Negative_SenderID()
        {
            //Arrange
            var sender = new UserInfo { DisplayName = "user11" };

            //Act            
            _messagingController.ReplyMessage(Constants.SocialMessaging_MessageId_1, "body", null, sender);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_ReplyMessage_Throws_On_Null_Subject()
        {
            //Arrange
            var sender = new UserInfo { DisplayName = "user11", UserID = Constants.USER_TenId };

            //Act, Assert
            _messagingController.ReplyMessage(Constants.SocialMessaging_MessageId_1, null, null, sender);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingController_ReplyMessage_Throws_On_Empty_Subject()
        {
            //Arrange
            var sender = new UserInfo { DisplayName = "user11", UserID = Constants.USER_TenId };

            //Act, Assert
            _messagingController.ReplyMessage(Constants.SocialMessaging_MessageId_1, "", null, sender);
        }

        [Test]
        [ExpectedException(typeof(AttachmentsNotAllowed))]
        public void MessagingController_ReplyMessage_Throws_On_Passing_Attachments_When_Its_Not_Enabled()
        {
            //Arrange                        
            var sender = new UserInfo { DisplayName = "user11", UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            
            _dtPortalSettings.Clear();
            _dtPortalSettings.Rows.Add(Constants.PORTALSETTING_MessagingAllowAttachments_Name, Constants.PORTALSETTING_MessagingAllowAttachments_Value_NO, Constants.CULTURE_EN_US);
            _dataProvider.Setup(d => d.GetPortalSettings(It.IsAny<int>(), It.IsAny<string>())).Returns(_dtPortalSettings.CreateDataReader());

            //Act, Assert
            _messagingController.ReplyMessage(Constants.SocialMessaging_MessageId_1, "body", new List<int> { Constants.FOLDER_ValidFileId }, sender);
        }

        #endregion

        #region Setting Message Status Tests

        [Test]
        public void MessagingController_SetReadMessage_Calls_DataService_UpdateSocialMessageReadStatus()
        {
            //Arrange
            var messageInstance = CreateValidMessage();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };    

            //Act
            _messagingController.MarkRead(messageInstance.ConversationId, user.UserID);

            //Assert
           _mockDataService.Verify(ds => ds.UpdateSocialMessageReadStatus(messageInstance.ConversationId, user.UserID, true));
        }

        [Test]
        public void MessagingController_SetUnReadMessage_Calls_DataService_UpdateSocialMessageReadStatus()
        {
            //Arrange
            var messageInstance = CreateValidMessage();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId }; 

            //Act
            _messagingController.MarkUnRead(messageInstance.ConversationId, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageReadStatus(messageInstance.ConversationId, user.UserID, false));                        
        }

        [Test]
        public void MessagingController_SetArchivedMessage_Calls_DataService_UpdateSocialMessageArchivedStatus()
        {
            //Arrange
            var messageInstance = CreateValidMessage();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId }; 

            //Act
            _messagingController.MarkArchived(messageInstance.ConversationId, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageArchivedStatus(messageInstance.ConversationId, user.UserID, true));                       
        }

        [Test]
        public void MessagingController_SetUnArchivedMessage_Calls_DataService_UpdateSocialMessageArchivedStatus()
        {
            //Arrange
            var messageInstance = CreateValidMessage();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };

            //Act
            _messagingController.MarkUnArchived(messageInstance.ConversationId, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageArchivedStatus(messageInstance.ConversationId, user.UserID, false));
        }   

        #endregion

        
        #endregion


        #region "Private Methods"

        private static Message CreateValidMessage()
        {
            var message = new Message
            {
                MessageID = 2,
                Subject  ="test",
                Body="body",
                ConversationId = 1,
                ReplyAllAllowed = false,
                SenderUserID =1
            };
            return message;
        }

        private static MessageRecipient CreateValidUnReadMessageRecipient()
        {
            var messageRecipient = new MessageRecipient
            {
                RecipientID  =1,
                MessageID = 1,
                UserID = 1,
                Read = false,
                Archived = false
            };
            return messageRecipient;
        }

        private void SetupDataTables()
        {
            //Messages
            _dtMessages = new DataTable("Messages");
            var pkMessagesMessageID = _dtMessages.Columns.Add("MessageID", typeof(int));
            _dtMessages.Columns.Add("To", typeof(string));
            _dtMessages.Columns.Add("Subject", typeof(string));
            _dtMessages.Columns.Add("Body", typeof(string));
            _dtMessages.Columns.Add("ConversationId", typeof(int));
            _dtMessages.Columns.Add("ReplyAllAllowed", typeof(bool));
            _dtMessages.Columns.Add("SenderUserID", typeof(int));
            _dtMessages.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessages.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessages.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessages.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessages.PrimaryKey = new[] { pkMessagesMessageID };

            //MessageRecipients
            _dtMessageRecipients = new DataTable("MessageRecipients");
            var pkMessageRecipientID = _dtMessageRecipients.Columns.Add("RecipientID", typeof(int));
            _dtMessageRecipients.Columns.Add("MessageID", typeof(int));
            _dtMessageRecipients.Columns.Add("UserID", typeof(int));
            _dtMessageRecipients.Columns.Add("Read", typeof(bool));
            _dtMessageRecipients.Columns.Add("Archived", typeof(bool));            
            _dtMessageRecipients.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessageRecipients.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessageRecipients.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessageRecipients.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessageRecipients.PrimaryKey = new[] { pkMessageRecipientID };


            //MessageAttachments
            _dtMessageAttachment = new DataTable("MessageAttachments");
            var pkMessageAttachmentID = _dtMessageAttachment.Columns.Add("MessageAttachmentID", typeof(int));
            _dtMessageAttachment.Columns.Add("MessageID", typeof(int));
            _dtMessageAttachment.Columns.Add("FileID", typeof(int));
            _dtMessageAttachment.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessageAttachment.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessageAttachment.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessageAttachment.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessageAttachment.PrimaryKey = new[] { pkMessageAttachmentID };
            
             //Portal Settings
            _dtPortalSettings = new DataTable("PortalSettings");    
            _dtPortalSettings.Columns.Add("SettingName", typeof(string));
            _dtPortalSettings.Columns.Add("SettingValue", typeof(string));
            _dtPortalSettings.Columns.Add("CultureCode", typeof(string));
            _dtPortalSettings.Columns.Add("CreatedByUserID", typeof(int));
            _dtPortalSettings.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtPortalSettings.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtPortalSettings.Columns.Add("LastModifiedOnDate", typeof(DateTime));
        }

        #endregion
    }
}