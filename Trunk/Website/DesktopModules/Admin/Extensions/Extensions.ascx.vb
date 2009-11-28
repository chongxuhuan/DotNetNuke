'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Application


Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Extensions Module Control is used to manage the Extensions
    ''' installed in this portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Extensions
        Inherits ModuleUserControlBase
        Implements Entities.Modules.IActionable

#Region "Private Members"

        Private _Mode As String = "All"

#End Region

#Region "Public Properties"

        Public ReadOnly Property PackageType() As String
            Get
                Return cboPackageTypes.SelectedValue
            End Get
        End Property

        Public ReadOnly Property Mode() As String
            Get
                If Not String.IsNullOrEmpty(ModuleContext.Settings("Extensions_Mode")) Then
                    _Mode = ModuleContext.Settings("Extensions_Mode")
                End If
                Return _Mode
            End Get
        End Property

#End Region

#Region "Private Methods"

		Protected Function GetIsPackageInUseInfo(ByVal dataItem As Object) As String
			If (TypeOf dataItem Is PackageInfo) Then
				Dim package As PackageInfo = DirectCast(dataItem, PackageInfo)

				If (package.PackageType.ToUpper() = "MODULE") Then
					If (PackagesInUse.ContainsKey(package.PackageID)) Then
						Return "<a href=""" & ModuleContext.EditUrl("PackageID", package.PackageID.ToString(), "UsageDetails") & """>Yes</a>"
					Else
						Return "No"
					End If
				End If
			End If

			Return String.Empty
		End Function

		Private Sub AddModulesToList(ByVal packages As List(Of PackageInfo))
			Dim portalModules As Dictionary(Of Integer, PortalDesktopModuleInfo) = DesktopModuleController.GetPortalDesktopModulesByPortalID(Me.ModuleContext.PortalId)
			For Each modulePackage As PackageInfo In PackageController.GetPackagesByType(Null.NullInteger, "Module")
				'Get DesktopModule
				Dim desktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByPackageID(modulePackage.PackageID)

				For Each portalModule As PortalDesktopModuleInfo In portalModules.Values
					If desktopModule IsNot Nothing AndAlso portalModule.DesktopModuleID = desktopModule.DesktopModuleID Then
						packages.Add(modulePackage)
					End If
				Next
			Next
		End Sub

		Private Sub BindGrid()
			Dim packages As New List(Of PackageInfo)
			Select Case Mode
				Case "All"
					If PackageType = "" Then
                        packages = PackageController.GetPackages(Null.NullInteger)
                        'If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                        'Else
                        '    'Get auth systems
                        '    packages = PackageController.GetPackagesByType(Null.NullInteger, "Auth_System")
                        '    packages.AddRange(PackageController.GetPackagesByType(Me.ModuleContext.PortalId, "Container"))
                        '    packages.AddRange(PackageController.GetPackagesByType(Null.NullInteger, "CoreLanguagePack"))

                        '    AddModulesToList(packages)

                        '    packages.AddRange(PackageController.GetPackagesByType(Me.ModuleContext.PortalId, "Skin"))
                        'End If
					Else
						Select Case PackageType
							Case "Module"
								If Not Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
									AddModulesToList(packages)
								Else
									packages = PackageController.GetPackagesByType(Null.NullInteger, "Module")
								End If
							Case "Skin", "Container"
								If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
									packages = PackageController.GetPackagesByType(Null.NullInteger, PackageType)
								Else
									packages = PackageController.GetPackagesByType(Me.ModuleContext.PortalId, PackageType)
								End If
							Case Else
								packages = PackageController.GetPackagesByType(Null.NullInteger, PackageType)
						End Select
					End If
				Case "LanguagePack"
					packages = PackageController.GetPackagesByType(Null.NullInteger, "CoreLanguagePack")
					packages.AddRange(PackageController.GetPackagesByType(Null.NullInteger, "ExtensionLanguagePack"))
				Case "Module"
					If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
						packages = PackageController.GetPackagesByType(Null.NullInteger, "Module")
					Else
						AddModulesToList(packages)
					End If
					grdExtensions.DataSource = packages
				Case "Skin"
					packages = PackageController.GetPackagesByType(Null.NullInteger, "Skin")
					packages.AddRange(PackageController.GetPackagesByType(Null.NullInteger, "Container"))
			End Select

			If packages.Count > 0 Then
				lblNoResults.Visible = False
				grdExtensions.Visible = True
				grdExtensions.DataSource = packages
				grdExtensions.DataBind()
			Else
				grdExtensions.Visible = False
				lblNoResults.Visible = True
			End If

			If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
				Select Case Mode
					Case "LanguagePack"
						trLanguageSelector.Visible = False
					Case Else
						trLanguageSelector.Visible = Not Request.IsLocal OrElse (PackageType = "CoreLanguagePack" OrElse PackageType = "ExtensionLanguagePack")
				End Select
			Else
				trLanguageSelector.Visible = False
			End If

		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' UpgradeIndicator returns the imageurl for the upgrade button for the module
		''' </summary>
		''' <returns></returns>
		''' <history>
		''' 	[cnurse]	08/22/2007	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Function UpgradeIndicator(ByVal version As System.Version, ByVal packageType As String, ByVal packageName As String, ByVal culture As String) As String
			Dim strURL As String = DotNetNuke.Services.Upgrade.Upgrade.UpgradeIndicator(version, packageType, packageName, culture, Request.IsLocal, Request.IsSecureConnection)
			If strURL = "" Then
				strURL = Common.Globals.ApplicationPath & "/images/spacer.gif"
			End If
			Return strURL
		End Function

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' UpgradeRedirect returns the url for the upgrade button for the module
		''' </summary>
		''' <returns></returns>
		''' <history>
		''' 	[cnurse]	08/22/2007	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Function UpgradeRedirect(ByVal version As System.Version, ByVal packageType As String, ByVal packageName As String, ByVal culture As String) As String
			Return DotNetNuke.Services.Upgrade.Upgrade.UpgradeRedirect(version, packageType, packageName, culture)
		End Function

#End Region

#Region "Protected Methods"

		Protected Function GetAboutTooltip(ByRef dataItem As Object) As String
            Dim returnValue As String = String.Empty
            Try
                If (Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab) Then
                    Dim portalID As Integer = Convert.ToInt32(DataBinder.Eval(dataItem, "PortalID"))

                    If (Not portalID = Null.NullInteger AndAlso Not portalID = Integer.MinValue) Then
                        Dim portal As PortalInfo = PortalCtrl.GetPortal(portalID)
                        returnValue = String.Format( _
                         Localization.GetString("InstalledOnPortal.Tooltip", LocalResourceFile), _
                         portal.PortalName)
                    Else
                        returnValue = Localization.GetString("InstalledOnHost.Tooltip", LocalResourceFile)
                    End If
                End If
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try

			Return returnValue
		End Function

		Protected Function FormatVersion(ByVal version As Object) As String
			Dim package As PackageInfo = TryCast(version, PackageInfo)
			Dim retValue As String = Null.NullString
			If package IsNot Nothing Then
				retValue = package.Version.ToString(3)
			End If
			Return retValue
		End Function

		Protected Function GetPackageTypeDescription(ByVal typeKey As String) As String
			Dim returnValue As String = typeKey
			If (PackageTypesList().ContainsKey(typeKey)) Then
				returnValue = PackageTypesList()(typeKey).Description
			End If
			Return returnValue
		End Function

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)

            For Each column As DataGridColumn In grdExtensions.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If
                    If imageColumn.CommandName = "Delete" Then
                        'The Friendly URL parser does not like non-alphanumeric characters
                        'so first create the format string with a dummy value and then
                        'replace the dummy value with the FormatString place holder
                        Dim params(1) As String
                        params(0) = "rtab=" & Me.ModuleContext.TabId.ToString()
                        params(1) = "packageId=KEYFIELD"

                        Dim formatString As String = NavigateURL(Me.ModuleContext.TabId, "UnInstall", params)
                        formatString = formatString.Replace("KEYFIELD", "{0}")
                        imageColumn.NavigateURLFormatString = formatString
                    End If
                    If imageColumn.CommandName = "Edit" Then
                        'The Friendly URL parser does not like non-alphanumeric characters
                        'so first create the format string with a dummy value and then
                        'replace the dummy value with the FormatString place holder
                        Dim formatString As String = ModuleContext.EditUrl("PackageID", "KEYFIELD", "Edit")
                        formatString = formatString.Replace("KEYFIELD", "{0}")
                        imageColumn.NavigateURLFormatString = formatString
                    End If
                End If
            Next
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Not Page.IsPostBack Then
                'Localize the Data Grid
                Localization.LocalizeDataGrid(grdExtensions, LocalResourceFile)

                If DotNetNuke.Services.Upgrade.Upgrade.UpgradeIndicator(DotNetNukeContext.Current.Application.Version, "", "~", "", Request.IsLocal, Request.IsSecureConnection) = "" Then
                    lblUpdate.Visible = False
					grdExtensions.Columns(8).HeaderText = ""
                End If

                Select Case Mode
                    Case "All"
						cboPackageTypes.DataSource = PackageTypesList().Values
                        cboPackageTypes.DataBind()
                        cboPackageTypes.Items.Insert(0, New ListItem(Localization.GetString("Not_Specified", Localization.SharedResourceFile), ""))
                        cboPackageTypes.SelectedIndex = 0
                    Case "LanguagePack"
                        trPackageType.Visible = False
                    Case Else
                        trPackageType.Visible = False
                End Select

                cboLocales.DataSource = Localization.GetLocales(Null.NullInteger).Values
                cboLocales.DataBind()
                cboLocales.Items.Insert(0, New ListItem(Localization.GetString("Not_Specified", Localization.SharedResourceFile), ""))
            End If

            BindGrid()
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            For Each column As DataGridColumn In grdExtensions.Columns
                If column.HeaderText = "Type" Then
					column.Visible = Mode = "Skin"
                End If
				If column.FooterText = "Portal" Then
					column.Visible = (Me.ModuleContext.IsHostMenu AndAlso (Mode = "Skin" OrElse (Mode = "All" AndAlso (PackageType = "" OrElse PackageType = "Skin" OrElse PackageType = "Container"))))
				End If
			Next
            MyBase.OnPreRender(e)
        End Sub

		Private Function CreateGridGroupHeader(ByVal title As String, ByVal paddingTop As String, ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) As DataGridItem
			Dim groupHeader As HtmlGenericControl = New HtmlControls.HtmlGenericControl("div")
			groupHeader.Attributes.Add("class", "Head")
			groupHeader.InnerHtml = title

			If (paddingTop = String.Empty) Then
				paddingTop = "5px"
			End If

			Dim cell As New TableCell()
			cell.Controls.Add(groupHeader)
			cell.Controls.Add(New HtmlControls.HtmlGenericControl("hr"))
			cell.ColumnSpan = e.Item.Cells.Count
			cell.HorizontalAlign = HorizontalAlign.Left
			cell.Style.Add("padding-bottom", "5px")
			cell.Style.Add("padding-top", "20px")

			Dim gridItem As New DataGridItem(e.Item.ItemIndex + 1, 0, ListItemType.Item)
			gridItem.Controls.Add(cell)

			Return gridItem
		End Function

#End Region

#Region "Public Methods"

        Public Function UpgradeService(ByVal version As System.Version, ByVal packageType As String, ByVal packageName As String) As String
            Dim strUpgradeService As String = ""
            strUpgradeService += "<a title=""" & Localization.GetString("UpgradeMessage", Me.LocalResourceFile) & """ href=""" & UpgradeRedirect(version, packageType, packageName, "") & """ target=""_new""><img title=""" & Localization.GetString("UpgradeMessage", Me.LocalResourceFile) & """ src=""" & UpgradeIndicator(version, packageType, packageName, "") & """ border=""0"" /></a>"
            If Not String.IsNullOrEmpty(cboLocales.SelectedValue) Then
                strUpgradeService += "<br />"
                strUpgradeService += "<a title=""" & Localization.GetString("LanguageMessage", Me.LocalResourceFile) & """ href=""" & UpgradeRedirect(version, packageType, packageName, cboLocales.SelectedItem.Value) & """ target=""_new""><img title=""" & Localization.GetString("LanguageMessage", Me.LocalResourceFile) & """ src=""" & UpgradeIndicator(version, packageType, packageName, cboLocales.SelectedItem.Value) & """ border=""0"" /></a>"
            End If
            Return strUpgradeService
        End Function

#End Region

#Region "Properties"

		Private _PackageTypes As IDictionary(Of String, PackageType) = Nothing
		Protected ReadOnly Property PackageTypesList() As IDictionary(Of String, PackageType)
			Get
				If (_PackageTypes Is Nothing) Then
                    _PackageTypes = New Dictionary(Of String, PackageType)
                    For Each packageType As PackageType In PackageController.GetPackageTypes()
                        _PackageTypes.Add(packageType.PackageType, packageType)
                    Next
				End If
                Return _PackageTypes
			End Get
		End Property

		Private _PackagesInUse As IDictionary(Of Integer, PackageInfo) = Nothing
		Protected ReadOnly Property PackagesInUse() As IDictionary(Of Integer, PackageInfo)
			Get
				If (_PackagesInUse Is Nothing) Then
					_PackagesInUse = PackageController.GetModulePackagesInUse( _
					 PortalController.GetCurrentPortalSettings().PortalId, _
					 Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab)
				End If

				Return _PackagesInUse
			End Get
		End Property

		Private _PortalCtrl As PortalController = Nothing
		Protected ReadOnly Property PortalCtrl() As PortalController
			Get
				If (_PortalCtrl Is Nothing) Then
					_PortalCtrl = New PortalController()
				End If

				Return _PortalCtrl
			End Get
		End Property

#End Region

#Region "Event Handlers"

		Protected Sub cboLocales_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboLocales.SelectedIndexChanged
			BindGrid()
		End Sub

		Protected Sub cboPackageTypes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPackageTypes.SelectedIndexChanged
			BindGrid()
		End Sub

		Private _CurrentGroupKey As String = String.Empty
		Protected Sub grdExtensions_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdExtensions.ItemCreated
			If (PackageType = "" AndAlso Mode = "All" AndAlso Not e.Item Is Nothing AndAlso Not e.Item.DataItem Is Nothing) Then
				If (e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item) Then
					'Add Group Headings
					Dim groupTitleKey As String = DataBinder.Eval(e.Item.DataItem, "PackageType").ToString()
					If (Not _CurrentGroupKey = groupTitleKey) Then
						'lookup text from the list
						Dim grid As DataGrid = DirectCast(sender, DataGrid)
						Dim childTable As Table = DirectCast(grid.Controls(0), Table)
						childTable.Rows.Add(CreateGridGroupHeader(GetPackageTypeDescription(groupTitleKey), "20px", sender, e))

						_CurrentGroupKey = groupTitleKey
					End If
				End If
			End If
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' grdPackages_ItemDataBound runs when a row in the grid is bound
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	10/10/2007	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		Protected Sub grdPackages_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdExtensions.ItemDataBound
			Dim item As DataGridItem = e.Item

			If item.ItemType = ListItemType.Item Or _
					item.ItemType = ListItemType.AlternatingItem Or _
					item.ItemType = ListItemType.SelectedItem Then

				Dim editHyperlink As HyperLink = TryCast(item.Controls(1).Controls(0), HyperLink)
				If editHyperlink IsNot Nothing Then
					Dim package As PackageInfo = CType(item.DataItem, PackageInfo)
					If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
						If package.IsSystemPackage Then
							editHyperlink.Visible = False
						Else
							editHyperlink.Visible = PackageController.CanDeletePackage(package, ModuleContext.PortalSettings)
						End If
					Else
						editHyperlink.Visible = False
					End If
				End If
				editHyperlink = TryCast(item.Controls(0).Controls(0), HyperLink)
				If editHyperlink IsNot Nothing Then
					editHyperlink.Visible = Me.ModuleContext.IsEditable
				End If
			End If

			If PackageType = "" AndAlso Mode = "All" Then
				If e.Item.ItemIndex = 0 Then
					Dim row As DataGridItem = New DataGridItem(0, 0, ListItemType.Item)
					Dim cell1 As TableCell = New TableCell()
					cell1.Text = ""
					row.Cells.Add(cell1)
					Dim cell2 As TableCell = New TableCell()
					cell2.Text = ""
					row.Cells.Add(cell2)
					Dim cell4 As TableCell = New TableCell()
					If Localization.GetString("AppTitle", Me.LocalResourceFile) = "" Then
						cell4.Text = DotNetNukeContext.Current.Application.Title
					Else
						cell4.Text = Localization.GetString("AppTitle", Me.LocalResourceFile)
					End If
					row.Cells.Add(cell4)
					Dim cell5 As TableCell = New TableCell()
					Dim appType As String = Localization.GetString("AppType", Me.LocalResourceFile)
					If (appType = String.Empty) Then
						appType = DotNetNukeContext.Current.Application.Type
					End If
					cell5.Text = appType
					row.Cells.Add(cell5)
					Dim cell6 As TableCell = New TableCell()
					cell6.Text = ""
					row.Cells.Add(cell6)
					Dim cell7 As TableCell = New TableCell()
					If Localization.GetString("AppDescription", Me.LocalResourceFile) = "" Then
						cell7.Text = DotNetNukeContext.Current.Application.Description
					Else
						cell7.Text = Localization.GetString("AppDescription", Me.LocalResourceFile)
					End If
					row.Cells.Add(cell7)
					Dim cell8 As TableCell = New TableCell()
					cell8.Text = DotNetNukeContext.Current.Application.Version.ToString(3)
					row.Cells.Add(cell8)
					Dim cell9 As TableCell = New TableCell()
                    cell9.Text = UpgradeService(DotNetNukeContext.Current.Application.Version, DotNetNukeContext.Current.Application.Type, DotNetNukeContext.Current.Application.Name)
					row.Cells.Add(cell9)
					grdExtensions.Controls(0).Controls.AddAt(1, row)

					grdExtensions.Controls(0).Controls.AddAt(1, CreateGridGroupHeader(appType, "", sender, e))
				End If
			End If
		End Sub

#End Region

#Region "Optional Interfaces"

		Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
			Get
				Dim Actions As New ModuleActionCollection
				Select Case Mode
					Case "All"
						'' install new Extension
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("ExtensionInstall.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", Util.InstallURL(ModuleContext.TabId, ""), False, SecurityAccessLevel.Host, True, False)

						'' Create new Extension
						If Me.ModuleContext.IsHostMenu Then
							Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateExtension.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("NewExtension"), False, SecurityAccessLevel.Host, True, False)
						End If

						'' batch install new Extensions
						If Me.ModuleContext.IsHostMenu Then
							Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("InstallExtensions.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", ModuleContext.EditUrl("BatchInstall"), False, SecurityAccessLevel.Host, True, False)
						End If
					Case "LanguagePack"
						'' install new LanguagePack
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("LanguagePackInstall.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", Util.InstallURL(ModuleContext.TabId, ""), False, SecurityAccessLevel.Host, True, False)

						'' Create new LanguagePack
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateLanguagePack.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("Type", "LanguagePack", "NewExtension"), False, SecurityAccessLevel.Host, True, False)

					Case "Module"
						'' install new Module
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("ModuleInstall.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", Util.InstallURL(ModuleContext.TabId, ""), False, SecurityAccessLevel.Host, True, False)

						'' Create new Module
                        Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateModule.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("Type", "Module", "EditModuleDefinition"), False, SecurityAccessLevel.Host, True, False)

						'' Import Module Definition
                        'Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("ImportModule.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", ModuleContext.EditUrl("ImportModuleDefinition"), False, SecurityAccessLevel.Host, True, False)
					Case "Skin"
						'' install new Skin
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("SkinInstall.Action", LocalResourceFile), ModuleActionType.AddContent, "", "action_import.gif", Util.InstallURL(ModuleContext.TabId, ""), False, SecurityAccessLevel.Host, True, False)

						'' Create new Skin
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateSkin.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("Type", "Skin", "NewExtension"), False, SecurityAccessLevel.Host, True, False)

						'' Create new Container
						Actions.Add(ModuleContext.GetNextActionID, Services.Localization.Localization.GetString("CreateContainer.Action", LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", ModuleContext.EditUrl("Type", "Container", "NewExtension"), False, SecurityAccessLevel.Host, True, False)
				End Select


				Return Actions
			End Get
		End Property

#End Region

    End Class

End Namespace
