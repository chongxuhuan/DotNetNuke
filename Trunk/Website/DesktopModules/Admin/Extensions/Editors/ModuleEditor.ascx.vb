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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.EventQueue
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.Features

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ModuleEditor ModuleUserControlBase is used to edit Module Definitions
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	02/04/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ModuleEditor
        Inherits PackageEditorBase

#Region "Private Members"

        Private _DesktopModule As DesktopModuleInfo = Nothing
        Private _ModuleDefinition As ModuleDefinitionInfo = Nothing

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property DesktopModule() As DesktopModuleInfo
            Get
                If _DesktopModule Is Nothing Then
                    _DesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(PackageID)
                End If
                Return _DesktopModule
            End Get
        End Property

        Protected Overrides ReadOnly Property EditorID() As String
            Get
                Return "ModuleEditor"
            End Get
        End Property

        Protected Property IsAddDefinitionMode() As Boolean
            Get
                Dim _IsAddDefinitionMode As Boolean = Null.NullBoolean
                If ViewState("IsAddDefinitionMode") Then
                    _IsAddDefinitionMode = Convert.ToBoolean(ViewState("IsAddDefinitionMode"))
                End If
                Return _IsAddDefinitionMode
            End Get
            Set(ByVal value As Boolean)
                ViewState("IsAddDefinitionMode") = value
            End Set
        End Property

        Protected Property ModuleDefinitionID() As Integer
            Get
                Dim _ModuleDefinitionID As Integer = Null.NullInteger
                If ViewState("ModuleDefinitionID") Then
                    _ModuleDefinitionID = Convert.ToInt32(ViewState("ModuleDefinitionID"))
                End If
                Return _ModuleDefinitionID
            End Get
            Set(ByVal value As Integer)
                ViewState("ModuleDefinitionID") = value
            End Set
        End Property

        Protected ReadOnly Property ModuleDefinition() As ModuleDefinitionInfo
            Get
                If _ModuleDefinition Is Nothing Then
                    _ModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByID(ModuleDefinitionID)
                End If
                Return _ModuleDefinition
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindDefinition()
            ctlDefinition.LocalResourceFile = LocalResourceFile
            If IsAddDefinitionMode Then
                Dim definition As New ModuleDefinitionInfo
                definition.DesktopModuleID = DesktopModule.DesktopModuleID
                definition.ModuleDefID = Null.NullInteger
                ctlDefinition.DataSource = definition
                ctlDefinition.DataBind()

                cmdDeleteDefinition.Visible = False
                cmdUpdateDefinition.Text = Localization.GetString("cmdCreateDefinition", LocalResourceFile)
                pnlDefinition.Visible = True
                pnlControls.Visible = False
            Else
                If ModuleDefinition IsNot Nothing AndAlso ModuleDefinition.DesktopModuleID = DesktopModule.DesktopModuleID Then
                    ctlDefinition.DataSource = ModuleDefinition
                    ctlDefinition.DataBind()

                    cmdDeleteDefinition.Visible = True
                    cmdUpdateDefinition.Text = Localization.GetString("cmdUpdateDefinition", LocalResourceFile)

                    If Not Page.IsPostBack Then
                        Localization.LocalizeDataGrid(grdControls, LocalResourceFile)
                    End If
                    grdControls.DataSource = ModuleDefinition.ModuleControls.Values
                    grdControls.DataBind()

                    pnlDefinition.Visible = True
                    pnlControls.Visible = True
                Else
                    pnlDefinition.Visible = False
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine Binds the DesktopModule
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/04/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindDesktopModule(ByVal refreshDefinitions As Boolean)
            If DesktopModule IsNot Nothing Then
                ctlDesktopModule.LocalResourceFile = LocalResourceFile
                ctlDesktopModule.DataSource = DesktopModule
                ctlDesktopModule.DataBind()
                fldIsPremium.Visible = Not DesktopModule.IsAdmin

                If Not Page.IsPostBack Then
                    If Not (Request.QueryString("ModuleDefinitionID") Is Nothing) Then
                        ModuleDefinitionID = Int32.Parse(Request.QueryString("ModuleDefinitionID"))
                    End If
                End If

                If Not Page.IsPostBack Or refreshDefinitions Then
                    cboDefinitions.DataSource = DesktopModule.ModuleDefinitions.Values
                    cboDefinitions.DataBind()

                    If ModuleDefinitionID = Null.NullInteger AndAlso cboDefinitions.Items.Count > 0 Then
                        ModuleDefinitionID = Integer.Parse(cboDefinitions.SelectedValue)
                    End If

                    If ModuleDefinitionID <> Null.NullInteger Then
                        'Set the Combos selected value
                        Dim selectedDefinition As ListItem = cboDefinitions.Items.FindByValue(ModuleDefinitionID.ToString)
                        If selectedDefinition IsNot Nothing Then
                            cboDefinitions.SelectedIndex = -1
                            selectedDefinition.Selected = True
                        End If
                    End If
                End If

                If Not IsSuperTab Then
                    BindPermissions()
                Else
                    pnlPermissions.Visible = False
                End If

                BindPortalDesktopModules()

                BindDefinition()

                lblDefinitionError.Visible = False
            End If
        End Sub

        Private Sub BindPermissions()
            Dim portalModule As PortalDesktopModuleInfo = DesktopModuleController.GetPortalDesktopModule(ModuleContext.PortalSettings.PortalId, DesktopModule.DesktopModuleID)
            If portalModule IsNot Nothing Then
                dgPermissions.PortalDesktopModuleID = portalModule.PortalDesktopModuleID
                Dim isVisible As Boolean = DesktopModulePermissionController.HasDesktopModulePermission(DesktopModulePermissionController.GetDesktopModulePermissions(portalModule.PortalDesktopModuleID), "DEPLOY") _
                                            OrElse ModuleContext.PortalSettings.UserInfo.IsInRole(ModuleContext.PortalSettings.AdministratorRoleName) _
                                            OrElse ModuleContext.PortalSettings.UserInfo.IsSuperUser
                pnlPermissions.Visible = isVisible
                If Not isVisible Then
                    lblHelp.Text = Localization.GetString("NoPermission", LocalResourceFile)
                End If
            End If
        End Sub

        Private Sub BindPortalDesktopModules()
            Dim objPortals As New PortalController
            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim dicPortalDesktopModules As Dictionary(Of Integer, PortalDesktopModuleInfo) = DesktopModuleController.GetPortalDesktopModulesByDesktopModuleID(DesktopModule.DesktopModuleID)

            For Each objPortalDesktopModule As PortalDesktopModuleInfo In dicPortalDesktopModules.Values
                For Each objPortal As PortalInfo In arrPortals
                    If objPortal.PortalID = objPortalDesktopModule.PortalID Then
                        arrPortals.Remove(objPortal)
                        Exit For
                    End If
                Next
            Next

            ctlPortals.AvailableDataSource = arrPortals
            ctlPortals.SelectedDataSource = dicPortalDesktopModules.Values
            ctlPortals.Visible = Not DesktopModule.IsAdmin
        End Sub

        Private Sub UpdateModuleInterfaces(ByVal BusinessControllerClass As String)
            'this cannot be done directly at this time because 
            'the module may not be loaded into the app domain yet
            'So send an EventMessage that will process the update 
            'after the App recycles
            Dim oAppStartMessage As New EventQueue.EventMessage
            oAppStartMessage.Sender = ModuleContext.PortalSettings.UserInfo.Username
            oAppStartMessage.Priority = MessagePriority.High
            oAppStartMessage.ExpirationDate = Now.AddYears(-1)
            oAppStartMessage.SentDate = System.DateTime.Now
            oAppStartMessage.Body = ""
            oAppStartMessage.ProcessorType = "DotNetNuke.Entities.Modules.EventMessageProcessor, DotNetNuke"
            oAppStartMessage.ProcessorCommand = "UpdateSupportedFeatures"

            'Add custom Attributes for this message
            oAppStartMessage.Attributes.Add("BusinessControllerClass", BusinessControllerClass)
            oAppStartMessage.Attributes.Add("DesktopModuleId", DesktopModule.DesktopModuleID.ToString())

            'send it to occur on next App_Start Event
            EventQueueController.SendMessage(oAppStartMessage, "Application_Start")

            'force an app restart
            DotNetNuke.Common.Utilities.Config.Touch()
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)

            If IsSuperTab Then
                lblHelp.Text = Localization.GetString("HostHelp", LocalResourceFile)
            Else
                lblHelp.Text = Localization.GetString("AdminHelp", LocalResourceFile)
            End If

            For Each column As DataGridColumn In grdControls.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName = "Delete" Then
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                    End If
                End If
            Next
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ctlPortals.LocalResourceFile = LocalResourceFile

            UI.Utilities.ClientAPI.AddButtonConfirm(cmdDeleteDefinition, Services.Localization.Localization.GetString("DeleteItem"))
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            pnlHelp.Visible = Not IsWizard
            pnlDefinitions.Visible = (Not IsWizard) AndAlso IsSuperTab
            cmdUpdate.Visible = (Not IsWizard) AndAlso (Not IsSuperTab AndAlso pnlPermissions.Visible)
            ctlPortals.Visible = Not IsWizard
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub Initialize()
            pnlDesktopModule.Visible = IsSuperTab
            BindDesktopModule(False)
        End Sub

        Public Overrides Sub UpdatePackage()
            Dim bUpdateSupportedFeatures As Boolean = Null.NullBoolean
            Dim _Package As PackageInfo = PackageController.GetPackage(PackageID)

            'Update module settings
            If ctlDesktopModule.IsValid Then
                Dim desktopModule As DesktopModuleInfo = TryCast(ctlDesktopModule.DataSource, DesktopModuleInfo)
                If desktopModule IsNot Nothing AndAlso _Package IsNot Nothing Then
                    desktopModule.FriendlyName = _Package.FriendlyName
                    desktopModule.Version = FormatVersion(_Package.Version)
                    If String.IsNullOrEmpty(desktopModule.BusinessControllerClass) Then
                        desktopModule.SupportedFeatures = 0
                    Else
                        bUpdateSupportedFeatures = True
                    End If
                    DesktopModuleController.SaveDesktopModule(desktopModule, False, True)
                End If
            End If

            If bUpdateSupportedFeatures Then
                UpdateModuleInterfaces(desktopModule.BusinessControllerClass)
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub cboDefinitions_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDefinitions.SelectedIndexChanged
            If Not IsAddDefinitionMode Then
                ModuleDefinitionID = Integer.Parse(cboDefinitions.SelectedValue)
                'Force Module Definition to refresh
                _ModuleDefinition = Nothing
                BindDefinition()
            End If
        End Sub

        Protected Sub cmdAddControl_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddControl.Click
            Response.Redirect(ModuleContext.EditUrl("ModuleControlID", "-1", "EditControl", "packageId=" & PackageID.ToString(), "moduledefid=" & ModuleDefinition.ModuleDefID.ToString()), True)
        End Sub

        Protected Sub cmdAddDefinition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddDefinition.Click
            IsAddDefinitionMode = True
            ModuleDefinitionID = Null.NullInteger
            _ModuleDefinition = Nothing
            BindDefinition()
        End Sub

        Protected Sub cmdDeleteDefinition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDeleteDefinition.Click
            Dim objModuleDefinitions As New ModuleDefinitionController
            objModuleDefinitions.DeleteModuleDefinition(ModuleDefinitionID)

            'Force Definitions list to refresh by rebinding DesktopModule
            ModuleDefinitionID = Null.NullInteger
            _ModuleDefinition = Nothing
            _DesktopModule = Nothing
            BindDesktopModule(True)
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim bUpdateSupportedFeatures As Boolean = Null.NullBoolean

            If Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                UpdatePackage()
            Else
                'Update DesktopModule Permissions
                Dim objCurrentPermissions As Security.Permissions.DesktopModulePermissionCollection
                objCurrentPermissions = DesktopModulePermissionController.GetDesktopModulePermissions(dgPermissions.PortalDesktopModuleID)
                If Not objCurrentPermissions.CompareTo(dgPermissions.Permissions) Then
                    DesktopModulePermissionController.DeleteDesktopModulePermissionsByPortalDesktopModuleID(dgPermissions.PortalDesktopModuleID)
                    For Each objPermission As DesktopModulePermissionInfo In dgPermissions.Permissions
                        DesktopModulePermissionController.AddDesktopModulePermission(objPermission)
                    Next
                End If

                DataCache.RemoveCache(String.Format(DataCache.PortalDesktopModuleCacheKey, Me.ModuleContext.PortalId.ToString()))

                dgPermissions.ResetPermissions()
            End If
        End Sub

        Protected Sub cmdUpdateDefinition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateDefinition.Click
            If ctlDefinition.IsValid AndAlso ctlDefinition.IsDirty Then
                Dim definition As ModuleDefinitionInfo = TryCast(ctlDefinition.DataSource, ModuleDefinitionInfo)
                If definition IsNot Nothing Then
                    If (IsAddDefinitionMode AndAlso ModuleDefinitionController.GetModuleDefinitionByFriendlyName(definition.FriendlyName) Is Nothing) _
                            OrElse (Not IsAddDefinitionMode) Then
                        ModuleDefinitionID = ModuleDefinitionController.SaveModuleDefinition(definition, False, True)

                        'Force Definitions list to refresh by rebinding DesktopModule
                        IsAddDefinitionMode = False
                        _DesktopModule = Nothing
                        BindDesktopModule(True)
                    Else
                        'The FriendlyName is being used
                        lblDefinitionError.Visible = True
                    End If
                End If

            End If
        End Sub

        Protected Sub ctlPortals_AddAllButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlPortals.AddAllButtonClick
            'Add all Portals
            Dim objPortals As New PortalController
            For Each objPortal As PortalInfo In objPortals.GetPortals
                DesktopModuleController.AddDesktopModuleToPortal(objPortal.PortalID, DesktopModule.DesktopModuleID, True, False)
            Next

            DataCache.ClearHostCache(True)

            BindDesktopModule(False)
        End Sub

        Protected Sub ctlPortals_AddButtonClick(ByVal sender As Object, ByVal e As UI.WebControls.DualListBoxEventArgs) Handles ctlPortals.AddButtonClick
            If e.Items IsNot Nothing Then
                For Each portal As String In e.Items
                    DesktopModuleController.AddDesktopModuleToPortal(Integer.Parse(portal), DesktopModule.DesktopModuleID, True, False)
                Next
            End If

            DataCache.ClearHostCache(True)

            BindDesktopModule(False)
        End Sub

        Protected Sub ctlPortals_RemoveAllButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlPortals.RemoveAllButtonClick
            'Add all Portals
            Dim objPortals As New PortalController
            For Each objPortal As PortalInfo In objPortals.GetPortals
                DesktopModuleController.RemoveDesktopModuleFromPortal(objPortal.PortalID, DesktopModule.DesktopModuleID, False)
            Next

            DataCache.ClearHostCache(True)

            BindDesktopModule(False)
        End Sub

        Protected Sub ctlPortals_RemoveButtonClick(ByVal sender As Object, ByVal e As UI.WebControls.DualListBoxEventArgs) Handles ctlPortals.RemoveButtonClick
            If e.Items IsNot Nothing Then
                For Each portal As String In e.Items
                    DesktopModuleController.RemoveDesktopModuleFromPortal(Integer.Parse(portal), DesktopModule.DesktopModuleID, False)
                Next
            End If

            DataCache.ClearHostCache(True)

            BindDesktopModule(False)
        End Sub

        Protected Sub grdControls_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdControls.DeleteCommand
            Dim controlID As Integer = Int32.Parse(e.CommandArgument.ToString)
            ModuleControlController.DeleteModuleControl(controlID)

            'Force Module Definition to refresh
            _ModuleDefinition = Nothing
            BindDefinition()
        End Sub

        Protected Sub grdControls_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdControls.ItemDataBound

            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim editHyperlink As HyperLink = TryCast(item.Controls(0).Controls(0), HyperLink)
                If editHyperlink IsNot Nothing Then
                    editHyperlink.NavigateUrl = ModuleContext.EditUrl("ModuleControlID", editHyperlink.NavigateUrl, "EditControl", "packageId=" & PackageID.ToString(), "moduledefid=" & ModuleDefinition.ModuleDefID.ToString())
                End If

            End If

        End Sub

#End Region

    End Class

End Namespace
