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
Namespace DotNetNuke.Services.UserProfile

    Public Class UserProfilePageHandler
        Implements IHttpHandler

        Private Shared Function GetUserId(ByVal username As String, ByVal PortalId As Integer) As Integer
            Dim _UserId As Integer = Null.NullInteger
            Dim userInfo As UserInfo = UserController.GetUserByName(PortalId, username)
            If userInfo IsNot Nothing Then
                _UserId = userInfo.UserID
            Else
                'The user cannot be found (potential DOS)
                Throw New HttpException(404, "Not Found")
            End If
            Return _UserId
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This handler handles requests for LinkClick.aspx, but only those specifc
        ''' to file serving
        ''' </summary>
        ''' <param name="context">System.Web.HttpContext)</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cpaterra]	4/19/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim UserId As Integer = Null.NullInteger
            Dim PortalId As Integer = _portalSettings.PortalId

            Try
                'try UserId
                If Not String.IsNullOrEmpty(context.Request.QueryString("UserId")) Then
                    UserId = Int32.Parse(context.Request.QueryString("UserId"))
                    If UserController.GetUserById(PortalId, UserId) Is Nothing Then
                        'The user cannot be found (potential DOS)
                        Throw New HttpException(404, "Not Found")
                    End If
                End If

                If UserId = Null.NullInteger Then
                    'try userName
                    If Not String.IsNullOrEmpty(context.Request.QueryString("UserName")) Then
                        UserId = GetUserId(context.Request.QueryString("UserName"), PortalId)
                    End If
                End If

                If UserId = Null.NullInteger Then
                    'try user
                    Dim user As String = context.Request.QueryString("User")
                    If Not String.IsNullOrEmpty(user) Then
                        If Not Int32.TryParse(user, UserId) Then
                            'User is not an integer, so try it as a name
                            UserId = GetUserId(user, PortalId)
                        Else
                            If UserController.GetUserById(PortalId, UserId) Is Nothing Then
                                'The user cannot be found (potential DOS)
                                Throw New HttpException(404, "Not Found")
                            End If
                        End If
                    End If
                End If

                If UserId = Null.NullInteger Then
                    'The user cannot be found (potential DOS)
                    Throw New HttpException(404, "Not Found")
                End If
            Catch ex As Exception
                'The user cannot be found (potential DOS)
                Throw New HttpException(404, "Not Found")
            End Try

            'Redirect to Userprofile Page
            context.Response.Redirect(UserProfileURL(UserId), True)

        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace