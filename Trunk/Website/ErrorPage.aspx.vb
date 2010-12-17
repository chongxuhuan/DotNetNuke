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

Imports System.IO
Imports DotNetNuke.Security

Imports DotNetNuke.Services.Localization
Imports System.Threading
Imports System.Security.Cryptography

Namespace DotNetNuke.Services.Exceptions
	''' -----------------------------------------------------------------------------
	''' Project	 : DotNetNuke
	''' Class	 : ErrorPage
	''' 
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' Trapped errors are redirected to this universal error page, resulting in a 
	''' graceful display.
	''' </summary>
	''' <remarks>
	''' 'get the last server error
	''' 'process this error using the Exception Management Application Block
	''' 'add to a placeholder and place on page
	''' 'catch direct access - No exception was found...you shouldn't end up here unless you go to this aspx page URL directly
	''' </remarks>
	''' <history>
	''' 	[sun1]	1/19/2004	Created
	''' </history>
	''' -----------------------------------------------------------------------------
    Partial  Class ErrorPage
        Inherits System.Web.UI.Page

        Private Sub ManageError(ByVal status As String)
            Dim strErrorMessage As String = HttpUtility.HtmlEncode(Request.QueryString("error"))

            Dim strLocalizedMessage As String = Localization.Localization.GetString(status + ".Error", Localization.Localization.GlobalResourceFile)

            If Not IsNothing(strLocalizedMessage) Then
                strLocalizedMessage = strLocalizedMessage.Replace("src=""images/403-3.gif""", "src=""" & ResolveUrl("~/images/403-3.gif") & """")
                ErrorPlaceHolder.Controls.Add(New LiteralControl(String.Format(strLocalizedMessage, strErrorMessage)))
            End If
            If IsNumeric(status) Then
                Response.StatusCode = status
            End If
        End Sub

        <Obsolete("Function obsoleted in 5.6.1 as no longer used in core - version identification can be useful to potential hackers if used incorrectly")> _
        Public Function ExtractOSVersion() As String
            'default name to OSVersion in case OS not recognised
            Dim commonName As String = System.Environment.OSVersion.ToString()
            Select Case Environment.OSVersion.Version.Major
                Case 5
                    Select Case Environment.OSVersion.Version.Minor
                        Case 0
                            commonName = "Windows 2000"
                            Exit Select
                        Case 1
                            commonName = "Windows XP"
                            Exit Select
                        Case 2
                            commonName = "Windows Server 2003"
                            Exit Select
                    End Select
                    Exit Select
                Case 6
                    Select Case Environment.OSVersion.Version.Minor
                        Case 0
                            commonName = "Windows Vista"
                            Exit Select
                        Case 1
                            commonName = "Windows Server 2008"
                            Exit Select
                        Case 2
                            commonName = "Windows 7"
                            Exit Select
                    End Select
                    Exit Select
            End Select
            Return commonName
        End Function

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Me.StyleSheet.Attributes("href") = ResolveUrl("~/Install/Install.css")
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim strLocalizedMessage As String = Null.NullString
            Dim objSecurity As New PortalSecurity
            Dim status As String = objSecurity.InputFilter(Request.QueryString("status"), PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)

            If Not String.IsNullOrEmpty(status) Then
                ManageError(status)
            Else
                'get the last server error
                Dim exc As Exception = Server.GetLastError
                Try
                    If Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") Then
                        ErrorPlaceHolder.Controls.Add(New LiteralControl(HttpUtility.HtmlEncode(exc.ToString)))
                    Else
                        Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

                        Dim lex As New PageLoadException(exc.Message.ToString, exc)
                        'process this error using the Exception Management Application Block
                        LogException(lex)
                        'add to a placeholder and place on page
                        strLocalizedMessage = Localization.Localization.GetString("Error.Text", Localization.Localization.GlobalResourceFile)
                        ErrorPlaceHolder.Controls.Add(New ErrorContainer(_portalSettings, strLocalizedMessage, lex).Container)
                    End If
                Catch
                    'No exception was found...you shouldn't end up here
                    ' unless you go to this aspx page URL directly
                    strLocalizedMessage = Localization.Localization.GetString("UnhandledError.Text", Localization.Localization.GlobalResourceFile)
                    ErrorPlaceHolder.Controls.Add(New LiteralControl(strLocalizedMessage))
                End Try
            End If

            strLocalizedMessage = Localization.Localization.GetString("Return.Text", Localization.Localization.GlobalResourceFile)
            hypReturn.Text = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + strLocalizedMessage

        End Sub

    End Class

End Namespace
