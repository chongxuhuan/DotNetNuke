'
' DotNetNuke® - http:'www.dotnetnuke.com
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
Imports System.Collections.Generic
Imports System.Text
Imports System.Web
Imports DotNetNuke.Common.Utils

Imports DotNetNuke.Common

Namespace DotNetNuke.HttpModules.RequestFilter

    Public Class RequestFilterModule
        Implements IHttpModule

        Private Const INSTALLED_KEY As String = "httprequestfilter.attemptedinstall"

        ''' <summary>
        ''' Implementation of <see cref="IHttpModule"/>
        ''' </summary>
        ''' <remarks>
        ''' Currently empty.  Nothing to really do, as I have no member variables.
        ''' </remarks>
        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

        Public Sub Init(ByVal context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.BeginRequest, AddressOf FilterRequest
        End Sub

        Private Sub FilterRequest(ByVal sender As Object, ByVal e As EventArgs)

            Dim app As HttpApplication = CType(sender, HttpApplication)
            If (app Is Nothing) OrElse (app.Context Is Nothing) OrElse (app.Context.Items Is Nothing) Then
                Exit Sub
            End If

            Dim request As HttpRequest = app.Context.Request
            Dim response As HttpResponse = app.Context.Response

            If request.Url.LocalPath.ToLower.EndsWith("scriptresource.axd") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("webresource.axd") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("gif") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("jpg") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("css") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("js") Then
                Exit Sub
            End If

            'Carry out first time initialization tasks
            Initialize.Init(app)

            If request.Url.LocalPath.ToLower.EndsWith("install.aspx") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") _
                    OrElse request.Url.LocalPath.ToLower.EndsWith("captcha.aspx") Then
                Exit Sub
            End If

            ' only do this if we havn't already attempted an install.  This prevents PreSendRequestHeaders from
            ' trying to add this item way to late.  We only want the first run through to do anything.
            ' also, we use the context to store whether or not we've attempted an add, as it's thread-safe and
            ' scoped to the request.  An instance of this module can service multiple requests at the same time,
            ' so we cannot use a member variable.
            If Not app.Context.Items.Contains(INSTALLED_KEY) Then
                ' log the install attempt in the HttpContext
                ' must do this first as several IF statements
                ' below skip full processing of this method
                app.Context.Items.Add(INSTALLED_KEY, True)

                Dim settings As RequestFilterSettings = RequestFilterSettings.GetSettings()
                If (settings Is Nothing OrElse settings.Rules.Count = 0 OrElse Not settings.Enabled) Then
                    Exit Sub
                End If

                For Each rule As RequestFilterRule In settings.Rules

                    ' Added ability to determine the specific value types for addresses
                    ' this check was necessary so that your rule could deal with IPv4 or IPv6
                    ' To use this mode, add ":IPv4" or ":IPv6" to your servervariable name.
                    Dim varArray As String() = rule.ServerVariable.Split(":"c)
                    Dim varVal As String = request.ServerVariables(varArray(0))
                    If varArray(0).EndsWith("_ADDR", StringComparison.InvariantCultureIgnoreCase) And varArray.Length > 1 Then
                        Select Case varArray(1)
                            Case "IPv4"
                                varVal = NetworkUtils.GetAddress(varVal, AddressType.IPv4)
                            Case "IPv6"
                                varVal = NetworkUtils.GetAddress(varVal, AddressType.IPv4)
                        End Select
                    End If

                    If (Not String.IsNullOrEmpty(varVal)) Then
                        If (rule.Matches(varVal)) Then
                            rule.Execute()
                        End If
                    End If
                Next
            End If
        End Sub

    End Class

End Namespace
