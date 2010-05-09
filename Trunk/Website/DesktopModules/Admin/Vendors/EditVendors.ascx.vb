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

Imports DotNetNuke.UI.UserControls
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Services.Vendors
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Vendors
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditVendors PortalModuleBase is used to add/edit a Vendor
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditVendors
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase


#Region "Private Members"

        Public VendorID As Integer = -1

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Return url redirects to the previous page, with or without filter info
        ''' </summary>
        ''' <param name="Filter"></param>
        ''' <history>
        ''' 	[erikvb]	10/18/2007
        ''' </history>
        Private Sub ReturnUrl(ByVal Filter As String)
            If String.IsNullOrEmpty(Filter.Trim) Then
                Response.Redirect(NavigateURL(), True)
            Else
                Response.Redirect(NavigateURL(Me.TabId, Null.NullString, "filter=" + Filter), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleMessage adds a module message
        ''' </summary>
        ''' <param name="message">The message</param>
        ''' <param name="type">The type of message</param>
        ''' <history>
        ''' 	[cnurse]	08/24/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddModuleMessage(ByVal message As String, ByVal type As Skins.Controls.ModuleMessage.ModuleMessageType)

            UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString(message, LocalResourceFile), type)

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim objTabs As New TabController
                Dim objTab As TabInfo
                Dim objModules As New ModuleController

                Dim blnBanner As Boolean = False
                Dim blnSignup As Boolean = False

                If Not (Request.QueryString("VendorID") Is Nothing) Then
                    VendorID = Int32.Parse(Request.QueryString("VendorID"))
                End If

                If Not Request.QueryString("ctl") Is Nothing And VendorID = -1 Then
                    blnSignup = True
                End If

                If Not Request.QueryString("banner") Is Nothing Then
                    blnBanner = True
                End If

                If Page.IsPostBack = False Then
                    ctlLogo.FileFilter = glbImageFileTypes

                    addresssVendor.ModuleId = ModuleId
                    addresssVendor.StartTabIndex = 4

                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    Dim objClassifications As New Services.Vendors.ClassificationController
                    Dim Arr As ArrayList = objClassifications.GetVendorClassifications(VendorID)
                    Dim i As Integer
                    For i = 0 To Arr.Count - 1
                        Dim lstItem As New ListItem
                        Dim objClassification As Services.Vendors.ClassificationInfo = CType(Arr(i), ClassificationInfo)
                        lstItem.Text = objClassification.ClassificationName
                        lstItem.Value = objClassification.ClassificationId.ToString
                        lstItem.Selected = objClassification.IsAssociated
                        lstClassifications.Items.Add(lstItem)
                    Next

                    Dim objVendors As New Services.Vendors.VendorController
                    If VendorID <> -1 Then
                        Dim objVendor As Services.Vendors.VendorInfo
                        If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId AndAlso UserInfo.IsSuperUser Then
                            'Get Host Vendor
                            objVendor = objVendors.GetVendor(VendorID, Null.NullInteger)
                        Else
                            'Get Portal Vendor
                            objVendor = objVendors.GetVendor(VendorID, PortalId)
                        End If
                        If Not objVendor Is Nothing Then
                            txtVendorName.Text = objVendor.VendorName
                            txtFirstName.Text = objVendor.FirstName
                            txtLastName.Text = objVendor.LastName
                            ctlLogo.Url = objVendor.LogoFile
                            addresssVendor.Unit = objVendor.Unit
                            addresssVendor.Street = objVendor.Street
                            addresssVendor.City = objVendor.City
                            addresssVendor.Region = objVendor.Region
                            addresssVendor.Country = objVendor.Country
                            addresssVendor.Postal = objVendor.PostalCode
                            addresssVendor.Telephone = objVendor.Telephone
                            addresssVendor.Fax = objVendor.Fax
                            addresssVendor.Cell = objVendor.Cell
                            txtEmail.Text = objVendor.Email
                            txtWebsite.Text = objVendor.Website
                            chkAuthorized.Checked = objVendor.Authorized
                            txtKeyWords.Text = objVendor.KeyWords

                            ctlAudit.CreatedByUser = objVendor.CreatedByUser
                            ctlAudit.CreatedDate = objVendor.CreatedDate.ToString
                        End If

                        ' use dispatch method to load modules
                        Dim objBanners As Banners
                        objBanners = CType(Me.LoadControl("~" & Me.TemplateSourceDirectory.Remove(0, Common.Globals.ApplicationPath.Length) & "/Banners.ascx"), Banners)
                        objBanners.ID = "/Banners.ascx"
                        objBanners.VendorID = Me.VendorID
                        objBanners.ModuleConfiguration = ModuleConfiguration
                        divBanners.Controls.Add(objBanners)

                        Dim objAffiliates As Affiliates

                        objAffiliates = CType(Me.LoadControl("~" & Me.TemplateSourceDirectory.Remove(0, Common.Globals.ApplicationPath.Length) & "/Affiliates.ascx"), Affiliates)
                        objAffiliates.ID = "/Affiliates.ascx"
                        objAffiliates.VendorID = Me.VendorID
                        objAffiliates.ModuleConfiguration = ModuleConfiguration
                        divAffiliates.Controls.Add(objAffiliates)

                    Else
                        chkAuthorized.Checked = True
                        pnlAudit.Visible = False
                        cmdDelete.Visible = False
                        pnlBanners.Visible = False
                        pnlAffiliates.Visible = False
                    End If

                    If blnSignup = True Or blnBanner = True Then
                        rowVendor1.Visible = False
                        rowVendor2.Visible = False
                        pnlVendor.Visible = False
                        cmdDelete.Visible = False
                        pnlAudit.Visible = False

                        If blnBanner = True Then
                            cmdUpdate.Visible = False
                        Else

                            cmdUpdate.Text = "Signup"
                        End If
                    Else
                        If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                            objTab = objTabs.GetTabByName("Vendors", Null.NullInteger)
                        Else
                            objTab = objTabs.GetTabByName("Vendors", PortalId)
                        End If
                        If Not objTab Is Nothing Then
                            ViewState("filter") = Request.QueryString("filter")
                        End If
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel button is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                ReturnUrl(Convert.ToString(ViewState("filter")))

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete button is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                If VendorID <> -1 Then
                    Dim objVendors As New VendorController
                    objVendors.DeleteVendor(VendorID)
                End If
                Response.Redirect(NavigateURL())

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update button is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try

                Dim intPortalID As Integer
                Dim strLogoFile As String = ""

                If Page.IsValid Then

                    If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                        intPortalID = -1
                    Else
                        intPortalID = PortalId
                    End If

                    Dim objVendors As New VendorController
                    Dim objVendor As New VendorInfo

                    objVendor.PortalId = intPortalID
                    objVendor.VendorId = VendorID
                    objVendor.VendorName = txtVendorName.Text
                    objVendor.Unit = addresssVendor.Unit
                    objVendor.Street = addresssVendor.Street
                    objVendor.City = addresssVendor.City
                    objVendor.Region = addresssVendor.Region
                    objVendor.Country = addresssVendor.Country
                    objVendor.PostalCode = addresssVendor.Postal
                    objVendor.Telephone = addresssVendor.Telephone
                    objVendor.Fax = addresssVendor.Fax
                    objVendor.Cell = addresssVendor.Cell
                    objVendor.Email = txtEmail.Text
                    objVendor.Website = txtWebsite.Text
                    objVendor.FirstName = txtFirstName.Text
                    objVendor.LastName = txtLastName.Text
                    objVendor.UserName = UserInfo.UserID.ToString
                    objVendor.LogoFile = ctlLogo.Url
                    objVendor.KeyWords = txtKeyWords.Text
                    objVendor.Authorized = chkAuthorized.Checked

                    If VendorID = -1 Then
                        Try
                            VendorID = objVendors.AddVendor(objVendor)
                        Catch
                            AddModuleMessage("ErrorAddVendor", Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            Exit Sub
                        End Try
                    Else

                        Dim objVendorCheck As New VendorInfo
                        objVendorCheck = objVendors.GetVendor(VendorID, intPortalID)
                        If Not objVendorCheck Is Nothing Then
                            objVendors.UpdateVendor(objVendor)
                        Else
                            Response.Redirect(NavigateURL())
                        End If
                    End If

                    ' update vendor classifications
                    Dim objClassifications As New ClassificationController
                    objClassifications.DeleteVendorClassifications(VendorID)
                    Dim lstItem As ListItem
                    For Each lstItem In lstClassifications.Items
                        If lstItem.Selected Then
                            objClassifications.AddVendorClassification(VendorID, Int32.Parse(lstItem.Value))
                        End If
                    Next

                    If cmdUpdate.Text = "Signup" Then

                        Dim Custom As New ArrayList
                        Custom.Add(Now().ToString)
                        Custom.Add(txtVendorName.Text)
                        Custom.Add(txtFirstName.Text)
                        Custom.Add(txtLastName.Text)
                        Custom.Add(addresssVendor.Unit)
                        Custom.Add(addresssVendor.Street)
                        Custom.Add(addresssVendor.City)
                        Custom.Add(addresssVendor.Region)
                        Custom.Add(addresssVendor.Country)
                        Custom.Add(addresssVendor.Postal)
                        Custom.Add(addresssVendor.Telephone)
                        Custom.Add(addresssVendor.Fax)
                        Custom.Add(addresssVendor.Cell)
                        Custom.Add(txtEmail.Text)
                        Custom.Add(txtWebsite.Text)

                        'send email to Admin
                        Mail.SendEmail(PortalSettings.Email, PortalSettings.Email, Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_VENDOR_REGISTRATION_ADMINISTRATOR_SUBJECT"), _
                                        Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_VENDOR_REGISTRATION_ADMINISTRATOR_BODY", Services.Localization.Localization.GlobalResourceFile, Custom))


                        'send email to vendor
                        Custom.Clear()
                        Custom.Add(txtFirstName.Text)
                        Custom.Add(txtLastName.Text)
                        Custom.Add(txtVendorName.Text)

                        Mail.SendEmail(PortalSettings.Email, txtEmail.Text, Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_VENDOR_REGISTRATION_SUBJECT"), _
                                        Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_VENDOR_REGISTRATION_BODY", Services.Localization.Localization.GlobalResourceFile, Custom))

                     
                        ReturnUrl(txtVendorName.Text.Substring(0, 1))
                     
                    Else
                        ReturnUrl(Convert.ToString(ViewState("filter")))
                    End If

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
