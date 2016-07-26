namespace Wheatech.Modulize.Mvc
{
    internal class MvcModuleRoutingExtension : IModuleContainerExtension
    {
        public void Initialize(IModuleContainer container)
        {
            container.ModuleLoaded += OnModuleLoaded;
        }

        private void OnModuleLoaded(object sender, ModuleEventArgs e)
        {
            ModuleRegistration.RegisterModule(e.Module);
            ModuleAreaRegistration.RegisterModuleAreas(e.Module);
        }

        public void Remove(IModuleContainer container)
        {
            container.ModuleLoaded -= OnModuleLoaded;
        }
    }
}
