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
Imports System.IO
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Modules
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.ModuleDefinitions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class CreateModuleDefinition
        Inherits ModuleUserControlBase

#Region "Private Members"

        Private _Package As PackageInfo = Nothing

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property IsAddMode() As Boolean
            Get
                Return (PackageID = Null.NullInteger)
            End Get
        End Property

        Protected ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing Then
                    If PackageID = Null.NullInteger Then
                        _Package = New PackageInfo()
                    Else
                        _Package = PackageController.GetPackage(PackageID)
                    End If
                End If
                Return _Package
            End Get
        End Property

        Public ReadOnly Property PackageID() As Integer
            Get
                Dim _PackageID As Integer = Null.NullInteger
                If Not (Request.QueryString("PackageID") Is Nothing) Then
                    _PackageID = Int32.Parse(Request.QueryString("PackageID"))
                End If
                Return _PackageID
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub AddFolder(ByVal parentFolder As String, ByVal newFolder As String)
            Dim parentFolderPath As String = ApplicationMapPath + "\DesktopModules"
            If Not String.IsNullOrEmpty(parentFolder) Then
                parentFolderPath += "\" + parentFolder
            End If

            Dim dinfo As New System.IO.DirectoryInfo(parentFolderPath)
            Dim dinfoNew As System.IO.DirectoryInfo
            dinfoNew = New System.IO.DirectoryInfo(parentFolderPath + "\" + newFolder)
            If Not dinfoNew.Exists Then
                dinfoNew = dinfo.CreateSubdirectory(newFolder)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="strRoot">The Root folder to parse from</param>
        ''' <param name="blnRecurse">True to iterate sub-folders</param>
        ''' <remarks>
        ''' Loads the cboSource control list with locations of controls.
        ''' </remarks>
        ''' <history>
        ''' 	[pgaryga]	18/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindControlList(ByVal strRoot As String, Optional ByVal blnRecurse As Boolean = True)
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strFile As String
            Dim arrFiles As String()

            If Directory.Exists(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot)) Then
                arrFolders = Directory.GetDirectories(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot))
                If blnRecurse Then
                    For Each strFolder In arrFolders
                        BindControlList(strFolder.Substring(Request.MapPath(Common.Globals.ApplicationPath).Length + 1).Replace("\"c, "/"c), blnRecurse)
                    Next
                End If
                arrFiles = Directory.GetFiles(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot), "*.ascx")
                For Each strFile In arrFiles
                    strFile = strRoot.Replace("\"c, "/"c) & "/" & Path.GetFileName(strFile)
                    'cboControl.Items.Add(New ListItem(strFile, strFile))
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="strRoot">The Root folder to parse from</param>
        ''' <param name="blnRecurse">True to iterate sub-folders</param>
        ''' <remarks>
        ''' Loads the cboSource control list with locations of controls.
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindManifestList(ByVal strRoot As String, Optional ByVal blnRecurse As Boolean = True)
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strFile As String
            Dim arrFiles As String()

            If Directory.Exists(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot)) Then
                arrFolders = Directory.GetDirectories(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot))
                If blnRecurse Then
                    For Each strFolder In arrFolders
                        BindManifestList(strFolder.Substring(Request.MapPath(Common.Globals.ApplicationPath).Length + 1).Replace("\"c, "/"c), blnRecurse)
                    Next
                End If
                arrFiles = Directory.GetFiles(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot), "*.dnn")
                For Each strFile In arrFiles
                    'cboManifest.Items.Add(New ListItem(Path.GetFileName(strFile), strFile))
                Next
            End If
        End Sub

        Private Function InvalidFilename(ByVal fileName As String) As Boolean
            Dim invalidFilenameChars As New Regex("[" & Regex.Escape(System.IO.Path.GetInvalidFileNameChars) & "]")
            Return invalidFilenameChars.IsMatch(fileName)
        End Function


        Private Function CreateControl(ByVal controlSrc As String) As String

            Dim folder As String = FileSystemUtils.RemoveTrailingSlash(GetSourceFolder())
            Dim className As String = GetClassName()
            Dim moduleControlPath As String = Server.MapPath("DesktopModules/" + folder + "/" + controlSrc)
            Dim message As String = Null.NullString

            Dim source As String = String.Format(Localization.GetString("ModuleControlTemplate", Me.LocalResourceFile), rblLanguage.SelectedValue, className)

            ' reset attributes
            If File.Exists(moduleControlPath) Then
                message = Localization.GetString("FileExists", Me.LocalResourceFile)
            Else
                ' write file
                Dim objStream As StreamWriter
                objStream = File.CreateText(moduleControlPath)
                objStream.WriteLine(source)
                objStream.Close()
            End If

            Return message
        End Function

        Private Function GetClassName() As String
            Dim strClass As String = Null.NullString
            If cboOwner.SelectedValue <> "" Then
                strClass += cboOwner.SelectedValue & "."
            End If
            If cboModule.SelectedValue <> "" Then
                strClass += cboModule.SelectedValue
            End If
            'return class and remove any spaces that might appear in folder structure
            Return strClass.Replace(" ", "")
        End Function
        Private Function GetSourceFolder() As String
            Dim strFolder As String = Null.NullString
            If cboOwner.SelectedValue <> "" Then
                strFolder += cboOwner.SelectedValue & "/"
            End If
            If cboModule.SelectedValue <> "" Then
                strFolder += cboModule.SelectedValue & "/"
            End If
            Return strFolder
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' Loads the cboSource control list with locations of controls.
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ImportControl(ByVal controlSrc As String) As ModuleDefinitionInfo
            Dim moduleDefinition As ModuleDefinitionInfo = Nothing
            Try
                Dim folder As String = FileSystemUtils.RemoveTrailingSlash(GetSourceFolder())
                Dim friendlyName As String = txtName.Text
                Dim name As String = GetClassName()
                Dim moduleControl As String = "DesktopModules/" + folder + "/" + controlSrc

                Dim package As New PackageInfo
                package.Name = name
                package.FriendlyName = friendlyName
                package.Description = txtDescription.Text
                package.Version = New Version(1, 0, 0)
                package.PackageType = "Module"
                package.License = Util.PACKAGE_NoLicense

                'Save Package
                PackageController.SavePackage(package)

                Dim objDesktopModules As New DesktopModuleController
                Dim objDesktopModule As New DesktopModuleInfo

                objDesktopModule.DesktopModuleID = Null.NullInteger
                objDesktopModule.ModuleName = name
                objDesktopModule.FolderName = folder
                objDesktopModule.FriendlyName = friendlyName
                objDesktopModule.Description = txtDescription.Text
                objDesktopModule.IsPremium = False
                objDesktopModule.IsAdmin = False
                objDesktopModule.Version = "01.00.00"
                objDesktopModule.BusinessControllerClass = ""
                objDesktopModule.CompatibleVersions = ""
                objDesktopModule.Dependencies = ""
                objDesktopModule.Permissions = ""
                objDesktopModule.PackageID = package.PackageID

                objDesktopModule.DesktopModuleID = objDesktopModules.AddDesktopModule(objDesktopModule)

                'Add module to all portals
                DesktopModuleController.AddDesktopModuleToPortals(objDesktopModule.DesktopModuleID)

                Dim objModuleDefinitions As New ModuleDefinitionController
                moduleDefinition = New ModuleDefinitionInfo

                moduleDefinition.ModuleDefID = Null.NullInteger
                moduleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID
                moduleDefinition.FriendlyName = friendlyName
                moduleDefinition.DefaultCacheTime = 0

                moduleDefinition.ModuleDefID = objModuleDefinitions.AddModuleDefinition(moduleDefinition)

                Dim objModuleControls As New ModuleControlController
                Dim objModuleControl As New ModuleControlInfo

                objModuleControl.ModuleControlID = Null.NullInteger
                objModuleControl.ModuleDefID = moduleDefinition.ModuleDefID
                objModuleControl.ControlKey = ""
                objModuleControl.ControlSrc = moduleControl
                objModuleControl.ControlTitle = ""
                objModuleControl.ControlType = SecurityAccessLevel.View
                objModuleControl.HelpURL = ""
                objModuleControl.IconFile = ""
                objModuleControl.ViewOrder = 0
                objModuleControl.SupportsPartialRendering = False

                ModuleControlController.AddModuleControl(objModuleControl)
            Catch exc As Exception
                LogException(exc)
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("ImportControl.ErrorMessage", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End Try

            Return moduleDefinition
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' Loads the cboSource control list with locations of controls.
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ImportManifest() As ModuleDefinitionInfo
            Dim moduleDefinition As ModuleDefinitionInfo = Nothing
            Try
                Dim folder As String = FileSystemUtils.RemoveTrailingSlash(GetSourceFolder())
                Dim manifest As String = Server.MapPath("~/DesktopModules/" + folder + "/" + cboFile.SelectedValue)
                Dim _Installer As New Installer(manifest, Request.MapPath("."), True)

                If _Installer.IsValid Then
                    'Reset Log
                    _Installer.InstallerInfo.Log.Logs.Clear()

                    'Install
                    _Installer.Install()

                    If _Installer.IsValid Then
                        Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByPackageID(_Installer.InstallerInfo.PackageID)
                        If desktopModule IsNot Nothing AndAlso desktopModule.ModuleDefinitions.Count > 0 Then
                            For Each kvp As KeyValuePair(Of String, ModuleDefinitionInfo) In desktopModule.ModuleDefinitions
                                moduleDefinition = kvp.Value
                                Exit For
                            Next
                        End If
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("InstallError.Text", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        phInstallLogs.Controls.Add(_Installer.InstallerInfo.Log.GetLogsTable)
                    End If
                Else
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("InstallError.Text", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    phInstallLogs.Controls.Add(_Installer.InstallerInfo.Log.GetLogsTable)
                End If
            Catch exc As Exception
                LogException(exc)
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("ImportControl.ErrorMessage", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End Try

            Return moduleDefinition
        End Function

        Private Sub LoadFiles(ByVal strExtensions As String)
            LoadFiles(strExtensions, "")
        End Sub

        Private Sub LoadFiles(ByVal strExtensions As String, ByVal strFolder As String)
            If strFolder = "" Then
                strFolder = Server.MapPath("~/DesktopModules/" + GetSourceFolder())
            End If

            cboFile.Items.Clear()
            Dim arrFiles As String() = Directory.GetFiles(strFolder)
            For Each strFile As String In arrFiles
                If strExtensions.Contains(Path.GetExtension(strFile)) Then
                    cboFile.Items.Add(Path.GetFileName(strFile))
                End If
            Next
        End Sub

        Private Sub LoadModuleFolders(ByVal selectedValue As String)
            cboModule.Items.Clear()
            Dim arrFolders As String() = Directory.GetDirectories(Common.Globals.ApplicationMapPath & "\DesktopModules\" & cboOwner.SelectedValue)
            For Each strFolder As String In arrFolders
                Dim item As New ListItem(strFolder.Replace(Path.GetDirectoryName(strFolder) & "\", ""))
                If item.Value = selectedValue Then
                    item.Selected = True
                End If
                cboModule.Items.Add(item)
            Next
            cboModule.Items.Insert(0, New ListItem("<" & Services.Localization.Localization.GetString("Not_Specified", Services.Localization.Localization.SharedResourceFile) & ">", ""))
        End Sub

        Private Sub LoadOwnerFolders(ByVal selectedValue As String)
            cboOwner.Items.Clear()
            Dim arrFolders As String() = Directory.GetDirectories(Common.Globals.ApplicationMapPath & "\DesktopModules\")
            For Each strFolder As String In arrFolders
                Dim files() As String = Directory.GetFiles(strFolder, "*.ascx")
                'exclude module folders
                If files.Length = 0 OrElse strFolder.ToLower = "admin" Then
                    Dim item As New ListItem(strFolder.Replace(Path.GetDirectoryName(strFolder) & "\", ""))
                    If item.Value = selectedValue Then
                        item.Selected = True
                    End If
                    cboOwner.Items.Add(item)
                End If
            Next
            cboOwner.Items.Insert(0, New ListItem("<" & Services.Localization.Localization.GetString("Not_Specified", Services.Localization.Localization.SharedResourceFile) & ">", ""))
        End Sub

        Private Sub SetupModuleFolders()
            Select Case cboCreate.SelectedValue
                Case "Control"
                    LoadFiles(".ascx")
                Case "Template"
                    LoadFiles(".module.template", HostMapPath & "Templates\")
                Case "Manifest"
                    LoadFiles(".dnn,.dnn5")
            End Select
        End Sub

        Private Sub SetupOwnerFolders()
            LoadModuleFolders(Null.NullString)
            SetupModuleFolders()
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Customise the Control Title
            If IsAddMode Then
                'Add
                ModuleContext.Configuration.ModuleTitle = Services.Localization.Localization.GetString("Add.Title", Me.LocalResourceFile)
            Else
                'Edit
                ModuleContext.Configuration.ModuleTitle = Services.Localization.Localization.GetString("Edit.Title", Me.LocalResourceFile)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    cboCreate.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", ""))
                    LoadOwnerFolders(Null.NullString)
                    LoadModuleFolders(Null.NullString)
                    'BindManifestList("DesktopModules")
                    'cboManifest.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", ""))
                    'BindControlList("DesktopModules")
                    'cboControl.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", ""))
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cboCreate_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCreate.SelectedIndexChanged
            LoadOwnerFolders(Null.NullString)
            LoadModuleFolders(Null.NullString)
            cboFile.Items.Clear()
            txtModule.Text = ""
            txtDescription.Text = ""

            Select Case cboCreate.SelectedValue
                Case ""
                    rowOwner1.Visible = False
                    cmdAddOwner.Visible = False
                    rowModule1.Visible = False
                    cmdAddModule.Visible = False
                    rowFile1.Visible = False
                    rowFile2.Visible = False
                    rowLang.Visible = False
                    rowName.Visible = False
                    rowDescription.Visible = False
                    rowSource.Visible = False
                    rowAddPage.Visible = False
                    cmdCreate.Visible = False
                Case "New"
                    rowOwner1.Visible = True
                    cmdAddOwner.Visible = True
                    rowModule1.Visible = True
                    cmdAddModule.Visible = True
                    rowFile1.Visible = False
                    rowFile2.Visible = True
                    rowLang.Visible = True
                    rowName.Visible = True
                    rowDescription.Visible = True
                    rowSource.Visible = False
                    rowAddPage.Visible = True
                    cmdCreate.Visible = True
                Case "Control"
                    rowOwner1.Visible = True
                    cmdAddOwner.Visible = False
                    rowModule1.Visible = True
                    cmdAddModule.Visible = False
                    rowFile1.Visible = True
                    rowFile2.Visible = False
                    rowLang.Visible = False
                    rowName.Visible = True
                    rowDescription.Visible = True
                    rowSource.Visible = False
                    rowAddPage.Visible = True
                    cmdCreate.Visible = True
                Case "Template"
                    rowOwner1.Visible = True
                    cmdAddOwner.Visible = True
                    rowModule1.Visible = True
                    cmdAddModule.Visible = True
                    rowFile1.Visible = True
                    rowFile2.Visible = False
                    rowLang.Visible = False
                    rowName.Visible = True
                    rowDescription.Visible = True
                    rowSource.Visible = False
                    rowAddPage.Visible = False
                    cmdCreate.Visible = True
                Case "Manifest"
                    rowOwner1.Visible = True
                    cmdAddOwner.Visible = False
                    rowModule1.Visible = True
                    cmdAddModule.Visible = False
                    rowFile1.Visible = True
                    rowFile2.Visible = False
                    rowLang.Visible = False
                    rowName.Visible = False
                    rowDescription.Visible = False
                    rowSource.Visible = False
                    rowAddPage.Visible = True
                    cmdCreate.Visible = True
            End Select
        End Sub

        Protected Sub cboModule_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboModule.SelectedIndexChanged
            SetupModuleFolders()
        End Sub

        Protected Sub cboOwner_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboOwner.SelectedIndexChanged
            SetupOwnerFolders()
        End Sub

        Protected Sub cmdAddModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddModule.Click
            rowModule1.Visible = False
            rowModule2.Visible = True
        End Sub

        Protected Sub cmdAddOwner_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddOwner.Click
            rowOwner1.Visible = False
            rowOwner2.Visible = True
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdCancelModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancelModule.Click
            rowModule1.Visible = True
            rowModule2.Visible = False
        End Sub

        Protected Sub cmdCancelOwner_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancelOwner.Click
            rowOwner1.Visible = True
            rowOwner2.Visible = False
        End Sub

        Protected Sub cmdCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCreate.Click
            Try
                Dim moduleDefinition As ModuleDefinitionInfo = Nothing
                Dim strMessage As String = Null.NullString
                Select Case cboCreate.SelectedValue
                    Case ""
                    Case "New"
                        If String.IsNullOrEmpty(cboModule.SelectedValue) Then
                            strMessage = Localization.GetString("ModuleFolder", Me.LocalResourceFile)
                            Exit Select
                        End If

                        If String.IsNullOrEmpty(rblLanguage.SelectedValue) Then
                            strMessage = Localization.GetString("LanguageError", Me.LocalResourceFile)
                            Exit Select
                        End If

                        'remove spaces so file is created correctly
                        Dim controlSrc As String = txtFile.Text.Replace(" ", "")
                        If InvalidFilename(controlSrc) Then
                            strMessage = Localization.GetString("InvalidFilename", Me.LocalResourceFile)
                            Exit Select
                        End If

                        If String.IsNullOrEmpty(controlSrc) Then
                            strMessage = Localization.GetString("MissingControl", Me.LocalResourceFile)
                            Exit Select
                        End If
                        If String.IsNullOrEmpty(txtName.Text) Then
                            strMessage = Localization.GetString("MissingFriendlyname", Me.LocalResourceFile)
                            Exit Select
                        End If
                        If Not controlSrc.EndsWith(".ascx") Then
                            controlSrc += ".ascx"
                        End If

                        Dim uniqueName As Boolean = True
                        Dim packages As List(Of PackageInfo) = New List(Of PackageInfo)
                        For Each package As PackageInfo In PackageController.GetPackages()
                            If package.Name = txtName.Text OrElse package.FriendlyName = txtName.Text Then
                                uniqueName = False
                                Exit For
                            End If
                        Next

                        If uniqueName = False Then
                            strMessage = Localization.GetString("NonuniqueName", Me.LocalResourceFile)
                            Exit Select
                        End If
                        'First create the control
                        strMessage = CreateControl(controlSrc)
                        If String.IsNullOrEmpty(strMessage) Then
                            'Next import the control
                            moduleDefinition = ImportControl(controlSrc)
                        End If
                    Case "Control"
                        moduleDefinition = ImportControl(cboFile.SelectedValue)
                    Case "Template"
                    Case "Manifest"
                        If String.IsNullOrEmpty(cboFile.SelectedValue) Then
                            strMessage = Localization.GetString("MissingManifest", Me.LocalResourceFile)
                            Exit Select
                        End If
                End Select

                If moduleDefinition Is Nothing Then
                    Skin.AddModuleMessage(Me, strMessage, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Else
                    If Not String.IsNullOrEmpty(cboCreate.SelectedValue) AndAlso chkAddPage.Checked Then
                        Dim tabName As String = "Test " + txtName.Text + " Page"
                        Dim tabPath As String = GenerateTabPath(Null.NullInteger, tabName)
                        Dim tabID As Integer = TabController.GetTabByTabPath(Me.ModuleContext.PortalId, tabPath)

                        If tabID = Null.NullInteger Then
                            'Create a new page
                            Dim newTab As New DotNetNuke.Entities.Tabs.TabInfo
                            newTab.TabName = "Test " + txtName.Text + " Page"
                            newTab.ParentId = Null.NullInteger
                            newTab.PortalID = Me.ModuleContext.PortalId
                            newTab.IsVisible = True
                            newTab.TabID = New TabController().AddTabBefore(newTab, Me.ModuleContext.PortalSettings.AdminTabId)

                            Dim objModule As New ModuleInfo
                            objModule.Initialize(Me.ModuleContext.PortalId)

                            objModule.PortalID = Me.ModuleContext.PortalId
                            objModule.TabID = newTab.TabID
                            objModule.ModuleOrder = Null.NullInteger
                            objModule.ModuleTitle = moduleDefinition.FriendlyName
                            objModule.PaneName = glbDefaultPane
                            objModule.ModuleDefID = moduleDefinition.ModuleDefID
                            objModule.InheritViewPermissions = True
                            objModule.AllTabs = False
                            Dim moduleCtl As New ModuleController
                            moduleCtl.AddModule(objModule)

                            Response.Redirect(NavigateURL(newTab.TabID), True)
                        Else
                            Skin.AddModuleMessage(Me, Localization.GetString("TabExists", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If

                    Else
                        'Redirect to main extensions page
                        Response.Redirect(NavigateURL(), True)
                    End If
                End If

            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub cmdSaveModule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveModule.Click
            AddFolder(cboOwner.SelectedValue, txtModule.Text)
            LoadModuleFolders(txtModule.Text)
            SetupModuleFolders()

            rowModule1.Visible = True
            rowModule2.Visible = False
        End Sub

        Protected Sub cmdSaveOwner_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveOwner.Click
            AddFolder("", txtOwner.Text)
            LoadOwnerFolders(txtOwner.Text)
            SetupOwnerFolders()

            rowOwner1.Visible = True
            rowOwner2.Visible = False
        End Sub

#End Region

    End Class

End Namespace
