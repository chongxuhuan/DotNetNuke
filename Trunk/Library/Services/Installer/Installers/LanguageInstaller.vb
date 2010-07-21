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
Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LanguageInstaller installs Language Packs to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/29/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LanguageInstaller
        Inherits FileInstaller

#Region "Private Members"

        Private Language As Locale
        Private TempLanguage As Locale
        Private InstalledLanguagePack As LanguagePackInfo
        Private LanguagePack As LanguagePackInfo
        Private LanguagePackType As LanguagePackType

#End Region

        Public Sub New(ByVal type As LanguagePackType)
            LanguagePackType = type
        End Sub

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("languageFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/29/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "languageFiles"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("languageFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/29/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "languageFile"
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "resx, xml"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteLanguage method deletes the Language
        ''' from the data Store.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteLanguage()
            Try
                'Attempt to get the LanguagePack
                Dim tempLanguagePack As LanguagePackInfo = LanguagePackController.GetLanguagePackByPackage(Package.PackageID)

                'Attempt to get the Locale
                Dim language As Locale = LocaleController.Instance().GetLocale(tempLanguagePack.LanguageID)

                If tempLanguagePack IsNot Nothing Then
                    LanguagePackController.DeleteLanguagePack(tempLanguagePack)
                End If

                If language IsNot Nothing AndAlso tempLanguagePack.PackageType = Localization.LanguagePackType.Core Then
                    Localization.Localization.DeleteLanguage(language)
                End If
                Log.AddInfo(String.Format(Util.LANGUAGE_UnRegistered, language.Text))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ProcessFile method determines what to do with parsed "file" node
        ''' </summary>
        ''' <param name="file">The file represented by the node</param>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ProcessFile(ByVal file As InstallFile, ByVal nav As System.Xml.XPath.XPathNavigator)
            'Call base method to set up for file processing
            MyBase.ProcessFile(file, nav)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadCustomManifest method reads the custom manifest items
        ''' </summary>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ReadCustomManifest(ByVal nav As XPathNavigator)

            Language = New Locale()
            LanguagePack = New LanguagePackInfo()

            'Get the Skin name
            Language.Code = Util.ReadElement(nav, "code")
            Language.Text = Util.ReadElement(nav, "displayName")
            Language.Fallback = Util.ReadElement(nav, "fallback")

            If LanguagePackType = Localization.LanguagePackType.Core Then
                LanguagePack.DependentPackageID = -2
            Else
                Dim packageName As String = Util.ReadElement(nav, "package")
                Dim package As PackageInfo = PackageController.GetPackageByName(packageName)
                LanguagePack.DependentPackageID = package.PackageID
            End If

            'Call base class
            MyBase.ReadCustomManifest(nav)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <remarks>In the case of Modules this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	01/15/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the language component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Try
                'Attempt to get the LanguagePack
                InstalledLanguagePack = LanguagePackController.GetLanguagePackByPackage(Package.PackageID)
                If InstalledLanguagePack IsNot Nothing Then
                    LanguagePack.LanguagePackID = InstalledLanguagePack.LanguagePackID
                End If

                'Attempt to get the Locale
                TempLanguage = LocaleController.Instance().GetLocale(Language.Code)

                If TempLanguage IsNot Nothing Then
                    Language.LanguageID = TempLanguage.LanguageID
                End If

                If LanguagePack.PackageType = Localization.LanguagePackType.Core Then
                    'Update language
                    Localization.Localization.SaveLanguage(Language)
                End If

                Dim _settings As PortalSettings = PortalController.GetCurrentPortalSettings()
                If _settings IsNot Nothing Then
                    Dim enabledLanguage As Locale = Nothing
                    If Not LocaleController.Instance().GetLocales(_settings.PortalId).TryGetValue(Language.Code, enabledLanguage) Then
                        'Add language to portal
                        Localization.Localization.AddLanguageToPortal(_settings.PortalId, Language.LanguageId, True)
                    End If
                End If

                'Set properties for Language Pack
                LanguagePack.PackageID = Package.PackageID
                LanguagePack.LanguageID = Language.LanguageID

                'Update LanguagePack
                LanguagePackController.SaveLanguagePack(LanguagePack)

                Log.AddInfo(String.Format(Util.LANGUAGE_Registered, Language.Text))

                'install (copy the files) by calling the base class
                MyBase.Install()

                Completed = True
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'If Temp Language exists then we need to update the DataStore with this 
            If TempLanguage Is Nothing Then
                'No Temp Language - Delete newly added Language
                DeleteLanguage()
            Else
                'Temp Language - Rollback to Temp
                Localization.Localization.SaveLanguage(TempLanguage)
            End If

            'Call base class to prcoess files
            MyBase.Rollback()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the language component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            DeleteLanguage()

            'Call base class to prcoess files
            MyBase.UnInstall()
        End Sub

#End Region

    End Class

End Namespace
