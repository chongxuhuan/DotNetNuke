#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;

using DotNetNuke.Common.Utilities;


namespace DotNetNuke.Modules.Html
{
    public partial class WorkflowStateController
    {
        #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddWorkFlow creates a new workflow for the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "PortalID">The ID of the Portal</param>
        /// <param name = "WorkflowName">The Name of the Workflow</param>
        /// <param name = "Description">The Description of the Workflow</param>
        /// <param name = "IsDeleted">Whether or not the Workflow is Active</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddWorkflow(int PortalID, string WorkflowName, string Description, bool IsDeleted)
        {
            return Convert.ToInt32(DataProvider.Instance().AddWorkflow(PortalID, WorkflowName, Description, IsDeleted));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddWorkFlowState creates a new workflow and state instance
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "objWorkflowState">A WorkflowStateInfo object</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public int AddWorkflowState(WorkflowStateInfo objWorkflowState)
        {
            int StateID =
                Convert.ToInt32(DataProvider.Instance().AddWorkflowState(objWorkflowState.WorkflowID,
                                                                         objWorkflowState.StateName,
                                                                         objWorkflowState.Order,
                                                                         objWorkflowState.Notify,
                                                                         objWorkflowState.IsActive));
            DataCache.RemoveCache(string.Format(WORKFLOW_CACHE_KEY, objWorkflowState.WorkflowID));
            return StateID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   CanDeleteWorkFlowState determines whether a workflow state is in use and can be deleted
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "StateID">A WorkflowState ID</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool CanDeleteWorkFlowState(int StateID)
        {
            return DataProvider.Instance().CanDeleteWorkFlowState(StateID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   DeleteWorkFlowState deletes a State instance
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "StateID">The StateID</param>
        /// <param name="WorkflowID">The WorkflowID</param>
        /// -----------------------------------------------------------------------------
        public void DeleteWorkflowState(int StateID, int WorkflowID)
        {
            DataProvider.Instance().DeleteWorkflowState(StateID);
            DataCache.RemoveCache(string.Format(WORKFLOW_CACHE_KEY, WorkflowID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetWorkFlow retrieves a Workflow object
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "WorkflowID">The ID of the Workflow</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public WorkflowStateInfo GetWorkflow(int WorkflowID)
        {
            return (WorkflowStateInfo) CBO.FillObject(DataProvider.Instance().GetWorkflow(WorkflowID), typeof (WorkflowStateInfo));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetWorkFlowState retrieves a WorkflowStateInfo object for the Workflow and State
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "StateID">The ID of the State</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public WorkflowStateInfo GetWorkflowState(int StateID)
        {
            return (WorkflowStateInfo) CBO.FillObject(DataProvider.Instance().GetWorkflowState(StateID), typeof (WorkflowStateInfo));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   UpdateWorkFlow updates an existing workflow for the portal
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "WorkflowID">The ID of the Workflow</param>
        /// <param name = "WorkflowName">The Name of the Workflow</param>
        /// <param name = "Description">The Description of the Workflow</param>
        /// <param name = "IsDeleted">Whether or not the Workflow is Active</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateWorkflow(int WorkflowID, string WorkflowName, string Description, bool IsDeleted)
        {
            DataProvider.Instance().UpdateWorkflow(WorkflowID, WorkflowName, Description, IsDeleted);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddWorkFlowState creates a new workflow and state instance
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name = "objWorkflowState">A WorkflowStateInfo object</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public void UpdateWorkflowState(WorkflowStateInfo objWorkflowState)
        {
            DataProvider.Instance().UpdateWorkflowState(objWorkflowState.StateID, objWorkflowState.StateName, objWorkflowState.Order, objWorkflowState.Notify, objWorkflowState.IsActive);
            DataCache.RemoveCache(string.Format(WORKFLOW_CACHE_KEY, objWorkflowState.WorkflowID));
        }

        #endregion
    }
}