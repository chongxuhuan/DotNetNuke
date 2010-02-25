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

Imports DotNetNuke.Services.Installer.Dependencies
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageInstaller class is an Installer for Packages
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/16/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageInstaller
        Inherits ComponentInstallerBase

#Region "Private Members"

        Private InstalledPackage As PackageInfo
        Private ComponentInstallers As New SortedList(Of Integer, ComponentInstallerBase)

        Private _DeleteFiles As Boolean = Null.NullBoolean
        Private _IsValid As Boolean = True

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new PackageInstaller instance
        ''' </summary>
        ''' <param name="package">A PackageInfo instance</param>
        ''' <history>
        ''' 	[cnurse]	01/21/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal package As PackageInfo)
            Me.Package = package

            If Not String.IsNullOrEmpty(package.Manifest) Then
                'Create an XPathDocument from the Xml
                Dim doc As New XPathDocument(New StringReader(package.Manifest))
                Dim nav As XPathNavigator = doc.CreateNavigator().SelectSingleNode("package")
                ReadComponents(nav)
            Else
                Dim installer As ComponentInstallerBase = InstallerFactory.GetInstaller(package.PackageType)
                If installer IsNot Nothing Then
                    'Set package
                    installer.Package = package

                    'Set type
                    installer.Type = package.PackageType
                    ComponentInstallers.Add(0, installer)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This Constructor creates a new PackageInstaller instance
        ''' </summary>
        ''' <param name="info">An InstallerInfo instance</param>
        ''' <param name="packageManifest">The manifest as a string</param>
        ''' <history>
        ''' 	[cnurse]	01/16/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal packageManifest As String, ByVal info As InstallerInfo)
            Package = New PackageInfo(info)
            Package.Manifest = packageManifest

            If Not String.IsNullOrEmpty(packageManifest) Then
                'Create an XPathDocument from the Xml
                Dim doc As New XPathDocument(New StringReader(packageManifest))
                Dim nav As XPathNavigator = doc.CreateNavigator().SelectSingleNode("package")
                ReadManifest(nav)
            End If
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Packages files are deleted when uninstalling the
        ''' package
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DeleteFiles() As Boolean
            Get
                Return _DeleteFiles
            End Get
            Set(ByVal value As Boolean)
                _DeleteFiles = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Package is Valid
        ''' </summary>
        ''' <value>A Boolean value</value>
        ''' <history>
        ''' 	[cnurse]	01/16/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return _IsValid
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CheckSecurity method checks whether the user has the appropriate security
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	09/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckSecurity()
            Dim type As PackageType = PackageController.GetPackageType(Package.PackageType)

            If type Is Nothing Then
                'This package type not registered
                Log.Logs.Clear()
                Log.AddFailure(Util.SECURITY_NotRegistered + " - " + Package.PackageType)
                _IsValid = False
            Else
                If type.SecurityAccessLevel > Package.InstallerInfo.SecurityAccessLevel Then
                    Log.Logs.Clear()
                    Log.AddFailure(Util.SECURITY_Installer)
                    _IsValid = False
                End If

            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadComponents method reads the components node of the manifest file.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/21/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ReadComponents(ByVal manifestNav As XPathNavigator)
            'Parse the component nodes
            For Each componentNav As XPathNavigator In manifestNav.CreateNavigator().Select("components/component")
                'Set default order to next value (ie the same as the size of the collection)
                Dim order As Integer = ComponentInstallers.Count

                Dim type As String = componentNav.GetAttribute("type", "")

                If InstallMode = InstallMode.Install Then
                    Dim installOrder As String = componentNav.GetAttribute("installOrder", "")
                    If Not String.IsNullOrEmpty(installOrder) Then
                        order = Integer.Parse(installOrder)
                    End If
                Else
                    Dim unInstallOrder As String = componentNav.GetAttribute("unInstallOrder", "")
                    If Not String.IsNullOrEmpty(unInstallOrder) Then
                        order = Integer.Parse(unInstallOrder)
                    End If
                End If

                If Package.InstallerInfo IsNot Nothing Then
                    Log.AddInfo(Util.DNN_ReadingComponent + " - " + type)
                End If

                Dim installer As ComponentInstallerBase = InstallerFactory.GetInstaller(componentNav, Package)
                If installer Is Nothing Then
                    Log.AddFailure(Util.EXCEPTION_InstallerCreate)
                Else
                    ComponentInstallers.Add(order, installer)
                    Me.Package.InstallerInfo.AllowableFiles += ", " + installer.AllowableFiles
                End If
            Next
        End Sub

        Private Function ReadTextFromFile(ByVal source As String) As String
            Dim strText As String = Null.NullString
            If Package.InstallerInfo.InstallMode <> Services.Installer.InstallMode.ManifestOnly Then
                'Load from file
                strText = FileSystemUtils.ReadFile(Package.InstallerInfo.TempInstallFolder + "\" + source)
            End If
            Return strText
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ValidateVersion method checks whether the package is already installed
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ValidateVersion(ByVal strVersion As String)
            If String.IsNullOrEmpty(strVersion) Then
                _IsValid = False
                Exit Sub
            End If

            Package.Version = New System.Version(strVersion)
            If InstalledPackage IsNot Nothing Then
                Package.InstalledVersion = InstalledPackage.Version
                If Package.InstalledVersion > Package.Version Then
                    Log.AddFailure(Util.INSTALL_Version + " - " + Package.InstalledVersion.ToString(3))
                    _IsValid = False
                ElseIf Package.InstalledVersion = Package.Version Then
                    Package.InstallerInfo.Installed = True
                    Package.InstallerInfo.PortalID = InstalledPackage.PortalID
                End If
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method commits the package installation
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/01/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
            For index As Integer = 0 To ComponentInstallers.Count - 1
                Dim compInstaller As ComponentInstallerBase = ComponentInstallers.Values(index)
                If compInstaller.Version > Package.InstalledVersion AndAlso compInstaller.Completed Then
                    compInstaller.Commit()
                End If
            Next
            If Log.Valid Then
                Log.AddInfo(Util.INSTALL_Committed)
            Else
                Log.AddFailure(Util.INSTALL_Aborted)
            End If

            Package.InstallerInfo.PackageID = Package.PackageID
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the components of the package
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Dim isCompleted As Boolean = True

            Try
                'Save the Package Information
                If InstalledPackage IsNot Nothing Then
                    Package.PackageID = InstalledPackage.PackageID
                End If

                'Save Package
                PackageController.SavePackage(Package)

                'Iterate through all the Components
                For index As Integer = 0 To ComponentInstallers.Count - 1
                    Dim compInstaller As ComponentInstallerBase = ComponentInstallers.Values(index)
                    If (InstalledPackage Is Nothing) OrElse (compInstaller.Version > Package.InstalledVersion) _
                                    OrElse (Package.InstallerInfo.RepairInstall) Then
                        Log.AddInfo(Util.INSTALL_Start + " - " + compInstaller.Type)
                        compInstaller.Install()
                        If compInstaller.Completed Then
                            Log.AddInfo(Util.COMPONENT_Installed + " - " + compInstaller.Type)
                        Else
                            Log.AddFailure(Util.INSTALL_Failed + " - " + compInstaller.Type)
                            isCompleted = False
                            Exit For
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.AddFailure(Util.INSTALL_Aborted + " - " + Package.Name)
            End Try

            If isCompleted Then
                'All components successfully installed so Commit any pending changes
                Commit()
            Else
                'There has been a failure so Rollback
                Rollback()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file and parses it into components.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)

            'Get Name Property
            Package.Name = Util.ReadAttribute(manifestNav, "name", Log, Util.EXCEPTION_NameMissing)

            'Get Type
            Package.PackageType = Util.ReadAttribute(manifestNav, "type", Log, Util.EXCEPTION_TypeMissing)

            'If Skin or Container then set PortalID
            If Package.PackageType = "Skin" OrElse Package.PackageType = "Container" Then
                Package.PortalID = Package.InstallerInfo.PortalID
            End If
            CheckSecurity()
            If Not IsValid Then
                Exit Sub
            End If

            'Attempt to get the Package from the Data Store (see if its installed)
            InstalledPackage = PackageController.GetPackageByName(Package.PortalID, Package.Name)

            'Get IsSystem
            Package.IsSystemPackage = Boolean.Parse(Util.ReadAttribute(manifestNav, "isSystem", False, Log, "", Boolean.FalseString))

            'Get Version
            Dim strVersion As String = Util.ReadAttribute(manifestNav, "version", Log, Util.EXCEPTION_VersionMissing)
            ValidateVersion(strVersion)
            If Not IsValid Then
                Exit Sub
            End If

            Log.AddInfo(Util.DNN_ReadingPackage + " - " + Package.PackageType + " - " + Package.Name)

            'Get Friendly Name
            Package.FriendlyName = Util.ReadElement(manifestNav, "friendlyName", Package.Name)

            'Get Description
            Package.Description = Util.ReadElement(manifestNav, "description")

            'Get Author
            Dim authorNav As XPathNavigator = manifestNav.SelectSingleNode("owner")
            If authorNav IsNot Nothing Then
                Package.Owner = Util.ReadElement(authorNav, "name")
                Package.Organization = Util.ReadElement(authorNav, "organization")
                Package.Url = Util.ReadElement(authorNav, "url")
                Package.Email = Util.ReadElement(authorNav, "email")
            End If

            'Get License
            Dim licenseNav As XPathNavigator = manifestNav.SelectSingleNode("license")
            If licenseNav IsNot Nothing Then
                Dim licenseSrc As String = Util.ReadAttribute(licenseNav, "src")
                If String.IsNullOrEmpty(licenseSrc) Then
                    'Load from element
                    Package.License = licenseNav.Value
                Else
                    Package.License = ReadTextFromFile(licenseSrc)
                End If
            End If

            If String.IsNullOrEmpty(Package.License) Then
                'Legacy Packages have no license
                Package.License = Util.PACKAGE_NoLicense
            End If

            'Get Release Notes
            Dim relNotesNav As XPathNavigator = manifestNav.SelectSingleNode("releaseNotes")
            If relNotesNav IsNot Nothing Then
                Dim relNotesSrc As String = Util.ReadAttribute(relNotesNav, "src")
                If String.IsNullOrEmpty(relNotesSrc) Then
                    'Load from element
                    Package.ReleaseNotes = relNotesNav.Value
                Else
                    Package.ReleaseNotes = ReadTextFromFile(relNotesSrc)
                End If
            End If

            If String.IsNullOrEmpty(Package.License) Then
                'Legacy Packages have no Release Notes
                Package.License = Util.PACKAGE_NoReleaseNotes
            End If

            'Parse the Dependencies
            Dim dependency As IDependency = Nothing
            For Each dependencyNav As XPathNavigator In manifestNav.CreateNavigator().Select("dependencies/dependency")
                dependency = DependencyFactory.GetDependency(dependencyNav)
                If Not dependency.IsValid Then
                    Log.AddFailure(dependency.ErrorMessage)
                    Exit Sub
                End If
            Next

            'Read Components
            ReadComponents(manifestNav)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method rolls back the package installation
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            For index As Integer = 0 To ComponentInstallers.Count - 1
                Dim compInstaller As ComponentInstallerBase = ComponentInstallers.Values(index)
                If compInstaller.Version > Package.InstalledVersion AndAlso compInstaller.Completed Then
                    Log.AddInfo(Util.COMPONENT_RollingBack + " - " + compInstaller.Type)
                    compInstaller.Rollback()
                    Log.AddInfo(Util.COMPONENT_RolledBack + " - " + compInstaller.Type)
                End If
            Next

            'If Previously Installed Package exists then we need to update the DataStore with this 
            If InstalledPackage Is Nothing Then
                'No Previously Installed Package - Delete newly added Package
                PackageController.DeletePackage(Package)
            Else
                'Previously Installed Package - Rollback to Previously Installed
                PackageController.SavePackage(InstalledPackage)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Uninstall method uninstalls the components of the package
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            'Iterate through all the Components
            For index As Integer = 0 To ComponentInstallers.Count - 1
                Dim compInstaller As ComponentInstallerBase = ComponentInstallers.Values(index)
                Dim fileInstaller As FileInstaller = TryCast(compInstaller, FileInstaller)

                If fileInstaller IsNot Nothing Then
                    fileInstaller.DeleteFiles = DeleteFiles
                End If

                Log.ResetFlags()
                Log.AddInfo(Util.UNINSTALL_StartComp + " - " + compInstaller.Type)
                compInstaller.UnInstall()
                Log.AddInfo(Util.COMPONENT_UnInstalled + " - " + compInstaller.Type)

                If Log.Valid Then
                    Log.AddInfo(Util.UNINSTALL_SuccessComp + " - " + compInstaller.Type)
                Else
                    Log.AddWarning(Util.UNINSTALL_WarningsComp + " - " + compInstaller.Type)
                End If
            Next

            'Remove the Package information from the Data Store
            PackageController.DeletePackage(Package)
        End Sub

#End Region

    End Class

End Namespace
