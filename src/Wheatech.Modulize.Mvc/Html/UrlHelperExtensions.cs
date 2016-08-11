using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/>. 
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Converts a virtual (module relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <param name="module">The module ID.</param>
        /// <returns>The application absolute path.</returns>
        public static string ModuleContent(this UrlHelper helper, string contentPath, string module)
        {
            return ModuleUtil.GenerateContentUrl(contentPath, module, helper.RequestContext);
        }

        /// <summary>
        /// Converts a virtual (module relative) path to an application absolute path.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <returns>The application absolute path.</returns>
        public static string ModuleContent(this UrlHelper helper, string contentPath)
        {
            return ModuleUtil.GenerateContentUrl(contentPath, null, helper.RequestContext);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName)
        {
            return ModuleUtil.GenerateUrl(null, actionName, null, null, null, helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, object routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, null, null, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, RouteValueDictionary routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, null, null, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and controller name.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, null, null, helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, object routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, null, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, null, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name and module ID.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, module, null, helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, module ID, and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module, object routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, module, new RouteValueDictionary(routeValues), helper.RouteCollection, helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, module ID, and route values.
        /// </summary>
        /// <param name="helper">The <see cref="UrlHelper"/>.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public static string ModuleAction(this UrlHelper helper, string actionName, string controllerName, string module, RouteValueDictionary routeValues)
        {
            return ModuleUtil.GenerateUrl(null, actionName, controllerName, module, routeValues, helper.RouteCollection, helper.RequestContext, true);
        }
    }
}
