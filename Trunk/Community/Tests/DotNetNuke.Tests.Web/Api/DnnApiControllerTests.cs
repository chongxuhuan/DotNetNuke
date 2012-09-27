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
using System.Globalization;
using System.Net.Http;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Tabs.Internal;
using DotNetNuke.Web.Api;
using Moq;
using NUnit.Framework;

namespace DotNetNuke.Tests.Web.Api
{
    [TestFixture]
    public class DnnApiControllerTests
    {
        private Mock<IModuleController> _mockModuleController;
        private IModuleController _moduleController;
        private Mock<ITabController> _mockTabController;
        private ITabController _tabController;

        private const int ValidPortalId = 0;
        private const int ValidModuleId = 456;
        private const int ValidTabId = 46;

        [SetUp]
        public void Setup()
        {
            RegisterMock(TestableModuleController.SetTestableInstance, out _mockModuleController, out _moduleController);
            RegisterMock(TestableTabController.SetTestableInstance, out _mockTabController, out _tabController);
        }

        [Test]
        public void LoadsActiveModule()
        {
            //Arrange
            var controller = new DnnApiControllerHelper();
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add("tabid", ValidTabId.ToString(CultureInfo.InvariantCulture));
            requestMessage.Headers.Add("moduleid", ValidModuleId.ToString(CultureInfo.InvariantCulture));
            controller.Request = requestMessage;

            _mockTabController.Setup(x => x.GetTab(ValidTabId, ValidPortalId)).Returns(new TabInfo());
            var moduleInfo = new ModuleInfo();
            _mockModuleController.Setup(x => x.GetModule(ValidModuleId, ValidTabId)).Returns(moduleInfo);

            //Act
            int tabId;
            controller.PublicValidateTabAndModuleContext(ValidPortalId, out tabId);

            //Assert
            Assert.AreSame(moduleInfo, controller.ActiveModule);
        }

        [Test]
        public void OmittedTabIdWillNotLoadModule()
        {
            //Arrange
            var controller = new DnnApiControllerHelper();
            //no tabid
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add("moduleid", ValidModuleId.ToString(CultureInfo.InvariantCulture));
            controller.Request = requestMessage;

            //Act
            int tabId;
            controller.PublicValidateTabAndModuleContext(ValidPortalId, out tabId);

            //Assert
            _mockTabController.Verify(x => x.GetTab(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockModuleController.Verify(x => x.GetModule(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            Assert.IsNull(controller.ActiveModule);
        }

        [Test]
        public void OmittedModuleIdWillNotLoadModule()
        {
            //Arrange
            var controller = new DnnApiControllerHelper();
            //no moduleid
            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add("tabid", ValidTabId.ToString(CultureInfo.InvariantCulture));
            controller.Request = requestMessage;

            _mockTabController.Setup(x => x.GetTab(ValidTabId, ValidPortalId)).Returns(new TabInfo());

            //Act
            int tabId;
            controller.PublicValidateTabAndModuleContext(ValidPortalId, out tabId);

            //Assert
            _mockModuleController.Verify(x => x.GetModule(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            Assert.IsNull(controller.ActiveModule);
        }

        private void RegisterMock<T>(Action<T> register, out Mock<T> mock, out T instance) where T : class
        {
            mock = new Mock<T>();
            instance = mock.Object;
            register(instance);
        }

        internal class DnnApiControllerHelper : DnnApiController
        {
            public void PublicValidateTabAndModuleContext(int portalId, out int tabId)
            {
                ValidateTabAndModuleContext(portalId, out tabId);
            }
        }
    }
}