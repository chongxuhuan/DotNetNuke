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

Imports System.Collections.Specialized
Imports System.Web

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.UI.Modules

Namespace DotNetNuke.Web.Mvp.Framework
    Public Class PresenterEnvironment
        Implements IPresenterEnvironment

#Region "Private Members"

        Private _ModuleContext As ModuleInstanceContext
        Private _HttpContext As HttpContextBase

#End Region

#Region "Constructors"

        Public Sub New(ByVal moduleContext As ModuleInstanceContext, ByVal httpContext As HttpContextBase)
            _ModuleContext = moduleContext
            _HttpContext = httpContext
        End Sub

#End Region

        Public Function PresenterUrl(ByVal destinationModel As PresenterModel) As String Implements IPresenterEnvironment.PresenterUrl
            'Get the information from the target presenter model
            Dim control As String = destinationModel.PresenterName
            Dim queryString As NameValueCollection = New NameValueCollection()
            destinationModel.BuildQueryString(queryString)

            'Convert the query string to a DotNetNuke supported format
            Dim additionalParameters As New List(Of String)

            For i As Integer = 0 To queryString.Count - 1
                Dim key As String = queryString.Keys(i)
                additionalParameters.Add(key)
                additionalParameters.Add(queryString(key))
            Next

            'Get the url
            Dim url As String

            If (String.IsNullOrEmpty(control)) Then
                url = NavigateURL(_ModuleContext.TabId, String.Empty, additionalParameters.ToArray())
            Else
                url = _ModuleContext.EditUrl(String.Empty, String.Empty, control, additionalParameters.ToArray())
            End If
            Return url
        End Function

        Public Sub RedirectToAccessDenied() Implements IPresenterEnvironment.RedirectToAccessDenied
            RedirectToUrl(AccessDeniedURL())
        End Sub

        Public Sub RedirectToLogin() Implements IPresenterEnvironment.RedirectToLogin
            RedirectToUrl(LoginURL(_HttpContext.Request.Url.ToString(), False))
        End Sub

        Public Sub RedirectToPresenter(ByVal destinationModel As PresenterModel) Implements IPresenterEnvironment.RedirectToPresenter
            'Redirect to Presenters Url
            RedirectToUrl(PresenterUrl(destinationModel))
        End Sub

        Public Sub RedirectToUrl(ByVal url As String) Implements IPresenterEnvironment.RedirectToUrl
            _HttpContext.Response.Redirect(url, True)
        End Sub

    End Class

End Namespace
