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

using System.Collections;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

//
// DotNetNuke?- http://www.dotnetnuke.com
// Copyright (c) 2002-2009
// by DotNetNuke Corporation
//

#endregion

namespace DotNetNuke.Security.Permissions
{
    public partial class WorkflowStatePermissionController
    {
        #region "Private Shared Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   ClearPermissionCache clears the WorkflowState Permission Cache
        /// </summary>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void ClearPermissionCache()
        {
            DataCache.RemoveCache(WorkflowStatePermissionCacheKey);
        }

        #endregion

        #region "Public Shared Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddWorkflowStatePermission adds a WorkflowState Permission to the Database
        /// </summary>
        /// <param name = "objWorkflowStatePermission">The WorkflowState Permission to add</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int AddWorkflowStatePermission(WorkflowStatePermissionInfo objWorkflowStatePermission)
        {
            int Id = provider.AddWorkflowStatePermission(objWorkflowStatePermission.StateID,
                                                         objWorkflowStatePermission.PermissionID,
                                                         objWorkflowStatePermission.RoleID,
                                                         objWorkflowStatePermission.AllowAccess,
                                                         objWorkflowStatePermission.UserID,
                                                         UserController.GetCurrentUserInfo().UserID);
            ClearPermissionCache();
            return Id;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   DeleteWorkflowStatePermission deletes a WorkflowState Permission in the Database
        /// </summary>
        /// <param name = "WorkflowStatePermissionID">The ID of the WorkflowState Permission to delete</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteWorkflowStatePermission(int WorkflowStatePermissionID)
        {
            provider.DeleteWorkflowStatePermission(WorkflowStatePermissionID);
            ClearPermissionCache();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   DeleteWorkflowStatePermissionsByStateID deletes a WorkflowState's 
        ///   WorkflowState Permission in the Database
        /// </summary>
        /// <param name = "StateID">The ID of the State to delete</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteWorkflowStatePermissionsByStateID(int StateID)
        {
            provider.DeleteWorkflowStatePermissionsByStateID(StateID);
            ClearPermissionCache();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   DeleteWorkflowStatePermissionsByUserID deletes a user's WorkflowState Permission in the Database
        /// </summary>
        /// <param name = "objUser">The user</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteWorkflowStatePermissionsByUserID(UserInfo objUser)
        {
            provider.DeleteWorkflowStatePermissionsByUserID(objUser.UserID);
            ClearPermissionCache();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetWorkflowStatePermission gets a WorkflowState Permission from the Database
        /// </summary>
        /// <param name = "WorkflowStatePermissionID">The ID of the WorkflowState Permission</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static WorkflowStatePermissionInfo GetWorkflowStatePermission(int WorkflowStatePermissionID)
        {
            return CBO.FillObject<WorkflowStatePermissionInfo>(provider.GetWorkflowStatePermission(WorkflowStatePermissionID), true);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   UpdateWorkflowStatePermission updates a WorkflowState Permission in the Database
        /// </summary>
        /// <param name = "objWorkflowStatePermission">The WorkflowState Permission to update</param>
        /// <history>
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdateWorkflowStatePermission(WorkflowStatePermissionInfo objWorkflowStatePermission)
        {
            provider.UpdateWorkflowStatePermission(objWorkflowStatePermission.WorkflowStatePermissionID,
                                                   objWorkflowStatePermission.StateID,
                                                   objWorkflowStatePermission.PermissionID,
                                                   objWorkflowStatePermission.RoleID,
                                                   objWorkflowStatePermission.AllowAccess,
                                                   objWorkflowStatePermission.UserID,
                                                   UserController.GetCurrentUserInfo().UserID);
            ClearPermissionCache();
        }


        public static ArrayList GetPermissionsByStateID(int StateID)
        {
            return CBO.FillCollection(provider.GetPermissionsByStateID(StateID), typeof (PermissionInfo));
        }

        #endregion
    }
}