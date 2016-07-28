using System.Web;
using System.Web.UI;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageHandlerFactory : PageHandlerFactory
    {
        private readonly ModuleDescriptor _module;
        internal static readonly object PageContextKey = new object();

        protected internal ModulePageHandlerFactory(ModuleDescriptor module)
        {
            _module = module;
        }

        public virtual IHttpHandler GetHandler(HttpContextBase context, string virtualPath)
        {
            // Virtual Path ----s up with query strings, so we need to strip them off
            int qmark = virtualPath.IndexOf('?');
            if (qmark != -1)
            {
                virtualPath = virtualPath.Substring(0, qmark);
            }
            virtualPath = PathHelper.ToAppRelativePath(context, PathUtils.ResolvePath(_module.ShadowPath, virtualPath));
            var page = (Page)base.GetHandler(context.ApplicationInstance.Context, context.Request.RequestType, virtualPath, null);
            if (page != null)
            {
                page.Items[PageContextKey] = _module;
            }
            return page;
        }
    }
}
