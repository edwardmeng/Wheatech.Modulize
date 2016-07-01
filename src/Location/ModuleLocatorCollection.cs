using System;
using System.Collections;
using System.Collections.Generic;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class ModuleLocatorCollection:ICollection<IModuleLocator>
    {
        private readonly List<IModuleLocator> _locators;

        #region Constructors

        public ModuleLocatorCollection()
        {
            _locators = new List<IModuleLocator>();
        }

        public ModuleLocatorCollection(IEnumerable<IModuleLocator> locators)
        {
            _locators = new List<IModuleLocator>(locators);
        }

        public ModuleLocatorCollection(int capacity)
        {
            _locators = new List<IModuleLocator>(capacity);
        }

        public ModuleLocatorCollection(params IModuleLocator[] locators)
            : this((IEnumerable<IModuleLocator>)locators)
        {
        }

        #endregion

        public IEnumerator<IModuleLocator> GetEnumerator()
        {
            return _locators.GetEnumerator();
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

        public void Add(IModuleLocator item)
        {
            ValidateReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _locators.Add(item);
        }

        public void Clear()
        {
            ValidateReadOnly();
            _locators.Clear();
        }

        public bool Contains(IModuleLocator item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _locators.Contains(item);
        }

        public void CopyTo(IModuleLocator[] array, int arrayIndex)
        {
            _locators.CopyTo(array, arrayIndex);
        }

        public bool Remove(IModuleLocator item)
        {
            ValidateReadOnly();
            return _locators.Remove(item);
        }

        public int Count => _locators.Count;

        public bool IsReadOnly { get; private set; }

        public IModuleLocator this[int index]
        {
            get { return _locators[index]; }
            set
            {
                ValidateReadOnly();
                _locators[index] = value;
            }
        }
    }
}
