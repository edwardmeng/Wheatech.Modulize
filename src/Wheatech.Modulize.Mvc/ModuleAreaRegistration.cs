using System;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    public abstract class ModuleAreaRegistration
    {
        public abstract void RegisterModuleArea(ModuleAreaRegistrationContext context);

        public abstract string AreaName { get; }

        public ModuleDescriptor Module { get; private set; }

        internal void CreateContextAndRegister(RouteCollection routes, ModuleDescriptor module, object state)
        {
            var context = new ModuleAreaRegistrationContext(module, AreaName, routes, state);
            string ns = GetType().Namespace;
            if (!string.IsNullOrEmpty(ns))
            {
                context.Namespaces.Add(ns + ".*");
            }
            RegisterModuleArea(context);
        }

        private static bool IsModuleAreaRegistrationType(Type type)
        {
            return typeof(ModuleAreaRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static void RegisterModuleAreas(ModuleDescriptor module, object state)
        {
            RegisterModuleAreas(RouteTable.Routes, module, state);
        }

        public static void RegisterModuleAreas(ModuleDescriptor module)
        {
            RegisterModuleAreas(module, null);
        }

        internal static void RegisterModuleAreas(RouteCollection routes, ModuleDescriptor module, object state)
        {
            foreach (Type type in module.FilterTypesInModule(IsModuleAreaRegistrationType))
            {
                var registration = (ModuleAreaRegistration)Activator.CreateInstance(type);
                registration.Module = module;
                registration.CreateContextAndRegister(routes, module, state);
            }
        }
    }
}
