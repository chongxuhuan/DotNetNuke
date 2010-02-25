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
Imports System.IO
Imports System.Web
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports System.Web.Caching
Imports System.Text
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.ComponentModel

Namespace DotNetNuke.Services.ModuleCache

    Public Class MemoryProvider
        Inherits ModuleCachingProvider

#Region "Private Members"

        Private Const cachePrefix As String = "ModuleCache:"

#End Region

        Private Function GetCacheKeys(ByVal tabModuleId As Integer) As System.Collections.Generic.List(Of String)

            Dim keys As New List(Of String)
            Dim CacheEnum As IDictionaryEnumerator = CachingProvider.Instance().GetEnumerator
            While (CacheEnum.MoveNext)
                If CacheEnum.Key.ToString.StartsWith(String.Concat(cachePrefix, "|", tabModuleId.ToString, "|")) Then
                    keys.Add(CacheEnum.Key.ToString)
                End If
            End While
            Return keys

        End Function

#Region "Abstract Method Implementation"

        Public Overrides Function GenerateCacheKey(ByVal tabModuleId As Integer, ByVal varyBy As SortedDictionary(Of String, String)) As String
            Dim cacheKey As New Text.StringBuilder
            If varyBy IsNot Nothing Then
                For Each kvp As KeyValuePair(Of String, String) In varyBy
                    cacheKey.Append(String.Concat(kvp.Key.ToLower(), "=", kvp.Value, "|"))
                Next
            End If
            Return String.Concat(cachePrefix, "|", tabModuleId.ToString, "|", cacheKey.ToString())
        End Function

        Public Overrides Function GetItemCount(ByVal tabModuleId As Integer) As Integer
            Return GetCacheKeys(tabModuleId).Count()
        End Function

        Public Overrides Function GetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String) As Byte()
            Return DataCache.GetCache(Of Byte())(cacheKey)
        End Function

        Public Overrides Sub PurgeCache(ByVal portalId As Integer)
            DataCache.ClearCache(cachePrefix)
        End Sub

        Public Overrides Sub Remove(ByVal tabModuleId As Integer)
            DataCache.ClearCache(String.Concat(cachePrefix, "|", tabModuleId.ToString()))
        End Sub

        Public Overloads Overrides Sub SetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String, ByVal duration As TimeSpan, ByVal moduleOutput As Byte())
            Dim dep As DNNCacheDependency = Nothing
            DataCache.SetCache(cacheKey, moduleOutput, dep, Date.UtcNow.Add(duration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        Public Overrides Sub PurgeExpiredItems(ByVal portalId As Integer)
            Throw New NotSupportedException()
        End Sub

#End Region

    End Class

End Namespace
