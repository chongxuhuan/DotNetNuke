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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.UI.Skins.Controls
Imports System.Collections.Specialized
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Web.Mvp.Framework

    Public MustInherit Class PresenterModel

#Region "Private Members"

        Private _HasPermission As Boolean
        Private _IsPostBack As Boolean
        Private _IsSuperUser As Boolean
        Private _ModuleId As Integer
        Private _PortalId As Integer
        Private _TabId As Integer
        Private _UserId As Integer

#End Region

#Region "Public Properties"

        Public MustOverride ReadOnly Property DefaultPresenter() As PresenterModel

        Public Property HasPermission() As Boolean
            Get
                Return _HasPermission
            End Get
            Set(ByVal value As Boolean)
                _HasPermission = value
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

        Public MustOverride ReadOnly Property PresenterName() As String

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

#End Region

#Region "Protected Methods"

        Protected Overridable Sub LoadFromForm(ByVal parameters As NameValueCollection)
        End Sub

        Protected Overridable Sub LoadFromModuleContext(ByVal context As ModuleInstanceContext)
            Dim permissionKey As String = Null.NullString
            Dim access As Security.SecurityAccessLevel = context.Configuration.ModuleControl.ControlType
            If access = Security.SecurityAccessLevel.Edit Then
                permissionKey = "CONTENT"
            End If
            HasPermission = ModulePermissionController.HasModuleAccess(access, permissionKey, context.Configuration)
            ModuleId = context.ModuleId
            PortalId = context.PortalId
            TabId = context.TabId
            UserId = context.PortalSettings.UserId
            IsSuperUser = (context.PortalSettings.UserInfo.IsSuperUser)
        End Sub

        Protected Overridable Sub LoadFromQueryString(ByVal parameters As NameValueCollection)
        End Sub

        Protected Overridable Sub LoadFromRequestParameters(ByVal parameters As NameValueCollection)
        End Sub

#End Region

#Region "Public Methods"

        Public Overridable Sub BuildQueryString(ByVal queryString As NameValueCollection)
        End Sub

        Public Overridable Sub LoadFromView(Of TView As IView, _
                                                TPresenter As {Presenter(Of TView, TPresenterModel), New}, _
                                                TPresenterModel As {PresenterModel, New}) _
                                                (ByVal view As ViewBase(Of TView, TPresenter, TPresenterModel))

            LoadFromModuleContext(view.ModuleContext)
            LoadFromRequestParameters(view.Context.Request.Params)
            LoadFromQueryString(view.Context.Request.QueryString)
            LoadFromForm(view.Context.Request.Form)

            IsPostBack = view.IsPostBack
        End Sub

#End Region

    End Class

End Namespace

