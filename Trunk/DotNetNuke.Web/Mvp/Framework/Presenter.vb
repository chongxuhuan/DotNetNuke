'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System.Web.UI

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Web.Validators

Namespace DotNetNuke.Web.Mvp.Framework

    Public Class Presenter(Of TView As IView, TPresenterModel As PresenterModel)

#Region "Private Members"

        Private _Environment As IPresenterEnvironment
        Private _Model As TPresenterModel
        Private _Validator As Validator
        Private _View As TView

#End Region

#Region "Protected Methods"

        Protected Friend Overridable ReadOnly Property AllowAnonymousAccess() As Boolean
            Get
                Return True
            End Get
        End Property

#End Region

#Region "Public Properties"

        Public Property Environment() As IPresenterEnvironment
            Get
                Return _Environment
            End Get
            Private Set(ByVal value As IPresenterEnvironment)
                _Environment = value
            End Set
        End Property

        Public Property Model() As TPresenterModel
            Get
                Return _Model
            End Get
            Private Set(ByVal value As TPresenterModel)
                _Model = value
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

        Public Property View() As TView
            Get
                Return _View
            End Get
            Private Set(ByVal value As TView)
                _View = value
            End Set
        End Property

#End Region

#Region "Protected/Friend Methods"

        Protected Friend Overridable Function CheckUserAuthorized() As Boolean
            Return Model.HasPermission
        End Function

        Protected Friend Sub LoadInternal()
            RunWithAuthPolicy(Function(p) p.Load())
        End Sub

        Protected Friend Overridable Sub OnNoCurrentUser()
            Environment.RedirectToLogin()
        End Sub

        Protected Friend Overridable Sub OnUnauthorizedUser()
            Environment.RedirectToAccessDenied()
        End Sub

        Public Overridable Sub RestoreState(ByVal stateBag As StateBag)
            AttributeBasedViewStateSerializer.DeSerialize(Me, stateBag)
        End Sub

        Protected Friend Overridable Function RunWithAuthPolicy(ByVal func As Func(Of Presenter(Of TView, TPresenterModel), Boolean)) As Boolean
            If (Model.UserId = Null.NullInteger AndAlso Not AllowAnonymousAccess) Then
                OnNoCurrentUser()
                Exit Function
            End If

            If (Not CheckUserAuthorized()) Then
                OnUnauthorizedUser()
            Else
                Return func.Invoke(Me)
            End If

            Return Nothing
        End Function

        Public Overridable Sub SaveState(ByVal stateBag As StateBag)
            AttributeBasedViewStateSerializer.Serialize(Me, stateBag)
        End Sub

#End Region

#Region "Public Methods"

        Public Sub Initialize(ByVal view As TView, ByVal presenterModel As TPresenterModel, ByVal environment As IPresenterEnvironment)
            Arg.NotNull("view", view)
            Arg.NotNull("presenterModel", presenterModel)
            Arg.NotNull("environment", environment)

            _Model = presenterModel
            _View = view
            _Environment = environment
            _Validator = New Validator(New DataAnnotationsObjectValidator())
        End Sub

        Public Overridable Function Load() As Boolean

        End Function

#End Region

    End Class

End Namespace
