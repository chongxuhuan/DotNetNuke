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
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Tests.Content
{
    public class ContentTestHelper
    {

        private static void AddBaseEntityColumns(DataTable table)
        {
            table.Columns.Add("CreatedByUserID", typeof(int));
            table.Columns.Add("CreatedOnDate", typeof(DateTime));
            table.Columns.Add("LastModifiedByUserID", typeof(int));
            table.Columns.Add("LastModifiedOnDate", typeof(DateTime));
        }

        internal static void AddContentTypeToTable(DataTable table, int id, string contentType)
        {
            table.Rows.Add(new object[] { id, contentType });
        }

        internal static void AddScopeTypeToTable(DataTable table, int id, string scopeType)
        {
            table.Rows.Add(new object[] { id, scopeType });
        }

        internal static void AddTermToTable(DataTable table, int id, int contentItemId, int vocabularyId, string name, string description, int weight, int parentId)
        {
            table.Rows.Add(new object[] { id, contentItemId, vocabularyId, name, description, weight, parentId });
        }

        internal static void AddVocabularyToTable(DataTable table, int id, int typeId, string name, string description, int scopeId, int scopeTypeId, int weight)
        {
            table.Rows.Add(new object[] { id, typeId, name, description, scopeId, scopeTypeId, weight });
        }

        internal static DataTable CopyScopeTypeRowsToNewTable(DataRow[] rows)
        {
            DataTable tempTable = ContentTestHelper.CreateScopeTypeTable();

            for (int i = 0; i < rows.Length; i++)
            {
                ContentTestHelper.AddScopeTypeToTable(tempTable,
                                (int)rows[i]["ScopeTypeID"],
                                (string)rows[i]["ScopeType"]
                );
            }

            return tempTable;
        }

        internal static DataTable CopyTermRowsToNewTable(DataRow[] rows)
        {
            DataTable tempTable = CreateTermTable();

            for (int i = 0; i < rows.Length; i++)
            {
                AddTermToTable(tempTable,
                                (int)rows[i]["TermID"],
                                (int)rows[i]["ContentItemID"],
                                (int)rows[i]["VocabularyID"],
                                (string)rows[i]["Name"],
                                (string)rows[i]["Description"],
                                (int)rows[i]["Weight"],
                                (int)rows[i]["ParentTermID"]
                );
            }

            return tempTable;
        }

        internal static DataTable CopyVocabularyRowsToNewTable(DataRow[] rows)
        {
            DataTable tempTable = CreateVocabularyTable();

            for (int i = 0; i < rows.Length; i++)
            {
                AddVocabularyToTable(tempTable,
                                    (int) rows[i]["VocabularyID"],
                                    (int) rows[i]["VocabularyTypeID"],
                                    (string) rows[i]["Name"],
                                    (string) rows[i]["Description"],
                                    (int) rows[i]["ScopeID"],
                                    (int) rows[i]["ScopeTypeID"],
                                    (int)rows[i]["Weight"]
                );
            }

            return tempTable;
        }

        internal static DataTable CreateContentTypeTable()
        {
            // Create Tags table.
            DataTable table = new DataTable();

            // Create columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("ContentTypeID", typeof(int));
            table.Columns.Add("ContentType", typeof(string));
            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { idColumn };

            return table;
        }

        internal static DataTable CreateScopeTypeTable()
        {
            // Create Tags table.
            DataTable table = new DataTable();

            // Create columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("ScopeTypeID", typeof(int));
            table.Columns.Add("ScopeType", typeof(string));
            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { idColumn };

            return table;
        }

        internal static DataTable CreateTermTable()
        {
            // Create Vocabulary table.
            DataTable table = new DataTable();

            // Create columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("TermID", typeof(int));
            table.Columns.Add("ContentItemID", typeof(int));
            table.Columns.Add("VocabularyID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Weight", typeof(int));
            table.Columns.Add("ParentTermID", typeof(int));
            table.Columns.Add("TermLeft", typeof(int));
            table.Columns.Add("TermRight", typeof(int));
            AddBaseEntityColumns(table);

            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { idColumn };

            return table;
        }

        internal static ContentItem CreateValidContentItem()
        {
            ContentItem content = new ContentItem { 
                                        Content = Constants.CONTENT_ValidContent, 
                                        ContentKey = Constants.CONTENT_ValidContentKey, 
                                        Indexed = Constants.CONTENT_IndexedFalse };
            return content;
        }

        internal static ContentType CreateValidContentType()
        {
            ContentType contentType = new ContentType { ContentType = Constants.CONTENTTYPE_ValidContentType };
            return contentType;
        }

        internal static Term CreateValidHeirarchicalTerm(int vocabularyId, int parentId)
        {
            Term term = new Term(vocabularyId)
            {
                Name = Constants.TERM_ValidName,
                Description = Constants.TERM_ValidName,
                Weight = Constants.TERM_ValidWeight,
                ParentTermId = parentId
            };
            return term;
        }

        internal static ScopeType CreateValidScopeType()
        {
            ScopeType scopeType = new ScopeType { ScopeType = Constants.SCOPETYPE_ValidScopeType };
            return scopeType;
        }

        internal static Term CreateValidSimpleTerm(int vocabularyId)
        {
            Term term = new Term(vocabularyId) { 
                                        Name = Constants.TERM_ValidName, 
                                        Description = Constants.TERM_ValidName, 
                                        Weight = Constants.TERM_ValidWeight };
            return term;
        }

        internal static Vocabulary CreateValidVocabulary()
        {
            Vocabulary vocabulary = new Vocabulary { 
                                        Name = Constants.VOCABULARY_ValidName, 
                                        Type = Constants.VOCABULARY_ValidType, 
                                        ScopeTypeId = Constants.VOCABULARY_ValidScopeTypeId, 
                                        ScopeId = Constants.VOCABULARY_ValidScopeId, 
                                        Weight = Constants.VOCABULARY_ValidWeight };

            return vocabulary;
        }

        internal static DataTable CreateVocabularyTable()
        {
            // Create Vocabulary table.
            DataTable table = new DataTable();

            // Create columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("VocabularyID", typeof(int));
            table.Columns.Add("VocabularyTypeID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("ScopeID", typeof(int));
            table.Columns.Add("ScopeTypeID", typeof(int));
            table.Columns.Add("Weight", typeof(int));
            AddBaseEntityColumns(table);

            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { idColumn };

            return table;
        }

        internal static string GetContent(int i)
        {
            return String.Format(String.Format(Constants.CONTENT_ValidContentFormat, i.ToString()));
        }

        public static string GetContentKey(int i)
        {
            return String.Format(String.Format(Constants.CONTENT_ValidContentKeyFormat, i.ToString()));
        }

        internal static string GetContentType(int i)
        {
            return String.Format(Constants.CONTENTTYPE_ValidContentTypeFormat, i.ToString());
        }

        internal static string GetScopeType(int i)
        {
            return String.Format(Constants.SCOPETYPE_ValidScopeTypeFormat, i.ToString());
        }

        internal static string GetTermName(int i)
        {
            return String.Format(Constants.TERM_ValidNameFormat, i.ToString());
        }

        internal static string GetVocabularyName(int i)
        {
            return String.Format(Constants.VOCABULARY_ValidNameFormat, i.ToString());
        }

    }
}
