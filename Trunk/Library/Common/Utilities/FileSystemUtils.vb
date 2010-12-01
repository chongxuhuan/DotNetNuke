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

Imports System.IO
Imports System.Net
Imports System.Collections.Generic
Imports System.Web.Caching
Imports System.Security.Cryptography
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports ICSharpCode.SharpZipLib.Checksums
Imports ICSharpCode.SharpZipLib.Zip
Imports DotNetNuke.Entities.Host
Imports System.Text.RegularExpressions

Namespace DotNetNuke.Common.Utilities

    Public Class FileSystemUtils

#Region "Enums"
        Private Enum EnumUserFolderElement
            Root = 0
            SubFolder = 1
        End Enum
#End Region

#Region "Private Methods"

        Private Shared Function AddFile(ByVal PortalId As Integer, ByVal inStream As Stream, ByVal fileName As String, ByVal contentType As String, ByVal length As Long, ByVal folderName As String, ByVal closeInputStream As Boolean, ByVal clearCache As Boolean) As String
            Return AddFile(PortalId, inStream, fileName, contentType, length, folderName, closeInputStream, clearCache, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a File
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="inStream">The stream to add</param>
        ''' <param name="contentType">The type of the content</param>
        ''' <param name="length">The length of the content</param>
        ''' <param name="folderName">The name of the folder</param>
        ''' <param name="closeInputStream">A flag that dermines if the Input Stream should be closed.</param>
        ''' <param name="ClearCache">A flag that indicates whether the file cache should be cleared</param>
        ''' <remarks>This method adds a new file
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    04/26/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function AddFile(ByVal PortalId As Integer, ByVal inStream As Stream, ByVal fileName As String, ByVal contentType As String, ByVal length As Long, ByVal folderName As String, ByVal closeInputStream As Boolean, ByVal clearCache As Boolean, ByVal synchronize As Boolean) As String

            Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
            Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
            Dim sourceFolderName As String = GetSubFolderPath(fileName, PortalId)
            Dim folder As FolderInfo = objFolderController.GetFolder(PortalId, sourceFolderName, False)
            Dim sourceFileName As String = GetFileName(fileName)
            Dim intFileID As Integer
            Dim retValue As String = ""
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo

            retValue += CheckValidFileName(fileName)
            If retValue.Length > 0 Then
                Return retValue
            End If

            Dim extension As String = Path.GetExtension(fileName).Replace(".", "")
            If contentType = "" Then
                contentType = GetContentType(extension)
            End If

            objFile = New DotNetNuke.Services.FileSystem.FileInfo(PortalId, sourceFileName, extension, CType(length, Integer), Null.NullInteger, Null.NullInteger, contentType, folderName, folder.FolderID, folder.StorageLocation, clearCache)

            'Calculate hash/save file to db/save file to filesys
            Select Case folder.StorageLocation
                Case FolderController.StorageLocationTypes.InsecureFileSystem, FolderController.StorageLocationTypes.SecureFileSystem
                    'Save file to File Storage
                    If folder.StorageLocation <> FolderController.StorageLocationTypes.InsecureFileSystem Or synchronize = False Then
                        WriteStream(intFileID, inStream, fileName, folder.StorageLocation, closeInputStream)
                    End If

                    If folder.StorageLocation = FolderController.StorageLocationTypes.InsecureFileSystem AndAlso File.Exists(fileName) Then
                        objFile.SHA1Hash = GetHash(File.ReadAllBytes(fileName))
                    ElseIf folder.StorageLocation = FolderController.StorageLocationTypes.SecureFileSystem AndAlso File.Exists(fileName + glbProtectedExtension) Then
                        objFile.SHA1Hash = GetHash(File.ReadAllBytes(fileName + glbProtectedExtension))
                    End If
                    'Add file to Database
                    intFileID = objFileController.AddFile(objFile)

                Case FolderController.StorageLocationTypes.DatabaseSecure
                    Dim bytes As Byte() = New Byte(Convert.ToInt32(inStream.Length)) {}
                    inStream.Read(bytes, 0, Convert.ToInt32(inStream.Length))
                    inStream.Position = 0
                    objFile.SHA1Hash = GetHash(bytes)
                    'Add file to Database
                    intFileID = objFileController.AddFile(objFile)
                    'Save file to File Storage
                    If synchronize = False Then WriteStream(intFileID, inStream, fileName, folder.StorageLocation, closeInputStream)
            End Select

            'Update the FileData
            retValue += UpdateFileData(intFileID, folder.FolderID, PortalId, sourceFileName, extension, contentType, length, folderName, objFile.StorageLocation, objFile.IsCached, objFile.SHA1Hash)

            If folder.StorageLocation <> FolderController.StorageLocationTypes.InsecureFileSystem Then
                'try and delete the Insecure file
                DeleteFile(fileName)
            End If

            If folder.StorageLocation <> FolderController.StorageLocationTypes.SecureFileSystem Then
                'try and delete the Secure file
                If Not fileName.EndsWith(".template") Then
                    DeleteFile(fileName & glbProtectedExtension)
                End If
            End If

            Return retValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Folder
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="relativePath">The relative path of the folder</param>
        ''' <param name="StorageLocation">The type of storage location</param>
        ''' <history>
        '''     [cnurse]    04/26/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function AddFolder(ByVal PortalId As Integer, ByVal relativePath As String, ByVal StorageLocation As Integer) As Integer

            Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
            Dim isProtected As Boolean = FileSystemUtils.DefaultProtectedFolders(relativePath)

            Dim objFolder As FolderInfo = New FolderInfo(PortalId, relativePath, StorageLocation, isProtected, False, Null.NullDate)
            Dim FolderID As Integer = objFolderController.AddFolder(objFolder)

            If PortalId <> Null.NullInteger Then
                'Set Folder Permissions to inherit from parent
                SetFolderPermissions(PortalId, FolderID, relativePath)
            End If

            Return FolderID

        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Folder with a specificed unique identifier
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="relativePath">The relative path of the folder</param>
        ''' <param name="StorageLocation">The type of storage location</param>
        ''' <param name="uniqueId">The unique identifier for the folder</param>
        ''' <history>
        '''     [vnguyen]    06/04/2010  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function AddFolder(ByVal PortalId As Integer, ByVal relativePath As String, ByVal StorageLocation As Integer, ByVal uniqueId As Guid) As Integer

            Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
            Dim isProtected As Boolean = FileSystemUtils.DefaultProtectedFolders(relativePath)

            Dim objFolder As FolderInfo = New FolderInfo(uniqueId, PortalId, relativePath, StorageLocation, isProtected, False, Null.NullDate)
            Dim FolderID As Integer = objFolderController.AddFolder(objFolder)

            If PortalId <> Null.NullInteger Then
                'Set Folder Permissions to inherit from parent
                SetFolderPermissions(PortalId, FolderID, relativePath)
            End If

            Return FolderID

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Checks that the file name is valid
        ''' </summary>
        ''' <param name="strFileName">The name of the file</param>
        ''' <history>
        '''     [cnurse]    04/26/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CheckValidFileName(ByVal strFileName As String) As String
            Dim retValue As String = Null.NullString

            ' check if file extension is restricted
            Dim strExtension As String = Path.GetExtension(strFileName).Replace(".", "")
            Dim strWhiteList As String = DotNetNuke.Entities.Host.Host.FileExtensions.ToLowerInvariant
            If String.IsNullOrEmpty(strExtension) OrElse ("," & strWhiteList & ",").IndexOf("," & strExtension.ToLowerInvariant & ",") = -1 Then
                If Not HttpContext.Current Is Nothing Then
                    retValue = "<br>" & String.Format(Localization.GetString("RestrictedFileType"), strFileName, Replace(strWhiteList, ",", ", *."))
                Else
                    retValue = "RestrictedFileType"
                End If
            End If

            'Return
            Return retValue
        End Function

        Private Shared Function CreateFile(ByVal RootPath As String, ByVal FileName As String, ByVal ContentLength As Long, ByVal ContentType As String, ByVal InputStream As Stream, ByVal NewFileName As String, Optional ByVal Unzip As Boolean = False) As String
            ' Obtain PortalSettings from Current Context
            Dim settings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim PortalId As Integer = GetFolderPortalId(settings)
            Dim isHost As Boolean = CType(IIf(settings.ActiveTab.ParentId = settings.SuperTabId, True, False), Boolean)

            Dim objPortalController As New PortalController
            Dim strMessage As String = ""
            Dim strFileName As String = FileName
            If NewFileName <> Null.NullString Then strFileName = NewFileName
            strFileName = RootPath & Path.GetFileName(strFileName)
            Dim strExtension As String = Replace(Path.GetExtension(strFileName), ".", "")
            Dim strFolderpath As String = GetSubFolderPath(strFileName, PortalId)

            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, strFolderpath, False)

            If FolderPermissionController.CanAddFolder(objFolder) Then
                If objPortalController.HasSpaceAvailable(PortalId, ContentLength) Then
                    If InStr(1, "," & Host.FileExtensions.ToUpper, "," & strExtension.ToUpper) <> 0 Or isHost Then
                        'Save Uploaded file to server
                        Try
                            strMessage += AddFile(PortalId, InputStream, strFileName, ContentType, ContentLength, strFolderpath, True, True)

                            'Optionally Unzip File?
                            If Path.GetExtension(strFileName).ToLower = ".zip" And Unzip = True Then
                                strMessage += UnzipFile(strFileName, RootPath, settings)
                            End If
                        Catch Exc As Exception
                            ' save error - can happen if the security settings are incorrect on the disk
                            strMessage += "<br>" & String.Format(Localization.GetString("SaveFileError"), strFileName)
                        End Try
                    Else
                        ' restricted file type
                        strMessage += "<br>" & String.Format(Localization.GetString("RestrictedFileType"), strFileName, Replace(Host.FileExtensions, ",", ", *."))
                    End If
                Else    ' file too large
                    strMessage += "<br>" & String.Format(Localization.GetString("DiskSpaceExceeded"), strFileName)
                End If
            Else ' insufficient folder permission in the application
                strMessage += "<br>" & String.Format(Localization.GetString("InsufficientFolderPermission"), strFolderpath)
            End If

            Return strMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the filename for a file path
        ''' </summary>
        ''' <param name="filePath">The full name of the file</param>
        ''' <history>
        '''     [cnurse]    04/26/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetFileName(ByVal filePath As String) As String
            Return System.IO.Path.GetFileName(filePath).Replace(glbProtectedExtension, "")
        End Function

        Private Shared Sub RemoveOrphanedFiles(ByVal folder As FolderInfo, ByVal PortalId As Integer)
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo
            Dim objFileController As New FileController

            If folder.StorageLocation <> FolderController.StorageLocationTypes.DatabaseSecure Then
                For Each objFile In GetFilesByFolder(PortalId, folder.FolderID)
                    RemoveOrphanedFile(objFile, PortalId)
                Next
            End If
        End Sub

        Private Shared Sub RemoveOrphanedFile(ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo, ByVal PortalId As Integer)
            Dim objFileController As New FileController

            Dim strFile As String = ""
            Select Case objFile.StorageLocation
                Case FolderController.StorageLocationTypes.InsecureFileSystem
                    strFile = objFile.PhysicalPath
                Case FolderController.StorageLocationTypes.SecureFileSystem
                    strFile = objFile.PhysicalPath & glbProtectedExtension
            End Select

            If strFile <> "" Then
                If Not File.Exists(strFile) Then
                    objFileController.DeleteFile(PortalId, objFile.FileName, objFile.FolderId, True)
                End If
            End If
        End Sub

        Private Shared Function ShouldSyncFolder(ByVal hideSystemFolders As Boolean, ByVal dirinfo As DirectoryInfo) As Boolean
            If hideSystemFolders AndAlso _
                (((dirinfo.Attributes And FileAttributes.Hidden) = FileAttributes.Hidden) OrElse _
                dirinfo.Name.StartsWith("_", StringComparison.CurrentCultureIgnoreCase)) Then

                Return False
            End If

            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates a File
        ''' </summary>
        ''' <param name="strSourceFile">The original File Name</param>
        ''' <param name="strDestFile">The new File Name</param>
        ''' <param name="isCopy">Flag determines whether file is to be be moved or copied</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/2/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function UpdateFile(ByVal strSourceFile As String, ByVal strDestFile As String, ByVal PortalId As Integer, ByVal isCopy As Boolean, ByVal isNew As Boolean, ByVal ClearCache As Boolean) As String
            Dim retValue As String = ""
            retValue += CheckValidFileName(strSourceFile) & " "
            retValue += CheckValidFileName(strDestFile)
            If retValue.Length > 1 Then
                Return retValue
            End If
            retValue = ""

            Dim sourceStream As Stream = Nothing

            Try
                Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
                Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
                Dim sourceFolderName As String = GetSubFolderPath(strSourceFile, PortalId)
                Dim sourceFileName As String = GetFileName(strSourceFile)
                Dim sourceFolder As FolderInfo = objFolderController.GetFolder(PortalId, sourceFolderName, False)

                Dim destFileName As String = GetFileName(strDestFile)
                Dim destFolderName As String = GetSubFolderPath(strDestFile, PortalId)

                Dim sourceFile As DotNetNuke.Services.FileSystem.FileInfo
                Dim file As DotNetNuke.Services.FileSystem.FileInfo

                If Not sourceFolder Is Nothing Then
                    sourceFile = objFileController.GetFile(sourceFileName, PortalId, sourceFolder.FolderID)
                    file = objFileController.GetFile(sourceFileName, PortalId, sourceFolder.FolderID)

                    If Not file Is Nothing Then
                        'Get the source Content from wherever it is
                        sourceStream = CType(GetFileStream(file), Stream)

                        If isCopy Then
                            'Add the new File
                            AddFile(PortalId, sourceStream, strDestFile, "", file.Size, destFolderName, True, ClearCache)
                        Else
                            'Move/Update existing file
                            Dim destinationFolder As FolderInfo = objFolderController.GetFolder(PortalId, destFolderName, False)

                            'Now move the file
                            If Not destinationFolder Is Nothing Then
                                file.FileName = destFileName
                                file.Folder = destFolderName
                                file.FolderId = destinationFolder.FolderID

                                'Write the content to the Destination
                                WriteStream(file.FileId, sourceStream, strDestFile, destinationFolder.StorageLocation, True)

                                If destinationFolder.StorageLocation <> FolderController.StorageLocationTypes.DatabaseSecure Then
                                    Select Case file.StorageLocation
                                        Case FolderController.StorageLocationTypes.DatabaseSecure
                                            If objFileController.GetFileById(file.FileId, file.PortalId) IsNot Nothing Then file.SHA1Hash = objFileController.GetFileById(file.FileId, file.PortalId).SHA1Hash
                                        Case FolderController.StorageLocationTypes.SecureFileSystem
                                            file.SHA1Hash = GetHash(System.IO.File.ReadAllBytes(file.PhysicalPath + glbProtectedExtension))
                                        Case FolderController.StorageLocationTypes.InsecureFileSystem
                                            file.SHA1Hash = GetHash(System.IO.File.ReadAllBytes(file.PhysicalPath))
                                    End Select
                                Else
                                    sourceStream = CType(GetFileStream(sourceFile), Stream)
                                    Dim bytes As Byte() = New Byte(Convert.ToInt32(sourceStream.Length)) {}
                                    sourceStream.Read(bytes, 0, Convert.ToInt32(sourceStream.Length))
                                    sourceStream.Close()
                                    sourceStream.Dispose()
                                    file.SHA1Hash = GetHash(bytes)
                                End If

                                'Update the File
                                file.VersionGuid = Guid.NewGuid()
                                objFileController.UpdateFile(file)
                                If sourceFolder.StorageLocation = FolderController.StorageLocationTypes.DatabaseSecure AndAlso destinationFolder.StorageLocation <> FolderController.StorageLocationTypes.DatabaseSecure Then
                                    objFileController.UpdateFileContent(file.FileId, Stream.Null)
                                End If

                                'Now we need to clean up the original files
                                If sourceFolder.StorageLocation = FolderController.StorageLocationTypes.InsecureFileSystem Then
                                    'try and delete the Insecure file
                                    DeleteFile(strSourceFile)
                                End If
                                If sourceFolder.StorageLocation = FolderController.StorageLocationTypes.SecureFileSystem Then
                                    'try and delete the Secure file
                                    DeleteFile(strSourceFile & glbProtectedExtension)
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                retValue = ex.Message
            Finally
                If (Not IsNothing(sourceStream)) Then
                    sourceStream.Close()
                    sourceStream.Dispose()
                End If
            End Try

            Return retValue

        End Function

        Private Shared Function UpdateFileData(ByVal fileID As Integer, ByVal folderID As Integer, ByVal PortalId As Integer, ByVal fileName As String, ByVal extension As String, ByVal contentType As String, ByVal length As Long, ByVal folderName As String, ByVal storageLocation As Integer, ByVal isCached As Boolean, ByVal hash As String) As String
            Dim retvalue As String = ""

            Try
                Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
                Dim objFile As New DotNetNuke.Services.FileSystem.FileInfo(PortalId, fileName, extension, CType(length, Integer), Null.NullInteger, Null.NullInteger, contentType, folderName, folderID, storageLocation, isCached, hash)
                Dim file As New DotNetNuke.Services.FileSystem.FileInfo

                If Convert.ToBoolean(InStr(1, glbImageFileTypes & ",", extension.ToLower & ",")) Then
                    Dim imgImage As System.Drawing.Image = Nothing
                    Dim imageStream As Stream = Nothing
                    Try
                        file = objFileController.GetFileById(fileID, PortalId)
                        objFile = file
                        If System.IO.File.Exists(objFile.PhysicalPath) Then objFile.SHA1Hash = GetHash(System.IO.File.ReadAllBytes(objFile.PhysicalPath))
                        imageStream = GetFileStream(file)
                        imgImage = Drawing.Image.FromStream(imageStream)
                        objFile.Height = imgImage.Height
                        objFile.Width = imgImage.Width
                    Catch
                        ' error loading image file
                        objFile.ContentType = "application/octet-stream"
                    Finally
                        If (Not IsNothing(imgImage)) Then
                            imgImage.Dispose()
                        End If
                        If (Not IsNothing(imageStream)) Then
                            imageStream.Close()
                            imageStream.Dispose()
                        End If
                        'Update the File info
                        objFileController.UpdateFile(objFile)
                    End Try
                Else
                    Try
                        file = objFileController.GetFileById(fileID, PortalId)
                        objFile = file

                        'Calcuate hash value
                        Select Case file.StorageLocation
                            Case FolderController.StorageLocationTypes.InsecureFileSystem
                                If System.IO.File.Exists(objFile.PhysicalPath) Then objFile.SHA1Hash = GetHash(System.IO.File.ReadAllBytes(objFile.PhysicalPath))
                            Case FolderController.StorageLocationTypes.SecureFileSystem
                                If System.IO.File.Exists(objFile.PhysicalPath + glbProtectedExtension) Then objFile.SHA1Hash = GetHash(System.IO.File.ReadAllBytes(objFile.PhysicalPath + glbProtectedExtension))
                            Case FolderController.StorageLocationTypes.DatabaseSecure
                                'Do nothing -- uneccessary to calc hash for non physical file.
                        End Select

                    Catch
                        ' error loading image file
                        objFile.ContentType = "application/octet-stream"
                    Finally
                        'Update the File info
                        objFileController.UpdateFile(objFile)
                    End Try
                End If

            Catch ex As Exception
                retvalue = ex.Message
            End Try

            Return retvalue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a Stream to the appropriate File Storage
        ''' </summary>
        ''' <param name="fileId">The Id of the File</param>
        ''' <param name="inStream">The Input Stream</param>
        ''' <param name="fileName">The name of the file</param>
        ''' <param name="StorageLocation">The type of storage location</param>
        ''' <param name="closeInputStream">A flag that dermines if the Input Stream should be closed.</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/27/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub WriteStream(ByVal fileId As Integer, ByVal inStream As Stream, ByVal fileName As String, ByVal storageLocation As Integer, ByVal closeInputStream As Boolean)

            Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
            Dim arrData(2048) As Byte
            Dim outStream As Stream = Nothing

            ' Buffer to read 2K bytes in chunk:
            Try
                Select Case storageLocation
                    Case FolderController.StorageLocationTypes.DatabaseSecure
                        objFileController.ClearFileContent(fileId)
                        outStream = New MemoryStream
                    Case FolderController.StorageLocationTypes.SecureFileSystem
                        If File.Exists(fileName & glbProtectedExtension) = True Then
                            File.Delete(fileName & glbProtectedExtension)
                        End If
                        outStream = New FileStream(fileName & glbProtectedExtension, FileMode.Create)
                    Case FolderController.StorageLocationTypes.InsecureFileSystem
                        If File.Exists(fileName) = True Then
                            File.Delete(fileName)
                        End If
                        outStream = New FileStream(fileName, FileMode.Create)
                End Select
            Catch ex As Exception
                If IsNothing(inStream) = False And closeInputStream Then
                    ' Close the file.
                    inStream.Close()
                    inStream.Dispose()
                End If
                If IsNothing(outStream) = False Then
                    ' Close the file.
                    outStream.Close()
                    outStream.Dispose()
                End If

                Throw ex
            End Try

            Try
                ' Total bytes to read:
                Dim intLength As Integer
                ' Read the data in buffer
                intLength = inStream.Read(arrData, 0, arrData.Length)
                While intLength > 0
                    ' Write the data to the current output stream.
                    outStream.Write(arrData, 0, intLength)

                    'Read the next chunk
                    intLength = inStream.Read(arrData, 0, arrData.Length)
                End While

                If storageLocation = FolderController.StorageLocationTypes.DatabaseSecure Then
                    outStream.Seek(0, SeekOrigin.Begin)
                    objFileController.UpdateFileContent(fileId, outStream)
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                If IsNothing(inStream) = False And closeInputStream Then
                    ' Close the file.
                    inStream.Close()
                    inStream.Dispose()
                End If
                If IsNothing(outStream) = False Then
                    ' Close the file.
                    outStream.Close()
                    outStream.Dispose()
                End If
            End Try

        End Sub

        Private Shared Sub WriteStream(ByVal objResponse As HttpResponse, ByVal objStream As Stream)
            ' Buffer to read 10K bytes in chunk:
            Dim bytBuffer(10000) As Byte

            ' Length of the file:
            Dim intLength As Integer

            ' Total bytes to read:
            Dim lngDataToRead As Long

            Try
                ' Total bytes to read:
                lngDataToRead = objStream.Length

                'objResponse.ContentType = "application/octet-stream"

                ' Read the bytes.
                While lngDataToRead > 0
                    ' Verify that the client is connected.
                    If objResponse.IsClientConnected Then
                        ' Read the data in buffer
                        intLength = objStream.Read(bytBuffer, 0, 10000)

                        ' Write the data to the current output stream.
                        objResponse.OutputStream.Write(bytBuffer, 0, intLength)

                        ' Flush the data to the HTML output.
                        objResponse.Flush()

                        ReDim bytBuffer(10000)       ' Clear the buffer
                        lngDataToRead = lngDataToRead - intLength
                    Else
                        'prevent infinite loop if user disconnects
                        lngDataToRead = -1
                    End If
                End While

            Catch ex As Exception
                ' Trap the error, if any.
                objResponse.Write("Error : " & ex.Message)
            Finally
                If IsNothing(objStream) = False Then
                    ' Close the file.
                    objStream.Close()
                    objStream.Dispose()
                End If
            End Try
        End Sub

#End Region

#Region "Path Manipulation Methods"

        Public Shared Function AddTrailingSlash(ByVal strSource As String) As String
            If Not strSource.EndsWith("\") Then strSource = strSource & "\"
            Return strSource
        End Function

        Public Shared Function RemoveTrailingSlash(ByVal strSource As String) As String
            If strSource = "" Then Return ""
            If Mid(strSource, Len(strSource), 1) = "\" Or Mid(strSource, Len(strSource), 1) = "/" Then
                Return strSource.Substring(0, Len(strSource) - 1)
            Else
                Return strSource
            End If
        End Function

        Public Shared Function StripFolderPath(ByVal strOrigPath As String) As String
            If strOrigPath.IndexOf("\") <> -1 Then
                Return Regex.Replace(strOrigPath, "^0\\", "")
            Else
                Return Replace(strOrigPath, "0", "", 1, 1)
            End If
        End Function

        Public Shared Function FormatFolderPath(ByVal strFolderPath As String) As String
            If strFolderPath = "" Then
                Return ""
            Else
                If strFolderPath.EndsWith("/") Then
                    Return strFolderPath
                Else
                    Return strFolderPath & "/"
                End If
            End If
        End Function

        ''' <summary>
        ''' The MapPath method maps the specified relative or virtual path to the corresponding physical directory on the server.
        ''' </summary>
        ''' <param name="path">Specifies the relative or virtual path to map to a physical directory. If Path starts with either 
        ''' a forward (/) or backward slash (\), the MapPath method returns a path as if Path were a full, virtual path. If Path 
        ''' doesn't start with a slash, the MapPath method returns a path relative to the directory of the .asp file being processed</param>
        ''' <returns></returns>
        ''' <remarks>If path is a null reference (Nothing in Visual Basic), then the MapPath method returns the full physical path 
        ''' of the directory that contains the current application</remarks>
        Public Shared Function MapPath(ByVal path As String) As String
            Dim convertedPath As String = path
            If ApplicationPath.Length > 1 AndAlso convertedPath.StartsWith(ApplicationPath) Then
                convertedPath = convertedPath.Substring(ApplicationPath.Length)
            End If
            convertedPath = convertedPath.Replace("/", "\")

            If path.StartsWith("~") Or path.StartsWith(".") Or path.StartsWith("/") Then
                Dim appMapPath As String = DotNetNuke.Common.Globals.ApplicationMapPath
                If convertedPath.Length > 1 Then
                    convertedPath = String.Concat(AddTrailingSlash(DotNetNuke.Common.Globals.ApplicationMapPath), convertedPath.Substring(1))
                Else
                    convertedPath = DotNetNuke.Common.Globals.ApplicationMapPath
                End If
            End If
            convertedPath = System.IO.Path.GetFullPath(convertedPath)

            If Not convertedPath.StartsWith(DotNetNuke.Common.Globals.ApplicationMapPath) Then
                Throw New System.Web.HttpException()
            End If

            Return convertedPath
        End Function

#End Region

#Region "Public Methods"

        Public Shared Sub AddAllUserReadPermission(ByVal folder As FolderInfo, ByVal permission As PermissionInfo)
            Dim folderPermission As FolderPermissionInfo
            Dim roleId As Integer = Int32.Parse(glbRoleAllUsers)
            folderPermission = (From p In folder.FolderPermissions.Cast(Of FolderPermissionInfo)() _
                                                            Where p.PermissionKey = "READ" _
                                                                AndAlso p.FolderID = folder.FolderID _
                                                                AndAlso p.RoleID = roleId _
                                                                AndAlso p.UserID = Null.NullInteger _
                                                            Select p).SingleOrDefault
            If folderPermission IsNot Nothing Then
                folderPermission.AllowAccess = True
            Else
                'Add new permission
                folderPermission = New FolderPermissionInfo(permission)
                folderPermission.FolderID = folder.FolderID
                folderPermission.UserID = Null.NullInteger
                folderPermission.RoleID = roleId
                folderPermission.AllowAccess = True

                folder.FolderPermissions.Add(folderPermission)
            End If

        End Sub

        Public Shared Sub AddFile(ByVal FileName As String, ByVal PortalId As Integer, ByVal Folder As String, ByVal HomeDirectoryMapPath As String, ByVal contentType As String)
            Dim strFile As String = HomeDirectoryMapPath & Folder & FileName

            ' add file to Files table
            Dim objFiles As New FileController
            Dim finfo As New System.IO.FileInfo(strFile)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, Folder, False)
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo
            objFile = objFiles.GetFile(FileName, PortalId, objFolder.FolderID)

            If objFile Is Nothing Then
                objFile = New DotNetNuke.Services.FileSystem.FileInfo()
                objFile.UniqueId = Guid.NewGuid
                objFile.VersionGuid = Guid.NewGuid

                objFile.PortalId = PortalId
                objFile.FileName = FileName
                objFile.Extension = finfo.Extension
                objFile.Size = CType(finfo.Length, Integer)
                objFile.Width = 0
                objFile.Height = 0
                objFile.ContentType = contentType
                objFile.Folder = FileSystemUtils.FormatFolderPath("")
                objFile.FolderId = objFolder.FolderID

                objFiles.AddFile(objFile)
            Else
                objFile.Extension = finfo.Extension
                objFile.Size = CType(finfo.Length, Integer)
                objFile.Width = 0
                objFile.Height = 0
                objFile.ContentType = contentType
                objFile.SHA1Hash = GetHash(File.ReadAllBytes(strFile))
                objFiles.UpdateFile(objFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a File
        ''' </summary>
        ''' <param name="strFile">The File Name</param>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="ClearCache">A flag that indicates whether the file cache should be cleared</param>
        ''' <remarks>This method is called by the SynchonizeFolder method, when the file exists in the file system
        ''' but not in the Database
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/2/2004	Created
        '''     [cnurse]    04/26/2006  Updated to account for secure storage
        '''     [cnurse]    04/07/2008  Made public
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddFile(ByVal strFile As String, ByVal PortalId As Integer, ByVal ClearCache As Boolean, ByVal folder As FolderInfo) As String
            Dim retValue As String = ""
            Dim hash As String = ""

            Try
                Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
                Dim fInfo As System.IO.FileInfo = New System.IO.FileInfo(strFile)
                Dim sourceFolderName As String = GetSubFolderPath(strFile, PortalId)

                Dim sourceFileName As String
                'Remove the resources extesnion if we are in a Secure Folder
                If folder.StorageLocation = FolderController.StorageLocationTypes.SecureFileSystem Then
                    sourceFileName = GetFileName(strFile)
                Else
                    'sourceFileName = strFile
                    Dim pathItems() As String = strFile.Split(CChar("\"))
                    sourceFileName = pathItems(pathItems.Length - 1)
                End If

                Dim file As DotNetNuke.Services.FileSystem.FileInfo = objFileController.GetFile(sourceFileName, PortalId, folder.FolderID)

                If file Is Nothing Then
                    'Add the new File
                    Dim fileStrm As FileStream = Nothing
                    Try
                        fileStrm = fInfo.OpenRead()
                        AddFile(PortalId, fileStrm, strFile, "", fInfo.Length, sourceFolderName, True, ClearCache, True)
                    Finally
                        If (Not IsNothing(fileStrm)) Then
                            fileStrm.Close()
                            fileStrm.Dispose()
                        End If
                    End Try
                Else
                    If System.IO.File.Exists(strFile) Then hash = GetHash(System.IO.File.ReadAllBytes(strFile))
                    If file.SHA1Hash <> hash OrElse file.Size <> fInfo.Length Then
                        Dim extension As String = Path.GetExtension(strFile).Replace(".", "")
                        UpdateFileData(file.FileId, folder.FolderID, PortalId, sourceFileName, extension, GetContentType(extension), fInfo.Length, sourceFolderName, file.StorageLocation, file.IsCached, hash)
                    End If
                End If
            Catch ex As Exception
                retValue = ex.Message
            End Try

            Return retValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Folder
        ''' </summary>
        ''' <param name="_PortalSettings">Portal Settings for the Portal</param>
        ''' <param name="parentFolder">The Parent Folder Name</param>
        ''' <param name="newFolder">The new Folder Name</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddFolder(ByVal _PortalSettings As PortalSettings, ByVal parentFolder As String, ByVal newFolder As String)
            AddFolder(_PortalSettings, parentFolder, newFolder, FolderController.StorageLocationTypes.InsecureFileSystem)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Folder
        ''' </summary>
        ''' <param name="_PortalSettings">Portal Settings for the Portal</param>
        ''' <param name="parentFolder">The Parent Folder Name</param>
        ''' <param name="newFolder">The new Folder Name</param>
        ''' <param name="StorageLocation">The Storage Location</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddFolder(ByVal _PortalSettings As PortalSettings, ByVal parentFolder As String, ByVal newFolder As String, ByVal StorageLocation As Integer)
            AddFolder(_PortalSettings, parentFolder, newFolder, StorageLocation, Guid.NewGuid())
        End Sub

        Public Shared Sub AddFolder(ByVal _PortalSettings As PortalSettings, ByVal parentFolder As String, ByVal newFolder As String, ByVal StorageLocation As Integer, ByVal uniqueId As Guid)
            Dim PortalId As Integer
            Dim ParentFolderName As String
            Dim dinfo As New System.IO.DirectoryInfo(parentFolder)
            Dim dinfoNew As System.IO.DirectoryInfo

            If _PortalSettings.ActiveTab.ParentId = _PortalSettings.SuperTabId Then
                PortalId = Null.NullInteger
                ParentFolderName = Common.Globals.HostMapPath
            Else
                PortalId = _PortalSettings.PortalId
                ParentFolderName = _PortalSettings.HomeDirectoryMapPath
            End If

            dinfoNew = New System.IO.DirectoryInfo(parentFolder & newFolder)
            If Not dinfoNew.Exists Then
                dinfoNew = dinfo.CreateSubdirectory(newFolder)
            End If

            Dim FolderPath As String = dinfoNew.FullName.Substring(ParentFolderName.Length).Replace("\", "/")

            'Persist in Database
            AddFolder(PortalId, FolderPath, StorageLocation, uniqueId)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a User Folder
        ''' </summary>
        ''' <param name="_PortalSettings">Portal Settings for the Portal</param>
        ''' <param name="parentFolder">The Parent Folder Name</param>
        ''' <param name="UserID">The UserID, in order to generate the path/foldername</param>
        ''' <param name="StorageLocation">The Storage Location</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[jlucarino]	02/26/2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddUserFolder(ByVal _PortalSettings As PortalSettings, ByVal parentFolder As String, ByVal StorageLocation As Integer, ByVal UserID As Integer)
            Dim PortalId As Integer
            Dim ParentFolderName As String
            Dim dinfoNew As System.IO.DirectoryInfo
            Dim RootFolder As String = ""
            Dim SubFolder As String = ""

            PortalId = _PortalSettings.PortalId
            ParentFolderName = _PortalSettings.HomeDirectoryMapPath

            RootFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.Root)
            SubFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.SubFolder)

            'create root folder
            Dim folderPath As String = ""
            folderPath = System.IO.Path.Combine(Path.Combine(ParentFolderName, "Users"), RootFolder)
            dinfoNew = New System.IO.DirectoryInfo(folderPath)
            If Not dinfoNew.Exists Then
                dinfoNew.Create()
                AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\", "/"), StorageLocation)
            End If

            'create two-digit subfolder
            folderPath = System.IO.Path.Combine(folderPath, SubFolder)
            dinfoNew = New System.IO.DirectoryInfo(folderPath)
            If Not dinfoNew.Exists Then
                dinfoNew.Create()
                AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\", "/"), StorageLocation)
            End If

            'create folder from UserID
            folderPath = System.IO.Path.Combine(folderPath, UserID.ToString)
            dinfoNew = New System.IO.DirectoryInfo(folderPath)
            If Not dinfoNew.Exists Then
                dinfoNew.Create()
                Dim folderID As Integer = AddFolder(PortalId, folderPath.Substring(ParentFolderName.Length).Replace("\", "/"), StorageLocation)

                Dim folder As FolderInfo = New FolderController().GetFolderInfo(PortalId, folderID)
                For Each permission As PermissionInfo In PermissionController.GetPermissionsByFolder()
                    If permission.PermissionKey.ToUpper = "READ" OrElse permission.PermissionKey.ToUpper = "WRITE" OrElse permission.PermissionKey.ToUpper = "BROWSE" Then
                        'Give User Read/Write/Browse Access to this folder
                        Dim folderPermission As New FolderPermissionInfo(permission)
                        folderPermission.FolderID = folderID
                        folderPermission.UserID = UserID
                        folderPermission.RoleID = Null.NullInteger
                        folderPermission.AllowAccess = True

                        folder.FolderPermissions.Add(folderPermission)

                        If permission.PermissionKey.ToUpper = "READ" Then
                            'Add All Users Read Access to the folder
                            AddAllUserReadPermission(folder, permission)
                        End If
                    End If
                Next

                FolderPermissionController.SaveFolderPermissions(folder)

            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns path to a User Folder 
        ''' </summary>
        ''' <history>
        ''' 	[jlucarino]	03/01/2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetUserFolderPath(ByVal UserID As Integer) As String
            Dim RootFolder As String
            Dim SubFolder As String
            Dim FullPath As String

            RootFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.Root)
            SubFolder = GetUserFolderPathElement(UserID, EnumUserFolderElement.SubFolder)

            FullPath = System.IO.Path.Combine(RootFolder, SubFolder)
            FullPath = System.IO.Path.Combine(FullPath, UserID.ToString)

            Return String.Format("Users/{0}/", FullPath.Replace("\", "/"))

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns Root and SubFolder elements of User Folder path
        ''' </summary>
        ''' <history>
        ''' 	[jlucarino]	03/01/2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetUserFolderPathElement(ByVal UserID As Integer, ByVal Mode As EnumUserFolderElement) As String
            Const SUBFOLDER_SEED_LENGTH As Integer = 2
            Const BYTE_OFFSET As Integer = 255
            Dim Element As String = ""

            Select Case Mode
                Case EnumUserFolderElement.Root
                    Element = (Convert.ToInt32(UserID) And BYTE_OFFSET).ToString("000")

                Case EnumUserFolderElement.SubFolder
                    Element = UserID.ToString("00").Substring(UserID.ToString("00").Length - SUBFOLDER_SEED_LENGTH, SUBFOLDER_SEED_LENGTH)

            End Select

            Return Element

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a File to a Zip File
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddToZip(ByRef ZipFile As ZipOutputStream, ByVal filePath As String, ByVal fileName As String, ByVal folder As String)
            Dim crc As Crc32 = New Crc32

            Dim fs As FileStream = Nothing
            Try
                'Open File Stream
                fs = File.OpenRead(filePath)

                'Read file into byte array buffer
                Dim buffer As Byte()
                ReDim buffer(Convert.ToInt32(fs.Length) - 1)
                fs.Read(buffer, 0, buffer.Length)

                'Create Zip Entry
                Dim entry As ZipEntry = New ZipEntry(Path.Combine(folder, fileName))
                entry.DateTime = DateTime.Now
                entry.Size = fs.Length
                fs.Close()
                crc.Reset()
                crc.Update(buffer)
                entry.Crc = crc.Value

                'Compress file and add to Zip file
                ZipFile.PutNextEntry(entry)
                ZipFile.Write(buffer, 0, buffer.Length)
            Finally
                If (Not IsNothing(fs)) Then
                    fs.Close()
                    fs.Dispose()
                End If
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Trys to copy a file in the file system
        ''' </summary>
        ''' <param name="sourceFileName">The name of the source file</param>
        ''' <param name="destFileName">The name of the destination file</param>
        ''' <history>
        '''     [cnurse]    06/27/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub CopyFile(ByVal sourceFileName As String, ByVal destFileName As String)
            If File.Exists(destFileName) Then
                File.SetAttributes(destFileName, FileAttributes.Normal)
            End If
            File.Copy(sourceFileName, destFileName, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Copies a File
        ''' </summary>
        ''' <param name="strSourceFile">The original File Name</param>
        ''' <param name="strDestFile">The new File Name</param>
        ''' <param name="settings">The Portal Settings for the Portal/Host Account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/2/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CopyFile(ByVal strSourceFile As String, ByVal strDestFile As String, ByVal settings As PortalSettings) As String
            Return UpdateFile(strSourceFile, strDestFile, GetFolderPortalId(settings), True, False, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UploadFile pocesses a single file 
        ''' </summary>
        ''' <param name="RootPath">The folder where the file will be put</param>
        ''' <param name="FileName">The file name</param>
        ''' <param name="FileData">Content of the file</param>
        ''' <param name="ContentType">Type of content, ie: text/html</param>
        ''' <param name="NewFileName"></param>
        ''' <param name="Unzip"></param> 
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        16/9/2004   Updated for localization, Help and 508
        '''     [Philip Beadle] 10/06/2004  Moved to Globals from WebUpload.ascx.vb so can be accessed by URLControl.ascx
        '''     [cnurse]        04/26/2006  Updated for Secure Storage
        '''     [sleupold]      08/14/2007  Added NewFileName
        '''     [sdarkis]       10/19/2009  Creates a file from a string
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateFileFromString(ByVal RootPath As String, ByVal FileName As String, ByVal FileData As String, ByVal ContentType As String, ByVal NewFileName As String, Optional ByVal Unzip As Boolean = False) As String
            Dim returnValue As String = String.Empty
            Dim memStream As MemoryStream = Nothing

            Try
                memStream = New MemoryStream()
                Dim fileDataBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(FileData)
                memStream.Write(fileDataBytes, 0, fileDataBytes.Length)
                memStream.Flush()
                memStream.Position = 0

                returnValue = CreateFile(RootPath, FileName, memStream.Length, ContentType, memStream, NewFileName, Unzip)
            Catch ex As Exception
                returnValue = ex.Message
            Finally
                If (Not memStream Is Nothing) Then
                    memStream.Close()
                    memStream.Dispose()
                End If
            End Try

            Return returnValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This checks to see if the folder is a protected type of folder 
        ''' </summary>
        ''' <param name="folderPath">String</param>
        ''' <returns>Boolean</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cpaterra]	4/7/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DefaultProtectedFolders(ByVal folderPath As String) As Boolean
            If folderPath = "" Or folderPath.ToLower = "skins" Or folderPath.ToLower = "containers" Or folderPath.ToLower.StartsWith("skins/") = True Or folderPath.ToLower.StartsWith("containers/") = True Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes file in areas with a high degree of concurrent file access (i.e. caching, logging) 
        ''' This solves file concurrency issues under heavy load.
        ''' </summary>
        ''' <param name="filename">String</param>
        ''' <param name="waitInMilliseconds">Int16</param>
        ''' <param name="maxAttempts">Int16</param>
        ''' <returns>Boolean</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[dcaron]	9/17/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteFileWithWait(ByVal filename As String, ByVal waitInMilliseconds As Int16, ByVal maxAttempts As Int16) As Boolean
            If Not File.Exists(filename) Then
                Return True
            End If
            Dim fileDeleted As Boolean = False
            Dim i As Integer = 0
            Do Until fileDeleted = True
                If i > maxAttempts Then Exit Do
                i = i + 1
                Try
                    If File.Exists(filename) Then
                        File.Delete(filename)
                    End If
                    fileDeleted = True 'we don't care if it didn't exist...the operation didn't fail, that's what we care about
                Catch ex As Exception
                    fileDeleted = False
                End Try
                If fileDeleted = False Then
                    Threading.Thread.Sleep(waitInMilliseconds)
                End If
            Loop
            Return fileDeleted
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Trys to delete a file from the file system
        ''' </summary>
        ''' <param name="strFileName">The name of the file</param>
        ''' <history>
        '''     [cnurse]    04/26/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteFile(ByVal strFileName As String)
            If File.Exists(strFileName) Then
                File.SetAttributes(strFileName, FileAttributes.Normal)
                File.Delete(strFileName)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a file
        ''' </summary>
        ''' <param name="strSourceFile">The File to delete</param>
        ''' <param name="settings">The Portal Settings for the Portal/Host Account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        '''     [cnurse]        12/6/2004   delete file from db
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteFile(ByVal strSourceFile As String, ByVal settings As PortalSettings) As String
            Return DeleteFile(strSourceFile, settings, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a file
        ''' </summary>
        ''' <param name="strSourceFile">The File to delete</param>
        ''' <param name="settings">The Portal Settings for the Portal/Host Account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        '''     [cnurse]        12/6/2004   delete file from db
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteFile(ByVal strSourceFile As String, ByVal settings As PortalSettings, ByVal ClearCache As Boolean) As String
            Dim retValue As String = ""
            Dim PortalId As Integer = GetFolderPortalId(settings)
            Dim folderName As String = GetSubFolderPath(strSourceFile, PortalId)
            Dim fileName As String = GetFileName(strSourceFile)

            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, folderName, False)

            If FolderPermissionController.CanAdminFolder(objFolder) Then
                Try
                    'try and delete the Insecure file
                    DeleteFile(strSourceFile)

                    'try and delete the Secure file
                    DeleteFile(strSourceFile & glbProtectedExtension)

                    'Remove file from DataBase
                    Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
                    objFileController.DeleteFile(PortalId, fileName, objFolder.FolderID, ClearCache)

                Catch ioEx As IOException
                    retValue += "<br>" & String.Format(Localization.GetString("FileInUse"), strSourceFile)
                Catch ex As Exception
                    retValue = ex.Message
                End Try
            Else ' insufficient folder permission in the application
                retValue += "<br>" & String.Format(Localization.GetString("InsufficientFolderPermission"), folderName)
            End If
            Return retValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a folder
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="folder">The Directory Info object to delete</param>
        ''' <param name="folderName">The Name of the folder relative to the Root of the Portal</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteFolder(ByVal PortalId As Integer, ByVal folder As System.IO.DirectoryInfo, ByVal folderName As String)

            'Delete Folder
            folder.Delete(False)

            'Remove Folder from DataBase
            Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
            objFolderController.DeleteFolder(PortalId, folderName.Replace("\", "/"))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Moved directly from FileManager code, probably should make extension lookup more generic
        ''' </summary>
        ''' <param name="FileLoc">File Location</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' 	[Jon Henning]	1/4/2005	Fixed extension comparison, added content length header - DNN-386
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DownloadFile(ByVal FileLoc As String)
            Dim objFile As New System.IO.FileInfo(FileLoc)
            Dim objResponse As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            Dim filename As String = objFile.Name
            If HttpContext.Current.Request.UserAgent.IndexOf("; MSIE ") > 0 Then
                filename = HttpUtility.UrlEncode(filename)
            End If
            If objFile.Exists Then
                objResponse.ClearContent()
                objResponse.ClearHeaders()
                objResponse.AppendHeader("content-disposition", "attachment; filename=""" & filename & """")
                objResponse.AppendHeader("Content-Length", objFile.Length.ToString())

                objResponse.ContentType = GetContentType(objFile.Extension.Replace(".", ""))

                WriteFile(objFile.FullName)

                objResponse.Flush()
                objResponse.End()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Streams a file to the output stream if the user has the proper permissions
        ''' </summary>
        ''' <param name="settings">Portal Settings</param>
        ''' <param name="FileId">FileId identifying file in database</param>
        ''' <param name="ClientCache">Cache file in client browser - true/false</param>
        ''' <param name="ForceDownload">Force Download File dialog box - true/false</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DownloadFile(ByVal settings As PortalSettings, ByVal FileId As Integer, ByVal ClientCache As Boolean, ByVal ForceDownload As Boolean) As Boolean

            Return DownloadFile(GetFolderPortalId(settings), FileId, ClientCache, ForceDownload)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Streams a file to the output stream if the user has the proper permissions
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal to which the file belongs</param>
        ''' <param name="FileId">FileId identifying file in database</param>
        ''' <param name="ClientCache">Cache file in client browser - true/false</param>
        ''' <param name="ForceDownload">Force Download File dialog box - true/false</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DownloadFile(ByVal PortalId As Integer, ByVal FileId As Integer, ByVal ClientCache As Boolean, ByVal ForceDownload As Boolean) As Boolean
            Dim blnDownload As Boolean = False

            ' get file
            Dim objFiles As New FileController
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo = objFiles.GetFileById(FileId, PortalId)
            If Not objFile Is Nothing Then
                Dim filename As String = objFile.FileName
                If HttpContext.Current.Request.UserAgent.IndexOf("; MSIE ") > 0 Then
                    filename = HttpUtility.UrlEncode(filename)
                End If

                Dim objFolders As New FolderController
                Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, objFile.Folder, False)

                ' check folder view permissions
                If FolderPermissionController.CanViewFolder(objFolder) Then
                    ' auto sync
                    Dim blnFileExists As Boolean = True
                    If Host.EnableFileAutoSync Then
                        Dim strFile As String = ""
                        Select Case objFile.StorageLocation
                            Case FolderController.StorageLocationTypes.InsecureFileSystem
                                strFile = objFile.PhysicalPath
                            Case FolderController.StorageLocationTypes.SecureFileSystem
                                strFile = objFile.PhysicalPath & glbProtectedExtension
                        End Select
                        If strFile <> "" Then
                            ' synchronize file
                            Dim objFileInfo As System.IO.FileInfo = New System.IO.FileInfo(strFile)
                            If objFileInfo.Exists Then
                                If objFile.Size <> objFileInfo.Length Then
                                    objFile.Size = CType(objFileInfo.Length, Integer)
                                    UpdateFileData(FileId, objFile.FolderId, PortalId, objFile.FileName, objFile.Extension, GetContentType(objFile.Extension), objFileInfo.Length, objFile.Folder, objFile.StorageLocation, objFile.IsCached, objFile.SHA1Hash)
                                End If
                            Else ' file does not exist
                                RemoveOrphanedFile(objFile, PortalId)
                                blnFileExists = False
                            End If
                        End If
                    End If

                    ' download file
                    If blnFileExists Then
                        ' save script timeout
                        Dim scriptTimeOut As Integer = HttpContext.Current.Server.ScriptTimeout

                        ' temporarily set script timeout to large value ( this value is only applicable when application is not running in Debug mode )
                        HttpContext.Current.Server.ScriptTimeout = Integer.MaxValue

                        Dim objResponse As HttpResponse = HttpContext.Current.Response

                        objResponse.ClearContent()
                        objResponse.ClearHeaders()

                        ' force download dialog
                        If ForceDownload Then
                            objResponse.AppendHeader("content-disposition", "attachment; filename=""" & filename & """")
                        Else
                            'use proper file name when browser forces download because of file type (save as name should match file name)
                            objResponse.AppendHeader("content-disposition", "inline; filename=""" & filename & """")
                        End If
                        objResponse.AppendHeader("Content-Length", objFile.Size.ToString())
                        objResponse.ContentType = GetContentType(objFile.Extension.Replace(".", ""))

                        'Stream the file to the response
                        Dim objStream As IO.Stream = Nothing
                        Try
                            objStream = FileSystemUtils.GetFileStream(objFile)
                            WriteStream(objResponse, objStream)
                        Catch ex As Exception
                            ' Trap the error, if any.
                            objResponse.Write("Error : " & ex.Message)
                        Finally
                            If IsNothing(objStream) = False Then
                                ' Close the file.
                                objStream.Close()
                                objStream.Dispose()
                            End If
                        End Try

                        objResponse.Flush()
                        objResponse.End()

                        ' reset script timeout
                        HttpContext.Current.Server.ScriptTimeout = scriptTimeOut

                        blnDownload = True
                    End If
                End If
            End If

            Return blnDownload

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' gets the content type based on the extension
        ''' </summary>
        ''' <param name="extension">The extension</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/26/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetContentType(ByVal extension As String) As String

            Dim contentType As String

            Select Case extension.ToLower
                Case "txt" : contentType = "text/plain"
                Case "htm", "html" : contentType = "text/html"
                Case "rtf" : contentType = "text/richtext"
                Case "jpg", "jpeg" : contentType = "image/jpeg"
                Case "gif" : contentType = "image/gif"
                Case "bmp" : contentType = "image/bmp"
                Case "mpg", "mpeg" : contentType = "video/mpeg"
                Case "avi" : contentType = "video/avi"
                Case "pdf" : contentType = "application/pdf"
                Case "doc", "dot" : contentType = "application/msword"
                Case "csv" : contentType = "text/csv"
                Case "xls", "xlt" : contentType = "application/x-msexcel"
                Case Else : contentType = "application/octet-stream"
            End Select

            Return contentType

        End Function

        Public Shared Function GetFileContent(ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo) As Byte()
            Dim objStream As Stream = Nothing
            Dim objContent As Byte() = Nothing
            Try
                objStream = FileSystemUtils.GetFileStream(objFile)
                Dim objBinaryReader As BinaryReader = New BinaryReader(objStream)
                objContent = objBinaryReader.ReadBytes(CType(objStream.Length, Integer))
                objBinaryReader.Close()
            Finally
                If (Not IsNothing(objStream)) Then
                    objStream.Close()
                    objStream.Dispose()
                End If
            End Try

            Return objContent
        End Function

        Public Shared Function GetFileStream(ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo) As Stream

            Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
            Dim fileStream As Stream = Nothing

            Select Case objFile.StorageLocation
                Case FolderController.StorageLocationTypes.InsecureFileSystem
                    ' read from file system 
                    fileStream = New FileStream(objFile.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Case FolderController.StorageLocationTypes.SecureFileSystem
                    ' read from file system 
                    fileStream = New FileStream(objFile.PhysicalPath & glbProtectedExtension, FileMode.Open, FileAccess.Read, FileShare.Read)
                Case FolderController.StorageLocationTypes.DatabaseSecure
                    ' read from database 
                    fileStream = New MemoryStream(objFileController.GetFileContent(objFile.FileId, objFile.PortalId))
            End Select

            Return fileStream

        End Function

        Public Shared Function GetFilesByFolder(ByVal PortalId As Integer, ByVal folderId As Integer) As ArrayList
            Dim objFileController As New FileController
            Return CBO.FillCollection(objFileController.GetFiles(PortalId, folderId), GetType(DotNetNuke.Services.FileSystem.FileInfo))
        End Function

        Public Shared Function GetFolderPortalId(ByVal settings As PortalSettings) As Integer
            Dim FolderPortalId As Integer = Null.NullInteger
            Dim isHost As Boolean = CType(IIf(settings.ActiveTab.ParentId = settings.SuperTabId, True, False), Boolean)
            If Not isHost Then
                FolderPortalId = settings.PortalId
            End If
            Return FolderPortalId
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets all the folders for a Portal
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/22/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetFolders(ByVal PortalID As Integer) As ArrayList
            Dim objFolderController As New FolderController
            Dim arrFolders As New ArrayList
            For Each folderPair As KeyValuePair(Of String, FolderInfo) In objFolderController.GetFoldersSorted(PortalID)
                arrFolders.Add(folderPair.Value)
            Next
            Return arrFolders
        End Function

        Public Shared Function GetFolder(ByVal PortalID As Integer, ByVal FolderPath As String) As FolderInfo
            Dim objFolderController As New FolderController
            Dim objFolder As FolderInfo = objFolderController.GetFolder(PortalID, FolderPath, False)
            If Host.EnableFileAutoSync Then
                ' synchronize files in folder
                If Not objFolder Is Nothing Then
                    SynchronizeFolder(objFolder.PortalID, objFolder.PhysicalPath, objFolder.FolderPath, False, True, False, False)
                End If
            End If
            Return objFolder
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets all the subFolders for a Parent
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	04/22/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetFoldersByParentFolder(ByVal PortalId As Integer, ByVal ParentFolder As String) As ArrayList

            Dim folders As ArrayList = GetFolders(PortalId)
            Dim subFolders As New ArrayList
            For Each folder As FolderInfo In folders
                Dim strfolderPath As String = folder.FolderPath
                If folder.FolderPath.IndexOf(ParentFolder) > -1 AndAlso _
                        folder.FolderPath <> Null.NullString AndAlso _
                        folder.FolderPath <> ParentFolder Then
                    If ParentFolder = Null.NullString Then
                        If strfolderPath.IndexOf("/") = strfolderPath.LastIndexOf("/") Then
                            subFolders.Add(folder)
                        End If
                    ElseIf strfolderPath.StartsWith(ParentFolder) Then
                        strfolderPath = strfolderPath.Substring(ParentFolder.Length + 1)
                        If strfolderPath.IndexOf("/") = strfolderPath.LastIndexOf("/") Then
                            subFolders.Add(folder)
                        End If
                    End If
                End If
            Next

            Return subFolders

        End Function

        Public Shared Function GetFoldersByUser(ByVal PortalID As Integer, ByVal IncludeSecure As Boolean, ByVal IncludeDatabase As Boolean, ByVal Permissions As String) As ArrayList
            Dim objFolderController As New FolderController
            Dim arrFolders As New ArrayList
            Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

            'Create Home folder if it doesn't exist
            Dim userFolderPath As String = GetUserFolderPath(UserController.GetCurrentUserInfo().UserID)
            Dim userFolder As FolderInfo = New FolderController().GetFolder(PortalID, userFolderPath, False)
            If userFolder Is Nothing Then
                'Add User folder
                FileSystemUtils.AddUserFolder(PortalSettings.Current, PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath, FolderController.StorageLocationTypes.InsecureFileSystem, UserController.GetCurrentUserInfo().UserID)
                userFolder = New FolderController().GetFolder(PortalID, userFolderPath, False)
            End If

            'Get all the folders for the Portal
            For Each folder As FolderInfo In objFolderController.GetFoldersByPermissionsSorted(PortalID, Permissions, objUserInfo.UserID).Values

                'Skip if database is ignored
                If (Not IncludeDatabase And folder.StorageLocation = FolderController.StorageLocationTypes.DatabaseSecure) Then
                    Continue For
                End If

                'Skip if secure is ignored
                If (Not IncludeSecure And folder.StorageLocation = FolderController.StorageLocationTypes.SecureFileSystem) Then
                    Continue For
                End If

                If (folder.FolderPath.StartsWith("users/", StringComparison.InvariantCultureIgnoreCase)) Then
                    If (folder.FolderID = userFolder.FolderID) Then
                        folder.DisplayPath = "My Folder/"
                        folder.DisplayName = "My Folder"
                    Else
                        Continue For
                    End If
                End If

                arrFolders.Add(folder)
            Next

            Return arrFolders

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Moves (Renames) a File
        ''' </summary>
        ''' <param name="strSourceFile">The original File Name</param>
        ''' <param name="strDestFile">The new File Name</param>
        ''' <param name="settings">The Portal Settings for the Portal/Host Account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/2/2004	Created
        ''' </history>host  dnnhost
        ''' 
        ''' -----------------------------------------------------------------------------
        Public Shared Function MoveFile(ByVal strSourceFile As String, ByVal strDestFile As String, ByVal settings As PortalSettings) As String
            Return UpdateFile(strSourceFile, strDestFile, GetFolderPortalId(settings), False, False, True)
        End Function

        Public Shared Function ReadFile(ByVal filePath As String) As String
            Dim objStreamReader As StreamReader = Nothing
            Dim fileContent As String = String.Empty
            Try
                objStreamReader = File.OpenText(filePath)
                fileContent = objStreamReader.ReadToEnd
            Finally
                If (Not IsNothing(objStreamReader)) Then
                    objStreamReader.Close()
                    objStreamReader.Dispose()
                End If
            End Try

            Return fileContent
        End Function

        Public Shared Sub RemoveOrphanedFolders(ByVal PortalId As Integer)
            Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
            Dim objFolder As FolderInfo
            Dim arrFolders As ArrayList = GetFolders(PortalId)
            For Each objFolder In arrFolders
                If objFolder.StorageLocation <> FolderController.StorageLocationTypes.DatabaseSecure Then
                    If Directory.Exists(objFolder.PhysicalPath) = False Then
                        RemoveOrphanedFiles(objFolder, PortalId)
                        objFolderController.DeleteFolder(PortalId, objFolder.FolderPath)
                    End If
                End If
            Next
        End Sub

        Public Shared Sub SaveFile(ByVal FullFileName As String, ByVal Buffer As Byte())
            If System.IO.File.Exists(FullFileName) Then
                System.IO.File.SetAttributes(FullFileName, FileAttributes.Normal)
            End If

            Dim fs As FileStream = Nothing
            Try
                fs = New FileStream(FullFileName, FileMode.Create, FileAccess.Write)
                fs.Write(Buffer, 0, Buffer.Length)
            Finally
                If (Not IsNothing(fs)) Then
                    fs.Close()
                    fs.Dispose()
                End If
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns 1 or more attributes to a file
        ''' </summary>
        ''' <param name="FileLoc">File Location</param>
        ''' <param name="FileAttributesOn">Pass in Attributes you wish to switch on (i.e. FileAttributes.Hidden + FileAttributes.ReadOnly)</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	11/1/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetFileAttributes(ByVal FileLoc As String, ByVal FileAttributesOn As Integer)
            System.IO.File.SetAttributes(FileLoc, CType(FileAttributesOn, FileAttributes))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a Folders Permissions to the Administrator Role
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="FolderId">The Id of the Folder</param>
        ''' <param name="AdministratorRoleId">The Id of the Administrator Role</param>
        ''' <param name="relativePath">The folder's Relative Path</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/4/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetFolderPermissions(ByVal PortalId As Integer, ByVal FolderId As Integer, ByVal AdministratorRoleId As Integer, ByVal relativePath As String)
            'Get the folder
            Dim folder As FolderInfo = New FolderController().GetFolderInfo(PortalId, FolderId)

            'Set Permissions
            For Each objPermission As PermissionInfo In PermissionController.GetPermissionsByFolder()
                Dim folderPermission As New FolderPermissionInfo(objPermission)
                folderPermission.FolderID = FolderId
                folderPermission.RoleID = AdministratorRoleId
                folder.FolderPermissions.Add(folderPermission, True)
            Next

            FolderPermissionController.SaveFolderPermissions(folder)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a Folders Permissions the same as the Folders parent folder
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="FolderId">The Id of the Folder</param>
        ''' <param name="relativePath">The folder's Relative Path</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetFolderPermissions(ByVal PortalId As Integer, ByVal FolderId As Integer, ByVal relativePath As String)
            If relativePath <> "" Then
                'Get the folder
                Dim folder As FolderInfo = New FolderController().GetFolderInfo(PortalId, FolderId)

                Dim parentFolderPath As String = relativePath.Substring(0, relativePath.Substring(0, relativePath.Length - 1).LastIndexOf("/") + 1)

                'Iterate parent permissions to see if permisison has already been added
                For Each objPermission As FolderPermissionInfo In FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, parentFolderPath)
                    Dim folderPermission As New FolderPermissionInfo(objPermission)
                    folderPermission.FolderID = FolderId
                    folderPermission.RoleID = objPermission.RoleID
                    folderPermission.UserID = objPermission.UserID
                    folderPermission.AllowAccess = objPermission.AllowAccess
                    folder.FolderPermissions.Add(folderPermission, True)
                Next

                FolderPermissionController.SaveFolderPermissions(folder)
            End If
        End Sub

        Public Shared Sub SetFolderPermission(ByVal PortalId As Integer, ByVal FolderId As Integer, ByVal PermissionId As Integer, ByVal RoleId As Integer, ByVal relativePath As String)
            SetFolderPermission(PortalId, FolderId, PermissionId, RoleId, Null.NullInteger, relativePath)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets a Folder Permission
        ''' </summary>
        ''' <param name="PortalId">The Id of the Portal</param>
        ''' <param name="FolderId">The Id of the Folder</param>
        ''' <param name="PermissionId">The Id of the Permission</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <param name="relativePath">The folder's Relative Path</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/12/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetFolderPermission(ByVal PortalId As Integer, ByVal FolderId As Integer, ByVal PermissionId As Integer, ByVal RoleId As Integer, ByVal UserId As Integer, ByVal relativePath As String)
            Dim objFolderPermissionInfo As FolderPermissionInfo
            Dim objController As New FolderController
            Dim folder As FolderInfo = objController.GetFolderInfo(PortalId, FolderId)

            'Iterate current permissions to see if permisison has already been added
            For Each objFolderPermissionInfo In folder.FolderPermissions
                If objFolderPermissionInfo.FolderID = FolderId And _
                    objFolderPermissionInfo.PermissionID = PermissionId And _
                    objFolderPermissionInfo.RoleID = RoleId And _
                    objFolderPermissionInfo.UserID = UserId And _
                    objFolderPermissionInfo.AllowAccess = True Then
                    Exit Sub
                End If
            Next

            'Permission not found so Add
            objFolderPermissionInfo = New FolderPermissionInfo
            objFolderPermissionInfo.FolderID = FolderId
            objFolderPermissionInfo.PermissionID = PermissionId
            objFolderPermissionInfo.RoleID = RoleId
            objFolderPermissionInfo.UserID = UserId
            objFolderPermissionInfo.AllowAccess = True
            folder.FolderPermissions.Add(objFolderPermissionInfo, True)

            FolderPermissionController.SaveFolderPermissions(folder)
        End Sub

        Public Shared Sub Synchronize(ByVal PortalId As Integer, ByVal AdministratorRoleId As Integer, ByVal HomeDirectory As String, ByVal hideSystemFolders As Boolean)
            Dim PhysicalRoot As String = HomeDirectory
            Dim VirtualRoot As String = ""

            'Call Synchronize Folder that recursively parses the subfolders
            SynchronizeFolder(PortalId, PhysicalRoot, VirtualRoot, True, True, True, hideSystemFolders)

            'Invalidate Cache
            DataCache.ClearFolderCache(PortalId)
        End Sub

        Public Shared Sub SynchronizeFolder(ByVal PortalId As Integer, ByVal physicalPath As String, ByVal relativePath As String, ByVal isRecursive As Boolean, ByVal hideSystemFolders As Boolean)
            SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, True, True, hideSystemFolders)
        End Sub

        Public Shared Sub SynchronizeFolder(ByVal PortalId As Integer, ByVal physicalPath As String, ByVal relativePath As String, ByVal isRecursive As Boolean, ByVal syncFiles As Boolean, ByVal forceFolderSync As Boolean, ByVal hideSystemFolders As Boolean)
            Dim objFolderController As New FolderController()
            Dim FolderId As Integer
            Dim isInSync As Boolean = True

            ' synchronize folder collection
            If forceFolderSync = True And relativePath = "" Then
                RemoveOrphanedFolders(PortalId)
            End If

            'Attempt to get the folder
            Dim folder As FolderInfo = objFolderController.GetFolder(PortalId, relativePath, False)

            Dim dirInfo As New DirectoryInfo(physicalPath)
            If dirInfo.Exists Then
                ' check to see if the folder exists in the db
                If folder Is Nothing Then
                    If ShouldSyncFolder(hideSystemFolders, dirInfo) Then
                        'Add Folder to database
                        FolderId = AddFolder(PortalId, relativePath, FolderController.StorageLocationTypes.InsecureFileSystem)
                        folder = objFolderController.GetFolder(PortalId, relativePath, True)
                        isInSync = False
                    Else
                        'Prevent further processing of this folder
                        Exit Sub
                    End If
                Else
                    'Check whether existing folder is in sync by comparing LastWriteTime of the physical folder with the LastUpdated value in the database
                    '*NOTE: dirInfo.LastWriteTime is updated when files are added to or deleted from a directory - but NOT when existing files are overwritten ( this is a known Windows Operating System issue )
                    isInSync = (dirInfo.LastWriteTime.ToString("yyyyMMddhhmmss") = folder.LastUpdated.ToString("yyyyMMddhhmmss"))
                End If

                If Not folder Is Nothing Then
                    If syncFiles = True And (isInSync = False Or forceFolderSync = True) Then

                        Select Case folder.StorageLocation
                            Case FolderController.StorageLocationTypes.InsecureFileSystem, FolderController.StorageLocationTypes.SecureFileSystem
                                'Get Physical Files in this Folder and sync them
                                Dim strFiles As String() = Directory.GetFiles(physicalPath)
                                For Each strFileName As String In strFiles
                                    'Add the File if it doesn't exist, Update it if the file size has changed
                                    AddFile(strFileName, PortalId, False, folder)
                                Next
                                'Removed orphaned files
                                RemoveOrphanedFiles(folder, PortalId)

                            Case FolderController.StorageLocationTypes.DatabaseSecure
                                'Get DB Files in this Folder and sync them
                                For Each finfo As DotNetNuke.Services.FileSystem.FileInfo In GetFilesByFolder(PortalId, folder.FolderID)
                                    'Add the File if it doesn't exist, Update it if the file size has changed
                                    Dim fctrl As New FileController
                                    Dim memStream As New MemoryStream()
                                    Dim bytes() As Byte = fctrl.GetFileContent(finfo.FileId, PortalId)

                                    If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                                        memStream.Write(bytes, 0, bytes.Length)
                                        memStream.Flush()
                                        memStream.Position = 0
                                    End If

                                    AddFile(PortalId, memStream, finfo.PhysicalPath, finfo.ContentType, finfo.Size, folder.FolderPath, True, True)
                                Next
                        End Select

                        'Update the folder with the LastWriteTime of the directory
                        folder.LastUpdated = dirInfo.LastWriteTime
                        objFolderController.UpdateFolder(folder)
                    End If

                    'Get Physical Sub Folders (and synchronize recursively)
                    If isRecursive Then
                        Dim strFolders As String() = Directory.GetDirectories(physicalPath)
                        For Each strFolder As String In strFolders
                            Dim dir As New DirectoryInfo(strFolder)
                            Dim relPath As String = Null.NullString
                            If relativePath = "" Then
                                relPath = dir.Name & "/"
                            Else
                                relPath = relativePath
                                If Not relativePath.EndsWith("/") Then
                                    relPath = relPath & "/"
                                End If
                                relPath = relPath & dir.Name & "/"
                            End If
                            SynchronizeFolder(PortalId, strFolder, relPath, True, syncFiles, forceFolderSync, hideSystemFolders)
                        Next
                    End If
                End If
            Else ' physical folder does not exist on file system
                If Not folder Is Nothing Then
                    ' folder exists in DB
                    If folder.StorageLocation <> FolderController.StorageLocationTypes.DatabaseSecure Then
                        ' remove files and folder from DB
                        RemoveOrphanedFiles(folder, PortalId)
                        objFolderController.DeleteFolder(PortalId, relativePath.Replace("\", "/"))
                    End If
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Unzips a File
        ''' </summary>
        ''' <param name="fileName">The zip File Name</param>
        ''' <param name="DestFolder">The folder where the file is extracted to</param>
        ''' <param name="settings">The Portal Settings for the Portal/Host Account</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UnzipFile(ByVal fileName As String, ByVal DestFolder As String, ByVal settings As PortalSettings) As String
            Dim objZipInputStream As ZipInputStream = Nothing
            Dim strMessage As String = ""

            Try
                Dim FolderPortalId As Integer = GetFolderPortalId(settings)
                Dim isHost As Boolean = CType(IIf(settings.ActiveTab.ParentId = settings.SuperTabId, True, False), Boolean)

                Dim objPortalController As New PortalController
                Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
                Dim objFileController As New DotNetNuke.Services.FileSystem.FileController
                Dim sourceFolderName As String = GetSubFolderPath(fileName, FolderPortalId)
                Dim sourceFileName As String = GetFileName(fileName)
                Dim folder As FolderInfo = objFolderController.GetFolder(FolderPortalId, sourceFolderName, False)
                Dim file As DotNetNuke.Services.FileSystem.FileInfo = objFileController.GetFile(sourceFileName, FolderPortalId, folder.FolderID)
                Dim storageLocation As Integer = folder.StorageLocation
                Dim objZipEntry As ZipEntry

                Dim strFileName As String = ""
                Dim strExtension As String

                'Get the source Content from wherever it is

                'Create a Zip Input Stream
                Try
                    objZipInputStream = New ZipInputStream(GetFileStream(file))
                Catch ex As Exception
                    Return ex.Message
                End Try

                Dim sortedFolders As New ArrayList

                objZipEntry = objZipInputStream.GetNextEntry

                'iterate folders
                While Not objZipEntry Is Nothing
                    If objZipEntry.IsDirectory Then
                        Try
                            sortedFolders.Add(objZipEntry.Name.ToString)
                        Catch ex As Exception
                            objZipInputStream.Close()
                            Return ex.Message
                        End Try
                    End If
                    objZipEntry = objZipInputStream.GetNextEntry
                End While

                sortedFolders.Sort()

                For Each s As String In sortedFolders
                    Try
                        AddFolder(settings, DestFolder, s.ToString, storageLocation)
                    Catch ex As Exception
                        Return ex.Message
                    End Try
                Next

                'Recreate the Zip Input Stream and parse it for the files
                objZipInputStream = New ZipInputStream(GetFileStream(file))
                objZipEntry = objZipInputStream.GetNextEntry
                While Not objZipEntry Is Nothing
                    If Not objZipEntry.IsDirectory Then
                        If objPortalController.HasSpaceAvailable(FolderPortalId, objZipEntry.Size) Then
                            strFileName = Path.GetFileName(objZipEntry.Name)
                            If strFileName <> "" Then
                                strExtension = Path.GetExtension(strFileName).Replace(".", "")
                                If InStr(1, "," & Host.FileExtensions.ToLower, "," & strExtension.ToLower) <> 0 Or isHost Then
                                    Try
                                        Dim folderPath As String = System.IO.Path.GetDirectoryName(DestFolder & Replace(objZipEntry.Name, "/", "\"))
                                        Dim Dinfo As New DirectoryInfo(folderPath)
                                        If Not Dinfo.Exists Then
                                            AddFolder(settings, DestFolder, objZipEntry.Name.Substring(0, Replace(objZipEntry.Name, "/", "\").LastIndexOf("\")))
                                        End If

                                        Dim zipEntryFileName As String = DestFolder & Replace(objZipEntry.Name, "/", "\")
                                        strMessage += AddFile(FolderPortalId, objZipInputStream, zipEntryFileName, "", objZipEntry.Size, GetSubFolderPath(zipEntryFileName, settings.PortalId), False, False)
                                    Catch ex As Exception
                                        If Not objZipInputStream Is Nothing Then
                                            objZipInputStream.Close()
                                        End If
                                        Return ex.Message
                                    End Try
                                Else
                                    ' restricted file type
                                    strMessage += "<br>" & String.Format(Localization.GetString("RestrictedFileType"), strFileName, Replace(Host.FileExtensions, ",", ", *."))
                                End If
                            End If
                        Else    ' file too large
                            strMessage += "<br>" & String.Format(Localization.GetString("DiskSpaceExceeded"), strFileName)
                        End If

                    End If

                    objZipEntry = objZipInputStream.GetNextEntry
                End While
            Finally
                If (Not IsNothing(objZipInputStream)) Then
                    objZipInputStream.Close()
                    objZipInputStream.Dispose()
                End If
            End Try

            Return strMessage
        End Function

        Public Shared Sub UnzipResources(ByVal zipStream As ZipInputStream, ByVal destPath As String)
            Try

                Dim objZipEntry As ZipEntry
                Dim LocalFileName, RelativeDir, FileNamePath As String

                objZipEntry = zipStream.GetNextEntry
                While Not objZipEntry Is Nothing
                    ' This gets the Zipped FileName (including the path)
                    LocalFileName = objZipEntry.Name

                    ' This creates the necessary directories if they don't 
                    ' already exist.
                    RelativeDir = Path.GetDirectoryName(objZipEntry.Name)
                    If (RelativeDir <> String.Empty) AndAlso (Not Directory.Exists(Path.Combine(destPath, RelativeDir))) Then
                        Directory.CreateDirectory(Path.Combine(destPath, RelativeDir))
                    End If

                    ' This block creates the file using buffered reads from the zipfile
                    If (Not objZipEntry.IsDirectory) AndAlso (LocalFileName <> "") Then
                        FileNamePath = Path.Combine(destPath, LocalFileName).Replace("/", "\")

                        Try
                            ' delete the file if it already exists
                            If File.Exists(FileNamePath) Then
                                File.SetAttributes(FileNamePath, FileAttributes.Normal)
                                File.Delete(FileNamePath)
                            End If

                            ' create the file
                            Dim objFileStream As FileStream = Nothing

                            Try
                                objFileStream = File.Create(FileNamePath)
                                Dim intSize As Integer = 2048
                                Dim arrData(2048) As Byte

                                intSize = zipStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objFileStream.Write(arrData, 0, intSize)
                                    intSize = zipStream.Read(arrData, 0, arrData.Length)
                                End While
                            Finally
                                If (Not IsNothing(objFileStream)) Then
                                    objFileStream.Close()
                                    objFileStream.Dispose()
                                End If
                            End Try
                        Catch
                            ' an error occurred saving a file in the resource file
                        End Try
                    End If

                    objZipEntry = zipStream.GetNextEntry
                End While
            Finally
                If (Not IsNothing(zipStream)) Then
                    zipStream.Close()
                    zipStream.Dispose()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' UploadFile pocesses a single file 
        ''' </summary>
        ''' <param name="RootPath">The folder wherr the file will be put</param>
        ''' <param name="objHtmlInputFile">The file to upload</param>
        Public Shared Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As HttpPostedFile) As String
            Return UploadFile(RootPath, objHtmlInputFile, Null.NullString, False)
        End Function

        ''' <summary>
        ''' UploadFile pocesses a single file 
        ''' </summary>
        ''' <param name="RootPath">The folder wherr the file will be put</param>
        ''' <param name="objHtmlInputFile">The file to upload</param>
        ''' <param name="Unzip"></param> 
        Public Shared Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As HttpPostedFile, ByVal Unzip As Boolean) As String
            Return UploadFile(RootPath, objHtmlInputFile, Null.NullString, Unzip)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UploadFile pocesses a single file 
        ''' </summary>
        ''' <param name="RootPath">The folder wherr the file will be put</param>
        ''' <param name="objHtmlInputFile">The file to upload</param>
        ''' <param name="NewFileName"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        16/9/2004   Updated for localization, Help and 508
        '''     [Philip Beadle] 10/06/2004  Moved to Globals from WebUpload.ascx.vb so can be accessed by URLControl.ascx
        '''     [cnurse]        04/26/2006  Updated for Secure Storage
        '''     [sleupold]      08/14/2007  Added NewFileName
        '''     [sdarkis]       10/19/2009  Calls CreateFile
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As HttpPostedFile, ByVal NewFileName As String) As String
            Return CreateFile(RootPath, objHtmlInputFile.FileName, objHtmlInputFile.ContentLength, objHtmlInputFile.ContentType, objHtmlInputFile.InputStream, NewFileName, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UploadFile pocesses a single file 
        ''' </summary>
        ''' <param name="RootPath">The folder wherr the file will be put</param>
        ''' <param name="objHtmlInputFile">The file to upload</param>
        ''' <param name="NewFileName"></param>
        ''' <param name="Unzip"></param> 
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        16/9/2004   Updated for localization, Help and 508
        '''     [Philip Beadle] 10/06/2004  Moved to Globals from WebUpload.ascx.vb so can be accessed by URLControl.ascx
        '''     [cnurse]        04/26/2006  Updated for Secure Storage
        '''     [sleupold]      08/14/2007  Added NewFileName
        '''     [sdarkis]       10/19/2009  Calls CreateFile
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As HttpPostedFile, ByVal NewFileName As String, ByVal Unzip As Boolean) As String
            Return CreateFile(RootPath, objHtmlInputFile.FileName, objHtmlInputFile.ContentLength, objHtmlInputFile.ContentType, objHtmlInputFile.InputStream, NewFileName, Unzip)
        End Function

        Public Shared Function UploadFile(ByVal RootPath As String, ByVal objHtmlInputFile As Telerik.Web.UI.UploadedFile, ByVal NewFileName As String) As String
            Return CreateFile(RootPath, objHtmlInputFile.FileName, objHtmlInputFile.ContentLength, objHtmlInputFile.ContentType, objHtmlInputFile.InputStream, NewFileName, False)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes file to response stream.  Workaround offered by MS for large files
        ''' http://support.microsoft.com/default.aspx?scid=kb;EN-US;812406
        ''' </summary>
        ''' <param name="strFileName">FileName</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	1/4/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteFile(ByVal strFileName As String)
            Dim objResponse As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            Dim objStream As System.IO.Stream = Nothing

            Try
                ' Open the file.
                objStream = New System.IO.FileStream(strFileName, System.IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

                WriteStream(objResponse, objStream)
            Catch ex As Exception
                ' Trap the error, if any.
                objResponse.Write("Error : " & ex.Message)
            Finally
                If IsNothing(objStream) = False Then
                    ' Close the file.
                    objStream.Close()
                    objStream.Dispose()
                End If
            End Try
        End Sub

        Public Shared Function DeleteFiles(ByVal arrPaths As Array) As String
            Dim strPath As String
            Dim strExceptions As String = ""

            For Each strPath In arrPaths
                ' remove comment
                If strPath.IndexOf("'") <> -1 Then
                    strPath = strPath.Substring(0, strPath.IndexOf("'"))
                End If

                If strPath.Trim <> "" Then
                    strPath = Common.Globals.ApplicationMapPath & "\" & strPath
                    If strPath.EndsWith("\") Then
                        ' folder
                        If Directory.Exists(strPath) Then
                            Try ' delete the folder
                                DotNetNuke.Common.DeleteFolderRecursive(strPath)
                            Catch ex As Exception
                                strExceptions += "Error: " & ex.Message & vbCrLf
                            End Try
                        End If
                    Else
                        ' file
                        If File.Exists(strPath) Then
                            Try ' delete the file
                                File.SetAttributes(strPath, FileAttributes.Normal)
                                File.Delete(strPath)
                            Catch ex As Exception
                                strExceptions += "Error: " & ex.Message & vbCrLf
                            End Try
                        End If
                    End If
                End If
            Next

            Return strExceptions
        End Function

        Public Shared Function SendFile(ByVal URL As String, ByVal FilePath As String) As String
            Dim strMessage As String = ""
            Try
                Dim objWebClient As New WebClient()
                Dim responseArray As Byte() = objWebClient.UploadFile(URL, "POST", FilePath)
            Catch ex As Exception
                strMessage = ex.Message
            End Try
            Return strMessage
        End Function

        Public Shared Function ReceiveFile(ByVal Request As HttpRequest, ByVal FolderPath As String) As String
            Dim strMessage As String = ""
            Try
                If Request.Files.AllKeys.Length <> 0 Then
                    Dim strKey As String = Request.Files.AllKeys(0)
                    Dim objFile As HttpPostedFile = Request.Files(strKey)
                    objFile.SaveAs(FolderPath & objFile.FileName)
                End If
            Catch ex As Exception
                strMessage = ex.Message
            End Try
            Return strMessage
        End Function

        Public Shared Function PullFile(ByVal URL As String, ByVal FilePath As String) As String
            Dim strMessage As String = ""
            Try
                Dim objWebClient As New WebClient()
                objWebClient.DownloadFile(URL, FilePath)
            Catch ex As Exception
                strMessage = ex.Message
            End Try
            Return strMessage
        End Function

        Public Shared Function GetHash(ByVal bytes As Byte()) As String
            Dim hashText As String = ""
            Dim hexValue As String = ""

            Dim hashData As Byte() = SHA1.Create().ComputeHash(bytes) 'SHA1 or MD5

            For Each b As Byte In hashData
                hexValue = b.ToString("X").ToLower() 'Lowercase for compatibility on case-sensitive systems
                hashText += (If(hexValue.Length = 1, "0", "")) & hexValue
            Next

            Return hashText
        End Function
#End Region

#Region "Obsoleted Methods, retained for Binary Compatability"

        'Overload to preserve backwards compatability
        <Obsolete("This function has been replaced by Synchronize(PortalId, AdministratorRoleId, HomeDirectory, hideSystemFolders). Deprecated in DotNetNuke 5.3.0")> _
        Public Shared Sub Synchronize(ByVal PortalId As Integer, ByVal AdministratorRoleId As Integer, ByVal HomeDirectory As String)
            Synchronize(PortalId, AdministratorRoleId, HomeDirectory, False)
        End Sub

        'Overload to preserve backwards compatability
        <Obsolete("This function has been replaced by SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, hideSystemFolders).  Deprecated in DotNetNuke 5.3.0")> _
        Public Shared Sub SynchronizeFolder(ByVal PortalId As Integer, ByVal physicalPath As String, ByVal relativePath As String, ByVal isRecursive As Boolean)
            SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, False)
        End Sub

        'Overload to preserve backwards compatability
        <Obsolete("This function has been replaced by SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, syncFiles, forceFolderSync, hideSystemFolders).  Deprecated in DotNetNuke 5.3.0")> _
        Public Shared Sub SynchronizeFolder(ByVal PortalId As Integer, ByVal physicalPath As String, ByVal relativePath As String, ByVal isRecursive As Boolean, ByVal syncFiles As Boolean, ByVal forceFolderSync As Boolean)
            SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, syncFiles, forceFolderSync, False)
        End Sub

        <Obsolete("This function has been replaced by GetFileContent(FileInfo)")> _
        Public Shared Function GetFileContent(ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo, ByVal PortalId As Integer, ByVal HomeDirectory As String) As Byte()
            Return GetFileContent(objFile)
        End Function

        <Obsolete("This function has been replaced by GetFilesByFolder(PortalId, FolderId)")> _
        Public Shared Function GetFilesByFolder(ByVal PortalId As Integer, ByVal folderPath As String) As ArrayList
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, folderPath, False)
            If objFolder Is Nothing Then
                Return Nothing
            End If
            Return GetFilesByFolder(PortalId, objFolder.FolderID)
        End Function

        <Obsolete("This function has been replaced by GetFileStream(FileInfo)")> _
        Public Shared Function GetFileStream(ByVal objFile As DotNetNuke.Services.FileSystem.FileInfo, ByVal PortalId As Integer, ByVal HomeDirectory As String) As Stream
            Return GetFileStream(objFile)
        End Function

        <Obsolete("This function has been replaced by GetFoldersByUser(ByVal PortalID As Integer, ByVal IncludeSecure As Boolean, ByVal IncludeDatabase As Boolean, ByVal Permissions As String)")> _
        Public Shared Function GetFoldersByUser(ByVal PortalID As Integer, ByVal IncludeSecure As Boolean, ByVal IncludeDatabase As Boolean, ByVal AllowAccess As Boolean, ByVal Permissions As String) As ArrayList
            Return GetFoldersByUser(PortalID, IncludeSecure, IncludeDatabase, Permissions)
        End Function

        <Obsolete("Deprecated in DNN 5.1.  Use FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, Folder).ToString(Permission) ")> _
        Public Shared Function GetRoles(ByVal Folder As String, ByVal PortalId As Integer, ByVal Permission As String) As String
            Return FolderPermissionController.GetFolderPermissionsCollectionByFolder(PortalId, Folder).ToString(Permission)
        End Function

        <Obsolete("This function has been replaced by SynchronizeFolder(Integer, Integer, String, String, Boolean)")> _
        Public Shared Sub SynchronizeFolder(ByVal PortalId As Integer, ByVal AdministratorRoleId As Integer, ByVal HomeDirectory As String, ByVal physicalPath As String, ByVal relativePath As String, ByVal isRecursive As Boolean)
            SynchronizeFolder(PortalId, physicalPath, relativePath, isRecursive, True, True, False)
        End Sub

#End Region

    End Class

End Namespace
