using System;
using System.Web.Routing;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web
{
    public abstract class ModuleRegistration
    {
        public ModuleDescriptor Module { get; private set; }

        public abstract void RegisterModule(ModuleRegistrationContext context);

        public static void RegisterModule(ModuleDescriptor module, object state)
        {
            RegisterModule(RouteTable.Routes, module, state);
        }

        public static void RegisterModule(ModuleDescriptor module)
        {
            RegisterModule(module, null);
        }

        internal static void RegisterModule(RouteCollection routes, ModuleDescriptor module, object state)
        {
            foreach (Type type in module.FilterTypesInModule(IsModuleRegistrationType))
            {
                var registration = (ModuleRegistration)Activator.CreateInstance(type);
                registration.Module = module;
                registration.CreateContextAndRegister(routes, module, state);
            }
        }

        internal void CreateContextAndRegister(RouteCollection routes, ModuleDescriptor module, object state)
        {
            RegisterModule(new ModuleRegistrationContext(module, routes, state));
        }

        private static bool IsModuleRegistrationType(Type type)
        {
            return typeof(ModuleRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
