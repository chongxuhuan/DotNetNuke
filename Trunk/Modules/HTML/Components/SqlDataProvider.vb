'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports System
Imports System.Data
Imports Microsoft.ApplicationBlocks.Data
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SqlDataProvider is a concrete class that provides the SQL Server implementation of the Data Access Layer for the HtmlText module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SqlDataProvider

        Inherits DataProvider

#Region "Private Members"

        Private Const ProviderType As String = "data"

        Private _providerConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private _connectionString As String
        Private _providerPath As String
        Private _objectQualifier As String
        Private _databaseOwner As String

#End Region

#Region "Constructors"

        Public Sub New()

            ' Read the configuration specific information for this provider
            Dim objProvider As Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), Framework.Providers.Provider)

            ' Read the attributes for this provider
            _connectionString = Config.GetConnectionString()

            _providerPath = objProvider.Attributes("providerPath")

            _objectQualifier = objProvider.Attributes("objectQualifier")
            If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
                _objectQualifier += "_"
            End If

            _databaseOwner = objProvider.Attributes("databaseOwner")
            If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
                _databaseOwner += "."
            End If

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property ConnectionString() As String
            Get
                Return _connectionString
            End Get
        End Property

        Public ReadOnly Property ProviderPath() As String
            Get
                Return _providerPath
            End Get
        End Property

        Public ReadOnly Property ObjectQualifier() As String
            Get
                Return _objectQualifier
            End Get
        End Property

        Public ReadOnly Property DatabaseOwner() As String
            Get
                Return _databaseOwner
            End Get
        End Property

#End Region

#Region "Public Methods"

        Private Function GetNull(ByVal Field As Object) As Object
            Return Common.Utilities.Null.GetNull(Field, DBNull.Value)
        End Function

        Public Overrides Function GetHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetHtmlText", ModuleID, ItemID), IDataReader)
        End Function

        Public Overrides Function GetTopHtmlText(ByVal ModuleID As Integer, ByVal IsPublished As Boolean) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetTopHtmlText", ModuleID, IsPublished), IDataReader)
        End Function

        Public Overrides Function GetAllHtmlText(ByVal ModuleID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetAllHtmlText", ModuleID), IDataReader)
        End Function

        Public Overrides Function AddHtmlText(ByVal ModuleID As Integer, ByVal Content As String, ByVal StateID As Integer, ByVal IsPublished As Boolean, ByVal CreatedByUserID As Integer, ByVal History As Integer) As Integer
            Return CType(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & "AddHtmlText", ModuleID, Content, StateID, IsPublished, CreatedByUserID, History), Integer)
        End Function

        Public Overrides Sub UpdateHtmlText(ByVal ItemID As Integer, ByVal Content As String, ByVal StateID As Integer, ByVal IsPublished As Boolean, ByVal LastModifiedByUserID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateHtmlText", ItemID, Content, StateID, IsPublished, LastModifiedByUserID)
        End Sub

        Public Overrides Sub DeleteHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteHtmlText", ModuleID, ItemID)
        End Sub

        Public Overrides Function GetHtmlTextLog(ByVal ItemID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetHtmlTextLog", ItemID), IDataReader)
        End Function

        Public Overrides Sub AddHtmlTextLog(ByVal ItemID As Integer, ByVal StateID As Integer, ByVal Comment As String, ByVal Approved As Boolean, ByVal CreatedByUserID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "AddHtmlTextLog", ItemID, StateID, Comment, Approved, CreatedByUserID)
        End Sub

        Public Overrides Function GetHtmlTextUser(ByVal UserID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetHtmlTextUser", UserID), IDataReader)
        End Function

        Public Overrides Sub AddHtmlTextUser(ByVal ItemID As Integer, ByVal StateID As Integer, ByVal ModuleID As Integer, ByVal TabID As Integer, ByVal UserID As Integer)
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "AddHtmlTextUser", ItemID, StateID, ModuleID, TabID, UserID)
        End Sub

        Public Overrides Sub DeleteHtmlTextUsers()
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "DeleteHtmlTextUsers")
        End Sub

        Public Overrides Function GetWorkflows(ByVal PortalID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflows", PortalID), IDataReader)
        End Function

        Public Overrides Function GetWorkflowStates(ByVal WorkflowID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflowStates", WorkflowID), IDataReader)
        End Function

        Public Overrides Function GetWorkflowStatePermissions() As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflowStatePermissions"), IDataReader)
        End Function

        Public Overrides Function GetWorkflowStatePermissionsByStateID(ByVal StateID As Integer) As IDataReader
            Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetWorkflowStatePermissionsByStateID", StateID), IDataReader)
        End Function

#End Region

    End Class

End Namespace
