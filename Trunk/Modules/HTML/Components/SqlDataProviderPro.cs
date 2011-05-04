#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

#region Usings

using System;
using System.Data;

using DotNetNuke.Common;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Modules.Html
{
    public partial class SqlDataProvider : DataProvider
    {
        #region "Public Methods"

        public override int AddWorkflow(int PortalID, string WorkflowName, string Description, bool IsDeleted)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddWorkflow", GetNull(PortalID), WorkflowName, Description, IsDeleted));
        }

        public override int AddWorkflowState(int WorkflowID, string StateName, int Order, bool Notify, bool IsActive)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddWorkflowState", WorkflowID, StateName, Order, Notify, IsActive));
        }

        public override bool CanDeleteWorkFlowState(int StateID)
        {
            return Convert.ToBoolean(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "CanDeleteWorkFlowState", StateID));
        }

        public override void DeleteWorkflowState(int StateID)
        {
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteWorkflowState", StateID);
        }

        public override IDataReader GetWorkflow(int WorkflowID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflow", WorkflowID);
        }

        public override IDataReader GetWorkflowState(int StateID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflowState", StateID);
        }

        public override void UpdateWorkflow(int WorkflowID, string WorkflowName, string Description, bool IsDeleted)
        {
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateWorkflow", WorkflowID, WorkflowName, Description, IsDeleted);
        }

        public override void UpdateWorkflowState(int StateID, string StateName, int Order, bool Notify, bool IsActive)
        {
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateWorkflowState", StateID, StateName, Order, Notify, IsActive);
        }

        private object GetRoleNull(int RoleID)
        {
            if (RoleID.ToString() == Globals.glbRoleNothing)
            {
                return DBNull.Value;
            }
            else
            {
                return RoleID;
            }
        }

        public override IDataReader GetPermissionsByStateID(int StateID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetPermissionsByStateID", StateID);
        }

        public override IDataReader GetWorkflowStatePermission(int WorkflowStatePermissionID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetWorkflowStatePermission", WorkflowStatePermissionID);
        }

        public override void DeleteWorkflowStatePermissionsByStateID(int StateID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteWorkflowStatePermissionsByStateID", StateID);
        }

        public override void DeleteWorkflowStatePermissionsByUserID(int userID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteWorkflowStatePermissionsByUserID", userID);
        }

        public override void DeleteWorkflowStatePermission(int WorkflowStatePermissionID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteWorkflowStatePermission", WorkflowStatePermissionID);
        }

        public override int AddWorkflowStatePermission(int stateID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddWorkflowStatePermission",
                                                        stateID,
                                                        permissionID,
                                                        GetRoleNull(roleID),
                                                        allowAccess,
                                                        GetNull(userID),
                                                        createdByUserID));
        }

        public override void UpdateWorkflowStatePermission(int WorkflowStatePermissionID, int portalWorkflowStateID, int permissionID, int roleID, bool allowAccess, int userID,
                                                           int lastModifiedByUserID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateWorkflowStatePermission",
                                      WorkflowStatePermissionID,
                                      portalWorkflowStateID,
                                      permissionID,
                                      GetRoleNull(roleID),
                                      allowAccess,
                                      GetNull(userID),
                                      lastModifiedByUserID);
        }

        #endregion
    }
}