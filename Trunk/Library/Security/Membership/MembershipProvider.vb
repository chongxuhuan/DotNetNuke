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
Imports DotNetNuke
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Security.Membership

    Public MustInherit Class MembershipProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As MembershipProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of MembershipProvider)()
        End Function

#End Region

#Region "Abstract Properties"

        Public MustOverride ReadOnly Property CanEditProviderProperties() As Boolean
        Public MustOverride Property MaxInvalidPasswordAttempts() As Integer
        Public MustOverride Property MinPasswordLength() As Integer
        Public MustOverride Property MinNonAlphanumericCharacters() As Integer
        Public MustOverride Property PasswordAttemptWindow() As Integer
        Public MustOverride Property PasswordFormat() As PasswordFormat
        Public MustOverride Property PasswordResetEnabled() As Boolean
        Public MustOverride Property PasswordRetrievalEnabled() As Boolean
        Public MustOverride Property PasswordStrengthRegularExpression() As String
        Public MustOverride Property RequiresQuestionAndAnswer() As Boolean
        Public MustOverride Property RequiresUniqueEmail() As Boolean

#End Region

#Region "Abstract Methods"

        'Users
        Public MustOverride Function ChangePassword(ByVal user As UserInfo, ByVal oldPassword As String, ByVal newPassword As String) As Boolean
        Public MustOverride Function ChangePasswordQuestionAndAnswer(ByVal user As UserInfo, ByVal password As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String) As Boolean
        Public MustOverride Function CreateUser(ByRef user As UserInfo) As UserCreateStatus
        Public MustOverride Function DeleteUser(ByVal user As UserInfo) As Boolean
        Public MustOverride Function GeneratePassword() As String
        Public MustOverride Function GeneratePassword(ByVal length As Integer) As String
        Public MustOverride Function GetPassword(ByVal user As UserInfo, ByVal passwordAnswer As String) As String
        Public MustOverride Function GetUserCountByPortal(ByVal portalId As Integer) As Integer
        Public MustOverride Sub GetUserMembership(ByRef user As UserInfo)
        Public MustOverride Function ResetPassword(ByVal user As UserInfo, ByVal passwordAnswer As String) As String
        Public MustOverride Function UnLockUser(ByVal user As UserInfo) As Boolean
        Public MustOverride Sub UpdateUser(ByVal user As UserInfo)
        Public MustOverride Function UserLogin(ByVal portalId As Integer, ByVal username As String, ByVal password As String, ByVal verificationCode As String, ByRef loginStatus As UserLoginStatus) As UserInfo
        Public MustOverride Function UserLogin(ByVal portalId As Integer, ByVal username As String, ByVal password As String, ByVal authType As String, ByVal verificationCode As String, ByRef loginStatus As UserLoginStatus) As UserInfo

        'Users Online
        Public MustOverride Sub DeleteUsersOnline(ByVal TimeWindow As Integer)
        Public MustOverride Function GetOnlineUsers(ByVal PortalId As Integer) As ArrayList
        Public MustOverride Function IsUserOnline(ByVal user As UserInfo) As Boolean
        Public MustOverride Sub UpdateUsersOnline(ByVal UserList As Hashtable)

        'Legacy
        Public Overridable Sub TransferUsersToMembershipProvider()
            'Do Nothing
        End Sub

        Public MustOverride Function GetUser(ByVal portalId As Integer, ByVal userId As Integer) As UserInfo
        Public MustOverride Function GetUserByUserName(ByVal portalId As Integer, ByVal username As String) As UserInfo
        Public MustOverride Function GetUnAuthorizedUsers(ByVal portalId As Integer) As ArrayList
        Public MustOverride Function GetUsers(ByVal portalId As Integer, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        Public MustOverride Function GetUsersByEmail(ByVal portalId As Integer, ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        Public MustOverride Function GetUsersByUserName(ByVal portalId As Integer, ByVal userNameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        Public MustOverride Function GetUsersByProfileProperty(ByVal portalId As Integer, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList


        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUnAuthorizedUsers(ByVal portalId As Integer, ByVal isHydrated As Boolean) As ArrayList
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUser(ByVal portalId As Integer, ByVal userId As Integer, ByVal isHydrated As Boolean) As UserInfo
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUserByUserName(ByVal portalId As Integer, ByVal username As String, ByVal isHydrated As Boolean) As UserInfo
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUsers(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUsersByEmail(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUsersByUserName(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal userNameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
        <Obsolete("Deprecated in 5.1 as Ishydrated is no longer supported")> _
        Public MustOverride Function GetUsersByProfileProperty(ByVal portalId As Integer, ByVal isHydrated As Boolean, ByVal propertyName As String, ByVal propertyValue As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList

#End Region

    End Class

End Namespace

