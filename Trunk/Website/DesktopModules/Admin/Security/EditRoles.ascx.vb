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
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Security

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditRoles PortalModuleBase is used to manage a Security Role
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditRoles
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private RoleID As Integer = -1

#End Region

#Region "Private Methods"
        Private Sub ActivateControls(ByVal enabled As Boolean)
            cboRoleGroups.Enabled = enabled
            chkIsPublic.Enabled = enabled
            chkAutoAssignment.Enabled = enabled
            txtServiceFee.Enabled = enabled
            txtBillingPeriod.Enabled = enabled
            cboBillingFrequency.Enabled = enabled
            txtTrialFee.Enabled = enabled
            txtTrialPeriod.Enabled = enabled
            cboTrialFrequency.Enabled = enabled
            txtRSVPCode.Enabled = enabled
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindGroups gets the role Groups from the Database and binds them to the DropDown
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    01/05/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGroups()

            Dim arrGroups As ArrayList = RoleController.GetRoleGroups(PortalId)

            cboRoleGroups.Items.Add(New ListItem(Localization.GetString("GlobalRoles"), "-1"))

            For Each roleGroup As RoleGroupInfo In arrGroups
                cboRoleGroups.Items.Add(New ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString))
            Next

        End Sub

        Private Sub UpdateFeeTextBoxes()
            If cboBillingFrequency.SelectedValue = "O" Then
                txtBillingPeriod.Text = 1
                txtBillingPeriod.Enabled = False
            Else
                txtBillingPeriod.Enabled = True
            End If
            If cboTrialFrequency.SelectedValue = "O" Then
                txtTrialPeriod.Text = 1
                txtTrialPeriod.Enabled = False
            Else
                txtTrialPeriod.Enabled = True
            End If

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
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not (Request.QueryString("RoleID") Is Nothing) Then
                    RoleID = Int32.Parse(Request.QueryString("RoleID"))
                End If

                Dim objPortalController As New PortalController
                Dim objPortalInfo As PortalInfo = objPortalController.GetPortal(PortalSettings.PortalId)
                If (objPortalInfo Is Nothing OrElse String.IsNullOrEmpty(objPortalInfo.ProcessorUserId)) Then
                    'Warn users about fee based roles if we have a Processor Id
                    lblProcessorWarning.Visible = True
                Else
                    trBillingPeriod.Visible = True
                    trServiceFee.Visible = True
                    trTrialFee.Visible = True
                    trTrialPeriod.Visible = True
                End If

                If Page.IsPostBack = False Then
                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    Dim objUser As New RoleController

                    Dim ctlList As New Common.Lists.ListController
                    Dim colFrequencies As Common.Lists.ListEntryInfoCollection = ctlList.GetListEntryInfoCollection("Frequency", "")

                    cboBillingFrequency.DataSource = colFrequencies
                    cboBillingFrequency.DataBind()
                    cboBillingFrequency.Items.FindByValue("N").Selected = True

                    cboTrialFrequency.DataSource = colFrequencies
                    cboTrialFrequency.DataBind()
                    cboTrialFrequency.Items.FindByValue("N").Selected = True

                    BindGroups()

                    ctlIcon.FileFilter = glbImageFileTypes

                    If RoleID <> -1 Then
                        lblRoleName.Visible = True
                        txtRoleName.Visible = False
                        valRoleName.Enabled = False

                        Dim objRoleInfo As RoleInfo = objUser.GetRole(RoleID, PortalSettings.PortalId)

                        If Not objRoleInfo Is Nothing Then
                            lblRoleName.Text = objRoleInfo.RoleName
                            txtDescription.Text = objRoleInfo.Description
                            If Not cboRoleGroups.Items.FindByValue(objRoleInfo.RoleGroupID.ToString) Is Nothing Then
                                cboRoleGroups.ClearSelection()
                                cboRoleGroups.Items.FindByValue(objRoleInfo.RoleGroupID.ToString).Selected = True
                            End If

                            If objRoleInfo.BillingFrequency <> "N" Then
                                txtServiceFee.Text = objRoleInfo.ServiceFee.ToString("N2", CultureInfo.CurrentCulture)
                                txtBillingPeriod.Text = objRoleInfo.BillingPeriod.ToString
                                If Not cboBillingFrequency.Items.FindByValue(objRoleInfo.BillingFrequency) Is Nothing Then
                                    cboBillingFrequency.ClearSelection()
                                    cboBillingFrequency.Items.FindByValue(objRoleInfo.BillingFrequency).Selected = True
                                End If
                            End If
                            If objRoleInfo.TrialFrequency <> "N" Then
                                txtTrialFee.Text = objRoleInfo.TrialFee.ToString("N2", CultureInfo.CurrentCulture)
                                txtTrialPeriod.Text = objRoleInfo.TrialPeriod.ToString
                                If Not cboTrialFrequency.Items.FindByValue(objRoleInfo.TrialFrequency) Is Nothing Then
                                    cboTrialFrequency.ClearSelection()
                                    cboTrialFrequency.Items.FindByValue(objRoleInfo.TrialFrequency).Selected = True
                                End If
                            End If

                            chkIsPublic.Checked = objRoleInfo.IsPublic
                            chkAutoAssignment.Checked = objRoleInfo.AutoAssignment
                            txtRSVPCode.Text = objRoleInfo.RSVPCode
                            If txtRSVPCode.Text <> "" Then
                                lblRSVPLink.Text = AddHTTP(GetDomainName(Request)) & "/" & glbDefaultPage & "?rsvp=" & txtRSVPCode.Text & "&portalid=" & PortalId.ToString()
                            End If
                            ctlIcon.Url = objRoleInfo.IconFile

                            UpdateFeeTextBoxes()
                        Else                         ' security violation attempt to access item not related to this Module
                            Response.Redirect(NavigateURL("Security Roles"))
                        End If

                        If RoleID = PortalSettings.AdministratorRoleId OrElse RoleID = PortalSettings.RegisteredRoleId Then
                            cmdDelete.Visible = False
                            ActivateControls(False)
                        End If

                        If RoleID = PortalSettings.RegisteredRoleId Then
                            cmdManage.Visible = False
                        End If
                    Else
                        cmdDelete.Visible = False
                        cmdManage.Visible = False
                        lblRoleName.Visible = False
                        txtRoleName.Visible = True
                    End If
                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cboBillingFrequency_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboBillingFrequency.SelectedIndexChanged
            UpdateFeeTextBoxes()
        End Sub

        Protected Sub cboTrialFrequency_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTrialFrequency.SelectedIndexChanged
            UpdateFeeTextBoxes()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' 	[jlucarino]	2/23/2009	Added CreatedByUserID and LastModifiedByUserID
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then

                    Dim sglServiceFee As Single = 0
                    Dim intBillingPeriod As Integer = 1
                    Dim strBillingFrequency As String = "N"

                    If cboBillingFrequency.SelectedItem.Value = "N" AndAlso txtServiceFee.Text <> "" Then
                        Skins.Skin.AddModuleMessage(Me, Localization.GetString("IncompatibleFee", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        Exit Sub
                    End If

                    If txtServiceFee.Text <> "" And txtBillingPeriod.Text <> "" And cboBillingFrequency.SelectedItem.Value <> "N" Then
                        sglServiceFee = Single.Parse(txtServiceFee.Text)
                        intBillingPeriod = Integer.Parse(txtBillingPeriod.Text)
                        strBillingFrequency = cboBillingFrequency.SelectedItem.Value
                    End If

                    Dim sglTrialFee As Single = 0
                    Dim intTrialPeriod As Integer = 1
                    Dim strTrialFrequency As String = "N"

                    If sglServiceFee <> 0 And txtTrialFee.Text <> "" And txtTrialPeriod.Text <> "" And cboTrialFrequency.SelectedItem.Value <> "N" Then
                        sglTrialFee = Single.Parse(txtTrialFee.Text)
                        intTrialPeriod = Integer.Parse(txtTrialPeriod.Text)
                        strTrialFrequency = cboTrialFrequency.SelectedItem.Value
                    End If

                    Dim objRoleController As New RoleController
                    Dim objRoleInfo As New RoleInfo
                    objRoleInfo.PortalID = PortalId
                    objRoleInfo.RoleID = RoleID
                    objRoleInfo.RoleGroupID = Integer.Parse(cboRoleGroups.SelectedValue)
                    objRoleInfo.RoleName = txtRoleName.Text
                    objRoleInfo.Description = txtDescription.Text
                    objRoleInfo.ServiceFee = sglServiceFee
                    objRoleInfo.BillingPeriod = intBillingPeriod
                    objRoleInfo.BillingFrequency = strBillingFrequency
                    objRoleInfo.TrialFee = sglTrialFee
                    objRoleInfo.TrialPeriod = intTrialPeriod
                    objRoleInfo.TrialFrequency = strTrialFrequency
                    objRoleInfo.IsPublic = chkIsPublic.Checked
                    objRoleInfo.AutoAssignment = chkAutoAssignment.Checked
                    objRoleInfo.RSVPCode = txtRSVPCode.Text
                    objRoleInfo.IconFile = ctlIcon.Url

                    If RoleID = -1 Then
                        If objRoleController.GetRoleByName(PortalId, objRoleInfo.RoleName) Is Nothing Then
                            objRoleController.AddRole(objRoleInfo)

                        Else
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("DuplicateRole", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            Exit Sub
                        End If
                    Else
                        objRoleController.UpdateRole(objRoleInfo)
                    End If

                    'Clear Roles Cache
                    DataCache.RemoveCache("GetRoles")

                    Response.Redirect(NavigateURL())

                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the delete Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                Dim objUser As New RoleController

                objUser.DeleteRole(RoleID, PortalSettings.PortalId)
              
                'Clear Roles Cache
                DataCache.RemoveCache("GetRoles")

                Response.Redirect(NavigateURL())

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdManage_Click runs when the Manage Users Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdManage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdManage.Click
            Try
                Response.Redirect(Me.EditUrl("RoleId", RoleID.ToString(), "User Roles"))
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub txtRSVPCode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRSVPCode.TextChanged
            lblRSVPLink.Text = AddHTTP(GetDomainName(Request)) & "/" & glbDefaultPage & "?rsvp=" & txtRSVPCode.Text & "&portalid=" & PortalId.ToString()
        End Sub

#End Region

    End Class

End Namespace
