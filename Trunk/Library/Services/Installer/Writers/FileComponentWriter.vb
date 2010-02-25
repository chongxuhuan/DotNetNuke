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

Imports System.Xml
Imports DotNetNuke.Services.Installer.Packages
Imports System.Collections.Generic
Imports DotNetNuke.Services.Installer.Log


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The FileComponentWriter class handles creating the manifest for File Component(s)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/01/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FileComponentWriter

#Region "Private Members"

        Private _BasePath As String
        Private _Files As Dictionary(Of String, InstallFile)
        Private _InstallOrder As Integer = Null.NullInteger
        Private _Package As PackageInfo
        Private _UnInstallOrder As Integer = Null.NullInteger

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the FileComponentWriter
        ''' </summary>
        ''' <param name="basePath">The Base Path for the files</param>
        ''' <param name="files">A Dictionary of files</param>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal basePath As String, ByVal files As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            _Files = files
            _BasePath = basePath
            _Package = package
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("files")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/01/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property CollectionNodeName() As String
            Get
                Return "files"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Component Type ("File")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/01/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property ComponentType() As String
            Get
                Return "File"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("file")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/01/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property ItemNodeName() As String
            Get
                Return "file"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Logger
        ''' </summary>
        ''' <value>A Logger</value>
        ''' <history>
        ''' 	[cnurse]	02/06/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property Log() As Logger
            Get
                Return _Package.Log
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Package
        ''' </summary>
        ''' <value>A PackageInfo</value>
        ''' <history>
        ''' 	[cnurse]	02/11/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property Package() As PackageInfo
            Get
                Return _Package
            End Get
        End Property


#End Region

#Region "Public Properties"

        Public Property InstallOrder() As Integer
            Get
                Return _InstallOrder
            End Get
            Set(ByVal value As Integer)
                _InstallOrder = value
            End Set
        End Property

        Public Property UnInstallOrder() As Integer
            Get
                Return _UnInstallOrder
            End Get
            Set(ByVal value As Integer)
                _UnInstallOrder = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The WriteCustomManifest method writes the custom manifest items (that subclasses
        ''' of FileComponentWriter may need)
        ''' </summary>
        ''' <param name="writer">The Xmlwriter to use</param>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub WriteCustomManifest(ByVal writer As XmlWriter)

        End Sub

        Protected Overridable Sub WriteFileElement(ByVal writer As XmlWriter, ByVal file As InstallFile)
            Log.AddInfo(String.Format(Util.WRITER_AddFileToManifest, File.Name))

            'Start file Element
            writer.WriteStartElement(ItemNodeName)

            'Write path
            If Not String.IsNullOrEmpty(file.Path) Then
                Dim path As String = file.Path
                If Not String.IsNullOrEmpty(_BasePath) Then
                    If file.Path.ToLowerInvariant.Contains(_BasePath.ToLowerInvariant()) Then
                        path = file.Path.ToLowerInvariant().Replace(_BasePath.ToLowerInvariant() & "\", "")
                    End If
                End If
                writer.WriteElementString("path", path)
            End If

            'Write name
            writer.WriteElementString("name", file.Name)

            'Write sourceFileName
            If Not String.IsNullOrEmpty(file.SourceFileName) Then
                writer.WriteElementString("sourceFileName", file.SourceFileName)
            End If

            'Close file Element
            writer.WriteEndElement()
        End Sub

#End Region

#Region "Public Methods"

        Public Overridable Sub WriteManifest(ByVal writer As XmlWriter)
            'Start component Element
            writer.WriteStartElement("component")
            writer.WriteAttributeString("type", ComponentType)
            If InstallOrder > Null.NullInteger Then
                writer.WriteAttributeString("installOrder", InstallOrder.ToString())
            End If
            If UnInstallOrder > Null.NullInteger Then
                writer.WriteAttributeString("unInstallOrder", UnInstallOrder.ToString())
            End If

            'Start files element
            writer.WriteStartElement(CollectionNodeName)

            'Write custom manifest items
            WriteCustomManifest(writer)

            'Write basePath Element
            If Not String.IsNullOrEmpty(_BasePath) Then
                writer.WriteElementString("basePath", _BasePath)
            End If

            For Each file As InstallFile In _Files.Values
                WriteFileElement(writer, file)
            Next

            'End files Element
            writer.WriteEndElement()

            'End component Element
            writer.WriteEndElement()
        End Sub

#End Region

    End Class

End Namespace

