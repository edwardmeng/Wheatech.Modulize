using System;
using System.Collections.Generic;
using Wheatech.Activation;

namespace Wheatech.Modulize
{
    public static class Modulizer
    {
        private static System.Lazy<IModuleContainer> _container;

        internal static IModuleContainer DefaultContainer
        {
            get
            {
                if (_container == null)
                {
                    _container = new System.Lazy<IModuleContainer>(() => new ModuleContainer());
                }
                return _container.Value;
            }
        }

        public static IModuleConfiguration Configure()
        {
            return DefaultContainer.Configure();
        }

        public static ModuleDescriptor[] GetModules(string moduleType = null)
        {
            return DefaultContainer.GetModules(moduleType);
        }

        /// <summary>
        /// Gets the module with specified module ID.
        /// </summary>
        /// <param name="moduleId">The specified module ID to lookup module.</param>
        /// <returns>The <see cref="ModuleDescriptor"/> if the module exists; otherwise, null.</returns>
        public static ModuleDescriptor GetModule(string moduleId)
        {
            return DefaultContainer.GetModule(moduleId);
        }

        public static FeatureDescriptor[] GetFeatures()
        {
            return DefaultContainer.GetFeatures();
        }

        /// <summary>
        /// Gets the feature with specified feature ID.
        /// </summary>
        /// <param name="featureId">The specified feature ID to lookup feature.</param>
        /// <returns>The <see cref="FeatureDescriptor"/> if the feature exists; otherwise, null.</returns>
        public static FeatureDescriptor GetFeature(string featureId)
        {
            return DefaultContainer.GetFeature(featureId);
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

        internal static void Reset()
        {
            if (_container != null && _container.IsValueCreated)
            {
                ((IDisposable)_container.Value).Dispose();
                _container = null;
            }
        }
    }
}
