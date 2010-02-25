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

Namespace DotNetNuke.Services.Installer.Dependencies

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DependencyFactory is a factory class that is used to instantiate the
    ''' appropriate Dependency
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	09/02/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DependencyFactory

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetDependency method instantiates (and returns) the relevant Dependency
        ''' </summary>
        ''' <param name="dependencyNav">The manifest (XPathNavigator) for the dependency</param>
        ''' <history>
        ''' 	[cnurse]	09/02/2007  created
        '''     [bdukes]    03/04/2009  added case-insensitivity to type attribute, added InvalidDependency for dependencies that can't be created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetDependency(ByVal dependencyNav As XPathNavigator) As IDependency
            Dim dependency As IDependency = Nothing
            Dim dependencyType As String = Util.ReadAttribute(dependencyNav, "type")

            Select Case dependencyType.ToLowerInvariant()
                Case "coreversion"
                    dependency = New CoreVersionDependency()
                Case "package"
                    dependency = New PackageDependency()
                Case "permission"
                    dependency = New PermissionsDependency()
                Case "type"
                    dependency = New TypeDependency()
                Case Else
                    'Dependency type is defined in the List
                    Dim listController As New Lists.ListController()
                    Dim entry As ListEntryInfo = listController.GetListEntryInfo("Dependency", dependencyType)

                    If entry IsNot Nothing AndAlso Not String.IsNullOrEmpty(entry.Text) Then
                        'The class for the Installer is specified in the Text property
                        dependency = CType(Reflection.CreateObject(entry.Text, "Dependency_" + entry.Value), DependencyBase)
                    End If
            End Select

            If dependency Is Nothing Then
                'Could not create dependency, show generic error message
                dependency = New InvalidDependency(Util.INSTALL_Dependencies)
            End If

            'Read Manifest
            dependency.ReadManifest(dependencyNav)

            Return dependency

        End Function

#End Region

    End Class
End Namespace
