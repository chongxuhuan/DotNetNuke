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
Imports System.IO
Imports DotNetNuke.Entities.Controllers
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports System.Text.RegularExpressions

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : SkinController
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handles the Business Control Layer for Skins
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[willhsc]	3/3/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinController

#Region "Public Shared Properties"

        Public Shared ReadOnly Property RootSkin() As String
            Get
                Return "Skins"
            End Get
        End Property

        Public Shared ReadOnly Property RootContainer() As String
            Get
                Return "Containers"
            End Get
        End Property

#End Region

#Region "Public Shared Methods"

        Public Shared Function AddSkin(ByVal skinPackageID As Integer, ByVal skinSrc As String) As Integer
            Return DataProvider.Instance().AddSkin(skinPackageID, skinSrc)
        End Function

        Public Shared Function AddSkinPackage(ByVal skinPackage As SkinPackageInfo) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_CREATED)
            Return DataProvider.Instance().AddSkinPackage(skinPackage.PackageID, skinPackage.PortalID, skinPackage.SkinName, skinPackage.SkinType, UserController.GetCurrentUserInfo.UserID)
        End Function

        Public Shared Function CanDeleteSkin(ByVal strFolderPath As String, ByVal portalHomeDirMapPath As String) As Boolean
            Dim strSkinType As String
            Dim strSkinFolder As String
            Dim canDelete As Boolean = True
            If strFolderPath.ToLower.IndexOf(Common.Globals.HostMapPath.ToLower) <> -1 Then
                strSkinType = "G"
                strSkinFolder = strFolderPath.ToLower.Replace(Common.Globals.HostMapPath.ToLower, "").Replace("\", "/")
            Else
                strSkinType = "L"
                strSkinFolder = strFolderPath.ToLower.Replace(portalHomeDirMapPath.ToLower, "").Replace("\", "/")
            End If

            Dim portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()

            Dim skin As String = "[" + strSkinType.ToLowerInvariant + "]" + strSkinFolder.ToLowerInvariant
            If strSkinFolder.ToLowerInvariant.Contains("skins") Then
                If Host.DefaultAdminSkin.ToLowerInvariant.StartsWith(skin) OrElse _
                        Host.DefaultPortalSkin.ToLowerInvariant.StartsWith(skin) OrElse _
                        portalSettings.DefaultAdminSkin.ToLowerInvariant.StartsWith(skin) OrElse _
                        portalSettings.DefaultPortalSkin.ToLowerInvariant.StartsWith(skin) Then
                    canDelete = False
                End If
            Else
                If Host.DefaultAdminContainer.ToLowerInvariant.StartsWith(skin) OrElse _
                        Host.DefaultPortalContainer.ToLowerInvariant.StartsWith(skin) OrElse _
                        portalSettings.DefaultAdminContainer.ToLowerInvariant.StartsWith(skin) OrElse _
                        portalSettings.DefaultPortalContainer.ToLowerInvariant.StartsWith(skin) Then
                    canDelete = False
                End If
            End If

            If canDelete Then
                'Check if used for Tabs or Modules
                canDelete = DataProvider.Instance().CanDeleteSkin(strSkinType, strSkinFolder)
            End If
            Return canDelete
        End Function

        Public Shared Sub DeleteSkin(ByVal skinID As Integer)
            DataProvider.Instance().DeleteSkin(skinID)
        End Sub

        Public Shared Sub DeleteSkinPackage(ByVal skinPackage As SkinPackageInfo)
            DataProvider.Instance().DeleteSkinPackage(skinPackage.SkinPackageID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_DELETED)
        End Sub

        Public Shared Function FormatMessage(ByVal Title As String, ByVal Body As String, ByVal Level As Integer, ByVal IsError As Boolean) As String
            Dim Message As String = Title

            If IsError Then
                Message = "<font class=""NormalRed"">" & Title & "</font>"
            End If

            Select Case Level
                Case -1
                    Message = "<hr><br><b>" & Message & "</b>"
                Case 0
                    Message = "<br><br><b>" & Message & "</b>"
                Case 1
                    Message = "<br><b>" & Message & "</b>"
                Case Else
                    Message = "<br><li>" & Message
            End Select

            Return Message & ": " & Body & vbCrLf

        End Function

        Public Shared Function FormatSkinPath(ByVal SkinSrc As String) As String
            Dim strSkinSrc As String = SkinSrc

            If strSkinSrc <> "" Then
                strSkinSrc = Left(strSkinSrc, InStrRev(strSkinSrc, "/"))
            End If

            Return strSkinSrc
        End Function

        Public Shared Function FormatSkinSrc(ByVal SkinSrc As String, ByVal PortalSettings As PortalSettings) As String
            Dim strSkinSrc As String = SkinSrc

            If strSkinSrc <> "" Then
                Select Case strSkinSrc.ToLowerInvariant.Substring(0, 3)
                    Case "[g]"
                        strSkinSrc = Regex.Replace(strSkinSrc, "\[g]", Common.Globals.HostPath, RegexOptions.IgnoreCase)
                    Case "[l]"
                        strSkinSrc = Regex.Replace(strSkinSrc, "\[l]", PortalSettings.HomeDirectory, RegexOptions.IgnoreCase)
                End Select
            End If

            Return strSkinSrc
        End Function

        Public Shared Function GetDefaultAdminContainer() As String
            Dim defaultContainer As SkinDefaults = SkinDefaults.GetSkinDefaults(SkinDefaultType.ContainerInfo)
            Return "[G]" & SkinController.RootContainer & defaultContainer.Folder & defaultContainer.AdminDefaultName
        End Function

        Public Shared Function GetDefaultAdminSkin() As String
            Dim defaultSkin As SkinDefaults = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo)
            Return "[G]" & SkinController.RootSkin & defaultSkin.Folder & defaultSkin.AdminDefaultName
        End Function

        Public Shared Function GetDefaultPortalContainer() As String
            Dim defaultContainer As SkinDefaults = SkinDefaults.GetSkinDefaults(SkinDefaultType.ContainerInfo)
            Return "[G]" & SkinController.RootContainer & defaultContainer.Folder & defaultContainer.DefaultName
        End Function

        Public Shared Function GetDefaultPortalSkin() As String
            Dim defaultSkin As SkinDefaults = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo)
            Return "[G]" & SkinController.RootSkin & defaultSkin.Folder & defaultSkin.DefaultName
        End Function

        Public Shared Function GetSkinByPackageID(ByVal packageID As Integer) As SkinPackageInfo
            Return CBO.FillObject(Of SkinPackageInfo)(DataProvider.Instance().GetSkinByPackageID(packageID))
        End Function

        Public Shared Function GetSkinPackage(ByVal portalId As Integer, ByVal skinName As String, ByVal skinType As String) As SkinPackageInfo
            Return CBO.FillObject(Of SkinPackageInfo)(DataProvider.Instance().GetSkinPackage(portalId, skinName, skinType))
        End Function

        Public Shared Function GetSkins(ByVal portalInfo As PortalInfo, ByVal skingRoot As String) As List(Of KeyValuePair(Of String, String))
            Dim strRoot As String
            Dim strLastFolder As String
            Dim strFolder As String
            Dim strFile As String
            Dim Skins As New List(Of KeyValuePair(Of String, String))
            'If optHost.Checked Then
            ' load host skins
            strLastFolder = ""
            strRoot = Common.Globals.HostMapPath & skingRoot
            If Directory.Exists(strRoot) Then
                For Each strFolder In Directory.GetDirectories(strRoot)
                    If Not strFolder.EndsWith(glbHostSkinFolder) Then
                        For Each strFile In Directory.GetFiles(strFolder, "*.ascx")
                            strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                            If strLastFolder <> strFolder Then
                                strLastFolder = strFolder
                            End If


                            Skins.Add(New KeyValuePair(Of String, String)(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[G]" & skingRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))

                        Next
                    End If
                Next
            End If
            'End If

            'If optSite.Checked Then
            ' load portal skins
            strLastFolder = ""
            strRoot = portalInfo.HomeDirectoryMapPath & skingRoot
            If Directory.Exists(strRoot) Then
                For Each strFolder In Directory.GetDirectories(strRoot)
                    For Each strFile In Directory.GetFiles(strFolder, "*.ascx")
                        strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                        If strLastFolder <> strFolder Then
                            strLastFolder = strFolder
                        End If
                        Skins.Add(New KeyValuePair(Of String, String)(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)), "[L]" & skingRoot & "/" & strFolder & "/" & Path.GetFileName(strFile)))
                    Next
                Next
            End If
            'End If

            Return Skins
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' format skin name
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strSkinFolder">The Folder Name</param>
        ''' <param name="strSkinFile">The File Name without extension</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
            If strSkinFolder.ToLower = "_default" Then
                ' host folder
                Return strSkinFile
            Else ' portal folder
                Select Case strSkinFile.ToLower
                    Case "skin", "container", "default"
                        Return strSkinFolder
                    Case Else
                        Return strSkinFolder & " - " & strSkinFile
                End Select
            End If
        End Function

        ''' <summary>
        ''' Determines if a given skin is defined as a global skin
        ''' </summary>
        ''' <param name="SkinSrc">This is the app relative path and filename of the skin to be checked.</param>
        ''' <returns>True if the skin is located in the HostPath child directories.</returns>
        ''' <remarks>This function performs a quick check to detect the type of skin that is
        ''' passed as a parameter.  Using this method abstracts knowledge of the actual location
        ''' of skins in the file system.
        ''' </remarks>
        ''' <history>
        '''     [Joe Brinkman]	10/20/2007	Created
        ''' </history>
        Public Shared Function IsGlobalSkin(ByVal SkinSrc As String) As Boolean
            Return SkinSrc.Contains(Common.Globals.HostPath)
        End Function

        Public Shared Sub SetSkin(ByVal SkinRoot As String, ByVal PortalId As Integer, ByVal SkinType As UI.Skins.SkinType, ByVal SkinSrc As String)

            Select Case SkinRoot
                Case "Skins"
                    If SkinType = Skins.SkinType.Admin Then
                        If PortalId = Null.NullInteger Then
                            HostController.Instance.Update("DefaultAdminSkin", SkinSrc)
                        Else
                            PortalController.UpdatePortalSetting(PortalId, "DefaultAdminSkin", SkinSrc)
                        End If
                    Else
                        If PortalId = Null.NullInteger Then
                            HostController.Instance.Update("DefaultPortalSkin", SkinSrc)
                        Else
                            PortalController.UpdatePortalSetting(PortalId, "DefaultPortalSkin", SkinSrc)
                        End If
                    End If
                Case "Containers"
                    If SkinType = Skins.SkinType.Admin Then
                        If PortalId = Null.NullInteger Then
                            HostController.Instance.Update("DefaultAdminContainer", SkinSrc)
                        Else
                            PortalController.UpdatePortalSetting(PortalId, "DefaultAdminContainer", SkinSrc)
                        End If
                    Else
                        If PortalId = Null.NullInteger Then
                            HostController.Instance.Update("DefaultPortalContainer", SkinSrc)
                        Else
                            PortalController.UpdatePortalSetting(PortalId, "DefaultPortalContainer", SkinSrc)
                        End If
                    End If

            End Select
        End Sub

        Public Shared Sub UpdateSkin(ByVal skinID As Integer, ByVal skinSrc As String)
            DataProvider.Instance().UpdateSkin(skinID, skinSrc)
        End Sub

        Public Shared Sub UpdateSkinPackage(ByVal skinPackage As SkinPackageInfo)
            DataProvider.Instance().UpdateSkinPackage(skinPackage.SkinPackageID, skinPackage.PackageID, skinPackage.PortalID, skinPackage.SkinName, skinPackage.SkinType, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(skinPackage, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.SKINPACKAGE_UPDATED)
            For Each kvp As KeyValuePair(Of Integer, String) In skinPackage.Skins
                UpdateSkin(kvp.Key, kvp.Value)
            Next
        End Sub

        Public Shared Function UploadLegacySkin(ByVal RootPath As String, ByVal SkinRoot As String, ByVal SkinName As String, ByVal objInputStream As Stream) As String

            Dim objZipInputStream As New ZipInputStream(objInputStream)

            Dim objZipEntry As ZipEntry
            Dim strExtension As String
            Dim strFileName As String
            Dim objFileStream As FileStream
            Dim intSize As Integer = 2048
            Dim arrData(2048) As Byte
            Dim strMessage As String = ""
            Dim arrSkinFiles As New ArrayList

            'Localized Strings
            Dim ResourcePortalSettings As PortalSettings = GetPortalSettings()
            Dim BEGIN_MESSAGE As String = Localization.GetString("BeginZip", ResourcePortalSettings)
            Dim CREATE_DIR As String = Localization.GetString("CreateDir", ResourcePortalSettings)
            Dim WRITE_FILE As String = Localization.GetString("WriteFile", ResourcePortalSettings)
            Dim FILE_ERROR As String = Localization.GetString("FileError", ResourcePortalSettings)
            Dim END_MESSAGE As String = Localization.GetString("EndZip", ResourcePortalSettings)
            Dim FILE_RESTICTED As String = Localization.GetString("FileRestricted", ResourcePortalSettings)

            strMessage += FormatMessage(BEGIN_MESSAGE, SkinName, -1, False)

            objZipEntry = objZipInputStream.GetNextEntry
            While Not objZipEntry Is Nothing
                If Not objZipEntry.IsDirectory Then
                    ' validate file extension
                    strExtension = objZipEntry.Name.Substring(objZipEntry.Name.LastIndexOf(".") + 1)
                    If InStr(1, ",ASCX,HTM,HTML,CSS,SWF,RESX,XAML,JS," & Host.FileExtensions.ToUpper, "," & strExtension.ToUpper) <> 0 Then

                        ' process embedded zip files
                        Select Case objZipEntry.Name.ToLower
                            Case SkinController.RootSkin.ToLower & ".zip"
                                Dim objMemoryStream As New MemoryStream
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objMemoryStream.Write(arrData, 0, intSize)
                                    intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                End While
                                objMemoryStream.Seek(0, SeekOrigin.Begin)
                                strMessage += UploadLegacySkin(RootPath, SkinController.RootSkin, SkinName, CType(objMemoryStream, Stream))
                            Case SkinController.RootContainer.ToLower & ".zip"
                                Dim objMemoryStream As New MemoryStream
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objMemoryStream.Write(arrData, 0, intSize)
                                    intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                End While
                                objMemoryStream.Seek(0, SeekOrigin.Begin)
                                strMessage += UploadLegacySkin(RootPath, SkinController.RootContainer, SkinName, CType(objMemoryStream, Stream))
                            Case Else
                                strFileName = RootPath & SkinRoot & "\" & SkinName & "\" & objZipEntry.Name

                                ' create the directory if it does not exist
                                If Not Directory.Exists(Path.GetDirectoryName(strFileName)) Then
                                    strMessage += FormatMessage(CREATE_DIR, Path.GetDirectoryName(strFileName), 2, False)
                                    Directory.CreateDirectory(Path.GetDirectoryName(strFileName))
                                End If

                                ' remove the old file
                                If File.Exists(strFileName) Then
                                    File.SetAttributes(strFileName, FileAttributes.Normal)
                                    File.Delete(strFileName)
                                End If
                                ' create the new file
                                objFileStream = File.Create(strFileName)

                                ' unzip the file
                                strMessage += FormatMessage(WRITE_FILE, Path.GetFileName(strFileName), 2, False)
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                While intSize > 0
                                    objFileStream.Write(arrData, 0, intSize)
                                    intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                                End While
                                objFileStream.Close()

                                ' save the skin file
                                Select Case Path.GetExtension(strFileName)
                                    Case ".htm", ".html", ".ascx", ".css"
                                        If strFileName.ToLower.IndexOf(glbAboutPage.ToLower) < 0 Then
                                            arrSkinFiles.Add(strFileName)
                                        End If
                                End Select
                        End Select
                    Else
                        strMessage += FormatMessage(FILE_ERROR, String.Format(FILE_RESTICTED, objZipEntry.Name, Replace(Host.FileExtensions.ToString, ",", ", *.")), 2, True)
                    End If
                End If
                objZipEntry = objZipInputStream.GetNextEntry
            End While
            strMessage += FormatMessage(END_MESSAGE, SkinName & ".zip", 1, False)
            objZipInputStream.Close()

            ' process the list of skin files
            Dim NewSkin As New UI.Skins.SkinFileProcessor(RootPath, SkinRoot, SkinName)
            strMessage += NewSkin.ProcessList(arrSkinFiles, SkinParser.Portable)

            ' log installation event
            Try
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Install Skin:", SkinName))
                Dim arrMessage As Array = Split(strMessage, "<br>")
                Dim strRow As String
                For Each strRow In arrMessage
                    objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Info:", HtmlUtils.StripTags(strRow, True)))
                Next
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(objEventLogInfo)
            Catch ex As Exception
                ' error
            End Try

            Return strMessage
        End Function

#End Region

        'Public Shared Function GetSkin(ByVal SkinRoot As String, ByVal PortalId As Integer, ByVal SkinType As UI.Skins.SkinType) As UI.Skins.SkinInfo
        '    Dim objSkin As SkinInfo = Nothing
        '    For Each skin As SkinInfo In GetSkins(PortalId)
        '        If skin.SkinRoot = SkinRoot And skin.SkinType = SkinType Then
        '            objSkin = skin
        '            Exit For
        '        End If
        '    Next
        '    Return objSkin
        'End Function

        'Public Shared Function GetSkins(ByVal PortalId As Integer) As ArrayList
        '    Dim arrSkins As ArrayList = Nothing

        '    ' data caching settings
        '    Dim intCacheTimeout As Integer
        '    ' calculate the cache settings based on the performance setting
        '    intCacheTimeout = 20 * Convert.ToInt32(Common.Globals.PerformanceSetting)

        '    arrSkins = CType(DataCache.GetCache("GetSkins" & PortalId.ToString), ArrayList)
        '    If arrSkins Is Nothing Then
        '        arrSkins = CBO.FillCollection(DataProvider.Instance().GetSkins(PortalId), GetType(SkinInfo))

        '        If intCacheTimeout <> 0 Then
        '            DataCache.SetCache("GetSkins" & PortalId.ToString, arrSkins, TimeSpan.FromMinutes(intCacheTimeout), False)
        '        End If
        '    End If
        '    Return arrSkins
        'End Function

#Region "Obsolete"

        <Obsolete("In DotNetNuke 5.0, the Skins are uploaded by using the new Installer")> _
        Public Shared Function UploadSkin(ByVal RootPath As String, ByVal SkinRoot As String, ByVal SkinName As String, ByVal Path As String) As String
            Dim strMessage As String = ""
            Dim objFileStream As FileStream
            objFileStream = New FileStream(Path, FileMode.Open, FileAccess.Read)

            strMessage = UploadLegacySkin(RootPath, SkinRoot, SkinName, CType(objFileStream, Stream))

            objFileStream.Close()
            Return strMessage
        End Function

        <Obsolete("In DotNetNuke 5.0, the Skins are uploaded by using the new Installer")> _
        Public Shared Function UploadSkin(ByVal RootPath As String, ByVal SkinRoot As String, ByVal SkinName As String, ByVal objInputStream As Stream) As String
            Return UploadLegacySkin(RootPath, SkinRoot, SkinName, objInputStream)
        End Function

#End Region

    End Class

End Namespace