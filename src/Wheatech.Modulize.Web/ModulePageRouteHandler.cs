using System;
using System.Web;
using System.Web.Routing;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageRouteHandler : IRouteHandler
    {
        private readonly bool _useRouteVirtualPath;
        private Route _routeVirtualPath;
        private readonly ModulePageHandlerFactory _handlerFactory;

        public ModulePageRouteHandler(ModuleDescriptor module, string virtualPath)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            _handlerFactory = new ModulePageHandlerFactory(module);
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
