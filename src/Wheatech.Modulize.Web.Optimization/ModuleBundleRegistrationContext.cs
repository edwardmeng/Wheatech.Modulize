using System.Web.Optimization;

namespace Wheatech.Modulize.Web.Optimization
{
    public class ModuleBundleRegistrationContext
    {
        public ModuleBundleRegistrationContext(ModuleDescriptor module, BundleCollection bundles, object state)
        {
            Module = module;
            Bundles = bundles;
            State = state;
        }

        public ModuleBundleRegistrationContext(ModuleDescriptor module, BundleCollection bundles) : this(module, bundles, null)
        {
        }

        public ModuleDescriptor Module { get; }

        protected BundleCollection Bundles { get; }

        public object State { get; }

        public ModuleBundle AddScriptBundle(string virtualPath, string cdnPath)
        {
            return new ModuleBundle(Module, new ScriptBundle(virtualPath, cdnPath));
        }

        public ModuleBundle AddScriptBundle(string virtualPath)
        {
            return AddScriptBundle(virtualPath, null);
        }

        public ModuleBundle AddStyleBundle(string virtualPath, string cdnPath)
        {
            return new ModuleBundle(Module, new StyleBundle(virtualPath, cdnPath));
        }

        public ModuleBundle AddStyleBundle(string virtualPath)
        {
            return AddStyleBundle(virtualPath, null);
        }
    }
}
