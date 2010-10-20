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
Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Web
Imports DotNetNuke.Entities.Controllers

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Security
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Personalization
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Application
Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Entities.Profile
Imports System.Xml.XPath
Imports DotNetNuke.Entities


Namespace DotNetNuke.Services.Upgrade

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Upgrade class provides Shared/Static methods to Upgrade/Install
    '''	a DotNetNuke Application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	11/6/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Upgrade

#Region "Private Shared Field"

        Private Shared startTime As DateTime
        Private Shared upgradeMemberShipProvider As Boolean = True

#End Region

#Region "Public Property"

        Public Shared ReadOnly Property DefaultProvider() As String
            Get
                Return Config.GetDefaultProvider("data").Name
            End Get
        End Property

        Public Shared ReadOnly Property RunTime() As TimeSpan
            Get
                Dim currentTime As DateTime = DateTime.Now()
                Return currentTime.Subtract(startTime)
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddAdminPages adds an Admin Page and an associated Module to all configured Portals
        ''' </summary>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        '''	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        '''	<param name="ModuleTitle">The Module's title</param>
        '''	<param name="ModuleIconFile">The Module's icon</param>
        ''' <history>
        ''' 	[cnurse]	11/16/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Sub AddAdminPages(ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String)

            'Call overload with InheritPermisions=True
            AddAdminPages(TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, ModuleDefId, ModuleTitle, ModuleIconFile, True)
        End Sub

        Private Shared Sub AddAdminRoleToPage(ByVal tabPath As String)
            Dim portalCtrl As New PortalController
            Dim tabCtrl As New TabController
            Dim tabID As Integer
            Dim tab As TabInfo

            For Each portal As PortalInfo In portalCtrl.GetPortals()
                tabID = TabController.GetTabByTabPath(portal.PortalID, tabPath)
                If (tabID <> Null.NullInteger) Then
                    tab = tabCtrl.GetTab(tabID, portal.PortalID, True)

                    If (tab.TabPermissions.Count = 0) Then
                        AddPagePermission(tab.TabPermissions, "View", Convert.ToInt32(portal.AdministratorRoleId))
                        AddPagePermission(tab.TabPermissions, "Edit", Convert.ToInt32(portal.AdministratorRoleId))
                        TabPermissionController.SaveTabPermissions(tab)
                    End If
                End If
            Next
        End Sub

        Private Shared Sub AddConsoleModuleSettings(ByVal tabID As Integer, ByVal moduleID As Integer)
            Dim modCtrl As ModuleController = New ModuleController()

            modCtrl.UpdateModuleSetting(moduleID, "DefaultSize", "IconFileLarge")
            modCtrl.UpdateModuleSetting(moduleID, "AllowSizeChange", "False")
            modCtrl.UpdateModuleSetting(moduleID, "DefaultView", "Hide")
            modCtrl.UpdateModuleSetting(moduleID, "AllowViewChange", "False")
            modCtrl.UpdateModuleSetting(moduleID, "ShowTooltip", "True")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleControl adds a new Module Control to the system
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="ModuleDefId">The Module Definition Id</param>
        '''	<param name="ControlKey">The key for this control in the Definition</param>
        '''	<param name="ControlTitle">The title of this control</param>
        '''	<param name="ControlSrc">Te source of ths control</param>
        '''	<param name="IconFile">The icon file</param>
        '''	<param name="ControlType">The type of control</param>
        '''	<param name="ViewOrder">The vieworder for this module</param>
        '''	<param name="HelpURL">The Help Url</param>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Sub AddModuleControl(ByVal ModuleDefId As Integer, ByVal ControlKey As String, ByVal ControlTitle As String, ByVal ControlSrc As String, ByVal IconFile As String, ByVal ControlType As SecurityAccessLevel, ByVal ViewOrder As Integer, ByVal HelpURL As String)

            ' check if module control exists
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId)
            If objModuleControl Is Nothing Then
                objModuleControl = New ModuleControlInfo

                objModuleControl.ModuleControlID = Null.NullInteger
                objModuleControl.ModuleDefID = ModuleDefId
                objModuleControl.ControlKey = ControlKey
                objModuleControl.ControlTitle = ControlTitle
                objModuleControl.ControlSrc = ControlSrc
                objModuleControl.ControlType = ControlType
                objModuleControl.ViewOrder = ViewOrder
                objModuleControl.IconFile = IconFile
                objModuleControl.HelpURL = HelpURL

                ModuleControlController.AddModuleControl(objModuleControl)
            End If
        End Sub

        Private Overloads Shared Sub AddModuleControl(ByVal ModuleDefId As Integer, ByVal ControlKey As String, ByVal ControlTitle As String, ByVal ControlSrc As String, ByVal IconFile As String, ByVal ControlType As SecurityAccessLevel, ByVal ViewOrder As Integer, ByVal HelpURL As String, ByVal SupportsPartialRendering As Boolean)

            ' check if module control exists
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId)
            If objModuleControl Is Nothing Then
                objModuleControl = New ModuleControlInfo

                objModuleControl.ModuleControlID = Null.NullInteger
                objModuleControl.ModuleDefID = ModuleDefId
                objModuleControl.ControlKey = ControlKey
                objModuleControl.ControlTitle = ControlTitle
                objModuleControl.ControlSrc = ControlSrc
                objModuleControl.ControlType = ControlType
                objModuleControl.ViewOrder = ViewOrder
                objModuleControl.IconFile = IconFile
                objModuleControl.SupportsPartialRendering = SupportsPartialRendering

                ModuleControlController.AddModuleControl(objModuleControl)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleDefinition adds a new Core Module Definition to the system
        ''' </summary>
        ''' <remarks>
        '''	This overload allows the caller to determine whether the module has a controller
        ''' class
        ''' </remarks>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        '''	<param name="Description">Description of the Module</param>
        '''	<param name="ModuleDefinitionName">The Module Definition Name</param>
        '''	<param name="Premium">A flag representing whether the module is a Premium module</param>
        '''	<param name="Admin">A flag representing whether the module is an Admin module</param>
        '''	<returns>The Module Definition Id of the new Module</returns>
        ''' <history>
        ''' 	[cnurse]	10/14/2004	documented
        '''     [cnurse]    11/11/2004  removed addition of Module Control (now in AddMOduleControl)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Function AddModuleDefinition(ByVal DesktopModuleName As String, ByVal Description As String, ByVal ModuleDefinitionName As String, ByVal Premium As Boolean, ByVal Admin As Boolean) As Integer
            Return AddModuleDefinition(DesktopModuleName, Description, ModuleDefinitionName, "", False, Premium, Admin)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleDefinition adds a new Core Module Definition to the system
        ''' </summary>
        ''' <remarks>
        '''	This overload allows the caller to determine whether the module has a controller
        ''' class
        ''' </remarks>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        '''	<param name="Description">Description of the Module</param>
        '''	<param name="ModuleDefinitionName">The Module Definition Name</param>
        '''	<param name="Premium">A flag representing whether the module is a Premium module</param>
        '''	<param name="Admin">A flag representing whether the module is an Admin module</param>
        '''	<returns>The Module Definition Id of the new Module</returns>
        ''' <history>
        ''' 	[cnurse]	10/14/2004	documented
        '''     [cnurse]    11/11/2004  removed addition of Module Control (now in AddMOduleControl)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Function AddModuleDefinition(ByVal DesktopModuleName As String, ByVal Description As String, ByVal ModuleDefinitionName As String, ByVal BusinessControllerClass As String, ByVal IsPortable As Boolean, ByVal Premium As Boolean, ByVal Admin As Boolean) As Integer
            ' check if desktop module exists
            Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger)
            If objDesktopModule Is Nothing Then
                Dim package As New PackageInfo
                package.Description = Description
                package.FriendlyName = DesktopModuleName
                package.Name = String.Concat("DotNetNuke.", DesktopModuleName)
                package.PackageType = "Module"
                package.Owner = "DotNetNuke"
                package.Organization = "DotNetNuke Corporation"
                package.Url = "www.dotnetnuke.com"
                package.Email = "support@dotnetnuke.com"
                If DesktopModuleName = "Extensions" OrElse DesktopModuleName = "Skin Designer" OrElse DesktopModuleName = "Dashboard" Then
                    package.IsSystemPackage = True
                End If
                package.Version = New System.Version(1, 0, 0)

                package.PackageID = PackageController.AddPackage(package, False)

                Dim moduleName As String = DesktopModuleName.Replace(" ", "")
                objDesktopModule = New DesktopModuleInfo
                objDesktopModule.DesktopModuleID = Null.NullInteger
                objDesktopModule.PackageID = package.PackageID
                objDesktopModule.FriendlyName = DesktopModuleName
                objDesktopModule.FolderName = "Admin/" + moduleName
                objDesktopModule.ModuleName = moduleName
                objDesktopModule.Description = Description
                objDesktopModule.Version = "01.00.00"
                objDesktopModule.BusinessControllerClass = BusinessControllerClass
                objDesktopModule.IsPortable = IsPortable
                objDesktopModule.SupportedFeatures = 0
                If (IsPortable) Then
                    objDesktopModule.SupportedFeatures = 1
                End If
                objDesktopModule.IsPremium = Premium
                objDesktopModule.IsAdmin = Admin

                objDesktopModule.DesktopModuleID = DesktopModuleController.SaveDesktopModule(objDesktopModule, False, False)

                If Not Premium Then
                    DesktopModuleController.AddDesktopModuleToPortals(objDesktopModule.DesktopModuleID)
                End If
            End If

            ' check if module definition exists
            Dim objModuleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(ModuleDefinitionName, objDesktopModule.DesktopModuleID)
            If objModuleDefinition Is Nothing Then
                objModuleDefinition = New ModuleDefinitionInfo

                objModuleDefinition.ModuleDefID = Null.NullInteger
                objModuleDefinition.DesktopModuleID = objDesktopModule.DesktopModuleID
                objModuleDefinition.FriendlyName = ModuleDefinitionName

                objModuleDefinition.ModuleDefID = ModuleDefinitionController.SaveModuleDefinition(objModuleDefinition, False, False)
            End If

            Return objModuleDefinition.ModuleDefID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleToPage adds a module to a Page
        ''' </summary>
        ''' <remarks>
        ''' This overload assumes ModulePermissions will be inherited
        ''' </remarks>
        '''	<param name="page">The Page to add the Module to</param>
        '''	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        '''	<param name="ModuleTitle">The Module's title</param>
        '''	<param name="ModuleIconFile">The Module's icon</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function AddModuleToPage(ByVal page As TabInfo, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String) As Integer
            'Call overload with InheritPermisions=True
            Return AddModuleToPage(page, ModuleDefId, ModuleTitle, ModuleIconFile, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPage adds a Tab Page
        ''' </summary>
        ''' <remarks>
        ''' Adds a Tab to a parentTab
        ''' </remarks>
        '''	<param name="parentTab">The Parent Tab</param>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        '''	<param name="permissions">Page Permissions Collection for this page</param>
        ''' <param name="IsAdmin">Is an admin page</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Function AddPage(ByVal parentTab As TabInfo, ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean, ByVal permissions As Security.Permissions.TabPermissionCollection, ByVal IsAdmin As Boolean) As TabInfo

            Dim ParentId As Integer = Null.NullInteger
            Dim PortalId As Integer = Null.NullInteger

            If Not parentTab Is Nothing Then
                ParentId = parentTab.TabID
                PortalId = parentTab.PortalID
            End If

            Return AddPage(PortalId, ParentId, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, permissions, IsAdmin)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPage adds a Tab Page
        ''' </summary>
        '''	<param name="PortalId">The Id of the Portal</param>
        '''	<param name="ParentId">The Id of the Parent Tab</param>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        '''	<param name="permissions">Page Permissions Collection for this page</param>
        ''' <param name="IsAdmin">Is and admin page</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Function AddPage(ByVal PortalId As Integer, ByVal ParentId As Integer, ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean, ByVal permissions As Security.Permissions.TabPermissionCollection, ByVal IsAdmin As Boolean) As TabInfo
            Dim objTabs As New TabController
            Dim objTab As TabInfo

            objTab = objTabs.GetTabByName(TabName, PortalId, ParentId)

            If objTab Is Nothing OrElse objTab.ParentId <> ParentId Then
                objTab = New TabInfo
                objTab.TabID = Null.NullInteger
                objTab.PortalID = PortalId
                objTab.TabName = TabName
                objTab.Title = ""
                objTab.Description = Description
                objTab.KeyWords = ""
                objTab.IsVisible = IsVisible
                objTab.DisableLink = False
                objTab.ParentId = ParentId
                objTab.IconFile = TabIconFile
                objTab.IconFileLarge = TabIconFileLarge
                objTab.IsDeleted = False
                objTab.TabID = objTabs.AddTab(objTab, Not IsAdmin)

                If (Not permissions Is Nothing) Then
                    Dim tabPermissionCtrl As New Security.Permissions.TabPermissionController
                    For Each tabPermission As TabPermissionInfo In permissions
                        objTab.TabPermissions.Add(tabPermission, True)
                    Next
                    TabPermissionController.SaveTabPermissions(objTab)
                End If
            End If

            Return objTab
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPagePermission adds a TabPermission to a TabPermission Collection
        ''' </summary>
        '''	<param name="permissions">Page Permissions Collection for this page</param>
        '''	<param name="key">The Permission key</param>
        '''	<param name="roleId">The role given the permission</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddPagePermission(ByRef permissions As Security.Permissions.TabPermissionCollection, ByVal key As String, ByVal roleId As Integer)


            Dim objPermissionController As New Security.Permissions.PermissionController
            Dim objPermission As Security.Permissions.PermissionInfo = CType(objPermissionController.GetPermissionByCodeAndKey("SYSTEM_TAB", key)(0), Security.Permissions.PermissionInfo)

            Dim objTabPermission As New Security.Permissions.TabPermissionInfo
            objTabPermission.PermissionID = objPermission.PermissionID
            objTabPermission.RoleID = roleId
            objTabPermission.AllowAccess = True
            permissions.Add(objTabPermission)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddSearchResults adds a top level Hidden Search Results Page
        ''' </summary>
        '''	<param name="ModuleDefId">The Module Deinition Id for the Search Results Module</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddSearchResults(ByVal ModuleDefId As Integer)

            Dim objPortals As New PortalController
            Dim objPortal As PortalInfo
            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim intPortal As Integer
            Dim newPage As TabInfo

            'Add Page to Admin Menu of all configured Portals
            For intPortal = 0 To arrPortals.Count - 1
                Dim objTabPermissions As New Security.Permissions.TabPermissionCollection

                objPortal = CType(arrPortals(intPortal), PortalInfo)

                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Common.Globals.glbRoleAllUsers))
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(objPortal.AdministratorRoleId))
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(objPortal.AdministratorRoleId))

                'Create New Page (or get existing one)
                newPage = AddPage(objPortal.PortalID, Null.NullInteger, "Search Results", "", "", "", False, objTabPermissions, False)

                'Add Module To Page
                AddModuleToPage(newPage, ModuleDefId, "Search Results", "")
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddSkinControl adds a new Module Control to the system
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="ControlKey">The key for this control in the Definition</param>
        '''	<param name="ControlSrc">Te source of ths control</param>
        ''' <history>
        ''' 	[cnurse]	05/26/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Overloads Shared Sub AddSkinControl(ByVal ControlKey As String, ByVal PackageName As String, ByVal ControlSrc As String)

            ' check if skin control exists
            Dim skinControl As SkinControlInfo = SkinControlController.GetSkinControlByKey(ControlKey)
            If skinControl Is Nothing Then
                Dim package As New PackageInfo
                package.Name = PackageName
                package.FriendlyName = String.Concat(ControlKey, "SkinObject")
                package.PackageType = "SkinObject"
                package.Version = New Version(1, 0, 0)
                LegacyUtil.ParsePackageName(package)

                Dim PackageId As Integer = PackageController.AddPackage(package, False)

                skinControl = New SkinControlInfo

                skinControl.PackageID = PackageId
                skinControl.ControlKey = ControlKey
                skinControl.ControlSrc = ControlSrc
                skinControl.SupportsPartialRendering = False

                SkinControlController.SaveSkinControl(skinControl)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CoreModuleExists determines whether a Core Module exists on the system
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module</param>
        '''	<returns>True if the Module exists, otherwise False</returns>
        ''' <history>
        ''' 	[cnurse]	10/14/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function CoreModuleExists(ByVal DesktopModuleName As String) As Boolean
            Dim blnExists As Boolean = False
            Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger)

            Return (Not objDesktopModule Is Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExecuteScript executes a SQl script file
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strScriptFile">The script to Execute</param>
        ''' <history>
        ''' 	[cnurse]	11/09/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function ExecuteScript(ByVal strScriptFile As String, ByVal writeFeedback As Boolean) As String
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Script: " + Path.GetFileName(strScriptFile))
            End If

            Dim strExceptions As String

            ' read script file for installation
            Dim strScript As String = FileSystemUtils.ReadFile(strScriptFile)

            ' execute SQL installation script
            strExceptions = DataProvider.Instance().ExecuteScript(strScript)

            ' log the results
            Try
                Dim objStream As StreamWriter
                objStream = File.CreateText(strScriptFile.Replace("." & DefaultProvider, "") & ".log.resources")
                objStream.WriteLine(strExceptions)
                objStream.Close()
            Catch
                ' does not have permission to create the log file
            End Try

            If writeFeedback Then
                HtmlUtils.WriteScriptSuccessError(HttpContext.Current.Response, (strExceptions = ""), Path.GetFileName(strScriptFile).Replace("." & DefaultProvider, ".log.resources"))
            End If

            Return strExceptions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleDefinition gets the Module Definition Id of a module
        ''' </summary>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        '''	<param name="ModuleDefinitionName">The Module Definition Name</param>
        '''	<returns>The Module Definition Id of the Module (-1 if no module definition)</returns>
        ''' <history>
        ''' 	[cnurse]	11/16/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetModuleDefinition(ByVal DesktopModuleName As String, ByVal ModuleDefinitionName As String) As Integer
            ' get desktop module 
            Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger)
            If objDesktopModule Is Nothing Then
                Return -1
            End If

            ' get module definition 
            Dim objModuleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(ModuleDefinitionName, objDesktopModule.DesktopModuleID)
            If objModuleDefinition Is Nothing Then
                Return -1
            End If

            Return objModuleDefinition.ModuleDefID

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' HostTabExists determines whether a tab of a given name exists under the Host tab
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="TabName">The Name of the Tab</param>
        '''	<returns>True if the Tab exists, otherwise False</returns>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function HostTabExists(ByVal TabName As String) As Boolean

            Dim blnExists As Boolean = False

            Dim objTabController As New TabController

            Dim hostTab As TabInfo = objTabController.GetTabByName("Host", Null.NullInteger)

            Dim objTabInfo As TabInfo = objTabController.GetTabByName(TabName, Null.NullInteger, hostTab.TabID)
            If Not objTabInfo Is Nothing Then
                blnExists = True
            Else
                blnExists = False
            End If

            Return blnExists

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallMemberRoleProvider - Installs the MemberRole Provider Db objects
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The Path to the Provider Directory</param>
        ''' <history>
        ''' 	[cnurse]	02/02/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function InstallMemberRoleProvider(ByVal strProviderPath As String, ByVal writeFeedback As Boolean) As String
            Dim strExceptions As String = ""

            Dim installMemberRole As Boolean = True
            If Not Config.GetSetting("InstallMemberRole") Is Nothing Then
                installMemberRole = Boolean.Parse(Config.GetSetting("InstallMemberRole"))
            End If

            If installMemberRole Then
                If writeFeedback Then
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing MemberRole Provider:<br>")
                End If

                'Install Common
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallCommon", writeFeedback)
                'Install Membership
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallMembership", writeFeedback)
                'Install Profile
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallProfile", writeFeedback)
                'Install Roles
                strExceptions += InstallMemberRoleProviderScript(strProviderPath, "InstallRoles", writeFeedback)

                'As we have just done an Install set the Upgrade Flag to false
                upgradeMemberShipProvider = False
            End If

            Return strExceptions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallMemberRoleProviderScript - Installs a specific MemberRole Provider script
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="providerPath">The Path to the Provider Directory</param>
        '''	<param name="scriptFile">The Name of the Script File</param>
        '''	<param name="writeFeedback">Whether or not to echo results</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function InstallMemberRoleProviderScript(ByVal providerPath As String, ByVal scriptFile As String, ByVal writeFeedback As Boolean) As String
            Dim strExceptions As String = ""

            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Script: " & scriptFile & "<br>")
            End If

            strExceptions = DataProvider.Instance().ExecuteScript(FileSystemUtils.ReadFile(providerPath + scriptFile & ".sql"))

            ' log the results
            Try
                Dim objStream As StreamWriter
                objStream = File.CreateText(providerPath + scriptFile & ".log.resources")
                objStream.WriteLine(strExceptions)
                objStream.Close()
            Catch
                ' does not have permission to create the log file
            End Try

            Return strExceptions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ParseFiles parses the Host Template's Files node
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="node">The Files node</param>
        '''	<param name="portalId">The PortalId (-1 for Host Files)</param>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub ParseFiles(ByVal node As XmlNode, ByVal portalId As Integer)
            Dim fileNode As XmlNode
            Dim objController As New DotNetNuke.Services.FileSystem.FileController
            Dim objFile As DotNetNuke.Services.FileSystem.FileInfo

            'Parse the File nodes
            For Each fileNode In node.SelectNodes("file")

                Dim strFileName As String = XmlUtils.GetNodeValue(fileNode, "filename")
                Dim strExtenstion As String = XmlUtils.GetNodeValue(fileNode, "extension")
                Dim fileSize As Long = Long.Parse(XmlUtils.GetNodeValue(fileNode, "size"))
                Dim iWidth As Integer = XmlUtils.GetNodeValueInt(fileNode, "width")
                Dim iHeight As Integer = XmlUtils.GetNodeValueInt(fileNode, "height")
                Dim strType As String = XmlUtils.GetNodeValue(fileNode, "contentType")
                Dim strFolder As String = XmlUtils.GetNodeValue(fileNode, "folder")
                Dim objFolders As New FolderController
                Dim objFolder As FolderInfo

                objFolder = objFolders.GetFolder(portalId, strFolder, False)
                objFile = New DotNetNuke.Services.FileSystem.FileInfo(portalId, strFileName, strExtenstion, CType(fileSize, Integer), iWidth, iHeight, strType, strFolder, objFolder.FolderID, objFolder.StorageLocation, True)

                objController.AddFile(objFile)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RemoveCoreModule removes a Core Module from the system
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module to Remove</param>
        '''	<param name="ParentTabName">The Name of the parent Tab/Page for this module</param>
        '''	<param name="TabName">The Name to tab that contains the Module</param>
        '''	<param name="TabRemove">A flag to determine whether to remove the Tab if it has no
        '''	other modules</param>
        ''' <history>
        ''' 	[cnurse]	10/14/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub RemoveCoreModule(ByVal DesktopModuleName As String, ByVal ParentTabName As String, ByVal TabName As String, ByVal TabRemove As Boolean)
            Dim ParentId As Integer
            Dim intModuleDefId As Integer = Null.NullInteger
            Dim intDesktopModuleId As Integer

            'Find and remove the Module from the Tab
            Select Case ParentTabName
                Case "Host"
                    Dim objTabs As New TabController
                    Dim objTab As TabInfo = objTabs.GetTabByName("Host", Null.NullInteger, Null.NullInteger)

                    If objTab IsNot Nothing Then
                        intModuleDefId = RemoveModule(DesktopModuleName, TabName, objTab.TabID, TabRemove)
                    End If
                Case "Admin"
                    Dim objPortals As New PortalController
                    Dim objPortal As PortalInfo

                    Dim arrPortals As ArrayList = objPortals.GetPortals
                    Dim intPortal As Integer

                    'Iterate through the Portals to remove the Module from the Tab
                    For intPortal = 0 To arrPortals.Count - 1
                        objPortal = CType(arrPortals(intPortal), PortalInfo)
                        ParentId = objPortal.AdminTabId
                        intModuleDefId = RemoveModule(DesktopModuleName, TabName, ParentId, TabRemove)
                    Next intPortal
            End Select

            Dim objDesktopModule As DesktopModuleInfo = Nothing
            If intModuleDefId = Null.NullInteger Then
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName(DesktopModuleName, Null.NullInteger)
                intDesktopModuleId = objDesktopModule.DesktopModuleID
            Else
                'Get the Module Definition
                Dim objModuleDefinitions As New ModuleDefinitionController
                Dim objModuleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByID(intModuleDefId)
                If objModuleDefinition IsNot Nothing Then
                    intDesktopModuleId = objModuleDefinition.DesktopModuleID
                    objDesktopModule = DesktopModuleController.GetDesktopModule(intDesktopModuleId, Null.NullInteger)
                End If
            End If

            If objDesktopModule IsNot Nothing Then
                'Delete the Desktop Module
                Dim objDesktopModules As New DesktopModuleController
                objDesktopModules.DeleteDesktopModule(intDesktopModuleId)

                'Delete the Package
                Packages.PackageController.DeletePackage(objDesktopModule.PackageID)
            End If

        End Sub

        Private Shared Function RemoveModule(ByVal DesktopModuleName As String, ByVal TabName As String, ByVal ParentId As Integer, ByVal TabRemove As Boolean) As Integer
            Dim objTabs As New TabController
            Dim objModules As New ModuleController
            Dim objTab As TabInfo = objTabs.GetTabByName(TabName, Null.NullInteger, ParentId)
            Dim intModuleDefId As Integer
            Dim intCount As Integer = 0

            'Get the Modules on the Tab
            If objTab IsNot Nothing Then
                For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In objModules.GetTabModules(objTab.TabID)
                    Dim objModule As ModuleInfo = kvp.Value
                    If objModule.DesktopModule.FriendlyName = DesktopModuleName Then
                        'Delete the Module from the Modules list
                        objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, False)
                        intModuleDefId = objModule.ModuleDefID
                    Else
                        intCount += 1
                    End If
                Next

                'If Tab has no modules optionally remove tab
                If intCount = 0 And TabRemove Then
                    objTabs.DeleteTab(objTab.TabID, objTab.PortalID)
                End If
            End If

            Return intModuleDefId
        End Function

        Private Overloads Shared Sub RemoveModuleControl(ByVal ModuleDefId As Integer, ByVal ControlKey As String)
            ' get Module Control
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByControlKey(ControlKey, ModuleDefId)
            If objModuleControl IsNot Nothing Then
                ModuleControlController.DeleteModuleControl(objModuleControl.ModuleControlID)
            End If
        End Sub

        Private Shared Sub RemoveModuleFromPortals(ByVal friendlyName As String)
            Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName)
            If objDesktopModule IsNot Nothing Then
                'Module was incorrectly assigned as "IsPremium=False"
                If objDesktopModule.PackageID > Null.NullInteger Then
                    objDesktopModule.IsPremium = True
                    DesktopModuleController.SaveDesktopModule(objDesktopModule, False, True)
                End If

                'Remove the module from Portals
                DesktopModuleController.RemoveDesktopModuleFromPortals(objDesktopModule.DesktopModuleID)
            End If
        End Sub

        Private Shared Function TabPermissionExists(ByVal tabPermission As TabPermissionInfo, ByVal PortalID As Integer) As Boolean
            Dim blnExists As Boolean = False

            For Each permission As TabPermissionInfo In TabPermissionController.GetTabPermissions(tabPermission.TabID, PortalID)
                If permission.TabID = tabPermission.TabID AndAlso _
                  permission.RoleID = tabPermission.RoleID AndAlso _
                  permission.PermissionID = tabPermission.PermissionID Then
                    blnExists = True
                    Exit For
                End If
            Next
            Return blnExists
        End Function

        Private Shared Sub UpgradeToVersion_323()
            'add new SecurityException
            Dim objLogController As New Log.EventLog.LogController
            Dim xmlConfigFile As String = Common.Globals.HostMapPath + "Logs\LogConfig\SecurityExceptionTemplate.xml.resources"
            objLogController.AddLogType(xmlConfigFile, Null.NullString)
        End Sub

        Private Shared Sub UpgradeToVersion_440()
            ' remove module cache files with *.htm extension ( they are now securely named *.resources )
            Dim objPortals As New PortalController
            Dim arrPortals As ArrayList = objPortals.GetPortals
            For Each objPortal As PortalInfo In arrPortals
                If Directory.Exists(ApplicationMapPath & "\Portals\" & objPortal.PortalID.ToString & "\Cache\") Then
                    Dim arrFiles As String() = Directory.GetFiles(ApplicationMapPath & "\Portals\" & objPortal.PortalID.ToString & "\Cache\", "*.htm")
                    For Each strFile As String In arrFiles
                        File.Delete(strFile)
                    Next
                End If
            Next
        End Sub

        Private Shared Sub UpgradeToVersion_470()
            Dim strHostTemplateFile As String = HostMapPath & "Templates\Default.page.template"
            If File.Exists(strHostTemplateFile) Then
                Dim objPortals As New PortalController
                Dim arrPortals As ArrayList = objPortals.GetPortals
                For Each objPortal As PortalInfo In arrPortals
                    Dim strPortalTemplateFolder As String = objPortal.HomeDirectoryMapPath & "Templates\"

                    If Not Directory.Exists(strPortalTemplateFolder) Then
                        'Create Portal Templates folder
                        Directory.CreateDirectory(strPortalTemplateFolder)
                    End If
                    Dim strPortalTemplateFile As String = strPortalTemplateFolder + "Default.page.template"
                    If Not File.Exists(strPortalTemplateFile) Then
                        File.Copy(strHostTemplateFile, strPortalTemplateFile)

                        'Synchronize the Templates folder to ensure the templates are accessible
                        FileSystemUtils.SynchronizeFolder(objPortal.PortalID, strPortalTemplateFolder, "Templates/", False, True, True, False)
                    End If
                Next
            End If
        End Sub

        Private Shared Sub UpgradeToVersion_482()
            'checks for the very rare case where the default validationkey prior to 4.08.02
            'is still being used and updates it
            Config.UpdateValidationKey()
        End Sub

        Private Shared Sub UpgradeToVersion_500()
            Dim objPortals As New PortalController
            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim controller As New TabController()

            'Add Edit Permissions for Admin Tabs to legacy portals
            Dim permissionControler As New PermissionController
            Dim tabPermissionControler As New TabPermissionController
            Dim permissions As ArrayList = permissionControler.GetPermissionByCodeAndKey("SYSTEM_TAB", "EDIT")
            Dim permissionID As Integer = Null.NullInteger
            If permissions.Count = 1 Then
                Dim permission As PermissionInfo = TryCast(permissions(0), PermissionInfo)
                permissionID = permission.PermissionID

                For Each portal As PortalInfo In arrPortals
                    Dim adminTab As TabInfo = controller.GetTab(portal.AdminTabId, portal.PortalID, True)
                    If adminTab IsNot Nothing Then
                        Dim tabPermission As New TabPermissionInfo
                        tabPermission.TabID = adminTab.TabID
                        tabPermission.PermissionID = permissionID
                        tabPermission.AllowAccess = True
                        tabPermission.RoleID = portal.AdministratorRoleId
                        If Not TabPermissionExists(tabPermission, portal.PortalID) Then
                            adminTab.TabPermissions.Add(tabPermission)
                        End If

                        'Save Tab Permissions to Data Base
                        TabPermissionController.SaveTabPermissions(adminTab)

                        For Each childTab As TabInfo In TabController.GetTabsByParent(portal.AdminTabId, portal.PortalID)
                            tabPermission = New TabPermissionInfo()
                            tabPermission.TabID = childTab.TabID
                            tabPermission.PermissionID = permissionID
                            tabPermission.AllowAccess = True
                            tabPermission.RoleID = portal.AdministratorRoleId
                            If Not TabPermissionExists(tabPermission, portal.PortalID) Then
                                childTab.TabPermissions.Add(tabPermission)
                            End If
                            'Save Tab Permissions to Data Base
                            TabPermissionController.SaveTabPermissions(childTab)
                        Next
                    End If
                Next
            End If

            'Update Host/Admin modules Visibility setting
            Dim superTabProcessed As Boolean = Null.NullBoolean
            Dim moduleController As New ModuleController
            For Each portal As PortalInfo In arrPortals
                If Not superTabProcessed Then
                    'Process Host Tabs
                    For Each childTab As TabInfo In TabController.GetTabsByParent(portal.SuperTabId, Null.NullInteger)
                        For Each tabModule As ModuleInfo In moduleController.GetTabModules(childTab.TabID).Values
                            tabModule.Visibility = VisibilityState.None
                            moduleController.UpdateModule(tabModule)
                        Next
                    Next
                End If

                'Process Portal Tabs
                For Each childTab As TabInfo In TabController.GetTabsByParent(portal.AdminTabId, portal.PortalID)
                    For Each tabModule As ModuleInfo In moduleController.GetTabModules(childTab.TabID).Values
                        tabModule.Visibility = VisibilityState.None
                        moduleController.UpdateModule(tabModule)
                    Next
                Next
            Next

            'Upgrade PortalDesktopModules to support new "model"
            permissions = permissionControler.GetPermissionByCodeAndKey("SYSTEM_DESKTOPMODULE", "DEPLOY")
            If permissions.Count = 1 Then
                Dim permission As PermissionInfo = TryCast(permissions(0), PermissionInfo)
                permissionID = permission.PermissionID
                For Each portal As PortalInfo In arrPortals
                    For Each desktopModule As DesktopModuleInfo In DesktopModuleController.GetDesktopModules(Null.NullInteger).Values
                        If Not desktopModule.IsPremium Then
                            'Parse the permissions
                            Dim deployPermissions As New DesktopModulePermissionCollection()
                            Dim deployPermission As DesktopModulePermissionInfo

                            ' if Not IsAdmin add Registered Users
                            If Not desktopModule.IsAdmin Then
                                deployPermission = New DesktopModulePermissionInfo
                                deployPermission.PermissionID = permissionID
                                deployPermission.AllowAccess = True
                                deployPermission.RoleID = portal.RegisteredRoleId
                                deployPermissions.Add(deployPermission)
                            End If

                            ' if Not a Host Module add Administrators
                            Dim hostModules As String = "Portals, SQL, HostSettings, Scheduler, SearchAdmin, Lists, SkinDesigner, Extensions"
                            If Not hostModules.Contains(desktopModule.ModuleName) Then
                                deployPermission = New DesktopModulePermissionInfo
                                deployPermission.PermissionID = permissionID
                                deployPermission.AllowAccess = True
                                deployPermission.RoleID = portal.AdministratorRoleId
                                deployPermissions.Add(deployPermission)
                            End If

                            'Add Portal/Module to PortalDesktopModules
                            DesktopModuleController.AddDesktopModuleToPortal(portal.PortalID, desktopModule, deployPermissions, False)
                        End If
                    Next

                    DataCache.ClearPortalCache(portal.PortalID, True)
                Next
            End If

            LegacyUtil.ProcessLegacyModules()
            LegacyUtil.ProcessLegacyLanguages()
            LegacyUtil.ProcessLegacySkins()
            LegacyUtil.ProcessLegacySkinControls()
        End Sub

        Private Shared Sub UpgradeToVersion_501()
            'add new Cache Error Event Type
            Dim objLogController As New Log.EventLog.LogController
            Dim xmlConfigFile As String = String.Format("{0}Logs\LogConfig\CacheErrorTemplate.xml.resources", Common.Globals.HostMapPath)
            objLogController.AddLogType(xmlConfigFile, Null.NullString)
        End Sub

        Private Shared Sub UpgradeToVersion_510()
            'Upgrade to .NET 3.5
            TryUpgradeNETFramework()

            Dim objPortalController As New PortalController
            Dim objTabController As New TabController
            Dim objModuleController As New ModuleController
            Dim ModuleDefID As Integer
            Dim ModuleID As Integer

            'add Dashboard module and tab
            If HostTabExists("Dashboard") = False Then
                ModuleDefID = AddModuleDefinition("Dashboard", "Provides a snapshot of your DotNetNuke Application.", "Dashboard", True, True)
                AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Dashboard/Dashboard.ascx", "icon_dashboard_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "Export", "", "DesktopModules/Admin/Dashboard/Export.ascx", "", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "DashboardControls", "", "DesktopModules/Admin/Dashboard/DashboardControls.ascx", "", SecurityAccessLevel.Host, 0)

                'Create New Host Page (or get existing one)
                Dim dashboardPage As TabInfo = AddHostPage("Dashboard", "Summary view of application and site settings.", "~/images/icon_dashboard_16px.gif", "~/images/icon_dashboard_32px.gif", True)

                'Add Module To Page
                AddModuleToPage(dashboardPage, ModuleDefID, "Dashboard", "~/images/icon_dashboard_32px.gif")
            Else
                'Module was incorrectly assigned as "IsPremium=False"
                RemoveModuleFromPortals("Dashboard")
                'fix path for dashboarcontrols
                ModuleDefID = GetModuleDefinition("Dashboard", "Dashboard")
                RemoveModuleControl(ModuleDefID, "DashboardControls")
                AddModuleControl(ModuleDefID, "DashboardControls", "", "DesktopModules/Admin/Dashboard/DashboardControls.ascx", "", SecurityAccessLevel.Host, 0)
            End If

            'Add the Extensions Module
            If CoreModuleExists("Extensions") = False Then
                ModuleDefID = AddModuleDefinition("Extensions", "", "Extensions")
                AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Extensions/Extensions.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.View, 0)
                AddModuleControl(ModuleDefID, "Edit", "Edit Feature", "DesktopModules/Admin/Extensions/EditExtension.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Edit, 0)
                AddModuleControl(ModuleDefID, "PackageWriter", "Package Writer", "DesktopModules/Admin/Extensions/PackageWriter.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "EditControl", "Edit Control", "DesktopModules/Admin/Extensions/Editors/EditModuleControl.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "ImportModuleDefinition", "Import Module Definition", "DesktopModules/Admin/Extensions/Editors/ImportModuleDefinition.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "BatchInstall", "Batch Install", "DesktopModules/Admin/Extensions/BatchInstall.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "NewExtension", "New Extension Wizard", "DesktopModules/Admin/Extensions/ExtensionWizard.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)
                AddModuleControl(ModuleDefID, "UsageDetails", "Usage Information", "DesktopModules/Admin/Extensions/UsageDetails.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0, "", True)
            Else
                ModuleDefID = GetModuleDefinition("Extensions", "Extensions")
                RemoveModuleControl(ModuleDefID, "EditLanguage")
                RemoveModuleControl(ModuleDefID, "TimeZone")
                RemoveModuleControl(ModuleDefID, "Verify")
                RemoveModuleControl(ModuleDefID, "LanguageSettings")
                RemoveModuleControl(ModuleDefID, "EditResourceKey")
                RemoveModuleControl(ModuleDefID, "EditSkins")
                AddModuleControl(ModuleDefID, "UsageDetails", "Usage Information", "DesktopModules/Admin/Extensions/UsageDetails.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0, "", True)

                'Module was incorrectly assigned as "IsPremium=False"
                RemoveModuleFromPortals("Extensions")
            End If

            'Remove Module Definitions Module from Host Page (if present)
            RemoveCoreModule("Module Definitions", "Host", "Module Definitions", False)

            'Remove old Module Definition Validator module
            DesktopModuleController.DeleteDesktopModule("Module Definition Validator")

            'Get Module Definitions
            Dim definitionsPage As TabInfo = objTabController.GetTabByName("Module Definitions", Null.NullInteger)

            'Add Module To Page if not present
            ModuleID = AddModuleToPage(definitionsPage, ModuleDefID, "Module Definitions", "~/images/icon_moduledefinitions_32px.gif")
            objModuleController.UpdateModuleSetting(ModuleID, "Extensions_Mode", "Module")

            'Add Extensions Host Page
            Dim extensionsPage As TabInfo = AddHostPage("Extensions", "Install, add, modify and delete extensions, such as modules, skins and language packs.", "~/images/icon_extensions_16px.gif", "~/images/icon_extensions_32px.gif", True)

            ModuleID = AddModuleToPage(extensionsPage, ModuleDefID, "Extensions", "~/images/icon_extensions_32px.gif")
            objModuleController.UpdateModuleSetting(ModuleID, "Extensions_Mode", "All")

            'Add Extensions Module to Admin Page for all Portals
            AddAdminPages("Extensions", "Install, add, modify and delete extensions, such as modules, skins and language packs.", "~/images/icon_extensions_16px.gif", "~/images/icon_extensions_32px.gif", True, ModuleDefID, "Extensions", "~/images/icon_extensions_32px.gif")

            'Remove Host Languages Page
            RemoveHostPage("Languages")

            'Remove Admin > Authentication Pages
            RemoveAdminPages("//Admin//Authentication")

            'Remove old Languages module
            DesktopModuleController.DeleteDesktopModule("Languages")

            'Add new Languages module
            ModuleDefID = AddModuleDefinition("Languages", "", "Languages", False, False)
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Languages/languageeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0)
            AddModuleControl(ModuleDefID, "Edit", "Edit Language", "DesktopModules/Admin/Languages/EditLanguage.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Edit, 0)
            AddModuleControl(ModuleDefID, "EditResourceKey", "Full Language Editor", "DesktopModules/Admin/Languages/languageeditorext.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Edit, 0)
            AddModuleControl(ModuleDefID, "LanguageSettings", "Language Settings", "DesktopModules/Admin/Languages/LanguageSettings.ascx", "", SecurityAccessLevel.Edit, 0)
            AddModuleControl(ModuleDefID, "TimeZone", "TimeZone Editor", "DesktopModules/Admin/Languages/timezoneeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Host, 0)
            AddModuleControl(ModuleDefID, "Verify", "Resource File Verifier", "DesktopModules/Admin/Languages/resourceverifier.ascx", "", SecurityAccessLevel.Host, 0)
            AddModuleControl(ModuleDefID, "PackageWriter", "Language Pack Writer", "DesktopModules/Admin/Languages/LanguagePackWriter.ascx", "", SecurityAccessLevel.Host, 0)

            'Add Module to Admin Page for all Portals
            AddAdminPages("Languages", "Manage Language Resources.", "~/images/icon_language_16px.gif", "~/images/icon_language_32px.gif", True, ModuleDefID, "Language Editor", "~/images/icon_language_32px.gif")

            'Remove Host Skins Page
            RemoveHostPage("Skins")

            'Remove old Skins module
            DesktopModuleController.DeleteDesktopModule("Skins")

            'Add new Skins module
            ModuleDefID = AddModuleDefinition("Skins", "", "Skins", False, False)
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Skins/editskins.ascx", "~/images/icon_skins_32px.gif", SecurityAccessLevel.View, 0)

            'Add Module to Admin Page for all Portals
            AddAdminPages("Skins", "Manage Skin Resources.", "~/images/icon_skins_16px.gif", "~/images/icon_skins_32px.gif", True, ModuleDefID, "Skin Editor", "~/images/icon_skins_32px.gif")

            'Remove old Skin Designer module
            DesktopModuleController.DeleteDesktopModule("Skin Designer")
            DesktopModuleController.DeleteDesktopModule("SkinDesigner")

            'Add new Skin Designer module
            ModuleDefID = AddModuleDefinition("Skin Designer", "Allows you to modify skin attributes.", "Skin Designer", True, True)
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SkinDesigner/Attributes.ascx", "~/images/icon_skins_32px.gif", SecurityAccessLevel.Host, 0)

            'Add new Skin Designer to every Admin Skins Tab
            AddModuleToPages("//Admin//Skins", ModuleDefID, "Skin Designer", "~/images/icon_skins_32px.gif", True)

            'Remove Admin Whats New Page
            RemoveAdminPages("//Admin//WhatsNew")

            'WhatsNew needs to be set to IsPremium and removed from all portals
            RemoveModuleFromPortals("WhatsNew")

            'Create New WhatsNew Host Page (or get existing one)
            Dim newPage As TabInfo = AddHostPage("What's New", "Provides a summary of the major features for each release.", "~/images/icon_whatsnew_16px.gif", "~/images/icon_whatsnew_32px.gif", True)

            'Add WhatsNew Module To Page
            ModuleDefID = GetModuleDefinition("WhatsNew", "WhatsNew")
            AddModuleToPage(newPage, ModuleDefID, "What's New", "~/images/icon_whatsnew_32px.gif")

            'add console module
            ModuleDefID = AddModuleDefinition("Console", "Display children pages as icon links for navigation.", "Console", "DotNetNuke.Modules.Console.Components.ConsoleController", True, False, False)
            AddModuleControl(ModuleDefID, "", "Console", "DesktopModules/Admin/Console/ViewConsole.ascx", "", SecurityAccessLevel.Anonymous, 0)
            AddModuleControl(ModuleDefID, "Settings", "Console Settings", "DesktopModules/Admin/Console/Settings.ascx", "", SecurityAccessLevel.Admin, 0)

            'add console module to host page
            ModuleID = AddModuleToPage("//Host", Null.NullInteger, ModuleDefID, "Basic Features", "", True)
            Dim tabID As Integer = TabController.GetTabByTabPath(Null.NullInteger, "//Host")
            Dim tab As TabInfo = Nothing

            'add console settings for host page
            If (tabID <> Null.NullInteger) Then
                tab = objTabController.GetTab(tabID, Null.NullInteger, True)
                If (Not tab Is Nothing) Then
                    AddConsoleModuleSettings(tabID, ModuleID)
                End If
            End If

            'add module to all admin pages
            For Each portal As PortalInfo In objPortalController.GetPortals()
                tabID = TabController.GetTabByTabPath(portal.PortalID, "//Admin")
                If (tabID <> Null.NullInteger) Then
                    tab = objTabController.GetTab(tabID, portal.PortalID, True)
                    If (Not tab Is Nothing) Then
                        ModuleID = AddModuleToPage(tab, ModuleDefID, "Basic Features", "", True)
                        AddConsoleModuleSettings(tabID, ModuleID)
                    End If
                End If
            Next

            'Add Google Analytics module
            ModuleDefID = AddModuleDefinition("Google Analytics", "Configure portal Google Analytics settings.", "GoogleAnalytics", False, False)
            AddModuleControl(ModuleDefID, "", "Google Analytics", "DesktopModules/Admin/Analytics/GoogleAnalyticsSettings.ascx", "", SecurityAccessLevel.Admin, 0)
            AddAdminPages("Google Analytics", "Configure portal Google Analytics settings.", "~/images/icon_analytics_16px.gif", "~/images/icon_analytics_32px.gif", True, ModuleDefID, "Google Analytics", "~/images/icon_analytics_32px.gif")
        End Sub

        Private Shared Sub UpgradeToVersion_511()
            'New Admin pages may not have administrator permission
            'Add Admin role if it does not exist for google analytics or extensions
            AddAdminRoleToPage("//Admin//Extensions")
            AddAdminRoleToPage("//Admin//GoogleAnalytics")
        End Sub

        Private Shared Sub UpgradeToVersion_513()
            'Ensure that default language is present (not neccessarily enabled)
            Dim defaultLanguage As Locale = LocaleController.Instance().GetLocale("en-US")
            If defaultLanguage Is Nothing Then
                defaultLanguage = New Locale
            End If
            defaultLanguage.Code = "en-US"
            defaultLanguage.Text = "English (United States)"
            Localization.Localization.SaveLanguage(defaultLanguage)

            'Ensure that there is a Default Authorization System
            Dim package As PackageInfo = PackageController.GetPackageByName("DefaultAuthentication")
            If package Is Nothing Then
                package = New PackageInfo()
                package.Name = "DefaultAuthentication"
                package.FriendlyName = "Default Authentication"
                package.Description = "The Default UserName/Password Authentication System for DotNetNuke."
                package.PackageType = "Auth_System"
                package.Version = New Version(1, 0, 0)
                package.Owner = "DotNetNuke"
                package.License = Localization.Localization.GetString("License", Localization.Localization.GlobalResourceFile)
                package.Organization = "DotNetNuke Corporation"
                package.Url = "www.dotnetnuke.com"
                package.Email = "support@dotnetnuke.com"
                package.ReleaseNotes = "There are no release notes for this version."
                package.IsSystemPackage = True
                PackageController.SavePackage(package)

                'Add Authentication System
                Dim authSystem As Authentication.AuthenticationInfo = Authentication.AuthenticationController.GetAuthenticationServiceByType("DNN")
                If authSystem Is Nothing Then
                    authSystem = New Authentication.AuthenticationInfo
                End If
                authSystem.PackageID = package.PackageID
                authSystem.AuthenticationType = "DNN"
                authSystem.SettingsControlSrc = "DesktopModules/AuthenticationServices/DNN/Settings.ascx"
                authSystem.LoginControlSrc = "DesktopModules/AuthenticationServices/DNN/Login.ascx"
                authSystem.IsEnabled = True

                If authSystem.AuthenticationID = Null.NullInteger Then
                    Authentication.AuthenticationController.AddAuthentication(authSystem)
                Else
                    Authentication.AuthenticationController.UpdateAuthentication(authSystem)
                End If
            End If
        End Sub

        Private Shared Sub UpgradeToVersion_520()
            Dim ModuleDefID As Integer

            'Add new ViewSource control
            AddModuleControl(Null.NullInteger, "ViewSource", "View Module Source", "Admin/Modules/ViewSource.ascx", "~/images/icon_source_32px.gif", SecurityAccessLevel.Host, 0, "", True)

            'Add Marketplace module definition
            ModuleDefID = AddModuleDefinition("Marketplace", "Search for DotNetNuke modules, extension and skins.", "Marketplace")
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Marketplace/Marketplace.ascx", "~/images/icon_marketplace_32px.gif", SecurityAccessLevel.Host, 0)

            'Add marketplace Module To Page
            Dim newPage As TabInfo = AddHostPage("Marketplace", "Search for DotNetNuke modules, extension and skins.", "~/images/icon_marketplace_16px.gif", "~/images/icon_marketplace_32px.gif", True)
            ModuleDefID = GetModuleDefinition("Marketplace", "Marketplace")
            AddModuleToPage(newPage, ModuleDefID, "Marketplace", "~/images/icon_marketplace_32px.gif")
        End Sub

        Private Shared Sub UpgradeToVersion_521()
            ' UpgradeDefaultLanguages is a temporary procedure containing code that
            ' needed to execute after the 5.1.3 application upgrade code above
            DataProvider.Instance.ExecuteNonQuery("UpgradeDefaultLanguages")

            ' This procedure is not intended to be part of the database schema
            ' and is therefore dropped once it has been executed.
            DataProvider.Instance.ExecuteSQL("DROP PROCEDURE {databaseOwner}{objectQualifier}UpgradeDefaultLanguages")
        End Sub

        Private Shared Sub UpgradeToVersion_530()
            Dim ModuleDefID As Integer

            'update languages module
            ModuleDefID = GetModuleDefinition("Languages", "Languages")
            RemoveModuleControl(ModuleDefID, "")
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Languages/languageEnabler.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0, "", True)
            AddModuleControl(ModuleDefID, "Editor", "", "DesktopModules/Admin/Languages/languageeditor.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.View, 0)

            'Add new View Profile module
            ModuleDefID = AddModuleDefinition("ViewProfile", "", "ViewProfile", False, False)
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/ViewProfile/ViewProfile.ascx", "~/images/icon_profile_32px.gif", SecurityAccessLevel.View, 0)
            AddModuleControl(ModuleDefID, "Settings", "Settings", "DesktopModules/Admin/ViewProfile/Settings.ascx", "~/images/icon_profile_32px.gif", SecurityAccessLevel.Edit, 0)

            'Add new Sitemap settings module
            ModuleDefID = AddModuleDefinition("Sitemap", "", "Sitemap", False, False)
            AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Sitemap/SitemapSettings.ascx", "~/images/icon_analytics_32px.gif", SecurityAccessLevel.View, 0)
            AddAdminPages("Search Engine Sitemap", "Configure the sitemap for submission to common search engines.", "~/images/icon_analytics_16px.gif", "~/images/icon_analytics_32px.gif", True, ModuleDefID, "Search Engine Sitemap", "~/images/icon_analytics_32px.gif")

            'Add new Photo Profile field to Host
            Dim objListController As New ListController
            Dim dataTypes As ListEntryInfoCollection = objListController.GetListEntryInfoCollection("DataType")

            Dim properties As ProfilePropertyDefinitionCollection = ProfileController.GetPropertyDefinitionsByPortal(Null.NullInteger)
            ProfileController.AddDefaultDefinition(Null.NullInteger, "Preferences", "Photo", "Image", 0, properties.Count * 2 + 2, UserVisibilityMode.AllUsers, dataTypes)

            Dim strInstallTemplateFile As String = String.Format("{0}Template\UserProfile.page.template", InstallMapPath)
            Dim strHostTemplateFile As String = String.Format("{0}Templates\UserProfile.page.template", HostMapPath)
            If File.Exists(strInstallTemplateFile) Then
                If Not File.Exists(strHostTemplateFile) Then
                    File.Copy(strInstallTemplateFile, strHostTemplateFile)
                End If
            End If
            If File.Exists(strHostTemplateFile) Then
                Dim tabController As New TabController()
                Dim objPortals As New PortalController
                Dim arrPortals As ArrayList = objPortals.GetPortals
                For Each objPortal As PortalInfo In arrPortals
                    properties = ProfileController.GetPropertyDefinitionsByPortal(objPortal.PortalID)

                    'Add new Photo Profile field to Portal
                    ProfileController.AddDefaultDefinition(objPortal.PortalID, "Preferences", "Photo", "Image", 0, properties.Count * 2 + 2, UserVisibilityMode.AllUsers, dataTypes)

                    'Rename old Default Page template
                    Dim DefaultPageTemplatePath As String = String.Format("{0}Templates\Default.page.template", objPortal.HomeDirectoryMapPath)
                    If File.Exists(DefaultPageTemplatePath) Then File.Move(DefaultPageTemplatePath, String.Format("{0}Templates\Default_old.page.template", objPortal.HomeDirectoryMapPath))

                    'Update Default profile template in every portal
                    objPortals.CopyPageTemplate("Default.page.template", objPortal.HomeDirectoryMapPath)

                    'Synchronize the Templates folder to ensure the templates are accessible
                    FileSystemUtils.SynchronizeFolder(objPortal.PortalID, String.Format("{0}Templates\", objPortal.HomeDirectoryMapPath), "Templates/", False, True, True, False)

                    Dim xmlDoc As New XmlDocument
                    Try
                        ' open the XML file
                        xmlDoc.Load(strHostTemplateFile)
                    Catch ex As Exception
                        LogException(ex)
                    End Try

                    'Update SiteSettings to point to the new page
                    If objPortal.UserTabId > Null.NullInteger Then
                        objPortal.RegisterTabId = objPortal.UserTabId
                    Else
                        Dim newTab As New TabInfo()
                        newTab = tabController.DeserializeTab(xmlDoc.SelectSingleNode("//portal/tabs/tab"), Nothing, objPortal.PortalID, PortalTemplateModuleAction.Merge)
                        objPortal.UserTabId = newTab.TabID
                    End If

                    objPortals.UpdatePortalInfo(objPortal)

                    'Add Users folder to every portal
                    Dim strUsersFolder As String = String.Format("{0}Users\", objPortal.HomeDirectoryMapPath)

                    If Not Directory.Exists(strUsersFolder) Then
                        'Create Users folder
                        Directory.CreateDirectory(strUsersFolder)

                        'Synchronize the Users folder to ensure the user folder is accessible
                        FileSystemUtils.SynchronizeFolder(objPortal.PortalID, strUsersFolder, "Users/", False, True, True, False)
                    End If
                Next
            End If

            AddEventQueue_Application_Start_FirstRequest()

            'Change Key for Module Defintions
            ModuleDefID = GetModuleDefinition("Extensions", "Extensions")
            RemoveModuleControl(ModuleDefID, "ImportModuleDefinition")
            AddModuleControl(ModuleDefID, "EditModuleDefinition", "Edit Module Definition", "DesktopModules/Admin/Extensions/Editors/EditModuleDefinition.ascx", "~/images/icon_extensions_32px.gif", SecurityAccessLevel.Host, 0)

            'Module was incorrectly assigned as "IsPremium=False"
            RemoveModuleFromPortals("Users And Roles")
        End Sub

        Private Shared Sub AddEventQueue_Application_Start_FirstRequest()
            'Add new EventQueue Event
            Dim config As DotNetNuke.Services.EventQueue.Config.EventQueueConfiguration = DotNetNuke.Services.EventQueue.Config.EventQueueConfiguration.GetConfig()
            If config IsNot Nothing Then
                If Not config.PublishedEvents.ContainsKey("Application_Start_FirstRequest") Then
                    For Each subscriber As DotNetNuke.Services.EventQueue.Config.SubscriberInfo In config.EventQueueSubscribers.Values
                        DotNetNuke.Services.EventQueue.Config.EventQueueConfiguration.RegisterEventSubscription(config, "Application_Start_FirstRequest", subscriber)
                    Next

                    DotNetNuke.Services.EventQueue.Config.EventQueueConfiguration.SaveConfig(config, String.Format("{0}EventQueue\EventQueue.config", HostMapPath))
                End If
            End If
        End Sub

        Private Shared Sub UpgradeToVersion_540()
            Dim configDoc As XmlDocument = Config.Load()
            Dim configNavigator As XPathNavigator = configDoc.CreateNavigator().SelectSingleNode("/configuration/system.web.extensions")
            If configNavigator Is Nothing Then
                'attempt to remove "System.Web.Extensions" configuration section
                Dim upgradeFile As String = String.Format("{0}\Config\SystemWebExtensions.config", DotNetNuke.Common.InstallMapPath)
                Dim strMessage As String = UpdateConfig(upgradeFile, DotNetNukeContext.Current.Application.Version, "Remove System.Web.Extensions")
                If String.IsNullOrEmpty(strMessage) Then
                    'Log Upgrade
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("UpgradeConfig", "Remove System Web Extensions", PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)
                Else
                    'Log Failed Upgrade
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("UpgradeConfig", String.Format("Remove System Web Extensions failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)
                End If
            End If

            'Add Styles Skin Object
            AddSkinControl("TAGS", "DotNetNuke.TagsSkinObject", "Admin/Skins/Tags.ascx")

            'Add Content List module definition
            Dim moduleDefID As Integer = AddModuleDefinition("ContentList", "This module displays a list of content by tag.", "Content List")
            AddModuleControl(moduleDefID, "", "", "DesktopModules/Admin/ContentList/ContentList.ascx", "", SecurityAccessLevel.View, 0)

            'Update registration page
            Dim objPortals As New PortalController
            Dim arrPortals As ArrayList = objPortals.GetPortals
            For Each objPortal As PortalInfo In arrPortals
                'objPortal.RegisterTabId = objPortal.UserTabId
                objPortals.UpdatePortalInfo(objPortal)

                'Add ContentList to Search Results Page
                Dim tabController As New TabController()
                Dim tabId As Integer = tabController.GetTabByTabPath(objPortal.PortalID, "//SearchResults")
                Dim searchPage As TabInfo = tabController.GetTab(tabId, objPortal.PortalID, False)
                AddModuleToPage(searchPage, moduleDefID, "Results", "")
            Next

        End Sub

        Private Shared Sub UpgradeToVersion_543()
            ' get log file path
            Dim LogFilePath As String = DotNetNuke.Data.DataProvider.Instance().GetProviderPath()
            If Directory.Exists(LogFilePath) Then
                'get log files
                For Each fileName As String In Directory.GetFiles(LogFilePath, "*.log")
                    If File.Exists(fileName & ".resources") Then
                        File.Delete(fileName & ".resources")
                    End If
                    'copy requires use of move
                    File.Move(fileName, fileName & ".resources")
                Next
            End If
        End Sub

        Private Shared Sub UpgradeToVersion_550()
            Dim ModuleDefID As Integer

            'update languages module
            ModuleDefID = GetModuleDefinition("Languages", "Languages")
            AddModuleControl(ModuleDefID, "TranslationStatus", "", "DesktopModules/Admin/Languages/TranslationStatus.ascx", "~/images/icon_language_32px.gif", SecurityAccessLevel.Edit, 0)

            'due to an error in 5.3.0 we need to recheck and readd Application_Start_FirstRequest
            AddEventQueue_Application_Start_FirstRequest()

            ' check if UserProfile page template exists in Host folder and if not, copy it from Install folder
            Dim strInstallTemplateFile As String = String.Format("{0}Templates\UserProfile.page.template", InstallMapPath)
            If File.Exists(strInstallTemplateFile) Then
                Dim strHostTemplateFile As String = String.Format("{0}Templates\UserProfile.page.template", HostMapPath)
                If Not File.Exists(strHostTemplateFile) Then
                    File.Copy(strInstallTemplateFile, strHostTemplateFile)
                End If
            End If

            'Fix the permission for User Folders
            Dim portalCtrl As New PortalController
            Dim folderCtrl As New FolderController
            For Each portal As PortalInfo In portalCtrl.GetPortals
                For Each folder As FolderInfo In folderCtrl.GetFoldersSorted(portal.PortalID).Values
                    If folder.FolderPath.StartsWith("Users/") Then
                        For Each permission As PermissionInfo In PermissionController.GetPermissionsByFolder()
                            If permission.PermissionKey.ToUpper = "READ" Then
                                'Add All Users Read Access to the folder
                                Dim roleId As Integer = Int32.Parse(glbRoleAllUsers)
                                If Not folder.FolderPermissions.Contains(permission.PermissionKey, folder.FolderID, roleId, Null.NullInteger) Then
                                    Dim folderPermission As New FolderPermissionInfo(permission)
                                    folderPermission = New FolderPermissionInfo(permission)
                                    folderPermission.FolderID = folder.FolderID
                                    folderPermission.UserID = Null.NullInteger
                                    folderPermission.RoleID = roleId
                                    folderPermission.AllowAccess = True

                                    folder.FolderPermissions.Add(folderPermission)
                                End If
                            End If
                        Next

                        FolderPermissionController.SaveFolderPermissions(folder)
                    End If
                Next
                'Remove user page template from portal if it exists (from 5.3)
                If File.Exists(String.Format("{0}Templates\UserProfile.page.template", portal.HomeDirectoryMapPath)) Then
                    File.Delete(String.Format("{0}Templates\UserProfile.page.template", portal.HomeDirectoryMapPath))
                End If
            Next

            'DNN-12894 -   Country Code for "United Kingdom" is incorrect
            Dim listController As New ListController()
            Dim listItem As ListEntryInfo = listController.GetListEntryInfo("Country", "UK")
            If listItem IsNot Nothing Then
                listItem.Value = "GB"
                listController.UpdateListEntry(listItem)
            End If


            For Each p As PortalInfo In New PortalController().GetPortals
                'fix issue where portal default language may be disabled
                Dim defaultLanguage As String = p.DefaultLanguage
                If IsLanguageEnabled(p.PortalID, defaultLanguage) = False Then
                    Dim language As Locale = LocaleController.Instance().GetLocale(defaultLanguage)
                    DotNetNuke.Services.Localization.Localization.AddLanguageToPortal(p.PortalID, language.LanguageId, True)
                End If
                'preemptively create any missing localization records rather than relying on dynamic creation
                For Each locale As Locale In LocaleController.Instance().GetLocales(p.PortalID).Values
                    DataProvider.Instance().EnsureLocalizationExists(p.PortalID, locale.Code)
                Next
            Next

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddAdminPages adds an Admin Page and an associated Module to all configured Portals
        ''' </summary>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        '''	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        '''	<param name="ModuleTitle">The Module's title</param>
        '''	<param name="ModuleIconFile">The Module's icon</param>
        '''	<param name="InheritPermissions">Modules Inherit the Pages View Permisions</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Shared Sub AddAdminPages(ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String, ByVal InheritPermissions As Boolean)

            Dim objPortals As New PortalController
            Dim objPortal As PortalInfo
            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim intPortal As Integer
            Dim newPage As TabInfo

            'Add Page to Admin Menu of all configured Portals
            For intPortal = 0 To arrPortals.Count - 1
                objPortal = CType(arrPortals(intPortal), PortalInfo)

                'Create New Admin Page (or get existing one)
                newPage = AddAdminPage(objPortal, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible)

                'Add Module To Page
                AddModuleToPage(newPage, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddAdminPage adds an Admin Tab Page
        ''' </summary>
        '''	<param name="Portal">The Portal</param>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Shared Function AddAdminPage(ByVal Portal As PortalInfo, ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean) As TabInfo

            Dim objTabController As New TabController
            Dim AdminPage As TabInfo = objTabController.GetTab(Portal.AdminTabId, Portal.PortalID, False)

            If Not AdminPage Is Nothing Then
                Dim objTabPermissions As New Security.Permissions.TabPermissionCollection
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Portal.AdministratorRoleId))
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(Portal.AdministratorRoleId))
                Return AddPage(AdminPage, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, objTabPermissions, True)
            Else
                Return Nothing
            End If

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddHostPage adds a Host Tab Page
        ''' </summary>
        '''	<param name="TabName">The Name to give this new Tab</param>
        '''	<param name="TabIconFile">The Icon for this new Tab</param>
        '''	<param name="IsVisible">A flag indicating whether the tab is visible</param>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Shared Function AddHostPage(ByVal TabName As String, ByVal Description As String, ByVal TabIconFile As String, ByVal TabIconFileLarge As String, ByVal IsVisible As Boolean) As TabInfo
            Dim objTabController As New TabController
            Dim HostPage As TabInfo = objTabController.GetTabByName("Host", Null.NullInteger)

            If Not HostPage Is Nothing Then
                Dim objTabPermissions As New Security.Permissions.TabPermissionCollection
                AddPagePermission(objTabPermissions, "View", Convert.ToInt32(Common.Globals.glbRoleSuperUser))
                AddPagePermission(objTabPermissions, "Edit", Convert.ToInt32(Common.Globals.glbRoleSuperUser))
                Return AddPage(HostPage, TabName, Description, TabIconFile, TabIconFileLarge, IsVisible, objTabPermissions, True)
            Else
                Return Nothing
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleControl adds a new Module Control to the system
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="ModuleDefId">The Module Definition Id</param>
        '''	<param name="ControlKey">The key for this control in the Definition</param>
        '''	<param name="ControlTitle">The title of this control</param>
        '''	<param name="ControlSrc">Te source of ths control</param>
        '''	<param name="IconFile">The icon file</param>
        '''	<param name="ControlType">The type of control</param>
        '''	<param name="ViewOrder">The vieworder for this module</param>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Shared Sub AddModuleControl(ByVal ModuleDefId As Integer, ByVal ControlKey As String, ByVal ControlTitle As String, ByVal ControlSrc As String, ByVal IconFile As String, ByVal ControlType As SecurityAccessLevel, ByVal ViewOrder As Integer)

            'Call Overload with HelpUrl = Null.NullString
            AddModuleControl(ModuleDefId, ControlKey, ControlTitle, ControlSrc, IconFile, ControlType, ViewOrder, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleDefinition adds a new Core Module Definition to the system
        ''' </summary>
        ''' <remarks>
        '''	This overload asumes the module is an Admin module and not a Premium Module
        ''' </remarks>
        '''	<param name="DesktopModuleName">The Friendly Name of the Module to Add</param>
        '''	<param name="Description">Description of the Module</param>
        '''	<param name="ModuleDefinitionName">The Module Definition Name</param>
        '''	<returns>The Module Definition Id of the new Module</returns>
        ''' <history>
        ''' 	[cnurse]	10/14/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Shared Function AddModuleDefinition(ByVal DesktopModuleName As String, ByVal Description As String, ByVal ModuleDefinitionName As String) As Integer
            'Call overload with Premium=False and Admin=True
            Return AddModuleDefinition(DesktopModuleName, Description, ModuleDefinitionName, False, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleToPage adds a module to a Page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="page">The Page to add the Module to</param>
        '''	<param name="ModuleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        '''	<param name="ModuleTitle">The Module's title</param>
        '''	<param name="ModuleIconFile">The Module's icon</param>
        '''	<param name="InheritPermissions">Inherit the Pages View Permisions</param>
        ''' <history>
        ''' 	[cnurse]	11/16/2004	created 
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddModuleToPage(ByVal page As TabInfo, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String, ByVal InheritPermissions As Boolean) As Integer
            Dim objModules As New ModuleController
            Dim objModule As New ModuleInfo
            Dim blnDuplicate As Boolean
            Dim moduleId As Integer = Null.NullInteger

            If Not page Is Nothing Then
                blnDuplicate = False
                For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In objModules.GetTabModules(page.TabID)
                    objModule = kvp.Value
                    If objModule.ModuleDefID = ModuleDefId Then
                        blnDuplicate = True
                        moduleId = objModule.ModuleID
                    End If
                Next

                If Not blnDuplicate Then
                    objModule = New ModuleInfo
                    objModule.ModuleID = Null.NullInteger
                    objModule.PortalID = page.PortalID
                    objModule.TabID = page.TabID
                    objModule.ModuleOrder = -1
                    objModule.ModuleTitle = ModuleTitle
                    objModule.PaneName = glbDefaultPane
                    objModule.ModuleDefID = ModuleDefId
                    objModule.CacheTime = 0
                    objModule.IconFile = ModuleIconFile
                    objModule.AllTabs = False
                    objModule.Visibility = VisibilityState.None
                    objModule.InheritViewPermissions = InheritPermissions

                    Try
                        moduleId = objModules.AddModule(objModule)
                    Catch
                        ' error adding module
                    End Try
                End If
            End If

            Return moduleId
        End Function

        Public Shared Function AddModuleToPage(ByVal tabPath As String, ByVal portalId As Integer, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String, ByVal InheritPermissions As Boolean) As Integer
            Dim objTabController As New TabController
            Dim moduleId As Integer = Null.NullInteger

            Dim tabID As Integer = TabController.GetTabByTabPath(portalId, tabPath)
            If (tabID <> Null.NullInteger) Then
                Dim tab As TabInfo = objTabController.GetTab(tabID, portalId, True)
                If (tab IsNot Nothing) Then
                    moduleId = AddModuleToPage(tab, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions)
                End If
            End If
            Return moduleId
        End Function

        Public Shared Sub AddModuleToPages(ByVal tabPath As String, ByVal ModuleDefId As Integer, ByVal ModuleTitle As String, ByVal ModuleIconFile As String, ByVal InheritPermissions As Boolean)
            Dim objPortalController As New PortalController
            Dim objTabController As New TabController

            Dim portals As ArrayList = objPortalController.GetPortals()
            For Each portal As PortalInfo In portals
                Dim tabID As Integer = TabController.GetTabByTabPath(portal.PortalID, tabPath)
                If (tabID <> Null.NullInteger) Then
                    Dim tab As TabInfo = objTabController.GetTab(tabID, portal.PortalID, True)
                    If (tab IsNot Nothing) Then
                        AddModuleToPage(tab, ModuleDefId, ModuleTitle, ModuleIconFile, InheritPermissions)
                    End If
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPortal manages the Installation of a new DotNetNuke Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/06/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AddPortal(ByVal node As XmlNode, ByVal status As Boolean, ByVal indent As Integer) As Integer
            Try
                Dim intPortalId As Integer
                Dim strHostPath As String = Common.Globals.HostMapPath
                Dim strChildPath As String = ""
                Dim strDomain As String = ""

                If Not HttpContext.Current Is Nothing Then
                    strDomain = GetDomainName(HttpContext.Current.Request, True).ToLowerInvariant().Replace("/install", "")
                End If

                Dim strPortalName As String = XmlUtils.GetNodeValue(node, "portalname")
                If status Then
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Creating Portal: " + strPortalName + "<br>")
                End If

                Dim objPortalController As New PortalController
                Dim adminNode As XmlNode = node.SelectSingleNode("administrator")
                Dim strFirstName As String = XmlUtils.GetNodeValue(adminNode, "firstname")
                Dim strLastName As String = XmlUtils.GetNodeValue(adminNode, "lastname")
                Dim strUserName As String = XmlUtils.GetNodeValue(adminNode, "username")
                Dim strPassword As String = XmlUtils.GetNodeValue(adminNode, "password")
                Dim strEmail As String = XmlUtils.GetNodeValue(adminNode, "email")
                Dim strDescription As String = XmlUtils.GetNodeValue(node, "description")
                Dim strKeyWords As String = XmlUtils.GetNodeValue(node, "keywords")
                Dim strTemplate As String = XmlUtils.GetNodeValue(node, "templatefile")
                Dim strServerPath As String = ApplicationMapPath & "\"
                Dim isChild As Boolean = Boolean.Parse(XmlUtils.GetNodeValue(node, "ischild"))
                Dim strHomeDirectory As String = XmlUtils.GetNodeValue(node, "homedirectory")

                'Get the Portal Alias
                Dim portalAliases As XmlNodeList = node.SelectNodes("portalaliases/portalalias")
                Dim strPortalAlias As String = strDomain
                If portalAliases.Count > 0 Then
                    If portalAliases(0).InnerText <> "" Then
                        strPortalAlias = portalAliases(0).InnerText
                    End If
                End If

                'Create default email
                If strEmail = "" Then
                    strEmail = "admin@" + strDomain.Replace("www.", "")
                    'Remove any domain subfolder information ( if it exists )
                    If strEmail.IndexOf("/") <> -1 Then
                        strEmail = strEmail.Substring(0, strEmail.IndexOf("/"))
                    End If
                End If

                If isChild Then
                    strChildPath = Mid(strPortalAlias, InStrRev(strPortalAlias, "/") + 1)
                End If

                'Create Portal
                intPortalId = objPortalController.CreatePortal(strPortalName, strFirstName, strLastName, strUserName, strPassword, strEmail, strDescription, strKeyWords, strHostPath, strTemplate, strHomeDirectory, strPortalAlias, strServerPath, strServerPath & strChildPath, isChild)

                If intPortalId > -1 Then
                    'Add Extra Aliases
                    For Each portalAlias As XmlNode In portalAliases
                        If portalAlias.InnerText <> "" Then
                            If status Then
                                HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "Creating Portal Alias: " + portalAlias.InnerText + "<br>")
                            End If
                            objPortalController.AddPortalAlias(intPortalId, portalAlias.InnerText)
                        End If
                    Next

                    'Force Administrator to Update Password on first log in
                    Dim objPortal As PortalInfo = objPortalController.GetPortal(intPortalId)
                    Dim objAdminUser As UserInfo = UserController.GetUserById(intPortalId, objPortal.AdministratorId)
                    objAdminUser.Membership.UpdatePassword = True
                    UserController.UpdateUser(intPortalId, objAdminUser)
                End If

                Return intPortalId

            Catch ex As Exception
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, indent, "<font color='red'>Error: " + ex.Message + ex.StackTrace + "</font><br>")
                Return -1 ' failure
            End Try
        End Function

        Public Shared Function BuildUserTable(ByVal dr As IDataReader, ByVal header As String, ByVal message As String) As String

            Dim strWarnings As String = Null.NullString
            Dim sbWarnings As New Text.StringBuilder
            Dim hasRows As Boolean = False

            sbWarnings.Append("<h3>" + header + "</h3>")
            sbWarnings.Append("<p>" + message + "</p>")
            sbWarnings.Append("<table cellspacing='4' cellpadding='4' border='0'>")
            sbWarnings.Append("<tr>")
            sbWarnings.Append("<td class='NormalBold'>ID</td>")
            sbWarnings.Append("<td class='NormalBold'>UserName</td>")
            sbWarnings.Append("<td class='NormalBold'>First Name</td>")
            sbWarnings.Append("<td class='NormalBold'>Last Name</td>")
            sbWarnings.Append("<td class='NormalBold'>Email</td>")
            sbWarnings.Append("</tr>")
            While dr.Read
                hasRows = True
                sbWarnings.Append("<tr>")
                sbWarnings.Append("<td class='Norma'>" + dr.GetInt32(0).ToString + "</td>")
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(1) + "</td>")
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(2) + "</td>")
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(3) + "</td>")
                sbWarnings.Append("<td class='Norma'>" + dr.GetString(4) + "</td>")
                sbWarnings.Append("</tr>")
            End While

            sbWarnings.Append("</table>")

            If hasRows Then
                strWarnings = sbWarnings.ToString()
            End If

            Return strWarnings

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CheckUpgrade checks whether there are any possible upgrade issues
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/11/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CheckUpgrade() As String
            Dim dataProvider As DataProvider = Data.DataProvider.Instance()
            Dim dr As IDataReader
            Dim strWarnings As String = Null.NullString
            Dim sbWarnings As New Text.StringBuilder
            Dim hasRows As Boolean = False

            Try
                dr = dataProvider.ExecuteReader("CheckUpgrade")

                strWarnings = BuildUserTable(dr, "Duplicate SuperUsers", "We have detected that the following SuperUsers have duplicate entries as Portal Users.  Although, no longer supported, these users may have been created in early Betas of DNN v3.0.  You need to be aware that after the upgrade, these users will only be able to log in using the Super User Account's password.")

                If dr.NextResult Then
                    strWarnings += BuildUserTable(dr, "Duplicate Portal Users", "We have detected that the following Users have duplicate entries (they exist in more than one portal).  You need to be aware that after the upgrade, the password for some of these users may have been automatically changed (as the system now only uses one password per user, rather than one password per user per portal). It is important to remember that your Users can always retrieve their password using the Password Reminder feature, which will be sent to the Email addess shown in the table.")
                End If

            Catch ex As SqlException
                strWarnings += ex.Message
            Catch ex As Exception
                strWarnings += ex.Message
            End Try

            Try
                dr = dataProvider.ExecuteReader("GetUserCount")
                dr.Read()
                Dim userCount As Integer = dr.GetInt32(0)
                Dim time As Double = userCount / 10834
                If userCount > 1000 Then
                    strWarnings += "<br/><h3>More than 1000 Users</h3><p>This DotNetNuke Database has " + userCount.ToString + " users. As the users and their profiles are transferred to a new format, it is estimated that the script will take ~" + time.ToString("F2") + " minutes to execute.</p>"
                End If
            Catch ex As Exception
                strWarnings += vbCrLf + vbCrLf + ex.Message
            End Try

            Return strWarnings

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteFiles - clean up deprecated files and folders
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="version">The Version being Upgraded</param>
        ''' <history>
        ''' 	[swalker]	11/09/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function DeleteFiles(ByVal strProviderPath As String, ByVal version As System.Version, ByVal writeFeedback As Boolean) As String
            Dim strExceptions As String = ""
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Cleaning Up Files: " + FormatVersion(version))
            End If

            Try
                Dim strListFile As String = DotNetNuke.Common.InstallMapPath & "Cleanup\" & GetStringVersion(version) & ".txt"

                If File.Exists(strListFile) Then
                    strExceptions = FileSystemUtils.DeleteFiles(FileSystemUtils.ReadFile(strListFile).Split(ControlChars.CrLf.ToCharArray()))
                End If
            Catch ex As Exception
                strExceptions += String.Format("Error: {0}{1}", ex.Message + ex.StackTrace, vbCrLf)
                ' log the results
                Try
                    Dim objStream As StreamWriter
                    objStream = File.CreateText(strProviderPath + FormatVersion(version) + "_Config.log")
                    objStream.WriteLine(strExceptions)
                    objStream.Close()
                Catch
                    ' does not have permission to create the log file
                End Try
            End Try

            If writeFeedback Then
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (strExceptions = ""))
            End If

            Return strExceptions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExecuteScripts manages the Execution of Scripts from the Install/Scripts folder.
        ''' It is also triggered by InstallDNN and UpgradeDNN
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        ''' <history>
        ''' 	[cnurse]	05/04/2005	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub ExecuteScripts(ByVal strProviderPath As String)
            Dim arrFiles As String()
            Dim strFile As String
            Dim ScriptPath As String = ApplicationMapPath & "\Install\Scripts\"
            If Directory.Exists(ScriptPath) Then
                arrFiles = Directory.GetFiles(ScriptPath)
                For Each strFile In arrFiles
                    'Execute if script is a provider script
                    If strFile.IndexOf("." + DefaultProvider) <> -1 Then
                        ExecuteScript(strFile, True)
                        ' delete the file
                        Try
                            File.SetAttributes(strFile, FileAttributes.Normal)
                            File.Delete(strFile)
                        Catch
                            ' error removing the file
                        End Try
                    End If
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExecuteScript executes a special script
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strFile">The script file to execute</param>
        ''' <history>
        ''' 	[cnurse]	04/11/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub ExecuteScript(ByVal strFile As String)
            'Execute if script is a provider script
            If strFile.IndexOf("." + DefaultProvider) <> -1 Then
                ExecuteScript(strFile, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetInstallTemplate retrieves the Installation Template as specifeid in web.config
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlDoc">The Xml Document to load</param>
        ''' <returns>A string which contains the error message - if appropriate</returns>
        ''' <history>
        ''' 	[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetInstallTemplate(ByVal xmlDoc As XmlDocument) As String
            Dim strErrorMessage As String = Null.NullString
            Dim installTemplate As String = Config.GetSetting("InstallTemplate")
            Try
                xmlDoc.Load(Common.Globals.ApplicationMapPath & "\Install\" & installTemplate)
            Catch    ' error
                strErrorMessage = "Failed to load Install template.<br><br>"
            End Try

            Return strErrorMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetInstallVersion retrieves the Base Instal Version as specifeid in the install
        ''' template
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlDoc">The Install Template</param>
        ''' <history>
        ''' 	[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetInstallVersion(ByVal xmlDoc As XmlDocument) As System.Version
            Dim strVersion As String = Null.NullString
            Dim node As XmlNode

            'get base version
            node = xmlDoc.SelectSingleNode("//dotnetnuke")
            If Not node Is Nothing Then
                strVersion = XmlUtils.GetNodeValue(node, "version")
            End If

            Return New System.Version(strVersion)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetLogFile gets the filename for the version's log file
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        '''	<param name="version">The Version</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetLogFile(ByVal strProviderPath As String, ByVal version As System.Version) As String
            Return strProviderPath + GetStringVersion(version) + ".log.resources"
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetScriptFile gets the filename for the version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        '''	<param name="version">The Version</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetScriptFile(ByVal strProviderPath As String, ByVal version As System.Version) As String
            Return strProviderPath + GetStringVersion(version) + "." + DefaultProvider
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetStringVersion gets the Version String (xx.xx.xx) from the Version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="version">The Version</param>
        ''' <history>
        ''' 	[cnurse]	02/15/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetStringVersion(ByVal version As System.Version) As String
            Dim intVersion As Integer()
            ReDim intVersion(2)
            intVersion(0) = version.Major
            intVersion(1) = version.Minor
            intVersion(2) = version.Build
            Dim strVersion As String = Null.NullString
            For i As Integer = 0 To 2
                Select Case intVersion(i)
                    Case 0
                        strVersion += "00"
                    Case 1 To 9
                        strVersion += "0" + intVersion(i).ToString
                    Case Is >= 10
                        strVersion += intVersion(i).ToString
                End Select
                If i < 2 Then
                    strVersion += "."
                End If
            Next
            Return strVersion
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSuperUser gets the superuser from the Install Template
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlTemplate">The install Templae</param>
        '''	<param name="writeFeedback">a flag to determine whether to output feedback</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSuperUser(ByVal xmlTemplate As XmlDocument, ByVal writeFeedback As Boolean) As UserInfo
            Dim node As XmlNode = xmlTemplate.SelectSingleNode("//dotnetnuke/superuser")
            Dim objSuperUserInfo As UserInfo = Nothing
            If Not node Is Nothing Then
                If writeFeedback Then
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Configuring SuperUser:<br>")
                End If

                'Parse the SuperUsers nodes
                Dim strFirstName As String = XmlUtils.GetNodeValue(node, "firstname")
                Dim strLastName As String = XmlUtils.GetNodeValue(node, "lastname")
                Dim strUserName As String = XmlUtils.GetNodeValue(node, "username")
                Dim strPassword As String = XmlUtils.GetNodeValue(node, "password")
                Dim strEmail As String = XmlUtils.GetNodeValue(node, "email")
                Dim strLocale As String = XmlUtils.GetNodeValue(node, "locale")
                Dim timeZone As Integer = XmlUtils.GetNodeValueInt(node, "timezone")

                objSuperUserInfo = New UserInfo
                objSuperUserInfo.PortalID = -1
                objSuperUserInfo.FirstName = strFirstName
                objSuperUserInfo.LastName = strLastName
                objSuperUserInfo.Username = strUserName
                objSuperUserInfo.DisplayName = strFirstName + " " + strLastName
                objSuperUserInfo.Membership.Password = strPassword
                objSuperUserInfo.Email = strEmail
                objSuperUserInfo.IsSuperUser = True
                objSuperUserInfo.Membership.Approved = True

                objSuperUserInfo.Profile.FirstName = strFirstName
                objSuperUserInfo.Profile.LastName = strLastName
                objSuperUserInfo.Profile.PreferredLocale = strLocale
                objSuperUserInfo.Profile.TimeZone = timeZone
            End If
            Return objSuperUserInfo
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetUpgradeScripts gets an ArrayList of the Scripts required to Upgrade to the
        ''' current Assembly Version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        '''	<param name="databaseVersion">The current Database Version</param>
        ''' <history>
        ''' 	[cnurse]	02/14/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetUpgradeScripts(ByVal strProviderPath As String, ByVal databaseVersion As Version) As ArrayList
            Dim scriptVersion As Version
            Dim arrScriptFiles As New ArrayList
            Dim strFile As String
            Dim arrFiles As String() = Directory.GetFiles(strProviderPath, "*." & DefaultProvider)
            For Each strFile In arrFiles
                ' script file name must conform to ##.##.##.DefaultProviderName
                If Len(Path.GetFileName(strFile)) = 9 + Len(DefaultProvider) Then
                    scriptVersion = New System.Version(Path.GetFileNameWithoutExtension(strFile))
                    ' check if script file is relevant for upgrade
                    If scriptVersion > databaseVersion AndAlso scriptVersion <= DotNetNukeContext.Current.Application.Version Then
                        arrScriptFiles.Add(strFile)
                    End If
                End If
            Next
            arrScriptFiles.Sort()

            Return arrScriptFiles
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InitialiseHostSettings gets the Host Settings from the Install Template
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlTemplate">The install Templae</param>
        '''	<param name="writeFeedback">a flag to determine whether to output feedback</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub InitialiseHostSettings(ByVal xmlTemplate As XmlDocument, ByVal writeFeedback As Boolean)
            Dim node As XmlNode = xmlTemplate.SelectSingleNode("//dotnetnuke/settings")
            If Not node Is Nothing Then
                If writeFeedback Then
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Loading Host Settings:<br>")
                End If

                Dim settingNode As XmlNode

                'Parse the Settings nodes
                For Each settingNode In node.ChildNodes
                    Dim strSettingName As String = settingNode.Name
                    Dim strSettingValue As String = settingNode.InnerText
                    Dim SecureAttrib As XmlAttribute = settingNode.Attributes("Secure")
                    Dim SettingIsSecure As Boolean = False
                    If Not SecureAttrib Is Nothing Then
                        If SecureAttrib.Value.ToLower = "true" Then
                            SettingIsSecure = True
                        End If
                    End If

                    Dim strDomainName As String = GetDomainName(HttpContext.Current.Request)

                    Select Case strSettingName
                        Case "HostURL"
                            If strSettingValue = "" Then
                                strSettingValue = strDomainName
                            End If
                        Case "HostEmail"
                            If strSettingValue = "" Then
                                strSettingValue = "support@" + strDomainName

                                'Remove any folders
                                strSettingValue = strSettingValue.Substring(0, strSettingValue.IndexOf("/"))
                                'Remove port number
                                If Not strSettingValue.IndexOf(":") = -1 Then
                                    strSettingValue = strSettingValue.Substring(0, strSettingValue.IndexOf(":"))
                                End If
                            End If

                    End Select

                    HostController.Instance.Update(New ConfigurationSetting() With {.Key = strSettingName, .Value = strSettingValue, .IsSecure = SettingIsSecure})

                Next
                'Need to clear the cache to pick up new HostSettings from the SQLDataProvider script
                DataCache.RemoveCache("GetHostSettings")
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallDatabase runs all the "scripts" identifed in the Install Template to 
        ''' install the base version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlDoc">The Xml Document to load</param>
        ''' <param name="writeFeedback">A flag that determines whether to output feedback to the Response Stream</param>
        ''' <returns>A string which contains the error message - if appropriate</returns>
        ''' <history>
        ''' 	[cnurse]	02/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InstallDatabase(ByVal version As System.Version, ByVal strProviderPath As String, ByVal xmlDoc As XmlDocument, ByVal writeFeedback As Boolean) As String
            Dim node As XmlNode
            Dim strScript As String = Null.NullString
            Dim strDefaultProvider As String = Config.GetDefaultProvider("data").Name
            Dim strMessage As String = Null.NullString

            'Output feedback line
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing Version: " + FormatVersion(version) + "<br>")
            End If

            'Parse the script nodes
            node = xmlDoc.SelectSingleNode("//dotnetnuke/scripts")
            If Not node Is Nothing Then
                ' Loop through the available scripts
                For Each scriptNode As XmlNode In node.SelectNodes("script")
                    strScript = scriptNode.InnerText + "." + strDefaultProvider
                    strMessage += ExecuteScript(strProviderPath & strScript, writeFeedback)
                Next
            End If

            ' update the version
            Globals.UpdateDataBaseVersion(version)

            'Optionally Install the memberRoleProvider
            strMessage += InstallMemberRoleProvider(strProviderPath, writeFeedback)

            Return strMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallDNN manages the Installation of a new DotNetNuke Application
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        ''' <history>
        ''' 	[cnurse]	11/06/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub InstallDNN(ByVal strProviderPath As String)

            Dim strExceptions As String = ""
            Dim intPortalId As Integer
            Dim strHostPath As String = Common.Globals.HostMapPath
            Dim xmlDoc As New XmlDocument
            Dim node As XmlNode
            Dim nodes As XmlNodeList
            Dim strScript As String = ""
            Dim strLogFile As String = ""
            Dim strErrorMessage As String = ""

            ' open the Install Template XML file
            strErrorMessage = GetInstallTemplate(xmlDoc)

            If strErrorMessage = "" Then
                'get base version
                Dim baseVersion As System.Version = GetInstallVersion(xmlDoc)

                'Install Base Version
                strErrorMessage = InstallDatabase(baseVersion, strProviderPath, xmlDoc, True)

                'Call Upgrade with the current DB Version to carry out any incremental upgrades
                UpgradeDNN(strProviderPath, baseVersion)

                ' parse Host Settings if available
                InitialiseHostSettings(xmlDoc, True)

                ' parse SuperUser if Available
                Dim superUser As UserInfo = GetSuperUser(xmlDoc, True)
                If superUser.Membership.Password.Contains("host") Then
                    superUser.Membership.UpdatePassword = True
                End If
                UserController.CreateUser(superUser)

                ' parse File List if available
                InstallFiles(xmlDoc, True)

                'Run any addition scripts in the Scripts folder
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Executing Additional Scripts:<br>")
                ExecuteScripts(strProviderPath)

                'Install optional resources if present
                InstallPackages("Module", True)
                InstallPackages("Skin", True)
                InstallPackages("Container", True)
                InstallPackages("Language", True)
                InstallPackages("Provider", True)
                InstallPackages("AuthSystem", True)
                InstallPackages("Package", True)

                'Set Status to None
                Globals.SetStatus(UpgradeStatus.None)

                ' parse portal(s) if available
                nodes = xmlDoc.SelectNodes("//dotnetnuke/portals/portal")
                For Each node In nodes
                    If Not node Is Nothing Then
                        intPortalId = AddPortal(node, True, 2)
                        If intPortalId > -1 Then
                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "<font color='green'>Successfully Installed Portal " & intPortalId & ":</font><br>")
                        Else
                            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "<font color='red'>Portal failed to install:Error!</font><br>")
                        End If
                    End If
                Next
            Else
                '500 Error - Redirect to ErrorPage
                If Not HttpContext.Current Is Nothing Then
                    Dim strURL As String = "~/ErrorPage.aspx?status=500&error=" & strErrorMessage
                    HttpContext.Current.Response.Clear()
                    HttpContext.Current.Server.Transfer(strURL)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallFiles intsalls any files listed in the Host Install Configuration file
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="xmlDoc">The Xml Document to load</param>
        ''' <param name="writeFeedback">A flag that determines whether to output feedback to the Response Stream</param>
        ''' <history>
        ''' 	[cnurse]	02/19/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub InstallFiles(ByVal xmlDoc As XmlDocument, ByVal writeFeedback As Boolean)
            Dim node As XmlNode

            'Parse the file nodes
            node = xmlDoc.SelectSingleNode("//dotnetnuke/files")
            If Not node Is Nothing Then
                If writeFeedback Then
                    HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Loading Host Files:<br>")
                End If
                ParseFiles(node, Null.NullInteger)
            End If

            'Synchronise Host Folder
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Synchronizing Host Files:<br>")
            End If
            FileSystemUtils.SynchronizeFolder(Null.NullInteger, Common.Globals.HostMapPath, "", True, False, True, False)

        End Sub

        Public Shared Function InstallPackage(ByVal strFile As String, ByVal packageType As String, ByVal writeFeedback As Boolean) As Boolean
            Dim blnSuccess As Boolean = Null.NullBoolean
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Installing Package File " & Path.GetFileNameWithoutExtension(strFile) & ": ")
            End If

            Dim deleteTempFolder As Boolean = True
            If packageType = "Skin" OrElse packageType = "Container" Then
                deleteTempFolder = Null.NullBoolean
            End If

            Dim objInstaller As New Installer.Installer(New FileStream(strFile, FileMode.Open, FileAccess.Read), Common.Globals.ApplicationMapPath, True, deleteTempFolder)

            'Check if manifest is valid
            If objInstaller.IsValid Then
                objInstaller.InstallerInfo.RepairInstall = True
                blnSuccess = objInstaller.Install()
            Else
                If objInstaller.InstallerInfo.ManifestFile Is Nothing Then
                    'Missing manifest
                    If packageType = "Skin" OrElse packageType = "Container" Then
                        'Legacy Skin/Container
                        Dim TempInstallFolder As String = objInstaller.TempInstallFolder
                        Dim ManifestFile As String = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(strFile) + ".dnn")
                        Dim manifestWriter As New StreamWriter(ManifestFile)
                        manifestWriter.Write(LegacyUtil.CreateSkinManifest(strFile, packageType, TempInstallFolder))
                        manifestWriter.Close()

                        objInstaller = New Installer.Installer(TempInstallFolder, ManifestFile, HttpContext.Current.Request.MapPath("."), True)

                        'Set the Repair flag to true for Batch Install
                        objInstaller.InstallerInfo.RepairInstall = True

                        blnSuccess = objInstaller.Install()
                    End If
                Else
                    blnSuccess = False
                End If
            End If

            If writeFeedback Then
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, blnSuccess)
            End If
            If blnSuccess Then
                ' delete file
                Try
                    File.SetAttributes(strFile, FileAttributes.Normal)
                    File.Delete(strFile)
                Catch
                    ' error removing the file
                End Try
            End If
            Return blnSuccess
        End Function

        Public Shared Sub InstallPackages(ByVal packageType As String, ByVal writeFeedback As Boolean)
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Installing Optional " + packageType + "s:<br>")
            End If
            Dim InstallPackagePath As String = ApplicationMapPath & "\Install\" + packageType
            If Directory.Exists(InstallPackagePath) Then
                For Each strFile As String In Directory.GetFiles(InstallPackagePath)
                    ' check if valid custom module
                    If Path.GetExtension(strFile.ToLower) = ".zip" Then
                        InstallPackage(strFile, packageType, writeFeedback)
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' IsSiteProtectedAgainstPaddingOracleAttack ensures that the website is protected
        ''' from the Padding Oracle Encryption Attack.  
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>This will no longer be needed once Microsoft release an update to ASP.NET</remarks>
        Friend Shared Function IsSiteProtectedAgainstPaddingOracleAttack() As Boolean
            Dim isSiteProtected As Boolean = True

            'Look for customErrors attribute
            Dim configFile As XmlDocument = Config.Load()
            If configFile IsNot Nothing Then
                Dim navigator As XPathNavigator = configFile.CreateNavigator()
                If navigator IsNot Nothing Then
                    Dim configNavigator As XPathNavigator = navigator.SelectSingleNode("//configuration/system.web/customErrors")
                    If configNavigator IsNot Nothing Then
                        'Check mode
                        Dim attributeString As String = configNavigator.GetAttribute("mode", "")
                        If String.IsNullOrEmpty(attributeString) OrElse attributeString.ToLowerInvariant = "off" Then
                            isSiteProtected = False
                        End If

                        'Check redirectMode
                        If isSiteProtected Then
                            attributeString = configNavigator.GetAttribute("redirectMode", "")
                            If String.IsNullOrEmpty(attributeString) OrElse attributeString <> "ResponseRewrite" Then
                                isSiteProtected = False
                            End If

                            'Check defaultRedirect
                            If isSiteProtected Then
                                attributeString = configNavigator.GetAttribute("defaultRedirect", "")
                                If String.IsNullOrEmpty(attributeString) OrElse attributeString.ToLowerInvariant <> "~/errorpage.aspx" Then
                                    isSiteProtected = False
                                End If
                            End If
                        End If
                    Else
                        isSiteProtected = False
                    End If
                Else
                    isSiteProtected = False
                End If
            Else
                isSiteProtected = False
            End If

            Return isSiteProtected
        End Function

        Public Shared Function IsNETFrameworkCurrent(ByVal version As String) As Boolean
            Dim isCurrent As Boolean = Null.NullBoolean
            Select Case version
                Case "3.5"
                    'Try and instantiate a 3.5 Class
                    If Framework.Reflection.CreateType("System.Data.Linq.DataContext", True) IsNot Nothing Then
                        isCurrent = True
                    End If
                Case "4.0"
                    'Look for requestValidationMode attribute
                    Dim configFile As XmlDocument = Config.Load()
                    Dim configNavigator As XPathNavigator = configFile.CreateNavigator().SelectSingleNode("//configuration/system.web/httpRuntime")
                    If configNavigator IsNot Nothing AndAlso _
                            Not String.IsNullOrEmpty(configNavigator.GetAttribute("requestValidationMode", "")) Then
                        isCurrent = True
                    End If
            End Select
            Return isCurrent
        End Function

        Public Shared Sub RemoveAdminPages(ByVal tabPath As String)
            Dim objPortalController As New PortalController
            Dim objTabController As New TabController

            Dim portals As ArrayList = objPortalController.GetPortals()
            For Each portal As PortalInfo In portals
                Dim tabID As Integer = TabController.GetTabByTabPath(portal.PortalID, tabPath)
                If (tabID <> Null.NullInteger) Then
                    objTabController.DeleteTab(tabID, portal.PortalID)
                End If
            Next
        End Sub

        Public Shared Sub RemoveHostPage(ByVal pageName As String)
            Dim controller As New TabController
            Dim skinsTab As TabInfo = controller.GetTabByName(pageName, Null.NullInteger)
            If skinsTab IsNot Nothing Then
                controller.DeleteTab(skinsTab.TabID, Null.NullInteger)
            End If
        End Sub

        Public Shared Sub StartTimer()

            'Start Upgrade Timer
            startTime = Now()

        End Sub

        Public Shared Sub TryUpgradeNETFramework()
            Select Case Common.Globals.NETFrameworkVersion.ToString(2)
                Case "3.5"
                    If Not IsNETFrameworkCurrent("3.5") Then
                        'Upgrade to .NET 3.5
                        Dim upgradeFile As String = String.Format("{0}\Config\Net35.config", DotNetNuke.Common.InstallMapPath)
                        Dim strMessage As String = UpdateConfig(upgradeFile, DotNetNukeContext.Current.Application.Version, ".NET 3.5 Upgrade")
                        If String.IsNullOrEmpty(strMessage) Then
                            'Remove old AJAX file
                            FileSystemUtils.DeleteFile(Path.Combine(ApplicationMapPath, "bin\System.Web.Extensions.dll"))

                            'Log Upgrade
                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog("UpgradeNet", "Upgraded Site to .NET 3.5", PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)

                        Else
                            'Log Failed Upgrade
                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog("UpgradeNet", String.Format("Upgrade to .NET 3.5 failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)
                        End If
                    End If
                Case "4.0"
                    If Not IsNETFrameworkCurrent("4.0") Then
                        'Upgrade to .NET 4.0
                        Dim upgradeFile As String = String.Format("{0}\Config\Net40.config", DotNetNuke.Common.InstallMapPath)
                        Dim strMessage As String = UpdateConfig(upgradeFile, DotNetNukeContext.Current.Application.Version, ".NET 4.0 Upgrade")
                        If String.IsNullOrEmpty(strMessage) Then
                            'Log Upgrade
                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog("UpgradeNet", "Upgraded Site to .NET 4.0", PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)
                        Else
                            'Log Failed Upgrade
                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog("UpgradeNet", String.Format("Upgrade to .NET 4.0 failed. Error reported during attempt to update:{0}", strMessage), PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.HOST_ALERT)
                        End If
                    End If
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpgradeApplication - This overload is used for general application upgrade operations. 
        ''' </summary>
        ''' <remarks>
        '''	Since it is not version specific and is invoked whenever the application is 
        '''	restarted, the operations must be re-executable.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/6/2004	documented
        '''     [cnurse]    02/27/2007  made public so it can be called from Wizard
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpgradeApplication()

            Dim objTabController As New TabController
            Dim HostPage As TabInfo = objTabController.GetTabByName("Host", Null.NullInteger)
            Dim newPage As TabInfo

            Dim ModuleDefID As Integer

            Try
                ' remove the system message module from the admin tab
                ' System Messages are now managed through Localization
                If CoreModuleExists("System Messages") Then
                    RemoveCoreModule("System Messages", "Admin", "Site Settings", False)
                End If

                ' remove portal alias module
                If CoreModuleExists("PortalAliases") Then
                    RemoveCoreModule("PortalAliases", "Admin", "Site Settings", False)
                End If

                ' add the log viewer module to the admin tab
                If CoreModuleExists("LogViewer") = False Then
                    ModuleDefID = AddModuleDefinition("LogViewer", "Allows you to view log entries for portal events.", "Log Viewer")
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/LogViewer/LogViewer.ascx", "", SecurityAccessLevel.Admin, 0)
                    AddModuleControl(ModuleDefID, "Edit", "Edit Log Settings", "DesktopModules/Admin/LogViewer/EditLogTypes.ascx", "", SecurityAccessLevel.Host, 0)

                    'Add the Module/Page to all configured portals
                    AddAdminPages("Log Viewer", "View a historical log of database events such as event schedules, exceptions, account logins, module and page changes, user account activities, security role activities, etc.", "icon_viewstats_16px.gif", "icon_viewstats_32px.gif", True, ModuleDefID, "Log Viewer", "icon_viewstats_16px.gif")
                End If

                ' add the schedule module to the host tab
                If CoreModuleExists("Scheduler") = False Then
                    ModuleDefID = AddModuleDefinition("Scheduler", "Allows you to schedule tasks to be run at specified intervals.", "Scheduler")
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Scheduler/ViewSchedule.ascx", "", SecurityAccessLevel.Admin, 0)
                    AddModuleControl(ModuleDefID, "Edit", "Edit Schedule", "DesktopModules/Admin/Scheduler/EditSchedule.ascx", "", SecurityAccessLevel.Host, 0)
                    AddModuleControl(ModuleDefID, "History", "Schedule History", "DesktopModules/Admin/Scheduler/ViewScheduleHistory.ascx", "", SecurityAccessLevel.Host, 0)
                    AddModuleControl(ModuleDefID, "Status", "Schedule Status", "DesktopModules/Admin/Scheduler/ViewScheduleStatus.ascx", "", SecurityAccessLevel.Host, 0)

                    'Create New Host Page (or get existing one)
                    newPage = AddHostPage("Schedule", "Add, modify and delete scheduled tasks to be run at specified intervals.", "icon_scheduler_16px.gif", "icon_scheduler_32px.gif", True)

                    'Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Schedule", "icon_scheduler_16px.gif")
                End If

                ' add the Search Admin module to the host tab
                If CoreModuleExists("SearchAdmin") = False Then
                    ModuleDefID = AddModuleDefinition("SearchAdmin", "The Search Admininstrator provides the ability to manage search settings.", "Search Admin")
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchAdmin/SearchAdmin.ascx", "", SecurityAccessLevel.Host, 0)

                    'Create New Host Page (or get existing one)
                    newPage = AddHostPage("Search Admin", "Manage search settings associated with DotNetNuke's search capability.", "icon_search_16px.gif", "icon_search_32px.gif", True)

                    'Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Search Admin", "icon_search_16px.gif")
                End If

                ' add the Search Input module
                If CoreModuleExists("SearchInput") = False Then
                    ModuleDefID = AddModuleDefinition("SearchInput", "The Search Input module provides the ability to submit a search to a given search results module.", "Search Input", False, False)
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchInput/SearchInput.ascx", "", SecurityAccessLevel.Anonymous, 0)
                    AddModuleControl(ModuleDefID, "Settings", "Search Input Settings", "DesktopModules/Admin/SearchInput/Settings.ascx", "", SecurityAccessLevel.Edit, 0)
                End If

                ' add the Search Results module
                If CoreModuleExists("SearchResults") = False Then
                    ModuleDefID = AddModuleDefinition("SearchResults", "The Search Reasults module provides the ability to display search results.", "Search Results", False, False)
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SearchResults/SearchResults.ascx", "", SecurityAccessLevel.Anonymous, 0)
                    AddModuleControl(ModuleDefID, "Settings", "Search Results Settings", "DesktopModules/Admin/SearchResults/Settings.ascx", "", SecurityAccessLevel.Edit, 0)

                    'Add the Search Module/Page to all configured portals
                    AddSearchResults(ModuleDefID)
                End If

                ' add the site wizard module to the admin tab 
                If CoreModuleExists("SiteWizard") = False Then
                    ModuleDefID = AddModuleDefinition("SiteWizard", "The Administrator can use this user-friendly wizard to set up the common Extensions of the Portal/Site.", "Site Wizard")
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/SiteWizard/Sitewizard.ascx", "", SecurityAccessLevel.Admin, 0)
                    AddAdminPages("Site Wizard", "Configure portal settings, page design and apply a site template using a step-by-step wizard.", "icon_wizard_16px.gif", "icon_wizard_32px.gif", True, ModuleDefID, "Site Wizard", "icon_wizard_16px.gif")
                End If

                'add Lists module and tab
                If HostTabExists("Lists") = False Then
                    ModuleDefID = AddModuleDefinition("Lists", "Allows you to edit common lists.", "Lists")
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Lists/ListEditor.ascx", "", SecurityAccessLevel.Host, 0)

                    'Create New Host Page (or get existing one)
                    newPage = AddHostPage("Lists", "Manage common lists.", "icon_lists_16px.gif", "icon_lists_32px.gif", True)

                    'Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Lists", "icon_lists_16px.gif")
                End If

                If HostTabExists("Superuser Accounts") = False Then
                    'add SuperUser Accounts module and tab
                    Dim objDesktopModuleInfo As DesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("Security", Null.NullInteger)
                    ModuleDefID = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("User Accounts", objDesktopModuleInfo.DesktopModuleID).ModuleDefID

                    'Create New Host Page (or get existing one)
                    newPage = AddHostPage("Superuser Accounts", "Manage host user accounts.", "icon_users_16px.gif", "icon_users_32px.gif", True)

                    'Add Module To Page
                    AddModuleToPage(newPage, ModuleDefID, "Superuser Accounts", "icon_users_32px.gif")
                End If

                'Add Edit Role Groups
                ModuleDefID = GetModuleDefinition("Security", "Security Roles")
                AddModuleControl(ModuleDefID, "EditGroup", "Edit Role Groups", "DesktopModules/Admin/Security/EditGroups.ascx", "icon_securityroles_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger)
                AddModuleControl(ModuleDefID, "UserSettings", "Manage User Settings", "DesktopModules/Admin/Security/UserSettings.ascx", "~/images/settings.gif", SecurityAccessLevel.Edit, Null.NullInteger)

                'Add User Accounts Controls
                ModuleDefID = GetModuleDefinition("Security", "User Accounts")
                AddModuleControl(ModuleDefID, "ManageProfile", "Manage Profile Definition", "DesktopModules/Admin/Security/ProfileDefinitions.ascx", "icon_users_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger)
                AddModuleControl(ModuleDefID, "EditProfileProperty", "Edit Profile Property Definition", "DesktopModules/Admin/Security/EditProfileDefinition.ascx", "icon_users_32px.gif", SecurityAccessLevel.Edit, Null.NullInteger)
                AddModuleControl(ModuleDefID, "UserSettings", "Manage User Settings", "DesktopModules/Admin/Security/UserSettings.ascx", "~/images/settings.gif", SecurityAccessLevel.Edit, Null.NullInteger)
                AddModuleControl(Null.NullInteger, "Profile", "Profile", "DesktopModules/Admin/Security/ManageUsers.ascx", "icon_users_32px.gif", SecurityAccessLevel.Anonymous, Null.NullInteger)
                AddModuleControl(Null.NullInteger, "SendPassword", "Send Password", "DesktopModules/Admin/Security/SendPassword.ascx", "", SecurityAccessLevel.Anonymous, Null.NullInteger)
                AddModuleControl(Null.NullInteger, "ViewProfile", "View Profile", "DesktopModules/Admin/Security/ViewProfile.ascx", "icon_users_32px.gif", SecurityAccessLevel.Anonymous, Null.NullInteger)

                'Update Child Portal subHost.aspx
                Dim objAliasController As New PortalAliasController
                Dim arrAliases As ArrayList = objAliasController.GetPortalAliasArrayByPortalID(Null.NullInteger)
                Dim objAlias As PortalAliasInfo
                Dim childPath As String

                For Each objAlias In arrAliases
                    'For the alias to be for a child it must be of the form ...../child
                    Dim intChild As Integer = objAlias.HTTPAlias.IndexOf("/")
                    If intChild <> -1 And intChild <> (objAlias.HTTPAlias.Length - 1) Then
                        childPath = ApplicationMapPath & "\" & objAlias.HTTPAlias.Substring(intChild + 1)
                        ' check if Folder exists
                        If Directory.Exists(childPath) Then
                            Dim objDefault As System.IO.FileInfo = New System.IO.FileInfo(childPath & "\" & glbDefaultPage)
                            Dim objSubHost As System.IO.FileInfo = New System.IO.FileInfo(Common.Globals.HostMapPath & "subhost.aspx")
                            ' check if upgrade is necessary
                            If objDefault.Length <> objSubHost.Length Then
                                'Rename existing file 
                                System.IO.File.Copy(childPath & "\" & glbDefaultPage, childPath & "\old_" & glbDefaultPage, True)
                                ' create the subhost default.aspx file
                                System.IO.File.Copy(Common.Globals.HostMapPath & "subhost.aspx", childPath & "\" & glbDefaultPage, True)
                            End If
                        End If
                    End If
                Next

                ' add the solutions explorer module to the admin tab 
                If CoreModuleExists("Solutions") = False Then
                    ModuleDefID = AddModuleDefinition("Solutions", "Browse additional solutions for your application.", "Solutions", False, False)
                    AddModuleControl(ModuleDefID, "", "", "DesktopModules/Admin/Solutions/Solutions.ascx", "", SecurityAccessLevel.Admin, 0)
                    AddAdminPages("Solutions", "DotNetNuke Solutions Explorer page provides easy access to locate free and commercial DotNetNuke modules, skin and more.", "icon_solutions_16px.gif", "icon_solutions_32px.gif", True, ModuleDefID, "Solutions Explorer", "icon_solutions_32px.gif")
                End If


                'Add Search Skin Object
                AddSkinControl("SEARCH", "DotNetNuke.SearchSkinObject", "Admin/Skins/Search.ascx")

                'Add TreeView Skin Object
                AddSkinControl("TREEVIEW", "DotNetNuke.TreeViewSkinObject", "Admin/Skins/TreeViewMenu.ascx")

                'Add Text Skin Object
                AddSkinControl("TEXT", "DotNetNuke.TextSkinObject", "Admin/Skins/Text.ascx")

                'Add Styles Skin Object
                AddSkinControl("STYLES", "DotNetNuke.StylesSkinObject", "Admin/Skins/Styles.ascx")

            Catch ex As Exception
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Upgraded DotNetNuke", "General")
                objEventLogInfo.AddProperty("Warnings", "Error: " & ex.Message & vbCrLf)
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLogInfo.BypassBuffering = True
                objEventLog.AddLog(objEventLogInfo)
                Try
                    LogException(ex)
                Catch
                    ' ignore
                End Try

            End Try

            'Remove any .txt and .config files that may exist in the Install folder
            For Each strFile As String In Directory.GetFiles(DotNetNuke.Common.InstallMapPath & "Cleanup\", "??.??.??.txt")
                FileSystemUtils.DeleteFile(strFile)
            Next
            For Each strFile As String In Directory.GetFiles(DotNetNuke.Common.InstallMapPath & "Config\", "??.??.??.config")
                FileSystemUtils.DeleteFile(strFile)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpgradeApplication - This overload is used for version specific application upgrade operations. 
        ''' </summary>
        ''' <remarks>
        '''	This should be used for file system modifications or upgrade operations which 
        '''	should only happen once. Database references are not recommended because future 
        '''	versions of the application may result in code incompatibilties.
        ''' </remarks>
        '''	<param name="Version">The Version being Upgraded</param>
        ''' <history>
        ''' 	[cnurse]	11/6/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UpgradeApplication(ByVal strProviderPath As String, ByVal version As System.Version, ByVal writeFeedback As Boolean) As String
            Dim strExceptions As String = ""
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, "Executing Application Upgrades: " + FormatVersion(version))
            End If
            Try
                Select Case version.ToString(3)
                    Case "3.2.3"
                        UpgradeToVersion_323()
                    Case "4.4.0"
                        UpgradeToVersion_440()
                    Case "4.7.0"
                        UpgradeToVersion_470()
                    Case "4.8.2"
                        UpgradeToVersion_482()
                    Case "5.0.0"
                        UpgradeToVersion_500()
                    Case "5.0.1"
                        UpgradeToVersion_501()
                    Case "5.1.0"
                        UpgradeToVersion_510()
                    Case "5.1.1"
                        UpgradeToVersion_511()
                    Case "5.1.3"
                        UpgradeToVersion_513()
                    Case "5.2.0"
                        UpgradeToVersion_520()
                    Case "5.2.1"
                        UpgradeToVersion_521()
                    Case "5.3.0"
                        UpgradeToVersion_530()
                    Case "5.4.0"
                        UpgradeToVersion_540()
                    Case "5.4.3"
                        UpgradeToVersion_543()
                    Case "5.5.0"
                        UpgradeToVersion_550()
                End Select

            Catch ex As Exception
                strExceptions += String.Format("Error: {0}{1}", ex.Message + ex.StackTrace, vbCrLf)
                ' log the results
                Try
                    Dim objStream As StreamWriter
                    objStream = File.CreateText(strProviderPath + FormatVersion(version) + "_Application.log.resources")
                    objStream.WriteLine(strExceptions)
                    objStream.Close()
                Catch
                    ' does not have permission to create the log file
                End Try
            End Try

            If writeFeedback Then
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (strExceptions = ""))
            End If

            Return strExceptions
        End Function

        Public Shared Function UpdateConfig(ByVal strProviderPath As String, ByVal version As System.Version, ByVal writeFeedback As Boolean) As String
            If writeFeedback Then
                HtmlUtils.WriteFeedback(HttpContext.Current.Response, 2, String.Format("Updating Config Files: {0}", FormatVersion(version)))
            End If
            Dim strExceptions As String = UpdateConfig(strProviderPath, DotNetNuke.Common.InstallMapPath & "Config\" & GetStringVersion(version) & ".config", version, "Core Upgrade")

            If writeFeedback Then
                HtmlUtils.WriteSuccessError(HttpContext.Current.Response, (strExceptions = ""))
            End If

            Return strExceptions
        End Function

        Public Shared Function UpdateConfig(ByVal strConfigFile As String, ByVal version As System.Version, ByVal strReason As String) As String
            Dim strExceptions As String = ""
            If File.Exists(strConfigFile) Then
                'Create XmlMerge instance from config file source
                Dim stream As StreamReader = File.OpenText(strConfigFile)
                Try
                    Dim merge As XmlMerge = New XmlMerge(stream, version.ToString(3), strReason)

                    'Process merge
                    merge.UpdateConfigs()
                Catch ex As Exception
                    strExceptions += String.Format("Error: {0}{1}", ex.Message + ex.StackTrace, vbCrLf)
                    ' log the results
                    LogException(ex)
                Finally
                    'Close stream
                    stream.Close()
                End Try
            End If
            Return strExceptions
        End Function

        Public Shared Function UpdateConfig(ByVal strProviderPath As String, ByVal strConfigFile As String, ByVal version As System.Version, ByVal strReason As String) As String
            Dim strExceptions As String = ""
            If File.Exists(strConfigFile) Then
                'Create XmlMerge instance from config file source
                Dim stream As StreamReader = File.OpenText(strConfigFile)
                Try
                    Dim merge As XmlMerge = New XmlMerge(stream, version.ToString(3), strReason)

                    'Process merge
                    merge.UpdateConfigs()
                Catch ex As Exception
                    strExceptions += String.Format("Error: {0}{1}", ex.Message + ex.StackTrace, vbCrLf)
                    ' log the results
                    Try
                        Dim objStream As StreamWriter
                        objStream = File.CreateText(strProviderPath + FormatVersion(version) + "_Config.log")
                        objStream.WriteLine(strExceptions)
                        objStream.Close()
                    Catch
                        ' does not have permission to create the log file
                    End Try
                Finally
                    'Close stream
                    stream.Close()
                End Try
            End If
            Return strExceptions
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpgradeDNN manages the Upgrade of an exisiting DotNetNuke Application
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strProviderPath">The path to the Data Provider</param>
        '''	<param name="dataBaseVersion">The current Database Version</param>
        ''' <history>
        ''' 	[cnurse]	11/06/2004	created (Upgrade code extracted from AutoUpgrade)
        '''     [cnurse]    11/10/2004  version specific upgrades extracted to ExecuteScript
        '''     [cnurse]    01/20/2005  changed to Public so Upgrade can be manually controlled
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub UpgradeDNN(ByVal strProviderPath As String, ByVal dataBaseVersion As System.Version)
            Dim version As System.Version

            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Upgrading to Version: " + FormatVersion(DotNetNukeContext.Current.Application.Version) + "<br/>")

            'Process the Upgrade Script files
            Dim versions As New List(Of Version)
            For Each strScriptFile As String In GetUpgradeScripts(strProviderPath, dataBaseVersion)
                version = New System.Version(Path.GetFileNameWithoutExtension(strScriptFile))
                versions.Add(New System.Version(Path.GetFileNameWithoutExtension(strScriptFile)))
                UpgradeVersion(strScriptFile, True)
            Next

            For Each version In versions
                '' perform version specific application upgrades
                UpgradeApplication(strProviderPath, version, True)
            Next

            For Each version In versions
                ' delete files which are no longer used
                DeleteFiles(strProviderPath, version, True)
            Next
            For Each version In versions
                'execute config file updates
                UpdateConfig(strProviderPath, version, True)
            Next

            ' perform general application upgrades
            HtmlUtils.WriteFeedback(HttpContext.Current.Response, 0, "Performing General Upgrades<br>")
            UpgradeApplication()

            DataCache.ClearHostCache(True)
        End Sub

        Public Shared Function UpgradeIndicator(ByVal Version As System.Version, ByVal IsLocal As Boolean, ByVal IsSecureConnection As Boolean) As String
            Return UpgradeIndicator(Version, DotNetNukeContext.Current.Application.Type, DotNetNukeContext.Current.Application.Name, "", IsLocal, IsSecureConnection)
        End Function

        Public Shared Function UpgradeIndicator(ByVal Version As System.Version, ByVal PackageType As String, ByVal PackageName As String, ByVal Culture As String, ByVal IsLocal As Boolean, ByVal IsSecureConnection As Boolean) As String
            Dim strURL As String = ""
            If Host.CheckUpgrade AndAlso Version <> New System.Version(0, 0, 0) AndAlso (IsLocal = False Or Config.GetSetting("ForceUpdateService") = "Y") Then
                strURL = DotNetNukeContext.Current.Application.UpgradeUrl & "/update.aspx"
                If IsSecureConnection Then
                    strURL = strURL.Replace("http://", "https://")
                End If
                strURL += "?core=" & FormatVersion(DotNetNukeContext.Current.Application.Version, "00", 3, "")
                strURL += "&version=" & FormatVersion(Version, "00", 3, "")
                strURL += "&type=" & PackageType
                strURL += "&name=" & PackageName
                If Culture <> "" Then
                    strURL += "&culture=" & Culture
                End If
                If PackageType.ToUpper = DotNetNukeContext.Current.Application.Type.ToUpper Then
                    strURL += "&os=" & FormatVersion(Common.Globals.OperatingSystemVersion, "00", 2, "")
                    strURL += "&net=" & FormatVersion(Common.Globals.NETFrameworkVersion, "00", 2, "")
                    strURL += "&db=" & FormatVersion(Common.Globals.DatabaseEngineVersion, "00", 2, "")
                End If
            End If
            Return strURL
        End Function

        Public Shared Function UpgradeRedirect() As String
            Return UpgradeRedirect(DotNetNukeContext.Current.Application.Version, DotNetNukeContext.Current.Application.Type, DotNetNukeContext.Current.Application.Name, "")
        End Function

        Public Shared Function UpgradeRedirect(ByVal Version As System.Version, ByVal PackageType As String, ByVal PackageName As String, ByVal Culture As String) As String
            Dim strURL As String = ""
            If Config.GetSetting("UpdateServiceRedirect") <> "" Then
                strURL = Config.GetSetting("UpdateServiceRedirect")
            Else
                strURL = DotNetNukeContext.Current.Application.UpgradeUrl & "/redirect.aspx"
                strURL += "?core=" & FormatVersion(DotNetNukeContext.Current.Application.Version, "00", 3, "")
                strURL += "&version=" & FormatVersion(Version, "00", 3, "")
                strURL += "&type=" & PackageType
                strURL += "&name=" & PackageName
                If Culture <> "" Then
                    strURL += "&culture=" & Culture
                End If
            End If
            Return strURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpgradeVersion upgrades a single version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="strScriptFile">The upgrade script file</param>
        ''' <history>
        ''' 	[cnurse]	02/14/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function UpgradeVersion(ByVal strScriptFile As String, ByVal writeFeedback As Boolean) As String
            Dim version As System.Version = New System.Version(Path.GetFileNameWithoutExtension(strScriptFile))
            Dim strExceptions As String = Null.NullString

            ' verify script has not already been run
            If Not Globals.FindDatabaseVersion(version.Major, version.Minor, version.Build) Then
                ' upgrade database schema
                DataProvider.Instance().UpgradeDatabaseSchema(version.Major, version.Minor, version.Build)

                ' execute script file (and version upgrades) for version
                strExceptions = ExecuteScript(strScriptFile, writeFeedback)

                ' update the version
                Globals.UpdateDataBaseVersion(version)

                Dim objEventLog As New Services.Log.EventLog.EventLogController
                Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                objEventLogInfo.AddProperty("Upgraded DotNetNuke", "Version: " + FormatVersion(version))
                If strExceptions.Length > 0 Then
                    objEventLogInfo.AddProperty("Warnings", strExceptions)
                Else
                    objEventLogInfo.AddProperty("No Warnings", "")
                End If
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                objEventLogInfo.BypassBuffering = True
                objEventLog.AddLog(objEventLogInfo)
            End If

            Return strExceptions
        End Function

#End Region

        Protected Shared Function IsLanguageEnabled(ByVal portalid As Integer, ByVal Code As String) As Boolean
            Dim enabledLanguage As Locale = Nothing
            Return LocaleController.Instance().GetLocales(portalid).TryGetValue(Code, enabledLanguage)
        End Function
    End Class

End Namespace
