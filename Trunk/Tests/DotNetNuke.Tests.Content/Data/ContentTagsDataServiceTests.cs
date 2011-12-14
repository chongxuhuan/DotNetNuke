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

using System.Data.SqlClient;

using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using NUnit.Framework;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    ///   Summary description for ContentTags Tests
    /// </summary>
    [TestFixture]
    public class ContentTagsDataServiceTests
    {
        private static string addTermToContent = "AddTermToContent";
        private static string removeTermsFromContent = "RemoveTermsFromContent";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\Terms";

        #region Test Database Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            //Set up Data Provider
            DataUtil.SetupDataBaseProvider(DataTestHelper.ConnectionString, DataTestHelper.DatabaseOwner, DataTestHelper.ObjectQualifier);

            //Create Database Tables
            ContentDataTestHelper.CreateDatabaseTables();

            //Add Data to Tables
            ContentDataTestHelper.AddDataToTables();
        }

        [TearDown]
        public void TearDown()
        {
            //Remove all records from Tables
            ContentDataTestHelper.EmptyDatabaseTables();
            MockComponentProvider.ResetContainer();
        }

        #endregion

        #region AddTermToContent Tests

        [Test]
        public void DataService_AddTermToContent_Adds_Record_If_Valid()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addTermToContent);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            DataService ds = new DataService();

            //Act
            ds.AddTermToContent(term, content);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName, rowCount + 1);
        }

        [Test]
        public void DataService_AddTermToContent_Throws_If_Duplicate()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addTermToContent);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.TAG_DuplicateContentItemId;
            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TAG_DuplicateTermId;

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.AddTermToContent(term, content));
        }

        #endregion

        #region RemoveTermsFromContent Tests

        [Test]
        public void DataService_RemoveTermsFromContent_Removes_Does_Nothing_If_ContentItem_Has_No_Terms()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), removeTermsFromContent);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.TAG_NoContentContentId;

            DataService ds = new DataService();

            //Act
            ds.RemoveTermsFromContent(content);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName, rowCount);
        }

        [Test]
        public void DataService_RemoveTermsFromContent_Removes_Terms_If_Valid()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), removeTermsFromContent);

            ContentItem content = ContentTestHelper.CreateValidContentItem();
            content.ContentItemId = Constants.TAG_ValidContentId;

            DataService ds = new DataService();

            //Act
            ds.RemoveTermsFromContent(content);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTagsTableName, rowCount - Constants.TAG_ValidContentCount);
        }

        #endregion
    }
}