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

Imports DotNetNuke.Services.Installer.Packages
Imports System.Collections.Generic


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinComponentWriter class handles creating the manifest for Skin Component(s)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/04/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinComponentWriter
        Inherits FileComponentWriter

#Region "Private Members"

        Private _SkinName As String

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the SkinComponentWriter
        ''' </summary>
        ''' <param name="skinName">The name of the Skin</param>
        ''' <param name="basePath">The Base Path for the files</param>
        ''' <param name="files">A Dictionary of files</param>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal skinName As String, ByVal basePath As String, ByVal files As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            MyBase.New(basePath, files, package)
            _SkinName = skinName
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("skinFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "skinFiles"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Component Type ("Skin")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ComponentType() As String
            Get
                Return "Skin"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("skinFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "skinFile"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the SkinName Node ("skinName")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property SkinNameNodeName() As String
            Get
                Return "skinName"
            End Get
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
        Protected Overrides Sub WriteCustomManifest(ByVal writer As System.Xml.XmlWriter)
            'Write basePath Element
            writer.WriteElementString(SkinNameNodeName, _SkinName)
        End Sub

#End Region

    End Class

End Namespace

