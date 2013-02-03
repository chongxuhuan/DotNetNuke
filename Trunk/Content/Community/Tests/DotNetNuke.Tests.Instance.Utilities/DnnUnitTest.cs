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
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Security;

using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.ComponentModel;
using DotNetNuke.Security.Membership;

namespace DotNetNuke.Tests.Instance.Utilities
{
    public class DnnUnitTest
    {
        protected SqlDataProvider SqlProvider;
        public int PortalId { get; private set; }

        public DnnUnitTest(int portalId)
        {
            var simulator = new HttpSimulator.HttpSimulator();
            simulator.SimulateRequest();

            InstallComponents();

            LoadDnnProviders("data;logging;caching;authentication;members;roles;profiles;permissions;folder");

            //fix Globals.ApplicationMapPath
            var appPath = ConfigurationManager.AppSettings["DefaultPhysicalAppPath"];
            if(!string.IsNullOrEmpty(appPath))
            {
                var mappath = typeof (Globals).GetField("_applicationMapPath", BindingFlags.Static | BindingFlags.NonPublic);
                mappath.SetValue(null, appPath);
            }

            //fix membership
            var providerProp = typeof(Membership).GetField("s_Provider", BindingFlags.Static | BindingFlags.NonPublic);
            providerProp.SetValue(null, Membership.Providers["AspNetSqlMembershipProvider"]);

            var objPortalAliasInfo = new DotNetNuke.Entities.Portals.PortalAliasInfo { PortalID = portalId };
            var ps = new DotNetNuke.Entities.Portals.PortalSettings(59, objPortalAliasInfo);
            System.Web.HttpContext.Current.Items.Add("PortalSettings", ps);
            SqlProvider = new SqlDataProvider();
            PortalId = portalId;
        }

        private static void InstallComponents()
        {
            //creates a simple container for the component model
            ComponentFactory.Container = new SimpleContainer();
            ComponentFactory.InstallComponents(new ProviderInstaller("data", typeof(DataProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("caching", typeof(DotNetNuke.Services.Cache.CachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("logging", typeof(DotNetNuke.Services.Log.EventLog.LoggingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("scheduling", typeof(DotNetNuke.Services.Scheduling.SchedulingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchIndex", typeof(DotNetNuke.Services.Search.IndexingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchDataStore", typeof(DotNetNuke.Services.Search.SearchDataStoreProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("friendlyUrl", typeof(DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("members", typeof(DotNetNuke.Security.Membership.MembershipProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("roles", typeof(DotNetNuke.Security.Roles.RoleProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("profiles", typeof(DotNetNuke.Security.Profile.ProfileProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("permissions", typeof(DotNetNuke.Security.Permissions.PermissionProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("outputCaching", typeof(DotNetNuke.Services.OutputCache.OutputCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("moduleCaching", typeof(DotNetNuke.Services.ModuleCache.ModuleCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("folder", typeof(DotNetNuke.Services.FileSystem.FolderProvider)));

            var provider = ComponentFactory.GetComponent<DotNetNuke.Security.Permissions.PermissionProvider>();
            if (provider == null)
            {
                ComponentFactory.RegisterComponentInstance<DotNetNuke.Security.Permissions.PermissionProvider>(new DotNetNuke.Security.Permissions.PermissionProvider());
            }

        }

        /// <summary>
        /// This proc loads up specified DNN providers, because the BuildManager doesn't get the context right
        /// The providers are cahced so that the DNN base buildManager calls don't have to load up hte providers
        /// </summary>
        private static void LoadDnnProviders(string providerList)
        {
            if (providerList != null)
            {
                var providers = providerList.Split(';');
                foreach (var provider in providers)
                {
                    if (provider.Length > 0)
                    {
                        var config = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(provider);
                        if (config != null)
                        {
                            foreach (string providerName in config.Providers.Keys)
                            {
                                var providerValue = (DotNetNuke.Framework.Providers.Provider)config.Providers[providerName];
                                var type = providerValue.Type;
                                var assembly = providerValue.Type;

                                if (type.Contains(", "))  //get the straight typename, no assembly, for the cache key
                                {
                                    assembly = type.Substring(type.IndexOf(", ") + 1);
                                    type = type.Substring(0, type.IndexOf(", "));
                                }

                                var cacheKey = type;

                                DotNetNuke.Framework.Reflection.CreateType(providerValue.Type, cacheKey, true, false);

                              
                                //if (provider == "logging" )
                                //{
                                //    var sqlDataProviderType = type;
                                //    var lastDot = type.Length - 1;

                                //    for (var i = type.Length - 1; i > -1; i--)
                                //    {
                                //        var c = type[i].ToString();
                                //        if (c == ".")
                                //        {
                                //            lastDot = i;
                                //            break;
                                //        }
                                //    }
                                //    //try and load the type from the same assembly
                                //    sqlDataProviderType = sqlDataProviderType.Substring(0, lastDot);  //remove the Provider type
                                //    sqlDataProviderType += ".SqlDataProvider"; //add on the dataprovider type
                                //    cacheKey = sqlDataProviderType;
                                //    try
                                //    {
                                //        object sqlDataProviderObject = DotNetNuke.Framework.Reflection.CreateType(sqlDataProviderType + ", " + assembly, cacheKey, true, true);
                                //        if (sqlDataProviderObject == null)
                                //            Console.WriteLine("SqlDataProvider type failed on pre-load: " + sqlDataProviderType + ", " + assembly);
                                //        else
                                //            Console.WriteLine("SqlDataProvider type load succeeded : " + sqlDataProviderType + ", " + assembly);
                                //    }
                                //    catch
                                //    {
                                //        Console.WriteLine("SqlDataProvider type failed on pre-load: " + sqlDataProviderType + ", " + assembly);
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
        }
    }
}
