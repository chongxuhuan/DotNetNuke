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
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Tokens

Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewProfile ProfileModuleUserControlBase is used to view a Users Profile
    ''' </summary>
    ''' <history>
    ''' 	[jlucarino]	02/25/2010   created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewProfile
        Inherits ProfileModuleUserControlBase

        Public Overrides ReadOnly Property DisplayModule() As Boolean
            Get
                Return True
            End Get
        End Property

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'empty
        End Sub

        Private ReadOnly Property IsAdmin() As Boolean
            Get
                Return PortalSecurity.IsInRole(Me.ModuleContext.PortalSettings.AdministratorRoleName)
            End Get
        End Property

        Private ReadOnly Property IsUser() As Boolean
            Get
                Return ProfileUserId = Me.ModuleContext.PortalSettings.UserId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[jlucarino]	02/25/2010    created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
            Try
                Dim oUser As UserInfo = UserController.GetUserById(Me.ModuleContext.PortalId, ProfileUserId)

                If Not IsUser Then
                    cmdEdit.Visible = False
                End If

                Dim properties As ProfilePropertyDefinitionCollection = oUser.Profile.ProfileProperties
                Dim visibleCount As Integer = 0

                'loop through properties to see if any are set to visible
                For Each profProperty As ProfilePropertyDefinition In properties
                    If profProperty.Visible Then
                        'Check Visibility
                        If profProperty.Visibility = UserVisibilityMode.AdminOnly Then
                            'Only Visible if Admin (or self)
                            profProperty.Visible = (IsAdmin Or IsUser)
                        ElseIf profProperty.Visibility = UserVisibilityMode.MembersOnly Then
                            'Only Visible if Is a Member (ie Authenticated)
                            profProperty.Visible = Request.IsAuthenticated
                        End If
                    End If
                    If profProperty.Visible Then visibleCount += 1
                Next

                If visibleCount = 0 Then
                    lblNoProperties.Visible = True

                Else
                    Dim Template As String = ""
                    Dim oToken As New TokenReplace

                    oToken.User = oUser                                               'user in profile
                    oToken.AccessingUser = Me.ModuleContext.PortalSettings.UserInfo   'user browsing the site

                    If Not Me.ModuleContext.Settings("ProfileTemplate") Is Nothing Then
                        Template = Convert.ToString(Me.ModuleContext.Settings("ProfileTemplate"))
                    Else
                        Template = Localization.GetString("DefaultTemplate", Me.LocalResourceFile)
                    End If

                    ProfileOutput.Text = oToken.ReplaceEnvironmentTokens(Template)

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
            Response.Redirect(NavigateURL(Me.ModuleContext.PortalSettings.ActiveTab.TabID, "Profile", "userId=" & ProfileUserId.ToString), True)
        End Sub

#End Region

    End Class

End Namespace
