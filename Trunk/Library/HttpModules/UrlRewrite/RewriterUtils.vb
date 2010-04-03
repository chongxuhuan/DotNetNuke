'
' DotNetNuke® - http://www.dotnetnuke.com
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

Imports System.Web

Namespace DotNetNuke.HttpModules

    Public Class RewriterUtils

        Friend Shared Sub RewriteUrl(ByVal context As HttpContext, ByVal sendToUrl As String)
            Dim x As String = ""
            Dim y As String = ""

            RewriteUrl(context, sendToUrl, x, y)
        End Sub

        Friend Shared Sub RewriteUrl(ByVal context As HttpContext, ByVal sendToUrl As String, ByRef sendToUrlLessQString As String, ByRef filePath As String)

            ' first strip the querystring, if any
            Dim queryString As String = String.Empty
            sendToUrlLessQString = sendToUrl

            If (sendToUrl.IndexOf("?") > 0) Then
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf("?"))
                queryString = sendToUrl.Substring(sendToUrl.IndexOf("?") + 1)
            End If

            ' grab the file's physical path
            filePath = String.Empty
            filePath = context.Server.MapPath(sendToUrlLessQString)

            ' rewrite the path..
            context.RewritePath(sendToUrlLessQString, String.Empty, queryString)

            ' NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
            ' If you are using .NET Framework 1.0, use the below form instead:
            ' context.RewritePath(sendToUrl);

        End Sub

        Friend Shared Function ResolveUrl(ByVal appPath As String, ByVal url As String) As String

            ' String is Empty, just return Url
            If (url.Length = 0) Then
                Return url
            End If

            ' String does not contain a ~, so just return Url
            If (url.StartsWith("~") = False) Then
                Return url
            End If

            ' There is just the ~ in the Url, return the appPath
            If (url.Length = 1) Then
                Return appPath
            End If

            Dim seperatorChar As Char = url.ToCharArray()(1)

            If (seperatorChar = "/" Or seperatorChar = "\") Then
                ' Url looks like ~/ or ~\
                If (appPath.Length > 1) Then
                    Return appPath + "/" & url.Substring(2)
                Else
                    Return "/" & url.Substring(2)
                End If
            Else
                ' Url look like ~something
                If (appPath.Length > 1) Then
                    Return appPath & "/" & url.Substring(1)
                Else
                    Return appPath & url.Substring(1)
                End If
            End If

        End Function

    End Class

End Namespace


