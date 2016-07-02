using System.Collections.Specialized;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The descriptor of features.
    /// </summary>
    public sealed class FeatureDescriptor
    {
        private DependencyDescriptorCollection _dependencies;

        public string FeatureId { get; internal set; }

        public string FeatureName { get; internal set; }

        public string Description { get; internal set; }

        public string Category { get; internal set; }

        public bool RequiresEnable => FeatureManager != null && FeatureManager.RequiresEnable;

        public FeatureRuntimeState RuntimeState { get; internal set; }

        public FeatureManageState ManageState { get; internal set; }

        internal FeatureManager FeatureManager { get; set; }

        public AssemblyIdentity EntryAssembly { get; internal set; }

        public DependencyDescriptorCollection Dependencies
        {
            get { return _dependencies ?? (_dependencies = new DependencyDescriptorCollection()); }
            internal set { _dependencies = value; }
        }

        public ModuleDescriptor Module { get; internal set; }

        internal NameValueCollection Attributes { get; set; }

        /// <summary>
        /// Gets the attribute value of the feature using the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The attribute value. If the attribute does not exist, returns null.</returns>
        public string this[string key] => Attributes?[key];
    }
}
