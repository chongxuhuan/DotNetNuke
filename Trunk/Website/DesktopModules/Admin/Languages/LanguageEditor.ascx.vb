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
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports DNNControls = DotNetNuke.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Installer
Imports Telerik.Web.UI

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages translations for Resource files
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[vmasanas]	10/04/2004  Created
    ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguageEditor
        Inherits PortalModuleBase
        Implements IActionable

#Region "Private Enums"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Identifies images in TreeView
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	07/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Enum eImageType
            Folder = 0
            Page = 1
        End Enum

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property Locale() As String
            Get
                Dim _Locale As String = Null.NullString

                If Request.QueryString("locale") <> "" Then
                    _Locale = Request.QueryString("locale")
                End If

                Return _Locale
            End Get
        End Property

        Protected ReadOnly Property PageSize() As Integer
            Get
                Dim _PageSize As Integer = 10
                If Me.Settings("PageSize") IsNot Nothing Then
                    _PageSize = CType(Me.Settings("PageSize"), Integer)

                    'Make sure Page Size is not invalid
                    If _PageSize < 1 Then _PageSize = 10
                End If
                Return _PageSize
            End Get
        End Property

        Protected ReadOnly Property UsePaging() As Boolean
            Get
                Dim _UsePaging As Boolean = False
                If Me.Settings("UsePaging") IsNot Nothing Then
                    _UsePaging = CType(Me.Settings("UsePaging"), Boolean)
                End If
                Return _UsePaging
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function AddResourceKey(ByVal resourceDoc As XmlDocument, ByVal resourceKey As String) As XmlNode
            Dim nodeData As XmlNode
            Dim attr As XmlAttribute

            ' missing entry
            nodeData = resourceDoc.CreateElement("data")
            attr = resourceDoc.CreateAttribute("name")
            attr.Value = resourceKey
            nodeData.Attributes.Append(attr)
            resourceDoc.SelectSingleNode("//root").AppendChild(nodeData)

            Return nodeData.AppendChild(resourceDoc.CreateElement("value"))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads Resource information into the datagrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        '''     [vmasanas}  25/03/2006  Modified to support new features
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGrid(ByVal reBind As Boolean)
            Dim EditTable As Hashtable
            Dim DefaultTable As Hashtable

            EditTable = LoadFile(rbMode.SelectedValue, "Edit")
            DefaultTable = LoadFile(rbMode.SelectedValue, "Default")

            lblResourceFile.Text = Path.GetFileName(ResourceFile(Locale, rbMode.SelectedValue).Replace(ApplicationMapPath, ""))
            lblFolder.Text = ResourceFile(Locale, rbMode.SelectedValue).Replace(ApplicationMapPath, "").Replace("\" + lblResourceFile.Text, "")

            ' check edit table
            ' if empty, just use default
            If EditTable.Count = 0 Then
                EditTable = DefaultTable
            Else
                'remove obsolete keys
                Dim ToBeDeleted As New ArrayList
                For Each key As String In EditTable.Keys
                    If Not DefaultTable.Contains(key) Then
                        ToBeDeleted.Add(key)
                    End If
                Next
                If ToBeDeleted.Count > 0 Then
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Obsolete", LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    For Each key As String In ToBeDeleted
                        EditTable.Remove(key)
                    Next
                End If

                'add missing keys
                For Each key As String In DefaultTable.Keys
                    If Not EditTable.Contains(key) Then
                        EditTable.Add(key, DefaultTable(key))
                    Else
                        ' Update default value
                        Dim p As Pair = CType(EditTable(key), Pair)
                        p.Second = CType(DefaultTable(key), Pair).First
                        EditTable(key) = p
                    End If
                Next
            End If

            Dim s As New SortedList(EditTable)

            resourcesGrid.DataSource = s
            If reBind Then
                resourcesGrid.DataBind()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initializes ResourceFile treeView
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadRootNodes()

            Dim node As New RadTreeNode()
            node.Text = "Local Resources"
            node.Value = "Local Resources"
            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            resourceFiles.Nodes.Add(node)

            node = New RadTreeNode()
            node.Text = "Global Resources"
            node.Value = "Global Resources"
            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
            resourceFiles.Nodes.Add(node)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads resources from file 
        ''' </summary>
        ''' <param name="mode">Active editor mode</param>
        ''' <param name="type">Resource being loaded (edit or default)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Depending on the editor mode, resources will be overrided using default DNN schema.
        ''' "Edit" resources will only load selected file.
        ''' When loading "Default" resources (to be used on the editor as helpers) fallback resource
        ''' chain will be used in order for the editor to be able to correctly see what 
        ''' is the current default value for the any key. This process depends on the current active
        ''' editor mode:
        ''' - System: when editing system base resources on en-US needs to be loaded
        ''' - Host: base en-US, and base locale especific resource
        ''' - Portal: base en-US, host override for en-US, base locale especific resource, and host override 
        ''' for locale
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadFile(ByVal mode As String, ByVal type As String) As Hashtable
            Dim file As String
            Dim ht As New Hashtable

            Select Case type
                Case "Edit"
                    ' Only load resources from the file being edited
                    file = ResourceFile(Locale, mode)
                    ht = LoadResource(ht, file)
                Case "Default"
                    ' Load system default
                    file = ResourceFile(Localization.SystemLocale, "System")
                    ht = LoadResource(ht, file)

                    Select Case mode
                        Case "Host"
                            ' Load base file for selected locale
                            file = ResourceFile(Locale, "System")
                            ht = LoadResource(ht, file)
                        Case "Portal"
                            'Load host override for default locale
                            file = ResourceFile(Localization.SystemLocale, "Host")
                            ht = LoadResource(ht, file)

                            If Locale <> Localization.SystemLocale Then
                                ' Load base file for locale
                                file = ResourceFile(Locale, "System")
                                ht = LoadResource(ht, file)

                                'Load host override for selected locale
                                file = ResourceFile(Locale, "Host")
                                ht = LoadResource(ht, file)
                            End If
                    End Select

            End Select

            Return ht

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads resources from file into the HastTable
        ''' </summary>
        ''' <param name="ht">Current resources HashTable</param>
        ''' <param name="filepath">Resources file</param>
        ''' <returns>Base table updated with new resources </returns>
        ''' <remarks>
        ''' Returned hashtable uses resourcekey as key.
        ''' Value contains a Pair object where:
        '''  First=>value to be edited
        '''  Second=>default value
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadResource(ByVal ht As Hashtable, ByVal filepath As String) As Hashtable
            Dim d As New XmlDocument
            Dim xmlLoaded As Boolean = False
            Try
                d.Load(filepath)
                xmlLoaded = True
            Catch    'exc As Exception
                xmlLoaded = False
            End Try
            If xmlLoaded Then
                Dim n As XmlNode
                For Each n In d.SelectNodes("root/data")
                    If n.NodeType <> XmlNodeType.Comment Then
                        Dim val As String = n.SelectSingleNode("value").InnerXml
                        If ht(n.Attributes("name").Value) Is Nothing Then
                            ht.Add(n.Attributes("name").Value, New Pair(val, val))
                        Else
                            ht(n.Attributes("name").Value) = New Pair(val, val)
                        End If
                    End If
                Next n
            End If
            Return ht
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the resource file name for a given resource and language
        ''' </summary>
        ''' <param name="mode">Identifies the resource being searched (System, Host, Portal)</param>
        ''' <returns>Localized File Name</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ResourceFile(ByVal language As String, ByVal mode As String) As String
            Return Localization.GetResourceFileName(SelectedResourceFile, language, mode, PortalId)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves / Gets the selected resource file being edited in viewstate
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	07/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property SelectedResourceFile() As String
            Get
                Return ViewState("SelectedResourceFile").ToString
            End Get
            Set(ByVal Value As String)
                ViewState("SelectedResourceFile") = Value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Configures the initial visibility status of the default label
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	26/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function ExpandDefault(ByVal p As Pair) As Boolean
            Return p.Second.ToString().Length < 150
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds the url for the lang. html editor control
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	07/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function OpenFullEditor(ByVal name As String) As String
            Dim file As String
            file = SelectedResourceFile.Replace(Server.MapPath(Common.Globals.ApplicationPath + "/"), "")
            Return EditUrl("Name", name, "EditResourceKey", "Locale=" & Locale, "ResourceFile=" & QueryStringEncode(file), "Mode=" & rbMode.SelectedValue, "Highlight=" & chkHighlight.Checked.ToString().ToLower())
        End Function

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            resourcesGrid.AllowPaging = UsePaging
            resourcesGrid.PageSize = PageSize
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads suported locales and shows default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"))

                    ' init tree
                    LoadRootNodes()

                    Dim language As Locale = LocaleController.Instance.GetLocale(Locale)
                    languageLabel.Language = language.Code

                    If Me.UserInfo.IsSuperUser Then
                        Dim mode As String = Request.QueryString("mode")
                        If Not String.IsNullOrEmpty(mode) AndAlso Not rbMode.Items.FindByValue(mode) Is Nothing Then
                            rbMode.SelectedValue = mode
                        Else
                            rbMode.SelectedValue = "Host"
                        End If
                    Else
                        rbMode.SelectedValue = "Portal"
                        rowMode.Visible = False
                    End If

                    Dim PersonalHighlight As String = Convert.ToString(Personalization.Personalization.GetProfile("LanguageEditor", "HighLight" & PortalId.ToString))
                    Dim highlight As String = Request.QueryString("highlight")
                    If Not String.IsNullOrEmpty(highlight) AndAlso highlight.ToLower() = "true" Then
                        chkHighlight.Checked = True
                    Else
                        If PersonalHighlight <> "" Then
                            chkHighlight.Checked = Convert.ToBoolean(PersonalHighlight)
                        End If
                    End If

                    If Request.QueryString("resourcefile") <> "" Then
                        SelectedResourceFile = Server.MapPath("~/" + QueryStringDecode(Request.QueryString("resourcefile")))
                    Else
                        SelectedResourceFile = Server.MapPath(Localization.GlobalResourceFile)
                    End If

                    If Not String.IsNullOrEmpty(Request.QueryString("message")) Then
                        Skins.Skin.AddModuleMessage(Me, Localization.GetString(Request.QueryString("message"), Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                    End If

                    BindGrid(Not IsPostBack)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Rebinds the grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub chkHighlight_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkHighlight.CheckedChanged
            Try
                Personalization.Personalization.SetProfile("LanguageEditor", "HighLight" & PortalSettings.PortalId.ToString, chkHighlight.Checked.ToString)
                BindGrid(True)
            Catch exc As Exception    'Module failed to load
                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes the localized file for a given locale
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' System Default file cannot be deleted
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Try
                If Locale = Localization.SystemLocale And rbMode.SelectedValue = "System" Then
                    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Delete.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                Else
                    Try
                        If File.Exists(ResourceFile(Locale, rbMode.SelectedValue)) Then
                            File.SetAttributes(ResourceFile(Locale, rbMode.SelectedValue), FileAttributes.Normal)
                            File.Delete(ResourceFile(Locale, rbMode.SelectedValue))
                            UI.Skins.Skin.AddModuleMessage(Me, String.Format(Localization.GetString("Deleted", Me.LocalResourceFile), ResourceFile(Locale, rbMode.SelectedValue)), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

                            BindGrid(True)

                            'Clear the resource file lookup dictionary as we have removed a file
                            DotNetNuke.Common.Utilities.DataCache.RemoveCache(DotNetNuke.Common.Utilities.DataCache.ResourceFileLookupDictionaryCacheKey)
                        End If
                    Catch
                        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    End Try
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates all values from the datagrid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        '''     [sleupold]  23/04/2010  Fixed missing parameters for navigateURL
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim node, parent As XmlNode
            Dim resDoc As New XmlDocument
            Dim defDoc As New XmlDocument
            Dim filename As String

            Try
                filename = ResourceFile(Locale, rbMode.SelectedValue)
                If Not File.Exists(filename) Then
                    ' load system default
                    resDoc.Load(ResourceFile(Localization.SystemLocale, "System"))
                Else
                    resDoc.Load(filename)
                End If
                defDoc.Load(ResourceFile(Localization.SystemLocale, "System"))

                ' only items different from default will be saved
                For Each di In resourcesGrid.Items
                    If (di.ItemType = GridItemType.Item Or di.ItemType = GridItemType.AlternatingItem) Then
                        Dim resourceKey As Label = CType(di.FindControl("resourceKey"), Label)
                        Dim txtValue As TextBox = CType(di.FindControl("txtValue"), TextBox)
                        Dim txtDefault As TextBox = CType(di.FindControl("txtDefault"), TextBox)

                        node = resDoc.SelectSingleNode("//root/data[@name='" + resourceKey.Text + "']/value")

                        Select Case rbMode.SelectedValue
                            Case "System"
                                ' this will save all items
                                If node Is Nothing Then
                                    node = AddResourceKey(resDoc, resourceKey.Text)
                                End If
                                node.InnerXml = Server.HtmlEncode(txtValue.Text)

                            Case "Host", "Portal"
                                ' only items different from default will be saved

                                If txtValue.Text <> txtDefault.Text Then
                                    If node Is Nothing Then
                                        node = AddResourceKey(resDoc, resourceKey.Text)
                                    End If
                                    node.InnerXml = Server.HtmlEncode(txtValue.Text)
                                ElseIf Not node Is Nothing Then
                                    ' remove item = default
                                    resDoc.SelectSingleNode("//root").RemoveChild(node.ParentNode)
                                End If
                        End Select
                    End If
                Next

                ' remove obsolete keys
                For Each node In resDoc.SelectNodes("//root/data")
                    If defDoc.SelectSingleNode("//root/data[@name='" + node.Attributes("name").Value + "']") Is Nothing Then
                        parent = node.ParentNode
                        parent.RemoveChild(node)
                    End If
                Next
                ' remove duplicate keys
                For Each node In resDoc.SelectNodes("//root/data")
                    If resDoc.SelectNodes("//root/data[@name='" + node.Attributes("name").Value + "']").Count > 1 Then
                        parent = node.ParentNode
                        parent.RemoveChild(node)
                    End If
                Next

                Select Case rbMode.SelectedValue
                    Case "System"
                        resDoc.Save(filename)
                    Case "Host", "Portal"
                        If resDoc.SelectNodes("//root/data").Count > 0 Then
                            ' there's something to save
                            resDoc.Save(filename)
                        Else
                            ' nothing to be saved, if file exists delete
                            If File.Exists(filename) Then
                                File.Delete(filename)
                            End If
                        End If
                End Select
                Dim selectedFile As String = SelectedResourceFile.Replace(Server.MapPath(Common.Globals.ApplicationPath + "/"), "")
                BindGrid(True)
                'Response.Redirect(NavigateURL(TabId, "", "Locale=" & Locale, "resourceFile=" & QueryStringEncode(selectedFile), _
                '                              "mode=" & rbMode.SelectedValue, "highlight=" & chkHighlight.Checked.ToString().ToLower(), _
                '                              "ctl=Editor", "mid=" & ModuleId, "Page=" & dgEditor.CurrentPageIndex.ToString, "message=FileSaved"), True)
            Catch exc As Exception    'Module failed to load
                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Rebinds the grid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        '''     [erikvb]    24/02/2010  added personalization
        ''' </history>
        ''' -----------------------------------------------------------------------------   
        Private Sub rbMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbMode.SelectedIndexChanged
            Try
                Personalization.Personalization.SetProfile("LanguageEditor", "Mode" & PortalSettings.PortalId.ToString, rbMode.SelectedValue)
                BindGrid(True)
            Catch
                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try
        End Sub

        Protected Sub resourceFiles_NodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles resourceFiles.NodeClick
            Try
                If e.Node.Nodes.Count = 0 Then
                    SelectedResourceFile = e.Node.Value
                    Try
                        BindGrid(True)
                    Catch
                        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    End Try
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub resourceFiles_NodeExpand(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles resourceFiles.NodeExpand
            Dim node As RadTreeNode
            Select Case e.Node.Value
                Case "Local Resources"
                    node = New RadTreeNode()
                    node.Text = "Admin"
                    node.Value = Server.MapPath("~/Admin")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = "Controls"
                    node.Value = Server.MapPath("~/Controls")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = "DesktopModules"
                    node.Value = Server.MapPath("~/DesktopModules")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = "Install"
                    node.Value = Server.MapPath("~/Install")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = "Providers"
                    node.Value = Server.MapPath("~/Providers")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)

                    node = New RadTreeNode()
                    node.Text = "HostSkins"
                    node.Value = Path.Combine(HostMapPath, "Skins")
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                    e.Node.Nodes.Add(node)

                    Dim portalSkinFolder As String = Path.Combine(PortalSettings.HomeDirectoryMapPath, "Skins")
                    If Directory.Exists(portalSkinFolder) AndAlso (PortalSettings.ActiveTab.ParentId = PortalSettings.AdminTabId) Then
                        node = New RadTreeNode()
                        node.Text = "PortalSkins"
                        node.Value = Path.Combine(PortalSettings.HomeDirectoryMapPath, "Skins")
                        node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack
                        e.Node.Nodes.Add(node)
                    End If
                Case "Global Resources"
                    node = New RadTreeNode()
                    node.Text = "Exceptions"
                    node.Value = Server.MapPath("~/App_GlobalResources/Exceptions")
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = Path.GetFileNameWithoutExtension(Localization.GlobalResourceFile)
                    node.Value = Server.MapPath(Localization.GlobalResourceFile)
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = Path.GetFileNameWithoutExtension(Localization.SharedResourceFile)
                    node.Value = Server.MapPath(Localization.SharedResourceFile)
                    e.Node.Nodes.Add(node)
                    node = New RadTreeNode()
                    node.Text = "WebControls"
                    node.Value = Server.MapPath("~/App_GlobalResources/WebControls")
                    e.Node.Nodes.Add(node)
                Case Else
                    For Each folder In Directory.GetDirectories(e.Node.Value)
                        Dim folderInfo As New System.IO.DirectoryInfo(folder)
                        node = New RadTreeNode()
                        node.Value = folderInfo.FullName
                        node.Text = folderInfo.Name
                        node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack

                        If folderInfo.GetFiles("*.resx").Length > 0 OrElse folderInfo.GetDirectories().Length > 0 Then
                            e.Node.Nodes.Add(node)
                        End If
                    Next
                    For Each file In Directory.GetFiles(e.Node.Value, "*.as?x.resx")
                        Dim fileInfo As New System.IO.FileInfo(file)
                        node = New RadTreeNode()
                        node.Value = fileInfo.FullName
                        node.Text = fileInfo.Name.Replace(".resx", "")

                        e.Node.Nodes.Add(node)
                    Next
                    For Each file In Directory.GetFiles(e.Node.Value, "SharedResources.resx")
                        Dim fileInfo As New System.IO.FileInfo(file)
                        node = New RadTreeNode()
                        node.Value = fileInfo.FullName
                        node.Text = fileInfo.Name.Replace(".resx", "")

                        e.Node.Nodes.Add(node)
                    Next
            End Select

            e.Node.Expanded = True
        End Sub

        Protected Sub resourcesGrid_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles resourcesGrid.ItemDataBound
            Try
                If e.Item.ItemType = GridItemType.AlternatingItem Or e.Item.ItemType = GridItemType.Item Then
                    Dim c As HyperLink
                    c = CType(e.Item.FindControl("lnkEdit"), HyperLink)
                    If Not c Is Nothing Then
                        ClientAPI.AddButtonConfirm(c, Localization.GetString("SaveWarning", Me.LocalResourceFile))
                    End If

                    Dim p As Pair = CType(CType(e.Item.DataItem, DictionaryEntry).Value, Pair)

                    Dim t As TextBox
                    Dim d As TextBox
                    t = CType(e.Item.FindControl("txtValue"), TextBox)
                    d = CType(e.Item.FindControl("txtDefault"), TextBox)
                    If p.First.ToString() = p.Second.ToString() And chkHighlight.Checked And Not p.Second.ToString = "" Then
                        t.CssClass = "Pending"
                    End If
                    Dim length As Integer = p.First.ToString().Length
                    If p.Second.ToString().Length > length Then
                        length = p.Second.ToString().Length
                    End If
                    If length > 30 Then
                        Dim height As Integer = 20 * (length \ 30)
                        If height > 100 Then height = 100
                        t.Height = New Unit(height)
                        t.TextMode = TextBoxMode.MultiLine
                        d.Height = New Unit(height)
                        d.TextMode = TextBoxMode.MultiLine
                    End If
                    t.Text = Server.HtmlDecode(p.First.ToString())
                    d.Text = Server.HtmlDecode(p.Second.ToString())
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub resourcesGrid_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles resourcesGrid.NeedDataSource
            BindGrid(False)
        End Sub

#End Region

#Region "Optional Interfaces"

        Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                Dim Actions As New ModuleActionCollection

                Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("Return.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", NavigateURL(), False, SecurityAccessLevel.Edit, True, False)

                Return Actions
            End Get
        End Property

#End Region

     End Class

End Namespace
