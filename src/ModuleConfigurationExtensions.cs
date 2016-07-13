using System;

namespace Wheatech.Modulize
{
    /// <summary>
    /// Extension methods for <see cref="IModuleConfiguration"/>. 
    /// </summary>
    public static class ModuleConfigurationExtensions
    {
        /// <summary>
        /// Register a new <see cref="IModuleLocator"/> to tell the engine how to locate the modules.
        /// </summary>
        /// <typeparam name="T">The type of registering locator.</typeparam>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register locator with.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration UseLocator<T>(this IModuleConfiguration configuration)
            where T : IModuleLocator, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseLocator(new T());
        }

        /// <summary>
        /// Specify a new <see cref="IManifestParser"/> with the name of manifest file, to tell the engine how to discover the metadata information of modules.
        /// </summary>
        /// <typeparam name="T">The type of registering manifest parser.</typeparam>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register manifest parser with.</param>
        /// <param name="fileName">The name of manifest file.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration UseManifest<T>(this IModuleConfiguration configuration, string fileName)
            where T : IManifestParser, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseManifest(fileName, new T());
        }

        /// <summary>
        /// Specify a new <see cref="IModuleDiscover"/> of the strategy to discover modules.
        /// </summary>
        /// <typeparam name="T">The type of registering discover strategy.</typeparam>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register discover strategy with.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration UseDiscover<T>(this IModuleConfiguration configuration)
            where T : IModuleDiscover, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseDiscover(new T());
        }

        /// <summary>
        /// Specify the <see cref="IPersistProvider"/> used to persist and recover the installation state of modules and enable states of features.
        /// </summary>
        /// <typeparam name="T">The type of registering persist provider.</typeparam>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register persist provider with.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration PersistWith<T>(this IModuleConfiguration configuration)
            where T:IPersistProvider,new ()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.PersistWith(new T());
        }
    }
}
