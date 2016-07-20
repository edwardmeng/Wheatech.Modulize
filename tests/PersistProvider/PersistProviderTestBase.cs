using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using Wheatech.Activation;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class PersistProviderTestBase : ModulizeTestBase
    {
        private IPersistProvider _persistProvider;

        protected PersistProviderTestBase()
        {
            _persistProvider = CreatePersistProvider();
        }

        protected abstract IPersistProvider CreatePersistProvider();

        [Fact]
        public void ModuleNotInstalled()
        {
            string moduleId = Guid.NewGuid().ToString();
            Version installVersion;
            Assert.False(_persistProvider.GetModuleInstalled(moduleId, out installVersion));
            Assert.Null(installVersion);
        }

        [Fact]
        public void ModuleInstalled()
        {
            string moduleId = Guid.NewGuid().ToString();
            string moduleId2 = Guid.NewGuid().ToString();
            _persistProvider.InstallModule(moduleId, new Version("1.0.5"));
            _persistProvider.InstallModule(moduleId2, new Version("1.0.6"));
            Version installVersion;
            Assert.True(_persistProvider.GetModuleInstalled(moduleId, out installVersion));
            Assert.Equal(new Version("1.0.5"), installVersion);

            _persistProvider.InstallModule(moduleId, new Version("2.0.5"));
            _persistProvider.InstallModule(moduleId2, new Version("2.0.6"));
            Assert.True(_persistProvider.GetModuleInstalled(moduleId, out installVersion));
            Assert.Equal(new Version("2.0.5"), installVersion);
        }

        [Fact]
        public void FeatureNotEnabled()
        {
            var featureId = Guid.NewGuid().ToString();
            Assert.False(_persistProvider.GetFeatureEnabled(featureId));
        }

        [Fact]
        public void FeatureEnabled()
        {
            var featureId = Guid.NewGuid().ToString();
            _persistProvider.EnableFeature(featureId);
            Assert.True(_persistProvider.GetFeatureEnabled(featureId));
        }

        [Fact]
        public void FeatureDisabled()
        {
            var featureId = Guid.NewGuid().ToString();
            _persistProvider.EnableFeature(featureId);
            Assert.True(_persistProvider.GetFeatureEnabled(featureId));
            _persistProvider.DisableFeature(featureId);
            Assert.False(_persistProvider.GetFeatureEnabled(featureId));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_persistProvider != null)
                {
                    (_persistProvider as IDisposable)?.Dispose();
                    _persistProvider = null;
                }
            }
        }
    }
}
