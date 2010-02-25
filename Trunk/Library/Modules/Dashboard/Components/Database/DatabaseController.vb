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


Imports System.Collections.Generic
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Modules.Dashboard.Data

Namespace DotNetNuke.Modules.Dashboard.Components.Database
    Public Class DatabaseController
        Implements IDashboardData

#Region "Public Static Methods"

        Public Shared Function GetDbInfo() As DbInfo
            Return CBO.FillObject(Of DbInfo)(DataService.GetDbInfo())
        End Function

        Public Shared Function GetDbBackups() As List(Of BackupInfo)
            Return CBO.FillCollection(Of BackupInfo)(DataService.GetDbBackups())
        End Function

        Public Shared Function GetDbFileInfo() As List(Of DbFileInfo)
            Return CBO.FillCollection(Of DbFileInfo)(DataService.GetDbFileInfo())
        End Function

#End Region

#Region "IDashboardData Members"
        Public Sub ExportData(ByVal writer As System.Xml.XmlWriter) Implements IDashboardData.ExportData
            Dim database As DbInfo = GetDbInfo()

            'Write start of Database 
            writer.WriteStartElement("database")

            writer.WriteElementString("productVersion", database.ProductVersion)
            writer.WriteElementString("servicePack", database.ServicePack)
            writer.WriteElementString("productEdition", database.ProductEdition)
            writer.WriteElementString("softwarePlatform", database.SoftwarePlatform)

            'Write start of Backups 
            writer.WriteStartElement("backups")

            'Iterate through Backups 
            For Each backup As BackupInfo In database.Backups
                writer.WriteStartElement("backup")
                writer.WriteElementString("name", backup.Name)
                writer.WriteElementString("backupType", backup.BackupType)
                writer.WriteElementString("size", backup.Size.ToString())
                writer.WriteElementString("startDate", backup.StartDate.ToString())
                writer.WriteElementString("finishDate", backup.FinishDate.ToString())
                writer.WriteEndElement()
            Next

            'Write end of Backups 
            writer.WriteEndElement()

            'Write start of Files 
            writer.WriteStartElement("files")

            'Iterate through Files 
            For Each dbFile As DbFileInfo In database.Files
                writer.WriteStartElement("file")
                writer.WriteElementString("name", dbFile.Name)
                writer.WriteElementString("fileType", dbFile.FileType)
                writer.WriteElementString("shortFileName", dbFile.ShortFileName)
                writer.WriteElementString("fileName", dbFile.FileName)
                writer.WriteElementString("size", dbFile.Size.ToString())
                writer.WriteElementString("megabytes", dbFile.Megabytes.ToString())
                writer.WriteEndElement()
            Next

            'Write end of Files 
            writer.WriteEndElement()

            'Write end of Database 
            writer.WriteEndElement()
        End Sub
#End Region
    End Class


End Namespace