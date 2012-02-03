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

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging.Data;
using DotNetNuke.Services.Social.Messaging;
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
        private MessagingController _messagingController;
        private Mock<DataProvider> _dataProvider;

        private DataTable _dtMessages;
        private DataTable _dtMessageAttachment;
        private DataTable _dtMessageRecipients;

		#endregion

		#region "Set Up"

		[SetUp]
		public void SetUp()
		{
			ComponentFactory.Container = new SimpleContainer();
            _mockDataService = new Mock<IDataService>();
            _dataProvider = MockComponentProvider.CreateDataProvider();
			MockComponentProvider.CreateDataCacheProvider();
			MockComponentProvider.CreateEventLogController();

            _messagingController = new MessagingController(_mockDataService.Object);

			SetupDataProvider();
		  //  SetupDataTables();
		}


        private void SetupDataProvider()
        {
            //Standard DataProvider Path for Logging
            _dataProvider.Setup(d => d.GetProviderPath()).Returns("");                    
        }

		#endregion

        #region Constructor Tests

        [Test]
        public void MessagingController_Constructor_Throws_On_Null_DataService()
        {
            //Arrange            

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new MessagingController(null));
        }

        #endregion

        #region Easy Wrapper APIs Tests

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
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessage_On_Valid_Message()
        {   
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };
            var sender = new UserInfo { DisplayName = "user11", UserID = Constants.USER_ElevenId };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, null, sender);

            //Assert
            _mockDataService.Verify(ds => ds.SaveSocialMessage(It.Is<Message>(v => v.Subject == "subject"
                                                                && v.Body == "body"
                                                                && v.To == "role1,user1"
                                                                && v.SenderUserID == sender.UserID)
                                                               , It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_One_User()
        {
            //Arrange
            var user = new UserInfo { DisplayName = Constants.USER_TenName, UserID = Constants.USER_TenId };
            var sender = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user }, null, sender);

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

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user10, user11 }, null, sender);

            //Assert
            Assert.AreEqual(message.To, Constants.USER_TenName + "," + Constants.USER_ElevenName);
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_One_Role()
        {
            //Arrange
            var role = new RoleInfo { RoleName = Constants.RoleName_Administrators };
            var sender = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo>{role}, new List<UserInfo>(), null, sender);

            //Assert
            Assert.AreEqual(message.To, Constants.RoleName_Administrators);
        }

        [Test]
        public void MessagingController_CreateMessage_Trims_Comma_For_Two_Roles()
        {
            //Arrange
            var role1 = new RoleInfo { RoleName = Constants.RoleName_Administrators };
            var role2 = new RoleInfo { RoleName = Constants.RoleName_Subscribers };
            var sender = new UserInfo { DisplayName = Constants.USER_ElevenName, UserID = Constants.USER_ElevenId };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role1, role2 }, new List<UserInfo>(), null, sender);

            //Assert
            Assert.AreEqual(message.To, Constants.RoleName_Administrators + "," + Constants.RoleName_Subscribers);
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessageAttachment_On_Passing_Attachments()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId};
            var role = new RoleInfo { RoleName = "role1" };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { 1 }, user);

            //Assert
            _mockDataService.Verify(ds => ds.SaveSocialMessageAttachment(It.Is<MessageAttachment>(v => v.MessageID == message.MessageID), It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_CreateSocialMessageRecipientsForRole_On_Passing_Roles()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId};
            var role = new RoleInfo { RoleName = "role1", RoleID = Constants.RoleID_RegisteredUsers };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.RoleID_RegisteredUsers }, user);

            //Assert
            _mockDataService.Verify(ds => ds.CreateSocialMessageRecipientsForRole(message.MessageID, Constants.RoleID_RegisteredUsers, It.IsAny<int>()));
        }

        [Test]
        public void MessagingController_CreateMessage_Calls_DataService_SaveSocialMessageRecipient_On_Passing_Users()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1" };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.RoleID_RegisteredUsers }, user);         

            //Assert
            _mockDataService.Verify(ds => ds.SaveSocialMessageRecipient(It.Is<MessageRecipient>(v => v.MessageID == message.MessageID && v.UserID == user.UserID), It.IsAny<int>()));            
        }

        [Test]
        public void MessagingController_CreateMessage_Sets_ReplyAll_To_False_On_Passing_Roles()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };
            var role = new RoleInfo { RoleName = "role1", RoleID = Constants.RoleID_RegisteredUsers };

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo> { role }, new List<UserInfo> { user }, new List<int> { Constants.RoleID_RegisteredUsers }, user);

            //Assert
            Assert.AreEqual(message.ReplyAllAllowed, false);            
        }

        [Test]
        public void MessagingController_CreateMessage_Sets_ReplyAll_To_True_On_Passing_User()
        {
            //Arrange
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };            

            //Act
            var message = _messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo> { user }, new List<int> { Constants.RoleID_RegisteredUsers }, user);
            
            //Assert
            Assert.AreEqual(message.ReplyAllAllowed, true);
        }

        [Test]
        public void MessagingController_SetReadMessage_Calls_DataService_UpdateSocialMessageReadStatus()
        {
            //Arrange
            var messageInstance = CreateValidUnReadMessageRecipient();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };    

            //Act
           _messagingController.MarkRead(messageInstance.RecipientID, user.UserID);

            //Assert
           _mockDataService.Verify(ds => ds.UpdateSocialMessageReadStatus(messageInstance.RecipientID, user.UserID, true));
        }

        [Test]
        public void MessagingController_SetUnReadMessage_Calls_DataService_UpdateSocialMessageReadStatus()
        {
            //Arrange
            var messageInstance = CreateValidUnReadMessageRecipient();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId }; 

            //Act
            _messagingController.MarkUnRead(messageInstance.RecipientID, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageReadStatus(messageInstance.RecipientID, user.UserID, false));                        
        }

        [Test]
        public void MessagingController_SetArchivedMessage_Calls_DataService_UpdateSocialMessageArchivedStatus()
        {
            //Arrange
            var messageInstance = CreateValidUnReadMessageRecipient();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId }; 

            //Act
            _messagingController.MarkArchived(messageInstance.RecipientID, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageArchivedStatus(messageInstance.RecipientID, user.UserID, true));                       
        }

        [Test]
        public void MessagingController_SetUnArchivedMessage_Calls_DataService_UpdateSocialMessageArchivedStatus()
        {
            //Arrange
            var messageInstance = CreateValidUnReadMessageRecipient();
            var user = new UserInfo { DisplayName = "user1", UserID = Constants.USER_TenId };

            //Act
            _messagingController.MarkUnArchived(messageInstance.RecipientID, user.UserID);

            //Assert
            _mockDataService.Verify(ds => ds.UpdateSocialMessageArchivedStatus(messageInstance.RecipientID, user.UserID, false));
        }   

        #endregion


        #region "Private Methods"

        private static Message CreateValidMessage()
        {
            var message = new Message
            {
                MessageID = 2,
                Subject  ="test",
                Body="body",
                ParentMessageID = 1,
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
            _dtMessages.Columns.Add("ParentMessageID", typeof(int));
            _dtMessages.Columns.Add("ReplyAllAllowed", typeof(bool));
            _dtMessages.Columns.Add("SenderUserID", typeof(int));
            _dtMessages.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessages.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessages.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessages.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessages.PrimaryKey = new[] { pkMessagesMessageID };

          //MessageRecipients
            _dtMessageRecipients = new DataTable("MessageRecipients");
            var pkMessagesRecipientID = _dtMessages.Columns.Add("RecipientID", typeof(int));
            _dtMessageRecipients.Columns.Add("MessageID", typeof(int));
            _dtMessageRecipients.Columns.Add("UserID", typeof(int));
            _dtMessageRecipients.Columns.Add("Status", typeof(int));
            _dtMessageRecipients.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessageRecipients.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessageRecipients.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessageRecipients.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessageRecipients.PrimaryKey = new[] { pkMessagesRecipientID };


            //MessageAttachments
            _dtMessageAttachment = new DataTable("MessageAttachments");
            var pkMessageAttachmentID = _dtMessages.Columns.Add("MessageAttachmentID", typeof(int));
            _dtMessageAttachment.Columns.Add("MessageID", typeof(int));
            _dtMessageAttachment.Columns.Add("FileID", typeof(int));
            _dtMessageAttachment.Columns.Add("CreatedByUserID", typeof(int));
            _dtMessageAttachment.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtMessageAttachment.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtMessageAttachment.Columns.Add("LastModifiedOnDate", typeof(DateTime));
            _dtMessageAttachment.PrimaryKey = new[] { pkMessageAttachmentID };
            
        }

        #endregion
    }
}