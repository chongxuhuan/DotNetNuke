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
    /// Summary description for HeirarchicalTerm Tests
    /// </summary>
    [TestFixture]
    public class HeirarchicalTermDataServiceTests
    {
        #region Private Members

        private static string addHeirarchicalTerm = "AddHeirarchicalTerm";
        private static string deleteHeirarchicalTerm = "DeleteHeirarchicalTerm";
        private static string keyField = "TermId";
        private static string updateHeirarchicalTerm = "UpdateHeirarchicalTerm";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts\\Terms";

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

        #region AddHeirarchicalTerm Tests

        [Test]
        public void DataService_AddHeirarchicalTerm_Throws_On_InValid_VocabularyId()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_InValidVocabularyId,
                                                                      Constants.TERM_ValidParentTermId);

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.AddHeirarchicalTerm(term, Constants.USER_ValidId));
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Throws_On_InValid_ParentTermId()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_ValidVocabularyId,
                                                                      Constants.TERM_InValidParentTermId);

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.AddHeirarchicalTerm(term, Constants.USER_ValidId));
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Adds_First_Record_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = new Term(4)
                            {
                                Name = "Test Term",
                                ParentTermId = null /*No Parent*/,
                                Weight = 0,
                                Description = "Test Term"
                            };

            DataService ds = new DataService();

            //Act
            int newTermId = ds.AddHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount + 1);
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                                   "TermID", newTermId);

            //Assert that new term is correct as first record in Vocabulary
            TestTerm(newTermId, 1, 2);
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Does_Not_Modify_Terms_From_Other_Vocabularies()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = new Term(4)
                            {
                                Name = "Test Term",
                                ParentTermId = null /*No Parent*/,
                                Weight = 0,
                                Description = "Test Term"
                            };

            DataService ds = new DataService();

            //Act
            int newTermId = ds.AddHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert that electronics terms are untouched
            //Televisions, Id = 1, Left=12, Right=17
            TestTerm(1, 12, 17);
            //Flash, Id = 9, Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18
            TestTerm(12, 1, 18);
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Inserts_Record_As_First_Child_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = new Term(2)
                            {
                                Name = "Game Console",
                                ParentTermId = 12 /*Electronics*/,
                                Weight = 0,
                                Description = "Game Consoles like X-Box 360"
                            };

            DataService ds = new DataService();

            //Act
            int newTermId = ds.AddHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount + 1);
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                                   "TermID", newTermId);

            //Assert that new term is correct as first child of electronics
            TestTerm(newTermId, 2, 3);

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=14, Right=19
            TestTerm(1, 14, 19);
            //Flash, Id = 9, Left=8, Right=9 - should be Left=10, Right=11
            TestTerm(9, 10, 11);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=20
            TestTerm(12, 1, 20);
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Inserts_Record_As_Last_Child_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = new Term(2)
                            {
                                Name = "iPod",
                                ParentTermId = 6 /*MP3 Players*/,
                                Weight = 0,
                                Description = "iPod"
                            };

            DataService ds = new DataService();

            //Act
            int newTermId = ds.AddHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount + 1);
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                                   "TermID", newTermId);

            //Assert that new term is correct as last child of mp3 players
            TestTerm(newTermId, 10, 11);

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=14, Right=19
            TestTerm(1, 14, 19);
            //Flash, Id = 9, Left=8, Right=9 - should be Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=20
            TestTerm(12, 1, 20);
        }

        [Test]
        public void DataService_AddHeirarchicalTerm_Inserts_Record_In_Alpha_Order_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), addHeirarchicalTerm);

            Term term = new Term(2)
                            {
                                Name = "Plasma",
                                ParentTermId = 1 /*Televisions*/,
                                Weight = 0,
                                Description = "Plasma"
                            };

            DataService ds = new DataService();

            //Act
            int newTermId = ds.AddHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount + 1);
            DatabaseAssert.RecordLastAddedIdEquals(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                                   "TermID", newTermId);

            //Assert that new term is correct as middle child of televisions
            TestTerm(newTermId, 15, 16);

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=12, Right=19
            TestTerm(1, 12, 19);
            //Flash, Id = 9, Left=8, Right=9 - should be Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=20
            TestTerm(12, 1, 20);
        }

        #endregion

        #region DeleteHeirarchicalTerm Tests

        [Test]
        public void DataService_DeleteHeirarchicalTerm_Should_Do_Nothing_On_InValid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteHeirarchicalTerm);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_ValidVocabularyId,
                                                                      Constants.TERM_ValidParentTermId);
            term.TermId = Constants.TERM_InValidTermId;

            DataService ds = new DataService();

            //Act
            ds.DeleteHeirarchicalTerm(term);

            //Assert
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();
                DatabaseAssert.RecordCountIsEqual(connection, ContentDataTestHelper.TermsTableName, rowCount);
            }
        }

        [Test]
        public void DataService_DeleteHeirarchicalTerm_Delete_Record_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteHeirarchicalTerm);

            int termId = 7;
            Term term = new Term(2)
                            {
                                TermId = 7,
                                Name = "CD Players",
                                ParentTermId = 2 /*Portable Electronics*/,
                                Weight = 0,
                                Description = "CD Players"
                            };

            DataService ds = new DataService();

            //Act
            ds.DeleteHeirarchicalTerm(term);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount - 1);
            DatabaseAssert.RecordDoesNotExist(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              keyField, termId.ToString());

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=10, Right=15
            TestTerm(1, 10, 15);
            //2-Way, Id = 8, Left=3, Right=4 - should be Left=3, Right=4
            TestTerm(8, 3, 4);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=16
            TestTerm(12, 1, 16);
        }

        [Test]
        public void DataService_DeleteHeirarchicalTerm_Does_Not_Modify_Terms_From_Other_Vocabularies()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteHeirarchicalTerm);

            Term term = new Term(3)
                            {
                                TermId = 16,
                                Name = "Test Delete",
                                ParentTermId = null,
                                Weight = 0,
                                Description = "Test Delete"
                            };

            DataService ds = new DataService();

            //Act
            ds.DeleteHeirarchicalTerm(term);

            //Assert that electronics terms are untouched
            //Televisions, Id = 1, Left=12, Right=17
            TestTerm(1, 12, 17);
            //Flash, Id = 9, Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18
            TestTerm(12, 1, 18);
        }

        [Test]
        public void DataService_DeleteHeirarchicalTerm_Delete_Record_Including_Children_On_Valid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), deleteHeirarchicalTerm);

            int termId = 6;
            Term term = new Term(2)
                            {
                                TermId = 6,
                                Name = "MP3 Players",
                                ParentTermId = 2 /*Portable Electronics*/,
                                Weight = 0,
                                Description = "MP3 Players"
                            };

            DataService ds = new DataService();

            //Act
            ds.DeleteHeirarchicalTerm(term);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount - 2);
            DatabaseAssert.RecordDoesNotExist(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              keyField, termId.ToString());

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=8, Right=13
            TestTerm(1, 8, 13);
            //2-Way, Id = 8, Left=3, Right=4 - should be Left=3, Right=4
            TestTerm(8, 3, 4);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=14
            TestTerm(12, 1, 14);
        }

        #endregion

        #region UpdateHeirarchicalTerm Tests

        [Test]
        public void DataService_UpdateHeirarchicalTerm_Throws_On_Invalid_VocabularyId()
        {
            //Arrange
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_InValidVocabularyId,
                                                                      Constants.TERM_ValidParentTermId);
            term.TermId = Constants.TERM_UpdateTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            DataService ds = new DataService();

            //Act/Assert
            Assert.Throws<SqlException>(() => ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId));
        }

        [Test]
        public void DataService_UpdateHeirarchicalTerm_Does_Nothing_On_InValid_Term()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = ContentTestHelper.CreateValidHeirarchicalTerm(Constants.VOCABULARY_ValidVocabularyId,
                                                                      Constants.TERM_ValidParentTermId);
            term.TermId = Constants.TERM_InValidTermId;
            term.Name = Constants.TERM_UpdateName;
            term.Weight = Constants.TERM_UpdateWeight;

            DataService ds = new DataService();

            //Act
            ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId);

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
        public void
            DataService_UpdateHeirarchicalTerm_Updates_Record_But_Does_Not_Modify_Other_Terms_If_ParentID_Does_Not_Change
            ()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = new Term(4)
                            {
                                TermId = 17,
                                Name = "Test Update",
                                ParentTermId = null,
                                Weight = 0,
                                Description = "Updated"
                            };

            DataService ds = new DataService();

            //Act
            ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount);

            //Assert that electronics terms are untouched
            //Televisions, Id = 1, Left=12, Right=17
            TestTerm(1, 12, 17);
            //Flash, Id = 9, Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18
            TestTerm(12, 1, 18);
        }

        [Test]
        public void DataService_UpdateHeirarchicalTerm_Does_Not_Modify_Terms_From_Other_Vocabularies()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = new Term(3)
                            {
                                TermId = 19,
                                Name = "Test Update",
                                ParentTermId = 18,
                                Weight = 0,
                                Description = "Test GrandChild 1"
                            };

            DataService ds = new DataService();

            //Act
            ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount);

            //Assert that electronics terms are untouched
            //Televisions, Id = 1, Left=12, Right=17
            TestTerm(1, 12, 17);
            //Flash, Id = 9, Left=8, Right=9
            TestTerm(9, 8, 9);
            //Electronics, Id = 12, Left=1, Right=18
            TestTerm(12, 1, 18);
        }

        [Test]
        public void DataService_UpdateHeirarchicalTerm_Updates_Records_On_Change_Parent_To_Sibling_Of_Parent()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = new Term(2)
                            {
                                TermId = 6,
                                Name = "MP3 Players",
                                ParentTermId = 12 /*Electronics*/,
                                Weight = 0,
                                Description = "MP3 Players"
                            };

            DataService ds = new DataService();

            //Act
            ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount);

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=12, Right=17
            TestTerm(1, 12, 17);
            //2-Way, Id = 8, Left=3, Right=4 - should be Left=7, Right=8
            TestTerm(8, 7, 8);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=18
            TestTerm(12, 1, 18);
            //MP3 Players, Id = 6, Left=7, Right=10 - should be Left=2, Right=5
            TestTerm(6, 2, 5);
            //Flash Players, Id = 9, Left=8, Right=9 - should be Left=3, Right=4
            TestTerm(9, 3, 4);
        }

        [Test]
        public void DataService_UpdateHeirarchicalTerm_Updates_Records_On_Change_Parent_To_Parent_Of_Parent()
        {
            //Arrange
            int rowCount = DataUtil.GetRecordCount(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName);
            DataUtil.AddDatabaseObject(string.Concat(ContentDataTestHelper.VirtualScriptRootPath, virtualScriptFilePath), updateHeirarchicalTerm);

            Term term = new Term(2)
                            {
                                TermId = 6,
                                Name = "MP3 Players",
                                ParentTermId = 1 /*Televisions*/,
                                Weight = 0,
                                Description = "MP3 Players"
                            };

            DataService ds = new DataService();

            //Act
            ds.UpdateHeirarchicalTerm(term, Constants.USER_ValidId);

            //Assert
            DatabaseAssert.RecordCountIsEqual(DataTestHelper.ConnectionString, ContentDataTestHelper.TermsTableName,
                                              rowCount);

            //Assert that existing terms are correct
            //Televisions, Id = 1, Left=12, Right=17 - should be Left=8, Right=17
            TestTerm(1, 8, 17);
            //2-Way, Id = 8, Left=3, Right=4 - should be Left=3, Right=4
            TestTerm(8, 3, 4);
            //Electronics, Id = 12, Left=1, Right=18 - should be Left=1, Right=18
            TestTerm(12, 1, 18);
            //MP3 Players, Id = 6, Left=7, Right=10 - should be Left=11, Right=14
            TestTerm(6, 11, 14);
            //Flash Players, Id = 9, Left=8, Right=9 - should be Left=12, Right=13
            TestTerm(9, 12, 13);
        }

        #endregion

        private void TestTerm(int newTermId, int expectedTermLeft, int expectedTermRight)
        {
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                //determine that record was inserted correctly
                IDataReader dataReader = DataUtil.ExecuteReader(connection,
                                                                "SELECT TermLeft, TermRight FROM " +
                                                                DataTestHelper.ObjectQualifier +
                                                                ContentDataTestHelper.TermsTableName +
                                                                " WHERE TermID = " + newTermId);

                if (dataReader.Read())
                {
                    //TermLeft should be termLeft, TermRight should be 3
                    Assert.AreEqual(expectedTermLeft, dataReader.GetInt32(0));
                    Assert.AreEqual(expectedTermRight, dataReader.GetInt32(1));
                }
            }
        }
    }
}