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
    ''' The ContainerComponentWriter class handles creating the manifest for Container
    ''' Component(s)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/04/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ContainerComponentWriter
        Inherits SkinComponentWriter

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the ContainerComponentWriter
        ''' </summary>
        ''' <param name="containerName">The name of the Container</param>
        ''' <param name="basePath">The Base Path for the files</param>
        ''' <param name="files">A Dictionary of files</param>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal containerName As String, ByVal basePath As String, ByVal files As Dictionary(Of String, InstallFile), ByVal package As PackageInfo)
            MyBase.New(containerName, basePath, files, package)
        End Sub

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("containerFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "containerFiles"
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
                Return "Container"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("containerFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "containerFile"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the SkinName Node ("containerName")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/04/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property SkinNameNodeName() As String
            Get
                Return "containerName"
            End Get
        End Property

#End Region

    End Class

End Namespace

