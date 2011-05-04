#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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
using System.Collections.Specialized;
using System.Linq;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Content.Mocks;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using MbUnit.Framework;

using Moq;

namespace DotNetNuke.Tests.Content
{
    /// <summary>
    ///   Summary description for ContentItemTests
    /// </summary>
    [TestFixture]
    public class ContentControllerTests
    {
        private Mock<CachingProvider> mockCache;

        #region Test Initialize

        [SetUp]
        public void SetUp()
        {
            //Register MockCachingProvider
            mockCache = MockComponentProvider.CreateNew<CachingProvider>();
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }

        #endregion

        #region AddContentItem Tests

        [Test]
        public void ContentController_AddContentItem_Throws_On_Null_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => controller.AddContentItem(null));
        }

        [Test]
        public void ContentController_AddContentItem_Calls_DataService_On_Valid_Arguments()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;

            //Act
            int contentId = controller.AddContentItem(content);

            //Assert
            mockDataService.Verify(ds => ds.AddContentItem(content, It.IsAny<int>()));
        }

        [Test]
        public void ContentController_AddContentItem_Returns_ValidId_On_Valid_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.AddContentItem(It.IsAny<ContentItem>(), It.IsAny<int>())).Returns(Constants.CONTENT_AddContentItemId);
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;

            //Act
            int contentId = controller.AddContentItem(content);

            //Assert
            Assert.AreEqual(Constants.CONTENT_AddContentItemId, contentId);
        }

        [Test]
        public void ContentController_AddContentItem_Sets_ContentItemId_Property_On_Valid_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.AddContentItem(It.IsAny<ContentItem>(), It.IsAny<int>())).Returns(Constants.CONTENT_AddContentItemId);
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;

            //Act
            int contentId = controller.AddContentItem(content);

            //Assert
            Assert.AreEqual(Constants.CONTENT_AddContentItemId, content.ContentItemId);
        }

        #endregion

        #region DeleteContentItem Tests

        [Test]
        [ExpectedArgumentNullException]
        public void ContentController_DeleteContentItem_Throws_On_Null_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);
            //Act, Arrange
            controller.DeleteContentItem(null);
        }

        [Test]
        public void ContentController_DeleteContentItem_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Null.NullInteger;

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.DeleteContentItem(content));
        }

        [Test]
        public void ContentController_DeleteContentItem_Calls_DataService_On_Valid_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_DeleteContentItemId;

            //Act
            controller.DeleteContentItem(content);

            //Assert
            mockDataService.Verify(ds => ds.DeleteContentItem(content));
        }

        #endregion

        #region GetContentItem Tests

        [Test]
        public void ContentController_GetContentItem_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.GetContentItem(Null.NullInteger));
        }

        [Test]
        public void ContentController_GetContentItem_Returns_Null_On_InValid_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItem(Constants.CONTENT_InValidContentItemId)).Returns(MockHelper.CreateEmptyContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            ContentItem content = controller.GetContentItem(Constants.CONTENT_InValidContentItemId);

            //Assert
            Assert.IsNull(content);
        }

        [Test]
        public void ContentController_GetContentItem_Calls_DataService_On_Valid_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItem(Constants.CONTENT_ValidContentItemId)).Returns(MockHelper.CreateValidContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            ContentItem content = controller.GetContentItem(Constants.CONTENT_ValidContentItemId);

            //Assert
            mockDataService.Verify(ds => ds.GetContentItem(Constants.CONTENT_ValidContentItemId));
        }

        [Test]
        public void ContentController_GetContentItem_Returns_ContentItem_On_Valid_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItem(Constants.CONTENT_ValidContentItemId)).Returns(MockHelper.CreateValidContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            ContentItem content = controller.GetContentItem(Constants.CONTENT_ValidContentItemId);

            //Assert
            Assert.AreEqual(Constants.CONTENT_ValidContentItemId, content.ContentItemId);
            Assert.AreEqual(ContentTestHelper.GetContent(Constants.CONTENT_ValidContentItemId), content.Content);
            Assert.AreEqual(ContentTestHelper.GetContentKey(Constants.CONTENT_ValidContentItemId), content.ContentKey);
        }

        #endregion

        #region GetContentItemsByTerm Tests

        [Test]
        public void ContentController_GetContentItemsByTerm_Throws_On_Null_Term()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.GetContentItemsByTerm(Null.NullString));
        }

        [Test]
        public void ContentController_GetContentItemsByTerm_Calls_DataService()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItemsByTerm(Constants.TERM_ValidName)).Returns(MockHelper.CreateValidContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetContentItemsByTerm(Constants.TERM_ValidName);

            //Assert
            mockDataService.Verify(ds => ds.GetContentItemsByTerm(Constants.TERM_ValidName));
        }

        [Test]
        public void ContentController_GetContentItemsByTerm_Returns_Empty_List_If_Term_Not_Used()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItemsByTerm(Constants.TERM_UnusedName)).Returns(MockHelper.CreateEmptyContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetContentItemsByTerm(Constants.TERM_UnusedName);

            //Assert
            Assert.AreEqual(0, contentItems.Count());
        }

        [Test]
        public void ContentController_GetContentItemsByTerm_Returns_List_Of_ContentItems()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetContentItemsByTerm(Constants.TERM_ValidName)).Returns(MockHelper.CreateValidContentItemsReader(Constants.CONTENT_TaggedItemCount,
                                                                                                                                             Constants.CONTENT_IndexedFalse,
                                                                                                                                             Null.NullInteger,
                                                                                                                                             Constants.TERM_ValidName));
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetContentItemsByTerm(Constants.TERM_ValidName);

            //Assert
            Assert.AreEqual(Constants.CONTENT_TaggedItemCount, contentItems.Count());
        }

        #endregion

        #region GetUnIndexedContentItems Tests

        [Test]
        public void ContentController_GetUnIndexedContentItems_Calls_DataService()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetUnIndexedContentItems()).Returns(MockHelper.CreateValidContentItemReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetUnIndexedContentItems();

            //Assert
            mockDataService.Verify(ds => ds.GetUnIndexedContentItems());
        }

        [Test]
        public void ContentController_GetUnIndexedContentItems_Returns_EmptyList_If_No_UnIndexed_Items()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetUnIndexedContentItems()).Returns(MockHelper.CreateEmptyContentItemReader());

            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetUnIndexedContentItems();

            //Assert
            Assert.AreEqual(0, contentItems.Count());
        }

        [Test]
        public void ContentController_GetUnIndexedContentItems_Returns_List_Of_UnIndexedContentItems()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetUnIndexedContentItems()).Returns(MockHelper.CreateValidContentItemsReader(Constants.CONTENT_IndexedFalseItemCount,
                                                                                                                        Constants.CONTENT_IndexedFalse,
                                                                                                                        Null.NullInteger,
                                                                                                                        Null.NullString));

            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            IQueryable<ContentItem> contentItems = controller.GetUnIndexedContentItems();

            //Assert
            Assert.AreEqual(Constants.CONTENT_IndexedFalseItemCount, contentItems.Count());
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
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => controller.UpdateContentItem(null));
        }

        [Test]
        public void ContentController_UpdateContentItem_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = new ContentItem();
            content.ContentItemId = Null.NullInteger;

            Assert.Throws<ArgumentException>(() => controller.UpdateContentItem(content));
        }

        [Test]
        public void ContentController_UpdateContentItem_Calls_DataService_On_Valid_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_UpdateContent;
            content.ContentKey = Constants.CONTENT_UpdateContentKey;

            //Act
            controller.UpdateContentItem(content);

            //Assert
            mockDataService.Verify(ds => ds.UpdateContentItem(content, It.IsAny<int>()));
        }

        #endregion

        #region AddMetaData Tests

        [Test]
        public void ContentController_AddMetaData_Throws_On_Null_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentNullException>(() => controller.AddMetaData(null, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_AddMetaData_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Null.NullInteger;

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.AddMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_AddMetaData_Throws_On_Null_MetaDataName()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.AddMetaData(content, Null.NullString, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_AddMetaData_Calls_DataService_On_Valid_Arguments()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;

            //Act
            controller.AddMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue);

            //Assert
            mockDataService.Verify(ds => ds.AddMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        #endregion

        #region DeleteMetaData Tests

        [Test]
        public void ContentController_DeleteMetaData_Throws_On_Null_ContentItem()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.AddMetaData(null, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_DeleteMetaData_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Null.NullInteger;

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.DeleteMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_DeleteMetaData_Throws_On_Null_MetaDataName()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.AddMetaData(content, Null.NullString, Constants.CONTENT_ValidMetaDataValue));
        }

        [Test]
        public void ContentController_DeleteMetaData_Calls_DataService_On_Valid_Arguments()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;

            //Act
            controller.DeleteMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue);

            //Assert
            mockDataService.Verify(ds => ds.DeleteMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue));
        }

        #endregion

        #region GetMetaData Tests

        [Test]
        public void ContentController_GetMetaData_Throws_On_Negative_ContentItemId()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            ContentController controller = new ContentController(mockDataService.Object);

            //Act, Arrange
            Assert.Throws<ArgumentException>(() => controller.GetMetaData(Null.NullInteger));
        }

        [Test]
        public void ContentController_GetMetaData_Calls_DataService_On_Valid_Arguments()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetMetaData(Constants.CONTENT_ValidContentItemId)).Returns(MockHelper.CreateValidMetaDataReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            NameValueCollection metaData = controller.GetMetaData(Constants.CONTENT_ValidContentItemId);

            //Assert
            mockDataService.Verify(ds => ds.GetMetaData(Constants.CONTENT_ValidContentItemId));
        }

        [Test]
        public void ContentController_GetMetaData_Returns_NameValueCollection_Of_MetaData()
        {
            //Arrange
            Mock<IDataService> mockDataService = new Mock<IDataService>();
            mockDataService.Setup(ds => ds.GetMetaData(Constants.CONTENT_ValidContentItemId)).Returns(MockHelper.CreateValidMetaDataReader());
            ContentController controller = new ContentController(mockDataService.Object);

            //Act
            NameValueCollection metaData = controller.GetMetaData(Constants.CONTENT_ValidContentItemId);

            //Assert
            Assert.AreEqual(Constants.CONTENT_MetaDataCount, metaData.Count);
        }

        #endregion
    }
}