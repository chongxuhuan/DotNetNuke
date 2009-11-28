' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2009 
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
Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.Dashboard.Data
Imports System.IO
Imports DotNetNuke.Common
Imports System.Xml

Namespace DotNetNuke.Modules.Dashboard.Components
    Public Class DashboardController

#Region "Public Static Methods"

        Public Shared Function AddDashboardControl(ByVal dashboardControl As DashboardControl) As Integer
            Return DataService.AddDashboardControl(dashboardControl.PackageID, dashboardControl.DashboardControlKey, dashboardControl.IsEnabled, _
                                                   dashboardControl.DashboardControlSrc, dashboardControl.DashboardControlLocalResources, _
                                                   dashboardControl.ControllerClass, dashboardControl.ViewOrder)
        End Function

        Public Shared Sub DeleteControl(ByVal dashboardControl As DashboardControl)
            DataService.DeleteDashboardControl(dashboardControl.DashboardControlID)
        End Sub

        Public Shared Sub Export(ByVal filename As String)
            Dim fullName As String = Path.Combine(Globals.HostMapPath, filename)
            Dim settings As New XmlWriterSettings()

            Using writer As XmlWriter = XmlWriter.Create(fullName, settings)
                'Write start of Dashboard 
                writer.WriteStartElement("dashboard")

                For Each dashboard As DashboardControl In GetDashboardControls(True)
                    Dim controller As IDashboardData = TryCast(Activator.CreateInstance(Reflection.CreateType(dashboard.ControllerClass)), IDashboardData)

                    If controller IsNot Nothing Then
                        controller.ExportData(writer)
                    End If
                Next

                'Write end of Host 
                writer.WriteEndElement()

                writer.Flush()
            End Using
        End Sub

        Public Shared Function GetDashboardControlByKey(ByVal dashboardControlKey As String) As DashboardControl
            Return CBO.FillObject(Of DashboardControl)(DataService.GetDashboardControlByKey(dashboardControlKey))
        End Function

        Public Shared Function GetDashboardControlByPackageId(ByVal packageId As Integer) As DashboardControl
            Return CBO.FillObject(Of DashboardControl)(DataService.GetDashboardControlByPackageId(packageId))
        End Function

        Public Shared Function GetDashboardControls(ByVal isEnabled As Boolean) As List(Of DashboardControl)
            Return CBO.FillCollection(Of DashboardControl)(DataService.GetDashboardControls(isEnabled))
        End Function

        Public Shared Sub UpdateDashboardControl(ByVal dashboardControl As DashboardControl)
            DataService.UpdateDashboardControl(dashboardControl.DashboardControlID, dashboardControl.DashboardControlKey, _
                                               dashboardControl.IsEnabled, dashboardControl.DashboardControlSrc, _
                                               dashboardControl.DashboardControlLocalResources, _
                                               dashboardControl.ControllerClass, dashboardControl.ViewOrder)
        End Sub

#End Region

    End Class



End Namespace