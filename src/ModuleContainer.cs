using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class ModuleContainer : IModuleContainer
    {
        #region Fields

        private ModuleDescriptor[] _modules;
        private FeatureDescriptor[] _features;
        private ModuleDiscoverCollection _discovers;
        private ModuleLocatorCollection _locators;
        private ManifestTable _manifests;
        private IActivatingEnvironment _environment;

        #endregion

        public ModuleContainer()
        {
            Discovers.Add(new AssemblyModuleDiscover());
            Discovers.Add(new CompressionModuleDiscover());
            Discovers.Add(new DirectoryModuleDiscover());

            Locators.Add(new JsonModuleLocator());
            Locators.Add(new XmlModuleLocator());

            Manifests.SetParser<JsonManifestParser>("manifest.json");
            Manifests.SetParser<XmlManifestParser>("manifest.config");
            Manifests.SetParser<TextManifestParser>("manifest.txt");
        }

        public ModuleDiscoverCollection Discovers => _discovers ?? (_discovers = new ModuleDiscoverCollection());

        public ModuleLocatorCollection Locators => _locators ?? (_locators = new ModuleLocatorCollection());

        public ManifestTable Manifests => _manifests ?? (_manifests = new ManifestTable());

        public IActivationProvider ActivationProvider { get; set; }

        public ModuleDescriptor[] GetModules()
        {
            ValidateStartup();
            return _modules;
        }

        public FeatureDescriptor[] GetFeatures()
        {
            ValidateStartup();
            return _features;
        }

        public void InstallModules(params string[] modules)
        {
            ValidateStartup();
            var moduleDescriptors = _modules.Where(module => modules.Contains(module.ModuleId)).ToArray();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
                if (moduleDescriptor.RuntimeState == ModuleRuntimeState.None && moduleDescriptor.ActivationState == ModuleActivationState.RequireInstall)
                {
                    moduleDescriptor.Install(_environment);
                    ActivationProvider.InstallModule(moduleDescriptor.ModuleId, moduleDescriptor.ModuleVersion);
                }
            }
            StartupModules(moduleDescriptors);
        }

        public void UninstallModules(params string[] modules)
        {
            ValidateStartup();
            var moduleDescriptors = _modules.Where(module => modules.Contains(module.ModuleId)).Reverse().ToArray();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
                if (moduleDescriptor.RuntimeState == ModuleRuntimeState.None && moduleDescriptor.ActivationState == ModuleActivationState.Installed)
                {
                    moduleDescriptor.Uninstall(_environment);
                    ActivationProvider.UninstallModule(moduleDescriptor.ModuleId);
                }
            }
            ShutdownModules(moduleDescriptors);
        }

        public void EnableFeatures(params string[] features)
        {
            ValidateStartup();
            var featureDescriptors = _features.Where(feature => features.Contains(feature.FeatureId)).ToArray();
            foreach (var featureDescriptor in featureDescriptors)
            {
                if (featureDescriptor.RuntimeState == FeatureRuntimeState.None && featureDescriptor.ActivationState == FeatureActivationState.RequireEnable)
                {
                    featureDescriptor.Enable(_environment);
                    ActivationProvider.EnableFeature(featureDescriptor.FeatureId);
                }
            }
        }

        public void DisableFeatures(params string[] features)
        {
            ValidateStartup();
            var featureDescriptors = _features.Where(feature => features.Contains(feature.FeatureId)).Reverse().ToArray();
            foreach (var featureDescriptor in featureDescriptors)
            {
                if (featureDescriptor.RuntimeState == FeatureRuntimeState.None && featureDescriptor.ActivationState == FeatureActivationState.Enabled)
                {
                    featureDescriptor.Disable(_environment);
                    ActivationProvider.DisableFeature(featureDescriptor.FeatureId);
                }
            }
        }

        public void Start(IActivatingEnvironment environment)
        {
            _environment = environment;
            Discovers.SetReadOnly(true);
            Locators.SetReadOnly(true);
            Manifests.SetReadOnly();
            Initialize((from discover in Discovers
                        from location in Locators.SelectMany(locator => locator.GetLocations())
                        from module in discover.Discover(new DiscoverContext { Location = location, Manifest = Manifests })
                        select module).ToArray(), environment);
            Recover(environment);
            StartupModules(_modules);
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveDepedencyAssembly;
        }

        private Assembly OnResolveDepedencyAssembly(object sender, ResolveEventArgs args)
        {
            // If the argument name contains directory seperator, indicates it is an physical path or uri path.
            if (args.Name.Contains('\\') || args.Name.Contains('/'))
            {
                return Assembly.LoadFrom(PathUtils.ResolvePath(args.Name));
            }
            AssemblyIdentity identity;
            if (!AssemblyIdentity.TryParse(args.Name, out identity))
            {
                return Assembly.LoadFrom(args.Name);
            }
            var assembly = ActivationHelper.GetDomainAssembly(identity);
            if (assembly != null) return assembly;
            foreach (var module in _modules)
            {
                if (module.TryLoadAssembly(identity, out assembly))
                {
                    return assembly;
                }
            }
            return null;
        }

        private void ValidateStartup()
        {
            if (_environment == null)
            {
                throw new InvalidOperationException(Strings.Container_NotStart);
            }
        }

        private void InitializeDependencies(ModuleDescriptor[] modules)
        {
            var features = modules.SelectMany(module => module.Features).ToArray();
            var featuresDictionary = features.ToDictionary(feature => feature.FeatureId);
            foreach (var feature in features)
            {
                foreach (var dependency in feature.Dependencies)
                {
                    FeatureDescriptor dependentFeature;
                    if (!featuresDictionary.TryGetValue(dependency.FeatureId, out dependentFeature))
                    {
                        feature.RuntimeState |= FeatureRuntimeState.MissingDependency;
                    }
                    else if (dependency.Version != null && !dependency.Version.Match(dependentFeature.Module.ModuleVersion))
                    {
                        feature.RuntimeState |= FeatureRuntimeState.IncompatibleDependency;
                    }
                    dependency.Feature = dependentFeature;
                    dependentFeature?.Dependings.Add(feature);
                }
            }
            foreach (var feature in features)
            {
                feature.Dependings.SetReadOnly();
            }
        }

        private void Initialize(ModuleDescriptor[] modules, IActivatingEnvironment environment)
        {
            var duplicateModules = (from module in modules
                                    group module by module.ModuleId into g
                                    where g.Count() > 1
                                    select g.Key).ToArray();
            if (duplicateModules.Length > 0)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateModules, string.Join(", ", duplicateModules)));
            }

            var duplicateFeatures = (from module in modules
                                     from feature in module.Features
                                     group feature by feature.FeatureId into g
                                     where g.Count() > 1
                                     select g.Key).ToArray();
            if (duplicateFeatures.Length > 0)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateFeatures, string.Join(", ", duplicateFeatures)));
            }

            // Load the assemblies in module configuration files or bin folder.
            // At this time the reference assemblies will not be requested by the framework.
            foreach (var module in modules)
            {
                module.LoadAssemblies();
            }

            InitializeDependencies(modules);

            foreach (var module in modules)
            {
                module.RefreshRuntimeState(environment);
                module.Initialize(environment);
                foreach (var feature in module.Features)
                {
                    feature.Initialize(environment);
                }
            }
            _features = modules.SelectMany(module => module.Features).Sort();
            _modules = _features.Select(feature => feature.Module).Distinct().ToArray();
        }

        private void Recover(IActivatingEnvironment environment)
        {
            var features = _modules.SelectMany(module => module.Features).Sort();
            foreach (var module in features.Select(feature => feature.Module).Distinct())
            {
                if (module.RuntimeState == ModuleRuntimeState.None)
                {
                    if (module.ActivationState == ModuleActivationState.RequireInstall)
                    {
                        Version installVersion;
                        if (ActivationProvider.GetModuleInstalled(module.ModuleId, out installVersion))
                        {
                            if (module.ModuleVersion > installVersion)
                            {
                                module.Upgrade(environment, installVersion);
                                ActivationProvider.InstallModule(module.ModuleId, module.ModuleVersion);
                            }
                            module.AutoInstalled(environment);
                        }
                    }
                    else if (module.ActivationState == ModuleActivationState.AutoInstall)
                    {
                        module.AutoInstalled(environment);
                    }
                }
            }
            foreach (var feature in features)
            {
                if (feature.RuntimeState == FeatureRuntimeState.None)
                {
                    if (feature.ActivationState == FeatureActivationState.AutoEnable ||
                        (feature.ActivationState == FeatureActivationState.RequireEnable && ActivationProvider.GetFeatureEnabled(feature.FeatureId)))
                    {
                        feature.Enable(environment);
                    }
                }
            }
        }

        private void StartupModules(IEnumerable<ModuleDescriptor> modules)
        {
            foreach (var module in modules)
            {
                if (module.RuntimeState == ModuleRuntimeState.None && module.ActivationState != ModuleActivationState.RequireInstall)
                {
                    ApplicationActivator.Startup(module.GetLoadedAssemblies());
                }
            }
        }

        private void ShutdownModules(IEnumerable<ModuleDescriptor> modules)
        {
            foreach (var module in modules)
            {
                if (module.RuntimeState == ModuleRuntimeState.None)
                {
                    ApplicationActivator.Shutdown(module.GetLoadedAssemblies());
                }
            }
        }
    }
}
