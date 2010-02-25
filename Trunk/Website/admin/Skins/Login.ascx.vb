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

Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.UI.Skins.Controls

    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	[smcculloch]10/15/2004	Fixed Logoff Link for FriendlyUrls
    ''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    '''                             brackets from property names
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Login
        Inherits UI.Skins.SkinObjectBase

#Region "Private Members"

        Private _text As String
        Private _cssClass As String
        Private _logoffText As String

        Private Const MyFileName As String = "Login.ascx"

#End Region

#Region "Public Members"
        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal Value As String)
                _text = Value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

        Public Property LogoffText() As String
            Get
                Return _logoffText
            End Get
            Set(ByVal Value As String)
                _logoffText = Value
            End Set
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' public attributes
                If CssClass <> "" Then
                    cmdLogin.CssClass = CssClass
                End If

                If Request.IsAuthenticated = True Then
                    If LogoffText <> "" Then
                        If LogoffText.IndexOf("src=") <> -1 Then
                            LogoffText = Replace(LogoffText, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                        End If
                        cmdLogin.Text = LogoffText
                    Else
                        cmdLogin.Text = Localization.GetString("Logout", Localization.GetResourceFile(Me, MyFileName))
                    End If
                Else
                    If Text <> "" Then
                        If Text.IndexOf("src=") <> -1 Then
                            Text = Replace(Text, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                        End If
                        cmdLogin.Text = Text
                    Else
                        cmdLogin.Text = Localization.GetString("Login", Localization.GetResourceFile(Me, MyFileName))
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdLogin_Click runs when the Login button is clicked
        ''' </summary>
        ''' <history>
        '''     [cnurse]    02/28/2008  DNN-6881 Logoff redirect
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLogin.Click
            Try
                If Request.IsAuthenticated = True Then
                    Response.Redirect(NavigateURL(PortalSettings.ActiveTab.TabID, "Logoff"), True)
                Else
                    Dim ReturnUrl As String = HttpContext.Current.Request.RawUrl
                    If ReturnUrl.IndexOf("?returnurl=") <> -1 Then
                        ReturnUrl = ReturnUrl.Substring(0, ReturnUrl.IndexOf("?returnurl="))
                    End If
                    ReturnUrl = HttpUtility.UrlEncode(ReturnUrl)

                    Response.Redirect(LoginURL(ReturnUrl, (Request.QueryString("override") IsNot Nothing)), True)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
