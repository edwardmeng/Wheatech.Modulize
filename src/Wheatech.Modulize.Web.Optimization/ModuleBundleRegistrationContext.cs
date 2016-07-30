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
            var bundle = new ScriptBundle(virtualPath, cdnPath);
            Bundles.Add(bundle);
            return new ModuleBundle(Module, bundle);
        }

        public ModuleBundle AddScriptBundle(string virtualPath)
        {
            return AddScriptBundle(virtualPath, null);
        }

        public ModuleBundle AddStyleBundle(string virtualPath, string cdnPath)
        {
            var bundle = new StyleBundle(virtualPath,cdnPath);
            Bundles.Add(bundle);
            return new ModuleBundle(Module, bundle);
        }

        public ModuleBundle AddStyleBundle(string virtualPath)
        {
            return AddStyleBundle(virtualPath, null);
        }
    }
}
