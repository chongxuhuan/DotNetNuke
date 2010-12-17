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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.Modules

    Public MustInherit Class ModuleHostBase
        Implements IModuleHost

        Private m_CachedContent As String = Null.NullString
        Private m_Host As ModuleHost
        Private m_IsCached As Boolean
        Private m_ModuleContext As ModuleInstanceContext

#Region "Protected Properties"

        Protected ReadOnly Property CachedContent As String
            Get
                Return m_CachedContent
            End Get
        End Property

        Protected ReadOnly Property HttpContext As HttpContextBase
            Get
                Return New HttpContextWrapper(System.Web.HttpContext.Current)
            End Get
        End Property

        Protected ReadOnly Property IsCached As Boolean
            Get
                Return m_IsCached
            End Get
        End Property

        Protected Overridable ReadOnly Property ModuleContext() As ModuleInstanceContext
            Get
                Return m_ModuleContext
            End Get
        End Property

        Protected ReadOnly Property ModuleHost As ModuleHost
            Get
                Return m_Host
            End Get
        End Property

        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings
            End Get
        End Property

#End Region

        Protected Function DisplayContent() As Boolean
            'module content visibility options
            Dim blnContent As Boolean = PortalSettings.UserMode <> PortalSettings.Mode.Layout
            If Not HttpContext.Request.QueryString("content") Is Nothing Then
                Select Case HttpContext.Request.QueryString("Content").ToLower
                    Case "1", "true"
                        blnContent = True
                    Case "0", "false"
                        blnContent = False
                End Select
            End If
            If IsAdminControl() = True Then
                blnContent = True
            End If
            Return blnContent
        End Function

        Protected Function IsViewMode() As Boolean
            Return Not (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, ModuleContext.Configuration)) _
                                OrElse PortalSettings.UserMode = PortalSettings.Mode.View
        End Function

        Protected Function SupportsCaching() As Boolean
            Return ModuleContext.Configuration.CacheTime > 0
        End Function

        Protected Overridable Sub TryLoadCached()
            m_IsCached = Null.NullBoolean

            Try
                Dim cache As ModuleCache.ModuleCachingProvider = ModuleCache.ModuleCachingProvider.Instance(ModuleContext.Configuration.GetEffectiveCacheMethod)
                Dim varyBy As New System.Collections.Generic.SortedDictionary(Of String, String)
                varyBy.Add("locale", System.Threading.Thread.CurrentThread.CurrentUICulture.ToString)
                Dim cacheKey As String = cache.GenerateCacheKey(ModuleContext.Configuration.TabModuleID, varyBy)
                Dim cachedBytes As Byte() = ModuleCache.ModuleCachingProvider.Instance(ModuleContext.Configuration.GetEffectiveCacheMethod).GetModule(ModuleContext.Configuration.TabModuleID, cacheKey)
                If Not cachedBytes Is Nothing AndAlso cachedBytes.Length > 0 Then
                    m_CachedContent = System.Text.Encoding.UTF8.GetString(cachedBytes)
                    m_IsCached = True
                End If
            Catch ex As Exception
                m_CachedContent = Null.NullString
                m_IsCached = False
            End Try
        End Sub

        Public Overridable Sub Initialize(ByVal configuration As ModuleInfo, ByVal host As ModuleHost) Implements IModuleHost.Initialize
            m_ModuleContext = New ModuleInstanceContext(configuration)
            m_Host = host

            'Clear Control Collection of Module Host
            ModuleHost.Controls.Clear()

            If DisplayContent() Then
                'if the module supports caching and caching is enabled for the instance and the user does not have Edit rights or is currently in View mode
                If SupportsCaching() AndAlso IsViewMode() Then
                    'attempt to load the cached content
                    TryLoadCached()
                End If
            End If
        End Sub

        Public Overridable Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter) Implements IModuleHost.Render

        End Sub

    End Class

End Namespace
