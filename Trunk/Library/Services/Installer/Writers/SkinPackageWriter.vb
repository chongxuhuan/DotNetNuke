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

Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Skins


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinPackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/30/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinPackageWriter
        Inherits PackageWriterBase

#Region "Private Members"

        Private _SkinPackage As SkinPackageInfo
        Private _SubFolder As String

#End Region

#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            _SkinPackage = SkinController.GetSkinByPackageID(package.PackageID)
            SetBasePath()
        End Sub

        Public Sub New(ByVal skinPackage As SkinPackageInfo, ByVal package As PackageInfo)
            MyBase.New(package)
            _SkinPackage = skinPackage
            SetBasePath()
        End Sub

        Public Sub New(ByVal skinPackage As SkinPackageInfo, ByVal package As PackageInfo, ByVal basePath As String)
            MyBase.New(package)
            _SkinPackage = skinPackage
            Me.BasePath = basePath
        End Sub

        Public Sub New(ByVal skinPackage As SkinPackageInfo, ByVal package As PackageInfo, ByVal basePath As String, ByVal subFolder As String)
            MyBase.New(package)
            _SkinPackage = skinPackage
            _SubFolder = subFolder
            Me.BasePath = Path.Combine(basePath, subFolder)
        End Sub

#End Region

#Region "Protected Properties"

        Public Overrides ReadOnly Property IncludeAssemblies() As Boolean
            Get
                Return False
            End Get
        End Property

        Protected ReadOnly Property SkinPackage() As SkinPackageInfo
            Get
                Return _SkinPackage
            End Get
        End Property

#End Region

        Public Sub SetBasePath()
            If _SkinPackage.SkinType = "Skin" Then
                BasePath = Path.Combine("Portals\_default\Skins", SkinPackage.SkinName)
            Else
                BasePath = Path.Combine("Portals\_default\Containers", SkinPackage.SkinName)
            End If
        End Sub

        Protected Overrides Sub GetFiles(ByVal includeSource As Boolean, ByVal includeAppCode As Boolean)
            'Call base class method with includeAppCode = false
            MyBase.GetFiles(includeSource, False)
        End Sub

        Protected Overrides Sub ParseFiles(ByVal folder As System.IO.DirectoryInfo, ByVal rootPath As String)
            'Add the Files in the Folder
            Dim files As FileInfo() = folder.GetFiles()
            For Each file As FileInfo In files
                Dim filePath As String = folder.FullName.Replace(rootPath, "")
                If filePath.StartsWith("\") Then
                    filePath = filePath.Substring(1)
                End If
                If file.Extension.ToLowerInvariant() <> ".dnn" Then
                    If String.IsNullOrEmpty(_SubFolder) Then
                        AddFile(Path.Combine(filePath, file.Name))
                    Else
                        filePath = Path.Combine(filePath, file.Name)
                        AddFile(filePath, Path.Combine(_SubFolder, filePath))
                    End If
                End If
            Next
        End Sub

        Protected Overrides Sub WriteFilesToManifest(ByVal writer As System.Xml.XmlWriter)
            Dim skinFileWriter As New SkinComponentWriter(SkinPackage.SkinName, BasePath, Files, Package)

            If SkinPackage.SkinType = "Skin" Then
                skinFileWriter = New SkinComponentWriter(SkinPackage.SkinName, BasePath, Files, Package)
            Else
                skinFileWriter = New ContainerComponentWriter(SkinPackage.SkinName, BasePath, Files, Package)
            End If
            skinFileWriter.WriteManifest(writer)
        End Sub

    End Class

End Namespace

