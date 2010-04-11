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
Imports System.Xml
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.FileSystem
Imports System.Collections.Generic
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
    Partial Class Import
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private Shadows ModuleId As Integer = -1
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

        Private Sub BindBeforeAfterTabControls()
            Dim listTabs As List(Of TabInfo)
            Dim parentTab As TabInfo = Nothing
            Dim noneSpecified As String = "<" + Localization.GetString("None_Specified") + ">"

            If cboParentTab.SelectedItem IsNot Nothing Then
                Dim parentTabID As Integer = Int32.Parse(cboParentTab.SelectedItem.Value)
                Dim controller As New TabController()
                parentTab = controller.GetTab(parentTabID, PortalId, False)
            End If

            If parentTab IsNot Nothing Then
                listTabs = New TabController().GetTabsByPortal(parentTab.PortalID).WithParentId(parentTab.TabID)
            Else
                listTabs = New TabController().GetTabsByPortal(PortalId).WithParentId(Null.NullInteger)
            End If
            listTabs = TabController.GetPortalTabs(listTabs, Null.NullInteger, True, noneSpecified, False, False, False, False, True)
            cboPositionTab.DataSource = listTabs
            cboPositionTab.DataBind()

            rbInsertPosition.Items.Clear()
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertBefore", LocalResourceFile), "Before"))
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertAfter", LocalResourceFile), "After"))
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertAtEnd", LocalResourceFile), "AtEnd"))
            rbInsertPosition.SelectedValue = "After"
        End Sub

        Private Sub BindFiles()
            cboTemplate.Items.Clear()
            If cboFolders.SelectedIndex <> 0 Then
                Dim files As String() = Directory.GetFiles(PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedValue, "*.page.template")
                For Each file As String In files
                    file = file.Replace(PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedValue, "")
                    cboTemplate.Items.Add(New ListItem(file.Replace(".page.template", ""), file))
                Next
                cboTemplate.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "None_Specified"))
                cboTemplate.SelectedIndex = 0
            End If
        End Sub

        Private Sub BindTabControls()
            cboParentTab.DataSource = GetTabs(True)
            cboParentTab.DataBind()

            BindBeforeAfterTabControls()
            If cboPositionTab.Items.Count > 0 Then
                trInsertPositionRow.Visible = True
            Else
                trInsertPositionRow.Visible = False
            End If
            cboParentTab.AutoPostBack = True

            If Not cboPositionTab.Items.FindByValue(TabId.ToString) Is Nothing Then
                cboPositionTab.ClearSelection()
                cboPositionTab.Items.FindByValue(TabId.ToString).Selected = True
            End If
        End Sub

        Private Sub DisplayNewRows()
            trTabName.Visible = (optMode.SelectedIndex = 0)
            trParentTabName.Visible = (optMode.SelectedIndex = 0)
            trInsertPositionRow.Visible = (optMode.SelectedIndex = 0)
        End Sub

        Private Function GetTabs(ByVal includeURL As Boolean) As List(Of TabInfo)
            Dim noneSpecified As String = "<" + Localization.GetString("None_Specified") + ">"

            Dim tabs As List(Of TabInfo) = TabController.GetPortalTabs(PortalId, Null.NullInteger, True, noneSpecified, True, False, includeURL, False, True)
            If Me.UserInfo.IsSuperUser Then
                Dim hostTabs As Dictionary(Of Integer, TabInfo) = New TabController().GetTabsByPortal(Null.NullInteger)
                For Each kvp As KeyValuePair(Of Integer, TabInfo) In hostTabs
                    tabs.Add(kvp.Value)
                Next
            End If

            Return tabs
        End Function

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not TabPermissionController.CanImportPage Then
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
        Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    cboFolders.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "-"))
                    Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, False, False, "READ, WRITE")
                    For Each folder As FolderInfo In folders
                        Dim FolderItem As New ListItem
                        If folder.FolderPath = Null.NullString Then
                            FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                        Else
                            FolderItem.Text = FileSystemUtils.RemoveTrailingSlash(folder.FolderPath)
                        End If
                        FolderItem.Value = folder.FolderPath
                        cboFolders.Items.Add(FolderItem)
                    Next

                    If Not cboFolders.Items.FindByValue("Templates/") Is Nothing Then
                        cboFolders.Items.FindByValue("Templates/").Selected = True
                    End If

                    BindFiles()
                    BindTabControls()
                    DisplayNewRows()
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
        Protected Sub cboFolders_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFolders.SelectedIndexChanged
            BindFiles()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
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
        Protected Sub cmdImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdImport.Click
            Try
                If cboTemplate.SelectedItem Is Nothing OrElse cboTemplate.SelectedValue = "None_Specified" Then
                    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("SpecifyFile", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    Exit Sub
                End If
                If optMode.SelectedIndex = -1 Then
                    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("SpecifyMode", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    Exit Sub
                End If

                'Load template
                Dim xmlDoc As New XmlDocument
                xmlDoc.Load(PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedValue & cboTemplate.SelectedValue)

                Dim nodeTab As XmlNode = xmlDoc.SelectSingleNode("//portal/tabs/tab")
                Dim objTab As TabInfo
                If optMode.SelectedValue = "ADD" Then
                    If String.IsNullOrEmpty(txtTabName.Text) Then
                        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("SpecifyName", LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        Exit Sub
                    End If

                    'New Tab
                    objTab = New TabInfo
                    objTab.PortalID = PortalId
                    objTab.TabName = txtTabName.Text
                    objTab.IsVisible = True
                    If cboParentTab.SelectedItem IsNot Nothing Then
                        objTab.ParentId = Int32.Parse(cboParentTab.SelectedItem.Value)
                    End If

                    objTab.TabPath = GenerateTabPath(objTab.ParentId, objTab.TabName)
                    Dim tabID As Integer = TabController.GetTabByTabPath(objTab.PortalID, objTab.TabPath)
                    Dim objTabs As New TabController

                    'Check if tab exists
                    If tabID <> Null.NullInteger Then
                        Dim existingTab As TabInfo = objTabs.GetTab(tabID, PortalId, False)
                        If existingTab IsNot Nothing AndAlso existingTab.IsDeleted Then
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("TabRecycled", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                        Else
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("TabExists", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If
                        Exit Sub
                    End If

                    Dim positionTabID As Integer = Int32.Parse(cboPositionTab.SelectedItem.Value)

                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    If rbInsertPosition.SelectedValue = "After" And positionTabID > Null.NullInteger Then
                        objTab.TabID = objTabs.AddTabAfter(objTab, positionTabID)
                    ElseIf rbInsertPosition.SelectedValue = "Before" And positionTabID > Null.NullInteger Then
                        objTab.TabID = objTabs.AddTabBefore(objTab, positionTabID)
                    Else
                        objTab.TabID = objTabs.AddTab(objTab)
                    End If
                    objEventLog.AddLog(objTab, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.TAB_CREATED)

                    'Update Tab properties from template
                    objTab = TabController.DeserializeTab(nodeTab, objTab, PortalId, PortalTemplateModuleAction.Replace)
                Else
                    'Replace Existing Tab
                    objTab = TabController.DeserializeTab(nodeTab, Tab, PortalId, PortalTemplateModuleAction.Replace)
                End If

                If optRedirect.SelectedValue = "VIEW" Then
                    Response.Redirect(NavigateURL(objTab.TabID), True)
                Else
                    Response.Redirect(NavigateURL(objTab.TabID, "Tab", "action=edit"), True)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cboParentTab_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParentTab.SelectedIndexChanged
            BindBeforeAfterTabControls()
        End Sub

        Protected Sub cboTemplate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTemplate.SelectedIndexChanged
            Try
                Dim filename As String

                If cboTemplate.SelectedIndex > 0 Then
                    filename = PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedItem.Value & cboTemplate.SelectedValue
                    Dim xmldoc As New XmlDocument
                    Dim node As XmlNode
                    xmldoc.Load(filename)
                    node = xmldoc.SelectSingleNode("//portal/description")
                    If Not node Is Nothing AndAlso node.InnerXml <> "" Then
                        lblTemplateDescription.Visible = True
                        lblTemplateDescription.Text = Server.HtmlDecode(node.InnerXml)
                        txtTabName.Text = cboTemplate.SelectedItem.Text
                    Else
                        lblTemplateDescription.Visible = False
                    End If
                Else
                    lblTemplateDescription.Visible = False
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub optMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optMode.SelectedIndexChanged
            DisplayNewRows()
        End Sub

#End Region

    End Class

End Namespace
