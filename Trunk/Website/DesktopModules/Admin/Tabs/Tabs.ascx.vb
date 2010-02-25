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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Utilities
Imports System.Collections.Generic
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Modules.Admin.Tabs

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Tabs PortalModuleBase is used to manage the Tabs/Pages for a 
    ''' portal.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Tabs
        Inherits Entities.Modules.PortalModuleBase

        Protected ReadOnly Property IsAdminRole() As Boolean
            Get
                Return PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)
            End Get
        End Property

        Protected ReadOnly Property Tabs() As List(Of TabInfo)
            Get
                If chkDisplayHost.Checked Then
                    Return TabController.GetPortalTabs(Null.NullInteger, Null.NullInteger, False, True, False, True)
                Else
                    Return TabController.GetPortalTabs(PortalId, Null.NullInteger, False, True, False, True)
                End If
            End Get
        End Property

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' EditTab redirects to the Edit Tab Page for the currently selected tab/page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddTab()
            ' Redirect to edit page of currently selected tab
            If lstTabs.SelectedIndex <> -1 Then
                ' Redirect to module settings page
                Dim objTab As TabInfo = Tabs(lstTabs.SelectedIndex)
                Response.Redirect(NavigateURL(objTab.TabID, "Tab", "action=add", "returntabid=" & TabId.ToString), True)
            Else
                Response.Redirect(NavigateURL(PortalSettings.HomeTabId, "Tab", "action=add", "returntabid=" & TabId.ToString), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteTab deletes the selected tab
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/27/2007	Created
        ''' 	[jlucarino]	02/26/2009	added LastModifiedByUserID
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteTab()
            If lstTabs.SelectedIndex <> -1 Then
                Dim objTab As TabInfo = Tabs(lstTabs.SelectedIndex)

                If Not TabController.IsSpecialTab(objTab.TabID, PortalSettings) Then
                    TabController.DeleteTab(objTab.TabID, PortalSettings, UserId)

                    ' Redirect to this site to refresh
                    Response.Redirect(NavigateURL(TabId), True)
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DeleteSpecialPage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If

            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' EditTab redirects to the Edit Tab Page for the currently selected tab/page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub EditTab()
            ' Redirect to edit page of currently selected tab
            If lstTabs.SelectedIndex <> -1 Then
                ' Redirect to module settings page
                Dim objTab As TabInfo = Tabs(lstTabs.SelectedIndex)
                Response.Redirect(NavigateURL(objTab.TabID, "Tab", "action=edit", "returntabid=" & TabId.ToString), True)
            End If
        End Sub

        Private Sub CheckSecurity()
            If (Not TabPermissionController.HasTabPermission("CONTENT")) AndAlso Not ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, "CONTENT, EDIT") Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ViewTab redirects to the Tab/Page for the currently selected tab/page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ViewTab()
            If lstTabs.SelectedIndex <> -1 Then
                Dim objTabs As New TabController
                Dim objTab As TabInfo = objTabs.GetTab(Tabs(lstTabs.SelectedIndex).TabID, PortalId, False)
                If Not objTab Is Nothing Then
                    Response.Redirect(NavigateURL(objTab.TabID), True)
                End If
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
        ''' 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                CheckSecurity()
                pnlHost.Visible = UserInfo.IsSuperUser

                ' If this is the first visit to the page, bind the tab data to the page listbox
                If Page.IsPostBack = False Then

                    If Not String.IsNullOrEmpty(Request.QueryString("isHost")) Then
                        chkDisplayHost.Checked = Boolean.Parse(Request.QueryString("isHost"))
                    End If

                    lstTabs.DataSource = Tabs
                    lstTabs.DataBind()

                    ' select the tab ( if specified )
                    If Not String.IsNullOrEmpty(Request.QueryString("selecttabid")) Then
                        If Not lstTabs.Items.FindByValue(Request.QueryString("selecttabid")) Is Nothing Then
                            lstTabs.Items.FindByValue(Request.QueryString("selecttabid")).Selected = True
                        End If
                    End If
                End If


                ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpDown_Click runs when either the cmdUp or cmdDown buttons is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	 9/9/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' 	[jlucarino]	2/26/2009	Added LastModifiedByUserID
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Move_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles cmdTop.Click, cmdBottom.Click, cmdDown.Click, cmdUp.Click, cmdLeft.Click, cmdRight.Click
            Try
                If lstTabs.SelectedIndex <> -1 Then
                    Dim objTab As TabInfo = Tabs(lstTabs.SelectedIndex)
                    Dim objTabs As New TabController

                    Select Case CType(sender, ImageButton).CommandName
                        Case "top"
                            objTabs.MoveTab(objTab, TabMoveType.Top)
                        Case "bottom"
                            objTabs.MoveTab(objTab, TabMoveType.Bottom)
                        Case "up"
                            objTabs.MoveTab(objTab, TabMoveType.Up)
                        Case "down"
                            objTabs.MoveTab(objTab, TabMoveType.Down)
                        Case "left"
                            objTabs.MoveTab(objTab, TabMoveType.Promote)
                        Case "right"
                            objTabs.MoveTab(objTab, TabMoveType.Demote)
                    End Select

                    ' Redirect to this site to refresh
                    Response.Redirect(NavigateURL(TabId, "", "selecttabid", objTab.TabID.ToString, "isHost", chkDisplayHost.Checked.ToString().ToLowerInvariant()), True)
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub chkDisplayHost_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDisplayHost.CheckedChanged
            lstTabs.DataSource = Tabs
            lstTabs.DataBind()
        End Sub

        Protected Sub cmdAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdAdd.Click
            Try
                AddTab()
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the cmdDelete button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/27/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdDelete.Click
            Try
                DeleteTab()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdEdit_Click runs when the cmdEdit button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdEdit_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdEdit.Click
            Try
                EditTab()
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdView_Click runs when the cmdView button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdView_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles cmdView.Click
            Try
                ViewTab()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
