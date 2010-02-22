/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using DotNetNuke.Data;
using DotNetNuke.ComponentModel;
using DotNetNuke.Tests.Utilities.Mocks;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Tests.Data
{
    public class DataUtil
    {
        #region Private Members

        private static Regex sqlDelimiterRegex = new Regex(@"(?<=(?:[^\w]+|^))GO(?=(?: |\t)*?(?:\r?\n|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #endregion

        #region Public Methods

        public static void AddDatabaseObject(string virtualScriptFilePath, string objectName)
        {
            using (SqlConnection connection = new SqlConnection(DataTestHelper.ConnectionString))
            {
                connection.Open();

                //add stored procedure to db
                CreateObject(connection, DataUtil.GetSqlScript(virtualScriptFilePath, objectName), objectName);
            }
        }

        public static void CreateDatabase(SqlConnection connection, string databaseName)
        {
            // Create the database
            ExecuteScript(connection, String.Format(DataResources.CreateDatabase, databaseName));

            //Verify that database was created
            DatabaseAssert.DatabaseExists(connection, databaseName);
        }

        public static void CreateObject(SqlConnection connection, string sqlScript, string objectName)
        {
            // Create the object
            ExecuteScript(connection, sqlScript);

            // Verify that the object was created
            DatabaseAssert.ObjectExists(connection, objectName);
        }

        public static void DropDatabase(SqlConnection connection, string databaseName)
        {
            // Drop the database
            ExecuteScript(connection, String.Format(DataResources.DropDatabase, DataTestHelper.DatabaseName));

            //Verify that database was dropped
            DatabaseAssert.DatabaseDoesNotExist(connection, databaseName);
        }

        public static void EmptyTable(SqlConnection connection, string tableName)
        {
            string clearContentItems = String.Format(RemoveTokens(DataResources.EmptyTable), tableName);

            DataUtil.ExecuteScript(connection, clearContentItems);

            // Verify that the table is empty
            DatabaseAssert.TableIsEmpty(connection, tableName);
        }

        public static void ExecuteNonQuery(SqlConnection connection, string sqlScript)
        {
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlScript;
                cmd.ExecuteNonQuery();
            }
        }

        public static SqlDataReader ExecuteReader(SqlConnection connection, string sqlScript)
        {
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = RemoveTokens(sqlScript);
                return cmd.ExecuteReader();
            }
        }

        public static int ExecuteScalar(SqlConnection connection, string sqlScript)
        {
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = sqlScript;
                return (int)cmd.ExecuteScalar();
            }
        }

        public static void ExecuteScript(SqlConnection connection, string sqlScript)
        {
            using (SqlCommand cmd = connection.CreateCommand())
            {
                string[] scripts = sqlDelimiterRegex.Split(sqlScript);

                foreach(string script in scripts)
                {
                    cmd.CommandText = RemoveTokens(script);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetRecordCount(string connectionString, string tableName)
        {
            int rowCount = Null.NullInteger;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                rowCount = GetRecordCount(connection, tableName);
            }
            return rowCount;
        }

        public static int GetRecordCount(SqlConnection connection, string tableName)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.RecordCountScript);
            sqlScript = String.Format(sqlScript, tableName);

            return DataUtil.ExecuteScalar(connection, sqlScript);
        }

        public static IDataReader GetRecordsByField(SqlConnection connection, string tableName, string fieldName, string fieldValue)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.GetRecordsByField);
            sqlScript = String.Format(sqlScript, tableName, fieldName, fieldValue);

            return DataUtil.ExecuteReader(connection, sqlScript);
        }

        public static Stream GetTestFileStream(string fileName, string filePath)
        {
            string fullName = filePath + "\\" + fileName;

            return new FileStream(fullName, FileMode.Open, FileAccess.Read);
        }

        public static string GetSqlScript(string scriptPath, string fileName)
        {
            fileName = scriptPath + "\\" + fileName;

            if (!fileName.EndsWith(".sql"))
                fileName += ".sql";

            string text = "";
            using (StreamReader reader = new StreamReader(GetTestFileStream(fileName, DataTestHelper.FilePath)))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    text += line + "\n";
                }
            }
            return text;
        }

        public static string RemoveTokens(string sqlScript)
        {
            sqlScript = sqlScript.Replace("{databaseOwner}", DataTestHelper.DatabaseOwner);
            sqlScript = sqlScript.Replace("{objectQualifier}", DataTestHelper.ObjectQualifier);

            return sqlScript;
        }

        public static void ReseedIdentityColumn(SqlConnection connection, string tableName, int newValue)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.ReSeedIdentityColumn);
            sqlScript = String.Format(sqlScript, tableName, newValue - 1);

            DataUtil.ExecuteNonQuery(connection, sqlScript);
        }

        public static void SetupDataBaseProvider(string connectionString, string databaseOwner, string objectQualifier)
        {
            if (ComponentFactory.Container == null)
                //Create a Container
                ComponentFactory.Container = new SimpleContainer();

            if (DataProvider.Instance() == null)
            {
                //Register the Settings for the SqlDataProvider
                Dictionary<string, string> settings = new Dictionary<string, string>();
                settings["providerName"] = DataResources.ProviderName;
                settings["providerPath"] = DataResources.ProviderPath;
                settings["connectionString"] = connectionString;
                settings["databaseOwner"] = databaseOwner;
                settings["objectQualifier"] = objectQualifier;
                settings["upgradeConnectionString"] = "";

                //Register the Settings
                ComponentFactory.Container.RegisterComponentSettings(DataResources.ProviderType, settings);

                //Register the DataProvider with the Container
                ComponentFactory.Container.RegisterComponentInstance<DataProvider>(new SqlDataProvider(false));

                //Register MockCachingProvider
                MockCachingProvider.CreateMockProvider();
            }
        }

        #endregion
    }
}
