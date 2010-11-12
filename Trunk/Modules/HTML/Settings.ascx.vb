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
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Settings ModuleSettingsBase is used to manage the 
    ''' settings for the HTML Module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[leupold]	    08/12/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class Settings
        Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region "Event handling"

        Protected Sub cboWorkflow_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboWorkflow.SelectedIndexChanged
            DisplayWorkflowDetails()
        End Sub
#End Region

#Region "Private methods"

        Private Sub DisplayWorkflowDetails()
            If Not cboWorkflow.SelectedValue Is Nothing Then
                Dim objWorkflow As New WorkflowStateController
                Dim strDescription As String = ""
                Dim arrStates As ArrayList = objWorkflow.GetWorkflowStates(Integer.Parse(cboWorkflow.SelectedValue))
                If arrStates.Count > 0 Then
                    For Each objState As WorkflowStateInfo In arrStates
                        strDescription = strDescription & " >> " & "<span class=""NormalBold"">" & objState.StateName & "</span>"
                    Next
                    strDescription = strDescription & "<br />" & CType(arrStates(0), WorkflowStateInfo).Description
                End If
                lblDescription.Text = strDescription
            End If
        End Sub

#End Region

#Region "Private Functions"
#End Region

#Region "Base Method Implementations"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSettings loads the settings from the Database and displays them
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub LoadSettings()
            Try
                If Not Page.IsPostBack Then
                    Dim _htmlTextController As New HtmlTextController
                    Dim _workflowStateController As New WorkflowStateController

                    ' get replace token settings
                    If CType(ModuleSettings("HtmlText_ReplaceTokens"), String) <> "" Then
                        chkReplaceTokens.Checked = CType(ModuleSettings("HtmlText_ReplaceTokens"), Boolean)
                    End If

                    ' get workflow/version settings
                    Dim arrWorkflows As New ArrayList
                    For Each objState As WorkflowStateInfo In _workflowStateController.GetWorkflows(PortalId)
                        If Not objState.IsDeleted Then
                            arrWorkflows.Add(objState)
                        End If
                    Next

                    cboWorkflow.DataSource = arrWorkflows
                    cboWorkflow.DataBind()

                    Dim workflow As KeyValuePair(Of String, Integer) = _htmlTextController.GetWorkflow(ModuleId, TabId, PortalId)
                    If Not cboWorkflow.Items.FindByValue(workflow.Value.ToString()) Is Nothing Then
                        cboWorkflow.Items.FindByValue(workflow.Value.ToString()).Selected = True
                    End If

                    DisplayWorkflowDetails()

                    If Not rblApplyTo.Items.FindByValue(workflow.Key) Is Nothing Then
                        rblApplyTo.Items.FindByValue(workflow.Key).Selected = True
                    End If

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


        ''' <summary>
        ''' Updates the module settings.
        ''' </summary>
        Public Overrides Sub UpdateSettings()
            Try

                Dim _htmlTextController As New HtmlTextController
                Dim objWorkflow As New WorkflowStateController

                ' update replace token setting
                Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
                objModules.UpdateModuleSetting(ModuleId, "HtmlText_ReplaceTokens", chkReplaceTokens.Checked.ToString)

                ' disable module caching if token replace is enabled
                If chkReplaceTokens.Checked Then
                    Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = objModules.GetModule(ModuleId, TabId, False)
                    If objModule.CacheTime > 0 Then
                        objModule.CacheTime = 0
                        objModules.UpdateModule(objModule)
                    End If
                End If

                ' update workflow/version settings
                Select Case rblApplyTo.SelectedValue
                    Case "Module"
                        _htmlTextController.UpdateWorkflow(ModuleId, rblApplyTo.SelectedValue, Integer.Parse(cboWorkflow.SelectedValue), chkReplace.Checked)
                    Case "Page"
                        _htmlTextController.UpdateWorkflow(TabId, rblApplyTo.SelectedValue, Integer.Parse(cboWorkflow.SelectedValue), chkReplace.Checked)
                    Case "Site"
                        _htmlTextController.UpdateWorkflow(PortalId, rblApplyTo.SelectedValue, Integer.Parse(cboWorkflow.SelectedValue), chkReplace.Checked)
                End Select

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace


