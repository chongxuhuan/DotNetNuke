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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The User UserModuleBase is used to manage the base parts of a User.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/01/2006  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class User
        Inherits UserUserControlBase

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the User is valid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/21/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return Validate()
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Password section is displayed
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/17/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowPassword() As Boolean
            Get
                Return tblPassword.Visible
            End Get
            Set(ByVal Value As Boolean)
                tblPassword.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Update button
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowUpdate() As Boolean
            Get
                Return pnlUpdate.Visible
            End Get
            Set(ByVal Value As Boolean)
                pnlUpdate.Visible = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub UpdateDisplayName()
            'Update DisplayName to conform to Format
            Dim setting As Object = UserModuleBase.GetSetting(UserPortalID, "Security_DisplayNameFormat")
            If (Not setting Is Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(setting))) Then
                User.UpdateDisplayName(Convert.ToString(setting))
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Validate validates the User
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/10/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function Validate() As Boolean
            Dim _IsValid As Boolean = True

            'Check User Editor
            If _IsValid Then
                _IsValid = UserEditor.IsValid
            End If

            'Check Password is valid
            If AddUser And ShowPassword Then
                Dim createStatus As UserCreateStatus = UserCreateStatus.AddUser
                If Not chkRandom.Checked Then
                    '1. Check Password and Confirm are the same
                    If txtPassword.Text <> txtConfirm.Text Then
                        createStatus = UserCreateStatus.PasswordMismatch
                    End If
                    '2. Check Password is Valid
                    If createStatus = UserCreateStatus.AddUser And Not UserController.ValidatePassword(txtPassword.Text) Then
                        createStatus = UserCreateStatus.InvalidPassword
                    End If
                    If createStatus = UserCreateStatus.AddUser Then
                        User.Membership.Password = txtPassword.Text
                    End If
                Else
                    'Generate a random password for the user
                    User.Membership.Password = UserController.GeneratePassword()
                End If

                'Check Question/Answer
                If createStatus = UserCreateStatus.AddUser And MembershipProviderConfig.RequiresQuestionAndAnswer Then
                    If String.IsNullOrEmpty(txtQuestion.Text) Then
                        'Invalid Question
                        createStatus = UserCreateStatus.InvalidQuestion
                    Else
                        User.Membership.PasswordQuestion = txtQuestion.Text
                    End If
                    If createStatus = UserCreateStatus.AddUser Then
                        If String.IsNullOrEmpty(txtAnswer.Text) Then
                            'Invalid Question
                            createStatus = UserCreateStatus.InvalidAnswer
                        Else
                            User.Membership.PasswordAnswer = txtAnswer.Text
                        End If
                    End If
                End If

                If createStatus <> UserCreateStatus.AddUser Then
                    _IsValid = False
                    valPassword.ErrorMessage = "<br/>" + UserController.GetUserCreateStatus(createStatus)
                    valPassword.IsValid = False
                End If

            End If

            Return _IsValid
        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateUser creates a new user in the Database
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CreateUser()

            'Update DisplayName to conform to Format
            UpdateDisplayName()

            If IsRegister Then
                'Set the Approved status based on the Portal Settings
                If PortalSettings.UserRegistration = PortalRegistrationType.PublicRegistration Then
                    User.Membership.Approved = True
                Else
                    User.Membership.Approved = False
                End If
            Else
                'Set the Approved status from the value in the Authorized checkbox
                User.Membership.Approved = chkAuthorize.Checked
            End If

            Dim createStatus As UserCreateStatus = UserController.CreateUser(User)

            Dim args As UserCreatedEventArgs
            If createStatus = UserCreateStatus.Success Then
                args = New UserCreatedEventArgs(User)
                args.Notify = chkNotify.Checked
            Else       ' registration error
                args = New UserCreatedEventArgs(Nothing)
            End If
            args.CreateStatus = createStatus
            OnUserCreated(args)
            OnUserCreateCompleted(args)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            If Page.IsPostBack = False Then
                Dim confirmString As String = Localization.GetString("DeleteItem")
                If IsUser Then
                    confirmString = Localization.GetString("ConfirmUnRegister", Me.LocalResourceFile)
                End If
                ClientAPI.AddButtonConfirm(cmdDelete, confirmString)
                chkRandom.Checked = False
            End If

            If AddUser Then
                cmdDelete.Visible = False
            Else
                cmdDelete.Visible = Not (User.UserID = PortalSettings.AdministratorId) AndAlso Not (IsUser And User.IsSuperUser)
            End If

            If IsUser Then
                cmdDelete.ResourceKey = "UnRegister"
            Else
                cmdDelete.ResourceKey = "Delete"
            End If

            If AddUser Then
                pnlAddUser.Visible = True
                If IsRegister Then
                    tblAddUser.Visible = False
                    trRandom.Visible = False
                    If ShowPassword Then
                        trQuestion.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer
                        trAnswer.Visible = MembershipProviderConfig.RequiresQuestionAndAnswer
                        lblPasswordHelp.Text = Localization.GetString("PasswordHelpUser", Me.LocalResourceFile)
                    End If
                Else
                    lblPasswordHelp.Text = Localization.GetString("PasswordHelpAdmin", Me.LocalResourceFile)
                End If
                txtConfirm.Attributes.Add("value", txtConfirm.Text)
                txtPassword.Attributes.Add("value", txtPassword.Text)
            End If

            UserEditor.DataSource = User
            UserEditor.DataBind()

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
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Get the base Page
            Dim basePage As PageBase = TryCast(Me.Page, PageBase)
            If basePage IsNot Nothing Then
                'Check if culture is RTL
                If basePage.PageCulture.TextInfo.IsRightToLeft Then
                    UserEditor.LabelMode = LabelMode.Right
                Else
                    UserEditor.LabelMode = LabelMode.Left
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
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            UserEditor.LocalResourceFile = Me.LocalResourceFile
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the delete Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Dim name As String = User.Username
            Dim id As Integer = UserId

            If UserController.DeleteUser(User, True, False) Then
                OnUserDeleted(New UserDeletedEventArgs(id, name))
            Else
                OnUserDeleteError(New UserUpdateErrorArgs(id, name, "UserDeleteError"))
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            If AddUser Then
                If IsValid Then
                    CreateUser()
                End If
            Else
                If UserEditor.IsValid AndAlso UserEditor.IsDirty AndAlso (Not User Is Nothing) Then
                    If User.UserID = PortalSettings.AdministratorId Then
                        'Clear the Portal Cache
                        DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(UserPortalID, False)
                    End If
                    Try
                        'Update DisplayName to conform to Format
                        UpdateDisplayName()

                        UserController.UpdateUser(UserPortalID, User)
                        OnUserUpdated(EventArgs.Empty)
                        OnUserUpdateCompleted(EventArgs.Empty)
                    Catch ex As Exception
                        Dim args As New UserUpdateErrorArgs(User.UserID, User.Username, "EmailError")
                        OnUserUpdateError(args)
                    End Try
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserEditorCreated runs when editor row in the Property Editor is created.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub UserEditorCreated(ByVal sender As Object, ByVal e As UI.WebControls.PropertyEditorItemEventArgs) Handles UserEditor.ItemCreated
            Dim setting As Object = Nothing

            Select Case e.Editor.Name.ToLower
                Case "displayname"
                    setting = UserModuleBase.GetSetting(UserPortalID, "Security_DisplayNameFormat")
                    If (Not setting Is Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(setting))) Then
                        If AddUser Then
                            e.Editor.Visible = False
                        Else
                            e.Editor.EditMode = PropertyEditorMode.View
                        End If
                    End If
                Case "email"
                    setting = UserModuleBase.GetSetting(UserPortalID, "Security_EmailValidation")
                    If (Not setting Is Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(setting))) Then
                        e.Editor.ValidationExpression = Convert.ToString(setting)
                    End If
            End Select
        End Sub

#End Region

    End Class

End Namespace