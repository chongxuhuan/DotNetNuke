/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
*/

using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Utilities.Mocks;
using MbUnit.Framework;
using Moq;
using DotNetNuke.Modules.Taxonomy.Views;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views.Models;
using System;
using DotNetNuke.Common.Utilities;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Web.Validators;
using DotNetNuke.Tests.Content.Mocks;

namespace DotNetNuke.Tests.Content.Presenters
{
    /// <summary>
    /// Summary description for CreateVocabularyPresenter Tests
    /// </summary>
    [TestFixture]
    public class CreateVocabularyPresenterTests
    {
        #region Private Members

        private Mock<CachingProvider> mockCache;

        #endregion

        #region SetUp and TearDown

        [SetUp()]
        public void SetUp()
        {
            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void CreateVocabularyPresenter_Constructor_Requires_Non_Null_VocabularyController()
        {
            //Arrange
            Mock<ICreateVocabularyView> view = new Mock<ICreateVocabularyView>();
            Mock<IScopeTypeController> scopeTypeController = new Mock<IScopeTypeController>();

            //Act, Assert
            AutoTester.ArgumentNull<IVocabularyController>(v => new CreateVocabularyPresenter(view.Object, v, scopeTypeController.Object));
        }

        [Test]
        public void CreateVocabularyPresenter_Constructor_Requires_Non_Null_ScopeTypeController()
        {
            //Arrange
            Mock<ICreateVocabularyView> view = new Mock<ICreateVocabularyView>();
            Mock<IVocabularyController> vocabularyController = new Mock<IVocabularyController>();

            //Act, Assert
            AutoTester.ArgumentNull<IScopeTypeController>(s => new CreateVocabularyPresenter(view.Object, vocabularyController.Object, s));
        }

        [Test]
        public void CreateVocabularyPresenter_Constructor_Calls_ScopeTypeController_GetScopeTypes()
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            mockView.Setup(v => v.Model).Returns(new CreateVocabularyModel());

            // Act
            CreateVocabularyPresenter presenter = CreatePresenter(mockView);

            // Assert
            Mock.Get(presenter.ScopeTypeController)
                .Verify(c => c.GetScopeTypes());
        }

       #endregion

        #region View Load Tests

        [Test]
        [Row(true)]
        [Row(false)]
        public void CreateVocabularyPresenter_OnLoad_Calls_View_BindVocabulary(bool isSuperUser)
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            mockView.Setup(v => v.Model).Returns(new CreateVocabularyModel());

            CreateVocabularyPresenter presenter = CreatePresenter(mockView);
            presenter.IsSuperUser = isSuperUser;

            // Act (Raise the Load Event)
            mockView.Raise(v => v.Load += null, EventArgs.Empty);

            // Assert
            mockView.Verify(v => v.BindVocabulary(It.Is<Vocabulary>(vm => vm.VocabularyId == Null.NullInteger),
                                                                            isSuperUser));
        }

        #endregion

        #region Cancel Tests

        [Test]
        public void CreateVocabularyPresenter_Cancel_Redirects_To_Vocabulary_List_View()
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            mockView.Setup(v => v.Model).Returns(new CreateVocabularyModel());

            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>();

            CreateVocabularyPresenter presenter = CreatePresenter(mockView, mockHttpResponse);
            presenter.TabId = Constants.TAB_ValidId;

            // Act (Raise the Cancel Event)
            mockView.Raise(v => v.Cancel += null, EventArgs.Empty);

            // Assert
            mockHttpResponse.Verify(r => r.Redirect(Globals.NavigateURL(Constants.TAB_ValidId)));
        }

        #endregion

        #region SaveVocabulary Tests

        [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Validates_Vocabulary()
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            CreateVocabularyModel model = new CreateVocabularyModel
            {
                Vocabulary = new Vocabulary()
                {
                    VocabularyId = Null.NullInteger,
                    ScopeTypeId = 1
                }
            };
            mockView.Setup(v => v.Model).Returns(model);

            CreateVocabularyPresenter presenter = CreatePresenter(mockView);

            Mock<ObjectValidator> mockValidator = MockHelper.EnableValidMockValidator(presenter.Validator, model.Vocabulary);

            // Act
            mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            mockValidator.Verify(v => v.ValidateObject(model.Vocabulary));
        }

        [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Does_Not_Save_If_Vocabulary_Invalid()
        {
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            CreateVocabularyModel model = new CreateVocabularyModel
            {
                Vocabulary = new Vocabulary()
                {
                    VocabularyId = Null.NullInteger,
                    ScopeTypeId = 1
                }
            };
            mockView.Setup(v => v.Model).Returns(model);

            CreateVocabularyPresenter presenter = CreatePresenter(mockView);

            Mock<ObjectValidator> mockValidator = MockHelper.EnableInvalidMockValidator(presenter.Validator, model.Vocabulary);

            // Act (Raise the Save Event)
            mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            Mock.Get(presenter.VocabularyController)
               .Verify(r => r.UpdateVocabulary(model.Vocabulary), Times.Never());
        }

        [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Saves_If_Vocabulary_Valid()
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            mockView.Setup(v => v.Model).Returns(new CreateVocabularyModel());

            CreateVocabularyPresenter presenter = CreatePresenter(mockView);

            // Act (Raise the Save Event)
            mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            Mock.Get(presenter.VocabularyController)
                .Verify(c => c.AddVocabulary(mockView.Object.Model.Vocabulary));
        }

        [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Redirects_To_Vocabulary_List_View_With_No_Errors()
        {
            // Arrange
            Mock<ICreateVocabularyView> mockView = new Mock<ICreateVocabularyView>();
            mockView.Setup(v => v.Model).Returns(new CreateVocabularyModel());

            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>();

            CreateVocabularyPresenter presenter = CreatePresenter(mockView, mockHttpResponse);
            presenter.TabId = Constants.TAB_ValidId;

            // Act (Raise the Cancel Event)
            mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            mockHttpResponse.Verify(r => r.Redirect(Globals.NavigateURL(Constants.TAB_ValidId)));
        }

        #endregion

        #region Helpers

        protected CreateVocabularyPresenter CreatePresenter(Mock<ICreateVocabularyView> mockView)
        {
            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>();

            return CreatePresenter(mockView, mockHttpResponse);

        }

        protected  CreateVocabularyPresenter CreatePresenter(Mock<ICreateVocabularyView> mockView, Mock<HttpResponseBase> mockHttpResponse)
        {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(h => h.Response).Returns(mockHttpResponse.Object);

            CreateVocabularyPresenter presenter = new CreateVocabularyPresenter(mockView.Object, MockHelper.CreateMockVocabularyController().Object,
                                                    MockHelper.CreateMockScopeTypeController().Object)
            {
                HttpContext = mockHttpContext.Object
            };

            return presenter;
        }

        #endregion
    }
}
