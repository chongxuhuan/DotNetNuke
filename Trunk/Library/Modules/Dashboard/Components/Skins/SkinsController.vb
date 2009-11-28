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
Imports System.IO
Imports DotNetNuke.Common
Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Modules.Dashboard.Components.Skins
    Public Class SkinsController
        Implements IDashboardData

#Region "Private Members"

        Private Shared Function isFallbackSkin(ByVal skinPath As String) As Boolean
            Dim defaultSkin As SkinDefaults = SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo)
            Dim defaultSkinPath As String = (Globals.HostMapPath + DotNetNuke.UI.Skins.SkinController.RootSkin + defaultSkin.Folder).Replace("/", "\")
            If defaultSkinPath.EndsWith("\") Then
                defaultSkinPath = defaultSkinPath.Substring(0, defaultSkinPath.Length - 1)
            End If
            Return skinPath.IndexOf(defaultSkinPath, StringComparison.CurrentCultureIgnoreCase) <> -1
        End Function

#End Region

#Region "Public Static Methods"

        Public Shared Function GetInstalledSkins() As List(Of SkinInfo)
            Dim list As New List(Of SkinInfo)()

            For Each folder As String In Directory.GetDirectories(Path.Combine(Globals.HostMapPath, "Skins"))
                If Not folder.EndsWith(Globals.glbHostSkinFolder) Then
                    Dim skin As New SkinInfo()
                    skin.SkinName = folder.Substring(folder.LastIndexOf("\") + 1)
                    skin.InUse = isFallbackSkin(folder) OrElse Not DotNetNuke.UI.Skins.SkinController.CanDeleteSkin(folder, "")
                    list.Add(skin)
                End If
            Next
            Return list
        End Function

#End Region

#Region "IDashboardData Members"

        Public Sub ExportData(ByVal writer As System.Xml.XmlWriter) Implements IDashboardData.ExportData
            'Write start of Installed Skins 
            writer.WriteStartElement("installedSkins")

            'Iterate through Installed Skins 
            For Each skin As SkinInfo In GetInstalledSkins()
                skin.WriteXml(writer)
            Next

            'Write end of Installed Skins 
            writer.WriteEndElement()
        End Sub
#End Region
    End Class

End Namespace