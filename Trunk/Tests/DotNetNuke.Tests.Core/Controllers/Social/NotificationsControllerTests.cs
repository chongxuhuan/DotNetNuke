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
using System.Data;
using System.Globalization;
using System.Text;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Exceptions;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Social.Notifications.Data;
using DotNetNuke.Services.Social.Notifications.Internal;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using Moq;

using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace DotNetNuke.Tests.Core.Controllers.Social
{
    [TestFixture]
    public class NotificationsControllerTests
    {
        #region Private Properties

        private Mock<IDataService> _mockDataService;
        private Mock<DotNetNuke.Services.Social.Messaging.Data.IDataService> _mockMessagingDataService;
        private Mock<IMessagingController> _mockMessagingController;
        private NotificationsControllerImpl _notificationsController;
        private Mock<NotificationsControllerImpl> _mockNotificationsController;
        private Mock<DataProvider> _dataProvider;
        private Mock<CachingProvider> _cachingProvider;
        private DataTable _dtNotificationTypes;
        private DataTable _dtNotificationTypeActions;
        private DataTable _dtNotificationActions;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            ComponentFactory.Container = new SimpleContainer();

            _mockDataService = new Mock<IDataService>();
            _mockMessagingDataService = new Mock<DotNetNuke.Services.Social.Messaging.Data.IDataService>();
            _dataProvider = MockComponentProvider.CreateDataProvider();
            _cachingProvider = MockComponentProvider.CreateDataCacheProvider();

            _notificationsController = new NotificationsControllerImpl(_mockDataService.Object, _mockMessagingDataService.Object);
            _mockNotificationsController = new Mock<NotificationsControllerImpl> { CallBase = true };

            _mockMessagingController = new Mock<IMessagingController>();
            MessagingController.SetTestableInstance(_mockMessagingController.Object);

            DataService.RegisterInstance(_mockDataService.Object);
            DotNetNuke.Services.Social.Messaging.Data.DataService.RegisterInstance(_mockMessagingDataService.Object);
            
            SetupDataProvider();
            SetupDataTables();
        }

        private void SetupDataProvider()
        {
            //Standard DataProvider Path for Logging
            _dataProvider.Setup(d => d.GetProviderPath()).Returns("");
        }

        #endregion

        #region CreateNotificationType

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotificationType_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.CreateNotificationType(
                name,
                Constants.Messaging_NotificationTypeDescription,
                TimeSpan.FromMinutes(Constants.Messaging_NotificationTypeTTL),
                Constants.Messaging_NotificationTypeDesktopModuleId);
        }

        [Test]
        public void CreateNotificationType_Calls_DataService_SaveNotificationType()
        {
            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationType(Null.NullInteger, Constants.Messaging_NotificationTypeName, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Object.CreateNotificationType(
                Constants.Messaging_NotificationTypeName,
                Constants.Messaging_NotificationTypeDescription,
                TimeSpan.FromMinutes(Constants.Messaging_NotificationTypeTTL),
                Constants.Messaging_NotificationTypeDesktopModuleId);

            _mockDataService.Verify();
        }

        [Test]
        [TestCase(int.MaxValue, int.MaxValue)]
        [TestCase(1, 1)]
        [TestCase(0, 0)]
        public void CreateNotificationType_Returns_Object_With_Valid_TimeToLive(int actualTimeToLiveTotalMinutes, int expectedTimeToLiveTotalMinutes)
        {
            var actualTimeToLive = TimeSpan.FromMinutes(actualTimeToLiveTotalMinutes);
            var messageType = _notificationsController.CreateNotificationType(
                Constants.Messaging_NotificationTypeName,
                Constants.Messaging_NotificationTypeDescription,
                actualTimeToLive,
                Constants.Messaging_NotificationTypeDesktopModuleId);

            Assert.AreEqual(expectedTimeToLiveTotalMinutes, (int)messageType.TimeToLive.TotalMinutes);
        }

        [Test]
        public void CreateNotificationType_Returns_Valid_Object()
        {
            var expectedNotificationType = CreateValidNotificationType();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationType(
                    Null.NullInteger,
                    expectedNotificationType.Name,
                    expectedNotificationType.Description,
                    (int)expectedNotificationType.TimeToLive.TotalMinutes,
                    expectedNotificationType.DesktopModuleId,
                    Constants.UserID_User12))
                .Returns(Constants.Messaging_NotificationTypeId);

            var actualNotificationType = _mockNotificationsController.Object.CreateNotificationType(
                expectedNotificationType.Name,
                expectedNotificationType.Description,
                expectedNotificationType.TimeToLive,
                expectedNotificationType.DesktopModuleId);

            Assert.IsTrue(new NotificationTypeComparer().Equals(expectedNotificationType, actualNotificationType));
        }

        #endregion

        #region DeleteNotificationType

        [Test]
        public void DeleteNotificationType_Calls_DataService_DeleteNotificationType()
        {
            _mockDataService.Setup(ds => ds.DeleteNotificationType(Constants.Messaging_NotificationTypeId)).Verifiable();
            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeCache());
            _mockNotificationsController.Object.DeleteNotificationType(Constants.Messaging_NotificationTypeId);
            _mockDataService.Verify();
        }

        [Test]
        public void DeleteNotificationType_Removes_Cache_Object()
        {
            _mockDataService.Setup(ds => ds.DeleteNotificationType(Constants.Messaging_NotificationTypeId));
            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeCache()).Verifiable();
            _mockNotificationsController.Object.DeleteNotificationType(Constants.Messaging_NotificationTypeId);
            _mockNotificationsController.Verify();
        }

        #endregion

        #region GetNotificationType

        [Test]
        public void GetNotificationType_By_Id_Gets_Object_From_Cache()
        {
            _cachingProvider.Object.PurgeCache();
            _cachingProvider.Setup(cp => cp.GetItem(It.IsAny<string>())).Verifiable();
            _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeId);
            _cachingProvider.Verify();
        }

        [Test]
        public void GetNotificationType_By_Id_Calls_DataService_GetNotificationType_When_Object_Is_Not_In_Cache()
        {
            _cachingProvider.Object.PurgeCache();

            var messageTypeDataTable = new DataTable();

            _mockDataService
                .Setup(ds => ds.GetNotificationType(Constants.Messaging_NotificationTypeId))
                .Returns(messageTypeDataTable.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeId);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationType_By_Id_Returns_Valid_Object()
        {
            _cachingProvider.Object.PurgeCache();

            var expectedNotificationType = CreateValidNotificationType();

            _dtNotificationTypes.Rows.Clear();

            _dtNotificationTypes.Rows.Add(
                expectedNotificationType.NotificationTypeId,
                expectedNotificationType.Name,
                expectedNotificationType.Description,
                (int)expectedNotificationType.TimeToLive.TotalMinutes,
                expectedNotificationType.DesktopModuleId);

            _mockDataService
                .Setup(ds => ds.GetNotificationType(Constants.Messaging_NotificationTypeId))
                .Returns(_dtNotificationTypes.CreateDataReader());

            var actualNotificationType = _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeId);

            Assert.IsTrue(new NotificationTypeComparer().Equals(expectedNotificationType, actualNotificationType));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNotificationType_By_Name_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.GetNotificationType(name);
        }

        [Test]
        public void GetNotificationType_By_Name_Gets_Object_From_Cache()
        {
            _cachingProvider.Object.PurgeCache();
            _cachingProvider.Setup(cp => cp.GetItem(It.IsAny<string>())).Verifiable();
            _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeName);
            _cachingProvider.Verify();
        }

        [Test]
        public void GetNotificationType_By_Name_Calls_DataService_GetNotificationTypeByName_When_Object_Is_Not_In_Cache()
        {
            _cachingProvider.Object.PurgeCache();

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeByName(Constants.Messaging_NotificationTypeName))
                .Returns(_dtNotificationTypes.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeName);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationType_By_Name_Returns_Valid_Object()
        {
            _cachingProvider.Object.PurgeCache();

            var expectedNotificationType = CreateValidNotificationType();

            _dtNotificationTypes.Rows.Clear();

            _dtNotificationTypes.Rows.Add(
                expectedNotificationType.NotificationTypeId,
                expectedNotificationType.Name,
                expectedNotificationType.Description,
                (int)expectedNotificationType.TimeToLive.TotalMinutes,
                expectedNotificationType.DesktopModuleId);

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeByName(Constants.Messaging_NotificationTypeName))
                .Returns(_dtNotificationTypes.CreateDataReader());

            var actualNotificationType = _notificationsController.GetNotificationType(Constants.Messaging_NotificationTypeName);

            Assert.IsTrue(new NotificationTypeComparer().Equals(expectedNotificationType, actualNotificationType));
        }

        #endregion

        #region UpdateNotificationType

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNotificationType_Throws_On_Null_NotificationType()
        {
            _notificationsController.UpdateNotificationType(null);
        }

        [Test]
        public void UpdateNotificationType_Calls_DataService_SaveNotificationType()
        {
            var notificationType = CreateValidNotificationType();

            _mockDataService
                .Setup(ds => ds.SaveNotificationType(
                    notificationType.NotificationTypeId,
                    notificationType.Name,
                    notificationType.Description,
                    Constants.Messaging_NotificationTypeTTL,
                    notificationType.DesktopModuleId,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockNotificationsController.Object.UpdateNotificationType(notificationType);

            _mockDataService.Verify();
        }

        [Test]
        public void UpdateNotificationType_Removes_Object_From_Cache()
        {
            var notificationType = CreateValidNotificationType();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationType(
                    notificationType.NotificationTypeId,
                    notificationType.Name,
                    notificationType.Description,
                    Constants.Messaging_NotificationTypeTTL,
                    notificationType.DesktopModuleId,
                    Constants.UserID_User12));

            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeCache()).Verifiable();
            _mockNotificationsController.Object.UpdateNotificationType(notificationType);
            _mockNotificationsController.Verify();
        }

        #endregion

        #region AddNotificationTypeActionToEnd

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionToEnd_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.AddNotificationTypeActionToEnd(
                Constants.Messaging_NotificationTypeId,
                name,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionToEnd_Throws_On_Null_Or_Empty_APICall(string apiCall)
        {
            _notificationsController.AddNotificationTypeActionToEnd(
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                apiCall);
        }

        [Test]
        public void AddNotificationTypeActionToEnd_Calls_DataService_AddNotificationTypeActionToEnd()
        {
            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionToEnd(
                    Constants.Messaging_NotificationTypeId,
                    Constants.Messaging_NotificationTypeActionNameResourceKey,
                    Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                    Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                    Constants.Messaging_NotificationTypeActionAPICall,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetNotificationTypeAction(It.IsAny<int>()));

            _mockNotificationsController.Object.AddNotificationTypeActionToEnd(
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);

            _mockDataService.Verify();
        }

        [Test]
        public void AddNotificationTypeActionToEnd_ReturnsValidObject()
        {
            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionToEnd(
                    expectedNotificationTypeAction.NotificationTypeId,
                    expectedNotificationTypeAction.NameResourceKey,
                    expectedNotificationTypeAction.DescriptionResourceKey,
                    expectedNotificationTypeAction.ConfirmResourceKey,
                    expectedNotificationTypeAction.APICall,
                    Constants.UserID_User12))
                .Returns(expectedNotificationTypeAction.NotificationTypeActionId);

            _mockNotificationsController
                .Setup(nc => nc.GetNotificationTypeAction(expectedNotificationTypeAction.NotificationTypeActionId))
                .Returns(expectedNotificationTypeAction);

            var actualNotificationTypeAction = _mockNotificationsController.Object.AddNotificationTypeActionToEnd(
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.APICall);

            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeAction));
        }

        #endregion

        #region AddNotificationTypeActionAfter

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionAfter_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.AddNotificationTypeActionAfter(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                name,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionAfter_Throws_On_Null_Or_Empty_APICall(string apiCall)
        {
            _notificationsController.AddNotificationTypeActionAfter(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                apiCall);
        }

        [Test]
        public void AddNotificationTypeActionAfter_Calls_DataService_AddNotificationTypeActionAfter()
        {
            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionAfter(
                    Constants.Messaging_NotificationTypeActionId,
                    Constants.Messaging_NotificationTypeId,
                    Constants.Messaging_NotificationTypeActionNameResourceKey,
                    Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                    Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                    Constants.Messaging_NotificationTypeActionAPICall,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetNotificationTypeAction(It.IsAny<int>()));

            _mockNotificationsController.Object.AddNotificationTypeActionAfter(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);

            _mockDataService.Verify();
        }

        [Test]
        public void AddNotificationTypeActionAfter_ReturnsValidObject()
        {
            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionAfter(
                    expectedNotificationTypeAction.NotificationTypeActionId,
                    expectedNotificationTypeAction.NotificationTypeId,
                    expectedNotificationTypeAction.NameResourceKey,
                    expectedNotificationTypeAction.DescriptionResourceKey,
                    expectedNotificationTypeAction.ConfirmResourceKey,
                    expectedNotificationTypeAction.APICall,
                    Constants.UserID_User12))
                .Returns(expectedNotificationTypeAction.NotificationTypeActionId);

            _mockNotificationsController
                .Setup(nc => nc.GetNotificationTypeAction(expectedNotificationTypeAction.NotificationTypeActionId))
                .Returns(expectedNotificationTypeAction);

            var actualNotificationTypeAction = _mockNotificationsController.Object.AddNotificationTypeActionAfter(
                expectedNotificationTypeAction.NotificationTypeActionId,
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.APICall);

            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeAction));
        }

        #endregion

        #region AddNotificationTypeActionBefore

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionBefore_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.AddNotificationTypeActionBefore(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                name,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNotificationTypeActionBefore_Throws_On_Null_Or_Empty_APICall(string apiCall)
        {
            _notificationsController.AddNotificationTypeActionBefore(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                apiCall);
        }

        [Test]
        public void AddNotificationTypeActionBefore_Calls_DataService_AddNotificationTypeActionBefore()
        {
            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionBefore(
                    Constants.Messaging_NotificationTypeActionId,
                    Constants.Messaging_NotificationTypeId,
                    Constants.Messaging_NotificationTypeActionNameResourceKey,
                    Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                    Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                    Constants.Messaging_NotificationTypeActionAPICall,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetNotificationTypeAction(It.IsAny<int>()));

            _mockNotificationsController.Object.AddNotificationTypeActionBefore(
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationTypeId,
                Constants.Messaging_NotificationTypeActionNameResourceKey,
                Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                Constants.Messaging_NotificationTypeActionAPICall);

            _mockDataService.Verify();
        }

        [Test]
        public void AddNotificationTypeActionBefore_ReturnsValidObject()
        {
            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.AddNotificationTypeActionBefore(
                    expectedNotificationTypeAction.NotificationTypeActionId,
                    expectedNotificationTypeAction.NotificationTypeId,
                    expectedNotificationTypeAction.NameResourceKey,
                    expectedNotificationTypeAction.DescriptionResourceKey,
                    expectedNotificationTypeAction.ConfirmResourceKey,
                    expectedNotificationTypeAction.APICall,
                    Constants.UserID_User12))
                .Returns(expectedNotificationTypeAction.NotificationTypeActionId);

            _mockNotificationsController
                .Setup(nc => nc.GetNotificationTypeAction(expectedNotificationTypeAction.NotificationTypeActionId))
                .Returns(expectedNotificationTypeAction);

            var actualNotificationTypeAction = _mockNotificationsController.Object.AddNotificationTypeActionBefore(
                expectedNotificationTypeAction.NotificationTypeActionId,
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.APICall);

            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeAction));
        }

        #endregion

        #region UpdateNotificationTypeAction

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNotificationTypeAction_Throws_On_Null_NotificationTypeAction()
        {
            _notificationsController.UpdateNotificationTypeAction(null);
        }

        [Test]
        public void UpdateNotificationTypeAction_Calls_DataService_UpdateNotificationTypeAction()
        {
            var notificationTypeAction = CreateValidNotificationTypeAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.UpdateNotificationTypeAction(
                    notificationTypeAction.NotificationTypeActionId,
                    notificationTypeAction.NameResourceKey,
                    notificationTypeAction.DescriptionResourceKey,
                    notificationTypeAction.ConfirmResourceKey,
                    notificationTypeAction.APICall,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Object.UpdateNotificationTypeAction(notificationTypeAction);

            _mockDataService.Verify();
        }

        [Test]
        public void UpdateNotificationTypeAction_Removes_Object_From_Cache()
        {
            var notificationTypeAction = CreateValidNotificationTypeAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.UpdateNotificationTypeAction(
                    notificationTypeAction.NotificationTypeActionId,
                    notificationTypeAction.NameResourceKey,
                    notificationTypeAction.DescriptionResourceKey,
                    notificationTypeAction.ConfirmResourceKey,
                    notificationTypeAction.APICall,
                    Constants.UserID_User12));

            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeActionCache()).Verifiable();
            _mockNotificationsController.Object.UpdateNotificationTypeAction(notificationTypeAction);
            _mockNotificationsController.Verify();
        }

        #endregion

        #region DeleteNotificationTypeAction

        [Test]
        public void DeleteNotificationTypeAction_Calls_DataService_DeleteNotificationTypeAction()
        {
            _mockDataService.Setup(ds => ds.DeleteNotificationTypeAction(Constants.Messaging_NotificationTypeActionId)).Verifiable();
            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeActionCache());
            _mockNotificationsController.Object.DeleteNotificationTypeAction(Constants.Messaging_NotificationTypeActionId);
            _mockDataService.Verify();
        }

        [Test]
        public void DeleteNotificationTypeAction_Removes_Cache_Object()
        {
            _mockDataService.Setup(ds => ds.DeleteNotificationTypeAction(Constants.Messaging_NotificationTypeActionId));
            _mockNotificationsController.Setup(nc => nc.RemoveNotificationTypeActionCache()).Verifiable();
            _mockNotificationsController.Object.DeleteNotificationTypeAction(Constants.Messaging_NotificationTypeActionId);
            _mockNotificationsController.Verify();
        }

        #endregion

        #region GetNotificationTypeAction

        [Test]
        public void GetNotificationTypeAction_By_Id_Gets_Object_From_Cache()
        {
            _cachingProvider.Object.PurgeCache();
            _cachingProvider.Setup(cp => cp.GetItem(It.IsAny<string>())).Verifiable();
            _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeActionId);
            _cachingProvider.Verify();
        }

        [Test]
        public void GetNotificationTypeAction_By_Id_Calls_DataService_GetNotificationTypeAction_When_Object_Is_Not_In_Cache()
        {
            _cachingProvider.Object.PurgeCache();

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeAction(Constants.Messaging_NotificationTypeActionId))
                .Returns(_dtNotificationTypeActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeActionId);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationTypeAction_By_Id_Returns_Valid_Object()
        {
            _cachingProvider.Object.PurgeCache();

            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _dtNotificationTypeActions.Clear();

            _dtNotificationTypeActions.Rows.Add(
                expectedNotificationTypeAction.NotificationTypeActionId,
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.Order,
                expectedNotificationTypeAction.APICall);

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeAction(Constants.Messaging_NotificationTypeActionId))
                .Returns(_dtNotificationTypeActions.CreateDataReader);

            var actualNotificationTypeAction = _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeActionId);

            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeAction));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNotificationTypeAction_By_Name_Throws_On_Null_Or_Empty_Name(string name)
        {
            _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeId, name);
        }

        [Test]
        public void GetNotificationTypeAction_By_Name_Gets_Object_From_Cache()
        {
            _cachingProvider.Object.PurgeCache();
            _cachingProvider.Setup(cp => cp.GetItem(It.IsAny<string>())).Verifiable();
            _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeId, Constants.Messaging_NotificationTypeActionNameResourceKey);
            _cachingProvider.Verify();
        }

        [Test]
        public void GetNotificationTypeAction_By_Name_Calls_DataService_GetNotificationTypeActionByName_When_Object_Is_Not_In_Cache()
        {
            _cachingProvider.Object.PurgeCache();

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeActionByName(
                    Constants.Messaging_NotificationTypeId,
                    Constants.Messaging_NotificationTypeActionNameResourceKey))
                .Returns(_dtNotificationTypeActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeId, Constants.Messaging_NotificationTypeActionNameResourceKey);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationTypeAction_By_Name_Returns_Valid_Object()
        {
            _cachingProvider.Object.PurgeCache();

            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _dtNotificationTypeActions.Clear();

            _dtNotificationTypeActions.Rows.Add(
                expectedNotificationTypeAction.NotificationTypeActionId,
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.Order,
                expectedNotificationTypeAction.APICall);

            _mockDataService
                .Setup(ds => ds.GetNotificationTypeActionByName(
                    Constants.Messaging_NotificationTypeId,
                    Constants.Messaging_NotificationTypeActionNameResourceKey))
                .Returns(_dtNotificationTypeActions.CreateDataReader());

            var actualNotificationTypeAction = _notificationsController.GetNotificationTypeAction(Constants.Messaging_NotificationTypeId, Constants.Messaging_NotificationTypeActionNameResourceKey);

            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeAction));
        }

        #endregion

        #region GetNotificationTypeActions

        [Test]
        public void GetNotificationTypeActions_Calls_DataService_GetNotificationTypeActions()
        {
            _mockDataService
                .Setup(ds => ds.GetNotificationTypeActions(Constants.Messaging_NotificationTypeId))
                .Returns(_dtNotificationTypeActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationTypeActions(Constants.Messaging_NotificationTypeId);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationTypeActions_Returns_Valid_Object()
        {
            var expectedNotificationTypeAction = CreateValidNotificationTypeAction();

            _dtNotificationTypeActions.Clear();

            _dtNotificationTypeActions.Rows.Add(
                expectedNotificationTypeAction.NotificationTypeActionId,
                expectedNotificationTypeAction.NotificationTypeId,
                expectedNotificationTypeAction.NameResourceKey,
                expectedNotificationTypeAction.DescriptionResourceKey,
                expectedNotificationTypeAction.ConfirmResourceKey,
                expectedNotificationTypeAction.Order,
                expectedNotificationTypeAction.APICall);

            _mockDataService.Setup(ds => ds.GetNotificationTypeActions(Constants.Messaging_NotificationTypeId)).Returns(_dtNotificationTypeActions.CreateDataReader());

            var actualNotificationTypeActions = _notificationsController.GetNotificationTypeActions(Constants.Messaging_NotificationTypeId);

            Assert.AreEqual(1, actualNotificationTypeActions.Count);
            Assert.IsTrue(new NotificationTypeActionComparer().Equals(expectedNotificationTypeAction, actualNotificationTypeActions[0]));
        }

        #endregion

        #region CreateNotificationAction

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotificationAction_Throws_On_Null_Or_Empty_Key(string key)
        {
            _notificationsController.CreateNotificationAction(
                Constants.Messaging_MessageId_1,
                Constants.Messaging_NotificationTypeActionId,
                key);
        }

        [Test]
        public void CreateNotificationAction_Calls_DataService_SaveNotificationAction()
        {
            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationAction(
                    Null.NullInteger,
                    Constants.Messaging_MessageId_1,
                    Constants.Messaging_NotificationTypeActionId,
                    Constants.Messaging_NotificationActionKey,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Object.CreateNotificationAction(
                Constants.Messaging_MessageId_1,
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationActionKey);

            _mockDataService.Verify();
        }

        [Test]
        public void CreateNotificationAction_Returns_Valid_Object()
        {
            var expectedNotificationAction = CreateValidNotificationAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationAction(
                    Null.NullInteger,
                    Constants.Messaging_MessageId_1,
                    Constants.Messaging_NotificationTypeActionId,
                    Constants.Messaging_NotificationActionKey,
                    Constants.UserID_User12))
                .Returns(Constants.Messaging_NotificationActionId);

            var actualNotificationAction = _mockNotificationsController.Object.CreateNotificationAction(
                Constants.Messaging_MessageId_1,
                Constants.Messaging_NotificationTypeActionId,
                Constants.Messaging_NotificationActionKey);

            Assert.IsTrue(new NotificationActionComparer().Equals(expectedNotificationAction, actualNotificationAction));
        }

        #endregion

        #region UpdateNotificationAction

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNotificationAction_Throws_On_Null_NotificationAction()
        {
            _notificationsController.UpdateNotificationAction(null);
        }

        [Test]
        public void UpdateNotificationAction_Calls_DataService_SaveNotificationAction()
        {
            var notificationAction = CreateValidNotificationAction();

            _mockNotificationsController.Setup(nc => nc.GetCurrentUserId()).Returns(Constants.UserID_User12);

            _mockDataService
                .Setup(ds => ds.SaveNotificationAction(
                    notificationAction.NotificationActionId,
                    notificationAction.NotificationId,
                    notificationAction.NotificationTypeActionId,
                    notificationAction.Key,
                    Constants.UserID_User12))
                .Verifiable();

            _mockNotificationsController.Object.UpdateNotificationAction(notificationAction);

            _mockDataService.Verify();
        }

        #endregion

        #region DeleteNotificationAction

        [Test]
        public void DeleteNotificationAction_Calls_DataService_DeleteNotificationAction()
        {
            _mockDataService.Setup(ds => ds.DeleteNotificationAction(Constants.Messaging_NotificationActionId)).Verifiable();
            _notificationsController.DeleteNotificationAction(Constants.Messaging_NotificationActionId);
            _mockDataService.Verify();
        }

        #endregion

        #region GetNotificationAction

        [Test]
        public void GetNotificationAction_Calls_DataService_GetNotificationAction()
        {
            _mockDataService
                .Setup(ds => ds.GetNotificationAction(Constants.Messaging_NotificationActionId))
                .Returns(_dtNotificationActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationAction(Constants.Messaging_NotificationActionId);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationAction_Returns_Valid_Object()
        {
            var expectedNotificationAction = CreateValidNotificationAction();

            _dtNotificationActions.Clear();

            _dtNotificationActions.Rows.Add(
                expectedNotificationAction.NotificationActionId,
                expectedNotificationAction.NotificationId,
                expectedNotificationAction.NotificationTypeActionId,
                expectedNotificationAction.Key);

            _mockDataService
                .Setup(ds => ds.GetNotificationAction(Constants.Messaging_NotificationActionId))
                .Returns(_dtNotificationActions.CreateDataReader());

            var actualNotificationAction = _notificationsController.GetNotificationAction(Constants.Messaging_NotificationActionId);

            Assert.IsTrue(new NotificationActionComparer().Equals(expectedNotificationAction, actualNotificationAction));
        }

        [Test]
        public void GetNotificationAction_Overload_Calls_DataService_GetNotificationActionByMessageAndNotificationTypeAction()
        {
            _mockDataService
                .Setup(ds => ds.GetNotificationActionByMessageAndNotificationTypeAction(Constants.Messaging_MessageId_1, Constants. Messaging_NotificationTypeActionId))
                .Returns(_dtNotificationActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationAction(Constants.Messaging_MessageId_1, Constants.Messaging_NotificationTypeActionId);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationAction_Overload_Returns_Valid_Object()
        {
            var expectedNotificationAction = CreateValidNotificationAction();

            _dtNotificationActions.Clear();

            _dtNotificationActions.Rows.Add(
                expectedNotificationAction.NotificationActionId,
                expectedNotificationAction.NotificationId,
                expectedNotificationAction.NotificationTypeActionId,
                expectedNotificationAction.Key);

            _mockDataService
                .Setup(ds => ds.GetNotificationActionByMessageAndNotificationTypeAction(Constants.Messaging_MessageId_1, Constants.Messaging_NotificationTypeActionId))
                .Returns(_dtNotificationActions.CreateDataReader());

            var actualNotificationAction = _notificationsController.GetNotificationAction(Constants.Messaging_MessageId_1, Constants.Messaging_NotificationTypeActionId);

            Assert.IsTrue(new NotificationActionComparer().Equals(expectedNotificationAction, actualNotificationAction));
        }

        #endregion

        #region GetNotificationActionsByNotificationId

        [Test]
        public void GetNotificationActionsByMessageId_Calls_DataService_GetNotificationActionsByMessageId()
        {
            _mockDataService
                .Setup(ds => ds.GetNotificationActionsByMessageId(Constants.Messaging_MessageId_1))
                .Returns(_dtNotificationActions.CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotificationActionsByNotificationId(Constants.Messaging_MessageId_1);

            _mockDataService.Verify();
        }

        [Test]
        public void GetNotificationActionsByMessageId_Returns_Valid_Object()
        {
            var expectedNotificationAction = CreateValidNotificationAction();

            _dtNotificationActions.Clear();

            _dtNotificationActions.Rows.Add(
                expectedNotificationAction.NotificationActionId,
                expectedNotificationAction.NotificationId,
                expectedNotificationAction.NotificationTypeActionId,
                expectedNotificationAction.Key);

            _mockDataService
                .Setup(ds => ds.GetNotificationActionsByMessageId(Constants.Messaging_MessageId_1))
                .Returns(_dtNotificationActions.CreateDataReader());

            var actualNotificationActions = _notificationsController.GetNotificationActionsByNotificationId(Constants.Messaging_MessageId_1);

            Assert.AreEqual(1, actualNotificationActions.Count);
            Assert.IsTrue(new NotificationActionComparer().Equals(expectedNotificationAction, actualNotificationActions[0]));
        }

        #endregion

        #region CreateNotification

        [Test]
        public void CreateNotification_Calls_Overload_With_Admin_Sender()
        {
            var adminUser = new UserInfo
            {
                UserID = Constants.UserID_Admin,
                DisplayName = Constants.UserDisplayName_Admin,
                PortalID = Constants.CONTENT_ValidPortalId
            };

            _mockNotificationsController.Setup(nc => nc.GetAdminUser()).Returns(adminUser);

            _mockNotificationsController
                .Setup(nc => nc.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction,
                    It.IsAny<IList<RoleInfo>>(),
                    It.IsAny<IList<UserInfo>>(),
                    adminUser))
                .Verifiable();

            _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                new List<RoleInfo>(),
                new List<UserInfo>());

            _mockNotificationsController.Verify();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotification_Throws_On_Null_Or_Empty_Subject_And_Body(string subject, string body)
        {
            _notificationsController.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                subject,
                body,
                Constants.Messaging_IncludeDismissAction,
                new List<RoleInfo>(),
                new List<UserInfo>(),
                new UserInfo());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotification_Throws_On_Null_Roles_And_Users()
        {
            _notificationsController.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                null,
                null,
                new UserInfo());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotification_Throws_On_Large_Subject()
        {
            var subject = new StringBuilder();
            for (var i = 0; i <= 40; i++)
            {
                subject.Append("1234567890");
            }

            _notificationsController.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                subject.ToString(),
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                new List<RoleInfo>(),
                new List<UserInfo>(),
                new UserInfo());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotification_Throws_On_Roles_And_Users_With_No_DisplayNames()
        {
            _notificationsController.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                new List<RoleInfo>(),
                new List<UserInfo>(),
                new UserInfo());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateNotification_Throws_On_Large_To_List()
        {
            var roles = new List<RoleInfo>();
            var users = new List<UserInfo>();

            for (var i = 0; i <= 100; i++)
            {
                roles.Add(new RoleInfo { RoleName = "1234567890" });
                users.Add(new UserInfo { DisplayName = "1234567890" });
            }

            _notificationsController.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                users,
                new UserInfo());
        }

        [Test]
        [ExpectedException(typeof(RecipientLimitExceededException))]
        public void CreateNotification_Throws_On_Recipient_Limit_Exceeded()
        {
            var adminUser = new UserInfo { PortalID = Constants.CONTENT_ValidPortalId };

            _mockNotificationsController.Setup(nc => nc.GetAdminUser()).Returns(adminUser);

            var roles = new List<RoleInfo>
                            {
                                new RoleInfo { RoleName = "Role1" },
                                new RoleInfo { RoleName = "Role2" }
                            };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(1);

            _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                null,
                new UserInfo());
        }

        [Test]
        public void CreateNotification_Filters_Input_When_ProfanityFilter_Is_Enabled()
        {
            const string expectedSubjectFiltered = "subject_filtered";
            const string expectedBodyFiltered = "body_filtered";

            var adminUser = new UserInfo
            {
                UserID = Constants.UserID_Admin,
                DisplayName = Constants.UserDisplayName_Admin,
                PortalID = Constants.CONTENT_ValidPortalId
            };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(10);

            var roles = new List<RoleInfo>();
            var users = new List<UserInfo>
                            {
                                new UserInfo
                                    {
                                        UserID = Constants.UserID_User12,
                                        DisplayName = Constants.UserDisplayName_User12
                                    }
                            };

            _mockDataService
                .Setup(ds => ds.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.UserDisplayName_User12,
                    Constants.UserDisplayName_Admin,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction, 
                    Constants.UserID_Admin, 
                    It.IsAny<int>(), 
                    It.IsAny<DateTime>()));

            _mockNotificationsController
                .Setup(mc => mc.GetPortalSetting("MessagingProfanityFilters", It.IsAny<int>(), It.IsAny<string>()))
                .Returns("YES");

            _mockNotificationsController.Setup(mc => mc.InputFilter(Constants.Messaging_NotificationSubject)).Returns(expectedSubjectFiltered);
            _mockNotificationsController.Setup(mc => mc.InputFilter(Constants.Messaging_NotificationBody)).Returns(expectedBodyFiltered);
            _mockNotificationsController.Setup(nc => nc.GetExpirationDate(It.IsAny<int>())).Returns(DateTime.MinValue);

            var notification = _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                users,
                adminUser);

            Assert.AreEqual(expectedSubjectFiltered, notification.Subject);
            Assert.AreEqual(expectedBodyFiltered, notification.Body);
        }

        [Test]
        public void CreateNotification_Calls_DataService_CreateNotification_On_Valid_Notification()
        {
            var adminUser = new UserInfo
                                {
                                    UserID = Constants.UserID_Admin,
                                    DisplayName = Constants.UserDisplayName_Admin,
                                    PortalID = Constants.CONTENT_ValidPortalId
                                };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(10);

            var roles = new List<RoleInfo>();
            var users = new List<UserInfo>
                            {
                                new UserInfo
                                    {
                                        UserID = Constants.UserID_User12,
                                        DisplayName = Constants.UserDisplayName_User12
                                    }
                            };

            _mockDataService
                .Setup(ds => ds.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.UserDisplayName_User12,
                    Constants.UserDisplayName_Admin,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction, 
                    Constants.UserID_Admin, 
                    It.IsAny<int>(), 
                    It.IsAny<DateTime>()))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetExpirationDate(It.IsAny<int>())).Returns(DateTime.MinValue);

            _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                users,
                adminUser);

            _mockDataService.Verify();
        }

        [Test]
        public void CreateNotification_Calls_Messaging_DataService_CreateSocialMessageRecipientsForRole_When_Passing_Roles()
        {
            var adminUser = new UserInfo
            {
                UserID = Constants.UserID_Admin,
                DisplayName = Constants.UserDisplayName_Admin,
                PortalID = Constants.CONTENT_ValidPortalId
            };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(10);

            var roles = new List<RoleInfo>
                            {
                                new RoleInfo
                                    {
                                        RoleID = Constants.RoleID_RegisteredUsers,
                                        RoleName = Constants.RoleName_RegisteredUsers
                                    }
                            };
            var users = new List<UserInfo>();

            _mockDataService
                .Setup(ds => ds.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.RoleName_RegisteredUsers,
                    Constants.UserDisplayName_Admin,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction, 
                    Constants.UserID_Admin, 
                    It.IsAny<int>(), 
                    It.IsAny<DateTime>()))
                .Returns(Constants.Messaging_MessageId_1);

            _mockMessagingDataService
                .Setup(mds => mds.CreateMessageRecipientsForRole(
                    Constants.Messaging_MessageId_1,
                    Constants.RoleID_RegisteredUsers.ToString(CultureInfo.InvariantCulture),
                    It.IsAny<int>()))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetExpirationDate(It.IsAny<int>())).Returns(DateTime.MinValue);

            _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                users,
                adminUser);

            _mockMessagingDataService.Verify();
        }

        [Test]
        public void CreateNotification_Calls_Messaging_DataService_SaveSocialMessageRecipient_When_Passing_Users()
        {
            var adminUser = new UserInfo
            {
                UserID = Constants.UserID_Admin,
                DisplayName = Constants.UserDisplayName_Admin,
                PortalID = Constants.CONTENT_ValidPortalId
            };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(10);

            var roles = new List<RoleInfo>();
            var users = new List<UserInfo>
                            {
                                new UserInfo
                                    {
                                        UserID = Constants.UserID_User12, 
                                        DisplayName = Constants.UserDisplayName_User12
                                    }
                            };

            _mockDataService
                .Setup(ds => ds.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.UserDisplayName_User12,
                    Constants.UserDisplayName_Admin,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction, 
                    Constants.UserID_Admin, 
                    It.IsAny<int>(), 
                    It.IsAny<DateTime>()))
                .Returns(Constants.Messaging_MessageId_1);

            _mockMessagingController
                .Setup(mc => mc.GetMessageRecipient(
                    Constants.Messaging_MessageId_1,
                    Constants.UserID_User12))
                .Returns((MessageRecipient)null);

            _mockMessagingDataService
                .Setup(mds => mds.SaveMessageRecipient(
                    It.Is<MessageRecipient>(mr => 
                                            mr.MessageID == Constants.Messaging_MessageId_1 && 
                                            mr.UserID == Constants.UserID_User12 && 
                                            mr.Read == false && 
                                            mr.RecipientID == Null.NullInteger),
                    It.IsAny<int>()))
                .Verifiable();

            _mockNotificationsController.Setup(nc => nc.GetExpirationDate(It.IsAny<int>())).Returns(DateTime.MinValue);

            _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction,
                roles,
                users,
                adminUser);

            _mockMessagingDataService.Verify();
        }

        [Test]
        public void CreateNotification_Returns_Valid_Object()
        {
            var expectedNotification = CreateValidNotification();

            var adminUser = new UserInfo
            {
                UserID = Constants.UserID_Admin,
                DisplayName = Constants.UserDisplayName_Admin,
                PortalID = Constants.CONTENT_ValidPortalId
            };

            _mockMessagingController.Setup(mc => mc.RecipientLimit(adminUser.PortalID)).Returns(10);

            var roles = new List<RoleInfo>();
            var users = new List<UserInfo>
                            {
                                new UserInfo
                                    {
                                        UserID = Constants.UserID_User12, 
                                        DisplayName = Constants.UserDisplayName_User12
                                    }
                            };

            _mockDataService
                .Setup(ds => ds.CreateNotification(
                    Constants.Messaging_NotificationTypeId,
                    Constants.PORTAL_Zero,
                    Constants.UserDisplayName_User12,
                    Constants.UserDisplayName_Admin,
                    Constants.Messaging_NotificationSubject,
                    Constants.Messaging_NotificationBody,
                    Constants.Messaging_IncludeDismissAction, 
                    Constants.UserID_Admin, 
                    It.IsAny<int>(), 
                    It.IsAny<DateTime>()))
                .Returns(Constants.Messaging_MessageId_1);

            _mockMessagingDataService
                .Setup(mds => mds.CreateMessageRecipientsForRole(
                    Constants.Messaging_MessageId_1,
                    Constants.RoleID_RegisteredUsers.ToString(CultureInfo.InvariantCulture),
                    It.IsAny<int>()));

            _mockMessagingController
                .Setup(mc => mc.GetMessageRecipient(
                    Constants.Messaging_MessageId_1,
                    Constants.UserID_User12))
                .Returns((MessageRecipient)null);

            _mockMessagingDataService
                .Setup(mds => mds.SaveMessageRecipient(
                    It.Is<MessageRecipient>(mr =>
                                            mr.MessageID == Constants.Messaging_MessageId_1 &&
                                            mr.UserID == Constants.UserID_User12 &&
                                            mr.Read == false &&
                                            mr.RecipientID == Null.NullInteger),
                    It.IsAny<int>()));

            _mockNotificationsController.Setup(nc => nc.GetExpirationDate(It.IsAny<int>())).Returns(DateTime.MinValue);

            var actualNotification = _mockNotificationsController.Object.CreateNotification(
                Constants.Messaging_NotificationTypeId,
                Constants.PORTAL_Zero,
                Constants.Messaging_NotificationSubject,
                Constants.Messaging_NotificationBody,
                Constants.Messaging_IncludeDismissAction, 
                roles,
                users,
                adminUser);

            Assert.IsTrue(new NotificationComparer().Equals(expectedNotification, actualNotification));
        }

        #endregion

        #region DeleteNotification

        [Test]
        public void DeleteNotification_Calls_DataService_DeleteNotification()
        {
            _mockDataService.Setup(ds => ds.DeleteNotification(Constants.Messaging_MessageId_1)).Verifiable();
            _notificationsController.DeleteNotification(Constants.Messaging_MessageId_1);
            _mockDataService.Verify();
        }

        #endregion

        #region GetNotifications

        [Test]
        public void GetNotifications_Calls_DataService_GetNotifications()
        {
            _mockDataService
                .Setup(ds => ds.GetNotifications(Constants.UserID_User12, Constants.PORTAL_Zero,It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new DataTable().CreateDataReader())
                .Verifiable();

            _notificationsController.GetNotifications(Constants.UserID_User12, Constants.PORTAL_Zero, 0, 10);
            _mockDataService.Verify();
        }

        #endregion

        #region CountNotifications

        [Test]
        public void CountNotifications_Calls_DataService_CountNotifications()
        {
            _mockDataService.Setup(ds => ds.CountNotifications(Constants.UserID_User12, Constants.PORTAL_Zero)).Verifiable();
            _notificationsController.CountNotifications(Constants.UserID_User12,Constants.PORTAL_Zero);
            _mockDataService.Verify();
        }

        #endregion

        #region DeleteNotificationRecipient

        [Test]
        public void DeleteNotificationRecipient_Calls_MessagingController_DeleteMessageRecipient()
        {
            _mockMessagingController.Setup(mc => mc.DeleteMessageRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12)).Verifiable();
            _mockMessagingController.Setup(mc => mc.GetMessageRecipients(Constants.Messaging_MessageId_1)).Returns(new List<MessageRecipient>());
            _notificationsController.DeleteNotificationRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12);
            _mockMessagingController.Verify();
        }

        [Test]
        public void DeleteNotificationRecipient_Does_Not_Delete_Notification_When_There_Are_More_Recipients()
        {
            var messageRecipients = new List<MessageRecipient>
                                        {
                                            new MessageRecipient()
                                        };

            _mockMessagingController.Setup(mc => mc.DeleteMessageRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12));
            _mockMessagingController.Setup(mc => mc.GetMessageRecipients(Constants.Messaging_MessageId_1)).Returns(messageRecipients);
            _mockNotificationsController.Object.DeleteNotificationRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12);
            
            _mockNotificationsController.Verify(nc => nc.DeleteNotification(Constants.Messaging_MessageId_1), Times.Never());
        }

        [Test]
        public void DeleteNotificationRecipient_Deletes_Notification_When_There_Are_No_More_Recipients()
        {
            _mockMessagingController.Setup(mc => mc.DeleteMessageRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12));
            _mockMessagingController.Setup(mc => mc.GetMessageRecipients(Constants.Messaging_MessageId_1)).Returns(new List<MessageRecipient>());
            _mockNotificationsController.Object.DeleteNotificationRecipient(Constants.Messaging_MessageId_1, Constants.UserID_User12);
            _mockNotificationsController.Verify(nc => nc.DeleteNotification(Constants.Messaging_MessageId_1));
        }

        #endregion

        #region Private Methods

        private static Notification CreateValidNotification()
        {
            return new Notification
            {
                NotificationID = Constants.Messaging_MessageId_1,
                NotificationTypeID = Constants.Messaging_NotificationTypeId,
                Subject = Constants.Messaging_NotificationSubject,
                Body = Constants.Messaging_NotificationBody,
                To = Constants.UserDisplayName_User12,
                From = Constants.UserDisplayName_Admin,
                SenderUserID = Constants.UserID_Admin
            };
        }

        private static NotificationType CreateValidNotificationType()
        {
            return new NotificationType
            {
                NotificationTypeId = Constants.Messaging_NotificationTypeId,
                Name = Constants.Messaging_NotificationTypeName,
                Description = Constants.Messaging_NotificationTypeDescription,
                TimeToLive = new TimeSpan(0, Constants.Messaging_NotificationTypeTTL, 0),
                DesktopModuleId = Constants.Messaging_NotificationTypeDesktopModuleId
            };
        }

        private static NotificationTypeAction CreateValidNotificationTypeAction()
        {
            return new NotificationTypeAction
            {
                NotificationTypeActionId = Constants.Messaging_NotificationTypeActionId,
                NotificationTypeId = Constants.Messaging_NotificationTypeId,
                NameResourceKey = Constants.Messaging_NotificationTypeActionNameResourceKey,
                DescriptionResourceKey = Constants.Messaging_NotificationTypeActionDescriptionResourceKey,
                ConfirmResourceKey =  Constants.Messaging_NotificationTypeActionConfirmResourceKey,
                APICall = Constants.Messaging_NotificationTypeActionAPICall
            };
        }

        private static NotificationAction CreateValidNotificationAction()
        {
            return new NotificationAction
            {
                NotificationActionId = Constants.Messaging_NotificationActionId,
                NotificationId = Constants.Messaging_MessageId_1,
                NotificationTypeActionId = Constants.Messaging_NotificationTypeActionId,
                Key = Constants.Messaging_NotificationActionKey
            };
        }

        private void SetupDataTables()
        {
            _dtNotificationTypes = new DataTable();
            _dtNotificationTypes.Columns.Add("NotificationTypeID", typeof(int));
            _dtNotificationTypes.Columns.Add("Name", typeof(string));
            _dtNotificationTypes.Columns.Add("Description", typeof(string));
            _dtNotificationTypes.Columns.Add("TTL", typeof(int));
            _dtNotificationTypes.Columns.Add("DesktopModuleID", typeof(int));
            _dtNotificationTypes.Columns.Add("CreatedByUserID", typeof(int));
            _dtNotificationTypes.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtNotificationTypes.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtNotificationTypes.Columns.Add("LastModifiedOnDate", typeof(DateTime));

            _dtNotificationTypeActions = new DataTable();
            _dtNotificationTypeActions.Columns.Add("NotificationTypeActionID", typeof(int));
            _dtNotificationTypeActions.Columns.Add("NotificationTypeID", typeof(int));
            _dtNotificationTypeActions.Columns.Add("NameResourceKey", typeof(string));
            _dtNotificationTypeActions.Columns.Add("DescriptionResourceKey", typeof(string));
            _dtNotificationTypeActions.Columns.Add("ConfirmResourceKey", typeof(string));
            _dtNotificationTypeActions.Columns.Add("Order", typeof(int));
            _dtNotificationTypeActions.Columns.Add("APICall", typeof(string));
            _dtNotificationTypeActions.Columns.Add("CreatedByUserID", typeof(int));
            _dtNotificationTypeActions.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtNotificationTypeActions.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtNotificationTypeActions.Columns.Add("LastModifiedOnDate", typeof(DateTime));

            _dtNotificationActions = new DataTable();
            _dtNotificationActions.Columns.Add("NotificationActionID");
            _dtNotificationActions.Columns.Add("MessageID");
            _dtNotificationActions.Columns.Add("NotificationTypeActionID");
            _dtNotificationActions.Columns.Add("Key");
            _dtNotificationActions.Columns.Add("CreatedByUserID", typeof(int));
            _dtNotificationActions.Columns.Add("CreatedOnDate", typeof(DateTime));
            _dtNotificationActions.Columns.Add("LastModifiedByUserID", typeof(int));
            _dtNotificationActions.Columns.Add("LastModifiedOnDate", typeof(DateTime));
        }

        #endregion

        #region Private Classes

        private class NotificationTypeComparer : IEqualityComparer<NotificationType>
        {
            public bool Equals(NotificationType x, NotificationType y)
            {
                if (x == null) return y == null;
                if (y == null) return false;

                return
                    x.NotificationTypeId == y.NotificationTypeId &&
                    x.Name == y.Name &&
                    x.Description == y.Description &&
                    x.TimeToLive == y.TimeToLive &&
                    x.DesktopModuleId == y.DesktopModuleId;
            }

            public int GetHashCode(NotificationType obj)
            {
                throw new NotImplementedException();
            }
        }

        private class NotificationTypeActionComparer : IEqualityComparer<NotificationTypeAction>
        {
            public bool Equals(NotificationTypeAction x, NotificationTypeAction y)
            {
                if (x == null) return y == null;
                if (y == null) return false;

                return
                    x.NotificationTypeActionId == y.NotificationTypeActionId &&
                    x.NotificationTypeId == y.NotificationTypeId &&
                    x.NameResourceKey == y.NameResourceKey &&
                    x.DescriptionResourceKey == y.DescriptionResourceKey &&
                    x.ConfirmResourceKey == y.ConfirmResourceKey &&
                    x.APICall == y.APICall;
            }

            public int GetHashCode(NotificationTypeAction obj)
            {
                throw new NotImplementedException();
            }
        }

        private class NotificationActionComparer : IEqualityComparer<NotificationAction>
        {
            public bool Equals(NotificationAction x, NotificationAction y)
            {
                if (x == null) return y == null;
                if (y == null) return false;

                return
                    x.NotificationActionId == y.NotificationActionId &&
                    x.NotificationId == y.NotificationId &&
                    x.NotificationTypeActionId == y.NotificationTypeActionId &&
                    x.Key == y.Key;
            }

            public int GetHashCode(NotificationAction obj)
            {
                throw new NotImplementedException();
            }
        }

        private class NotificationComparer : IEqualityComparer<Notification>
        {
            public bool Equals(Notification x, Notification y)
            {
                if (x == null) return y == null;
                if (y == null) return false;

                return
                    x.NotificationID == y.NotificationID &&
                    x.NotificationTypeID == y.NotificationTypeID &&
                    x.Subject == y.Subject &&
                    x.Body == y.Body &&
                    x.To == y.To &&
                    x.From == y.From &&
                    x.SenderUserID == y.SenderUserID;
            }

            public int GetHashCode(Notification obj)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
// ReSharper restore InconsistentNaming