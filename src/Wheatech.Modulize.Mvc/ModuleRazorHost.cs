using System.Web.Mvc.Razor;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleRazorHost : MvcWebPageRazorHost
    {
        public ModuleRazorHost(string virtualPath, string physicalPath) : base(virtualPath, physicalPath)
        {
            NamespaceImports.Add("Wheatech.Modulize");
            NamespaceImports.Add("Wheatech.Modulize.Mvc");
            DefaultPageBaseClass = typeof(ModuleViewPage).FullName;
        }
    }
}
