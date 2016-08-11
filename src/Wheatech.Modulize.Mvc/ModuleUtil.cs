using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wheatech.Modulize.Mvc.Properties;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Mvc
{
    internal static class ModuleUtil
    {
        private static readonly MethodInfo GenerateClientUrlMethod = typeof (TagBuilder).Assembly.GetType("System.Web.WebPages.UrlUtil")
            .GetMethod("GenerateClientUrl", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (HttpContextBase), typeof (string)}, null);
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
            return vpd == null ? null : (string)GenerateClientUrlMethod.Invoke(null, new object[] { requestContext.HttpContext, vpd.VirtualPath });
        }

        public static string GenerateContentUrl(string contentPath, string module, RequestContext requestContext)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(contentPath));
            }
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }
            if (contentPath[0] == '~')
            {
                var moduleDescriptor = !string.IsNullOrEmpty(module) ? Modulizer.GetModule(module) : GetModule(requestContext.RouteData);
                if (moduleDescriptor != null)
                {
                    contentPath = PathHelper.ToAppRelativePath(PathUtils.ResolvePath(moduleDescriptor.ShadowPath, contentPath));
                }
            }
            return UrlHelper.GenerateContentUrl(contentPath, requestContext.HttpContext);
        }
    }
}
