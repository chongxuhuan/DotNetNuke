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

using DotNetNuke.Modules.Taxonomy.ViewModels;
using DotNetNuke.Tests.Content.Fakes.Views;
using DotNetNuke.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Tests.Content.Taxonomy.Fakes;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Tests.Content.Views
{
    /// <summary>
    /// Summary description for VocabulariesListPresenter Tests
    /// </summary>
    [TestClass]
    public class ViewVocabularyPresenterTests
    {

        #region Private Members

        FakeEditVocabularyView editView;
        FakeVocabulariesListView listView;
        FakeViewVocabularyView viewView;

        EditVocabularyPresenter editPresenter;
        ViewVocabularyPresenter viewPresenter;
        VocabularyListPresenter listPresenter;

        #endregion

        #region Constructors

        public ViewVocabularyPresenterTests()
        {
            //First Create the Fake Views and Repositories
            editView = new FakeEditVocabularyView();
            listView = new FakeVocabulariesListView();
            viewView = new FakeViewVocabularyView();
            FakeVocabularyRepository vocabularyRepository = new FakeVocabularyRepository();
            FakeScopeTypeRepository scopeTypeRepository = new FakeScopeTypeRepository();

            //Create the Presenters
            editPresenter = new EditVocabularyPresenter(editView, vocabularyRepository, scopeTypeRepository);
            listPresenter = new VocabularyListPresenter(listView, vocabularyRepository, scopeTypeRepository);
            viewPresenter = new ViewVocabularyPresenter(viewView, vocabularyRepository, scopeTypeRepository);
        }

        #endregion

        #region EditVocabularyPresenter Tests

        [TestMethod]
        [Description("This tests that the EditVocabularyPresenter passes new Vocabulary to the View when the Id is negative.")]
        public void EditVocabularyPresenter_Passes_New_Vocabulary_To_View_When_VocabularyId_Is_Negative_1()
        {
            //Arrange
            editView.VocabularyId = Null.NullInteger;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            editPresenter.OnViewLoaded();

            //Assert
            Assert.IsNotNull(editView.Vocabulary);
            Assert.AreEqual<string>(Null.NullString, editView.Vocabulary.Name);
        }

        [TestMethod]
        [Description("This tests that the EditVocabularyPresenter sets the Views IsAddMode property to true when the Id is negative.")]
        public void EditVocabularyPresenter_Sets_IsAddMode_To_True_When_VocabularyId_Is_Negative_1()
        {
            //Arrange
            editView.VocabularyId = Null.NullInteger;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            editPresenter.OnViewLoaded();

            //Assert
            Assert.IsTrue(editView.IsAddMode);
        }

        [TestMethod]
        [Description("This tests that the EditPresenter passes Vocabulary to the View when the Id is valid.")]
        public void EditVocabularyPresenter_Passes_Vocabulary_To_View_When_VocabularyId_Is_Valid()
        {
            //Arrange
            editView.VocabularyId = Constants.VOCABULARY_ValidVocabularyId;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            editPresenter.OnViewLoaded();

            //Assert
            Assert.IsNotNull(editView.Vocabulary);
            Assert.AreEqual<string>(Constants.VOCABULARY_ValidName, editView.Vocabulary.Name);
        }

        [TestMethod]
        [Description("This tests that the EditVocabularyPresenter sets the Views IsAddMode property to false when the Id is valid.")]
        public void EditVocabularyPresenter_Sets_IsAddMode_To_False_When_VocabularyId_Is_Valid()
        {
            //Arrange
            editView.VocabularyId = Constants.VOCABULARY_ValidVocabularyId;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            editPresenter.OnViewLoaded();

            //Assert
            Assert.IsFalse(editView.IsAddMode);
        }


        #endregion

        #region VocabulariesListPresenter

        [TestMethod]
        [Description("This tests that the VocabulariesListPresenter passes all the Vocabularies to the View.")]
        public void VocabulariesListPresenter_Passes_All_Vocabularies_To_View()
        {
            //Arrange

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            listPresenter.OnViewLoaded();

            //Assert that the View displays the correct number of Vocabularies
            Assert.AreEqual<int>(Constants.VOCABULARY_ValidCount, listView.Vocabularies.Count);
        }

        [TestMethod]
        [Description("This tests that the VocabulariesListPresenter sets the EditUrl property of the Vocabulary View Model correctly.")]
        public void VocabulariesListPresenter_Sets_EditUrl_Of_VocabularyViewModel()
        {
            //Arrange
            listView.TabId = Constants.TAB_ValidId;
            listView.ModuleId = Constants.MODULE_ValidId;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            listPresenter.OnViewLoaded();

            //Assert
            foreach(VocabularyViewModel model in listView.Vocabularies)
            {
                Assert.IsTrue(model.EditUrl.Contains("ctl=Edit"));
                Assert.IsTrue(model.EditUrl.Contains("tabid=" + Constants.TAB_ValidId.ToString()));
                Assert.IsTrue(model.EditUrl.Contains("mid=" + Constants.MODULE_ValidId.ToString()));
                Assert.IsTrue(model.EditUrl.Contains("vocabularyId=" + model.VocabularyId.ToString()));
            }
        }

        [TestMethod]
        [Description("This tests that the VocabulariesListPresenter sets the NavigateUrl property of the Vocabulary View Model correctly.")]
        public void VocabulariesListPresenter_Sets_NavigateUrl_Of_VocabularyViewModel()
        {
            //Arrange
            listView.TabId = Constants.TAB_ValidId;
            listView.ModuleId = Constants.MODULE_ValidId;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            listPresenter.OnViewLoaded();

            //Assert
            foreach (VocabularyViewModel model in listView.Vocabularies)
            {
                Assert.IsTrue(model.NavigateUrl.Contains("ctl=View"));
                Assert.IsTrue(model.NavigateUrl.Contains("tabid=" + Constants.TAB_ValidId.ToString()));
                Assert.IsTrue(model.NavigateUrl.Contains("mid=" + Constants.MODULE_ValidId.ToString()));
                Assert.IsTrue(model.NavigateUrl.Contains("vocabularyId=" + model.VocabularyId.ToString()));
            }
        }

        #endregion

        #region ViewVocabularyPresenter Tests

        [TestMethod]
        [Description("This tests that the ViewVocabularyPresenter passes Vocabulary to the View.")]
        public void ViewVocabularyPresenter_Passes_Vocabulary_To_View_When_VocabularyId_Is_Valid()
        {
            //Arrange
            viewView.VocabularyId = Constants.VOCABULARY_ValidVocabularyId;

            //Act -- Call the presenters OnViewLoaded method to simulate the Page Load
            viewPresenter.OnViewLoaded();

            //Assert
            Assert.IsNotNull(viewView.Vocabulary);
            Assert.AreEqual<string>(Constants.VOCABULARY_ValidName, viewView.Vocabulary.Name);
        }

        #endregion

    }
}
