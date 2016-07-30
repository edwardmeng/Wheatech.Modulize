using System;
using System.Web.Routing;

namespace Wheatech.Modulize.Web
{
    public class ModuleRouteRegistrationContext
    {
        public ModuleRouteRegistrationContext(ModuleDescriptor module, RouteCollection routes, object state)
        {
            Module = module;
            Routes = routes;
            State = state;
        }

        public ModuleRouteRegistrationContext(ModuleDescriptor module, RouteCollection routes) : this(module, routes, null)
        {
        }

        public ModuleDescriptor Module { get; }

        protected RouteCollection Routes { get; }

        public object State { get; }

        public virtual Route MapPageRoute(string name, string url, string physicalFile, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            var route = new Route(url, defaults, constraints, new ModulePageRouteHandler(Module, physicalFile));
            Routes.Add(name, route);
            return route;
        }

        public virtual Route MapPageRoute(string name, string url, string physicalFile, RouteValueDictionary defaults)
        {
            return MapPageRoute(name, url, physicalFile, defaults, null);
        }

        public virtual Route MapPageRoute(string name, string url, string physicalFile)
        {
            return MapPageRoute(name, url, physicalFile, null);
        }
    }
}
