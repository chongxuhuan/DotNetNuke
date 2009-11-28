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

Imports System
Imports System.Web

Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.HttpModules.Exceptions

    Public Class ExceptionModule

        Implements IHttpModule

        Public ReadOnly Property ModuleName() As String
            Get
                Return "ExceptionModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init

            AddHandler application.Error, AddressOf Me.OnErrorRequest

        End Sub

        Public Sub OnErrorRequest(ByVal s As Object, ByVal e As EventArgs)

            Try
                Dim Context As HttpContext = HttpContext.Current
                Dim Server As HttpServerUtility = Context.Server
                Dim Request As HttpRequest = Context.Request

                'exit if a request for a .net mapping that isn't a content page is made i.e. axd
                If Request.Url.LocalPath.ToLower.EndsWith(".aspx") = False _
                        AndAlso Request.Url.LocalPath.ToLower.EndsWith(".asmx") = False _
                        AndAlso Request.Url.LocalPath.ToLower.EndsWith(".ashx") = False Then
                    Exit Sub
                End If

                Dim lastException As Exception = Server.GetLastError()

                'HttpExceptions are logged elsewhere
                If Not (TypeOf lastException Is HttpException) Then
                    Dim lex As New Exception("Unhandled Error: ", Server.GetLastError)
                    Dim objExceptionLog As New Services.Log.EventLog.ExceptionLogController
                    Try
                        objExceptionLog.AddLog(lex)
                    Catch
                    End Try
                End If
            Catch ex As Exception
                ' it is possible when terminating the request for the context not to exist
                ' in this case we just want to exit since there is nothing else we can do
            End Try

        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

    End Class

End Namespace
