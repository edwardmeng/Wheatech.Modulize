namespace Wheatech.Modulize.Web.Optimization
{
    internal class ModuleBundleExtension : IModuleContainerExtension
    {
        public void Initialize(IModuleContainer container)
        {
            container.ModuleLoaded += OnModuleLoaded;
        }

        private void OnModuleLoaded(object sender, ModuleEventArgs e)
        {
            ModuleBundleRegistration.RegisterModule(e.Module);
        }

        public void Remove(IModuleContainer container)
        {
            container.ModuleLoaded -= OnModuleLoaded;
        }
    }
}
