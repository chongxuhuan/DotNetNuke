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

Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.Modules.Admin.LogViewer

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : EditLogTypes
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manage the Log Types for the portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''   [cnurse] 17/9/2004  Updated for localization, Help and 508. 
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditLogTypes

        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

#Region "Controls"

        'DataGrid List

        'Logging Settings

        'Email Notification Settings
        Protected plThresholdNotificationTime As UI.UserControls.LabelControl
        Protected plThresholdNotificationTimeType As UI.UserControls.LabelControl

        'tasks

#End Region

#Region "Private Methods"

        Private Sub BindDetailData()
            Dim pc As New PortalController
            ddlLogTypePortalID.DataTextField = "PortalName"
            ddlLogTypePortalID.DataValueField = "PortalID"
            ddlLogTypePortalID.DataSource = pc.GetPortals()
            ddlLogTypePortalID.DataBind()

            Dim i As New ListItem
            i.Text = Services.Localization.Localization.GetString("All")
            i.Value = "*"
            ddlLogTypePortalID.Items.Insert(0, i)


            pnlEditLogTypeConfigInfo.Visible = True
            pnlLogTypeConfigInfo.Visible = False
            Dim l As New LogController

            Dim arrLogTypeInfo As ArrayList
            arrLogTypeInfo = l.GetLogTypeInfo()

            arrLogTypeInfo.Sort(New LogTypeSortFriendlyName)


            ddlLogTypeKey.DataTextField = "LogTypeFriendlyName"
            ddlLogTypeKey.DataValueField = "LogTypeKey"
            ddlLogTypeKey.DataSource = arrLogTypeInfo
            ddlLogTypeKey.DataBind()

            Dim items() As Integer = {1, 2, 3, 4, 5, 10, 25, 100, 250, 500}
            ddlKeepMostRecent.Items.Clear()
            ddlKeepMostRecent.Items.Add(New ListItem(Services.Localization.Localization.GetString("All"), "*"))
            For Each item As Integer In items
                If item = 1 Then
                    ddlKeepMostRecent.Items.Add(New ListItem(item.ToString & Services.Localization.Localization.GetString("LogEntry", Me.LocalResourceFile), item.ToString))
                Else
                    ddlKeepMostRecent.Items.Add(New ListItem(item.ToString & Services.Localization.Localization.GetString("LogEntries", Me.LocalResourceFile), item.ToString))
                End If
            Next

            Dim items2() As Integer = {1, 2, 3, 4, 5, 10, 25, 100, 250, 500, 1000}
            ddlThreshold.Items.Clear()
            For Each item As Integer In items2
                If item = 1 Then
                    ddlThreshold.Items.Add(New ListItem(item.ToString & Services.Localization.Localization.GetString("Occurence", Me.LocalResourceFile), item.ToString))
                Else
                    ddlThreshold.Items.Add(New ListItem(item.ToString & Services.Localization.Localization.GetString("Occurences", Me.LocalResourceFile), item.ToString))
                End If
            Next

            Dim j As New ListItem
            j.Text = Services.Localization.Localization.GetString("All")
            j.Value = "*"
            ddlLogTypeKey.Items.Insert(0, j)
        End Sub

        Private Sub BindSummaryData()
            Dim objLogController As New LogController
            Dim arrLogTypeConfigInfo As ArrayList = objLogController.GetLogTypeConfigInfo()

            'Localize the Headers
            If Not Page.IsPostBack Then
                Services.Localization.Localization.LocalizeDataGrid(dgLogTypeConfigInfo, Me.LocalResourceFile)
            End If

            dgLogTypeConfigInfo.DataSource() = arrLogTypeConfigInfo
            dgLogTypeConfigInfo.DataBind()
            pnlEditLogTypeConfigInfo.Visible = False
            pnlLogTypeConfigInfo.Visible = True
        End Sub

        Private Sub DisableLoggingControls()
            If chkIsActive.Checked = True Then
                ddlLogTypeKey.Enabled = True
                ddlLogTypePortalID.Enabled = True
                ddlKeepMostRecent.Enabled = True
                txtFileName.Enabled = True
            Else
                ddlLogTypeKey.Enabled = False
                ddlLogTypePortalID.Enabled = False
                ddlKeepMostRecent.Enabled = False
                txtFileName.Enabled = False
            End If

        End Sub

        Private Sub DisableNotificationControls()
            If chkEmailNotificationStatus.Checked = True Then
                ddlThreshold.Enabled = True
                ddlThresholdNotificationTime.Enabled = True
                ddlThresholdNotificationTimeType.Enabled = True
                txtMailFromAddress.Enabled = True
                txtMailToAddress.Enabled = True
            Else
                ddlThreshold.Enabled = False
                ddlThresholdNotificationTime.Enabled = False
                ddlThresholdNotificationTimeType.Enabled = False
                txtMailFromAddress.Enabled = False
                txtMailToAddress.Enabled = False
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
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    If Request.QueryString("action") = "add" Then
                        BindDetailData()
                    Else
                        BindSummaryData()
                    End If
                End If


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
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                BindSummaryData()

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
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Dim objLogTypeConfigInfo As New LogTypeConfigInfo
            Dim l As New LogController
            objLogTypeConfigInfo.ID = Convert.ToString(ViewState("LogID"))
            Try
                l.DeleteLogTypeConfigInfo(objLogTypeConfigInfo)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("ConfigDeleted", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                BindSummaryData()
            Catch
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DeleteError", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/17/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim objLogTypeConfigInfo As New LogTypeConfigInfo
            objLogTypeConfigInfo.LoggingIsActive = chkIsActive.Checked
            objLogTypeConfigInfo.LogTypeKey = ddlLogTypeKey.SelectedItem.Value
            objLogTypeConfigInfo.LogTypePortalID = ddlLogTypePortalID.SelectedItem.Value
            objLogTypeConfigInfo.KeepMostRecent = ddlKeepMostRecent.SelectedItem.Value
            objLogTypeConfigInfo.LogFileName = txtFileName.Text

            objLogTypeConfigInfo.EmailNotificationIsActive = chkEmailNotificationStatus.Checked
            objLogTypeConfigInfo.NotificationThreshold = Convert.ToInt32(ddlThreshold.SelectedItem.Value)
            objLogTypeConfigInfo.NotificationThresholdTime = Convert.ToInt32(ddlThresholdNotificationTime.SelectedItem.Value)
            objLogTypeConfigInfo.NotificationThresholdTimeType = CType(ddlThresholdNotificationTimeType.SelectedItem.Value, LogTypeConfigInfo.NotificationThresholdTimeTypes)
            objLogTypeConfigInfo.MailFromAddress = txtMailFromAddress.Text
            objLogTypeConfigInfo.MailToAddress = txtMailToAddress.Text

            Dim l As New LogController

            If Not ViewState("LogID") Is Nothing Then
                objLogTypeConfigInfo.ID = Convert.ToString(ViewState("LogID"))
                l.UpdateLogTypeConfigInfo(objLogTypeConfigInfo)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("ConfigUpdated", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            Else
                objLogTypeConfigInfo.ID = Guid.NewGuid.ToString
                l.AddLogTypeConfigInfo(objLogTypeConfigInfo)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("ConfigAdded", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            End If

            BindSummaryData()

        End Sub

        Private Sub cmdReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReturn.Click
            Response.Redirect(NavigateURL(), True)
        End Sub

        Private Sub chkEmailNotificationStatus_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkEmailNotificationStatus.CheckedChanged
            DisableNotificationControls()
        End Sub

        Private Sub chkIsActive_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkIsActive.CheckedChanged
            DisableLoggingControls()
        End Sub

        Public Sub dgLogTypeConfigInfo_EditCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgLogTypeConfigInfo.EditCommand
            Dim LogID As String = Convert.ToString(dgLogTypeConfigInfo.DataKeys(e.Item.ItemIndex))
            ViewState("LogID") = LogID

            BindDetailData()

            Dim l As New LogController

            Dim objLogTypeConfigInfo As LogTypeConfigInfo = l.GetLogTypeConfigInfoByID(LogID)

            txtFileName.Text = objLogTypeConfigInfo.LogFileName
            chkIsActive.Checked = objLogTypeConfigInfo.LoggingIsActive
            chkEmailNotificationStatus.Checked = objLogTypeConfigInfo.EmailNotificationIsActive

            If Not ddlLogTypeKey.Items.FindByValue(objLogTypeConfigInfo.LogTypeKey) Is Nothing Then
                ddlLogTypeKey.ClearSelection()
                ddlLogTypeKey.Items.FindByValue(objLogTypeConfigInfo.LogTypeKey).Selected = True
            End If
            If Not ddlLogTypePortalID.Items.FindByValue(objLogTypeConfigInfo.LogTypePortalID) Is Nothing Then
                ddlLogTypePortalID.ClearSelection()
                ddlLogTypePortalID.Items.FindByValue(objLogTypeConfigInfo.LogTypePortalID).Selected = True
            End If
            If Not ddlKeepMostRecent.Items.FindByValue(objLogTypeConfigInfo.KeepMostRecent) Is Nothing Then
                ddlKeepMostRecent.ClearSelection()
                ddlKeepMostRecent.Items.FindByValue(objLogTypeConfigInfo.KeepMostRecent).Selected = True
            End If
            If Not ddlThreshold.Items.FindByValue(objLogTypeConfigInfo.NotificationThreshold.ToString) Is Nothing Then
                ddlThreshold.ClearSelection()
                ddlThreshold.Items.FindByValue(objLogTypeConfigInfo.NotificationThreshold.ToString).Selected = True
            End If
            If Not ddlThresholdNotificationTime.Items.FindByValue(objLogTypeConfigInfo.NotificationThresholdTime.ToString) Is Nothing Then
                ddlThresholdNotificationTime.ClearSelection()
                ddlThresholdNotificationTime.Items.FindByValue(objLogTypeConfigInfo.NotificationThresholdTime.ToString).Selected = True
            End If
            If Not ddlThresholdNotificationTimeType.Items.FindByText(objLogTypeConfigInfo.NotificationThresholdTimeType.ToString) Is Nothing Then
                ddlThresholdNotificationTimeType.ClearSelection()
                ddlThresholdNotificationTimeType.Items.FindByText(objLogTypeConfigInfo.NotificationThresholdTimeType.ToString).Selected = True
            End If
            txtMailFromAddress.Text = objLogTypeConfigInfo.MailFromAddress
            txtMailToAddress.Text = objLogTypeConfigInfo.MailToAddress

            DisableLoggingControls()

        End Sub

#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection
                Actions.Add(GetNextActionID, Services.Localization.Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl("action", "add"), False, SecurityAccessLevel.Admin, True, False)
                Return Actions
            End Get
        End Property
#End Region

    End Class

End Namespace