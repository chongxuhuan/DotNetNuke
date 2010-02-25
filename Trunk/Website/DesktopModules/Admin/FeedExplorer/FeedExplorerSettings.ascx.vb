'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
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
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Admin.Syndication

    Partial Class FeedExplorerSettings

        Inherits DotNetNuke.Entities.Modules.ModuleSettingsBase

#Region "Event Handlers"

        Public Overrides Sub LoadSettings()
            Try
                If Not Page.IsPostBack Then

                    If Not ContentSource.Items.FindByValue(CType(TabModuleSettings("ContentSource"), String)) Is Nothing Then
                        ContentSource.Items.FindByValue(CType(TabModuleSettings("ContentSource"), String)).Selected = True
                    End If

                    If Not Theme.Items.FindByValue(CType(TabModuleSettings("Theme"), String)) Is Nothing Then
                        Theme.Items.FindByValue(CType(TabModuleSettings("Theme"), String)).Selected = True
                    End If

                    If CType(TabModuleSettings("OpmlUrlFile"), String) <> "" Then
                        OpmlUrlFile.Text = CType(TabModuleSettings("OpmlUrlFile"), String)
                    Else
                        OpmlUrlFile.Text = ""
                    End If

                    If CType(TabModuleSettings("OpmlText"), String) <> "" Then
                        OpmlText.Text = CType(TabModuleSettings("OpmlText"), String)
                    Else
                        Dim opml As New StringBuilder()
                        opml.Append("<outline text=""Tab One"" type=""none"" category=""Tab"">" + ControlChars.NewLine.ToString())
                        opml.Append("    <outline text=""Section 1"" type=""none"" category=""Section"">" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 1"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 2"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("    </outline>" + ControlChars.NewLine.ToString())
                        opml.Append("    <outline text=""Section 2"" type=""none"" category=""Section"">" + ControlChars.NewLine)
                        opml.Append("        <outline text=""Category 1"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 2"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("    </outline>" + ControlChars.NewLine.ToString())
                        opml.Append("</outline>" + ControlChars.NewLine.ToString())
                        opml.Append("<outline text=""Tab Two"" type=""none"" category=""Tab"">" + ControlChars.NewLine.ToString())
                        opml.Append("    <outline text=""Section 1"" type=""none"" category=""Section"">" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 1"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 2"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("    </outline>" + ControlChars.NewLine.ToString())
                        opml.Append("    <outline text=""Section 2"" type=""none"" category=""Section"">" + ControlChars.NewLine)
                        opml.Append("        <outline text=""Category 1"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("        <outline text=""Category 2"" type=""rss"" category=""Category"" xmlUrl=""http://news.google.com/?output=rss"" />" + ControlChars.NewLine.ToString())
                        opml.Append("    </outline>" + ControlChars.NewLine.ToString())
                        opml.Append("</outline>" + ControlChars.NewLine.ToString())
                        opml.Append("<outline text=""DotNetNuke"" type=""link"" category=""Feed"" xmlUrl=""http://www.dotnetnuke.com/Portals/25/SolutionsExplorer/GlobalDirectory.opml"" />" + ControlChars.NewLine.ToString())
                        OpmlText.Text = opml.ToString()
                    End If
                End If
            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Public Overrides Sub UpdateSettings()
            Try
                If Page.IsValid Then
                    Dim objModules As New Entities.Modules.ModuleController

                    objModules.UpdateTabModuleSetting(TabModuleId, "ContentSource", ContentSource.SelectedValue)
                    objModules.UpdateTabModuleSetting(TabModuleId, "Theme", Theme.SelectedValue)
                    objModules.UpdateTabModuleSetting(TabModuleId, "OpmlUrlFile", OpmlUrlFile.Text)
                    objModules.UpdateTabModuleSetting(TabModuleId, "OpmlText", OpmlText.Text)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

#End Region

    End Class

End Namespace
