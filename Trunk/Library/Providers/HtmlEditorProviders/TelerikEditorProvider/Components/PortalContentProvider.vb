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
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services
Imports DotNetNuke.Security.Permissions

Imports System.Collections.Generic

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider

    Public Class PortalContentProvider
        Inherits Telerik.Web.UI.Widgets.FileSystemContentProvider

        ''' <summary>
        ''' The current portal will be used for file access.
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="searchPatterns"></param>
        ''' <param name="viewPaths"></param>
        ''' <param name="uploadPaths"></param>
        ''' <param name="deletePaths"></param>
        ''' <param name="selectedUrl"></param>
        ''' <param name="selectedItemTag"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal context As HttpContext, ByVal searchPatterns As String(), ByVal viewPaths As String(), ByVal uploadPaths As String(), ByVal deletePaths As String(), ByVal selectedUrl As String, ByVal selectedItemTag As String)
            MyBase.New(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        End Sub

#Region "Overrides"

        'Protected Overrides Function IsValid(ByVal directory As System.IO.DirectoryInfo) As Boolean
        '    Return MyBase.IsValid(directory)
        'End Function

        'Protected Overrides Function IsValid(ByVal file As System.IO.FileInfo) As Boolean
        '    Return MyBase.IsValid(file)
        'End Function

        Public Overrides ReadOnly Property CanCreateDirectory() As Boolean
            Get
                Return MyBase.CanCreateDirectory
            End Get
        End Property

        Public Overrides Function CheckWritePermissions(ByVal folderPath As String) As Boolean
            Dim folder As FolderInfo = DNNFolderCtrl.GetFolder(PortalSettings.PortalId, FileSystemValidation.ToDBPath(folderPath), True)
            Return FolderPermissionController.CanManageFolder(folder)
        End Function

        Public Overrides Function GetFile(ByVal url As String) As System.IO.Stream
            'base calls CheckWritePermissions method
            Return TelerikContent.GetFile(FileSystemValidation.ToVirtualPath(url))
        End Function

        Public Overrides Function CheckDeletePermissions(ByVal folderPath As String) As Boolean
            Dim folder As FolderInfo = DNNFolderCtrl.GetFolder(PortalSettings.PortalId, FileSystemValidation.ToDBPath(folderPath), True)
            Return FolderPermissionController.CanManageFolder(folder)
        End Function

        Public Overrides Function GetPath(ByVal url As String) As String
            Return TelerikContent.GetPath(FileSystemValidation.ToVirtualPath(url))
        End Function

        Public Overrides Function GetFileName(ByVal url As String) As String
            Return TelerikContent.GetFileName(FileSystemValidation.ToVirtualPath(url))
        End Function

        Public Overrides Function CreateDirectory(ByVal path As String, ByVal name As String) As String
            Try
                Dim virtualPath As String = FileSystemValidation.ToVirtualPath(path)

                Dim returnValue As String = DNNValidator.OnCreateFolder(virtualPath, name)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                'Returns errors or empty string when successful (ie: DirectoryAlreadyExists, InvalidCharactersInPath)
                returnValue = TelerikContent.CreateDirectory(virtualPath, name)

                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return GetTelerikMessage(returnValue)
                End If

                If (String.IsNullOrEmpty(returnValue)) Then
                    Dim virtualNewPath As String = FileSystemValidation.CombineVirtualPath(virtualPath, name)
                    Dim newFolderID As Integer = DNNFolderCtrl.AddFolder(PortalSettings.PortalId, FileSystemValidation.ToDBPath(virtualNewPath))
                    FileSystemUtils.SetFolderPermissions(PortalSettings.PortalId, newFolderID, FileSystemValidation.ToDBPath(virtualNewPath))
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, name)
            End Try
        End Function

        Public Overrides Function MoveDirectory(ByVal path As String, ByVal newPath As String) As String
            Try
                Dim virtualPath As String = FileSystemValidation.ToVirtualPath(path)
                Dim virtualNewPath As String = FileSystemValidation.ToVirtualPath(newPath)
                Dim virtualDestinationPath As String = FileSystemValidation.GetDestinationFolder(virtualNewPath)

                Dim returnValue As String = String.Empty
                If (FileSystemValidation.GetDestinationFolder(virtualPath) = virtualDestinationPath) Then
                    'rename directory
                    returnValue = DNNValidator.OnRenameFolder(virtualPath)
                    If (Not String.IsNullOrEmpty(returnValue)) Then
                        Return returnValue
                    End If
                Else
                    'move directory
                    returnValue = DNNValidator.OnMoveFolder(virtualPath, virtualDestinationPath)
                    If (Not String.IsNullOrEmpty(returnValue)) Then
                        Return returnValue
                    End If
                End If

                'Are all items visible to user?
                Dim folder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                If (Not CheckAllChildrenVisible(folder)) Then
                    Return DNNValidator.LogDetailError(ErrorCodes.CannotMoveFolder_ChildrenVisible)
                End If

                'Returns errors or empty string when successful (ie: Cannot create a file when that file already exists)
                returnValue = TelerikContent.MoveDirectory(virtualPath, virtualNewPath)

                If (String.IsNullOrEmpty(returnValue)) Then
                    'Sync to remove old folder & files
                    FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualPath), FileSystemValidation.ToDBPath(virtualPath), True, True, True, True)
                    'Sync to add new folder & files
                    FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualNewPath), FileSystemValidation.ToDBPath(virtualNewPath), True, True, True, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, newPath)
            End Try
        End Function

        Public Overrides Function CopyDirectory(ByVal path As String, ByVal newPath As String) As String
            Try
                Dim virtualPath As String = FileSystemValidation.ToVirtualPath(path)
                Dim virtualNewPath As String = FileSystemValidation.ToVirtualPath(newPath)
                Dim virtualDestinationPath As String = FileSystemValidation.GetDestinationFolder(virtualNewPath)

                Dim returnValue As String = DNNValidator.OnCopyFolder(virtualPath, virtualDestinationPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                'Are all items visible to user?
                'todo: copy visible files and folders only?
                Dim folder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                If (Not CheckAllChildrenVisible(folder)) Then
                    Return DNNValidator.LogDetailError(ErrorCodes.CannotCopyFolder_ChildrenVisible)
                End If

                returnValue = TelerikContent.CopyDirectory(virtualPath, virtualNewPath)

                If (String.IsNullOrEmpty(returnValue)) Then
                    'Sync to add new folder & files
                    FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualNewPath), FileSystemValidation.ToDBPath(virtualNewPath), True, True, True, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, newPath)
            End Try
        End Function

        Public Overrides Function DeleteDirectory(ByVal path As String) As String
            Try
                Dim virtualPath As String = FileSystemValidation.ToVirtualPath(path)

                Dim returnValue As String = DNNValidator.OnDeleteFolder(virtualPath)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                'Are all items visible to user?
                Dim folder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                If (Not CheckAllChildrenVisible(folder)) Then
                    Return DNNValidator.LogDetailError(ErrorCodes.CannotDeleteFolder_ChildrenVisible)
                End If

                returnValue = TelerikContent.DeleteDirectory(virtualPath)

                If (String.IsNullOrEmpty(returnValue)) Then
                    'Sync to remove old folder & files
                    FileSystemUtils.SynchronizeFolder(PortalSettings.PortalId, HttpContext.Current.Request.MapPath(virtualPath), FileSystemValidation.ToDBPath(virtualPath), True, True, True, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path)
            End Try
        End Function

        Public Overrides Function DeleteFile(ByVal path As String) As String
            Try
                Dim virtualPathAndFile As String = FileSystemValidation.ToVirtualPath(path)

                Dim returnValue As String = DNNValidator.OnDeleteFile(virtualPathAndFile)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = TelerikContent.DeleteFile(virtualPathAndFile)

                If (String.IsNullOrEmpty(returnValue)) Then
                    Dim virtualPath As String = FileSystemValidation.RemoveFileName(virtualPathAndFile)
                    Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                    DNNFileCtrl.DeleteFile(PortalSettings.PortalId, System.IO.Path.GetFileName(virtualPathAndFile), dnnFolder.FolderID, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path)
            End Try
        End Function

        Public Overrides Function MoveFile(ByVal path As String, ByVal newPath As String) As String
            Try
                Dim virtualPathAndFile As String = FileSystemValidation.ToVirtualPath(path)
                Dim virtualNewPathAndFile As String = FileSystemValidation.ToVirtualPath(newPath)

                Dim virtualPath As String = FileSystemValidation.RemoveFileName(virtualPathAndFile)
                Dim virtualNewPath As String = FileSystemValidation.RemoveFileName(virtualNewPathAndFile)

                Dim returnValue As String = String.Empty
                If (virtualPath = virtualNewPath) Then
                    'rename file
                    returnValue = DNNValidator.OnRenameFile(virtualPathAndFile)
                    If (Not String.IsNullOrEmpty(returnValue)) Then
                        Return returnValue
                    End If
                Else
                    'move file
                    returnValue = DNNValidator.OnMoveFile(virtualPathAndFile, virtualNewPathAndFile)
                    If (Not String.IsNullOrEmpty(returnValue)) Then
                        Return returnValue
                    End If
                End If

                'Returns errors or empty string when successful (ie: NewFileAlreadyExists)
                returnValue = TelerikContent.MoveFile(virtualPathAndFile, virtualNewPathAndFile)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return GetTelerikMessage(returnValue)
                End If

                If (String.IsNullOrEmpty(returnValue)) Then
                    Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualNewPath)
                    Dim dnnFileInfo As FileSystem.FileInfo = New FileSystem.FileInfo()
                    FillFileInfo(virtualNewPathAndFile, dnnFileInfo)

                    DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, True)

                    Dim dnnOriginalFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                    Dim originalFileName As String = System.IO.Path.GetFileName(virtualPathAndFile)

                    DNNFileCtrl.DeleteFile(PortalSettings.PortalId, originalFileName, dnnOriginalFolder.FolderID, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, newPath)
            End Try
        End Function

        Public Overrides Function CopyFile(ByVal path As String, ByVal newPath As String) As String
            Try
                Dim virtualPathAndFile As String = FileSystemValidation.ToVirtualPath(path)
                Dim virtualNewPathAndFile As String = FileSystemValidation.ToVirtualPath(newPath)

                Dim returnValue As String = DNNValidator.OnCopyFile(virtualPathAndFile, virtualNewPathAndFile)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                'Returns errors or empty string when successful (ie: NewFileAlreadyExists)
                returnValue = TelerikContent.CopyFile(virtualPathAndFile, virtualNewPathAndFile)

                If (String.IsNullOrEmpty(returnValue)) Then
                    Dim virtualNewPath As String = FileSystemValidation.RemoveFileName(virtualNewPathAndFile)
                    Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualNewPath)
                    Dim dnnFileInfo As FileSystem.FileInfo = New FileSystem.FileInfo()
                    FillFileInfo(virtualNewPathAndFile, dnnFileInfo)

                    DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, newPath)
            End Try
        End Function

        Public Overrides Function StoreFile(ByVal file As System.Web.HttpPostedFile, ByVal path As String, ByVal name As String, ByVal ParamArray arguments() As String) As String
            Return StoreFile(Telerik.Web.UI.UploadedFile.FromHttpPostedFile(file), path, name, arguments)
        End Function

        Public Overrides Function StoreFile(ByVal file As Telerik.Web.UI.UploadedFile, ByVal path As String, ByVal name As String, ByVal ParamArray arguments() As String) As String
            Try
                Dim virtualPath As String = FileSystemValidation.ToVirtualPath(path)

                Dim returnValue As String = DNNValidator.OnCreateFile(FileSystemValidation.CombineVirtualPath(virtualPath, name), file.ContentLength)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = TelerikContent.StoreFile(file, virtualPath, name, arguments)

                Dim dnnFileInfo As FileSystem.FileInfo = New FileSystem.FileInfo()
                FillFileInfo(file, dnnFileInfo)

                'Add or update file
                Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                Dim dnnFile As FileSystem.FileInfo = DNNFileCtrl.GetFile(name, PortalSettings.PortalId, dnnFolder.FolderID)
                If (Not IsNothing(dnnFile)) Then
                    DNNFileCtrl.UpdateFile(dnnFile.FileId, dnnFileInfo.FileName, dnnFileInfo.Extension, file.ContentLength, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID)
                Else
                    DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, file.ContentLength, dnnFileInfo.Width, dnnFileInfo.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, path, name)
            End Try
        End Function

        Public Overrides Function StoreBitmap(ByVal bitmap As System.Drawing.Bitmap, ByVal url As String, ByVal format As System.Drawing.Imaging.ImageFormat) As String
            Try
                'base calls CheckWritePermissions method			
                Dim virtualPathAndFile As String = FileSystemValidation.ToVirtualPath(url)
                Dim virtualPath As String = FileSystemValidation.RemoveFileName(virtualPathAndFile)
                Dim returnValue As String = DNNValidator.OnCreateFile(virtualPathAndFile, 0)
                If (Not String.IsNullOrEmpty(returnValue)) Then
                    Return returnValue
                End If

                returnValue = TelerikContent.StoreBitmap(bitmap, virtualPathAndFile, format)

                Dim dnnFileInfo As FileSystem.FileInfo = New FileSystem.FileInfo()
                FillFileInfo(virtualPathAndFile, dnnFileInfo)

                'check again with real contentLength
                Dim errMsg As String = DNNValidator.OnCreateFile(virtualPathAndFile, dnnFileInfo.Size)
                If (Not String.IsNullOrEmpty(errMsg)) Then
                    TelerikContent.DeleteFile(virtualPathAndFile)
                    Return errMsg
                End If

                Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(virtualPath)
                Dim dnnFile As FileSystem.FileInfo = DNNFileCtrl.GetFile(dnnFileInfo.FileName, PortalSettings.PortalId, dnnFolder.FolderID)

                If (Not IsNothing(dnnFile)) Then
                    DNNFileCtrl.UpdateFile(dnnFile.FileId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, bitmap.Width, bitmap.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID)
                Else
                    DNNFileCtrl.AddFile(PortalSettings.PortalId, dnnFileInfo.FileName, dnnFileInfo.Extension, dnnFileInfo.Size, bitmap.Width, bitmap.Height, dnnFileInfo.ContentType, dnnFolder.FolderPath, dnnFolder.FolderID, True)
                End If

                Return returnValue
            Catch ex As Exception
                Return DNNValidator.LogUnknownError(ex, url)
            End Try
        End Function

        Public Overrides Function ResolveDirectory(ByVal path As String) As Widgets.DirectoryItem
            Try
                'System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveDirectory: " + path)
                Return GetDirectoryItemWithDNNPermissions(path, True)
            Catch ex As Exception
                DNNValidator.LogUnknownError(ex, path)
                Return Nothing
            End Try
        End Function

        Public Overrides Function ResolveRootDirectoryAsTree(ByVal path As String) As Widgets.DirectoryItem
            Try
                'System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveRootDirectoryAsTree: " + path)
                Return GetDirectoryItemWithDNNPermissions(path, False)
            Catch ex As Exception
                DNNValidator.LogUnknownError(ex, path)
                Return Nothing
            End Try
        End Function

        Public Overrides Function ResolveRootDirectoryAsList(ByVal path As String) As Telerik.Web.UI.Widgets.DirectoryItem()
            Try
                'System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + "ResolveRootDirectoryAsList: " + path)
                Return GetDirectoryItemWithDNNPermissions(path, False).Directories
            Catch ex As Exception
                DNNValidator.LogUnknownError(ex, path)
                Return Nothing
            End Try
        End Function

#End Region

#Region "Properties"

        Private _DNNValidator As FileSystemValidation = Nothing
        Private ReadOnly Property DNNValidator() As FileSystemValidation
            Get
                If (_DNNValidator Is Nothing) Then
                    _DNNValidator = New FileSystemValidation()
                End If

                Return _DNNValidator
            End Get
        End Property

        Private ReadOnly Property PortalSettings() As DotNetNuke.Entities.Portals.PortalSettings
            Get
                Return DotNetNuke.Entities.Portals.PortalSettings.Current
            End Get
        End Property

        Private ReadOnly Property CurrentUser() As DotNetNuke.Entities.Users.UserInfo
            Get
                Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()
            End Get
        End Property

        Private _TelerikContent As Widgets.FileSystemContentProvider = Nothing
        Private ReadOnly Property TelerikContent() As Widgets.FileSystemContentProvider
            Get
                If (_TelerikContent Is Nothing) Then
                    _TelerikContent = New Widgets.FileSystemContentProvider(Me.Context, Me.SearchPatterns, _
                     New String() {FileSystemValidation.HomeDirectory}, New String() {FileSystemValidation.HomeDirectory}, New String() {FileSystemValidation.HomeDirectory}, FileSystemValidation.ToVirtualPath(Me.SelectedUrl), FileSystemValidation.ToVirtualPath(Me.SelectedItemTag))
                End If
                Return _TelerikContent
            End Get
        End Property

        Private _DNNFolderCtrl As FileSystem.FolderController = Nothing
        Private ReadOnly Property DNNFolderCtrl() As FileSystem.FolderController
            Get
                If (_DNNFolderCtrl Is Nothing) Then
                    _DNNFolderCtrl = New FileSystem.FolderController()
                End If
                Return _DNNFolderCtrl
            End Get
        End Property

        Private _DNNFileCtrl As FileSystem.FileController = Nothing
        Private ReadOnly Property DNNFileCtrl() As FileSystem.FileController
            Get
                If (_DNNFileCtrl Is Nothing) Then
                    _DNNFileCtrl = New FileSystem.FileController()
                End If
                Return _DNNFileCtrl
            End Get
        End Property

#End Region

#Region "Private"

        Private Function GetDirectoryItemWithDNNPermissions(ByVal path As String, ByVal loadFiles As Boolean) As Widgets.DirectoryItem
            Dim radDirectory As Widgets.DirectoryItem = TelerikContent().ResolveDirectory(FileSystemValidation.ToVirtualPath(path))
            Dim returnValues = AddChildDirectoriesToList(New Widgets.DirectoryItem() {radDirectory}, True, loadFiles)

            If (Not returnValues Is Nothing AndAlso returnValues.Length > 0) Then
                Return returnValues(0)
            End If

            Return Nothing
        End Function

        Private Function AddChildDirectoriesToList(ByRef radDirectories As Widgets.DirectoryItem(), ByVal recursive As Boolean, ByVal loadFiles As Boolean) As Widgets.DirectoryItem()
            Dim newDirectories As ArrayList = New ArrayList()

            For Each radDirectory As Widgets.DirectoryItem In radDirectories
                'System.Diagnostics.Debug.WriteLine(DateTime.Now.ToLongTimeString() + " AddChildDirectoriesToList " + radDirectory.Name)

                If (radDirectory Is Nothing) Then
                    Continue For
                End If

                Dim endUserPath As String = FileSystemValidation.ToEndUserPath(radDirectory.FullPath)
                Dim dnnFolder As FileSystem.FolderInfo = DNNValidator.GetUserFolder(radDirectory.FullPath)

                If (IsNothing(dnnFolder)) Then
                    Continue For
                End If

                If (Not dnnFolder Is Nothing) Then
                    'Don't show protected folders
                    If (Not String.IsNullOrEmpty(dnnFolder.FolderPath) AndAlso dnnFolder.IsProtected) Then
                        Continue For
                    End If

                    'Don't show Cache folder
                    If (dnnFolder.FolderPath.ToLowerInvariant() = "cache/") Then
                        Continue For
                    End If

                    Dim showFiles As ArrayList = New ArrayList()
                    Dim folderPermissions As Widgets.PathPermissions = Widgets.PathPermissions.Read

                    If (DNNValidator.CanViewFilesInFolder(dnnFolder)) Then
                        If (DNNValidator.CanAddToFolder(dnnFolder)) Then
                            folderPermissions = folderPermissions Or Widgets.PathPermissions.Upload
                        End If

                        If (DNNValidator.CanDeleteFolder(dnnFolder)) Then
                            folderPermissions = folderPermissions Or Widgets.PathPermissions.Delete
                        End If

                        If (loadFiles) Then
                            Dim dnnFiles As IDictionary(Of String, FileSystem.FileInfo) = GetDNNFiles(dnnFolder.FolderID)

                            If (dnnFolder.StorageLocation <> FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem) Then
                                'check Telerik search patterns to filter out files
                                For Each dnnFile As FileSystem.FileInfo In dnnFiles.Values
                                    If (CheckSearchPatterns(dnnFile.FileName, MyBase.SearchPatterns)) Then
                                        Dim url As String = DotNetNuke.Common.Globals.LinkClick("fileid=" + dnnFile.FileId.ToString(), Null.NullInteger, Null.NullInteger)
                                        '= DotNetNuke.Common.Globals.ApplicationPath & "/LinkClick.aspx?fileticket=" & UrlUtils.EncryptParameter(dnnFile.FileId)

                                        Dim fileItem As Widgets.FileItem = New Widgets.FileItem( _
                                         dnnFile.FileName, dnnFile.Extension, dnnFile.Size, endUserPath, _
                                         url, "", folderPermissions)

                                        showFiles.Add(fileItem)
                                    End If
                                Next
                            Else
                                'check Telerik search patterns to filter out files
                                For Each telerikFile As Widgets.FileItem In radDirectory.Files
                                    If (dnnFiles.ContainsKey(telerikFile.Name)) Then
                                        Dim fileItem As Widgets.FileItem = New Widgets.FileItem( _
                                         telerikFile.Name, telerikFile.Extension, telerikFile.Length, "", _
                                         FileSystemValidation.ToVirtualPath(radDirectory.FullPath) + telerikFile.Name, "", folderPermissions)

                                        showFiles.Add(fileItem)
                                    End If
                                Next
                            End If
                        End If
                    End If

                    Dim folderFiles As Widgets.FileItem() = showFiles.ToArray(GetType(Widgets.FileItem))

                    'Root folder name
                    Dim dirName As String = radDirectory.Name
                    If (dnnFolder.FolderPath = String.Empty And dnnFolder.FolderName = String.Empty) Then
                        dirName = FileSystemValidation.EndUserHomeDirectory
                    End If

                    Dim newDirectory As Widgets.DirectoryItem
                    If (recursive) Then
                        radDirectory = TelerikContent().ResolveRootDirectoryAsTree(radDirectory.Path)
                        newDirectory = New Widgets.DirectoryItem( _
                          dirName, "", endUserPath, "", _
                          folderPermissions, folderFiles, AddChildDirectoriesToList(radDirectory.Directories, False, False))
                    Else
                        newDirectory = New Widgets.DirectoryItem( _
                          dirName, "", endUserPath, "", _
                          folderPermissions, folderFiles, New Widgets.DirectoryItem() {})
                    End If

                    newDirectories.Add(newDirectory)
                End If
            Next

            Return newDirectories.ToArray(GetType(Widgets.DirectoryItem))
        End Function

        Private Function GetDNNFiles(ByVal dnnFolderID As Integer) As IDictionary(Of String, FileSystem.FileInfo)
            Dim drFiles As System.Data.IDataReader = Nothing
            Dim dnnFiles As IDictionary(Of String, FileSystem.FileInfo)

            Try
                drFiles = DNNFileCtrl.GetFiles(PortalSettings.PortalId, dnnFolderID)
                dnnFiles = CBO.FillDictionary(Of String, FileSystem.FileInfo)("FileName", drFiles)
            Finally
                If (Not drFiles Is Nothing) Then
                    If (Not drFiles.IsClosed) Then
                        drFiles.Close()
                    End If
                End If
            End Try

            Return dnnFiles
        End Function

        Private Function CheckAllChildrenVisible(ByRef folder As FileSystem.FolderInfo) As Boolean
            Dim virtualPath As String = FileSystemValidation.ToVirtualPath(folder.FolderPath)

            'check files are visible
            Dim files As IDictionary(Of String, FileSystem.FileInfo) = GetDNNFiles(folder.FolderID)
            Dim visibleFileCount As Integer = 0
            For Each fileItem As FileSystem.FileInfo In files.Values
                If (CheckSearchPatterns(fileItem.FileName, MyBase.SearchPatterns)) Then
                    visibleFileCount = visibleFileCount + 1
                End If
            Next

            If (visibleFileCount <> Directory.GetFiles(HttpContext.Current.Request.MapPath(virtualPath)).Length) Then
                Return False
            End If

            'check folders
            If (Not IsNothing(folder)) Then
                Dim childUserFolders As IDictionary(Of String, FileSystem.FolderInfo) = DNNValidator.GetChildUserFolders(virtualPath)

                If (childUserFolders.Count <> Directory.GetDirectories(HttpContext.Current.Request.MapPath(virtualPath)).Length) Then
                    Return False
                End If

                'check children
                For Each childFolder As FileSystem.FolderInfo In childUserFolders.Values
                    'do recursive check
                    If (Not CheckAllChildrenVisible(childFolder)) Then
                        Return False
                    End If
                Next
            End If

            Return True
        End Function

        Private Sub FillFileInfo(ByVal virtualPathAndFile As String, ByRef fileInfo As FileSystem.FileInfo)
            fileInfo.FileName = Path.GetFileName(virtualPathAndFile)
            fileInfo.Extension = Path.GetExtension(virtualPathAndFile)
            If (fileInfo.Extension.StartsWith(".")) Then
                fileInfo.Extension = fileInfo.Extension.Remove(0, 1)
            End If

            fileInfo.ContentType = FileSystemUtils.GetContentType(fileInfo.Extension)

            Dim fileStream As FileStream = Nothing
            Try
                fileStream = File.OpenRead(HttpContext.Current.Request.MapPath(virtualPathAndFile))
                FillImageInfo(fileStream, fileInfo)
            Finally
                If (Not IsNothing(fileStream)) Then
                    fileStream.Close()
                    fileStream.Dispose()
                End If
            End Try
        End Sub

        Private Sub FillFileInfo(ByVal file As Telerik.Web.UI.UploadedFile, ByRef fileInfo As FileSystem.FileInfo)
            'The core API expects the path to be stripped off the filename
            fileInfo.FileName = If(file.FileName.Contains("\"), System.IO.Path.GetFileName(file.FileName), file.FileName)
            fileInfo.Extension = file.GetExtension()
            If (fileInfo.Extension.StartsWith(".")) Then
                fileInfo.Extension = fileInfo.Extension.Remove(0, 1)
            End If

            fileInfo.ContentType = FileSystemUtils.GetContentType(fileInfo.Extension)

            FillImageInfo(file.InputStream, fileInfo)
        End Sub

        Private Sub FillImageInfo(ByVal fileStream As Stream, ByRef fileInfo As FileSystem.FileInfo)
            If Convert.ToBoolean(InStr(1, DotNetNuke.Common.Globals.glbImageFileTypes + ",", fileInfo.Extension.ToLowerInvariant() & ",")) Then
                Dim img As System.Drawing.Image = Nothing
                Try
                    img = Drawing.Image.FromStream(fileStream)
                    If (fileStream.Length > Integer.MaxValue) Then
                        fileInfo.Size = Integer.MaxValue
                    Else
                        fileInfo.Size = Integer.Parse(fileStream.Length.ToString())
                    End If
                    fileInfo.Width = img.Width
                    fileInfo.Height = img.Height
                Catch
                    ' error loading image file
                    fileInfo.ContentType = "application/octet-stream"
                Finally
                    If (Not IsNothing(img)) Then
                        img.Dispose()
                    End If
                End Try
            End If
        End Sub

#End Region

#Region "Search Patterns"

        Private Function CheckSearchPatterns(ByVal dnnFileName As String, ByRef searchPatterns As String()) As Boolean
            If (searchPatterns Is Nothing Or searchPatterns.Length < 1) Then
                Return True
            End If

            Dim returnValue As Boolean = False
            For Each pattern As String In searchPatterns
                Dim result As Boolean = New System.Text.RegularExpressions.Regex(ConvertToRegexPattern(pattern), Text.RegularExpressions.RegexOptions.IgnoreCase).IsMatch(dnnFileName)

                If (result) Then
                    returnValue = True
                    Exit For
                End If
            Next

            Return returnValue
        End Function

        Private Function ConvertToRegexPattern(ByVal pattern As String)
            Dim returnValue As String = System.Text.RegularExpressions.Regex.Escape(pattern)
            returnValue = returnValue.Replace("\*", ".*")
            returnValue = returnValue.Replace("\?", ".") + "$"
            Return returnValue
        End Function

        Private Function GetTelerikMessage(ByVal key As String)
            Dim returnValue As String = key
            Select Case key
                Case "DirectoryAlreadyExists"
                    returnValue = DNNValidator.GetString("ErrorCodes.DirectoryAlreadyExists")
                    Exit Select
                Case "InvalidCharactersInPath"
                    returnValue = DNNValidator.GetString("ErrorCodes.InvalidCharactersInPath")
                    Exit Select
                Case "NewFileAlreadyExists"
                    returnValue = DNNValidator.GetString("ErrorCodes.NewFileAlreadyExists")
                    Exit Select
                    'Case ""
                    '	Exit Select
            End Select

            Return returnValue
        End Function

#End Region

    End Class

End Namespace

