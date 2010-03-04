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
using MbUnit.Framework;
using Moq;

namespace DotNetNuke.Tests.Content
{
    /// <summary>
    /// Summary description for ScopeTypeTests
    /// </summary>
    [TestFixture]
    public class ScopeTypeControllerTests
    {

        #region Private Members

        FakeDataService dataService = new FakeDataService();
        private Mock<CachingProvider> mockCache;
        ScopeTypeController scopeTypeController;

        #endregion

        #region Test Initialize

        [SetUp()]
        public void SetUp()
        {
            scopeTypeController = new ScopeTypeController(dataService);

            //Register MockCachingProvider
            mockCache = MockCachingProvider.CreateMockProvider();
        }

        #endregion

        #region AddScopeType

       [Test]
        public void ScopeTypeController_AddScopeType_Throws_On_Null_ScopeType()
        {
            AutoTester.ArgumentNull<ScopeType>(scopeType => scopeTypeController.AddScopeType(scopeType));
        }

       [Test]
        public void ScopeTypeController_AddScopeType_Returns_ValidId_On_Valid_ScopeType()
        {
            //Arrange
            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();

            //Act
            int scopeTypeId = scopeTypeController.AddScopeType(scopeType);

            //Assert
            Assert.AreEqual<int>(Constants.SCOPETYPE_AddScopeTypeId, scopeTypeId);
        }

       [Test]
        public void ScopeTypeController_AddScopeType_Sets_ValidId_On_Valid_ScopeType()
        {
            //Arrange
            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();

            //Act
            scopeTypeController.AddScopeType(scopeType);

            //Assert
            Assert.AreEqual(Constants.SCOPETYPE_AddScopeTypeId, scopeType.ScopeTypeId);
        }

        #endregion

        #region DeleteScopeType

       [Test]
        public void ScopeTypeController_DeleteScopeType_Throws_On_Null_ScopeType()
        {
            AutoTester.ArgumentNull<ScopeType>(scopeType => scopeTypeController.DeleteScopeType(scopeType));
        }

       [Test]
        public void ScopeTypeController_DeleteScopeType_Throws_On_Negative_ScopeTypeId()
        {
            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Null.NullInteger;

            ExceptionAssert.Throws<ArgumentException>(() => scopeTypeController.DeleteScopeType(scopeType));
        }

       [Test]
        public void ScopeTypeController_DeleteScopeType_Succeeds_On_Valid_ScopeTypeId()
        {
            //Arrange
            dataService.SetUpScopeTypeTable();
            dataService.AddScopeTypesToTable(Constants.SCOPETYPE_ValidScopeTypeCount);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_DeleteScopeTypeId;

            //Act
            scopeTypeController.DeleteScopeType(scopeType);

            //Assert
            Assert.IsNull(dataService.GetScopeTypeFromTable(Constants.SCOPETYPE_DeleteScopeTypeId));
        }

        #endregion

        #region GetScopeTypes

       [Test]
        public void ScopeTypeController_GetScopeTypes_Returns_Empty_List_Of_ScopeTypes_If_No_ScopeTypes()
        {
            //Arrange
            dataService.SetUpScopeTypeTable();

            //Act
            List<ScopeType> scopes = scopeTypeController.GetScopeTypes().ToList();

            //Assert
            Assert.IsNotNull(scopes);
            Assert.AreEqual<int>(0, scopes.Count);
        }

       [Test]
        public void ScopeTypeController_GetScopeTypes_Returns_Dictionary_Of_ScopeTypes()
        {
            //Arrange
            dataService.SetUpScopeTypeTable();
            dataService.AddScopeTypesToTable(Constants.SCOPETYPE_ValidScopeTypeCount);

            //Act
            List<ScopeType> scopes = scopeTypeController.GetScopeTypes().ToList();

            //Assert
            Assert.AreEqual<int>(Constants.SCOPETYPE_ValidScopeTypeCount, scopes.Count);
        }

        #endregion

        #region UpdateScopeType

       [Test]
        public void ScopeTypeController_UpdateScopeType_Throws_On_Null_ScopeType()
        {
            AutoTester.ArgumentNull<ScopeType>(scopeType => scopeTypeController.UpdateScopeType(scopeType));
        }

       [Test]
        public void ScopeTypeController_UpdateScopeType_Throws_On_Negative_ScopeTypeId()
        {
            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeType = Constants.SCOPETYPE_InValidScopeType;

            ExceptionAssert.Throws<ArgumentException>(() => scopeTypeController.UpdateScopeType(scopeType));
        }

       [Test]
        public void ScopeTypeController_UpdateScopeType_Succeeds_On_Valid_ScopeType()
        {
            //Arrange
            dataService.SetUpScopeTypeTable();
            dataService.AddScopeTypesToTable(Constants.SCOPETYPE_ValidScopeTypeCount);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_UpdateScopeTypeId;
            scopeType.ScopeType = Constants.SCOPETYPE_UpdateScopeType;

            //Act
            scopeTypeController.UpdateScopeType(scopeType);

            //Assert
            scopeType = dataService.GetScopeTypeFromTable(Constants.SCOPETYPE_UpdateScopeTypeId);
            Assert.AreEqual<int>(Constants.SCOPETYPE_UpdateScopeTypeId, scopeType.ScopeTypeId);
            Assert.AreEqual<string>(Constants.SCOPETYPE_UpdateScopeType, scopeType.ScopeType);
        }

        #endregion

    }
}
