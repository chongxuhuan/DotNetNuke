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

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Cache;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Content.Fakes.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using Moq;
using MbUnit.Framework;
using DotNetNuke.ComponentModel;

namespace DotNetNuke.Tests.Content.Taxonomy
{
    /// <summary>
    /// Summary description for TermTests
    /// </summary>
    [TestFixture]
    public class TermControllerTests
    {
        #region Private Members

        private FakeDataService dataService;
        private Mock<CachingProvider> mockCache;
        private TermController termController;

        #endregion

        #region Test Initialize

        [SetUp()]
        public void SetUp()
        {
            dataService = new FakeDataService();
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, 
                                                Constants.VOCABULARY_ValidScopeTypeId,
                                                i => i,
                                                j => j == Constants.VOCABULARY_HierarchyVocabularyId ? Constants.VOCABULARY_HierarchyTypeId : Constants.VOCABULARY_SimpleTypeId);

            VocabularyController vocabularyController = new VocabularyController(dataService);
            TestUtil.RegisterComponent<IVocabularyController>(vocabularyController);

            termController = new TermController(dataService);

            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region AddTerm

       [Test]
        public void TermController_AddTerm_Throws_On_Null_Term()
        {
            AutoTester.ArgumentNull<Term>(term => termController.AddTerm(term));
        }

       [Test]
        public void TermController_AddTerm_Throws_On_Invalid_Term()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.Name = Constants.TERM_InValidName;

            ExceptionAssert.Throws<ArgumentException>(() => termController.AddTerm(term));
        }

       [Test]
        public void TermController_AddTerm_Throws_On_Negative_VocabularyId()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Null.NullInteger);

            ExceptionAssert.Throws<ArgumentException>(() => termController.AddTerm(term));
        }

       [Test]
        public void TermController_AddTerm_Should_Call_DataService_AddSimpleTerm_If_Term_Is_Simple_Term()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            // Act
            int termId = termController.AddTerm(term);

            // Assert
            mockDataService.Verify(ds => ds.AddSimpleTerm(term, UserController.GetCurrentUserInfo().UserID));
        }

       [Test]
        public void TermController_AddTerm_Should_Call_DataService_AddHeirarchicalTerm_If_Term_Is_Heirarchical_Term()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_HierarchyVocabularyId, Constants.TERM_ValidParentTermId);

            // Act
            int termId = termController.AddTerm(term);

            // Assert
            mockDataService.Verify(ds => ds.AddHeirarchicalTerm(term, UserController.GetCurrentUserInfo().UserID));
        }

       [Test]
        public void TermController_AddTerm_Returns_Valid_Id_On_Valid_Term_If_Term_Is_Simple_Term()
        {
            //Arrange
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            //Act
            int termId = termController.AddTerm(term);

            //Assert
            Assert.AreEqual<int>(Constants.TERM_AddTermId, termId);
        }

       [Test]
        public void TermController_AddTerm_Sets_Valid_Id_On_Valid_Term_If_Term_Is_Simple_Term()
        {
            //Arrange
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            //Act
            termController.AddTerm(term);

            //Assert
            Assert.AreEqual<int>(Constants.TERM_AddTermId, term.TermId);
        }

       [Test]
        public void TermController_AddTerm_Returns_Valid_Id_On_Valid_Term_If_Term_Is_Heirarchical_Term()
        {
            //Arrange
            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_ValidVocabularyId, Constants.TERM_ValidParentTermId);

            //Act
            int termId = termController.AddTerm(term);

            //Assert
            Assert.AreEqual<int>(Constants.TERM_AddTermId, termId);
        }

       [Test]
        public void TermController_AddTerm_Sets_Valid_Id_On_Valid_Term_If_Term_Is_Heirarchical_Term()
        {
            //Arrange
            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_ValidVocabularyId, Constants.TERM_ValidParentTermId);

            //Act
            termController.AddTerm(term);

            //Assert
            Assert.AreEqual<int>(Constants.TERM_AddTermId, term.TermId);
        }

       [Test]
        public void TermController_AddTerm_Clears_Term_Cache_On_Valid_Term()
        {
            //Arrange
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            //Act
            termController.AddTerm(term);

            //Assert
            mockCache.Verify(cache => cache.Remove(String.Format(Constants.TERM_CacheKey, Constants.VOCABULARY_ValidVocabularyId)));
        }

        #endregion

        #region AddTermToContent

       [Test]
        public void TermController_AddTermToContent_Throws_On_Null_Term()
        {
            ContentItem content = ContentTestHelper.CreateValidContentItem();
            AutoTester.ArgumentNull<Term>(term => termController.AddTermToContent(term, content));
        }

       [Test]
        public void TermController_AddTermToContent_Throws_On_Null_ContentItem()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            AutoTester.ArgumentNull<ContentItem>(content => termController.AddTermToContent(term, content));
        }

       [Test]
        public void TermController_AddTermToContent_Should_Call_DataService_AddTermToContent_If_Valid_Params()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            // Act
            termController.AddTermToContent(term, content);

            // Assert
            mockDataService.Verify(ds => ds.AddTermToContent(term, content));
        }

        #endregion

        #region DeleteTerm

       [Test]
        public void TermController_DeleteTerm_Throws_On_Null_Term()
        {
            AutoTester.ArgumentNull<Term>(term => termController.DeleteTerm(term));
        }

       [Test]
        public void TermController_DeleteTerm_Throws_On_Negative_TermId()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => termController.DeleteTerm(term));
        }

       [Test]
       public void TermController_DeleteTerm_Should_Call_DataService_DeleteSimpleTerm_If_Term_Is_Simple_Term()
        {
            // Arrange
            var mockController = new Mock<IDataService>();
            termController = new TermController(mockController.Object);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_DeleteTermId;

            // Act
            termController.DeleteTerm(term);

            // Assert
            mockController.Verify(ds => ds.DeleteSimpleTerm(term));
        }

       [Test]
       public void TermController_DeleteTerm_Should_Call_DataService_DeleteHeirarchicalTerm_If_Term_Is_Heirarchical_Term()
        {
            // Arrange
            var mockController = new Mock<IDataService>();
            termController = new TermController(mockController.Object);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_HierarchyVocabularyId, Constants.TERM_ValidParentTermId);
            term.TermId = Constants.TERM_DeleteTermId;

            // Act
            termController.DeleteTerm(term);

            // Assert
            mockController.Verify(ds => ds.DeleteHeirarchicalTerm(term));
        }

       [Test]
        public void TermController_DeleteTerm_Succeeds_On_Valid_TermId()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => Constants.VOCABULARY_ValidVocabularyId);

            Term term = new Term(Constants.VOCABULARY_ValidVocabularyId) { TermId = Constants.TERM_DeleteTermId };

            //Act
            termController.DeleteTerm(term);

            //Assert
            Assert.IsNull(dataService.GetTermFromTable(Constants.TERM_DeleteTermId));
        }

       [Test]
        public void TermController_DeleteTerm_Clears_Term_Cache_On_Valid_Term()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            Term term = new Term(Constants.VOCABULARY_ValidVocabularyId) { TermId = Constants.TERM_DeleteTermId };

            //Act
            termController.DeleteTerm(term);

            //Assert
            mockCache.Verify(cache => cache.Remove(String.Format(Constants.TERM_CacheKey, Constants.VOCABULARY_ValidVocabularyId)));
        }

        #endregion

        #region GetTerm

       [Test]
        public void TermController_GetTerm_Throws_On_Negative_TermId()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => termController.GetTerm(Null.NullInteger));
        }

       [Test]
        public void TermController_GetTerm_Returns_Null_On_InValidTermId()
        {
            //Arrange
            int termId = Constants.TERM_InValidTermId;
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            //Act
            Term term = termController.GetTerm(termId);

            //Assert
            Assert.IsNull(term);
        }

       [Test]
        public void TermController_GetTerm_Returns_Term_On_Valid_TermId()
        {
            //Arrange
            int termId = Constants.TERM_ValidTermId;
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            //Act
            Term term = termController.GetTerm(termId);

            //Assert
            Assert.AreEqual<int>(Constants.TERM_ValidTermId, term.TermId);
            Assert.AreEqual<string>(ContentTestHelper.GetTermName(Constants.TERM_ValidTermId), term.Name);
        }

        #endregion

        #region GetTermsByContent

       [Test]
        public void TermController_GetTermsByContent_Throws_On_Invalid_ContentItemId()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => termController.GetTermsByContent(Null.NullInteger));
        }

       [Test]
        public void TermController_GetTermsByContent_Returns_Terms_On_Valid_ContentItemId()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount,
                            c => (c <= Constants.TERM_ValidCountForContent1) ? Constants.TERM_ValidContent1 : Constants.TERM_ValidContent2,
                            v => v);

            //Act
            List<Term> terms = termController.GetTermsByContent(Constants.TERM_ValidContent1).ToList();

            //Assert
            Assert.AreEqual(Constants.TERM_ValidCountForContent1, terms.Count);

            for (int i = 0; i < Constants.TERM_ValidCountForContent1; i++)
            {
                Assert.AreEqual(i + Constants.TERM_ValidTermId, terms[i].TermId);
                Assert.AreEqual<string>(ContentTestHelper.GetTermName(i + Constants.TERM_ValidTermId), terms[i].Name);
            }
        }

        #endregion

        #region GetTermsByVocabulary

       [Test]
        public void TermController_GetTermsByVocabulary_Throws_On_Invalid_VocabularyId()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => termController.GetTermsByVocabulary(Null.NullInteger));
        }

       [Test]
        public void TermController_GetTermsByVocabulary_Returns_Terms_On_Valid_VocabularyId()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount,
                            c => c,
                            v => (v <= Constants.TERM_ValidCountForVocabulary1) ? Constants.TERM_ValidVocabulary1 : Constants.TERM_ValidVocabulary2);

            //Act
            List<Term> terms = termController.GetTermsByVocabulary(Constants.TERM_ValidVocabulary1).ToList();

            //Assert
            Assert.AreEqual(Constants.TERM_ValidCountForVocabulary1, terms.Count);

            for (int i = 0; i < Constants.TERM_ValidCountForVocabulary1; i++)
            {
                Assert.AreEqual(i + Constants.TERM_ValidTermId, terms[i].TermId);
                Assert.AreEqual<string>(ContentTestHelper.GetTermName(i + Constants.TERM_ValidTermId), terms[i].Name);
            }
        }

        #endregion

        #region RemoveTermsFromContent

       [Test]
        public void TermController_RemoveTermsFromContent_Throws_On_Null_ContentItem()
        {
            AutoTester.ArgumentNull<ContentItem>(content => termController.RemoveTermsFromContent(content));
        }

       [Test]
        public void TermController_RemoveTermsFromContent_Should_Call_DataService_RemoveTermsFromContent_If_Valid_Params()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();

            // Act
            termController.RemoveTermsFromContent(content);

            // Assert
            mockDataService.Verify(ds => ds.RemoveTermsFromContent(content));
        }

        #endregion

        #region UpdateTerm

       [Test]
        public void TermController_UpdateTerm_Throws_On_Null_Term()
        {
            AutoTester.ArgumentNull<Term>(term => termController.UpdateTerm(term));
        }

       [Test]
        public void TermController_UpdateTerm_Throws_On_Negative_TermId()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Null.NullInteger);

            ExceptionAssert.Throws<ArgumentException>(() => termController.UpdateTerm(term));
        }

       [Test]
        public void TermController_UpdateTerm_Throws_On_Invalid_Term()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.Name = Constants.TERM_InValidName;

            ExceptionAssert.Throws<ArgumentException>(() => termController.UpdateTerm(term));
        }

       [Test]
        public void TermController_UpdateTerm_Throws_On_Negative_VocabularyId()
        {
            Term term = ContentTestHelper.CreateValidSimpleTerm(Null.NullInteger);

            ExceptionAssert.Throws<ArgumentException>(() => termController.UpdateTerm(term));
        }

       [Test]
       public void TermController_UpdateTerm_Should_Call_DataService_UpdateSimpleTerm_If_Term_Is_Simple_Term()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            // Act
            termController.UpdateTerm(term);

            // Assert
            mockDataService.Verify(ds => ds.UpdateSimpleTerm(term, UserController.GetCurrentUserInfo().UserID));
        }

       [Test]
       public void TermController_UpdateTerm_Should_Call_DataService_UpdateHeirarchicalTerm_If_Term_Is_Heirarchical_Term()
        {
            // Arrange
            var mockDataService = new Mock<IDataService>();
            termController = new TermController(mockDataService.Object);

            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_HierarchyVocabularyId,
                                                                    Constants.TERM_ValidParentTermId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            // Act
            termController.UpdateTerm(term);

            // Assert
            mockDataService.Verify(ds => ds.UpdateHeirarchicalTerm(term, UserController.GetCurrentUserInfo().UserID));
        }

       [Test]
        public void TermController_UpdateTerm_Succeeds_On_Valid_Term()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            //Act
            termController.UpdateTerm(term);

            //Assert
            term = dataService.GetTermFromTable(Constants.TERM_UpdateTermId);
            Assert.AreEqual<int>(Constants.TERM_UpdateTermId, term.TermId);
            Assert.AreEqual<string>(Constants.TERM_UpdateName, term.Name);
            Assert.AreEqual<int>(Constants.TERM_UpdateWeight, term.Weight);
        }

       [Test]
        public void TermController_UpdateTerm_Clears_Term_Cache_On_Valid_Term()
        {
            //Arrange
            dataService.SetUpTermTable();
            dataService.AddTermsToTable(Constants.TERM_ValidCount, i => i, j => j);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            //Act
            termController.UpdateTerm(term);

            //Assert
            mockCache.Verify(cache => cache.Remove(String.Format(Constants.TERM_CacheKey, Constants.VOCABULARY_ValidVocabularyId)));
        }

        #endregion
    }
}
