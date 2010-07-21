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
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Xml
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.FileSystem
    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : FileController
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Business Class that provides access to the Database for the functions within the calling classes
    ''' Instantiates the instance of the DataProvider and returns the object, if any
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[DYNST]	   2/1/2004	   Created
    '''     [vnguyen]  30/04/2010  Modified: Added Guid and VersionG
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FileController

        Friend Function FileChanged(ByVal drOriginalFile As DataRow, ByVal NewFileName As String, ByVal NewExtension As String, ByVal NewSize As Long, ByVal NewWidth As Integer, ByVal NewHeight As Integer, ByVal NewContentType As String, ByVal NewFolder As String) As Boolean
            If Convert.ToString(drOriginalFile("FileName")) <> NewFileName _
            Or Convert.ToString(drOriginalFile("Extension")) <> NewExtension _
            Or Convert.ToInt32(drOriginalFile("Size")) <> NewSize _
            Or Convert.ToInt32(drOriginalFile("Width")) <> NewWidth _
            Or Convert.ToInt32(drOriginalFile("Height")) <> NewHeight _
            Or Convert.ToString(drOriginalFile("ContentType")) <> NewContentType _
            Or Convert.ToString(drOriginalFile("Folder")) <> NewFolder Then
                Return True
            End If
            Return False
        End Function


#Region "Private Methods"
        Private Shared Sub UpdateFileVersion(ByVal fileId As Integer)
            DataProvider.Instance.UpdateFileVersion(fileId, Guid.NewGuid())
        End Sub
#End Region

#Region "Public Methods"
        Public Function AddFile(ByVal file As FileInfo) As Integer
            Dim FileId As Integer = DataProvider.Instance().AddFile(file.PortalId, file.UniqueId, file.VersionGuid, file.FileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, file.Folder, file.FolderId, UserController.GetCurrentUserInfo.UserID, file.SHA1Hash)
            DataCache.RemoveCache("GetFileById" & FileId.ToString)
            Return FileId
        End Function

        Public Sub UpdateFile(ByVal file As FileInfo)
            DataProvider.Instance().UpdateFile(file.FileId, file.VersionGuid, file.FileName, file.Extension, file.Size, file.Width, file.Height, file.ContentType, file.Folder, file.FolderId, UserController.GetCurrentUserInfo.UserID, file.SHA1Hash)
            DataCache.RemoveCache("GetFileById" & file.FileId.ToString)
        End Sub

        Public Sub ClearFileContent(ByVal FileId As Integer)
            DataProvider.Instance().UpdateFileContent(FileId, Nothing)
            UpdateFileVersion(FileId)
        End Sub

        Public Function ConvertFilePathToFileId(ByVal FilePath As String, ByVal PortalID As Integer) As Integer
            Dim FileName As String = ""
            Dim FolderName As String = ""
            Dim FileId As Integer = -1

            If FilePath <> "" Then
                FileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1)
                FolderName = FilePath.Replace(FileName, "")
            End If

            Dim objFiles As New FileController
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalID, FolderName, False)
            If Not objFolder Is Nothing Then
                Dim objFile As FileInfo = objFiles.GetFile(FileName, PortalID, objFolder.FolderID)
                If Not objFile Is Nothing Then
                    FileId = objFile.FileId
                End If
            End If
            Return FileId

        End Function

        Public Sub DeleteFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal FolderID As Integer, ByVal ClearCache As Boolean)

            DataProvider.Instance().DeleteFile(PortalId, FileName, FolderID)

            If ClearCache Then
                GetAllFilesRemoveCache()
            End If

        End Sub

        Public Sub DeleteFiles(ByVal PortalId As Integer)
            DeleteFiles(PortalId, True)
        End Sub

        Public Sub DeleteFiles(ByVal PortalId As Integer, ByVal ClearCache As Boolean)

            DataProvider.Instance().DeleteFiles(PortalId)

            If ClearCache Then
                GetAllFilesRemoveCache()
            End If

        End Sub

        Public Function GetAllFiles() As DataTable
            Dim dt As DataTable = CType(DataCache.GetCache("GetAllFiles"), DataTable)

            If dt Is Nothing Then
                dt = DataProvider.Instance.GetAllFiles()
                DataCache.SetCache("GetAllFiles", dt)
            End If
            If Not dt Is Nothing Then
                Return dt.Copy
            Else
                Return New DataTable
            End If
        End Function

        Public Sub GetAllFilesRemoveCache()
            DataCache.RemoveCache("GetAllFiles")
        End Sub

        Public Function GetFile(ByVal FileName As String, ByVal PortalId As Integer, ByVal FolderID As Integer) As FileInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetFile(FileName, PortalId, FolderID), GetType(FileInfo)), FileInfo)
        End Function

        Public Function GetFileById(ByVal FileId As Integer, ByVal PortalId As Integer) As FileInfo

            Dim objFile As FileInfo

            Dim strCacheKey As String = "GetFileById" & FileId.ToString

            objFile = CType(DataCache.GetCache(strCacheKey), FileInfo)

            If objFile Is Nothing Then
                objFile = CType(CBO.FillObject(DataProvider.Instance().GetFileById(FileId, PortalId), GetType(FileInfo)), FileInfo)

                If Not objFile Is Nothing Then
                    ' cache data
                    Dim intCacheTimeout As Integer = 20 * Convert.ToInt32(Host.PerformanceSetting)
                    DataCache.SetCache(strCacheKey, objFile, TimeSpan.FromMinutes(intCacheTimeout))
                End If
            End If

            Return objFile

        End Function

        Public Function GetFileByUniqueID(ByVal UniqueId As Guid) As FileInfo
            Dim objFile As FileInfo
            objFile = CType(CBO.FillObject(DataProvider.Instance().GetFileByUniqueID(UniqueId), GetType(FileInfo)), FileInfo)
            Return objFile
        End Function

        Public Function GetFileContent(ByVal FileId As Integer, ByVal PortalId As Integer) As Byte()
            Dim objContent() As Byte = Nothing
            Dim dr As IDataReader = Nothing
            Try
                dr = DataProvider.Instance().GetFileContent(FileId, PortalId)
                If dr.Read Then
                    objContent = CType(dr("Content"), Byte())
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
            Return objContent
        End Function

        Public Function GetFiles(ByVal PortalId As Integer, ByVal FolderID As Integer) As IDataReader
            Return DataProvider.Instance().GetFiles(PortalId, FolderID)
        End Function

        Public Sub UpdateFileContent(ByVal FileId As Integer, ByVal Content As Stream)
            If Content IsNot Stream.Null Then
                Dim objBinaryReader As BinaryReader = New BinaryReader(Content)
                Dim objContent() As Byte = objBinaryReader.ReadBytes(CType(Content.Length, Integer))
                objBinaryReader.Close()
                Content.Close()
                DataProvider.Instance().UpdateFileContent(FileId, objContent)
            Else
                DataProvider.Instance().UpdateFileContent(FileId, Nothing)
            End If
            
            UpdateFileVersion(FileId)
        End Sub

        Public Sub UpdateFileContent(ByVal FileId As Integer, ByVal Content() As Byte)
            DataProvider.Instance().UpdateFileContent(FileId, Content)
            UpdateFileVersion(FileId)
        End Sub

        Public Shared Function SerializeFile(ByVal xmlFile As XmlDocument, ByVal objFile As FileInfo) As XmlNode
            Dim nodeTab As XmlNode

            CBO.SerializeObject(objFile, xmlFile)

            nodeTab = xmlFile.SelectSingleNode("file")
            nodeTab.Attributes.Remove(nodeTab.Attributes.ItemOf("xmlns:xsd"))
            nodeTab.Attributes.Remove(nodeTab.Attributes.ItemOf("xmlns:xsi"))

            Return nodeTab
        End Function

        Public Shared Function DeserializeFile(ByVal nodeFile As XmlNode, ByVal portalId As Integer, ByVal folderId As Integer) As FileInfo
            Dim fileCtrl As New FileController
            Dim objFile As New FileInfo

            Dim node As XmlNode = nodeFile.SelectSingleNode("file")

            objFile.UniqueId = New Guid(XmlUtils.GetNodeValue(node, "uniqueid"))
            objFile.VersionGuid = New Guid(XmlUtils.GetNodeValue(node, "versionguid"))
            objFile.PortalId = portalId
            objFile.FileName = XmlUtils.GetNodeValue(node, "filename")
            objFile.Folder = XmlUtils.GetNodeValue(node, "folder")
            objFile.FolderId = folderId
            objFile.ContentType = XmlUtils.GetNodeValue(node, "contenttype")
            objFile.Extension = XmlUtils.GetNodeValue(node, "extension")
            objFile.StorageLocation = XmlUtils.GetNodeValueInt(node, "storagelocation")
            objFile.IsCached = XmlUtils.GetNodeValueBoolean(node, "iscached", False)
            objFile.Size = XmlUtils.GetNodeValueInt(node, "size", Null.NullInteger)
            objFile.Width = XmlUtils.GetNodeValueInt(node, "width", Null.NullInteger)
            objFile.Height = XmlUtils.GetNodeValueInt(node, "height", Null.NullInteger)

            ' create/update file
            Dim objFileOriginal As FileInfo = fileCtrl.GetFileByUniqueID(objFile.UniqueId)
            If objFileOriginal Is Nothing Then
                objFile.FileId = fileCtrl.AddFile(objFile)
            Else
                objFile.FileId = objFileOriginal.FileId
                fileCtrl.UpdateFile(objFile)
                objFile.FileId = fileCtrl.GetFileByUniqueID(objFile.UniqueId).FileId
            End If

            Return objFile
        End Function

#End Region

#Region "Obsolete Methods"

        <Obsolete("This function has been replaced by AddFile(ByVal file As FileInfo)")> _
        Public Function AddFile(ByVal file As FileInfo, ByVal FolderPath As String) As Integer
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(file.PortalId, FolderPath, False)
            file.FolderId = objFolder.FolderID
            file.Folder = FolderPath
            Return AddFile(file)
        End Function

        <Obsolete("This function has been replaced by AddFile(ByVal file As FileInfo)")> _
        Public Function AddFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal FolderPath As String) As Integer
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            Dim objFile As New FileInfo

            objFile.UniqueId = Guid.NewGuid
            objFile.VersionGuid = Guid.NewGuid

            objFile.PortalId = PortalId
            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = FileSystemUtils.FormatFolderPath(FolderPath)
            objFile.FolderId = objFolder.FolderID
            objFile.IsCached = True

            Return AddFile(objFile)
        End Function

        <Obsolete("This function has been replaced by AddFile(ByVal file As FileInfo)")> _
        Public Function AddFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal FolderPath As String, ByVal ClearCache As Boolean) As Integer
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            Dim objFile As New FileInfo

            objFile.UniqueId = Guid.NewGuid
            objFile.VersionGuid = Guid.NewGuid

            objFile.PortalId = PortalId
            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = FileSystemUtils.FormatFolderPath(FolderPath)
            objFile.FolderId = objFolder.FolderID
            objFile.IsCached = ClearCache

            Return AddFile(objFile)
        End Function

        <Obsolete("This function has been replaced by DeleteFile(PortalId, FileName, FolderID, ClearCache)")> _
        Public Sub DeleteFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal FolderPath As String, ByVal ClearCache As Boolean)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            DeleteFile(PortalId, FileName, objFolder.FolderID, ClearCache)
        End Sub

        <Obsolete("This function has been replaced by DeleteFile(PortalId, FileName, FolderID, ClearCache)")> _
        Public Sub DeleteFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal FolderPath As String)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            DeleteFile(PortalId, FileName, objFolder.FolderID, True)
        End Sub

        <Obsolete("This function has been replaced by GetFile(FileName, PortalId, FolderID)")> _
        Public Function GetFile(ByVal FilePath As String, ByVal PortalId As Integer) As FileInfo
            Dim objFolders As New FolderController
            Dim FileName As String = Path.GetFileName(FilePath)
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FilePath.Replace(FileName, ""), False)
            If objFolder Is Nothing Then
                Return Nothing
            Else
                Return GetFile(FileName, PortalId, objFolder.FolderID)
            End If
        End Function

        <Obsolete("This function has been replaced by GetFile(FileName, PortalId, FolderID)")> _
        Public Function GetFile(ByVal FileName As String, ByVal PortalId As Integer, ByVal FolderPath As String) As FileInfo
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            If objFolder Is Nothing Then
                Return Nothing
            Else
                Return GetFile(FileName, PortalId, objFolder.FolderID)
            End If
        End Function

        <Obsolete("This function has been replaced by GetFiles(PortalId, FolderID)")> _
        Public Function GetFiles(ByVal PortalId As Integer, ByVal FolderPath As String) As IDataReader
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            If objFolder Is Nothing Then
                Return Nothing
            End If
            Return GetFiles(PortalId, objFolder.FolderID)
        End Function

        <Obsolete("This function has been replaced by ???")> _
        Public Function GetFilesByFolder(ByVal PortalId As Integer, ByVal FolderPath As String) As ArrayList
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, FolderPath, False)
            If objFolder Is Nothing Then
                Return Nothing
            End If
            Return CBO.FillCollection(GetFiles(PortalId, objFolder.FolderID), GetType(FileInfo))
        End Function

        <Obsolete("Deprecated in DotNetNuke 5.5. This function has been replaced by UpdateFile(ByVal file As FileInfo)")> _
        Public Sub UpdateFile(ByVal PortalId As Integer, ByVal OriginalFileName As String, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal SourceFolder As String, ByVal DestinationFolder As String)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, DestinationFolder, False)
            Dim objFile As FileInfo = GetFile(OriginalFileName, PortalId, objFolder.FolderID)

            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = DestinationFolder

            If Not objFile Is Nothing Then
                UpdateFile(objFile)
            End If
        End Sub

        <Obsolete("Deprecated in DotNetNuke 5.5. This function has been replaced by UpdateFile(ByVal file As FileInfo)")> _
        Public Sub UpdateFile(ByVal PortalId As Integer, ByVal OriginalFileName As String, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal SourceFolder As String, ByVal DestinationFolder As String, ByVal ClearCache As Boolean)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(PortalId, DestinationFolder, False)
            Dim objFile As FileInfo = GetFile(OriginalFileName, PortalId, objFolder.FolderID)

            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = DestinationFolder

            If Not objFile Is Nothing Then
                UpdateFile(objFile)
            End If
        End Sub

        <Obsolete("Deprecated in DotNetNuke 5.5. This function has been replaced by UpdateFile(ByVal file As FileInfo)")> _
        Public Sub UpdateFile(ByVal PortalId As Integer, ByVal OriginalFileName As String, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal SourceFolder As String, ByVal DestinationFolder As String, ByVal FolderID As Integer, ByVal ClearCache As Boolean)
            Dim objFile As FileInfo = GetFile(OriginalFileName, PortalId, FolderID)

            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = DestinationFolder
            objFile.FolderId = FolderID

            If Not objFile Is Nothing Then
                UpdateFile(objFile)
            End If
        End Sub

        <Obsolete("Deprecated in DotNetNuke 5.5. This function has been replaced by AddFile(ByVal file As FileInfo)")> _
        Public Function AddFile(ByVal PortalId As Integer, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal FolderPath As String, ByVal FolderID As Integer, ByVal ClearCache As Boolean) As Integer
            Dim objFile As New FileInfo

            objFile.UniqueId = Guid.NewGuid
            objFile.VersionGuid = Guid.NewGuid

            objFile.PortalId = PortalId
            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = FileSystemUtils.FormatFolderPath(FolderPath)
            objFile.FolderId = FolderID

            If ClearCache Then
                GetAllFilesRemoveCache()
            End If

            Return AddFile(objFile)
        End Function

        <Obsolete("Deprecated in DotNetNuke 5.5. This function has been replaced by UpdateFile(ByVal file As FileInfo)")> _
        Public Sub UpdateFile(ByVal FileId As Integer, ByVal FileName As String, ByVal Extension As String, ByVal Size As Long, ByVal Width As Integer, ByVal Height As Integer, ByVal ContentType As String, ByVal DestinationFolder As String, ByVal FolderID As Integer)
            Dim objFile As New FileInfo

            objFile.FileId = FileId
            objFile.VersionGuid = Guid.NewGuid

            objFile.FileName = FileName
            objFile.Extension = Extension
            objFile.Size = CType(Size, Integer)
            objFile.Width = Width
            objFile.Height = Height
            objFile.ContentType = ContentType
            objFile.Folder = FileSystemUtils.FormatFolderPath(DestinationFolder)
            objFile.FolderId = FolderID

            UpdateFile(objFile)
        End Sub

#End Region

    End Class

End Namespace
