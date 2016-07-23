using System;
using System.Collections.Generic;
using Wheatech.Activation;

namespace Wheatech.Modulize
{
    public interface IModuleContainer
    {
        /// <summary>
        /// Gets an instance of <see cref="IModuleConfiguration"/> used to configure the module container.
        /// </summary>
        /// <returns>An instance of <see cref="IModuleConfiguration"/> to configure the module container.</returns>
        /// <exception cref="System.InvalidOperationException">The container has been started.</exception>
        IModuleConfiguration Configure();

        /// <summary>
        /// Gets all the discovered modules.
        /// </summary>
        /// <param name="moduleType">The specified module type.</param>
        /// <returns>All the discovered modules.</returns>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        ModuleDescriptor[] GetModules(string moduleType = null);

        /// <summary>
        /// Gets the module with specified module ID.
        /// </summary>
        /// <param name="moduleId">The specified module ID to lookup module.</param>
        /// <returns>The <see cref="ModuleDescriptor"/> if the module exists; otherwise, null.</returns>
        ModuleDescriptor GetModule(string moduleId);

        /// <summary>
        /// Gets all the discovered features.
        /// </summary>
        /// <returns>All the discovered features.</returns>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        FeatureDescriptor[] GetFeatures();

        /// <summary>
        /// Gets the feature with specified feature ID.
        /// </summary>
        /// <param name="featureId">The specified feature ID to lookup feature.</param>
        /// <returns>The <see cref="FeatureDescriptor"/> if the feature exists; otherwise, null.</returns>
        FeatureDescriptor GetFeature(string featureId);

        /// <summary>
        /// Installs modules by using an <see cref="IEnumerable{T}"/> of module IDs.
        /// </summary>
        /// <param name="modules">The module IDs of the modules to be installed.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="modules"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        void InstallModules(IEnumerable<string> modules);

        /// <summary>
        /// Uninstalls modules by using an <see cref="IEnumerable{T}"/> of module IDs.
        /// </summary>
        /// <param name="modules">The module IDs of the modules to be uninstalled.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="modules"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        void UninstallModules(IEnumerable<string> modules);

        /// <summary>
        /// Enables features by using an <see cref="IEnumerable{T}"/> of feature IDs.
        /// </summary>
        /// <param name="features">The feature IDs of the feature to be enabled.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="features"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        void EnableFeatures(IEnumerable<string> features);

        /// <summary>
        /// Disables features by using an <see cref="IEnumerable{T}"/> of feature IDs.
        /// </summary>
        /// <param name="features">The feature IDs of the feature to be disabled.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="features"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        void DisableFeatures(IEnumerable<string> features);

        /// <summary>
        /// Starts the module container by using the specified <see cref="IActivatingEnvironment"/>.
        /// </summary>
        /// <param name="environment">The <see cref="IActivatingEnvironment"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="environment"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">The container has been started.</exception>
        void Start(IActivatingEnvironment environment);

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="IModuleContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IModuleContainer"/> object that this method was called on.</returns>
        IModuleContainer AddExtension(IModuleContainerExtension extension);

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="extensionType"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        IModuleContainerExtension GetExtension(Type extensionType);

        event EventHandler<ModuleEventArgs> ModuleLoaded;

        event EventHandler<ModuleEventArgs> ModuleUnloaded; 

        event EventHandler<ModuleEventArgs> ModuleInstalled;

        event EventHandler<ModuleEventArgs> ModuleUninstalled;

        event EventHandler<FeatureEventArgs> FeatureEnabled;

        event EventHandler<FeatureEventArgs> FeatureDisabled;
    }
}
