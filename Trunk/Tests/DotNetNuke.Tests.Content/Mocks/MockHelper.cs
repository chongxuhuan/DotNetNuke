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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Tests.Utilities;
using DotNetNuke.Web.Validators;
using Moq;
using DotNetNuke.ComponentModel;
using System.Web;
using DotNetNuke.Entities.Content.Data;
using DotNetNuke.Entities.Content;
using DotNetNuke.Common.Utilities;
using System.Data;

namespace DotNetNuke.Tests.Content.Mocks
{
    public static class MockHelper
    {

        private static void AddBaseEntityColumns(DataTable table)
        {
            table.Columns.Add("CreatedByUserID", typeof(int));
            table.Columns.Add("CreatedOnDate", typeof(DateTime));
            table.Columns.Add("LastModifiedByUserID", typeof(int));
            table.Columns.Add("LastModifiedOnDate", typeof(DateTime));
        }

        private static void AddContentItemToTable(DataTable table, int id, string content, string contentKey, bool indexed, int userId, string term)
        {
            table.Rows.Add(new object[] { id, content, Null.NullInteger, Null.NullInteger, Null.NullInteger, 
                                            contentKey, indexed, userId, term });
        }

        private static DataTable CreateContentItemTable()
        {
            // Create Categories table.
            DataTable table = new DataTable();

            // Create columns, ID and Name.
            DataColumn idColumn = table.Columns.Add("ContentItemID", typeof(int));
            table.Columns.Add("Content", typeof(string));
            table.Columns.Add("ContentTypeID", typeof(int));
            table.Columns.Add("TabID", typeof(int));
            table.Columns.Add("ModuleID", typeof(int));
            table.Columns.Add("ContentKey", typeof(string));
            table.Columns.Add("Indexed", typeof(bool));
            table.Columns.Add("UserID", typeof(int));
            table.Columns.Add("Term", typeof(string));
            AddBaseEntityColumns(table);

            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { idColumn };

            return table;
        }

        internal static IDataReader CreateEmptyContentItemReader()
        {
            return CreateContentItemTable().CreateDataReader();
        }

        private static Mock<TMock> RegisterMockController<TMock>(Mock<TMock> mock) where TMock : class
        {
            if (ComponentFactory.Container == null)
                //Create a Container
                ComponentFactory.Container = new SimpleContainer();

            //Try and get mock
            var getMock = ComponentFactory.GetComponent<Mock<TMock>>();

            if (getMock == null)
            {
                // Create the mock
                getMock = mock;

                // Add both mock and mock.Object to Container
                ComponentFactory.RegisterComponentInstance<Mock<TMock>>(getMock);
                ComponentFactory.RegisterComponentInstance<TMock>(getMock.Object);
            }
            return getMock;
        }

        internal static Mock<HttpContextBase> CreateMockHttpContext()
        {
            Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> httpResponse = new Mock<HttpResponseBase>();
            httpContext.Setup(h => h.Response).Returns(httpResponse.Object);

            return httpContext;
        }

        internal static Mock<IScopeTypeController> CreateMockScopeTypeController()
        {
            // Create the mock
            var mockScopeTypes = new Mock<IScopeTypeController>();
            mockScopeTypes.Setup(s => s.GetScopeTypes())
                            .Returns(TestScopeTypes);

            //Register Mock
            return RegisterMockController<IScopeTypeController>(mockScopeTypes);
        }

        internal static Mock<ITermController> CreateMockTermController()
        {
            // Create the mock
            var mockTerms = new Mock<ITermController>();
            mockTerms.Setup(t => t.GetTermsByVocabulary(Constants.VOCABULARY_ValidVocabularyId))
                        .Returns(TestTerms);

            //Return Mock
            return mockTerms;
        }

        internal static Mock<IVocabularyController> CreateMockVocabularyController()
        {
            // Create the mock
            var mockVocabularies = new Mock<IVocabularyController>();
            mockVocabularies.Setup(v => v.GetVocabularies())
                            .Returns(TestVocabularies);

            //Return Mock
            return mockVocabularies;
        }

        internal static IDataReader CreateValidContentItemReader()
        {
            DataTable table = CreateContentItemTable();
            AddContentItemToTable(table, Constants.CONTENT_ValidContentItemId,
                                    ContentTestHelper.GetContent(Constants.CONTENT_ValidContentItemId),
                                    ContentTestHelper.GetContentKey(Constants.CONTENT_ValidContentItemId),
                                    true, Constants.USER_ValidId, Null.NullString);

            return table.CreateDataReader();
        }

        internal static IDataReader CreateValidContentItemsReader(int count, bool indexed, int startUserId, string term)
        {
            DataTable table = CreateContentItemTable();
            for (int i = Constants.CONTENT_ValidContentItemId;
                       i < Constants.CONTENT_ValidContentItemId + count;
                       i++)
            {
                string content = (count == 1) ? Constants.CONTENT_ValidContent : ContentTestHelper.GetContent(i);
                string contentKey = (count == 1) ? Constants.CONTENT_ValidContentKey : ContentTestHelper.GetContentKey(i);
                int userId = (startUserId == Null.NullInteger) ? Constants.USER_ValidId + i : startUserId;

                AddContentItemToTable(table, i, content, contentKey, indexed, startUserId, term);
            }

            return table.CreateDataReader();
        }

        internal static IDataReader CreateValidMetaDataReader()
        {
            // Create Categories table.
            using (DataTable table = new DataTable())
            {
                // Create columns, ID and Name.
                table.Columns.Add("MetaDataName", typeof(string));
                table.Columns.Add("MetaDataValue", typeof(string));
                for (int i = 0; i < Constants.CONTENT_MetaDataCount; i++)
                {
                    table.Rows.Add(new object[] { String.Format("{0} {1}", Constants.CONTENT_ValidMetaDataName, i), Constants.CONTENT_ValidMetaDataValue });
                }
                return table.CreateDataReader();
            }
        }



        internal static IQueryable<ScopeType> TestScopeTypes
        {
            get
            {
                List<ScopeType> scopeTypes = new List<ScopeType>()
                    {
                        new ScopeType() { ScopeTypeId = 1, ScopeType = "Application" },
                        new ScopeType() { ScopeTypeId = 2, ScopeType = "Portal" }
                    };

                return scopeTypes.AsQueryable();
            }
        }

        internal static IQueryable<Term> TestTerms
        {
            get
            {
                List<Term> terms = new List<Term>();

                for (int i = Constants.TERM_ValidTermId;
                           i < Constants.TERM_ValidTermId + Constants.TERM_ValidCount;
                           i++)
                {
                    Term term = new Term(Constants.VOCABULARY_ValidVocabularyId);
                    term.TermId = i;
                    term.Name = ContentTestHelper.GetTermName(i);
                    term.Description = ContentTestHelper.GetTermName(i);
                    term.Weight = Constants.TERM_ValidWeight;

                    terms.Add(term);
                }

                return terms.AsQueryable();
            }
        }
        
        internal static IQueryable<Vocabulary> TestVocabularies
        {
            get
            {
                List<Vocabulary> vocabularies = new List<Vocabulary>();

                for (int i = Constants.VOCABULARY_ValidVocabularyId;
                           i < Constants.VOCABULARY_ValidVocabularyId + Constants.VOCABULARY_ValidCount;
                           i++)
                {
                    Vocabulary vocabulary = new Vocabulary();
                    vocabulary.VocabularyId = i;
                    vocabulary.Name = ContentTestHelper.GetVocabularyName(i);
                    vocabulary.Type = VocabularyType.Simple;
                    vocabulary.Description = ContentTestHelper.GetVocabularyName(i);
                    vocabulary.ScopeTypeId = Constants.SCOPETYPE_ValidScopeTypeId;
                    vocabulary.Weight = Constants.VOCABULARY_ValidWeight;

                    vocabularies.Add(vocabulary);
                }

                return vocabularies.AsQueryable();
            }
        }
        
        internal static readonly ValidationResult InvalidResult = new ValidationResult(new[] { new ValidationError() { ErrorMessage = "Foo", PropertyName = "Bar" } });

        internal static Mock<ObjectValidator> EnableValidMockValidator<T>(Validator validator, T target) where T : class
        {
            return EnableMockValidator(validator, ValidationResult.Successful, target);
        }

        internal static Mock<ObjectValidator> EnableInvalidMockValidator<T>(Validator validator, T target) where T : class
        {
            return EnableMockValidator(validator,InvalidResult, target);
        }

        internal static Mock<ObjectValidator> EnableMockValidator<T>(Validator validator, ValidationResult result, T target) where T : class
        {
            Mock<ObjectValidator> mockValidator = new Mock<ObjectValidator>();

            Expression<Func<ObjectValidator, ValidationResult>> setupExpression;
            if (target == null)
            {
                setupExpression = v => v.ValidateObject(It.IsAny<T>());
            }
            else
            {
                setupExpression = v => v.ValidateObject(target);
            }
            mockValidator.Setup(setupExpression).Returns(result);

            validator.Validators.Clear();
            validator.Validators.Add(mockValidator.Object);
            return mockValidator;
        }

    }
}