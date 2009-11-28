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

Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.UI.UserControls
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Vendors

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditBanner PortalModuleBase is used to add/edit a Banner
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/21/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditBanner
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private VendorId As Integer = Null.NullInteger
        Protected WithEvents lblBannerGroup As System.Web.UI.WebControls.Label
        Private BannerId As Integer = Null.NullInteger

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
            Try
                If Not (Request.QueryString("VendorId") Is Nothing) Then
                    VendorId = Int32.Parse(Request.QueryString("VendorId"))
                End If
                If Not (Request.QueryString("BannerId") Is Nothing) Then
                    BannerId = Int32.Parse(Request.QueryString("BannerId"))
                End If

                'this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
                cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)

                If Page.IsPostBack = False Then

                    ctlImage.FileFilter = glbImageFileTypes
                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    Dim objBannerTypes As New Services.Vendors.BannerTypeController
                    ' Get the banner types from the database
                    cboBannerType.DataSource = objBannerTypes.GetBannerTypes()
                    cboBannerType.DataBind()

                    Dim objBanners As New BannerController
                    If Not BannerId = Null.NullInteger Then

                        ' Obtain a single row of banner information
                        Dim objBanner As BannerInfo = objBanners.GetBanner(BannerId, VendorId, PortalId)

                        If Not objBanner Is Nothing Then
                            txtBannerName.Text = objBanner.BannerName
                            cboBannerType.Items.FindByValue(objBanner.BannerTypeId.ToString).Selected = True
                            DNNTxtBannerGroup.Text = objBanner.GroupName
                            ctlImage.Url = objBanner.ImageFile
                            If objBanner.Width <> 0 Then
                                txtWidth.Text = objBanner.Width.ToString
                            End If
                            If objBanner.Height <> 0 Then
                                txtHeight.Text = objBanner.Height.ToString
                            End If
                            txtDescription.Text = objBanner.Description
                            If Not IsDBNull(objBanner.URL) Then
                                ctlURL.Url = objBanner.URL
                            End If
                            txtImpressions.Text = objBanner.Impressions.ToString
                            txtCPM.Text = objBanner.CPM.ToString
                            If Not Null.IsNull(objBanner.StartDate) Then
                                txtStartDate.Text = objBanner.StartDate.ToShortDateString
                            End If
                            If Not Null.IsNull(objBanner.EndDate) Then
                                txtEndDate.Text = objBanner.EndDate.ToShortDateString
                            End If
                            optCriteria.Items.FindByValue(objBanner.Criteria.ToString).Selected = True

                            ctlAudit.CreatedByUser = objBanner.CreatedByUser
                            ctlAudit.CreatedDate = objBanner.CreatedDate.ToString

                            Dim arrBanners As New ArrayList
                            arrBanners.Add(objBanner)
                            lstBanners.DataSource = arrBanners
                            lstBanners.DataBind()
                        Else       ' security violation attempt to access item not related to this Module
                            Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)
                        End If
                    Else
                        txtImpressions.Text = "0"
                        txtCPM.Text = "0"
                        optCriteria.Items.FindByValue("1").Selected = True

                        cmdDelete.Visible = False
                        cmdCopy.Visible = False
                        cmdEmail.Visible = False
                        ctlAudit.Visible = False
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

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
            Try
                ' Redirect back to the portal home page
                Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
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
            Try
                If BannerId <> -1 Then
                    Dim objBanner As New Services.Vendors.BannerController
                    objBanner.DeleteBanner(BannerId)

                    ' Redirect back to the portal home page
                    Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
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
            Try
                ' Only Update if the Entered Data is val
                If Page.IsValid = True Then

                    If Not cmdCopy.Visible Then
                        BannerId = -1
                    End If
                    Dim StartDate As Date = Null.NullDate
                    If txtStartDate.Text <> "" Then
                        StartDate = Convert.ToDateTime(txtStartDate.Text)
                    End If

                    Dim EndDate As Date = Null.NullDate
                    If txtEndDate.Text <> "" Then
                        EndDate = Convert.ToDateTime(txtEndDate.Text)
                    End If

                    ' Create an instance of the Banner DB component
                    Dim objBanner As New BannerInfo
                    objBanner.BannerId = BannerId
                    objBanner.VendorId = VendorId
                    objBanner.BannerName = txtBannerName.Text
                    objBanner.BannerTypeId = Convert.ToInt32(cboBannerType.SelectedItem.Value)
                    objBanner.GroupName = DNNTxtBannerGroup.Text
                    objBanner.ImageFile = ctlImage.Url
                    If txtWidth.Text <> "" Then
                        objBanner.Width = Integer.Parse(txtWidth.Text)
                    Else
                        objBanner.Width = 0
                    End If
                    If txtHeight.Text <> "" Then
                        objBanner.Height = Integer.Parse(txtHeight.Text)
                    Else
                        objBanner.Height = 0
                    End If
                    objBanner.Description = txtDescription.Text
                    objBanner.URL = ctlURL.Url
                    objBanner.Impressions = Integer.Parse(txtImpressions.Text)
                    objBanner.CPM = Double.Parse(txtCPM.Text)
                    objBanner.StartDate = StartDate
                    objBanner.EndDate = EndDate
                    objBanner.Criteria = Integer.Parse(optCriteria.SelectedItem.Value)
                    objBanner.CreatedByUser = UserInfo.UserID.ToString

                    Dim objBanners As New Services.Vendors.BannerController
                    If BannerId = Null.NullInteger Then
                        ' Add the banner within the Banners table
                        objBanners.AddBanner(objBanner)
                    Else
                        ' Update the banner within the Banners table
                        objBanners.UpdateBanner(objBanner)
                    End If

                    ' Redirect back to the portal home page
                    Response.Redirect(EditUrl("VendorId", VendorId.ToString), True)

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCopy.Click
            Try
                txtStartDate.Text = ""
                txtEndDate.Text = ""
                cmdDelete.Visible = False
                cmdCopy.Visible = False
                cmdEmail.Visible = False
                ctlAudit.Visible = False
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEmail.Click

            ' send email summary to vendor
            Dim objBanners As New BannerController
            Dim objBanner As BannerInfo = objBanners.GetBanner(BannerId, VendorId, PortalId)
            If Not objBanner Is Nothing Then
                Dim objVendors As New VendorController
                Dim objVendor As VendorInfo = objVendors.GetVendor(objBanner.VendorId, PortalId)
                If Not objVendor Is Nothing Then
                    If Not Null.IsNull(objVendor.Email) Then

                        Dim Custom As New ArrayList
                        Custom.Add(objBanner.BannerName)
                        Custom.Add(objBanner.Description)
                        Custom.Add(objBanner.ImageFile)
                        Custom.Add(objBanner.CPM.ToString("#0.#####"))
                        Custom.Add(objBanner.Impressions.ToString)
                        Custom.Add(objBanner.StartDate.ToShortDateString)
                        Custom.Add(objBanner.EndDate.ToShortDateString)
                        Custom.Add(objBanner.Views.ToString)
                        Custom.Add(objBanner.ClickThroughs.ToString)

                        Dim errorMsg As String = Mail.SendMail(PortalSettings.Email, objVendor.Email, "", _
                            Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_BANNER_NOTIFICATION_SUBJECT", Services.Localization.Localization.GlobalResourceFile, Custom), _
                            Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_BANNER_NOTIFICATION_BODY", Services.Localization.Localization.GlobalResourceFile, Custom), _
                            "", "", "", "", "", "")

                        Dim strMessage As String
                        If errorMsg = "" Then
                            'Success
                            strMessage = Services.Localization.Localization.GetString("EmailSuccess", Me.LocalResourceFile)
                            UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                        Else
                            'Failed
                            strMessage = Services.Localization.Localization.GetString("EmailFailure", Me.LocalResourceFile)
                            strMessage = String.Format(strMessage, errorMsg)
                            UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If

                    End If
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DNNTxtBannerGroup_PopulateOnDemand runs when something is entered on the
        ''' BannerGroup field
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	9/29/2006	Implement a callback to display current groups
        '''  to user so the BannerGroup can be easily selected
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub DNNTxtBannerGroup_PopulateOnDemand(ByVal source As Object, ByVal e As UI.WebControls.DNNTextSuggestEventArgs) Handles DNNTxtBannerGroup.PopulateOnDemand

            Dim dt As DataTable
            Dim objNode As DotNetNuke.UI.WebControls.DNNNode

            Dim objBanners As New BannerController
            dt = objBanners.GetBannerGroups(PortalId)
            Dim dr() As DataRow
            dt.CaseSensitive = False
            dr = dt.Select("GroupName like '" & e.Text & "%'")
            For Each d As DataRow In dr
                objNode = New DotNetNuke.UI.WebControls.DNNNode(d("GroupName").ToString())
                objNode.ID = e.Nodes.Count.ToString
                e.Nodes.Add(objNode)
            Next

        End Sub
#End Region

#Region "Public Methods"
        Public Function FormatItem(ByVal VendorId As Integer, ByVal BannerId As Integer, ByVal BannerTypeId As Integer, ByVal BannerName As String, ByVal ImageFile As String, ByVal Description As String, ByVal URL As String, ByVal Width As Integer, ByVal Height As Integer) As String
            Dim objBanners As New BannerController
            Return objBanners.FormatBanner(VendorId, BannerId, BannerTypeId, BannerName, ImageFile, Description, URL, Width, Height, CType(IIf(PortalId = -1, "G", "L"), String), PortalSettings.HomeDirectory)
        End Function
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

    End Class

End Namespace