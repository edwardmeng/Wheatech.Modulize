using System;
using System.Web.Optimization;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web.Optimization
{
    public abstract class ModuleBundleRegistration
    {
        public ModuleDescriptor Module { get; private set; }

        public abstract void RegisterBundles(ModuleBundleRegistrationContext context);

        public static void RegisterModule(ModuleDescriptor module, object state)
        {
            RegisterModule(BundleTable.Bundles, module, state);
        }

        public static void RegisterModule(ModuleDescriptor module)
        {
            RegisterModule(module, null);
        }

        internal static void RegisterModule(BundleCollection bundles, ModuleDescriptor module, object state)
        {
            foreach (Type type in module.FilterTypesInModule(IsModuleRegistrationType))
            {
                var registration = (ModuleBundleRegistration)Activator.CreateInstance(type);
                registration.Module = module;
                registration.CreateContextAndRegister(bundles, module, state);
            }
        }

        internal void CreateContextAndRegister(BundleCollection bundles, ModuleDescriptor module, object state)
        {
            RegisterBundles(new ModuleBundleRegistrationContext(module, bundles, state));
        }

        private static bool IsModuleRegistrationType(Type type)
        {
            return typeof(ModuleBundleRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
