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
Imports DotNetNuke.ComponentModel
Imports DotNetNuke.Common.Utilities
Imports System.Web


Namespace DotNetNuke.Common

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : Global
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[sun1]	1/18/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class [Global]
        Inherits System.Web.HttpApplication

#Region "Application Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Application_Start
        ''' Executes on the first web request into the portal application, 
        ''' when a new DLL is deployed, or when web.config is modified.
        ''' </summary>
        ''' <param name="Sender"></param>
        ''' <param name="E"></param>
        ''' <remarks>
        ''' - global variable initialization
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Application_Start(ByVal Sender As Object, ByVal E As EventArgs)
            If Config.GetSetting("ServerName") = "" Then
                ServerName = Server.MachineName
            Else
                ServerName = Config.GetSetting("ServerName")
            End If

            ComponentFactory.Container = New SimpleContainer()

            'Install most Providers as Singleton LifeStyle
            ComponentFactory.InstallComponents(New ProviderInstaller("data", GetType(DotNetNuke.Data.DataProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("caching", GetType(Services.Cache.CachingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("logging", GetType(Services.Log.EventLog.LoggingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("scheduling", GetType(Services.Scheduling.SchedulingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("searchIndex", GetType(Services.Search.IndexingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("searchDataStore", GetType(Services.Search.SearchDataStoreProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("friendlyUrl", GetType(Services.Url.FriendlyUrl.FriendlyUrlProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("members", GetType(DotNetNuke.Security.Membership.MembershipProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("roles", GetType(DotNetNuke.Security.Roles.RoleProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("profiles", GetType(DotNetNuke.Security.Profile.ProfileProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("permissions", GetType(DotNetNuke.Security.Permissions.PermissionProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("outputCaching", GetType(DotNetNuke.Services.OutputCache.OutputCachingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("moduleCaching", GetType(DotNetNuke.Services.ModuleCache.ModuleCachingProvider)))
            ComponentFactory.InstallComponents(New ProviderInstaller("sitemap", GetType(DotNetNuke.Services.Sitemap.SitemapProvider)))

            Dim provider As DotNetNuke.Security.Permissions.PermissionProvider = DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of DotNetNuke.Security.Permissions.PermissionProvider)()
            If provider Is Nothing Then
                ComponentFactory.RegisterComponentInstance(Of DotNetNuke.Security.Permissions.PermissionProvider)(New DotNetNuke.Security.Permissions.PermissionProvider())
            End If

            'Install Navigation and Html Providers as NewInstance Lifestyle (ie a new instance is generated each time the type is requested, as there are often multiple instances on the page)
            ComponentFactory.InstallComponents(New ProviderInstaller("htmlEditor", GetType(Modules.HTMLEditorProvider.HtmlEditorProvider), ComponentLifeStyleType.Transient))
            ComponentFactory.InstallComponents(New ProviderInstaller("navigationControl", GetType(Modules.NavigationProvider.NavigationProvider), ComponentLifeStyleType.Transient))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Application_End
        ''' Executes when the Application times out
        ''' </summary>
        ''' <param name="Sender"></param>
        ''' <param name="E"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Application_End(ByVal Sender As Object, ByVal E As EventArgs)

            ' log APPLICATION_END event
            Initialize.LogEnd()

            ' stop scheduled jobs
            Initialize.StopScheduler()

        End Sub

#End Region

        Private Sub Global_BeginRequest(ByVal sender As Object, ByVal e As EventArgs) Handles Me.BeginRequest
            Dim app As HttpApplication = CType(sender, HttpApplication)
            Dim Request As HttpRequest = app.Request

            If Request.Url.LocalPath.ToLower.EndsWith("scriptresource.axd") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("webresource.axd") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("gif") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("jpg") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("css") _
                    OrElse Request.Url.LocalPath.ToLower.EndsWith("js") Then
                Exit Sub
            End If

            ' all of the logic which was previously in Application_Start was moved to Init() in order to support IIS7 integrated pipeline mode ( which no longer provides access to HTTP context within Application_Start )
            Initialize.Init(app)

            'run schedule if in Request mode
            Initialize.RunSchedule(Request)

        End Sub


    End Class

End Namespace
