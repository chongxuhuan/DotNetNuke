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
Imports System.Web

Imports DotNetNuke
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Personalization


Namespace DotNetNuke.HttpModules.Personalization

    Public Class PersonalizationModule

        Implements IHttpModule

        Public ReadOnly Property ModuleName() As String
            Get
                Return "PersonalizationModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init

            AddHandler application.EndRequest, AddressOf Me.OnEndRequest

        End Sub

        Public Sub OnEndRequest(ByVal s As Object, ByVal e As EventArgs)

            Dim Context As HttpContext = CType(s, HttpApplication).Context
            Dim Request As HttpRequest = Context.Request

            'exit if a request for a .net mapping that isn't a content page is made i.e. axd
            If Request.Url.LocalPath.ToLower.EndsWith(".aspx") = False _
                    AndAlso Request.Url.LocalPath.ToLower.EndsWith(".asmx") = False _
                    AndAlso Request.Url.LocalPath.ToLower.EndsWith(".ashx") = False Then
                Exit Sub
            End If

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = CType(Context.Items("PortalSettings"), PortalSettings)

            If Not (_portalSettings Is Nothing) Then

                ' load the user info object
                Dim UserInfo As Entities.Users.UserInfo = Entities.Users.UserController.GetCurrentUserInfo

                Dim objPersonalization As New PersonalizationController
                objPersonalization.SaveProfile(Context, UserInfo.UserID, _portalSettings.PortalId)
            End If

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
