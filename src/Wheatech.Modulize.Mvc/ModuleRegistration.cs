using System;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
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
                var registration = (ModuleRegistration) Activator.CreateInstance(type);
                registration.Module = module;
                registration.CreateContextAndRegister(routes, module, state);
            }
        }

        internal void CreateContextAndRegister(RouteCollection routes, ModuleDescriptor module, object state)
        {
            var context = new ModuleRegistrationContext(module, routes, state);
            string ns = GetType().Namespace;
            if (!string.IsNullOrEmpty(ns))
            {
                context.Namespaces.Add(ns + ".*");
            }
            RegisterModule(context);
        }

        private static bool IsModuleRegistrationType(Type type)
        {
            return typeof(ModuleRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
