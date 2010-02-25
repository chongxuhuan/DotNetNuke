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
Imports System.Web.Caching
Imports System.Diagnostics

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Host
Imports System.Threading

Namespace DotNetNuke.Common.Utilities

    Public Enum CoreCacheType
        Host = 1
        Portal = 2
        Tab = 3
    End Enum

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Common.Utilities
    ''' Class:      DataCache
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DataCache class is a facade class for the CachingProvider Instance's
    ''' </summary>
    ''' <history>
    '''     [cnurse]	12/01/2007	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DataCache

#Region "Private Shared Members"

        Private Shared _CachePersistenceEnabled As String = ""

        'Object used to lock SyncLock block
        Private Shared dictionaryLock As New ReaderWriterLock
        Private Shared lockDictionary As New Dictionary(Of String, Object)

#End Region

#Region "Public Constants"

        'Host keys
        Public Const SecureHostSettingsCacheKey As String = "SecureHostSettings"
        Public Const HostSettingsCacheKey As String = "HostSettings"
        Public Const HostSettingsCachePriority As CacheItemPriority = CacheItemPriority.NotRemovable
        Public Const HostSettingsCacheTimeOut As Integer = 20

        'Portal keys
        Public Const PortalAliasCacheKey As String = "PortalAlias"
        Public Const PortalAliasCachePriority As CacheItemPriority = CacheItemPriority.NotRemovable
        Public Const PortalAliasCacheTimeOut As Integer = 200

        Public Const PortalSettingsCacheKey As String = "PortalSettings{0}"
        Public Const PortalSettingsCachePriority As CacheItemPriority = CacheItemPriority.NotRemovable
        Public Const PortalSettingsCacheTimeOut As Integer = 20

        Public Const PortalDictionaryCacheKey As String = "PortalDictionary"
        Public Const PortalDictionaryCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const PortalDictionaryTimeOut As Integer = 20

        Public Const PortalCacheKey As String = "Portal{0}"
        Public Const PortalCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const PortalCacheTimeOut As Integer = 20

        'Tab cache keys
        Public Const TabCacheKey As String = "Tab_Tabs{0}"
        Public Const TabCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const TabCacheTimeOut As Integer = 20
        Public Const TabPathCacheKey As String = "Tab_TabPathDictionary{0}"
        Public Const TabPathCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const TabPathCacheTimeOut As Integer = 20
        Public Const TabPermissionCacheKey As String = "Tab_TabPermissions{0}"
        Public Const TabPermissionCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const TabPermissionCacheTimeOut As Integer = 20

        Public Const AuthenticationServicesCacheKey As String = "AuthenticationServices"
        Public Const AuthenticationServicesCachePriority As CacheItemPriority = CacheItemPriority.NotRemovable
        Public Const AuthenticationServicesCacheTimeOut As Integer = 20

        Public Const DesktopModulePermissionCacheKey As String = "DesktopModulePermissions"
        Public Const DesktopModulePermissionCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const DesktopModulePermissionCacheTimeOut As Integer = 20

        Public Const DesktopModuleCacheKey As String = "DesktopModulesByPortal{0}"
        Public Const DesktopModuleCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const DesktopModuleCacheTimeOut As Integer = 20

        Public Const PortalDesktopModuleCacheKey As String = "PortalDesktopModules{0}"
        Public Const PortalDesktopModuleCachePriority As CacheItemPriority = CacheItemPriority.AboveNormal
        Public Const PortalDesktopModuleCacheTimeOut As Integer = 20

        Public Const ModuleDefinitionCacheKey As String = "ModuleDefinitions"
        Public Const ModuleDefinitionCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const ModuleDefinitionCacheTimeOut As Integer = 20

        Public Const ModuleControlsCacheKey As String = "ModuleControls"
        Public Const ModuleControlsCachePriority As CacheItemPriority = CacheItemPriority.High
        Public Const ModuleControlsCacheTimeOut As Integer = 20

        Public Const TabModuleCacheKey As String = "TabModules{0}"
        Public Const TabModuleCachePriority As CacheItemPriority = CacheItemPriority.AboveNormal
        Public Const TabModuleCacheTimeOut As Integer = 20

        Public Const ModulePermissionCacheKey As String = "ModulePermissions{0}"
        Public Const ModulePermissionCachePriority As CacheItemPriority = CacheItemPriority.AboveNormal
        Public Const ModulePermissionCacheTimeOut As Integer = 20

        Public Const ModuleCacheKey As String = "Modules{0}"
        Public Const ModuleCacheTimeOut As Integer = 20

        Public Const FolderCacheKey As String = "Folders{0}"
        Public Const FolderCacheTimeOut As Integer = 20
        Public Const FolderCachePriority As CacheItemPriority = CacheItemPriority.Normal

        Public Const FolderPermissionCacheKey As String = "FolderPermissions{0}"
        Public Const FolderPermissionCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const FolderPermissionCacheTimeOut As Integer = 20

        Public Const ListsCacheKey As String = "Lists{0}"
        Public Const ListsCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const ListsCacheTimeOut As Integer = 20

        Public Const ProfileDefinitionsCacheKey As String = "ProfileDefinitions{0}"
        Public Const ProfileDefinitionsCacheTimeOut As Integer = 20

        Public Const UserCacheKey As String = "UserInfo|{0}|{1}"
        Public Const UserCacheTimeOut As Integer = 1
        Public Const UserCachePriority As CacheItemPriority = CacheItemPriority.Normal

        Public Const LocalesCacheKey As String = "Locales{0}"
        Public Const LocalesCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const LocalesCacheTimeOut As Integer = 20

        Public Const SkinDefaultsCacheKey As String = "SkinDefaults_{0}"
        Public Const SkinDefaultsCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const SkinDefaultsCacheTimeOut As Integer = 20

        Public Const ResourceFilesCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const ResourceFilesCacheTimeOut As Integer = 20

        Public Const ResourceFileLookupDictionaryCacheKey As String = "ResourceFileLookupDictionary"
        Public Const ResourceFileLookupDictionaryCachePriority As CacheItemPriority = CacheItemPriority.NotRemovable
        Public Const ResourceFileLookupDictionaryTimeOut As Integer = 200

        Public Const SkinsCacheKey As String = "GetSkins{0}"

        Public Const BannersCacheKey As String = "Banners:{0}:{1}:{2}"
        Public Const BannersCachePriority As CacheItemPriority = CacheItemPriority.Normal
        Public Const BannersCacheTimeOut As Integer = 20

#End Region

#Region "Public Shared Properties"

        Public Shared ReadOnly Property CachePersistenceEnabled() As Boolean
            Get
                If String.IsNullOrEmpty(_CachePersistenceEnabled) Then
                    If Config.GetSetting("EnableCachePersistence") Is Nothing Then
                        _CachePersistenceEnabled = "false"
                    Else
                        _CachePersistenceEnabled = Config.GetSetting("EnableCachePersistence")
                    End If
                End If
                Return Boolean.Parse(_CachePersistenceEnabled)
            End Get
        End Property

#End Region

#Region "Private Shared Methods"

        Private Shared Function GetDnnCacheKey(ByVal CacheKey As String) As String
            Return CachingProvider.GetCacheKey(CacheKey)
        End Function

#End Region

#Region "Friend Methods"

        Friend Shared Sub ItemRemovedCallback(ByVal key As String, ByVal value As Object, ByVal removedReason As CacheItemRemovedReason)

            ' if the item was removed from the cache, log the key and reason to the event log
            Try
                If Globals.Status = Globals.UpgradeStatus.None Then
                    Dim objEventLogInfo As New LogInfo
                    Select Case removedReason
                        Case CacheItemRemovedReason.Removed
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_REMOVED.ToString()
                        Case CacheItemRemovedReason.Expired
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_EXPIRED.ToString()
                        Case CacheItemRemovedReason.Underused
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_UNDERUSED.ToString()
                        Case CacheItemRemovedReason.DependencyChanged
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_DEPENDENCYCHANGED.ToString()
                    End Select
                    objEventLogInfo.LogProperties.Add(New LogDetailInfo(key, removedReason.ToString))
                    Dim objEventLog As New EventLogController()
                    objEventLog.AddLog(objEventLogInfo)
                End If
            Catch ex As Exception
                'Swallow exception
            End Try

        End Sub

#End Region

#Region "Public Shared Methods"

#Region "Clear Cache Methods"

#Region "Clear Groups of Keys"

        Public Shared Sub ClearCache()
            CachingProvider.Instance().Clear("Prefix", "DNN_")

            ' log the cache clear event
            Dim objEventLogInfo As New LogInfo
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_REFRESH.ToString()
            objEventLogInfo.LogProperties.Add(New LogDetailInfo("*", "Refresh"))
            Dim objEventLog As New EventLogController()
            objEventLog.AddLog(objEventLogInfo)
        End Sub

        Public Shared Sub ClearCache(ByVal cachePrefix As String)
            CachingProvider.Instance().Clear("Prefix", GetDnnCacheKey(cachePrefix))
        End Sub

        Public Shared Sub ClearFolderCache(ByVal PortalId As Integer)
            CachingProvider.Instance().Clear("Folder", PortalId.ToString)
        End Sub

        Public Shared Sub ClearHostCache(ByVal Cascade As Boolean)
            If Cascade Then
                ClearCache()
            Else
                CachingProvider.Instance().Clear("Host", "")
            End If
        End Sub

        Public Shared Sub ClearModuleCache(ByVal TabId As Integer)
            CachingProvider.Instance().Clear("Module", TabId.ToString)
            Dim portals As Dictionary(Of Integer, Integer) = PortalController.GetPortalDictionary
            If portals.ContainsKey(TabId) Then
                OutputCache.OutputCachingProvider.RemoveItemFromAllProviders(TabId)
            End If
        End Sub

        Public Shared Sub ClearModulePermissionsCachesByPortal(ByVal PortalId As Integer)
            CachingProvider.Instance().Clear("ModulePermissionsByPortal", PortalId.ToString)
        End Sub

        Public Shared Sub ClearPortalCache(ByVal PortalId As Integer, ByVal Cascade As Boolean)
            If Cascade Then
                CachingProvider.Instance().Clear("PortalCascade", PortalId.ToString)
            Else
                CachingProvider.Instance().Clear("Portal", PortalId.ToString)
            End If
        End Sub

        Public Shared Sub ClearTabsCache(ByVal PortalId As Integer)
            CachingProvider.Instance().Clear("Tab", PortalId.ToString)
        End Sub

#End Region

#Region "Clear Single/Small Group Keys"

        Public Shared Sub ClearDefinitionsCache(ByVal PortalId As Integer)
            RemoveCache(String.Format(ProfileDefinitionsCacheKey, PortalId))
        End Sub

        Public Shared Sub ClearDesktopModulePermissionsCache()
            RemoveCache(DesktopModulePermissionCacheKey)
        End Sub

        Public Shared Sub ClearFolderPermissionsCache(ByVal PortalId As Integer)
            RemoveCache(String.Format(FolderPermissionCacheKey, PortalId))
        End Sub

        Public Shared Sub ClearListsCache(ByVal PortalId As Integer)
            RemoveCache(String.Format(ListsCacheKey, PortalId))
        End Sub

        Public Shared Sub ClearModulePermissionsCache(ByVal TabId As Integer)
            RemoveCache(String.Format(ModulePermissionCacheKey, TabId))
        End Sub

        Public Shared Sub ClearTabPermissionsCache(ByVal PortalId As Integer)
            RemoveCache(String.Format(TabPermissionCacheKey, PortalId))
        End Sub

        Public Shared Sub ClearUserCache(ByVal PortalId As Integer, ByVal username As String)
            RemoveCache(String.Format(UserCacheKey, PortalId, username))
        End Sub

#End Region

#End Region

#Region "GetCachedData"

        Public Shared Function GetCachedData(Of TObject)(ByVal cacheItemArgs As CacheItemArgs, ByVal cacheItemExpired As CacheItemExpiredCallback) As TObject
            ' declare local object and try and retrieve item from the cache
            Dim objObject As Object = GetCache(cacheItemArgs.CacheKey)

            ' if item is not cached
            If objObject Is Nothing Then
                'Get Unique Lock for cacheKey
                Dim lock As Object = GetUniqueLockObject(cacheItemArgs.CacheKey)

                ' prevent other threads from entering this block while we regenerate the cache
                SyncLock lock
                    ' try to retrieve object from the cache again (in case another thread loaded the object since we first checked)
                    objObject = GetCache(cacheItemArgs.CacheKey)

                    ' if object was still not retrieved
                    If objObject Is Nothing Then
                        ' set cache timeout
                        Dim timeOut As Integer = cacheItemArgs.CacheTimeOut * Convert.ToInt32(Host.PerformanceSetting)

                        ' get object from data source using delegate
                        Try
                            objObject = cacheItemExpired(cacheItemArgs)
                        Catch ex As Exception
                            objObject = Nothing
                            LogException(ex)
                        End Try

                        ' if we retrieved a valid object and we are using caching
                        If objObject IsNot Nothing AndAlso timeOut > 0 Then
                            ' save the object in the cache
                            DataCache.SetCache(cacheItemArgs.CacheKey, objObject, cacheItemArgs.CacheDependency, Cache.NoAbsoluteExpiration, _
                                               TimeSpan.FromMinutes(timeOut), cacheItemArgs.CachePriority, cacheItemArgs.CacheCallback)

                            ' check if the item was actually saved in the cache
                            If DataCache.GetCache(cacheItemArgs.CacheKey) Is Nothing Then

                                ' log the event if the item was not saved in the cache ( likely because we are out of memory )
                                Dim objEventLogInfo As New LogInfo
                                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_OVERFLOW.ToString()
                                objEventLogInfo.LogProperties.Add(New LogDetailInfo(cacheItemArgs.CacheKey, "Overflow - Item Not Cached"))
                                Dim objEventLog As New EventLogController()
                                objEventLog.AddLog(objEventLogInfo)

                            End If
                        End If
                        'This thread won so remove unique Lock from collection
                        RemoveUniqueLockObject(cacheItemArgs.CacheKey)
                    End If
                End SyncLock
            End If

            ' return the object
            If objObject Is Nothing Then
                Return Nothing
            Else
                Return DirectCast(objObject, TObject)
            End If

        End Function

        Private Shared Function GetUniqueLockObject(ByVal key As String) As Object
            Dim lock As Object = Nothing
            dictionaryLock.AcquireReaderLock(New TimeSpan(0, 0, 5))
            Try

                'Try to get lock Object (for key) from Dictionary
                If lockDictionary.ContainsKey(key) Then
                    lock = lockDictionary(key)
                End If
            Finally
                dictionaryLock.ReleaseReaderLock()
            End Try

            If lock Is Nothing Then
                dictionaryLock.AcquireWriterLock(New TimeSpan(0, 0, 5))
                Try
                    'Double check dictionary
                    If Not lockDictionary.ContainsKey(key) Then
                        'Create new lock
                        lockDictionary(key) = New Object
                    End If

                    'Retrieve lock
                    lock = lockDictionary(key)
                Finally
                    dictionaryLock.ReleaseWriterLock()
                End Try
            End If
            Return lock
        End Function

        Private Shared Sub RemoveUniqueLockObject(ByVal key As String)
            dictionaryLock.AcquireWriterLock(New TimeSpan(0, 0, 5))
            Try
                'check dictionary
                If lockDictionary.ContainsKey(key) Then
                    'Remove lock
                    lockDictionary.Remove(key)
                End If
            Finally
                dictionaryLock.ReleaseWriterLock()
            End Try
        End Sub

#End Region

#Region "GetCache Methods"

        Public Shared Function GetCache(Of TObject)(ByVal CacheKey As String) As TObject
            Dim objObject As Object = GetCache(CacheKey)
            If objObject Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(objObject, TObject)
        End Function

        Public Shared Function GetCache(ByVal CacheKey As String) As Object
            Return CachingProvider.Instance().GetItem(GetDnnCacheKey(CacheKey))
        End Function

#End Region

#Region "Remove Cache"

        Public Shared Sub RemoveCache(ByVal CacheKey As String)
            CachingProvider.Instance().Remove(GetDnnCacheKey(CacheKey))
        End Sub

#End Region

#Region "Set Cache Methods"

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DNNCacheDependency)
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal AbsoluteExpiration As Date)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal SlidingExpiration As TimeSpan)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)
            If objObject IsNot Nothing Then
                ' if no OnRemoveCallback value is specified, use the default method
                If OnRemoveCallback Is Nothing Then
                    OnRemoveCallback = AddressOf ItemRemovedCallback
                End If
                CachingProvider.Instance().Insert(GetDnnCacheKey(CacheKey), objObject, objDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
            End If
        End Sub

#End Region

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.0 - Replace by ClearHostCache(True)")> _
        Public Shared Sub ClearModuleCache()
            ClearHostCache(True)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Function GetPersistentCacheItem(ByVal CacheKey As String, ByVal objType As Type) As Object
            Return CachingProvider.Instance().GetItem(GetDnnCacheKey(CacheKey))
        End Function

        <Obsolete("Deprecated in DNN 5.1.1 - Should have been declared Friend")> _
        Public Shared Sub ClearDesktopModuleCache(ByVal PortalId As Integer)
            RemoveCache(String.Format(DesktopModuleCacheKey, portalId.ToString()))
            RemoveCache(ModuleDefinitionCacheKey)
            RemoveCache(ModuleControlsCacheKey)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.1 - Should have been declared Friend")> _
        Public Shared Sub ClearHostSettingsCache()
            RemoveCache(HostSettingsCacheKey)
            RemoveCache(SecureHostSettingsCacheKey)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Sub RemovePersistentCacheItem(ByVal CacheKey As String)
            CachingProvider.Instance().Remove(GetDnnCacheKey(CacheKey))
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal PersistAppRestart As Boolean)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal PersistAppRestart As Boolean)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal AbsoluteExpiration As Date, ByVal PersistAppRestart As Boolean)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal SlidingExpiration As TimeSpan, ByVal PersistAppRestart As Boolean)
            Dim objDependency As DNNCacheDependency = Nothing
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DotNetNuke.Services.Cache.DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal PersistAppRestart As Boolean)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DotNetNuke.Services.Cache.DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback, ByVal PersistAppRestart As Boolean)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)
            SetCache(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
        End Sub

#End Region

    End Class

End Namespace
