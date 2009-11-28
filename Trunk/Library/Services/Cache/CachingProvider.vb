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
Imports System.Web.Caching
Imports DotNetNuke.Services.Log.EventLog
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Services.Cache

    Public MustInherit Class CachingProvider

#Region "Private Members"

        Private Shared _objCache As Caching.Cache
        Private Shared _cachePrefix As String = "DNN_{0}"

#End Region

#Region "Protected Properties"

        Protected Shared ReadOnly Property Cache() As System.Web.Caching.Cache
            Get
                'create singleton of the cache object
                If _objCache Is Nothing Then
                    _objCache = HttpRuntime.Cache
                End If
                Return _objCache
            End Get
        End Property

#End Region

#Region "Shared/Static Methods"

        Public Shared Function Instance() As CachingProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of CachingProvider)()
        End Function

        Public Shared Function GetCacheKey(ByVal CacheKey As String) As String
            If String.IsNullOrEmpty(CacheKey) Then
                Throw New ArgumentException("Argument cannot be null or an empty string", "CacheKey")
            End If
            Return String.Format(_cachePrefix, CacheKey)
        End Function

#End Region

#Region "Private Methods"

        Private Sub ClearCacheInternal(ByVal prefix As String, ByVal clearRuntime As Boolean)
            For Each objDictionaryEntry As DictionaryEntry In HttpRuntime.Cache
                If CType(objDictionaryEntry.Key, String).StartsWith(prefix) Then
                    If clearRuntime Then
                        'remove item from runtime cache
                        RemoveInternal(CType(objDictionaryEntry.Key, String))
                    Else
                        'Call provider's remove method
                        Remove(CType(objDictionaryEntry.Key, String))
                    End If
                End If
            Next
        End Sub

        Private Sub ClearCacheKeysByPortalInternal(ByVal portalId As Integer, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.LocalesCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.ProfileDefinitionsCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.ListsCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.SkinsCacheKey, clearRuntime, portalId)
        End Sub

        Private Sub ClearDesktopModuleCacheInternal(ByVal portalId As Integer, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.DesktopModuleCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.PortalDesktopModuleCacheKey, clearRuntime, portalId)
            RemoveCacheKey(DataCache.ModuleDefinitionCacheKey, clearRuntime)
            RemoveCacheKey(DataCache.ModuleControlsCacheKey, clearRuntime)
        End Sub

        Private Sub ClearFolderCacheInternal(ByVal portalId As Integer, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.FolderCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.FolderPermissionCacheKey, clearRuntime, portalId)
        End Sub

        Private Sub ClearHostCacheInternal(ByVal clearRuntime As Boolean)
            RemoveCacheKey(DataCache.HostSettingsCacheKey, clearRuntime)
            RemoveCacheKey(DataCache.SecureHostSettingsCacheKey, clearRuntime)
            RemoveCacheKey(DataCache.PortalAliasCacheKey, clearRuntime)
            RemoveCacheKey("CSS", clearRuntime)
            RemoveCacheKey(DataCache.DesktopModulePermissionCacheKey, clearRuntime)
            RemoveCacheKey("GetRoles", clearRuntime)
            RemoveCacheKey("CompressionConfig", clearRuntime)

            'Clear "portal keys" for Host
            ClearFolderCacheInternal(-1, clearRuntime)
            ClearDesktopModuleCacheInternal(-1, clearRuntime)
            ClearCacheKeysByPortalInternal(-1, clearRuntime)
        End Sub

        Private Sub ClearModuleCacheInternal(ByVal tabId As Integer, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.TabModuleCacheKey, clearRuntime, tabId)
            RemoveFormattedCacheKey(DataCache.ModulePermissionCacheKey, clearRuntime, tabId)
        End Sub

        Private Sub ClearModulePermissionsCachesByPortalInternal(ByVal portalId As Integer, ByVal clearRuntime As Boolean)
            Dim objTabs As New TabController
            For Each tabPair As KeyValuePair(Of Integer, DotNetNuke.Entities.Tabs.TabInfo) In objTabs.GetTabsByPortal(portalId)
                RemoveFormattedCacheKey(DataCache.ModulePermissionCacheKey, clearRuntime, tabPair.Value.TabID)
            Next
        End Sub

        Private Sub ClearPortalCacheInternal(ByVal portalId As Integer, ByVal cascade As Boolean, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.PortalCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.PortalSettingsCacheKey, clearRuntime, portalId)

            If cascade Then
                Dim objTabs As New TabController
                For Each tabPair As KeyValuePair(Of Integer, DotNetNuke.Entities.Tabs.TabInfo) In objTabs.GetTabsByPortal(portalId)
                    ClearModuleCacheInternal(tabPair.Value.TabID, clearRuntime)
                Next

                Dim moduleController As New DotNetNuke.Entities.Modules.ModuleController()
                For Each moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo In moduleController.GetModules(portalId)
                    RemoveCacheKey("GetModuleSettings" & moduleInfo.ModuleID.ToString(), clearRuntime)
                Next
            End If

            'Clear "portal keys" for Portal
            ClearFolderCacheInternal(portalId, clearRuntime)
            ClearCacheKeysByPortalInternal(portalId, clearRuntime)
            ClearDesktopModuleCacheInternal(portalId, clearRuntime)
            ClearTabCacheInternal(portalId, clearRuntime)
        End Sub

        Private Sub ClearTabCacheInternal(ByVal portalId As Integer, ByVal clearRuntime As Boolean)
            RemoveFormattedCacheKey(DataCache.TabCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.TabPathCacheKey, clearRuntime, portalId)
            RemoveFormattedCacheKey(DataCache.TabPermissionCacheKey, clearRuntime, portalId)
        End Sub

        Private Sub RemoveCacheKey(ByVal CacheKey As String, ByVal clearRuntime As Boolean)
            If clearRuntime Then
                'remove item from runtime cache
                RemoveInternal(GetCacheKey(CacheKey))
            Else
                'Call provider's remove method
                Remove(GetCacheKey(CacheKey))
            End If

        End Sub

        Private Sub RemoveFormattedCacheKey(ByVal CacheKeyBase As String, ByVal clearRuntime As Boolean, ByVal ParamArray parameters As Object())
            If clearRuntime Then
                'remove item from runtime cache
                RemoveInternal(String.Format(GetCacheKey(CacheKeyBase), parameters))
            Else
                'Call provider's remove method
                Remove(String.Format(GetCacheKey(CacheKeyBase), parameters))
            End If

        End Sub

#End Region

#Region "Protected Methods"

        Protected Sub ClearCacheInternal(ByVal cacheType As String, ByVal data As String, ByVal clearRuntime As Boolean)
            Select Case cacheType
                Case "Prefix"
                    ClearCacheInternal(data, clearRuntime)
                Case "Host"
                    ClearHostCacheInternal(clearRuntime)
                Case "Folder"
                    ClearFolderCacheInternal(Integer.Parse(data), clearRuntime)
                Case "Module"
                    ClearModuleCacheInternal(Integer.Parse(data), clearRuntime)
                Case "ModulePermissionsByPortal"
                    ClearModulePermissionsCachesByPortalInternal(Integer.Parse(data), clearRuntime)
                Case "Portal"
                    ClearPortalCacheInternal(Integer.Parse(data), False, clearRuntime)
                Case "PortalCascade"
                    ClearPortalCacheInternal(Integer.Parse(data), True, clearRuntime)
                Case "Tab"
                    ClearTabCacheInternal(Integer.Parse(data), clearRuntime)
            End Select
        End Sub

        Protected Sub RemoveInternal(ByVal CacheKey As String)
            ' remove item from memory
            If Not Cache(CacheKey) Is Nothing Then
                Cache.Remove(CacheKey)
            End If
        End Sub

#End Region

#Region "Virtual Methods"

        Public Overridable Sub Clear(ByVal type As String, ByVal data As String)
            ClearCacheInternal(type, data, False)
        End Sub

        Public Overridable Function GetEnumerator() As IDictionaryEnumerator
            Return Cache.GetEnumerator
        End Function

        Public Overridable Function GetItem(ByVal CacheKey As String) As Object
            Return Cache(CacheKey)
        End Function

        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object)
            Dim objDependency As DNNCacheDependency = Nothing
            Insert(CacheKey, objObject, objDependency, Caching.Cache.NoAbsoluteExpiration, Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DNNCacheDependency)
            Insert(CacheKey, objObject, objDependency, Caching.Cache.NoAbsoluteExpiration, Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)
            Insert(CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal Value As Object, ByVal objDependency As DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)
            If objDependency Is Nothing Then
                Cache.Insert(CacheKey, Value, Nothing, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
            Else
                Cache.Insert(CacheKey, Value, objDependency.SystemCacheDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
            End If
        End Sub

        Public Overridable Function IsWebFarm() As Boolean
            Return (ServerController.GetEnabledServers.Count > 1)
        End Function

        Public Overridable Function PurgeCache() As String
            Return Localization.Localization.GetString("PurgeCacheUnsupported.Text", Localization.Localization.GlobalResourceFile)
        End Function

        Public Overridable Sub Remove(ByVal CacheKey As String)
            RemoveInternal(CacheKey)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.1 - Use one of the INsert methods")> _
        Public Overridable Function Add(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency, ByVal AbsoluteExpiration As DateTime, ByVal SlidingExpiration As TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback) As Object
            Dim retValue As Object = GetItem(CacheKey)
            If retValue Is Nothing Then
                Insert(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
            End If
            Return retValue
        End Function

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Overridable Function GetPersistentCacheItem(ByVal CacheKey As String, ByVal objType As Type) As Object
            Return GetItem(CacheKey)
        End Function

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal PersistAppRestart As Boolean)
            Insert(CacheKey, objObject)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal PersistAppRestart As Boolean)
            Insert(CacheKey, objObject, New DNNCacheDependency(objDependency), Caching.Cache.NoAbsoluteExpiration, Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal PersistAppRestart As Boolean)
            Insert(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")> _
        Public Overridable Overloads Sub Insert(ByVal Key As String, ByVal Value As Object, ByVal objDependency As System.Web.Caching.CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback, ByVal PersistAppRestart As Boolean)
            Insert(Key, Value, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency)
            Insert(CacheKey, objObject, New DNNCacheDependency(objDependency), Caching.Cache.NoAbsoluteExpiration, Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)
            Insert(CacheKey, objObject, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Default, Nothing)
        End Sub

        <Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")> _
        Public Overridable Overloads Sub Insert(ByVal CacheKey As String, ByVal Value As Object, ByVal objDependency As CacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)
            Insert(CacheKey, Value, New DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback)
        End Sub

        <Obsolete("Deprecated in DNN 5.1.1 - Cache Persistence is not supported")> _
        Public Overridable Sub RemovePersistentCacheItem(ByVal CacheKey As String)
            Remove(CacheKey)
        End Sub


#End Region

    End Class

End Namespace
