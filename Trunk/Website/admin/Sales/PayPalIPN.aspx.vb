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
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Sales

    Partial Class PayPalIPN
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
                Dim strName As String
                Dim objStream As StreamWriter
                Dim blnValid As Boolean = True
                Dim strTransactionID As String
                Dim strTransactionType As String
                Dim intRoleID As Integer
                Dim intPortalID As Integer = PortalSettings.PortalId
                Dim intUserID As Integer
                Dim strDescription As String
                Dim dblAmount As Double
                Dim strEmail As String
                Dim blnCancel As Boolean = False
                Dim strPayPalID As String = Null.NullString

                Dim objRoles As New RoleController
                Dim objPortalController As New PortalController

                Dim strPost As String = "cmd=_notify-validate"
                For Each strName In Request.Form
                    Dim strValue As String = Request.Form(strName)
                    Select Case strName
                        Case "txn_type"       ' get the transaction type
                            strTransactionType = strValue
                            Select Case strTransactionType
                                Case "subscr_signup", "subscr_payment", "web_accept"
                                Case "subscr_cancel"
                                    blnCancel = True
                                Case Else
                                    blnValid = False
                            End Select
                        Case "payment_status"       ' verify the status
                            If strValue <> "Completed" Then
                                blnValid = False
                            End If
                        Case "txn_id"       ' verify the transaction id for duplicates
                            strTransactionID = strValue
                        Case "receiver_email"       ' verify the PayPalId
                            strPayPalID = strValue
                        Case "mc_gross"       ' verify the price
                            dblAmount = Double.Parse(strValue)
                        Case "item_number"       ' get the RoleID
                            intRoleID = Int32.Parse(strValue)
                            Dim objRole As RoleInfo = objRoles.GetRole(intRoleID, intPortalID)
                        Case "item_name"       ' get the product description
                            strDescription = strValue
                        Case "custom"       ' get the UserID
                            intUserID = Int32.Parse(strValue)
                        Case "email"       ' get the email
                            strEmail = strValue
                    End Select
                    ' reconstruct post for postback validation
                    strPost += String.Format("&{0}={1}", HTTPPOSTEncode(strName), HTTPPOSTEncode(strValue))
                Next
                ' postback to verify the source
                If blnValid Then
                    Dim objRequest As HttpWebRequest = CType(WebRequest.Create("https://www.paypal.com/cgi-bin/webscr"), HttpWebRequest)
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

                    Select Case strResponse
                        Case "VERIFIED"
                        Case Else
                            ' possible fraud
                            blnValid = False
                    End Select
                End If

                If blnValid Then
                    Dim intAdministratorRoleId As Integer
                    Dim strProcessorID As String = Null.NullString
                    Dim objPortalInfo As PortalInfo = objPortalController.GetPortal(intPortalID)
                    If Not objPortalInfo Is Nothing Then
                        intAdministratorRoleId = objPortalInfo.AdministratorRoleId
                        strProcessorID = objPortalInfo.ProcessorUserId.ToLower
                    End If
                    If intRoleID = intAdministratorRoleId Then
                        ' admin portal renewal
                        strProcessorID = Host.ProcessorUserId.ToLower
                        Dim portalPrice As Single = objPortalInfo.HostFee
                        If (portalPrice.ToString = dblAmount.ToString) And (HttpUtility.UrlDecode(strPayPalID.ToLower) = strProcessorID) Then
                            objPortalController.UpdatePortalExpiry(intPortalID)
                        Else
                            Try
                                Dim objEventLog As New Services.Log.EventLog.EventLogController
                                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                                objEventLogInfo.LogPortalID = intPortalID
                                objEventLogInfo.LogPortalName = PortalSettings.PortalName
                                objEventLogInfo.LogUserID = intUserID
                                objEventLogInfo.LogTypeKey = "POTENTIAL PAYPAL PAYMENT FRAUD"
                                objEventLog.AddLog(objEventLogInfo)
                            Catch ex As Exception

                            End Try
                        End If
                    Else
                        ' user subscription
                        Dim objRoleInfo As RoleInfo = objRoles.GetRole(intRoleID, intPortalID)
                        Dim rolePrice As Single = objRoleInfo.ServiceFee
                        Dim trialPrice As Single = objRoleInfo.TrialFee
                        If (rolePrice.ToString = dblAmount.ToString OrElse trialPrice.ToString = dblAmount.ToString) And (HttpUtility.UrlDecode(strPayPalID.ToLower) = strProcessorID) Then
                            objRoles.UpdateUserRole(intPortalID, intUserID, intRoleID, blnCancel)
                        Else
                            Try
                                Dim objEventLog As New Services.Log.EventLog.EventLogController
                                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                                objEventLogInfo.LogPortalID = intPortalID
                                objEventLogInfo.LogPortalName = PortalSettings.PortalName
                                objEventLogInfo.LogUserID = intUserID
                                objEventLogInfo.LogTypeKey = "POTENTIAL PAYPAL PAYMENT FRAUD"
                                objEventLog.AddLog(objEventLogInfo)
                            Catch ex As Exception

                            End Try
                        End If
                    End If
                End If
            Catch exc As Exception    'Page failed to load
                ProcessPageLoadException(exc)
            End Try
        End Sub

    End Class

End Namespace
