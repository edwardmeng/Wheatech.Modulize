using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    /// <summary>
    /// Extension methods for <see cref="UrlHelper"/>. 
    /// </summary>
    public class ModuleUrlHelper
    {
        private readonly UrlHelper _helper;

        internal ModuleUrlHelper(UrlHelper helper)
        {
            _helper = helper;
        }

        #region Content

        /// <summary>
        /// Converts a virtual (module relative) path to an application absolute path.
        /// </summary>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <param name="module">The module ID.</param>
        /// <returns>The application absolute path.</returns>
        public string Content(string contentPath, string module)
        {
            return ModuleHelper.GenerateContentUrl(contentPath, module, _helper.RequestContext);
        }

        /// <summary>
        /// Converts a virtual (module relative) path to an application absolute path.
        /// </summary>
        /// <param name="contentPath">The virtual path of the content.</param>
        /// <returns>The application absolute path.</returns>
        public string Content(string contentPath)
        {
            return ModuleHelper.GenerateContentUrl(contentPath, null, _helper.RequestContext);
        }

        #endregion

        #region Action

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null, null, null, _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null, null, new RouteValueDictionary(routeValues), _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, null, null, routeValues, _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name and controller name.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, null, _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, new RouteValueDictionary(routeValues), _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, null, routeValues, _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name and module ID.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName, string module)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, null, _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, module ID, and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName, string module, object routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, new RouteValueDictionary(routeValues), _helper.RouteCollection, _helper.RequestContext, true);
        }

        /// <summary>
        /// Generates a fully qualified URL to a module action method by using the specified action name, controller name, module ID, and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The fully qualified URL to a module action method.</returns>
        public string Action(string actionName, string controllerName, string module, RouteValueDictionary routeValues)
        {
            return ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues, _helper.RouteCollection, _helper.RequestContext, true);
        }

        #endregion
    }
}
