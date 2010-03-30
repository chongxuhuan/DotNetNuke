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

Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Security.Profile
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage


Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ManageUsers UserModuleBase is used to manage Users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    '''     [cnurse]    2/21/2005   Updated to use new User UserControl
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageUsers
        Inherits UserModuleBase
        Implements IActionable

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to display the Manage Services tab
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property DisplayServices() As Boolean
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Profile_ManageServices")
                Return CType(setting, Boolean) And Not (IsEdit Or User.IsSuperUser)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Redirect URL (after successful registration)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property RedirectURL() As String
            Get
                Dim _RedirectURL As String = ""

                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Redirect_AfterRegistration")

                If CType(setting, Integer) = Null.NullInteger Then
                    If Not Request.QueryString("returnurl") Is Nothing Then
                        ' return to the url passed to register
                        _RedirectURL = HttpUtility.UrlDecode(Request.QueryString("returnurl"))
                        ' redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                        If _RedirectURL.Contains("://") Then
                            _RedirectURL = ""
                        End If

                        If _RedirectURL.Contains("?returnurl") Then
                            Dim baseURL As String = _RedirectURL.Substring(0, _RedirectURL.IndexOf("?returnurl"))
                            Dim returnURL As String = _RedirectURL.Substring(_RedirectURL.IndexOf("?returnurl") + 11)

                            _RedirectURL = String.Concat(baseURL, "?returnurl", HttpUtility.UrlEncode(returnURL))
                        End If
                    End If
                    If _RedirectURL = "" Then
                        ' redirect to current page 
                        _RedirectURL = NavigateURL()
                    End If
                Else ' redirect to after registration page
                    _RedirectURL = NavigateURL(CType(setting, Integer))
                End If

                Return _RedirectURL
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether a profile is required in AddUser mode
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property RequireProfile() As Boolean
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Security_RequireValidProfile")
                Return CType(setting, Boolean) And IsRegister
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url for the page
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ReturnUrl() As String
            Get
                Return NavigateURL(TabId, "", IIf(UserFilter <> "", UserFilter, "").ToString())
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Captcha control is used to validate registration
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UseCaptcha() As Boolean
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Security_CaptchaRegister")
                Return CType(setting, Boolean) And IsRegister
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Filter to use
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UserFilter() As String
            Get
                Dim filterString As String = IIf(Not String.IsNullOrEmpty(Request("filter")), "filter=" & Request("filter"), "").ToString()
                Dim filterProperty As String = IIf(Not String.IsNullOrEmpty(Request("filterproperty")), "filterproperty=" & Request("filterproperty"), "").ToString()
                Dim page As String = IIf(Not String.IsNullOrEmpty(Request("currentpage")), "currentpage=" & Request("currentpage"), "").ToString()

                If Not String.IsNullOrEmpty(filterString) Then
                    filterString += "&"
                End If

                If Not String.IsNullOrEmpty(filterProperty) Then
                    filterString += filterProperty + "&"
                End If

                If Not String.IsNullOrEmpty(page) Then
                    filterString += page
                End If

                Return filterString
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the current Page No
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PageNo() As Integer
            Get
                Dim _PageNo As Integer = 0
                If Not ViewState("PageNo") Is Nothing Then
                    _PageNo = CInt(ViewState("PageNo"))
                End If
                Return _PageNo
            End Get
            Set(ByVal Value As Integer)
                ViewState("PageNo") = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData binds the controls to the Data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            If Not User Is Nothing Then
                'If trying to add a SuperUser - check that user is a SuperUser
                If AddUser And IsHostMenu And Not Me.UserInfo.IsSuperUser Then
                    AddModuleMessage("NoUser", ModuleMessageType.YellowWarning, True)
                    DisableForm()
                    Exit Sub
                End If

                'Check if User is a member of the Current Portal
                If User.PortalID <> Null.NullInteger And User.PortalID <> PortalId Then
                    AddModuleMessage("InvalidUser", ModuleMessageType.YellowWarning, True)
                    DisableForm()
                    Exit Sub
                End If

                'Check if User is a SuperUser and that the current User is a SuperUser
                If User.IsSuperUser And Not Me.UserInfo.IsSuperUser Then
                    AddModuleMessage("NoUser", ModuleMessageType.YellowWarning, True)
                    DisableForm()
                    Exit Sub
                End If

                If IsEdit Then
                    'Check if user has admin rights
                    If Not IsAdmin OrElse (User.IsInRole(PortalSettings.AdministratorRoleName) AndAlso Not PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)) Then
                        AddModuleMessage("NotAuthorized", ModuleMessage.ModuleMessageType.YellowWarning, True)
                        DisableForm()
                        Exit Sub
                    End If
                Else
                    If Not IsUser Then
                        If Request.IsAuthenticated Then
                            If Not PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                                'Display current user's profile
                                Response.Redirect(NavigateURL(PortalSettings.UserTabId, "", "UserID=" + UserInfo.UserID.ToString), True)
                            End If
                        Else
                            If (User.UserID > Null.NullInteger) Then
                                AddModuleMessage("NotAuthorized", ModuleMessageType.YellowWarning, True)
                                DisableForm()
                                Exit Sub
                            End If
                        End If
                    End If
                End If

                If AddUser Then
                    If Not Request.IsAuthenticated Then
                        BindRegister()
                    Else
                        cmdRegister.Text = Localization.GetString("AddUser", LocalResourceFile)
                        lblTitle.Text = Localization.GetString("AddUser", LocalResourceFile)
                    End If
                Else
                    If Not Request.IsAuthenticated Then
                        trTitle.Visible = False
                    Else
                        If IsUser And IsProfile Then
                            trTitle.Visible = False
                        Else
                            lblTitle.Text = String.Format(Localization.GetString("UserTitle", LocalResourceFile), User.Username, User.UserID.ToString)
                        End If
                    End If
                End If

                If Not Page.IsPostBack Then
                    PageNo = 0
                End If

                ShowPanel()
            Else
                AddModuleMessage("NoUser", ModuleMessageType.YellowWarning, True)
                DisableForm()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindMembership binds the membership controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindMembership()

            ctlMembership.User = User
            ctlMembership.DataBind()

            AddModuleMessage("UserLockedOut", ModuleMessageType.YellowWarning, ctlMembership.Membership.LockedOut AndAlso (Not Page.IsPostBack))
            imgLockedOut.Visible = ctlMembership.Membership.LockedOut
            imgOnline.Visible = ctlMembership.Membership.IsOnLine

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindRegister binds the register controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/20/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindRegister()

            If UseCaptcha Then
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Me.LocalResourceFile)
                ctlCaptcha.Text = Localization.GetString("CaptchaText", Me.LocalResourceFile)
            End If

            ' Verify that the current user has access to this page
            If PortalSettings.UserRegistration = PortalRegistrationType.NoRegistration And Request.IsAuthenticated = False Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If

            lblTitle.Text = Localization.GetString("Register", LocalResourceFile)
            cmdRegister.Text = Localization.GetString("cmdRegister", LocalResourceFile)

            lblUserHelp.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS")
            Select Case PortalSettings.UserRegistration
                Case PortalRegistrationType.PrivateRegistration
                    lblUserHelp.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile)
                Case PortalRegistrationType.PublicRegistration
                    lblUserHelp.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile)
                Case PortalRegistrationType.VerifiedRegistration
                    lblUserHelp.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile)
            End Select
            lblUserHelp.Text += Localization.GetString("Required", LocalResourceFile)
            lblUserHelp.Text += Localization.GetString("RegisterWarning", LocalResourceFile)
            trHelp.Visible = True

            pnlCaptcha.Visible = UseCaptcha
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindUser binds the user controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindUser()

            If AddUser Then
                ctlUser.ShowUpdate = False
                CheckQuota()
            End If
            ctlUser.User = User
            ctlUser.DataBind()

            'Bind the Membership
            If AddUser Or (IsUser And Not IsAdmin) Then
                ctlMembership.Visible = False
            Else
                BindMembership()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CheckQuota checks whether the User Quota will be exceeded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckQuota()

            If PortalSettings.Users < PortalSettings.UserQuota Or UserInfo.IsSuperUser Or PortalSettings.UserQuota = 0 Then
                cmdRegister.Enabled = True
            Else
                cmdRegister.Enabled = False
                If IsRegister Then
                    AddModuleMessage("ExceededRegisterQuota", Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning, True)
                Else
                    AddModuleMessage("ExceededUserQuota", Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning, True)
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisableForm disbles the form (if the user is not authorised)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DisableForm()

            pnlTabs.Visible = False
            pnlUser.Visible = False
            pnlRoles.Visible = False
            pnlPassword.Visible = False
            pnlProfile.Visible = False
            pnlServices.Visible = False

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowPanel displays the correct "panel"
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowPanel()

            Dim showUser As Boolean = (PageNo = 0)
            Dim showRoles As Boolean = (PageNo = 1)
            Dim showPassword As Boolean = (PageNo = 2)
            Dim showProfile As Boolean = (PageNo = 3)
            Dim showServices As Boolean = (PageNo = 4)

            pnlRoles.Visible = showRoles
            pnlPassword.Visible = showPassword
            pnlServices.Visible = showServices

            cmdUser.Enabled = Not showUser

            If AddUser Then
                pnlTabs.Visible = False

                If Request.IsAuthenticated AndAlso MembershipProviderConfig.RequiresQuestionAndAnswer Then
                    'Admin adding user
                    pnlUser.Visible = False
                    pnlRegister.Visible = False
                    AddModuleMessage("CannotAddUser", ModuleMessageType.YellowWarning, True)
                Else
                    pnlUser.Visible = True
                    pnlRegister.Visible = True
                End If

                BindUser()

                If RequireProfile Then
                    pnlProfile.Visible = True
                    If AddUser Then
                        ctlProfile.ShowUpdate = False
                    End If
                    ctlProfile.User = User
                    ctlProfile.DataBind()
                End If
            Else
                pnlUser.Visible = showUser
                pnlProfile.Visible = showProfile

                If (Not IsAdmin And Not IsUser) Then
                    cmdPassword.Visible = False
                Else
                    cmdPassword.Enabled = Not showPassword
                End If

                If (Not IsEdit Or User.IsSuperUser) Then
                    cmdRoles.Visible = False
                Else
                    cmdRoles.Enabled = Not showRoles
                End If

                cmdProfile.Enabled = Not showProfile

                If (Not DisplayServices) Then
                    cmdServices.Visible = False
                Else
                    cmdServices.Enabled = Not showServices
                End If

                Select Case PageNo
                    Case 0
                        BindUser()
                    Case 1
                        ctlRoles.DataBind()
                    Case 2
                        ctlPassword.User = User
                        ctlPassword.DataBind()
                    Case 3
                        ctlProfile.User = User
                        ctlProfile.DataBind()
                    Case 4
                        ctlServices.User = User
                        ctlServices.DataBind()
                End Select

            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Set the Membership Control Properties
            ctlMembership.ID = "Membership"
            ctlMembership.ModuleConfiguration = Me.ModuleConfiguration
            ctlMembership.UserId = UserId

            'Set the User Control Properties
            ctlUser.ID = "User"
            ctlUser.ModuleConfiguration = Me.ModuleConfiguration
            ctlUser.UserId = UserId

            'Set the Roles Control Properties
            ctlRoles.ID = "SecurityRoles"
            ctlRoles.ModuleConfiguration = Me.ModuleConfiguration
            ctlRoles.ParentModule = Me

            'Set the Password Control Properties
            ctlPassword.ID = "Password"
            ctlPassword.ModuleConfiguration = Me.ModuleConfiguration
            ctlPassword.UserId = UserId

            'Set the Profile Control Properties
            ctlProfile.ID = "Profile"
            ctlProfile.ModuleConfiguration = Me.ModuleConfiguration
            ctlProfile.UserId = UserId

            'Set the Services Control Properties
            ctlServices.ID = "MemberServices"
            ctlServices.ModuleConfiguration = Me.ModuleConfiguration
            ctlServices.UserId = UserId

            'Customise the Control Title
            If AddUser Then
                If Not Request.IsAuthenticated Then
                    'Register
                    Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("Register.Title", Me.LocalResourceFile)
                Else
                    'Add User
                    Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("AddUser.Title", Me.LocalResourceFile)
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                'Add an Action Event Handler to the Skin
                AddActionHandler(AddressOf ModuleAction_Click)

                'Bind the User information to the controls
                BindData()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL(), True)
        End Sub

        Protected Sub cmdLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLogin.Click
            Response.Redirect(RedirectURL, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdPassword_Click runs when the Manage Password button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/02/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdPassword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdPassword.Click
            PageNo = 2
            ShowPanel()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdProfile_Click runs when the Manage profile button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/02/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdProfile_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdProfile.Click
            PageNo = 3
            ShowPanel()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRegister_Click runs when the Register button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/18/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cmdRegister_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRegister.Click
            If ((UseCaptcha AndAlso ctlCaptcha.IsValid) OrElse (Not UseCaptcha)) AndAlso _
                        ctlUser.IsValid AndAlso ((RequireProfile AndAlso ctlProfile.IsValid) OrElse (Not RequireProfile)) Then
                'hide the succesful captcha
                trCaptcha.Visible = False
                'Call the Create User method of the User control so that it can create
                'the user and raise the appropriate event(s)
                ctlUser.CreateUser()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRoles_Click runs when the Manage roles button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/02/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRoles_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRoles.Click
            PageNo = 1
            ShowPanel()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdServices_Click runs when the Manage Services button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdServices_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdServices.Click
            PageNo = 4
            ShowPanel()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUser_Click runs when the Manage user credentials button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/02/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUser.Click
            PageNo = 0
            ShowPanel()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleAction_Click handles all ModuleAction events raised from the skin
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="sender"> The object that triggers the event</param>
        ''' <param name="e">An ActionEventArgs object</param>
        ''' <history>
        ''' 	[cnurse]	03/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ModuleAction_Click(ByVal sender As Object, ByVal e As ActionEventArgs)
            Select Case e.Action.CommandArgument
                Case "ManageRoles"
                    pnlRoles.Visible = True
                    pnlUser.Visible = False
                Case "Cancel"
                    'OnCancelAction()
                Case "Delete"
                    'OnDeleteAction()
                Case "Edit"
                    'OnEditAction()
                Case "Save"
                    'OnSaveAction()
                Case Else
                    'OnModuleAction(e.Action.CommandArgument)
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MembershipAuthorized runs when the User has been unlocked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MembershipAuthorized(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlMembership.MembershipAuthorized
            Try
                AddModuleMessage("UserAuthorized", ModuleMessageType.GreenSuccess, True)

                'Send Notification to User
                If String.IsNullOrEmpty(User.Membership.Password) AndAlso _
                        Not MembershipProviderConfig.RequiresQuestionAndAnswer AndAlso _
                        MembershipProviderConfig.PasswordRetrievalEnabled Then
                    User.Membership.Password = UserController.GetPassword(User, "")
                End If
                Mail.SendMail(User, MessageType.UserRegistrationPublic, PortalSettings)

                BindMembership()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MembershipPasswordUpdateChanged runs when the Admin has forced the User to update their password
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/14/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub MembershipPasswordUpdateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlMembership.MembershipPasswordUpdateChanged
            Try
                AddModuleMessage("UserPasswordUpdateChanged", ModuleMessageType.GreenSuccess, True)

                BindMembership()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MembershipUnAuthorized runs when the User has been unlocked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MembershipUnAuthorized(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlMembership.MembershipUnAuthorized
            Try
                AddModuleMessage("UserUnAuthorized", ModuleMessageType.GreenSuccess, True)

                BindMembership()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MembershipUnLocked runs when the User has been unlocked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MembershipUnLocked(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlMembership.MembershipUnLocked
            Try
                AddModuleMessage("UserUnLocked", ModuleMessageType.GreenSuccess, True)

                BindMembership()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' PasswordQuestionAnswerUpdated runs when the Password Q and A have been updated.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/09/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PasswordQuestionAnswerUpdated(ByVal sender As Object, ByVal e As Password.PasswordUpdatedEventArgs) Handles ctlPassword.PasswordQuestionAnswerUpdated
            Dim status As PasswordUpdateStatus = e.UpdateStatus

            If status = PasswordUpdateStatus.Success Then
                AddModuleMessage("PasswordQAChanged", ModuleMessageType.GreenSuccess, True)
            Else
                AddModuleMessage(status.ToString(), ModuleMessageType.RedError, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' PasswordUpdated runs when the Password has been updated or reset
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PasswordUpdated(ByVal sender As Object, ByVal e As Password.PasswordUpdatedEventArgs) Handles ctlPassword.PasswordUpdated
            Dim status As PasswordUpdateStatus = e.UpdateStatus

            If status = PasswordUpdateStatus.Success Then
                'Send Notification to User
                Try
                    Dim accessingUser = CType(HttpContext.Current.Items("UserInfo"), UserInfo)
                    If accessingUser.UserID <> User.UserID Then
                        'The password was changed by someone else 
                        Mail.SendMail(User, MessageType.PasswordReminder, PortalSettings)
                    Else
                        'The User changed his own password
                        Mail.SendMail(User, MessageType.UserUpdatedOwnPassword, PortalSettings)
                    End If
                    AddModuleMessage("PasswordChanged", ModuleMessageType.GreenSuccess, True)
                Catch ex As Exception
                    AddModuleMessage("PasswordMailError", ModuleMessageType.YellowWarning, True)
                    LogException(ex)
                End Try
            Else
                AddModuleMessage(status.ToString(), ModuleMessageType.RedError, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProfileUpdateCompleted runs when the Profile has been updated
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/20/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProfileUpdateCompleted(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlProfile.ProfileUpdateCompleted
            If IsUser Then
                'Notify the user that his/her profile was updated
                Mail.SendMail(User, MessageType.ProfileUpdated, PortalSettings)

                Dim localeProperty As ProfilePropertyDefinition = User.Profile.GetProperty("PreferredLocale")
                If localeProperty.IsDirty Then
                    'store preferredlocale in cookie
                    Localization.SetLanguage(User.Profile.PreferredLocale)
                End If
            End If

            'Redirect to same page (this will update all controls for any changes to profile
            'and leave us at Page 0 (User Credentials)
            Response.Redirect(Request.RawUrl, True)
        End Sub

        Private Sub SubscriptionUpdated(ByVal sender As Object, ByVal e As Security.MemberServices.SubscriptionUpdatedEventArgs) Handles ctlServices.SubscriptionUpdated
            Dim message As String = Null.NullString

            If e.Cancel Then
                message = String.Format(Localization.GetString("UserUnSubscribed", Me.LocalResourceFile), e.RoleName)
            Else
                message = String.Format(Localization.GetString("UserSubscribed", Me.LocalResourceFile), e.RoleName)
            End If

            PortalSecurity.ClearRoles()

            AddLocalizedModuleMessage(message, ModuleMessageType.GreenSuccess, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserCreateCompleted runs when a new user has been Created
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/06/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserCreateCompleted(ByVal sender As Object, ByVal e As User.UserCreatedEventArgs) Handles ctlUser.UserCreateCompleted
            Dim strMessage As String = ""

            Try
                If e.CreateStatus = UserCreateStatus.Success Then
                    strMessage = CompleteUserCreation(e.CreateStatus, e.NewUser, e.Notify, IsRegister)

                    If IsRegister Then
                        If (String.IsNullOrEmpty(strMessage)) Then
                            Response.Redirect(RedirectURL, True)
                        Else
                            Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Redirect_AfterRegistration")
                            If CType(setting, Integer) = Null.NullInteger Then
                                DisableForm()
                                cmdRegister.Visible = False
                                cmdLogin.Visible = True
                            Else ' redirect to after registration page
                                Response.Redirect(RedirectURL, True)
                            End If
                            DisableForm()
                            cmdRegister.Visible = False
                            cmdLogin.Visible = True
                        End If
                    Else
                        Response.Redirect(ReturnUrl, True)
                    End If
                Else
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(e.CreateStatus), ModuleMessageType.RedError, True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserDeleted runs when the User has been deleted
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserDeleted(ByVal sender As Object, ByVal e As User.UserDeletedEventArgs) Handles ctlUser.UserDeleted
            Try
                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserUpdateCompleted runs when a user has been updated
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserUpdateCompleted(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlUser.UserUpdateCompleted
            'Redirect to same page (this will update all controls for any changes)
            Response.Redirect(Request.RawUrl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserUpdateError runs when there is an error updating the user
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	2/07/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserUpdateError(ByVal sender As Object, ByVal e As User.UserUpdateErrorArgs) Handles ctlUser.UserUpdateError, ctlUser.UserDeleteError
            AddModuleMessage(e.Message, ModuleMessageType.RedError, True)
        End Sub

#End Region

#Region "Optional Interfaces"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleActions for this ModuleControl
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                If Not IsProfile() Then
                    If Not AddUser Then
                        Actions.Add(GetNextActionID, Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", EditUrl(), False, SecurityAccessLevel.Admin, True, False)

                        If ProfileProviderConfig.CanEditProviderProperties Then
                            Actions.Add(GetNextActionID, Localization.GetString("ManageProfile.Action", LocalResourceFile), ModuleActionType.AddContent, "", "icon_profile_16px.gif", EditUrl("ManageProfile"), False, SecurityAccessLevel.Admin, True, False)
                        End If
                    End If

                    Actions.Add(GetNextActionID, Localization.GetString("Cancel.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, False, SecurityAccessLevel.Admin, True, False)
                End If
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
