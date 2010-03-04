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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Cache;
using DotNetNuke.Entities.Content;
using DotNetNuke.Tests.Content.Fakes.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using Moq;
using MbUnit.Framework;
using System.Linq;

namespace DotNetNuke.Tests.Content
{
    /// <summary>
    /// Summary description for ContentItemTests
    /// </summary>
    [TestFixture]
    public class ContentControllerTests
    {

        #region Private Members

        ContentController contentController;
        FakeDataService dataService = new FakeDataService();
        private Mock<CachingProvider> mockCache;

        #endregion

        #region Test Initialize

        [SetUp()]
        public void SetUp()
        {
            contentController = new ContentController(dataService);

            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region AddContentItem Tests

       [Test]
        public void ContentController_AddContentItem_Throws_On_Null_ContentItem()
        {
            AutoTester.ArgumentNull<ContentItem>(content => contentController.AddContentItem(content));
        }

       [Test]
        public void ContentController_AddContentItem_Returns_ValidId_On_Valid_ContentItem()
        {
            //Arrange
            ContentItem content = ContentTestHelper.CreateValidContentItem();

            //Act
            int contentId = contentController.AddContentItem(content);

            //Assert
            Assert.AreEqual<int>(Constants.CONTENT_AddContentItemId, contentId);
        }

       [Test]
        public void ContentController_AddContentItem_Sets_ContentItemId_Property_On_Valid_ContentItem()
        {
            //Arrange
            ContentItem content = ContentTestHelper.CreateValidContentItem();

            //Act
            int contentId = contentController.AddContentItem(content);

            //Assert
            Assert.AreEqual<int>(Constants.CONTENT_AddContentItemId, content.ContentItemId);
        }

        #endregion

        #region DeleteContentItem Tests

       [Test]
        public void ContentController_DeleteContentItem_Throws_On_Null_ContentItem()
        {
            AutoTester.ArgumentNull<ContentItem>(content => contentController.DeleteContentItem(content));
        }

       [Test]
        public void ContentController_DeleteContentItem_Throws_On_Negative_ContentItemId()
        {
            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => contentController.DeleteContentItem(content));
        }

       [Test]
        public void ContentController_DeleteContentItem_Succeeds_On_Valid_ContentItemId()
        {
            //Arrange
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                        Constants.CONTENT_IndexedTrue,
                                        Null.NullInteger, i => "");

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_DeleteContentItemId;

            //Act
            contentController.DeleteContentItem(content);

            //Assert
            Assert.IsNull(dataService.GetContentItemFromTable(Constants.CONTENT_DeleteContentItemId));
        }

        #endregion

        #region GetContentItem Tests

       [Test]
        public void ContentController_GetContentItem_Throws_On_Negative_ContentItemId()
        {
            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => contentController.GetContentItem(Null.NullInteger));
        }

       [Test]
        public void ContentController_GetContentItem_Returns_Null_On_InValid_ContentItemId()
        {
            //Arrange
            int contentItemId = Constants.CONTENT_InValidContentItemId;
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                        Constants.CONTENT_IndexedTrue,
                                        Null.NullInteger, i => "");

            //Act
            ContentItem content = contentController.GetContentItem(contentItemId);

            //Assert
            Assert.IsNull(content);
        }

       [Test]
        public void ContentController_GetContentItem_Returns_ContentItem_On_Valid_ContentItemId()
        {
            //Arrange
            int contentItemId = Constants.CONTENT_ValidContentItemId;
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                        Constants.CONTENT_IndexedTrue,
                                        Constants.USER_ValidId, i => "");

            //Act
            ContentItem content = contentController.GetContentItem(contentItemId);

            //Assert
            Assert.AreEqual<int>(Constants.CONTENT_ValidContentItemId, content.ContentItemId);
            Assert.AreEqual<string>(ContentTestHelper.GetContent(Constants.CONTENT_ValidContentItemId), content.Content);
            Assert.AreEqual<string>(ContentTestHelper.GetContentKey(Constants.CONTENT_ValidContentItemId), content.ContentKey);
        }

        #endregion

        #region GetContentItemsByTerm Tests

        [Test]
        public void ContentController_GetContentItemsByTerm_Throws_On_Null_Term()
       {
           ExceptionAssert.Throws<ArgumentException>(() => contentController.GetContentItemsByTerm(Null.NullString));
       }

        [Test]
        public void ContentController_GetContentItemsByTerm_Returns_Empty_List_If_Term_Not_Used()
        {
            //Arrange
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                       Constants.CONTENT_IndexedFalse,
                                       Null.NullInteger, i => (i < Constants.CONTENT_TaggedItemCount) ? Constants.TERM_ValidName : "");

            //Act
            IQueryable<ContentItem> contentItems = contentController.GetContentItemsByTerm(Constants.TERM_UnusedName);

            //Assert
            Assert.AreEqual<int>(0, contentItems.Count());
        }

        [Test]
        public void ContentController_GetContentItemsByTerm_Returns_List_Of_ContentItems()
        {
           //Arrange
           dataService.SetUpContentItemTable();
           dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                      Constants.CONTENT_IndexedFalse,
                                       Null.NullInteger, i => (i <= Constants.CONTENT_TaggedItemCount) ? Constants.TERM_ValidName : "");

           //Act
           IQueryable<ContentItem> contentItems = contentController.GetContentItemsByTerm(Constants.TERM_ValidName);

           //Assert
           Assert.AreEqual<int>(Constants.CONTENT_TaggedItemCount, contentItems.Count());
        }

        #endregion
       
        #region GetUnIndexedContentItems Tests

        [Test]
        public void ContentController_GetUnIndexedContentItems_Returns_EmptyList_If_No_UnIndexed_Items()
        {
            //Arrange
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_IndexedTrueItemCount,
                                       Constants.CONTENT_IndexedTrue,
                                       Null.NullInteger, i => "");
            
            //Act
            IQueryable<ContentItem> contentItems = contentController.GetUnIndexedContentItems();

            //Assert
            Assert.AreEqual<int>(0, contentItems.Count());
        }

        [Test]
        public void ContentController_GetUnIndexedContentItems_Returns_List_Of_UnIndexedContentItems()
        {
            //Arrange
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_IndexedFalseItemCount,
                                       Constants.CONTENT_IndexedFalse,
                                       Null.NullInteger, i => "");

            //Act
            IQueryable<ContentItem> contentItems = contentController.GetUnIndexedContentItems();

            //Assert
            Assert.AreEqual<int>(Constants.CONTENT_IndexedFalseItemCount, contentItems.Count());

            foreach (ContentItem content in contentItems)
            {
                Assert.IsFalse(content.Indexed);
            }
        }

        #endregion

        #region UpdateContentItem Tests

       [Test]
        public void ContentController_UpdateContentItem_Throws_On_Null_ContentItem()
        {
            AutoTester.ArgumentNull<ContentItem>(content => contentController.UpdateContentItem(content));
        }

       [Test]
        public void ContentController_UpdateContentItem_Throws_On_Negative_ContentItemId()
        {
            ContentItem content = new ContentItem();
            content.ContentItemId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => contentController.UpdateContentItem(content));
        }

       [Test]
        public void ContentController_UpdateContentItem_Succeeds_On_Valid_ContentItem()
        {
            //Arrange
            dataService.SetUpContentItemTable();
            dataService.AddContentItemsToTable(Constants.CONTENT_ValidContentItemCount,
                                        Constants.CONTENT_IndexedTrue,
                                        Null.NullInteger, i => "");

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_UpdateContent;
            content.ContentKey = Constants.CONTENT_UpdateContentKey;

            //Act
            contentController.UpdateContentItem(content);

            //Assert
            content = dataService.GetContentItemFromTable(Constants.CONTENT_UpdateContentItemId);
            Assert.AreEqual<int>(Constants.CONTENT_UpdateContentItemId, content.ContentItemId);
            Assert.AreEqual<string>(Constants.CONTENT_UpdateContent, content.Content);
            Assert.AreEqual<string>(Constants.CONTENT_UpdateContentKey, content.ContentKey);
            Assert.AreEqual<bool>(Constants.CONTENT_IndexedFalse, content.Indexed);
        }

        #endregion
    }
}
