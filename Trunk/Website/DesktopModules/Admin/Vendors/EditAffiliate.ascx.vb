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

Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Vendors

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditAffiliate PortalModuleBase is used to add/edit an Affiliate
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial  Class EditAffiliate

        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Controls"



#End Region

#Region "Private Members"

        Private VendorId As Integer = -1
        Private AffiliateId As Integer = -1

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Not (Request.QueryString("VendorId") Is Nothing) Then
                VendorId = Int32.Parse(Request.QueryString("VendorId"))
            End If

            If Not (Request.QueryString("AffilId") Is Nothing) Then
                AffiliateId = Int32.Parse(Request.QueryString("AffilId"))
            End If

            'this needs to execute always to the client script code is registred in InvokePopupCal
            cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
            cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)

            If Page.IsPostBack = False Then
                ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                Dim objAffiliates As New AffiliateController
                If Not AffiliateId = Null.NullInteger Then

                    ' Obtain a single row of banner information
                    Dim objAffiliate As AffiliateInfo = objAffiliates.GetAffiliate(AffiliateId, VendorId, PortalId)

                    If Not objAffiliate Is Nothing Then
                        If Not Null.IsNull(objAffiliate.StartDate) Then
                            txtStartDate.Text = objAffiliate.StartDate.ToShortDateString
                        End If
                        If Not Null.IsNull(objAffiliate.EndDate) Then
                            txtEndDate.Text = objAffiliate.EndDate.ToShortDateString
                        End If
                        txtCPC.Text = objAffiliate.CPC.ToString("#0.0####")
                        txtCPA.Text = objAffiliate.CPA.ToString("#0.0####")

                    Else       ' security violation attempt to access item not related to this Module
                        Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)
                    End If
                Else
                    txtCPC.Text = 0.ToString("#0.0####")
                    txtCPA.Text = 0.ToString("#0.0####")

                    cmdDelete.Visible = False
                End If

            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click

            ' Redirect back to the portal home page
            Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click

            If AffiliateId <> -1 Then
                Dim objAffiliates As New Services.Vendors.AffiliateController
                objAffiliates.DeleteAffiliate(AffiliateId)

                ' Redirect back to the portal home page
                Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdSend_Click runs when the Send Notification Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSend.Click

            Dim objVendors As New VendorController
            Dim objVendor As VendorInfo

            objVendor = objVendors.GetVendor(VendorId, PortalId)
            If Not objVendor Is Nothing Then
                If Not Null.IsNull(objVendor.Email) Then

                    Dim Custom As New ArrayList
                    Custom.Add(objVendor.VendorName)
                    Custom.Add(GetPortalDomainName(PortalSettings.PortalAlias.HTTPAlias, Request) & "/" & glbDefaultPage & "?AffiliateId=" & VendorId.ToString)

                    Dim errorMsg As String = Mail.SendMail(PortalSettings.Email, objVendor.Email, "", _
                        Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_SUBJECT"), _
                        Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_AFFILIATE_NOTIFICATION_BODY", Services.Localization.Localization.GlobalResourceFile, Custom), _
                        "", "", "", "", "", "")
                    Dim strMessage As String
                    If errorMsg = "" Then
                        'Success
                        strMessage = Services.Localization.Localization.GetString("NotificationSuccess", Me.LocalResourceFile)
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                    Else
                        'Failed
                        strMessage = Services.Localization.Localization.GetString("NotificationFailure", Me.LocalResourceFile)
                        strMessage = String.Format(strMessage, errorMsg)
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)

                    End If

                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click

            If Page.IsValid = True Then

                Dim objAffiliate As New AffiliateInfo

                objAffiliate.AffiliateId = AffiliateId
                objAffiliate.VendorId = VendorId
                If txtStartDate.Text <> "" Then
                    objAffiliate.StartDate = Date.Parse(txtStartDate.Text)
                Else
                    objAffiliate.StartDate = Null.NullDate
                End If
                If txtEndDate.Text <> "" Then
                    objAffiliate.EndDate = Date.Parse(txtEndDate.Text)
                Else
                    objAffiliate.EndDate = Null.NullDate
                End If
                objAffiliate.CPC = Double.Parse(txtCPC.Text)
                objAffiliate.CPA = Double.Parse(txtCPA.Text)

                Dim objAffiliates As New Services.Vendors.AffiliateController

                If AffiliateId = -1 Then
                    objAffiliates.AddAffiliate(objAffiliate)
                Else
                    objAffiliates.UpdateAffiliate(objAffiliate)
                End If

                ' Redirect back to the portal home page
                Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)

            End If

        End Sub

#End Region

#Region "Public Properties"
        Public Shadows ReadOnly Property PortalId() As Integer
            Get
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    Return -1
                Else
                    Return PortalSettings.PortalId
                End If
            End Get

        End Property
#End Region

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

    End Class

End Namespace