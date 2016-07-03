using System;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The attribute to mark the module installer classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleInstallerAttribute : Attribute
    {
        /// <summary>
        /// Initialize new instance of <see cref="ModuleInstallerAttribute"/> by specified target version.
        /// </summary>
        /// <param name="version">The target version of the module installer.</param>
        public ModuleInstallerAttribute(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(version));
            }
            Version = Version.Parse(version);
        }

        /// <summary>
        /// Gets the target version of the module installer.
        /// </summary>
        public Version Version { get; }
    }
}
