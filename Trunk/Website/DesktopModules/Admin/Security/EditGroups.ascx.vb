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
    Partial Class EditGroups
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private RoleGroupID As Integer = -1

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
                If Not (Request.QueryString("RoleGroupID") Is Nothing) Then
                    RoleGroupID = Int32.Parse(Request.QueryString("RoleGroupID"))
                End If

                If Page.IsPostBack = False Then
                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))
                    Dim objRoles As New RoleController
                    If RoleGroupID <> -1 Then
                        Dim objRoleGroupInfo As RoleGroupInfo = RoleController.GetRoleGroup(PortalId, RoleGroupID)
                        If Not objRoleGroupInfo Is Nothing Then
                            txtRoleGroupName.Text = objRoleGroupInfo.RoleGroupName
                            txtDescription.Text = objRoleGroupInfo.Description

                            'Check if Group has any roles assigned
                            Dim roleCount As Integer = objRoles.GetRolesByGroup(PortalId, RoleGroupID).Count

                            If roleCount > 0 Then
                                cmdDelete.Visible = False
                            End If
                        Else       ' security violation attempt to access item not related to this Module
                            Response.Redirect(NavigateURL("Security Roles"))
                        End If
                    Else
                        cmdDelete.Visible = False
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
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
        ''' 	[jlucarino]	2/26/2009	Added CreatedByUserID and LastModifiedByUserID
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    Dim objRoleGroupInfo As New RoleGroupInfo
                    objRoleGroupInfo.PortalID = PortalId
                    objRoleGroupInfo.RoleGroupID = RoleGroupID
                    objRoleGroupInfo.RoleGroupName = txtRoleGroupName.Text
                    objRoleGroupInfo.Description = txtDescription.Text

                    If RoleGroupID = -1 Then
                        Try
                            RoleController.AddRoleGroup(objRoleGroupInfo)
                        Catch
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("DuplicateRoleGroup", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            Exit Sub
                        End Try
                        Response.Redirect(NavigateURL(TabId, ""))
                    Else
                        RoleController.UpdateRoleGroup(objRoleGroupInfo)
                        Response.Redirect(NavigateURL(TabId, "", "RoleGroupID=" & RoleGroupID.ToString))
                    End If
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
                RoleController.DeleteRoleGroup(PortalId, RoleGroupID)
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
                If RoleGroupID = -1 Then
                    Response.Redirect(NavigateURL(TabId, ""))
                Else
                    Response.Redirect(NavigateURL(TabId, "", "RoleGroupID=" & RoleGroupID.ToString))
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
