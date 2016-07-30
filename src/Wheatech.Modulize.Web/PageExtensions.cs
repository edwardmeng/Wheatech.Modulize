using System;
using System.Web.UI;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web
{
    public static class PageExtensions
    {
        public static string ResolveModuleUrl(this Page page, string relativeUrl)
        {
            if (relativeUrl == null)
            {
                throw new ArgumentNullException(nameof(relativeUrl));
            }
            var module = (ModuleDescriptor)page.Items[ModulePageHandlerFactory.PageContextKey];
            if (module != null && (relativeUrl.StartsWith("~/") || relativeUrl.StartsWith("~\\")))
            {
                return page.ResolveUrl(PathHelper.ToAppRelativePath(module.ShadowPath)) + "/" + relativeUrl.Substring(2);
            }
            return page.ResolveUrl(relativeUrl);
        }
    }
}
