using System;
using System.Web.Routing;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Mvc
{
    public abstract class ModuleAreaRouteRegistration
    {
        public abstract void RegisterRoutes(ModuleAreaRouteRegistrationContext context);

        public abstract string AreaName { get; }

        public ModuleDescriptor Module { get; private set; }

        internal void CreateContextAndRegister(RouteCollection routes, ModuleDescriptor module, object state)
        {
            var context = new ModuleAreaRouteRegistrationContext(module, AreaName, routes, state);
            string ns = GetType().Namespace;
            if (!string.IsNullOrEmpty(ns))
            {
                context.Namespaces.Add(ns + ".*");
            }
            RegisterRoutes(context);
        }

        private static bool IsModuleAreaRegistrationType(Type type)
        {
            return typeof(ModuleAreaRouteRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
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
                var registration = (ModuleAreaRouteRegistration)Activator.CreateInstance(type);
                registration.Module = module;
                registration.CreateContextAndRegister(routes, module, state);
            }
        }
    }
}
