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

Imports System.Web.Compilation
Imports System.Net
Imports System.Security.Permissions

Namespace DotNetNuke.Framework

    Public Class SecurityPolicy

        Private Shared m_Initialized As Boolean = False
        Private Shared m_ReflectionPermission As Boolean
        Private Shared m_WebPermission As Boolean
        Private Shared m_AspNetHostingPermission As Boolean

        ' all supported permissions need an associated public string constant
        Public Const ReflectionPermission As String = "ReflectionPermission"
        Public Const WebPermission As String = "WebPermission"
        Public Const AspNetHostingPermission As String = "AspNetHostingPermission"

        Private Shared Sub GetPermissions()
            If Not m_Initialized Then
                ' test RelectionPermission
                Dim securityTest As System.Security.CodeAccessPermission
                Try
                    securityTest = New ReflectionPermission(PermissionState.Unrestricted)
                    securityTest.Demand()
                    m_ReflectionPermission = True
                Catch
                    ' code access security error
                    m_ReflectionPermission = False
                End Try

                ' test WebPermission
                Try
                    securityTest = New System.Net.WebPermission(PermissionState.Unrestricted)
                    securityTest.Demand()
                    m_WebPermission = True
                Catch
                    ' code access security error
                    m_WebPermission = False
                End Try

                ' test WebHosting Permission (Full Trust)
                Try
                    securityTest = New AspNetHostingPermission(AspNetHostingPermissionLevel.Unrestricted)
                    securityTest.Demand()
                    m_AspNetHostingPermission = True
                Catch
                    ' code access security error
                    m_AspNetHostingPermission = False
                End Try

                m_Initialized = True
            End If
        End Sub

        Public Shared Function HasAspNetHostingPermission() As Boolean
            GetPermissions()
            Return m_AspNetHostingPermission
        End Function

        Public Shared Function HasReflectionPermission() As Boolean
            GetPermissions()
            Return m_ReflectionPermission
        End Function

        Public Shared Function HasWebPermission() As Boolean
            GetPermissions()
            Return m_WebPermission
        End Function

        Public Shared Function HasPermissions(ByVal permissions As String, ByRef permission As String) As Boolean
            Dim _HasPermission As Boolean = True

            If permissions <> "" Then
                For Each permission In (permissions & ";").Split(Convert.ToChar(";"))
                    If permission.Trim <> "" Then
                        Select Case permission
                            Case AspNetHostingPermission
                                If HasAspNetHostingPermission() = False Then
                                    _HasPermission = False
                                    Exit Function
                                End If
                            Case ReflectionPermission
                                If HasReflectionPermission() = False Then
                                    _HasPermission = False
                                    Exit Function
                                End If
                            Case WebPermission
                                If HasWebPermission() = False Then
                                    _HasPermission = False
                                    Exit Function
                                End If
                        End Select
                    End If
                Next
            End If

            Return _HasPermission
        End Function

        Public Shared ReadOnly Property Permissions() As String
            Get
                Dim strPermissions As String = ""
                If Framework.SecurityPolicy.HasReflectionPermission Then
                    strPermissions += ", " & Framework.SecurityPolicy.ReflectionPermission
                End If
                If Framework.SecurityPolicy.HasWebPermission Then
                    strPermissions += ", " & Framework.SecurityPolicy.WebPermission
                End If
                If Framework.SecurityPolicy.HasAspNetHostingPermission Then
                    strPermissions += ", " & Framework.SecurityPolicy.AspNetHostingPermission
                End If
                If strPermissions <> "" Then
                    strPermissions = strPermissions.Substring(2)
                End If
                Return strPermissions
            End Get
        End Property

        <Obsolete("Replaced by correctly spelt method")> _
        Public Shared Function HasRelectionPermission() As Boolean
            GetPermissions()
            Return m_ReflectionPermission
        End Function
    End Class
End Namespace