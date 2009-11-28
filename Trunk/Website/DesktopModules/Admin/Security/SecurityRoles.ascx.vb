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

Imports DotNetNuke.UI.Skins.Controls
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Modules.Admin.Security

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SecurityRoles PortalModuleBase is used to manage the users and roles they
    ''' have
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SecurityRoles
        Inherits PortalModuleBase
        Implements IActionable

#Region "Private Members"

        Private _ParentModule As PortalModuleBase

        Private RoleId As Integer = -1
        Private _SelectedUserID As Integer = -1
        Private Shadows UserId As Integer = -1

        Private _Role As RoleInfo
        Private _User As UserInfo

#End Region

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url for the page
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/14/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ReturnUrl() As String
            Get
                Dim _ReturnURL As String
                Dim FilterParams(IIf(Request.QueryString("filterproperty") = "", 2, 3)) As String

                If (Request.QueryString("filterProperty") = "") Then
                    FilterParams.SetValue("filter=" & Request.QueryString("filter"), 0)
                    FilterParams.SetValue("currentpage=" & Request.QueryString("currentpage"), 1)
                Else
                    FilterParams.SetValue("filter=" & Request.QueryString("filter"), 0)
                    FilterParams.SetValue("filterProperty=" & Request.QueryString("filterProperty"), 1)
                    FilterParams.SetValue("currentpage=" & Request.QueryString("currentpage"), 2)
                End If

                If String.IsNullOrEmpty(Request.QueryString("filter")) Then
                    _ReturnURL = NavigateURL(TabId)
                Else
                    _ReturnURL = NavigateURL(TabId, "", FilterParams)
                End If

                Return _ReturnURL
            End Get
        End Property

        Protected ReadOnly Property Role() As RoleInfo
            Get
                If _Role Is Nothing Then
                    Dim objRoleController As New RoleController
                    If RoleId <> Null.NullInteger Then
                        _Role = objRoleController.GetRole(RoleId, PortalId)
                    ElseIf cboRoles.SelectedItem IsNot Nothing Then
                        _Role = objRoleController.GetRole(Convert.ToInt32(cboRoles.SelectedItem.Value), PortalId)
                    End If
                End If
                Return _Role
            End Get
        End Property

        Protected ReadOnly Property User() As UserInfo
            Get
                If _User Is Nothing Then
                    If UserId <> Null.NullInteger Then
                        _User = UserController.GetUserById(PortalId, UserId)
                    ElseIf UsersControl = UsersControl.TextBox AndAlso txtUsers.Text <> "" Then
                        _User = UserController.GetUserByName(PortalId, txtUsers.Text)
                    ElseIf UsersControl = UsersControl.Combo AndAlso (Not cboUsers.SelectedItem Is Nothing) Then
                        _User = UserController.GetUserById(PortalId, Convert.ToInt32(cboUsers.SelectedItem.Value))
                    End If
                End If
                Return _User
            End Get
        End Property

        Protected Property SelectedUserID() As Integer
            Get
                Return _SelectedUserID
            End Get
            Set(ByVal value As Integer)
                _SelectedUserID = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the control should use a Combo Box or Text Box to display the users
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UsersControl() As UsersControl
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Security_UsersControl")
                Return CType(setting, UsersControl)
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ParentModule (if one exists)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ParentModule() As PortalModuleBase
            Get
                Return _ParentModule
            End Get
            Set(ByVal Value As PortalModuleBase)
                _ParentModule = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData loads the controls from the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            Dim objRoles As New RoleController

            ' bind all portal roles to dropdownlist
            If RoleId = -1 Then
                If cboRoles.Items.Count = 0 Then
                    Dim arrRoles As ArrayList = objRoles.GetPortalRoles(PortalId)

                    'Remove access to Admin Role if use is not a member of the role
                    Dim roleIndex As Integer = Null.NullInteger
                    For Each tmpRole As RoleInfo In arrRoles
                        If tmpRole.RoleName = PortalSettings.AdministratorRoleName Then
                            If Not PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                                roleIndex = arrRoles.IndexOf(tmpRole)
                            End If
                        End If
                        Exit For
                    Next
                    If roleIndex > Null.NullInteger Then
                        arrRoles.RemoveAt(roleIndex)
                    End If
                    cboRoles.DataSource = arrRoles
                    cboRoles.DataBind()
                End If
            Else
                If Not Page.IsPostBack Then
                    If Not Role Is Nothing Then
                        cboRoles.Items.Add(New ListItem(Role.RoleName, Role.RoleID.ToString))
                        cboRoles.Items(0).Selected = True
                        lblTitle.Text = String.Format(Localization.GetString("RoleTitle.Text", LocalResourceFile), Role.RoleName, Role.RoleID.ToString)
                    End If
                    cboRoles.Visible = False
                    plRoles.Visible = False
                End If
            End If

            ' bind all portal users to dropdownlist
            If UserId = -1 Then
                'Make sure user has enough permissions
                If Role.RoleName = PortalSettings.AdministratorRoleName AndAlso Not PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                    Skin.AddModuleMessage(Me, Localization.GetString("NotAuthorized", LocalResourceFile), ModuleMessage.ModuleMessageType.YellowWarning)
                    pnlRoles.Visible = False
                    pnlUserRoles.Visible = False
                    chkNotify.Visible = False
                    Exit Sub
                End If
                If UsersControl = UsersControl.Combo Then
                    If cboUsers.Items.Count = 0 Then
                        For Each objUser As UserInfo In UserController.GetUsers(PortalId)
                            cboUsers.Items.Add(New ListItem(objUser.DisplayName + " (" + objUser.Username + ")", objUser.UserID.ToString()))
                        Next
                        'cboUsers.DataSource = UserController.GetUsers(PortalId, False)
                        'cboUsers.DataBind()
                    End If
                    txtUsers.Visible = False
                    cboUsers.Visible = True
                    cmdValidate.Visible = False
                Else
                    txtUsers.Visible = True
                    cboUsers.Visible = False
                    cmdValidate.Visible = True
                End If
            Else
                If Not User Is Nothing Then
                    txtUsers.Text = User.UserID.ToString
                    lblTitle.Text = String.Format(Localization.GetString("UserTitle.Text", LocalResourceFile), User.Username, User.UserID.ToString)
                End If
                txtUsers.Visible = False
                cboUsers.Visible = False
                cmdValidate.Visible = False
                plUsers.Visible = False
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindGrid loads the data grid from the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGrid()
            Dim objRoleController As New RoleController

            If RoleId <> -1 Then
                cmdAdd.Text = Localization.GetString("AddUser.Text", LocalResourceFile)
                grdUserRoles.DataKeyField = "UserId"
                grdUserRoles.Columns(2).Visible = False
                grdUserRoles.DataSource = objRoleController.GetUserRolesByRoleName(PortalId, Role.RoleName)
                grdUserRoles.DataBind()
            End If
            If UserId <> -1 Then
                cmdAdd.Text = Localization.GetString("AddRole.Text", LocalResourceFile)
                grdUserRoles.DataKeyField = "RoleId"
                grdUserRoles.Columns(1).Visible = False
                grdUserRoles.DataSource = objRoleController.GetUserRolesByUsername(PortalId, User.Username, Null.NullString)
                grdUserRoles.DataBind()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetDates gets the expiry/effective Dates of a Users Role membership
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="UserId">The Id of the User</param>
        ''' <param name="RoleId">The Id of the Role</param>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [cnurse]    01/20/2006  Added support for Effective Date
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetDates(ByVal UserId As Integer, ByVal RoleId As Integer)

            Dim strExpiryDate As String = ""
            Dim strEffectiveDate As String = ""

            Dim objRoles As New RoleController
            Dim objUserRole As UserRoleInfo = objRoles.GetUserRole(PortalId, UserId, RoleId)
            If Not objUserRole Is Nothing Then
                If Null.IsNull(objUserRole.EffectiveDate) = False Then
                    strEffectiveDate = objUserRole.EffectiveDate.ToShortDateString
                End If
                If Null.IsNull(objUserRole.ExpiryDate) = False Then
                    strExpiryDate = objUserRole.ExpiryDate.ToShortDateString
                End If
            Else    ' new role assignment
                Dim objRole As RoleInfo = objRoles.GetRole(RoleId, PortalId)

                If objRole.BillingPeriod > 0 Then
                    Select Case objRole.BillingFrequency
                        Case "D" : strExpiryDate = DateAdd(DateInterval.Day, objRole.BillingPeriod, Now).ToShortDateString
                        Case "W" : strExpiryDate = DateAdd(DateInterval.Day, (objRole.BillingPeriod * 7), Now).ToShortDateString
                        Case "M" : strExpiryDate = DateAdd(DateInterval.Month, objRole.BillingPeriod, Now).ToShortDateString
                        Case "Y" : strExpiryDate = DateAdd(DateInterval.Year, objRole.BillingPeriod, Now).ToShortDateString
                    End Select
                End If
            End If

            txtEffectiveDate.Text = strEffectiveDate
            txtExpiryDate.Text = strExpiryDate

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/10/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            If Not ModulePermissionController.CanEditModuleContent(Me.ModuleConfiguration) Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If

            MyBase.DataBind()

            'this needs to execute always to the client script code is registred in InvokePopupCal
            cmdEffectiveCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEffectiveDate)
            cmdExpiryCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtExpiryDate)

            Dim localizedCalendarText As String = Localization.GetString("Calendar")
            Dim calendarText As String = "<img src='" + ResolveUrl("~/images/calendar.png") + "' border='0' alt='" + localizedCalendarText + "'>"
            cmdExpiryCalendar.Text = calendarText
            cmdEffectiveCalendar.Text = calendarText

            'Localize Headers
            Localization.LocalizeDataGrid(grdUserRoles, Me.LocalResourceFile)

            'Bind the role data to the datalist
            BindData()

            BindGrid()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteButonVisible returns a boolean indicating if the delete button for
        ''' the specified UserID, RoleID pair should be shown
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="UserID">The ID of the user to check delete button visibility for</param>
        ''' <param name="RoleID">The ID of the role to check delete button visibility for</param>
        ''' <history>
        ''' 	[anurse]	01/13/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function DeleteButtonVisible(ByVal UserID As Integer, ByVal RoleID As Integer) As Boolean
            ' [DNN-4285] Check if the role can be removed (only handles case of Administrator and Administrator Role
            Dim canDelete As Boolean = RoleController.CanRemoveUserFromRole(Me.PortalSettings, UserID, RoleID)
            If RoleID = PortalSettings.AdministratorRoleId AndAlso canDelete Then
                'User can only delete if in Admin role
                canDelete = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)
            End If

            Return canDelete
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatExpiryDate formats the expiry/effective date and filters out nulls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="DateTime">The Date object to format</param>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function FormatDate(ByVal DateTime As Date) As String
            If Not Null.IsNull(DateTime) Then
                Return DateTime.ToShortDateString
            Else
                Return ""
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatExpiryDate formats the expiry/effective date and filters out nulls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function FormatUser(ByVal UserID As Integer, ByVal DisplayName As String) As String
            Return "<a href=""" & Common.LinkClick("userid=" & UserID.ToString, TabId, ModuleId) & """ class=""CommandButton"">" & DisplayName & "</a>"
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            If Not (Request.QueryString("RoleId") Is Nothing) Then
                RoleId = Int32.Parse(Request.QueryString("RoleId"))
            End If

            If Not (Request.QueryString("UserId") Is Nothing) Then
                UserId = Int32.Parse(Request.QueryString("UserId"))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If ParentModule Is Nothing Then
                    DataBind()
                End If
            Catch exc As Threading.ThreadAbortException
                'Do nothing if ThreadAbort as this is caused by a redirect
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cboUsers_SelectedIndexChanged runs when the selected User is changed in the
        ''' Users Drop-Down
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cboUsers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboUsers.SelectedIndexChanged
            If (Not cboUsers.SelectedItem Is Nothing) And (Not cboRoles.SelectedItem Is Nothing) Then
                SelectedUserID = Int32.Parse(cboUsers.SelectedItem.Value)
                GetDates(SelectedUserID, Int32.Parse(cboRoles.SelectedItem.Value))
            End If
            BindGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdValidate_Click executes when a user selects the Validate link for a username
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdValidate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdValidate.Click
            If txtUsers.Text <> "" Then
                ' validate username
                Dim objUser As UserInfo = UserController.GetUserByName(PortalId, txtUsers.Text)
                If Not objUser Is Nothing Then
                    GetDates(objUser.UserID, RoleId)
                    SelectedUserID = objUser.UserID
                Else
                    txtUsers.Text = ""
                End If
            End If
            BindGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cboRoles_SelectedIndexChanged runs when the selected Role is changed in the
        ''' Roles Drop-Down
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cboRoles_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboRoles.SelectedIndexChanged
            GetDates(UserId, Int32.Parse(cboRoles.SelectedItem.Value))
            BindGrid()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdAdd_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAdd.Click
            Try
                If Page.IsValid Then
                    If (Not Role Is Nothing) AndAlso (Not User Is Nothing) Then
                        ' do not modify the portal Administrator account dates
                        If User.UserID = PortalSettings.AdministratorId And Role.RoleID = PortalSettings.AdministratorRoleId.ToString Then
                            txtEffectiveDate.Text = ""
                            txtExpiryDate.Text = ""
                        End If

                        Dim datEffectiveDate As Date
                        If txtEffectiveDate.Text <> "" Then
                            datEffectiveDate = Date.Parse(txtEffectiveDate.Text)
                        Else
                            datEffectiveDate = Null.NullDate
                        End If
                        Dim datExpiryDate As Date
                        If txtExpiryDate.Text <> "" Then
                            datExpiryDate = Date.Parse(txtExpiryDate.Text)
                        Else
                            datExpiryDate = Null.NullDate
                        End If

                        'Add User to Role
                        RoleController.AddUserRole(User, Role, PortalSettings, datEffectiveDate, datExpiryDate, UserId, chkNotify.Checked)
                    End If
                End If

                BindGrid()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdUserRoles_Delete runs when one of the Delete Buttons in the UserRoles Grid
        ''' is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub grdUserRoles_Delete(ByVal sender As Object, ByVal e As DataGridCommandEventArgs)
            Try
                Dim strMessage As String = ""
                If RoleId <> Null.NullInteger Then
                    If Not RoleController.DeleteUserRole(Integer.Parse(Convert.ToString(grdUserRoles.DataKeys(e.Item.ItemIndex))), Role, PortalSettings, chkNotify.Checked) Then
                        strMessage = Services.Localization.Localization.GetString("RoleRemoveError", Me.LocalResourceFile)
                    End If
                End If
                If UserId <> Null.NullInteger Then
                    If Not RoleController.DeleteUserRole(Integer.Parse(Convert.ToString(grdUserRoles.DataKeys(e.Item.ItemIndex))), User, PortalSettings, chkNotify.Checked) Then
                        strMessage = Services.Localization.Localization.GetString("RoleRemoveError", Me.LocalResourceFile)
                    End If
                End If

                grdUserRoles.EditItemIndex = -1
                BindGrid()

                If strMessage <> "" Then
                    UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdUserRoles_ItemCreated runs when an item in the UserRoles Grid is created
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdUserRoles_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdUserRoles.ItemCreated
            Try

                Dim cmdDeleteUserRole As Control = e.Item.FindControl("cmdDeleteUserRole")

                If Not cmdDeleteUserRole Is Nothing Then
                    ClientAPI.AddButtonConfirm(CType(cmdDeleteUserRole, ImageButton), Services.Localization.Localization.GetString("DeleteItem"))
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdUserRoles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdUserRoles.ItemDataBound
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim userRole As UserRoleInfo = CType(item.DataItem, UserRoleInfo)

                If RoleId = Null.NullInteger Then
                    If userRole.RoleID = Convert.ToInt32(cboRoles.SelectedValue) Then
                        cmdAdd.Text = Localization.GetString("UpdateRole.Text", LocalResourceFile)
                    End If
                End If

                If UserId = Null.NullInteger Then
                    If userRole.UserID = SelectedUserID Then
                        cmdAdd.Text = Localization.GetString("UpdateRole.Text", LocalResourceFile)
                    End If
                End If


            End If
        End Sub

#End Region

#Region "Optional Interfaces"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleActions for this ModuleControl
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	3/01/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection

                Actions.Add(GetNextActionID, Localization.GetString("Cancel.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", ReturnUrl, False, SecurityAccessLevel.Edit, True, False)
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
