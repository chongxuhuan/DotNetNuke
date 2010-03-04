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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views;
using DotNetNuke.Services.Cache;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.UI.Mvp;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using Moq;
using MbUnit.Framework;
using DotNetNuke.Tests.Content.Mocks;

namespace DotNetNuke.Tests.Content.Presenters
{
    /// <summary>
    /// Summary description for CreateVocabularyPresenter Tests
    /// </summary>
    [TestFixture]
    public class CreateVocabularyPresenterTests: PresenterTestBase<CreateVocabularyPresenter, ICreateVocabularyView, CreateVocabularyPresenterModel>
    {

        private Mock<CachingProvider> mockCache;

        [SetUp()]
        public void SetUp()
        {
            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #region Initialization Tests

       [Test]
        public void CreateVocabularyPresenter_Constructor_Requires_Non_Null_VocabularyController()
        {
            AutoTester.ArgumentNull<IVocabularyController>(m =>
                        new CreateVocabularyPresenter(m, new Mock<IScopeTypeController>().Object));
        }

       [Test]
        public void CreateVocabularyPresenter_Constructor_Requires_Non_NullScopeTypeController()
        {
            AutoTester.ArgumentNull<IScopeTypeController>(s =>
                        new CreateVocabularyPresenter(new Mock<IVocabularyController>().Object, s));
        }

       [Test]
        public void CreateVocabularyPresenter_Initialize_Requires_Non_Null_View()
        {
            RunInitializeWithNullViewTest();
        }

       [Test]
        public void CreateVocabularyPresenter_Initialize_Requires_Non_Null_Model()
        {
            RunInitializeWithNullModelTest();
        }

       [Test]
        public void CreateVocabularyPresenter_Initialize_Requires_Non_Null_Environment()
        {
            RunInitializeWithNullEnvironmentTest();
        }

        #endregion

        #region View Load Tests

       [Test]
        public void CreateVocabularyPresenter_Does_Nothing_On_View_Load_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.BindVocabulary(It.IsAny<Vocabulary>(), false))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert (done by the call to Never() above)
        }

       [Test]
        public void CreateVocabularyPresenter_Loads_New_Vocabulary_On_View_Load()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.TabId = Constants.TAB_ValidId;
            presenter.Model.ModuleId = Constants.MODULE_ValidId;
            presenter.Model.HasPermission = true;

            // Act
            presenter.LoadInternal();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindVocabulary(It.Is<Vocabulary>(vm => vm.VocabularyId == Null.NullInteger), 
                                                                            presenter.Model.IsSuperUser));
        }

       [Test]
        public void CreateVocabularyPresenter_Redirects_To_AccessDenied_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.BindVocabulary(It.IsAny<Vocabulary>(),
                                                    presenter.Model.IsSuperUser))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToAccessDenied());
        }

        #endregion

        #region Cancel Tests

       [Test]
        public void CreateVocabularyPresenter_Cancel_Redirects_To_Vocabulary_List_View()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();

            // Act
            presenter.Cancel();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<VocabularyListPresenterModel>()));
        }

        #endregion

        #region SaveVocabulary Tests

       [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Validates_Vocabulary()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            Vocabulary newVocabulary = ContentTestHelper.CreateValidVocabulary();

            var mockValidator = MockHelper.EnableValidMockValidator(presenter.Validator, newVocabulary);

            presenter.Vocabulary = newVocabulary;

            // Act
            presenter.SaveVocabulary();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(newVocabulary));
        }

       [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Does_Not_Save_If_Vocabulary_Invalid()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            Vocabulary newVocabulary = ContentTestHelper.CreateValidVocabulary();

            Mock.Get(presenter.VocabularyController)
                .Setup(r => r.AddVocabulary(newVocabulary))
                .Never();

            var mockValidator = MockHelper.EnableInvalidMockValidator(presenter.Validator, newVocabulary);

            presenter.Vocabulary = newVocabulary;

            // Act
            presenter.SaveVocabulary();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(newVocabulary));
        }

       [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Saves_If_Vocabulary_Valid()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            Vocabulary newVocabulary = ContentTestHelper.CreateValidVocabulary();

            presenter.Vocabulary = newVocabulary;

            // Act
            presenter.SaveVocabulary();

            // Assert
            Mock.Get(presenter.VocabularyController)
                .Verify(r => r.AddVocabulary(It.Is<Vocabulary>(v => v.VocabularyId == newVocabulary.VocabularyId)));
        }

       [Test]
        public void CreateVocabularyPresenter_SaveVocabulary_Redirects_To_Vocabulary_List_View_With_No_Errors()
        {
            // Arrange
            CreateVocabularyPresenter presenter = CreatePresenter();
            Vocabulary newVocabulary = ContentTestHelper.CreateValidVocabulary();

            presenter.Vocabulary = newVocabulary;

            // Act
            presenter.SaveVocabulary();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<VocabularyListPresenterModel>()));
        }

        #endregion

        #region Helpers

        protected override CreateVocabularyPresenter ConstructPresenter()
        {
            return new CreateVocabularyPresenter(MockHelper.CreateMockVocabularyController().Object, 
                                                    MockHelper.CreateMockScopeTypeController().Object);
        }

        #endregion
    }
}
