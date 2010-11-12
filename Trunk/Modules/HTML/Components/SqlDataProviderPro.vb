'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    Partial Public Class SqlDataProvider
        Inherits DataProvider

#Region "Public Methods"

        Public Overrides Function AddWorkflow(ByVal PortalID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddWorkflow", GetNull(PortalID), WorkflowName, Description, IsDeleted), Integer)
        End Function

        Public Overrides Function AddWorkflowState(ByVal WorkflowID As Integer, ByVal StateName As String, ByVal Order As Integer, ByVal Notify As Boolean, ByVal IsActive As Boolean) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddWorkflowState", WorkflowID, StateName, Order, Notify, IsActive), Integer)
        End Function

        Public Overrides Function CanDeleteWorkFlowState(ByVal StateID As Integer) As Boolean
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "CanDeleteWorkFlowState", StateID), Boolean)
        End Function

        Public Overrides Sub DeleteWorkflowState(ByVal StateID As Integer)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteWorkflowState", StateID)
        End Sub

        Public Overrides Function GetWorkflow(ByVal WorkflowID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflow", WorkflowID), IDataReader)
        End Function

        Public Overrides Function GetWorkflowState(ByVal StateID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflowState", StateID), IDataReader)
        End Function

        Public Overrides Sub UpdateWorkflow(ByVal WorkflowID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateWorkflow", WorkflowID, WorkflowName, Description, IsDeleted)
        End Sub

        Public Overrides Sub UpdateWorkflowState(ByVal StateID As Integer, ByVal StateName As String, ByVal Order As Integer, ByVal Notify As Boolean, ByVal IsActive As Boolean)
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateWorkflowState", StateID, StateName, Order, Notify, IsActive)
        End Sub

        Private Function GetRoleNull(ByVal RoleID As Integer) As Object
            If RoleID.ToString = Common.Globals.glbRoleNothing Then
                Return DBNull.Value
            Else
                Return RoleID
            End If
        End Function

        Public Overrides Function GetPermissionsByStateID(ByVal StateID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetPermissionsByStateID", StateID), IDataReader)
        End Function

        Public Overrides Function GetWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflowStatePermission", WorkflowStatePermissionID), IDataReader)
        End Function

        Public Overrides Sub DeleteWorkflowStatePermissionsByStateID(ByVal StateID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteWorkflowStatePermissionsByStateID", StateID)
        End Sub

        Public Overrides Sub DeleteWorkflowStatePermissionsByUserID(ByVal userID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteWorkflowStatePermissionsByUserID", userID)
        End Sub

        Public Overrides Sub DeleteWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteWorkflowStatePermission", WorkflowStatePermissionID)
        End Sub

        Public Overrides Function AddWorkflowStatePermission(ByVal stateID As Integer, ByVal permissionID As Integer, ByVal roleID As Integer, ByVal allowAccess As Boolean, ByVal userID As Integer, ByVal createdByUserID As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddWorkflowStatePermission", stateID, permissionID, GetRoleNull(roleID), allowAccess, GetNull(userID), createdByUserID), Integer)
        End Function

        Public Overrides Sub UpdateWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer, ByVal portalWorkflowStateID As Integer, ByVal permissionID As Integer, ByVal roleID As Integer, ByVal allowAccess As Boolean, ByVal userID As Integer, ByVal lastModifiedByUserID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateWorkflowStatePermission", WorkflowStatePermissionID, portalWorkflowStateID, permissionID, GetRoleNull(roleID), allowAccess, GetNull(userID), lastModifiedByUserID)
        End Sub

#End Region

    End Class

End Namespace
