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

using DotNetNuke.Entities.Content.Taxonomy;

namespace DotNetNuke.Tests.Utilities
{
    public class Constants
    {
        #region Cacheing Constants

        public const string CACHEING_InValidKey = "InValidKey";
        public const string CACHEING_ParamCacheKey = "CacheKey";
        public const string CACHEING_ValidKey = "ValidKey";
        public const string CACHEING_ValidValue = "ValidValue";

        #endregion

        #region ContentItem Constants

        //Valid Content values
        public const int CONTENT_ValidContentItemId = 1;
        public const string CONTENT_ValidContent = "Content";
        public const string CONTENT_ValidContentKey = "ContentKey";
        public const int CONTENT_ValidModuleId = 30;
        public const int CONTENT_ValidPortalId = 20;
        public const int CONTENT_ValidTabId = 10;

        public const int CONTENT_ValidContentItemCount = 5;
        public const string CONTENT_ValidContentFormat = "Content {0}";
        public const string CONTENT_ValidContentKeyFormat = "ContentKey {0}";
        public const int CONTENT_ValidStartTabId = 10;
        public const int CONTENT_ValidStartModuleId = 100;

        //InValid Content values
        public const int CONTENT_InValidContentItemId = 999;
        public const string CONTENT_InValidContent = "";
        public const int CONTENT_InValidModuleId = 888;
        public const int CONTENT_InValidPortalId = 777;
        public const int CONTENT_InValidTabId = 99;

        public const int CONTENT_IndexedTrueItemCount = 2;
        public const int CONTENT_TaggedItemCount = 4;
        public const int CONTENT_IndexedFalseItemCount = 3;
        public const bool CONTENT_IndexedFalse = false;
        public const bool CONTENT_IndexedTrue = true;

        public const int CONTENT_AddContentItemId = 2;
        public const int CONTENT_DeleteContentItemId = 3;
        public const int CONTENT_UpdateContentItemId = 4;

        public const string CONTENT_UpdateContent = "Update";
        public const string CONTENT_UpdateContentKey = "UpdateKey";

        #endregion

        #region ContentType Constants

        public const int CONTENTTYPE_ValidContentTypeId = 1;
        public const string CONTENTTYPE_ValidContentType = "ContentType Name";

        public const int CONTENTTYPE_ValidContentTypeCount = 5;
        public const string CONTENTTYPE_ValidContentTypeFormat = "ContentType Name {0}";

        public const int CONTENTTYPE_InValidContentTypeId = 999;
        public const string CONTENTTYPE_InValidContentType = "Invalid ContentType";

        public const int CONTENTTYPE_AddContentTypeId = 2;
        public const int CONTENTTYPE_DeleteContentTypeId = 3;
        public const int CONTENTTYPE_UpdateContentTypeId = 4;
        public const int CONTENTTYPE_GetByNameContentTypeId = 5;
        public const string CONTENTTYPE_GetByNameContentType = "TestGetByName";
        public const string CONTENTTYPE_OriginalUpdateContentType = "TestUpdate";

        public const string CONTENTTYPE_UpdateContentType = "Update Name";

        #endregion

        #region ScopeType Constants

        public const int SCOPETYPE_ValidScopeTypeId = 1;
        public const string SCOPETYPE_ValidScopeType = "ScopeType Name";

        public const int SCOPETYPE_ValidScopeTypeCount = 5;
        public const string SCOPETYPE_ValidScopeTypeFormat = "ScopeType Name {0}";

        public const int SCOPETYPE_InValidScopeTypeId = 999;
        public const string SCOPETYPE_InValidScopeType = "Invalid ScopeType";

        public const int SCOPETYPE_AddScopeTypeId = 2;
        public const int SCOPETYPE_DeleteScopeTypeId = 3;
        public const int SCOPETYPE_UpdateScopeTypeId = 4;
        public const int SCOPETYPE_GetByNameScopeTypeId = 5;
        public const string SCOPETYPE_GetByNameScopeType = "TestGetByName";
        public const string SCOPETYPE_OriginalUpdateScopeType = "TestUpdate";

        public const string SCOPETYPE_UpdateScopeType = "Update Name";

        #endregion

        #region Tag Constants

        public const int TAG_DuplicateContentItemId = 1;
        public const int TAG_DuplicateTermId = 6;
        public const int TAG_NoContentContentId = 99;
        public const int TAG_ValidContentId = 1;
        public const int TAG_ValidContentCount = 2;

        #endregion

        #region Term Constants

        public const string TERM_CacheKey = "DNN_Terms_{0}";

        public const int TERM_ValidTermId = 1;
        public const int TERM_InValidTermId = 999;
        public const int TERM_AddTermId = 2;
        public const int TERM_DeleteTermId = 3;
        public const int TERM_UpdateTermId = 4;
        public const int TERM_ValidParentTermId = 2;
        public const int TERM_InValidParentTermId = 888;

        public const string TERM_ValidName = "Term Name";
        public const string TERM_InValidName = "";
        public const string TERM_UnusedName = "Unused";
        public const string TERM_UpdateName = "Update Name";
        public const string TERM_OriginalUpdateName = "LCD";
        public const int TERM_ValidVocabularyId = 2;
        public const int TERM_ValidGetTermsByVocabularyCount = 9;

        public const int TERM_InsertChildBeforeParentId = 2;

        public const int TERM_ValidTermStartId = 1;
        public const int TERM_ValidCount = 5;
        public const string TERM_ValidNameFormat = "Term Name {0}";
        public const int TERM_ValidCountForVocabulary1 = 2;
        public const int TERM_ValidVocabulary1 = 1;
        public const int TERM_ValidVocabulary2 = 2;
        public const int TERM_ValidWeight = 0;
        public const int TERM_UpdateWeight = 5;

        public const int TERM_ValidCountForContent1 = 2;
        public const int TERM_ValidContent1 = 1;
        public const int TERM_ValidContent2 = 2;

        #endregion

        #region Vocabulary Constants

        public const string VOCABULARY_CacheKey = "DNN_Vocabularies";

        public const int VOCABULARY_ValidVocabularyId = 1;
        public const int VOCABULARY_HierarchyVocabularyId = 2;
        public const int VOCABULARY_InValidVocabularyId = 999;
        public const int VOCABULARY_AddVocabularyId = 2;
        public const int VOCABULARY_DeleteVocabularyId = 3;
        public const int VOCABULARY_UpdateVocabularyId = 4;

        public const string VOCABULARY_ValidName = "Vocabulary Name";
        public const string VOCABULARY_InValidName = "";
        public const string VOCABULARY_UpdateName = "Update Name";

        public const VocabularyType VOCABULARY_ValidType = VocabularyType.Simple;
        public const int VOCABULARY_SimpleTypeId = 1;
        public const int VOCABULARY_HierarchyTypeId = 2;

        public const int VOCABULARY_ValidScopeTypeId = 2;
        public const int VOCABULARY_InValidScopeTypeId = 888;
        public const int VOCABULARY_UpdateScopeTypeId = 3;

        public const int VOCABULARY_ValidScopeId = 1;
        public const int VOCABULARY_InValidScopeId = 3;
        public const int VOCABULARY_UpdateScopeId = 2;
        public const string VOCABULARY_OriginalUpdateName = "TestUpdate";

        public const int VOCABULARY_ValidWeight = 0;
        public const int VOCABULARY_UpdateWeight = 5;

        public const int VOCABULARY_ValidCount = 5;
        public const string VOCABULARY_ValidNameFormat = "Vocabulary Name {0}";
        public const int VOCABULARY_ValidCountForScope1 = 2;
        public const int VOCABULARY_ValidScope1 = 1;
        public const int VOCABULARY_ValidScope2 = 2;




        #endregion

        public const int USER_ValidId = 200;
        public const int USER_InValidId = 42;
        public const int USER_AnonymousUserId = -1;
        public const int TAB_ValidId = 10;
        public const int MODULE_ValidId = 100;
    }
}
