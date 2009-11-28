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
    ''' The ScriptComponentWriter class handles creating the manifest for Script
    ''' Component(s)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/11/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ScriptComponentWriter
        Inherits FileComponentWriter

#Region "Constructors"

        Public Sub New(ByVal basePath As String, ByVal scripts As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            MyBase.New(basePath, scripts, package)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("scripts")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/11/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "scripts"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Component Type ("Script")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/11/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ComponentType() As String
            Get
                Return "Script"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("script")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/11/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "script"
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Sub WriteFileElement(ByVal writer As XmlWriter, ByVal file As InstallFile)
            Log.AddInfo(String.Format(Util.WRITER_AddFileToManifest, file.Name))

            Dim type As String = "Install"
            Dim version As String = Null.NullString
            Dim fileName As String = Path.GetFileNameWithoutExtension(file.Name)
            If fileName.ToLower() = "uninstall" Then    '' UnInstall.SqlDataprovider
                type = "UnInstall"
                version = Package.Version.ToString(3)
            ElseIf fileName.ToLower() = "install" Then  '' Install.SqlDataprovider
                type = "Install"
                version = New Version(0, 0, 0).ToString(3)
            ElseIf fileName.StartsWith("Install") Then  '' Install.xx.xx.xx.SqlDataprovider
                type = "Install"
                version = fileName.Replace("Install.", "")
            Else                                        '' xx.xx.xx.SqlDataprovider
                type = "Install"
                version = fileName
            End If

            'Start file Element
            writer.WriteStartElement(ItemNodeName)
            writer.WriteAttributeString("type", type)

            'Write path
            If Not String.IsNullOrEmpty(file.Path) Then
                writer.WriteElementString("path", file.Path)
            End If

            'Write name
            writer.WriteElementString("name", file.Name)

            'Write sourceFileName
            If Not String.IsNullOrEmpty(file.SourceFileName) Then
                writer.WriteElementString("sourceFileName", file.SourceFileName)
            End If

            'Write Version
            writer.WriteElementString("version", version)

            'Close file Element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

