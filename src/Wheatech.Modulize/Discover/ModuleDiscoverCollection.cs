using System;
using System.Collections;
using System.Collections.Generic;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class ModuleDiscoverCollection : ICollection<IModuleDiscover>
    {
        private readonly List<IModuleDiscover> _discovers;

        #region Constructors

        public ModuleDiscoverCollection()
        {
            _discovers = new List<IModuleDiscover>();
        }

        public ModuleDiscoverCollection(IEnumerable<IModuleDiscover> discovers)
        {
            _discovers = new List<IModuleDiscover>(discovers);
        }

        public ModuleDiscoverCollection(int capacity)
        {
            _discovers = new List<IModuleDiscover>(capacity);
        }

        public ModuleDiscoverCollection(params IModuleDiscover[] discovers)
            : this((IEnumerable<IModuleDiscover>)discovers)
        {
        }

        #endregion

        public IEnumerator<IModuleDiscover> GetEnumerator()
        {
            return _discovers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void SetReadOnly(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        private void ValidateReadOnly()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(Strings.Collection_ReadOnly);
            }
        }

        public void Add(IModuleDiscover item)
        {
            ValidateReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _discovers.Add(item);
        }

        public void Clear()
        {
            ValidateReadOnly();
            _discovers.Clear();
        }

        public bool Contains(IModuleDiscover item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _discovers.Contains(item);
        }

        public void CopyTo(IModuleDiscover[] array, int arrayIndex)
        {
            _discovers.CopyTo(array, arrayIndex);
        }

        public bool Remove(IModuleDiscover item)
        {
            ValidateReadOnly();
            return _discovers.Remove(item);
        }

        public int Count => _discovers.Count;

        public bool IsReadOnly { get; private set; }

        public IModuleDiscover this[int index]
        {
            get { return _discovers[index]; }
            set
            {
                ValidateReadOnly();
                _discovers[index] = value;
            }
        }
    }
}
