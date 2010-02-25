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
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.Web

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      UserOnlineController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserOnlineController class provides Business Layer methods for Users Online
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/14/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserOnlineController

        Private Shared memberProvider As Membership.MembershipProvider = Membership.MembershipProvider.Instance()

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clears the cached Users Online Information
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ClearUserList()

            Dim key As String = "OnlineUserList"

            DataCache.RemoveCache(key)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Online time window
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetOnlineTimeWindow() As Integer
            Return Host.Host.UsersOnlineTimeWindow
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the cached Users Online Information
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetUserList() As Hashtable

            Dim key As String = "OnlineUserList"
            Dim userList As Hashtable = CType(DataCache.GetCache(key), Hashtable)

            'Do we have the Hashtable?
            If (userList Is Nothing) Then
                userList = New Hashtable
                DataCache.SetCache(key, userList)
            End If

            Return userList

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Users Online functionality is enabled
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsEnabled() As Boolean
            Return Host.Host.EnableUsersOnline
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether a User is online
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsUserOnline(ByVal user As UserInfo) As Boolean

            Dim isOnline As Boolean = False
            If IsEnabled() Then
                isOnline = memberProvider.IsUserOnline(user)
            End If
            Return isOnline

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the cached Users Online Information
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetUserList(ByVal userList As Hashtable)

            Dim key As String = "OnlineUserList"

            DataCache.SetCache(key, userList)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Tracks an Anonymous User
        ''' </summary>
        ''' <param name="context">An HttpContext Object</param>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub TrackAnonymousUser(ByVal context As HttpContext)

            Dim cookieName As String = "DotNetNukeAnonymous"

            Dim portalSettings As portalSettings = CType(context.Items("PortalSettings"), portalSettings)

            If portalSettings Is Nothing Then
                Return
            End If

            Dim user As AnonymousUserInfo
            Dim userList As Hashtable = GetUserList()
            Dim userID As String

            ' Check if the Tracking cookie exists
            Dim cookie As HttpCookie = context.Request.Cookies(cookieName)

            ' Track Anonymous User
            If (cookie Is Nothing) Then
                ' Create a temporary userId
                userID = Guid.NewGuid().ToString()

                ' Create a new cookie
                cookie = New HttpCookie(cookieName)
                cookie.Value = userID
                cookie.Expires = DateTime.Now.AddMinutes(20)
                context.Response.Cookies.Add(cookie)

                ' Create a user
                user = New AnonymousUserInfo
                user.UserID = userID
                user.PortalID = portalSettings.PortalId
                user.TabID = portalSettings.ActiveTab.TabID
                user.CreationDate = DateTime.Now
                user.LastActiveDate = DateTime.Now

                ' Add the user
                If Not (userList.Contains(userID)) Then
                    userList(userID) = user
                End If
            Else
                If (cookie.Value Is Nothing) Then
                    ' Expire the cookie, there is something wrong with it
                    context.Response.Cookies(cookieName).Expires = New System.DateTime(1999, 10, 12)

                    ' No need to do anything else
                    Return
                End If

                ' Get userID out of cookie
                userID = cookie.Value

                ' Find the cookie in the user list
                If (userList(userID) Is Nothing) Then
                    userList(userID) = New AnonymousUserInfo
                    CType(userList(userID), AnonymousUserInfo).CreationDate = DateTime.Now
                End If

                user = CType(userList(userID), AnonymousUserInfo)
                user.UserID = userID
                user.PortalID = portalSettings.PortalId
                user.TabID = portalSettings.ActiveTab.TabID
                user.LastActiveDate = DateTime.Now

                ' Reset the expiration on the cookie
                cookie = New HttpCookie(cookieName)
                cookie.Value = userID
                cookie.Expires = DateTime.Now.AddMinutes(20)
                context.Response.Cookies.Add(cookie)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Tracks an Authenticated User
        ''' </summary>
        ''' <param name="context">An HttpContext Object</param>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub TrackAuthenticatedUser(ByVal context As HttpContext)

            ' Retrieve Portal Settings
            Dim portalSettings As portalSettings = CType(context.Items("PortalSettings"), portalSettings)

            If portalSettings Is Nothing Then
                Return
            End If

            ' Get the logged in User ID
            Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo

            ' Get user list
            Dim userList As Hashtable = GetUserList()

            Dim user As OnlineUserInfo = New OnlineUserInfo
            If objUserInfo.UserID > 0 Then
                ' forms authentication
                user.UserID = objUserInfo.UserID
            End If
            user.PortalID = portalSettings.PortalId
            user.TabID = portalSettings.ActiveTab.TabID
            user.LastActiveDate = DateTime.Now
            If (userList(objUserInfo.UserID.ToString()) Is Nothing) Then
                user.CreationDate = user.LastActiveDate
            End If

            userList(objUserInfo.UserID.ToString()) = user

            SetUserList(userList)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Tracks an online User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub TrackUsers()

            Dim context As HttpContext = HttpContext.Current

            ' Have we already done the work for this request?
            If Not (context.Items("CheckedUsersOnlineCookie") Is Nothing) Then
                Return
            Else
                context.Items("CheckedUsersOnlineCookie") = "true"
            End If

            If (context.Request.IsAuthenticated) Then
                TrackAuthenticatedUser(context)
            Else
                If (context.Request.Browser.Cookies) Then
                    TrackAnonymousUser(context)
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Update the Users Online information
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateUsersOnline()

            ' Get a Current User List
            Dim userList As Hashtable = GetUserList()

            ' Create a shallow copy of the list to Process
            Dim listToProcess As Hashtable = CType(userList.Clone(), Hashtable)

            ' Clear the list
            ClearUserList()

            ' Persist the current User List
            Try
                memberProvider.UpdateUsersOnline(listToProcess)
            Catch ex As Exception
            End Try

            ' Remove users that have expired
            memberProvider.DeleteUsersOnline(GetOnlineTimeWindow())
        End Sub

#End Region

    End Class

End Namespace
