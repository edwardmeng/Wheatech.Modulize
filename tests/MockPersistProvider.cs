namespace Wheatech.Modulize.UnitTests
{
    public class MockPersistProvider : IPersistProvider
    {
        public void InstallModule(string moduleId, Version version)
        {
        }

        public void UninstallModule(string moduleId)
        {
        }

        public bool GetModuleInstalled(string moduleId, out Version version)
        {
            version = null;
            return false;
        }

        public void EnableFeature(string featureId)
        {
        }

        public void DisableFeature(string featureId)
        {
        }

        public bool GetFeatureEnabled(string featureId)
        {
            return false;
        }
    }
}
