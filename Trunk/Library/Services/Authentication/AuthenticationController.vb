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
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.Windows.Forms

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Services.Authentication

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AuthenticationController class provides the Business Layer for the 
    ''' Authentication Systems.
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	07/10/2007  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AuthenticationController

#Region "Private Members"

        Private Shared provider As DataProvider = DataProvider.Instance()

#End Region

#Region "Private Shared Methods"

        Private Shared Function GetAuthenticationServicesCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillCollection(Of AuthenticationInfo)(provider.GetAuthenticationServices)
        End Function

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddAuthentication adds a new Authentication System to the Data Store.
        ''' </summary>
        ''' <param name="authSystem">The new Authentication System to add</param>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddAuthentication(ByVal authSystem As AuthenticationInfo) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_CREATED)
            Return provider.AddAuthentication(authSystem.PackageID, authSystem.AuthenticationType, authSystem.IsEnabled, authSystem.SettingsControlSrc, authSystem.LoginControlSrc, authSystem.LogoffControlSrc, UserController.GetCurrentUserInfo.UserID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddUserAuthentication adds a new UserAuthentication to the User.
        ''' </summary>
        ''' <param name="userID">The new Authentication System to add</param>
        ''' <param name="authenticationType">The authentication type</param>
        ''' <param name="authenticationToken">The authentication token</param>
        ''' <history>
        ''' 	[cnurse]	07/12/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddUserAuthentication(ByVal userID As Integer, ByVal authenticationType As String, ByVal authenticationToken As String) As Integer
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("userID/authenticationType", userID.ToString & "/" & authenticationType.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_USER_CREATED)
            Return provider.AddUserAuthentication(userID, authenticationType, authenticationToken, UserController.GetCurrentUserInfo.UserID)
        End Function

        Public Shared Sub DeleteAuthentication(ByVal authSystem As AuthenticationInfo)
            provider.DeleteAuthentication(authSystem.AuthenticationID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_DELETED)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAuthenticationService fetches a single Authentication Systems 
        ''' </summary>
        ''' <param name="authenticationID">The ID of the Authentication System</param>
        ''' <returns>An AuthenticationInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAuthenticationService(ByVal authenticationID As Integer) As AuthenticationInfo
            Dim authInfo As AuthenticationInfo = Nothing
            For Each authService As AuthenticationInfo In GetAuthenticationServices()
                If authService.AuthenticationID = authenticationID Then
                    authInfo = authService
                    Exit For
                End If
            Next
            If authInfo Is Nothing Then
                'Go to database
                Return CBO.FillObject(Of AuthenticationInfo)(provider.GetAuthenticationService(authenticationID))
            End If
            Return authInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAuthenticationServiceByPackageID fetches a single Authentication System 
        ''' </summary>
        ''' <param name="packageID">The id of the Package</param>
        ''' <returns>An AuthenticationInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAuthenticationServiceByPackageID(ByVal packageID As Integer) As AuthenticationInfo
            Dim authInfo As AuthenticationInfo = Nothing
            For Each authService As AuthenticationInfo In GetAuthenticationServices()
                If authService.PackageID = packageID Then
                    authInfo = authService
                    Exit For
                End If
            Next
            If authInfo Is Nothing Then
                'Go to database
                Return CBO.FillObject(Of AuthenticationInfo)(provider.GetAuthenticationServiceByPackageID(packageID))
            End If
            Return authInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAuthenticationServiceByType fetches a single Authentication Systems 
        ''' </summary>
        ''' <param name="authenticationType">The type of the Authentication System</param>
        ''' <returns>An AuthenticationInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	07/31/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAuthenticationServiceByType(ByVal authenticationType As String) As AuthenticationInfo
            Dim authInfo As AuthenticationInfo = Nothing
            For Each authService As AuthenticationInfo In GetAuthenticationServices()
                If authService.AuthenticationType = authenticationType Then
                    authInfo = authService
                    Exit For
                End If
            Next

            If authInfo Is Nothing Then
                'Go to database
                Return CBO.FillObject(Of AuthenticationInfo)(provider.GetAuthenticationServiceByType(authenticationType))
            End If
            Return authInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAuthenticationServices fetches a list of all the Authentication Systems 
        ''' installed in the system
        ''' </summary>
        ''' <returns>A List of AuthenticationInfo objects</returns>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAuthenticationServices() As List(Of AuthenticationInfo)
            Return CBO.GetCachedObject(Of List(Of AuthenticationInfo))(New CacheItemArgs(DataCache.AuthenticationServicesCacheKey, _
                                                        DataCache.AuthenticationServicesCacheTimeOut, DataCache.AuthenticationServicesCachePriority), _
                                                        AddressOf GetAuthenticationServicesCallBack)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAuthenticationType fetches the authentication method used by the currently logged on user
        ''' </summary>
        ''' <returns>An AuthenticationInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	07/23/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetAuthenticationType() As AuthenticationInfo
            Dim objAuthentication As AuthenticationInfo = Nothing
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing Then
                Try
                    objAuthentication = GetAuthenticationServiceByType(HttpContext.Current.Request("authentication"))
                Catch
                End Try
            End If

            Return objAuthentication
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetEnabledAuthenticationServices fetches a list of all the Authentication Systems 
        ''' installed in the system that have been enabled by the Host user
        ''' </summary>
        ''' <returns>A List of AuthenticationInfo objects</returns>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetEnabledAuthenticationServices() As List(Of AuthenticationInfo)
            Dim enabled As New List(Of AuthenticationInfo)
            For Each authService As AuthenticationInfo In GetAuthenticationServices()
                If authService.IsEnabled Then
                    enabled.Add(authService)
                End If
            Next
            Return enabled
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetLogoffRedirectURL fetches the url to redirect too after logoff
        ''' </summary>
        ''' <param name="settings">A PortalSettings object</param>
        ''' <param name="request">The current Request</param>
        ''' <returns>The Url</returns>
        ''' <history>
        ''' 	[cnurse]	08/15/2007  Created
        '''     [cnurse]    02/28/2008  DNN-6881 Logoff redirect
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetLogoffRedirectURL(ByVal settings As PortalSettings, ByVal request As HttpRequest) As String
            Dim _RedirectURL As String = ""

            Dim setting As Object = UserModuleBase.GetSetting(settings.PortalId, "Redirect_AfterLogout")

            If CType(setting, Integer) = Null.NullInteger Then
                If TabPermissionController.CanViewPage() Then
                    ' redirect to current page
                    _RedirectURL = NavigateURL(settings.ActiveTab.TabID)
                ElseIf settings.HomeTabId <> -1 Then
                    ' redirect to portal home page specified
                    _RedirectURL = NavigateURL(settings.HomeTabId)
                Else ' redirect to default portal root
                    _RedirectURL = GetPortalDomainName(settings.PortalAlias.HTTPAlias, request) & "/" & glbDefaultPage
                End If
            Else ' redirect to after logout page
                _RedirectURL = NavigateURL(CType(setting, Integer))
            End If

            Return _RedirectURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetAuthenticationType sets the authentication method used by the currently logged on user
        ''' </summary>
        ''' <param name="value">The Authentication type</param>
        ''' <history>
        ''' 	[cnurse]	07/23/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetAuthenticationType(ByVal value As String)
            SetAuthenticationType(value, False)
        End Sub

        Public Shared Sub SetAuthenticationType(ByVal value As String, ByVal CreatePersistentCookie As Boolean)
            Try
                Dim PersistentCookieTimeout As Integer = Config.GetPersistentCookieTimeout()
                Dim Response As HttpResponse = HttpContext.Current.Response
                If Response Is Nothing Then
                    Return
                End If

                ' save the authenticationmethod as a cookie
                Dim cookie As System.Web.HttpCookie = Nothing
                cookie = Response.Cookies.Get("authentication")
                If (cookie Is Nothing) Then
                    If value <> "" Then
                        cookie = New System.Web.HttpCookie("authentication", value)
                        If CreatePersistentCookie Then
                            cookie.Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout)
                        End If
                        Response.Cookies.Add(cookie)
                    End If
                Else
                    cookie.Value = value
                    If value <> "" Then
                        If CreatePersistentCookie Then
                            cookie.Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout)
                        End If
                        Response.Cookies.Set(cookie)
                    Else
                        Response.Cookies.Remove("authentication")
                    End If
                End If
            Catch
                Return
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateAuthentication updates an existing Authentication System in the Data Store.
        ''' </summary>
        ''' <param name="authSystem">The new Authentication System to update</param>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateAuthentication(ByVal authSystem As AuthenticationInfo)
            provider.UpdateAuthentication(authSystem.AuthenticationID, authSystem.PackageID, authSystem.AuthenticationType, authSystem.IsEnabled, authSystem.SettingsControlSrc, authSystem.LoginControlSrc, authSystem.LogoffControlSrc, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(authSystem, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.AUTHENTICATION_UPDATED)
        End Sub

#End Region

    End Class

End Namespace
