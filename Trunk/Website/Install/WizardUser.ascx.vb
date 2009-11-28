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

Imports DotNetNuke.Security.Membership

Namespace DotNetNuke.Services.Install

    Partial Class WizardUser
        Inherits System.Web.UI.UserControl

#Region "Properties"

        Public Property FirstName() As String
            Get
                Return Me.txtFirstName.Text
            End Get
            Set(ByVal value As String)
                Me.txtFirstName.Text = value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return Me.txtLastName.Text
            End Get
            Set(ByVal value As String)
                Me.txtLastName.Text = value
            End Set
        End Property

        Public Property UserName() As String
            Get
                Return Me.txtUserName.Text
            End Get
            Set(ByVal value As String)
                Me.txtUserName.Text = value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return Me.txtPassword.Text
            End Get
            Set(ByVal value As String)
                Me.txtPassword.Text = value
            End Set
        End Property

        Public Property Confirm() As String
            Get
                Return Me.txtConfirm.Text
            End Get
            Set(ByVal value As String)
                Me.txtConfirm.Text = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return Me.txtEmail.Text
            End Get
            Set(ByVal value As String)
                Me.txtEmail.Text = value
            End Set
        End Property

        Public Property FirstNameLabel() As String
            Get
                Return Me.lblFirstName.Text
            End Get
            Set(ByVal value As String)
                Me.lblFirstName.Text = value
            End Set
        End Property

        Public Property LastNameLabel() As String
            Get
                Return Me.lblLastName.Text
            End Get
            Set(ByVal value As String)
                Me.lblLastName.Text = value
            End Set
        End Property

        Public Property UserNameLabel() As String
            Get
                Return Me.lblUserName.Text
            End Get
            Set(ByVal value As String)
                Me.lblUserName.Text = value
            End Set
        End Property

        Public Property PasswordLabel() As String
            Get
                Return Me.lblPassword.Text
            End Get
            Set(ByVal value As String)
                Me.lblPassword.Text = value
            End Set
        End Property

        Public Property ConfirmLabel() As String
            Get
                Return Me.lblConfirm.Text
            End Get
            Set(ByVal value As String)
                Me.lblConfirm.Text = value
            End Set
        End Property

        Public Property EmailLabel() As String
            Get
                Return Me.lblEmail.Text
            End Get
            Set(ByVal value As String)
                Me.lblEmail.Text = value
            End Set
        End Property

#End Region

        Public Function Validate() As String
            Dim strErrorMessage As String = Null.NullString

            If txtUserName.Text.Length < 4 Then
                strErrorMessage = "MinUserNamelength"
            ElseIf String.IsNullOrEmpty(txtPassword.Text) Then
                strErrorMessage = "NoPassword"
            ElseIf txtUserName.Text = txtPassword.Text Then
                strErrorMessage = "PasswordUser"
            ElseIf txtPassword.Text.Length < MembershipProviderConfig.MinPasswordLength Then
                strErrorMessage = "PasswordLength"
            ElseIf txtPassword.Text <> txtConfirm.Text Then
                strErrorMessage = "ConfirmPassword"
            ElseIf Not System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, glbEmailRegEx) Then
                strErrorMessage = "InValidEmail"
            End If

            Return strErrorMessage

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_PreRender runs just before the page is rendered
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'Make sure that the password is not cleared on pastback
            txtConfirm.Attributes("value") = txtConfirm.Text
            txtPassword.Attributes("value") = txtPassword.Text
        End Sub

    End Class

End Namespace

