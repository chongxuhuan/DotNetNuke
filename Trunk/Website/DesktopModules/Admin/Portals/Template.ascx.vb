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
Imports System.Xml
Imports DotNetNuke
Imports System.IO
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports ICSharpCode.SharpZipLib.Zip
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.Modules.Admin.Portals

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Template PortalModuleBase is used to export a Portal as a Template
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Template
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes all Files
        ''' </summary>
        ''' <param name="objportal">Portal to serialize</param>
        ''' <param name="folderPath">The folder containing the files</param>
        ''' <remarks>
        ''' The serialization uses the xml attributes defined in FileInfo class.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	Created
        '''     [cnurse]    05/20/2004  Extracted adding of file to zip to new FileSystemUtils method
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeFiles(ByVal writer As XmlWriter, ByVal objportal As PortalInfo, ByVal folderPath As String, ByRef zipFile As ZipOutputStream)
            Dim objFolders As New FolderController
            Dim objFolder As FolderInfo = objFolders.GetFolder(objportal.PortalID, folderPath, False)

            writer.WriteStartElement("files")

            For Each objFile As Services.FileSystem.FileInfo In FileSystemUtils.GetFilesByFolder(objportal.PortalID, objFolder.FolderID)
                ' verify that the file exists on the file system
                Dim filePath As String = objportal.HomeDirectoryMapPath & folderPath & objFile.FileName
                If File.Exists(filePath) Then
                    writer.WriteStartElement("file")

                    writer.WriteElementString("contenttype", objFile.ContentType)
                    writer.WriteElementString("extension", objFile.Extension)
                    writer.WriteElementString("filename", objFile.FileName)
                    writer.WriteElementString("height", objFile.Height.ToString())
                    writer.WriteElementString("size", objFile.Size.ToString())
                    writer.WriteElementString("width", objFile.Width.ToString())

                    writer.WriteEndElement()

                    FileSystemUtils.AddToZip(zipFile, filePath, objFile.FileName, folderPath)
                End If
            Next

            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes all Folders including Permissions
        ''' </summary>
        ''' <param name="objportal">Portal to serialize</param>
        ''' <remarks>
        ''' The serialization uses the xml attributes defined in FolderInfo class.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeFolders(ByVal writer As XmlWriter, ByVal objportal As PortalInfo, ByRef zipFile As ZipOutputStream)
            ' Sync db and filesystem before exporting so all required files are found
            FileSystemUtils.Synchronize(objportal.PortalID, objportal.AdministratorRoleId, objportal.HomeDirectoryMapPath, PortalSettings.HideFoldersEnabled)

            writer.WriteStartElement("folders")

            For Each folder As FolderInfo In New FolderController().GetFoldersSorted(objportal.PortalID).Values
                writer.WriteStartElement("folder")

                writer.WriteElementString("folderpath", folder.FolderPath)
                writer.WriteElementString("storagelocation", folder.StorageLocation.ToString())

                'Serialize Folder Permissions
                SerializeFolderPermissions(writer, objportal, folder.FolderPath)

                ' Serialize files
                SerializeFiles(writer, objportal, folder.FolderPath, zipFile)

                writer.WriteEndElement()
            Next

            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes all Folder Permissions
        ''' </summary>
        ''' <param name="objportal">Portal to serialize</param>
        ''' <param name="folderPath">The folder containing the files</param>
        ''' <remarks>
        ''' The serialization uses the xml attributes defined in FolderInfo class.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeFolderPermissions(ByVal writer As XmlWriter, ByVal objportal As PortalInfo, ByVal folderPath As String)
            Dim permissions As FolderPermissionCollection = FolderPermissionController.GetFolderPermissionsCollectionByFolder(objportal.PortalID, folderPath)

            writer.WriteStartElement("folderpermissions")

            For Each permission As FolderPermissionInfo In permissions
                writer.WriteStartElement("permission")

                writer.WriteElementString("permissioncode", permission.PermissionCode)
                writer.WriteElementString("permissionkey", permission.PermissionKey)
                writer.WriteElementString("rolename", permission.RoleName)
                writer.WriteElementString("allowaccess", permission.AllowAccess.ToString().ToLowerInvariant())

                writer.WriteEndElement()
            Next

            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes all Profile Definitions
        ''' </summary>
        ''' <param name="objportal">Portal to serialize</param>
        ''' <remarks>
        ''' The serialization uses the xml attributes defined in ProfilePropertyDefinition class.
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeProfileDefinitions(ByVal writer As XmlWriter, ByVal objportal As PortalInfo)
            Dim objListController As New ListController
            Dim objList As ListEntryInfo

            writer.WriteStartElement("profiledefinitions")

            For Each objProfileProperty As ProfilePropertyDefinition In ProfileController.GetPropertyDefinitionsByPortal(objportal.PortalID, False)
                writer.WriteStartElement("profiledefinition")

                writer.WriteElementString("propertycategory", objProfileProperty.PropertyCategory)
                writer.WriteElementString("propertyname", objProfileProperty.PropertyName)

                objList = objListController.GetListEntryInfo(objProfileProperty.DataType)
                If objList Is Nothing Then
                    writer.WriteElementString("datatype", "Unknown")
                Else
                    writer.WriteElementString("datatype", objList.Value)
                End If

                writer.WriteElementString("length", objProfileProperty.Length.ToString())
                writer.WriteElementString("defaultvisibility", Convert.ToInt32(objProfileProperty.DefaultVisibility).ToString())

                writer.WriteEndElement()
            Next

            writer.WriteEndElement()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes all portal Tabs
        ''' </summary>
        ''' <param name="objportal">Portal to serialize</param>
        ''' <remarks>
        ''' Only portal tabs will be exported to the template, Admin tabs are not exported.
        ''' On each tab, all modules will also be exported.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	23/09/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeTabs(ByVal writer As XmlWriter, ByVal objportal As PortalInfo)
            Dim nodeTab As XmlNode
            Dim xmlTab As XmlDocument
            Dim objtab As TabInfo
            Dim objtabs As New TabController

            'supporting object to build the tab hierarchy
            Dim hTabs As New Hashtable

            writer.WriteStartElement("tabs")

            For Each objtab In objtabs.GetTabsByPortal(objportal.PortalID).AsList
                'if not deleted
                If Not objtab.IsDeleted Then
                    'Serialize the Tab
                    xmlTab = New XmlDocument()
                    nodeTab = TabController.SerializeTab(xmlTab, hTabs, objtab, objportal, chkContent.Checked)

                    nodeTab.WriteTo(writer)
                End If
            Next

            writer.WriteEndElement()
        End Sub

#End Region

#Region "EventHandlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	23/09/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    Dim objportals As New PortalController
                    cboPortals.DataTextField = "PortalName"
                    cboPortals.DataValueField = "PortalId"
                    cboPortals.DataSource = objportals.GetPortals()
                    cboPortals.DataBind()
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	09/02/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Exports the selected portal
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Template will be saved in Portals\_default folder.
        ''' An extension of .template will be added to filename if not entered
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	23/09/2004	Created
        ''' 	[cnurse]	11/08/2004	Addition of files to template
        '''  	[aprasad]	1/17/2011	New setting AutoAddPortalAlias
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExport.Click
            Try
                Dim resourcesFile As ZipOutputStream
                Dim sb As New StringBuilder()
                Dim settings As New XmlWriterSettings()
                settings.ConformanceLevel = ConformanceLevel.Fragment
                settings.OmitXmlDeclaration = True
                settings.Indent = True

                If Not Page.IsValid Then
                    Exit Sub
                End If

                Dim filename As String
                filename = Common.Globals.HostMapPath & txtTemplateName.Text
                If Not filename.EndsWith(".template") Then
                    filename += ".template"
                End If
                Dim writer As XmlWriter = XmlWriter.Create(filename, settings)

                writer.WriteStartElement("portal")
                writer.WriteAttributeString("version", "5.0")

                'Add template description
                writer.WriteElementString("description", Server.HtmlEncode(txtDescription.Text))

                'Serialize portal settings
                Dim objportal As PortalInfo
                Dim objportals As New PortalController
                objportal = objportals.GetPortal(Convert.ToInt32(cboPortals.SelectedValue))

                writer.WriteStartElement("settings")

                writer.WriteElementString("logofile", objportal.LogoFile)
                writer.WriteElementString("footertext", objportal.FooterText)
                writer.WriteElementString("userregistration", objportal.UserRegistration.ToString())
                writer.WriteElementString("banneradvertising", objportal.BannerAdvertising.ToString())
                writer.WriteElementString("defaultlanguage", objportal.DefaultLanguage)

                Dim settingsDictionary As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(objportal.PortalID)

                Dim setting As String = ""
                settingsDictionary.TryGetValue("DefaultPortalSkin", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("skinsrc", setting)
                settingsDictionary.TryGetValue("DefaultAdminSkin", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("skinsrcadmin", setting)
                settingsDictionary.TryGetValue("DefaultPortalContainer", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("containersrc", setting)
                settingsDictionary.TryGetValue("DefaultAdminContainer", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("containersrcadmin", setting)
                settingsDictionary.TryGetValue("EnableSkinWidgets", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("enableskinwidgets", setting)
                settingsDictionary.TryGetValue("PortalAliasMapping", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("portalaliasmapping", setting)
                settingsDictionary.TryGetValue("TimeZone", setting)
                If Not String.IsNullOrEmpty(setting) Then writer.WriteElementString("timezone", setting)

                writer.WriteElementString("hostspace", objportal.HostSpace.ToString())
                writer.WriteElementString("userquota", objportal.UserQuota.ToString())
                writer.WriteElementString("pagequota", objportal.PageQuota.ToString())

                'End Portal Settings
                writer.WriteEndElement()

                ' Serialize Profile Definitions
                SerializeProfileDefinitions(writer, objportal)

                ' Serialize Portal Desktop Modules
                DesktopModuleController.SerializePortalDesktopModules(writer, objportal.PortalID)

                'Serialize Roles
                RoleController.SerializeRoleGroups(writer, objportal.PortalID)

                ' Serialize tabs
                SerializeTabs(writer, objportal)

                If chkContent.Checked Then

                    'Create Zip File to hold files
                    resourcesFile = New ZipOutputStream(File.Create(filename & ".resources"))
                    resourcesFile.SetLevel(6)

                    ' Serialize folders (while adding files to zip file)
                    SerializeFolders(writer, objportal, resourcesFile)

                    'Finish and Close Zip file
                    resourcesFile.Finish()
                    resourcesFile.Close()
                End If

                writer.WriteEndElement()

                writer.Close()

                lblMessage.Text = String.Format(Services.Localization.Localization.GetString("ExportedMessage", Me.LocalResourceFile), filename)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace
