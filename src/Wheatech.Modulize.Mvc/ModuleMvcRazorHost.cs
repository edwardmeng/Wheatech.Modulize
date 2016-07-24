using System.Web.Mvc.Razor;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleMvcRazorHost : MvcWebPageRazorHost
    {
        public ModuleMvcRazorHost(string virtualPath, string physicalPath) : base(virtualPath, physicalPath)
        {
            RegisterSpecialFile("_ModuleStart",typeof(ModuleStartPage));
            NamespaceImports.Add("Wheatech.Modulize");
        }
    }
}
