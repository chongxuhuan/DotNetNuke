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

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Exceptions

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' <summary>
	''' The EditHtml PortalModuleBase is used to manage Html
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
    ''' </history>
	''' -----------------------------------------------------------------------------
	Public Partial Class EditHtml
		Inherits Entities.Modules.PortalModuleBase

        Private WorkflowID As Integer = -1
        Private ItemID As Integer = -1

#Region "Private Methods"

        Private Function FormatContent(ByVal objContent As HtmlTextInfo) As String
            Dim strContent As String = HttpUtility.HtmlDecode(objContent.Content)
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "src", PortalId)
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "background", PortalId)
            Return HttpUtility.HtmlEncode(strContent)
        End Function

        Private Sub DisplayHistory(ByVal objContent As HtmlTextInfo)
            Dim objLog As New HtmlTextLogController
            lblVersion.Text = objContent.Version.ToString()
            lblWorkflow.Text = Localization.GetString(objContent.WorkflowName, Me.LocalResourceFile)
            lblState.Text = Localization.GetString(objContent.StateName, Me.LocalResourceFile)
            grdLog.DataSource = objLog.GetHtmlTextLog(objContent.ItemID)
            grdLog.DataBind()
        End Sub

        Private Sub DisplayPreview(ByVal strContent As String)
            lblPreview.Controls.Add(New LiteralControl(HtmlTextController.FormatHtmlText(ModuleId, strContent, Settings)))
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialized
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            For Each column As DataGridColumn In grdVersions.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    ' localize image column text
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If
                End If
            Next

            ' localize datagrids
            DotNetNuke.Services.Localization.Localization.LocalizeDataGrid(grdLog, Me.LocalResourceFile)
            DotNetNuke.Services.Localization.Localization.LocalizeDataGrid(grdVersions, Me.LocalResourceFile)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try
                Dim objHTML As New HtmlTextController

                WorkflowID = objHTML.GetWorkflow(ModuleId, TabId, PortalId).Value

                ' get content
                Dim objContent As HtmlTextInfo = objHTML.GetTopHtmlText(ModuleId, False, WorkflowID)
                If Not objContent Is Nothing Then
                    ItemID = objContent.ItemID
                End If

                ' check review security
                If Request.QueryString("ctl").ToUpper = "REVIEW" AndAlso WorkflowStatePermissionController.HasWorkflowStatePermission(WorkflowStatePermissionController.GetWorkflowStatePermissions(objContent.StateID), "REVIEW") = False Then
                    Response.Redirect(AccessDeniedURL(), True)
                End If

                If Not Page.IsPostBack Then

                    ' load states for workflow
                    Dim objWorkflow As New WorkflowStateController
                    Dim arrStates As ArrayList = objWorkflow.GetWorkflowStates(WorkflowID)

                    ' load versions
                    grdVersions.DataSource = objHTML.GetAllHtmlText(ModuleId)
                    grdVersions.DataBind()

                    If ItemID <> -1 Then
                        ' load content
                        txtContent.Text = FormatContent(objContent)
                        DisplayHistory(objContent)
                        DisplayPreview(objContent.Content)

                        'Get master language
                        Dim objModule As ModuleInfo = New ModuleController().GetModule(ModuleId, TabId)
                        If objModule.DefaultLanguageModule IsNot Nothing Then
                            Dim masterContent As HtmlTextInfo = objHTML.GetTopHtmlText(objModule.DefaultLanguageModule.ModuleID, False, WorkflowID)
                            If masterContent IsNot Nothing Then
                                lblMaster.Controls.Add(New LiteralControl(HtmlTextController.FormatHtmlText(objModule.DefaultLanguageModule.ModuleID, FormatContent(masterContent), Settings)))
                            End If
                        End If
                        dshMaster.Visible = objModule.DefaultLanguageModule IsNot Nothing
                        tblMaster.Visible = objModule.DefaultLanguageModule IsNot Nothing
                    Else
                        ' initialize content
                        txtContent.Text = Localization.GetString("AddContent", LocalResourceFile)
                        lblWorkflow.Text = CType(arrStates(0), WorkflowStateInfo).WorkflowName
                        dshVersions.Visible = False
                        tblVersions.Visible = False
                        Select Case arrStates.Count
                            Case 0, 1
                                plState.Visible = False
                                lblState.Visible = False
                            Case Else
                                lblState.Text = CType(arrStates(0), WorkflowStateInfo).StateName
                        End Select
                    End If

                    ' direct publish workflows do not need the Publish checkbox option
                    If arrStates.Count = 1 Then
                        rowPublish.Visible = False
                    Else
                        rowPublish.Visible = True
                    End If

                    ' handle Edit and Review actions
                    Select Case Request.QueryString("ctl").ToUpper
                        Case "EDIT"
                            dshReview.Visible = False
                            tblReview.Visible = False
                        Case "REVIEW"
                            tblEdit.Visible = False
                            dshVersions.Visible = False
                            tblVersions.Visible = False
                            dshPreview.IsExpanded = True
                            If Request.QueryString("action").ToUpper = "APPROVE" Then
                                cmdReview.Text = Localization.GetString("ApproveContent.Action", LocalResourceFile)
                            Else
                                cmdReview.Text = Localization.GetString("RejectContent.Action", LocalResourceFile)
                            End If
                    End Select

                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click, cmdCancel2.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdSave_Click runs when the Save button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
            Try
                ' get content
                Dim objHTML As HtmlTextController = New HtmlTextController
                Dim objWorkflow As New WorkflowStateController
                Dim objContent As HtmlTextInfo = objHTML.GetTopHtmlText(ModuleId, False, WorkflowID)
                If objContent Is Nothing Then
                    objContent = New HtmlTextInfo
                    objContent.ItemID = -1
                End If

                ' set content attributes
                objContent.ModuleID = ModuleId
                objContent.Content = txtContent.Text
                objContent.WorkflowID = WorkflowID
                objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID)

                ' publish content
                If rowPublish.Visible = True AndAlso chkPublish.Checked Then
                    objContent.StateID = objWorkflow.GetNextWorkflowStateID(WorkflowID, objContent.StateID)
                End If

                ' save content
                objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))

                ' redirect back to portal
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdReview_Click runs when the Review button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdReview_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReview.Click
            Try
                Dim blnIsApproved As Boolean = (Request.QueryString("action").ToUpper = "APPROVE")

                ' approve content
                Dim objHTML As New HtmlTextController
                Dim objWorkflow As New WorkflowStateController
                Dim objContent As HtmlTextInfo = objHTML.GetHtmlText(ModuleId, ItemID)

                ' if the user has permissions to review the content
                If WorkflowStatePermissionController.HasWorkflowStatePermission(WorkflowStatePermissionController.GetWorkflowStatePermissions(objContent.StateID), "REVIEW") Then
                    If blnIsApproved Then
                        ' promote to next state
                        objContent.ModuleID = ModuleId
                        objContent.WorkflowID = WorkflowID
                        objContent.StateID = objWorkflow.GetNextWorkflowStateID(WorkflowID, objContent.StateID)
                        objContent.Comment = txtComment.Text
                        objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))

                        ' redirect back to portal
                        Response.Redirect(NavigateURL(), True)
                    Else
                        ' rejections must have a comment
                        If txtComment.Text <> "" Then
                            ' reset to first state
                            objContent.ModuleID = ModuleId
                            objContent.WorkflowID = WorkflowID
                            objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID)
                            objContent.Comment = txtComment.Text
                            objContent.Approved = False
                            objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))

                            ' redirect back to portal
                            Response.Redirect(NavigateURL(), True)
                        Else
                            UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("RejectMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If
                    End If
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdPreview_Click runs when the Preview button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdPreview_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPreview.Click
            Try
                DisplayPreview(txtContent.Text)
                dshPreview.IsExpanded = True
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdLog_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdLog.ItemDataBound
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                'Localize columns
                item.Cells(2).Text = Localization.GetString(item.Cells(2).Text, Me.LocalResourceFile)
                item.Cells(3).Text = Localization.GetString(item.Cells(3).Text, Me.LocalResourceFile)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdVersions_ItemCommand is executed when the Preview button is selected in the Versions datagrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdVersions_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdVersions.ItemCommand
            Try
                Dim objHTML As New HtmlTextController
                Dim objContent As HtmlTextInfo = objHTML.GetHtmlText(ModuleId, Integer.Parse(e.CommandArgument.ToString))
                DisplayHistory(objContent)
                dshHistory.IsExpanded = True
                DisplayPreview(objContent.Content)
                dshPreview.IsExpanded = True
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' grdVersions_EditCommand is executed when the Rollback button is selected in the Versions datagrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdVersions_EditCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdVersions.EditCommand
            Try
                Dim objHTML As New HtmlTextController
                Dim objWorkflow As New WorkflowStateController

                Dim objContent As HtmlTextInfo = objHTML.GetHtmlText(ModuleId, Integer.Parse(e.CommandArgument.ToString))
                objContent.ModuleID = ModuleId
                objContent.WorkflowID = WorkflowID
                objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID)
                objContent.StateID = objWorkflow.GetNextWorkflowStateID(WorkflowID, objContent.StateID)
                objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))

                ' redirect back to portal
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdVersions_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdVersions.ItemDataBound

            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim objColumnControl As Control
                Dim objImage As ImageButton

                objColumnControl = item.Controls(1).Controls(0)
                If TypeOf objColumnControl Is ImageButton Then
                    objImage = CType(objColumnControl, ImageButton)
                    objImage.Visible = (item.ItemIndex <> 0)
                End If

                'Localize columns
                item.Cells(5).Text = Localization.GetString(item.Cells(5).Text, Me.LocalResourceFile)

            End If

        End Sub

#End Region

    End Class

End Namespace
