using System;
using System.Web.Compilation;

namespace Wheatech.Modulize.Web
{
    internal class ModuleWebAssemblyExtension : IModuleContainerExtension
    {
        public void Initialize(IModuleContainer container)
        {
            container.ModuleLoaded += OnModuleLoaded;
        }

        private void OnModuleLoaded(object sender, ModuleEventArgs e)
        {
            try
            {
                foreach (var assembly in e.Module.Assemblies)
                {
                    BuildManager.AddReferencedAssembly(assembly);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void Remove(IModuleContainer container)
        {
            container.ModuleLoaded -= OnModuleLoaded;
        }
    }
}
