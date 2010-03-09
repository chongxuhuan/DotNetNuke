Imports DotNetNuke
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions.Exceptions
Imports DotNetNuke.UI
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Common.Globals
Imports System.IO
Imports System.Collections.Generic
Imports DotNetNuke.Services.Sitemap
Imports Telerik.Web.UI

Namespace DotNetNuke.Modules.Admin.Sitemap
    Partial Public Class SitemapSettings
        Inherits Entities.Modules.PortalModuleBase


#Region "Page Events"

        Public Sub New()
            DotNetNuke.Framework.jQuery.RequestRegistration()
            DotNetNuke.Framework.AJAX.RegisterScriptManager()
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Page.IsPostBack = False Then

                    LoadConfiguration()

                    If IsChildPortal(PortalSettings, Context) Then
                        lnkSiteMapUrl.Text = AddHTTP(GetDomainName(Request)) & "/SiteMap.aspx?portalid=" & _
                                             PortalId.ToString
                    Else
                        lnkSiteMapUrl.Text = AddHTTP(PortalSettings.PortalAlias.HTTPAlias) & "/SiteMap.aspx"
                    End If

                    lnkSiteMapUrl.NavigateUrl = lnkSiteMapUrl.Text

                    Dim ProviderEditCol = New GridEditCommandColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderEditCol)

                    Dim ProviderNameCol = New GridBoundColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderNameCol)

                    Dim ProviderDescriptionCol = New GridBoundColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderDescriptionCol)

                    Dim ProviderPriorityOverrideCol = New GridCheckBoxColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderPriorityOverrideCol)

                    Dim ProviderPriorityCol = New GridBoundColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderPriorityCol)

                    Dim ProviderEnabledCol = New GridCheckBoxColumn()
                    grdProviders.MasterTableView.Columns.Add(ProviderEnabledCol)

                    grdProviders.MasterTableView.EditMode = GridEditMode.InPlace

                    ProviderEditCol.HeaderText = String.Empty
                    ProviderEditCol.HeaderStyle.Width = 0

                    ProviderNameCol.DataField = "Name"
                    ProviderNameCol.ReadOnly = True
                    ProviderNameCol.HeaderText = Localization.GetString("Name.Header", Me.LocalResourceFile)

                    ProviderDescriptionCol.DataField = "Description"
                    ProviderDescriptionCol.ReadOnly = True
                    ProviderDescriptionCol.HeaderText = Localization.GetString("Description.Header", Me.LocalResourceFile)

                    ProviderPriorityCol.DataField = "Priority"
                    ProviderPriorityCol.HeaderStyle.Width = 0
                    ProviderPriorityCol.HeaderText = Localization.GetString("Priority.Header", Me.LocalResourceFile)

                    ProviderPriorityOverrideCol.DataField = "OverridePriority"
                    ProviderPriorityOverrideCol.HeaderStyle.Width = 0
                    ProviderPriorityOverrideCol.HeaderText = Localization.GetString("ProviderOverride.Header", Me.LocalResourceFile)

                    ProviderEnabledCol.DataField = "Enabled"
                    ProviderEnabledCol.HeaderStyle.Width = 0
                    ProviderEnabledCol.HeaderText = Localization.GetString("ProviderEnabled.Header", Me.LocalResourceFile)

                    BindProviders()
                    SetSearchEngineSubmissionURL()
                End If

                grdProviders_needDataSorce()

                AddHandler grdProviders.NeedDataSource, AddressOf grdProviders_needDataSorce
                AddHandler grdProviders.ItemCommand, AddressOf grdProviders_itemCommand

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Configuration Handlers"

        Private Sub lnkSaveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSaveAll.Click

            SavePrioritySettings()

            PortalController.UpdatePortalSetting(PortalId, "SitemapIncludeHidden", chkIncludeHidden.Checked.ToString())
            PortalController.UpdatePortalSetting(PortalId, "SitemapExcludePriority", txtExcludePriority.Text)

            If (cmbDaysToCache.SelectedIndex = 0) Then
                PortalController.UpdatePortalSetting(PortalId, "SitemapCached", Boolean.FalseString)
                ResetCache()
            Else
                PortalController.UpdatePortalSetting(PortalId, "SitemapCached", Boolean.TrueString)
            End If

            PortalController.UpdatePortalSetting(PortalId, "SitemapCacheDays", cmbDaysToCache.SelectedIndex)

            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("MessageUpdated", LocalResourceFile), _
                                                      Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)

            LoadConfiguration()

        End Sub

        Private Sub LoadConfiguration()
            ' core settings
            chkLevelPriority.Checked = _
                Boolean.Parse(PortalController.GetPortalSetting("SitemapLevelMode", PortalId, "False"))
            Dim minPriority As Single
            minPriority = Single.Parse(PortalController.GetPortalSetting("SitemapMinPriority", PortalId, "0.1"), NumberFormatInfo.InvariantInfo())
            txtMinPagePriority.Text = minPriority.ToString()
            chkIncludeHidden.Checked = _
                Boolean.Parse(PortalController.GetPortalSetting("SitemapIncludeHidden", PortalId, "False"))

            ' General settings
            txtExcludePriority.Text = PortalController.GetPortalSetting("SitemapExcludePriority", PortalId, "0")

            ' If chkCache.Checked Then
            ' txtCacheDays.Text = PortalController.GetPortalSetting("SitemapCacheDays", PortalId, "1")

            Dim cahceEnabled = Boolean.Parse(PortalController.GetPortalSetting("SitemapCached", PortalId, "False"))
            cmbDaysToCache.SelectedIndex = PortalController.GetPortalSetting("SitemapCacheDays", PortalId, "1")
            ' End If
            'lnkResetCache.Visible = chkCache.Checked
        End Sub

        Private Sub SavePrioritySettings()
            PortalController.UpdatePortalSetting(PortalId, "SitemapLevelMode", chkLevelPriority.Checked.ToString())

            If Single.Parse(txtMinPagePriority.Text) < 0 Then
                txtMinPagePriority.Text = "0"
            End If
            Dim minPriority As Single = Single.Parse(txtMinPagePriority.Text)
            PortalController.UpdatePortalSetting(PortalId, "SitemapMinPriority", minPriority.ToString(NumberFormatInfo.InvariantInfo))

        End Sub

#End Region


        Private Sub ResetCache()

            Dim CacheFolder = New DirectoryInfo(PortalSettings.HomeDirectoryMapPath + "sitemap\")

            If CacheFolder.Exists Then
                For Each file In CacheFolder.GetFiles("sitemap*.xml")
                    file.Delete()
                Next

                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("ResetOK", LocalResourceFile), _
                                                           Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            End If

        End Sub

        Private Function IsChildPortal(ByVal ps As PortalSettings, ByVal context As HttpContext) As Boolean
            Dim isChild As Boolean = False
            Dim portalName As String
            Dim aliasController As New PortalAliasController
            Dim arr As ArrayList = aliasController.GetPortalAliasArrayByPortalID(ps.PortalId)
            Dim serverPath As String = DotNetNuke.Common.Globals.GetAbsoluteServerPath(context.Request)

            If arr.Count > 0 Then
                Dim portalAlias As PortalAliasInfo = CType(arr(0), PortalAliasInfo)
                portalName = DotNetNuke.Common.Globals.GetPortalDomainName(ps.PortalAlias.HTTPAlias)
                If Convert.ToBoolean(InStr(1, portalAlias.HTTPAlias, "/")) Then
                    portalName = Mid(portalAlias.HTTPAlias, InStrRev(portalAlias.HTTPAlias, "/") + 1)
                End If
                If portalName <> "" AndAlso System.IO.Directory.Exists(serverPath & portalName) Then
                    isChild = True
                End If
            End If
            Return isChild
        End Function


        Private Sub BindProviders()

            Dim builder As New SitemapBuilder(PortalSettings)
            grdProviders.DataSource = CType(builder.Providers, IEnumerable)

        End Sub

        Protected Sub grdProviders_itemCommand(ByVal [source] As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles grdProviders.ItemCommand
            If e.CommandName = RadGrid.UpdateCommandName Then
                If Not Page.IsValid Then
                    e.Canceled = True
                End If
            End If
        End Sub 'RadGrid1_ItemCommand

        Protected Sub grdProvider_UpdateCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) _
            Handles grdProviders.UpdateCommand

            grdProviders.Rebind()

            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)
            Dim editMan As GridEditManager = editedItem.EditManager

            Dim editedSiteMap = CType(e.Item.DataItem, SitemapProvider)

            Dim column As GridColumn

            Dim nameCol As GridColumn = DirectCast(DirectCast(source, DotNetNuke.Web.UI.WebControls.DnnGrid).Columns.FindByUniqueName("Name"), Telerik.Web.UI.GridColumn)
            Dim nameEditor As IGridColumnEditor = editMan.GetColumnEditor(nameCol)
            Dim key As String = CType(nameEditor, GridTextColumnEditor).Text

            Dim providers = CType(grdProviders.DataSource, List(Of SitemapProvider))
            Dim editedProvider As SitemapProvider = Nothing

            For Each p In providers

                If (String.Equals(key, p.Name, StringComparison.InvariantCultureIgnoreCase)) Then
                    editedProvider = p
                End If
            Next

            Dim providerEnabled As Boolean
            Dim providerPriorityString = String.Empty
            Dim providerOverride As Boolean

            For Each column In e.Item.OwnerTableView.Columns
                If TypeOf column Is IGridEditableColumn Then
                    Dim editableCol As IGridEditableColumn = CType(column, IGridEditableColumn)


                    If (editableCol.IsEditable) Then

                        Dim editor As IGridColumnEditor = editMan.GetColumnEditor(editableCol)

                        Dim editorType As String = CType(editor, Object).ToString()
                        Dim editorText As String = "unknown"
                        Dim editorValue As Object = Nothing

                        If (TypeOf editor Is GridTextColumnEditor) Then
                            editorText = CType(editor, GridTextColumnEditor).Text
                            editorValue = CType(editor, GridTextColumnEditor).Text
                        End If

                        If (TypeOf editor Is GridBoolColumnEditor) Then
                            editorText = CType(editor, GridBoolColumnEditor).Value.ToString()
                            editorValue = CType(editor, GridBoolColumnEditor).Value
                        End If

                        If (column.UniqueName = "Enabled") Then
                            providerEnabled = editorValue
                        ElseIf (column.UniqueName = "Priority") Then
                            providerPriorityString = editorValue
                        ElseIf (column.UniqueName = "OverridePriority") Then
                            providerOverride = editorValue
                        End If
                    End If
                End If
            Next

            Dim providerPriority As Single

            If (Single.TryParse(providerPriorityString, providerPriority)) Then
                editedProvider.Enabled = providerEnabled
                editedProvider.OverridePriority = providerOverride
                editedProvider.Priority = providerPriority
            Else
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("valPriority.Text", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If


        End Sub


        Private Sub grdProviders_needDataSorce()
            BindProviders()
        End Sub


        Protected Sub lnkResetCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles lnkResetCache.Click
            ResetCache()
        End Sub


#Region "Site Submission"

        Private Sub SetSearchEngineSubmissionURL()
            Try
                If Not cboSearchEngine.SelectedItem Is Nothing Then
                    Dim strURL As String = ""
                    Select Case cboSearchEngine.SelectedItem.Text.ToLower().Trim()
                        Case "google"
                            strURL += "http://www.google.com/addurl?q=" & _
                                      HTTPPOSTEncode(AddHTTP(GetDomainName(Request)))
                            strURL += "&dq="
                            If PortalSettings.PortalName <> "" Then
                                strURL += HTTPPOSTEncode(PortalSettings.PortalName)
                            End If
                            If PortalSettings.Description <> "" Then
                                strURL += HTTPPOSTEncode(PortalSettings.Description)
                            End If
                            If PortalSettings.KeyWords <> "" Then
                                strURL += HTTPPOSTEncode(PortalSettings.KeyWords)
                            End If
                            strURL += "&submit=Add+URL"
                        Case "yahoo!"
                            strURL = "http://siteexplorer.search.yahoo.com/submit"
                        Case "bing"
                            strURL = "http://www.bing.com/webmaster"
                    End Select

                    cmdSubmitSitemap.NavigateUrl = strURL
                    cmdSubmitSitemap.Target = "_blank"

                    'UrlUtils.OpenNewWindow(Me.Page, Me.GetType(), strURL)
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cboSearchEngine_SelectedIndexChanged(ByVal o As Object, ByVal e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs) Handles cboSearchEngine.SelectedIndexChanged
            SetSearchEngineSubmissionURL()
        End Sub

        Protected Sub cmdVerification_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles cmdVerification.Click

            If txtVerification.Text <> "" AndAlso txtVerification.Text.EndsWith(".html") Then
                If Not File.Exists(ApplicationMapPath & "\" & txtVerification.Text) Then
                    ' write SiteMap verification file
                    Dim objStream As StreamWriter
                    objStream = File.CreateText(ApplicationMapPath & "\" & txtVerification.Text)
                    objStream.WriteLine("Google SiteMap Verification File")
                    objStream.WriteLine(" - " & lnkSiteMapUrl.Text)
                    objStream.WriteLine(" - " & UserInfo.DisplayName)
                    objStream.Close()
                End If
            End If
        End Sub

#End Region
    End Class
End Namespace
