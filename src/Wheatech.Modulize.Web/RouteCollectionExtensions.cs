using System;
using System.Web.Routing;

namespace Wheatech.Modulize.Web
{
    /// <summary>
    /// Extends a <see cref="RouteCollection"/> object for module Web Forms page routing.
    /// </summary>
    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <param name="dataTokens">Values that are associated with the route that are not used to determine whether a route matches a URL pattern.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string routeName, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            var route = new ModulePageRoute(module, routeUrl, defaults, constraints, dataTokens, new ModulePageRouteHandler(module, physicalFile))
            {
                DataTokens =
                {
                    ["module"] = module
                }
            };
            if (string.IsNullOrEmpty(routeName))
            {
                routes.Add(route);
            }
            else
            {
                routes.Add(routeName, route);
            }
            return route;
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <param name="dataTokens">Values that are associated with the route that are not used to determine whether a route matches a URL pattern.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            return MapModulePageRoute(routes, null, module, routeUrl, physicalFile, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <param name="dataTokens">Values that are associated with the route that are not used to determine whether a route matches a URL pattern.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
        {
            return MapModulePageRoute(routes, module, routeUrl, routeUrl, defaults, constraints, dataTokens);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string routeName, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            return MapModulePageRoute(routes, routeName, module, routeUrl, physicalFile, defaults, constraints, null);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            return MapModulePageRoute(routes, null, module, routeUrl, physicalFile, defaults, constraints);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <param name="constraints">Constraints that a URL request must meet in order to be processed as this route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            return MapModulePageRoute(routes, module, routeUrl, routeUrl, defaults, constraints);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string routeName, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults)
        {
            return MapModulePageRoute(routes, routeName, module, routeUrl, physicalFile, defaults, null);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, string physicalFile, RouteValueDictionary defaults)
        {
            return MapModulePageRoute(routes, null, module, routeUrl, physicalFile, defaults);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="defaults">Default values for the route parameters.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, RouteValueDictionary defaults)
        {
            return MapModulePageRoute(routes, module, routeUrl, routeUrl, defaults);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string routeName, string module, string routeUrl, string physicalFile)
        {
            return MapModulePageRoute(routes, routeName, module, routeUrl, physicalFile, null);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <param name="physicalFile">The module relative physical URL for the route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl, string physicalFile)
        {
            return MapModulePageRoute(routes, null, module, routeUrl, physicalFile);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <param name="routeUrl">The module relative URL pattern for the route.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module, string routeUrl)
        {
            return MapModulePageRoute(routes, module, routeUrl, routeUrl);
        }

        /// <summary>
        /// Provides a way to define routes for module Web Forms page.
        /// </summary>
        /// <param name="routes">A collection of routes for the application.</param>
        /// <param name="module">The module ID of the route to map.</param>
        /// <returns>The route that is added to the route collection.</returns>
        public static RouteBase MapModulePageRoute(this RouteCollection routes, string module)
        {
            return MapModulePageRoute(routes, module, null);
        }
    }
}
