using System.Web.WebPages.Razor;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleRazorHostFactory : WebRazorHostFactory
    {
        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            var host = base.CreateHost(virtualPath, physicalPath);
            if (!host.IsSpecialPage)
            {
                return new ModuleMvcRazorHost(virtualPath, physicalPath);
            }
            return host;
        }
    }
}
