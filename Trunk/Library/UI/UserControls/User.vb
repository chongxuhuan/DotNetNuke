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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Lists

Namespace DotNetNuke.UI.UserControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The User UserControl is used to manage User Details
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	02/21/2005	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class User
        Inherits Framework.UserControlBase

#Region "Controls"

        Protected plFirstName As UI.UserControls.LabelControl
        Protected WithEvents txtFirstName As System.Web.UI.WebControls.TextBox
        Protected WithEvents valFirstName As System.Web.UI.WebControls.RequiredFieldValidator
        Protected plLastName As UI.UserControls.LabelControl
        Protected WithEvents txtLastName As System.Web.UI.WebControls.TextBox
        Protected WithEvents valLastName As System.Web.UI.WebControls.RequiredFieldValidator
        Protected plUserName As UI.UserControls.LabelControl
        Protected WithEvents txtUsername As System.Web.UI.WebControls.TextBox
        Protected WithEvents valUsername As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents lblUsernameAsterisk As System.Web.UI.WebControls.Label
        Protected WithEvents lblUsername As System.Web.UI.WebControls.Label

        Protected WithEvents PasswordRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plPassword As UI.UserControls.LabelControl
        Protected WithEvents txtPassword As System.Web.UI.WebControls.TextBox
        Protected WithEvents valPassword As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents ConfirmPasswordRow As System.Web.UI.HtmlControls.HtmlTableRow
        Protected plConfirm As UI.UserControls.LabelControl
        Protected WithEvents txtConfirm As System.Web.UI.WebControls.TextBox
        Protected WithEvents valConfirm1 As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents valConfirm2 As System.Web.UI.WebControls.CompareValidator
        Protected plEmail As UI.UserControls.LabelControl
        Protected WithEvents txtEmail As System.Web.UI.WebControls.TextBox
        Protected WithEvents valEmail1 As System.Web.UI.WebControls.RequiredFieldValidator
        Protected WithEvents valEmail2 As System.Web.UI.WebControls.RegularExpressionValidator
        Protected plWebsite As UI.UserControls.LabelControl
        Protected WithEvents txtWebsite As System.Web.UI.WebControls.TextBox
        Protected plIM As UI.UserControls.LabelControl
        Protected WithEvents txtIM As System.Web.UI.WebControls.TextBox


#End Region

#Region "Private Members"

        Private _ModuleId As Integer
        Private _ShowPassword As Boolean
        Private _LabelColumnWidth As String = ""
        Private _ControlColumnWidth As String = ""
        Private _StartTabIndex As Integer = 1
        Private _FirstName As String
        Private _LastName As String
        Private _UserName As String
        Private _Password As String
        Private _Confirm As String
        Private _Email As String
        Private _Website As String
        Private _IM As String

        Private MyFileName As String = "User.ascx"

#End Region

#Region "Properties"

        Public Property ModuleId() As Integer
            Get
                ModuleId = Convert.ToInt32(ViewState("ModuleId"))
            End Get
            Set(ByVal Value As Integer)
                _ModuleId = Value
            End Set
        End Property

        Public Property LabelColumnWidth() As String
            Get
                LabelColumnWidth = Convert.ToString(ViewState("LabelColumnWidth"))
            End Get
            Set(ByVal Value As String)
                _LabelColumnWidth = Value
            End Set
        End Property

        Public Property ControlColumnWidth() As String
            Get
                ControlColumnWidth = Convert.ToString(ViewState("ControlColumnWidth"))
            End Get
            Set(ByVal Value As String)
                _ControlColumnWidth = Value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                FirstName = txtFirstName.Text
            End Get
            Set(ByVal Value As String)
                _FirstName = Value
            End Set
        End Property

        Public Property LastName() As String
            Get
                LastName = txtLastName.Text
            End Get
            Set(ByVal Value As String)
                _LastName = Value
            End Set
        End Property

        Public Property UserName() As String
            Get
                UserName = txtUsername.Text
            End Get
            Set(ByVal Value As String)
                _UserName = Value
            End Set
        End Property

        Public Property Password() As String
            Get
                Password = txtPassword.Text
            End Get
            Set(ByVal Value As String)
                _Password = Value
            End Set
        End Property

        Public Property Confirm() As String
            Get
                Confirm = txtConfirm.Text
            End Get
            Set(ByVal Value As String)
                _Confirm = Value
            End Set
        End Property

        Public Property Email() As String
            Get
                Email = txtEmail.Text
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property

        Public Property Website() As String
            Get
                Website = txtWebsite.Text
            End Get
            Set(ByVal Value As String)
                _Website = Value
            End Set
        End Property

        Public Property IM() As String
            Get
                IM = txtIM.Text
            End Get
            Set(ByVal Value As String)
                _IM = Value
            End Set
        End Property

        Public WriteOnly Property StartTabIndex() As Integer
            Set(ByVal Value As Integer)
                _StartTabIndex = Value
            End Set
        End Property

        Public WriteOnly Property ShowPassword() As Boolean
            Set(ByVal Value As Boolean)
                _ShowPassword = Value
            End Set
        End Property

        Public ReadOnly Property LocalResourceFile() As String
            Get
                Return Services.Localization.Localization.GetResourceFile(Me, MyFileName)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/21/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then

                    txtFirstName.TabIndex = Convert.ToInt16(_StartTabIndex)
                    txtLastName.TabIndex = Convert.ToInt16(_StartTabIndex + 1)
                    txtUsername.TabIndex = Convert.ToInt16(_StartTabIndex + 2)
                    txtPassword.TabIndex = Convert.ToInt16(_StartTabIndex + 3)
                    txtConfirm.TabIndex = Convert.ToInt16(_StartTabIndex + 4)
                    txtEmail.TabIndex = Convert.ToInt16(_StartTabIndex + 5)
                    txtWebsite.TabIndex = Convert.ToInt16(_StartTabIndex + 6)
                    txtIM.TabIndex = Convert.ToInt16(_StartTabIndex + 7)

                    txtFirstName.Text = _FirstName
                    txtLastName.Text = _LastName
                    txtUsername.Text = _UserName
                    lblUsername.Text = _UserName
                    txtPassword.Text = _Password
                    txtConfirm.Text = _Confirm
                    txtEmail.Text = _Email
                    txtWebsite.Text = _Website
                    txtIM.Text = _IM

                    If _ControlColumnWidth <> "" Then
                        txtFirstName.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtLastName.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtUsername.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtPassword.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtConfirm.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtEmail.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtWebsite.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                        txtIM.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth)
                    End If

                    If Not _ShowPassword Then
                        valPassword.Enabled = False
                        valConfirm1.Enabled = False
                        valConfirm2.Enabled = False
                        PasswordRow.Visible = False
                        ConfirmPasswordRow.Visible = False
                        txtUsername.Visible = False
                        valUsername.Enabled = False
                        lblUsername.Visible = True
                        lblUsernameAsterisk.Visible = False
                    Else
                        txtUsername.Visible = True
                        valUsername.Enabled = True
                        lblUsername.Visible = False
                        lblUsernameAsterisk.Visible = True
                        valPassword.Enabled = True
                        valConfirm1.Enabled = True
                        valConfirm2.Enabled = True
                        PasswordRow.Visible = True
                        ConfirmPasswordRow.Visible = True
                    End If

                    ViewState("ModuleId") = Convert.ToString(_ModuleId)
                    ViewState("LabelColumnWidth") = _LabelColumnWidth
                    ViewState("ControlColumnWidth") = _ControlColumnWidth

                End If

                txtPassword.Attributes.Add("value", txtPassword.Text)
                txtConfirm.Attributes.Add("value", txtConfirm.Text)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
