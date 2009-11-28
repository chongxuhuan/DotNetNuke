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

Imports System.IO
Imports System.Text
Imports System.Xml.XPath
Imports DotNetNuke.Application

Imports DotNetNuke.Common.Lists
Imports System.Text.RegularExpressions

Namespace DotNetNuke.Services.Installer.Dependencies

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CoreVersionDependency determines whether the CoreVersion is correct
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	09/02/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CoreVersionDependency
        Inherits DependencyBase

        Private minVersion As System.Version

        Public Overrides ReadOnly Property ErrorMessage() As String
            Get
                Return Util.INSTALL_Compatibility
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Dim _IsValid As Boolean = True

                If DotNetNukeContext.Current.Application.Version < minVersion Then
                    _IsValid = False
                End If

                Return _IsValid
            End Get
        End Property

        Public Overrides Sub ReadManifest(ByVal dependencyNav As XPathNavigator)
            minVersion = New System.Version(dependencyNav.Value)
        End Sub

    End Class

End Namespace
