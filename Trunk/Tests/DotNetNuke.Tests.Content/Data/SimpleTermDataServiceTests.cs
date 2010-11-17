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
    /// Summary description for SimpleTerm Tests
    /// </summary>
    [TestFixture]
    public class SimpleTermDataServiceTests
    {
        #region Private Members

        private static string addSimpleTerm = "AddSimpleTerm";
        private static string deleteTerm = "DeleteSimpleTerm";
        private static string getTerm = "GetTerm";
        private static string getTermsByVocabulary = "GetTermsByVocabulary";
        private static string keyField = "TermId";
        private static string updateTerm = "UpdateSimpleTerm";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\Terms";
        private static int columnCount = 12;

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

        #region AddSimpleTerm Tests

        [Test]
        public void DataService_AddSimpleTerm_Throws_On_InValid_VocabularyId()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addSimpleTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_InValidVocabularyId);

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.AddSimpleTerm(term, Constants.USER_ValidId));
        }

        [Test]
        public void DataService_AddSimpleTerm_Adds_Record_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addSimpleTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            DataService ds = new DataService();

            //Act
            int termItemId = ds.AddSimpleTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount + 1);
        }

        [Test]
        public void DataService_AddSimpleTerm_Returns_Correct_Id_On_Valid_Term()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addSimpleTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);

            DataService ds = new DataService();

            //Act
            int termItemId = ds.AddSimpleTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                                   "TermID", termItemId);
        }

        #endregion

        #region DeleteSimpleTerm Tests

        [Test]
        public void DataService_DeleteSimpleTerm_Should_Do_Nothing_On_InValid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_InValidTermId;

            DataService ds = new DataService();

            //Act
            ds.DeleteSimpleTerm(term);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.TermsTableName, rowCount);
            }
        }

        [Test]
        public void DataService_DeleteSimpleTerm_Delete_Record_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_DeleteTermId;

            DataService ds = new DataService();

            //Act
            ds.DeleteSimpleTerm(term);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.TermsTableName, rowCount - 1);
                DatabaseAssert.RecordDoesNotExist(connection, ContentDataTestHelper.TermsTableName, keyField,
                                                  Constants.TERM_DeleteTermId.ToString());
            }
        }

        #endregion

        #region GetTerm Tests

        [Test]
        public void DataService_GetTerm_Returns_Empty_Reader_On_InValid_Id()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getTerm);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetTerm(Constants.TERM_InValidTermId);

            //Assert
            DatabaseAssert.ReaderColumnCountIsEqual(dataReader, columnCount);
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, 0);

            dataReader.Close();
        }

        [Test]
        public void DataService_GetTerm_Returns_Reader_Of_The_Term_On_Valid_Id()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getTerm);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetTerm(Constants.TERM_ValidTermId);

            //Assert
            int records = 0;
            while (dataReader.Read())
            {
                DatabaseAssert.ReaderColumnIsEqual(dataReader, keyField, Constants.TERM_ValidTermId);
                records += 1;
            }

            dataReader.Close();

            //Assert that the count is correct
            Assert.AreEqual(1, records);
        }

        #endregion

        #region GetTermsByVocabulary Tests

        [Test]
        public void DataService_GetTermsByVocabulary_Returns_Empty_Reader_On_InValid_VocabularyId()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getTermsByVocabulary);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetTermsByVocabulary(Constants.VOCABULARY_InValidVocabularyId);

            //Assert
            DatabaseAssert.ReaderColumnCountIsEqual(dataReader, columnCount);
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, 0);

            dataReader.Close();
        }

        [Test]
        public void DataService_GetTermsByVocabulary_Returns_Reader_Of_The_Terms_On_Valid_Id()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), getTermsByVocabulary);

            DataService ds = new DataService();

            //Act
            IDataReader dataReader = ds.GetTermsByVocabulary(Constants.TERM_ValidVocabularyId);

            //Assert
            DatabaseAssert.ReaderColumnCountIsEqual(dataReader, columnCount);
            DatabaseAssert.ReaderRowCountIsEqual(dataReader, Constants.TERM_ValidGetTermsByVocabularyCount);

            dataReader.Close();
        }

        #endregion

        #region UpdateSimpleTerm Tests

        [Test]
        public void DataService_UpdateSimpleTerm_Throws_On_InValid_VocabularyId()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_InValidVocabularyId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.UpdateSimpleTerm(term, Constants.USER_ValidId));
        }

        [Test]
        public void DataService_UpdateSimpleTerm_Does_Nothing_On_InValid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_InValidTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            DataService ds = new DataService();

            //Act
            ds.UpdateSimpleTerm(term, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.TermsTableName, rowCount);

                //Check that values have not changed
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.TermsTableName,
                                                                    keyField, Constants.TERM_UpdateTermId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Name", Constants.TERM_OriginalUpdateName);
                }

                dataReader.Close();
            }
        }

        [Test]
        public void DataService_UpdateSimpleTerm_Updates_Record_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateTerm);

            Term term = ContentTestHelper.CreateValidSimpleTerm(Constants.VOCABULARY_ValidVocabularyId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            DataService ds = new DataService();

            //Act
            ds.UpdateSimpleTerm(term, Constants.USER_ValidId);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.TermsTableName, rowCount);

                //Check Values are updated
                IDataReader dataReader = DataUtil.GetRecordsByField(connection, ContentDataTestHelper.TermsTableName,
                                                                    keyField, Constants.TERM_UpdateTermId.ToString());
                while (dataReader.Read())
                {
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Name", Constants.TERM_UpdateName);
                    DatabaseAssert.ReaderColumnIsEqual(dataReader, "Weight", Constants.TERM_UpdateWeight);
                }

                dataReader.Close();
            }
        }

        #endregion
    }
}