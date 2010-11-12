'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009 
' by DotNetNuke Corporation
'

Imports System
Imports DotNetNuke

Namespace DotNetNuke.Modules.Html

    Partial Public MustInherit Class DataProvider

#Region "Abstract methods"

        Public MustOverride Function AddWorkflow(ByVal PortalID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean) As Integer
        Public MustOverride Function AddWorkflowState(ByVal WorkflowID As Integer, ByVal StateName As String, ByVal Order As Integer, ByVal Notify As Boolean, ByVal IsActive As Boolean) As Integer
        Public MustOverride Function CanDeleteWorkFlowState(ByVal StateID As Integer) As Boolean
        Public MustOverride Sub DeleteWorkflowState(ByVal StateID As Integer)
        Public MustOverride Function GetWorkflow(ByVal WorkflowID As Integer) As IDataReader
        Public MustOverride Function GetWorkflowState(ByVal StateID As Integer) As IDataReader
        Public MustOverride Sub UpdateWorkflow(ByVal WorkflowID As Integer, ByVal WorkflowName As String, ByVal Description As String, ByVal IsDeleted As Boolean)
        Public MustOverride Sub UpdateWorkflowState(ByVal StateID As Integer, ByVal StateName As String, ByVal Order As Integer, ByVal Notify As Boolean, ByVal IsActive As Boolean)

        Public MustOverride Function GetPermissionsByStateID(ByVal StateID As Integer) As IDataReader
        Public MustOverride Function GetWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer) As IDataReader
        Public MustOverride Sub DeleteWorkflowStatePermissionsByStateID(ByVal StateID As Integer)
        Public MustOverride Sub DeleteWorkflowStatePermissionsByUserID(ByVal userID As Integer)
        Public MustOverride Sub DeleteWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer)
        Public MustOverride Function AddWorkflowStatePermission(ByVal StateID As Integer, ByVal permissionID As Integer, ByVal roleID As Integer, ByVal allowAccess As Boolean, ByVal userID As Integer, ByVal createdByUserID As Integer) As Integer
        Public MustOverride Sub UpdateWorkflowStatePermission(ByVal WorkflowStatePermissionID As Integer, ByVal StateID As Integer, ByVal permissionID As Integer, ByVal roleID As Integer, ByVal allowAccess As Boolean, ByVal userID As Integer, ByVal lastModifiedByUserID As Integer)

#End Region

    End Class

End Namespace
