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

Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.UI.Skins


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ContainerPackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ContainerPackageWriter
        Inherits SkinPackageWriter


#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            BasePath = "Portals\_default\Containers\" + SkinPackage.SkinName
        End Sub

        Public Sub New(ByVal skinPackage As SkinPackageInfo, ByVal package As PackageInfo)
            MyBase.New(skinPackage, package)
            BasePath = "Portals\_default\Containers\" + skinPackage.SkinName
        End Sub

#End Region

#Region "Public Properties"


#End Region

        Protected Overrides Sub WriteFilesToManifest(ByVal writer As System.Xml.XmlWriter)
            Dim containerFileWriter As New ContainerComponentWriter(SkinPackage.SkinName, BasePath, Files, Package)
            containerFileWriter.WriteManifest(writer)
        End Sub

    End Class

End Namespace

