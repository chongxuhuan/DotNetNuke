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
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using MbUnit.Framework;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    /// Summary description for ContentItemTests
    /// </summary>
    [TestFixture]
    public class ContentDataServiceTests
    {
        #region Private Members

        private static string addContentItem = "AddContentItem";
        private static string contentItemsTableName = "ContentItems";
        private static string deleteContentItem = "DeleteContentItem";
        private static string getContentItem = "GetContentItem";
        private static string getContentItemsByTermName = "GetContentItemsByTerm";
        private static string getUnIndexedContentItemsName = "GetUnIndexedContentItems";
        private static string keyField = "ContentItemId";
        private static string upateContentItem = "UpdateContentItem";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\ContentItems";
        private static int columnCount = 11;

        #endregion

        #region Test Database Setup and Teardown

        [SetUp()]
        public void SetUp()
        {
            //Set up Data Provider
            DataUtil.SetupDataBaseProvider(DataTestHelper.ConnectionString, DataTestHelper.DatabaseOwner, DataTestHelper.ObjectQualifier);

            //Create Database Tables
            ContentDataTestHelper.CreateDatabaseTables();

            //Add Data to Tables
            ContentDataTestHelper.AddDataToTables();
        }

        [TearDown()]
        public void TearDown()
        {
            //Remove all records from Tables
            ContentDataTestHelper.EmptyDatabaseTables();
        }

        #endregion

        #region AddContentItem Tests

        [Test]
        public void DataService_AddContentItem_Adds_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addContentItem);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addContentItem);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteContentItem);

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
        public void DataService_DeleteContentItem_Delete_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteContentItem);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getContentItem);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getContentItem);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetContentItem(Constants.CONTENT_ValidContentItemId);

            //Assert
            int records = 0;
            while (dataReader.Read())
            {
                DatabaseAssert.ReaderColumnIsEqual<int>(dataReader, keyField, Constants.CONTENT_ValidContentItemId);
                records += 1;
            }

            dataReader.Close();

            //Assert that the count is correct
            Assert.AreEqual<int>(1, records);
        }

        #endregion

        #region GetContentItemsByTerm Tests

        [Test]
        [Row("Black", 0)]           //No items tagged as black
        [Row("Blue", 2)]            //Both items tagged as blue      
        [Row("LCD", 1)]             //ContentItem 2 tagged as LCD    
        [Row("MP3 PLayers", 1)]     //ContentItem 1 tagged as MP3 PLayers    
        [Row("Televisions", 1)]     //ContentItem 2 tagged as LCD (child of Televisions)
        [Row("Portable Electronics", 1)]     //ContentItem 1 tagged as MP3 (child of Portable Electronics)
        [Row("Electronics", 2)]     //Both MP3 and LCD are children of Electronics
        public void DataService_GetContentItemsByTerm_Returns_Reader_Of_ContentItems(string term, int count)
        {
            //Arrange
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getContentItemsByTermName);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getUnIndexedContentItemsName);

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
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateContentItem);

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
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "Content", String.Format(Constants.CONTENT_ValidContentFormat, Constants.CONTENT_UpdateContentItemId));
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "ContentKey", String.Format(Constants.CONTENT_ValidContentKeyFormat, Constants.CONTENT_UpdateContentItemId));
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateContentItem_Updates_Record_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateContentItem);

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
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "Content", Constants.CONTENT_UpdateContent);
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "ContentKey", Constants.CONTENT_UpdateContentKey);
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateContentItem_Updates_TabId_On_Valid_Content()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, contentItemsTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateContentItem);

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
    }
}
