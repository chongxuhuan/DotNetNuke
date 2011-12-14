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
using System.Data;
using System.Data.SqlClient;

using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using NUnit.Framework;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    ///   Summary description for ContentItemTests
    /// </summary>
    [TestFixture]
    public class ContentDataServiceTests
    {
        private static string addContentItem = "AddContentItem";
        private static string addMetaData = "AddMetaData";
        private static string contentItemsTableName = "ContentItems";
        private static string contentMetaDataTableName = "ContentItems_MetaData";
        private static string deleteContentItem = "DeleteContentItem";
        private static string deleteMetaData = "DeleteMetaData";
        private static string getContentItem = "GetContentItem";
        private static string getContentItemsByTermName = "GetContentItemsByTerm";
        private static string getMetaData = "GetMetaData";
        private static string getUnIndexedContentItemsName = "GetUnIndexedContentItems";
        private static string keyField = "ContentItemId";
        private static string metaDataTableName = "MetaData";
        private static string upateContentItem = "UpdateContentItem";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\ContentItems";
        private static int columnCount = 11;

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

        #region AddContentItem Tests

        [Test]
        public void DataService_AddContentItem_Adds_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addContentItem);

            ContentItem content = new ContentItem();
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            int contentItemId = ds.AddContentItem(content, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentItemsTableName, rowCount + 1);
        }

        [Test]
        public void DataService_AddContentItem_Returns_Correct_Id_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addContentItem);

            ContentItem content = new ContentItem();
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            int contentItemId = ds.AddContentItem(content, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, contentItemsTableName, "ContentItemID", contentItemId);
        }

        #endregion

        #region DeleteContentItem Tests

        [Test]
        public void DataService_DeleteContentItem_Should_Do_Nothing_On_InValid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteContentItem);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_InValidContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.DeleteContentItem(content);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentItemsTableName, rowCount);
        }

        [Test]
        public void DataService_DeleteContentItem_Deletes_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteContentItem);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_ValidContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.DeleteContentItem(content);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentItemsTableName, rowCount - 1);
            DatabaseAssert.RecordDoesNotExist(DataTestHelper.ConnectionString, contentItemsTableName, keyField, Constants.CONTENT_ValidContentItemId.ToString());
        }

        #endregion

        #region GetContentItem Tests

        [Test]
        public void DataService_GetContentItem_Returns_Empty_Reader_On_InValid_Id()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getContentItem);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetContentItem(Constants.CONTENT_InValidContentItemId);

            //Assert
            DatabaseAssert.ReaderColumnCountIsEqual(dataReader, columnCount);
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, 0);

            dataReader.Close();
        }

        [Test]
        public void DataService_GetContentItem_Returns_Reader_Of_The_ContentItem_On_Valid_Id()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getContentItem);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetContentItem(Constants.CONTENT_ValidContentItemId);

            //Assert
            int records = 0;
            while (dataReader.Read())
            {
                DatabaseAssert.ReaderColumnIsEqual(dataReader, keyField, Constants.CONTENT_ValidContentItemId);
                records += 1;
            }

            dataReader.Close();

            //Assert that the count is correct
            Assert.AreEqual(1, records);
        }

        #endregion

        #region GetContentItemsByTerm Tests

        [Test]
        [TestCase("Black", 0)] //No items tagged as black
        [TestCase("Blue", 2)] //Both items tagged as blue      
        [TestCase("LCD", 1)] //ContentItem 2 tagged as LCD    
        [TestCase("MP3 PLayers", 1)] //ContentItem 1 tagged as MP3 PLayers    
        [TestCase("Televisions", 1)] //ContentItem 2 tagged as LCD (child of Televisions)
        [TestCase("Portable Electronics", 1)] //ContentItem 1 tagged as MP3 (child of Portable Electronics)
        [TestCase("Electronics", 2)] //Both MP3 and LCD are children of Electronics
        public void DataService_GetContentItemsByTerm_Returns_Reader_Of_ContentItems(string term, int count)
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getContentItemsByTermName);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetContentItemsByTerm(term);

            //Assert that the count is correct
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, count);
        }

        #endregion

        #region GetUnIndexedContent Tests

        [Test]
        public void DataService_GetUnIndexedContent_Returns_Reader_Of_UnIndexed_ContentItems()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getUnIndexedContentItemsName);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetUnIndexedContentItems();

            //Assert that the count is correct
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, Constants.CONTENT_IndexedFalseItemCount);
        }

        #endregion

        #region UpdateContentItem Tests

        [Test]
        public void DataService_UpdateContentItem_Does_Nothing_On_InValid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), upateContentItem);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_InValidContentItemId;
            content.Content = Constants.CONTENT_UpdateContent;
            content.ContentKey = Constants.CONTENT_UpdateContentKey;

            DataService ds = new DataService();

            //Act
            ds.UpdateContentItem(content, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, contentItemsTableName, rowCount);

                //Check that values have not changed
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, contentItemsTableName, keyField, Constants.CONTENT_UpdateContentItemId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Content", String.Format(Constants.CONTENT_ValidContentFormat, Constants.CONTENT_UpdateContentItemId));
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ContentKey", String.Format(Constants.CONTENT_ValidContentKeyFormat, Constants.CONTENT_UpdateContentItemId));
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateContentItem_Updates_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), upateContentItem);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_UpdateContent;
            content.ContentKey = Constants.CONTENT_UpdateContentKey;

            DataService ds = new DataService();

            //Act
            ds.UpdateContentItem(content, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, contentItemsTableName, rowCount);

                //Check Values are updated
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, contentItemsTableName, keyField, Constants.CONTENT_UpdateContentItemId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Content", Constants.CONTENT_UpdateContent);
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ContentKey", Constants.CONTENT_UpdateContentKey);
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateContentItem_Updates_TabId_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), upateContentItem);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = String.Format(Constants.CONTENT_ValidContentFormat, Constants.CONTENT_UpdateContentItemId);
            content.ContentKey = String.Format(Constants.CONTENT_ValidContentKeyFormat, Constants.CONTENT_UpdateContentItemId);

            DataService ds = new DataService();

            //Act
            ds.UpdateContentItem(content, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, contentItemsTableName, rowCount);
            }
        }

        #endregion

        #region AddMetaData Tests

        [Test]
        public void DataService_AddMetaData_Adds_Value()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentMetaDataTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addMetaData);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.AddMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentMetaDataTableName, rowCount + 1);
        }

        [Test]
        public void DataService_AddMetaData_Adds_MetaDataType_If_Not_Present()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, metaDataTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addMetaData);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.AddMetaData(content, Constants.CONTENT_NewMetaDataName, Constants.CONTENT_NewMetaDataValue);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, metaDataTableName, rowCount + 1);
        }

        #endregion

        #region DeleteMetaData Tests

        [Test]
        public void DataService_DeleteMetaData_Deletes_Record_On_Valid_MetaData()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentMetaDataTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteMetaData);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.DeleteMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentMetaDataTableName, rowCount - 1);
            DatabaseAssert.RecordDoesNotExist(DataTestHelper.ConnectionString, contentMetaDataTableName, keyField, Constants.CONTENT_ValidContentItemId.ToString());
        }

        [Test]
        public void DataService_DeleteMetaData_Should_Do_Nothing_On_InValid_MetaData()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentMetaDataTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteMetaData);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_UpdateContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.DeleteMetaData(content, Constants.CONTENT_InValidMetaDataName, Constants.CONTENT_InValidMetaDataValue);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentMetaDataTableName, rowCount);
        }

        [Test]
        public void DataService_DeleteMetaData_Should_Do_Nothing_On_InValid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentMetaDataTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteMetaData);

            ContentItem content = new ContentItem();
            content.ContentItemId = Constants.CONTENT_InValidContentItemId;
            content.Content = Constants.CONTENT_ValidContent;
            content.ContentKey = Constants.CONTENT_ValidContentKey;

            DataService ds = new DataService();

            //Act
            ds.DeleteMetaData(content, Constants.CONTENT_ValidMetaDataName, Constants.CONTENT_ValidMetaDataValue);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, contentMetaDataTableName, rowCount);
        }

        #endregion

        #region GetMetaData Tests

        [Test]
        public void DataService_GetMetaData_Returns_Reader_Of_MetaData()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getMetaData);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetMetaData(Constants.CONTENT_UpdateContentItemId);

            //Assert that the count is correct
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, Constants.CONTENT_MetaDataCount);
        }

        #endregion
    }
}