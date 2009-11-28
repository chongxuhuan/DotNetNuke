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
Imports DotNetNuke.Entities.Modules.Definitions


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinControlPackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/28/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinControlPackageWriter
        Inherits PackageWriterBase

#Region "Private Members"

        Private _SkinControl As SkinControlInfo

#End Region

#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            _SkinControl = SkinControlController.GetSkinControlByPackageID(package.PackageID)
            BasePath = Path.Combine("DesktopModules", package.Name.ToLower).Replace("/", "\")
            AppCodePath = Path.Combine("App_Code", package.Name.ToLower).Replace("/", "\")
        End Sub

        Public Sub New(ByVal skinControl As SkinControlInfo, ByVal package As PackageInfo)
            MyBase.New(package)
            _SkinControl = skinControl
            BasePath = Path.Combine("DesktopModules", package.Name.ToLower).Replace("/", "\")
            AppCodePath = Path.Combine("App_Code", package.Name.ToLower).Replace("/", "\")
        End Sub

        Public Sub New(ByVal manifestNav As XPathNavigator, ByVal installer As InstallerInfo)
            _SkinControl = New SkinControlInfo

            'Create a Package
            Package = New PackageInfo(installer)

            ReadLegacyManifest(manifestNav, True)

            Package.Description = Null.NullString
            Package.Version = New Version(1, 0, 0)
            Package.PackageType = "SkinObject"
            Package.License = Util.PACKAGE_NoLicense

            BasePath = Path.Combine("DesktopModules", Package.Name.ToLower).Replace("/", "\")
            AppCodePath = Path.Combine("App_Code", Package.Name.ToLower).Replace("/", "\")
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the associated SkinControl
        ''' </summary>
        ''' <value>A SkinControlInfo object</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SkinControl() As SkinControlInfo
            Get
                Return _SkinControl
            End Get
            Set(ByVal value As SkinControlInfo)
                _SkinControl = value
            End Set
        End Property

#End Region

        Private Sub ReadLegacyManifest(ByVal legacyManifest As XPathNavigator, ByVal processModule As Boolean)
            Dim folderNav As XPathNavigator = legacyManifest.SelectSingleNode("folders/folder")

            If processModule Then
                Package.Name = Util.ReadElement(folderNav, "name")
                Package.FriendlyName = Package.Name

                'Process legacy controls Node
                For Each controlNav As XPathNavigator In folderNav.Select("modules/module/controls/control")

                    SkinControl.ControlKey = Util.ReadElement(controlNav, "key")
                    SkinControl.ControlSrc = Path.Combine(Path.Combine("DesktopModules", Package.Name.ToLower), Util.ReadElement(controlNav, "src")).Replace("\", "/")
                    Dim supportsPartialRendering As String = Util.ReadElement(controlNav, "supportspartialrendering")
                    If Not String.IsNullOrEmpty(supportsPartialRendering) Then
                        SkinControl.SupportsPartialRendering = Boolean.Parse(supportsPartialRendering)
                    End If
                Next
            End If

            'Process legacy files Node
            For Each fileNav As XPathNavigator In folderNav.Select("files/file")
                Dim fileName As String = Util.ReadElement(fileNav, "name")
                Dim filePath As String = Util.ReadElement(fileNav, "path")

                AddFile(Path.Combine(filePath, fileName), fileName)
            Next

            'Process resource file Node
            If Not String.IsNullOrEmpty(Util.ReadElement(folderNav, "resourcefile")) Then
                AddFile(Util.ReadElement(folderNav, "resourcefile"))
            End If

        End Sub


#Region "Protected Methods"

        Protected Overrides Sub WriteManifestComponent(ByVal writer As System.Xml.XmlWriter)
            'Start component Element
            writer.WriteStartElement("component")
            writer.WriteAttributeString("type", "SkinObject")

            'Write SkinControl Manifest
            CBO.SerializeObject(SkinControl, writer)

            'End component Element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

