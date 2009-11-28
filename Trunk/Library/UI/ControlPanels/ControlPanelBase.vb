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

Imports System
Imports System.Configuration
Imports System.Data
Imports System.IO

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Portals.PortalSettings
Imports System.Collections.Generic

Namespace DotNetNuke.UI.ControlPanels

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ControlPanel class defines a custom base class inherited by all
    ''' ControlPanel controls.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/11/2008  documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ControlPanelBase
        Inherits System.Web.UI.UserControl

#Region "Enums"

        Protected Enum ViewPermissionType
            View = 0
            Edit = 1
        End Enum

#End Region

#Region "Private Members"

        Private _localResourceFile As String

#End Region

#Region "Protected Properties"

        Protected Function IsModuleAdmin() As Boolean
            Dim _IsModuleAdmin As Boolean = Null.NullBoolean
            For Each objModule As ModuleInfo In TabController.CurrentPage.Modules
                If Not objModule.IsDeleted Then
                    Dim blnHasModuleEditPermissions As Boolean = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, objModule)
                    If blnHasModuleEditPermissions = True AndAlso objModule.ModuleDefinition.DefaultCacheTime <> -1 Then
                        _IsModuleAdmin = True
                        Exit For
                    End If
                End If
            Next
            Return PortalSettings.ControlPanelSecurity = PortalSettings.ControlPanelPermission.ModuleEditor AndAlso _IsModuleAdmin
        End Function

        Protected Function IsPageAdmin() As Boolean
            Dim _IsPageAdmin As Boolean = Null.NullBoolean
            If TabPermissionController.CanAddContentToPage OrElse TabPermissionController.CanAddPage OrElse _
                TabPermissionController.CanAdminPage OrElse TabPermissionController.CanCopyPage OrElse _
                TabPermissionController.CanDeletePage OrElse TabPermissionController.CanExportPage OrElse _
                TabPermissionController.CanImportPage OrElse TabPermissionController.CanManagePage Then
                _IsPageAdmin = True
            End If
            Return _IsPageAdmin
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the ControlPanel is Visible
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property IsVisible() As Boolean
            Get
                Return PortalSettings.ControlPanelVisible
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current Portal Settings
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the User mode of the Control Panel
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UserMode() As Mode
            Get
                Return PortalSettings.UserMode
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a Module Permission
        ''' </summary>
        ''' <param name="permission">The permission to add</param>
        ''' <param name="roleId">The Id of the role to add the permission for.</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AddModulePermission(ByVal objModule As ModuleInfo, ByVal permission As PermissionInfo, ByVal roleId As Integer, ByVal userId As Integer, ByVal allowAccess As Boolean) As ModulePermissionInfo
            Dim objModulePermission As New ModulePermissionInfo
            objModulePermission.ModuleID = objModule.ModuleID
            objModulePermission.PermissionID = permission.PermissionID
            objModulePermission.RoleID = roleId
            objModulePermission.UserID = userId
            objModulePermission.PermissionKey = permission.PermissionKey
            objModulePermission.AllowAccess = allowAccess

            ' add the permission to the collection
            If Not objModule.ModulePermissions.Contains(objModulePermission) Then
                objModule.ModulePermissions.Add(objModulePermission)
            End If

            Return objModulePermission
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds an Existing Module to a Pane
        ''' </summary>
        ''' <param name="align">The alignment for the Modue</param>
        ''' <param name="moduleId">The Id of the existing module</param>
        ''' <param name="tabId">The id of the tab</param>
        ''' <param name="paneName">The pane to add the module to</param>
        ''' <param name="position">The relative position within the pane for the module</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub AddExistingModule(ByVal moduleId As Integer, ByVal tabId As Integer, ByVal paneName As String, ByVal position As Integer, ByVal align As String)

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo
            Dim objEventLog As New Services.Log.EventLog.EventLogController

            Dim UserId As Integer = -1
            If Request.IsAuthenticated Then
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                UserId = objUserInfo.UserID
            End If

            objModule = objModules.GetModule(moduleId, tabId, False)
            If Not objModule Is Nothing Then
                ' clone the module object ( to avoid creating an object reference to the data cache )
                Dim objClone As ModuleInfo = objModule.Clone()
                objClone.TabID = PortalSettings.ActiveTab.TabID
                objClone.ModuleOrder = position
                objClone.PaneName = paneName
                objClone.Alignment = align
                objModules.AddModule(objClone)
                objEventLog.AddLog(objClone, PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_CREATED)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a New Module to a Pane
        ''' </summary>
        ''' <param name="align">The alignment for the Modue</param>
        ''' <param name="desktopModuleId">The Id of the DesktopModule</param>
        ''' <param name="permissionType">The View Permission Type for the Module</param>
        ''' <param name="title">The Title for the resulting module</param>
        ''' <param name="paneName">The pane to add the module to</param>
        ''' <param name="position">The relative position within the pane for the module</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub AddNewModule(ByVal title As String, ByVal desktopModuleId As Integer, ByVal paneName As String, ByVal position As Integer, ByVal permissionType As ViewPermissionType, ByVal align As String)

            Dim objTabPermissions As TabPermissionCollection = PortalSettings.ActiveTab.TabPermissions
            Dim objPermissionController As New PermissionController
            Dim objModules As New ModuleController
            Dim objModuleDefinition As ModuleDefinitionInfo
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            Dim j As Integer

            Try
                Dim desktopModule As DesktopModuleInfo = Nothing
                If Not DesktopModuleController.GetDesktopModules(PortalSettings.PortalId).TryGetValue(desktopModuleId, desktopModule) Then
                    Throw New ArgumentException("desktopModuleId")
                End If
            Catch ex As Exception
                LogException(ex)
            End Try

            Dim UserId As Integer = -1
            If Request.IsAuthenticated Then
                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                UserId = objUserInfo.UserID
            End If

            For Each objModuleDefinition In ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(desktopModuleId).Values
                Dim objModule As New ModuleInfo
                objModule.Initialize(PortalSettings.PortalId)

                objModule.PortalID = PortalSettings.PortalId
                objModule.TabID = PortalSettings.ActiveTab.TabID
                objModule.ModuleOrder = position
                If title = "" Then
                    objModule.ModuleTitle = objModuleDefinition.FriendlyName
                Else
                    objModule.ModuleTitle = title
                End If
                objModule.PaneName = paneName
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID
                If objModuleDefinition.DefaultCacheTime > 0 Then
                    objModule.CacheTime = objModuleDefinition.DefaultCacheTime
                    If PortalSettings.Current.DefaultModuleId > Null.NullInteger AndAlso PortalSettings.Current.DefaultTabId > Null.NullInteger Then
                        Dim defaultModule As ModuleInfo = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, True)
                        If Not defaultModule Is Nothing Then
                            objModule.CacheTime = defaultModule.CacheTime
                        End If
                    End If
                End If

                Select Case permissionType
                    Case ViewPermissionType.View
                        objModule.InheritViewPermissions = True
                    Case ViewPermissionType.Edit
                        objModule.InheritViewPermissions = False
                End Select

                ' get the default module view permissions
                Dim arrSystemModuleViewPermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW")

                ' get the permissions from the page
                For Each objTabPermission As TabPermissionInfo In objTabPermissions
                    If objTabPermission.PermissionKey = "VIEW" AndAlso permissionType = ViewPermissionType.View Then
                        'Don't need to explicitly add View permisisons if "Same As Page"
                        Continue For
                    End If

                    ' get the system module permissions for the permissionkey
                    Dim arrSystemModulePermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey)
                    ' loop through the system module permissions
                    For j = 0 To arrSystemModulePermissions.Count - 1
                        ' create the module permission
                        Dim objSystemModulePermission As PermissionInfo
                        objSystemModulePermission = CType(arrSystemModulePermissions(j), PermissionInfo)
                        If objSystemModulePermission.PermissionKey = "VIEW" AndAlso permissionType = ViewPermissionType.Edit AndAlso _
                             objTabPermission.PermissionKey <> "EDIT" Then
                            'Only Page Editors get View permissions if "Page Editors Only"
                            Continue For
                        End If

                        Dim objModulePermission As ModulePermissionInfo = AddModulePermission(objModule, _
                                                                                objSystemModulePermission, _
                                                                                objTabPermission.RoleID, objTabPermission.UserID, _
                                                                                objTabPermission.AllowAccess)

                        ' ensure that every EDIT permission which allows access also provides VIEW permission
                        If objModulePermission.PermissionKey = "EDIT" And objModulePermission.AllowAccess Then
                            Dim objModuleViewperm As ModulePermissionInfo = AddModulePermission(objModule, _
                                                                                CType(arrSystemModuleViewPermissions(0), PermissionInfo), _
                                                                                objModulePermission.RoleID, objModulePermission.UserID, _
                                                                                True)
                        End If
                    Next

                    'Get the custom Module Permissions,  Assume that roles with Edit Tab Permissions
                    'are automatically assigned to the Custom Module Permissions
                    If objTabPermission.PermissionKey = "EDIT" Then
                        Dim arrCustomModulePermissions As ArrayList = objPermissionController.GetPermissionsByModuleDefID(objModule.ModuleDefID)

                        ' loop through the custom module permissions
                        For j = 0 To arrCustomModulePermissions.Count - 1
                            ' create the module permission
                            Dim objCustomModulePermission As PermissionInfo
                            objCustomModulePermission = CType(arrCustomModulePermissions(j), PermissionInfo)

                            AddModulePermission(objModule, objCustomModulePermission, _
                                                                    objTabPermission.RoleID, objTabPermission.UserID, _
                                                                    objTabPermission.AllowAccess)
                        Next
                    End If
                Next

                objModule.AllTabs = False
                objModule.Alignment = align

                objModules.AddModule(objModule)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Builds a URL
        ''' </summary>
        ''' <param name="FriendlyName">The friendly name of the Module</param>
        ''' <param name="PortalID">The ID of the portal</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function BuildURL(ByVal PortalID As Integer, ByVal FriendlyName As String) As String
            Dim strURL As String = "~/" & glbDefaultPage

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModuleByDefinition(PortalID, FriendlyName)
            If Not objModule Is Nothing Then
                If PortalID = Null.NullInteger Then
                    strURL = NavigateURL(objModule.TabID, True)
                Else
                    strURL = NavigateURL(objModule.TabID)
                End If
            End If

            Return strURL
        End Function

        Protected Function GetModulePermission(ByVal PortalID As Integer, ByVal FriendlyName As String) As Boolean
            Dim AllowAccess As Boolean = Null.NullBoolean
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModuleByDefinition(PortalID, FriendlyName)
            If Not objModule Is Nothing Then
                AllowAccess = ModulePermissionController.CanViewModule(objModule)
            End If
            Return AllowAccess
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the UserMode
        ''' </summary>
        ''' <param name="userMode">The userMode to set</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub SetUserMode(ByVal userMode As String)
            Personalization.Personalization.SetProfile("Usability", "UserMode" & PortalSettings.PortalId.ToString, userMode.ToUpper())
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets the current Visible Mode
        ''' </summary>
        ''' <param name="isVisible">A flag indicating whether the Control Panel should be visible</param>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub SetVisibleMode(ByVal isVisible As Boolean)
            Personalization.Personalization.SetProfile("Usability", "ControlPanelVisible" & PortalSettings.PortalId.ToString, isVisible.ToString)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Local ResourceFile for the Control Panel
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/11/2008  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LocalResourceFile() As String
            Get
                Dim fileRoot As String

                If _localResourceFile = "" Then
                    fileRoot = Me.TemplateSourceDirectory & "/" & Services.Localization.Localization.LocalResourceDirectory & "/" & Me.ID
                Else
                    fileRoot = _localResourceFile
                End If
                Return fileRoot
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in 5.0. Replaced By UserMode.")> _
        Protected ReadOnly Property ShowContent() As Boolean
            Get
                Return PortalSettings.UserMode <> Mode.Layout
            End Get
        End Property

        <Obsolete("Deprecated in 5.0. Replaced By UserMode.")> _
        Protected ReadOnly Property IsPreview() As Boolean
            Get
                If PortalSettings.UserMode = PortalSettings.Mode.Edit Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        <Obsolete("Deprecated in 5.0. Replaced by SetMode(UserMode).")> _
        Protected Sub SetContentMode(ByVal showContent As Boolean)
            Personalization.Personalization.SetProfile("Usability", "ContentVisible" & PortalSettings.PortalId.ToString, showContent.ToString)
        End Sub

        <Obsolete("Deprecated in 5.0. Replaced by SetMode(UserMode).")> _
        Protected Sub SetPreviewMode(ByVal isPreview As Boolean)
            If isPreview Then
                Personalization.Personalization.SetProfile("Usability", "UserMode" & PortalSettings.PortalId.ToString, "View")
            Else
                Personalization.Personalization.SetProfile("Usability", "UserMode" & PortalSettings.PortalId.ToString, "Edit")
            End If
        End Sub

#End Region

    End Class

End Namespace
