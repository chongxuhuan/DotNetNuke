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
Imports System.Xml
Imports System.IO
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Common.Lists
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml.XPath
Imports System.Threading
Imports System.Text

Namespace DotNetNuke.Entities.Portals

    Public Class PortalController

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPortalCallback gets the Portal from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetPortalCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim cultureCode As String = DirectCast(cacheItemArgs.ParamList(1), String)
            Dim objPortal As Object
            If Localization.ActiveLanguagesByPortalID(portalID) = 1 Then
                'only 1 language active, no need for fallback check
                Return CBO.FillObject(Of PortalInfo)(DataProvider.Instance.GetPortal(portalID, cultureCode))
            Else
                Dim dr As System.Data.IDataReader
                dr = DataProvider.Instance.GetPortal(portalID, cultureCode)
                objPortal = CBO.FillObject(Of PortalInfo)(dr)
                If objPortal Is Nothing Then
                    'Get Fallback language
                    Dim fallbackLanguage As String = String.Empty
                    Dim userLocale As Locale = LocaleController.Instance().GetLocale(cultureCode)
                    If userLocale IsNot Nothing AndAlso Not String.IsNullOrEmpty(userLocale.Fallback) Then
                        fallbackLanguage = userLocale.Fallback
                    End If
                    dr = DataProvider.Instance.GetPortal(portalID, fallbackLanguage)
                    objPortal = CBO.FillObject(Of PortalInfo)(dr)
                    If objPortal Is Nothing Then
                        objPortal = CBO.FillObject(Of PortalInfo)(DataProvider.Instance.GetPortal(portalID, PortalController.GetActivePortalLanguage(portalID)))
                    End If
                    'if we cannot find any fallback, it mean's it's a non portal default langauge
                    DataProvider.Instance().EnsureLocalizationExists(portalID, PortalController.GetActivePortalLanguage(portalID))
                    objPortal = CBO.FillObject(Of PortalInfo)(DataProvider.Instance.GetPortal(portalID, PortalController.GetActivePortalLanguage(portalID)))
                    dr.Close()
                    dr.Dispose()
                End If
            End If
            Return objPortal
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPortalDictioanryCallback gets the Portal Lookup Dictionary from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	07/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetPortalDictionaryCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalDic As New Dictionary(Of Integer, Integer)

            If Host.Host.PerformanceSetting <> PerformanceSettings.NoCaching Then
                ' get all tabs
                Dim intField As Integer
                Dim dr As IDataReader = DataProvider.Instance().GetTabPaths(Null.NullInteger, Null.NullString)
                Try
                    While dr.Read
                        ' add to dictionary
                        portalDic(Convert.ToInt32(Null.SetNull(dr("TabID"), intField))) = Convert.ToInt32(Null.SetNull(dr("PortalID"), intField))
                    End While
                Catch exc As Exception
                    LogException(exc)
                Finally
                    ' close datareader
                    CBO.CloseDataReader(dr, True)
                End Try
            End If
            Return portalDic
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPortalSettingsDictionaryCallback gets a Dictionary of Portal Settings
        ''' from the the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetPortalSettingsDictionaryCallback(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalID As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Dim dicSettings As New Dictionary(Of String, String)
            Dim dr As IDataReader = DataProvider.Instance.GetPortalSettings(portalID, PortalController.GetActivePortalLanguage(portalID))
            Try
                While dr.Read()
                    If Not dr.IsDBNull(1) Then
                        dicSettings.Add(dr.GetString(0), dr.GetString(1))
                    End If
                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
            Return dicSettings
        End Function

#End Region

#Region "Public Shared Methods"

        Public Shared Sub AddPortalDictionary(ByVal portalId As Integer, ByVal tabId As Integer)
            Dim portalDic As Dictionary(Of Integer, Integer) = GetPortalDictionary()
            portalDic(tabId) = portalId

            DataCache.SetCache(DataCache.PortalDictionaryCacheKey, portalDic)
        End Sub

        Public Shared Sub DeleteExpiredPortals(ByVal serverPath As String)
            For Each portal As PortalInfo In GetExpiredPortals()
                DeletePortal(portal, serverPath)
            Next
        End Sub

        Public Shared Function IsChildPortal(ByVal portal As PortalInfo, ByVal serverPath As String) As Boolean
            Dim isChild As Boolean = Null.NullBoolean
            Dim portalName As String
            Dim aliasController As New PortalAliasController
            Dim arr As ArrayList = aliasController.GetPortalAliasArrayByPortalID(portal.PortalID)
            If arr.Count > 0 Then
                Dim portalAlias As PortalAliasInfo = CType(arr(0), PortalAliasInfo)
                portalName = GetPortalDomainName(portalAlias.HTTPAlias)
                If Convert.ToBoolean(InStr(1, portalAlias.HTTPAlias, "/")) Then
                    portalName = Mid(portalAlias.HTTPAlias, InStrRev(portalAlias.HTTPAlias, "/") + 1)
                End If
                If portalName <> "" AndAlso System.IO.Directory.Exists(serverPath & portalName) Then
                    isChild = True
                End If
            End If
            Return isChild
        End Function

        Public Shared Function DeletePortal(ByVal portal As PortalInfo, ByVal serverPath As String) As String
            Dim strPortalName As String
            Dim strMessage As String = String.Empty

            ' check if this is the last portal
            Dim portalCount As Integer = DataProvider.Instance.GetPortalCount()

            If portalCount > 1 Then
                If Not portal Is Nothing Then
                    ' delete custom resource files
                    DeleteFilesRecursive(serverPath, ".Portal-" + portal.PortalID.ToString + ".resx")

                    'If child portal delete child folder
                    Dim objPortalAliasController As New PortalAliasController
                    Dim arr As ArrayList = objPortalAliasController.GetPortalAliasArrayByPortalID(portal.PortalID)
                    If arr.Count > 0 Then
                        Dim objPortalAliasInfo As PortalAliasInfo = CType(arr(0), PortalAliasInfo)
                        strPortalName = GetPortalDomainName(objPortalAliasInfo.HTTPAlias)
                        If Convert.ToBoolean(InStr(1, objPortalAliasInfo.HTTPAlias, "/")) Then
                            strPortalName = Mid(objPortalAliasInfo.HTTPAlias, InStrRev(objPortalAliasInfo.HTTPAlias, "/") + 1)
                        End If
                        If strPortalName <> "" AndAlso System.IO.Directory.Exists(serverPath & strPortalName) Then
                            DeleteFolderRecursive(serverPath & strPortalName)
                        End If
                    End If

                    ' delete upload directory
                    DeleteFolderRecursive(serverPath & "Portals\" & portal.PortalID.ToString)
                    If Not String.IsNullOrEmpty(portal.HomeDirectory) Then
                        Dim HomeDirectory As String = portal.HomeDirectoryMapPath
                        If System.IO.Directory.Exists(HomeDirectory) Then
                            DeleteFolderRecursive(HomeDirectory)
                        End If
                    End If

                    ' remove database references
                    Dim objPortalController As New PortalController
                    objPortalController.DeletePortalInfo(portal.PortalID)
                End If
            Else
                strMessage = Localization.GetString("LastPortal")
            End If

            Return strMessage
        End Function

        Public Shared Function GetPortalDictionary() As Dictionary(Of Integer, Integer)
            Dim cacheKey As String = String.Format(DataCache.PortalDictionaryCacheKey)
            Return CBO.GetCachedObject(Of Dictionary(Of Integer, Integer))(New CacheItemArgs(cacheKey, DataCache.PortalDictionaryTimeOut, DataCache.PortalDictionaryCachePriority), _
                                                                                        AddressOf GetPortalDictionaryCallback)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPortalsByName gets all the portals whose name matches a provided filter expression
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="nameToMatch">The email address to use to find a match.</param>
        ''' <param name="pageIndex">The page of records to return.</param>
        ''' <param name="pageSize">The size of the page</param>
        ''' <param name="totalRecords">The total no of records that satisfy the criteria.</param>
        ''' <returns>An ArrayList of PortalInfo objects.</returns>
        ''' <history>
        '''     [cnurse]	11/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetPortalsByName(ByVal nameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As ArrayList
            If pageIndex = -1 Then
                pageIndex = 0
                pageSize = Integer.MaxValue
            End If

            Return CBO.FillCollection(DataProvider.Instance().GetPortalsByName(nameToMatch, pageIndex, pageSize), GetType(PortalInfo), totalRecords)
        End Function

        Public Shared Function GetCurrentPortalSettings() As PortalSettings
            Dim objPortalSettings As PortalSettings = Nothing
            If HttpContext.Current IsNot Nothing Then
                objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            End If
            Return objPortalSettings
        End Function

        Public Shared Function GetExpiredPortals() As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetExpiredPortals(), GetType(PortalInfo))
        End Function

        Public Shared Sub DeletePortalSetting(ByVal portalID As Integer, ByVal settingName As String)
            DeletePortalSetting(portalID, settingName, PortalController.GetActivePortalLanguage(portalID))
        End Sub

        Public Shared Sub DeletePortalSetting(ByVal portalID As Integer, ByVal settingName As String, ByVal CultureCode As String)
            DataProvider.Instance.DeletePortalSetting(portalID, settingName, CultureCode.ToString.ToLower)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("SettingName", settingName.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_DELETED)
            DataCache.ClearPortalCache(portalID, False)
        End Sub

        Public Shared Sub DeletePortalSettings(ByVal portalID As Integer)
            DataProvider.Instance.DeletePortalSettings(portalID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalID", portalID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_DELETED)
            DataCache.ClearPortalCache(portalID, False)
        End Sub

        Public Shared Function GetPortalSettingsDictionary(ByVal portalID As Integer) As Dictionary(Of String, String)
            Dim cacheKey As String = String.Format(DataCache.PortalSettingsCacheKey, portalID.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of String, String))(New CacheItemArgs(cacheKey, DataCache.PortalSettingsCacheTimeOut, DataCache.PortalSettingsCachePriority, portalID), _
                                                                                        AddressOf GetPortalSettingsDictionaryCallback, True)
        End Function

        Public Shared Function GetPortalSetting(ByVal settingName As String, ByVal portalID As Integer, ByVal defaultValue As String) As String
            Dim retValue As String = Null.NullString
            Try
                Dim setting As String = Null.NullString
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(settingName, setting)
                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = setting
                End If
            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

        Public Shared Function GetPortalSettingAsBoolean(ByVal key As String, ByVal portalID As Integer, ByVal defaultValue As Boolean) As Boolean
            Dim retValue As Boolean
            Try
                Dim setting As String = Null.NullString
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(key, setting)
                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = (setting.StartsWith("Y", StringComparison.InvariantCultureIgnoreCase) OrElse setting.ToUpperInvariant = "TRUE")
                End If
            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

        Public Shared Function GetPortalSettingAsInteger(ByVal key As String, ByVal portalID As Integer, ByVal defaultValue As Integer) As Integer
            Dim retValue As Integer
            Try
                Dim setting As String = Null.NullString
                PortalController.GetPortalSettingsDictionary(portalID).TryGetValue(key, setting)
                If String.IsNullOrEmpty(setting) Then
                    retValue = defaultValue
                Else
                    retValue = Convert.ToInt32(setting)
                End If
            Catch ex As Exception
                'we just want to trap the error as we may not be installed so there will be no Settings
            End Try
            Return retValue
        End Function

        Public Shared Sub UpdatePortalSetting(ByVal portalID As Integer, ByVal settingName As String, ByVal settingValue As String)
            UpdatePortalSetting(portalID, settingName, settingValue, True)
        End Sub

        Public Shared Sub UpdatePortalSetting(ByVal portalID As Integer, ByVal settingName As String, ByVal settingValue As String, ByVal clearCache As Boolean)
            Dim culture As String = Thread.CurrentThread.CurrentCulture.ToString().ToLower()
            If (String.IsNullOrEmpty(culture)) Then
                culture = GetPortalSetting("DefaultLanguage", portalID, "").ToLower()
            End If

            If (String.IsNullOrEmpty(culture)) Then
                culture = Localization.SystemLocale.ToLower()
            End If
            UpdatePortalSetting(portalID, settingName, settingValue, clearCache, culture)
        End Sub

        Public Shared Sub UpdatePortalSetting(ByVal portalID As Integer, ByVal settingName As String, ByVal settingValue As String, ByVal clearCache As Boolean, ByVal culturecode As String)
            DataProvider.Instance.UpdatePortalSetting(portalID, settingName, settingValue, UserController.GetCurrentUserInfo.UserID, culturecode)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(settingName.ToString, settingValue.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)

            If clearCache Then
                DataCache.ClearPortalCache(portalID, False)
            End If
        End Sub

        Public Shared Function CheckDesktopModulesInstalled(ByVal nav As XPathNavigator) As String
            Dim friendlyName As String = Null.NullString
            Dim desktopModule As DesktopModuleInfo = Nothing
            Dim modulesNotInstalled As New StringBuilder

            For Each desktopModuleNav As XPathNavigator In nav.Select("portalDesktopModule")
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname")

                If Not String.IsNullOrEmpty(friendlyName) Then
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName)
                    If desktopModule Is Nothing Then
                        modulesNotInstalled.Append(friendlyName)
                        modulesNotInstalled.Append("<br/>")
                    End If
                End If
            Next
            Return modulesNotInstalled.ToString
        End Function
        ''' <summary>
        ''' function provides the language for portalinfo requests
        ''' in case where language has not been installed yet, will return the core install default of en-us
        ''' </summary>
        ''' <param name="portalID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetActivePortalLanguage(ByVal portalID As Integer) As String
            ' get Language
            Dim Language As String = Localization.SystemLocale 'handles case where portalcontroller methods invoked before a language is installed
            Dim tmpLanguage As String = GetPortalDefaultLanguage(portalID)
            If Not String.IsNullOrEmpty(tmpLanguage) Then
                Language = tmpLanguage
            End If
            If portalID > Null.NullInteger AndAlso Status = UpgradeStatus.None AndAlso _
                            Localization.ActiveLanguagesByPortalID(portalID) = 1 Then
                Return Language
            End If
            If HttpContext.Current IsNot Nothing AndAlso Status = UpgradeStatus.None Then
                If Not HttpContext.Current.Request.QueryString("language") Is Nothing Then
                    Language = HttpContext.Current.Request.QueryString("language")
                Else
                    Dim _PortalSettings As PortalSettings = GetCurrentPortalSettings()
                    If _PortalSettings IsNot Nothing AndAlso _PortalSettings.ActiveTab IsNot Nothing AndAlso Not String.IsNullOrEmpty(_PortalSettings.ActiveTab.CultureCode) Then
                        Language = _PortalSettings.ActiveTab.CultureCode
                    Else
                        'PortalSettings IS Nothing - probably means we haven't set it yet (in Begin Request)
                        'so try detecting the user's browser
                        Dim Culture As CultureInfo = Localization.GetBrowserCulture(portalID)

                        If Culture IsNot Nothing Then
                            Language = Culture.Name
                        End If
                    End If
                End If
            End If
            Return Language
        End Function

        ''' <summary>
        ''' return the current DefaultLanguage value from the Portals table for the requested Portalid
        ''' </summary>
        ''' <param name="portalID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetPortalDefaultLanguage(ByVal portalID As Integer) As String
            Return DataProvider.Instance().GetPortalDefaultLanguage(portalID)
        End Function

        ''' <summary>
        ''' set the required DefaultLanguage in the Portals table for a particular portal
        ''' saves having to update an entire PortalInfo object
        ''' </summary>
        ''' <param name="portalID"></param>
        ''' <param name="CultureCode"></param>
        ''' <remarks></remarks>
        Public Shared Sub UpdatePortalDefaultLanguage(ByVal portalID As Integer, ByVal CultureCode As String)
            DataProvider.Instance().UpdatePortalDefaultLanguage(portalID, CultureCode)
            'ensure localization record exists as new portal default language may be relying on fallback chain
            'of which it is now the final part
            DataProvider.Instance().EnsureLocalizationExists(portalID, CultureCode)
        End Sub
#End Region

#Region "Private Methods"

#Region "Role Methods"

        Private Sub CreateDefaultPortalRoles(ByVal portalId As Integer, ByVal administratorId As Integer, ByVal administratorRoleId As Integer, ByVal registeredRoleId As Integer, ByVal subscriberRoleId As Integer)
            Dim controller As New RoleController()

            ' create required roles if not already created
            If administratorRoleId = -1 Then
                administratorRoleId = CreateRole(portalId, "Administrators", "Portal Administrators", 0, 0, "M", 0, 0, "N", False, False)
            End If
            If registeredRoleId = -1 Then
                registeredRoleId = CreateRole(portalId, "Registered Users", "Registered Users", 0, 0, "M", 0, 0, "N", False, True)
            End If
            If subscriberRoleId = -1 Then
                subscriberRoleId = CreateRole(portalId, "Subscribers", "A public role for portal subscriptions", 0, 0, "M", 0, 0, "N", True, True)
            End If

            controller.AddUserRole(portalId, administratorId, administratorRoleId, Null.NullDate, Null.NullDate)
            controller.AddUserRole(portalId, administratorId, registeredRoleId, Null.NullDate, Null.NullDate)
            controller.AddUserRole(portalId, administratorId, subscriberRoleId, Null.NullDate, Null.NullDate)
        End Sub

        Private Function CreateRole(ByVal role As RoleInfo) As Integer
            Dim objRoleInfo As RoleInfo
            Dim objRoleController As New RoleController
            Dim roleId As Integer = Null.NullInteger

            'First check if the role exists
            objRoleInfo = objRoleController.GetRoleByName(role.PortalID, role.RoleName)

            If objRoleInfo Is Nothing Then
                roleId = objRoleController.AddRole(role)
            Else
                roleId = objRoleInfo.RoleID
            End If
            Return roleId
        End Function

        Private Function CreateRole(ByVal portalId As Integer, ByVal roleName As String, ByVal description As String, ByVal serviceFee As Single, ByVal billingPeriod As Integer, ByVal billingFrequency As String, ByVal trialFee As Single, ByVal trialPeriod As Integer, ByVal trialFrequency As String, ByVal isPublic As Boolean, ByVal isAuto As Boolean) As Integer
            Dim objRoleInfo As New RoleInfo
            objRoleInfo.PortalID = portalId
            objRoleInfo.RoleName = roleName
            objRoleInfo.RoleGroupID = Null.NullInteger
            objRoleInfo.Description = description
            objRoleInfo.ServiceFee = CType(IIf(serviceFee < 0, 0, serviceFee), Single)
            objRoleInfo.BillingPeriod = billingPeriod
            objRoleInfo.BillingFrequency = billingFrequency
            objRoleInfo.TrialFee = CType(IIf(trialFee < 0, 0, trialFee), Single)
            objRoleInfo.TrialPeriod = trialPeriod
            objRoleInfo.TrialFrequency = trialFrequency
            objRoleInfo.IsPublic = isPublic
            objRoleInfo.AutoAssignment = isAuto

            Return CreateRole(objRoleInfo)
        End Function

        Private Sub CreateRoleGroup(ByVal roleGroup As RoleGroupInfo)
            Dim objRoleGroupInfo As RoleGroupInfo
            Dim objRoleController As New RoleController
            Dim roleGroupId As Integer = Null.NullInteger

            'First check if the role exists
            objRoleGroupInfo = RoleController.GetRoleGroupByName(roleGroup.PortalID, roleGroup.RoleGroupName)

            If objRoleGroupInfo Is Nothing Then
                roleGroup.RoleGroupID = RoleController.AddRoleGroup(roleGroup)
            Else
                roleGroup.RoleGroupID = objRoleGroupInfo.RoleGroupID
            End If
        End Sub

        Private Sub ParseRoles(ByVal nav As XPathNavigator, ByVal portalID As Integer, ByVal administratorId As Integer)
            Dim administratorRoleId As Integer = -1
            Dim registeredRoleId As Integer = -1
            Dim subscriberRoleId As Integer = -1
            Dim controller As New RoleController()

            For Each roleNav As XPathNavigator In nav.Select("role")
                Dim role As RoleInfo = CBO.DeserializeObject(Of RoleInfo)(New StringReader(roleNav.OuterXml))
                role.PortalID = portalID
                role.RoleGroupID = Null.NullInteger
                Select Case role.RoleType
                    Case RoleType.Administrator
                        administratorRoleId = CreateRole(role)
                    Case RoleType.RegisteredUser
                        registeredRoleId = CreateRole(role)
                    Case RoleType.Subscriber
                        subscriberRoleId = CreateRole(role)
                    Case RoleType.None
                        CreateRole(role)
                End Select
            Next

            ' create required roles if not already created
            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId)

            ' update portal setup
            Dim objportal As PortalInfo
            objportal = GetPortal(portalID)
            UpdatePortalSetup(portalID, administratorId, administratorRoleId, registeredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(portalID))
        End Sub

        Private Sub ParseRoleGroups(ByVal nav As XPathNavigator, ByVal portalID As Integer, ByVal administratorId As Integer)
            Dim administratorRoleId As Integer = -1
            Dim registeredRoleId As Integer = -1
            Dim subscriberRoleId As Integer = -1
            Dim controller As New RoleController()

            For Each roleGroupNav As XPathNavigator In nav.Select("rolegroup")
                Dim roleGroup As RoleGroupInfo = CBO.DeserializeObject(Of RoleGroupInfo)(New StringReader(roleGroupNav.OuterXml))
                If roleGroup.RoleGroupName <> "GlobalRoles" Then
                    roleGroup.PortalID = portalID
                    CreateRoleGroup(roleGroup)
                End If

                For Each role As RoleInfo In roleGroup.Roles.Values
                    role.PortalID = portalID
                    role.RoleGroupID = roleGroup.RoleGroupID
                    Select Case role.RoleType
                        Case RoleType.Administrator
                            administratorRoleId = CreateRole(role)
                        Case RoleType.RegisteredUser
                            registeredRoleId = CreateRole(role)
                        Case RoleType.Subscriber
                            subscriberRoleId = CreateRole(role)
                        Case RoleType.None
                            CreateRole(role)
                    End Select
                Next
            Next

            CreateDefaultPortalRoles(portalID, administratorId, administratorRoleId, registeredRoleId, subscriberRoleId)

            ' update portal setup
            Dim objportal As PortalInfo
            objportal = GetPortal(portalID)
            UpdatePortalSetup(portalID, administratorId, administratorRoleId, registeredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(portalID))
        End Sub

#End Region

        Private Sub AddFolderPermissions(ByVal PortalId As Integer, ByVal folderId As Integer)
            Dim objPortal As PortalInfo = GetPortal(PortalId)
            Dim objController As New FolderController
            Dim objFolderPermission As FolderPermissionInfo
            Dim folder As FolderInfo = objController.GetFolderInfo(PortalId, folderId)

            Dim objPermissionController As New Permissions.PermissionController
            For Each objpermission As Permissions.PermissionInfo In objPermissionController.GetPermissionByCodeAndKey("SYSTEM_FOLDER", "")
                objFolderPermission = New FolderPermissionInfo(objpermission)
                objFolderPermission.FolderID = folder.FolderID
                objFolderPermission.RoleID = objPortal.AdministratorRoleId
                folder.FolderPermissions.Add(objFolderPermission)

                If objpermission.PermissionKey = "READ" Then
                    ' add READ permissions to the All Users Role
                    FileSystemUtils.AddAllUserReadPermission(folder, objpermission)
                End If
            Next

            FolderPermissionController.SaveFolderPermissions(folder)
        End Sub

        Private Function CreateProfileDefinitions(ByVal PortalId As Integer, ByVal TemplatePath As String, ByVal TemplateFile As String) As String

            Dim strMessage As String = Null.NullString
            Try
                ' add profile definitions
                Dim xmlDoc As New XmlDocument
                Dim node As XmlNode
                ' open the XML template file
                Try
                    xmlDoc.Load(TemplatePath & TemplateFile)
                Catch
                    ' error
                End Try

                ' parse profile definitions if available
                node = xmlDoc.SelectSingleNode("//portal/profiledefinitions")
                If Not node Is Nothing Then
                    ParseProfileDefinitions(node, PortalId)
                Else ' template does not contain profile definitions ( ie. was created prior to DNN 3.3.0 )
                    ProfileController.AddDefaultDefinitions(PortalId)
                End If

            Catch ex As Exception
                strMessage = Localization.GetString("CreateProfileDefinitions.Error")
                LogException(ex)
            End Try

            Return strMessage

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new portal based on the portal template provided.
        ''' </summary>
        ''' <param name="PortalName">Name of the portal to be created</param>
        ''' <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	03/09/2004	Modified to support new template format.
        '''                             Portal template file should be processed before admin.template
        '''     [cnurse]    01/11/2005  Template parsing moved to CreatePortal
        '''     [cnurse]    05/10/2006  Removed unneccessary use of Administrator properties
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreatePortal(ByVal PortalName As String, ByVal HomeDirectory As String) As Integer
            ' add portal
            Dim PortalId As Integer = -1
            Try
                ' Use host settings as default values for these parameters
                ' This can be overwritten on the portal template
                Dim datExpiryDate As Date
                If Host.Host.DemoPeriod > Null.NullInteger Then
                    datExpiryDate = Convert.ToDateTime(GetMediumDate(DateAdd(DateInterval.Day, Host.Host.DemoPeriod, Now()).ToString))
                Else
                    datExpiryDate = Null.NullDate
                End If

                PortalId = DataProvider.Instance().CreatePortal(PortalName, Host.Host.HostCurrency, datExpiryDate, Host.Host.HostFee, Host.Host.HostSpace, Host.Host.PageQuota, Host.Host.UserQuota, Host.Host.SiteLogHistory, HomeDirectory, UserController.GetCurrentUserInfo.UserID)
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog("PortalName", PortalName.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_CREATED)
            Catch ex As Exception
                'Log Exception
                LogException(ex)
            End Try

            Return PortalId
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes all Files from the template
        ''' </summary>
        ''' <param name="nodeFiles">Template file node for the Files</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <history>
        ''' 	[cnurse]	11/09/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParseFiles(ByVal nodeFiles As XmlNodeList, ByVal PortalId As Integer, ByVal objFolder As FolderInfo)

            Dim node As XmlNode
            Dim FileId As Integer
            Dim objController As New FileController
            Dim objInfo As DotNetNuke.Services.FileSystem.FileInfo
            Dim fileName As String

            For Each node In nodeFiles
                fileName = XmlUtils.GetNodeValue(node, "filename")

                'First check if the file exists
                objInfo = objController.GetFile(fileName, PortalId, objFolder.FolderID)

                If objInfo Is Nothing Then
                    objInfo = New DotNetNuke.Services.FileSystem.FileInfo
                    objInfo.PortalId = PortalId
                    objInfo.FileName = fileName
                    objInfo.Extension = XmlUtils.GetNodeValue(node, "extension")
                    objInfo.Size = XmlUtils.GetNodeValueInt(node, "size")
                    objInfo.Width = XmlUtils.GetNodeValueInt(node, "width")
                    objInfo.Height = XmlUtils.GetNodeValueInt(node, "height")
                    objInfo.ContentType = XmlUtils.GetNodeValue(node, "contenttype")
                    objInfo.SHA1Hash = XmlUtils.GetNodeValue(node, "sha1hash")
                    objInfo.FolderId = objFolder.FolderID
                    objInfo.Folder = objFolder.FolderPath

                    'Save new File 
                    FileId = objController.AddFile(objInfo)
                Else
                    'Get Id from File
                    FileId = objInfo.FileId
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes all Folders from the template
        ''' </summary>
        ''' <param name="nodeFolders">Template file node for the Folders</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <history>
        ''' 	[cnurse]	11/09/2004	Created
        '''     [vnguyen]   30/04/2010  Updated: Added Guid's to AddFolder method call
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParseFolders(ByVal nodeFolders As XmlNode, ByVal PortalId As Integer)
            Dim node As XmlNode
            Dim FolderId As Integer
            Dim objController As New FolderController
            Dim objInfo As FolderInfo
            Dim folderPath As String
            Dim storageLocation As Integer
            Dim isProtected As Boolean = False

            For Each node In nodeFolders.SelectNodes("//folder")
                folderPath = XmlUtils.GetNodeValue(node, "folderpath")

                'First check if the folder exists
                objInfo = objController.GetFolder(PortalId, folderPath, False)

                If objInfo Is Nothing Then
                    isProtected = FileSystemUtils.DefaultProtectedFolders(folderPath)
                    If isProtected = True Then
                        ' protected folders must use insecure storage
                        storageLocation = FolderController.StorageLocationTypes.InsecureFileSystem
                    Else
                        storageLocation = CType(XmlUtils.GetNodeValue(node, "storagelocation", "0"), Integer)
                        isProtected = CType(XmlUtils.GetNodeValue(node, "isprotected", "0"), Boolean)
                    End If
                    'Save new folder
                    Dim folder As FolderInfo = New FolderInfo(PortalId, folderPath, storageLocation, isProtected, False, Null.NullDate)
                    FolderId = objController.AddFolder(folder)
                    objInfo = objController.GetFolder(PortalId, folderPath, False)

                End If

                Dim nodeFolderPermissions As XmlNodeList = node.SelectNodes("folderpermissions/permission")
                ParseFolderPermissions(nodeFolderPermissions, PortalId, objInfo)

                Dim nodeFiles As XmlNodeList = node.SelectNodes("files/file")
                If folderPath <> "" Then
                    folderPath += "/"
                End If
                ParseFiles(nodeFiles, PortalId, objInfo)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Parses folder permissions
        ''' </summary>
        ''' <param name="nodeFolderPermissions">Node for folder permissions</param>
        ''' <param name="PortalID">PortalId of new portal</param>
        ''' <param name="folder">The folder being processed</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/09/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParseFolderPermissions(ByVal nodeFolderPermissions As XmlNodeList, ByVal PortalId As Integer, ByVal folder As FolderInfo)
            Dim objPermissionController As New Security.Permissions.PermissionController
            Dim objRoleController As New RoleController
            Dim PermissionID As Integer

            'Clear the current folder permissions
            folder.FolderPermissions.Clear()

            For Each xmlFolderPermission As XmlNode In nodeFolderPermissions
                Dim PermissionKey As String = XmlUtils.GetNodeValue(xmlFolderPermission, "permissionkey")
                Dim PermissionCode As String = XmlUtils.GetNodeValue(xmlFolderPermission, "permissioncode")
                Dim RoleName As String = XmlUtils.GetNodeValue(xmlFolderPermission, "rolename")
                Dim AllowAccess As Boolean = XmlUtils.GetNodeValueBoolean(xmlFolderPermission, "allowaccess")

                For Each objPermission As PermissionInfo In objPermissionController.GetPermissionByCodeAndKey(PermissionCode, PermissionKey)
                    PermissionID = objPermission.PermissionID
                Next
                Dim RoleID As Integer = Integer.MinValue
                Select Case RoleName
                    Case glbRoleAllUsersName
                        RoleID = Convert.ToInt32(glbRoleAllUsers)
                    Case Common.Globals.glbRoleUnauthUserName
                        RoleID = Convert.ToInt32(glbRoleUnauthUser)
                    Case Else
                        Dim objRole As RoleInfo = objRoleController.GetRoleByName(PortalId, RoleName)
                        If Not objRole Is Nothing Then
                            RoleID = objRole.RoleID
                        End If
                End Select

                ' if role was found add, otherwise ignore
                If RoleID <> Integer.MinValue Then
                    Dim objFolderPermission As New FolderPermissionInfo
                    objFolderPermission.FolderID = folder.FolderID
                    objFolderPermission.PermissionID = PermissionID
                    objFolderPermission.RoleID = RoleID
                    objFolderPermission.AllowAccess = AllowAccess
                    folder.FolderPermissions.Add(objFolderPermission)
                End If
            Next

            FolderPermissionController.SaveFolderPermissions(folder)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes the settings node
        ''' </summary>
        ''' <param name="nodeSettings">Template file node for the settings</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	27/08/2004	Created
        ''' 	[VMasanas]	15/10/2004	Modified for new skin structure
        '''     [cnurse]    11/21/2004  Modified to use GetNodeValueDate for ExpiryDate
        '''     [VMasanas]  02/21/2005  Modified to not overwrite ExpiryDate if not present
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParsePortalSettings(ByVal nodeSettings As XmlNode, ByVal PortalId As Integer)

            Dim objPortal As PortalInfo
            objPortal = GetPortal(PortalId)

            objPortal.LogoFile = ImportFile(PortalId, XmlUtils.GetNodeValue(nodeSettings, "logofile"))
            objPortal.FooterText = XmlUtils.GetNodeValue(nodeSettings, "footertext")
            If Not nodeSettings.SelectSingleNode("expirydate") Is Nothing Then
                objPortal.ExpiryDate = XmlUtils.GetNodeValueDate(nodeSettings, "expirydate", Null.NullDate)
            End If
            objPortal.UserRegistration = XmlUtils.GetNodeValueInt(nodeSettings, "userregistration")
            objPortal.BannerAdvertising = XmlUtils.GetNodeValueInt(nodeSettings, "banneradvertising")
            If XmlUtils.GetNodeValue(nodeSettings, "currency") <> "" Then
                objPortal.Currency = XmlUtils.GetNodeValue(nodeSettings, "currency")
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "hostfee") <> "" Then
                objPortal.HostFee = XmlUtils.GetNodeValueSingle(nodeSettings, "hostfee")
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "hostspace") <> "" Then
                objPortal.HostSpace = XmlUtils.GetNodeValueInt(nodeSettings, "hostspace")
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "pagequota") <> "" Then
                objPortal.PageQuota = XmlUtils.GetNodeValueInt(nodeSettings, "pagequota")
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "userquota") <> "" Then
                objPortal.UserQuota = XmlUtils.GetNodeValueInt(nodeSettings, "userquota")
            End If
            objPortal.BackgroundFile = XmlUtils.GetNodeValue(nodeSettings, "backgroundfile")
            objPortal.PaymentProcessor = XmlUtils.GetNodeValue(nodeSettings, "paymentprocessor")
            If XmlUtils.GetNodeValue(nodeSettings, "siteloghistory") <> "" Then
                objPortal.SiteLogHistory = XmlUtils.GetNodeValueInt(nodeSettings, "siteloghistory")
            End If
            objPortal.DefaultLanguage = XmlUtils.GetNodeValue(nodeSettings, "defaultlanguage", "en-US")
            objPortal.TimeZoneOffset = XmlUtils.GetNodeValueInt(nodeSettings, "timezoneoffset", -8)

            UpdatePortalInfo(objPortal.PortalID, objPortal.PortalName, objPortal.LogoFile, objPortal.FooterText, _
             objPortal.ExpiryDate, objPortal.UserRegistration, objPortal.BannerAdvertising, objPortal.Currency, objPortal.AdministratorId, objPortal.HostFee, _
             objPortal.HostSpace, objPortal.PageQuota, objPortal.UserQuota, objPortal.PaymentProcessor, objPortal.ProcessorUserId, objPortal.ProcessorPassword, objPortal.Description, _
             objPortal.KeyWords, objPortal.BackgroundFile, objPortal.SiteLogHistory, objPortal.SplashTabId, objPortal.HomeTabId, objPortal.LoginTabId, objPortal.RegisterTabId, objPortal.UserTabId, _
             objPortal.DefaultLanguage, objPortal.TimeZoneOffset, objPortal.HomeDirectory)

            ' set portal skins and containers
            If XmlUtils.GetNodeValue(nodeSettings, "skinsrc", "") <> "" Then
                UpdatePortalSetting(PortalId, "DefaultPortalSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrc", ""))
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", "") <> "" Then
                UpdatePortalSetting(PortalId, "DefaultAdminSkin", XmlUtils.GetNodeValue(nodeSettings, "skinsrcadmin", ""))
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "containersrc", "") <> "" Then
                UpdatePortalSetting(PortalId, "DefaultPortalContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrc", ""))
            End If
            If XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", "") <> "" Then
                UpdatePortalSetting(PortalId, "DefaultAdminContainer", XmlUtils.GetNodeValue(nodeSettings, "containersrcadmin", ""))
            End If

            'Enable Skin Widgets Setting
            If XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", "") <> "" Then
                UpdatePortalSetting(PortalId, "EnableSkinWidgets", XmlUtils.GetNodeValue(nodeSettings, "enableskinwidgets", ""))
            End If
        End Sub

        Private Sub ParsePortalDesktopModules(ByVal nav As XPathNavigator, ByVal portalID As Integer)
            Dim friendlyName As String = Null.NullString
            Dim desktopModule As DesktopModuleInfo = Nothing

            For Each desktopModuleNav As XPathNavigator In nav.Select("portalDesktopModule")
                friendlyName = XmlUtils.GetNodeValue(desktopModuleNav, "friendlyname")

                If Not String.IsNullOrEmpty(friendlyName) Then
                    desktopModule = DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName)
                    If desktopModule IsNot Nothing Then
                        'Parse the permissions
                        Dim permissions As New DesktopModulePermissionCollection()
                        For Each permissionNav As XPathNavigator In desktopModuleNav.Select("portalDesktopModulePermissions/portalDesktopModulePermission")
                            Dim code As String = XmlUtils.GetNodeValue(permissionNav, "permissioncode")
                            Dim key As String = XmlUtils.GetNodeValue(permissionNav, "permissionkey")
                            Dim desktopModulePermission As DesktopModulePermissionInfo = Nothing

                            Dim arrPermissions As ArrayList = New PermissionController().GetPermissionByCodeAndKey(code, key)
                            If arrPermissions.Count > 0 Then
                                Dim permission As PermissionInfo = TryCast(arrPermissions(0), PermissionInfo)
                                If permission IsNot Nothing Then
                                    desktopModulePermission = New DesktopModulePermissionInfo(permission)
                                End If
                            End If

                            desktopModulePermission.AllowAccess = Boolean.Parse(XmlUtils.GetNodeValue(permissionNav, "allowaccess"))

                            Dim rolename As String = XmlUtils.GetNodeValue(permissionNav, "rolename")
                            If Not String.IsNullOrEmpty(rolename) Then
                                Dim role As RoleInfo = New RoleController().GetRoleByName(portalID, rolename)
                                If role IsNot Nothing Then
                                    desktopModulePermission.RoleID = role.RoleID
                                End If

                            End If

                            permissions.Add(desktopModulePermission)
                        Next

                        DesktopModuleController.AddDesktopModuleToPortal(portalID, desktopModule, permissions, False)
                    End If
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes all Profile Definitions from the template
        ''' </summary>
        ''' <param name="nodeProfileDefinitions">Template file node for the Profile Definitions</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParseProfileDefinitions(ByVal nodeProfileDefinitions As XmlNode, ByVal PortalId As Integer)

            Dim node As XmlNode

            Dim objListController As New ListController
            Dim colDataTypes As ListEntryInfoCollection = objListController.GetListEntryInfoCollection("DataType")

            Dim OrderCounter As Integer = -1

            Dim objProfileDefinition As ProfilePropertyDefinition

            For Each node In nodeProfileDefinitions.SelectNodes("//profiledefinition")
                OrderCounter += 2

                Dim typeInfo As ListEntryInfo = colDataTypes.Item("DataType:" + XmlUtils.GetNodeValue(node, "datatype"))
                If typeInfo Is Nothing Then
                    typeInfo = colDataTypes.Item("DataType:Unknown")
                End If

                objProfileDefinition = New ProfilePropertyDefinition(PortalId)
                objProfileDefinition.DataType = typeInfo.EntryID
                objProfileDefinition.DefaultValue = ""
                objProfileDefinition.ModuleDefId = Null.NullInteger
                objProfileDefinition.PropertyCategory = XmlUtils.GetNodeValue(node, "propertycategory")
                objProfileDefinition.PropertyName = XmlUtils.GetNodeValue(node, "propertyname")
                objProfileDefinition.Required = False
                objProfileDefinition.Visible = True
                objProfileDefinition.ViewOrder = OrderCounter
                objProfileDefinition.Length = XmlUtils.GetNodeValueInt(node, "length")
                Select Case XmlUtils.GetNodeValueInt(node, "defaultvisibility", 2)
                    Case 0 : objProfileDefinition.DefaultVisibility = UserVisibilityMode.AllUsers
                    Case 1 : objProfileDefinition.DefaultVisibility = UserVisibilityMode.MembersOnly
                    Case 2 : objProfileDefinition.DefaultVisibility = UserVisibilityMode.AdminOnly
                End Select

                ProfileController.AddPropertyDefinition(objProfileDefinition)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes all tabs from the template
        ''' </summary>
        ''' <param name="nodeTabs">Template file node for the tabs</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <param name="IsAdminTemplate">True when processing admin template, false when processing portal template</param>
        ''' <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        ''' <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        ''' <remarks>
        ''' When a special tab is found (HomeTab, UserTab, LoginTab, AdminTab) portal information will be updated.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	26/08/2004	Removed code to allow multiple tabs with same name.
        ''' 	[VMasanas]	15/10/2004	Modified for new skin structure
        '''		[cnurse]	15/10/2004	Modified to allow for merging template
        '''								with existing pages
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ParseTabs(ByVal nodeTabs As XmlNode, ByVal PortalId As Integer, ByVal IsAdminTemplate As Boolean, ByVal mergeTabs As PortalTemplateModuleAction, ByVal IsNewPortal As Boolean)

            Dim nodeTab As XmlNode
            'used to control if modules are true modules or instances
            'will hold module ID from template / new module ID so new instances can reference right moduleid
            'only first one from the template will be create as a true module, 
            'others will be moduleinstances (tabmodules)
            Dim hModules As New Hashtable
            Dim hTabs As New Hashtable

            'if running from wizard we need to pre populate htabs with existing tabs so ParseTab 
            'can find all existing ones
            Dim tabname As String
            If Not IsNewPortal Then
                Dim hTabNames As New Hashtable
                Dim objTabs As New TabController
                For Each tabPair As KeyValuePair(Of Integer, TabInfo) In objTabs.GetTabsByPortal(PortalId)
                    Dim objTab As TabInfo = tabPair.Value

                    If Not objTab.IsDeleted Then
                        tabname = objTab.TabName
                        If Not Null.IsNull(objTab.ParentId) Then
                            tabname = CType(hTabNames(objTab.ParentId), String) + "/" + objTab.TabName
                        End If
                        hTabNames.Add(objTab.TabID, tabname)
                    End If
                Next

                'when parsing tabs we will need tabid given tabname
                For Each i As Integer In hTabNames.Keys
                    If hTabs(hTabNames(i)) Is Nothing Then
                        hTabs.Add(hTabNames(i), i)
                    End If
                Next
                hTabNames = Nothing
            End If

            For Each nodeTab In nodeTabs.SelectNodes("//tab")
                ParseTab(nodeTab, PortalId, IsAdminTemplate, mergeTabs, hModules, hTabs, IsNewPortal)
            Next

            'Process tabs that are linked to tabs
            For Each nodeTab In nodeTabs.SelectNodes("//tab[url/@type = 'Tab']")
                Dim tabId As Integer = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger)
                Dim tabPath As String = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString)
                If tabId > Null.NullInteger Then
                    Dim controller As New TabController()
                    Dim objTab As TabInfo = controller.GetTab(tabId, PortalId, False)
                    objTab.Url = TabController.GetTabByTabPath(PortalId, tabPath).ToString()
                    controller.UpdateTab(objTab)
                End If

            Next

            'Process tabs that are linked to files
            For Each nodeTab In nodeTabs.SelectNodes("//tab[url/@type = 'File']")
                Dim tabId As Integer = XmlUtils.GetNodeValueInt(nodeTab, "tabid", Null.NullInteger)
                Dim filePath As String = XmlUtils.GetNodeValue(nodeTab, "url", Null.NullString)
                If tabId > Null.NullInteger Then
                    Dim controller As New TabController()
                    Dim objTab As TabInfo = controller.GetTab(tabId, PortalId, False)
                    objTab.Url = "FileID=" + New FileController().ConvertFilePathToFileId(filePath, PortalId).ToString()
                    controller.UpdateTab(objTab)
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes a single tab from the template
        ''' </summary>
        ''' <param name="nodeTab">Template file node for the tabs</param>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <param name="IsAdminTemplate">True when processing admin template, false when processing portal template</param>
        ''' <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        ''' <param name="hModules">Used to control if modules are true modules or instances</param>
        ''' <param name="hTabs">Supporting object to build the tab hierarchy</param>
        ''' <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        ''' <remarks>
        ''' When a special tab is found (HomeTab, UserTab, LoginTab, AdminTab) portal information will be updated.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	26/08/2004	Removed code to allow multiple tabs with same name.
        ''' 	[VMasanas]	15/10/2004	Modified for new skin structure
        '''		[cnurse]	15/10/2004	Modified to allow for merging template
        '''								with existing pages
        '''     [cnurse]    11/21/2204  modified to use GetNodeValueDate for Start and End Dates
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ParseTab(ByVal nodeTab As XmlNode, ByVal PortalId As Integer, ByVal IsAdminTemplate As Boolean, ByVal mergeTabs As PortalTemplateModuleAction, ByRef hModules As Hashtable, ByRef hTabs As Hashtable, ByVal IsNewPortal As Boolean) As Integer
            Dim objTab As TabInfo = Nothing
            Dim objTabs As New TabController
            Dim strName As String = XmlUtils.GetNodeValue(nodeTab, "name")
            Dim objportal As PortalInfo = GetPortal(PortalId)

            If strName <> "" Then
                If Not IsNewPortal Then ' running from wizard: try to find the tab by path
                    Dim parenttabname As String = ""

                    If XmlUtils.GetNodeValue(nodeTab, "parent") <> "" Then
                        parenttabname = XmlUtils.GetNodeValue(nodeTab, "parent") + "/"
                    End If
                    If Not hTabs(parenttabname + strName) Is Nothing Then
                        objTab = objTabs.GetTab(Convert.ToInt32(hTabs(parenttabname + strName)), PortalId, False)
                    End If
                End If

                If objTab Is Nothing Or IsNewPortal Then
                    objTab = TabController.DeserializeTab(nodeTab, Nothing, hTabs, PortalId, IsAdminTemplate, mergeTabs, hModules)
                End If
                Dim objEventLog As New Services.Log.EventLog.EventLogController

                ' when processing the template we should try and identify the Admin tab
                If objTab.TabName = "Admin" Then
                    objportal.AdminTabId = objTab.TabID
                    UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId))
                    objEventLog.AddLog("AdminTab", objTab.TabID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)
                End If
                ' when processing the template we can find: hometab, usertab, logintab
                Select Case XmlUtils.GetNodeValue(nodeTab, "tabtype", "")
                    Case "splashtab"
                        objportal.SplashTabId = objTab.TabID
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId))
                        objEventLog.AddLog("SplashTab", objTab.TabID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)
                    Case "hometab"
                        objportal.HomeTabId = objTab.TabID
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId))
                        objEventLog.AddLog("HomeTab", objTab.TabID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)
                    Case "logintab"
                        objportal.LoginTabId = objTab.TabID
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId))
                        objEventLog.AddLog("LoginTab", objTab.TabID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)
                    Case "usertab"
                        objportal.UserTabId = objTab.TabID
                        UpdatePortalSetup(PortalId, objportal.AdministratorId, objportal.AdministratorRoleId, objportal.RegisteredRoleId, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, objportal.AdminTabId, PortalController.GetActivePortalLanguage(PortalId))
                        objEventLog.AddLog("UserTab", objTab.TabID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTAL_SETTING_UPDATED)
                End Select
            End If
        End Function

        Private Sub UpdatePortalSetup(ByVal PortalId As Integer, ByVal AdministratorId As Integer, ByVal AdministratorRoleId As Integer, ByVal RegisteredRoleId As Integer, ByVal SplashTabId As Integer, ByVal HomeTabId As Integer, ByVal LoginTabId As Integer, ByVal RegisterTabId As Integer, ByVal UserTabId As Integer, ByVal AdminTabId As Integer, ByVal CultureCode As String)
            DataProvider.Instance().UpdatePortalSetup(PortalId, AdministratorId, AdministratorRoleId, RegisteredRoleId, SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, AdminTabId, CultureCode)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalId", PortalId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED)
            DataCache.ClearPortalCache(PortalId, True)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new portal alias
        ''' </summary>
        ''' <param name="PortalId">Id of the portal</param>
        ''' <param name="PortalAlias">Portal Alias to be created</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    01/11/2005  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddPortalAlias(ByVal PortalId As Integer, ByVal PortalAlias As String)
            Dim objPortalAliasController As New PortalAliasController

            'Check if the Alias exists
            Dim objPortalAliasInfo As PortalAliasInfo = objPortalAliasController.GetPortalAlias(PortalAlias, PortalId)

            'If alias does not exist add new
            If objPortalAliasInfo Is Nothing Then
                objPortalAliasInfo = New PortalAliasInfo
                objPortalAliasInfo.PortalID = PortalId
                objPortalAliasInfo.HTTPAlias = PortalAlias
                objPortalAliasController.AddPortalAlias(objPortalAliasInfo)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new portal.
        ''' </summary>
        ''' <param name="PortalName">Name of the portal to be created</param>
        ''' <param name="FirstName">Portal Administrator's first name</param>
        ''' <param name="LastName">Portal Administrator's last name</param>
        ''' <param name="Username">Portal Administrator's username</param>
        ''' <param name="Password">Portal Administrator's password</param>
        ''' <param name="Email">Portal Administrator's email</param>
        ''' <param name="Description">Description for the new portal</param>
        ''' <param name="KeyWords">KeyWords for the new portal</param>
        ''' <param name="TemplatePath">Path where the templates are stored</param>
        ''' <param name="TemplateFile">Template file</param>
        ''' <param name="PortalAlias">Portal Alias String</param>
        ''' <param name="ServerPath">The Path to the root of the Application</param>
        ''' <param name="ChildPath">The Path to the Child Portal Folder</param>
        ''' <param name="IsChildPortal">True if this is a child portal</param>
        ''' <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        ''' <remarks>
        ''' After the selected portal template is parsed the admin template ("admin.template") will be
        ''' also processed. The admin template should only contain the "Admin" menu since it's the same
        ''' on all portals. The selected portal template can contain a <settings/> node to specify portal
        ''' properties and a <roles/> node to define the roles that will be created on the portal by default.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/08/2004	created (most of this code was moved from SignUp.ascx.vb)
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreatePortal(ByVal PortalName As String, ByVal FirstName As String, ByVal LastName As String, ByVal Username As String, ByVal Password As String, ByVal Email As String, ByVal Description As String, ByVal KeyWords As String, ByVal TemplatePath As String, ByVal TemplateFile As String, ByVal HomeDirectory As String, ByVal PortalAlias As String, ByVal ServerPath As String, ByVal ChildPath As String, ByVal IsChildPortal As Boolean) As Integer
            Dim objAdminUser As New UserInfo
            objAdminUser.FirstName = FirstName
            objAdminUser.LastName = LastName
            objAdminUser.Username = Username
            objAdminUser.DisplayName = FirstName + " " + LastName
            objAdminUser.Membership.Password = Password
            objAdminUser.Email = Email
            objAdminUser.IsSuperUser = False
            objAdminUser.Membership.Approved = True

            objAdminUser.Profile.FirstName = FirstName
            objAdminUser.Profile.LastName = LastName

            Return CreatePortal(PortalName, objAdminUser, Description, KeyWords, TemplatePath, TemplateFile, HomeDirectory, PortalAlias, ServerPath, ChildPath, IsChildPortal)
        End Function

        Public Sub CopyPageTemplate(ByVal templateFile As String, ByVal MappedHomeDirectory As String)
            Dim strHostTemplateFile As String = String.Format("{0}Templates\{1}", HostMapPath, templateFile)
            If File.Exists(strHostTemplateFile) Then
                Dim strPortalTemplateFolder As String = String.Format("{0}Templates\", MappedHomeDirectory)
                If Not Directory.Exists(strPortalTemplateFolder) Then
                    'Create Portal Templates folder
                    Directory.CreateDirectory(strPortalTemplateFolder)
                End If
                Dim strPortalTemplateFile As String = strPortalTemplateFolder + templateFile
                If Not File.Exists(strPortalTemplateFile) Then
                    File.Copy(strHostTemplateFile, strPortalTemplateFile)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new portal.
        ''' </summary>
        ''' <param name="PortalName">Name of the portal to be created</param>
        ''' <param name="objAdminUser">Portal Administrator</param>
        ''' <param name="Description">Description for the new portal</param>
        ''' <param name="KeyWords">KeyWords for the new portal</param>
        ''' <param name="TemplatePath">Path where the templates are stored</param>
        ''' <param name="TemplateFile">Template file</param>
        ''' <param name="PortalAlias">Portal Alias String</param>
        ''' <param name="ServerPath">The Path to the root of the Application</param>
        ''' <param name="ChildPath">The Path to the Child Portal Folder</param>
        ''' <param name="IsChildPortal">True if this is a child portal</param>
        ''' <returns>PortalId of the new portal if there are no errors, -1 otherwise.</returns>
        ''' <remarks>
        ''' After the selected portal template is parsed the admin template ("admin.template") will be
        ''' also processed. The admin template should only contain the "Admin" menu since it's the same
        ''' on all portals. The selected portal template can contain a <settings/> node to specify portal
        ''' properties and a <roles/> node to define the roles that will be created on the portal by default.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreatePortal(ByVal PortalName As String, ByVal objAdminUser As UserInfo, ByVal Description As String, ByVal KeyWords As String, ByVal TemplatePath As String, ByVal TemplateFile As String, ByVal HomeDirectory As String, ByVal PortalAlias As String, ByVal ServerPath As String, ByVal ChildPath As String, ByVal IsChildPortal As Boolean) As Integer
            Dim objFolderController As New Services.FileSystem.FolderController
            Dim strMessage As String = Null.NullString
            Dim AdministratorId As Integer = Null.NullInteger

            'Attempt to create a new portal
            Dim intPortalId As Integer = CreatePortal(PortalName, HomeDirectory)

            If intPortalId <> -1 Then
                If HomeDirectory = "" Then
                    HomeDirectory = "Portals/" + intPortalId.ToString
                End If
                Dim MappedHomeDirectory As String = objFolderController.GetMappedDirectory(Common.Globals.ApplicationPath + "/" + HomeDirectory + "/")

                strMessage += CreateProfileDefinitions(intPortalId, TemplatePath, TemplateFile)
                If strMessage = Null.NullString Then
                    ' add administrator
                    Try
                        objAdminUser.PortalID = intPortalId
                        Dim createStatus As UserCreateStatus = UserController.CreateUser(objAdminUser)
                        If createStatus = UserCreateStatus.Success Then
                            AdministratorId = objAdminUser.UserID
                        Else
                            strMessage += UserController.GetUserCreateStatus(createStatus)
                        End If
                    Catch Exc As Exception
                        strMessage += Localization.GetString("CreateAdminUser.Error") + Exc.Message + Exc.StackTrace
                    End Try
                Else
                    Throw New Exception(strMessage)
                End If

                If strMessage = "" And AdministratorId > 0 Then
                    Try
                        ' the upload directory may already exist if this is a new DB working with a previously installed application
                        If Directory.Exists(MappedHomeDirectory) Then
                            DeleteFolderRecursive(MappedHomeDirectory)
                        End If
                    Catch Exc As Exception
                        strMessage += Localization.GetString("DeleteUploadFolder.Error") + Exc.Message + Exc.StackTrace
                    End Try

                    'Set up Child Portal
                    If strMessage = Null.NullString Then
                        Try
                            If IsChildPortal Then
                                ' create the subdirectory for the new portal
                                If Not Directory.Exists(ChildPath) Then
                                    System.IO.Directory.CreateDirectory(ChildPath)
                                End If

                                ' create the subhost default.aspx file
                                If Not File.Exists(ChildPath & "\" & glbDefaultPage) Then
                                    System.IO.File.Copy(Common.Globals.HostMapPath & "subhost.aspx", ChildPath & "\" & glbDefaultPage)
                                End If
                            End If
                        Catch Exc As Exception
                            strMessage += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace
                        End Try
                    Else
                        Throw New Exception(strMessage)
                    End If

                    If strMessage = Null.NullString Then
                        Try
                            ' create the upload directory for the new portal
                            System.IO.Directory.CreateDirectory(MappedHomeDirectory)

                            'ensure that the Templates folder exists
                            Dim templateFolder As String = String.Format("{0}Templates", MappedHomeDirectory)
                            If Not Directory.Exists(templateFolder) Then
                                System.IO.Directory.CreateDirectory(templateFolder)
                            End If

                            'ensure that the Users folder exists
                            Dim usersFolder As String = String.Format("{0}Users", MappedHomeDirectory)
                            If Not Directory.Exists(usersFolder) Then
                                System.IO.Directory.CreateDirectory(usersFolder)
                            End If

                            'copy the default page template
                            CopyPageTemplate("Default.page.template", MappedHomeDirectory)

                            ' process zip resource file if present
                            ProcessResourceFile(MappedHomeDirectory, TemplatePath & TemplateFile)
                        Catch Exc As Exception
                            strMessage += Localization.GetString("ChildPortal.Error") + Exc.Message + Exc.StackTrace
                        End Try
                    Else
                        Throw New Exception(strMessage)
                    End If

                    If strMessage = Null.NullString Then
                        ' parse portal template
                        Try
                            ParseTemplate(intPortalId, TemplatePath, TemplateFile, AdministratorId, PortalTemplateModuleAction.Replace, True)
                        Catch Exc As Exception
                            strMessage += Localization.GetString("PortalTemplate.Error") + Exc.Message + Exc.StackTrace
                        End Try
                    Else
                        Throw New Exception(strMessage)
                    End If

                    If strMessage = Null.NullString Then
                        ' update portal setup
                        Dim objportal As PortalInfo = GetPortal(intPortalId)

                        ' update portal info
                        objportal.Description = Description
                        objportal.KeyWords = KeyWords
                        objportal.UserTabId = TabController.GetTabByTabPath(objportal.PortalID, "//UserProfile", objportal.CultureCode)
                        UpdatePortalInfo(objportal.PortalID, objportal.PortalName, objportal.LogoFile, objportal.FooterText, _
                         objportal.ExpiryDate, objportal.UserRegistration, objportal.BannerAdvertising, objportal.Currency, objportal.AdministratorId, objportal.HostFee, _
                         objportal.HostSpace, objportal.PageQuota, objportal.UserQuota, objportal.PaymentProcessor, objportal.ProcessorUserId, objportal.ProcessorPassword, objportal.Description, _
                         objportal.KeyWords, objportal.BackgroundFile, objportal.SiteLogHistory, objportal.SplashTabId, objportal.HomeTabId, objportal.LoginTabId, objportal.RegisterTabId, objportal.UserTabId, _
                         objportal.DefaultLanguage, objportal.TimeZoneOffset, objportal.HomeDirectory)

                        'Update Administrators Locale/TimeZone
                        objAdminUser.Profile.PreferredLocale = objportal.DefaultLanguage
                        objAdminUser.Profile.TimeZone = objportal.TimeZoneOffset

                        'Save Admin User
                        UserController.UpdateUser(objportal.PortalID, objAdminUser)

                        'Add DesktopModules to Portal
                        DesktopModuleController.AddDesktopModulesToPortal(intPortalId)

                        'Add Languages to Portal
                        Localization.AddLanguagesToPortal(intPortalId)

                        'Create Portal Alias
                        AddPortalAlias(intPortalId, PortalAlias)

                        ' log installation event
                        Try
                            Dim objEventLogInfo As New Services.Log.EventLog.LogInfo
                            objEventLogInfo.BypassBuffering = True
                            objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Install Portal:", PortalName))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("FirstName:", objAdminUser.FirstName))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("LastName:", objAdminUser.LastName))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Username:", objAdminUser.Username))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Email:", objAdminUser.Email))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Description:", Description))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("Keywords:", KeyWords))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("TemplatePath:", TemplatePath))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("TemplateFile:", TemplateFile))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("HomeDirectory:", HomeDirectory))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("PortalAlias:", PortalAlias))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ServerPath:", ServerPath))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("ChildPath:", ChildPath))
                            objEventLogInfo.LogProperties.Add(New DotNetNuke.Services.Log.EventLog.LogDetailInfo("IsChildPortal:", IsChildPortal.ToString()))
                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog(objEventLogInfo)
                        Catch ex As Exception
                            ' error
                        End Try
                    Else
                        Throw New Exception(strMessage)
                    End If
                Else    ' clean up
                    DeletePortalInfo(intPortalId)
                    intPortalId = -1
                    Throw New Exception(strMessage)
                End If
            Else
                strMessage += Localization.GetString("CreatePortal.Error")
                Throw New Exception(strMessage)
            End If

            Return intPortalId
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Deletes a portal permanently
        ''' </summary>
        ''' <param name="PortalId">PortalId of the portal to be deleted</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	03/09/2004	Created
        ''' 	[VMasanas]	26/10/2004	Remove dependent data (skins, modules)
        '''     [cnurse]    24/11/2006  Removal of Modules moved to sproc
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeletePortalInfo(ByVal PortalId As Integer)
            'delete portal users
            UserController.DeleteUsers(PortalId, False, True)

            'delete portal
            DataProvider.Instance().DeletePortalInfo(PortalId)

            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalId", PortalId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALINFO_DELETED)

            ' clear portal alias cache and entire portal
            DataCache.ClearHostCache(True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets information of a portal
        ''' </summary>
        ''' <param name="PortalId">Id of the portal</param>
        ''' <returns>PortalInfo object with portal definition</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPortal(ByVal PortalId As Integer) As PortalInfo
            Dim defaultLanguage As String = PortalController.GetActivePortalLanguage(PortalId)
            Return GetPortal(PortalId, defaultLanguage)
        End Function

        Public Function GetPortal(ByVal PortalId As Integer, ByVal CultureCode As String) As PortalInfo
            Dim cacheKey As String = String.Format(DataCache.PortalCacheKey, PortalId.ToString(), CultureCode)
            Return CBO.GetCachedObject(Of PortalInfo)(New CacheItemArgs(cacheKey, DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, PortalId, CultureCode), _
                                                                                        AddressOf GetPortalCallback)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets information from all portals
        ''' </summary>
        ''' <returns>ArrayList of PortalInfo objects</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPortals() As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().GetPortals(), GetType(PortalInfo))
        End Function

        Public Function GetPortal(ByVal uniqueId As Guid) As PortalInfo
            Dim portals As ArrayList = GetPortals()
            Dim targetPortal As PortalInfo

            For Each currentPortal As PortalInfo In portals
                If currentPortal.GUID = uniqueId Then
                    targetPortal = currentPortal
                    Exit For
                End If
            Next
            Return targetPortal
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the space used at the host level
        ''' </summary>
        ''' <returns>Space used in bytes</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	19/04/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPortalSpaceUsedBytes() As Long
            Return GetPortalSpaceUsedBytes(-1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the space used by a portal in bytes
        ''' </summary>
        ''' <param name="PortalId">Id of the portal</param>
        ''' <returns>Space used in bytes</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	07/04/2006	Created
        '''     [VMasanas]  11/05/2006  Use file size stored on the db. This is necessary
        '''         to take into account the new secure file storage options
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetPortalSpaceUsedBytes(ByVal portalId As Integer) As Long
            Dim size As Long = 0

            Dim dr As IDataReader = Nothing
            dr = DataProvider.Instance().GetPortalSpaceUsed(portalId)
            Try
                If dr.Read Then
                    If Not IsDBNull(dr("SpaceUsed")) Then
                        size = Convert.ToInt64(dr("SpaceUsed"))
                    End If
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try

            Return size
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Verifies if there's enough space to upload a new file on the given portal
        ''' </summary>
        ''' <param name="PortalId">Id of the portal</param>
        ''' <param name="fileSizeBytes">Size of the file being uploaded</param>
        ''' <returns>True if there's enough space available to upload the file</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	19/04/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function HasSpaceAvailable(ByVal portalId As Integer, ByVal fileSizeBytes As Long) As Boolean

            Dim hostSpace As Integer

            If portalId = -1 Then
                hostSpace = 0
            Else
                Dim ps As PortalSettings = GetCurrentPortalSettings()
                If Not ps Is Nothing AndAlso ps.PortalId = portalId Then
                    hostSpace = ps.HostSpace
                Else
                    Dim portal As PortalInfo = GetPortal(portalId)
                    hostSpace = portal.HostSpace
                End If
            End If

            Return (((GetPortalSpaceUsedBytes(portalId) + fileSizeBytes) / 1024 ^ 2) <= hostSpace) Or hostSpace = 0

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processess a template file for the new portal. This method will be called twice: for the portal template and for the admin template
        ''' </summary>
        ''' <param name="PortalId">PortalId of the new portal</param>
        ''' <param name="TemplatePath">Path for the folder where templates are stored</param>
        ''' <param name="TemplateFile">Template file to process</param>
        ''' <param name="AdministratorId">UserId for the portal administrator. This is used to assign roles to this user</param>
        ''' <param name="mergeTabs">Flag to determine whether Module content is merged.</param>
        ''' <param name="IsNewPortal">Flag to determine is the template is applied to an existing portal or a new one.</param>
        ''' <remarks>
        ''' The roles and settings nodes will only be processed on the portal template file.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	27/08/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ParseTemplate(ByVal PortalId As Integer, ByVal TemplatePath As String, ByVal TemplateFile As String, ByVal AdministratorId As Integer, ByVal mergeTabs As PortalTemplateModuleAction, ByVal IsNewPortal As Boolean)
            Dim xmlPortal As New XmlDocument
            Dim node As XmlNode
            Dim objFolder As FolderInfo

            ' open the XML file
            Try
                xmlPortal.Load(TemplatePath & TemplateFile)
            Catch    ' error
            End Try

            ' parse portal settings if available only for new portals
            node = xmlPortal.SelectSingleNode("//portal/settings")
            If Not node Is Nothing And IsNewPortal Then
                ParsePortalSettings(node, PortalId)
            End If

            ' parse role groups if available (version 5.0 templates)
            node = xmlPortal.SelectSingleNode("//portal/rolegroups")
            If node IsNot Nothing Then
                ParseRoleGroups(node.CreateNavigator(), PortalId, AdministratorId)
            End If

            ' parse roles if available (version 3.0 templates)
            node = xmlPortal.SelectSingleNode("//portal/roles")
            If node IsNot Nothing Then
                ParseRoles(node.CreateNavigator(), PortalId, AdministratorId)
            End If

            ' parse portal desktop modules (version 5.0 templates)
            node = xmlPortal.SelectSingleNode("//portal/portalDesktopModules")
            If node IsNot Nothing Then
                ParsePortalDesktopModules(node.CreateNavigator(), PortalId)
            End If

            ' parse portal folders
            node = xmlPortal.SelectSingleNode("//portal/folders")
            If Not node Is Nothing Then
                ParseFolders(node, PortalId)
            End If

            ' force creation of root folder if not present on template
            Dim objController As New FolderController
            If objController.GetFolder(PortalId, "", False) Is Nothing Then
                objFolder = New FolderInfo(PortalId, "", Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, True, False, Null.NullDate)
                Dim folderid As Integer = objController.AddFolder(objFolder)
                AddFolderPermissions(PortalId, folderid)
            End If

            ' force creation of templates folder if not present on template
            If objController.GetFolder(PortalId, "Templates/", False) Is Nothing Then
                objFolder = New FolderInfo(PortalId, "Templates/", Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, True, False, Null.NullDate)
                Dim folderid As Integer = objController.AddFolder(objFolder)
                AddFolderPermissions(PortalId, folderid)
            End If

            ' force creation of templates folder if not present on template
            If objController.GetFolder(PortalId, "Users/", False) Is Nothing Then
                objFolder = New FolderInfo(PortalId, "Users/", Services.FileSystem.FolderController.StorageLocationTypes.InsecureFileSystem, True, False, Null.NullDate)
                Dim folderid As Integer = objController.AddFolder(objFolder)
                AddFolderPermissions(PortalId, folderid)
            End If

            'Remove Exising Tabs if doing a "Replace"
            If mergeTabs = PortalTemplateModuleAction.Replace Then
                Dim objTabs As New TabController
                Dim objTab As TabInfo
                For Each tabPair As KeyValuePair(Of Integer, TabInfo) In objTabs.GetTabsByPortal(PortalId)
                    objTab = tabPair.Value
                    'soft delete Tab
                    objTab.TabName = objTab.TabName & "_old"
                    objTab.TabPath = GenerateTabPath(objTab.ParentId, objTab.TabName)
                    objTab.IsDeleted = True
                    objTabs.UpdateTab(objTab)
                    'Delete all Modules
                    Dim objModules As New ModuleController
                    Dim objModule As ModuleInfo
                    For Each modulePair As KeyValuePair(Of Integer, ModuleInfo) In objModules.GetTabModules(objTab.TabID)
                        objModule = modulePair.Value
                        objModules.DeleteTabModule(objModule.TabID, objModule.ModuleID, False)
                    Next
                Next
            End If

            ' parse portal tabs
            node = xmlPortal.SelectSingleNode("//portal/tabs")
            If Not node Is Nothing Then
                Dim version As String = xmlPortal.DocumentElement.GetAttribute("version")

                If version <> "5.0" Then
                    Dim xmlAdmin As New XmlDocument

                    ' open the XML file
                    Try
                        xmlAdmin.Load(TemplatePath & "admin.template")
                    Catch    ' error
                    End Try

                    Dim adminNode As XmlNode = xmlAdmin.SelectSingleNode("//portal/tabs")

                    'Add Admin Nodes to portal Template
                    For Each adminTabNode As XmlNode In adminNode.ChildNodes
                        node.AppendChild(xmlPortal.ImportNode(adminTabNode, True))
                    Next
                End If
                ParseTabs(node, PortalId, False, mergeTabs, IsNewPortal)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Processes the resource file for the template file selected
        ''' </summary>
        ''' <param name="portalPath">New portal's folder</param>
        ''' <param name="TemplateFile">Selected template file</param>
        ''' <remarks>
        ''' The resource file is a zip file with the same name as the selected template file and with
        ''' an extension of .resources (to unable this file being downloaded).
        ''' For example: for template file "portal.template" a resource file "portal.template.resources" can be defined.
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	10/09/2004	Created
        '''     [cnurse]    11/08/2004  Moved from SignUp to PortalController
        '''     [cnurse]    03/04/2005  made Public
        '''     [cnurse]    05/20/2005  moved most of processing to new method in FileSystemUtils
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ProcessResourceFile(ByVal portalPath As String, ByVal TemplateFile As String)

            Dim objZipInputStream As ZipInputStream
            Try
                objZipInputStream = New ZipInputStream(New FileStream(TemplateFile & ".resources", FileMode.Open, FileAccess.Read))
                FileSystemUtils.UnzipResources(objZipInputStream, portalPath)
            Catch exc As Exception
                ' error opening file
            End Try

        End Sub

        Public Sub UpdatePortalExpiry(ByVal PortalId As Integer)
            UpdatePortalExpiry(PortalId, PortalController.GetActivePortalLanguage(PortalId))
        End Sub

        Public Sub UpdatePortalExpiry(ByVal PortalId As Integer, ByVal CultureCode As String)

            Dim ExpiryDate As DateTime

            Dim dr As IDataReader = Nothing
            Try
                dr = DataProvider.Instance().GetPortal(PortalId, CultureCode)
                If dr.Read Then
                    If Not IsDBNull(dr("ExpiryDate")) Then
                        ExpiryDate = Convert.ToDateTime(dr("ExpiryDate"))
                    Else
                        ExpiryDate = Now()
                    End If

                    DataProvider.Instance().UpdatePortalInfo(PortalId, Convert.ToString(dr("PortalName")), Convert.ToString(dr("LogoFile")), Convert.ToString(dr("FooterText")), DateAdd(DateInterval.Month, 1, ExpiryDate), Convert.ToInt32(dr("UserRegistration")), Convert.ToInt32(dr("BannerAdvertising")), Convert.ToString(dr("Currency")), Convert.ToInt32(dr("AdministratorId")), Convert.ToDouble(dr("HostFee")), Convert.ToDouble(dr("HostSpace")), Convert.ToInt32(dr("PageQuota")), Convert.ToInt32(dr("UserQuota")), Convert.ToString(dr("PaymentProcessor")), Convert.ToString(dr("ProcessorUserId")), Convert.ToString(dr("ProcessorPassword")), Convert.ToString(dr("Description")), Convert.ToString(dr("KeyWords")), Convert.ToString(dr("BackgroundFile")), Convert.ToInt32(dr("SiteLogHistory")), Convert.ToInt32(dr("SplashTabId")), Convert.ToInt32(dr("HomeTabId")), Convert.ToInt32(dr("LoginTabId")), Convert.ToInt32(dr("RegisterTabId")), Convert.ToInt32(dr("UserTabId")), Convert.ToString(dr("DefaultLanguage")), Convert.ToInt32(dr("TimeZoneOffset")), Convert.ToString(dr("HomeDirectory")), UserController.GetCurrentUserInfo.UserID, CultureCode)
                    'as all other changes are maintained, only log the altered expirydate
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("ExpiryDate", ExpiryDate.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED)
                End If
            Catch ex As Exception
                LogException(ex)
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates basic portal information
        ''' </summary>
        ''' <param name="Portal"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/13/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdatePortalInfo(ByVal Portal As PortalInfo)

            UpdatePortalInfo(Portal.PortalID, Portal.PortalName, _
             Portal.LogoFile, Portal.FooterText, Portal.ExpiryDate, Portal.UserRegistration, _
             Portal.BannerAdvertising, Portal.Currency, Portal.AdministratorId, _
             Portal.HostFee, Portal.HostSpace, Portal.PageQuota, Portal.UserQuota, Portal.PaymentProcessor, Portal.ProcessorUserId, _
             Portal.ProcessorPassword, Portal.Description, Portal.KeyWords, _
             Portal.BackgroundFile, Portal.SiteLogHistory, Portal.SplashTabId, Portal.HomeTabId, _
             Portal.LoginTabId, Portal.RegisterTabId, Portal.UserTabId, Portal.DefaultLanguage, _
             Portal.TimeZoneOffset, Portal.HomeDirectory, PortalController.GetActivePortalLanguage(Portal.PortalID))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates basic portal information
        ''' </summary>
        ''' <param name="PortalId"></param>
        ''' <param name="PortalName"></param>
        ''' <param name="LogoFile"></param>
        ''' <param name="FooterText"></param>
        ''' <param name="ExpiryDate"></param>
        ''' <param name="UserRegistration"></param>
        ''' <param name="BannerAdvertising"></param>
        ''' <param name="Currency"></param>
        ''' <param name="AdministratorId"></param>
        ''' <param name="HostFee"></param>
        ''' <param name="HostSpace"></param>
        ''' <param name="PaymentProcessor"></param>
        ''' <param name="ProcessorUserId"></param>
        ''' <param name="ProcessorPassword"></param>
        ''' <param name="Description"></param>
        ''' <param name="KeyWords"></param>
        ''' <param name="BackgroundFile"></param>
        ''' <param name="SiteLogHistory"></param>
        ''' <param name="HomeTabId"></param>
        ''' <param name="LoginTabId"></param>
        ''' <param name="UserTabId"></param>
        ''' <param name="DefaultLanguage"></param>
        ''' <param name="TimeZoneOffset"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdatePortalInfo(ByVal PortalId As Integer, ByVal PortalName As String, ByVal LogoFile As String, ByVal FooterText As String, ByVal ExpiryDate As Date, ByVal UserRegistration As Integer, ByVal BannerAdvertising As Integer, ByVal Currency As String, ByVal AdministratorId As Integer, ByVal HostFee As Double, ByVal HostSpace As Double, ByVal PageQuota As Integer, ByVal UserQuota As Integer, ByVal PaymentProcessor As String, ByVal ProcessorUserId As String, ByVal ProcessorPassword As String, ByVal Description As String, ByVal KeyWords As String, ByVal BackgroundFile As String, ByVal SiteLogHistory As Integer, ByVal SplashTabId As Integer, ByVal HomeTabId As Integer, ByVal LoginTabId As Integer, ByVal RegisterTabId As Integer, ByVal UserTabId As Integer, ByVal DefaultLanguage As String, ByVal TimeZoneOffset As Integer, ByVal HomeDirectory As String)
            UpdatePortalInfo(PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee, HostSpace, PageQuota, UserQuota, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory, SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory, PortalController.GetActivePortalLanguage(PortalId))
        End Sub

        Public Sub UpdatePortalInfo(ByVal PortalId As Integer, ByVal PortalName As String, ByVal LogoFile As String, ByVal FooterText As String, ByVal ExpiryDate As Date, ByVal UserRegistration As Integer, ByVal BannerAdvertising As Integer, ByVal Currency As String, ByVal AdministratorId As Integer, ByVal HostFee As Double, ByVal HostSpace As Double, ByVal PageQuota As Integer, ByVal UserQuota As Integer, ByVal PaymentProcessor As String, ByVal ProcessorUserId As String, ByVal ProcessorPassword As String, ByVal Description As String, ByVal KeyWords As String, ByVal BackgroundFile As String, ByVal SiteLogHistory As Integer, ByVal SplashTabId As Integer, ByVal HomeTabId As Integer, ByVal LoginTabId As Integer, ByVal RegisterTabId As Integer, ByVal UserTabId As Integer, ByVal DefaultLanguage As String, ByVal TimeZoneOffset As Integer, ByVal HomeDirectory As String, ByVal CultureCode As String)
            DataProvider.Instance().UpdatePortalInfo(PortalId, PortalName, LogoFile, FooterText, ExpiryDate, UserRegistration, BannerAdvertising, Currency, AdministratorId, HostFee, HostSpace, PageQuota, UserQuota, PaymentProcessor, ProcessorUserId, ProcessorPassword, Description, KeyWords, BackgroundFile, SiteLogHistory, SplashTabId, HomeTabId, LoginTabId, RegisterTabId, UserTabId, DefaultLanguage, TimeZoneOffset, HomeDirectory, UserController.GetCurrentUserInfo.UserID, CultureCode)

            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalId", PortalId.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALINFO_UPDATED)
            'ensure a localization item exists (in case a new default language has been set)
            DataProvider.Instance().EnsureLocalizationExists(PortalId, DefaultLanguage)
            ' clear portal cache
            DataCache.ClearPortalCache(PortalId, False)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("This function has been replaced by GetPortalSpaceUsedBytes")> _
        Public Function GetPortalSpaceUsed(Optional ByVal portalId As Integer = -1) As Integer
            Dim size As Integer
            Try
                size = Convert.ToInt32(GetPortalSpaceUsedBytes(portalId))
            Catch ex As Exception
                size = Integer.MaxValue
            End Try

            Return size
        End Function

        <Obsolete("This function has been replaced by TabController.DeserializePanes")> _
        Public Sub ParsePanes(ByVal nodePanes As XmlNode, ByVal PortalId As Integer, ByVal TabId As Integer, ByVal mergeTabs As PortalTemplateModuleAction, ByVal hModules As Hashtable)
            TabController.DeserializePanes(nodePanes, PortalId, TabId, mergeTabs, hModules)
        End Sub

#End Region

    End Class

End Namespace
