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
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Web.Services
{
    public sealed class ServicesRoutingManager
    {
        private readonly RouteCollection _routes;
        private IList<string> _prefixes;

        internal ServicesRoutingManager() : this(RouteTable.Routes) {}

        internal ServicesRoutingManager(RouteCollection routes)
        {
            _routes = routes;
            TypeLocator = new TypeLocator();
            PortalController = new PortalController();
        }

        internal void RegisterRoutes()
        {
            _routes.Clear();
            LocateServicesAndMapRoutes();
            DnnLog.Trace("Registered a total of {0} routes", _routes.Count);
        }

        private void LocateServicesAndMapRoutes()
        {
            RegisterSystemRoutes();

            foreach (var routeMapper in GetServiceRouteMappers())
            {
                try
                {
                    routeMapper.RegisterRoutes(this);
                }
                catch (Exception e)
                {
                    DnnLog.Error("{0}.RegisterRoutes threw an exception.  {1}\r\n{2}", routeMapper.GetType().FullName, e.Message, e.StackTrace);
                }
            }
        }

        private void RegisterSystemRoutes()
        {
            _routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }

        private IEnumerable<IServiceRouteMapper> GetServiceRouteMappers()
        {
            var types = GetAllServiceRouteMapperTypes();

            DnnLog.Trace("Located {0} types that implement IServiceRouteMapper");

            foreach (var routeMapperType in types)
            {
                IServiceRouteMapper routeMapper;
                try
                {
                    routeMapper = Activator.CreateInstance(routeMapperType) as IServiceRouteMapper;
                }
                catch(Exception e)
                {
                    DnnLog.Error("Unable to create {0} while registering service routes.  {1}", routeMapperType.FullName, e.Message);
                    routeMapper = null;
                }

                if(routeMapper != null)
                {
                    yield return routeMapper;
                }
            }
        }

        private IEnumerable<Type> GetAllServiceRouteMapperTypes()
        {
            return TypeLocator.GetAllMatchingTypes(IsValidServiceRouteMapper);
        }

        internal ITypeLocator TypeLocator { get; set; }

        internal PortalControllerBase PortalController { get; set; }

        internal static bool IsValidServiceRouteMapper(Type t)
        {
            return t != null && t.IsClass && !t.IsAbstract && t.IsVisible && typeof(IServiceRouteMapper).IsAssignableFrom(t);
        }

        /// <summary>
        /// Sets up the route(s) for DotNetNuke services
        /// </summary>
        /// <param name="moduleFolderName">The name of the folder under DesktopModules in which your module resides</param>
        /// <param name="name">The name of the route</param>
        /// <param name="url">The parameterized portion of the route</param>
        /// <param name="defaults">Default values for the route parameters</param>
        /// <param name="constraints">The constraints</param>
        /// <param name="namespaces">The namespace(s) in which to locate the controllers for this route</param>
        /// <returns>A list of all routes that were registered.</returns>
        public IList<Route> MapRoute(string moduleFolderName, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if(namespaces == null || namespaces.Length == 0 || String.IsNullOrEmpty(namespaces[0]))
            {
                throw new ArgumentException("At least one namespace must be specified.");
            }

            if(String.IsNullOrEmpty(moduleFolderName))
            {
                throw new ArgumentNullException("moduleFolderName");
            }

            url = url.Trim(new[] {'/', '\\'});

            var prefixes = GetRoutePrefixes();
            var routes = new List<Route>();

            int i = 0;
            foreach (var prefix in prefixes)
            {
                var routeName = moduleFolderName + "-" + name + "-" + i;
                var routeUrl = string.Format("{0}DesktopModules/{1}/API/{2}", prefix, moduleFolderName, url);
                routes.Add(_routes.MapRoute(routeName, routeUrl, defaults, constraints, namespaces));
                DnnLog.Trace("Mapping route: " + routeName + " @ " + routeUrl);

                i++;
            }

            return routes;
        }

        /// <summary>
        /// Sets up the route(s) for DotNetNuke services
        /// </summary>
        /// <param name="moduleFolderName">The name of the folder under DesktopModules in which your module resides</param>
        /// <param name="name">The name of the route</param>
        /// <param name="url">The parameterized portion of the route</param>
        /// <param name="defaults">Default values for the route parameters</param>
        /// <param name="namespaces">The namespace(s) in which to locate the controllers for this route</param>
        /// <returns>A list of all routes that were registered.</returns>
        public IList<Route> MapRoute(string moduleFolderName, string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(moduleFolderName, name, url, defaults, null, namespaces);
        }

        private IEnumerable<string> GetRoutePrefixes()
        {
            if(_prefixes == null)
            {
                List<int> segmentCounts = CountSegmentsInPortalAliases();

                //generate prefixes
                _prefixes = new List<string>();

                foreach (var segmentCount in segmentCounts)
                {
                    if(segmentCount == 0)
                    {
                        _prefixes.Add("");
                    }
                    else
                    {
                        string prefix = "";

                        for(int i = segmentCount - 1; i >= 0; i--)
                        {
                            prefix = "{prefix" + i + "}/" + prefix;
                        }

                        _prefixes.Add(prefix);
                    }
                }
            }

            return _prefixes.OrderByDescending(x => x.Length);
        }

        private List<int> CountSegmentsInPortalAliases()
        {
            var portals = PortalController.GetPortals();

            var segmentCounts = new List<int>();

            var portalAliasController = new PortalAliasController();
            foreach (PortalInfo portal in portals)
            {
                var aliases =
                    portalAliasController.GetPortalAliasByPortalID(portal.PortalID).Values.Cast<PortalAliasInfo>().Select(x => x.HTTPAlias);

                foreach (string alias in aliases)
                {
                    int count = alias.Where(c => c == '/').Count();

                    if (!segmentCounts.Contains(count))
                    {
                        segmentCounts.Add(count);
                    }
                }
            }
            return segmentCounts;
        }
    }
}