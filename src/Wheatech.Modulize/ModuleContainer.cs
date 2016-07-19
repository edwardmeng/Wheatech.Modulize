using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
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

        #region DependentFeature

        private class DependentFeature
        {
            public DependentFeature(FeatureDescriptor feature)
            {
                Feature = feature;
            }

            public IList<DependentFeature> Incomings { get; } = new List<DependentFeature>();

            public IList<DependentFeature> Outcommings { get; } = new List<DependentFeature>();

            public FeatureDescriptor Feature { get; }
        }

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
            if (_environment != null)
            {
                throw new InvalidOperationException(Strings.Container_Started);
            }
            return _configuration;
        }

        internal void Reset()
        {
            _modules = null;
            _features = null;
            _environment = null;
            _configuration.SetReadOnly(false);
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
            ValidateModules(moduleIds);
            using (var transaction = new ModulizeTransation())
            {
                foreach (var module in _modules)
                {
                    // If the specified module has errors, it cannot be installed.
                    if (module.Errors != ModuleErrors.None && module.InstallState != ModuleInstallState.Installed && moduleIds.Contains(module.ModuleId))
                    {
                        throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotInstallModule, module.ModuleId));
                    }
                    // Install the specified modules and depending autoinstalled modules.
                    if (module.Errors == ModuleErrors.None && module.InstallState != ModuleInstallState.Installed)
                    {
                        bool installed = false;
                        var moduleId = module.ModuleId;
                        var moduleVersion = module.ModuleVersion;
                        if (!module.HasInstallers)
                        {
                            module.Install(_environment, transaction);
                            installed = true;
                        }
                        else if (module.HasInstallers && moduleIds.Contains(moduleId))
                        {
                            module.Install(_environment, transaction, () =>
                            {
                                _configuration.PersistProvider.InstallModule(moduleId, moduleVersion);
                            });
                            installed = true;
                        }
                        if (installed)
                        {
                            EnableFeatures(transaction, module.Features.Select(feature => feature.FeatureId).ToArray(), false);
                        }
                    }
                }
                transaction.Complete();
            }
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
            ValidateModules(moduleIds);
            using (var transaction = new ModulizeTransation())
            {
                foreach (var module in _modules.Reverse())
                {
                    if (module.InstallState == ModuleInstallState.Installed && module.HasUninstallers && moduleIds.Contains(module.ModuleId))
                    {
                        var blockModules = module.Features
                            .SelectMany(feature => feature.GetDependingFeatures())
                            .Where(depending => depending.CanDisable && depending.EnableState == FeatureEnableState.Enabled && depending.Module.HasInstallers)
                            .Select(feature => feature.Module).Distinct().ToArray();
                        if (blockModules.Length != 0)
                        {
                            throw new ModuleDependencyException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotUninstallModule, module.ModuleId,
                                string.Join(", ", blockModules.Select(x => x.ModuleId))));
                        }
                        // Disable features underlying module before uninstall module.
                        foreach (var feature in _features.Reverse())
                        {
                            if (feature.EnableState == FeatureEnableState.Enabled && module.Features.Contains(feature))
                            {
                                DisableFeature(transaction, feature, true);
                            }
                        }
                        var moduleId = module.ModuleId;
                        module.Uninstall(_environment, transaction, () =>
                         {
                             _configuration.PersistProvider.UninstallModule(moduleId);
                         });
                    }
                }
                transaction.Complete();
            }
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
            ValidateFeatures(featureIds);
            using (var transaction = new ModulizeTransation())
            {
                EnableFeatures(transaction, featureIds, true);
                transaction.Complete();
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
            ValidateFeatures(featureIds);
            using (var transaction = new ModulizeTransation())
            {
                foreach (var feature in _features.Reverse())
                {
                    if (feature.CanDisable && feature.EnableState == FeatureEnableState.Enabled && featureIds.Contains(feature.FeatureId))
                    {
                        var blockFeatures = feature.GetDependingFeatures().Where(depending => depending.CanDisable && depending.EnableState == FeatureEnableState.Enabled).ToArray();
                        if (blockFeatures.Length != 0)
                        {
                            throw new ModuleDependencyException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotDisableFeature, feature.FeatureId,
                                string.Join(", ", blockFeatures.Select(x => x.FeatureId))));
                        }
                        foreach (var depending in feature.Dependings)
                        {
                            DisableFeature(transaction, depending, false);
                        }
                        var featureId = feature.FeatureId;
                        feature.Disable(_environment, transaction, () =>
                        {
                            _configuration.PersistProvider.DisableFeature(featureId);
                        });
                    }
                }
                transaction.Complete();
            }
        }

        private bool DisableFeature(ModulizeTransation transation, FeatureDescriptor feature, bool force)
        {
            if (feature.EnableState != FeatureEnableState.Enabled) return true;
            if (force || !feature.CanDisable)
            {
                if (feature.Dependings.Any(depending => !DisableFeature(transation, depending, force)))
                {
                    return false;
                }
                Action callback = null;
                if (feature.CanDisable)
                {
                    var featureId = feature.FeatureId;
                    callback = () => _configuration.PersistProvider.DisableFeature(featureId);
                }
                feature.Disable(_environment, transation, callback);
                return true;
            }
            return false;
        }

        private void EnableFeatures(ModulizeTransation transation, string[] features, bool force)
        {
            foreach (var feature in _features)
            {
                if (force && feature.Errors != FeatureErrors.None && feature.EnableState != FeatureEnableState.Enabled && features.Contains(feature.FeatureId))
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotEnableFeature, feature.FeatureId));
                }
                if (feature.Errors == FeatureErrors.None && feature.EnableState != FeatureEnableState.Enabled)
                {
                    if (!feature.CanEnable)
                    {
                        feature.Enable(_environment, transation);
                    }
                    else if (feature.CanEnable && features.Contains(feature.FeatureId))
                    {
                        var featureId = feature.FeatureId;
                        feature.Enable(_environment, transation, () =>
                        {
                            _configuration.PersistProvider.EnableFeature(featureId);
                        });
                    }
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
            ValidateConfiguration();
            _environment = environment;
            _configuration.SetReadOnly(true);
            var locations = _configuration.Locators.SelectMany(locator => locator.GetLocations()).ToArray();
            var discovers = _configuration.Discovers.ToArray();
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveDepedencyAssembly;
            Initialize((from location in locations
                        from discover in discovers
                        from module in discover.Discover(new DiscoverContext { Location = location, Manifest = _configuration.Manifests, ShadowPath = _configuration.ShadowPath })
                        select module).ToArray(), environment);
            Recover(environment);
            StartupModules(_modules);

            // Attach events to shutdown the application.
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            typeof(HttpRuntime).GetEvent("AppDomainShutdown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
                .AddMethod.Invoke(null, new object[] { new BuildManagerHostUnloadEventHandler(OnAppDomainShutdown) });
            System.Web.Hosting.HostingEnvironment.StopListening += OnStopListening;
        }

        private void RemoveEventHandlers()
        {
            AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
            System.Web.Hosting.HostingEnvironment.StopListening -= OnStopListening;
            typeof(HttpRuntime).GetEvent("AppDomainShutdown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
              .RemoveMethod.Invoke(null, new object[] { new BuildManagerHostUnloadEventHandler(OnAppDomainShutdown) });
        }

        private void OnDomainUnload(object sender, EventArgs e)
        {
            Dispose();
        }

        private void OnStopListening(object sender, EventArgs e)
        {
            Dispose();
        }

        private void OnAppDomainShutdown(object sender, BuildManagerHostUnloadEventArgs args)
        {
            Dispose();
        }

        #region Validation

        private void ValidateStarted()
        {
            if (_environment == null)
            {
                throw new InvalidOperationException(Strings.Container_NotStart);
            }
        }

        private void ValidateConfiguration()
        {
            if (_configuration.PersistProvider == null)
            {
                throw new ModuleConfigurationException(Strings.Configuration_MissingPersistProvider);
            }
            if (string.IsNullOrEmpty(_configuration.ShadowPath))
            {
                _configuration.ShadowPath = "~/Temporary Modulize Files";
            }
        }

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("ModuleContainer");
            }
        }

        private void ValidateModules(string[] modules)
        {
            var missingModuleIds = modules.Where(moduleId => _modules.All(module => module.ModuleId != moduleId)).ToArray();
            if (missingModuleIds.Length > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_MissingModules, string.Join(", ", missingModuleIds)));
            }
        }

        private void ValidateFeatures(string[] features)
        {
            var missingFeatureIds = features.Where(featureId => _features.All(feature => feature.FeatureId != featureId)).ToArray();
            if (missingFeatureIds.Length > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_MissingFeatures, string.Join(", ", missingFeatureIds)));
            }
        }

        #endregion

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
            var assembly = ActivationHelper.GetDomainAssembly(_environment, identity);
            if (assembly != null) return assembly;
            return _modules.Any(module => module.TryLoadAssembly(_environment, identity, out assembly)) ? assembly : null;
        }

        private void Initialize(ModuleDescriptor[] modules, IActivatingEnvironment environment)
        {
            var duplicateModules = (from module in modules
                                    group module by module.ModuleId into g
                                    where g.Count() > 1
                                    select g.Key).ToArray();
            if (duplicateModules.Length > 0)
            {
                throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateModules, string.Join(", ", duplicateModules)));
            }

            var duplicateFeatures = (from module in modules
                                     from feature in module.Features
                                     group feature by feature.FeatureId into g
                                     where g.Count() > 1
                                     select g.Key).ToArray();
            if (duplicateFeatures.Length > 0)
            {
                throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateFeatures, string.Join(", ", duplicateFeatures)));
            }

            _features = SortFeatures(modules);
            _modules = _features.Select(feature => feature.Module).Distinct().ToArray();

            // Load the assemblies in module configuration files or bin folder.
            // At this time the reference assemblies will not be requested by the framework.
            foreach (var module in _modules)
            {
                module.LoadAssemblies(environment);
            }

            foreach (var module in modules)
            {
                module.Initialize(environment);
                module.RefreshErrors(environment);
                foreach (var feature in module.Features)
                {
                    feature.Initialize(environment);
                }
            }
        }

        private void Recover(IActivatingEnvironment environment)
        {
            var enabledFeatures = new ConcurrentDictionary<string, bool>();
            foreach (var module in _modules)
            {
                if (module.Errors == ModuleErrors.None)
                {
                    if (module.HasInstallers)
                    {
                        Version installVersion;
                        if (_configuration.PersistProvider.GetModuleInstalled(module.ModuleId, out installVersion))
                        {
                            if (module.ModuleVersion > installVersion)
                            {
                                module.Upgrade(environment, installVersion);
                                _configuration.PersistProvider.InstallModule(module.ModuleId, module.ModuleVersion);
                            }
                            module.Install(environment);
                        }
                    }
                    else if (module.InstallState != ModuleInstallState.Installed)
                    {
                        module.Install(environment);
                    }
                    foreach (var feature in _features)
                    {
                        if (feature.Errors == FeatureErrors.None &&
                            (!feature.CanEnable ||
                             (feature.EnableState == FeatureEnableState.RequireEnable &&
                              enabledFeatures.GetOrAdd(feature.FeatureId, featureId => _configuration.PersistProvider.GetFeatureEnabled(featureId)))))
                        {
                            feature.Enable(environment);
                        }
                    }
                }
            }
        }

        private void StartupModules(IEnumerable<ModuleDescriptor> modules)
        {
            var assemblies = new List<Assembly>();
            foreach (var module in modules)
            {
                if (module.Errors == ModuleErrors.None && module.InstallState != ModuleInstallState.RequireInstall)
                {
                    assemblies.AddRange(module.GetLoadedAssemblies());
                }
            }
            if (assemblies.Count > 0)
            {
                ApplicationActivator.Startup(assemblies.Distinct().ToArray());
            }
        }

        private void ShutdownModules(IEnumerable<ModuleDescriptor> modules)
        {
            foreach (var module in modules)
            {
                if (module.Errors == ModuleErrors.None)
                {
                    ApplicationActivator.Shutdown(module.GetLoadedAssemblies());
                }
            }
        }

        private IEnumerable<DependentFeature> CreateFeatureGraph(IEnumerable<ModuleDescriptor> modules)
        {
            var nodes = modules.SelectMany(module => module.Features).ToDictionary(feature => feature.FeatureId, feature => new DependentFeature(feature));
            foreach (var feature in nodes.Values)
            {
                foreach (var dependency in feature.Feature.Dependencies)
                {
                    DependentFeature dependentFeature;
                    if (!nodes.TryGetValue(dependency.FeatureId, out dependentFeature))
                    {
                        feature.Feature.Errors |= FeatureErrors.MissingDependency;
                    }
                    else if (dependency.Version != null && !dependency.Version.Match(dependentFeature.Feature.Module.ModuleVersion))
                    {
                        feature.Feature.Errors |= FeatureErrors.IncompatibleDependency;
                    }
                    if (dependentFeature != null)
                    {
                        dependency.Feature = dependentFeature.Feature;
                        dependentFeature.Feature.Dependings.Add(feature.Feature);
                        dependentFeature.Incomings.Add(feature);
                        feature.Outcommings.Add(dependentFeature);
                    }
                }
            }
            foreach (var feature in nodes.Values)
            {
                feature.Feature.Dependings.SetReadOnly();
            }
            return nodes.Values;
        }

        private FeatureDescriptor[] SortFeatures(IEnumerable<ModuleDescriptor> modules)
        {
            var graph = CreateFeatureGraph(modules).ToList();
            var sorted = new List<FeatureDescriptor>();
            var queue = new Queue<DependentFeature>(graph.Where(node => node.Outcommings.Count == 0));
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Outcommings.Count == 0)
                {
                    foreach (var incomeNode in node.Incomings)
                    {
                        incomeNode.Outcommings.Remove(node);
                        queue.Enqueue(incomeNode);
                    }
                    sorted.Add(node.Feature);
                    graph.Remove(node);
                }
            }
            if (graph.Count > 0)
            {
                var circle = new List<DependentFeature>();
                var currentNode = graph[0];
                while (!circle.Contains(currentNode))
                {
                    circle.Add(currentNode);
                    currentNode = currentNode.Outcommings.First();
                }
                throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CircleDependency,
                    string.Join(", ", circle.Select(node => node.Feature.FeatureId))));
            }
            return sorted.ToArray();
        }

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            RemoveEventHandlers();
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnResolveDepedencyAssembly;
                ShutdownModules(from module in _modules
                                where module.InstallState != ModuleInstallState.RequireInstall && module.Errors == ModuleErrors.None
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
