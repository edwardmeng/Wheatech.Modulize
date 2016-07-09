namespace Wheatech.Modulize
{
    /// <summary>
    /// The enumeration for the install state of <see cref="ModuleDescriptor"/>.
    /// </summary>
    public enum ModuleInstallState
    {
        /// <summary>
        /// The module has declared installers and the installers have not been invoked.
        /// </summary>
        RequireInstall,
        /// <summary>
        /// The module has declared installers and the installers have been invoked.
        /// </summary>
        Installed,
        /// <summary>
        /// There is no installers have been declared for the module. And the module can be installed automatically without any invocation.
        /// </summary>
        AutoInstall
    }
}
