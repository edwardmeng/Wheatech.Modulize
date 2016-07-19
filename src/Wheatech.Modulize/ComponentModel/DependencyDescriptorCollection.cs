using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class DependencyDescriptorCollection : ICollection<DependencyDescriptor>
    {
        private readonly List<DependencyDescriptor> _dependencies;

        #region Constructors

        public DependencyDescriptorCollection()
        {
            _dependencies = new List<DependencyDescriptor>();
        }

        public DependencyDescriptorCollection(IEnumerable<DependencyDescriptor> dependencies)
        {
            _dependencies = new List<DependencyDescriptor>(dependencies);
        }

        public DependencyDescriptorCollection(int capacity)
        {
            _dependencies = new List<DependencyDescriptor>(capacity);
        }

        public DependencyDescriptorCollection(params DependencyDescriptor[] dependencies)
            : this((IEnumerable<DependencyDescriptor>)dependencies)
        {
        }

        #endregion

        public IEnumerator<DependencyDescriptor> GetEnumerator()
        {
            return _dependencies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void SetReadOnly()
        {
            IsReadOnly = true;
        }

        private void ValidateReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(Strings.Collection_ReadOnly);
            }
        }

        public void Add(DependencyDescriptor item)
        {
            ValidateReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _dependencies.Add(item);
        }

        public void Clear()
        {
            ValidateReadOnly();
            _dependencies.Clear();
        }

        public bool Contains(DependencyDescriptor item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _dependencies.Contains(item);
        }

        public void CopyTo(DependencyDescriptor[] array, int arrayIndex)
        {
            _dependencies.CopyTo(array, arrayIndex);
        }

        public bool Remove(DependencyDescriptor item)
        {
            ValidateReadOnly();
            return _dependencies.Remove(item);
        }

        public int Count => _dependencies.Count;

        public bool IsReadOnly { get; private set; }

        public DependencyDescriptor this[int index]
        {
            get { return _dependencies[index]; }
            set
            {
                ValidateReadOnly();
                _dependencies[index] = value;
            }
        }

        public bool DependOn(FeatureDescriptor feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));
            return FindDependency(feature) != null;
        }

        public bool? Satisfies(FeatureDescriptor feature)
        {
            var dependency = FindDependency(feature);
            if (dependency == null) return null;
            if (dependency.Version == null) return true;
            return dependency.Version.Match(feature.Module.ModuleVersion);
        }

        private DependencyDescriptor FindDependency(FeatureDescriptor feature)
        {
            return _dependencies.FirstOrDefault(x => string.Equals(x.FeatureId, feature.FeatureId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
