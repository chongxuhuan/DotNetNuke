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
Imports System.Web

Imports DotNetNuke
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.HttpModules.UsersOnline

    Public Class UsersOnlineModule

        Implements IHttpModule

        Public ReadOnly Property ModuleName() As String
            Get
                Return "UsersOnlineModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init
            AddHandler application.AuthorizeRequest, AddressOf Me.OnAuthorizeRequest
        End Sub

        Public Sub OnAuthorizeRequest(ByVal s As Object, ByVal e As EventArgs)

            'First check if we are upgrading/installing
            Dim app As HttpApplication = CType(s, HttpApplication)
            Dim Request As HttpRequest = app.Request

            'check if we are upgrading/installing or if this is a captcha request
            If Request.Url.LocalPath.ToLower.EndsWith("install.aspx") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("captcha.aspx") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("scriptresource.axd") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("webresource.axd") _
                    Then
                Exit Sub
            End If

            ' Create a Users Online Controller
            Dim objUserOnlineController As New UserOnlineController

            ' Is Users Online Enabled?
            If (objUserOnlineController.IsEnabled()) Then
                ' Track the current user
                objUserOnlineController.TrackUsers()
            End If

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
