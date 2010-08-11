'
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
'
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace DotNetNuke.Services.OutputCache

    Public MustInherit Class OutputCachingProvider

#Region "Shared/Static Methods"

        Public Shared Function GetProviderList() As Dictionary(Of String, OutputCachingProvider)
            Return ComponentModel.ComponentFactory.GetComponents(Of OutputCachingProvider)()
        End Function

        Public Shared Function Instance(ByVal FriendlyName As String) As OutputCachingProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of OutputCachingProvider)(FriendlyName)
        End Function

        Public Shared Sub RemoveItemFromAllProviders(ByVal tabId As Integer)
                For Each kvp As KeyValuePair(Of String, OutputCachingProvider) In GetProviderList()
                    kvp.Value.Remove(tabId)
                Next
        End Sub

#End Region

#Region "Abstract Methods"

        Public MustOverride Function GenerateCacheKey(ByVal tabId As Integer, ByVal includeVaryByKeys As System.Collections.Specialized.StringCollection, ByVal excludeVaryByKeys As System.Collections.Specialized.StringCollection, ByVal varyBy As SortedDictionary(Of String, String)) As String
        Public MustOverride Function GetItemCount(ByVal tabId As Integer) As Integer
        Public MustOverride Function GetOutput(ByVal tabId As Integer, ByVal cacheKey As String) As Byte()
        Public MustOverride Function GetResponseFilter(ByVal tabId As Integer, ByVal maxVaryByCount As Integer, ByVal responseFilter As Stream, ByVal cacheKey As String, ByVal cacheDuration As TimeSpan) As OutputCacheResponseFilter
        Public MustOverride Sub Remove(ByVal tabId As Integer)
        Public MustOverride Overloads Sub SetOutput(ByVal tabId As Integer, ByVal cacheKey As String, ByVal duration As TimeSpan, ByVal output As Byte())
        Public MustOverride Function StreamOutput(ByVal tabId As Integer, ByVal cacheKey As String, ByVal context As HttpContext) As Boolean

#End Region


#Region "Virtual Methods"

        Public Overridable Sub PurgeCache(ByVal portalId As Integer)
        End Sub

        Public Overridable Sub PurgeExpiredItems(ByVal portalId As Integer)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("This method was deprecated in 5.2.1")> _
        Public Overridable Sub PurgeCache()
        End Sub

        <Obsolete("This method was deprecated in 5.2.1")> _
        Public Overridable Sub PurgeExpiredItems()
        End Sub

#End Region

    End Class

End Namespace
