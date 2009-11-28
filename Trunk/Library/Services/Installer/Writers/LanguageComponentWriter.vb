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
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The LanguageComponentWriter class handles creating the manifest for Language
    ''' Component(s)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/08/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LanguageComponentWriter
        Inherits FileComponentWriter

#Region "Private Members"

        Private _DependentPackageID As Integer
        Private _Language As Locale
        Private _PackageType As LanguagePackType

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the LanguageComponentWriter
        ''' </summary>
        ''' <param name="files">A Dictionary of files</param>
        ''' <history>
        ''' 	[cnurse]	02/08/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal language As Locale, ByVal basePath As String, ByVal files As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            MyBase.New(basePath, files, package)
            _Language = language
            _PackageType = LanguagePackType.Core
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the LanguageComponentWriter
        ''' </summary>
        ''' <param name="files">A Dictionary of files</param>
        ''' <history>
        ''' 	[cnurse]	02/08/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal languagePack As LanguagePackInfo, ByVal basePath As String, ByVal files As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            MyBase.New(basePath, files, package)
            _Language = Localization.Localization.GetLocaleByID(languagePack.LanguageID)
            _PackageType = languagePack.PackageType
            _DependentPackageID = languagePack.DependentPackageID
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("languageFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/08/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "languageFiles"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Component Type ("CoreLanguage/ExtensionLanguage")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/08/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ComponentType() As String
            Get
                If _PackageType = LanguagePackType.Core Then
                    Return "CoreLanguage"
                Else
                    Return "ExtensionLanguage"
                End If
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("languageFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/08/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "languageFile"
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The WriteCustomManifest method writes the custom manifest items
        ''' </summary>
        ''' <param name="writer">The Xmlwriter to use</param>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub WriteCustomManifest(ByVal writer As System.Xml.XmlWriter)
            'Write language Elements
            writer.WriteElementString("code", _Language.Code)

            If _PackageType = LanguagePackType.Core Then
                writer.WriteElementString("displayName", _Language.Text)
                writer.WriteElementString("fallback", _Language.Fallback)
            Else
                Dim package As PackageInfo = PackageController.GetPackage(_DependentPackageID)
                writer.WriteElementString("package", package.Name)
            End If
        End Sub

#End Region

    End Class

End Namespace

