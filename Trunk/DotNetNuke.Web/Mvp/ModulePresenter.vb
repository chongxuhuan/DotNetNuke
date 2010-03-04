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

Imports System.Web.UI
Imports WebFormsMvp
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Web.Validators

Namespace DotNetNuke.Web.Mvp

    Public MustInherit Class ModulePresenter(Of TView As {Class, IView})
        Inherits Presenter(Of TView)

#Region "Private Members"

        Private _IsPostBack As Boolean
        Private _ModuleContext As ModuleInstanceContext
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
                _ModuleContext = moduleControl.ModuleContext
            End If
            _Validator = New Validator(New DataAnnotationsObjectValidator())

            'Try and cast view to IInitializeView to tie up Initialization Event
            Dim initializeView As IInitializeView = TryCast(view, IInitializeView)
            If initializeView IsNot Nothing Then
                AddHandler initializeView.Initialize, AddressOf InitializeInternal
            End If

        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsPostBack() As Boolean
            Get
                Return _IsPostBack
            End Get
        End Property

        Public ReadOnly Property IsSuperUser() As Boolean
            Get
                If ModuleContext IsNot Nothing Then
                    Return ModuleContext.PortalSettings.UserInfo.IsSuperUser
                End If
            End Get
        End Property

        Public ReadOnly Property ModuleContext() As ModuleInstanceContext
            Get
                Return _ModuleContext
            End Get
        End Property

        Public ReadOnly Property ModuleId() As Integer
            Get
                If ModuleContext IsNot Nothing Then
                    Return ModuleContext.ModuleId
                End If
            End Get
        End Property

        Public ReadOnly Property PortalId() As Integer
            Get
                If ModuleContext IsNot Nothing Then
                    Return ModuleContext.PortalSettings.PortalId
                End If
            End Get
        End Property

        Public ReadOnly Property TabId() As Integer
            Get
                If ModuleContext IsNot Nothing Then
                    Return ModuleContext.TabId
                End If
            End Get
        End Property

        Public ReadOnly Property UserId() As Integer
            Get
                If ModuleContext IsNot Nothing Then
                    Return ModuleContext.PortalSettings.UserId
                End If
            End Get
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
            Initialize()
        End Sub

#End Region

#Region "Protected Methods"

        Protected Friend Overridable ReadOnly Property AllowAnonymousAccess() As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Friend Overridable Function CheckUserAuthorized() As Boolean
            Return True
        End Function

        Protected Overridable Sub Initialize()

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

        'Protected Friend Overridable Sub OnNoCurrentUser()
        '    'Environment.RedirectToLogin()
        'End Sub

        'Protected Friend Overridable Sub OnUnauthorizedUser()
        '    'Environment.RedirectToAccessDenied()
        'End Sub

        'Protected Friend Overridable Function RunWithAuthPolicy(ByVal func As Func(Of Presenter, Boolean)) As Boolean
        '    'If (Model.UserId = Null.NullInteger AndAlso Not AllowAnonymousAccess) Then
        '    '    OnNoCurrentUser()
        '    '    Exit Function
        '    'End If

        '    'If (Not CheckUserAuthorized()) Then
        '    '    OnUnauthorizedUser()
        '    'Else
        '    '    Return func.Invoke(Me)
        '    'End If

        '    'Return Nothing
        'End Function

        'Private Function RunIfValid(ByVal func As Func(Of Boolean), ByVal validationGroup As String) As Boolean
        '    'If (String.IsNullOrEmpty(validationGroup)) Then
        '    '    Page.Validate()
        '    'Else
        '    '    Page.Validate(validationGroup)
        '    'End If
        '    'If (Page.IsValid) Then
        '    '    Return func.Invoke(Presenter)
        '    'Else
        '    '    Return Nothing
        '    'End If
        'End Function

        'Protected Overridable Function CreateSaveHandler(ByVal presenterFunction As Action(Of Object, Boolean)) As EventHandler
        '    Return CreateSaveHandler(presenterFunction, Nothing)
        'End Function

        'Protected Overridable Function CreateSaveHandler(ByVal presenterFunction As Func(Of Boolean), ByVal validationGroup As String) As EventHandler
        '    'Return Function(sender, args) Presenter.RunWithAuthPolicy(Function(func1) RunIfValid(Function(func2) presenterFunction(Presenter), validationGroup))
        'End Function

        'Protected Overridable Function CreateSimpleHandler(ByVal presenterFunction As Func(Of Boolean)) As EventHandler
        '    'Return Function(sender, args) presenterFunction(Presenter)
        'End Function

        'Protected Overridable Function CreateSimpleHandler(Of TEventArgs As EventArgs)(ByVal presenterFunction As Func(Of TPresenter, TEventArgs, Boolean)) As EventHandler(Of TEventArgs)
        '    'Return Function(sender, args) presenterFunction(Presenter, args)
        'End Function

    End Class

End Namespace

