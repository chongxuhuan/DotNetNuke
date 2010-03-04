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

using System.Collections.Generic;
using System.Linq;
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
    public class VocabularyListPresenterTests : PresenterTestBase<VocabularyListPresenter, IVocabularyListView, VocabularyListPresenterModel>
    {
        #region Initialization Tests

       [Test]
        public void VocabularyListPresenter_Constructor_Requires_Non_Null_VocabularyController()
        {
            AutoTester.ArgumentNull<IVocabularyController>(m => new VocabularyListPresenter(m));
        }


       [Test]
        public void VocabularyListPresenter_Initialize_Requires_Non_Null_View()
        {
            RunInitializeWithNullViewTest();
        }

       [Test]
        public void VocabularyListPresenter_Initialize_Requires_Non_Null_Model()
        {
            RunInitializeWithNullModelTest();
        }

       [Test]
        public void VocabularyListPresenter_Initialize_Requires_Non_Null_Environment()
        {
            RunInitializeWithNullEnvironmentTest();
        }

        #endregion

        #region View Load Tests

       [Test]
        public void VocabularyListPresenter_Does_Nothing_On_View_Load_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            VocabularyListPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.ShowVocabularies(It.IsAny<IEnumerable<Vocabulary>>()))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert (done by the call to Never() above)
        }

       [Test]
        public void VocabularyListPresenter_Redirects_To_AccessDenied_If_CurrentUser_Does_Not_Have_Permissions()
        {
            // Arrange
            VocabularyListPresenter presenter = CreatePresenter();
            presenter.Model.HasPermission = false;

            Mock.Get(presenter.View)
                .Setup(v => v.ShowVocabularies(It.IsAny<IEnumerable<Vocabulary>>()))
                .Never();

            // Act
            presenter.LoadInternal();

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToAccessDenied());
        }

       [Test]
        public void VocabularyListPresenter_Retrieves_All_Vocabularies_And_Passes_Them_To_View()
        {
            // Arrange
            VocabularyListPresenter presenter = CreatePresenter();

            // Act
            presenter.Load();

            // Assert
            Assert.AreEqual(MockHelper.TestVocabularies.ToList().Count, presenter.Vocabularies.Count);
            Mock.Get(presenter.View)
                .Verify(v => v.ShowVocabularies(It.IsAny<IEnumerable<Vocabulary>>()));
        }

        #endregion

        #region AddVocabulary Tests

       [Test]
        public void VocabularyListPresenter_On_Add_Redirects_To_CreateVocabulary_Presenter_With_No_QueryString()
        {
            // Arrange
            VocabularyListPresenter presenter = CreatePresenter();
            Mock.Get(presenter.View)
                .Setup(v => v.ShowVocabularies(It.IsAny<IEnumerable<Vocabulary>>()))
                .Never();

            // Act
            presenter.AddVocabulary(new object(), new System.EventArgs());

            // Assert
            Mock.Get(presenter.Environment)
                .Verify(c => c.RedirectToPresenter(It.IsAny<CreateVocabularyPresenterModel>()));
        }

        #endregion

        #region Helpers

        protected override VocabularyListPresenter ConstructPresenter()
        {
            return new VocabularyListPresenter(MockHelper.CreateMockVocabularyController().Object);
        }

        #endregion
    }
}
