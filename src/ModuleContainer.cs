using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class ModuleContainer : IModuleContainer, IDisposable
    {
        #region Fields

        private ModuleConfiguration _configuration;
        private ModuleDescriptor[] _modules;
        private FeatureDescriptor[] _features;
        private IActivatingEnvironment _environment;
        private bool _disposed;

        #endregion

        /// <summary>
        /// Initialize new instance of <see cref="ModuleContainer"/>.
        /// </summary>
        public ModuleContainer()
        {
            _configuration = new ModuleConfiguration();
            _configuration.Discovers.Add(new AssemblyModuleDiscover());
            _configuration.Discovers.Add(new CompressionModuleDiscover());
            _configuration.Discovers.Add(new DirectoryModuleDiscover());

            _configuration.Locators.Add(new JsonModuleLocator());
            _configuration.Locators.Add(new XmlModuleLocator());

            _configuration.Manifests.SetParser<JsonManifestParser>("manifest.json");
            _configuration.Manifests.SetParser<XmlManifestParser>("manifest.config");
            _configuration.Manifests.SetParser<TextManifestParser>("manifest.txt");
        }

        /// <summary>
        /// Gets an instance of <see cref="IModuleConfiguration"/> used to configure the module container.
        /// </summary>
        /// <returns>An instance of <see cref="IModuleConfiguration"/> to configure the module container.</returns>
        /// <exception cref="InvalidOperationException">The container has been started.</exception>
        public IModuleConfiguration Configure()
        {
            ValidateDisposed();
            if (_environment == null)
            {
                throw new InvalidOperationException(Strings.Container_Started);
            }
            return _configuration;
        }

        /// <summary>
        /// Gets all the discovered modules.
        /// </summary>
        /// <returns>All the discovered modules.</returns>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public ModuleDescriptor[] GetModules()
        {
            ValidateDisposed();
            ValidateStarted();
            return _modules;
        }

        /// <summary>
        /// Gets all the discovered features.
        /// </summary>
        /// <returns>All the discovered features.</returns>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public FeatureDescriptor[] GetFeatures()
        {
            ValidateDisposed();
            ValidateStarted();
            return _features;
        }

        /// <summary>
        /// Installs modules by using an <see cref="IEnumerable{T}"/> of module IDs.
        /// </summary>
        /// <param name="modules">The module IDs of the modules to be installed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public void InstallModules(IEnumerable<string> modules)
        {
            ValidateDisposed();
            ValidateStarted();
            if (modules == null)
            {
                throw new ArgumentNullException(nameof(modules));
            }
            var moduleIds = modules.ToArray();
            var moduleDescriptors = _modules.Where(module => moduleIds.Contains(module.ModuleId)).ToArray();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
                if (moduleDescriptor.RuntimeState == ModuleRuntimeState.None && moduleDescriptor.ActivationState == ModuleActivationState.RequireInstall)
                {
                    moduleDescriptor.Install(_environment);
                    _configuration.PersistProvider.InstallModule(moduleDescriptor.ModuleId, moduleDescriptor.ModuleVersion);
                }
            }
            StartupModules(moduleDescriptors);
        }

        /// <summary>
        /// Uninstalls modules by using an <see cref="IEnumerable{T}"/> of module IDs.
        /// </summary>
        /// <param name="modules">The module IDs of the modules to be uninstalled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public void UninstallModules(IEnumerable<string> modules)
        {
            ValidateDisposed();
            ValidateStarted();
            if (modules == null)
            {
                throw new ArgumentNullException(nameof(modules));
            }
            var moduleIds = modules.ToArray();
            var moduleDescriptors = _modules.Where(module => moduleIds.Contains(module.ModuleId)).Reverse().ToArray();
            foreach (var moduleDescriptor in moduleDescriptors)
            {
                if (moduleDescriptor.RuntimeState == ModuleRuntimeState.None && moduleDescriptor.ActivationState == ModuleActivationState.Installed)
                {
                    moduleDescriptor.Uninstall(_environment);
                    _configuration.PersistProvider.UninstallModule(moduleDescriptor.ModuleId);
                }
            }
            ShutdownModules(moduleDescriptors);
        }

        /// <summary>
        /// Enables features by using an <see cref="IEnumerable{T}"/> of feature IDs.
        /// </summary>
        /// <param name="features">The feature IDs of the feature to be enabled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="features"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public void EnableFeatures(IEnumerable<string> features)
        {
            ValidateDisposed();
            ValidateStarted();
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }
            var featureIds = features.ToArray();
            var featureDescriptors = _features.Where(feature => featureIds.Contains(feature.FeatureId)).ToArray();
            foreach (var featureDescriptor in featureDescriptors)
            {
                if (featureDescriptor.RuntimeState == FeatureRuntimeState.None && featureDescriptor.ActivationState == FeatureActivationState.RequireEnable)
                {
                    featureDescriptor.Enable(_environment);
                    _configuration.PersistProvider.EnableFeature(featureDescriptor.FeatureId);
                }
            }
        }

        /// <summary>
        /// Disables features by using an <see cref="IEnumerable{T}"/> of feature IDs.
        /// </summary>
        /// <param name="features">The feature IDs of the feature to be disabled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="features"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public void DisableFeatures(IEnumerable<string> features)
        {
            ValidateDisposed();
            ValidateStarted();
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }
            var featureIds = features.ToArray();
            var featureDescriptors = _features.Where(feature => featureIds.Contains(feature.FeatureId)).Reverse().ToArray();
            foreach (var featureDescriptor in featureDescriptors)
            {
                if (featureDescriptor.RuntimeState == FeatureRuntimeState.None && featureDescriptor.ActivationState == FeatureActivationState.Enabled)
                {
                    featureDescriptor.Disable(_environment);
                    _configuration.PersistProvider.DisableFeature(featureDescriptor.FeatureId);
                }
            }
        }

        /// <summary>
        /// Starts the module container by using the specified <see cref="IActivatingEnvironment"/>.
        /// </summary>
        /// <param name="environment">The <see cref="IActivatingEnvironment"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="environment"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The container has been started.</exception>
        public void Start(IActivatingEnvironment environment)
        {
            ValidateDisposed();
            if (_environment != null)
            {
                throw new InvalidOperationException(Strings.Container_StartAgain);
            }
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            _environment = environment;
            _configuration.SetReadOnly();
            Initialize((from discover in _configuration.Discovers
                        from location in _configuration.Locators.SelectMany(locator => locator.GetLocations())
                        from module in discover.Discover(new DiscoverContext { Location = location, Manifest = _configuration.Manifests, ShadowPath = _configuration.ShadowPath })
                        select module).ToArray(), environment);
            Recover(environment);
            StartupModules(_modules);
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveDepedencyAssembly;
        }

        private void ValidateStarted()
        {
            if (_environment == null)
            {
                throw new InvalidOperationException(Strings.Container_NotStart);
            }
        }

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("ModuleContainer");
            }
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
                        if (_configuration.PersistProvider.GetModuleInstalled(module.ModuleId, out installVersion))
                        {
                            if (module.ModuleVersion > installVersion)
                            {
                                module.Upgrade(environment, installVersion);
                                _configuration.PersistProvider.InstallModule(module.ModuleId, module.ModuleVersion);
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
                        (feature.ActivationState == FeatureActivationState.RequireEnable && _configuration.PersistProvider.GetFeatureEnabled(feature.FeatureId)))
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

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ShutdownModules(from module in _modules
                                where module.ActivationState != ModuleActivationState.RequireInstall && module.RuntimeState == ModuleRuntimeState.None
                                select module);
                _configuration.Dispose(disposing);
                _configuration = null;
                _modules = null;
                _features = null;
                _environment = null;
            }
        }
    }
}
