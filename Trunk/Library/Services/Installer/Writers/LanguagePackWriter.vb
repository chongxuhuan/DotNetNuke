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

Imports System
Imports System.IO
Imports System.Xml.XPath

Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Entities.Modules
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LanguagePackWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LanguagePackWriter
        Inherits PackageWriterBase

#Region "Private Members"

        Private _IsCore As Boolean = Null.NullBoolean
        Private _Language As Locale
        Private _LanguagePack As LanguagePackInfo

#End Region

#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            _LanguagePack = LanguagePackController.GetLanguagePackByPackage(package.PackageID)
            If LanguagePack IsNot Nothing Then
                _Language = Localization.Localization.GetLocaleByID(_LanguagePack.LanguageID)
                If LanguagePack.PackageType = LanguagePackType.Core Then
                    BasePath = Null.NullString
                Else
                    'Get the BasePath of the Dependent Package
                    Dim dependendentPackage As PackageInfo = PackageController.GetPackage(LanguagePack.DependentPackageID)
                    Dim dependentPackageWriter As PackageWriterBase = PackageWriterFactory.GetWriter(dependendentPackage)
                    BasePath = dependentPackageWriter.BasePath
                End If
            Else
                BasePath = Null.NullString
            End If

        End Sub

        Public Sub New(ByVal manifestNav As XPathNavigator, ByVal installer As InstallerInfo)
            _Language = New Locale
            Dim cultureNav As XPathNavigator = manifestNav.SelectSingleNode("Culture")
            _Language.Text = Util.ReadAttribute(cultureNav, "DisplayName")
            _Language.Code = Util.ReadAttribute(cultureNav, "Code")
            _Language.Fallback = Services.Localization.Localization.SystemLocale

            'Create a Package
            Package = New PackageInfo(installer)
            Package.Name = Language.Text
            Package.FriendlyName = Language.Text
            Package.Description = Null.NullString
            Package.Version = New Version(1, 0, 0)
            Package.License = Util.PACKAGE_NoLicense

            ReadLegacyManifest(manifestNav)

            If _IsCore Then
                Package.PackageType = "CoreLanguagePack"
            Else
                Package.PackageType = "ExtensionLanguagePack"
            End If

            BasePath = Null.NullString
        End Sub

        Public Sub New(ByVal language As Locale, ByVal package As PackageInfo)
            MyBase.New(package)
            _Language = language

            BasePath = Null.NullString
        End Sub

#End Region

#Region "Public Properties"

        Public Overrides ReadOnly Property IncludeAssemblies() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Language
        ''' </summary>
        ''' <value>An Locale object</value>
        ''' <history>
        ''' 	[cnurse]	01/30/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Language() As Locale
            Get
                Return _Language
            End Get
            Set(ByVal value As Locale)
                _Language = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated Language Pack
        ''' </summary>
        ''' <value>An LanguagePackInfo object</value>
        ''' <history>
        ''' 	[cnurse]	05/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LanguagePack() As LanguagePackInfo
            Get
                Return _LanguagePack
            End Get
            Set(ByVal value As LanguagePackInfo)
                _LanguagePack = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub ReadLegacyManifest(ByVal manifestNav As System.Xml.XPath.XPathNavigator)
            Dim fileName As String = Null.NullString
            Dim filePath As String = Null.NullString
            Dim sourceFileName As String = Null.NullString
            Dim resourcetype As String = Null.NullString
            Dim moduleName As String = Null.NullString

            For Each fileNav As XPathNavigator In manifestNav.Select("Files/File")
                fileName = Util.ReadAttribute(fileNav, "FileName").ToLowerInvariant()
                resourcetype = Util.ReadAttribute(fileNav, "FileType")
                moduleName = Util.ReadAttribute(fileNav, "ModuleName").ToLowerInvariant()
                sourceFileName = Path.Combine(resourcetype, Path.Combine(moduleName, fileName))

                Dim extendedExtension As String = "." + Language.Code.ToLowerInvariant() + ".resx"
                Select Case resourcetype
                    Case "GlobalResource"
                        filePath = "App_GlobalResources"
                        _IsCore = True
                    Case "ControlResource"
                        filePath = "Controls\App_LocalResources"
                    Case "AdminResource"
                        _IsCore = True
                        Select Case moduleName
                            Case "authentication"
                                filePath = "DesktopModules\Admin\Authentication\App_LocalResources"
                            Case "controlpanel"
                                filePath = "Admin\ControlPanel\App_LocalResources"
                            Case "files"
                                filePath = "DesktopModules\Admin\FileManager\App_LocalResources"
                            Case "host"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "authentication.ascx"
                                        filePath = ""
                                    Case "friendlyurls.ascx"
                                        filePath = "DesktopModules\Admin\HostSettings\App_LocalResources"
                                    Case "hostsettings.ascx"
                                        filePath = "DesktopModules\Admin\HostSettings\App_LocalResources"
                                    Case "requestfilters.ascx"
                                        filePath = "DesktopModules\Admin\HostSettings\App_LocalResources"
                                    Case "solutions.ascx"
                                        filePath = "DesktopModules\Admin\Solutions\App_LocalResources"
                                End Select
                            Case "lists"
                                filePath = "DesktopModules\Admin\Lists\App_LocalResources"
                            Case "localization"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "languageeditor.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case "languageeditorext.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case "timezoneeditor.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case "resourceverifier.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case Else
                                        filePath = ""
                                End Select
                            Case "log"
                                filePath = "DesktopModules\Admin\SiteLog\App_LocalResources"
                            Case "logging"
                                filePath = "DesktopModules\Admin\LogViewer\App_LocalResources"
                            Case "moduledefinitions"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "editmodulecontrol.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case "importmoduledefinition.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case "timezoneeditor.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case Else
                                        filePath = ""
                                End Select
                            Case "modules"
                                filePath = "Admin\Modules\App_LocalResources"
                            Case "packages"
                                filePath = "DesktopModules\Admin\Extensions\App_LocalResources"
                            Case "portal"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "editportalalias.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "portalalias.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "portals.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "privacy.ascx"
                                        filePath = "Admin\Portal\App_LocalResources"
                                    Case "signup.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "sitesettings.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "sitewizard.ascx"
                                        filePath = "DesktopModules\Admin\SiteWizard\App_LocalResources"
                                    Case "sql.ascx"
                                        filePath = "DesktopModules\Admin\SQL\App_LocalResources"
                                    Case "systemmessages.ascx"
                                        filePath = ""
                                    Case "template.ascx"
                                        filePath = "DesktopModules\Admin\Portals\App_LocalResources"
                                    Case "terms.ascx"
                                        filePath = "Admin\Portal\App_LocalResources"
                                End Select
                            Case "scheduling"
                                filePath = "DesktopModules\Admin\Scheduler\App_LocalResources"
                            Case "search"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "inputsettings.ascx"
                                        filePath = "DesktopModules\Admin\SearchInput\App_LocalResources"
                                    Case "resultssettings.ascx"
                                        filePath = "DesktopModules\Admin\SearchResults\App_LocalResources"
                                    Case "searchadmin.ascx"
                                        filePath = "DesktopModules\Admin\SearchAdmin\App_LocalResources"
                                    Case "searchinput.ascx"
                                        filePath = "DesktopModules\Admin\SearchInput\App_LocalResources"
                                    Case "searchresults.ascx"
                                        filePath = "DesktopModules\Admin\SearchResults\App_LocalResources"
                                End Select
                            Case "security"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "accessdenied.ascx"
                                        filePath = "Admin\Security\App_LocalResources"
                                    Case "authenticationsettings.ascx"
                                        filePath = ""
                                    Case "editgroups.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "editroles.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "register.ascx"
                                        filePath = ""
                                    Case "roles.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "securityroles.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "sendpassword.ascx"
                                        filePath = "Admin\Security\App_LocalResources"
                                    Case "signin.ascx"
                                        filePath = ""
                                End Select
                            Case "skins"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "attributes.ascx"
                                        filePath = "DesktopModules\Admin\SkinDesigner\App_LocalResources"
                                    Case "editskins.ascx"
                                        filePath = "DesktopModules\Admin\Extensions\Editors\App_LocalResources"
                                    Case Else
                                        filePath = "Admin\Skins\App_LocalResources"
                                End Select
                            Case "syndication"
                                filePath = "DesktopModules\Admin\FeedExplorer\App_LocalResources"
                            Case "tabs"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "export.ascx"
                                        filePath = "Admin\Tabs\App_LocalResources"
                                    Case "import.ascx"
                                        filePath = "Admin\Tabs\App_LocalResources"
                                    Case "managetabs.ascx"
                                        filePath = "DesktopModules\Admin\Tabs\App_LocalResources"
                                    Case "recyclebin.ascx"
                                        filePath = "DesktopModules\Admin\RecycleBin\App_LocalResources"
                                    Case "tabs.ascx"
                                        filePath = "DesktopModules\Admin\Tabs\App_LocalResources"
                                End Select
                            Case "users"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "bulkemail.ascx"
                                        filePath = "DesktopModules\Admin\Newsletters\App_LocalResources"
                                        fileName = "Newsletter.ascx" + extendedExtension
                                    Case "editprofiledefinition.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "manageusers.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "memberservices.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "membership.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "password.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "profile.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "profiledefinitions.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "user.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "users.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "usersettings.ascx"
                                        filePath = "DesktopModules\Admin\Security\App_LocalResources"
                                    Case "viewprofile.ascx"
                                        filePath = "Admin\Users\App_LocalResources"
                                End Select
                            Case "vendors"
                                Select Case fileName.Replace(extendedExtension, "")
                                    Case "adsense.ascx"
                                        filePath = ""
                                    Case "editadsense.ascx"
                                        filePath = ""
                                    Case "affiliates.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                    Case "banneroptions.ascx"
                                        filePath = "DesktopModules\Admin\Banners\App_LocalResources"
                                    Case "banners.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                    Case "displaybanners.ascx"
                                        filePath = "DesktopModules\Admin\Banners\App_LocalResources"
                                    Case "editaffiliate.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                    Case "editbanner.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                    Case "editvendors.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                    Case "vendors.ascx"
                                        filePath = "DesktopModules\Admin\Vendors\App_LocalResources"
                                End Select
                        End Select
                    Case "LocalResource"
                        filePath = Path.Combine("DesktopModules", Path.Combine(moduleName, "App_LocalResources"))
                        'Two assumptions are made here
                        ' 1. Core files appear in the package before extension files
                        ' 2. Module packages only include one module
                        If Not _IsCore AndAlso _LanguagePack Is Nothing Then
                            'Check if language is installed
                            Dim locale As Locale = Localization.Localization.GetLocale(_Language.Code)
                            If locale Is Nothing Then
                                LegacyError = "CoreLanguageError"
                            Else
                                'Attempt to figure out the Extension
                                For Each kvp As KeyValuePair(Of Integer, DesktopModuleInfo) In DesktopModuleController.GetDesktopModules(Null.NullInteger)
                                    If kvp.Value.FolderName.ToLowerInvariant = moduleName Then
                                        'Found Module - Get Package
                                        Dim dependentPackage As PackageInfo = PackageController.GetPackage(kvp.Value.PackageID)
                                        Package.Name += "_" + dependentPackage.Name
                                        Package.FriendlyName += " " + dependentPackage.FriendlyName
                                        _
                                        _LanguagePack = New LanguagePackInfo()

                                        _LanguagePack.DependentPackageID = dependentPackage.PackageID
                                        _LanguagePack.LanguageID = locale.LanguageID
                                        Exit For
                                    End If
                                Next

                                If _LanguagePack Is Nothing Then
                                    LegacyError = "DependencyError"
                                End If
                            End If
                        End If
                    Case "ProviderResource"
                        filePath = Path.Combine("Providers", Path.Combine(moduleName, "App_LocalResources"))
                    Case "InstallResource"
                        filePath = "Install\App_LocalResources"
                End Select

                If Not String.IsNullOrEmpty(filePath) Then
                    AddFile(Path.Combine(filePath, fileName), sourceFileName)
                End If
            Next
        End Sub

#End Region

#Region "Protected methods"

        Protected Overrides Sub GetFiles(ByVal includeSource As Boolean, ByVal includeAppCode As Boolean)
            'Language file starts at the root
            ParseFolder(Path.Combine(ApplicationMapPath, BasePath), ApplicationMapPath)
        End Sub

        Protected Overrides Sub ParseFiles(ByVal folder As System.IO.DirectoryInfo, ByVal rootPath As String)
            If LanguagePack.PackageType = LanguagePackType.Core Then
                If folder.FullName.ToLowerInvariant.Contains("desktopmodules") AndAlso _
                    Not folder.FullName.ToLowerInvariant.Contains("admin") _
                    OrElse folder.FullName.ToLowerInvariant.Contains("providers") Then
                    Exit Sub
                End If
                If folder.FullName.ToLowerInvariant.Contains("install") AndAlso _
                    folder.FullName.ToLowerInvariant.Contains("temp")  Then
                    Exit Sub
                End If
            End If

            If folder.Name.ToLowerInvariant = "app_localresources" OrElse folder.Name.ToLowerInvariant = "app_globalresources" Then
                'Add the Files in the Folder
                Dim files As FileInfo() = folder.GetFiles()
                For Each file As FileInfo In files
                    Dim filePath As String = folder.FullName.Replace(rootPath, "")
                    If filePath.StartsWith("\") Then
                        filePath = filePath.Substring(1)
                    End If

                    If file.Name.ToLowerInvariant.Contains(Language.Code.ToLowerInvariant) OrElse _
                            (Language.Code.ToLowerInvariant = "en-us" AndAlso Not file.Name.Contains("-")) Then
                        AddFile(Path.Combine(filePath, file.Name))
                    End If
                Next
            End If
        End Sub

        Protected Overrides Sub WriteFilesToManifest(ByVal writer As System.Xml.XmlWriter)
            Dim languageFileWriter As LanguageComponentWriter
            If LanguagePack Is Nothing Then
                languageFileWriter = New LanguageComponentWriter(Language, BasePath, Files, Package)
            Else
                languageFileWriter = New LanguageComponentWriter(LanguagePack, BasePath, Files, Package)
            End If
            languageFileWriter.WriteManifest(writer)
        End Sub

#End Region

    End Class

End Namespace

