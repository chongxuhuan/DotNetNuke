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
using DotNetNuke.Services.Cache;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Content.Fakes.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using Moq;
using MbUnit.Framework;

namespace DotNetNuke.Tests.Content.Taxonomy
{
    /// <summary>
    /// Summary description for VocabularyTests
    /// </summary>
    [TestFixture]
    public class VocabularyControllerTests
    {
        #region Private Members

        private FakeDataService dataService = new FakeDataService();
        private Mock<CachingProvider> mockCache;
        private VocabularyController vocabularyController;

        #endregion

        #region Test Initialize

        [SetUp()]
        public void SetUp()
        {
            vocabularyController = new VocabularyController(dataService);

            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region AddVocabulary

       [Test]
        public void VocabularyController_AddVocabulary_Throws_On_Null_Vocabulary()
        {
            AutoTester.ArgumentNull<Vocabulary>(vocabulary => vocabularyController.AddVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_AddVocabulary_Throws_On_Invalid_Name()
        {
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.Name = Constants.VOCABULARY_InValidName;

            ExceptionAssert.Throws<ArgumentException>(() => vocabularyController.AddVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_AddVocabulary_Throws_On_Negative_ScopeTypeID()
        {
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.ScopeTypeId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => vocabularyController.AddVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_AddVocabulary_Returns_ValidId_On_Valid_Vocabulary()
        {
            //Arrange
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();

            //Act
            int vocabularyId = vocabularyController.AddVocabulary(vocabulary);

            //Assert
            Assert.AreEqual<int>(Constants.VOCABULARY_AddVocabularyId, vocabularyId);
        }

       [Test]
        public void VocabularyController_AddVocabulary_Sets_ValidId_On_Valid_Vocabulary()
        {
            //Arrange
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();

            //Act
            vocabularyController.AddVocabulary(vocabulary);

            //Assert
            Assert.AreEqual<int>(Constants.VOCABULARY_AddVocabularyId, vocabulary.VocabularyId);
        }

       [Test]
        public void VocabularyController_AddVocabulary_Clears_Vocabulary_Cache_On_Valid_Vocabulary()
        {
            //Arrange
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();

            //Act
            vocabularyController.AddVocabulary(vocabulary);

            //Assert
            mockCache.Verify(cache => cache.Remove(Constants.VOCABULARY_CacheKey));
        }

        #endregion

        #region DeleteVocabulary

       [Test]
        public void VocabularyController_DeleteVocabulary_Throws_On_Null_Vocabulary()
        {
            AutoTester.ArgumentNull<Vocabulary>(marker => vocabularyController.DeleteVocabulary(marker));
        }

       [Test]
        public void VocabularyController_DeleteVocabulary_Throws_On_Negative_VocabularyId()
        {
            Vocabulary vocabulary = new Vocabulary();
            vocabulary.VocabularyId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => vocabularyController.DeleteVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_DeleteVocabulary_Succeeds_On_Valid_VocabularyId()
        {
            //Arrange
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, Constants.VOCABULARY_ValidScopeTypeId, i => i, j => Constants.VOCABULARY_SimpleTypeId);

            Vocabulary vocabulary = new Vocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_DeleteVocabularyId;

            //Act
            vocabularyController.DeleteVocabulary(vocabulary);

            //Assert
            Assert.IsNull(dataService.GetVocabularyFromTable(Constants.VOCABULARY_DeleteVocabularyId));
        }

       [Test]
        public void VocabularyController_DeleteVocabulary_Clears_Vocabulary_Cache_On_Valid_Vocabulary()
        {
            //Arrange
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, Constants.VOCABULARY_ValidScopeTypeId, i => i, j => Constants.VOCABULARY_SimpleTypeId);

            Vocabulary vocabulary = new Vocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_DeleteVocabularyId;

            //Act
            vocabularyController.DeleteVocabulary(vocabulary);

            //Assert
            mockCache.Verify(cache => cache.Remove(Constants.VOCABULARY_CacheKey));
        }

        #endregion

        #region GetVocabularies

       [Test]
        public void VocabularyController_GetVocabularies_Returns_All_Vocabularies()
        {
            //Arrange
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, 
                                                Constants.VOCABULARY_ValidScopeTypeId, i => i, 
                                                j => Constants.VOCABULARY_SimpleTypeId);

            //Act
            List<Vocabulary> vocabularys = vocabularyController.GetVocabularies().ToList<Vocabulary>();

            //Assert
            Assert.AreEqual(Constants.VOCABULARY_ValidCount, vocabularys.Count);
        }

        #endregion

        #region UpdateVocabulary

       [Test]
        public void VocabularyController_UpdateVocabulary_Throws_On_Null_Vocabulary()
        {
            AutoTester.ArgumentNull<Vocabulary>(marker => vocabularyController.UpdateVocabulary(marker));
        }

       [Test]
        public void VocabularyController_UpdateVocabulary_Throws_On_Negative_VocabularyId()
        {
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => vocabularyController.UpdateVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_UpdateVocabulary_Throws_On_Invalid_Name()
        {
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.Name = Constants.VOCABULARY_InValidName;

            ExceptionAssert.Throws<ArgumentException>(() => vocabularyController.UpdateVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_UpdateVocabulary_Throws_On_Negative_ScopeTypeID()
        {
            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.ScopeTypeId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => vocabularyController.UpdateVocabulary(vocabulary));
        }

       [Test]
        public void VocabularyController_UpdateVocabulary_Succeeds_On_Valid_Vocabulary()
        {
            //Arrange
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, 
                                                Constants.VOCABULARY_ValidScopeTypeId,
                                                i => i, 
                                                j => Constants.VOCABULARY_SimpleTypeId);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_UpdateVocabularyId;
            vocabulary.Name = Constants.VOCABULARY_UpdateName;
            vocabulary.Weight = Constants.VOCABULARY_UpdateWeight;

            //Act
            vocabularyController.UpdateVocabulary(vocabulary);

            //Assert
            vocabulary = dataService.GetVocabularyFromTable(Constants.VOCABULARY_UpdateVocabularyId);
            Assert.AreEqual<int>(Constants.VOCABULARY_UpdateVocabularyId, vocabulary.VocabularyId);
            Assert.AreEqual<string>(Constants.VOCABULARY_UpdateName, vocabulary.Name);
            Assert.AreEqual<int>(Constants.VOCABULARY_UpdateWeight, vocabulary.Weight);
        }

       [Test]
        public void VocabularyController__UpdateVocabulary_Clears_Vocabulary_Cache_On_Valid_Vocabulary()
        {
            //Arrange
            dataService.SetUpVocabularyTable();
            dataService.AddVocabulariesToTable(Constants.VOCABULARY_ValidCount, 
                                                Constants.VOCABULARY_ValidScopeTypeId,
                                                i => i, 
                                                j => Constants.VOCABULARY_SimpleTypeId);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_UpdateVocabularyId;
            vocabulary.Name = Constants.VOCABULARY_UpdateName;
            vocabulary.Weight = Constants.VOCABULARY_UpdateWeight;

            //Act
            vocabularyController.UpdateVocabulary(vocabulary);

            //Assert
            mockCache.Verify(cache => cache.Remove(Constants.VOCABULARY_CacheKey));
        }

        #endregion
    }
}
