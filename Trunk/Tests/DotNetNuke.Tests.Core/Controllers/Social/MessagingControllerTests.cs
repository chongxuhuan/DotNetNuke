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

            _messagingController = new MessagingController(_mockDataService.Object);

			SetupDataProvider();						
		}


        private void SetupDataProvider()
        {
            //Standard DataProvider Path for Logging
            _dataProvider.Setup(d => d.GetProviderPath()).Returns("");                    
        }

		#endregion

        #region #region Easy Wrapper APIs Tests

        [Test]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Null_Body_And_Subject()
        {
            //Act, Assert
            _messagingController.CreateMessage(null, null, null, null);
        }

        [Test]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Null_Roles_And_Users()
        {
            //Act, Assert
            _messagingController.CreateMessage("subject", "body", null, null);
        }

        [Test]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Empty_Roles_And_Users_Lists()
        {
            //Act, Assert
            _messagingController.CreateMessage("subject", "body", new List<RoleInfo>(), new List<UserInfo>());
        }

        [Test]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
        public void MessagingController_CreateMessage_Throws_On_Large_Subject()
        {
            //Arrange
            StringBuilder subject = new StringBuilder();
            for (int i = 0; i <= 40; i++)
            {
                subject.Append("1234567890");
            }

            //Act, Assert
            _messagingController.CreateMessage(subject.ToString(), "body", new List<RoleInfo> { new RoleInfo() }, new List<UserInfo> { new UserInfo() });
        }

        [Test]
        [ExpectedException(typeof(InvalidEnumArgumentException))]
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
            _messagingController.CreateMessage("subject", "body", roles, users);
        }

        #endregion


        #region "Private Methods"

        #endregion
    }
}