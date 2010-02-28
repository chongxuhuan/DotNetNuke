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

Namespace DotNetNuke.Entities.Content.Taxonomy
    Public Class ScopeTypeController
        Implements IScopeTypeController

#Region "Private Members"

        Private _DataService As IDataService

        Private _CacheKey As String = "ScopeTypes"
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

        Private Function GetScopeTypesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillQueryable(Of ScopeType)(_DataService.GetScopeTypes())
        End Function

#End Region

#Region "Public Methods"

        Public Function AddScopeType(ByVal scopeType As ScopeType) As Integer Implements IScopeTypeController.AddScopeType
            'Argument Contract
            Arg.NotNull("scopeType", scopeType)
            Arg.PropertyNotNullOrEmpty("scopeType", "ScopeType", scopeType.ScopeType)

            scopeType.ScopeTypeId = _DataService.AddScopeType(scopeType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)

            Return scopeType.ScopeTypeId
        End Function

        Public Sub ClearScopeTypeCache() Implements IScopeTypeController.ClearScopeTypeCache
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Sub DeleteScopeType(ByVal scopeType As ScopeType) Implements IScopeTypeController.DeleteScopeType
            'Argument Contract
            Arg.NotNull("scopeType", scopeType)
            Arg.PropertyNotNegative("scopeType", "ScopeTypeId", scopeType.ScopeTypeId)

            _DataService.DeleteScopeType(scopeType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)
        End Sub

        Public Function GetScopeTypes() As IQueryable(Of ScopeType) Implements IScopeTypeController.GetScopeTypes
            Return CBO.GetCachedObject(Of IQueryable(Of ScopeType)) _
                                (New CacheItemArgs(_CacheKey, _CacheTimeOut), AddressOf GetScopeTypesCallBack)
        End Function

        Public Sub UpdateScopeType(ByVal scopeType As ScopeType) Implements IScopeTypeController.UpdateScopeType
            'Argument Contract
            Arg.NotNull("scopeType", scopeType)
            Arg.PropertyNotNegative("scopeType", "ScopeTypeId", scopeType.ScopeTypeId)
            Arg.PropertyNotNullOrEmpty("scopeType", "ScopeType", scopeType.ScopeType)

            _DataService.UpdateScopeType(scopeType)

            'Refresh cached collection of types
            DataCache.RemoveCache(_CacheKey)
        End Sub

#End Region

    End Class

End Namespace
