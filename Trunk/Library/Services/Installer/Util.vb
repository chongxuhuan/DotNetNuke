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
Imports System.Xml.XPath

Imports DotNetNuke.Services.Installer.Log
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Entities.Host


Namespace DotNetNuke.Services.Installer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The InstallerBase class is a Base Class for all Installer
    '''	classes that need to use Localized Strings.  It provides these strings
    '''	as localized Constants.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/05/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Util

#Region "Public Constants"

        Public Const DEFAULT_MANIFESTEXT As String = ".manifest"

        Public Shared ASSEMBLY_Added As String = GetLocalizedString("ASSEMBLY_Added")
        Public Shared ASSEMBLY_InUse As String = GetLocalizedString("ASSEMBLY_InUse")
        Public Shared ASSEMBLY_Registered As String = GetLocalizedString("ASSEMBLY_Registered")
        Public Shared ASSEMBLY_UnRegistered As String = GetLocalizedString("ASSEMBLY_UnRegistered")
        Public Shared ASSEMBLY_Updated As String = GetLocalizedString("ASSEMBLY_Updated")

        Public Shared AUTHENTICATION_ReadSuccess As String = GetLocalizedString("AUTHENTICATION_ReadSuccess")
        Public Shared AUTHENTICATION_LoginSrcMissing As String = GetLocalizedString("AUTHENTICATION_LoginSrcMissing")
        Public Shared AUTHENTICATION_Registered As String = GetLocalizedString("AUTHENTICATION_Registered")
        Public Shared AUTHENTICATION_SettingsSrcMissing As String = GetLocalizedString("AUTHENTICATION_SettingsSrcMissing")
        Public Shared AUTHENTICATION_TypeMissing As String = GetLocalizedString("AUTHENTICATION_TypeMissing")
        Public Shared AUTHENTICATION_UnRegistered As String = GetLocalizedString("AUTHENTICATION_UnRegistered")

        Public Shared CLEANUP_Processing As String = GetLocalizedString("CLEANUP_Processing")
        Public Shared CLEANUP_ProcessComplete As String = GetLocalizedString("CLEANUP_ProcessComplete")

        Public Shared COMPONENT_Installed As String = GetLocalizedString("COMPONENT_Installed")
        Public Shared COMPONENT_RolledBack As String = GetLocalizedString("COMPONENT_RolledBack")
        Public Shared COMPONENT_RollingBack As String = GetLocalizedString("COMPONENT_RollingBack")
        Public Shared COMPONENT_UnInstalled As String = GetLocalizedString("COMPONENT_UnInstalled")

        Public Shared CONFIG_Committed As String = GetLocalizedString("CONFIG_Committed")
        Public Shared CONFIG_RolledBack As String = GetLocalizedString("CONFIG_RolledBack")
        Public Shared CONFIG_Updated As String = GetLocalizedString("CONFIG_Updated")

        Public Shared DASHBOARD_ReadSuccess As String = GetLocalizedString("DASHBOARD_ReadSuccess")
        Public Shared DASHBOARD_SrcMissing As String = GetLocalizedString("DASHBOARD_SrcMissing")
        Public Shared DASHBOARD_Registered As String = GetLocalizedString("DASHBOARD_Registered")
        Public Shared DASHBOARD_KeyMissing As String = GetLocalizedString("DASHBOARD_KeyMissing")
        Public Shared DASHBOARD_LocalResourcesMissing As String = GetLocalizedString("DASHBOARD_LocalResourcesMissing")
        Public Shared DASHBOARD_UnRegistered As String = GetLocalizedString("DASHBOARD_UnRegistered")

        Public Shared DNN_Reading As String = GetLocalizedString("DNN_Reading")
        Public Shared DNN_ReadingComponent As String = GetLocalizedString("DNN_ReadingComponent")
        Public Shared DNN_ReadingPackage As String = GetLocalizedString("DNN_ReadingPackage")
        Public Shared DNN_Success As String = GetLocalizedString("DNN_Success")

        Public Shared EVENTMESSAGE_CommandMissing As String = GetLocalizedString("EVENTMESSAGE_CommandMissing")
        Public Shared EVENTMESSAGE_TypeMissing As String = GetLocalizedString("EVENTMESSAGE_TypeMissing")

        Public Shared EXCEPTION As String = GetLocalizedString("EXCEPTION")
        Public Shared EXCEPTION_NameMissing As String = GetLocalizedString("EXCEPTION_NameMissing")
        Public Shared EXCEPTION_TypeMissing As String = GetLocalizedString("EXCEPTION_TypeMissing")
        Public Shared EXCEPTION_VersionMissing As String = GetLocalizedString("EXCEPTION_VersionMissing")
        Public Shared EXCEPTION_FileLoad As String = GetLocalizedString("EXCEPTION_FileLoad")
        Public Shared EXCEPTION_FileRead As String = GetLocalizedString("EXCEPTION_FileRead")
        Public Shared EXCEPTION_InstallerCreate As String = GetLocalizedString("EXCEPTION_InstallerCreate")
        Public Shared EXCEPTION_MissingDnn As String = GetLocalizedString("EXCEPTION_MissingDnn")
        Public Shared EXCEPTION_MultipleDnn As String = GetLocalizedString("EXCEPTION_MultipleDnn")
        Public Shared EXCEPTION_Type As String = GetLocalizedString("EXCEPTION_Type")

        Public Shared FILE_CreateBackup As String = GetLocalizedString("FILE_CreateBackup")
        Public Shared FILE_Created As String = GetLocalizedString("FILE_Created")
        Public Shared FILE_Deleted As String = GetLocalizedString("FILE_Deleted")
        Public Shared FILE_Found As String = GetLocalizedString("FILE_Found")
        Public Shared FILE_Loading As String = GetLocalizedString("FILE_Loading")
        Public Shared FILE_NotAllowed As String = GetLocalizedString("FILE_NotAllowed")
        Public Shared FILE_NotFound As String = GetLocalizedString("FILE_NotFound")
        Public Shared FILE_ReadSuccess As String = GetLocalizedString("FILE_ReadSuccess")
        Public Shared FILE_RestoreBackup As String = GetLocalizedString("FILE_RestoreBackup")
        Public Shared FILE_RolledBack As String = GetLocalizedString("FILE_RolledBack")

        Public Shared FILES_CreatedResources As String = GetLocalizedString("FILES_CreatedResources")
        Public Shared FILES_Expanding As String = GetLocalizedString("FILES_Expanding")
        Public Shared FILES_Loading As String = GetLocalizedString("FILES_Loading")
        Public Shared FILES_Reading As String = GetLocalizedString("FILES_Reading")
        Public Shared FILES_ReadingEnd As String = GetLocalizedString("FILES_ReadingEnd")

        Public Shared FOLDER_Created As String = GetLocalizedString("FOLDER_Created")
        Public Shared FOLDER_Deleted As String = GetLocalizedString("FOLDER_Deleted")
        Public Shared FOLDER_DeletedBackup As String = GetLocalizedString("FOLDER_DeletedBackup")

        Public Shared INSTALL_Compatibility As String = GetLocalizedString("INSTALL_Compatibility")
        Public Shared INSTALL_Dependencies As String = GetLocalizedString("INSTALL_Dependencies")
        Public Shared INSTALL_Aborted As String = GetLocalizedString("INSTALL_Aborted")
        Public Shared INSTALL_Failed As String = GetLocalizedString("INSTALL_Failed")
        Public Shared INSTALL_Committed As String = GetLocalizedString("INSTALL_Committed")
        Public Shared INSTALL_Namespace As String = GetLocalizedString("INSTALL_Namespace")
        Public Shared INSTALL_Package As String = GetLocalizedString("INSTALL_Package")
        Public Shared INSTALL_Permissions As String = GetLocalizedString("INSTALL_Permissions")
        Public Shared INSTALL_Start As String = GetLocalizedString("INSTALL_Start")
        Public Shared INSTALL_Success As String = GetLocalizedString("INSTALL_Success")
        Public Shared INSTALL_Version As String = GetLocalizedString("INSTALL_Version")

        Public Shared LANGUAGE_PortalsEnabled As String = GetLocalizedString("LANGUAGE_PortalsEnabled")
        Public Shared LANGUAGE_Registered As String = GetLocalizedString("LANGUAGE_Registered")
        Public Shared LANGUAGE_UnRegistered As String = GetLocalizedString("LANGUAGE_UnRegistered")

        Public Shared MODULE_ControlKeyMissing As String = GetLocalizedString("MODULE_ControlKeyMissing")
        Public Shared MODULE_ControlTypeMissing As String = GetLocalizedString("MODULE_ControlTypeMissing")
        Public Shared MODULE_FriendlyNameMissing As String = GetLocalizedString("MODULE_FriendlyNameMissing")
        Public Shared MODULE_ReadSuccess As String = GetLocalizedString("MODULE_ReadSuccess")
        Public Shared MODULE_Registered As String = GetLocalizedString("MODULE_Registered")
        Public Shared MODULE_UnRegistered As String = GetLocalizedString("MODULE_UnRegistered")

        Public Shared PACKAGE_NoLicense As String = GetLocalizedString("PACKAGE_NoLicense")
        Public Shared PACKAGE_NoReleaseNotes As String = GetLocalizedString("PACKAGE_NoReleaseNotes")
        Public Shared PACKAGE_UnRecognizable As String = GetLocalizedString("PACKAGE_UnRecognizable")

        Public Shared SECURITY_Installer As String = GetLocalizedString("SECURITY_Installer")
        Public Shared SECURITY_NotRegistered As String = GetLocalizedString("SECURITY_NotRegistered")

        Public Shared SKIN_BeginProcessing As String = GetLocalizedString("SKIN_BeginProcessing")
        Public Shared SKIN_Installed As String = GetLocalizedString("SKIN_Installed")
        Public Shared SKIN_EndProcessing As String = GetLocalizedString("SKIN_EndProcessing")
        Public Shared SKIN_Registered As String = GetLocalizedString("SKIN_Registered")
        Public Shared SKIN_UnRegistered As String = GetLocalizedString("SKIN_UnRegistered")

        Public Shared SQL_Begin As String = GetLocalizedString("SQL_Begin")
        Public Shared SQL_BeginFile As String = GetLocalizedString("SQL_BeginFile")
        Public Shared SQL_BeginUnInstall As String = GetLocalizedString("SQL_BeginUnInstall")
        Public Shared SQL_Committed As String = GetLocalizedString("SQL_Committed")
        Public Shared SQL_End As String = GetLocalizedString("SQL_End")
        Public Shared SQL_EndFile As String = GetLocalizedString("SQL_EndFile")
        Public Shared SQL_EndUnInstall As String = GetLocalizedString("SQL_EndUnInstall")
        Public Shared SQL_Exceptions As String = GetLocalizedString("SQL_Exceptions")
        Public Shared SQL_Executing As String = GetLocalizedString("SQL_Executing")
        Public Shared SQL_RolledBack As String = GetLocalizedString("SQL_RolledBack")

        Public Shared UNINSTALL_Start As String = GetLocalizedString("UNINSTALL_Start")
        Public Shared UNINSTALL_StartComp As String = GetLocalizedString("UNINSTALL_StartComp")
        Public Shared UNINSTALL_Failure As String = GetLocalizedString("UNINSTALL_Failure")
        Public Shared UNINSTALL_Success As String = GetLocalizedString("UNINSTALL_Success")
        Public Shared UNINSTALL_SuccessComp As String = GetLocalizedString("UNINSTALL_SuccessComp")
        Public Shared UNINSTALL_Warnings As String = GetLocalizedString("UNINSTALL_Warnings")
        Public Shared UNINSTALL_WarningsComp As String = GetLocalizedString("UNINSTALL_WarningsComp")

        Public Shared WRITER_AddFileToManifest As String = GetLocalizedString("WRITER_AddFileToManifest")
        Public Shared WRITER_CreateArchive As String = GetLocalizedString("WRITER_CreateArchive")
        Public Shared WRITER_CreatedManifest As String = GetLocalizedString("WRITER_CreatedManifest")
        Public Shared WRITER_CreatedPackage As String = GetLocalizedString("WRITER_CreatedPackage")
        Public Shared WRITER_CreatingManifest As String = GetLocalizedString("WRITER_CreatingManifest")
        Public Shared WRITER_CreatingPackage As String = GetLocalizedString("WRITER_CreatingPackage")
        Public Shared WRITER_SavedFile As String = GetLocalizedString("WRITER_SavedFile")
        Public Shared WRITER_SaveFileError As String = GetLocalizedString("WRITER_SaveFileError")

        Public Shared REGEX_Version As String = "\d{2}.\d{2}.\d{2}"

#End Region

#Region "Private Shared Methods"


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The StreamToStream method reads a source stream and wrtites it to a destination stream
        ''' </summary>
        ''' <param name="SourceStream">The Source Stream</param>
        ''' <param name="DestStream">The Destination Stream</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub StreamToStream(ByVal SourceStream As Stream, ByVal DestStream As Stream)
            Dim buf(1024) As Byte
            Dim count As Integer = 0
            Do
                ' Read the chunk from the source
                count = SourceStream.Read(buf, 0, 1024)

                ' Write the chunk to the destination
                DestStream.Write(buf, 0, count)
            Loop While count > 0
            DestStream.Flush()
        End Sub

        Private Shared Sub TryDeleteFolder(ByVal folder As DirectoryInfo, ByVal log As Logger)
            If folder.GetFiles().Length = 0 AndAlso folder.GetDirectories().Length = 0 Then
                folder.Delete()
                log.AddInfo(String.Format(Util.FOLDER_Deleted, folder.Name))

                TryDeleteFolder(folder.Parent, log)
            End If
        End Sub

        Private Shared Function ValidateNode(ByVal propValue As String, ByVal isRequired As Boolean, ByVal log As Logger, ByVal logmessage As String, ByVal defaultValue As String) As String
            If String.IsNullOrEmpty(propValue) Then
                If isRequired Then
                    'Log Error
                    log.AddFailure(logmessage)
                Else
                    'Use Default
                    propValue = defaultValue
                End If
            End If

            Return propValue
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The BackupFile method backs up a file to the backup folder
        ''' </summary>
        ''' <param name="installFile">The file to backup</param>
        ''' <param name="basePath">The basePath to the file</param>
        ''' <param name="log">A Logger to log the result</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub BackupFile(ByVal installFile As InstallFile, ByVal basePath As String, ByVal log As Logger)
            Dim fullFileName As String = Path.Combine(basePath, installFile.FullName)
            Dim backupFileName As String = Path.Combine(installFile.BackupPath, installFile.Name + ".config")

            ' create the backup folder if neccessary
            If Not Directory.Exists(installFile.BackupPath) Then
                Directory.CreateDirectory(installFile.BackupPath)
            End If

            'Copy file to backup location
            FileSystemUtils.CopyFile(fullFileName, backupFileName)

            log.AddInfo(String.Format(Util.FILE_CreateBackup, installFile.FullName))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CopyFile method copies a file from the temporary extract location.
        ''' </summary>
        ''' <param name="installFile">The file to copy</param>
        ''' <param name="basePath">The basePath to the file</param>
        ''' <param name="log">A Logger to log the result</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub CopyFile(ByVal installFile As InstallFile, ByVal basePath As String, ByVal log As Logger)
            Dim filePath As String = Path.Combine(basePath, installFile.Path)
            Dim fullFileName As String = Path.Combine(basePath, installFile.FullName)

            ' create the folder if neccessary
            If Not Directory.Exists(filePath) Then
                log.AddInfo(String.Format(Util.FOLDER_Created, filePath))
                Directory.CreateDirectory(filePath)
            End If

            'Copy file from temp location
            FileSystemUtils.CopyFile(installFile.TempFileName, fullFileName)

            log.AddInfo(String.Format(Util.FILE_Created, installFile.FullName))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteFile method deletes a file.
        ''' </summary>
        ''' <param name="installFile">The file to delete</param>
        ''' <param name="basePath">The basePath to the file</param>
        ''' <param name="log">A Logger to log the result</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteFile(ByVal installFile As InstallFile, ByVal basePath As String, ByVal log As Logger)
            DeleteFile(installFile.FullName, basePath, log)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteFile method deletes a file.
        ''' </summary>
        ''' <param name="fileName">The file to delete</param>
        ''' <param name="basePath">The basePath to the file</param>
        ''' <param name="log">A Logger to log the result</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteFile(ByVal fileName As String, ByVal basePath As String, ByVal log As Logger)
            Dim fullFileName As String = Path.Combine(basePath, fileName)

            If File.Exists(fullFileName) Then
                FileSystemUtils.DeleteFile(fullFileName)
                log.AddInfo(String.Format(Util.FILE_Deleted, fileName))

                Dim folderName As String = Path.GetDirectoryName(fullFileName)
                Dim folder As DirectoryInfo = New DirectoryInfo(folderName)
                TryDeleteFolder(folder, log)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetLocalizedString method provides a conveniencewrapper around the 
        ''' Localization of Strings
        ''' </summary>
        ''' <param name="key">The localization key</param>
        ''' <returns>The localized string</returns>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetLocalizedString(ByVal key As String) As String
            Return DotNetNuke.Services.Localization.Localization.GetString(key)
            'Return key
        End Function

        Public Shared Function IsFileValid(ByVal file As InstallFile, ByVal packageWhiteList As String) As Boolean
            Dim _IsFileValid As Boolean = Null.NullBoolean

            'Check the White List
            Dim systemWhiteList As String = Host.FileExtensions.ToLower

            'Check the White Lists
            Dim strExtension As String = file.Extension.ToLowerInvariant()
            If (strExtension = "dnn" OrElse _
                    systemWhiteList.Contains(strExtension) OrElse _
                    packageWhiteList.Contains(strExtension) OrElse _
                    (packageWhiteList.Contains("*dataprovider") AndAlso strExtension.EndsWith("dataprovider"))) Then
                'Install File is Valid
                _IsFileValid = True
            End If

            Return _IsFileValid
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The InstallURL method provides a utility method to build the correct url
        ''' to install a package (and return to where you came from)
        ''' </summary>
        ''' <param name="tabId">The id of the tab you are on</param>
        ''' <param name="type">The type of package you are installing</param>
        ''' <returns>The localized string</returns>
        ''' <history>
        ''' 	[cnurse]	07/26/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InstallURL(ByVal tabId As Integer, ByVal type As String) As String
            Dim params(1) As String
            params(0) = "rtab=" & tabId.ToString()
            If Not String.IsNullOrEmpty(type) Then
                params(1) = "ptype=" & type
            End If

            Return DotNetNuke.Common.Globals.NavigateURL(tabId, "Install", params)
        End Function

        Public Shared Function InstallURL(ByVal tabId As Integer, ByVal returnUrl As String, ByVal type As String) As String
            Dim params(2) As String
            params(0) = "rtab=" & tabId.ToString()
            params(1) = "returnUrl=" & returnUrl
            If Not String.IsNullOrEmpty(type) Then
                params(2) = "ptype=" & type
            End If

            Return DotNetNuke.Common.Globals.NavigateURL(tabId, "Install", params)
        End Function

        Public Shared Function UnInstallURL(ByVal tabId As Integer, ByVal packageId As Integer, ByVal returnUrl As String) As String
            Dim params(2) As String
            params(0) = "rtab=" & tabId.ToString()
            params(1) = "returnUrl=" & returnUrl
            params(2) = "packageId=" & packageId.ToString()

            Return DotNetNuke.Common.Globals.NavigateURL(tabId, "UnInstall", params)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The PackageWriterURL method provides a utility method to build the correct url
        ''' to create a package (and return to where you came from)
        ''' </summary>
        ''' <param name="context">The ModuleContext of the module</param>
        ''' <param name="packageId">The id of the package you are packaging</param>
        ''' <returns>The localized string</returns>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function PackageWriterURL(ByVal context As ModuleInstanceContext, ByVal packageId As Integer) As String
            Dim params(2) As String
            params(0) = "rtab=" & context.TabId.ToString()
            params(1) = "packageId=" & packageId.ToString()
            params(2) = "mid=" & context.ModuleId.ToString()

            Return DotNetNuke.Common.Globals.NavigateURL(context.TabId, "PackageWriter", params)
        End Function

#Region "ReadAttribute"

        Public Shared Function ReadAttribute(ByVal nav As XPathNavigator, ByVal attributeName As String) As String
            Return ValidateNode(nav.GetAttribute(attributeName, ""), False, Nothing, "", "")
        End Function

        Public Shared Function ReadAttribute(ByVal nav As XPathNavigator, ByVal attributeName As String, ByVal log As Logger, ByVal logmessage As String) As String
            Return ValidateNode(nav.GetAttribute(attributeName, ""), True, log, logmessage, "")
        End Function

        Public Shared Function ReadAttribute(ByVal nav As XPathNavigator, ByVal attributeName As String, ByVal isRequired As Boolean, ByVal log As Logger, ByVal logmessage As String, ByVal defaultValue As String) As String
            Return ValidateNode(nav.GetAttribute(attributeName, ""), isRequired, log, logmessage, defaultValue)
        End Function

#End Region

#Region "ReadElement"

        Public Shared Function ReadElement(ByVal nav As XPathNavigator, ByVal elementName As String) As String
            Return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), False, Nothing, "", "")
        End Function

        Public Shared Function ReadElement(ByVal nav As XPathNavigator, ByVal elementName As String, ByVal defaultValue As String) As String
            Return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), False, Nothing, "", defaultValue)
        End Function

        Public Shared Function ReadElement(ByVal nav As XPathNavigator, ByVal elementName As String, ByVal log As Logger, ByVal logmessage As String) As String
            Return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), True, log, logmessage, "")
        End Function

        Public Shared Function ReadElement(ByVal nav As XPathNavigator, ByVal elementName As String, ByVal isRequired As Boolean, ByVal log As Logger, ByVal logmessage As String, ByVal defaultValue As String) As String
            Return ValidateNode(XmlUtils.GetNodeValue(nav, elementName), isRequired, log, logmessage, defaultValue)
        End Function

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The RestoreFile method restores a file from the backup folder
        ''' </summary>
        ''' <param name="installFile">The file to restore</param>
        ''' <param name="basePath">The basePath to the file</param>
        ''' <param name="log">A Logger to log the result</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub RestoreFile(ByVal installFile As InstallFile, ByVal basePath As String, ByVal log As Logger)
            Dim fullFileName As String = Path.Combine(basePath, installFile.FullName)
            Dim backupFileName As String = Path.Combine(installFile.BackupPath, installFile.Name + ".config")

            'Copy File back over install file
            FileSystemUtils.CopyFile(backupFileName, fullFileName)

            log.AddInfo(String.Format(Util.FILE_RestoreBackup, installFile.FullName))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstallURL method provides a utility method to build the correct url
        ''' to uninstall a package (and return to where you came from)
        ''' </summary>
        ''' <param name="tabId">The id of the tab you are on</param>
        ''' <param name="packageId">The id of the package you are uninstalling</param>
        ''' <returns>The localized string</returns>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UnInstallURL(ByVal tabId As Integer, ByVal packageId As Integer) As String
            Dim params(1) As String
            params(0) = "rtab=" & tabId.ToString()
            params(1) = "packageId=" & packageId.ToString()

            Return DotNetNuke.Common.Globals.NavigateURL(tabId, "UnInstall", params)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The WriteStream reads a source stream and writes it to a destination file
        ''' </summary>
        ''' <param name="SourceStream">The Source Stream</param>
        ''' <param name="DestFileName">The Destination file</param>
        ''' <history>
        ''' 	[cnurse]	08/03/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub WriteStream(ByVal SourceStream As Stream, ByVal DestFileName As String)
            ' Delete the file
            FileSystemUtils.DeleteFile(DestFileName)

            Dim file As New FileInfo(DestFileName)
            If Not file.Directory.Exists Then
                file.Directory.Create()
            End If

            Dim fileStrm As Stream = file.Create()

            StreamToStream(SourceStream, fileStrm)

            ' Close the stream
            fileStrm.Close()
        End Sub

#End Region

    End Class

End Namespace

