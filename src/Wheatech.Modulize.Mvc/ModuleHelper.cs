using System;
using System.Globalization;
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

        public static string GenerateUrl(string routeName, string actionName, string controllerName, string module, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues)
        {
            if (routeCollection == null)
            {
                throw new ArgumentNullException(nameof(routeCollection));
            }

            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            var mergedRouteValues = RouteValuesHelpers.MergeRouteValues(actionName, controllerName, module, requestContext.RouteData.Values, routeValues, includeImplicitMvcValues);
            var vpd = routeCollection.GetVirtualPathForModuleArea(requestContext, routeName, mergedRouteValues);
            return vpd == null ? null : PathHelpers.GenerateClientUrl(requestContext.HttpContext, vpd.VirtualPath);
        }

        public static string GenerateUrl(string routeName, string actionName, string controllerName, string module, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection, RequestContext requestContext, bool includeImplicitMvcValues)
        {
            string url = GenerateUrl(routeName, actionName, controllerName, module, routeValues, routeCollection, requestContext, includeImplicitMvcValues);

            if (url == null) return null;
            if (!string.IsNullOrEmpty(fragment))
            {
                url = url + "#" + fragment;
            }

            if (string.IsNullOrEmpty(protocol) && string.IsNullOrEmpty(hostName)) return url;
            Uri requestUrl = requestContext.HttpContext.Request.Url;
            protocol = !string.IsNullOrEmpty(protocol) ? protocol : Uri.UriSchemeHttp;
            hostName = !string.IsNullOrEmpty(hostName) ? hostName : requestUrl?.Host;

            string port = string.Empty;
            string requestProtocol = requestUrl?.Scheme;

            if (string.Equals(protocol, requestProtocol, StringComparison.OrdinalIgnoreCase))
            {
                port = requestUrl.IsDefaultPort ? string.Empty : ":" + Convert.ToString(requestUrl.Port, CultureInfo.InvariantCulture);
            }

            return protocol + Uri.SchemeDelimiter + hostName + port + url;
        }
    }
}
