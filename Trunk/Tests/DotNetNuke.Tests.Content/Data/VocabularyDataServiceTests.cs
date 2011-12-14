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

using System.Data;
using System.Data.SqlClient;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Data;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using NUnit.Framework;

namespace DotNetNuke.Tests.Content.Data
{
    /// <summary>
    ///   Summary description for VocabularyTests
    /// </summary>
    [TestFixture]
    public class VocabularyDataServiceTests
    {
        private static string addVocabulary = "AddVocabulary";
        private static string deleteVocabulary = "DeleteVocabulary";
        private static string getVocabularies = "GetVocabularies";
        private static string keyField = "VocabularyId";
        private static string upateVocabulary = "UpdateVocabulary";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\Vocabularies";

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

        #region AddVocabulary Tests

        [Test]
        public void DataService_AddVocabulary_Adds_Record_On_Valid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();

            DataService ds = new DataService();

            //Act
            int vocabularyId = ds.AddVocabulary(vocabulary, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName, rowCount + 1);
        }

        [Test]
        public void DataService_AddVocabulary_Returns_Correct_Id_On_Valid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();

            DataService ds = new DataService();

            //Act
            int vocabularyItemId = ds.AddVocabulary(vocabulary, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName, "VocabularyID", vocabularyItemId);
        }

        [Test]
        public void DataService_AddVocabulary_Sets_ScopeId_To_DbNull_If_Negative_1()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.ScopeId = Null.NullInteger;

            DataService ds = new DataService();

            //Act
            int vocabularyItemId = ds.AddVocabulary(vocabulary, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                //Check ScopeId is DBNull
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.VocabulariesTableName, keyField, vocabularyItemId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsNull(dataReader, "ScopeId");
                }

                dataReader.Close();
            }
        }

        #endregion

        #region DeleteVocabulary Tests

        [Test]
        public void DataService_DeleteVocabulary_Should_Do_Nothing_On_InValid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_InValidVocabularyId;

            DataService ds = new DataService();

            //Act
            ds.DeleteVocabulary(vocabulary);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.VocabulariesTableName, rowCount);
            }
        }

        [Test]
        public void DataService_DeleteVocabulary_Delete_Record_On_Valid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_DeleteVocabularyId;

            DataService ds = new DataService();

            //Act
            ds.DeleteVocabulary(vocabulary);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.VocabulariesTableName, rowCount - 1);
                DatabaseAssert.RecordDoesNotExist(connection, ContentDataTestHelper.VocabulariesTableName, keyField, Constants.VOCABULARY_DeleteVocabularyId.ToString());
            }
        }

        #endregion

        #region GetVocabularies Tests

        [Test]
        public void DataService_GetVocabularies_Returns_Reader_Of_All_The_Vocabulary_Records()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getVocabularies);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetVocabularies();

            //Assert
            int records = 0;
            while (dataReader.Read())
            {
                records += 1;
            }

            dataReader.Close();

            //Assert that the count is correct
            Assert.AreEqual(Constants.VOCABULARY_ValidCount, records);
        }

        #endregion

        #region UpdateVocabulary Tests

        [Test]
        public void DataService_UpdateVocabulary_Does_Nothing_On_InValid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), upateVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_InValidVocabularyId;
            vocabulary.Name = Constants.VOCABULARY_UpdateName;

            DataService ds = new DataService();

            //Act
            ds.UpdateVocabulary(vocabulary, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.VocabulariesTableName, rowCount);

                //Check that values have not changed
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.VocabulariesTableName, keyField, Constants.VOCABULARY_UpdateVocabularyId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Name", Constants.VOCABULARY_OriginalUpdateName);
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateVocabulary_Updates_Record_On_Valid_Vocabulary()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.VocabulariesTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), upateVocabulary);

            Vocabulary vocabulary = ContentTestHelper.CreateValidVocabulary();
            vocabulary.VocabularyId = Constants.VOCABULARY_UpdateVocabularyId;
            vocabulary.Name = Constants.VOCABULARY_UpdateName;
            vocabulary.ScopeId = Constants.VOCABULARY_UpdateScopeId;
            vocabulary.ScopeTypeId = Constants.VOCABULARY_UpdateScopeTypeId;
            vocabulary.Weight = Constants.VOCABULARY_UpdateWeight;

            DataService ds = new DataService();

            //Act
            ds.UpdateVocabulary(vocabulary, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.VocabulariesTableName, rowCount);

                //Check Values are updated
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.VocabulariesTableName, keyField, Constants.VOCABULARY_UpdateVocabularyId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Name", Constants.VOCABULARY_UpdateName);
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ScopeID", Constants.VOCABULARY_UpdateScopeId);
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "ScopeTypeID", Constants.VOCABULARY_UpdateScopeTypeId);
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Weight", Constants.VOCABULARY_UpdateWeight);
                }

                dataReader.Close();
            }
        }

        #endregion
    }
}