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

#Region "Imports Statements"

Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.Web
Imports DotNetNuke.Entities.Host

#End Region

Namespace DotNetNuke.Services.FileSystem

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : FolderController
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Business Class that provides access to the Database for the functions within the calling classes
    ''' Instantiates the instance of the DataProvider and returns the object, if any
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Public Class FolderController

#Region "Enumerators"

        Enum StorageLocationTypes
            InsecureFileSystem = 0
            SecureFileSystem = 1
            DatabaseSecure = 2
        End Enum

#End Region

#Region "Private methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetFoldersCallBack gets a Dictionary of Folders by Portal from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	07/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetFoldersSortedCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return CBO.FillSortedList(Of String, FolderInfo)("FolderPath", DataProvider.Instance().GetFoldersByPortal(portalID))
        End Function

        Private Sub UpdateParentFolder(ByVal PortalID As Integer, ByVal FolderPath As String)

            If FolderPath.Length > 0 Then
                Dim parentFolderPath As String = FolderPath.Substring(0, FolderPath.Substring(0, FolderPath.Length - 1).LastIndexOf("/") + 1)
                Dim objFolder As FolderInfo = GetFolder(PortalID, parentFolderPath, False)
                If Not objFolder Is Nothing Then
                    UpdateFolder(objFolder)
                End If
            End If

        End Sub

#End Region

#Region "Public Methods"

        Public Function AddFolder(ByVal PortalID As Integer, ByVal FolderPath As String) As Integer
            Return AddFolder(PortalID, FolderPath, StorageLocationTypes.InsecureFileSystem, False, False)
        End Function

        Public Function AddFolder(ByVal PortalID As Integer, ByVal FolderPath As String, ByVal StorageLocation As Integer, ByVal IsProtected As Boolean, ByVal IsCached As Boolean) As Integer
            Return AddFolder(PortalID, FolderPath, StorageLocation, IsProtected, IsCached, Null.NullDate)
        End Function

        Public Function AddFolder(ByVal PortalID As Integer, ByVal FolderPath As String, ByVal StorageLocation As Integer, ByVal IsProtected As Boolean, ByVal IsCached As Boolean, ByVal LastUpdated As Date) As Integer
            Dim FolderId As Integer

            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath)

            Dim folder As FolderInfo = GetFolder(PortalID, FolderPath, True)
            If folder Is Nothing Then
                FolderId = DataProvider.Instance().AddFolder(PortalID, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated, UserController.GetCurrentUserInfo.UserID)
                'Refetch folder for logging
                folder = GetFolder(PortalID, FolderPath, True)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(folder, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.FOLDER_CREATED)
                UpdateParentFolder(PortalID, FolderPath)
            Else
                FolderId = folder.FolderID
                DataProvider.Instance().UpdateFolder(PortalID, FolderId, FolderPath, StorageLocation, IsProtected, IsCached, LastUpdated, UserController.GetCurrentUserInfo.UserID)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog("FolderPath", FolderPath, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.FOLDER_UPDATED)
            End If

            'Invalidate Cache
            DataCache.ClearFolderCache(PortalID)

            Return FolderId
        End Function

        Public Sub DeleteFolder(ByVal PortalID As Integer, ByVal FolderPath As String)
            DataProvider.Instance().DeleteFolder(PortalID, FileSystemUtils.FormatFolderPath(FolderPath))
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("FolderPath", FolderPath, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.FOLDER_DELETED)
            UpdateParentFolder(PortalID, FolderPath)

            'Invalidate Cache
            DataCache.ClearFolderCache(PortalID)
        End Sub

        Public Function GetFolder(ByVal PortalID As Integer, ByVal FolderPath As String, ByVal ignoreCache As Boolean) As FolderInfo
            FolderPath = FileSystemUtils.FormatFolderPath(FolderPath)

            Dim folder As FolderInfo = Nothing
            Dim bFound As Boolean = False
            If Not ignoreCache Then
                Dim dicFolders As SortedList(Of String, FolderInfo)
                'First try the cache
                dicFolders = GetFoldersSorted(PortalID)
                bFound = dicFolders.TryGetValue(FolderPath, folder)
            End If

            If ignoreCache Or Not bFound Then
                folder = CBO.FillObject(Of FolderInfo)(DataProvider.Instance().GetFolder(PortalID, FolderPath))
            End If
            Return folder
        End Function

        Public Function GetFolderInfo(ByVal PortalID As Integer, ByVal FolderID As Integer) As FolderInfo
            Return CBO.FillObject(Of FolderInfo)(DataProvider.Instance().GetFolder(PortalID, FolderID))
        End Function

        Public Function GetFoldersSorted(ByVal PortalID As Integer) As SortedList(Of String, FolderInfo)
            Dim cacheKey As String = String.Format(DataCache.FolderCacheKey, PortalID.ToString())
            Return CBO.GetCachedObject(Of SortedList(Of String, FolderInfo))(New CacheItemArgs(cacheKey, DataCache.FolderCacheTimeOut, DataCache.FolderCachePriority, PortalID), _
                                                                                        AddressOf GetFoldersSortedCallBack)
        End Function

        Public Function GetMappedDirectory(ByVal VirtualDirectory As String) As String
            Dim MappedDir As String = Convert.ToString(DataCache.GetCache("DirMap:" + VirtualDirectory))
            Try
                If MappedDir = "" AndAlso Not HttpContext.Current Is Nothing Then
                    MappedDir = FileSystemUtils.AddTrailingSlash(HttpContext.Current.Server.MapPath(VirtualDirectory))
                    DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir)
                End If
            Catch exc As Exception
                LogException(exc)
            End Try
            Return MappedDir
        End Function

        Public Sub SetMappedDirectory(ByVal VirtualDirectory As String)
            Try
                Dim MappedDir As String = FileSystemUtils.AddTrailingSlash(HttpContext.Current.Server.MapPath(VirtualDirectory))
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir)
            Catch exc As Exception
                LogException(exc)
            End Try
        End Sub

        Public Sub SetMappedDirectory(ByVal VirtualDirectory As String, ByVal context As HttpContext)
            Try
                Dim MappedDir As String = FileSystemUtils.AddTrailingSlash(context.Server.MapPath(VirtualDirectory))
                DataCache.SetCache("DirMap:" + VirtualDirectory, MappedDir)
            Catch exc As Exception
                LogException(exc)
            End Try
        End Sub

        Public Sub SetMappedDirectory(ByVal portalInfo As PortalInfo, ByVal context As HttpContext)
            Try
                Dim VirtualDirectory As String = Common.Globals.ApplicationPath + "/" + portalInfo.HomeDirectory + "/"
                SetMappedDirectory(VirtualDirectory, context)

            Catch exc As Exception
                LogException(exc)
            End Try
        End Sub

        Public Sub UpdateFolder(ByVal objFolderInfo As FolderInfo)
            DataProvider.Instance().UpdateFolder(objFolderInfo.PortalID, objFolderInfo.FolderID, FileSystemUtils.FormatFolderPath(objFolderInfo.FolderPath), objFolderInfo.StorageLocation, objFolderInfo.IsProtected, objFolderInfo.IsCached, objFolderInfo.LastUpdated, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objFolderInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.FOLDER_UPDATED)

            'Invalidate Cache
            DataCache.ClearFolderCache(objFolderInfo.PortalID)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.0.  It has been replaced by GetFolderInfo(ByVal PortalID As Integer, ByVal FolderID As Integer) As FolderInfo ")> _
        Public Function GetFolder(ByVal PortalID As Integer, ByVal FolderID As Integer) As ArrayList
            Dim arrFolders As New ArrayList
            Dim folder As FolderInfo = GetFolderInfo(PortalID, FolderID)
            If Not folder Is Nothing Then
                arrFolders.Add(folder)
            End If
            Return arrFolders
        End Function

        <Obsolete("Deprecated in DNN 5.0.  It has been replaced by GetFolderInfo(ByVal PortalID As Integer, ByVal FolderID As Integer, ByVal ignoreCache As Boolean) ")> _
        Public Function GetFolder(ByVal PortalID As Integer, ByVal FolderPath As String) As FolderInfo
            Return GetFolder(PortalID, FolderPath, True)
        End Function

        <Obsolete("Deprecated in DNN 5.1.1.  It has been replaced by GetFolders(ByVal PortalID As Integer) As SortedList ")> _
        Public Function GetFolders(ByVal PortalID As Integer) As Dictionary(Of String, FolderInfo)
            Return New Dictionary(Of String, FolderInfo)(GetFoldersSorted(PortalID))
        End Function

        <Obsolete("Deprecated in DNN 5.0.  It has been replaced by GetFolders(ByVal PortalID As Integer) ")> _
        Public Function GetFoldersByPortal(ByVal PortalID As Integer) As ArrayList
            Dim arrFolders As New ArrayList
            For Each folderPair As KeyValuePair(Of String, FolderInfo) In GetFoldersSorted(PortalID)
                arrFolders.Add(folderPair.Value)
            Next
            Return arrFolders
        End Function

#End Region

    End Class

End Namespace
