using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Web.Services
{
    public class ServicesRoutingManager
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
                    DnnLog.Error("{0}.RegisterRoutes threw and exception.  {1}", routeMapper.GetType().FullName, e.Message);
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

        internal IPortalController PortalController { get; set; }

        internal static bool IsValidServiceRouteMapper(Type t)
        {
            return t != null && t.IsClass && !t.IsAbstract && t.IsVisible && typeof(IServiceRouteMapper).IsAssignableFrom(t);
        }

        //todo paramter ordering informed by MVC, or just make it work nice for us
        public IList<Route> MapRoute(string uniqueServiceName, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if(namespaces == null || namespaces.Length == 0 || String.IsNullOrEmpty(namespaces[0]))
            {
                throw new ArgumentException("At least one namespace must be specified.");
            }

            if(String.IsNullOrEmpty(uniqueServiceName))
            {
                throw new ArgumentNullException("uniqueServiceName");
            }

            url = url.Trim(new[] {'/', '\\'});

            var prefixes = GetRoutePrefixes();
            var routes = new List<Route>();

            int i = 0;
            foreach (var prefix in prefixes)
            {
                var routeName = uniqueServiceName + "-" + name + "-" + i;
                var routeUrl = prefix + "DesktopModules/API/" + uniqueServiceName + "/" + url;
                routes.Add(_routes.MapRoute(routeName, routeUrl, defaults, constraints, namespaces));
                DnnLog.Trace("Mapping route: " + routeName + "/" + routeUrl);

                i++;
            }

            return routes;
        }

        public IList<Route> MapRoute(string uniqueServiceName, string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(uniqueServiceName, name, url, defaults, null, namespaces);
        }

        private List<string> GetRoutePrefixes()
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

            return _prefixes;
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