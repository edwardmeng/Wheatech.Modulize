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

        /// <summary>
        /// Gets all the registered strategies to discover modules.
        /// </summary>
        public ModuleDiscoverCollection Discovers
        {
            get
            {
                ValidateDisposed();
                return _discovers ?? (_discovers = new ModuleDiscoverCollection());
            }
        }

        /// <summary>
        /// Gets all the registered strategies to locate modules.
        /// </summary>
        public ModuleLocatorCollection Locators
        {
            get
            {
                ValidateDisposed();
                return _locators ?? (_locators = new ModuleLocatorCollection()); }
        }

        /// <summary>
        /// Gets all the registered strategies to parse modules and features metadata information.
        /// </summary>
        public ManifestTable Manifests
        {
            get
            {
                ValidateDisposed();
                return _manifests ?? (_manifests = new ManifestTable()); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IPersistProvider"/> to persist and recover the installation state of modules and enable states of features.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the shadow file root path for the discovering strategies.
        /// </summary>
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

        /// <summary>
        /// Sets the configuration to be read-only, and cannot be changed through any exported methods.
        /// </summary>
        public void SetReadOnly(bool isReadOnly)
        {
            ValidateDisposed();
            _isReadOnly = isReadOnly;
            Discovers.SetReadOnly(isReadOnly);
            Locators.SetReadOnly(isReadOnly);
            Manifests.SetReadOnly(isReadOnly);
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

        /// <summary>
        /// Specify a new <see cref="IModuleLocator"/> to tell the engine how to locate the modules.
        /// </summary>
        /// <param name="locator">The <see cref="IModuleLocator"/> to locate modules.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
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

        /// <summary>
        /// Specify a new <see cref="IManifestParser"/> with the name of manifest file, to tell the engine how to discover the metadata information of modules.
        /// </summary>
        /// <param name="fileName">The name of manifest file.</param>
        /// <param name="parser">The parser to discover module metadata information.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
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

        /// <summary>
        /// Specify a new <see cref="IModuleDiscover"/> of the strategy to discover modules.
        /// </summary>
        /// <param name="disover">The strategy to discover modules.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
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

        /// <summary>
        /// Specify the shadow file root path for the discovering strategies.
        /// </summary>
        /// <param name="shadowPath">The root path of shadow files.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
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

        /// <summary>
        /// Specify the <see cref="IPersistProvider"/> used to persist and recover the installation state of modules and enable states of features.
        /// </summary>
        /// <param name="provider">The <see cref="IPersistProvider"/> to persist and recover.</param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
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

        /// <summary>
        /// Dispose this configuration instance.
        /// </summary>
        public void Dispose()
        {
            if(_disposed)return;
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        /// <summary>
        /// Dispose this configuration instance.
        /// </summary>
        /// <remarks>
        /// This class doesn't have a finalizer, so <paramref name="disposing"/> will always be true.
        /// </remarks>
        /// <param name="disposing">True if being called from the IDisposable.Dispose
        /// method, false if being called from a finalizer.</param>
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
