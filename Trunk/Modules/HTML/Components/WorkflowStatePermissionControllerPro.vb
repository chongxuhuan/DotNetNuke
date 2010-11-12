'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users

Namespace DotNetNuke.Security.Permissions

    Partial Public Class WorkflowStatePermissionController

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ClearPermissionCache clears the WorkflowState Permission Cache
        ''' </summary>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub ClearPermissionCache()
            DataCache.RemoveCache(WorkflowStatePermissionCacheKey)
        End Sub

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddWorkflowStatePermission adds a WorkflowState Permission to the Database
        ''' </summary>
        ''' <param name="objWorkflowStatePermission">The WorkflowState Permission to add</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddWorkflowStatePermission(ByVal objWorkflowStatePermission As WorkflowStatePermissionInfo) As Integer
            Dim Id As Integer = provider.AddWorkflowStatePermission(objWorkflowStatePermission.StateID, objWorkflowStatePermission.PermissionID, objWorkflowStatePermission.RoleID, _
                                                              objWorkflowStatePermission.AllowAccess, objWorkflowStatePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache()
            Return Id
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteWorkflowStatePermission deletes a WorkflowState Permission in the Database
        ''' </summary>
        ''' <param name="WorkflowStatePermissionID">The ID of the WorkflowState Permission to delete</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer)
            provider.DeleteWorkflowStatePermission(WorkflowStatePermissionID)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteWorkflowStatePermissionsByStateID deletes a WorkflowState's 
        ''' WorkflowState Permission in the Database
        ''' </summary>
        ''' <param name="StateID">The ID of the State to delete</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteWorkflowStatePermissionsByStateID(ByVal StateID As Integer)
            provider.DeleteWorkflowStatePermissionsByStateID(StateID)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteWorkflowStatePermissionsByUserID deletes a user's WorkflowState Permission in the Database
        ''' </summary>
        ''' <param name="objUser">The user</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub DeleteWorkflowStatePermissionsByUserID(ByVal objUser As UserInfo)
            provider.DeleteWorkflowStatePermissionsByUserID(objUser.UserID)
            ClearPermissionCache()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkflowStatePermission gets a WorkflowState Permission from the Database
        ''' </summary>
        ''' <param name="WorkflowStatePermissionID">The ID of the WorkflowState Permission</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer) As WorkflowStatePermissionInfo
            Return CBO.FillObject(Of WorkflowStatePermissionInfo)(provider.GetWorkflowStatePermission(WorkflowStatePermissionID), True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateWorkflowStatePermission updates a WorkflowState Permission in the Database
        ''' </summary>
        ''' <param name="objWorkflowStatePermission">The WorkflowState Permission to update</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpdateWorkflowStatePermission(ByVal objWorkflowStatePermission As WorkflowStatePermissionInfo)
            provider.UpdateWorkflowStatePermission(objWorkflowStatePermission.WorkflowStatePermissionID, objWorkflowStatePermission.StateID, objWorkflowStatePermission.PermissionID, _
                                             objWorkflowStatePermission.RoleID, objWorkflowStatePermission.AllowAccess, objWorkflowStatePermission.UserID, UserController.GetCurrentUserInfo.UserID)
            ClearPermissionCache()
        End Sub


        Public Shared Function GetPermissionsByStateID(ByVal StateID As Integer) As ArrayList
            Return CBO.FillCollection(provider.GetPermissionsByStateID(StateID), GetType(PermissionInfo))
        End Function

#End Region

    End Class



End Namespace
