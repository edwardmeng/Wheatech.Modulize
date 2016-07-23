namespace Wheatech.Modulize
{
    /// <summary>
    /// Interface for all <see cref="IModuleContainer"/> extension objects. 
    /// </summary>
    public interface IModuleContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        void Initialize(IModuleContainer container);

        /// <summary>
        /// Removes the extension's functions from the container. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        void Remove(IModuleContainer container);
    }
}
