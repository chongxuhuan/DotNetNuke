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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Writers
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Host
Imports System.Text
Imports DotNetNuke.Entities.Controllers


Namespace DotNetNuke.Services.Installer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LegacyUtil class is a Utility class that provides helper methods to transfer
    ''' legacy packages to Cambrian's Universal Installer based system
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/23/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LegacyUtil

        Private Shared AdminModules As String = "Adsense, MarketShare, Authentication, Banners, FeedExplorer, FileManager, HostSettings, Lists, LogViewer, Newsletters, PortalAliases, Portals, RecycleBin, Scheduler, SearchAdmin, SearchInput, SearchResults, Security, SiteLog, SiteWizard, SkinDesigner, Solutions, SQL, Tabs, Vendors,"
        Private Shared CoreModules As String = "DNN_Announcements, Blog, DNN_Documents, DNN_Events, DNN_FAQs, DNN_Feedback, DNN_Forum, Help, DNN_HTML, DNN_IFrame, DNN_Links, DNN_Media, DNN_NewsFeeds, DNN_Reports, Repository, Repository Dashboard, Store Admin, Store Account, Store Catalog, Store Mini Cart, Store Menu, DNN_Survey, DNN_UserDefinedTable, DNN_UsersOnline, Wiki, DNN_XML,"
        Private Shared KnownSkinObjects As String = "ACTIONBUTTON, ACTIONS, BANNER, BREADCRUMB, COPYRIGHT, CURRENTDATE, DOTNETNUKE, DROPDOWNACTIONS, HELP, HOSTNAME, ICON, LANGUAGE, LINKACTIONS, LINKS, LOGIN, LOGO, MENU, NAV, PRINTMODULE, PRIVACY, SEARCH, SIGNIN, SOLPARTACTIONS, SOLPARTMENU, STYLES, TERMS, TEXT, TITLE, TREEVIEW, USER, VISIBILITY,"
        Private Shared KnownSkins As String = "DNN-Blue, DNN-Gray, MinimalExtropy,"

        Private Shared Function CreateSkinPackage(ByVal skin As SkinPackageInfo) As PackageInfo
            'Create a Package
            Dim package As New PackageInfo(New InstallerInfo())
            package.Name = skin.SkinName
            package.FriendlyName = skin.SkinName
            package.Description = Null.NullString
            package.Version = New Version(1, 0, 0)
            package.PackageType = skin.SkinType
            package.License = Util.PACKAGE_NoLicense

            'See if the Skin is using a Namespace (or is a known skin)
            ParsePackageName(package)

            Return package
        End Function

        Private Shared Sub CreateSkinManifest(ByVal writer As XmlWriter, ByVal skinFolder As String, ByVal skinType As String, ByVal tempInstallFolder As String, ByVal subFolder As String)
            Dim skinName As String = Path.GetFileNameWithoutExtension(skinFolder)
            Dim skin As New SkinPackageInfo
            skin.SkinName = skinName
            skin.SkinType = skinType

            'Create a Package
            Dim package As PackageInfo = CreateSkinPackage(skin)

            'Create a SkinPackageWriter
            Dim skinWriter As New SkinPackageWriter(skin, package, tempInstallFolder, subFolder)
            skinWriter.GetFiles(False)

            'We need to reset the BasePath so it using the correct basePath rather than the Temp InstallFolder
            skinWriter.SetBasePath()

            'Writer package manifest fragment to writer
            skinWriter.WriteManifest(writer, True)
        End Sub

        Private Shared Sub ProcessLegacySkin(ByVal skinFolder As String, ByVal skinType As String)
            'Dim skinName As String = Path.GetFileNameWithoutExtension(skinFolder)
            Dim skinName As String = Path.GetFileName(skinFolder)
            If skinName <> "_default" Then

                Dim skin As New SkinPackageInfo
                skin.SkinName = skinName
                skin.SkinType = skinType

                'Create a Package
                Dim package As PackageInfo = CreateSkinPackage(skin)

                'Create a SkinPackageWriter
                Dim skinWriter As New SkinPackageWriter(skin, package)
                skinWriter.GetFiles(False)

                'Save the manifest
                package.Manifest = skinWriter.WriteManifest(True)

                'Save Package
                PackageController.SavePackage(package)

                'Update Skin Package with new PackageID
                skin.PackageID = package.PackageID

                'Save Skin Package
                skin.SkinPackageID = SkinController.AddSkinPackage(skin)

                For Each skinFile As InstallFile In skinWriter.Files.Values
                    If skinFile.Type = InstallFileType.Ascx Then
                        If skinType = "Skin" Then
                            SkinController.AddSkin(skin.SkinPackageID, Path.Combine("[G]" & SkinController.RootSkin, Path.Combine(skin.SkinName, skinFile.FullName)))
                        Else
                            SkinController.AddSkin(skin.SkinPackageID, Path.Combine("[G]" & SkinController.RootContainer, Path.Combine(skin.SkinName, skinFile.FullName)))
                        End If
                    End If
                Next
            End If
        End Sub

        Private Shared Sub ParsePackageName(ByVal package As PackageInfo, ByVal separator As String)
            'See if the Module is using a "Namespace" for its name
            Dim ownerIndex As Integer = package.Name.IndexOf(separator)
            If ownerIndex > 0 Then
                package.Owner = package.Name.Substring(0, ownerIndex)
            End If
        End Sub

#Region "Public Shared Methods"

        Public Shared Function CreateSkinManifest(ByVal skinFolder As String, ByVal skinType As String, ByVal tempInstallFolder As String) As String
            'Test if there are Skins and Containers folders in TempInstallFolder (ie it is a legacy combi package)
            Dim isCombi As Boolean = False
            Dim installFolder As New DirectoryInfo(tempInstallFolder)
            Dim subFolders As DirectoryInfo() = installFolder.GetDirectories()
            If subFolders.Length > 0 Then
                If (subFolders(0).Name.ToLowerInvariant = "containers" OrElse subFolders(0).Name.ToLowerInvariant = "skins") Then
                    isCombi = True
                End If
            End If

            'Create a writer to create the processed manifest
            Dim sb As New StringBuilder
            Dim writer As XmlWriter = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment))

            PackageWriterBase.WriteManifestStartElement(writer)

            If isCombi Then
                If Directory.Exists(Path.Combine(tempInstallFolder, "Skins")) Then
                    'Add Skin Package Fragment
                    CreateSkinManifest(writer, skinFolder, "Skin", tempInstallFolder.Replace(ApplicationMapPath + "\", ""), "Skins")
                End If
                If Directory.Exists(Path.Combine(tempInstallFolder, "Containers")) Then
                    'Add Container PAckage Fragment
                    CreateSkinManifest(writer, skinFolder, "Container", tempInstallFolder.Replace(ApplicationMapPath + "\", ""), "Containers")
                End If
            Else
                'Add Package Fragment
                CreateSkinManifest(writer, skinFolder, skinType, tempInstallFolder.Replace(ApplicationMapPath + "\", ""), "")
            End If

            PackageWriterBase.WriteManifestEndElement(writer)

            'Close XmlWriter
            writer.Close()

            'Return new manifest
            Return sb.ToString()

        End Function

        Public Shared Sub ParsePackageName(ByVal package As PackageInfo)
            ParsePackageName(package, ".")
            If String.IsNullOrEmpty(package.Owner) Then
                ParsePackageName(package, "\")
            End If
            If String.IsNullOrEmpty(package.Owner) Then
                ParsePackageName(package, "_")
            End If

            If package.PackageType = "Module" AndAlso AdminModules.Contains(package.Name & ",") OrElse _
                    package.PackageType = "Module" AndAlso CoreModules.Contains(package.Name & ",") OrElse _
                    (package.PackageType = "Container" OrElse package.PackageType = "Skin") AndAlso KnownSkins.Contains(package.Name & ",") OrElse _
                    package.PackageType = "SkinObject" AndAlso KnownSkinObjects.Contains(package.Name & ",") Then
                If String.IsNullOrEmpty(package.Owner) Then
                    package.Owner = "DotNetNuke"
                    package.Name = "DotNetNuke." + package.Name
                    Select Case package.PackageType
                        Case "Skin"
                            package.Name += ".Skin"
                            package.FriendlyName += " Skin"
                        Case "Container"
                            package.Name += ".Container"
                            package.FriendlyName += " Container"
                        Case "SkinObject"
                            package.Name += "SkinObject"
                            package.FriendlyName += " SkinObject"
                    End Select
                End If
            End If
            If package.Owner = "DotNetNuke" Then
                package.License = Localization.Localization.GetString("License", Localization.Localization.GlobalResourceFile)
                package.Organization = "DotNetNuke Corporation"
                package.Url = "www.dotnetnuke.com"
                package.Email = "support@dotnetnuke.com"
                package.ReleaseNotes = "There are no release notes for this version."
            Else
                package.License = Util.PACKAGE_NoLicense
            End If
        End Sub

        Public Shared Sub ProcessLegacyLanguages()
            Dim filePath As String = Common.Globals.ApplicationMapPath & Localization.Localization.SupportedLocalesFile.Substring(1).Replace("/", "\")

            If File.Exists(filePath) Then
                Dim doc As New XPathDocument(filePath)

                'Check for Browser and Url settings

                Dim browserNav As XPathNavigator = doc.CreateNavigator.SelectSingleNode("root/browserDetection")
                If browserNav IsNot Nothing Then
                    HostController.Instance.Update("EnableBrowserLanguage", Util.ReadAttribute(browserNav, "enabled", False, Nothing, Null.NullString, "true"))
                End If

                Dim urlNav As XPathNavigator = doc.CreateNavigator.SelectSingleNode("root/languageInUrl")
                If urlNav IsNot Nothing Then
                    HostController.Instance.Update("EnableUrlLanguage", Util.ReadAttribute(urlNav, "enabled", False, Nothing, Null.NullString, "true"))
                End If

                'Process each language
                For Each nav As XPathNavigator In doc.CreateNavigator.Select("root/language")
                    If nav.NodeType <> XmlNodeType.Comment Then
                        Dim language As New Locale
                        language.Text = Util.ReadAttribute(nav, "name")
                        language.Code = Util.ReadAttribute(nav, "key")
                        language.Fallback = Util.ReadAttribute(nav, "fallback")

                        'Save Language
                        Localization.Localization.SaveLanguage(language)

                        If language.Code <> Localization.Localization.SystemLocale Then

                            'Create a Package
                            Dim package As New PackageInfo(New InstallerInfo())
                            package.Name = language.Text
                            package.FriendlyName = language.Text
                            package.Description = Null.NullString
                            package.Version = New Version(1, 0, 0)
                            package.PackageType = "CoreLanguagePack"
                            package.License = Util.PACKAGE_NoLicense

                            'Create a LanguagePackWriter
                            Dim packageWriter As New LanguagePackWriter(language, package)

                            'Save the manifest
                            package.Manifest = packageWriter.WriteManifest(True)

                            'Save Package
                            PackageController.SavePackage(package)

                            Dim languagePack As New LanguagePackInfo()
                            languagePack.LanguageID = language.LanguageId
                            languagePack.PackageID = package.PackageID
                            languagePack.DependentPackageID = -2

                            LanguagePackController.SaveLanguagePack(languagePack)
                        End If
                    End If
                Next
            End If

            'Process Portal Locales files
            For Each portal As PortalInfo In New PortalController().GetPortals
                Dim portalID As Integer = portal.PortalID
                filePath = String.Format(Common.Globals.ApplicationMapPath & Localization.Localization.ApplicationResourceDirectory.Substring(1).Replace("/", "\") & "\Locales.Portal-{0}.xml", portalID.ToString())

                If File.Exists(filePath) Then
                    Dim doc As New XPathDocument(filePath)

                    'Check for Browser and Url settings
                    Dim browserNav As XPathNavigator = doc.CreateNavigator.SelectSingleNode("locales/browserDetection")
                    If browserNav IsNot Nothing Then
                        PortalController.UpdatePortalSetting(portalID, "EnableBrowserLanguage", Util.ReadAttribute(browserNav, "enabled", False, Nothing, Null.NullString, "true"))
                    End If

                    Dim urlNav As XPathNavigator = doc.CreateNavigator.SelectSingleNode("locales/languageInUrl")
                    If urlNav IsNot Nothing Then
                        PortalController.UpdatePortalSetting(portalID, "EnableUrlLanguage", Util.ReadAttribute(urlNav, "enabled", False, Nothing, Null.NullString, "true"))
                    End If

                    For Each installedLanguage As Locale In LocaleController.Instance().GetLocales(Null.NullInteger).Values
                        Dim code As String = installedLanguage.Code
                        Dim bFound As Boolean = False

                        'Check if this language is "inactive"
                        For Each inactiveNav As XPathNavigator In doc.CreateNavigator.Select("locales/inactive/locale")
                            If inactiveNav.Value = code Then
                                bFound = True
                                Exit For
                            End If
                        Next

                        If Not bFound Then
                            'Language is enabled - add to portal
                            Localization.Localization.AddLanguageToPortal(portalID, installedLanguage.LanguageId, False)
                        End If
                    Next
                Else
                    For Each installedLanguage As Locale In LocaleController.Instance().GetLocales(Null.NullInteger).Values
                        'Language is enabled - add to portal
                        Localization.Localization.AddLanguageToPortal(portalID, installedLanguage.LanguageId, False)
                    Next
                End If
            Next
        End Sub

        Public Shared Sub ProcessLegacyModules()
            For Each desktopModule As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(Null.NullInteger).Values
                If desktopModule.PackageID = Null.NullInteger Then

                    'Get the Module folder
                    Dim moduleFolder As String = Path.Combine(ApplicationMapPath, Path.Combine("DesktopModules", desktopModule.FolderName))

                    'Find legacy manifest
                    Dim rootNav As XPathNavigator = Nothing
                    Try
                        Dim hostModules As String = "Portals, SQL, HostSettings, Scheduler, SearchAdmin, Lists, SkinDesigner, Extensions"
                        Dim files As String() = Directory.GetFiles(moduleFolder, "*.dnn.config")
                        If files.Length > 0 Then
                            'Create an XPathDocument from the Xml
                            Dim doc As New XPathDocument(New FileStream(files(0), FileMode.Open, FileAccess.Read))
                            rootNav = doc.CreateNavigator().SelectSingleNode("dotnetnuke")
                        End If

                        'Module is not affiliated with a Package
                        Dim package As New PackageInfo(New InstallerInfo)
                        package.Name = desktopModule.ModuleName

                        package.FriendlyName = desktopModule.FriendlyName
                        package.Description = desktopModule.Description
                        package.Version = New Version(1, 0, 0)
                        If Not String.IsNullOrEmpty(desktopModule.Version) Then
                            package.Version = New Version(desktopModule.Version)
                        End If
                        If hostModules.Contains(desktopModule.ModuleName) Then
                            'Host Module so make this a system package
                            package.IsSystemPackage = True
                            desktopModule.IsAdmin = True
                        Else
                            desktopModule.IsAdmin = False
                        End If
                        package.PackageType = "Module"

                        'See if the Module is using a "Namespace" for its name
                        ParsePackageName(package)

                        If files.Length > 0 Then
                            Dim modulewriter As New ModulePackageWriter(desktopModule, rootNav, package)
                            package.Manifest = modulewriter.WriteManifest(True)
                        Else
                            package.Manifest = "" ' module has no manifest
                        End If

                        'Save Package
                        PackageController.SavePackage(package)

                        'Update Desktop Module with new PackageID
                        desktopModule.PackageID = package.PackageID

                        'Save DesktopModule
                        DesktopModuleController.SaveDesktopModule(desktopModule, False, False)
                    Catch ex As Exception

                    End Try
                End If
            Next
        End Sub

        Public Shared Sub ProcessLegacySkinControls()
            For Each skinControl As SkinControlInfo In SkinControlController.GetSkinControls().Values
                If skinControl.PackageID = Null.NullInteger Then

                    Try
                        'SkinControl is not affiliated with a Package
                        Dim package As New PackageInfo(New InstallerInfo)
                        package.Name = skinControl.ControlKey

                        package.FriendlyName = skinControl.ControlKey
                        package.Description = Null.NullString
                        package.Version = New Version(1, 0, 0)
                        package.PackageType = "SkinObject"

                        'See if the SkinControl is using a "Namespace" for its name
                        ParsePackageName(package)

                        Dim skinControlWriter As New SkinControlPackageWriter(skinControl, package)
                        package.Manifest = skinControlWriter.WriteManifest(True)

                        'Save Package
                        PackageController.SavePackage(package)

                        'Update SkinControl with new PackageID
                        skinControl.PackageID = package.PackageID

                        'Save SkinControl
                        SkinControlController.SaveSkinControl(skinControl)
                    Catch ex As Exception

                    End Try
                End If
            Next
        End Sub

        Public Shared Sub ProcessLegacySkins()
            'Process Legacy Skins
            Dim skinRootPath As String = Path.Combine(Common.Globals.HostMapPath, SkinController.RootSkin)
            For Each skinFolder As String In Directory.GetDirectories(skinRootPath)
                ProcessLegacySkin(skinFolder, "Skin")
            Next

            'Process Legacy Containers
            skinRootPath = Path.Combine(Common.Globals.HostMapPath, SkinController.RootContainer)
            For Each skinFolder As String In Directory.GetDirectories(skinRootPath)
                ProcessLegacySkin(skinFolder, "Container")
            Next
        End Sub

#End Region

    End Class

End Namespace

