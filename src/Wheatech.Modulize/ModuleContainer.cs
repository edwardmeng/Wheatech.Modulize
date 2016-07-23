using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static readonly object EventModuleLoaded = new object();
        private static readonly object EventModuleUnloaded = new object();
        private static readonly object EventModuleInstalled = new object();
        private static readonly object EventModuleUninstalled = new object();
        private static readonly object EventFeatureEnabled = new object();
        private static readonly object EventFeatureDisabled = new object();
        private EventHandlerList _events;
        private ModuleConfiguration _configuration;
        private ModuleDescriptor[] _modules;
        private IDictionary<string, ModuleDescriptor> _keyedModules;
        private FeatureDescriptor[] _features;
        private IDictionary<string, FeatureDescriptor> _keyedFeatures;
        private IActivatingEnvironment _environment;
        private Assembly[] _applicationAssemblies;
        private bool _disposed;
        private List<IModuleContainerExtension> _extensions = new List<IModuleContainerExtension>();

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

        /// <summary>
        /// Gets all the discovered modules.
        /// </summary>
        /// <returns>All the discovered modules.</returns>
        /// <exception cref="InvalidOperationException">The container has not been started.</exception>
        public ModuleDescriptor[] GetModules(string moduleType = null)
        {
            ValidateDisposed();
            ValidateStarted();
            return string.IsNullOrEmpty(moduleType) ? _modules : _modules.Where(module => string.Equals(module.ModuleType, moduleType, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// Gets the module with specified module ID.
        /// </summary>
        /// <param name="moduleId">The specified module ID to lookup module.</param>
        /// <returns>The <see cref="ModuleDescriptor"/> if the module exists; otherwise, null.</returns>
        public ModuleDescriptor GetModule(string moduleId)
        {
            ValidateDisposed();
            ValidateStarted();
            if (string.IsNullOrEmpty(moduleId))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(moduleId));
            }
            ModuleDescriptor module;
            return _keyedModules.TryGetValue(moduleId, out module) ? module : null;
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
        /// Gets the feature with specified feature ID.
        /// </summary>
        /// <param name="featureId">The specified feature ID to lookup feature.</param>
        /// <returns>The <see cref="FeatureDescriptor"/> if the feature exists; otherwise, null.</returns>
        public FeatureDescriptor GetFeature(string featureId)
        {
            ValidateDisposed();
            ValidateStarted();
            if (string.IsNullOrEmpty(featureId))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(featureId));
            }
            FeatureDescriptor feature;
            return _keyedFeatures.TryGetValue(featureId, out feature) ? feature : null;
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
            var installedModules = new List<ModuleDescriptor>();
            var enabledFeatures = new List<FeatureDescriptor>();
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
                            installedModules.Add(module);
                            enabledFeatures.AddRange(EnableFeatures(transaction, module.Features.Select(feature => feature.FeatureId).ToArray(), false));
                        }
                    }
                }
                transaction.Complete();
            }
            foreach (var module in installedModules)
            {
                OnModuleInstalled(new ModuleEventArgs(module));
            }
            StartupModules(installedModules);
            foreach (var feature in enabledFeatures)
            {
                OnFeatureEnabled(new FeatureEventArgs(feature));
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
            var uninstalledModules = new List<ModuleDescriptor>();
            var disabledFeatures = new List<FeatureDescriptor>();
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
                                DisableFeature(transaction, feature, true, disabledFeatures);
                            }
                        }
                        var moduleId = module.ModuleId;
                        module.Uninstall(_environment, transaction, () =>
                         {
                             _configuration.PersistProvider.UninstallModule(moduleId);
                         });
                        uninstalledModules.Add(module);
                    }
                }
                transaction.Complete();
            }
            foreach (var feature in disabledFeatures)
            {
                OnFeatureDisabled(new FeatureEventArgs(feature));
            }
            ShutdownModules(uninstalledModules);
            foreach (var module in uninstalledModules)
            {
                OnModuleUninstalled(new ModuleEventArgs(module));
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
            var enabledFeatures = new List<FeatureDescriptor>();
            using (var transaction = new ModulizeTransation())
            {
                enabledFeatures.AddRange(EnableFeatures(transaction, featureIds, true));
                transaction.Complete();
            }
            foreach (var feature in enabledFeatures)
            {
                OnFeatureEnabled(new FeatureEventArgs(feature));
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
            var disabledFeatures = new List<FeatureDescriptor>();
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
                            DisableFeature(transaction, depending, false, disabledFeatures);
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
            foreach (var feature in disabledFeatures)
            {
                OnFeatureDisabled(new FeatureEventArgs(feature));
            }
        }

        private bool DisableFeature(ModulizeTransation transation, FeatureDescriptor feature, bool force, List<FeatureDescriptor> disabledFeatures)
        {
            if (feature.EnableState != FeatureEnableState.Enabled) return true;
            if (force || !feature.CanDisable)
            {
                if (feature.Dependings.Any(depending => !DisableFeature(transation, depending, force, disabledFeatures)))
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
                disabledFeatures.Add(feature);
                return true;
            }
            return false;
        }

        private IEnumerable<FeatureDescriptor> EnableFeatures(ModulizeTransation transation, string[] features, bool force)
        {
            var enabledFeatures = new List<FeatureDescriptor>();
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
                        enabledFeatures.Add(feature);
                    }
                    else if (feature.CanEnable && features.Contains(feature.FeatureId))
                    {
                        var featureId = feature.FeatureId;
                        feature.Enable(_environment, transation, () =>
                        {
                            _configuration.PersistProvider.EnableFeature(featureId);
                        });
                        enabledFeatures.Add(feature);
                    }
                }
            }
            return enabledFeatures;
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
            _applicationAssemblies = environment.GetAssemblies().ToArray();
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

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="IModuleContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IModuleContainer"/> object that this method was called on.</returns>
        public IModuleContainer AddExtension(IModuleContainerExtension extension)
        {
            ValidateDisposed();
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }
            _extensions.Add(extension);
            extension.Initialize(this);
            return this;
        }

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="extensionType"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public IModuleContainerExtension GetExtension(Type extensionType)
        {
            ValidateDisposed();
            if (extensionType == null)
            {
                throw new ArgumentNullException(nameof(extensionType));
            }
            return _extensions.FirstOrDefault(ex => extensionType.GetTypeInfo().IsAssignableFrom(ex.GetType().GetTypeInfo()));
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
            _keyedFeatures = _features.ToDictionary(feature => feature.FeatureId);
            _keyedModules = _modules.ToDictionary(module => module.ModuleId);
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
            var startupModules = new List<ModuleDescriptor>();
            foreach (var module in modules)
            {
                if (module.Errors == ModuleErrors.None && module.InstallState != ModuleInstallState.RequireInstall)
                {
                    assemblies.AddRange(module.GetLoadedAssemblies());
                    startupModules.Add(module);
                }
            }
            if (assemblies.Count > 0)
            {
                ApplicationActivator.Startup(assemblies.Distinct().Except(_applicationAssemblies).ToArray());
            }
            foreach (var module in startupModules)
            {
                OnModuleLoaded(new ModuleEventArgs(module));
            }
        }

        private void ShutdownModules(IEnumerable<ModuleDescriptor> modules)
        {
            var assemblies = new List<Assembly>();
            var shutdownModules = new List<ModuleDescriptor>();
            foreach (var module in modules)
            {
                if (module.Errors == ModuleErrors.None && module.InstallState != ModuleInstallState.RequireInstall)
                {
                    assemblies.AddRange(module.GetLoadedAssemblies());
                    shutdownModules.Add(module);
                }
            }
            if (assemblies.Count > 0)
            {
                ApplicationActivator.Shutdown(assemblies.Distinct().Except(_applicationAssemblies).ToArray());
            }
            foreach (var module in shutdownModules)
            {
                OnModuleUnloaded(new ModuleEventArgs(module));
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

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn't have a finalizer, so <paramref name="disposing"/> will always be true.
        /// </remarks>
        /// <param name="disposing">True if being called from the IDisposable.Dispose
        /// method, false if being called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
                System.Web.Hosting.HostingEnvironment.StopListening -= OnStopListening;
                typeof(HttpRuntime).GetEvent("AppDomainShutdown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
                  .RemoveMethod.Invoke(null, new object[] { new BuildManagerHostUnloadEventHandler(OnAppDomainShutdown) });
                AppDomain.CurrentDomain.AssemblyResolve -= OnResolveDepedencyAssembly;

                if (_extensions!=null)
                {
                    var toRemove = new List<IModuleContainerExtension>(_extensions);
                    toRemove.Reverse();
                    foreach (var extension in toRemove)
                    {
                        extension.Remove(this);
                        (extension as IDisposable)?.Dispose();
                    }
                    _extensions = null;
                }

                if (_modules != null)
                {
                    ShutdownModules(from module in _modules
                                    where module.InstallState != ModuleInstallState.RequireInstall && module.Errors == ModuleErrors.None
                                    select module);
                    _modules = null;
                    _features = null;
                }
                if (_configuration != null)
                {
                    _configuration.Dispose(disposing);
                    _configuration = null;
                }
                if (_events != null)
                {
                    _events.Dispose();
                    _events = null;
                }
                _environment = null;
                _applicationAssemblies = null;
            }
        }

        #region Events

        protected EventHandlerList Events
        {
            get
            {
                ValidateDisposed();
                return _events ?? (_events = new EventHandlerList());
            }
        }

        public event EventHandler<ModuleEventArgs> ModuleLoaded
        {
            add
            {
                Events.AddHandler(EventModuleLoaded, value);
            }
            remove
            {
                Events.RemoveHandler(EventModuleLoaded, value);
            }
        }

        protected virtual void OnModuleLoaded(ModuleEventArgs e)
        {
            ((EventHandler<ModuleEventArgs>)Events[EventModuleLoaded])?.Invoke(this, e);
        }

        public event EventHandler<ModuleEventArgs> ModuleUnloaded
        {
            add
            {
                Events.AddHandler(EventModuleUnloaded, value);
            }
            remove
            {
                Events.RemoveHandler(EventModuleUnloaded, value);
            }
        }

        protected virtual void OnModuleUnloaded(ModuleEventArgs e)
        {
            ((EventHandler<ModuleEventArgs>)Events[EventModuleUnloaded])?.Invoke(this, e);
        }

        public event EventHandler<ModuleEventArgs> ModuleInstalled
        {
            add
            {
                Events.AddHandler(EventModuleInstalled, value);
            }
            remove
            {
                Events.RemoveHandler(EventModuleInstalled, value);
            }
        }

        protected virtual void OnModuleInstalled(ModuleEventArgs e)
        {
            ((EventHandler<ModuleEventArgs>)Events[EventModuleInstalled])?.Invoke(this, e);
        }

        public event EventHandler<ModuleEventArgs> ModuleUninstalled
        {
            add
            {
                Events.AddHandler(EventModuleUninstalled, value);
            }
            remove
            {
                Events.RemoveHandler(EventModuleUninstalled, value);
            }
        }

        protected virtual void OnModuleUninstalled(ModuleEventArgs e)
        {
            ((EventHandler<ModuleEventArgs>)Events[EventModuleUninstalled])?.Invoke(this, e);
        }

        public event EventHandler<FeatureEventArgs> FeatureEnabled
        {
            add
            {
                Events.AddHandler(EventFeatureEnabled, value);
            }
            remove
            {
                Events.RemoveHandler(EventFeatureEnabled, value);
            }
        }

        protected virtual void OnFeatureEnabled(FeatureEventArgs e)
        {
            ((EventHandler<FeatureEventArgs>)Events[EventFeatureEnabled])?.Invoke(this, e);
        }

        public event EventHandler<FeatureEventArgs> FeatureDisabled
        {
            add
            {
                Events.AddHandler(EventFeatureDisabled,value);
            }
            remove
            {
                Events.RemoveHandler(EventFeatureDisabled,value);
            }
        }

        protected virtual void OnFeatureDisabled(FeatureEventArgs e)
        {
            ((EventHandler<FeatureEventArgs>)Events[EventFeatureDisabled])?.Invoke(this, e);
        }

        #endregion
    }
}
