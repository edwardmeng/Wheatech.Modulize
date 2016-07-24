using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    internal static class ModuleHelper
    {
        public static string GetModule(RouteData routeData)
        {
            object value;
            if (routeData.DataTokens.TryGetValue("module", out value))
            {
                return value as string;
            }
            return GetModule(routeData.Route);
        }

        public static string GetModule(RouteBase route)
        {
            return (route as IRouteWithModule)?.Module ?? (route as Route)?.DataTokens?["area"] as string;
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
