'
' DotNetNuke - http://www.dotnetnuke.com
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
'

Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Entities.Content.Data

    Public Interface IDataService

        'Content Item Methods
        Function AddContentItem(ByVal contentItem As ContentItem, ByVal createdByUserId As Integer) As Integer
        Sub DeleteContentItem(ByVal contentItem As ContentItem)
        Function GetContentItem(ByVal contentItemId As Integer) As IDataReader
        Function GetContentItemsByTerm(ByVal term As String) As IDataReader
        Function GetUnIndexedContentItems() As IDataReader
        Sub UpdateContentItem(ByVal contentItem As ContentItem, ByVal lastModifiedByUserId As Integer)

        'ContentType Methods
        Function AddContentType(ByVal contentType As ContentType) As Integer
        Sub DeleteContentType(ByVal contentType As ContentType)
        Function GetContentTypes() As IDataReader
        Sub UpdateContentType(ByVal contentType As ContentType)

        'ScopeType Methods
        Function AddScopeType(ByVal scopeType As ScopeType) As Integer
        Sub DeleteScopeType(ByVal scopeType As ScopeType)
        Function GetScopeTypes() As IDataReader
        Sub UpdateScopeType(ByVal scopeType As ScopeType)

        'Term Methods
        Function AddHeirarchicalTerm(ByVal term As Term, ByVal createdByUserId As Integer) As Integer
        Function AddSimpleTerm(ByVal term As Term, ByVal createdByUserId As Integer) As Integer
        Sub AddTermToContent(ByVal term As Term, ByVal contentItem As ContentItem)
        Sub DeleteSimpleTerm(ByVal term As Term)
        Sub DeleteHeirarchicalTerm(ByVal term As Term)
        Function GetTerm(ByVal termId As Integer) As IDataReader
        Function GetTermsByContent(ByVal contentItemId As Integer) As IDataReader
        Function GetTermsByVocabulary(ByVal vocabularyId As Integer) As IDataReader
        Sub RemoveTermsFromContent(ByVal contentItem As ContentItem)
        Sub UpdateHeirarchicalTerm(ByVal term As Term, ByVal lastModifiedByUserId As Integer)
        Sub UpdateSimpleTerm(ByVal term As Term, ByVal lastModifiedByUserId As Integer)

        'Vocabulary Methods
        Function AddVocabulary(ByVal vocabulary As Vocabulary, ByVal createdByUserId As Integer) As Integer
        Sub DeleteVocabulary(ByVal vocabulary As Vocabulary)
        Function GetVocabularies() As IDataReader
        Sub UpdateVocabulary(ByVal vocabulary As Vocabulary, ByVal lastModifiedByUserId As Integer)

    End Interface

End Namespace
