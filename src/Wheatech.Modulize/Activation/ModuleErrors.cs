using System;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The enumeration to indicate the runtime errors of the <see cref="ModuleDescriptor"/>.
    /// </summary>
    [Flags]
    public enum ModuleErrors
    {
        /// <summary>
        /// No error.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// The module is not compatible with the hosting application version.
        /// </summary>
        IncompatibleHost = 0x01,
        /// <summary>
        /// All the underlying features have been forbidden.
        /// </summary>
        ForbiddenFeatures = 0x02
    }
}
