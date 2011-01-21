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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Authentication

Namespace DotNetNuke.Modules.Admin.Authentication

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : Logoff
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Logoff UserModuleBase is used to log off a registered user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]        07/23/2007   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Logoff
        Inherits UserModuleBase

        Private Sub Redirect()
            ' Redirect browser back to portal 
            Response.Redirect(AuthenticationController.GetLogoffRedirectURL(PortalSettings, Request), True)
        End Sub

        Private Sub Logoff()
            Try
                'Remove user from cache
                If Me.User IsNot Nothing Then
                    DataCache.ClearUserCache(Me.PortalSettings.PortalId, Context.User.Identity.Name)
                End If

                Dim objPortalSecurity As New PortalSecurity
                objPortalSecurity.SignOut()

            Catch exc As Exception    'Page failed to load
                ProcessPageLoadException(exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/23/2006  Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'Get the Authentication System associated with the current User
                Dim authSystem As AuthenticationInfo = AuthenticationController.GetAuthenticationType()

                If authSystem IsNot Nothing AndAlso Not String.IsNullOrEmpty(authSystem.LogoffControlSrc) Then
                    Dim authLogoffControl As AuthenticationLogoffBase = CType(LoadControl("~/" & authSystem.LogoffControlSrc), AuthenticationLogoffBase)

                    ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                    ' this is necessary for the Localization in PageBase
                    authLogoffControl.AuthenticationType = authSystem.AuthenticationType
                    authLogoffControl.ID = Path.GetFileNameWithoutExtension(authSystem.LogoffControlSrc) + "_" + authSystem.AuthenticationType
                    authLogoffControl.LocalResourceFile = authLogoffControl.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/" & Path.GetFileNameWithoutExtension(authSystem.LogoffControlSrc)
                    authLogoffControl.ModuleConfiguration = Me.ModuleConfiguration

                    AddHandler authLogoffControl.LogOff, AddressOf Me.UserLogOff
                    AddHandler authLogoffControl.Redirect, AddressOf Me.UserRedirect

                    'Add Login Control to Control
                    pnlLogoffContainer.Controls.Add(authLogoffControl)
                Else
                    'The current auth system has no custom logoff control so LogOff
                    Logoff()

                    Redirect()
                End If

            Catch exc As Exception    'Page failed to load
                ProcessPageLoadException(exc)
            End Try
        End Sub

        Protected Sub UserLogOff(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Logoff()
        End Sub

        Protected Sub UserRedirect(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Redirect()
        End Sub

    End Class

End Namespace

