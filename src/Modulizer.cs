using System.Collections.Generic;
using Wheatech.Activation;

namespace Wheatech.Modulize
{
    public static class Modulizer
    {
        private static readonly System.Lazy<IModuleContainer> _container = new System.Lazy<IModuleContainer>(() => new ModuleContainer());

        internal static IModuleContainer DefaultContainer => _container.Value;

        public static IModuleConfiguration Configure()
        {
            return DefaultContainer.Configure();
        }

        public static ModuleDescriptor[] GetModules()
        {
            return DefaultContainer.GetModules();
        }

        public static FeatureDescriptor[] GetFeatures()
        {
            return DefaultContainer.GetFeatures();
        }

        public static void InstallModules(IEnumerable<string> modules)
        {
            DefaultContainer.InstallModules(modules);
        }

        public static void InstallModules(params string[] modules)
        {
            DefaultContainer.InstallModules(modules);
        }

        public static void UninstallModules(IEnumerable<string> modules)
        {
            DefaultContainer.UninstallModules(modules);
        }

        public static void UninstallModules(params string[] modules)
        {
            DefaultContainer.UninstallModules(modules);
        }

        public static void EnableFeatures(IEnumerable<string> features)
        {
            DefaultContainer.EnableFeatures(features);
        }

        public static void EnableFeatures(params string[] features)
        {
            DefaultContainer.EnableFeatures(features);
        }

        public static void DisableFeatures(IEnumerable<string> features)
        {
            DefaultContainer.DisableFeatures(features);
        }

        public static void DisableFeatures(params string[] features)
        {
            DefaultContainer.DisableFeatures(features);
        }

        public static void Start(IActivatingEnvironment environment)
        {
            DefaultContainer.Start(environment);
        }
    }
}
