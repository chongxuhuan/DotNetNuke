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

Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Skins


Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The WidgetPackageWriter class 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	11/24/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WidgetPackageWriter
        Inherits PackageWriterBase

#Region "Constructors"

        Public Sub New(ByVal package As PackageInfo)
            MyBase.New(package)
            Dim company As String = package.Name.Substring(0, package.Name.IndexOf("."))
            BasePath = Path.Combine("Resources\Widgets\User", company)
        End Sub
#End Region

#Region "Public Properties"

        Public Overrides ReadOnly Property IncludeAssemblies() As Boolean
            Get
                Return False
            End Get
        End Property

#End Region

        Protected Overrides Sub GetFiles(ByVal includeSource As Boolean, ByVal includeAppCode As Boolean)
            'Call base class method with includeAppCode = false
            MyBase.GetFiles(includeSource, False)
        End Sub

        Protected Overrides Sub WriteFilesToManifest(ByVal writer As System.Xml.XmlWriter)
            Dim company As String = Package.Name.Substring(0, Package.Name.IndexOf("."))
            Dim widgetFileWriter As New WidgetComponentWriter(company, Files, Package)
            widgetFileWriter.WriteManifest(writer)
        End Sub

    End Class

End Namespace

