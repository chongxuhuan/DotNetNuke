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

Imports System.IO
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Writers
Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.Languages
    Partial Class LanguagePackWriter
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

        Private packageWriter As DotNetNuke.Services.Installer.Writers.LanguagePackWriter
        Protected ReadOnly Property BasePath() As String
            Get
                Return Server.MapPath("~/Install/Language")
            End Get
        End Property

        Private _Files As Dictionary(Of String, InstallFile)
        Private _Manifest As String = Null.NullString
        Private _IsPackCreated As Boolean = True

        Private Sub CreateAuthSystemPackage(ByVal authPackage As PackageInfo, ByVal createZip As Boolean)

            Dim Package As New PackageInfo()
            Package.Name = authPackage.Name
            Package.FriendlyName = authPackage.Name
            Package.Version = authPackage.Version
            Package.License = Util.PACKAGE_NoLicense

            Dim fileName As String = Path.Combine(BasePath, "ResourcePack." & Package.Name)

            Dim authSystem As DotNetNuke.Services.Authentication.AuthenticationInfo = DotNetNuke.Services.Authentication.AuthenticationController.GetAuthenticationServiceByPackageID(authPackage.PackageID)
            Dim authPath As String = authSystem.LoginControlSrc.Substring(0, authSystem.LoginControlSrc.LastIndexOf("/"))
            CreatePackage(Package, authPackage.PackageID, authPath.Replace("/", "\"), fileName, createZip)
        End Sub

        Private Sub CreateCorePackage(ByVal createZip As Boolean)
            Dim Package As New PackageInfo()
            Package.Name = DotNetNuke.Common.Globals.CleanFileName(txtFileName.Text)
            Package.Version = DotNetNukeContext.Current.Application.Version
            Package.License = Util.PACKAGE_NoLicense

            Dim fileName As String = Path.Combine(BasePath, "ResourcePack." & Package.Name)

            CreatePackage(Package, -2, "", fileName, createZip)
        End Sub

        Private Sub CreateFullPackage()
            Dim language As Locale = Localization.GetLocale(cboLanguage.SelectedValue)
            Dim Package As New PackageInfo()
            Package.Name = DotNetNuke.Common.Globals.CleanFileName(txtFileName.Text)
            Package.Version = DotNetNukeContext.Current.Application.Version
            Package.License = Util.PACKAGE_NoLicense
            Package.PackageType = "CoreLanguagePack"

            _Files = New Dictionary(Of String, InstallFile)
            CreateCorePackage(False)
            For Each desktopModule As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(Null.NullInteger).Values
                If Not desktopModule.FolderName.StartsWith("Admin/") Then
                    CreateModulePackage(desktopModule, False)
                End If
            Next
            For Each provider As PackageInfo In PackageController.GetPackagesByType("Provider")
                CreateProviderPackage(provider, False)
            Next
            For Each authSystem As PackageInfo In PackageController.GetPackagesByType("Auth_System")
                CreateAuthSystemPackage(authSystem, False)
            Next

            Dim fileName As String = Path.Combine(BasePath, "ResourcePack." & Package.Name)
            fileName = fileName & "." & Package.Version.ToString(3) & "." & language.Code & ".zip"

            packageWriter = PackageWriterFactory.GetWriter(Package)
            packageWriter.Language = language
            packageWriter.BasePath = ""
            For Each kvp As KeyValuePair(Of String, InstallFile) In _Files
                packageWriter.Files.Add(kvp.Key, kvp.Value)
            Next

            packageWriter.CreatePackage(fileName, Package.Name + " " + language.Text + ".dnn", _Manifest, True)
        End Sub

        Private Sub CreateModulePackage(ByVal desktopModule As DesktopModuleInfo, ByVal createZip As Boolean)
            'Get the Module Package
            Dim modulePackage As PackageInfo = PackageController.GetPackage(desktopModule.PackageID)

            Dim Package As New PackageInfo()
            Package.Name = modulePackage.Name
            Package.FriendlyName = modulePackage.Name
            Package.Version = modulePackage.Version
            Package.License = Util.PACKAGE_NoLicense

            Dim fileName As String = Path.Combine(BasePath, "ResourcePack." & Package.Name)

            CreatePackage(Package, modulePackage.PackageID, Path.Combine("DesktopModules\", desktopModule.FolderName), fileName, createZip)
        End Sub

        Private Sub CreatePackage(ByVal package As PackageInfo, ByVal dependentPackageID As Integer, ByVal basePath As String, ByVal fileName As String, ByVal createZip As Boolean)
            Dim manifest As String

            Dim language As Locale = Localization.GetLocale(cboLanguage.SelectedValue)
            Dim languagePack As New LanguagePackInfo()
            languagePack.LanguageID = language.LanguageID
            languagePack.DependentPackageID = dependentPackageID

            If dependentPackageID = -2 Then
                package.PackageType = "CoreLanguagePack"
            Else
                package.PackageType = "ExtensionLanguagePack"
            End If

            package.Name += " " + language.Text
            package.FriendlyName += " " + language.Text

            packageWriter = PackageWriterFactory.GetWriter(package)
            packageWriter.Language = language
            packageWriter.LanguagePack = languagePack
            packageWriter.BasePath = basePath
            packageWriter.GetFiles(False)

            If packageWriter.Files.Count > 0 Then
                _IsPackCreated = True
                If createZip Then
                    'Create Zip for this package
                    manifest = packageWriter.WriteManifest(True)
                    fileName = fileName & "." & package.Version.ToString(3) & "." & language.Code & ".zip"
                    packageWriter.CreatePackage(fileName, package.Name + ".dnn", manifest, True)
                Else
                    'Save manifest and Files
                    packageWriter.BasePath = ""
                    _Manifest += packageWriter.WriteManifest(True)
                    For Each kvp As KeyValuePair(Of String, InstallFile) In packageWriter.Files
                        _Files(kvp.Key) = kvp.Value
                    Next
                End If
            Else
                _IsPackCreated = False
            End If
        End Sub

        Private Sub CreateProviderPackage(ByVal providerPackage As PackageInfo, ByVal createZip As Boolean)

            Dim Package As New PackageInfo()
            Package.Name = providerPackage.Name
            Package.FriendlyName = providerPackage.Name
            Package.Version = providerPackage.Version
            Package.License = Util.PACKAGE_NoLicense

            Dim fileName As String = Path.Combine(BasePath, "ResourcePack." & Package.Name)

            'Get the provider "path"
            Dim configDoc As XmlDocument = Config.Load()
            Dim providerName As String = Package.Name
            If providerName.IndexOf(".") > Null.NullInteger Then
                providerName = providerName.Substring(providerName.IndexOf(".") + 1)
            End If
            Select Case providerName
                Case "SchedulingProvider"
                    providerName = "DNNScheduler"
                Case "SearchIndexProvider"
                    providerName = "ModuleIndexProvider"
                Case "SearchProvider"
                    providerName = "SearchDataStoreProvider"
            End Select
            Dim providerNavigator As XPathNavigator = configDoc.CreateNavigator.SelectSingleNode("/configuration/dotnetnuke/*/providers/add[@name='" & providerName & "']")
            If providerNavigator IsNot Nothing Then
                Dim providerPath As String = providerNavigator.GetAttribute("providerPath", "")
                CreatePackage(Package, providerPackage.PackageID, providerPath.Substring(2, providerPath.Length - 3).Replace("/", "\"), fileName, createZip)
            End If
        End Sub

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not Page.IsPostBack Then
                For Each language As Locale In Localization.GetLocales(Null.NullInteger).Values
                    cboLanguage.Items.Add(New ListItem(language.Text, language.Code))
                Next

                rowitems.Visible = False
            End If
        End Sub

        Private Sub rbPackType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbPackType.SelectedIndexChanged
            pnlLogs.Visible = False
            Select Case rbPackType.SelectedValue
                Case "Core"
                    rowitems.Visible = False
                    txtFileName.Text = "Core"
                    lblFilenameFix.Text = Server.HtmlEncode(".<version>.<locale>.zip")
                    rowFileName.Visible = True
                Case "Module"
                    rowitems.Visible = True
                    lstItems.Items.Clear()
                    lstItems.ClearSelection()
                    For Each objDM As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(Null.NullInteger).Values
                        If Not objDM.FolderName.StartsWith("Admin/") Then
                            If Null.IsNull(objDM.Version) Then
                                lstItems.Items.Add(New ListItem(objDM.FriendlyName, objDM.DesktopModuleID))
                            Else
                                lstItems.Items.Add(New ListItem(objDM.FriendlyName + " [" + objDM.Version + "]", objDM.DesktopModuleID))
                            End If
                        End If
                    Next
                    lblItems.Text = Localization.GetString("SelectModules", LocalResourceFile)
                    rowFileName.Visible = False
                Case "Provider"
                    rowitems.Visible = True
                    lstItems.Items.Clear()
                    lstItems.ClearSelection()
                    For Each objPackage As PackageInfo In PackageController.GetPackagesByType("Provider")
                        If Null.IsNull(objPackage.Version) Then
                            lstItems.Items.Add(New ListItem(objPackage.FriendlyName, objPackage.PackageID))
                        Else
                            lstItems.Items.Add(New ListItem(objPackage.FriendlyName + " [" + FormatVersion(objPackage.Version) + "]", objPackage.PackageID))
                        End If
                    Next

                    rowFileName.Visible = False
                Case "AuthSystem"
                    rowitems.Visible = True
                    lstItems.Items.Clear()
                    lstItems.ClearSelection()
                    For Each objPackage As PackageInfo In PackageController.GetPackagesByType("Auth_System")
                        If Null.IsNull(objPackage.Version) Then
                            lstItems.Items.Add(New ListItem(objPackage.FriendlyName, objPackage.PackageID))
                        Else
                            lstItems.Items.Add(New ListItem(objPackage.FriendlyName + " [" + FormatVersion(objPackage.Version) + "]", objPackage.PackageID))
                        End If
                    Next

                    rowFileName.Visible = False
                Case "Full"
                    rowitems.Visible = False
                    txtFileName.Text = "Full"
                    lblFilenameFix.Text = Server.HtmlEncode(".<version>.<locale>.zip")
                    rowFileName.Visible = True
            End Select
        End Sub

        Private Sub cmdCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCreate.Click
            _IsPackCreated = True
            Try
                Select Case rbPackType.SelectedValue
                    Case "Core"
                        CreateCorePackage(True)
                    Case "Module"
                        For Each moduleItem As ListItem In lstItems.Items
                            If moduleItem.Selected Then
                                'Get the Module
                                Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModule(Integer.Parse(moduleItem.Value), Null.NullInteger)
                                CreateModulePackage(desktopModule, True)
                            End If
                        Next
                    Case "Provider"
                        For Each providerItem As ListItem In lstItems.Items
                            If providerItem.Selected Then
                                'Get the Provider
                                Dim provider As PackageInfo = PackageController.GetPackage(Integer.Parse(providerItem.Value))
                                CreateProviderPackage(provider, True)
                            End If
                        Next
                    Case "AuthSystem"
                        For Each authItem As ListItem In lstItems.Items
                            If authItem.Selected Then
                                'Get the AuthSystem
                                Dim authSystem As PackageInfo = PackageController.GetPackage(Integer.Parse(authItem.Value))
                                CreateAuthSystemPackage(authSystem, True)
                            End If
                        Next
                    Case "Full"
                        CreateFullPackage()
                End Select
                If _IsPackCreated = True Then
                    Skins.Skin.AddModuleMessage(Me, String.Format(Localization.GetString("Success", Me.LocalResourceFile), PortalSettings.PortalAlias.HTTPAlias), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                Else
                    Skins.Skin.AddModuleMessage(Me, Localization.GetString("Failure", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

    End Class
End Namespace