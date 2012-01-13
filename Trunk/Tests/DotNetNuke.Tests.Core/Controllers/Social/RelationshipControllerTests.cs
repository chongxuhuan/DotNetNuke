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
using System.Data;

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social.Data;
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
	public class RelationshipControllerTests
	{
		#region "Private Properties"

        private Mock<IDataService> _mockDataService;		
        private RelationshipController _relationshipController;
        private Mock<DataProvider> _dataProvider;

        private DataTable _dtRelationshipTypes;
        private DataTable _dtRelationships;
        private DataTable _dtUserRelationships;
        private DataTable _dtUserRelationshipPreferences;		


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

            _relationshipController = new RelationshipController(_mockDataService.Object);

			SetupDataProvider();						
		}


        private void SetupDataProvider()
        {
            //Standard DataProvider Path for Logging
            _dataProvider.Setup(d => d.GetProviderPath()).Returns("");

            //RelationshipTypes
            _dtRelationshipTypes = new DataTable("RelationshipTypes");
            var pkRelationshipTypeID = _dtRelationshipTypes.Columns.Add("RelationshipTypeID", typeof(int));
            _dtRelationshipTypes.Columns.Add("Name", typeof(string));
            _dtRelationshipTypes.Columns.Add("Description", typeof(string));
            _dtRelationshipTypes.Columns.Add("Direction", typeof(int));

            _dtRelationshipTypes.PrimaryKey = new[] { pkRelationshipTypeID };

            _dtRelationshipTypes.Rows.Add(DefaultRelationshipTypes.Friends, DefaultRelationshipTypes.Friends.ToString(), DefaultRelationshipTypes.Friends.ToString(), RelationshipDirection.TwoWay);
            _dtRelationshipTypes.Rows.Add( DefaultRelationshipTypes.Followers, DefaultRelationshipTypes.Followers.ToString(), DefaultRelationshipTypes.Followers.ToString(), RelationshipDirection.OneWay);

            _mockDataService.Setup(md => md.GetAllRelationshipTypes()).Returns(_dtRelationshipTypes.CreateDataReader());


            //Relationships
            _dtRelationships = new DataTable("Relationships");
            var pkRelationshipID = _dtRelationships.Columns.Add("RelationshipID", typeof(int));
            _dtRelationships.Columns.Add("RelationshipTypeID", typeof(int));
            _dtRelationships.Columns.Add("Name", typeof(string));
            _dtRelationships.Columns.Add("Description", typeof(string));
            _dtRelationships.Columns.Add("PortalID", typeof(int));
            _dtRelationships.Columns.Add("UserID", typeof(int));
            _dtRelationships.Columns.Add("DefaultResponse", typeof(int));
            _dtRelationships.PrimaryKey = new[] { pkRelationshipID };

            //Create default Friend and Social Relationships
            _dtRelationships.Rows.Add(Constants.SOCIAL_FriendRelationshipID, DefaultRelationshipTypes.Friends, DefaultRelationshipTypes.Friends.ToString(), DefaultRelationshipTypes.Friends.ToString(), Constants.PORTAL_Zero, Constants.USER_Null, RelationshipStatus.None);
            _dtRelationships.Rows.Add(Constants.SOCIAL_FollowerRelationshipID, DefaultRelationshipTypes.Followers, DefaultRelationshipTypes.Followers.ToString(), DefaultRelationshipTypes.Followers.ToString(), Constants.PORTAL_Zero, Constants.USER_Null, RelationshipStatus.None);

            _mockDataService.Setup(md => md.GetRelationshipsByPortalID(It.IsAny<int>())).Returns(_dtRelationships.CreateDataReader());

            //UserRelationships
            _dtUserRelationships = new DataTable("UserRelationships");
            var pkUserRelationshipID = _dtUserRelationships.Columns.Add("UserRelationshipID", typeof(int));
            _dtUserRelationships.Columns.Add("UserID", typeof(int));
            _dtUserRelationships.Columns.Add("RelatedUserID", typeof(int));
            _dtUserRelationships.Columns.Add("RelationshipID", typeof(int));
            _dtUserRelationships.Columns.Add("Status", typeof(int));
            _dtUserRelationships.PrimaryKey = new[] { pkUserRelationshipID };

            //UserRelationships
            _dtUserRelationshipPreferences = new DataTable("UserRelationshipPreferences");
            var pkPreferenceID = _dtUserRelationshipPreferences.Columns.Add("PreferenceID", typeof(int));
            _dtUserRelationshipPreferences.Columns.Add("UserID", typeof(int));
            _dtUserRelationshipPreferences.Columns.Add("RelationshipID", typeof(int));
            _dtUserRelationshipPreferences.Columns.Add("DefaultResponse", typeof(int));
            _dtUserRelationshipPreferences.PrimaryKey = new[] { pkPreferenceID };

            //create default UserRelationship records            
        }

		#endregion

        #region Relationship Business APIs Tests

        #region AddFriend Tests

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RelationshipController_AddFriend_Throws_When_On_Null_Initiating_User()
        {
            //Arrange
            var targetUser = new UserInfo();

            //Act, Assert
            _relationshipController.AddFriend(null, targetUser);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RelationshipController_AddFriend_Throws_When_On_Null_Target_User()
        {
            //Arrange
            var initiatingUser = new UserInfo();

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelationshipController_AddFriend_Throws_On_Negative_Initiating_UserID()
        {
            //Arrange
            var initiatingUser = new UserInfo();
            var targetUser = new UserInfo {UserID = Constants.USER_TenId};

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelationshipController_AddFriend_Throws_On_Negative_Target_UserID()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId};
            var targetUser = new UserInfo();

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }
        
        [Test]
        [ExpectedException(typeof(UserRelationshipForSameUsersException))]
        public void RelationshipController_AddFriend_Throws_When_Same_Users_Are_Passed()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero};

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, initiatingUser);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelationshipController_AddFriend_Throws_On_Negative_Initiating_PortalID()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId};
            var targetUser = new UserInfo {UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero};

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelationshipController_AddFriend_Throws_On_Negative_Target_PortalID()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero};
            var targetUser = new UserInfo {UserID = Constants.USER_ElevenId};

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipForDifferentPortalException))]
        public void RelationshipController_AddFriend_Throws_On_Users_With_Different_PortalID()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero};
            var targetUser = new UserInfo {UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_One};

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipExistsException))]
        public void RelationshipController_AddFriend_Throws_On_Existing_Relationship()
        {
            //Arrange
            var initiatingUser = new UserInfo { UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            var targetUser = new UserInfo { UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero };

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService            
            _mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RelationshipDirection>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            _relationshipController.AddFriend(initiatingUser, targetUser);
        }
    
        #endregion

        #region InitiateUserRelationship Tests

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RelationshipController_InitiateUserRelationship_Throws_On_Negative_RelationshipID()
        {
            //Arrange
            var initiatingUser = new UserInfo {UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero};
            var targetUser = new UserInfo {UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero};
            var relationship = new Relationship();

            //Act, Assert
            _relationshipController.InitiateUserRelationship(initiatingUser, targetUser, relationship);
        }     

        [Test]       
        public void RelationshipController_InitiateUserRelationship_Returns_Status_Accepted_When_Default_Relationship_Action_Is_Accepted()
        {
            //Arrange
            var initiatingUser = new UserInfo { UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            var targetUser = new UserInfo { UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero };
            var relationship = new Relationship { RelationshipID = Constants.SOCIAL_FollowerRelationshipID, RelationshipTypeID = Constants.SOCIAL_FollowerRelationshipTypeID, DefaultResponse = RelationshipStatus.Accepted };
           
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RelationshipDirection>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.GetUserRelationshipPreference(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtUserRelationshipPreferences.CreateDataReader());
            mockDataService.Setup(md => md.GetAllRelationshipTypes()).Returns(_dtRelationshipTypes.CreateDataReader());

            //Act
            var userRelationship = (new RelationshipController(mockDataService.Object)).InitiateUserRelationship(initiatingUser, targetUser, relationship);

            //Assert
            Assert.AreEqual(userRelationship.Status, RelationshipStatus.Accepted);
        }

        [Test]
        public void RelationshipController_InitiateUserRelationship_Returns_Status_Initiated_When_Default_Relationship_Action_Is_None()
        {
            //Arrange
            var initiatingUser = new UserInfo { UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            var targetUser = new UserInfo { UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero };
            var relationship = new Relationship { RelationshipID = Constants.SOCIAL_FollowerRelationshipID, RelationshipTypeID = Constants.SOCIAL_FollowerRelationshipTypeID, DefaultResponse = RelationshipStatus.None };

            _dtUserRelationships.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RelationshipDirection>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.GetUserRelationshipPreference(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtUserRelationshipPreferences.CreateDataReader());
            mockDataService.Setup(md => md.GetAllRelationshipTypes()).Returns(_dtRelationshipTypes.CreateDataReader());

            //Act
            var userRelationship = (new RelationshipController(mockDataService.Object)).InitiateUserRelationship(initiatingUser, targetUser, relationship);

            //Assert
            Assert.AreEqual(userRelationship.Status, RelationshipStatus.Initiated);
        }

        [Test]
        public void RelationshipController_InitiateUserRelationship_Returns_Status_Accepted_When_TargetUsers_Relationship_Action_Is_Accepted()
        {
            //Arrange
            var initiatingUser = new UserInfo { UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            var targetUser = new UserInfo { UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero };
            var relationship = new Relationship { RelationshipID = Constants.SOCIAL_FollowerRelationshipID, RelationshipTypeID = Constants.SOCIAL_FollowerRelationshipTypeID, DefaultResponse = RelationshipStatus.Accepted };

            _dtUserRelationships.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Add(Constants.SOCIAL_PrefereceIDForUser11, Constants.USER_TenId, Constants.USER_ElevenId, RelationshipStatus.Accepted);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RelationshipDirection>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.GetUserRelationshipPreference(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtUserRelationshipPreferences.CreateDataReader());
            mockDataService.Setup(md => md.GetAllRelationshipTypes()).Returns(_dtRelationshipTypes.CreateDataReader());

            //Act
            var userRelationship = (new RelationshipController(mockDataService.Object)).InitiateUserRelationship(initiatingUser, targetUser, relationship);

            //Assert
            Assert.AreEqual(userRelationship.Status, RelationshipStatus.Accepted);
        }

        [Test]
        public void RelationshipController_InitiateUserRelationship_Returns_Status_Initiated_When_TargetUsers_Relationship_Action_Is_None()
        {
            //Arrange
            var initiatingUser = new UserInfo { UserID = Constants.USER_TenId, PortalID = Constants.PORTAL_Zero };
            var targetUser = new UserInfo { UserID = Constants.USER_ElevenId, PortalID = Constants.PORTAL_Zero };
            var relationship = new Relationship { RelationshipID = Constants.SOCIAL_FollowerRelationshipID, RelationshipTypeID = Constants.SOCIAL_FollowerRelationshipTypeID, DefaultResponse = RelationshipStatus.Accepted };

            _dtUserRelationships.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Clear();
            _dtUserRelationshipPreferences.Rows.Add(Constants.SOCIAL_PrefereceIDForUser11, Constants.USER_TenId, Constants.USER_ElevenId, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<RelationshipDirection>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.GetUserRelationshipPreference(It.IsAny<int>(), It.IsAny<int>())).Returns(_dtUserRelationshipPreferences.CreateDataReader());
            mockDataService.Setup(md => md.GetAllRelationshipTypes()).Returns(_dtRelationshipTypes.CreateDataReader());

            //Act
            var userRelationship = (new RelationshipController(mockDataService.Object)).InitiateUserRelationship(initiatingUser, targetUser, relationship);

            //Assert
            Assert.AreEqual(userRelationship.Status, RelationshipStatus.Initiated);
        }

        #endregion

        #region UpdateRelationship Tests

        #region UserRelationshipDoesNotExist Exception

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_RemoveUserRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).RemoveUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_AcceptRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).AcceptUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_IgnoreRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).IgnoreUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_RejectRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).RejectUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_BlockRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).BlockUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        [Test]
        [ExpectedException(typeof(UserRelationshipDoesNotExistException))]
        public void RelationshipController_ReportRelationship_Throws_On_NonExistent_Relationship()
        {
            //Arrange

            //No UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());

            //Act, Assert
            new RelationshipController(mockDataService.Object).ReportUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);
        }

        #endregion

        #region Verify Update of UserRelationship Status calls Data Layer
        
        [Test]
        public void RelationshipController_BlockUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.SaveUserRelationship(It.IsAny<UserRelationship>(),It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).BlockUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.SaveUserRelationship(It.IsAny<UserRelationship>(),It.IsAny<int>()));            
        }

        [Test]
        public void RelationshipController_AcceptUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).AcceptUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));
        }

        [Test]
        public void RelationshipController_RejectUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).RejectUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));
        }

        [Test]
        public void RelationshipController_IgnoreUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).IgnoreUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));
        }

        [Test]
        public void RelationshipController_ReportUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).ReportUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.SaveUserRelationship(It.IsAny<UserRelationship>(), It.IsAny<int>()));
        }

        [Test]
        public void RelationshipController_RemoveUserRelationship_Calls_DataService_On_Valid_RelationshipID()
        {
            //Arrange

            //Any UserRelationship between user10 and user11
            _dtUserRelationships.Rows.Clear();
            _dtUserRelationships.Rows.Add(Constants.SOCIAL_UserRelationshipIDUser10User11, Constants.USER_TenId, Constants.USER_ElevenId, Constants.SOCIAL_FriendRelationshipID, RelationshipStatus.None);

            //setup mock DataService
            var mockDataService = new Mock<IDataService>();
            mockDataService.Setup(md => md.GetUserRelationship(It.IsAny<int>())).Returns(_dtUserRelationships.CreateDataReader());
            mockDataService.Setup(md => md.DeleteUserRelationship(It.IsAny<int>()));

            //Act
            new RelationshipController(mockDataService.Object).RemoveUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11);

            //Assert
            mockDataService.Verify(ds => ds.DeleteUserRelationship(Constants.SOCIAL_UserRelationshipIDUser10User11));
        }

        #endregion

        #endregion

        #endregion
       

        #region "Private Methods"

        #endregion
    }
}