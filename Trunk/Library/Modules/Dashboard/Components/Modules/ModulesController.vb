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


Imports System.Collections.Generic
Imports System.Xml
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Dashboard.Data

Namespace DotNetNuke.Modules.Dashboard.Components.Modules
    Public Class ModulesController
        Implements IDashboardData

#Region "Public Static Methods"

        Public Shared Function GetInstalledModules() As List(Of ModuleInfo)
            Return CBO.FillCollection(Of ModuleInfo)(DataService.GetInstalledModules())
        End Function

#End Region

#Region "IDashboardData Members"
        Public Sub ExportData(ByVal writer As XmlWriter) Implements IDashboardData.ExportData
            'Write start of Installed Modules 
            writer.WriteStartElement("installedModules")

            'Iterate through Installed Modules 
            For Each [module] As ModuleInfo In GetInstalledModules()
                [module].WriteXml(writer)
            Next

            'Write end of Installed Modules 
            writer.WriteEndElement()
        End Sub
#End Region
    End Class


End Namespace