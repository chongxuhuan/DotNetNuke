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

Imports System.Net
Imports System.IO
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Sales
    Partial Class Purchase
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase


        Private RoleID As Integer = -1

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
                Dim dblTotal As Double
                Dim strCurrency As String

                If Not (Request.QueryString("RoleID") Is Nothing) Then
                    RoleID = Int32.Parse(Request.QueryString("RoleID"))
                End If

                If Page.IsPostBack = False Then
                    Dim objRoles As New RoleController

                    If RoleID <> -1 Then
                        Dim objRole As RoleInfo = objRoles.GetRole(RoleID, PortalSettings.PortalId)

                        If Not objRole.RoleID = -1 Then
                            lblServiceName.Text = objRole.RoleName
                            If Not Null.IsNull(objRole.Description) Then
                                lblDescription.Text = objRole.Description
                            End If
                            If RoleID = PortalSettings.AdministratorRoleId Then
                                If Not Null.IsNull(PortalSettings.HostFee) Then
                                    lblFee.Text = Format(PortalSettings.HostFee, "#,##0.00")
                                End If
                            Else
                                If Not Null.IsNull(objRole.ServiceFee) Then
                                    lblFee.Text = Format(objRole.ServiceFee, "#,##0.00")
                                End If
                            End If
                            If Not Null.IsNull(objRole.BillingFrequency) Then
                                Dim ctlEntry As New Common.Lists.ListController
                                Dim entry As Common.Lists.ListEntryInfo = ctlEntry.GetListEntryInfo("Frequency", objRole.BillingFrequency)
                                lblFrequency.Text = entry.Text
                            End If
                            txtUnits.Text = "1"
                            If objRole.BillingFrequency = "1" Then       ' one-time fee
                                txtUnits.Enabled = False
                            End If
                        Else       ' security violation attempt to access item not related to this Module
                            Response.Redirect(NavigateURL(), True)
                        End If
                    End If

                    ' Store URL Referrer to return to portal
                    If Not Request.UrlReferrer Is Nothing Then
                        ViewState("UrlReferrer") = Convert.ToString(Request.UrlReferrer)
                    Else
                        ViewState("UrlReferrer") = ""
                    End If
                End If

                If RoleID = PortalSettings.AdministratorRoleId Then
                    strCurrency = Host.HostCurrency
                Else
                    strCurrency = PortalSettings.Currency
                End If

                dblTotal = Val(lblFee.Text) * Val(txtUnits.Text)
                lblTotal.Text = Format(dblTotal, "#,##0.00")

                lblFeeCurrency.Text = strCurrency
                lblTotalCurrency.Text = strCurrency

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub PurchaseBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPurchase.Click
            Try
                Dim strPaymentProcessor As String = ""
                Dim strProcessorUserId As String = ""
                Dim strProcessorPassword As String = ""

                If Page.IsValid Then

                    Dim objPortalController As New PortalController
                    Dim objPortalInfo As PortalInfo = objPortalController.GetPortal(PortalSettings.PortalId)
                    If Not objPortalInfo Is Nothing Then
                        strPaymentProcessor = objPortalInfo.PaymentProcessor
                        strProcessorUserId = objPortalInfo.ProcessorUserId
                        strProcessorPassword = objPortalInfo.ProcessorPassword
                    End If

                    If strPaymentProcessor = "PayPal" Then
                        ' build secure PayPal URL
                        Dim strPayPalURL As String = ""
                        strPayPalURL = "https://www.paypal.com/xclick/business=" & HTTPPOSTEncode(strProcessorUserId)
                        strPayPalURL = strPayPalURL & "&item_name=" & HTTPPOSTEncode(PortalSettings.PortalName & " - " & lblDescription.Text & " ( " & txtUnits.Text & " units @ " & lblFee.Text & " " & lblFeeCurrency.Text & " per " & lblFrequency.Text & " )")
                        strPayPalURL = strPayPalURL & "&item_number=" & HTTPPOSTEncode(CType(RoleID, String))
                        strPayPalURL = strPayPalURL & "&quantity=1"
                        strPayPalURL = strPayPalURL & "&custom=" & HTTPPOSTEncode(UserInfo.UserID.ToString)
                        strPayPalURL = strPayPalURL & "&amount=" & HTTPPOSTEncode(lblTotal.Text)
                        strPayPalURL = strPayPalURL & "&currency_code=" & HTTPPOSTEncode(lblTotalCurrency.Text)
                        strPayPalURL = strPayPalURL & "&return=" & HTTPPOSTEncode("http://" & GetDomainName(Request))
                        strPayPalURL = strPayPalURL & "&cancel_return=" & HTTPPOSTEncode("http://" & GetDomainName(Request))
                        strPayPalURL = strPayPalURL & "&notify_url=" & HTTPPOSTEncode("http://" & GetDomainName(Request) & "/admin/Sales/PayPalIPN.aspx")
                        strPayPalURL = strPayPalURL & "&undefined_quantity=&no_note=1&no_shipping=1"

                        ' redirect to PayPal
                        Response.Redirect(strPayPalURL, True)
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(Convert.ToString(ViewState("UrlReferrer")), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Function ConvertCurrency(ByVal Amount As String, ByVal FromCurrency As String, ByVal ToCurrency As String) As Double
            Dim strPost As String = "Amount=" & Amount & "&From=" & FromCurrency & "&To=" & ToCurrency
            Dim objStream As StreamWriter

            ConvertCurrency = 0

            Try
                Dim objRequest As HttpWebRequest = CType(WebRequest.Create("http://www.xe.com/ucc/convert.cgi"), HttpWebRequest)
                objRequest.Method = "POST"
                objRequest.ContentLength = strPost.Length
                objRequest.ContentType = "application/x-www-form-urlencoded"

                objStream = New StreamWriter(objRequest.GetRequestStream())
                objStream.Write(strPost)
                objStream.Close()

                Dim objResponse As HttpWebResponse = CType(objRequest.GetResponse(), HttpWebResponse)
                Dim sr As StreamReader
                sr = New StreamReader(objResponse.GetResponseStream())
                Dim strResponse As String = sr.ReadToEnd()
                sr.Close()

                Dim intPos1 As Integer = InStr(1, strResponse, ToCurrency & "</B>")
                Dim intPos2 As Integer = InStrRev(strResponse, "<B>", intPos1)

                ConvertCurrency = Val(Mid(strResponse, intPos2 + 3, (intPos1 - intPos2) - 4))

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Function

    End Class

End Namespace
