using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Instrumentation;

namespace DotNetNuke.Web.Services
{
    public class ServicesRoutingManager
    {
        private readonly DnnLogger _logger = DnnLogger.GetClassLogger(typeof(ServicesRoutingManager));
        private readonly RouteCollection _routes;
        
        internal ServicesRoutingManager() : this(RouteTable.Routes) {}

        internal ServicesRoutingManager(RouteCollection routes)
        {
            _routes = routes;
            TypeLocator = new TypeLocator();
        }

        internal void RegisterRoutes()
        {
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
                    _logger.ErrorFormat("{0}.RegisterRoutes threw and exception.  {1}", routeMapper.GetType().FullName, e.Message);
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
                    _logger.ErrorFormat("Unable to create {0} while registering service routes.  {1}", routeMapperType.FullName, e.Message);
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

        internal static bool IsValidServiceRouteMapper(Type t)
        {
            return t != null && t.IsClass && !t.IsAbstract && t.IsVisible && typeof(IServiceRouteMapper).IsAssignableFrom(t);
        }

        //todo paramter ordering informed by MVC, or just make it work nice for us
        public Route MapRoute(string uniqueServiceName, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if(namespaces == null || namespaces.Length == 0 || String.IsNullOrEmpty(namespaces[0]))
            {
                throw new ArgumentException("At least one namespace must be specified.");
            }

            if(String.IsNullOrEmpty(uniqueServiceName))
            {
                throw new ArgumentNullException("uniqueServiceName");
            }

            //TODO what about sites that are not at the root e.g. http://www.foo.com/dnn is the dnn root
            url = url.Trim(new[] {'/', '\\'});
            return _routes.MapRoute(uniqueServiceName + "-" + name, "DesktopModules/API/" + uniqueServiceName + "/" + url, defaults, constraints, namespaces);
        }

        public Route MapRoute(string uniqueServiceName, string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(uniqueServiceName, name, url, defaults, null, namespaces);
        }
    }
}