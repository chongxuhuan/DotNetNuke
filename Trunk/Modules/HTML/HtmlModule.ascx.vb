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

Imports DotNetNuke.Common
Imports System.Web.UI
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The HtmlModule Class provides the UI for displaying the Html
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class HtmlModule
        Inherits Entities.Modules.PortalModuleBase
        Implements Entities.Modules.IActionable

        Private WorkflowID As Integer
        Private EditorEnabled As Boolean = PortalSettings.InlineEditorEnabled

#Region "Private Methods"

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
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Try
                Dim objHtml As New HtmlTextController
                WorkflowID = objHtml.GetWorkflowID(ModuleId, PortalId)

                'Add an Action Event Handler to the Skin
                AddActionHandler(AddressOf ModuleAction_Click)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
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
                Dim objWorkflow As New WorkflowStateController

                ' edit in place
                If EditorEnabled = True AndAlso Me.IsEditable = True AndAlso PortalSettings.UserMode = DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit Then
                    EditorEnabled = True
                Else
                    EditorEnabled = False
                End If

                ' get content
                Dim objContent As HtmlTextInfo = Nothing
                Dim strContent As String = ""

                objContent = objHTML.GetTopHtmlText(ModuleId, Not Me.IsEditable, WorkflowID)

                If Not objContent Is Nothing Then
                    'don't decode yet (this is done in FormatHtmlText)
                    strContent = objContent.Content
                Else
                    ' get default content from resource file
                    If PortalSettings.UserMode = DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit Then
                        If EditorEnabled Then
                            strContent = Localization.GetString("AddContentFromToolBar.Text", LocalResourceFile)
                        Else
                            strContent = Localization.GetString("AddContentFromActionMenu.Text", LocalResourceFile)
                        End If
                    Else
                        ' hide the module if no content and in view mode
                        Me.ContainerControl.Visible = False
                    End If
                End If

                ' token replace
                If EditorEnabled AndAlso CType(Settings("HtmlText_ReplaceTokens"), String) <> "" Then
                    EditorEnabled = Not CType(Settings("HtmlText_ReplaceTokens"), Boolean)
                End If

                ' localize toolbar
                If EditorEnabled Then
                    For Each objButton As DotNetNuke.UI.WebControls.DNNToolBarButton In Me.tbEIPHTML.Buttons
                        objButton.ToolTip = Services.Localization.Localization.GetString("cmd" & objButton.ToolTip, LocalResourceFile)
                    Next
                Else
                    Me.tbEIPHTML.Visible = False
                End If

                lblContent.EditEnabled = EditorEnabled

                ' add content to module
                lblContent.Controls.Add(New LiteralControl(HtmlTextController.FormatHtmlText(ModuleId, strContent, Settings)))

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' lblContent_UpdateLabel allows for inline editing of content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lblContent_UpdateLabel(ByVal source As Object, ByVal e As UI.WebControls.DNNLabelEditEventArgs) Handles lblContent.UpdateLabel
            Try
                ' verify security 
                If (Not New PortalSecurity().InputFilter(e.Text, PortalSecurity.FilterFlag.NoScripting).Equals(e.Text)) Then
                    Throw New SecurityException()
                ElseIf EditorEnabled = True AndAlso Me.IsEditable = True AndAlso PortalSettings.UserMode = DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit Then

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
                    objContent.Content = Server.HtmlEncode(e.Text)
                    objContent.WorkflowID = WorkflowID
                    objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID)

                    ' save the content
                    objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))
                Else
                    Throw New SecurityException()
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleAction_Click handles all ModuleAction events raised from the action menu
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ModuleAction_Click(ByVal sender As Object, ByVal e As Entities.Modules.Actions.ActionEventArgs)
            Try
                If e.Action.CommandArgument = "publish" Then
                    ' verify security 
                    If Me.IsEditable = True AndAlso PortalSettings.UserMode = DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit Then
                        ' get content
                        Dim objHTML As HtmlTextController = New HtmlTextController
                        Dim objContent As HtmlTextInfo = objHTML.GetTopHtmlText(ModuleId, False, WorkflowID)

                        Dim objWorkflow As New WorkflowStateController
                        If objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID) Then
                            ' publish content
                            objContent.StateID = objWorkflow.GetNextWorkflowStateID(objContent.WorkflowID, objContent.StateID)

                            ' save the content
                            objHTML.UpdateHtmlText(objContent, objHTML.GetMaximumVersionHistory(PortalId))

                            ' refresh page
                            Response.Redirect(NavigateURL(), True)
                        End If
                    End If
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Optional Interfaces"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleActions is an interface property that returns the module actions collection for the module
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                ' add the Edit Text action
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                Actions.Add(GetNextActionID, Localization.GetString(Entities.Modules.Actions.ModuleActionType.AddContent, LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "", EditUrl(), False, Security.SecurityAccessLevel.Edit, True, False)

                ' get the content
                Dim objHTML As New HtmlTextController
                Dim objWorkflow As New WorkflowStateController
                WorkflowID = objHTML.GetWorkflowID(ModuleId, PortalId)

                Dim objContent As HtmlTextInfo = objHTML.GetTopHtmlText(ModuleId, False, WorkflowID)
                If Not objContent Is Nothing Then
                    ' if content is in the first state
                    If objContent.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID) Then
                        ' if not direct publish workflow
                        If objWorkflow.GetWorkflowStates(WorkflowID).Count > 1 Then
                            ' add publish action
                            Actions.Add(GetNextActionID, Localization.GetString("PublishContent.Action", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "publish", "grant.gif", "", True, Security.SecurityAccessLevel.Edit, True, False)
                        End If
                    Else
                        ' if the content is not in the last state of the workflow then review is required
                        If objContent.StateID <> objWorkflow.GetLastWorkflowStateID(WorkflowID) Then
                            ' if the user has permissions to review the content
                            If WorkflowStatePermissionController.HasWorkflowStatePermission(WorkflowStatePermissionController.GetWorkflowStatePermissions(objContent.StateID), "REVIEW") Then
                                ' add approve and reject actions
                                Actions.Add(GetNextActionID, Localization.GetString("ApproveContent.Action", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "grant.gif", EditUrl("action", "approve", "Review"), False, Security.SecurityAccessLevel.Edit, True, False)
                                Actions.Add(GetNextActionID, Localization.GetString("RejectContent.Action", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.AddContent, "", "deny.gif", EditUrl("action", "reject", "Review"), False, Security.SecurityAccessLevel.Edit, True, False)
                            End If
                        End If
                    End If
                End If

                ' add mywork to action menu
                Actions.Add(GetNextActionID, Localization.GetString("MyWork.Action", LocalResourceFile), "MyWork.Action", "", "view.gif", EditUrl("MyWork"), False, Security.SecurityAccessLevel.Edit, True, False)

                Return Actions

            End Get
        End Property

#End Region

    End Class

End Namespace

