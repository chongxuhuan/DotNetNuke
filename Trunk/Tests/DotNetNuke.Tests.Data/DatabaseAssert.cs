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
using System.Data.SqlClient;
using System.Data;
using MbUnit.Framework;

namespace DotNetNuke.Tests.Data
{
    public class DatabaseAssert
    {

        #region Private Methods

        private static void DatabaseCountIsEqual(SqlConnection connection, string databaseName, int expectedCount)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.DatabaseCountScript);
            sqlScript = String.Format(sqlScript, databaseName);

            Assert.AreEqual<int>(expectedCount, DataUtil.ExecuteScalar(connection, sqlScript));
        }

        private static void ObjectCountIsEqual(SqlConnection connection, string objectName, int expectedCount)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.ObjectCountScript);
            sqlScript = String.Format(sqlScript, objectName);

            Assert.AreEqual<int>(expectedCount, DataUtil.ExecuteScalar(connection, sqlScript));
        }

        #endregion

        #region Public Methods

        public static void DatabaseExists(SqlConnection connection, string databaseName)
        {
            DatabaseCountIsEqual(connection, databaseName, 1);
        }

        public static void DatabaseDoesNotExist(SqlConnection connection, string databaseName)
        {
            DatabaseCountIsEqual(connection, databaseName, 0);
        }

        public static void ObjectExists(SqlConnection connection, string objectName)
        {
            ObjectCountIsEqual(connection, objectName, 1);
        }

        public static void ObjectDoesNotExist(SqlConnection connection, string objectName)
        {
            ObjectCountIsEqual(connection,  objectName, 0);
        }

        public static void ReaderColumnNameIsEqual(IDataReader reader, int columnNo, string expectedName)
        {
            DataTable schema = reader.GetSchemaTable();
            DataRow schemaColumn = schema.Rows[columnNo];
            string columnName = schemaColumn["ColumnName"] as string;

            Assert.AreEqual<string>(expectedName, columnName);
        }

        public static void ReaderColumnCountIsEqual(IDataReader reader, int expectedCount)
        {
            Assert.AreEqual<int>(expectedCount, reader.FieldCount);
        }

        public static void ReaderRowCountIsEqual(IDataReader reader, int expectedCount)
        {
            int records = 0;
            while (reader.Read())
            {
                records += 1;
            }

            Assert.AreEqual<int>(expectedCount, records);
        }

        public static void ReaderRowCountIsNotEqual(IDataReader reader, int expectedCount)
        {
            int records = 0;
            while (reader.Read())
            {
                records += 1;
            }

            Assert.AreNotEqual<int>(expectedCount, records);
        }

        public static void ReaderColumnIsNull(IDataReader reader, string fieldName)
        {
            Assert.AreEqual(DBNull.Value, reader[fieldName]);
        }

        public static void ReaderColumnIsEqual<T>(IDataReader reader, string fieldName, T expectedValue)
        {
            if (typeof(T) == typeof(int))
            {
                int intValue = Convert.ToInt32(reader[fieldName]);
                int intExpectedValue = Convert.ToInt32(expectedValue);
                Assert.AreEqual<int>(intExpectedValue, intValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                bool boolValue = Convert.ToBoolean(reader[fieldName]);
                bool boolExpectedValue = Convert.ToBoolean(expectedValue);
                Assert.AreEqual<bool>(boolExpectedValue, boolValue);
            }
            else if (typeof(T) == typeof(string))
            {
                string stringValue = Convert.ToString(reader[fieldName]);
                string stringExpectedValue = Convert.ToString(expectedValue);
                Assert.AreEqual<string>(stringExpectedValue, stringValue);
            }
            else
            {
                object columnValue = reader[fieldName];
                Assert.AreEqual<object>(expectedValue, columnValue);
            }
        }

        public static void ReaderColumnIsNotEqual<T>(IDataReader reader, string fieldName, T expectedValue)
        {
            if (reader.Read())
            {
                if (typeof(T) == typeof(int))
                {
                    int intValue = Convert.ToInt32(reader[fieldName]);
                    int intExpectedValue = Convert.ToInt32(expectedValue);
                    Assert.AreNotEqual<int>(intExpectedValue, intValue);
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool boolValue = Convert.ToBoolean(reader[fieldName]);
                    bool boolExpectedValue = Convert.ToBoolean(expectedValue);
                    Assert.AreNotEqual<bool>(boolExpectedValue, boolValue);
                }
                else if (typeof(T) == typeof(string))
                {
                    string stringValue = Convert.ToString(reader[fieldName]);
                    string stringExpectedValue = Convert.ToString(expectedValue);
                    Assert.AreNotEqual<string>(stringExpectedValue, stringValue);
                }
                else
                {
                    object columnValue = reader[fieldName];
                    Assert.AreNotEqual<object>(expectedValue, columnValue);
                }
            }
        }

        public static void RecordCountIsEqual(SqlConnection connection, string tableName, int expectedCount)
        {
            Assert.AreEqual<int>(expectedCount, DataUtil.GetRecordCount(connection, tableName));
        }

        public static void RecordCountIsEqual(string connetionString, string tableName, int expectedCount)
        {
            using (SqlConnection connection = new SqlConnection(connetionString))
            {
                connection.Open();
                RecordCountIsEqual(connection, tableName, expectedCount);
            }
        }

        public static void RecordDoesNotExist(SqlConnection connection, string tableName, string fieldName, string fieldValue)
        {
            ReaderRowCountIsEqual(DataUtil.GetRecordsByField(connection, tableName, fieldName, fieldValue), 0);
        }

        public static void RecordDoesNotExist(string connetionString, string tableName, string fieldName, string fieldValue)
        {
            using (SqlConnection connection = new SqlConnection(connetionString))
            {
                connection.Open();
                ReaderRowCountIsEqual(DataUtil.GetRecordsByField(connection, tableName, fieldName, fieldValue), 0);
            }
        }

        public static void RecordExists(SqlConnection connection, string tableName, string fieldName, string fieldValue)
        {
            ReaderRowCountIsEqual(DataUtil.GetRecordsByField(connection, tableName, fieldName, fieldValue), 1);
        }

        public static void RecordLastAddedIdEquals(SqlConnection connection, string tableName, string keyFieldName, int expectedValue)
        {
            string sqlScript = DataUtil.RemoveTokens(DataResources.GetLastAddedRecordID);
            sqlScript = String.Format(sqlScript, tableName, keyFieldName);

            Assert.AreEqual<int>(expectedValue, DataUtil.ExecuteScalar(connection, sqlScript));
        }

        public static void RecordLastAddedIdEquals(string connetionString, string tableName, string keyFieldName, int expectedValue)
        {
            using (SqlConnection connection = new SqlConnection(connetionString))
            {
                connection.Open();
                RecordLastAddedIdEquals(connection, tableName, keyFieldName, expectedValue);
            }
        }

        public static void TableIsEmpty(SqlConnection connection, string tableName)
        {
            RecordCountIsEqual(connection, tableName, 0);
        }

        #endregion
    }
}
