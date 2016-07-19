using System.Diagnostics;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The descriptor of features.
    /// </summary>
    [DebuggerDisplay("ID={FeatureId}")]
    public sealed partial class FeatureDescriptor
    {
        private DependencyDescriptorCollection _dependencies;
        private FeatureDescriptorCollection _dependings;

        public string FeatureId { get; internal set; }

        public string FeatureName { get; internal set; }

        public string Description { get; internal set; }

        public string Category { get; internal set; }

        public AssemblyIdentity EntryAssembly { get; internal set; }

        public DependencyDescriptorCollection Dependencies
        {
            get { return _dependencies ?? (_dependencies = new DependencyDescriptorCollection()); }
            internal set { _dependencies = value; }
        }

        internal FeatureDescriptorCollection Dependings => _dependings ?? (_dependings = new FeatureDescriptorCollection());

        public ModuleDescriptor Module { get; internal set; }
    }
}
