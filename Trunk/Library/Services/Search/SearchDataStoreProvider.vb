'
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
'

Imports System
Imports DotNetNuke

Namespace DotNetNuke.Services.Search
    Public MustInherit Class SearchDataStoreProvider

        'Protected Overridable ReadOnly Property SupportsExtendedSearchMethods() As Boolean
        '    Get
        '        Return False
        '    End Get
        'End Property

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As SearchDataStoreProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of SearchDataStoreProvider)()
        End Function

#End Region

#Region "Abstract Methods"

        Public MustOverride Sub StoreSearchItems(ByVal SearchItems As SearchItemInfoCollection)
        Public MustOverride Function GetSearchResults(ByVal PortalID As Integer, ByVal Criteria As String) As SearchResultsInfoCollection
        Public MustOverride Function GetSearchItems(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal ModuleID As Integer) As SearchResultsInfoCollection

#End Region

        'Public Overridable Sub AddSearchItem(ByVal SearchItem As SearchItemInfo)
        'End Sub

        'Public Overridable Sub AddSearchItems(ByVal SearchItems As SearchItemInfoCollection)
        'End Sub

        'Public Overridable Sub DeleteSearchItem(ByVal SearchItem As SearchItemInfo)
        'End Sub

        'Public Overridable Sub DeleteSearchItems(ByVal SearchItems As SearchItemInfoCollection)
        'End Sub

        'Public Overridable Sub UpdateSearchItem(ByVal SearchItem As SearchItemInfo)
        'End Sub

        'Public Overridable Sub UpdateSearchItems(ByVal SearchItems As SearchItemInfoCollection)
        'End Sub

    End Class

End Namespace