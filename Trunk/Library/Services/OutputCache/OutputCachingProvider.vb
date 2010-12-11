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
Imports System.Text

Namespace DotNetNuke.Services.OutputCache

    Public MustInherit Class OutputCachingProvider

#Region "Protected Methods"

        Protected Function ByteArrayToString(ByVal arrInput() As Byte) As String
            Dim i As Integer
            Dim sOutput As New System.Text.StringBuilder(arrInput.Length)
            For i = 0 To arrInput.Length - 1
                sOutput.Append(arrInput(i).ToString("X2"))
            Next
            Return sOutput.ToString()
        End Function

        Protected Function GenerateCacheKeyHash(ByVal tabId As Integer, ByVal cacheKey As String) As String
            Dim hash As Byte() = Text.ASCIIEncoding.ASCII.GetBytes(cacheKey)
            Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider()
            hash = md5.ComputeHash(hash)
            Return String.Concat(tabId.ToString, "_", ByteArrayToString(hash))
        End Function

        Protected Sub WriteStreamAsText(ByVal context As HttpContext, ByVal stream As Stream, ByVal offset As Long, ByVal length As Long)
            If (length < 0) Then
                length = (stream.Length - offset)
            End If

            If (length > 0) Then
                If (offset > 0) Then
                    stream.Seek(offset, SeekOrigin.Begin)
                End If
                Dim buffer As Byte() = New Byte(CInt(length) - 1) {}
                Dim count As Integer = stream.Read(buffer, 0, CInt(length))
                Dim output As Char() = Encoding.Default.GetChars(buffer, 0, count)
                context.Response.ContentEncoding = Encoding.Default
                context.Response.Output.Write(output)
            End If

        End Sub



#End Region

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

        Public MustOverride Function GetItemCount(ByVal tabId As Integer) As Integer
        Public MustOverride Function GetOutput(ByVal tabId As Integer, ByVal cacheKey As String) As Byte()
        Public MustOverride Function GetResponseFilter(ByVal tabId As Integer, ByVal maxVaryByCount As Integer, ByVal responseFilter As Stream, ByVal cacheKey As String, ByVal cacheDuration As TimeSpan) As OutputCacheResponseFilter
        Public MustOverride Sub Remove(ByVal tabId As Integer)
        Public MustOverride Overloads Sub SetOutput(ByVal tabId As Integer, ByVal cacheKey As String, ByVal duration As TimeSpan, ByVal output As Byte())
        Public MustOverride Function StreamOutput(ByVal tabId As Integer, ByVal cacheKey As String, ByVal context As HttpContext) As Boolean

#End Region


#Region "Virtual Methods"

        Public Overridable Function GenerateCacheKey(ByVal tabId As Integer, ByVal includeVaryByKeys As System.Collections.Specialized.StringCollection, ByVal excludeVaryByKeys As System.Collections.Specialized.StringCollection, ByVal varyBy As SortedDictionary(Of String, String)) As String
            Dim cacheKey As New Text.StringBuilder
            If varyBy IsNot Nothing Then
                Dim varyByParms As SortedDictionary(Of String, String).Enumerator = varyBy.GetEnumerator()
                While (varyByParms.MoveNext)
                    Dim key As String = varyByParms.Current.Key.ToLower()
                    If includeVaryByKeys.Contains(key) And Not excludeVaryByKeys.Contains(key) Then
                        cacheKey.Append(String.Concat(key, "=", varyByParms.Current.Value, "|"))
                    End If
                End While
            End If
            Return GenerateCacheKeyHash(tabId, cacheKey.ToString())
        End Function

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
