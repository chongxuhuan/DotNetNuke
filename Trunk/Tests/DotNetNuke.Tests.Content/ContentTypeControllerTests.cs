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
using DotNetNuke.Entities.Content;
using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Content.Fakes.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using MbUnit.Framework;
using Moq;

namespace DotNetNuke.Tests.Content
{
    /// <summary>
    /// Summary description for ContentTypeTests
    /// </summary>
    [TestFixture]
    public class ContentTypeControllerTests
    {

        #region Private Members

        FakeDataService dataService = new FakeDataService();
        private Mock<CachingProvider> mockCache;
        ContentTypeController contentTypeController;

        #endregion

        #region Test Initialize

        [SetUp()]
        public void SetUp()
        {
            contentTypeController = new ContentTypeController(dataService);

            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region AddContentType

       [Test]
        public void ContentTypeController_AddContentType_Throws_On_Null_ContentType()
        {
            AutoTester.ArgumentNull<ContentType>(contentType => contentTypeController.AddContentType(contentType));
        }

       [Test]
        public void ContentTypeController_AddContentType_Returns_ValidId_On_Valid_ContentType()
        {
            //Arrange
            ContentType contentType = ContentTestHelper.CreateValidContentType();

            //Act
            int contentTypeId = contentTypeController.AddContentType(contentType);

            //Assert
            Assert.AreEqual<int>(Constants.CONTENTTYPE_AddContentTypeId, contentTypeId);
        }

       [Test]
        public void ContentTypeController_AddContentType_Sets_ValidId_On_Valid_ContentType()
        {
            //Arrange
            ContentType contentType = ContentTestHelper.CreateValidContentType();

            //Act
            contentTypeController.AddContentType(contentType);

            //Assert
            Assert.AreEqual(Constants.CONTENTTYPE_AddContentTypeId, contentType.ContentTypeId);
        }

        #endregion

        #region DeleteContentType

       [Test]
        public void ContentTypeController_DeleteContentType_Throws_On_Null_ContentType()
        {
            AutoTester.ArgumentNull<ContentType>(contentType => contentTypeController.DeleteContentType(contentType));
        }

       [Test]
        public void ContentTypeController_DeleteContentType_Throws_On_Negative_ContentTypeId()
        {
            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => contentTypeController.DeleteContentType(contentType));
        }

       [Test]
        public void ContentTypeController_DeleteContentType_Succeeds_On_Valid_ContentTypeId()
        {
            //Arrange
            dataService.SetUpContentTypeTable();
            dataService.AddContentTypesToTable(Constants.CONTENTTYPE_ValidContentTypeCount);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_DeleteContentTypeId;

            //Act
            contentTypeController.DeleteContentType(contentType);

            //Assert
            Assert.IsNull(dataService.GetContentTypeFromTable(Constants.CONTENTTYPE_DeleteContentTypeId));
        }

        #endregion

        #region GetContentTypes

       [Test]
        public void ContentTypeController_GetContentTypes_Returns_Empty_List_Of_ContentTypes_If_No_ContentTypes()
        {
            //Arrange
            dataService.SetUpContentTypeTable();

            //Act
            List<ContentType> scopes = contentTypeController.GetContentTypes().ToList();

            //Assert
            Assert.IsNotNull(scopes);
            Assert.AreEqual<int>(0, scopes.Count);
        }

       [Test]
        public void ContentTypeController_GetContentTypes_Returns_Dictionary_Of_ContentTypes()
        {
            //Arrange
            dataService.SetUpContentTypeTable();
            dataService.AddContentTypesToTable(Constants.CONTENTTYPE_ValidContentTypeCount);

            //Act
            List<ContentType> scopes = contentTypeController.GetContentTypes().ToList();

            //Assert
            Assert.AreEqual<int>(Constants.CONTENTTYPE_ValidContentTypeCount, scopes.Count);
        }

        #endregion

        #region UpdateContentType

       [Test]
        public void ContentTypeController_UpdateContentType_Throws_On_Null_ContentType()
        {
            AutoTester.ArgumentNull<ContentType>(contentType => contentTypeController.UpdateContentType(contentType));
        }

       [Test]
        public void ContentTypeController_UpdateContentType_Throws_On_Negative_ContentTypeId()
        {
            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentType = Constants.CONTENTTYPE_InValidContentType;

            ExceptionAssert.Throws<ArgumentException>(() => contentTypeController.UpdateContentType(contentType));
        }

       [Test]
        public void ContentTypeController_UpdateContentType_Succeeds_On_Valid_ContentType()
        {
            //Arrange
            dataService.SetUpContentTypeTable();
            dataService.AddContentTypesToTable(Constants.CONTENTTYPE_ValidContentTypeCount);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_UpdateContentTypeId;
            contentType.ContentType = Constants.CONTENTTYPE_UpdateContentType;

            //Act
            contentTypeController.UpdateContentType(contentType);

            //Assert
            contentType = dataService.GetContentTypeFromTable(Constants.CONTENTTYPE_UpdateContentTypeId);
            Assert.AreEqual<int>(Constants.CONTENTTYPE_UpdateContentTypeId, contentType.ContentTypeId);
            Assert.AreEqual<string>(Constants.CONTENTTYPE_UpdateContentType, contentType.ContentType);
        }

        #endregion

    }
}
