using System.Collections.Specialized;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The descriptor of module.
    /// </summary>
    public class ModuleDescriptor
    {
        /// <summary>
        /// The identity of the module.
        /// </summary>
        public string ModuleId { get; internal set; }

        /// <summary>
        /// The friendly name of the module.
        /// </summary>
        public string ModuleName { get; internal set; }

        /// <summary>
        /// The type of the module.
        /// </summary>
        public string ModuleType { get; internal set; }

        /// <summary>
        /// The description of the module.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The semantic version of the module.
        /// </summary>
        public Version ModuleVersion { get; internal set; }

        /// <summary>
        /// The version requirements of the hosting platform.
        /// </summary>
        public IVersionComparator HostVersion { get; internal set; }

        public AssemblyIdentity EntryAssembly { get; internal set; }

        /// <summary>
        /// The authors of the module.
        /// </summary>
        public string[] Authors { get; internal set; }

        /// <summary>
        /// The tags of the module.
        /// </summary>
        public string[] Tags { get; internal set; }

        /// <summary>
        /// The web site address of the module.
        /// </summary>
        public string WebSite { get; internal set; }

        public string License { get; internal set; }

        public string ShadowPath { get; internal set; }

        public string ModulePath { get; internal set; }

        public ModuleRuntimeState RuntimeState { get; internal set; }

        public ModuleManageState ManageState { get; internal set; }

        //public IModuleManager ModuleManager { get; internal set; }

        internal NameValueCollection Attributes { get; set; }

        /// <summary>
        /// Gets the attribute value of the module using the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The attribute value. If the attribute does not exist, returns null.</returns>
        public string this[string key] => Attributes?[key];

        /// <summary>
        /// Get all the features belong to the module.
        /// </summary>
        public FeatureDescriptorCollection Features { get; internal set; }

        public IAssemblyLoader[] Assemblies { get; internal set; }
    }
}
