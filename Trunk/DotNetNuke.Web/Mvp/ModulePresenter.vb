'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports System.Web.UI
Imports WebFormsMvp
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Web.Validators
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Web.Mvp

    Public MustInherit Class ModulePresenter(Of TView As {Class, IModuleView(Of TModel)}, TModel As {New, Class})
        Inherits Presenter(Of TView)

#Region "Private Members"

        Private _IsEditable As Boolean
        Private _IsPostBack As Boolean
        Private _IsSuperUser As Boolean
        Private _LocalResourceFile As String
        Private _ModuleContext As ModuleInstanceContext
        Private _ModuleId As Integer
        Private _PortalId As Integer
        Private _TabId As Integer
        Private _UserId As Integer
        Private _Validator As Validator

#End Region

#Region "Constructors"

        Public Sub New(ByVal view As TView)
            MyBase.New(view)

            'Try and cast view to Control to get common control properties
            Dim control As Control = TryCast(view, Control)
            If control IsNot Nothing AndAlso control.Page IsNot Nothing Then
                _IsPostBack = control.Page.IsPostBack
            End If

            'Try and cast view to IModuleControl to get the Context
            Dim moduleControl As IModuleControl = TryCast(view, IModuleControl)
            If moduleControl IsNot Nothing Then
                _LocalResourceFile = moduleControl.LocalResourceFile
                _ModuleContext = moduleControl.ModuleContext
                LoadFromContext()
            End If
            _Validator = New Validator(New DataAnnotationsObjectValidator())

            AddHandler view.Initialize, AddressOf InitializeInternal
            AddHandler view.Load, AddressOf LoadInternal
        End Sub

#End Region

#Region "Protected Properties"

        Protected Friend Overridable ReadOnly Property AllowAnonymousAccess() As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Friend Overridable ReadOnly Property IsUserAuthorized() As Boolean
            Get
                Return True
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public Property IsEditable() As Boolean
            Get
                Return _IsEditable
            End Get
            Set(ByVal value As Boolean)
                _IsEditable = value
            End Set
        End Property

        Public Property IsPostBack() As Boolean
            Get
                Return _IsPostBack
            End Get
            Set(ByVal value As Boolean)
                _IsPostBack = value
            End Set
        End Property

        Public Property IsSuperUser() As Boolean
            Get
                Return _IsSuperUser
            End Get
            Set(ByVal value As Boolean)
                _IsSuperUser = value
            End Set
        End Property

        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

        Public Property ModuleContext() As ModuleInstanceContext
            Get
                Return _ModuleContext
            End Get
            Set(ByVal value As ModuleInstanceContext)
                _ModuleContext = value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Return _ModuleId
            End Get
            Set(ByVal value As Integer)
                _ModuleId = value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
            End Set
        End Property

        Public Property TabId() As Integer
            Get
                Return _TabId
            End Get
            Set(ByVal value As Integer)
                _TabId = value
            End Set
        End Property

        Public Property UserId() As Integer
            Get
                Return _UserId
            End Get
            Set(ByVal value As Integer)
                _UserId = value
            End Set
        End Property

        Public Property Validator() As Validator
            Get
                Return _Validator
            End Get
            Set(ByVal value As Validator)
                _Validator = Validator
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub InitializeInternal(ByVal sender As Object, ByVal e As EventArgs)
            OnInit()
        End Sub

        Private Sub LoadInternal(ByVal sender As Object, ByVal e As EventArgs)
            If CheckAuthPolicy() Then
                OnLoad()
            End If
        End Sub

#End Region

#Region "Protected Methods"

        Protected Friend Overridable Function CheckAuthPolicy() As Boolean
            If (UserId = Null.NullInteger AndAlso Not AllowAnonymousAccess) Then
                OnNoCurrentUser()
                Exit Function
            End If

            If (Not IsUserAuthorized) Then
                OnUnauthorizedUser()
                Exit Function
            End If

            Return True
        End Function

        Protected Overridable Sub LoadFromContext()
            If ModuleContext IsNot Nothing Then
                IsEditable = ModuleContext.IsEditable
                IsSuperUser = ModuleContext.PortalSettings.UserInfo.IsSuperUser
                ModuleId = ModuleContext.ModuleId
                PortalId = ModuleContext.PortalId
                TabId = ModuleContext.TabId
                UserId = ModuleContext.PortalSettings.UserInfo.UserID
            End If
        End Sub

        Protected Overridable Function LocalizeString(ByVal key As String) As String
            Dim localizedString As String
            If Not String.IsNullOrEmpty(key) AndAlso Not String.IsNullOrEmpty(LocalResourceFile) Then
                localizedString = Localization.GetString(key, LocalResourceFile)
            Else
                localizedString = Null.NullString
            End If
            Return localizedString
        End Function

        Protected Overridable Sub OnInit()
        End Sub

        Protected Overridable Sub OnLoad()
        End Sub

        Protected Overridable Sub OnNoCurrentUser()
            RedirectToLogin()
        End Sub

        Protected Overridable Sub OnUnauthorizedUser()
            RedirectToAccessDenied()
        End Sub

        Protected Sub RedirectToAccessDenied()
            Response.Redirect(AccessDeniedURL, True)
        End Sub

        Protected Sub RedirectToLogin()
            Response.Redirect(LoginURL(Request.RawUrl, False), True)
        End Sub

        Protected Sub ProcessModuleLoadException(ByVal ex As Exception)
            View.ProcessModuleLoadException(ex)
        End Sub

        Protected Sub ShowMessage(ByVal messageHeaderKey As String, ByVal messageKey As String, ByVal messageType As ModuleMessageType)
            If Not String.IsNullOrEmpty(messageKey) Then
                Dim header As String = Null.NullString
                If Not String.IsNullOrEmpty(messageHeaderKey) Then
                    header = LocalizeString(messageKey)
                End If
                View.ShowMessage(header, LocalizeString(messageKey), messageType)
            End If
        End Sub

        Protected Sub ShowMessage(ByVal messageKey As String, ByVal messageType As ModuleMessageType)
            If Not String.IsNullOrEmpty(messageKey) Then
                View.ShowMessage(Null.NullString, LocalizeString(messageKey), messageType)
            End If
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub ReleaseView()
        End Sub

        Public Overridable Sub RestoreState(ByVal stateBag As StateBag)
            AttributeBasedViewStateSerializer.DeSerialize(Me, stateBag)
        End Sub

        Public Overridable Sub SaveState(ByVal stateBag As StateBag)
            AttributeBasedViewStateSerializer.Serialize(Me, stateBag)
        End Sub

#End Region

    End Class

End Namespace

