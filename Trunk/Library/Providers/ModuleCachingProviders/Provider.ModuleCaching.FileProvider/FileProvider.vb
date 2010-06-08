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
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports System.Web.Caching
Imports System.Text
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Services.Cache
Imports System.Globalization
Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.Services.ModuleCache

    Public Class FileProvider
        Inherits ModuleCachingProvider

#Region "Private Members"

        Private Const DataFileExtension As String = ".data.resources"
        Private Const AttribFileExtension As String = ".attrib.resources"

#End Region

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

#End Region

#Region "Private Methods"

        Private Function GenerateCacheKeyHash(ByVal tabModuleId As Integer, ByVal cacheKey As String) As String
            Dim hash As Byte() = Text.ASCIIEncoding.ASCII.GetBytes(cacheKey)
            Dim md5 As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider()
            hash = md5.ComputeHash(hash)
            Return tabModuleId.ToString & "_" & ByteArrayToString(hash)
        End Function

        Private Shared Function GetAttribFileName(ByVal tabModuleId As Integer, ByVal cacheKey As String) As String
            Return String.Concat(GetCacheFolder(), cacheKey, AttribFileExtension)
        End Function

        Private Shared Function GetCachedItemCount(ByVal tabModuleId As Integer) As Integer
            Return Directory.GetFiles(GetCacheFolder(), String.Format("*{0}", FileProvider.DataFileExtension)).Length
        End Function

        Private Shared Function GetCachedOutputFileName(ByVal tabModuleId As Integer, ByVal cacheKey As String) As String
            Return String.Concat(GetCacheFolder(), cacheKey, DataFileExtension)
        End Function

        Private Shared Function GetCacheFolder(ByVal portalId As Integer) As String
            Dim portalController As PortalController = New PortalController()
            Dim portalInfo As PortalInfo = portalController.GetPortal(portalId)

            Dim cacheFolder As String = String.Concat(portalInfo.HomeDirectoryMapPath(), "\Cache\Modules\")
            If Not Directory.Exists(cacheFolder) AndAlso Path.IsPathRooted(cacheFolder) AndAlso IsPathInApplication(cacheFolder) Then
                Directory.CreateDirectory(cacheFolder)
            End If
            Return cacheFolder
        End Function

        Private Shared Function GetCacheFolder() As String
            Dim portalId As Integer = PortalController.GetCurrentPortalSettings.PortalId
            Return GetCacheFolder(portalId)
        End Function

        Private Function IsFileExpired(ByVal file As String) As Boolean
            Dim oRead As System.IO.StreamReader = Nothing
            Try
                oRead = System.IO.File.OpenText(file)
                Dim expires As Date = Date.Parse(oRead.ReadLine(), CultureInfo.InvariantCulture)
                If expires < Date.UtcNow Then
                    Return True
                Else
                    Return False
                End If
            Finally
                If oRead IsNot Nothing Then
                    oRead.Close()
                End If
            End Try
        End Function

        Private Overloads Sub PurgeCache(ByVal folder As String)
            Dim filesNotDeleted As New System.Text.StringBuilder()
            Dim i As Integer = 0
            For Each File As String In Directory.GetFiles(folder, "*.resources")
                If Not DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFileWithWait(File, 100, 200) Then
                    filesNotDeleted.Append(String.Format("{0};", File))
                Else
                    i += 1
                End If
            Next
            If filesNotDeleted.Length > 0 Then
                Throw New IOException(String.Format("Deleted {0} files, however, some files are locked.  Could not delete the following files: {1}", i, filesNotDeleted))
            End If
        End Sub

        Private Shared Function IsPathInApplication(ByVal cacheFolder As String) As Boolean
            Return cacheFolder.Contains(DotNetNuke.Common.ApplicationMapPath)
        End Function

#End Region

#Region "Abstract Method Implementation"

        Public Overrides Function GenerateCacheKey(ByVal tabModuleId As Integer, ByVal varyBy As SortedDictionary(Of String, String)) As String
            Dim cacheKey As New Text.StringBuilder
            If varyBy IsNot Nothing Then
                Dim varyByParms As SortedDictionary(Of String, String).Enumerator = varyBy.GetEnumerator()
                While (varyByParms.MoveNext)
                    Dim key As String = varyByParms.Current.Key.ToLower()
                    cacheKey.Append(String.Concat(key, "=", varyByParms.Current.Value, "|"))
                End While
            End If
            Return GenerateCacheKeyHash(tabModuleId, cacheKey.ToString())
        End Function

        Public Overrides Function GetItemCount(ByVal tabModuleId As Integer) As Integer
            Return GetCachedItemCount(tabModuleId)
        End Function

        Public Overrides Function GetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String) As Byte()
            Dim cachedModule As String = GetCachedOutputFileName(tabModuleId, cacheKey)
            Dim br As BinaryReader = Nothing
            Dim fStream As FileStream = Nothing
            Dim data As Byte()
            Try
                If Not File.Exists(cachedModule) Then
                    Return Nothing
                End If
                Dim fInfo As New FileInfo(cachedModule)
                Dim numBytes As Long = fInfo.Length
                fStream = New FileStream(cachedModule, FileMode.Open, FileAccess.Read)
                br = New BinaryReader(fStream)
                data = br.ReadBytes(CInt(numBytes))
            Finally
                If br IsNot Nothing Then br.Close()
                If fStream IsNot Nothing Then fStream.Close()
            End Try

            Return data

        End Function

        Public Overrides Sub PurgeCache(ByVal portalId As Integer)
            PurgeCache(GetCacheFolder(portalId))
        End Sub

        Public Overrides Sub PurgeExpiredItems(ByVal portalId As Integer)
            Dim filesNotDeleted As New System.Text.StringBuilder()
            Dim i As Integer = 0

            Dim cacheFolder As String = GetCacheFolder(portalId)

            If Directory.Exists(cacheFolder) And IsPathInApplication(cacheFolder) Then
                For Each File As String In Directory.GetFiles(cacheFolder, String.Format("*{0}", FileProvider.AttribFileExtension))
                    If IsFileExpired(File) Then
                        Dim fileToDelete As String = File.Replace(FileProvider.AttribFileExtension, FileProvider.DataFileExtension)
                        If Not DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFileWithWait(fileToDelete, 100, 200) Then
                            filesNotDeleted.Append(String.Format("{0};", fileToDelete))
                        Else
                            i += 1
                        End If
                    End If
                Next
            End If
            If filesNotDeleted.Length > 0 Then
                Throw New IOException(String.Format("Deleted {0} files, however, some files are locked.  Could not delete the following files: {1}", i, filesNotDeleted))
            End If
        End Sub

        Public Overloads Overrides Sub SetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String, ByVal duration As TimeSpan, ByVal output As Byte())

            Dim attribFile As String = GetAttribFileName(tabModuleId, cacheKey)
            Dim cachedOutputFile As String = GetCachedOutputFileName(tabModuleId, cacheKey)

            Try
                If File.Exists(cachedOutputFile) Then
                    DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFileWithWait(cachedOutputFile, 100, 200)
                End If

                Dim captureStream As New FileStream(cachedOutputFile, FileMode.CreateNew, FileAccess.Write)
                captureStream.Write(output, 0, output.Length)
                captureStream.Close()

                Dim oWrite As System.IO.StreamWriter
                oWrite = File.CreateText(attribFile)
                oWrite.WriteLine(Date.UtcNow.Add(duration).ToString(CultureInfo.InvariantCulture))
                oWrite.Close()
            Catch ex As Exception
                ' TODO: Need to implement multi-threading.  
                ' The current code is not thread safe and threw error if two threads tried creating cache file
                ' A thread could create a file between the time another thread deleted it and tried to create new cache file.
                ' This would result in a system.IO.IOException.  Also, there was no error handling in place so the 
                ' Error would bubble up to the user and provide details on the file structure of the site.
                LogException(ex)
            End Try

        End Sub

        Public Overrides Sub Remove(ByVal tabModuleId As Integer)
            Dim filesNotDeleted As New System.Text.StringBuilder()
            Dim i As Integer = 0
            For Each File As String In Directory.GetFiles(GetCacheFolder(), tabModuleId.ToString & "_*.*")
                If Not DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFileWithWait(File, 100, 200) Then
                    filesNotDeleted.Append(File + ";")
                Else
                    i += 1
                End If
            Next
            If filesNotDeleted.Length > 0 Then
                Throw New IOException("Deleted " + i.ToString + " files, however, some files are locked.  Could not delete the following files: " + filesNotDeleted.ToString())
            End If
        End Sub

#End Region

    End Class

End Namespace
