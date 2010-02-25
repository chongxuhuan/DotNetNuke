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
Imports System.Web

Imports DotNetNuke.Entities.Portals

Namespace DotNetNuke.Entities.Users
    Public Class UserTime

        Public Sub New()
        End Sub

        Public Function ConvertToUserTime(ByVal dt As DateTime, ByVal ClientTimeZone As Double) As DateTime
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return dt.AddMinutes(FromClientToServerFactor(ClientTimeZone, _portalSettings.TimeZoneOffset))
        End Function

        Public Function ConvertToServerTime(ByVal dt As DateTime, ByVal ClientTimeZone As Double) As DateTime
            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Return dt.AddMinutes(FromServerToClientFactor(ClientTimeZone, _portalSettings.TimeZoneOffset))
        End Function

        Public Shared Function CurrentTimeForUser(ByVal User As UserInfo) As DateTime
            If User Is Nothing OrElse User.UserID = -1 OrElse User.Profile.TimeZone = Null.NullInteger Then
                Dim intOffset As Integer = 0
                Dim objSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
                If Not objSettings Is Nothing Then
                    intOffset = objSettings.TimeZoneOffset
                Else
                    Dim objPCtr As PortalInfo = New PortalController().GetPortal(User.PortalID)
                    intOffset = objPCtr.TimeZoneOffset
                End If
                Return DateTime.UtcNow.AddMinutes(intOffset)
            Else
                Return DateTime.UtcNow.AddMinutes(User.Profile.TimeZone)
            End If
        End Function

        Public ReadOnly Property CurrentUserTime() As DateTime
            Get
                Dim context As HttpContext = HttpContext.Current
                Dim objSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
                If Not (context.Request.IsAuthenticated) Then
                    Return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset)
                Else
                    Return DateTime.UtcNow.AddMinutes(objSettings.TimeZoneOffset).AddMinutes(ClientToServerTimeZoneFactor)
                End If
            End Get
        End Property

        Public ReadOnly Property ClientToServerTimeZoneFactor() As Double
            Get
                ' Obtain PortalSettings from Current Context
                Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                Return FromClientToServerFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset)
            End Get
        End Property

        Public ReadOnly Property ServerToClientTimeZoneFactor() As Double
            Get
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                ' Obtain PortalSettings from Current Context
                Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Return FromServerToClientFactor(objUserInfo.Profile.TimeZone, _portalSettings.TimeZoneOffset)
            End Get
        End Property

        Private Function FromClientToServerFactor(ByVal Client As Double, ByVal Server As Double) As Double
            Return Client - Server
        End Function

        Private Function FromServerToClientFactor(ByVal Client As Double, ByVal Server As Double) As Double
            Return Server - Client
        End Function

    End Class
End Namespace