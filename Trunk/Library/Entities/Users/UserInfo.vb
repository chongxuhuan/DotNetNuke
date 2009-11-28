'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Tokens

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      UserInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The UserInfo class provides Business Layer model for Users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	12/13/2005	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class UserInfo
        Inherits BaseEntityInfo
        Implements IPropertyAccess

#Region "Private Members"

        Private _UserID As Integer
        Private _Username As String
        Private _DisplayName As String
        Private _FullName As String
        Private _Email As String
        Private _PortalID As Integer
        Private _IsSuperUser As Boolean
        Private _AffiliateID As Integer
        Private _Membership As UserMembership
        Private _Profile As UserProfile
        Private _Roles As String()
        Private _RolesHydrated As Boolean = Null.NullBoolean
        Private _LastIPAddress As String
        Private _RefreshRoles As Boolean = Null.NullBoolean
        Private _IsDeleted As Boolean = Null.NullBoolean

#End Region

#Region "Constructors"

        Public Sub New()
            _UserID = Null.NullInteger
            _PortalID = Null.NullInteger
            _IsSuperUser = Null.NullBoolean
            _AffiliateID = Null.NullInteger
            _Roles = New String() {}
        End Sub

#End Region

#Region "Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the AffiliateId for this user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property AffiliateID() As Integer
            Get
                Return _AffiliateID
            End Get
            Set(ByVal Value As Integer)
                _AffiliateID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Display Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(3), Required(True), MaxLength(128)> _
        Public Property DisplayName() As String
            Get
                Return _DisplayName
            End Get
            Set(ByVal Value As String)
                _DisplayName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Email Address
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/27/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(4), MaxLength(256), Required(True), _
        RegularExpressionValidator(glbEmailRegEx)> _
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the First Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(1), MaxLength(50), Required(True)> _
        Public Property FirstName() As String
            Get
                Return Profile.FirstName
            End Get
            Set(ByVal Value As String)
                Profile.FirstName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the User is deleted
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property IsDeleted() As Boolean
            Get
                Return _IsDeleted
            End Get
            Set(ByVal Value As Boolean)
                _IsDeleted = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the User is a SuperUser
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property IsSuperUser() As Boolean
            Get
                Return _IsSuperUser
            End Get
            Set(ByVal Value As Boolean)
                _IsSuperUser = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last IP address used by user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/13/2009	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property LastIPAddress() As String
            Get
                Return _LastIPAddress
            End Get
            Set(ByVal Value As String)
                _LastIPAddress = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Last Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(2), MaxLength(50), Required(True)> _
        Public Property LastName() As String
            Get
                Return Profile.LastName
            End Get
            Set(ByVal Value As String)
                Profile.LastName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Membership object
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property Membership() As Entities.Users.UserMembership
            Get
                'implemented progressive hydration
                'this object will be hydrated on demand
                If _Membership Is Nothing Then
                    _Membership = New UserMembership(Me)
                    If (Not Me.Username Is Nothing) AndAlso (Me.Username.Length > 0) Then
                        UserController.GetUserMembership(Me)
                    End If
                End If
                Return _Membership
            End Get
            Set(ByVal Value As Entities.Users.UserMembership)
                _Membership = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PortalId
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Profile Object
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property Profile() As UserProfile
            Get
                'implemented progressive hydration
                'this object will be hydrated on demand
                If _Profile Is Nothing Then
                    _Profile = New UserProfile
                    ProfileController.GetUserProfile(Me)
                End If
                Return _Profile
            End Get
            Set(ByVal Value As UserProfile)
                _Profile = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the User's roles should be refreshed
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/18/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property RefreshRoles() As Boolean
            Get
                Return _RefreshRoles
            End Get
            Set(ByVal Value As Boolean)
                _RefreshRoles = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Roles for this User
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        '''     [sleupold]  08/14/2007  auto hydration of roles added
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property Roles() As String()
            Get
                If Not _RolesHydrated Then 'fill:
                    If _UserID > Null.NullInteger Then
                        Dim controller As DotNetNuke.Security.Roles.RoleController = New DotNetNuke.Security.Roles.RoleController
                        _Roles = controller.GetRolesByUser(_UserID, PortalID)
                        _RolesHydrated = True
                    End If
                End If
                Return _Roles
            End Get
            Set(ByVal Value As String())
                _Roles = Value
                _RolesHydrated = True
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User Id
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property UserID() As Integer
            Get
                Return _UserID
            End Get
            Set(ByVal Value As Integer)
                _UserID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the User Name
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/24/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <SortOrder(0), MaxLength(100), IsReadOnly(True), Required(True)> _
        Public Property Username() As String
            Get
                Return _Username
            End Get
            Set(ByVal Value As String)
                _Username = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IsInRole determines whether the user is in the role passed
        ''' </summary>
        ''' <param name="role">The role to check</param>
        ''' <returns>A Boolean indicating success or failure.</returns>
        ''' <history>
        '''     [cnurse]	12/13/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsInRole(ByVal role As String) As Boolean

            If IsSuperUser Or role = glbRoleAllUsersName Then
                Return True
            Else
                If "[" & UserID & "]" = role Then
                    Return True
                End If
                If Not Roles Is Nothing Then
                    For Each strRole As String In Roles
                        If strRole = role Then
                            Return True
                        End If
                    Next
                End If
            End If

            Return False

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateDisplayName updates the displayname to the format provided
        ''' </summary>
        ''' <param name="format">The format to use</param>
        ''' <history>
        '''     [cnurse]	02/21/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateDisplayName(ByVal format As String)

            'Replace TOKENS
            format = format.Replace("[USERID]", Me.UserID.ToString())
            format = format.Replace("[FIRSTNAME]", Me.FirstName)
            format = format.Replace("[LASTNAME]", Me.LastName)
            format = format.Replace("[USERNAME]", Me.Username)

            DisplayName = format

        End Sub

#End Region

#Region "Deprecated Members"

        <Browsable(False), Obsolete("Deprecated in DNN 5.1. This property has been deprecated in favour of Display Name")> _
        Public Property FullName() As String
            Get
                If _FullName = "" Then
                    'Build from component names
                    _FullName = FirstName & " " & LastName
                End If
                Return _FullName
            End Get
            Set(ByVal Value As String)
                _FullName = Value
            End Set
        End Property

#End Region

#Region "IPropertyAccess Implementation"

        Dim strAdministratorRoleName As String

        ''' <summary>
        ''' Determine, if accessing user is Administrator
        ''' </summary>
        ''' <param name="AccessingUser">userinfo of the user to query</param>
        ''' <returns>true, if user is portal administrator or superuser</returns>
        ''' <history>
        '''    2007-10-20 [sleupold] added
        ''' </history>
        Private Function isAdminUser(ByRef AccessingUser As UserInfo) As Boolean
            If AccessingUser Is Nothing OrElse AccessingUser.UserID = -1 Then
                Return False
            ElseIf strAdministratorRoleName = "" Then
                Dim ps As DotNetNuke.Entities.Portals.PortalInfo = New DotNetNuke.Entities.Portals.PortalController().GetPortal(AccessingUser.PortalID)
                strAdministratorRoleName = ps.AdministratorRoleName
            End If
            Return AccessingUser.IsInRole(strAdministratorRoleName) OrElse AccessingUser.IsSuperUser
        End Function

        ''' <summary>
        ''' Property access, initially provided for TokenReplace
        ''' </summary>
        ''' <param name="strPropertyName">Name of the Property</param>
        ''' <param name="strFormat">format string</param>
        ''' <param name="formatProvider">format provider for numbers, dates, currencies</param>
        ''' <param name="AccessingUser">userinfo of the user, who queries the data (used to determine permissions)</param>
        ''' <param name="CurrentScope">requested maximum access level, might be restricted due to user level</param>
        ''' <param name="PropertyNotFound">out: flag, if property could be retrieved.</param>
        ''' <returns>current value of the property for this userinfo object</returns>
        ''' <history>
        '''    2007-10-20   [sleupold]   documented and extended with differenciated access permissions
        '''    2007-10-20   [sleupold]   role access added (for user himself or admin only).
        ''' </history>
        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As UserInfo, ByVal CurrentScope As Services.Tokens.Scope, ByRef PropertyNotFound As Boolean) As String Implements Services.Tokens.IPropertyAccess.GetProperty

            'Limit permissions according to user type (admin, user itself, registered member or unauthenticated):
            Dim internScope As DotNetNuke.Services.Tokens.Scope
            If Me.UserID = -1 And CurrentScope > Scope.Configuration Then
                internScope = Scope.Configuration 'anonymous users only get access to displayname
            ElseIf Me.UserID <> AccessingUser.UserID AndAlso Not isAdminUser(AccessingUser) AndAlso CurrentScope > Scope.DefaultSettings Then
                internScope = Scope.DefaultSettings 'registerd users can access username and userID as well
            Else
                internScope = CurrentScope 'admins and user himself can access all data
            End If

            Dim OutputFormat As String = String.Empty
            If strFormat = String.Empty Then OutputFormat = "g" Else OutputFormat = strFormat

            Select Case strPropertyName.ToLower
                Case "verificationcode"
                    If internScope < Scope.SystemMessages Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return Me.PortalID.ToString & "-" & Me.UserID.ToString
                Case "affiliateid"
                    If internScope < Scope.SystemMessages Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (Me.AffiliateID.ToString(OutputFormat, formatProvider))
                Case "displayname"
                    If internScope < Scope.Configuration Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return PropertyAccess.FormatString(Me.DisplayName, strFormat)
                Case "email"
                    If internScope < Scope.DefaultSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(Me.Email, strFormat))
                Case "firstname" 'using profile property is recommended!
                    If internScope < Scope.DefaultSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(Me.FirstName, strFormat))
                Case "issuperuser"
                    If internScope < Scope.Debug Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (Me.IsSuperUser.ToString(formatProvider))
                Case "lastname" 'using profile property is recommended!
                    If internScope < Scope.DefaultSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(Me.LastName, strFormat))
                Case "portalid"
                    If internScope < Scope.Configuration Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (Me.PortalID.ToString(OutputFormat, formatProvider))
                Case "userid"
                    If internScope < Scope.DefaultSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (Me.UserID.ToString(OutputFormat, formatProvider))
                Case "username"
                    If internScope < Scope.DefaultSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(Me.Username, strFormat))
                Case "fullname" 'fullname is obsolete, it will return DisplayName
                    If internScope < Scope.Configuration Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(Me.DisplayName, strFormat))
                Case "roles"
                    If CurrentScope < Scope.SystemMessages Then PropertyNotFound = True : Return PropertyAccess.ContentLocked
                    Return (PropertyAccess.FormatString(String.Join(", ", Me.Roles), strFormat))
            End Select

            PropertyNotFound = True : Return String.Empty

        End Function

        <Browsable(False)> Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.notCacheable
            End Get
        End Property

#End Region

    End Class

End Namespace
