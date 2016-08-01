using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ModuleAction(this UrlHelper helper, string actionName)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null, null, null, helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null, null, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null,null, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, null, helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, null, helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }
    }
}
