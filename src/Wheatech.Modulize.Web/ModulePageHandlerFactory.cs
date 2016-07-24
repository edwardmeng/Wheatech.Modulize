using System.Web;
using System.Web.UI;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageHandlerFactory : PageHandlerFactory
    {
        private readonly string _modulePath;

        protected internal ModulePageHandlerFactory(string modulePath)
        {
            _modulePath = modulePath;
        }

        public virtual IHttpHandler GetHandler(HttpContextBase context, string virtualPath)
        {
            // Virtual Path ----s up with query strings, so we need to strip them off
            int qmark = virtualPath.IndexOf('?');
            if (qmark != -1)
            {
                virtualPath = virtualPath.Substring(0, qmark);
            }
            virtualPath = PathHelper.ToAppRelativePath(context, PathUtils.ResolvePath(_modulePath, virtualPath));
            return base.GetHandler(context.ApplicationInstance.Context, context.Request.RequestType, virtualPath, null);
        }
    }
}
