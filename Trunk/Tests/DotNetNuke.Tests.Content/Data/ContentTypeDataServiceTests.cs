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

using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using MbUnit.Framework;
using DotNetNuke.Entities.Content;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    /// Summary description for ContentTypeTests
    /// </summary>
    [TestFixture]
    public class ContentTypeDataServiceTests
    {
        #region Private Members

        private static string addContentType = "AddContentType";
        private static string deleteContentType = "DeleteContentType";
        private static string getContentTypes = "GetContentTypes";
        private static string keyField = "ContentTypeId";
        private static string upateContentType = "UpdateContentType";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\ContentType";
        private static int columnCount = 2;

        #endregion

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

        [TearDown()]
        public void TearDown()
        {
            //Remove all records from Tables
            ContentDataTestHelper.EmptyDatabaseTables();
        }

        #endregion

        #region AddContentType Tests

       [Test]
        public void DataService_AddContentType_Adds_Record_On_Valid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addContentType);

            ContentType contentType = new ContentType();
            contentType.ContentType = Constants.CONTENTTYPE_ValidContentType;

            DataService ds = new DataService();

            //Act
            int contentTypeItemId = ds.AddContentType(contentType);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName, rowCount + 1);
        }

       [Test]
        public void DataService_AddContentType_Returns_Correct_Id_On_Valid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addContentType);

            ContentType contentType = new ContentType();
            contentType.ContentType = Constants.CONTENTTYPE_ValidContentType;

            DataService ds = new DataService();

            //Act
            int contentTypeItemId = ds.AddContentType(contentType);

            //Assert
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName, "ContentTypeID", contentTypeItemId);
        }

        #endregion

        #region DeleteContentType Tests

       [Test]
        public void DataService_DeleteContentType_Should_Do_Nothing_On_InValid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteContentType);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_InValidContentTypeId;

            DataService ds = new DataService();

            //Act
            ds.DeleteContentType(contentType);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName, rowCount);
        }

       [Test]
        public void DataService_DeleteContentType_Delete_Record_On_Valid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteContentType);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_ValidContentTypeId;

            DataService ds = new DataService();

            //Act
            ds.DeleteContentType(contentType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ContentTypesTableName, rowCount - 1);
                DatabaseAssert.RecordDoesNotExist(connection, ContentDataTestHelper.ContentTypesTableName, keyField, Constants.CONTENTTYPE_ValidContentTypeId.ToString());
            }
        }

        #endregion

        #region GetContentTypes Tests

       [Test]
        public void DataService_GetContentTypes_Returns_Reader_Of_The_ContentTypes()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getContentTypes);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetContentTypes();

            //Assert
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, Constants.CONTENTTYPE_ValidContentTypeCount);
        }

        #endregion

        #region UpdateContentType Tests

       [Test]
        public void DataService_UpdateContentType_Does_Nothing_On_InValid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateContentType);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_InValidContentTypeId;
            contentType.ContentType = Constants.CONTENTTYPE_UpdateContentType;

            DataService ds = new DataService();

            //Act
            ds.UpdateContentType(contentType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ContentTypesTableName, rowCount);

                //Check that values have not changed
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.ContentTypesTableName, keyField, Constants.CONTENTTYPE_UpdateContentTypeId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "ContentType", Constants.CONTENTTYPE_OriginalUpdateContentType);
                }

                dataReader.Close();
            }
        }

       [Test]
        public void DataService_UpdateContentType_Updates_Record_On_Valid_ContentType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.ContentTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateContentType);

            ContentType contentType = ContentTestHelper.CreateValidContentType();
            contentType.ContentTypeId = Constants.CONTENTTYPE_UpdateContentTypeId;
            contentType.ContentType = Constants.CONTENTTYPE_UpdateContentType;

            DataService ds = new DataService();

            //Act
            ds.UpdateContentType(contentType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ContentTypesTableName, rowCount);

                //Check Values are updated
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.ContentTypesTableName, keyField, Constants.CONTENTTYPE_UpdateContentTypeId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual<string>(dataReader, "ContentType", Constants.CONTENTTYPE_UpdateContentType);
                }

                dataReader.Close();
            }
        }

        #endregion
    }
}
