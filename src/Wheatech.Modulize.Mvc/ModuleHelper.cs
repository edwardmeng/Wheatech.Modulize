using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    internal static class ModuleHelper
    {
        public static ModuleDescriptor GetModule(RouteData routeData)
        {
            object value;
            if (routeData.DataTokens.TryGetValue("module", out value))
            {
                return (ModuleDescriptor)value;
            }
            return GetModule(routeData.Route);
        }

        public static ModuleDescriptor GetModule(RouteBase route)
        {
            var moduleId = (route as IRouteWithModule)?.Module;
            if (moduleId != null)
            {
                return Modulizer.GetModule(moduleId);
            }
            return (route as Route)?.DataTokens?["module"] as ModuleDescriptor;
        }

        public static string GetAreaName(RouteBase route)
        {
            return (route as IRouteWithArea)?.Area ?? (route as Route)?.DataTokens?["area"] as string;
        }

        public static string GetAreaName(RouteData routeData)
        {
            object value;
            if (routeData.DataTokens.TryGetValue("area", out value))
            {
                return value as string;
            }
            return GetAreaName(routeData.Route);
        }

    }
}
