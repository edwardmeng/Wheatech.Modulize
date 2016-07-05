using System;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal class ModuleConfiguration : IModuleConfiguration, IDisposable
    {
        #region Fields

        private ModuleDiscoverCollection _discovers;
        private ModuleLocatorCollection _locators;
        private ManifestTable _manifests;
        private bool _isReadOnly;
        private string _shadowPath;
        private IPersistProvider _persistProvider;
        private bool _disposed;

        #endregion

        #region Properties

        public ModuleDiscoverCollection Discovers
        {
            get
            {
                ValidateDisposed();
                return _discovers ?? (_discovers = new ModuleDiscoverCollection());
            }
        }

        public ModuleLocatorCollection Locators
        {
            get
            {
                ValidateDisposed();
                return _locators ?? (_locators = new ModuleLocatorCollection()); }
        }

        public ManifestTable Manifests
        {
            get
            {
                ValidateDisposed();
                return _manifests ?? (_manifests = new ManifestTable()); }
        }

        public IPersistProvider PersistProvider
        {
            get
            {
                ValidateDisposed();
                return _persistProvider; }
            set
            {
                ValidateDisposed();
                ValidateReadOnly();
                _persistProvider = value;
            }
        }

        public string ShadowPath
        {
            get
            {
                ValidateDisposed();
                return _shadowPath; }
            set
            {
                ValidateDisposed();
                ValidateReadOnly();
                _shadowPath = value;
            }
        }

        #endregion

        #region Methods

        public void SetReadOnly()
        {
            ValidateDisposed();
            _isReadOnly = true;
            Discovers.SetReadOnly(true);
            Locators.SetReadOnly(true);
            Manifests.SetReadOnly();
        }

        private void ValidateReadOnly()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException(Strings.Configuration_ReadOnly);
            }
        }

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("ModuleConfiguration");
            }
        }

        public IModuleConfiguration UseLocator(IModuleLocator locator)
        {
            ValidateDisposed();
            ValidateReadOnly();
            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator));
            }
            Locators.Add(locator);
            return this;
        }

        public IModuleConfiguration UseManifest(string fileName, IManifestParser parser)
        {
            ValidateDisposed();
            ValidateReadOnly();
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(fileName));
            }
            Manifests.SetParser(fileName, parser);
            return this;
        }

        public IModuleConfiguration UseDiscover(IModuleDiscover disover)
        {
            ValidateDisposed();
            ValidateReadOnly();
            if (disover == null)
            {
                throw new ArgumentNullException(nameof(disover));
            }
            Discovers.Add(disover);
            return this;
        }

        public IModuleConfiguration UseShadowPath(string shadowPath)
        {
            ValidateDisposed();
            ValidateReadOnly();
            if (string.IsNullOrEmpty(shadowPath))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(shadowPath));
            }
            ShadowPath = shadowPath;
            return this;
        }

        public IModuleConfiguration PersistWith(IPersistProvider provider)
        {
            ValidateDisposed();
            ValidateReadOnly();
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            PersistProvider = provider;
            return this;
        }

        public void Dispose()
        {
            if(_disposed)return;
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        protected internal virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_discovers != null)
                {
                    foreach (var discover in _discovers)
                    {
                        (discover as IDisposable)?.Dispose();
                    }
                    _discovers = null;
                }
                if (_locators != null)
                {
                    foreach (var locator in _locators)
                    {
                        (locator as IDisposable)?.Dispose();
                    }
                }
                if (_manifests != null)
                {
                    foreach (var parser in _manifests.Parsers)
                    {
                        (parser as IDisposable)?.Dispose();
                    }
                    _manifests = null;
                }
                if (_persistProvider != null)
                {
                    (_persistProvider as IDisposable)?.Dispose();
                    _persistProvider = null;
                }
                _shadowPath = null;
            }
        }

        #endregion
    }
}
