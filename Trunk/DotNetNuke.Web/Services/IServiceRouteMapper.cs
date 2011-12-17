using System.Web.Routing;

namespace DotNetNuke.Web.Services
{
    public interface IServiceRouteMapper
    {
        void RegisterRoutes(ServicesRoutingManager routeManager);
    }
}