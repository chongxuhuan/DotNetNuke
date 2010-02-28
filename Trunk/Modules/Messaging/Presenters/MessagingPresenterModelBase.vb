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
Imports DotNetNuke.Web.Mvp.Framework

Namespace DotNetNuke.Modules.Messaging.Presenters

    Public MustInherit Class MessagingPresenterModelBase
        Inherits PresenterModel

#Region "Private Members"

        Private _DefaultModel As PresenterModel
        Private _IndexId As Integer = Null.NullInteger
        Private Const _IndexIdQueryString As String = "IndexId"

#End Region

#Region "Public Properties"

        Public Overrides ReadOnly Property DefaultPresenter() As PresenterModel
            Get
                If _DefaultModel Is Nothing Then
                    _DefaultModel = New MessagesListPresenterModel
                End If
                Return _DefaultModel
            End Get
        End Property

        Public Property IndexId() As Integer
            Get
                Return _IndexId
            End Get
            Set(ByVal value As Integer)
                _IndexId = value
            End Set
        End Property

#End Region

#Region "Virtual Method Overrides"

        Public Overrides Sub BuildQueryString(ByVal queryString As System.Collections.Specialized.NameValueCollection)
            MyBase.BuildQueryString(queryString)
            queryString.AddIfNotNull(_IndexIdQueryString, IndexId)
        End Sub

        Protected Overrides Sub LoadFromQueryString(ByVal parameters As System.Collections.Specialized.NameValueCollection)
            MyBase.LoadFromQueryString(parameters)
            IndexId = parameters.ToInt32(_IndexIdQueryString)
        End Sub

#End Region

    End Class

End Namespace

