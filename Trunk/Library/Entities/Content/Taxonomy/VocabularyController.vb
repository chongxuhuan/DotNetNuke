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

Imports System.Collections.Generic

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Data
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Entities.Content.Taxonomy
    Public Class VocabularyController
        Implements IVocabularyController

#Region "Private Members"

        Private _CacheKey As String = "Vocabularies"
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

        Private Function GetVocabulariesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillQueryable(Of Vocabulary)(_DataService.GetVocabularies())
        End Function

#End Region

#Region "Public Methods"

        Public Function AddVocabulary(ByVal vocabulary As Vocabulary) As Integer Implements IVocabularyController.AddVocabulary
            'Argument Contract
            Arg.NotNull("vocabulary", vocabulary)
            Arg.PropertyNotNullOrEmpty("vocabulary", "Name", vocabulary.Name)
            Arg.PropertyNotNegative("vocabulary", "ScopeTypeId", vocabulary.ScopeTypeId)

            vocabulary.VocabularyId = _DataService.AddVocabulary(vocabulary, UserController.GetCurrentUserInfo.UserID)

            'Refresh Cache
            DataCache.RemoveCache(_CacheKey)

            Return vocabulary.VocabularyId
        End Function

        Public Sub ClearVocabularyCache() Implements IVocabularyController.ClearVocabularyCache
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Sub DeleteVocabulary(ByVal vocabulary As Vocabulary) Implements IVocabularyController.DeleteVocabulary
            'Argument Contract
            Arg.NotNull("vocabulary", vocabulary)
            Arg.PropertyNotNegative("vocabulary", "VocabularyId", vocabulary.VocabularyId)

            _DataService.DeleteVocabulary(vocabulary)

            'Refresh Cache
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Function GetVocabularies() As IQueryable(Of Vocabulary) Implements IVocabularyController.GetVocabularies
            Return CBO.GetCachedObject(Of IQueryable(Of Vocabulary)) _
                                (New CacheItemArgs(_CacheKey, _CacheTimeOut), AddressOf GetVocabulariesCallBack)
        End Function

        Public Sub UpdateVocabulary(ByVal vocabulary As Vocabulary) Implements IVocabularyController.UpdateVocabulary
            'Argument Contract
            Arg.NotNull("vocabulary", vocabulary)
            Arg.PropertyNotNegative("vocabulary", "VocabularyId", vocabulary.VocabularyId)
            Arg.PropertyNotNullOrEmpty("vocabulary", "Name", vocabulary.Name)

            'Refresh Cache
            DataCache.RemoveCache(_CacheKey)

            _DataService.UpdateVocabulary(vocabulary, UserController.GetCurrentUserInfo.UserID)
        End Sub

#End Region

    End Class

End Namespace
