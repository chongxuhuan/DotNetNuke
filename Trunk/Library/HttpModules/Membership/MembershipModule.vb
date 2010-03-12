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
Imports System.Security
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Security
Imports System.IO

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Personalization
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.HttpModules.Membership

    Public Class MembershipModule

        Implements IHttpModule

        Public ReadOnly Property ModuleName() As String
            Get
                Return "DNNMembershipModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init

            AddHandler application.AuthenticateRequest, AddressOf Me.OnAuthenticateRequest

        End Sub

        Public Sub OnAuthenticateRequest(ByVal s As Object, ByVal e As EventArgs)

            Dim Context As HttpContext = CType(s, HttpApplication).Context
            Dim Request As HttpRequest = Context.Request
            Dim Response As HttpResponse = Context.Response

            'First check if we are upgrading/installing
            If Request.Url.LocalPath.ToLower.EndsWith("install.aspx") OrElse Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") Then
                Exit Sub
            End If

            'exit if a request for a .net mapping that isn't a content page is made i.e. axd
            If Request.Url.LocalPath.ToLower.EndsWith(".aspx") = False _
                    AndAlso Request.Url.LocalPath.ToLower.EndsWith(".asmx") = False _
                    AndAlso Request.Url.LocalPath.ToLower.EndsWith(".ashx") = False Then
                Exit Sub
            End If

            ' Obtain PortalSettings from Current Context
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            If Request.IsAuthenticated = True And Not _portalSettings Is Nothing Then
                Dim arrPortalRoles() As String
                Dim objRoleController As New RoleController

                Dim objUser As UserInfo = UserController.GetCachedUser(_portalSettings.PortalId, Context.User.Identity.Name)

                If Not Request.Cookies("portalaliasid") Is Nothing Then
                    Dim PortalCookie As FormsAuthenticationTicket = FormsAuthentication.Decrypt(Context.Request.Cookies("portalaliasid").Value)
                    ' check if user has switched portals
                    If _portalSettings.PortalAlias.PortalAliasID <> Int32.Parse(PortalCookie.UserData) Then
                        ' expire cookies if portal has changed
                        Response.Cookies("portalaliasid").Value = Nothing
                        Response.Cookies("portalaliasid").Path = "/"
                        Response.Cookies("portalaliasid").Expires = DateTime.Now.AddYears(-30)

                        Response.Cookies("portalroles").Value = Nothing
                        Response.Cookies("portalroles").Path = "/"
                        Response.Cookies("portalroles").Expires = DateTime.Now.AddYears(-30)
                    End If
                End If

                ' authenticate user and set last login ( this is necessary for users who have a permanent Auth cookie set ) 
                If objUser Is Nothing OrElse objUser.IsDeleted OrElse objUser.Membership.LockedOut = True OrElse _
                                             objUser.Membership.Approved = False OrElse _
                                             objUser.Username.ToLower <> Context.User.Identity.Name.ToLower Then
                    Dim objPortalSecurity As New PortalSecurity
                    objPortalSecurity.SignOut()

                    'Remove user from cache
                    If objUser IsNot Nothing Then
                        DataCache.ClearUserCache(_portalSettings.PortalId, Context.User.Identity.Name)
                    End If
                    ' Redirect browser back to home page
                    Response.Redirect(Request.RawUrl, True)
                    Exit Sub

                Else ' valid Auth cookie

                    ' if users LastActivityDate is outside of the UsersOnlineTimeWindow then record user activity
                    If Date.Compare(objUser.Membership.LastActivityDate.AddMinutes(Entities.Host.Host.UsersOnlineTimeWindow), Now) < 0 Then
                        ' update LastActivityDate and IP Address for user
                        objUser.Membership.LastActivityDate = Now
                        objUser.LastIPAddress = Request.UserHostAddress
                        UserController.UpdateUser(_portalSettings.PortalId, objUser, False)
                    End If

                    ' refreshroles is set when a role is added to a user by an administrator
                    Dim refreshCookies As Boolean = objUser.RefreshRoles

                    ' check for RSVP code
                    If Not objUser.RefreshRoles AndAlso Not Request.QueryString("rsvp") Is Nothing AndAlso String.IsNullOrEmpty(Request.QueryString("rsvp")) = False Then
                        For Each objRole As RoleInfo In objRoleController.GetPortalRoles(_portalSettings.PortalId)
                            If objRole.RSVPCode = Request.QueryString("rsvp") Then
                                objRoleController.UpdateUserRole(_portalSettings.PortalId, objUser.UserID, objRole.RoleID)
                                'clear portalroles so the new role is added to the cookie below
                                refreshCookies = True
                            End If
                        Next
                    End If

                    ' create cookies if they do not exist yet for this session.
                    If Request.Cookies("portalroles") Is Nothing OrElse refreshCookies Then
                        ' keep cookies in sync
                        Dim CurrentDateTime As Date = DateTime.Now

                        ' create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                        Dim PortalTicket As New FormsAuthenticationTicket(1, Context.User.Identity.Name, CurrentDateTime, CurrentDateTime.AddHours(1), False, _portalSettings.PortalAlias.PortalAliasID.ToString)
                        ' encrypt the ticket
                        Dim strPortalAliasID As String = FormsAuthentication.Encrypt(PortalTicket)
                        ' send portal cookie to client
                        Response.Cookies("portalaliasid").Value = strPortalAliasID
                        Response.Cookies("portalaliasid").Path = "/"
                        Response.Cookies("portalaliasid").Expires = CurrentDateTime.AddMinutes(1)

                        ' get roles from UserRoles table
                        arrPortalRoles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId)

                        ' create a string to persist the roles, attach a portalID so that cross-portal impersonation cannot occur
                        Dim strPortalRoles As String = _portalSettings.PortalId & "!!" & Join(arrPortalRoles, New Char() {";"c})

                        ' create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                        Dim RolesTicket As New FormsAuthenticationTicket(1, Context.User.Identity.Name, CurrentDateTime, CurrentDateTime.AddHours(1), False, strPortalRoles)
                        ' encrypt the ticket
                        Dim strRoles As String = FormsAuthentication.Encrypt(RolesTicket)
                        ' send roles cookie to client
                        Response.Cookies("portalroles").Value = strRoles
                        Response.Cookies("portalroles").Path = "/"
                        Response.Cookies("portalroles").Expires = CurrentDateTime.AddMinutes(1)

                        If refreshCookies Then
                            ' if rsvp, update portalroles in context because it is being used later
                            Context.Request.Cookies("portalroles").Value = strRoles
                        End If
                    End If

                    If Not Request.Cookies("portalroles") Is Nothing Then
                        ' get roles from roles cookie
                        If Request.Cookies("portalroles").Value <> "" Then
                            Dim RoleTicket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(Context.Request.Cookies("portalroles").Value)

                            If Not RoleTicket Is Nothing Then
                                ' get the role data and split it into portalid and a string array of role data
                                Dim rolesdata As String = RoleTicket.UserData
                                Dim mySplit() As Char = "!!".ToCharArray
                                'need to use StringSplitOptions.None to preserve case where superuser has no roles
                                Dim RolesParts As String() = rolesdata.Split(mySplit, StringSplitOptions.None)

                                'if cookie is for a different portal than current force a refresh of roles else used cookie cached version
                                If Convert.ToInt32(RolesParts(0)) <> _portalSettings.PortalId Then
                                    objUser.Roles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId)
                                Else
                                    objUser.Roles = RolesParts(2).Split(";"c)
                                End If
                            Else
                                objUser.Roles = objRoleController.GetRolesByUser(objUser.UserID, _portalSettings.PortalId)
                            End If

                            'Clear RefreshRoles flag
                            If objUser.RefreshRoles Then
                                objUser.RefreshRoles = False
                                UserController.UpdateUser(_portalSettings.PortalId, objUser, False)
                            End If
                        End If

                        ' save userinfo object in context
                        Context.Items.Add("UserInfo", objUser)

                        ' load the personalization object
                        Dim objPersonalizationController As New PersonalizationController
                        objPersonalizationController.LoadProfile(Context, objUser.UserID, objUser.PortalID)

                        'Localization.SetLanguage also updates the user profile, so this needs to go after the profile is loaded
                        Localization.SetLanguage(objUser.Profile.PreferredLocale)
                    End If

                End If
            End If

            If CType(HttpContext.Current.Items("UserInfo"), UserInfo) Is Nothing Then
                Context.Items.Add("UserInfo", New UserInfo)
            End If

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
