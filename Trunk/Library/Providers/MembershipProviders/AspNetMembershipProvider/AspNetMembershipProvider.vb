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

Imports AspNetSecurity = System.Web.Security
Imports AspNetProfile = System.Web.Profile

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Security.Membership.Data
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Security.Membership

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Security.Membership
    ''' Class:      AspNetMembershipProvider
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The AspNetMembershipProvider overrides the default MembershipProvider to provide
    ''' an AspNet Membership Component (MemberRole) implementation
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	12/09/2005	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AspNetMembershipProvider
        Inherits DotNetNuke.Security.Membership.MembershipProvider

#Region "Private Members"

        Private dataProvider As DotNetNuke.Security.Membership.Data.DataProvider

#End Region

#Region "Constructors"

        Public Sub New()
            dataProvider = DotNetNuke.Security.Membership.Data.DataProvider.Instance()
            If dataProvider Is Nothing Then
                ' get the provider configuration based on the type
                Dim defaultprovider As String = DotNetNuke.Data.DataProvider.Instance.DefaultProviderName
                Dim dataProviderNamespace As String = "DotNetNuke.Security.Membership.Data"
                If defaultprovider = "SqlDataProvider" Then
                    dataProvider = New SqlDataProvider
                Else
                    Dim providerType As String = dataProviderNamespace + "." + defaultprovider
                    dataProvider = CType(Framework.Reflection.CreateObject(providerType, providerType, True), DataProvider)
                End If
                ComponentFactory.RegisterComponentInstance(Of DataProvider)(dataProvider)
            End If
        End Sub

#End Region

#Region "Private Methods"

        Private Function AutoUnlockUser(ByVal aspNetUser As AspNetSecurity.MembershipUser) As Boolean
            If Host.AutoAccountUnlockDuration <> 0 Then
                If aspNetUser.LastLockoutDate < Date.Now.AddMinutes(-1 * Host.AutoAccountUnlockDuration) Then
                    'Unlock user in Data Store
                    If aspNetUser.UnlockUser() Then
                        Return True
                    End If
                End If
            End If

            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateDNNUser persists the DNN User information to the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <returns>The UserId of the newly created user.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateDNNUser(ByRef user As UserInfo) As UserCreateStatus

            Dim objSecurity As New PortalSecurity
            Dim userName As String = objSecurity.InputFilter(user.Username, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim email As String = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim lastName As String = objSecurity.InputFilter(user.LastName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim firstName As String = objSecurity.InputFilter(user.FirstName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim createStatus As UserCreateStatus = UserCreateStatus.Success
            Dim displayName As String = objSecurity.InputFilter(user.DisplayName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim updatePassword As Boolean = user.Membership.UpdatePassword
            Dim isApproved As Boolean = user.Membership.Approved

            Try
                user.UserID = CType(dataProvider.AddUser(user.PortalID, userName, firstName, lastName, user.AffiliateID, user.IsSuperUser, email, displayName, updatePassword, isApproved, UserController.GetCurrentUserInfo.UserID), Integer)
            Catch ex As Exception
                'Clear User (duplicate User information)
                LogException(ex)
                user = Nothing
                createStatus = UserCreateStatus.ProviderError
            End Try

            Return createStatus

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateMemberhipUser persists the User as an AspNet MembershipUser to the AspNet
        ''' Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <returns>A UserCreateStatus enumeration indicating success or reason for failure.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateMemberhipUser(ByVal user As UserInfo) As UserCreateStatus

            Dim objSecurity As New PortalSecurity
            Dim userName As String = objSecurity.InputFilter(user.Username, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim email As String = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim objStatus As AspNetSecurity.MembershipCreateStatus = AspNetSecurity.MembershipCreateStatus.Success

            Dim objMembershipUser As AspNetSecurity.MembershipUser

            If MembershipProviderConfig.RequiresQuestionAndAnswer Then
                objMembershipUser = AspNetSecurity.Membership.CreateUser(userName, user.Membership.Password, email, user.Membership.PasswordQuestion, user.Membership.PasswordAnswer, True, objStatus)
            Else
                objMembershipUser = AspNetSecurity.Membership.CreateUser(userName, user.Membership.Password, email, Nothing, Nothing, True, objStatus)
            End If

            Dim createStatus As UserCreateStatus = UserCreateStatus.Success
            Select Case objStatus
                Case AspNetSecurity.MembershipCreateStatus.DuplicateEmail
                    createStatus = UserCreateStatus.DuplicateEmail
                Case AspNetSecurity.MembershipCreateStatus.DuplicateProviderUserKey
                    createStatus = UserCreateStatus.DuplicateProviderUserKey
                Case AspNetSecurity.MembershipCreateStatus.DuplicateUserName
                    createStatus = UserCreateStatus.DuplicateUserName
                Case AspNetSecurity.MembershipCreateStatus.InvalidAnswer
                    createStatus = UserCreateStatus.InvalidAnswer
                Case AspNetSecurity.MembershipCreateStatus.InvalidEmail
                    createStatus = UserCreateStatus.InvalidEmail
                Case AspNetSecurity.MembershipCreateStatus.InvalidPassword
                    createStatus = UserCreateStatus.InvalidPassword
                Case AspNetSecurity.MembershipCreateStatus.InvalidProviderUserKey
                    createStatus = UserCreateStatus.InvalidProviderUserKey
                Case AspNetSecurity.MembershipCreateStatus.InvalidQuestion
                    createStatus = UserCreateStatus.InvalidQuestion
                Case AspNetSecurity.MembershipCreateStatus.InvalidUserName
                    createStatus = UserCreateStatus.InvalidUserName
                Case AspNetSecurity.MembershipCreateStatus.ProviderError
                    createStatus = UserCreateStatus.ProviderError
                Case AspNetSecurity.MembershipCreateStatus.UserRejected
                    createStatus = UserCreateStatus.UserRejected
            End Select

            Return createStatus

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteMembershipUser deletes the User as an AspNet MembershipUser from the AspNet
        ''' Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to delete from the Data Store.</param>
        ''' <returns>A Boolean indicating success or failure.</returns>
        ''' <history>
        '''     [cnurse]	12/22/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function DeleteMembershipUser(ByVal user As UserInfo) As Boolean
            Dim retValue As Boolean = True
            Try
                AspNetSecurity.Membership.DeleteUser(user.Username, True)
            Catch ex As Exception
                retValue = False
            End Try
            Return retValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds a UserMembership object from an AspNet MembershipUser
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="aspNetUser">The MembershipUser object to use to fill the DNN UserMembership.</param>
        ''' <history>
        ''' 	[cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillUserMembership(ByVal aspNetUser As AspNetSecurity.MembershipUser, ByVal user As UserInfo)
            'Fill Membership Property
            If Not aspNetUser Is Nothing Then
                If user.Membership Is Nothing Then
                    user.Membership = New UserMembership(user)
                End If

                user.Membership.CreatedDate = aspNetUser.CreationDate
                user.Membership.LastActivityDate = aspNetUser.LastActivityDate
                user.Membership.LastLockoutDate = aspNetUser.LastLockoutDate
                user.Membership.LastLoginDate = aspNetUser.LastLoginDate
                user.Membership.LastPasswordChangeDate = aspNetUser.LastPasswordChangedDate
                user.Membership.LockedOut = aspNetUser.IsLockedOut
                user.Membership.PasswordQuestion = aspNetUser.PasswordQuestion
                If user.IsSuperUser Then
                    'For superusers the Approved info is stored in aspnet membership
                    user.Membership.Approved = aspNetUser.IsApproved
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an AspNet MembershipUser from the DataStore
        ''' </summary>
        ''' <param name="user">The user to get from the Data Store.</param>
        ''' <returns>The User as a AspNet MembershipUser object</returns>
        ''' <history>
        '''     [cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetMembershipUser(ByVal user As UserInfo) As AspNetSecurity.MembershipUser
            Return GetMembershipUser(user.Username)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an AspNet MembershipUser from the DataStore
        ''' </summary>
        ''' <param name="userName">The name of the user.</param>
        ''' <returns>The User as a AspNet MembershipUser object</returns>
        ''' <history>
        '''     [cnurse]	04/25/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetMembershipUser(ByVal userName As String) As AspNetSecurity.MembershipUser
            Return AspNetSecurity.Membership.GetUser(userName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetTotalRecords method gets the number of Records returned.
        ''' </summary>
        ''' <param name="dr">An <see cref="IDataReader"/> containing the Total no of records</param>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/30/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetTotalRecords(ByRef dr As IDataReader) As Integer

            Dim total As Integer = 0

            If dr.Read Then
                Try
                    total = Convert.ToInt32(dr("TotalRecords"))
                Catch ex As Exception
                    total = -1
                End Try
            End If

            Return total

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserByAuthToken retrieves a User from the DataStore using an Authentication Token
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="userToken">The authentication token of the user being retrieved from the Data Store.</param>
        ''' <param name="authType">The type of Authentication Used</param>
        ''' <returns>The User as a UserInfo object</returns>
        ''' <history>
        '''     [cnurse]	07/09/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetUserByAuthToken(ByVal portalId As Integer, ByVal userToken As String, ByVal authType As String) As UserInfo

            Dim dr As IDataReader = dataProvider.GetUserByAuthToken(portalId, userToken, authType)
            Dim objUserInfo As UserInfo = UserController.FillUserInfo(portalId, dr, True)

            Return objUserInfo

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateUserMembership persists a user's Membership to the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateUserMembership(ByVal user As UserInfo)

            Dim objSecurity As New PortalSecurity
            Dim email As String = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)

            'Persist the Membership Properties to the AspNet Data Store
            Dim objMembershipUser As AspNetSecurity.MembershipUser
            objMembershipUser = AspNetSecurity.Membership.GetUser(user.Username)
            objMembershipUser.Email = email
            objMembershipUser.LastActivityDate = Now
            If user.IsSuperUser Then
                'For superusers the Approved info is stored in aspnet membership
                objMembershipUser.IsApproved = user.Membership.Approved
            End If
            AspNetSecurity.Membership.UpdateUser(objMembershipUser)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Validates the users credentials against the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal the user belongs to</param>
        ''' <param name="username">The user name of the User attempting to log in</param>
        ''' <param name="password">The password of the User attempting to log in</param>
        ''' <returns>A Boolean result</returns>
        ''' <history>
        '''     [cnurse]	12/12/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ValidateUser(ByVal portalId As Integer, ByVal username As String, ByVal password As String) As Boolean
            Return AspNetSecurity.Membership.ValidateUser(username, password)
        End Function

#End Region

#Region "Membership Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Provider Properties can be edited
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property CanEditProviderProperties() As Boolean
            Get
                Return False
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the maximum number of invlaid attempts to login are allowed
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property MaxInvalidPasswordAttempts() As Integer
            Get
                Return AspNetSecurity.Membership.MaxInvalidPasswordAttempts
            End Get
            Set(ByVal Value As Integer)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Mimimum no of Non AlphNumeric characters required
        ''' </summary>
        ''' <returns>An Integer.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property MinNonAlphanumericCharacters() As Integer
            Get
                Return AspNetSecurity.Membership.MinRequiredNonAlphanumericCharacters
            End Get
            Set(ByVal Value As Integer)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Mimimum Password Length
        ''' </summary>
        ''' <returns>An Integer.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property MinPasswordLength() As Integer
            Get
                Return AspNetSecurity.Membership.MinRequiredPasswordLength
            End Get
            Set(ByVal Value As Integer)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the window in minutes that the maxium attempts are tracked for
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property PasswordAttemptWindow() As Integer
            Get
                Return AspNetSecurity.Membership.PasswordAttemptWindow
            End Get
            Set(ByVal Value As Integer)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Password Format as set in the web.config file
        ''' </summary>
        ''' <returns>A PasswordFormat enumeration.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property PasswordFormat() As PasswordFormat
            Get
                Select Case AspNetSecurity.Membership.Provider.PasswordFormat
                    Case Web.Security.MembershipPasswordFormat.Clear
                        Return PasswordFormat.Clear
                    Case Web.Security.MembershipPasswordFormat.Encrypted
                        Return PasswordFormat.Encrypted
                    Case Web.Security.MembershipPasswordFormat.Hashed
                        Return PasswordFormat.Hashed
                End Select
            End Get
            Set(ByVal Value As PasswordFormat)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Users's Password can be reset
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property PasswordResetEnabled() As Boolean
            Get
                Return AspNetSecurity.Membership.EnablePasswordReset
            End Get
            Set(ByVal Value As Boolean)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Users's Password can be retrieved
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	03/02/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property PasswordRetrievalEnabled() As Boolean
            Get
                Return AspNetSecurity.Membership.EnablePasswordRetrieval
            End Get
            Set(ByVal Value As Boolean)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether a Question/Answer is required for Password retrieval
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property RequiresQuestionAndAnswer() As Boolean
            Get
                Return AspNetSecurity.Membership.RequiresQuestionAndAnswer
            End Get
            Set(ByVal Value As Boolean)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a Regular Expression that deermines the strength of the password
        ''' </summary>
        ''' <returns>A String.</returns>
        ''' <history>
        '''     [cnurse]	02/07/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property PasswordStrengthRegularExpression() As String
            Get
                Return AspNetSecurity.Membership.PasswordStrengthRegularExpression
            End Get
            Set(ByVal Value As String)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether a Unique Email is required
        ''' </summary>
        ''' <returns>A Boolean.</returns>
        ''' <history>
        '''     [cnurse]	02/06/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property RequiresUniqueEmail() As Boolean
            Get
                Return AspNetSecurity.Membership.Provider.RequiresUniqueEmail
            End Get
            Set(ByVal Value As Boolean)
                Throw New NotSupportedException("Provider properties for AspNetMembershipProvider must be set in web.config")
            End Set
        End Property
#End Region

#Region "Membership Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ChangePassword attempts to change the users password
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to update.</param>
        ''' <param name="oldPassword">The old password.</param>
        ''' <param name="newPassword">The new password.</param>
        ''' <returns>A Boolean indicating success or failure.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function ChangePassword(ByVal user As UserInfo, ByVal oldPassword As String, ByVal newPassword As String) As Boolean

            Dim retValue As Boolean = False

            ' Get AspNet MembershipUser
            Dim aspnetUser As AspNetSecurity.MembershipUser = GetMembershipUser(user)

            If oldPassword = Null.NullString Then
                'Prevent exception if User is Locked Out - as this option is only calledby an admin user
                'this is safe to do
                aspnetUser.UnlockUser()
                oldPassword = aspnetUser.GetPassword()
            End If

            retValue = aspnetUser.ChangePassword(oldPassword, newPassword)

            If retValue And Me.PasswordRetrievalEnabled And Not Me.RequiresQuestionAndAnswer Then
                Dim confirmPassword As String = aspnetUser.GetPassword()

                If confirmPassword = newPassword Then
                    user.Membership.Password = confirmPassword
                    retValue = True
                Else
                    retValue = False
                End If
            End If

            Return retValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ChangePasswordQuestionAndAnswer attempts to change the users password Question
        ''' and PasswordAnswer
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to update.</param>
        ''' <param name="password">The password.</param>
        ''' <param name="passwordQuestion">The new password question.</param>
        ''' <param name="passwordAnswer">The new password answer.</param>
        ''' <returns>A Boolean indicating success or failure.</returns>
        ''' <history>
        '''     [cnurse]	02/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal user As UserInfo, ByVal password As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String) As Boolean

            Dim retValue As Boolean = False

            ' Get AspNet MembershipUser
            Dim aspnetUser As AspNetSecurity.MembershipUser = GetMembershipUser(user)

            If password = Null.NullString Then
                password = aspnetUser.GetPassword()
            End If

            retValue = aspnetUser.ChangePasswordQuestionAndAnswer(password, passwordQuestion, passwordAnswer)

            Return retValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateUser persists a User to the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <returns>A UserCreateStatus enumeration indicating success or reason for failure.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function CreateUser(ByRef user As UserInfo) As UserCreateStatus
            Dim createStatus As UserCreateStatus

            Try
                ' check if username exists in database for any portal
                Dim objVerifyUser As UserInfo = GetUserByUserName(Null.NullInteger, user.Username, False)
                If Not objVerifyUser Is Nothing Then
                    If objVerifyUser.IsSuperUser Then
                        ' the username belongs to an existing super user
                        createStatus = UserCreateStatus.UserAlreadyRegistered
                    Else
                        ' the username exists so we should now verify the password
                        If ValidateUser(objVerifyUser.PortalID, user.Username, user.Membership.Password) Then

                            ' check if user exists for the portal specified
                            objVerifyUser = GetUserByUserName(user.PortalID, user.Username, False)
                            If Not objVerifyUser Is Nothing Then
                                ' the user is already registered for this portal
                                createStatus = UserCreateStatus.UserAlreadyRegistered
                            Else
                                ' the user does not exist in this portal - add them
                                createStatus = UserCreateStatus.AddUserToPortal
                            End If
                        Else
                            ' not the same person - prevent registration
                            createStatus = UserCreateStatus.UsernameAlreadyExists
                        End If
                    End If
                Else
                    ' the user does not exist
                    createStatus = UserCreateStatus.AddUser
                End If

                'If new user - add to aspnet membership
                If createStatus = UserRegistrationStatus.AddUser Then
                    createStatus = CreateMemberhipUser(user)
                End If

                'If asp user has been successfully created or we are adding a existing user
                'to a new portal 
                If createStatus = UserCreateStatus.Success OrElse createStatus = UserCreateStatus.AddUserToPortal Then
                    'Create the DNN User Record
                    createStatus = CreateDNNUser(user)

                    If createStatus = UserCreateStatus.Success Then
                        'Persist the Profile to the Data Store
                        ProfileController.UpdateUserProfile(user)
                    End If
                End If

                If createStatus = UserCreateStatus.UserAlreadyRegistered Then
                    'If soft-deleted then undelete
                    If objVerifyUser.IsDeleted Then
                        objVerifyUser.IsDeleted = False
                        objVerifyUser.Membership.Approved = user.Membership.Approved
                        UpdateUser(objVerifyUser)
                        createStatus = UserCreateStatus.Success
                        user.UserID = objVerifyUser.UserID
                    End If
                End If

            Catch exc As Exception    ' an unexpected error occurred
                LogException(exc)
                createStatus = UserCreateStatus.UnexpectedError
            End Try

            Return createStatus

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteUser deletes a single User from the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to delete from the Data Store.</param>
        ''' <returns>A Boolean indicating success or failure.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function DeleteUser(ByVal user As UserInfo) As Boolean
            Dim retValue As Boolean = True
            Dim dr As IDataReader = Nothing

            Try
                dr = dataProvider.GetRolesByUser(user.UserID, user.PortalID)
                While dr.Read
                    dataProvider.DeleteUserRole(user.UserID, Convert.ToInt32(dr("RoleId")))
                End While

                dataProvider.DeleteUserPortal(user.UserID, user.PortalID)
            Catch ex As Exception
                LogException(ex)
                retValue = False
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            Return retValue
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes all UserOnline inof from the database that has activity outside of the
        ''' time window
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="TimeWindow">Time Window in Minutes</param>
        ''' <history>
        '''     [cnurse]	03/15/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DeleteUsersOnline(ByVal TimeWindow As Integer)
            dataProvider.DeleteUsersOnline(TimeWindow)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates a new random password (Length = Minimum Length + 4)
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        '''     [cnurse]	03/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function GeneratePassword() As String

            Return GeneratePassword(MinPasswordLength + 4)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Generates a new random password
        ''' </summary>
        ''' <param name="length">The length of password to generate.</param>
        ''' <returns>A String</returns>
        ''' <history>
        '''     [cnurse]	03/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function GeneratePassword(ByVal length As Integer) As String

            Return AspNetSecurity.Membership.GeneratePassword(length, MinNonAlphanumericCharacters)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a collection of Online Users
        ''' </summary>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <returns>An ArrayList of UserInfo objects</returns>
        ''' <history>
        '''     [cnurse]	03/15/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetOnlineUsers(ByVal PortalId As Integer) As ArrayList

            Dim totalRecords As Integer
            Return UserController.FillUserCollection(PortalId, dataProvider.GetOnlineUsers(PortalId), totalRecords)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Current Password Information for the User 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to delete from the Data Store.</param>
        ''' <param name="passwordAnswer">The answer to the Password Question, ues to confirm the user
        ''' has the right to obtain the password.</param>
        ''' <returns>A String</returns>
        ''' <history>
        '''     [cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetPassword(ByVal user As UserInfo, ByVal passwordAnswer As String) As String
            Dim retValue As String = ""
            Dim unLocked As Boolean = True

            ' Get AspNet MembershipUser
            Dim aspnetUser As AspNetSecurity.MembershipUser = GetMembershipUser(user)

            'Try and unlock user
            If aspnetUser.IsLockedOut Then
                unLocked = AutoUnlockUser(aspnetUser)
            End If

            If RequiresQuestionAndAnswer Then
                retValue = aspnetUser.GetPassword(passwordAnswer)
            Else
                retValue = aspnetUser.GetPassword()
            End If

            Return retValue

        End Function

        Public Overrides Function GetUnAuthorizedUsers(ByVal portalId As Integer) As ArrayList
            Return UserController.FillUserCollection(portalId, dataProvider.GetUnAuthorizedUsers(portalId))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserByUserName retrieves a User from the DataStore
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="userId">The id of the user being retrieved from the Data Store.</param>
        ''' <returns>The User as a UserInfo object</returns>
        ''' <history>
        '''     [cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUser(ByVal portalId As Integer, ByVal userId As Integer) As UserInfo
            Dim dr As IDataReader = dataProvider.GetUser(portalId, userId)
            Dim objUserInfo As UserInfo = UserController.FillUserInfo(portalId, dr, True)

            Return objUserInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserByUserName retrieves a User from the DataStore
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="username">The username of the user being retrieved from the Data Store.</param>
        ''' <returns>The User as a UserInfo object</returns>
        ''' <history>
        '''     [cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUserByUserName(ByVal portalId As Integer, ByVal username As String) As UserInfo
            Dim dr As IDataReader = dataProvider.GetUserByUsername(portalId, username)
            Dim objUserInfo As UserInfo = UserController.FillUserInfo(portalId, dr, True)

            Return objUserInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserCountByPortal gets the number of users in the portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <returns>The no of users</returns>
        ''' <history>
        '''     [cnurse]	05/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUserCountByPortal(ByVal portalId As Integer) As Integer

            Return dataProvider.GetUserCountByPortal(portalId)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUserMembership retrieves the UserMembership information from the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user whose Membership information we are retrieving.</param>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub GetUserMembership(ByRef user As UserInfo)
            Dim aspnetUser As AspNetSecurity.MembershipUser = Nothing

            'Get AspNet MembershipUser
            aspnetUser = GetMembershipUser(user)

            'Fill Membership Property
            FillUserMembership(aspnetUser, user)

            'Get Online Status
            user.Membership.IsOnLine = IsUserOnline(user)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUsers gets all the users of the portal
        ''' </summary>
        ''' <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="pageIndex">The page of records to return.</param>
        ''' <param name="pageSize">The size of the page</param>
        ''' <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        ''' <returns>An ArrayList of UserInfo objects.</returns>
        ''' <history>
        '''     [cnurse]	12/14/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Overrides Function GetUsers(ByVal portalId As Integer, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            If pageIndex = -1 Then
                pageIndex = 0
                pageSize = Integer.MaxValue
            End If

            Return UserController.FillUserCollection(portalId, dataProvider.GetAllUsers(portalId, pageIndex, pageSize), totalRecords)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUsersByEmail gets all the users of the portal whose email matches a provided
        ''' filter expression
        ''' </summary>
        ''' <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="emailToMatch">The email address to use to find a match.</param>
        ''' <param name="pageIndex">The page of records to return.</param>
        ''' <param name="pageSize">The size of the page</param>
        ''' <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        ''' <returns>An ArrayList of UserInfo objects.</returns>
        ''' <history>
        '''     [cnurse]	12/14/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUsersByEmail(ByVal portalId As Integer, ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            If pageIndex = -1 Then
                pageIndex = 0
                pageSize = Integer.MaxValue
            End If

            Return UserController.FillUserCollection(portalId, dataProvider.GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize), totalRecords)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUsersByUserName gets all the users of the portal whose username matches a provided
        ''' filter expression
        ''' </summary>
        ''' <remarks>If all records are required, (ie no paging) set pageSize = -1</remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="userNameToMatch">The username to use to find a match.</param>
        ''' <param name="pageIndex">The page of records to return.</param>
        ''' <param name="pageSize">The size of the page</param>
        ''' <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        ''' <returns>An ArrayList of UserInfo objects.</returns>
        ''' <history>
        '''     [cnurse]	12/14/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUsersByUserName(ByVal portalId As Integer, ByVal userNameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            If pageIndex = -1 Then
                pageIndex = 0
                pageSize = Integer.MaxValue
            End If

            Return UserController.FillUserCollection(portalId, dataProvider.GetUsersByUsername(portalId, userNameToMatch, pageIndex, pageSize), totalRecords)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUsersByProfileProperty gets all the users of the portal whose profile matches
        ''' the profile property pased as a parameter
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal</param>
        ''' <param name="propertyName">The name of the property being matched.</param>
        ''' <param name="propertyValue">The value of the property being matched.</param>
        ''' <param name="pageIndex">The page of records to return.</param>
        ''' <param name="pageSize">The size of the page</param>
        ''' <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        ''' <returns>An ArrayList of UserInfo objects.</returns>
        ''' <history>
        '''     [cnurse]	02/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function GetUsersByProfileProperty(ByVal portalId As Integer, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            If pageIndex = -1 Then
                pageIndex = 0
                pageSize = Integer.MaxValue
            End If

            Return UserController.FillUserCollection(portalId, dataProvider.GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize), totalRecords)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the user in question is online
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user.</param>
        ''' <returns>A Boolean indicating whether the user is online.</returns>
        ''' <history>
        '''     [cnurse]	03/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function IsUserOnline(ByVal user As UserInfo) As Boolean

            Dim isOnline As Boolean = False
            Dim objUsersOnline As New UserOnlineController

            If objUsersOnline.IsEnabled Then
                'First try the Cache
                Dim userList As Hashtable = objUsersOnline.GetUserList()
                Dim onlineUser As OnlineUserInfo = TryCast(userList(user.UserID.ToString()), OnlineUserInfo)

                If Not onlineUser Is Nothing Then
                    isOnline = True
                Else
                    'Next try the Database
                    onlineUser = CType(CBO.FillObject(dataProvider.GetOnlineUser(user.UserID), GetType(OnlineUserInfo)), OnlineUserInfo)
                    If Not onlineUser Is Nothing Then
                        isOnline = True
                    End If
                End If
            End If

            Return isOnline

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ResetPassword resets a user's password and returns the newly created password
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to update.</param>
        ''' <param name="passwordAnswer">The answer to the user's password Question.</param>
        ''' <returns>The new Password.</returns>
        ''' <history>
        '''     [cnurse]	02/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function ResetPassword(ByVal user As UserInfo, ByVal passwordAnswer As String) As String

            Dim retValue As String = ""

            ' Get AspNet MembershipUser
            Dim aspnetUser As AspNetSecurity.MembershipUser = GetMembershipUser(user)

            If RequiresQuestionAndAnswer Then
                retValue = aspnetUser.ResetPassword(passwordAnswer)
            Else
                retValue = aspnetUser.ResetPassword()
            End If

            Return retValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Unlocks the User's Account
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user whose account is being Unlocked.</param>
        ''' <returns>True if successful, False if unsuccessful.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function UnLockUser(ByVal user As UserInfo) As Boolean
            Dim objMembershipUser As AspNetSecurity.MembershipUser
            objMembershipUser = AspNetSecurity.Membership.GetUser(user.Username)
            Return objMembershipUser.UnlockUser()
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateUser persists a user to the Data Store
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="user">The user to persist to the Data Store.</param>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateUser(ByVal user As UserInfo)

            Dim objSecurity As New PortalSecurity
            Dim firstName As String = objSecurity.InputFilter(user.FirstName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim lastName As String = objSecurity.InputFilter(user.LastName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim email As String = objSecurity.InputFilter(user.Email, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim displayName As String = objSecurity.InputFilter(user.DisplayName, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            Dim updatePassword As Boolean = user.Membership.UpdatePassword
            Dim isApproved As Boolean = user.Membership.Approved

            If displayName = "" Then
                displayName = firstName + " " + lastName
            End If

            'Persist the Membership to the Data Store
            UpdateUserMembership(user)

            'Persist the DNN User to the Database
            dataProvider.UpdateUser(user.UserID, user.PortalID, firstName, lastName, email, displayName, updatePassword, isApproved, user.RefreshRoles, user.LastIPAddress, user.IsDeleted, UserController.GetCurrentUserInfo.UserID)

            'Persist the Profile to the Data Store
            ProfileController.UpdateUserProfile(user)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates UserOnline info
        ''' time window
        ''' </summary>
        ''' <param name="UserList">List of users to update</param>
        ''' <history>
        '''     [cnurse]	03/15/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateUsersOnline(ByVal UserList As Hashtable)

            dataProvider.UpdateUsersOnline(UserList)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserLogin attempts to log the user in, and returns the User if successful
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal the user belongs to</param>
        ''' <param name="username">The user name of the User attempting to log in</param>
        ''' <param name="password">The password of the User attempting to log in</param>
        ''' <param name="VerificationCode">The verification code of the User attempting to log in</param>
        ''' <param name="loginStatus">An enumerated value indicating the login status.</param>
        ''' <returns>The User as a UserInfo object</returns>
        ''' <history>
        '''     [cnurse]	12/10/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function UserLogin(ByVal portalId As Integer, ByVal username As String, ByVal password As String, ByVal verificationCode As String, ByRef loginStatus As UserLoginStatus) As UserInfo
            Return UserLogin(portalId, username, password, "DNN", verificationCode, loginStatus)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UserLogin attempts to log the user in, and returns the User if successful
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="portalId">The Id of the Portal the user belongs to</param>
        ''' <param name="username">The user name of the User attempting to log in</param>
        ''' <param name="password">The password of the User attempting to log in (may not be used by all Auth types)</param>
        ''' <param name="authType">The type of Authentication Used</param>
        ''' <param name="VerificationCode">The verification code of the User attempting to log in</param>
        ''' <param name="loginStatus">An enumerated value indicating the login status.</param>
        ''' <returns>The User as a UserInfo object</returns>
        ''' <history>
        '''     [cnurse]	07/09/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function UserLogin(ByVal portalId As Integer, ByVal username As String, ByVal password As String, ByVal authType As String, ByVal verificationCode As String, ByRef loginStatus As UserLoginStatus) As UserInfo
            'For now, we are going to ignore the possibility that the User may exist in the 
            'Global Data Store but not in the Local DataStore ie. A shared Global Data Store

            'Initialise Login Status to Failure
            loginStatus = UserLoginStatus.LOGIN_FAILURE

            'Get a light-weight (unhydrated) DNN User from the Database, we will hydrate it later if neccessary
            Dim user As UserInfo = Nothing

            If authType = "DNN" Then
                user = GetUserByUserName(portalId, username, False)
            Else
                user = GetUserByAuthToken(portalId, username, authType)
            End If

            If user IsNot Nothing AndAlso Not user.IsDeleted Then
                'Get AspNet MembershipUser
                Dim aspnetUser As AspNetSecurity.MembershipUser = Nothing
                aspnetUser = GetMembershipUser(user)

                'Fill Membership Property from AspNet MembershipUser
                FillUserMembership(aspnetUser, user)

                'Check if the User is Locked Out (and unlock if AutoUnlock has expired)
                If aspnetUser.IsLockedOut Then
                    If AutoUnlockUser(aspnetUser) Then
                        'Unlock User
                        user.Membership.LockedOut = False

                    Else
                        loginStatus = UserLoginStatus.LOGIN_USERLOCKEDOUT
                    End If

                End If

                'Check in a verified situation whether the user is Approved
                If user.Membership.Approved = False And user.IsSuperUser = False Then
                    'Check Verification code
                    If verificationCode = (portalId.ToString & "-" & user.UserID) Then
                        'Approve User
                        user.Membership.Approved = True

                        'Persist to Data Store
                        UpdateUser(user)
                    Else
                        loginStatus = UserLoginStatus.LOGIN_USERNOTAPPROVED
                    End If
                End If

                'Verify User Credentials
                Dim bValid As Boolean = False
                If loginStatus <> UserLoginStatus.LOGIN_USERLOCKEDOUT And loginStatus <> UserLoginStatus.LOGIN_USERNOTAPPROVED Then
                    If authType = "DNN" Then
                        If user.IsSuperUser Then
                            If ValidateUser(Null.NullInteger, username, password) Then
                                loginStatus = UserLoginStatus.LOGIN_SUPERUSER
                                bValid = True
                            End If
                        Else
                            If ValidateUser(portalId, username, password) Then
                                loginStatus = UserLoginStatus.LOGIN_SUCCESS
                                bValid = True
                            End If
                        End If
                    Else
                        If user.IsSuperUser Then
                            loginStatus = UserLoginStatus.LOGIN_SUPERUSER
                            bValid = True
                        Else
                            loginStatus = UserLoginStatus.LOGIN_SUCCESS
                            bValid = True
                        End If
                    End If
                End If

                If Not bValid Then
                    'Clear the user object
                    user = Nothing
                End If
            Else
                'Clear the user object
                user = Nothing
            End If

            Return user

        End Function

#End Region

#Region "Obsolete Methods"

        Public Overrides Function GetUnAuthorizedUsers(ByVal portalId As Integer, ByVal isHydrated As Boolean) As ArrayList
            Return GetUnAuthorizedUsers(portalId)
        End Function

        Public Overrides Function GetUser(ByVal portalId As Integer, ByVal userId As Integer, ByVal isHydrated As Boolean) As UserInfo
            Return GetUser(portalId, userId)
        End Function

        Public Overrides Function GetUserByUserName(ByVal portalId As Integer, ByVal username As String, ByVal isHydrated As Boolean) As UserInfo
            Return GetUserByUserName(portalId, username)
        End Function

        Public Overloads Overrides Function GetUsers(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            Return GetUsers(portalId, pageIndex, pageSize, totalRecords)
        End Function

        Public Overrides Function GetUsersByEmail(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            Return GetUsersByEmail(portalId, emailToMatch, pageIndex, pageSize, totalRecords)
        End Function

        Public Overrides Function GetUsersByUserName(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal userNameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            Return GetUsersByUserName(portalId, userNameToMatch, pageIndex, pageSize, totalRecords)
        End Function

        Public Overrides Function GetUsersByProfileProperty(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            Return GetUsersByProfileProperty(portalId, propertyName, propertyValue, pageIndex, pageSize, totalRecords)
        End Function

#End Region

    End Class

End Namespace
