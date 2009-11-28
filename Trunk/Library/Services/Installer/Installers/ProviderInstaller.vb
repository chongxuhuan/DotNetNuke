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

Imports System.Xml.XPath

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ProviderInstaller installs Provider Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	05/29/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ProviderInstaller
        Inherits ComponentInstallerBase

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, xml, htc, html, htm, text, vbproj, csproj, sln"
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Overrides Sub Commit()
            Completed = True
        End Sub

        Public Overrides Sub Install()
            Completed = True
        End Sub

        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)
        End Sub

        Public Overrides Sub Rollback()
            Completed = True
        End Sub

        Public Overrides Sub UnInstall()
            Completed = True
        End Sub

#End Region

    End Class

End Namespace
