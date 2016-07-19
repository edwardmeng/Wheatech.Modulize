namespace Wheatech.Modulize
{
    /// <summary>
    /// The interface to define the core methods of fluent configuration API.
    /// </summary>
    public interface IModuleConfiguration
    {
        /// <summary>
        /// Specify a new <see cref="IModuleLocator"/> to tell the engine how to locate the modules.
        /// </summary>
        /// <param name="locator">The <see cref="IModuleLocator"/> to locate modules.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        IModuleConfiguration UseLocator(IModuleLocator locator);

        /// <summary>
        /// Specify a new <see cref="IManifestParser"/> with the name of manifest file, to tell the engine how to discover the metadata information of modules.
        /// </summary>
        /// <param name="fileName">The name of manifest file.</param>
        /// <param name="parser">The parser to discover module metadata information.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        IModuleConfiguration UseManifest(string fileName, IManifestParser parser);

        /// <summary>
        /// Specify a new <see cref="IModuleDiscover"/> of the strategy to discover modules.
        /// </summary>
        /// <param name="disover">The strategy to discover modules.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        IModuleConfiguration UseDiscover(IModuleDiscover disover);

        /// <summary>
        /// Specify the shadow file root path for the discovering strategies.
        /// </summary>
        /// <param name="shadowPath">The root path of shadow files.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        IModuleConfiguration UseShadowPath(string shadowPath);

        /// <summary>
        /// Specify the <see cref="IPersistProvider"/> used to persist and recover the installation state of modules and enable states of features.
        /// </summary>
        /// <param name="provider">The <see cref="IPersistProvider"/> to persist and recover.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        IModuleConfiguration PersistWith(IPersistProvider provider);
    }
}
