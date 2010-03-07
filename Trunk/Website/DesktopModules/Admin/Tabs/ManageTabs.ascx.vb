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

Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Security.Permissions
Imports System.Linq
Imports DotNetNuke.Entities.Content

Namespace DotNetNuke.Modules.Admin.Tabs

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ManageTabs PortalModuleBase is used to manage a Tab/Page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ManageTabs
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private strAction As String = ""
        Private _Tab As TabInfo

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property Tab() As TabInfo
            Get
                If _Tab Is Nothing Then
                    Select Case strAction
                        Case "", "add", "copy"
                            _Tab = New TabInfo()
                            _Tab.TabID = Null.NullInteger
                            _Tab.PortalID = PortalId
                        Case Else
                            Dim objTabs As New TabController
                            _Tab = objTabs.GetTab(TabId, PortalId, False)
                    End Select
                End If
                Return _Tab
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData loads the Controls with Tab Data from the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindTab()

            If Not Page.IsPostBack Then
                'Load TabControls
                BindTabControls(Tab)

                If Not Tab Is Nothing Then
                    If strAction <> "copy" Then
                        txtTabName.Text = Tab.TabName
                        txtTitle.Text = Tab.Title
                        txtDescription.Text = Tab.Description
                        txtKeyWords.Text = Tab.KeyWords
                        ctlURL.Url = Tab.Url
                    End If
                    ctlIcon.Url = Tab.IconFile
                    ctlIconLarge.Url = Tab.IconFileLarge
                    If Not cboParentTab.Items.FindByValue(Tab.ParentId.ToString) Is Nothing Then
                        cboParentTab.ClearSelection()
                        cboParentTab.Items.FindByValue(Tab.ParentId.ToString).Selected = True
                    End If
                    chkMenu.Checked = Tab.IsVisible

                    chkDisableLink.Checked = Tab.DisableLink
                    If TabId = PortalSettings.AdminTabId OrElse TabId = PortalSettings.SplashTabId OrElse _
                        TabId = PortalSettings.HomeTabId OrElse TabId = PortalSettings.LoginTabId OrElse _
                        TabId = PortalSettings.UserTabId OrElse TabId = PortalSettings.SuperTabId Then
                        chkDisableLink.Enabled = False
                    End If

                    ctlSkin.SkinSrc = Tab.SkinSrc
                    ctlContainer.SkinSrc = Tab.ContainerSrc

                    If PortalSettings.SSLEnabled Then
                        chkSecure.Enabled = True
                        chkSecure.Checked = Tab.IsSecure
                    Else
                        chkSecure.Enabled = False
                        chkSecure.Checked = Tab.IsSecure
                    End If
                    txtPriority.Text = Tab.SiteMapPriority.ToString()
                    If Not Null.IsNull(Tab.StartDate) Then
                        txtStartDate.Text = Tab.StartDate.ToShortDateString
                    End If
                    If Not Null.IsNull(Tab.EndDate) Then
                        txtEndDate.Text = Tab.EndDate.ToShortDateString
                    End If
                    If Tab.RefreshInterval <> Null.NullInteger Then
                        txtRefreshInterval.Text = Tab.RefreshInterval.ToString
                    End If

                    txtPageHeadText.Text = Tab.PageHeadText
                    chkPermanentRedirect.Checked = Tab.PermanentRedirect

                    ShowPermissions(Not Tab.IsSuperTab AndAlso TabPermissionController.CanAdminPage)
                    ctlAudit.Entity = Tab

                    termsSelector.PortalId = Tab.PortalID
                    termsSelector.Terms = Tab.Terms
                    termsSelector.DataBind()
                End If

                If strAction = "" OrElse strAction = "add" OrElse strAction = "copy" Then
                    InitializeTab()
                End If

                ' copy page options
            	cboCopyPage.DataSource = GetTabs(True, False, False)
                cboCopyPage.DataBind()
                rowModules.Visible = False
            End If
        End Sub

        Private Sub BindBeforeAfterTabControls()
            Dim listTabs As List(Of TabInfo)
            Dim parentTab As TabInfo = Nothing

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
            listTabs = TabController.GetPortalTabs(listTabs, Null.NullInteger, False, Null.NullString, False, False, False, False, True)
            cboPositionTab.DataSource = listTabs
            cboPositionTab.DataBind()

            rbInsertPosition.Items.Clear()
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertBefore", LocalResourceFile), "Before"))
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertAfter", LocalResourceFile), "After"))
            rbInsertPosition.Items.Add(New ListItem(Localization.GetString("InsertAtEnd", LocalResourceFile), "AtEnd"))
            rbInsertPosition.SelectedValue = "After"

            If parentTab IsNot Nothing AndAlso parentTab.IsSuperTab Then
                ShowPermissions(False)
            Else
                ShowPermissions(True)
            End If
        End Sub

        Private Sub BindTabControls(ByVal objTab As TabInfo)
            If String.IsNullOrEmpty(strAction) OrElse strAction = "copy" Then
                cboParentTab.DataSource = GetTabs(True, True, False)
            Else
                cboParentTab.DataSource = GetTabs(False, True, True)
            End If
            cboParentTab.DataBind()

            If strAction = "" OrElse strAction = "add" OrElse strAction = "copy" Then
                ' tab administrators can only create children of the current tab
                cboParentTab.ClearSelection()
                If Not TabPermissionController.CanAdminPage() Then
                    If Not cboParentTab.Items.FindByValue(TabId.ToString) Is Nothing Then
                        cboParentTab.Items.FindByValue(TabId.ToString).Selected = True
                    End If
                Else
                    If Not PortalSettings.ActiveTab.IsSuperTab AndAlso cboParentTab.Items.FindByValue(PortalSettings.ActiveTab.ParentId.ToString) IsNot Nothing Then
                        'Select the current tabs parent
                        cboParentTab.Items.FindByValue(PortalSettings.ActiveTab.ParentId.ToString).Selected = True
                    Else
                        ' select the <None Specified> option
                        cboParentTab.Items(0).Selected = True
                    End If
                End If

                BindBeforeAfterTabControls()
                If cboPositionTab.Items.Count > 0 Then
                    trInsertPositionRow.Visible = True
                Else
                    trInsertPositionRow.Visible = False
                End If
                cboParentTab.AutoPostBack = True
            Else
                trInsertPositionRow.Visible = False
                cboParentTab.AutoPostBack = False
            End If

            ' if editing a tab, load tab parent so parent link is not lost
            ' parent tab might not be loaded in cbotab if user does not have edit rights on it
            If Not TabPermissionController.CanAdminPage() And Not objTab Is Nothing Then
                If cboParentTab.Items.FindByValue(objTab.ParentId.ToString) Is Nothing Then
                    Dim objtabs As New TabController
                    Dim objparent As TabInfo = objtabs.GetTab(objTab.ParentId, objTab.PortalID, False)
                    If objparent IsNot Nothing Then
                        cboParentTab.Items.Add(New ListItem(objparent.LocalizedTabName, objparent.TabID.ToString))
                    End If
                End If
            End If

            cboCacheProvider.DataSource = OutputCache.OutputCachingProvider.GetProviderList()
            cboCacheProvider.DataBind()
            cboCacheProvider.Items.Insert(0, New ListItem(Localization.GetString("None_Specified"), ""))
            If objTab Is Nothing Then
                cboCacheProvider.Items(0).Selected = True
                rblCacheIncludeExclude.Items(0).Selected = True
            End If

            Dim tabController As New TabController
            Dim tabSettings As Hashtable = tabController.GetTabSettings(TabId)
            SetValue(cboCacheProvider, tabSettings, "CacheProvider")
            SetValue(txtCacheDuration, tabSettings, "CacheDuration")
            SetValue(rblCacheIncludeExclude, tabSettings, "CacheIncludeExclude")
            SetValue(txtIncludeVaryBy, tabSettings, "IncludeVaryBy")
            SetValue(txtExcludeVaryBy, tabSettings, "ExcludeVaryBy")
            SetValue(txtMaxVaryByCount, tabSettings, "MaxVaryByCount")

            ShowCacheRows()

        End Sub

        Private Sub SetValue(ByVal control As Control, ByVal tabSettings As Hashtable, ByVal tabSettingsKey As String)
            If control.GetType Is GetType(TextBox) Then
                If String.IsNullOrEmpty(tabSettings(tabSettingsKey)) Then
                    CType(control, TextBox).Text = ""
                Else
                    CType(control, TextBox).Text = tabSettings(tabSettingsKey).ToString
                End If
            ElseIf control.GetType Is GetType(DropDownList) Then
                If Not String.IsNullOrEmpty(tabSettings(tabSettingsKey)) Then
                    CType(control, DropDownList).ClearSelection()
                    CType(control, DropDownList).Items.FindByValue(tabSettings(tabSettingsKey).ToString).Selected = True
                Else
                    CType(control, DropDownList).Items.FindByValue("").Selected = True
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CheckQuota checks whether the Page Quota will be exceeded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckQuota()
            If PortalSettings.Pages < PortalSettings.PageQuota Or UserInfo.IsSuperUser Or PortalSettings.PageQuota = 0 Then
                cmdUpdate.Enabled = True
            Else
                cmdUpdate.Enabled = False
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("ExceededQuota", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes Tab
        ''' </summary>
        ''' <param name="deleteTabId">ID of the parent tab</param>
        ''' <remarks>
        ''' Will delete tab
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	30/09/2004	Created
        '''     [VMasanas]  01/09/2005  A tab will be deleted only if all descendants can be deleted
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function DeleteTab(ByVal deleteTabId As Integer) As Boolean
            Dim bDeleted As Boolean = Null.NullBoolean
            If TabPermissionController.CanDeletePage() Then
                bDeleted = TabController.DeleteTab(deleteTabId, PortalSettings, UserId)
                If Not bDeleted Then
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DeleteSpecialPage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            Else
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DeletePermissionError", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

            Return bDeleted
        End Function

        Private Sub DisplayTabModules()
            Select Case cboCopyPage.SelectedIndex
                Case 0
                    rowModules.Visible = False
                Case Else ' selected tab
                    If TabPermissionController.CanAddContentToPage() Then
                        grdModules.DataSource = LoadTabModules(Integer.Parse(cboCopyPage.SelectedItem.Value))
                        grdModules.DataBind()
                        rowModules.Visible = True
                    Else
                        rowModules.Visible = False
                    End If
            End Select
        End Sub

        Private Sub GetHostTabs(ByVal tabs As List(Of TabInfo))
            For Each kvp As KeyValuePair(Of Integer, TabInfo) In New TabController().GetTabsByPortal(Null.NullInteger)
                tabs.Add(kvp.Value)
            Next
        End Sub

        Private Function GetTabs(ByVal includeCurrent As Boolean, ByVal includeURL As Boolean, ByVal includeParent As Boolean) As List(Of TabInfo)
            Dim noneSpecified As String = "<" + Localization.GetString("None_Specified") + ">"
            Dim tabs As New List(Of TabInfo)()
            Dim controller As New TabController

            Dim excludeTabId As Integer = Null.NullInteger
            If Not includeCurrent Then
                excludeTabId = PortalSettings.ActiveTab.TabID
            End If

            If PortalSettings.ActiveTab.IsSuperTab Then
                Dim objTab As New TabInfo
                objTab.TabID = -1
                objTab.TabName = noneSpecified
                objTab.TabOrder = 0
                objTab.ParentId = -2
                tabs.Add(objTab)

                GetHostTabs(tabs)
            Else
                tabs = TabController.GetPortalTabs(PortalId, excludeTabId, PortalSecurity.IsInRole("Administrators"), noneSpecified, True, False, includeURL, False, True)
                'Need to include the Parent Tab if its not already in the list of tabs
                Dim parentTab = (From tab In tabs Select tab Where tab.TabID = PortalSettings.ActiveTab.ParentId).FirstOrDefault

                If includeParent And Not PortalSettings.ActiveTab.ParentId = Null.NullInteger And parentTab Is Nothing Then
                    tabs.Add(controller.GetTab(PortalSettings.ActiveTab.ParentId, PortalId, False))
                End If
                If Me.UserInfo.IsSuperUser And TabId = Null.NullInteger Then
                    GetHostTabs(tabs)
                End If
            End If

            Return tabs
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InitializeTab loads the Controls with default Tab Data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitializeTab()
            If Not cboPositionTab.Items.FindByValue(TabId.ToString) Is Nothing Then
                cboPositionTab.ClearSelection()
                cboPositionTab.Items.FindByValue(TabId.ToString).Selected = True
            End If

            cboFolders.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "-"))
            Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, False, False, "READ, WRITE")
            For Each folder As FolderInfo In folders
                Dim FolderItem As New ListItem
                If folder.FolderPath = Null.NullString Then
                    FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                Else
                    FolderItem.Text = folder.FolderPath
                End If
                FolderItem.Value = folder.FolderPath
                cboFolders.Items.Add(FolderItem)
                If FolderItem.Value = "Templates/" Then
                    FolderItem.Selected = True
                    LoadTemplates()
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Checks if parent tab will cause a circular reference
        ''' </summary>
        ''' <param name="intTabId">Tabid</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	28/11/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function IsCircularReference(ByVal intTabId As Integer) As Boolean
            If intTabId <> -1 Then
                Dim objTabs As New TabController
                Dim objtab As TabInfo = objTabs.GetTab(intTabId, PortalId, False)

                If objtab.Level = 0 Then
                    Return False
                Else
                    If TabId = objtab.ParentId Then
                        Return True
                    Else
                        Return IsCircularReference(objtab.ParentId)
                    End If
                End If
            Else
                Return False
            End If
        End Function

        Private Function LoadTabModules(ByVal TabID As Integer) As ArrayList
            Dim objModules As New ModuleController
            Dim arrModules As New ArrayList

            For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In objModules.GetTabModules(TabID)
                Dim objModule As ModuleInfo = kvp.Value

                If TabPermissionController.CanAddContentToPage() AndAlso Not objModule.IsDeleted AndAlso Not objModule.AllTabs Then
                    arrModules.Add(objModule)
                End If
            Next

            Return arrModules
        End Function

        Private Sub LoadTemplates()
            cboTemplate.Items.Clear()
            If cboFolders.SelectedIndex <> 0 Then
                Dim arrFiles As ArrayList = Common.Globals.GetFileList(PortalId, "page.template", False, cboFolders.SelectedItem.Value)
                Dim objFile As FileItem
                For Each objFile In arrFiles
                    Dim FileItem As New ListItem
                    FileItem.Text = objFile.Text.Replace(".page.template", "")
                    FileItem.Value = objFile.Text
                    cboTemplate.Items.Add(FileItem)
                    If Not Page.IsPostBack AndAlso FileItem.Text = "Default" Then
                        FileItem.Selected = True
                    End If
                Next
                cboTemplate.Items.Insert(0, New ListItem(Localization.GetString("None_Specified"), "-1"))
                If cboTemplate.SelectedIndex = -1 Then
                    cboTemplate.SelectedIndex = 0
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SaveTabData saves the Tab to the Database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strAction">The action to perform "edit" or "add"</param>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' 	[jlucarino]	2/26/2009	Added CreatedByUserID and LastModifiedByUserID
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SaveTabData(ByVal strAction As String) As Integer

            Dim strIcon As String = ""
            Dim strIconLarge As String = ""
            strIcon = ctlIcon.Url
            strIconLarge = ctlIconLarge.Url

            Dim objTabs As New TabController

            Tab.TabName = txtTabName.Text
            Tab.Title = txtTitle.Text
            Tab.Description = txtDescription.Text
            Tab.KeyWords = txtKeyWords.Text
            Tab.IsVisible = chkMenu.Checked
            Tab.DisableLink = chkDisableLink.Checked

            Dim parentTab As TabInfo = Nothing
            If cboParentTab.SelectedItem IsNot Nothing Then
                Dim parentTabID As Integer = Int32.Parse(cboParentTab.SelectedItem.Value)
                Dim controller As New TabController()
                parentTab = controller.GetTab(parentTabID, PortalId, False)
            End If

            If parentTab IsNot Nothing Then
                Tab.PortalID = parentTab.PortalID
                Tab.ParentId = parentTab.TabID
                Tab.Level = parentTab.Level + 1
            Else
                If strAction = "edit" AndAlso TabId = PortalSettings.SuperTabId Then
                    Tab.PortalID = Null.NullInteger
                Else
                    Tab.PortalID = PortalId
                End If
                Tab.ParentId = Null.NullInteger
                Tab.Level = 0
            End If
            Tab.IconFile = strIcon
            Tab.IconFileLarge = strIconLarge
            Tab.IsDeleted = False
            Tab.Url = ctlURL.Url

            Tab.TabPermissions.Clear()
            If Tab.PortalID <> Null.NullInteger Then
                Tab.TabPermissions.AddRange(dgPermissions.Permissions)
            End If

            Tab.Terms.Clear()
            Tab.Terms.AddRange(termsSelector.Terms)

            Tab.SkinSrc = ctlSkin.SkinSrc
            Tab.ContainerSrc = ctlContainer.SkinSrc
            Tab.TabPath = GenerateTabPath(Tab.ParentId, Tab.TabName)

            'Check for invalid 
            If Regex.IsMatch(Tab.TabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase) Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("InvalidTabName", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Return Null.NullInteger
            End If

            'Validate Tab Path
            If String.IsNullOrEmpty(strAction) Then
                Dim tabID As Integer = TabController.GetTabByTabPath(Tab.PortalID, Tab.TabPath)

                If tabID <> Null.NullInteger Then
                    Dim existingTab As TabInfo = objTabs.GetTab(tabID, PortalId, False)
                    If existingTab IsNot Nothing AndAlso existingTab.IsDeleted Then
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("TabRecycled", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("TabExists", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If
                    Return Null.NullInteger
                End If
            End If

            If txtStartDate.Text <> "" Then
                Tab.StartDate = Convert.ToDateTime(txtStartDate.Text)
            Else
                Tab.StartDate = Null.NullDate
            End If
            If txtEndDate.Text <> "" Then
                Tab.EndDate = Convert.ToDateTime(txtEndDate.Text)
            Else
                Tab.EndDate = Null.NullDate
            End If
            If Tab.StartDate > Null.NullDate AndAlso Tab.EndDate > Null.NullDate _
                            AndAlso Tab.StartDate.AddDays(1) >= Tab.EndDate Then
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("InvalidTabDates", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Return Null.NullInteger
            End If
            If txtRefreshInterval.Text.Length > 0 AndAlso IsNumeric(txtRefreshInterval.Text) Then
                Tab.RefreshInterval = Convert.ToInt32(txtRefreshInterval.Text)
            End If

            Tab.SiteMapPriority = Single.Parse(txtPriority.Text)
            Tab.PageHeadText = txtPageHeadText.Text
            Tab.IsSecure = chkSecure.Checked
            Tab.PermanentRedirect = chkPermanentRedirect.Checked

            If strAction = "edit" Then
                ' trap circular tab reference
                If cboParentTab.SelectedItem IsNot Nothing AndAlso Tab.TabID <> Int32.Parse(cboParentTab.SelectedItem.Value) _
                                    AndAlso Not IsCircularReference(Int32.Parse(cboParentTab.SelectedItem.Value)) Then
                    objTabs.UpdateTab(Tab)
                    If Me.IsHostMenu AndAlso Tab.PortalID <> Null.NullInteger Then
                        'Host Tab moved to Portal so clear Host cache
                        objTabs.ClearCache(Null.NullInteger)
                    End If
                    If Not Me.IsHostMenu AndAlso Tab.PortalID = Null.NullInteger Then
                        'Portal Tab moved to Host so clear portal cache
                        objTabs.ClearCache(PortalId)
                    End If
                    UpdateTabSettings(Tab.TabID)
                End If
            Else ' add or copy
                If cboPositionTab.SelectedItem Is Nothing Then
                    Tab.TabID = objTabs.AddTab(Tab)
                Else

                    Dim positionTabID As Integer = Int32.Parse(cboPositionTab.SelectedItem.Value)

                    If rbInsertPosition.SelectedValue = "After" And positionTabID > Null.NullInteger Then
                        Tab.TabID = objTabs.AddTabAfter(Tab, positionTabID)
                    ElseIf rbInsertPosition.SelectedValue = "Before" And positionTabID > Null.NullInteger Then
                        Tab.TabID = objTabs.AddTabBefore(Tab, positionTabID)
                    Else
                        Tab.TabID = objTabs.AddTab(Tab)
                    End If
                End If

                UpdateTabSettings(Tab.TabID)

                Dim copyTabId As Integer = Int32.Parse(cboCopyPage.SelectedItem.Value)
                If copyTabId <> -1 Then
                    Dim objDataGridItem As DataGridItem
                    Dim objModules As New ModuleController
                    Dim objModule As ModuleInfo
                    Dim chkModule As CheckBox
                    Dim optNew As RadioButton
                    Dim optCopy As RadioButton
                    Dim optReference As RadioButton
                    Dim txtCopyTitle As TextBox

                    For Each objDataGridItem In grdModules.Items
                        chkModule = CType(objDataGridItem.FindControl("chkModule"), CheckBox)
                        If chkModule.Checked Then
                            Dim intModuleID As Integer = CType(grdModules.DataKeys(objDataGridItem.ItemIndex), Integer)
                            optNew = CType(objDataGridItem.FindControl("optNew"), RadioButton)
                            optCopy = CType(objDataGridItem.FindControl("optCopy"), RadioButton)
                            optReference = CType(objDataGridItem.FindControl("optReference"), RadioButton)
                            txtCopyTitle = CType(objDataGridItem.FindControl("txtCopyTitle"), TextBox)

                            objModule = objModules.GetModule(intModuleID, copyTabId, False)
                            If Not objModule Is Nothing Then
                                'Clone module as it exists in the cache and changes we make will update the cached object
                                Dim newModule As ModuleInfo = objModule.Clone()

                                If Not optReference.Checked Then
                                    newModule.ModuleID = Null.NullInteger
                                End If

                                newModule.TabID = Tab.TabID
                                newModule.ModuleTitle = txtCopyTitle.Text
                                newModule.ModuleID = objModules.AddModule(newModule)

                                If optCopy.Checked Then
                                    If newModule.DesktopModule.BusinessControllerClass <> "" Then
                                        Dim objObject As Object = Framework.Reflection.CreateObject(newModule.DesktopModule.BusinessControllerClass, newModule.DesktopModule.BusinessControllerClass)
                                        If TypeOf objObject Is IPortable Then
                                            Dim Content As String = CType(CType(objObject, IPortable).ExportModule(intModuleID), String)
                                            If Content <> "" Then
                                                CType(objObject, IPortable).ImportModule(newModule.ModuleID, Content, newModule.DesktopModule.Version, UserInfo.UserID)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                Else
                    ' create the page from a template
                    If cboTemplate.SelectedItem IsNot Nothing AndAlso cboTemplate.SelectedItem.Value <> Null.NullInteger.ToString Then
                        Dim xmlDoc As New XmlDocument
                        Try
                            ' open the XML file
                            xmlDoc.Load(PortalSettings.HomeDirectoryMapPath & cboFolders.SelectedValue & cboTemplate.SelectedValue)
                        Catch ex As Exception
                            LogException(ex)

                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("BadTemplate", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            Return Null.NullInteger
                        End Try
                        TabController.DeserializePanes(xmlDoc.SelectSingleNode("//portal/tabs/tab/panes"), Tab.PortalID, Tab.TabID, PortalTemplateModuleAction.Ignore, New Hashtable)
                    End If
                End If
            End If

            ' url tracking
            Dim objUrls As New UrlController
            objUrls.UpdateUrl(PortalId, ctlURL.Url, ctlURL.UrlType, 0, Null.NullDate, Null.NullDate, ctlURL.Log, ctlURL.Track, Null.NullInteger, ctlURL.NewWindow)

            'Clear the Tab's Cached modules
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(TabId)

            'Update Cached Tabs as TabPath may be needed before cache is cleared
            Dim tempTab As TabInfo = Nothing
            If New TabController().GetTabsByPortal(PortalId).TryGetValue(Tab.TabID, tempTab) Then
                tempTab.TabPath = Tab.TabPath
            End If
            Return Tab.TabID
        End Function

        Private Sub UpdateTabSettings(ByVal tabId As Integer)

            Dim t As New TabController
            t.UpdateTabSetting(tabId, "CacheProvider", cboCacheProvider.SelectedValue)
            t.UpdateTabSetting(tabId, "CacheDuration", txtCacheDuration.Text)
            t.UpdateTabSetting(tabId, "CacheIncludeExclude", rblCacheIncludeExclude.SelectedValue)
            t.UpdateTabSetting(tabId, "IncludeVaryBy", txtIncludeVaryBy.Text)
            t.UpdateTabSetting(tabId, "ExcludeVaryBy", txtExcludeVaryBy.Text)
            t.UpdateTabSetting(tabId, "MaxVaryByCount", txtMaxVaryByCount.Text)


        End Sub

        Private Sub ShowPermissions(ByVal show As Boolean)
            rowPerm.Visible = show
        End Sub

#End Region

#Region "EventHandlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ' Verify that the current user has access to edit this module
            If Not TabPermissionController.HasTabPermission("ADD,EDIT,COPY,DELETE,MANAGE") Then
                Response.Redirect(AccessDeniedURL(), True)
            End If
            If Not (Request.QueryString("action") Is Nothing) Then
                strAction = Request.QueryString("action").ToLower
            End If

            If Tab.ContentItemId = Null.NullInteger AndAlso Tab.TabID <> Null.NullInteger Then
                Dim tabCtl As New TabController
                'This tab does not have a valid ContentItem
                tabCtl.CreateContentItem(Tab)
                tabCtl.UpdateTab(Tab)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim objModules As New ModuleController

                'this needs to execute always to the client script code is registred in InvokePopupCal
                cmdStartCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtStartDate)
                cmdEndCalendar.NavigateUrl = Common.Utilities.Calendar.InvokePopupCal(txtEndDate)

                If Page.IsPostBack = False Then

                    ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    ' load the list of files found in the upload directory
                    ctlIcon.ShowFiles = True
                    ctlIcon.ShowImages = True
                    ctlIcon.ShowTabs = False
                    ctlIcon.ShowUrls = False
                    ctlIcon.Required = False

                    ctlIcon.ShowLog = False
                    ctlIcon.ShowNewWindow = False
                    ctlIcon.ShowTrack = False
                    ctlIcon.FileFilter = glbImageFileTypes
                    ctlIcon.Width = "275px"

                    ctlIconLarge.ShowFiles = ctlIcon.ShowFiles
                    ctlIconLarge.ShowImages = ctlIcon.ShowImages
                    ctlIconLarge.ShowTabs = ctlIcon.ShowTabs
                    ctlIconLarge.ShowUrls = ctlIcon.ShowUrls
                    ctlIconLarge.Required = ctlIcon.Required

                    ctlIconLarge.ShowLog = ctlIcon.ShowLog
                    ctlIconLarge.ShowNewWindow = ctlIcon.ShowNewWindow
                    ctlIconLarge.ShowTrack = ctlIcon.ShowTrack
                    ctlIconLarge.FileFilter = ctlIcon.FileFilter
                    ctlIconLarge.Width = ctlIcon.Width

                    ' tab administrators can only manage their own tab
                    If Not TabPermissionController.CanAdminPage() Then
                        cboParentTab.Enabled = False
                    End If

                    ctlSkin.Width = "275px"
                    ctlSkin.SkinRoot = SkinController.RootSkin
                    ctlContainer.Width = "275px"
                    ctlContainer.SkinRoot = SkinController.RootContainer

                    ctlURL.Width = "275px"

                    rowCopySkin.Visible = False
                    rowCopyPerm.Visible = False
                    cboCopyPage.ClearSelection()
                    tblManageTabs.Visible = TabPermissionController.HasTabPermission("ADD,EDIT,COPY,MANAGE")
                    cmdUpdate.Visible = TabPermissionController.HasTabPermission("ADD,EDIT,COPY,MANAGE")
                    Select Case strAction
                        Case "", "add"       ' add
                            CheckQuota()
                            rowTemplate1.Visible = True
                            rowTemplate2.Visible = True
                            dshCopy.Visible = TabPermissionController.CanCopyPage()
                            tblCopy.Visible = TabPermissionController.CanCopyPage()
                            cboCopyPage.SelectedIndex = 0
                            cmdDelete.Visible = False
                            ctlURL.IncludeActiveTab = True
                            ctlAudit.Visible = False
                        Case "edit"
                            Dim tabCtrl As New TabController
                            rowCopyPerm.Visible = (TabPermissionController.CanAdminPage() AndAlso tabCtrl.GetTabsByPortal(PortalId).DescendentsOf(TabId).Count > 0)
                            rowCopySkin.Visible = True
                            dshCopy.Visible = False
                            tblCopy.Visible = False
                            cmdDelete.Visible = TabPermissionController.CanDeletePage() AndAlso Not TabController.IsSpecialTab(TabId, PortalSettings)
                            ctlURL.IncludeActiveTab = False
                            ctlAudit.Visible = True
                        Case "copy"
                            CheckQuota()
                            dshCopy.Visible = TabPermissionController.CanCopyPage()
                            tblCopy.Visible = TabPermissionController.CanCopyPage()
                            If Not cboCopyPage.Items.FindByValue(TabId.ToString) Is Nothing Then
                                cboCopyPage.Items.FindByValue(TabId.ToString).Selected = True
                                DisplayTabModules()
                            End If
                            cmdDelete.Visible = False
                            ctlURL.IncludeActiveTab = True
                            ctlAudit.Visible = False
                        Case "delete"
                            If DeleteTab(TabId) Then
                                Response.Redirect(AddHTTP(PortalAlias.HTTPAlias), True)
                            Else
                                strAction = "edit"
                                dshCopy.Visible = False
                                tblCopy.Visible = False
                                cmdDelete.Visible = TabPermissionController.CanDeletePage()
                            End If
                            ctlURL.IncludeActiveTab = False
                            ctlAudit.Visible = True
                    End Select

                    BindTab()

                    'Set the tab id of the permissions grid to the TabId (Note If in add mode
                    'this means that the default permissions inherit from the parent)
                    If strAction = "edit" OrElse strAction = "delete" OrElse Not TabPermissionController.CanAdminPage() Then
                        dgPermissions.TabID = TabId
                    Else
                        If cboParentTab.SelectedItem IsNot Nothing Then
                            dgPermissions.TabID = cboParentTab.SelectedValue
                        Else
                            dgPermissions.TabID = TabController.CurrentPage.TabID
                        End If
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub rblCacheIncludeExclude_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblCacheIncludeExclude.SelectedIndexChanged
            ShowCacheIncludeExcludeRows()
        End Sub
        Protected Sub cboCacheProvider_Change(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCacheProvider.SelectedIndexChanged
            ShowCacheRows()
        End Sub
        Protected Sub cmdClearPageCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdClearPageCache.Click
            OutputCache.OutputCachingProvider.Instance(cboCacheProvider.SelectedValue).Remove(Me.TabId)
            ShowCacheRows()
        End Sub
        Protected Sub cmdClearAllPageCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdClearAllPageCache.Click
            OutputCache.OutputCachingProvider.Instance(cboCacheProvider.SelectedValue).PurgeCache(Me.PortalId)
            ShowCacheRows()
        End Sub
        Private Sub ShowCacheRows()
            If cboCacheProvider.SelectedValue <> "" Then
                trCacheDuration.Visible = True
                trCacheIncludeExclude.Visible = True
                trMaxVaryByCount.Visible = True
                cmdClearAllPageCache.Visible = True
                cmdClearPageCache.Visible = True
                ShowCacheIncludeExcludeRows()
                trCacheStatus.Visible = True
                Dim cachedItemCount As Integer = OutputCache.OutputCachingProvider.Instance(cboCacheProvider.SelectedValue).GetItemCount(Me.TabId)
                If cachedItemCount = 0 Then
                    cmdClearAllPageCache.Enabled = False
                    cmdClearPageCache.Enabled = False
                Else
                    cmdClearAllPageCache.Enabled = True
                    cmdClearPageCache.Enabled = True
                End If
                lblCachedItemCount.Text = String.Format(Localization.GetString("lblCachedItemCount.Text", Me.LocalResourceFile), cachedItemCount)

            Else
                trCacheStatus.Visible = False
                trCacheDuration.Visible = False
                trCacheIncludeExclude.Visible = False
                trMaxVaryByCount.Visible = False
                trExcludeVaryBy.Visible = False
                trIncludeVaryBy.Visible = False
            End If

        End Sub
        Private Sub ShowCacheIncludeExcludeRows()
            If rblCacheIncludeExclude.SelectedItem Is Nothing Then
                rblCacheIncludeExclude.Items(0).Selected = True
            End If
            If rblCacheIncludeExclude.SelectedValue = "0" Then
                trExcludeVaryBy.Visible = False
                trIncludeVaryBy.Visible = True
            Else
                trExcludeVaryBy.Visible = True
                trIncludeVaryBy.Visible = False
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            trRedirect.Visible = Not (ctlURL.UrlType = "N")
        End Sub

        Private Sub cboCopyPage_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCopyPage.SelectedIndexChanged
            DisplayTabModules()
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
            LoadTemplates()
        End Sub

        Protected Sub cboParentTab_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboParentTab.SelectedIndexChanged
            BindBeforeAfterTabControls()

            If cboPositionTab.Items.Count > 0 Then
                trInsertPositionRow.Visible = True
            Else
                trInsertPositionRow.Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Dim strURL As String = NavigateURL()

                If Not Request.QueryString("returntabid") Is Nothing Then
                    ' return to admin tab
                    strURL = NavigateURL(Convert.ToInt32(Request.QueryString("returntabid")))
                End If

                Response.Redirect(strURL, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdCopyPerm_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCopyPerm.Click
            Try
                TabController.CopyPermissionsToChildren(New TabController().GetTab(TabId, PortalId, False), dgPermissions.Permissions)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("PermissionsCopied", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch ex As Exception
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("PermissionCopyError", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub cmdCopySkin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCopySkin.Click
            Try
                TabController.CopyDesignToChildren(New TabController().GetTab(TabId, PortalId, False), ctlSkin.SkinSrc, ctlContainer.SkinSrc)
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DesignCopied", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            Catch ex As Exception
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("DesignCopyError", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [VMasanas]  30/09/2004  When a parent tab is deleted all child are also marked as deleted.
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal Sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
            Try
                If DeleteTab(TabId) Then
                    Dim strURL As String = GetPortalDomainName(PortalAlias.HTTPAlias, Request)

                    If Not Request.QueryString("returntabid") Is Nothing Then
                        ' return to admin tab
                        strURL = NavigateURL(Convert.ToInt32(Request.QueryString("returntabid").ToString))
                    End If

                    Response.Redirect(strURL, True)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal Sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    Dim intTabId As Integer = SaveTabData(strAction)

                    If intTabId <> Null.NullInteger Then
                        Dim strURL As String = NavigateURL(intTabId)

                        If Not Request.QueryString("returntabid") Is Nothing Then
                            ' return to admin tab
                            strURL = NavigateURL(Convert.ToInt32(Request.QueryString("returntabid").ToString))
                        Else
                            If ctlURL.Url <> "" Or chkDisableLink.Checked Then
                                ' redirect to current tab if URL was specified ( add or copy )
                                strURL = NavigateURL(TabId)
                            End If
                        End If

                        Response.Redirect(strURL, True)
                    End If
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub rbInsertPosition_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbInsertPosition.SelectedIndexChanged
            If rbInsertPosition.SelectedValue = "AtEnd" Then
                cboPositionTab.Visible = False
                cboPositionTab.SelectedIndex = -1
            Else
                cboPositionTab.Visible = True
            End If
        End Sub

#End Region

    End Class

End Namespace
