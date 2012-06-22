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
using System.Data;


namespace DotNetNuke.Modules.Html
{
    public abstract partial class DataProvider
    {
        #region Abstract methods

        public abstract int AddWorkflow(int PortalID, string WorkflowName, string Description, bool IsDeleted);

        public abstract int AddWorkflowState(int WorkflowID, string StateName, int Order, bool Notify, bool IsActive);

        public abstract bool CanDeleteWorkFlowState(int StateID);

        public abstract void DeleteWorkflowState(int StateID);

        public abstract IDataReader GetWorkflow(int WorkflowID);

        public abstract IDataReader GetWorkflowState(int StateID);

        public abstract void UpdateWorkflow(int WorkflowID, string WorkflowName, string Description, bool IsDeleted);

        public abstract void UpdateWorkflowState(int StateID, string StateName, int Order, bool Notify, bool IsActive);

        public abstract IDataReader GetPermissionsByStateID(int StateID);

        public abstract IDataReader GetWorkflowStatePermission(int WorkflowStatePermissionID);

        public abstract void DeleteWorkflowStatePermissionsByStateID(int StateID);

        public abstract void DeleteWorkflowStatePermissionsByUserID(int userID);

        public abstract void DeleteWorkflowStatePermission(int WorkflowStatePermissionID);

        public abstract int AddWorkflowStatePermission(int StateID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID);

        public abstract void UpdateWorkflowStatePermission(int WorkflowStatePermissionID, int StateID, int permissionID, int roleID, bool allowAccess, int userID, int lastModifiedByUserID);

        #endregion
    }
}