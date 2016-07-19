using System.Collections.Generic;

namespace Wheatech.Modulize.UnitTests
{
    public class MockPersistProvider : IPersistProvider
    {
        private readonly IDictionary<string, Version> _installedModules = new Dictionary<string, Version>(); 
        private readonly IList<string> _enabledFeatures = new List<string>(); 

        public void InstallModule(string moduleId, Version version)
        {
            _installedModules[moduleId] = version;
        }

        public void UninstallModule(string moduleId)
        {
            _installedModules.Remove(moduleId);
        }

        public bool GetModuleInstalled(string moduleId, out Version version)
        {
            return _installedModules.TryGetValue(moduleId, out version);
        }

        public void EnableFeature(string featureId)
        {
            _enabledFeatures.Add(featureId);
        }

        public void DisableFeature(string featureId)
        {
            _enabledFeatures.Remove(featureId);
        }

        public bool GetFeatureEnabled(string featureId)
        {
            return _enabledFeatures.Contains(featureId);
        }
    }
}
