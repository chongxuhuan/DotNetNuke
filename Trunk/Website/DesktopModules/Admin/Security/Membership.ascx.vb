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
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Modules.Admin.Users
    ''' Class:      Membership
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Membership UserModuleBase is used to manage the membership aspects of a
    ''' User
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	03/01/2006  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Membership
        Inherits UserModuleBase

#Region "Events"

        Public Event MembershipAuthorized As EventHandler
        Public Event MembershipPasswordUpdateChanged As EventHandler
        Public Event MembershipUnAuthorized As EventHandler
        Public Event MembershipUnLocked As EventHandler

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UserMembership associated with this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Membership() As UserMembership
            Get
                Dim _Membership As UserMembership = Nothing
                If Not User Is Nothing Then
                    _Membership = User.Membership
                End If
                Return _Membership
            End Get
        End Property

#End Region

#Region "Event Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the MembershipAuthorized Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnMembershipAuthorized(ByVal e As EventArgs)
            RaiseEvent MembershipAuthorized(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the MembershipPasswordUpdateChanged Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/14/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnMembershipPasswordUpdateChanged(ByVal e As EventArgs)
            RaiseEvent MembershipPasswordUpdateChanged(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the MembershipUnAuthorized Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnMembershipUnAuthorized(ByVal e As EventArgs)
            RaiseEvent MembershipUnAuthorized(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the MembershipUnLocked Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnMembershipUnLocked(ByVal e As EventArgs)
            RaiseEvent MembershipUnLocked(Me, e)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            'disable/enable buttons
            If UserInfo.UserID = User.UserID Then
                cmdAuthorize.Visible = False
                cmdUnAuthorize.Visible = False
                cmdUnLock.Visible = False
                cmdPassword.Visible = False
            Else
                cmdUnLock.Visible = Membership.LockedOut
                cmdUnAuthorize.Visible = Membership.Approved
                cmdAuthorize.Visible = Not Membership.Approved
                cmdPassword.Visible = Not Membership.UpdatePassword
            End If

            MembershipEditor.DataSource = Membership
            MembershipEditor.DataBind()

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
                    MembershipEditor.LabelMode = LabelMode.Right
                Else
                    MembershipEditor.LabelMode = LabelMode.Left
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
            MembershipEditor.LocalResourceFile = Me.LocalResourceFile
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdAuthorize_Click runs when the Authorize User Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAuthorize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAuthorize.Click
            'Get the Membership Information from the property editors
            User.Membership = CType(MembershipEditor.DataSource, UserMembership)

            User.Membership.Approved = True

            'Update User
            UserController.UpdateUser(PortalId, User)

            OnMembershipAuthorized(EventArgs.Empty)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdPassword_Click runs when the ChangePassword Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/15/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdPassword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdPassword.Click
            'Get the Membership Information from the property editors
            User.Membership = CType(MembershipEditor.DataSource, UserMembership)

            User.Membership.UpdatePassword = True

            'Update User
            UserController.UpdateUser(PortalId, User)

            OnMembershipPasswordUpdateChanged(EventArgs.Empty)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUnAuthorize_Click runs when the UnAuthorize User Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUnAuthorize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUnAuthorize.Click

            'Get the Membership Information from the property editors
            User.Membership = CType(MembershipEditor.DataSource, UserMembership)

            User.Membership.Approved = False

            'Update User
            UserController.UpdateUser(PortalId, User)

            OnMembershipUnAuthorized(EventArgs.Empty)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUnlock_Click runs when the Unlock Account Button is clicked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUnLock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUnLock.Click
            ' update the user record in the database
            Dim isUnLocked As Boolean = UserController.UnLockUser(User)

            If isUnLocked Then
                User.Membership.LockedOut = False

                OnMembershipUnLocked(EventArgs.Empty)
            End If
        End Sub

#End Region

    End Class

End Namespace