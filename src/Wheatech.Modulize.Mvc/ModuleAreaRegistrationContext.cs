using System;
using System.Web.Routing;
using Wheatech.Modulize.Mvc.Properties;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleAreaRegistrationContext : ModuleRegistrationContext
    {
        public ModuleAreaRegistrationContext(ModuleDescriptor module, string areaName, RouteCollection routes, object state) : base(module, routes, state)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty,nameof(areaName));
            }
            AreaName = areaName;
        }

        public ModuleAreaRegistrationContext(ModuleDescriptor module, string areaName, RouteCollection routes) : this(module, areaName, routes, null)
        {
        }

        public string AreaName { get; }

        public override Route MapRoute(string name, string url, object defaults, object constraints, string[] namespaces)
        {
            var route = base.MapRoute(name, url, defaults, constraints, namespaces);
            route.DataTokens["area"] = AreaName;
            return route;
        }
    }
}
