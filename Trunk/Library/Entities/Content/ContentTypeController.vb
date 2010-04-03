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

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Entities.Content.Data

Namespace DotNetNuke.Entities.Content
    Public Class ContentTypeController
        Implements IContentTypeController

#Region "Private Members"

        Private _DataService As IDataService

        Private _CacheKey As String = "ContentTypes"
        Private _CacheTimeOut As Integer = 20

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

        Private Function GetContentTypesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillQueryable(Of ContentType)(_DataService.GetContentTypes())
        End Function

#End Region

#Region "Public Methods"

        Public Function AddContentType(ByVal contentType As ContentType) As Integer Implements IContentTypeController.AddContentType
            'Argument Contract
            Requires.NotNull("contentType", contentType)
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.ContentType)

            ContentType.ContentTypeId = _DataService.AddContentType(ContentType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)

            Return ContentType.ContentTypeId
        End Function

        Public Sub ClearContentTypeCache() Implements IContentTypeController.ClearContentTypeCache
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Sub DeleteContentType(ByVal contentType As ContentType) Implements IContentTypeController.DeleteContentType
            'Argument Contract
            Requires.NotNull("contentType", contentType)
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId)

            _DataService.DeleteContentType(ContentType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Function GetContentTypes() As IQueryable(Of ContentType) Implements IContentTypeController.GetContentTypes
            Return CBO.GetCachedObject(Of IQueryable(Of ContentType)) _
                                (New CacheItemArgs(_CacheKey, _CacheTimeOut), AddressOf GetContentTypesCallBack)
        End Function

        Public Sub UpdateContentType(ByVal contentType As ContentType) Implements IContentTypeController.UpdateContentType
            'Argument Contract
            Requires.NotNull("contentType", contentType)
            Requires.PropertyNotNegative("contentType", "ContentTypeId", contentType.ContentTypeId)
            Requires.PropertyNotNullOrEmpty("contentType", "ContentType", contentType.ContentType)

            _DataService.UpdateContentType(ContentType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)
        End Sub

#End Region

    End Class

End Namespace
