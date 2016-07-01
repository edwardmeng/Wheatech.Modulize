using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class FeatureDescriptorCollection : ICollection<FeatureDescriptor>
    {

        private readonly List<FeatureDescriptor> _features;

        #region Constructors

        public FeatureDescriptorCollection()
        {
            _features = new List<FeatureDescriptor>();
        }

        public FeatureDescriptorCollection(IEnumerable<FeatureDescriptor> features)
        {
            _features = new List<FeatureDescriptor>(features);
        }

        public FeatureDescriptorCollection(int capacity)
        {
            _features = new List<FeatureDescriptor>(capacity);
        }

        public FeatureDescriptorCollection(params FeatureDescriptor[] features)
            : this((IEnumerable<FeatureDescriptor>)features)
        {
        }

        #endregion

        public IEnumerator<FeatureDescriptor> GetEnumerator()
        {
            return _features.GetEnumerator();
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

        public void Add(FeatureDescriptor item)
        {
            ValidateReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _features.Add(item);
        }

        public void Clear()
        {
            ValidateReadOnly();
            _features.Clear();
        }

        public bool Contains(FeatureDescriptor item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _features.Contains(item);
        }

        public void CopyTo(FeatureDescriptor[] array, int arrayIndex)
        {
            _features.CopyTo(array, arrayIndex);
        }

        public bool Remove(FeatureDescriptor item)
        {
            ValidateReadOnly();
            return _features.Remove(item);
        }

        public int Count => _features.Count;

        public bool IsReadOnly { get; private set; }

        public FeatureDescriptor this[int index]
        {
            get { return _features[index]; }
            set
            {
                ValidateReadOnly();
                _features[index] = value;
            }
        }

        public FeatureDescriptor this[string featureId]
        {
            get
            {
                return _features.FirstOrDefault(x => string.Equals(x.FeatureId, featureId));
            }
        }
    }
}
