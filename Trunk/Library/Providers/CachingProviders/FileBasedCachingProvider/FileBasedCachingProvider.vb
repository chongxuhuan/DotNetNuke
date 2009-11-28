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
Imports System.IO
Imports System.Web
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports System.Web.Caching
Imports System.Text
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.Cache.FileBasedCachingProvider

    Public Class FBCachingProvider
        Inherits CachingProvider

        Friend Shared CachingDirectory As String = "Cache\"
        Friend Const CacheFileExtension As String = ".resources"

#Region "Abstract Method Implementation"

        Public Overloads Overrides Sub Insert(ByVal Key As String, ByVal Value As Object, ByVal Dependency As DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)
            ' initialize cache dependency
            Dim d As DNNCacheDependency = Dependency

            ' if web farm is enabled
            If IsWebFarm Then
                ' get hashed file name
                Dim f(0) As String
                f(0) = GetFileName(Key)
                ' create a cache file for item
                CreateCacheFile(f(0), Key)
                ' create a cache dependency on the cache file
                d = New DNNCacheDependency(f, Nothing, Dependency)
            End If

            'Call base class method to add obect to cache
            MyBase.Insert(Key, Value, d, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
        End Sub

        Public Overrides Function IsWebFarm() As Boolean
            Dim _IsWebFarm As Boolean = Null.NullBoolean
            If Not String.IsNullOrEmpty(Config.GetSetting("IsWebFarm")) Then
                _IsWebFarm = Boolean.Parse(Config.GetSetting("IsWebFarm"))
            End If
            Return _IsWebFarm
        End Function

        Public Overrides Function PurgeCache() As String
            ' called by scheduled job to remove cache files which are no longer active
            Return PurgeCacheFiles(HostMapPath + CachingDirectory)
        End Function

        Public Overrides Sub Remove(ByVal Key As String)
            'Call base class method to remove obect from cache
            MyBase.Remove(Key)

            ' if web farm is enabled in config file
            If IsWebFarm() Then
                ' get hashed filename
                Dim f As String = GetFileName(Key)
                ' delete cache file - this synchronizes the cache across servers in the farm
                DeleteCacheFile(f)
            End If
        End Sub

#End Region

#Region "Private Methods"

        Private Shared Function ByteArrayToString(ByVal arrInput() As Byte) As String
            Dim i As Integer
            Dim sOutput As New StringBuilder(arrInput.Length)
            For i = 0 To arrInput.Length - 1
                sOutput.Append(arrInput(i).ToString("X2"))
            Next
            Return sOutput.ToString()
        End Function

        Private Shared Sub CreateCacheFile(ByVal FileName As String, ByVal CacheKey As String)
            ' declare stream
            Dim s As StreamWriter = Nothing
            Try
                ' if the cache file does not already exist
                If Not File.Exists(FileName) Then
                    ' create the cache file
                    s = File.CreateText(FileName)
                    ' write the CacheKey to the file to provide a documented link between cache item and cache file
                    s.Write(CacheKey)
                    ' close the stream
                End If
            Catch ex As Exception
                ' permissions issue creating cache file or more than one thread may have been trying to write the cache file simultaneously
                LogException(ex)
            Finally
                If Not s Is Nothing Then
                    s.Close()
                End If
            End Try
        End Sub

        Private Shared Sub DeleteCacheFile(ByVal FileName As String)
            Try
                If File.Exists(FileName) Then
                    File.Delete(FileName)
                End If
            Catch ex As Exception
                ' an error occurred when trying to delete the cache file - this is serious as it means that the cache will not be synchronized
                LogException(ex)
            End Try
        End Sub

        Private Shared Function GetFileName(ByVal CacheKey As String) As String
            ' cache key may contain characters invalid for a filename - this method creates a valid filename
            Dim FileNameBytes As Byte() = Text.ASCIIEncoding.ASCII.GetBytes(CacheKey)
            Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider()
            FileNameBytes = md5.ComputeHash(FileNameBytes)
            Dim FinalFileName As String = ByteArrayToString(FileNameBytes)
            Return Path.GetFullPath(HostMapPath + CachingDirectory + FinalFileName + CacheFileExtension)
        End Function

        Private Function PurgeCacheFiles(ByVal Folder As String) As String
            ' declare counters
            Dim PurgedFiles As Integer = 0
            Dim PurgeErrors As Integer = 0
            Dim i As Integer

            ' get list of cache files
            Dim f() As String
            f = Directory.GetFiles(Folder)

            ' loop through cache files
            For i = 0 To f.Length - 1
                ' get last write time for file
                Dim dtLastWrite As Date
                dtLastWrite = File.GetLastWriteTime(f(i))
                ' if the cache file is more than 2 hours old ( no point in checking most recent cache files )
                If dtLastWrite < Now.Subtract(New TimeSpan(2, 0, 0)) Then
                    ' get cachekey
                    Dim strCacheKey As String = Path.GetFileNameWithoutExtension(f(i))
                    ' if the cache key does not exist in memory
                    If DataCache.GetCache(strCacheKey) Is Nothing Then
                        Try
                            ' delete the file
                            File.Delete(f(i))
                            PurgedFiles += 1
                        Catch ex As Exception
                            ' an error occurred
                            PurgeErrors += 1
                        End Try
                    End If
                End If
            Next

            ' return a summary message for the job
            Return String.Format("Cache Synchronization Files Processed: " + f.Length.ToString + ", Purged: " + PurgedFiles.ToString + ", Errors: " + PurgeErrors.ToString)
        End Function

#End Region

    End Class

End Namespace
