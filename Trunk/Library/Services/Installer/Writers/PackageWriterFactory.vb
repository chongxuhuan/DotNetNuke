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

Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Services.Installer.Writers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PackageWriterFactory is a factory class that is used to instantiate the
    ''' appropriate Package Writer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/31/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PackageWriterFactory

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetWriter method instantiates the relevant PackageWriter Installer
        ''' </summary>
        ''' <param name="package">The associated PackageInfo instance</param>
        ''' <history>
        ''' 	[cnurse]	01/31/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetWriter(ByVal package As PackageInfo) As PackageWriterBase
            Dim writer As PackageWriterBase = Nothing

            Select Case package.PackageType
                Case "Auth_System"
                    writer = New AuthenticationPackageWriter(package)
                Case "Module"
                    writer = New ModulePackageWriter(package)
                Case "Container"
                    writer = New ContainerPackageWriter(package)
                Case "Skin"
                    writer = New SkinPackageWriter(package)
                Case "CoreLanguagePack", "ExtensionLanguagePack"
                    writer = New LanguagePackWriter(package)
                Case "SkinObject"
                    writer = New SkinControlPackageWriter(package)
                Case "Provider"
                    writer = New ProviderPackageWriter(package)
                Case "Library"
                    writer = New LibraryPackageWriter(package)
                Case "Widget"
                    writer = New WidgetPackageWriter(package)
                Case Else
                    'PackageType is defined in the List
                    Dim listController As New Lists.ListController()
                    Dim entry As ListEntryInfo = listController.GetListEntryInfo("PackageWriter", package.PackageType)

                    If entry IsNot Nothing AndAlso Not String.IsNullOrEmpty(entry.Text) Then
                        'The class for the Installer is specified in the Text property
                        writer = CType(Reflection.CreateObject(entry.Text, "PackageWriter_" + entry.Value), PackageWriterBase)
                    End If
            End Select

            Return writer

        End Function

#End Region

    End Class

End Namespace
