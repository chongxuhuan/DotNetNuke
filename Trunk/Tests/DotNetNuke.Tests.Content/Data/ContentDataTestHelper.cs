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
using System.Data.SqlClient;
using DotNetNuke.Tests.Data;

namespace DotNetNuke.Tests.Content.Data
{
    public class ContentDataTestHelper
    {
        private static string SetupScript = "TestSetupScript";

        public static string ContentItemsTableName = "ContentItems";
        public static string ContentMetaDataTableName = "ContentItems_MetaData";
        public static string ContentTypesTableName = "ContentTypes";
        public static string MetaDataTableName = "MetaData";
        public static string ScopeTypesTableName = "Taxonomy_ScopeTypes";
        public static string ContentTagsTableName = "ContentItems_Tags";
        public static string TermsTableName = "Taxonomy_Terms";
        public static string VocabulariesTableName = "Taxonomy_Vocabularies";
        public static string VocabularyTypesTableName = "Taxonomy_VocabularyTypes";
        private static string virtualScriptFilePath = "Library\\Entities\\Content\\Data\\Scripts";

        public static void AddDataToTables()
        {
            string sqlScript = DataUtil.GetSqlScript(virtualScriptFilePath, SetupScript);

            // Connect to the database to add data to the tables
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                DataUtil.ExecuteScript(connection, sqlScript);
            }
        }

        public static void CreateDatabaseTables()
        {
            // Connect to the database to create the tables
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                //Create VocabularyTypes Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath,
                                                            "\\Tables\\" + VocabularyTypesTableName),
                                      VocabularyTypesTableName);

                //Create ContentTypes Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + ContentTypesTableName),
                                      ContentTypesTableName);

                //Create ScopeTypes Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + ScopeTypesTableName),
                                      ScopeTypesTableName);

                //Create Vocabularies Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + VocabulariesTableName),
                                      VocabulariesTableName);

                //Create Terms Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + TermsTableName),
                                      TermsTableName);

                //Create ContentItems Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + ContentItemsTableName),
                                      ContentItemsTableName);

                //Create MetaData Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + MetaDataTableName),
                                      MetaDataTableName);

                //Create ContentMetaData Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath,
                                                            "\\Tables\\" + ContentMetaDataTableName),
                                      ContentMetaDataTableName);

                //Create Tags Table
                DataUtil.CreateObject(connection,
                                      DataUtil.GetSqlScript(virtualScriptFilePath, "\\Tables\\" + ContentTagsTableName),
                                      ContentTagsTableName);
            }
        }

        public static void EmptyDatabaseTables()
        {
            // Connect to the database to empty the tables
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                //Remove all records in MetaData
                DataUtil.EmptyTable(connection, MetaDataTableName);

                //Remove all records in ContentMetaData
                DataUtil.EmptyTable(connection, ContentMetaDataTableName);

                //Remove all records in Tags
                DataUtil.EmptyTable(connection, ContentTagsTableName);

                //Remove all records in ContentTypes
                DataUtil.EmptyTable(connection, ContentTypesTableName);

                //Remove all records in ContentItems
                DataUtil.EmptyTable(connection, ContentItemsTableName);

                //Remove all records in Terms Table
                DataUtil.EmptyTable(connection, TermsTableName);

                //Remove all records in Vocabularies Table
                DataUtil.EmptyTable(connection, VocabulariesTableName);

                //Remove all records in VocabularyTypes Table
                DataUtil.EmptyTable(connection, VocabularyTypesTableName);

                //Remove all records in ScopeTypes
                DataUtil.EmptyTable(connection, ScopeTypesTableName);
            }
        }
    }
}