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
Imports System.Xml
Imports System.Web
Imports System.Reflection
Imports DotNetNuke.Services.FileSystem
Imports Telerik.Web.UI
Imports DotNetNuke
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services
Imports DotNetNuke.Security.Permissions

Imports System.Collections.Generic
Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider

    Public Class FileSystemValidation

        Public EnableDetailedLogging As Boolean = True

#Region "Public Folder Validate Methods"

        Public Overridable Function OnCreateFolder(ByVal virtualPath As String, ByVal folderName As String) As String
            Dim returnValue As String = String.Empty
            Try
                returnValue = Check_CanAddToFolder(virtualPath)
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPath, folderName)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnDeleteFolder(ByVal virtualPath As String) As String
            Dim returnValue As String = String.Empty
            Try
                returnValue = Check_CanDeleteFolder(virtualPath)
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPath)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnMoveFolder(ByVal virtualPath As String, ByVal virtualDestinationPath As String) As String
            Dim returnValue As String = String.Empty
            Try
                returnValue = Check_CanDeleteFolder(virtualPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_CanAddToFolder(virtualDestinationPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPath, virtualDestinationPath)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnRenameFolder(ByVal virtualPath As String) As String
            Dim returnValue As String = String.Empty
            Try
                returnValue = Check_CanAddToFolder(FileSystemValidation.GetDestinationFolder(virtualPath))
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_CanDeleteFolder(virtualPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPath)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnCopyFolder(ByVal virtualPath As String, ByVal virtualDestinationPath As String) As String
            Dim returnValue As String = String.Empty
            Try
                returnValue = Check_CanCopyFolder(virtualPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_CanAddToFolder(virtualDestinationPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPath, virtualDestinationPath)
            End Try

            Return returnValue
        End Function

#End Region

#Region "Public File Validate Methods"

        Public Overridable Function OnCreateFile(ByVal virtualPathAndFile As String, ByVal contentLength As Integer) As String
            Dim returnValue As String = String.Empty
            Try
                Dim virtualPath As String = RemoveFileName(virtualPathAndFile)
                returnValue = Check_CanAddToFolder(virtualPath, True)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_FileName(virtualPathAndFile)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_DiskSpace(virtualPathAndFile, contentLength)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndFile, contentLength)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnDeleteFile(ByVal virtualPathAndFile As String) As String
            Dim returnValue As String = String.Empty
            Try
                Dim virtualPath As String = RemoveFileName(virtualPathAndFile)

                returnValue = Check_CanDeleteFolder(virtualPath, True)
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndFile)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnRenameFile(ByVal virtualPathAndFile As String) As String
            Dim returnValue As String = String.Empty
            Try
                Dim virtualPath As String = RemoveFileName(virtualPathAndFile)

                returnValue = Check_CanAddToFolder(virtualPath, True)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = Check_CanDeleteFolder(virtualPath, True)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                Return returnValue
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndFile)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnMoveFile(ByVal virtualPathAndFile As String, ByVal virtualNewPathAndFile As String) As String
            Dim returnValue As String = String.Empty
            Try
                Dim virtualPath As String = RemoveFileName(virtualPathAndFile)

                returnValue = Check_CanDeleteFolder(virtualPath, True)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                Return OnCreateFile(virtualNewPathAndFile, 0)
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndFile, virtualNewPathAndFile)
            End Try

            Return returnValue
        End Function

        Public Overridable Function OnCopyFile(ByVal virtualPathAndFile As String, ByVal virtualNewPathAndFile As String) As String
            Dim returnValue As String = String.Empty
            Try
                Dim existingFileSize As Integer = GetFileSize(virtualPathAndFile)
                If (existingFileSize < 0) Then
                    Return LogDetailError(ErrorCodes.FileDoesNotExist, virtualPathAndFile, True)
                End If

                Dim virtualPath As String = RemoveFileName(virtualPathAndFile)
                returnValue = Check_CanCopyFolder(virtualPath, True)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                Return OnCreateFile(virtualNewPathAndFile, existingFileSize)
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndFile, virtualNewPathAndFile)
            End Try

            Return returnValue
        End Function

#End Region

#Region "Public Shared Path Properties and Convert Methods"

        ''' <summary>
        ''' Gets the DotNetNuke Portal Directory Virtual path
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property HomeDirectory() As String
            Get
                Dim homeDir As String = PortalController.GetCurrentPortalSettings().HomeDirectory
                homeDir = homeDir.Replace("\", "/")

                If (homeDir.EndsWith("/")) Then
                    homeDir = homeDir.Remove(homeDir.Length - 1, 1)
                End If

                Return homeDir
            End Get
        End Property

        ''' <summary>
        ''' Gets the DotNetNuke Portal Directory Root localized text to display to the end user
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property EndUserHomeDirectory() As String
            Get
                Dim text As String = Localization.Localization.GetString("PortalRoot.Text")
                If (String.IsNullOrEmpty(text)) Then
                    Return "Portal Root"
                End If

                Return text.Replace("/", " ").Replace("\", " ").Trim()
            End Get
        End Property

        ''' <summary>
        ''' Gets the DotNetNuke Portal Directory Root as stored in the database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property DBHomeDirectory() As String
            Get
                Return String.Empty
            End Get
        End Property

        ''' <summary>
        ''' Results in a virtual path to a folder or file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToVirtualPath(ByVal path As String)
            path = path.Replace("\", "/")

            If (path.StartsWith(EndUserHomeDirectory)) Then
                path = HomeDirectory + path.Substring(EndUserHomeDirectory.Length)
            End If

            If (Not path.StartsWith(HomeDirectory)) Then
                path = CombineVirtualPath(HomeDirectory, path)
            End If

            If (IO.Path.GetExtension(path) = String.Empty AndAlso Not path.EndsWith("/")) Then
                path = path + "/"
            End If

            Return path.Replace("\", "/")
        End Function

        ''' <summary>
        ''' Results in the path displayed to the end user
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToEndUserPath(ByVal path As String)
            path = path.Replace("\", "/")

            If (path.StartsWith(HomeDirectory)) Then
                path = EndUserHomeDirectory + path.Substring(HomeDirectory.Length)
            End If

            If (Not path.StartsWith(EndUserHomeDirectory)) Then
                If (Not path.StartsWith("/")) Then
                    path = "/" + path
                End If
                path = EndUserHomeDirectory + path
            End If

            If (IO.Path.GetExtension(path) = String.Empty AndAlso Not path.EndsWith("/")) Then
                path = path + "/"
            End If

            Return path
        End Function

        ''' <summary>
        ''' Results in a path that can be used in database calls
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToDBPath(ByVal path As String)
            Return ToDBPath(path, True)
        End Function

        Private Shared Function ToDBPath(ByVal path As String, ByVal removeFileName As Boolean)
            Dim returnValue As String = path

            returnValue = returnValue.Replace("\", "/")
            returnValue = FileSystemValidation.RemoveFileName(returnValue)

            If (returnValue.StartsWith(HomeDirectory)) Then
                returnValue = returnValue.Substring(HomeDirectory.Length)
            End If

            If (returnValue.StartsWith(EndUserHomeDirectory)) Then
                returnValue = returnValue.Substring(EndUserHomeDirectory.Length)
            End If

            'folders in dnn db do not start with /
            If (returnValue.StartsWith("/")) Then
                returnValue = returnValue.Remove(0, 1)
            End If

            'Root directory is an empty string
            If (returnValue = "/" Or returnValue = "\") Then
                returnValue = String.Empty
            End If

            'root folder (empty string) does not contain / - all other folders must contain a slash at the end
            If (Not String.IsNullOrEmpty(returnValue) AndAlso Not returnValue.EndsWith("/")) Then
                returnValue = returnValue + "/"
            End If

            Return returnValue
        End Function

        Public Shared Function CombineVirtualPath(ByVal virtualPath As String, ByVal folderOrFileName As String)
            Dim returnValue As String = Path.Combine(virtualPath, folderOrFileName)
            returnValue = returnValue.Replace("\", "/")

            If (Path.GetExtension(returnValue) = String.Empty AndAlso Not returnValue.EndsWith("/")) Then
                returnValue = returnValue + "/"
            End If

            Return returnValue
        End Function

        Public Shared Function RemoveFileName(ByVal path As String)
            If (Not String.IsNullOrEmpty(System.IO.Path.GetExtension(path))) Then
                path = System.IO.Path.GetDirectoryName(path).Replace("\", "/") + "/"
            End If

            Return path
        End Function

#End Region

#Region "Public Data Access"

        Public Overridable Function GetUserFolders() As IDictionary(Of String, FileSystem.FolderInfo)
            Return UserFolders
        End Function

        Public Overridable Function GetUserFolder(ByVal path As String) As FileSystem.FolderInfo
            Dim dbPath As String = FileSystemValidation.ToDBPath(path)

            If (UserFolders.ContainsKey(dbPath)) Then
                Return UserFolders(dbPath)
            End If

            Return Nothing
        End Function

        Public Overridable Function GetChildUserFolders(ByVal parentPath As String) As IDictionary(Of String, FileSystem.FolderInfo)
            Dim dbPath As String = FileSystemValidation.ToDBPath(parentPath)
            Dim returnValue As IDictionary(Of String, FileSystem.FolderInfo) = New Dictionary(Of String, FileSystem.FolderInfo)

            If (String.IsNullOrEmpty(dbPath)) Then
                'Get first folder children
                For Each folderPath As String In UserFolders.Keys
                    If (folderPath.IndexOf("/") = folderPath.LastIndexOf("/")) Then
                        returnValue.Add(folderPath, UserFolders(folderPath))
                    End If
                Next
            Else
                For Each folderPath As String In UserFolders.Keys
                    If (folderPath = dbPath Or Not folderPath.StartsWith(dbPath)) Then
                        Continue For
                    End If

                    If (folderPath.Contains(dbPath)) Then
                        Dim childPath As String = folderPath.Substring(dbPath.Length)
                        If (childPath.LastIndexOf("/") > -1) Then
                            childPath = childPath.Substring(0, childPath.Length - 1)
                        End If

                        If (Not childPath.Contains("/")) Then
                            returnValue.Add(folderPath, UserFolders(folderPath))
                        End If
                    End If
                Next
            End If

            Return returnValue
        End Function

        Public Shared Function GetDestinationFolder(ByVal virtualPath As String) As String
            Dim splitPath As String = virtualPath
            If (splitPath.Substring(splitPath.Length - 1) = "/") Then
                splitPath = splitPath.Remove(splitPath.Length - 1, 1)
            End If

            If (splitPath = FileSystemValidation.HomeDirectory) Then
                Return splitPath
            End If

            Dim pathList As String() = splitPath.Split("/")
            If (pathList.Length > 0) Then
                Dim folderName As String = pathList(pathList.Length - 1)

                Dim folderSubString As String = splitPath.Substring(splitPath.Length - folderName.Length)
                If (folderSubString = folderName) Then
                    Return splitPath.Substring(0, splitPath.Length - folderName.Length)
                End If
            End If

            Return String.Empty
        End Function

#End Region

#Region "Public Permissions Checks"

        Public Overridable Function CanViewFolder(ByVal path As String) As Boolean
            Return UserFolders.ContainsKey(ToDBPath(path))
        End Function

        Public Overridable Function CanViewFolder(ByVal dnnFolder As FileSystem.FolderInfo) As Boolean
            Return UserFolders.ContainsKey(dnnFolder.FolderPath)
        End Function

        Public Overridable Function CanViewFilesInFolder(ByVal path As String) As Boolean
            Return CanViewFilesInFolder(GetUserFolder(path))
        End Function

        Public Overridable Function CanViewFilesInFolder(ByVal dnnFolder As FileSystem.FolderInfo) As Boolean
            If (IsNothing(dnnFolder)) Then
                Return False
            End If

            If (Not CanViewFolder(dnnFolder)) Then
                Return False
            End If

            If (Not FolderPermissionController.CanViewFolder(dnnFolder)) Then
                Return False
            End If

            Return True
        End Function

        Public Overridable Function CanAddToFolder(ByVal dnnFolder As FileSystem.FolderInfo) As Boolean
            If (Not FolderPermissionController.CanAddFolder(dnnFolder)) Then
                Return False
            End If

            'If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
            '    Return False
            'End If

            Return True
        End Function

        Public Overridable Function CanDeleteFolder(ByVal dnnFolder As FileSystem.FolderInfo) As Boolean
            If (Not FolderPermissionController.CanDeleteFolder(dnnFolder)) Then
                Return False
            End If

            If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
                Return False
            End If

            Return True
        End Function

        'In Addition to Permissions:
        'don't allow upload or delete for database or secured file folders, because this provider does not handle saving to db or adding .resource extensions
        'is protected means it is a system folder that cannot be deleted
        Private Function Check_CanAddToFolder(ByVal virtualPath As String) As String
            Return Check_CanAddToFolder(GetDNNFolder(virtualPath), False, EnableDetailedLogging)
        End Function

        Private Function Check_CanAddToFolder(ByVal virtualPath As String, ByVal isFileCheck As Boolean) As String
            Return Check_CanAddToFolder(GetDNNFolder(virtualPath), isFileCheck, EnableDetailedLogging)
        End Function

        Private Function Check_CanAddToFolder(ByVal dnnFolder As FileSystem.FolderInfo, ByVal isFileCheck As Boolean, ByVal logDetail As Boolean) As String
            If (dnnFolder Is Nothing) Then
                Return LogDetailError(ErrorCodes.FolderDoesNotExist, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'check permissions
            If (Not FolderPermissionController.CanAddFolder(dnnFolder)) Then
                Return LogDetailError(ErrorCodes.AddFolder_NoPermission, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'only allow management of regular storage type
            If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
                Return LogDetailError(ErrorCodes.AddFolder_NotInsecureFolder, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            Return String.Empty
        End Function

        Private Function Check_CanCopyFolder(ByVal virtualPath As String) As String
            Return Check_CanCopyFolder(GetDNNFolder(virtualPath), False, EnableDetailedLogging)
        End Function

        Private Function Check_CanCopyFolder(ByVal virtualPath As String, ByVal isFileCheck As Boolean) As String
            Return Check_CanCopyFolder(GetDNNFolder(virtualPath), isFileCheck, EnableDetailedLogging)
        End Function

        Private Function Check_CanCopyFolder(ByVal dnnFolder As FileSystem.FolderInfo, ByVal isFileCheck As Boolean, ByVal logDetail As Boolean) As String
            If (dnnFolder Is Nothing) Then
                Return LogDetailError(ErrorCodes.FolderDoesNotExist, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'check permissions 
            If (Not FolderPermissionController.CanCopyFolder(dnnFolder)) Then
                Return LogDetailError(ErrorCodes.CopyFolder_NoPermission, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'only allow management of regular storage type
            If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
                Return LogDetailError(ErrorCodes.CopyFolder_NotInsecureFolder, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            Return String.Empty
        End Function

        Private Function Check_CanDeleteFolder(ByVal virtualPath As String) As String
            Return Check_CanDeleteFolder(GetDNNFolder(virtualPath), False, EnableDetailedLogging)
        End Function

        Private Function Check_CanDeleteFolder(ByVal virtualPath As String, ByVal isFileCheck As Boolean) As String
            Return Check_CanDeleteFolder(GetDNNFolder(virtualPath), isFileCheck, EnableDetailedLogging)
        End Function

        Private Function Check_CanDeleteFolder(ByVal virtualPath As String, ByVal isFileCheck As Boolean, ByVal logDetail As Boolean) As String
            Return Check_CanDeleteFolder(GetDNNFolder(virtualPath), isFileCheck, EnableDetailedLogging)
        End Function

        Private Function Check_CanDeleteFolder(ByVal dnnFolder As FileSystem.FolderInfo, ByVal isFileCheck As Boolean, ByVal logDetail As Boolean) As String
            If (dnnFolder Is Nothing) Then
                Return LogDetailError(ErrorCodes.FolderDoesNotExist, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'skip additional folder checks when it is a file
            If (Not isFileCheck) Then
                'Don't allow delete of root folder, root is a protected folder, but show a special message
                If (dnnFolder.FolderPath = DBHomeDirectory) Then
                    Return LogDetailError(ErrorCodes.DeleteFolder_Root, ToVirtualPath(dnnFolder.FolderPath))
                End If

                'Don't allow deleting of any protected folder
                If (dnnFolder.IsProtected) Then
                    Return LogDetailError(ErrorCodes.DeleteFolder_Protected, ToVirtualPath(dnnFolder.FolderPath), logDetail)
                End If
            End If

            'check permissions 
            If (Not FolderPermissionController.CanDeleteFolder(dnnFolder)) Then
                Return LogDetailError(ErrorCodes.DeleteFolder_NoPermission, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            'only allow management of regular storage type
            If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
                Return LogDetailError(ErrorCodes.DeleteFolder_NotInsecureFolder, ToVirtualPath(dnnFolder.FolderPath), logDetail)
            End If

            Return String.Empty
        End Function

#End Region


#Region "Private Check Methods"

        Private Function Check_FileName(ByVal virtualPathAndName As String) As String
            Try
                Dim fileName As String = Path.GetFileName(virtualPathAndName)
                System.Diagnostics.Debug.Assert(Not String.IsNullOrEmpty(fileName), "fileName is empty")

                Dim extension As String = Path.GetExtension(fileName).Replace(".", "").ToLowerInvariant()
                Dim validExtensions As String = DotNetNuke.Entities.Host.Host.FileExtensions.ToLowerInvariant()

                If String.IsNullOrEmpty(extension) OrElse ("," + validExtensions + ",").IndexOf("," + extension + ",") = -1 Then
                    If Not HttpContext.Current Is Nothing Then
                        Return String.Format(Localization.Localization.GetString("RestrictedFileType") _
                          , ToEndUserPath(virtualPathAndName) _
                          , Replace(validExtensions, ",", ", *."))
                    Else
                        Return "RestrictedFileType"
                    End If
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndName)
            End Try

            Return String.Empty
        End Function

        ''' <summary>
        ''' Validates disk space available
        ''' </summary>
        ''' <param name="virtualPathAndName">The system path. ie: C:\WebSites\DotNetNuke_Community\Portals\0\sample.gif</param>
        ''' <param name="contentLength">Content Length</param>
        ''' <returns>The error message or empty string</returns>
        ''' <remarks></remarks>
        Private Function Check_DiskSpace(ByVal virtualPathAndName As String, ByVal contentLength As Integer)
            Try
                Dim fileName As String = Path.GetFileName(virtualPathAndName)
                Dim portalCtrl As New PortalController
                If (Not portalCtrl.HasSpaceAvailable(PortalController.GetCurrentPortalSettings.PortalId, contentLength)) Then
                    Return String.Format(Localization.Localization.GetString("DiskSpaceExceeded"), ToEndUserPath(virtualPathAndName))
                End If
            Catch ex As Exception
                Return LogUnknownError(ex, virtualPathAndName, contentLength)
            End Try

            Return String.Empty
        End Function

#End Region

#Region "Misc Helper Methods"

        Private Function GetFileSize(ByVal virtualPathAndFile As String) As Integer
            Dim returnValue As Integer = -1

            If (Not File.Exists(virtualPathAndFile)) Then
                Dim openFile As FileStream = Nothing
                Try
                    openFile = File.OpenRead(virtualPathAndFile)
                    returnValue = openFile.Length
                Finally
                    If (Not IsNothing(openFile)) Then
                        openFile.Close()
                        openFile.Dispose()
                    End If
                    returnValue = -1
                End Try
            End If

            Return returnValue
        End Function

        Private _UserFolders As IDictionary(Of String, FileSystem.FolderInfo) = Nothing
        Private ReadOnly Property UserFolders() As IDictionary(Of String, FileSystem.FolderInfo)
            Get
                If (_UserFolders Is Nothing) Then
                    _UserFolders = New Dictionary(Of String, FileSystem.FolderInfo)

                    For Each folder As FolderInfo In FileSystemUtils.GetFoldersByUser(PortalSettings.PortalId, True, True, "READ")

                        Dim folderPath As String = folder.FolderPath

                        If (Not String.IsNullOrEmpty(folderPath)) Then

                            folderPath = folderPath.TrimEnd("/")

                            If (folderPath.Contains("/")) Then
                                Dim folderPaths As String() = folderPath.Split("/")
                                'If (folderPaths.Length > 0) Then
                                Dim addPath As String = String.Empty
                                For Each addFolderPath In folderPaths
                                    If (addPath = String.Empty) Then
                                        addPath = addFolderPath + "/"
                                    Else
                                        addPath = addPath + addFolderPath + "/"
                                    End If

                                    If (_UserFolders.ContainsKey(addPath)) Then
                                        Continue For
                                    End If

                                    Dim addFolder As FileSystem.FolderInfo = GetDNNFolder(addPath)
                                    If (addFolder Is Nothing) Then
                                        Exit For
                                    End If

                                    _UserFolders.Add(addFolder.FolderPath, addFolder)
                                Next
                            Else
                                _UserFolders.Add(folder.FolderPath, folder)
                            End If
                        Else
                            _UserFolders.Add(folder.FolderPath, folder)
                        End If
                    Next
                End If

                Return _UserFolders
            End Get
        End Property

        Private Function GetDNNFolder(ByVal path As String) As FileSystem.FolderInfo
            Return DNNFolderCtrl.GetFolder(PortalSettings.PortalId, FileSystemValidation.ToDBPath(path), False)
        End Function

        Private _DNNFolderCtrl As FileSystem.FolderController = Nothing
        Private ReadOnly Property DNNFolderCtrl() As FileSystem.FolderController
            Get
                If (_DNNFolderCtrl Is Nothing) Then
                    _DNNFolderCtrl = New FileSystem.FolderController()
                End If
                Return _DNNFolderCtrl
            End Get
        End Property

        Private _PortalSettings As FileSystemValidation = Nothing
        Private ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalSettings.Current
            End Get
        End Property

        Protected Friend Function LogUnknownError(ByRef ex As Exception, ByVal ParamArray params As String()) As String
            Dim returnValue As String = GetUnknownText()
            Dim exc As FileManagerException = New FileManagerException(GetSystemErrorText(params), ex)
            Exceptions.LogException(exc)
            Return returnValue
        End Function

        Public Function LogDetailError(ByVal errorCode As ErrorCodes) As String
            Return LogDetailError(errorCode, String.Empty, EnableDetailedLogging)
        End Function

        Public Function LogDetailError(ByVal errorCode As ErrorCodes, ByVal virtualPath As String) As String
            Return LogDetailError(errorCode, virtualPath, EnableDetailedLogging)
        End Function

        Public Function LogDetailError(ByVal errorCode As ErrorCodes, ByVal virtualPath As String, ByVal logError As Boolean) As String
            Dim endUserPath As String = String.Empty
            If (Not String.IsNullOrEmpty(virtualPath)) Then
                endUserPath = ToEndUserPath(virtualPath)
            End If

            Dim returnValue As String = GetPermissionErrorText(endUserPath)
            Dim logMsg As String = String.Empty

            Select Case errorCode
                Case ErrorCodes.AddFolder_NoPermission _
                 , ErrorCodes.AddFolder_NotInsecureFolder _
                 , ErrorCodes.CopyFolder_NoPermission _
                 , ErrorCodes.CopyFolder_NotInsecureFolder _
                 , ErrorCodes.DeleteFolder_NoPermission _
                 , ErrorCodes.DeleteFolder_NotInsecureFolder _
                 , ErrorCodes.DeleteFolder_Protected _
                 , ErrorCodes.CannotMoveFolder_ChildrenVisible _
                 , ErrorCodes.CannotDeleteFolder_ChildrenVisible _
                 , ErrorCodes.CannotCopyFolder_ChildrenVisible
                    logMsg = GetString("ErrorCodes." + errorCode.ToString())
                    Exit Select
                Case ErrorCodes.DeleteFolder_Root _
                 , ErrorCodes.RenameFolder_Root
                    logMsg = GetString("ErrorCodes." + errorCode.ToString())
                    returnValue = String.Format("{0} [{1}]", GetString("ErrorCodes." + errorCode.ToString()), endUserPath)
                Case ErrorCodes.FileDoesNotExist _
                 , ErrorCodes.FolderDoesNotExist
                    logMsg = String.Empty
                    returnValue = String.Format("{0} [{1}]", GetString("ErrorCodes." + errorCode.ToString()), endUserPath)
                    Exit Select
            End Select

            If (Not String.IsNullOrEmpty(logMsg)) Then
                Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController()
                Dim objEventLogInfo As New DotNetNuke.Services.Log.EventLog.LogInfo()

                objEventLogInfo.AddProperty("From", "TelerikHtmlEditorProvider Message")

                If (Not IsNothing(PortalSettings.ActiveTab)) Then
                    objEventLogInfo.AddProperty("TabID", PortalSettings.ActiveTab.TabID)
                    objEventLogInfo.AddProperty("TabName", PortalSettings.ActiveTab.TabName)
                End If

                Dim user As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()
                If (Not IsNothing(user)) Then
                    objEventLogInfo.AddProperty("UserID", user.UserID)
                    objEventLogInfo.AddProperty("UserName", user.Username)
                End If

                objEventLogInfo.LogTypeKey = DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT.ToString()
                objEventLogInfo.AddProperty("Message", logMsg)
                objEventLogInfo.AddProperty("Path", virtualPath)
                objEventLog.AddLog(objEventLogInfo)
            End If

            Return returnValue
        End Function

#End Region

#Region "Localized Messages"

        Public Function GetString(ByVal key As String) As String
            Dim resourceFile As String = "/Providers/HtmlEditorProviders/Telerik/" + DotNetNuke.Services.Localization.Localization.LocalResourceDirectory + "/FileManager.resx"
            Return DotNetNuke.Services.Localization.Localization.GetString(key, resourceFile)
        End Function

        Private Function GetUnknownText() As String
            Try
                Return GetString("SystemError.Text")
            Catch ex As Exception
                Return "An unknown error occurred."
            End Try
        End Function

        Private Function GetSystemErrorText(ByVal ParamArray params As String()) As String
            Try
                Return GetString("SystemError.Text") + " " + String.Join(" | ", params)
            Catch ex As Exception
                Return "An unknown error occurred." + " " + String.Join(" | ", params)
            End Try
        End Function

        Private Function GetPermissionErrorText(ByVal path As String) As String
            Return GetString("ErrorCodes." + ErrorCodes.General_PermissionDenied.ToString())
            'message text is weird in this scenario
            'Return String.Format(Localization.Localization.GetString("InsufficientFolderPermission"), path)
        End Function

#End Region

    End Class

End Namespace

