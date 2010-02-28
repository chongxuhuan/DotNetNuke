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
Imports System.Web.Caching

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Entities.Content.Taxonomy
    Public Class TermController
        Implements ITermController

#Region "Private Members"

        Private _CacheKey As String = "Terms_{0}"
        Private _CachePriority As CacheItemPriority = CacheItemPriority.Normal
        Private _CacheTimeOut As Integer = 20

        Private _DataService As IDataService

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(Util.GetDataService())
        End Sub

        Public Sub New(ByVal dataService As IDataService)
            _DataService = dataService
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetTermsCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim vocabularyId As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillQueryable(Of Term)(_DataService.GetTermsByVocabulary(vocabularyId))
        End Function

#End Region

#Region "Public Methods"

        Public Function AddTerm(ByVal term As Term) As Integer Implements ITermController.AddTerm
            'Argument Contract
            Arg.NotNull("term", term)
            Arg.PropertyNotNegative("term", "VocabularyId", term.VocabularyId)
            Arg.PropertyNotNullOrEmpty("term", "Name", term.Name)

            If (term.IsHeirarchical) Then
                term.TermId = _DataService.AddHeirarchicalTerm(term, UserController.GetCurrentUserInfo.UserID)
            Else
                term.TermId = _DataService.AddSimpleTerm(term, UserController.GetCurrentUserInfo.UserID)
            End If

            'Clear Cache
            DataCache.RemoveCache(String.Format(_CacheKey, term.VocabularyId))

            Return term.TermId
        End Function

        Public Sub AddTermToContent(ByVal term As Term, ByVal contentItem As ContentItem) Implements ITermController.AddTermToContent
            'Argument Contract
            Arg.NotNull("term", term)
            Arg.NotNull("contentItem", contentItem)

            _DataService.AddTermToContent(term, contentItem)
        End Sub

        Public Sub DeleteTerm(ByVal term As Term) Implements ITermController.DeleteTerm
            'Argument Contract
            Arg.NotNull("term", term)
            Arg.PropertyNotNegative("term", "TermId", term.TermId)

            If (term.IsHeirarchical) Then
                _DataService.DeleteHeirarchicalTerm(term)
            Else
                _DataService.DeleteSimpleTerm(term)
            End If

            'Clear Cache
            DataCache.RemoveCache(String.Format(_CacheKey, term.VocabularyId))
        End Sub

        Public Function GetTerm(ByVal termId As Integer) As Term Implements ITermController.GetTerm
            'Argument Contract
            Arg.NotNegative("termId", termId)

            Return CBO.FillObject(Of Term)(_DataService.GetTerm(termId))
        End Function

        Public Function GetTermsByContent(ByVal contentItemId As Integer) As IQueryable(Of Term) Implements ITermController.GetTermsByContent
            'Argument Contract
            Arg.NotNegative("contentItemId", contentItemId)

            Return CBO.FillQueryable(Of Term)(_DataService.GetTermsByContent(contentItemId))
        End Function

        Public Function GetTermsByVocabulary(ByVal vocabularyId As Integer) As IQueryable(Of Term) Implements ITermController.GetTermsByVocabulary
            'Argument Contract
            Arg.NotNegative("vocabularyId", vocabularyId)

            Return CBO.GetCachedObject(Of IQueryable(Of Term)) _
                    (New CacheItemArgs(String.Format(_CacheKey, vocabularyId), _
                                       _CacheTimeOut, _CachePriority, vocabularyId), AddressOf GetTermsCallBack)
        End Function

        Public Sub RemoveTermsFromContent(ByVal contentItem As ContentItem) Implements ITermController.RemoveTermsFromContent
            'Argument Contract
            Arg.NotNull("contentItem", contentItem)

            _DataService.RemoveTermsFromContent(contentItem)
        End Sub

        Public Sub UpdateTerm(ByVal term As Term) Implements ITermController.UpdateTerm
            'Argument Contract
            Arg.NotNull("term", term)
            Arg.PropertyNotNegative("term", "TermId", term.TermId)
            Arg.PropertyNotNegative("term", "Vocabulary.VocabularyId", term.VocabularyId)
            Arg.PropertyNotNullOrEmpty("term", "Name", term.Name)

            If (term.IsHeirarchical) Then
                'Arg.PropertyNotNull("term", "ParentTermId", term.ParentTermId)
                _DataService.UpdateHeirarchicalTerm(term, UserController.GetCurrentUserInfo.UserID)
            Else
                _DataService.UpdateSimpleTerm(term, UserController.GetCurrentUserInfo.UserID)
            End If

            'Clear Cache
            DataCache.RemoveCache(String.Format(_CacheKey, term.VocabularyId))
        End Sub

#End Region

    End Class

End Namespace
