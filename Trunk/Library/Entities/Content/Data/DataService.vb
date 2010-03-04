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

Imports DotNetNuke.Data
Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Entities.Content.Data

    Public Class DataService
        Implements IDataService

#Region "Private Members"

        Private provider As DataProvider = DataProvider.Instance()

#End Region

#Region "ContentItem Methods"

        Public Function AddContentItem(ByVal contentItem As ContentItem, ByVal createdByUserId As Integer) As Integer Implements IDataService.AddContentItem
            Return provider.ExecuteScalar(Of Integer)("AddContentItem", _
                                                        contentItem.Content, _
                                                        contentItem.ContentTypeId, _
                                                        contentItem.TabID, _
                                                        contentItem.ModuleID, _
                                                        contentItem.ContentKey, _
                                                        contentItem.Indexed, _
                                                        createdByUserId)
        End Function

        Public Sub DeleteContentItem(ByVal contentItem As ContentItem) Implements IDataService.DeleteContentItem
            provider.ExecuteNonQuery("DeleteContentItem", contentItem.ContentItemId)
        End Sub

        Public Function GetContentItem(ByVal contentItemId As Integer) As IDataReader Implements IDataService.GetContentItem
            Return provider.ExecuteReader("GetContentItem", contentItemId)
        End Function

        Public Function GetContentItemsByTerm(ByVal term As String) As IDataReader Implements IDataService.GetContentItemsByTerm
            Return provider.ExecuteReader("GetContentItemsByTerm", term)
        End Function

        Public Function GetUnIndexedContentItems() As IDataReader Implements IDataService.GetUnIndexedContentItems
            Return provider.ExecuteReader("GetUnIndexedContentItems")
        End Function

        Public Sub UpdateContentItem(ByVal contentItem As ContentItem, ByVal createdByUserId As Integer) Implements IDataService.UpdateContentItem
            provider.ExecuteNonQuery("UpdateContentItem", _
                                        contentItem.ContentItemId, _
                                        contentItem.Content, _
                                        contentItem.ContentTypeId, _
                                        contentItem.TabID, _
                                        contentItem.ModuleID, _
                                        contentItem.ContentKey, _
                                        contentItem.Indexed, _
                                        createdByUserId)
        End Sub

#End Region

#Region "ContentType Methods"

        Function AddContentType(ByVal contentType As ContentType) As Integer Implements IDataService.AddContentType
            Return provider.ExecuteScalar(Of Integer)("AddContentType", contentType.ContentType)
        End Function

        Sub DeleteContentType(ByVal contentType As ContentType) Implements IDataService.DeleteContentType
            provider.ExecuteNonQuery("DeleteContentType", contentType.ContentTypeId)
        End Sub

        Function GetContentTypes() As IDataReader Implements IDataService.GetContentTypes
            Return provider.ExecuteReader("GetContentTypes")
        End Function

        Sub UpdateContentType(ByVal contentType As ContentType) Implements IDataService.UpdateContentType
            provider.ExecuteNonQuery("UpdateContentType", contentType.ContentTypeId, contentType.ContentType)
        End Sub

#End Region

#Region "ScopeType Methods"

        Public Function AddScopeType(ByVal scopeType As ScopeType) As Integer Implements IDataService.AddScopeType
            Return provider.ExecuteScalar(Of Integer)("AddScopeType", scopeType.ScopeType)
        End Function

        Public Sub DeleteScopeType(ByVal scopeType As ScopeType) Implements IDataService.DeleteScopeType
            provider.ExecuteNonQuery("DeleteScopeType", scopeType.ScopeTypeId)
        End Sub

        Public Function GetScopeTypes() As IDataReader Implements IDataService.GetScopeTypes
            Return provider.ExecuteReader("GetScopeTypes")
        End Function

        Public Sub UpdateScopeType(ByVal scopeType As ScopeType) Implements IDataService.UpdateScopeType
            provider.ExecuteNonQuery("UpdateScopeType", scopeType.ScopeTypeId, scopeType.ScopeType)
        End Sub

#End Region

#Region "Term Methods"

        Public Function AddHeirarchicalTerm(ByVal term As Term, ByVal createdByUserId As Integer) As Integer Implements IDataService.AddHeirarchicalTerm
            Return provider.ExecuteScalar(Of Integer)("AddHeirarchicalTerm", term.VocabularyId, term.ParentTermId, term.Name, term.Description, term.Weight, createdByUserId)
        End Function

        Public Function AddSimpleTerm(ByVal term As Term, ByVal createdByUserId As Integer) As Integer Implements IDataService.AddSimpleTerm
            Return provider.ExecuteScalar(Of Integer)("AddSimpleTerm", term.VocabularyId, term.Name, term.Description, term.Weight, createdByUserId)
        End Function

        Public Sub AddTermToContent(ByVal term As Term, ByVal contentItem As ContentItem) Implements IDataService.AddTermToContent
            provider.ExecuteNonQuery("AddTermToContent", term.TermId, contentItem.ContentItemId)
        End Sub

        Public Sub DeleteSimpleTerm(ByVal term As Term) Implements IDataService.DeleteSimpleTerm
            provider.ExecuteNonQuery("DeleteSimpleTerm", term.TermId)
        End Sub

        Public Sub DeleteHeirarchicalTerm(ByVal term As Term) Implements IDataService.DeleteHeirarchicalTerm
            provider.ExecuteNonQuery("DeleteHeirarchicalTerm", term.TermId)
        End Sub

        Public Function GetTerm(ByVal termId As Integer) As IDataReader Implements IDataService.GetTerm
            Return provider.ExecuteReader("GetTerm", termId)
        End Function

        Public Function GetTermsByContent(ByVal contentItemId As Integer) As IDataReader Implements IDataService.GetTermsByContent
            Return provider.ExecuteReader("GetTermsByContent", contentItemId)
        End Function

        Public Function GetTermsByVocabulary(ByVal vocabularyId As Integer) As IDataReader Implements IDataService.GetTermsByVocabulary
            Return provider.ExecuteReader("GetTermsByVocabulary", vocabularyId)
        End Function

        Public Sub RemoveTermsFromContent(ByVal contentItem As ContentItem) Implements IDataService.RemoveTermsFromContent
            provider.ExecuteNonQuery("RemoveTermsFromContent", contentItem.ContentItemId)
        End Sub

        Public Sub UpdateHeirarchicalTerm(ByVal term As Term, ByVal lastModifiedByUserId As Integer) Implements IDataService.UpdateHeirarchicalTerm
            provider.ExecuteNonQuery("UpdateHeirarchicalTerm", term.TermId, term.VocabularyId, term.ParentTermId, term.Name, term.Description, term.Weight, lastModifiedByUserId)
        End Sub

        Public Sub UpdateSimpleTerm(ByVal term As Term, ByVal lastModifiedByUserId As Integer) Implements IDataService.UpdateSimpleTerm
            provider.ExecuteNonQuery("UpdateSimpleTerm", term.TermId, term.VocabularyId, term.Name, term.Description, term.Weight, lastModifiedByUserId)
        End Sub

#End Region

#Region "Vocabulary Methods"

        Public Function AddVocabulary(ByVal vocabulary As Vocabulary, ByVal createdByUserId As Integer) As Integer Implements IDataService.AddVocabulary
            Return provider.ExecuteScalar(Of Integer)("AddVocabulary", vocabulary.Type, vocabulary.Name, vocabulary.Description, vocabulary.Weight, provider.GetNull(vocabulary.ScopeId), vocabulary.ScopeTypeId, createdByUserId)
        End Function

        Public Sub DeleteVocabulary(ByVal vocabulary As Vocabulary) Implements IDataService.DeleteVocabulary
            provider.ExecuteNonQuery("DeleteVocabulary", vocabulary.VocabularyId)
        End Sub

        Public Function GetVocabularies() As IDataReader Implements IDataService.GetVocabularies
            Return provider.ExecuteReader("GetVocabularies")
        End Function

        Public Sub UpdateVocabulary(ByVal vocabulary As Vocabulary, ByVal lastModifiedByUserId As Integer) Implements IDataService.UpdateVocabulary
            provider.ExecuteNonQuery("UpdateVocabulary", vocabulary.VocabularyId, vocabulary.Type, vocabulary.Name, vocabulary.Description, vocabulary.Weight, vocabulary.ScopeId, vocabulary.ScopeTypeId, lastModifiedByUserId)
        End Sub

#End Region

    End Class

End Namespace


