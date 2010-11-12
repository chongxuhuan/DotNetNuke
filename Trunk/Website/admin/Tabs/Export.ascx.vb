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
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports System.Reflection
Imports System.IO
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports System.Xml
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Modules.Admin.Tabs

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Export
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _Tab As TabInfo

#End Region

#Region "Public Properties"

        Public ReadOnly Property Tab() As TabInfo
            Get
                If _Tab Is Nothing Then
                    Dim objTabs As New TabController
                    _Tab = objTabs.GetTab(TabId, PortalId, False)
                End If
                Return _Tab
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Serializes the Tab
        ''' </summary>
        ''' <param name="xmlTemplate">Reference to XmlDocument context</param>
        ''' <param name="nodeTabs">Node to add the serialized objects</param>
        ''' <history>
        ''' 	[cnurse]	10/02/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SerializeTab(ByVal xmlTemplate As XmlDocument, ByVal nodeTabs As XmlNode)
            Dim nodeTab As XmlNode
            Dim xmlTab As XmlDocument

            xmlTab = New XmlDocument()
            nodeTab = TabController.SerializeTab(xmlTab, Tab, chkContent.Checked)
            nodeTabs.AppendChild(xmlTemplate.ImportNode(nodeTab, True))
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not TabPermissionController.CanExportPage Then
                Response.Redirect(AccessDeniedURL(), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    'cboFolders.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "-"))
                    Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, False, False, "ADD")
                    For Each folder As FolderInfo In folders
                        Dim FolderItem As New ListItem
                        If folder.FolderPath = Null.NullString Then
                            FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                        Else
                            FolderItem.Text = FileSystemUtils.RemoveTrailingSlash(folder.DisplayPath)
                        End If
                        FolderItem.Value = folder.FolderPath
                        cboFolders.Items.Add(FolderItem)
                    Next

                    If Tab IsNot Nothing Then
                        txtFile.Text = CleanName(Tab.TabName)
                    End If

                    If Not cboFolders.Items.FindByValue("Templates/") Is Nothing Then
                        cboFolders.Items.FindByValue("Templates/").Selected = True
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExport.Click
            Try
                Dim xmlTemplate As XmlDocument
                Dim nodePortal As XmlNode

                If Not Page.IsValid Then
                    Exit Sub
                End If

                Dim filename As String
                filename = PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedItem.Value & txtFile.Text + ".page.template"
                filename = filename.Replace("/", "\")

                xmlTemplate = New XmlDocument
                nodePortal = xmlTemplate.AppendChild(xmlTemplate.CreateElement("portal"))
                nodePortal.Attributes.Append(XmlUtils.CreateAttribute(xmlTemplate, "version", "3.0"))

                'Add template description
                Dim node As XmlElement = xmlTemplate.CreateElement("description")
                node.InnerXml = Server.HtmlEncode(txtDescription.Text)
                nodePortal.AppendChild(node)

                ' Serialize tabs
                Dim nodeTabs As XmlNode
                nodeTabs = nodePortal.AppendChild(xmlTemplate.CreateElement("tabs"))
                SerializeTab(xmlTemplate, nodeTabs)

                xmlTemplate.Save(filename)
                lblMessage.Text = String.Format(Services.Localization.Localization.GetString("ExportedMessage", Me.LocalResourceFile), filename)

                ' add file to Files table
                FileSystemUtils.AddFile(txtFile.Text + ".page.template", PortalId, cboFolders.SelectedItem.Value, PortalSettings.HomeDirectoryMapPath, "application/octet-stream")
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace
