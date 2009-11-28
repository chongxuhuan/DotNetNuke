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

Imports System.Web.Security

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.UI.Skins


Namespace DotNetNuke.Modules.Admin.Security

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The MemberServices UserModuleBase is used to manage a User's services
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/03/2006
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class MemberServices
        Inherits UserModuleBase

#Region "Delegates"

        Public Delegate Sub SubscriptionUpdatedEventHandler(ByVal sender As Object, ByVal e As SubscriptionUpdatedEventArgs)

#End Region

#Region "Events"

        Public Event SubscriptionUpdated As SubscriptionUpdatedEventHandler

#End Region

#Region "Private Members"

        Private RoleID As Integer = -1

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatPrice formats the Fee amount and filters out null-values
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="price">The price to format</param>
        '''	<returns>The correctly formatted price</returns>
        ''' <history>
        ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FormatPrice(ByVal price As Single) As String
            Dim _FormatPrice As String = Null.NullString
            Try
                If price <> Null.NullSingle Then
                    _FormatPrice = price.ToString("##0.00")
                Else
                    _FormatPrice = ""
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _FormatPrice
        End Function

        Private Function GetRoles(ByVal portalId As Integer, ByVal userId As Integer) As ArrayList
            Dim objRoles As New RoleController

            Return objRoles.GetUserRoles(portalId, userId, False)

        End Function

        Private Sub Subscribe(ByVal roleID As Integer, ByVal cancel As Boolean)
            Dim objRoles As New RoleController
            Dim objRole As RoleInfo = objRoles.GetRole(roleID, PortalSettings.PortalId)

            If objRole.IsPublic And objRole.ServiceFee = 0.0 Then
                objRoles.UpdateUserRole(PortalId, UserInfo.UserID, roleID, cancel)

                'Raise SubscriptionUpdated Event
                OnSubscriptionUpdated(New SubscriptionUpdatedEventArgs(cancel, objRole.RoleName))
            Else
                If Not cancel Then
                    Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" & TabId & "&RoleID=" & roleID.ToString, True)
                Else
                    Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" & TabId & "&RoleID=" & roleID.ToString & "&cancel=1", True)
                End If
            End If
        End Sub

        Private Sub UseTrial(ByVal roleID As Integer)
            Dim objRoles As New RoleController
            Dim objRole As RoleInfo = objRoles.GetRole(roleID, PortalSettings.PortalId)

            If objRole.IsPublic And objRole.TrialFee = 0.0 Then
                objRoles.UpdateUserRole(PortalId, UserInfo.UserID, roleID, False)

                'Raise SubscriptionUpdated Event
                OnSubscriptionUpdated(New SubscriptionUpdatedEventArgs(False, objRole.RoleName))
            Else
                Response.Redirect("~/admin/Sales/PayPalSubscription.aspx?tabid=" & TabId & "&RoleID=" & roleID.ToString, True)
            End If
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatExpiryDate formats the expiry date and filters out null-values
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="expiryDate">The date to format</param>
        '''	<returns>The correctly formatted date</returns>
        ''' <history>
        ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatExpiryDate(ByVal expiryDate As Date) As String
            Dim _FormatExpiryDate As String = Null.NullString
            Try
                If Not Null.IsNull(expiryDate) Then
                    If expiryDate > Date.Today Then
                        _FormatExpiryDate = expiryDate.ToShortDateString
                    Else
                        _FormatExpiryDate = Localization.GetString("Expired", Me.LocalResourceFile)
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _FormatExpiryDate
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatPrice formats the Fee amount and filters out null-values
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="price">The price to format</param>
        '''	<returns>The correctly formatted price</returns>
        ''' <history>
        ''' 	[cnurse]	01/18/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatPrice(ByVal price As Single, ByVal period As Integer, ByVal frequency As String) As String
            Dim _FormatPrice As String = Null.NullString
            Try
                Select Case frequency
                    Case "N", ""
                        _FormatPrice = Localization.GetString("NoFee", Me.LocalResourceFile)
                    Case "O"
                        _FormatPrice = FormatPrice(price)
                    Case Else
                        _FormatPrice = String.Format(Localization.GetString("Fee", Me.LocalResourceFile), FormatPrice(price), period, Localization.GetString("Frequency_" + frequency, Me.LocalResourceFile))
                End Select
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _FormatPrice
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatTrial formats the Trial Fee amount and filters out null-values
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="price">The price to format</param>
        '''	<returns>The correctly formatted price</returns>
        ''' <history>
        ''' 	[cnurse]	03/28/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatTrial(ByVal price As Single, ByVal period As Integer, ByVal frequency As String) As String
            Dim _FormatTrial As String = Null.NullString
            Try
                Select Case frequency
                    Case "N", ""
                        _FormatTrial = Localization.GetString("NoFee", Me.LocalResourceFile)
                    Case "O"
                        _FormatTrial = FormatPrice(price)
                    Case Else
                        _FormatTrial = String.Format(Localization.GetString("TrialFee", Me.LocalResourceFile), FormatPrice(price), period, Localization.GetString("Frequency_" + frequency, Me.LocalResourceFile))
                End Select
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _FormatTrial
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatURL correctly formats a URL
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<returns>The correctly formatted url</returns>
        ''' <history>
        ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatURL() As String
            Dim _FormatURL As String = Null.NullString
            Try
                Dim strServerPath As String

                strServerPath = Request.ApplicationPath
                If Not strServerPath.EndsWith("/") Then
                    strServerPath += "/"
                End If

                _FormatURL = strServerPath & "Register.aspx?tabid=" & TabId
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _FormatURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ServiceText gets the Service Text (Cancel or Subscribe)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="Subscribed">The service state</param>
        '''	<returns>The correctly formatted text</returns>
        ''' <history>
        ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function ServiceText(ByVal Subscribed As Boolean, ByVal expiryDate As DateTime) As String
            Dim _ServiceText As String = Null.NullString
            Try
                If Not Subscribed Then
                    _ServiceText = Localization.GetString("Subscribe", Me.LocalResourceFile)
                Else
                    _ServiceText = Localization.GetString("Unsubscribe", Me.LocalResourceFile)
                    If Not Null.IsNull(expiryDate) Then
                        If expiryDate < Date.Today Then
                            _ServiceText = Localization.GetString("Renew", Me.LocalResourceFile)
                        End If
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            Return _ServiceText
        End Function

        Protected Function ShowSubscribe(ByVal roleID As Integer) As Boolean
            Dim objRoles As New RoleController
            Dim _ShowSubscribe As Boolean = Null.NullBoolean
            Dim objRole As RoleInfo = objRoles.GetRole(roleID, PortalSettings.PortalId)

            If objRole.IsPublic Then
                Dim objPortals As New PortalController
                Dim objPortal As PortalInfo = objPortals.GetPortal(PortalSettings.PortalId)
                If objRole.ServiceFee = 0.0 Then
                    _ShowSubscribe = True
                ElseIf objPortal IsNot Nothing AndAlso Not String.IsNullOrEmpty(objPortal.ProcessorUserId) Then
                    _ShowSubscribe = True
                End If
            End If

            Return _ShowSubscribe
        End Function

        Protected Function ShowTrial(ByVal roleID As Integer) As Boolean
            Dim objRoles As New RoleController
            Dim _ShowTrial As Boolean = Null.NullBoolean
            Dim objRole As RoleInfo = objRoles.GetRole(roleID, PortalSettings.PortalId)

            If objRole.TrialFrequency = "N" OrElse (objRole.IsPublic AndAlso objRole.ServiceFee = 0.0) Then
                _ShowTrial = Null.NullBoolean
            ElseIf objRole.IsPublic And objRole.TrialFee = 0.0 Then
                'Use Trial?
                Dim objUserRole As UserRoleInfo = objRoles.GetUserRole(PortalId, UserInfo.UserID, roleID)

                If (objUserRole Is Nothing) OrElse (Not objUserRole.IsTrialUsed) Then
                    _ShowTrial = True
                End If
            End If

            Return _ShowTrial
        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/13/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            If Request.IsAuthenticated Then
                grdServices.DataSource = GetRoles(PortalId, UserInfo.UserID)
                grdServices.DataBind()

                ' if no service available then hide options
                ServicesRow.Visible = (grdServices.Items.Count > 0)
            End If

        End Sub

#End Region

#Region "Event Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Raises the SubscriptionUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/17/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub OnSubscriptionUpdated(ByVal e As SubscriptionUpdatedEventArgs)
            RaiseEvent SubscriptionUpdated(Me, e)
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
        ''' 	[cnurse]	03/13/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                lblRSVP.Text = ""

                ' If this is the first visit to the page, localize the datalist
                If Page.IsPostBack = False Then
                    'Localize the Headers
                    Localization.LocalizeDataGrid(grdServices, Me.LocalResourceFile)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRSVP_Click runs when the Subscribe to RSVP Code Roles Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/19/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRSVP_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRSVP.Click

            'Get the RSVP code
            Dim code As String = txtRSVPCode.Text
            Dim blnRSVPCodeExists As Boolean = False

            If code <> "" Then
                'Get the roles from the Database
                Dim objRoles As New RoleController
                Dim objRole As RoleInfo
                Dim arrRoles As ArrayList = objRoles.GetPortalRoles(PortalSettings.PortalId)

                'Parse the roles
                For Each objRole In arrRoles
                    If objRole.RSVPCode = code Then
                        'Subscribe User to Role
                        objRoles.UpdateUserRole(PortalId, UserInfo.UserID, objRole.RoleID)
                        blnRSVPCodeExists = True

                        'Raise SubscriptionUpdated Event
                        OnSubscriptionUpdated(New SubscriptionUpdatedEventArgs(False, objRole.RoleName))
                    End If
                Next

                If blnRSVPCodeExists Then
                    lblRSVP.Text = Localization.GetString("RSVPSuccess", Me.LocalResourceFile)
                    'Reset RSVP Code field
                    txtRSVPCode.Text = ""
                Else
                    lblRSVP.Text = Localization.GetString("RSVPFailure", Me.LocalResourceFile)
                End If
            End If

            DataBind()

        End Sub

        Protected Sub grdServices_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdServices.ItemCommand
            Dim commandName As String = e.CommandName
            Dim roleID As Integer = CType(e.CommandArgument, Integer)

            Select Case commandName
                Case Localization.GetString("Subscribe", Me.LocalResourceFile), Localization.GetString("Renew", Me.LocalResourceFile)
                    'Subscribe
                    Subscribe(roleID, False)
                Case Localization.GetString("Unsubscribe", Me.LocalResourceFile)
                    'Unsubscribe
                    Subscribe(roleID, True)
                Case Localization.GetString("Unsubscribe", Me.LocalResourceFile)
                    'Unsubscribe
                    Subscribe(roleID, True)
                Case "UseTrial"
                    'Use Trial
                    UseTrial(roleID)
            End Select

            'Rebind Grid
            DataBind()
        End Sub

#End Region

#Region "Event Args"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The SubscriptionUpdatedEventArgs class provides a customised EventArgs class for
        ''' the SubscriptionUpdated Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/17/2006  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class SubscriptionUpdatedEventArgs

            Private _cancel As Boolean
            Private _roleName As String

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Constructs a new SubscriptionUpdatedEventArgs
            ''' </summary>
            ''' <param name="cancel">Whether this is a subscription cancellation</param>
            ''' <history>
            ''' 	[cnurse]	01/17/2006  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal cancel As Boolean, ByVal roleName As String)
                _cancel = cancel
                _roleName = roleName
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets whether this was a cancelation
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	01/17/2006  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Cancel() As Boolean
                Get
                    Return _cancel
                End Get
                Set(ByVal Value As Boolean)
                    _cancel = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Gets and sets the RoleName that was (un)subscribed to
            ''' </summary>
            ''' <history>
            ''' 	[cnurse]	01/17/2006  created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property RoleName() As String
                Get
                    Return _roleName
                End Get
                Set(ByVal Value As String)
                    _roleName = Value
                End Set
            End Property

        End Class

#End Region

    End Class

End Namespace
