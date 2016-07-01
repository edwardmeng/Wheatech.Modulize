using System.Collections.Generic;

namespace Wheatech.Modulize
{
    /// <summary>
    /// Interface to define the method to provide locations for the module discover algorithms.
    /// </summary>
    public interface IModuleLocator
    {
        /// <summary>
        /// Gets the locations to discover modules.
        /// </summary>
        /// <returns>The locations information to discover modules.</returns>
        IEnumerable<ModuleLocation> GetLocations();
    }
}
