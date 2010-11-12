'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'


Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    Partial Public Class WorkflowStateController

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddWorkFlow creates a new workflow for the portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <param name="WorkflowName">The Name of the Workflow</param>
        ''' <param name="Description">The Description of the Workflow</param>
        ''' <param name="IsDeleted">Whether or not the Workflow is Active</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AddWorkflow(ByVal PortalID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean) As Integer
            Return CType(DataProvider.Instance().AddWorkflow(PortalID, WorkflowName, Description, IsDeleted), Integer)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddWorkFlowState creates a new workflow and state instance
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objWorkflowState">A WorkflowStateInfo object</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function AddWorkflowState(ByVal objWorkflowState As WorkflowStateInfo) As Integer
            Dim StateID As Integer = CType(DataProvider.Instance().AddWorkflowState(objWorkflowState.WorkflowID, objWorkflowState.StateName, objWorkflowState.Order, objWorkflowState.Notify, objWorkflowState.IsActive), Integer)
            DataCache.RemoveCache(String.Format(WORKFLOW_CACHE_KEY, objWorkflowState.WorkflowID.ToString))
            Return StateID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CanDeleteWorkFlowState determines whether a workflow state is in use and can be deleted
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="StateID">A WorkflowState ID</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CanDeleteWorkFlowState(ByVal StateID As Integer) As Boolean
            Return DataProvider.Instance().CanDeleteWorkFlowState(StateID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteWorkFlowState deletes a State instance
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="StateID">The StateID</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteWorkflowState(ByVal StateID As Integer, ByVal WorkflowID As Integer)
            DataProvider.Instance().DeleteWorkflowState(StateID)
            DataCache.RemoveCache(String.Format(WORKFLOW_CACHE_KEY, WorkflowID.ToString))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlow retrieves a Workflow object
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflow(ByVal WorkflowID As Integer) As WorkflowStateInfo
            Return CBO.FillObject(DataProvider.Instance().GetWorkflow(WorkflowID), GetType(WorkflowStateInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlowState retrieves a WorkflowStateInfo object for the Workflow and State
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="StateID">The ID of the State</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflowState(ByVal StateID As Integer) As WorkflowStateInfo
            Return CBO.FillObject(DataProvider.Instance().GetWorkflowState(StateID), GetType(WorkflowStateInfo))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateWorkFlow updates an existing workflow for the portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <param name="WorkflowName">The Name of the Workflow</param>
        ''' <param name="Description">The Description of the Workflow</param>
        ''' <param name="IsDeleted">Whether or not the Workflow is Active</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateWorkflow(ByVal WorkflowID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean)
            DataProvider.Instance().UpdateWorkflow(WorkflowID, WorkflowName, Description, IsDeleted)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddWorkFlowState creates a new workflow and state instance
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objWorkflowState">A WorkflowStateInfo object</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateWorkflowState(ByVal objWorkflowState As WorkflowStateInfo)
            DataProvider.Instance().UpdateWorkflowState(objWorkflowState.StateID, objWorkflowState.StateName, objWorkflowState.Order, objWorkflowState.Notify, objWorkflowState.IsActive)
            DataCache.RemoveCache(String.Format(WORKFLOW_CACHE_KEY, objWorkflowState.WorkflowID.ToString))
        End Sub

#End Region

    End Class

End Namespace

