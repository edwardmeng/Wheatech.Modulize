using System;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The enumeration to indicate the runtime errors of the <see cref="FeatureDescriptor"/>.
    /// </summary>
    [Flags]
    public enum FeatureErrors
    {
        /// <summary>
        /// No error.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// The relative module has been forbidden for any reason.
        /// </summary>
        ForbiddenModule = 0x01,
        /// <summary>
        /// The relative module has been uninstalled or has not been installed.
        /// </summary>
        UninstallModule = 0x02,
        /// <summary>
        /// Any dependency feature has been missing.
        /// </summary>
        MissingDependency = 0x04,
        /// <summary>
        /// Any dependency feature version is not compatible with the dependency requirement.
        /// </summary>
        IncompatibleDependency = 0x08,
        /// <summary>
        /// Any dependency feature has been forbidden.
        /// </summary>
        ForbiddenDependency = 0x10,
        /// <summary>
        /// Any dependency feature has been disabled.
        /// </summary>
        DisabledDependency = 0x20
    }
}
