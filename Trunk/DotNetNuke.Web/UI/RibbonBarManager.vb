'
' DotNetNuke - http://www.dotnetnuke.com
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
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text.RegularExpressions

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Web.UI

	Public Class RibbonBarManager

		Public Shared Function InitTabInfoObject() As DotNetNuke.Entities.Tabs.TabInfo
			Return InitTabInfoObject(Nothing, TabRelativeLocation.AFTER)
		End Function

		Public Shared Function InitTabInfoObject(ByVal relativeToTab As DotNetNuke.Entities.Tabs.TabInfo) As DotNetNuke.Entities.Tabs.TabInfo
			Return InitTabInfoObject(relativeToTab, TabRelativeLocation.AFTER)
		End Function

		Public Shared Function InitTabInfoObject(ByVal relativeToTab As DotNetNuke.Entities.Tabs.TabInfo, ByVal location As TabRelativeLocation) As DotNetNuke.Entities.Tabs.TabInfo
			Dim tabCtrl As New TabController()
			If (IsNothing(relativeToTab)) Then
				If (Not IsNothing(PortalSettings.Current) AndAlso Not IsNothing(PortalSettings.Current.ActiveTab)) Then
					relativeToTab = PortalSettings.Current.ActiveTab
				End If
			End If

			Dim newTab As New DotNetNuke.Entities.Tabs.TabInfo()
			newTab.TabID = Null.NullInteger
			newTab.TabName = ""
			newTab.Title = ""
			newTab.IsVisible = False
			newTab.DisableLink = False
			newTab.IsDeleted = False
			newTab.IsSecure = False
			newTab.PermanentRedirect = False

			Dim parentTab As DotNetNuke.Entities.Tabs.TabInfo = GetParentTab(relativeToTab, location)

			If (Not IsNothing(parentTab)) Then
				newTab.PortalID = parentTab.PortalID
				newTab.ParentId = parentTab.TabID
				newTab.Level = parentTab.Level + 1
				If (PortalSettings.Current.SSLEnabled) Then
					newTab.IsSecure = parentTab.IsSecure 'Inherit from parent
				End If
			Else
				newTab.PortalID = PortalSettings.Current.PortalId
				newTab.ParentId = Null.NullInteger
				newTab.Level = 0
			End If

			'Inherit permissions from parent
			newTab.TabPermissions.Clear()
			If (newTab.PortalID <> Null.NullInteger AndAlso Not IsNothing(parentTab)) Then
				newTab.TabPermissions.AddRange(parentTab.TabPermissions)
			ElseIf (newTab.PortalID <> Null.NullInteger) Then
				'Give admin full permission
				Dim permissions As ArrayList = DotNetNuke.Security.Permissions.PermissionController.GetPermissionsByTab()

				For Each permission As DotNetNuke.Security.Permissions.PermissionInfo In permissions
					Dim newTabPermission As New DotNetNuke.Security.Permissions.TabPermissionInfo()
					newTabPermission.PermissionID = permission.PermissionID
					newTabPermission.PermissionKey = permission.PermissionKey
					newTabPermission.PermissionName = permission.PermissionName
					newTabPermission.AllowAccess = True
					newTabPermission.RoleID = PortalSettings.Current.AdministratorRoleId
					newTab.TabPermissions.Add(newTabPermission)
				Next
			End If

			Return newTab
		End Function

		Public Shared Function GetParentTab(ByVal relativeToTab As DotNetNuke.Entities.Tabs.TabInfo, ByVal location As TabRelativeLocation) As TabInfo
			If (IsNothing(relativeToTab)) Then
				Return Nothing
			End If

			Dim tabCtrl As New TabController()
			Dim parentTab As DotNetNuke.Entities.Tabs.TabInfo = Nothing
			If (location = TabRelativeLocation.CHILD) Then
				parentTab = relativeToTab
			ElseIf (Not IsNothing(relativeToTab) AndAlso relativeToTab.ParentId <> Null.NullInteger) Then
				parentTab = tabCtrl.GetTab(relativeToTab.ParentId, relativeToTab.PortalID, False)
			End If

			Return parentTab
		End Function

		Public Shared Function GetPagesList() As IList(Of DotNetNuke.Entities.Tabs.TabInfo)
			Dim portalTabs As IList(Of DotNetNuke.Entities.Tabs.TabInfo) = Nothing
			Dim userInfo As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()
			If (Not IsNothing(userInfo) AndAlso userInfo.UserID <> Null.NullInteger) Then
				Dim tabCtrl As New TabController()
				If (userInfo.IsSuperUser AndAlso PortalSettings.Current.ActiveTab.IsSuperTab) Then
					portalTabs = tabCtrl.GetTabsByPortal(Null.NullInteger).AsList()
				Else
					portalTabs = TabController.GetPortalTabs(PortalSettings.Current.PortalId, Null.NullInteger, False, Null.NullString, True, False, True, False, True)
				End If
			End If

			If (IsNothing(portalTabs)) Then
				portalTabs = New List(Of DotNetNuke.Entities.Tabs.TabInfo)
			End If

			Return portalTabs
		End Function

		Public Shared Function IsHostConsolePage() As Boolean
			Return (PortalSettings.Current.ActiveTab.IsSuperTab AndAlso PortalSettings.Current.ActiveTab.TabPath = "//Host")
		End Function

		Public Shared Function IsHostConsolePage(ByVal tab As TabInfo) As Boolean
			Return (tab.IsSuperTab AndAlso tab.TabPath = "//Host")
		End Function

		Public Shared Function CanMovePage() As Boolean
			'Cannot move the host console page
			If (IsHostConsolePage()) Then
				Return False
			End If

			'Page Editors - Can only move children they have 'Manage' permission to, they cannot move the top level page
			If (Not DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
				Dim parentTabID As Integer = PortalSettings.Current.ActiveTab.ParentId
				If (parentTabID = Null.NullInteger) Then
					Return False
				End If

				Dim parentTab As TabInfo = New TabController().GetTab(parentTabID, PortalSettings.Current.ActiveTab.PortalID, False)
				Dim permissionList As String = "MANAGE"
				If (Not DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission(parentTab.TabPermissions, permissionList)) Then
					Return False
				End If
			End If

			Return True
		End Function

		'todo: Settings
        Public Shared Function SaveTabInfoObject(ByVal tab As DotNetNuke.Entities.Tabs.TabInfo, ByVal relativeToTab As DotNetNuke.Entities.Tabs.TabInfo, ByVal location As TabRelativeLocation, ByVal templateMapPath As String) As Integer
            Dim tabCtrl As New TabController()

            'Validation:
            'Tab name is required
            'Tab name is invalid
            If (tab.TabName = String.Empty) Then
                Throw New DotNetNukeException("Page name is required.", DotNetNukeErrorCode.PageNameRequired)
            ElseIf (Regex.IsMatch(tab.TabName, "^AUX$|^CON$|^LPT[1-9]$|^CON$|^COM[1-9]$|^NUL$|^SITEMAP$|^LINKCLICK$|^KEEPALIVE$|^DEFAULT$|^ERRORPAGE$", RegexOptions.IgnoreCase)) Then
                Throw New DotNetNukeException("Page name is invalid.", DotNetNukeErrorCode.PageNameInvalid)
            ElseIf (Validate_IsCircularReference(tab.PortalID, tab.TabID)) Then
                Throw New DotNetNukeException("Cannot move page to that location.", DotNetNukeErrorCode.PageCircularReference)
            End If


            Dim usingDefaultLanguage As Boolean = (tab.CultureCode = PortalSettings.Current.DefaultLanguage)
            If PortalSettings.Current.ContentLocalizationEnabled Then

                If (Not usingDefaultLanguage) Then
                    Dim defaultLanguageSelectedTab As DotNetNuke.Entities.Tabs.TabInfo = tab.DefaultLanguageTab

                    If (defaultLanguageSelectedTab Is Nothing) Then
                        'get the siblings from the selectedtab and iterate through until you find a sibbling with a corresponding defaultlanguagetab
                        'if none are found get a list of all the tabs from the default language and then select the last one
                        Dim selectedTabSibblings As List(Of TabInfo) = tabCtrl.GetTabsByPortal(tab.PortalID).WithCulture(tab.CultureCode, True).AsList()
                        For Each sibling In selectedTabSibblings
                            Dim siblingDefaultTab As TabInfo = sibling.DefaultLanguageTab
                            If (Not siblingDefaultTab Is Nothing) Then
                                defaultLanguageSelectedTab = siblingDefaultTab
                                Exit For
                            End If
                        Next

                        If (defaultLanguageSelectedTab Is Nothing) Then 'still haven't found it
                            Dim defaultLanguageTabs As List(Of TabInfo) = tabCtrl.GetTabsByPortal(tab.PortalID).WithCulture(PortalSettings.Current.DefaultLanguage, True).AsList()
                            defaultLanguageSelectedTab = defaultLanguageTabs(defaultLanguageTabs.Count) 'get the last tab
                        End If

                    End If

                    relativeToTab = defaultLanguageSelectedTab
                End If
            End If



            If (location <> TabRelativeLocation.NOTSET) Then
                'Check Host tab - don't allow adding before or after
                If (IsHostConsolePage(relativeToTab) And (location = TabRelativeLocation.AFTER Or location = TabRelativeLocation.BEFORE)) Then
                    Throw New DotNetNukeException("You cannot add or move pages before or after the Host tab.", DotNetNukeErrorCode.HostBeforeAfterError)
                End If

                Dim parentTab As DotNetNuke.Entities.Tabs.TabInfo = GetParentTab(relativeToTab, location)
                Dim permissionList As String = "ADD,COPY,EDIT,MANAGE"
                'Check permissions for Page Editors when moving or inserting
                If (Not DotNetNuke.Security.PortalSecurity.IsInRole("Administrators")) Then
                    If (IsNothing(parentTab) OrElse Not DotNetNuke.Security.Permissions.TabPermissionController.HasTabPermission(parentTab.TabPermissions, permissionList)) Then
                        Throw New DotNetNukeException("You do not have permissions to add or move pages to this location. You can only add or move pages as children of pages you can edit.", DotNetNukeErrorCode.PageEditorPermissionError)
                    End If
                End If

                If (Not IsNothing(parentTab)) Then
                    tab.ParentId = parentTab.TabID
                    tab.Level = parentTab.Level + 1
                Else
                    tab.ParentId = Null.NullInteger
                    tab.Level = 0
                End If
            End If

            If (tab.TabID > Null.NullInteger AndAlso tab.TabID = tab.ParentId) Then
                Throw New DotNetNukeException("Parent page is invalid.", DotNetNukeErrorCode.ParentTabInvalid)
            End If

            tab.TabPath = DotNetNuke.Common.GenerateTabPath(tab.ParentId, tab.TabName)

            Try
                If (tab.TabID < 0) Then
                    If (tab.TabPermissions.Count = 0 AndAlso tab.PortalID <> Null.NullInteger) Then
                        'Give admin full permission
                        Dim permissions As ArrayList = DotNetNuke.Security.Permissions.PermissionController.GetPermissionsByTab()

                        For Each permission As DotNetNuke.Security.Permissions.PermissionInfo In permissions
                            Dim newTabPermission As New DotNetNuke.Security.Permissions.TabPermissionInfo()
                            newTabPermission.PermissionID = permission.PermissionID
                            newTabPermission.PermissionKey = permission.PermissionKey
                            newTabPermission.PermissionName = permission.PermissionName
                            newTabPermission.AllowAccess = True
                            newTabPermission.RoleID = PortalSettings.Current.AdministratorRoleId
                            tab.TabPermissions.Add(newTabPermission)
                        Next
                    End If

                    Dim _PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()

                    If _PortalSettings.ContentLocalizationEnabled Then
                        Dim defaultLocale As Locale = LocaleController.Instance.GetDefaultLocale(tab.PortalID)
                        tab.CultureCode = defaultLocale.Code
                    Else
                        tab.CultureCode = Null.NullString
                    End If

                    If (location = TabRelativeLocation.AFTER AndAlso Not IsNothing(relativeToTab)) Then
                        tab.TabID = tabCtrl.AddTabAfter(tab, relativeToTab.TabID)
                    ElseIf (location = TabRelativeLocation.BEFORE AndAlso Not IsNothing(relativeToTab)) Then
                        tab.TabID = tabCtrl.AddTabBefore(tab, relativeToTab.TabID)
                    Else
                        tab.TabID = tabCtrl.AddTab(tab)
                    End If

                    If _PortalSettings.ContentLocalizationEnabled Then
                        tabCtrl.CreateLocalizedCopies(tab)
                    End If

                    tabCtrl.UpdateTabSetting(tab.TabID, "CacheProvider", "")
                    tabCtrl.UpdateTabSetting(tab.TabID, "CacheDuration", "")
                    tabCtrl.UpdateTabSetting(tab.TabID, "CacheIncludeExclude", "0")
                    tabCtrl.UpdateTabSetting(tab.TabID, "IncludeVaryBy", "")
                    tabCtrl.UpdateTabSetting(tab.TabID, "ExcludeVaryBy", "")
                    tabCtrl.UpdateTabSetting(tab.TabID, "MaxVaryByCount", "")
                Else
                    tabCtrl.UpdateTab(tab)

                    If (location = TabRelativeLocation.AFTER AndAlso Not IsNothing(relativeToTab)) Then
                        tabCtrl.MoveTabAfter(tab, relativeToTab.TabID)
                    ElseIf (location = TabRelativeLocation.BEFORE AndAlso Not IsNothing(relativeToTab)) Then
                        tabCtrl.MoveTabBefore(tab, relativeToTab.TabID)
                    End If
                End If
            Catch ex As Exception
                If (ex.Message = "Tab Exists") Then
                    Throw New DotNetNukeException("Page already exists.", DotNetNukeErrorCode.PageExists)
                End If
            End Try

            ' create the page from a template
            If (Not String.IsNullOrEmpty(templateMapPath)) Then
                Dim xmlDoc As New System.Xml.XmlDocument()
                Try
                    xmlDoc.Load(templateMapPath)
                    TabController.DeserializePanes(xmlDoc.SelectSingleNode("//portal/tabs/tab/panes"), tab.PortalID, tab.TabID, DotNetNuke.Entities.Portals.PortalTemplateModuleAction.Ignore, New Hashtable)
                Catch ex As Exception
                    LogException(ex)
                    Throw New DotNetNukeException("Unable to process page template.", ex, DotNetNukeErrorCode.DeserializePanesFailed)
                End Try
            End If

            'todo: reload tab from db or send back tabid instead?
            Return tab.TabID
        End Function

        Public Shared Function Validate_IsCircularReference(ByVal portalID As Integer, ByVal tabID As Integer) As Boolean
            If tabID <> -1 Then
                Dim objTabs As New TabController
                Dim objtab As TabInfo = objTabs.GetTab(tabID, portalID, False)

                If (IsNothing(objtab)) Then
                    Return False
                ElseIf objtab.Level = 0 Then
                    Return False
                Else
                    If tabID = objtab.ParentId Then
                        Return True
                    Else
                        Return Validate_IsCircularReference(portalID, objtab.ParentId)
                    End If
                End If
            Else
                Return False
            End If
        End Function

	End Class

	Public Class DotNetNukeException
		Inherits System.Exception
		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
			MyBase.New(message, innerException)
		End Sub

		Public Sub New(ByVal message As String, ByVal errorCode As DotNetNukeErrorCode)
			MyBase.New(message)
			_ErrorCode = errorCode
		End Sub

		Public Sub New(ByVal message As String, ByVal innerException As System.Exception, ByVal errorCode As DotNetNukeErrorCode)
			MyBase.New(message, innerException)
			_ErrorCode = errorCode
		End Sub

		Public Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub

		Private _ErrorCode As DotNetNukeErrorCode = DotNetNukeErrorCode.NotSet
		Public ReadOnly Property ErrorCode() As DotNetNukeErrorCode
			Get
				Return _ErrorCode
			End Get
		End Property
	End Class

	Public Enum DotNetNukeErrorCode
		NotSet
		PageExists
		PageNameRequired
		PageNameInvalid
		DeserializePanesFailed
		PageCircularReference
		ParentTabInvalid
		PageEditorPermissionError
		HostBeforeAfterError
	End Enum

	Public Enum TabRelativeLocation
		NOTSET
		BEFORE
		AFTER
		CHILD
	End Enum

End Namespace

