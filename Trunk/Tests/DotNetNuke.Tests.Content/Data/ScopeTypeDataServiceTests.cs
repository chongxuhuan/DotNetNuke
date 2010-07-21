// '
// ' DotNetNuke® - http://www.dotnetnuke.com
// ' Copyright (c) 2002-2010
// ' by DotNetNuke Corporation
// '
// ' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// ' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// ' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// ' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// '
// ' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// ' of the Software.
// '
// ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// ' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// ' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// ' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// ' DEALINGS IN THE SOFTWARE.
// '
using System.Data;
using System.Data.SqlClient;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;
using MbUnit.Framework;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    /// Summary description for ScopeTypeTests
    /// </summary>
    [TestFixture]
    public class ScopeTypeDataServiceTests
    {
        #region Private Members

        private static string addScopeType = "AddScopeType";
        private static string deleteScopeType = "DeleteScopeType";
        private static string getScopeTypes = "GetScopeTypes";
        private static string keyField = "ScopeTypeId";
        private static string upateScopeType = "UpdateScopeType";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\Scope";
        private static int columnCount = 2;

        #endregion

        #region Test Database Setup and Teardown

        [SetUp]
        public void SetUp()
        {
            //Set up Data Provider
            DataUtil.SetupDataBaseProvider(DataTestHelper.ConnectionString, DataTestHelper.DatabaseOwner,
                                           DataTestHelper.ObjectQualifier);

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

        #region AddScopeType Tests

        [Test]
        public void DataService_AddScopeType_Adds_Record_On_Valid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addScopeType);

            ScopeType scopeType = new ScopeType();
            scopeType.ScopeType = Constants.SCOPETYPE_ValidScopeType;

            DataService ds = new DataService();

            //Act
            int scopeTypeItemId = ds.AddScopeType(scopeType);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ScopeTypesTableName,
                                              rowCount + 1);
        }

        [Test]
        public void DataService_AddScopeType_Returns_Correct_Id_On_Valid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, addScopeType);

            ScopeType scopeType = new ScopeType();
            scopeType.ScopeType = Constants.SCOPETYPE_ValidScopeType;

            DataService ds = new DataService();

            //Act
            int scopeTypeItemId = ds.AddScopeType(scopeType);

            //Assert
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName, "ScopeTypeID",
                                                   scopeTypeItemId);
        }

        #endregion

        #region DeleteScopeType Tests

        [Test]
        public void DataService_DeleteScopeType_Should_Do_Nothing_On_InValid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteScopeType);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_InValidScopeTypeId;

            DataService ds = new DataService();

            //Act
            ds.DeleteScopeType(scopeType);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.ScopeTypesTableName,
                                              rowCount);
        }

        [Test]
        public void DataService_DeleteScopeType_Delete_Record_On_Valid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, deleteScopeType);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_ValidScopeTypeId;

            DataService ds = new DataService();

            //Act
            ds.DeleteScopeType(scopeType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ScopeTypesTableName, rowCount - 1);
                DatabaseAssert.RecordDoesNotExist(connection, ContentDataTestHelper.ScopeTypesTableName, keyField,
                                                  Constants.SCOPETYPE_ValidScopeTypeId.ToString());
            }
        }

        #endregion

        #region GetScopeTypes Tests

        [Test]
        public void DataService_GetScopeTypes_Returns_Reader_Of_The_ScopeTypes()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, getScopeTypes);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetScopeTypes();

            //Assert
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, Constants.SCOPETYPE_ValidScopeTypeCount);
        }

        #endregion

        #region UpdateScopeType Tests

        [Test]
        public void DataService_UpdateScopeType_Does_Nothing_On_InValid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateScopeType);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_InValidScopeTypeId;
            scopeType.ScopeType = Constants.SCOPETYPE_UpdateScopeType;

            DataService ds = new DataService();

            //Act
            ds.UpdateScopeType(scopeType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ScopeTypesTableName, rowCount);

                //Check that values have not changed
                IDataReader dataReader = DataUtil.GetRecordsByField(connection,
                                                                    ContentDataTestHelper.ScopeTypesTableName, keyField,
                                                                    Constants.SCOPETYPE_UpdateScopeTypeId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ScopeType",
                                                       Constants.SCOPETYPE_OriginalUpdateScopeType);
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateScopeType_Updates_Record_On_Valid_ScopeType()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString,
                                                   ContentDataTestHelper.ScopeTypesTableName);
            DataUtil.AddDatabaseObject(virtualScriptFilePath, upateScopeType);

            ScopeType scopeType = ContentTestHelper.CreateValidScopeType();
            scopeType.ScopeTypeId = Constants.SCOPETYPE_UpdateScopeTypeId;
            scopeType.ScopeType = Constants.SCOPETYPE_UpdateScopeType;

            DataService ds = new DataService();

            //Act
            ds.UpdateScopeType(scopeType);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.ScopeTypesTableName, rowCount);

                //Check Values are updated
                IDataReader dataReader = DataUtil.GetRecordsByField(connection,
                                                                    ContentDataTestHelper.ScopeTypesTableName, keyField,
                                                                    Constants.SCOPETYPE_UpdateScopeTypeId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ScopeType", Constants.SCOPETYPE_UpdateScopeType);
                }

                dataReader.Close();
            }
        }

        #endregion
    }
}