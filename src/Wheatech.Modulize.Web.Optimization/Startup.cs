namespace Wheatech.Modulize.Web.Optimization
{
    internal class Startup
    {
        public void Configuration(IModuleContainer container)
        {
            container.AddExtension<ModuleBundleExtension>();
        }
    }
}
