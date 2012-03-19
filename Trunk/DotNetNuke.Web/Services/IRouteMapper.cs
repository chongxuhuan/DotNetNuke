using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace DotNetNuke.Web.Services
{
    public interface IRouteMapper
    {
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
        IList<Route> MapRoute(string moduleFolderName, string name, string url, object defaults, object constraints, string[] namespaces);

        /// <summary>
        /// Sets up the route(s) for DotNetNuke services
        /// </summary>
        /// <param name="moduleFolderName">The name of the folder under DesktopModules in which your module resides</param>
        /// <param name="name">The name of the route</param>
        /// <param name="url">The parameterized portion of the route</param>
        /// <param name="defaults">Default values for the route parameters</param>
        /// <param name="namespaces">The namespace(s) in which to locate the controllers for this route</param>
        /// <returns>A list of all routes that were registered.</returns>
        IList<Route> MapRoute(string moduleFolderName, string name, string url, object defaults, string[] namespaces);

        /// <summary>
        /// Sets up the route(s) for DotNetNuke services
        /// </summary>
        /// <param name="moduleFolderName">The name of the folder under DesktopModules in which your module resides</param>
        /// <param name="name">The name of the route</param>
        /// <param name="url">The parameterized portion of the route</param>
        /// <param name="namespaces">The namespace(s) in which to locate the controllers for this route</param>
        /// <returns>A list of all routes that were registered.</returns>
        IList<Route> MapRoute(string moduleFolderName, string name, string url, string[] namespaces);
    }
}