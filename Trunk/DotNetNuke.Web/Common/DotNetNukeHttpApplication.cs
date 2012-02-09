#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Net;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Modules.HTMLEditorProvider;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Profile;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.ModuleCache;
using DotNetNuke.Services.OutputCache;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Sitemap;
using DotNetNuke.Services.Url.FriendlyUrl;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Services;

#endregion

namespace DotNetNuke.Web.Common.Internal
{
    /// <summary>
    /// DotNetNuke Http Application. It will handle Start, End, BeginRequest, Error event for whole application.
    /// </summary>
    public class DotNetNukeHttpApplication : HttpApplication
    {
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            DnnLog.Trace("Dumping all Application Errors");
            if (HttpContext.Current != null)
            {
                foreach (Exception exc in HttpContext.Current.AllErrors) DnnLog.Fatal(exc);
            }
            DnnLog.Trace("End Dumping all Application Errors");
        }

        private void Application_Start(object Sender, EventArgs E)
        {
            DnnLog.Info("Application Starting");

            if (String.IsNullOrEmpty(Config.GetSetting("ServerName")))
            {
                Globals.ServerName = Dns.GetHostName();
            }
            else
            {
                Globals.ServerName = Config.GetSetting("ServerName");
            }

            ComponentFactory.Container = new SimpleContainer();
            ComponentFactory.InstallComponents(new ProviderInstaller("data", typeof(DataProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("caching", typeof(CachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("logging", typeof(LoggingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("scheduling", typeof(SchedulingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchIndex", typeof(IndexingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchDataStore", typeof(SearchDataStoreProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("friendlyUrl", typeof(FriendlyUrlProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("members", typeof(MembershipProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("roles", typeof(RoleProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("profiles", typeof(ProfileProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("permissions", typeof(PermissionProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("outputCaching", typeof(OutputCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("moduleCaching", typeof(ModuleCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("sitemap", typeof(SitemapProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("folder", typeof(FolderProvider)));
            RegisterIfNotAlreadyRegistered<FolderProvider, StandardFolderProvider>("StandardFolderProvider");
            RegisterIfNotAlreadyRegistered<FolderProvider, SecureFolderProvider>("SecureFolderProvider");
            RegisterIfNotAlreadyRegistered<FolderProvider, DatabaseFolderProvider>("DatabaseFolderProvider");
            RegisterIfNotAlreadyRegistered<PermissionProvider>();
            ComponentFactory.InstallComponents(new ProviderInstaller("htmlEditor", typeof(HtmlEditorProvider), ComponentLifeStyleType.Transient));
            ComponentFactory.InstallComponents(new ProviderInstaller("navigationControl", typeof(NavigationProvider), ComponentLifeStyleType.Transient));
            ComponentFactory.InstallComponents(new ProviderInstaller("clientcapability", typeof(ClientCapabilityProvider)));
            
            DnnLog.Info("Application Started");
        }
        
        private static void RegisterIfNotAlreadyRegistered<TConcrete>() where TConcrete : class, new()
        {
            RegisterIfNotAlreadyRegistered<TConcrete, TConcrete>("");
        }

        private static void RegisterIfNotAlreadyRegistered<TAbstract, TConcrete>(string name)
            where TAbstract : class
            where TConcrete : class, new()
        {
            var provider = ComponentFactory.GetComponent<TAbstract>();
            if (provider == null)
            {
                if (String.IsNullOrEmpty(name))
                {
                    ComponentFactory.RegisterComponentInstance<TAbstract>(new TConcrete());
                }
                else
                {
                    ComponentFactory.RegisterComponentInstance<TAbstract>(name, new TConcrete());
                }
            }
        }

        private void Application_End(object Sender, EventArgs E)
        {
            DnnLog.MethodEntry();
            DnnLog.Info("Application Ending");
            Initialize.LogEnd();
            Initialize.StopScheduler();

            DnnLog.Trace("Dumping all Application Errors");
            if (HttpContext.Current != null)
            {
                foreach (Exception exc in HttpContext.Current.AllErrors) DnnLog.Fatal(exc);
            }
            DnnLog.Trace("End Dumping all Application Errors");
            DnnLog.Info("Application Ended");
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            string requsetUrl = app.Request.Url.LocalPath.ToLower();
            if (!requsetUrl.EndsWith(".aspx") && !requsetUrl.EndsWith("/") &&
                (requsetUrl.EndsWith("scriptresource.axd") || requsetUrl.EndsWith("webresource.axd") || requsetUrl.EndsWith(".gif") ||
                requsetUrl.EndsWith(".jpg") || requsetUrl.EndsWith(".css") || requsetUrl.EndsWith(".js")) || requsetUrl.EndsWith(".js"))
            {
                return;
            }

            Initialize.Init(app);
            Initialize.RunSchedule(app.Request);
        }
    }
}