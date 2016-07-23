using System;
using System.Globalization;
using System.Web;
using System.Web.Routing;
using Wheatech.Modulize.Web.Properties;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageRouteHandler : IRouteHandler
    {
        private readonly bool _useRouteVirtualPath;
        private Route _routeVirtualPath;
        private readonly ModulePageHandlerFactory _handlerFactory;

        public ModulePageRouteHandler(string moduleId, string virtualPath)
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(moduleId));
            }
            var module = Modulizer.GetModule(moduleId);
            if (module == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_ModuleNotFound, moduleId));
            }
            _handlerFactory = new ModulePageHandlerFactory(module.ShadowPath);
            VirtualPath = virtualPath;
            _useRouteVirtualPath = !string.IsNullOrEmpty(virtualPath) && VirtualPath.Contains("{");
        }

        /// <summary>
        /// This is the full virtual path (using tilde syntax) to the WebForm page.
        /// </summary>
        /// <remarks>
        /// Needs to be thread safe so this is only settable via ctor.
        /// </remarks>
        public string VirtualPath { get; }

        private Route RouteVirtualPath
            => _routeVirtualPath ?? (_routeVirtualPath = string.IsNullOrEmpty(VirtualPath) ? null : new Route(VirtualPath.StartsWith("~/") ? VirtualPath.Substring(2) : VirtualPath, this));

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }
            string virtualPath = GetSubstitutedVirtualPath(requestContext);
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = (string)requestContext.RouteData.DataTokens["Module_RelativePath"];
            }
            requestContext.RouteData.DataTokens.Remove("Module_RelativePath");
            if (string.IsNullOrEmpty(virtualPath)) return null;
            return _handlerFactory.GetHandler(requestContext.HttpContext, virtualPath);
        }

        /// <summary>
        /// Gets the virtual path to the resource after applying substitutions based on route data.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        private string GetSubstitutedVirtualPath(RequestContext requestContext)
        {
            if (!_useRouteVirtualPath) return VirtualPath;
            var vpd = RouteVirtualPath.GetVirtualPath(requestContext, requestContext.RouteData.Values);
            return vpd == null ? VirtualPath : vpd.VirtualPath;
        }
    }
}
