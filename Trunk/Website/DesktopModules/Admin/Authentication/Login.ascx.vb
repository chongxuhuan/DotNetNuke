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
Imports System.Collections.Generic
Imports System.IO

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.UI.UserControls
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Authentication

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : Authentication
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Signin UserModuleBase is used to provide a login for a registered user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]        07/03/2007   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Login
        Inherits UserModuleBase

#Region "Private Members"

        Private loginControls As New List(Of AuthenticationLoginBase)

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the current AuthenticationType
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/12/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property AuthenticationType() As String
            Get
                Dim _AuthenticationType As String = Null.NullString
                If Not ViewState("AuthenticationType") Is Nothing Then
                    _AuthenticationType = CStr(ViewState("AuthenticationType"))
                End If
                Return _AuthenticationType
            End Get
            Set(ByVal Value As String)
                ViewState("AuthenticationType") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag that determines whether the user should be automatically registered
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/16/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property AutoRegister() As Boolean
            Get
                Dim _AutoRegister As Boolean = Null.NullBoolean
                If Not ViewState("AutoRegister") Is Nothing Then
                    _AutoRegister = CBool(ViewState("AutoRegister"))
                End If
                Return _AutoRegister
            End Get
            Set(ByVal Value As Boolean)
                ViewState("AutoRegister") = Value
            End Set
        End Property

        Protected Property ProfileProperties() As NameValueCollection
            Get
                Dim _Profile As New NameValueCollection
                If Not ViewState("ProfileProperties") Is Nothing Then
                    _Profile = CType(ViewState("ProfileProperties"), NameValueCollection)
                End If
                Return _Profile
            End Get
            Set(ByVal value As NameValueCollection)
                ViewState("ProfileProperties") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the current Page No
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/09/2006  Created
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property PageNo() As Integer
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Redirect URL (after successful login)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/18/2006  Created
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property RedirectURL() As String
            Get
                Dim _RedirectURL As String = ""

                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Redirect_AfterLogin")

                If CType(setting, Integer) = Null.NullInteger Then
                    If Not Request.QueryString("returnurl") Is Nothing Then
                        ' return to the url passed to signin
                        _RedirectURL = HttpUtility.UrlDecode(Request.QueryString("returnurl"))
                        ' redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                        If _RedirectURL.Contains("://") Then
                            _RedirectURL = ""
                        End If
                    End If
                    If Not Request.Params("appctx") Is Nothing Then
                        'HACK return to the url passed to signin (LiveID) 
                        _RedirectURL = HttpUtility.UrlDecode(Request.Params("appctx"))
                        ' redirect url should never contain a protocol ( if it does, it is likely a cross-site request forgery attempt )
                        If _RedirectURL.Contains("://") Then
                            _RedirectURL = ""
                        End If
                    End If
                    If _RedirectURL = "" Then
                        If PortalSettings.LoginTabId <> -1 And PortalSettings.HomeTabId <> -1 Then
                            ' redirect to portal home page specified
                            _RedirectURL = NavigateURL(PortalSettings.HomeTabId)
                        Else
                            ' redirect to current page 
                            _RedirectURL = NavigateURL()
                        End If
                    End If
                Else ' redirect to after login page
                    _RedirectURL = NavigateURL(CType(setting, Integer))
                End If

                'replace language parameter in querystring, to make sure that user will see page in correct language
                If UserId <> -1 Then
                    If User.Profile.PreferredLocale <> CultureInfo.CurrentCulture.Name Then
                        _RedirectURL = UrlUtils.ReplaceQSParam(_RedirectURL, "language", User.Profile.PreferredLocale)
                    End If
                End If

                'check for insecure account defaults
                Dim qsDelimiter As String = "?"
                If _RedirectURL.Contains("?") Then
                    qsDelimiter = "&"
                End If
                If LoginStatus = UserLoginStatus.LOGIN_INSECUREADMINPASSWORD Then
                    _RedirectURL = _RedirectURL & qsDelimiter & "runningDefault=1"
                ElseIf LoginStatus = UserLoginStatus.LOGIN_INSECUREHOSTPASSWORD Then
                    _RedirectURL = _RedirectURL & qsDelimiter & "runningDefault=2"
                End If

                Return _RedirectURL

            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Captcha control is used to validate the login
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/12/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UseCaptcha() As Boolean
            Get
                Dim setting As Object = GetSetting(PortalId, "Security_CaptchaLogin")
                Return CType(setting, Boolean)
            End Get
        End Property

        Protected Property LoginStatus() As UserLoginStatus
            Get
                Dim _LoginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
                If Not ViewState("LoginStatus") Is Nothing Then
                    _LoginStatus = CType(ViewState("LoginStatus"), UserLoginStatus)
                End If
                Return _LoginStatus
            End Get
            Set(ByVal value As UserLoginStatus)
                ViewState("LoginStatus") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the current UserToken
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/12/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property UserToken() As String
            Get
                Dim _UserToken As String = 0
                If Not ViewState("UserToken") Is Nothing Then
                    _UserToken = CStr(ViewState("UserToken"))
                End If
                Return _UserToken
            End Get
            Set(ByVal Value As String)
                ViewState("UserToken") = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub DisplayLoginControl(ByVal authLoginControl As AuthenticationLoginBase, ByVal addHeader As Boolean, ByVal addFooter As Boolean)
            'Create a <div> to hold the control
            Dim container As HtmlGenericControl = New HtmlGenericControl()
            container.TagName = "div"
            container.ID = authLoginControl.AuthenticationType

            'Add Settings Control to Container
            container.Controls.Add(authLoginControl)

            'Add a Section Header
            Dim sectionHeadControl As SectionHeadControl = Nothing
            If addHeader Then
                sectionHeadControl = CType(LoadControl("~/controls/SectionHeadControl.ascx"), SectionHeadControl)
                sectionHeadControl.IncludeRule = True
                sectionHeadControl.CssClass = "Head"
                sectionHeadControl.Text = Localization.GetString("Title", authLoginControl.LocalResourceFile)

                sectionHeadControl.Section = container.ID

                'Add Section Head Control to Container
                pnlLoginContainer.Controls.Add(sectionHeadControl)
            End If

            'Add Container to Controls
            pnlLoginContainer.Controls.Add(container)


            'Add LineBreak
            If addFooter Then
                pnlLoginContainer.Controls.Add(New LiteralControl("<br/>"))
            End If

            pnlLoginContainer.Visible = True
        End Sub

        Private Sub DisplayTabbedLoginControl(ByVal authLoginControl As AuthenticationLoginBase, ByVal Tabs As DotNetNuke.UI.WebControls.TabStripTabCollection)

            Dim tab As New DotNetNuke.UI.WebControls.DNNTab(Localization.GetString("Title", authLoginControl.LocalResourceFile))
            tab.ID = authLoginControl.AuthenticationType

            tab.Controls.Add(authLoginControl)
            Tabs.Add(tab)
            tsLogin.Visible = True

        End Sub

        Private Sub BindLoginControl(ByVal authLoginControl As AuthenticationLoginBase, ByVal authSystem As AuthenticationInfo)
            ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
            ' this is necessary for the Localization in PageBase
            authLoginControl.AuthenticationType = authSystem.AuthenticationType
            authLoginControl.ID = Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc) + "_" + authSystem.AuthenticationType
            authLoginControl.LocalResourceFile = authLoginControl.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/" & Path.GetFileNameWithoutExtension(authSystem.LoginControlSrc)
            authLoginControl.RedirectURL = RedirectURL
            authLoginControl.ModuleConfiguration = Me.ModuleConfiguration

            'attempt to inject control attributes
            AddLoginControlAttributes(authLoginControl)
            AddHandler authLoginControl.UserAuthenticated, AddressOf Me.UserAuthenticated
        End Sub

        Private Sub BindLogin()
            If PortalSettings.UserRegistration = PortalRegistrationType.NoRegistration Then
                tdRegister.Visible = False
            End If
            lblLogin.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_LOGIN_INSTRUCTIONS")

            Dim authSystems As List(Of AuthenticationInfo) = AuthenticationController.GetEnabledAuthenticationServices()
            Dim defaultLoginControl As AuthenticationLoginBase = Nothing

            For Each authSystem As AuthenticationInfo In authSystems
                Try
                    Dim authLoginControl As AuthenticationLoginBase = CType(LoadControl("~/" & authSystem.LoginControlSrc), AuthenticationLoginBase)

                    BindLoginControl(authLoginControl, authSystem)

                    If authSystem.AuthenticationType = "DNN" Then
                        defaultLoginControl = authLoginControl
                    End If

                    'Check if AuthSystem is Enabled
                    If authLoginControl.Enabled Then
                        'Add Login Control to List
                        loginControls.Add(authLoginControl)
                    End If
                Catch ex As Exception
                    LogException(ex)
                End Try
            Next

            Dim authCount As Integer = loginControls.Count
            Select Case authCount
                Case 0
                    'No enabled controls - inject default dnn control
                    If defaultLoginControl Is Nothing Then
                        'No controls enabled for portal, and default DNN control is not enabled by host, so load system default (DNN)
                        Dim authSystem As AuthenticationInfo = AuthenticationController.GetAuthenticationServiceByType("DNN")
                        Dim authLoginControl As AuthenticationLoginBase = CType(LoadControl("~/" & authSystem.LoginControlSrc), AuthenticationLoginBase)
                        BindLoginControl(authLoginControl, authSystem)
                        DisplayLoginControl(authLoginControl, False, False)
                    Else
                        'Portal has no login controls enabled so load default DNN control
                        DisplayLoginControl(defaultLoginControl, False, False)
                    End If
                Case 1
                    'We don't want the control to render with tabbed interface
                    DisplayLoginControl(loginControls(0), False, False)
                Case Else
                    For Each authLoginControl As AuthenticationLoginBase In loginControls
                        DisplayTabbedLoginControl(authLoginControl, tsLogin.Tabs)
                    Next
            End Select
        End Sub

        Private Sub AddLoginControlAttributes(ByVal loginControl As AuthenticationLoginBase)
            'search selected authentication control for username and password fields
            'and inject autocomplete=off so browsers do not remember sensitive details
            Dim username As WebControl = loginControl.FindControl("txtUsername")
            If Not username Is Nothing Then
                username.Attributes.Add("AUTOCOMPLETE", "off")
            End If
            Dim password As WebControl = loginControl.FindControl("txtPassword")
            If Not password Is Nothing Then
                password.Attributes.Add("AUTOCOMPLETE", "off")
            End If

            'see if the portal supports persistant cookies
            Dim rememberme As CheckBox = CType(Me.FindControl("chkCookie"), CheckBox)
            rememberme.Visible = Host.RememberCheckbox
        End Sub

        Private Sub BindRegister()
            lblType.Text = AuthenticationType
            lblToken.Text = UserToken

            ' Verify that the current user has access to this page
            If PortalSettings.UserRegistration = PortalRegistrationType.NoRegistration And Request.IsAuthenticated = False Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If

            lblRegisterHelp.Text = Localization.GetSystemMessage(PortalSettings, "MESSAGE_REGISTRATION_INSTRUCTIONS")
            Select Case PortalSettings.UserRegistration
                Case PortalRegistrationType.PrivateRegistration
                    lblRegisterHelp.Text += Localization.GetString("PrivateMembership", Localization.SharedResourceFile)
                Case PortalRegistrationType.PublicRegistration
                    lblRegisterHelp.Text += Localization.GetString("PublicMembership", Localization.SharedResourceFile)
                Case PortalRegistrationType.VerifiedRegistration
                    lblRegisterHelp.Text += Localization.GetString("VerifiedMembership", Localization.SharedResourceFile)
            End Select

            If AutoRegister Then
                InitialiseUser()
            End If

            Dim UserValid As String = True
            If String.IsNullOrEmpty(User.Username) OrElse String.IsNullOrEmpty(User.Email) OrElse String.IsNullOrEmpty(User.FirstName) OrElse String.IsNullOrEmpty(User.LastName) Then
                UserValid = Null.NullBoolean
            End If

            If AutoRegister AndAlso UserValid Then
                ctlUser.Visible = False
                lblRegisterTitle.Text = Localization.GetString("CreateTitle", LocalResourceFile)
                cmdCreateUser.Text = Localization.GetString("cmdCreate", LocalResourceFile)
            Else
                lblRegisterHelp.Text += Localization.GetString("Required", Localization.SharedResourceFile)
                lblRegisterTitle.Text = Localization.GetString("RegisterTitle", LocalResourceFile)
                cmdCreateUser.Text = Localization.GetString("cmdRegister", LocalResourceFile)
                ctlUser.ShowPassword = False
                ctlUser.ShowUpdate = False
                ctlUser.User = User
                ctlUser.DataBind()
            End If
        End Sub

        Private Sub InitialiseUser()
            'Set UserName to authentication Token
            User.Username = UserToken.Replace("http://", "").TrimEnd("/"c)

            'Load any Profile properties that may have been returned
            UpdateProfile(User, False)

            'Set DisplayName to UserToken if null
            If String.IsNullOrEmpty(User.DisplayName) Then
                User.DisplayName = UserToken.Replace("http://", "").TrimEnd("/"c)
            End If

            'Parse DisplayName into FirstName/LastName
            If User.DisplayName.IndexOf(" "c) > 0 Then
                User.FirstName = User.DisplayName.Substring(0, User.DisplayName.IndexOf(" "c))
                User.LastName = User.DisplayName.Substring(User.DisplayName.IndexOf(" "c) + 1)
            End If

            'Set FirstName to Authentication Type (if null)
            If String.IsNullOrEmpty(User.FirstName) Then
                User.FirstName = AuthenticationType
            End If

            'Set FirstName to "User" (if null)
            If String.IsNullOrEmpty(User.LastName) Then
                User.LastName = "User"
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowPanel controls what "panel" is to be displayed
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/21/2006
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowPanel()
            Dim showLogin As Boolean = (PageNo = 0)
            Dim showRegister As Boolean = (PageNo = 1)
            Dim showPassword As Boolean = (PageNo = 2)
            Dim showProfile As Boolean = (PageNo = 3)

            pnlProfile.Visible = showProfile
            pnlPassword.Visible = showPassword
            pnlLogin.Visible = showLogin
            pnlRegister.Visible = showRegister
            pnlAssociate.Visible = showRegister

            Select Case PageNo
                Case 0
                    BindLogin()
                Case 1
                    BindRegister()
                Case 2
                    ctlPassword.UserId = UserId
                    ctlPassword.DataBind()
                Case 3
                    ctlProfile.UserId = UserId
                    ctlProfile.DataBind()
            End Select

        End Sub

        Private Sub UpdateProfile(ByVal objUser As UserInfo, ByVal update As Boolean)
            Dim bUpdateUser As Boolean
            If ProfileProperties.Count > 0 Then
                For Each key As String In ProfileProperties
                    Select Case key
                        Case "FirstName"
                            If objUser.FirstName <> ProfileProperties(key) Then
                                objUser.FirstName = ProfileProperties(key)
                                bUpdateUser = True
                            End If
                        Case "LastName"
                            If objUser.LastName <> ProfileProperties(key) Then
                                objUser.LastName = ProfileProperties(key)
                                bUpdateUser = True
                            End If
                        Case "Email"
                            If objUser.Email <> ProfileProperties(key) Then
                                objUser.Email = ProfileProperties(key)
                                bUpdateUser = True
                            End If
                        Case "DisplayName"
                            If objUser.DisplayName <> ProfileProperties(key) Then
                                objUser.DisplayName = ProfileProperties(key)
                                bUpdateUser = True
                            End If
                        Case Else
                            objUser.Profile.SetProfileProperty(key, ProfileProperties(key))
                    End Select
                Next
                If update Then
                    If bUpdateUser Then
                        UserController.UpdateUser(PortalId, objUser)
                    End If
                    ProfileController.UpdateUserProfile(objUser)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ValidateUser runs when the user has been authorized by the data store.  It validates for
        ''' things such as an expiring password, valid profile, or missing DNN User Association
        ''' </summary>
        ''' <param name="objUser">The logged in User</param>
        ''' <param name="ignoreExpiring">Ignore the situation where the password is expiring (but not yet expired)</param>
        ''' <history>
        ''' 	[cnurse]	03/15/2006
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ValidateUser(ByVal objUser As UserInfo, ByVal ignoreExpiring As Boolean)
            Dim validStatus As UserValidStatus = UserValidStatus.VALID
            Dim strMessage As String = Null.NullString
            Dim expiryDate As DateTime = Null.NullDate

            If Not objUser.IsSuperUser Then
                validStatus = UserController.ValidateUser(objUser, PortalId, ignoreExpiring)
            End If

            If PasswordConfig.PasswordExpiry > 0 Then
                expiryDate = objUser.Membership.LastPasswordChangeDate.AddDays(PasswordConfig.PasswordExpiry)
            End If

            UserId = objUser.UserID

            'Check if the User has valid Password/Profile
            Select Case validStatus
                Case UserValidStatus.VALID
                    'Set the Page Culture(Language) based on the Users Preferred Locale
                    If (Not objUser.Profile Is Nothing) AndAlso (Not objUser.Profile.PreferredLocale Is Nothing) Then
                        Localization.SetLanguage(objUser.Profile.PreferredLocale)
                    Else
                        Localization.SetLanguage(PortalSettings.DefaultLanguage)
                    End If

                    'Set the Authentication Type used 
                    AuthenticationController.SetAuthenticationType(AuthenticationType)

                    'Complete Login
                    UserController.UserLogin(PortalId, objUser, PortalSettings.PortalName, AuthenticationLoginBase.GetIPAddress(), chkCookie.Checked)

                    ' redirect browser
                    Response.Redirect(RedirectURL, True)
                Case UserValidStatus.PASSWORDEXPIRED
                    strMessage = String.Format(Localization.GetString("PasswordExpired", Me.LocalResourceFile), expiryDate.ToLongDateString)
                    AddLocalizedModuleMessage(strMessage, ModuleMessageType.YellowWarning, True)
                    PageNo = 2
                    pnlProceed.Visible = False
                Case UserValidStatus.PASSWORDEXPIRING
                    strMessage = String.Format(Localization.GetString("PasswordExpiring", Me.LocalResourceFile), expiryDate.ToLongDateString)
                    AddLocalizedModuleMessage(strMessage, ModuleMessageType.YellowWarning, True)
                    PageNo = 2
                    pnlProceed.Visible = True
                Case UserValidStatus.UPDATEPASSWORD
                    AddModuleMessage("PasswordUpdate", ModuleMessageType.YellowWarning, True)
                    PageNo = 2
                    pnlProceed.Visible = False
                Case UserValidStatus.UPDATEPROFILE
                    'Admin has forced profile update
                    AddModuleMessage("ProfileUpdate", ModuleMessageType.YellowWarning, True)
                    PageNo = 3
            End Select

            ShowPanel()

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
        ''' 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    07/08/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'Set the User Control Properties
            ctlUser.ID = "User"

            'Set the Profile Control Properties
            ctlPassword.ID = "Password"

            'Set the Profile Control Properties
            ctlProfile.ID = "Profile"

            'Override the redirected page title if page has loaded with ctl=Login
            If Not Request.QueryString("ctl") Is Nothing Then
                If Request.QueryString("ctl").ToLower = "login" Then
                    Dim myPage As DotNetNuke.Framework.CDefault
                    myPage = CType(Me.Page, CDefault)
                    If myPage.PortalSettings.LoginTabId = Me.TabId Or myPage.PortalSettings.LoginTabId = -1 Then
                        myPage.Title = Localization.GetString("ControlTitle_login", Me.LocalResourceFile)
                    End If
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
        ''' 	[cnurse]	9/8/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    07/08/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' Verify if portal has a customized login page
            If Not Null.IsNull(PortalSettings.LoginTabId) And IsAdminControl() Then
                If ValidateLoginTabID(PortalSettings.LoginTabId) Then
                    ' login page exists and trying to access this control directly with url param -> not allowed
                    Dim params(2) As String
                    If Not String.IsNullOrEmpty(Request.QueryString("returnUrl")) Then
                        params(0) = "returnUrl=" & Request.QueryString("returnUrl")
                    End If
                    If Not String.IsNullOrEmpty(Request.QueryString("username")) Then
                        params(1) = "username=" & Request.QueryString("username")
                    End If
                    If Not String.IsNullOrEmpty(Request.QueryString("verificationcode")) Then
                        params(2) = "verificationcode=" & Request.QueryString("verificationcode")
                    End If
                    Response.Redirect(NavigateURL(PortalSettings.LoginTabId, "", params))
                End If
            End If

            If Page.IsPostBack = False Then
                Try
                    PageNo = 0
                Catch
                    'control not there 
                End Try
            End If

            If Not Request.IsAuthenticated Then
                ShowPanel()
            Else ' user is already authenticated
                ' if a Login Page has not been specified for the portal
                If IsAdminControl() Then
                    ' redirect to current page 
                    Response.Redirect(NavigateURL(), True)
                Else ' make module container invisible if user is not a page admin
                    If TabPermissionController.CanAdminPage() Then
                        ShowPanel()
                    Else
                        ContainerControl.Visible = False
                    End If
                End If
            End If

            trCaptcha.Visible = UseCaptcha

            If UseCaptcha Then
                ctlCaptcha.ErrorMessage = Localization.GetString("InvalidCaptcha", Localization.SharedResourceFile)
                ctlCaptcha.Text = Localization.GetString("CaptchaText", Localization.SharedResourceFile)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdAssociate_Click runs when the associate button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/12/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cmdAssociate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAssociate.Click
            If (UseCaptcha And ctlCaptcha.IsValid) OrElse (Not UseCaptcha) Then
                Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
                Dim objUser As UserInfo = UserController.ValidateUser(PortalId, txtUsername.Text, txtPassword.Text, "DNN", "", PortalSettings.PortalName, AuthenticationLoginBase.GetIPAddress(), loginStatus)

                If loginStatus = UserLoginStatus.LOGIN_SUCCESS Then
                    'Assocate alternate Login with User and proceed with Login
                    AuthenticationController.AddUserAuthentication(objUser.UserID, AuthenticationType, UserToken)

                    If Not objUser Is Nothing Then
                        UpdateProfile(objUser, True)
                    End If

                    ValidateUser(objUser, True)
                Else
                    AddModuleMessage("AssociationFailed", ModuleMessageType.RedError, True)
                End If

            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCreateUser runs when the register (as new user) button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/12/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cmdCreateUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCreateUser.Click
            'Generate a random password for the user
            User.Membership.Password = UserController.GeneratePassword()

            If AutoRegister Then
                ctlUser.User = User

                'Call the Create User method of the User control so that it can create
                'the user and raise the appropriate event(s)
                ctlUser.CreateUser()
            Else
                If ctlUser.IsValid Then
                    'Call the Create User method of the User control so that it can create
                    'the user and raise the appropriate event(s)
                    ctlUser.CreateUser()
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdPassword_Click runs when the Password Reminder button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdPassword_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdPassword.Click
            Dim ReturnUrl As String = NavigateURL()
            If Not String.IsNullOrEmpty(Request.QueryString("returnurl")) Then
                ReturnUrl = Request.QueryString("returnurl")
            End If
            ReturnUrl = HttpUtility.UrlEncode(ReturnUrl)

            Response.Redirect(NavigateURL("SendPassword", "returnurl=" + ReturnUrl), True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdProceed_Click runs when the Proceed Anyway button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	06/30/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdProceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdProceed.Click
            Dim _User As UserInfo = ctlPassword.User
            ValidateUser(_User, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRegister_Click runs when the register button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/24/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    12/11/2005  Updated to reflect abstraction of Membership
        '''     [cnurse]    07/03/2007  Moved from Sign.ascx.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRegister_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRegister.Click
            If PortalSettings.UserRegistration <> PortalRegistrationType.NoRegistration Then
                Dim ReturnUrl As String = NavigateURL()
                If Not String.IsNullOrEmpty(Request.QueryString("returnurl")) Then
                    ReturnUrl = Request.QueryString("returnurl")
                End If
                ReturnUrl = HttpUtility.UrlEncode(ReturnUrl)

                If PortalSettings.UserRegistration <> PortalRegistrationType.NoRegistration Then
                    'Pass return url to register as original url  and return to this control to login after regsitration
                    Response.Redirect(RegisterURL(ReturnUrl, Null.NullString), True)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' PasswordUpdated runs when the password is updated
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/15/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PasswordUpdated(ByVal sender As Object, ByVal e As Users.Password.PasswordUpdatedEventArgs) Handles ctlPassword.PasswordUpdated

            Dim status As PasswordUpdateStatus = e.UpdateStatus

            If status = PasswordUpdateStatus.Success Then
                AddModuleMessage("PasswordChanged", ModuleMessageType.GreenSuccess, True)

                'Authorize User
                Dim _User As UserInfo = ctlPassword.User
                _User.Membership.LastPasswordChangeDate = Now()
                _User.Membership.UpdatePassword = False
                If _User.IsSuperUser Then
                    LoginStatus = UserLoginStatus.LOGIN_SUPERUSER
                Else
                    LoginStatus = UserLoginStatus.LOGIN_SUCCESS
                End If
                UserController.CheckInsecurePassword(_User.Username, _User.Membership.Password, LoginStatus)
                ValidateUser(_User, True)
            Else
                AddModuleMessage(status.ToString(), ModuleMessageType.RedError, True)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProfileUpdated runs when the profile is updated
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/16/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProfileUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlProfile.ProfileUpdated
            'Authorize User
            ValidateUser(ctlProfile.User, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserAuthenticated runs when the user is authenticated by one of the child
        ''' Authentication controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/10/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserAuthenticated(ByVal sender As Object, ByVal e As UserAuthenticatedEventArgs)
            LoginStatus = e.LoginStatus

            'Check the Login Status
            Select Case LoginStatus
                Case UserLoginStatus.LOGIN_USERNOTAPPROVED
                    Select Case e.Message
                        Case "EnterCode"
                            AddModuleMessage(e.Message, ModuleMessageType.YellowWarning, True)
                        Case "InvalidCode", "UserNotAuthorized"
                            AddModuleMessage(e.Message, ModuleMessageType.RedError, True)
                        Case Else
                            AddLocalizedModuleMessage(e.Message, ModuleMessageType.RedError, True)
                    End Select
                Case UserLoginStatus.LOGIN_USERLOCKEDOUT
                    AddLocalizedModuleMessage(String.Format(Localization.GetString("UserLockedOut", Me.LocalResourceFile), Host.AutoAccountUnlockDuration), ModuleMessageType.RedError, True)
                    ' notify administrator about account lockout ( possible hack attempt )
                    Dim Custom As New ArrayList
                    Custom.Add(e.UserToken)
                    Mail.SendMail(PortalSettings.Email, PortalSettings.Email, "", _
                        Localization.GetSystemMessage(PortalSettings, "EMAIL_USER_LOCKOUT_SUBJECT", Localization.GlobalResourceFile, Custom), _
                        Localization.GetSystemMessage(PortalSettings, "EMAIL_USER_LOCKOUT_BODY", Localization.GlobalResourceFile, Custom), _
                        "", "", "", "", "", "")
                Case UserLoginStatus.LOGIN_FAILURE
                    'A Login Failure can mean one of two things:
                    '  1 - User was authenticated by the Authentication System but is not "affiliated" with a DNN Account
                    '  2 - User was not authenticated
                    If e.Authenticated Then
                        PageNo = 1
                        AuthenticationType = e.AuthenticationType
                        AutoRegister = e.AutoRegister
                        ProfileProperties = e.Profile
                        UserToken = e.UserToken

                        ShowPanel()
                    Else
                        If String.IsNullOrEmpty(e.Message) Then
                            AddModuleMessage("LoginFailed", ModuleMessageType.RedError, True)
                        Else
                            Me.AddLocalizedModuleMessage(e.Message, ModuleMessageType.RedError, True)
                        End If
                    End If
                Case Else
                    If e.User IsNot Nothing Then
                        'First update the profile (if any properties have been passed)
                        AuthenticationType = e.AuthenticationType
                        ProfileProperties = e.Profile

                        UpdateProfile(e.User, True)

                        ValidateUser(e.User, False)
                    End If
            End Select

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserCreateCompleted runs when a new user has been Created
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/12/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UserCreateCompleted(ByVal sender As Object, ByVal e As Entities.Modules.UserUserControlBase.UserCreatedEventArgs) Handles ctlUser.UserCreateCompleted
            Dim strMessage As String = ""

            Try
                If e.CreateStatus = UserCreateStatus.Success Then
                    'Assocate alternate Login with User and proceed with Login
                    AuthenticationController.AddUserAuthentication(e.NewUser.UserID, AuthenticationType, UserToken)

                    strMessage = CompleteUserCreation(e.CreateStatus, e.NewUser, e.Notify, True)
                    If (String.IsNullOrEmpty(strMessage)) Then
                        'First update the profile (if any properties have been passed)
                        UpdateProfile(e.NewUser, True)

                        ValidateUser(e.NewUser, True)
                    End If
                Else
                    AddLocalizedModuleMessage(UserController.GetUserCreateStatus(e.CreateStatus), ModuleMessageType.RedError, True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace

