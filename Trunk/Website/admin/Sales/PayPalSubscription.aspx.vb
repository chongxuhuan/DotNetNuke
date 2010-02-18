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

Imports System.Net
Imports System.IO
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Vendors
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.Sales

    Partial Class PayPalSubscription

        Inherits Framework.PageBase

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim objUserInfo As UserInfo = Nothing
                Dim intUserID As Integer = -1
                If Request.IsAuthenticated Then
                    objUserInfo = UserController.GetCurrentUserInfo
                    If Not objUserInfo Is Nothing Then
                        intUserID = objUserInfo.UserID
                    End If
                End If

                Dim intRoleId As Integer = -1
                If Not Request.QueryString("roleid") Is Nothing Then
                    intRoleId = Integer.Parse(Request.QueryString("roleid"))
                End If

                Dim strProcessorUserId As String = ""
                Dim objPortalController As New PortalController
                Dim objPortalInfo As PortalInfo = objPortalController.GetPortal(PortalSettings.PortalId)
                If Not objPortalInfo Is Nothing Then
                    strProcessorUserId = objPortalInfo.ProcessorUserId
                End If

                Dim settings As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(PortalSettings.PortalId)
                Dim strPayPalURL As String

                If intUserID <> -1 And intRoleId <> -1 And strProcessorUserId <> "" Then

                    ' Sandbox mode
                    If settings.ContainsKey("paypalsandbox") AndAlso Not String.IsNullOrEmpty(settings("paypalsandbox")) AndAlso settings("paypalsandbox") = "True" Then
                        strPayPalURL = "https://www.sandbox.paypal.com/cgi-bin/webscr?"
                    Else
                        strPayPalURL = "https://www.paypal.com/cgi-bin/webscr?"
                    End If

                    If Not Request.QueryString("cancel") Is Nothing Then
                        ' build the cancellation PayPal URL
                        strPayPalURL += "cmd=_subscr-find&alias=" & HTTPPOSTEncode(strProcessorUserId)
                    Else
                        strPayPalURL += "cmd=_ext-enter"

                        Dim objRoles As New RoleController

                        Dim objRole As RoleInfo = objRoles.GetRole(intRoleId, PortalSettings.PortalId)
                        If Not objRole.RoleID = -1 Then
                            Dim intTrialPeriod As Integer = 1
                            If objRole.TrialPeriod <> 0 Then
                                intTrialPeriod = objRole.TrialPeriod
                            End If
                            Dim intBillingPeriod As Integer = 1
                            If objRole.BillingPeriod <> 0 Then
                                intBillingPeriod = objRole.BillingPeriod
                            End If
                            ' explicitely format numbers using en-US so numbers are correctly built
                            Dim enFormat As System.Globalization.CultureInfo = New System.Globalization.CultureInfo("en-US")
                            Dim strService As String = String.Format(enFormat.NumberFormat, "{0:#####0.00}", objRole.ServiceFee)
                            Dim strTrial As String = String.Format(enFormat.NumberFormat, "{0:#####0.00}", objRole.TrialFee)

                            If objRole.BillingFrequency = "O" Or objRole.TrialFrequency = "O" Then 'one-time payment
                                ' build the payment PayPal URL
                                strPayPalURL += "&redirect_cmd=_xclick&business=" & HTTPPOSTEncode(strProcessorUserId)
                                strPayPalURL += "&item_name=" & HTTPPOSTEncode(PortalSettings.PortalName & " - " & objRole.RoleName & " ( " & Format(objRole.ServiceFee, "0.00") & " " & PortalSettings.Currency & " )")
                                strPayPalURL += "&item_number=" & HTTPPOSTEncode(intRoleId.ToString)
                                strPayPalURL += "&no_shipping=1&no_note=1"
                                strPayPalURL += "&quantity=1"
                                strPayPalURL += "&amount=" & HTTPPOSTEncode(strService)
                                strPayPalURL += "&currency_code=" & HTTPPOSTEncode(PortalSettings.Currency)
                            Else 'recurring payments
                                ' build the subscription PayPal URL
                                strPayPalURL += "&redirect_cmd=_xclick-subscriptions&business=" & HTTPPOSTEncode(strProcessorUserId)
                                strPayPalURL += "&item_name=" & HTTPPOSTEncode(PortalSettings.PortalName & " - " & objRole.RoleName & " ( " & Format(objRole.ServiceFee, "0.00") & " " & PortalSettings.Currency & " every " & intBillingPeriod.ToString & " " & GetBillingFrequencyCode(objRole.BillingFrequency) & " )")
                                strPayPalURL += "&item_number=" & HTTPPOSTEncode(intRoleId.ToString)
                                strPayPalURL += "&no_shipping=1&no_note=1"
                                If objRole.TrialFrequency <> "N" Then
                                    strPayPalURL += "&a1=" & HTTPPOSTEncode(strTrial)
                                    strPayPalURL += "&p1=" & HTTPPOSTEncode(intTrialPeriod.ToString)
                                    strPayPalURL += "&t1=" & HTTPPOSTEncode(objRole.TrialFrequency)
                                End If
                                strPayPalURL += "&a3=" & HTTPPOSTEncode(strService)
                                strPayPalURL += "&p3=" & HTTPPOSTEncode(intBillingPeriod.ToString)
                                strPayPalURL += "&t3=" & HTTPPOSTEncode(objRole.BillingFrequency)
                                strPayPalURL += "&src=1"
                                strPayPalURL += "&currency_code=" & HTTPPOSTEncode(PortalSettings.Currency)
                            End If
                        End If

                        Dim ctlList As New Common.Lists.ListController

                        strPayPalURL += "&custom=" & HTTPPOSTEncode(intUserID.ToString)
                        strPayPalURL += "&first_name=" & HTTPPOSTEncode(objUserInfo.Profile.FirstName)
                        strPayPalURL += "&last_name=" & HTTPPOSTEncode(objUserInfo.Profile.LastName)
                        Try
                            If objUserInfo.Profile.Country = "United States" Then
                                Dim colList As Common.Lists.ListEntryInfo = ctlList.GetListEntryInfo("Region", objUserInfo.Profile.Region)
                                strPayPalURL += "&address1=" & HTTPPOSTEncode(Convert.ToString(IIf(objUserInfo.Profile.Unit <> "", objUserInfo.Profile.Unit & " ", "")) & objUserInfo.Profile.Street)
                                strPayPalURL += "&city=" & HTTPPOSTEncode(objUserInfo.Profile.City)
                                strPayPalURL += "&state=" & HTTPPOSTEncode(colList.Value)
                                strPayPalURL += "&zip=" & HTTPPOSTEncode(objUserInfo.Profile.PostalCode)
                            End If
                        Catch
                            ' issue getting user address
                        End Try

                        ' Return URL
                        If settings.ContainsKey("paypalsubscriptionreturn") AndAlso Not String.IsNullOrEmpty(settings("paypalsubscriptionreturn")) Then
                            strPayPalURL += "&return=" & HTTPPOSTEncode(settings("paypalsubscriptionreturn"))
                        Else
                            strPayPalURL += "&return=" & HTTPPOSTEncode(AddHTTP(GetDomainName(Request)))
                        End If

                        ' Cancellation URL
                        If settings.ContainsKey("paypalsubscriptioncancelreturn") AndAlso Not String.IsNullOrEmpty(settings("paypalsubscriptioncancelreturn")) Then
                            strPayPalURL += "&cancel_return=" & HTTPPOSTEncode(settings("paypalsubscriptioncancelreturn"))
                        Else
                            strPayPalURL += "&cancel_return=" & HTTPPOSTEncode(AddHTTP(GetDomainName(Request)))
                        End If

                        ' Instant Payment Notification URL
                        If settings.ContainsKey("paypalsubscriptionnotifyurl") AndAlso Not String.IsNullOrEmpty(settings("paypalsubscriptionnotifyurl")) Then
                            strPayPalURL += "&notify_url=" & HTTPPOSTEncode(settings("paypalsubscriptionnotifyurl"))
                        Else
                            strPayPalURL += "&notify_url=" & HTTPPOSTEncode(AddHTTP(GetDomainName(Request)) & "/admin/Sales/PayPalIPN.aspx")
                        End If

                        strPayPalURL += "&sra=1"       ' reattempt on failure
                    End If

                    ' redirect to PayPal
                    Response.Redirect(strPayPalURL, True)
                Else
                    ' Cancellation URL
                    If settings.ContainsKey("paypalsubscriptioncancelreturn") AndAlso Not String.IsNullOrEmpty(settings("paypalsubscriptioncancelreturn")) Then
                        strPayPalURL = settings("paypalsubscriptioncancelreturn")
                    Else
                        strPayPalURL = AddHTTP(GetDomainName(Request))
                    End If

                    ' redirect to PayPal
                    Response.Redirect(strPayPalURL, True)
                End If

            Catch exc As Exception    'Page failed to load
                ProcessPageLoadException(exc)
            End Try
        End Sub
        Private Function GetBillingFrequencyCode(ByVal Value As String) As String

            Dim ctlEntry As New Common.Lists.ListController
            Dim entry As Common.Lists.ListEntryInfo = ctlEntry.GetListEntryInfo("Frequency", Value)
            Return entry.Value

        End Function


    End Class

End Namespace
