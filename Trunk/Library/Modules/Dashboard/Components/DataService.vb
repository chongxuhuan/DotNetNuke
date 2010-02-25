' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2010 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Imports System
Imports System.Data
Imports DotNetNuke.Data

Namespace DotNetNuke.Modules.Dashboard.Data
    Public Class DataService

#Region "Private Members"

        Private Shared provider As DataProvider = DataProvider.Instance()
        Private Shared moduleQualifier As String = "Dashboard_"

#End Region

        Private Shared Function GetFullyQualifiedName(ByVal name As String) As String
            Return [String].Concat(moduleQualifier, name)
        End Function

#Region "Public Static Methods"

        Public Shared Function AddDashboardControl(ByVal packageId As Integer, ByVal dashboardControlKey As String, ByVal isEnabled As Boolean, _
                                                   ByVal dashboardControlSrc As String, ByVal dashboardControlLocalResources As String, _
                                                   ByVal controllerClass As String, ByVal viewOrder As Integer) As Integer
            Return provider.ExecuteScalar(Of Integer)(GetFullyQualifiedName("AddControl"), packageId, dashboardControlKey, isEnabled, dashboardControlSrc, _
                                          dashboardControlLocalResources, controllerClass, viewOrder)
        End Function

        Public Shared Sub DeleteDashboardControl(ByVal dashboardControlId As Integer)
            provider.ExecuteNonQuery(GetFullyQualifiedName("DeleteControl"), dashboardControlId)
        End Sub

        Public Shared Function GetDashboardControlByKey(ByVal dashboardControlKey As String) As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetDashboardControlByKey"), dashboardControlKey)
        End Function

        Public Shared Function GetDashboardControlByPackageId(ByVal packageId As Integer) As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetDashboardControlByPackageId"), packageId)
        End Function

        Public Shared Function GetDashboardControls(ByVal isEnabled As Boolean) As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetControls"), isEnabled)
        End Function

        Public Shared Function GetDbInfo() As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetDbInfo"))
        End Function

        Public Shared Function GetDbFileInfo() As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetDbFileInfo"))
        End Function

        Public Shared Function GetDbBackups() As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetDbBackups"))
        End Function

        Public Shared Function GetInstalledModules() As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetInstalledModules"))
        End Function

        Public Shared Function GetPortals() As IDataReader
            Return provider.GetPortals(System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower)
        End Function

        Public Shared Function GetPortals(ByVal CultureCode As String) As IDataReader
            Return provider.GetPortals(CultureCode)
        End Function


        Public Shared Function GetServerErrors() As IDataReader
            Return provider.ExecuteReader(GetFullyQualifiedName("GetServerErrors"))
        End Function

        Public Shared Sub UpdateDashboardControl(ByVal dashboardControlId As Integer, ByVal dashboardControlKey As String, _
                                                 ByVal isEnabled As Boolean, ByVal dashboardControlSrc As String, _
                                                 ByVal dashboardControlLocalResources As String, _
                                                 ByVal controllerClass As String, ByVal viewOrder As Integer)
            provider.ExecuteNonQuery(GetFullyQualifiedName("UpdateControl"), dashboardControlId, dashboardControlKey, isEnabled, _
                                            dashboardControlSrc, dashboardControlLocalResources, controllerClass, viewOrder)
        End Sub

#End Region

    End Class


End Namespace
