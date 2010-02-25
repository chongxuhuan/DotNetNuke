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
Imports System.Text.RegularExpressions

Namespace DotNetNuke.Services.Installer.Dependencies

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PermissionsDependency determines whether the DotNetNuke site has the
    ''' corretc permissions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	09/02/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PermissionsDependency
        Inherits DependencyBase

        Private Permissions As String
        Private Permission As String = Null.NullString

        Public Overrides ReadOnly Property ErrorMessage() As String
            Get
                Return Util.INSTALL_Permissions + " - " + Permission
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return Framework.SecurityPolicy.HasPermissions(Permissions, Permission)
            End Get
        End Property

        Public Overrides Sub ReadManifest(ByVal dependencyNav As XPathNavigator)
            Permissions = dependencyNav.Value
        End Sub

    End Class

End Namespace
