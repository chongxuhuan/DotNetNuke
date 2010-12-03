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
Imports System.Data
Imports System.Web
Imports System.Collections
Imports DotNetNuke.Application

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Controllers

Namespace DotNetNuke.Entities.Portals

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' PortalSettings Class
    '''
    ''' This class encapsulates all of the settings for the Portal, as well
    ''' as the configuration settings required to execute the current tab
    ''' view within the portal.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/21/2004	documented
    ''' 	[cnurse]	10/21/2004	added GetTabModuleSettings
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PortalSettings
        Inherits BaseEntityInfo
        Implements IPropertyAccess

#Region "Enums"

        Public Enum Mode
            View
            Edit
            Layout
        End Enum

        Public Enum ControlPanelPermission
            TabEditor
            ModuleEditor
        End Enum

        Public Enum PortalAliasMapping
            None
            CanonicalUrl
            Redirect
        End Enum

#End Region

#Region "Private Members"

        Private _PortalId As Integer
        Private _PortalName As String
        Private _HomeDirectory As String
        Private _LogoFile As String
        Private _FooterText As String
        Private _ExpiryDate As Date
        Private _UserRegistration As Integer
        Private _BannerAdvertising As Integer
        Private _Currency As String
        Private _AdministratorId As Integer
        Private _Email As String
        Private _HostFee As Single
        Private _HostSpace As Integer
        Private _PageQuota As Integer
        Private _UserQuota As Integer
        Private _AdministratorRoleId As Integer
        Private _AdministratorRoleName As String
        Private _RegisteredRoleId As Integer
        Private _RegisteredRoleName As String
        Private _Description As String
        Private _KeyWords As String
        Private _BackgroundFile As String
        Private _GUID As Guid
        Private _SiteLogHistory As Integer
        Private _AdminTabId As Integer
        Private _SuperTabId As Integer
        Private _SplashTabId As Integer
        Private _HomeTabId As Integer
        Private _LoginTabId As Integer
        Private _RegisterTabId As Integer
        Private _UserTabId As Integer
        Private _SearchTabId As Integer
        Private _DefaultLanguage As String
        Private _TimeZoneOffset As Integer
        Private _Version As String
        Private _ActiveTab As TabInfo
        Private _PortalAlias As PortalAliasInfo
        Private _AdminContainer As SkinInfo
        Private _AdminSkin As SkinInfo
        Private _PortalContainer As SkinInfo
        Private _PortalSkin As SkinInfo
        Private _Users As Integer
        Private _Pages As Integer
        Private _CultureCode As String

#End Region

#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(ByVal portalID As Integer)
            Me.New(Null.NullInteger, portalID)
        End Sub

        Public Sub New(ByVal tabID As Integer, ByVal portalID As Integer)
            Dim controller As New PortalController
            Dim portal As PortalInfo = controller.GetPortal(portalID)

            GetPortalSettings(tabID, portal)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The PortalSettings Constructor encapsulates all of the logic
        ''' necessary to obtain configuration settings necessary to render
        ''' a Portal Tab view for a given request.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="tabId">The current tab</param>
        '''	<param name="objPortalAliasInfo">The current portal</param>
        ''' <history>
        ''' 	[cnurse]	10/21/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal tabID As Integer, ByVal objPortalAliasInfo As PortalAliasInfo)
            _ActiveTab = New TabInfo

            PortalId = objPortalAliasInfo.PortalID
            PortalAlias = objPortalAliasInfo

            Dim controller As New PortalController
            Dim portal As PortalInfo = controller.GetPortal(PortalId)

            If Not portal Is Nothing Then
                GetPortalSettings(tabID, portal)
            End If
        End Sub

        Public Sub New(ByVal portal As PortalInfo)
            _ActiveTab = New TabInfo

            GetPortalSettings(Null.NullInteger, portal)
        End Sub

        Public Sub New(ByVal tabID As Integer, ByVal portal As PortalInfo)
            _ActiveTab = New TabInfo

            GetPortalSettings(tabID, portal)
        End Sub

#End Region

#Region "Public Properties"

#Region "PortalInfo Properties"

        Public Property CultureCode() As String
            Get
                Return _CultureCode
            End Get
            Set(ByVal value As String)
                _CultureCode = value
            End Set
        End Property

        Public Property FooterText() As String
            Get
                Return _FooterText
            End Get
            Set(ByVal Value As String)
                _FooterText = Value
            End Set
        End Property

        Public Property HomeDirectory() As String
            Get
                Return _HomeDirectory
            End Get
            Set(ByVal Value As String)
                _HomeDirectory = Value
            End Set
        End Property

        Public ReadOnly Property HomeDirectoryMapPath() As String
            Get
                Dim objFolderController As New Services.FileSystem.FolderController
                Return objFolderController.GetMappedDirectory(HomeDirectory)
            End Get
        End Property

        Public Property LogoFile() As String
            Get
                Return _LogoFile
            End Get
            Set(ByVal Value As String)
                _LogoFile = Value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        Public Property PortalName() As String
            Get
                Return _PortalName
            End Get
            Set(ByVal Value As String)
                _PortalName = Value
            End Set
        End Property

        Public ReadOnly Property UserId() As Integer
            Get
                If HttpContext.Current.Request.IsAuthenticated Then
                    UserId = UserInfo.UserID
                Else
                    UserId = Null.NullInteger
                End If
            End Get
        End Property

        Public ReadOnly Property UserInfo() As UserInfo
            Get
                Return UserController.GetCurrentUserInfo
            End Get
        End Property

        Public Property ExpiryDate() As Date
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As Date)
                _ExpiryDate = Value
            End Set
        End Property

        Public Property UserRegistration() As Integer
            Get
                Return _UserRegistration
            End Get
            Set(ByVal Value As Integer)
                _UserRegistration = Value
            End Set
        End Property

        Public Property BannerAdvertising() As Integer
            Get
                Return _BannerAdvertising
            End Get
            Set(ByVal Value As Integer)
                _BannerAdvertising = Value
            End Set
        End Property

        Public Property Currency() As String
            Get
                Return _Currency
            End Get
            Set(ByVal Value As String)
                _Currency = Value
            End Set
        End Property

        Public Property AdministratorId() As Integer
            Get
                Return _AdministratorId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorId = Value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property

        Public Property HostFee() As Single
            Get
                Return _HostFee
            End Get
            Set(ByVal Value As Single)
                _HostFee = Value
            End Set
        End Property

        Public Property HostSpace() As Integer
            Get
                Return _HostSpace
            End Get
            Set(ByVal Value As Integer)
                _HostSpace = Value
            End Set
        End Property

        Public Property PageQuota() As Integer
            Get
                Return _PageQuota
            End Get
            Set(ByVal Value As Integer)
                _PageQuota = Value
            End Set
        End Property

        Public Property UserQuota() As Integer
            Get
                Return _UserQuota
            End Get
            Set(ByVal Value As Integer)
                _UserQuota = Value
            End Set
        End Property

        Public Property AdministratorRoleId() As Integer
            Get
                Return _AdministratorRoleId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorRoleId = Value
            End Set
        End Property

        Public Property AdministratorRoleName() As String
            Get
                Return _AdministratorRoleName
            End Get
            Set(ByVal Value As String)
                _AdministratorRoleName = Value
            End Set
        End Property

        Public Property RegisteredRoleId() As Integer
            Get
                Return _RegisteredRoleId
            End Get
            Set(ByVal Value As Integer)
                _RegisteredRoleId = Value
            End Set
        End Property

        Public Property RegisteredRoleName() As String
            Get
                Return _RegisteredRoleName
            End Get
            Set(ByVal Value As String)
                _RegisteredRoleName = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property KeyWords() As String
            Get
                Return _KeyWords
            End Get
            Set(ByVal Value As String)
                _KeyWords = Value
            End Set
        End Property

        Public Property BackgroundFile() As String
            Get
                Return _BackgroundFile
            End Get
            Set(ByVal Value As String)
                _BackgroundFile = Value
            End Set
        End Property

        Public Property GUID() As Guid
            Get
                Return _GUID
            End Get
            Set(ByVal Value As Guid)
                _GUID = Value
            End Set
        End Property

        Public Property SiteLogHistory() As Integer
            Get
                Return _SiteLogHistory
            End Get
            Set(ByVal Value As Integer)
                _SiteLogHistory = Value
            End Set
        End Property

        Public Property AdminTabId() As Integer
            Get
                Return _AdminTabId
            End Get
            Set(ByVal Value As Integer)
                _AdminTabId = Value
            End Set
        End Property

        Public Property SuperTabId() As Integer
            Get
                Return _SuperTabId
            End Get
            Set(ByVal Value As Integer)
                _SuperTabId = Value
            End Set
        End Property

        Public Property SplashTabId() As Integer
            Get
                Return _SplashTabId
            End Get
            Set(ByVal Value As Integer)
                _SplashTabId = Value
            End Set
        End Property

        Public Property HomeTabId() As Integer
            Get
                Return _HomeTabId
            End Get
            Set(ByVal Value As Integer)
                _HomeTabId = Value
            End Set
        End Property

        Public Property LoginTabId() As Integer
            Get
                Return _LoginTabId
            End Get
            Set(ByVal Value As Integer)
                _LoginTabId = Value
            End Set
        End Property

        Public Property RegisterTabId() As Integer
            Get
                Return _RegisterTabId
            End Get
            Set(ByVal Value As Integer)
                _RegisterTabId = Value
            End Set
        End Property

        Public Property UserTabId() As Integer
            Get
                Return _UserTabId
            End Get
            Set(ByVal Value As Integer)
                _UserTabId = Value
            End Set
        End Property

        Public Property SearchTabId() As Integer
            Get
                Return _SearchTabId
            End Get
            Set(ByVal Value As Integer)
                _SearchTabId = Value
            End Set
        End Property

        Public Property DefaultLanguage() As String
            Get
                Return _DefaultLanguage
            End Get
            Set(ByVal Value As String)
                _DefaultLanguage = Value
            End Set
        End Property

        Public Property TimeZoneOffset() As Integer
            Get
                Return _TimeZoneOffset
            End Get
            Set(ByVal Value As Integer)
                _TimeZoneOffset = Value
            End Set
        End Property

        Public Property Users() As Integer
            Get
                Return _Users
            End Get
            Set(ByVal Value As Integer)
                _Users = Value
            End Set
        End Property

        Public Property Pages() As Integer
            Get
                Return _Pages
            End Get
            Set(ByVal Value As Integer)
                _Pages = Value
            End Set
        End Property

#End Region

        Public Property ActiveTab() As TabInfo
            Get
                Return _ActiveTab
            End Get
            Set(ByVal Value As TabInfo)
                _ActiveTab = Value
            End Set
        End Property

        Public ReadOnly Property ContentLocalizationEnabled As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("ContentLocalizationEnabled", PortalId, False)
            End Get
        End Property

        Public ReadOnly Property ControlPanelSecurity() As ControlPanelPermission
            Get
                Dim security As ControlPanelPermission = ControlPanelPermission.ModuleEditor
                Dim setting As String = Null.NullString
                If PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelSecurity", setting) Then
                    If setting.ToUpperInvariant = "TAB" Then
                        security = ControlPanelPermission.TabEditor
                    Else
                        security = ControlPanelPermission.ModuleEditor
                    End If
                End If
                Return security
            End Get
        End Property

        Public ReadOnly Property ControlPanelVisible() As Boolean
            Get
                Dim isVisible As Boolean = True
                Dim setting As String = Convert.ToString(Personalization.Personalization.GetProfile("Usability", "ControlPanelVisible" & PortalId.ToString))
                If setting = "" Then
                    isVisible = DefaultControlPanelVisibility()
                Else
                    isVisible = Convert.ToBoolean(setting)
                End If
                Return isVisible
            End Get
        End Property

        Public ReadOnly Property DefaultAdminContainer() As String
            Get
                Return PortalController.GetPortalSetting("DefaultAdminContainer", PortalId, Host.Host.DefaultAdminContainer)
            End Get
        End Property

        Public ReadOnly Property DefaultAdminSkin() As String
            Get
                Return PortalController.GetPortalSetting("DefaultAdminSkin", PortalId, Host.Host.DefaultAdminSkin)
            End Get
        End Property

        Public ReadOnly Property DefaultControlPanelMode() As Mode
            Get
                Dim mode As Mode = mode.Edit
                Dim setting As String = Null.NullString

                If PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelMode", setting) Then
                    If setting.ToUpperInvariant() = "VIEW" Then
                        mode = PortalSettings.Mode.View
                    End If
                End If
                Return mode
            End Get
        End Property

        Public ReadOnly Property DefaultControlPanelVisibility() As Boolean
            Get
                Dim isVisible As Boolean = True
                Dim setting As String = ""
                If PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("ControlPanelVisibility", setting) Then
                    isVisible = Not (setting.ToUpperInvariant() = "MIN")
                End If
                Return isVisible
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Module Id
        ''' </summary>
        ''' <remarks>Defaults to Null.NullInteger</remarks>
        ''' <history>
        ''' 	[cnurse]	05/02/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultModuleId() As Integer
            Get
                Return PortalController.GetPortalSettingAsInteger("defaultmoduleid", PortalId, Null.NullInteger)
            End Get
        End Property

        Public ReadOnly Property DefaultPortalAlias() As String
            Get
                Return PortalController.GetPortalSetting("DefaultPortalAlias", PortalId, "")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Tab Id
        ''' </summary>
        ''' <remarks>Defaults to Null.NullInteger</remarks>
        ''' <history>
        ''' 	[cnurse]	05/02/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultTabId() As Integer
            Get
                Return PortalController.GetPortalSettingAsInteger("defaulttabid", PortalId, Null.NullInteger)
            End Get
        End Property

        Public ReadOnly Property DefaultPortalContainer() As String
            Get
                Return PortalController.GetPortalSetting("DefaultPortalContainer", PortalId, Host.Host.DefaultPortalContainer)
            End Get
        End Property

        Public ReadOnly Property DefaultPortalSkin() As String
            Get
                Return PortalController.GetPortalSetting("DefaultPortalSkin", PortalId, Host.Host.DefaultPortalSkin)
            End Get
        End Property

        Public ReadOnly Property PortalAliasMappingMode() As PortalAliasMapping
            Get
                Dim aliasMapping As PortalAliasMapping = PortalAliasMapping.None
                Dim setting As String = Null.NullString
                If PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue("PortalAliasMapping", setting) Then
                    Select Case setting.ToUpperInvariant
                        Case "CANONICALURL"
                            aliasMapping = PortalAliasMapping.CanonicalUrl
                        Case "REDIRECT"
                            aliasMapping = PortalAliasMapping.Redirect
                        Case Else
                            aliasMapping = PortalAliasMapping.None
                    End Select
                End If
                Return aliasMapping
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Browser Language Detection is Enabled
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property EnableBrowserLanguage() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("EnableBrowserLanguage", PortalId, Host.Host.EnableBrowserLanguage)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to use the Language in the Url
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property EnableUrlLanguage() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("EnableUrlLanguage", PortalId, Host.Host.EnableUrlLanguage)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Skin Widgets are enabled/supported
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	07/03/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property EnableSkinWidgets() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("EnableSkinWidgets", PortalId, True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Inline Editor is enabled
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	08/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property InlineEditorEnabled() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("InlineEditorEnabled", PortalId, True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether folders which are hidden or whose name begins with underscore
        ''' are included in folder synchronization.
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	08/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property HideFoldersEnabled() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("HideFoldersEnabled", PortalId, True)
            End Get
        End Property

        Public Property PortalAlias() As PortalAliasInfo
            Get
                Return _PortalAlias
            End Get
            Set(ByVal Value As PortalAliasInfo)
                _PortalAlias = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Common Words in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchIncludeCommon() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("SearchIncludeCommon", PortalId, Host.Host.SearchIncludeNumeric)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Numbers in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchIncludeNumeric() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("SearchIncludeNumeric", PortalId, Host.Host.SearchIncludeNumeric)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 25</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchMaxWordlLength() As Integer
            Get
                Return PortalController.GetPortalSettingAsInteger("MaxSearchWordLength", PortalId, Host.Host.SearchMaxWordlLength)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 3</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchMinWordlLength() As Integer
            Get
                Return PortalController.GetPortalSettingAsInteger("MinSearchWordLength", PortalId, Host.Host.SearchMinWordlLength)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the filter used for inclusion of tag info
        ''' </summary>
        ''' <remarks>Defaults to ""</remarks>
        ''' <history>
        '''    [vnguyen]   09/03/2010   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SearchIncludedTagInfoFilter() As String
            Get
                Return PortalController.GetPortalSetting("SearchIncludedTagInfoFilter", PortalId, Host.Host.SearchIncludedTagInfoFilter)
            End Get
        End Property

        Public ReadOnly Property SSLEnabled() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("SSLEnabled", PortalId, False)
            End Get
        End Property

        Public ReadOnly Property SSLEnforced() As Boolean
            Get
                Return PortalController.GetPortalSettingAsBoolean("SSLEnforced", PortalId, False)
            End Get
        End Property

        Public ReadOnly Property SSLURL() As String
            Get
                Return PortalController.GetPortalSetting("SSLURL", PortalId, Null.NullString)
            End Get
        End Property

        Public ReadOnly Property STDURL() As String
            Get
                Return PortalController.GetPortalSetting("STDURL", PortalId, Null.NullString)
            End Get
        End Property

        Public ReadOnly Property UserMode() As Mode
            Get
                Dim mode As Mode
                If HttpContext.Current.Request.IsAuthenticated Then
                    mode = DefaultControlPanelMode
                    Dim setting As String = Convert.ToString(Personalization.Personalization.GetProfile("Usability", "UserMode" & PortalId.ToString))
                    Select Case setting.ToUpper()
                        Case "VIEW"
                            mode = PortalSettings.Mode.View
                        Case "EDIT"
                            mode = PortalSettings.Mode.Edit
                        Case "LAYOUT"
                            mode = PortalSettings.Mode.Layout
                    End Select
                Else
                    mode = PortalSettings.Mode.View
                End If
                Return mode
            End Get
        End Property

#End Region

#Region "Public Shared Properties"

        Public Shared ReadOnly Property Current() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings()
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub GetBreadCrumbsRecursively(ByRef objBreadCrumbs As ArrayList, ByVal intTabId As Integer)
            ' find the tab in the tabs collection
            Dim objTab As TabInfo = Nothing
            Dim objTabController As New TabController()
            Dim portalTabs As TabCollection = objTabController.GetTabsByPortal(PortalId)
            Dim hostTabs As TabCollection = objTabController.GetTabsByPortal(Null.NullInteger)

            Dim blnFound As Boolean = portalTabs.TryGetValue(intTabId, objTab)
            If Not blnFound Then
                blnFound = hostTabs.TryGetValue(intTabId, objTab)
            End If

            ' if tab was found
            If blnFound Then
                ' add tab to breadcrumb collection
                objBreadCrumbs.Insert(0, objTab.Clone)

                ' get the tab parent
                If Not Null.IsNull(objTab.ParentId) Then
                    GetBreadCrumbsRecursively(objBreadCrumbs, objTab.ParentId)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The GetPortalSettings method builds the site Settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="tabID">The current tabs id</param>
        '''	<param name="portal">The Portal object</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetPortalSettings(ByVal tabID As Integer, ByVal portal As PortalInfo)
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            Me.PortalId = portal.PortalID
            Me.PortalName = portal.PortalName
            Me.LogoFile = portal.LogoFile
            Me.FooterText = portal.FooterText
            Me.ExpiryDate = portal.ExpiryDate
            Me.UserRegistration = portal.UserRegistration
            Me.BannerAdvertising = portal.BannerAdvertising
            Me.Currency = portal.Currency
            Me.AdministratorId = portal.AdministratorId
            Me.Email = portal.Email
            Me.HostFee = portal.HostFee
            Me.HostSpace = portal.HostSpace
            Me.PageQuota = portal.PageQuota
            Me.UserQuota = portal.UserQuota
            Me.AdministratorRoleId = portal.AdministratorRoleId
            Me.AdministratorRoleName = portal.AdministratorRoleName
            Me.RegisteredRoleId = portal.RegisteredRoleId
            Me.RegisteredRoleName = portal.RegisteredRoleName
            Me.Description = portal.Description
            Me.KeyWords = portal.KeyWords
            Me.BackgroundFile = portal.BackgroundFile
            Me.GUID = portal.GUID
            Me.SiteLogHistory = portal.SiteLogHistory
            Me.AdminTabId = portal.AdminTabId
            Me.SuperTabId = portal.SuperTabId
            Me.SplashTabId = portal.SplashTabId
            Me.HomeTabId = portal.HomeTabId
            Me.LoginTabId = portal.LoginTabId
            Me.RegisterTabId = portal.RegisterTabId
            Me.UserTabId = portal.UserTabId
            Me.SearchTabId = portal.SearchTabId
            Me.DefaultLanguage = portal.DefaultLanguage
            Me.TimeZoneOffset = portal.TimeZoneOffset
            Me.HomeDirectory = portal.HomeDirectory
            Me.Pages = portal.Pages
            Me.Users = portal.Users
            Me.CultureCode = portal.CultureCode

            ' update properties with default values
            If Null.IsNull(Me.HostSpace) Then
                Me.HostSpace = 0
            End If
            If Null.IsNull(Me.DefaultLanguage) Then
                Me.DefaultLanguage = Localization.SystemLocale
            End If
            If Null.IsNull(Me.TimeZoneOffset) Then
                Me.TimeZoneOffset = Localization.SystemTimeZoneOffset
            End If
            Me.HomeDirectory = Common.Globals.ApplicationPath + "/" + portal.HomeDirectory + "/"

            'verify tab for portal. This assigns the Active Tab based on the Tab Id/PortalId
            If VerifyPortalTab(PortalId, tabID) Then
                If Not Me.ActiveTab Is Nothing Then
                    ' skin
                    If IsAdminSkin() Then
                        Me.ActiveTab.SkinSrc = Me.DefaultAdminSkin
                    Else
                        If Me.ActiveTab.SkinSrc = "" Then
                            Me.ActiveTab.SkinSrc = Me.DefaultPortalSkin
                        End If
                    End If
                    Me.ActiveTab.SkinSrc = SkinController.FormatSkinSrc(Me.ActiveTab.SkinSrc, Me)
                    Me.ActiveTab.SkinPath = SkinController.FormatSkinPath(Me.ActiveTab.SkinSrc)

                    ' container
                    If IsAdminSkin() Then
                        Me.ActiveTab.ContainerSrc = Me.DefaultAdminContainer
                    Else
                        If Me.ActiveTab.ContainerSrc = "" Then
                            Me.ActiveTab.ContainerSrc = Me.DefaultPortalContainer
                        End If
                    End If
                    Me.ActiveTab.ContainerSrc = SkinController.FormatSkinSrc(Me.ActiveTab.ContainerSrc, Me)
                    Me.ActiveTab.ContainerPath = SkinController.FormatSkinPath(Me.ActiveTab.ContainerSrc)

                    ' initialize collections
                    Me.ActiveTab.BreadCrumbs = New ArrayList
                    Me.ActiveTab.Panes = New ArrayList
                    Me.ActiveTab.Modules = New ArrayList

                    ' get breadcrumbs for current tab
                    GetBreadCrumbsRecursively(Me.ActiveTab.BreadCrumbs, Me.ActiveTab.TabID)
                End If
            End If

            ' get current tab modules
            If Me.ActiveTab IsNot Nothing Then
                Dim objPaneModules As New Dictionary(Of String, Integer)

                For Each kvp As KeyValuePair(Of Integer, ModuleInfo) In Me.ActiveTab.ChildModules
                    ' clone the module object ( to avoid creating an object reference to the data cache )
                    Dim cloneModule As ModuleInfo = kvp.Value.Clone

                    ' set custom properties
                    If Null.IsNull(cloneModule.StartDate) Then
                        cloneModule.StartDate = Date.MinValue
                    End If
                    If Null.IsNull(cloneModule.EndDate) Then
                        cloneModule.EndDate = Date.MaxValue
                    End If

                    ' container
                    If cloneModule.ContainerSrc = "" Then
                        cloneModule.ContainerSrc = Me.ActiveTab.ContainerSrc
                    End If
                    cloneModule.ContainerSrc = SkinController.FormatSkinSrc(cloneModule.ContainerSrc, Me)
                    cloneModule.ContainerPath = SkinController.FormatSkinPath(cloneModule.ContainerSrc)

                    ' process tab panes
                    If objPaneModules.ContainsKey(cloneModule.PaneName) = False Then
                        objPaneModules.Add(cloneModule.PaneName, 0)
                    End If
                    cloneModule.PaneModuleCount = 0
                    If Not cloneModule.IsDeleted Then
                        objPaneModules(cloneModule.PaneName) = objPaneModules(cloneModule.PaneName) + 1
                        cloneModule.PaneModuleIndex = objPaneModules(cloneModule.PaneName) - 1
                    End If

                    Me.ActiveTab.Modules.Add(cloneModule)
                Next

                ' set pane module count
                For Each objModule In Me.ActiveTab.Modules
                    objModule.PaneModuleCount = objPaneModules(objModule.PaneName)
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The VerifyPortalTab method verifies that the TabId/PortalId combination
        ''' is allowed and returns default/home tab ids if not
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="PortalId">The Portal's id</param>
        '''	<param name="TabId">The current tab's id</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function VerifyPortalTab(ByVal PortalId As Integer, ByVal TabId As Integer) As Boolean
            Dim objTab As TabInfo = Nothing
            Dim objSplashTab As TabInfo = Nothing
            Dim objHomeTab As TabInfo = Nothing
            Dim isVerified As Boolean = False

            Dim objTabController As New TabController()
            Dim portalTabs As TabCollection = objTabController.GetTabsByPortal(PortalId)
            Dim hostTabs As TabCollection = objTabController.GetTabsByPortal(Null.NullInteger)

            ' find the tab in the portalTabs collection
            If TabId <> Null.NullInteger Then
                If portalTabs.TryGetValue(TabId, objTab) Then
                    'Check if Tab has been deleted (is in recycle bin)
                    If Not (objTab.IsDeleted) Then
                        Me.ActiveTab = objTab.Clone()
                        isVerified = True
                    End If
                End If
            End If

            ' find the tab in the hostTabs collection
            If Not isVerified AndAlso TabId <> Null.NullInteger Then
                If hostTabs.TryGetValue(TabId, objTab) Then
                    'Check if Tab has been deleted (is in recycle bin)
                    If Not (objTab.IsDeleted) Then
                        Me.ActiveTab = objTab.Clone()
                        isVerified = True
                    End If
                End If
            End If

            ' if tab was not found 
            If Not isVerified AndAlso Me.SplashTabId > 0 Then
                ' use the splash tab ( if specified )
                objSplashTab = objTabController.GetTab(Me.SplashTabId, PortalId, False)
                Me.ActiveTab = objSplashTab.Clone()
                isVerified = True
            End If

            ' if tab was not found 
            If Not isVerified AndAlso Me.HomeTabId > 0 Then
                ' use the home tab ( if specified )
                objHomeTab = objTabController.GetTab(Me.HomeTabId, PortalId, False)
                Me.ActiveTab = objHomeTab.Clone()
                isVerified = True
            End If

            ' if tab was not found 
            If Not isVerified Then
                ' get the first tab in the collection (that is valid)
                For Each objTab In portalTabs.AsList()
                    'Check if Tab has not been deleted (not in recycle bin) and is visible
                    If Not (objTab.IsDeleted) And objTab.IsVisible Then
                        Me.ActiveTab = objTab.Clone()
                        isVerified = True
                        Exit For
                    End If
                Next
            End If

            If Null.IsNull(Me.ActiveTab.StartDate) Then
                Me.ActiveTab.StartDate = Date.MinValue
            End If
            If Null.IsNull(Me.ActiveTab.EndDate) Then
                Me.ActiveTab.EndDate = Date.MaxValue
            End If

            Return isVerified

        End Function

#End Region

#Region "IPropertyAccess Implementation"

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            Dim OutputFormat As String = String.Empty
            If strFormat = String.Empty Then OutputFormat = "g"
            Dim lowerPropertyName As String = strPropertyName.ToLower

            'Content locked for NoSettings
            If AccessLevel = Scope.NoSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked

            PropertyNotFound = True
            Dim result As String = String.Empty
            Dim PublicProperty As Boolean = True

            Select Case lowerPropertyName
                Case "url"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.PortalAlias.HTTPAlias(), strFormat)
                Case "portalid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.PortalId.ToString(OutputFormat, formatProvider))
                Case "portalname"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.PortalName, strFormat)
                Case "homedirectory"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.HomeDirectory, strFormat)
                Case "homedirectorymappath"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.HomeDirectoryMapPath, strFormat)
                Case "logofile"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.LogoFile, strFormat)
                Case "footertext"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.FooterText, strFormat)
                Case "expirydate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ExpiryDate.ToString(OutputFormat, formatProvider))
                Case "userregistration"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.UserRegistration.ToString(OutputFormat, formatProvider))
                Case "banneradvertising"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.BannerAdvertising.ToString(OutputFormat, formatProvider))
                Case "currency"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Currency, strFormat)
                Case "administratorid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.AdministratorId.ToString(OutputFormat, formatProvider))
                Case "email"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Email, strFormat)
                Case "hostfee"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.HostFee.ToString(OutputFormat, formatProvider))
                Case "hostspace"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.HostSpace.ToString(OutputFormat, formatProvider))
                Case "pagequota"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.PageQuota.ToString(OutputFormat, formatProvider))
                Case "userquota"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.UserQuota.ToString(OutputFormat, formatProvider))
                Case "administratorroleid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.AdministratorRoleId.ToString(OutputFormat, formatProvider))
                Case "administratorrolename"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.AdministratorRoleName, strFormat)
                Case "registeredroleid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.RegisteredRoleId.ToString(OutputFormat, formatProvider))
                Case "registeredrolename"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.RegisteredRoleName, strFormat)
                Case "description"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Description, strFormat)
                Case "keywords"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.KeyWords, strFormat)
                Case "backgroundfile"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.BackgroundFile, strFormat)
                Case "siteloghistory"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.SiteLogHistory.ToString(OutputFormat, formatProvider))
                Case "admintabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.AdminTabId.ToString(OutputFormat, formatProvider))
                Case "supertabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.SuperTabId.ToString(OutputFormat, formatProvider))
                Case "splashtabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.SplashTabId.ToString(OutputFormat, formatProvider))
                Case "hometabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.HomeTabId.ToString(OutputFormat, formatProvider))
                Case "logintabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.LoginTabId.ToString(OutputFormat, formatProvider))
                Case "registertabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.RegisterTabId.ToString(OutputFormat, formatProvider))
                Case "usertabid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.UserTabId.ToString(OutputFormat, formatProvider))
                Case "defaultlanguage"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DefaultLanguage, strFormat)
                Case "timezoneoffset"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.TimeZoneOffset.ToString(OutputFormat, formatProvider))
                Case "users"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.Users.ToString(OutputFormat, formatProvider))
                Case "pages"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.Pages.ToString(OutputFormat, formatProvider))
                Case "contentvisible"
                    'Property deprecated
                    PublicProperty = False : PropertyNotFound = True
                Case "controlpanelvisible"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.ControlPanelVisible, formatProvider))
            End Select

            If Not PublicProperty And AccessLevel <> Scope.Debug Then
                PropertyNotFound = True
                result = PropertyAccess.ContentLocked
            End If

            Return result
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.fullyCacheable
            End Get
        End Property

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.0. Replaced by DefaultAdminContainer")> _
        Public Property AdminContainer() As SkinInfo
            Get
                Return _AdminContainer
            End Get
            Set(ByVal value As SkinInfo)
                _AdminContainer = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DefaultAdminSkin")> _
        Public Property AdminSkin() As SkinInfo
            Get
                Return _AdminSkin
            End Get
            Set(ByVal value As SkinInfo)
                _AdminSkin = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by Host.GetHostSettingsDictionary")> _
        Public ReadOnly Property HostSettings() As Hashtable
            Get
                Dim h As New Hashtable
                For Each kvp As ConfigurationSetting In HostController.Instance.GetSettings().Values
                    h.Add(kvp.Key, kvp.Value)
                Next
                Return h
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by extended UserMode property.")> _
        Public ReadOnly Property ContentVisible() As Boolean
            Get
                Return UserMode <> Mode.Layout
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.ExecuteScript")> _
        Public Shared Function ExecuteScript(ByVal strScript As String) As String
            Return DataProvider.Instance().ExecuteScript(strScript)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.ExecuteScript")> _
        Public Shared Function ExecuteScript(ByVal strScript As String, ByVal UseTransactions As Boolean) As String
            Return DataProvider.Instance().ExecuteScript(strScript, UseTransactions)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by Globals.FindDatabaseVersion")> _
        Public Shared Function FindDatabaseVersion(ByVal Major As Integer, ByVal Minor As Integer, ByVal Build As Integer) As Boolean
            Return Globals.FindDatabaseVersion(Major, Minor, Build)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.GetDatabaseVersion")> _
        Public Shared Function GetDatabaseVersion() As IDataReader
            Return DataProvider.Instance().GetDataBaseVersion
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by Host.GetHostSettingsDictionary")> _
        Public Shared Function GetHostSettings() As Hashtable
            Dim h As New Hashtable
            For Each kvp As KeyValuePair(Of String, String) In Host.Host.GetHostSettingsDictionary()
                h.Add(kvp.Key, kvp.Value)
            Next
            Return h
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use ModuleController.GetModuleSettings(ModuleId)")> _
        Public Shared Function GetModuleSettings(ByVal ModuleId As Integer) As Hashtable
            Return New ModuleController().GetModuleSettings(ModuleId)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalAliasController.GetPortalAliasInfo")> _
        Public Shared Function GetPortalAliasInfo(ByVal PortalAlias As String) As PortalAliasInfo
            Return PortalAliasController.GetPortalAliasInfo(PortalAlias)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalAliasController.GetPortalAliasByPortal")> _
        Public Shared Function GetPortalByID(ByVal portalId As Integer, ByVal portalAlias As String) As String
            Return PortalAliasController.GetPortalAliasByPortal(portalId, portalAlias)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalAliasController.GetPortalAliasByTab")> _
        Public Shared Function GetPortalByTab(ByVal tabID As Integer, ByVal portalAlias As String) As String
            Return PortalAliasController.GetPortalAliasByTab(tabID, portalAlias)
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalAliasController.GetPortalAliasLookup")> _
        Public Shared Function GetPortalAliasLookup() As PortalAliasCollection
            Return PortalAliasController.GetPortalAliasLookup()
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.GetProviderPath")> _
        Public Shared Function GetProviderPath() As String
            Return DataProvider.Instance().GetProviderPath()
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalController.GetPortalSettingsDictionary")> _
        Public Shared Function GetSiteSettings(ByVal PortalId As Integer) As Hashtable
            Dim h As New Hashtable
            For Each kvp As KeyValuePair(Of String, String) In PortalController.GetPortalSettingsDictionary(PortalId)
                h.Add(kvp.Key, kvp.Value)
            Next
            Return h
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalController.GetPortalSettingsDictionary(portalId).TryGetValue(settingName) or for the most part by proeprties of PortalSettings")> _
        Public Shared Function GetSiteSetting(ByVal PortalId As Integer, ByVal SettingName As String) As String
            Dim setting As String = Null.NullString
            PortalController.GetPortalSettingsDictionary(PortalId).TryGetValue(SettingName, setting)
            Return setting
        End Function

        <Obsolete("Deprecated in DNN 5.0. Replaced by DefaultPortalContainer")> _
        Public Property PortalContainer() As SkinInfo
            Get
                Return _PortalContainer
            End Get
            Set(ByVal value As SkinInfo)
                _PortalContainer = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DefaultPortalSkin")> _
        Public Property PortalSkin() As SkinInfo
            Get
                Return _PortalSkin
            End Get
            Set(ByVal value As SkinInfo)
                _PortalSkin = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0.  Please use ModuleController.GetTabModuleSettings(TabModuleId)")> _
        Public Shared Function GetTabModuleSettings(ByVal TabModuleId As Integer) As Hashtable
            Return New ModuleController().GetTabModuleSettings(TabModuleId)
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use ModuleController.GetTabModuleSettings(ModuleId)")> _
        Public Shared Function GetTabModuleSettings(ByVal TabModuleId As Integer, ByVal moduleSettings As Hashtable) As Hashtable
            Dim tabModuleSettings As Hashtable = New ModuleController().GetTabModuleSettings(TabModuleId)

            ' add the TabModuleSettings to the ModuleSettings
            For Each strKey As String In tabModuleSettings.Keys
                moduleSettings(strKey) = tabModuleSettings(strKey)
            Next

            Return moduleSettings
        End Function

        <Obsolete("Deprecated in DNN 5.0.  Please use ModuleController.GetTabModuleSettings(ModuleId)")> _
        Public Shared Function GetTabModuleSettings(ByVal moduleSettings As Hashtable, ByVal tabModuleSettings As Hashtable) As Hashtable
            ' add the TabModuleSettings to the ModuleSettings
            For Each strKey As String In tabModuleSettings.Keys
                moduleSettings(strKey) = tabModuleSettings(strKey)
            Next

            'Return the modifed ModuleSettings
            Return moduleSettings
        End Function

        Private _DesktopTabs As ArrayList

        <Obsolete("Deprecated in DNN 5.0. Tabs are cached independeently of Portal Settings, and this property is thus redundant")> _
        Public ReadOnly Property DesktopTabs() As ArrayList
            Get
                If _DesktopTabs Is Nothing Then
                    _DesktopTabs = New ArrayList

                    'Add each portal Tab to DesktopTabs
                    Dim objPortalTab As TabInfo
                    For Each objTab As TabInfo In TabController.GetTabsBySortOrder(Me.PortalId, Me.CultureCode, True)
                        ' clone the tab object ( to avoid creating an object reference to the data cache )
                        objPortalTab = objTab.Clone()

                        ' set custom properties
                        If objPortalTab.TabOrder = 0 Then
                            objPortalTab.TabOrder = 999
                        End If
                        If Null.IsNull(objPortalTab.StartDate) Then
                            objPortalTab.StartDate = Date.MinValue
                        End If
                        If Null.IsNull(objPortalTab.EndDate) Then
                            objPortalTab.EndDate = Date.MaxValue
                        End If

                        _DesktopTabs.Add(objPortalTab)
                    Next

                    'Add each host Tab to DesktopTabs
                    Dim objHostTab As TabInfo
                    For Each objTab As TabInfo In TabController.GetTabsBySortOrder(Null.NullInteger, Null.NullString, True)
                        ' clone the tab object ( to avoid creating an object reference to the data cache )
                        objHostTab = objTab.Clone()
                        objHostTab.PortalID = Me.PortalId
                        objHostTab.StartDate = Date.MinValue
                        objHostTab.EndDate = Date.MaxValue

                        _DesktopTabs.Add(objHostTab)
                    Next
                End If

                Return _DesktopTabs
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.UpgradeDatabaseSchema")> _
        Public Shared Sub UpgradeDatabaseSchema(ByVal Major As Integer, ByVal Minor As Integer, ByVal Build As Integer)
            DataProvider.Instance().UpgradeDatabaseSchema(Major, Minor, Build)
        End Sub

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.UpdateDatabaseVersion")> _
        Public Shared Sub UpdateDatabaseVersion(ByVal Major As Integer, ByVal Minor As Integer, ByVal Build As Integer)
            DataProvider.Instance().UpdateDatabaseVersion(Major, Minor, Build, DotNetNukeContext.Current.Application.Name)
        End Sub

        <Obsolete("Deprecated in DNN 5.0. Replaced by DataProvider.UpdatePortalSetting(Integer, String, String)")> _
        Public Shared Sub UpdatePortalSetting(ByVal PortalId As Integer, ByVal SettingName As String, ByVal SettingValue As String)
            PortalController.UpdatePortalSetting(PortalId, SettingName, SettingValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.0. Replaced by PortalController.UpdatePortalSetting(Integer, String, String)")> _
        Public Shared Sub UpdateSiteSetting(ByVal PortalId As Integer, ByVal SettingName As String, ByVal SettingValue As String)
            PortalController.UpdatePortalSetting(PortalId, SettingName, SettingValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.1. Replaced by Application.Version")> _
        Public Property Version() As String
            Get
                If String.IsNullOrEmpty(_Version) Then
                    _Version = DotNetNukeContext.Current.Application.Version.ToString(3)
                End If
                Return _Version
            End Get
            Set(ByVal Value As String)
                _Version = Value
            End Set
        End Property


#End Region

    End Class

End Namespace
