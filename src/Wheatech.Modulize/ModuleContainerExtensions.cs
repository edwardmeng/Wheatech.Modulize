using System;

namespace Wheatech.Modulize
{
    public static class ModuleContainerExtensions
    {
        public static void InstallModules(this IModuleContainer container, params string[] modules)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.InstallModules(modules);
        }

        public static void UninstallModules(this IModuleContainer container, params string[] modules)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.UninstallModules(modules);
        }

        public static void EnableFeatures(this IModuleContainer container, params string[] features)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.EnableFeatures(features);
        }

        public static void DisableFeatures(this IModuleContainer container, params string[] features)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.DisableFeatures(features);
        }

        public static void AddExtension<TExtension>(this IModuleContainer container)
            where TExtension : IModuleContainerExtension, new()
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            container.AddExtension(new TExtension());
        }
    }
}
