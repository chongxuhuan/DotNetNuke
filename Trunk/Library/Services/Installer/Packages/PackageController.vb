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

Imports System.Collections.Generic
Imports System.IO
Imports System.Xml.XPath
Imports DotNetNuke.UI.Skins
Imports System.Xml
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Authentication

Namespace DotNetNuke.Services.Installer.Packages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageController class provides the business class for the packages
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/26/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageController

#Region "Private Members"

        Private Shared provider As DataProvider = DataProvider.Instance()

#End Region

#Region "Public Shared Methods"

        Public Shared Function AddPackage(ByVal package As PackageInfo, ByVal includeDetail As Boolean) As Integer
            Dim packageID As Integer = provider.AddPackage(package.PortalID, package.Name, package.FriendlyName, package.Description, _
                                                           package.PackageType, package.Version.ToString(3), package.License, _
                                                           package.Manifest, package.Owner, package.Organization, package.Url, _
                                                           package.Email, package.ReleaseNotes, package.IsSystemPackage, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(package, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_CREATED)

            If includeDetail Then
                Select Case package.PackageType
                    Case "Auth_System"
                        'Create a new Auth System
                        Dim authSystem As New AuthenticationInfo
                        authSystem.AuthenticationType = package.Name
                        authSystem.IsEnabled = Null.NullBoolean
                        authSystem.PackageID = packageID
                        AuthenticationController.AddAuthentication(authSystem)
                    Case "Container", "Skin"
                        Dim skinPackage As New SkinPackageInfo
                        skinPackage.SkinName = package.Name
                        skinPackage.PackageID = packageID
                        skinPackage.SkinType = package.PackageType
                        SkinController.AddSkinPackage(skinPackage)
                    Case "CoreLanguagePack"
                        Dim locale As Locale = Localization.Localization.GetLocale(PortalController.GetCurrentPortalSettings.DefaultLanguage)

                        Dim languagePack As New LanguagePackInfo
                        languagePack.PackageID = packageID
                        languagePack.LanguageID = locale.LanguageID
                        languagePack.DependentPackageID = -2
                        LanguagePackController.SaveLanguagePack(languagePack)
                    Case "ExtensionLanguagePack"
                        Dim locale As Locale = Localization.Localization.GetLocale(PortalController.GetCurrentPortalSettings.DefaultLanguage)

                        Dim languagePack As New LanguagePackInfo
                        languagePack.PackageID = packageID
                        languagePack.LanguageID = locale.LanguageID
                        languagePack.DependentPackageID = Null.NullInteger
                        LanguagePackController.SaveLanguagePack(languagePack)
                    Case "Module"
                        'Create a new DesktopModule
                        Dim desktopModule As New DesktopModuleInfo
                        desktopModule.PackageID = packageID
                        desktopModule.ModuleName = package.Name
                        desktopModule.FriendlyName = package.FriendlyName
                        desktopModule.FolderName = package.Name
                        desktopModule.Description = package.Description
                        desktopModule.Version = package.Version.ToString(3)
                        desktopModule.SupportedFeatures = 0
                        Dim desktopModuleId As Integer = DesktopModuleController.SaveDesktopModule(desktopModule, False, True)
                        If desktopModuleId > Null.NullInteger Then
                            DesktopModuleController.AddDesktopModuleToPortals(desktopModuleId)
                        End If
                    Case "SkinObject"
                        Dim skinControl As New SkinControlInfo
                        skinControl.PackageID = packageID
                        skinControl.ControlKey = package.Name
                        SkinControlController.SaveSkinControl(skinControl)
                End Select
            End If

            Return packageID
        End Function

        Public Shared Function CanDeletePackage(ByVal package As PackageInfo, ByVal portalSettings As PortalSettings) As Boolean
            Dim bCanDelete As Boolean = True

            Select Case package.PackageType
                Case "Skin", "Container"
                    ''Need to get path of skin being deleted so we can call the public CanDeleteSkin function in the SkinController
                    Dim strFolderPath As String = String.Empty
                    Dim strRootSkin As String = IIf(package.PackageType = "Skin", SkinController.RootSkin, SkinController.RootContainer).ToString()
                    Dim _SkinPackageInfo As SkinPackageInfo = SkinController.GetSkinByPackageID(package.PackageID)

                    For Each kvp As System.Collections.Generic.KeyValuePair(Of Integer, String) In _SkinPackageInfo.Skins()
                        If kvp.Value.Contains(Common.Globals.HostMapPath) Then
                            strFolderPath = Path.Combine(Path.Combine(Common.Globals.HostMapPath, strRootSkin), _SkinPackageInfo.SkinName)
                        Else
                            strFolderPath = Path.Combine(Path.Combine(portalSettings.HomeDirectoryMapPath, strRootSkin), _SkinPackageInfo.SkinName)
                        End If

                        ''Only needed to look at the path of one skin/container, so exit loop
                        Exit For
                    Next

                    bCanDelete = SkinController.CanDeleteSkin(strFolderPath, portalSettings.HomeDirectoryMapPath)
                Case "Provider"
                    'Check if the provider is the default provider
                    Dim configDoc As XmlDocument = Config.Load()
                    Dim providerName As String = package.Name
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
                    Dim providerNavigator As XPathNavigator = configDoc.CreateNavigator.SelectSingleNode("/configuration/dotnetnuke/*[@defaultProvider='" & providerName & "']")

                    bCanDelete = (providerNavigator Is Nothing)
            End Select

            Return bCanDelete
        End Function

        Public Shared Sub DeletePackage(ByVal package As PackageInfo)
            Select Case package.PackageType
                Case "Auth_System"
                    Dim authSystem As Authentication.AuthenticationInfo = Authentication.AuthenticationController.GetAuthenticationServiceByPackageID(package.PackageID)
                    If authSystem IsNot Nothing Then
                        Authentication.AuthenticationController.DeleteAuthentication(authSystem)
                    End If
                Case "CoreLanguagePack"
                    Dim languagePack As LanguagePackInfo = Localization.LanguagePackController.GetLanguagePackByPackage(package.PackageID)
                    If languagePack IsNot Nothing Then
                        LanguagePackController.DeleteLanguagePack(languagePack)
                    End If
                Case "Module"
                    Dim controller As New DesktopModuleController()
                    Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByPackageID(package.PackageID)
                    If desktopModule IsNot Nothing Then
                        controller.DeleteDesktopModule(desktopModule)
                    End If
                Case "SkinObject"
                    Dim skinControl As SkinControlInfo = SkinControlController.GetSkinControlByPackageID(package.PackageID)
                    If skinControl IsNot Nothing Then
                        SkinControlController.DeleteSkinControl(skinControl)
                    End If
            End Select
            DeletePackage(package.PackageID)
        End Sub

        Public Shared Sub DeletePackage(ByVal packageID As Integer)
            provider.DeletePackage(packageID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("packageID", packageID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_DELETED)
        End Sub

        Public Shared Function GetPackage(ByVal packageID As Integer) As PackageInfo
            Return CBO.FillObject(Of PackageInfo)(provider.GetPackage(packageID))
        End Function

        Public Shared Function GetPackageByName(ByVal name As String) As PackageInfo
            Return GetPackageByName(Null.NullInteger, name)
        End Function

        Public Shared Function GetPackageByName(ByVal portalId As Integer, ByVal name As String) As PackageInfo
            Return CBO.FillObject(Of PackageInfo)(provider.GetPackageByName(portalId, name))
        End Function

        Public Shared Function GetPackages() As List(Of PackageInfo)
            Return GetPackages(Null.NullInteger)
        End Function

        Public Shared Function GetPackages(ByVal portalID As Integer) As List(Of PackageInfo)
            Return CBO.FillCollection(Of PackageInfo)(provider.GetPackages(portalID))
        End Function

        Public Shared Function GetPackagesByType(ByVal type As String) As List(Of PackageInfo)
            Return GetPackagesByType(Null.NullInteger, type)
        End Function

		Public Shared Function GetModulePackagesInUse(ByVal portalID As Integer, ByVal forHost As Boolean) As IDictionary(Of Integer, PackageInfo)
			Return CBO.FillDictionary(Of Integer, PackageInfo)("PackageID", provider.GetModulePackagesInUse(portalID, forHost))
		End Function

		Public Shared Function GetPackagesByType(ByVal portalID As Integer, ByVal type As String) As List(Of PackageInfo)
			Return CBO.FillCollection(Of PackageInfo)(provider.GetPackagesByType(portalID, type))
		End Function

        Public Shared Function GetPackageType(ByVal type As String) As PackageType
            Return CBO.FillObject(Of PackageType)(provider.GetPackageType(type))
        End Function

        Public Shared Function GetPackageTypes() As List(Of PackageType)
            Return CBO.FillCollection(Of PackageType)(provider.GetPackageTypes())
        End Function

        Public Shared Sub SavePackage(ByVal package As PackageInfo)
            If package.PackageID = Null.NullInteger Then
                package.PackageID = AddPackage(package, False)
            Else
                UpdatePackage(package)
            End If
        End Sub

        Public Shared Sub UpdatePackage(ByVal package As PackageInfo)
            provider.UpdatePackage(package.PortalID, package.Name, package.FriendlyName, package.Description, _
                                   package.PackageType, package.Version.ToString(3), package.License, _
                                   package.Manifest, package.Owner, package.Organization, package.Url, _
                                   package.Email, package.ReleaseNotes, package.IsSystemPackage, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(package, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.PACKAGE_UPDATED)
        End Sub

#End Region

    End Class

End Namespace
