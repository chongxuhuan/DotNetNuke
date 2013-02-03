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

#region Usings

using System;
using System.Collections.Generic;

using DotNetNuke.Entities.Controllers;
using DotNetNuke.Modules.Admin.Models;
using DotNetNuke.Modules.Admin.Presenters;
using DotNetNuke.Modules.Admin.Views;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

#endregion

namespace DotNetNuke.Tests.Admin
{
    [TestFixture]
    public class HostSettingsPresenterTests
    {
        #region Constructor Tests

        [Test]
        public void HostSettingsPresenter_Constructor_Requires_Non_Null_HostController()
        {
            //Arrange
            var view = new Mock<IHostSettingsView>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new HostSettingsPresenter(view.Object, null));
        }

        #endregion

        #region View Initialize Tests

        public void HostSettingsPresenter_OnLoad()
        {
            //TODO: the code is not currently testable due to dependencies
        }

        #endregion

        #region View Load Tests

        [Test]
        public void HostSettingsPresenter_OnLoad_Calls_View_BindVocabulary()
        {
            // Arrange
            var mockView = new Mock<IHostSettingsView>();
            mockView.Setup(v => v.Model).Returns(new HostSettingsModel());
            var mockController = new Mock<IHostController>();

            var presenter = new HostSettingsPresenter(mockView.Object, mockController.Object);

            // Act (Raise the Load Event)
            mockView.Raise(v => v.Load += null, EventArgs.Empty);

            // Assert
            mockView.Verify(v => v.BindSettings());
        }

        #endregion

        #region SaveSettings Tests

        [Test]
        public void HostSettingsPresenter_SaveSettings_Calls_HostController_Update()
        {
            //Arrange
            var mockCache = MockComponentProvider.CreateDataCacheProvider();

            var mockView = new Mock<IHostSettingsView>();
            var model = new HostSettingsModel { Settings = new Dictionary<string, string>
                                                      {
                                                          {"String_1_S", "MyValue"},
                                                          {"String_2_S", "MyNextValue"},
                                                          {"String_3_U", "MyOtherValue"},
                                                      }
                                                };
            mockView.Setup(v => v.Model).Returns(model);

            var mockController = new Mock<IHostController>();

            var presenter = new HostSettingsPresenter(mockView.Object, mockController.Object);

            //Act (Raise the Save Event)
            mockView.Raise(v => v.SaveSettings += null, EventArgs.Empty);

            //Assert
            mockController.Verify((c) => c.Update(It.IsAny<Dictionary<string,string>>()));
        }

        [Test]
        public void HostSettingsPresenter_SaveSettings_Removes_Config_Settings_Before_Update()
        {
            //Arrange
            var mockCache = MockComponentProvider.CreateDataCacheProvider();

            var mockView = new Mock<IHostSettingsView>();
            var model = new HostSettingsModel
            {
                Settings = new Dictionary<string, string>
                                                      {
                                                          {"Config_1", "MyValue"},
                                                          {"Config_2", "MyNextValue"},
                                                          {"Config_3", "MyOtherValue"},
                                                      }
            };
            mockView.Setup(v => v.Model).Returns(model);

            var mockController = new Mock<IHostController>();

            var presenter = new HostSettingsPresenter(mockView.Object, mockController.Object);

            //Act (Raise the Save Event)
            mockView.Raise(v => v.SaveSettings += null, EventArgs.Empty);

            //Assert
            mockController.Verify((c) => c.Update(It.IsAny<Dictionary<string, string>>()), Times.Never());
        }


        #endregion
    }
}