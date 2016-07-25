using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleRegistrationContext
    {
        private readonly HashSet<string> _namespaces;

        public ModuleRegistrationContext(ModuleDescriptor module, RouteCollection routes, object state)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }
            _namespaces = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Module = module;
            Routes = routes;
            State = state;
        }

        public ModuleRegistrationContext(ModuleDescriptor module, RouteCollection routes) : this(module, routes, null)
        {
        }

        public ModuleDescriptor Module { get; }

        public ICollection<string> Namespaces => _namespaces;

        public RouteCollection Routes { get; }

        public object State { get; }

        public virtual Route MapRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (namespaces == null && Namespaces != null)
            {
                namespaces = Namespaces.ToArray();
            }
            var route = Routes.MapRoute(name, url, defaults, constraints, namespaces);
            route.DataTokens["module"] = Module;
            route.DataTokens["UseNamespaceFallback"] = namespaces == null || namespaces.Length == 0;
            return route;
        }

        public Route MapRoute(string name, string url, object defaults, string[] namespaces)
        {
            return MapRoute(name, url, defaults, null, namespaces);
        }

        public Route MapRoute(string name, string url, object defaults, object constraints)
        {
            return MapRoute(name, url, defaults, constraints, null);
        }

        public Route MapRoute(string name, string url, string[] namespaces)
        {
            return MapRoute(name, url, null, namespaces);
        }

        public Route MapRoute(string name, string url, object defaults)
        {
            return MapRoute(name, url, defaults, null);
        }

        public Route MapRoute(string name, string url)
        {
            return MapRoute(name, url, null);
        }
    }
}