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

        Protected Sub cboWorkflow_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboWorkflow.SelectedIndexChanged
            DisplayWorkflow()
        End Sub

        Private Sub DisplayWorkflow()
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

#Region "Base Method Implementations"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSettings loads the settings from the Database and displays them
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub LoadSettings()
            Try
                If Not Page.IsPostBack Then
                    Dim objHtml As New HtmlTextController
                    Dim objWorkflow As New WorkflowStateController

                    ' get replace token settings
                    If CType(ModuleSettings("HtmlText_ReplaceTokens"), String) <> "" Then
                        chkReplaceTokens.Checked = CType(ModuleSettings("HtmlText_ReplaceTokens"), Boolean)
                    End If

                    ' expose workflow management option if available and user is administrator
                    If Not ModuleControlController.GetModuleControlByControlKey("WorkFlow", ModuleConfiguration.ModuleDefID) Is Nothing Then
                        If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                            rowWorkflow.Visible = True
                        End If
                    End If

                    ' get workflow/version settings
                    Dim arrWorkflows As New ArrayList
                    For Each objState As WorkflowStateInfo In objWorkflow.GetWorkflows(PortalId)
                        If Not objState.IsDeleted Then
                            arrWorkflows.Add(objState)
                        End If
                    Next
                    cboWorkflow.DataSource = arrWorkflows
                    cboWorkflow.DataBind()
                    Dim intWorkflowID As Integer = objHtml.GetWorkflowID(ModuleId, PortalId)
                    If Not cboWorkflow.Items.FindByValue(intWorkflowID.ToString) Is Nothing Then
                        cboWorkflow.Items.FindByValue(intWorkflowID.ToString).Selected = True
                    End If
                    DisplayWorkflow()

                    ' expose default option if user is administrator
                    If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                        rowDefault.Visible = True
                    End If

                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateSettings saves the modified settings to the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UpdateSettings()
            Try

                Dim objHtml As New HtmlTextController
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
                If chkDefault.Checked Then
                    If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                        objHtml.UpdateWorkflowID(Null.NullInteger, PortalId, Integer.Parse(cboWorkflow.SelectedValue))
                    End If
                Else
                    objHtml.UpdateWorkflowID(ModuleId, PortalId, Integer.Parse(cboWorkflow.SelectedValue))
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdWorkflow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdWorkflow.Click
            If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Then
                Response.Redirect(EditUrl("Workflow"))
            End If
        End Sub

#End Region

    End Class

End Namespace


