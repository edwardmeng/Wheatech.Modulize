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
        /// <returns>All the discovered modules.</returns>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        ModuleDescriptor[] GetModules();

        /// <summary>
        /// Gets all the discovered features.
        /// </summary>
        /// <returns>All the discovered features.</returns>
        /// <exception cref="System.InvalidOperationException">The container has not been started.</exception>
        FeatureDescriptor[] GetFeatures();

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
    }
}
