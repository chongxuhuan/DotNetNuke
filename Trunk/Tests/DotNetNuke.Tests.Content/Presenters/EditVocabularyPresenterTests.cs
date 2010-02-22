/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.UI.Mvp;
using DotNetNuke.Tests.Utilities;
using Moq;
using MbUnit.Framework;
using DotNetNuke.Tests.Content.Mocks;

namespace DotNetNuke.Tests.Content.Presenters
{
    /// <summary>
    /// Summary description for VocabulariesListPresenter Tests
    /// </summary>
    [TestFixture]
    public class EditVocabularyPresenterTests : PresenterTestBase<EditVocabularyPresenter, IEditVocabularyView, EditVocabularyPresenterModel>
    {
        #region Private Members

        private EditVocabularyPresenter presenter;
        private Term term;
        private Vocabulary vocabulary;

        #endregion

        #region Initialization Tests

       [Test]
        public void EditVocabularyPresenter_Constructor_Requires_Non_Null_VocabularyController()
        {
            AutoTester.ArgumentNull<IVocabularyController>(v =>
                        new EditVocabularyPresenter(v,
                                                    new Mock<ITermController>().Object));
        }


       [Test]
        public void EditVocabularyPresenter_Constructor_Requires_Non_TermController()
        {
            AutoTester.ArgumentNull<ITermController>(t =>
                        new EditVocabularyPresenter(new Mock<IVocabularyController>().Object,
                                                    t));
        }

       [Test]
        public void EditVocabularyPresenter_Initialize_Requires_Non_Null_View()
        {
            RunInitializeWithNullViewTest();
        }

       [Test]
        public void EditVocabularyPresenter_Initialize_Requires_Non_Null_Model()
        {
            RunInitializeWithNullModelTest();
        }

       [Test]
        public void EditVocabularyPresenter_Initialize_Requires_Non_Null_Environment()
        {
            RunInitializeWithNullEnvironmentTest();
        }

        #endregion

        #region View Load Tests

       [Test]
        public void EditVocabularyPresenter_Does_Nothing_On_View_Load_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            EditVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.BindVocabulary(It.IsAny<Vocabulary>(), false,
                                                presenter.Model.IsSuperUser))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert (done by the call to Never() above)
        }

       [Test]
        public void EditVocabularyPresenter_Loads_Vocabulary_With_Id_Specified_In_Context_If_NotNull_And_Not_PostBack()
        {
            // Arrange
            EditVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.TabId = Constants.TAB_ValidId;
            presenter.Model.ModuleId = Constants.MODULE_ValidId;
            presenter.Model.VocabularyId = Constants.VOCABULARY_ValidVocabularyId;
            presenter.Model.HasPermission = true;

            // Act
            presenter.LoadInternal();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindVocabulary(It.IsAny<Vocabulary>(), false,
                                                    presenter.Model.IsSuperUser));
        }

       [Test]
        public void EditVocabularyPresenter_Retrieves_All_Terms_For_Vocabulary_And_Passes_Them_To_View()
        {
            // Arrange
            EditVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.TabId = Constants.TAB_ValidId;
            presenter.Model.ModuleId = Constants.MODULE_ValidId;
            presenter.Model.VocabularyId = Constants.VOCABULARY_ValidVocabularyId;
            presenter.Model.HasPermission = true;

            // Act
            presenter.LoadInternal();

            // Assert
            Assert.AreEqual(MockHelper.TestTerms.ToList().Count, presenter.Terms.Count);
            Mock.Get(presenter.View)
                .Verify(v => v.BindTerms(It.IsAny<IEnumerable<Term>>(), 
                                            false,
                                            true));
        }

       [Test]
        public void EditVocabularyPresenter_Redirects_To_AccessDenied_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            EditVocabularyPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.BindVocabulary(It.IsAny<Vocabulary>(), false,
                                               presenter.Model.IsSuperUser))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToAccessDenied());
        }

        #endregion

        #region AddTerm Tests

       [Test]
        public void EditVocabularyPresenter_AddTerm_Loads_New_Term()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.AddTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindTerm(It.Is<Term>(t => t.TermId == Null.NullInteger),
                                        It.IsAny<IEnumerable<Term>>(),
                                        presenter.IsHeirarchical,
                                        false,
                                        false));
        }

       [Test]
        public void EditVocabularyPresenter_AddTerm_Displays_Term_Editor()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.AddTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ShowTermEditor(true));
        }

       [Test]
        public void EditVocabularyPresenter_AddTerm_Correctly_Sets_TermEditor_Mode()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.AddTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.SetTermEditorMode(true, Null.NullInteger));
        }

        #endregion

        #region Cancel Tests

       [Test]
        public void EditVocabularyPresenter_Cancel_Redirects_To_Vocabulary_List_View()
        {
            // Arrange
            EditVocabularyPresenter presenter = CreatePresenter();

            // Act
            presenter.Cancel();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<VocabularyListPresenterModel>()));
        }

        #endregion

       #region CancelTerm Tests

       [Test]
       public void EditVocabularyPresenter_CancelTerm_Hides_Term_Editor()
       {
           // Arrange
           SetUpPresenter();

           // Act
           presenter.CancelTerm();

           // Assert
           Mock.Get(presenter.View)
               .Verify(v => v.ShowTermEditor(false));
       }

       [Test]
       public void EditVocabularyPresenter_CancelTerm_Clears_SelectedTerm()
       {
           // Arrange
           SetUpPresenter();

           // Act
           presenter.CancelTerm();

           // Assert
           Mock.Get(presenter.View)
               .Verify(v => v.ClearSelectedTerm());
       }

       #endregion

        #region DeleteTerm Tests

       [Test]
        public void EditVocabularyPresenter_DeleteTerm_Deletes_Term()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteTerm();

            // Assert
            Mock.Get(presenter.TermController)
                .Verify(r => r.DeleteTerm(It.Is<Term>(t => t.TermId == term.TermId)));
        }

       [Test]
        public void EditVocabularyPresenter_DeleteTerm_Clears_SelectedTerm()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ClearSelectedTerm());
        }

       [Test]
        public void EditVocabularyPresenter_DeleteTerm_Refreshes_Terms()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindTerms(It.IsAny<IEnumerable<Term>>(),
                                            vocabulary.Type == VocabularyType.Hierarchy,
                                            true));
        }

       [Test]
        public void EditVocabularyPresenter_DeleteTerm_Hides_Term_Editor()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ShowTermEditor(false));
        }

        #endregion

        #region DeleteVocabulary Tests

       [Test]
        public void EditVocabularyPresenter_DeleteVocabulary_Redirects_To_Vocabulary_List_View()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteVocabulary();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<VocabularyListPresenterModel>()));
        }

       [Test]
        public void EditVocabularyPresenter_DeleteVocabulary_Deletes_Vocabulary()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.DeleteVocabulary();

            // Assert
            Mock.Get(presenter.VocabularyController)
                .Verify(r => r.DeleteVocabulary(It.Is<Vocabulary>(v => v.VocabularyId == vocabulary.VocabularyId)));
        }

        #endregion

        #region SaveTerm Tests

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Validates_Term()
        {
            // Arrange
            SetUpPresenter();

            var mockValidator = MockHelper.EnableValidMockValidator(presenter.Validator, term);

            // Act
            presenter.SaveTerm();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(term));
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Does_Not_Save_If_Term_Invalid()
        {
            // Arrange
            SetUpPresenter();

            Mock.Get(presenter.TermController)
                .Setup(r => r.UpdateTerm(term))
                .Never();

            var mockValidator = MockHelper.EnableInvalidMockValidator(presenter.Validator, term);

            // Act
            presenter.SaveTerm();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(term));
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Adds_New_Term_If_Term_Valid_And_TermId_Negative_1()
        {
            // Arrange
            SetUpPresenter();

            presenter.Vocabulary = vocabulary;
            presenter.Term = term;            

            // Act
            presenter.SaveTerm();

            // Assert
            Mock.Get(presenter.TermController)
                .Verify(r => r.AddTerm(It.Is<Term>(t => t.TermId == term.TermId)));
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Updates_Term_If_Term_Valid()
        {
            // Arrange
            SetUpPresenter(Constants.TERM_ValidTermId);

            // Act
            presenter.SaveTerm();

            // Assert
            Mock.Get(presenter.TermController)
                .Verify(r => r.UpdateTerm(It.Is<Term>(t => t.TermId == term.TermId)));
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Sets_CurrentTerm_To_Null_With_No_Errors()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveTerm();

            // Assert
            Assert.IsNull(presenter.Term);
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Clears_SelectedTerm_With_No_Errors()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ClearSelectedTerm());
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Refreshes_Terms_With_No_Errors()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindTerms(It.IsAny<IEnumerable<Term>>(),
                                            vocabulary.Type == VocabularyType.Hierarchy,
                                            true));
        }

       [Test]
        public void EditVocabularyPresenter_SaveTerm_Hides_Term_Editor()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveTerm();

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ShowTermEditor(false));
        }

        #endregion

        #region SaveVocabulary Tests

       [Test]
        public void EditVocabularyPresenter_SaveVocabulary_Validates_Vocabulary()
        {
            // Arrange
            SetUpPresenter();

            var mockValidator = MockHelper.EnableValidMockValidator(presenter.Validator, vocabulary);

            // Act
            presenter.SaveVocabulary();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(vocabulary));
        }

       [Test]
        public void EditVocabularyPresenter_SaveVocabulary_Does_Not_Save_If_Vocabulary_Invalid()
        {
            // Arrange
            SetUpPresenter();

            Mock.Get(presenter.VocabularyController)
                .Setup(r => r.UpdateVocabulary(vocabulary))
                .Never();

            var mockValidator = MockHelper.EnableInvalidMockValidator(presenter.Validator, vocabulary);

            // Act
            presenter.SaveVocabulary();

            // Assert
            mockValidator.Verify(v => v.ValidateObject(vocabulary));
        }

       [Test]
        public void EditVocabularyPresenter_SaveVocabulary_Saves_If_Vocabulary_Valid()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveVocabulary();

            // Assert
            Mock.Get(presenter.VocabularyController)
                .Verify(r => r.UpdateVocabulary(It.Is<Vocabulary>(v => v.VocabularyId == vocabulary.VocabularyId)));
        }

       [Test]
        public void EditVocabularyPresenter_SaveVocabulary_Redirects_To_Vocabulary_List_View_With_No_Errors()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SaveVocabulary();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<VocabularyListPresenterModel>()));
        }

        #endregion

        #region SelectedTerm Tests

       [Test]
        public void EditVocabularyPresenter_Loads_SelectedTerm_When_Term_Is_Selected()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SelectTerm(term);

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.BindTerm(term,
                                        It.IsAny<IEnumerable<Term>>(),
                                        presenter.IsHeirarchical,
                                        false,
                                        false));
        }

       [Test]
        public void EditVocabularyPresenter_Displays_Term_Editor_When_Term_Is_Selected()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SelectTerm(term);

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.ShowTermEditor(true));
        }

       [Test]
        public void EditVocabularyPresenter_Correctly_Sets_TermEditor_Mode_When_Term_Is_Selected()
        {
            // Arrange
            SetUpPresenter();

            // Act
            presenter.SelectTerm(term);

            // Assert
            Mock.Get(presenter.View)
                .Verify(v => v.SetTermEditorMode(false, term.TermId));
        }

        #endregion

        #region Helpers

        protected override EditVocabularyPresenter ConstructPresenter()
        {
            return new EditVocabularyPresenter(MockHelper.CreateMockVocabularyController().Object,
                                                MockHelper.CreateMockTermController().Object);
        }

        private void SetUpPresenter()
        {
            SetUpPresenter(Null.NullInteger);
        }

        private void SetUpPresenter(int termId)
        {
            presenter = CreatePresenter();
            vocabulary = ContentTestHelper.CreateValidVocabulary();
            term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = termId;

            presenter.Vocabulary = vocabulary;
            presenter.Term = term;
        }

        #endregion
    }
}
