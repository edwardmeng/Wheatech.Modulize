using System;

namespace Wheatech.Modulize.MySQL
{
    public class MySQLPersistProvider : IPersistProvider
    {
        public void InstallModule(string moduleId, Version version)
        {
            throw new NotImplementedException();
        }

        public void UninstallModule(string moduleId)
        {
            throw new NotImplementedException();
        }

        public void EnableFeature(string featureId)
        {
            throw new NotImplementedException();
        }

        public void DisableFeature(string featureId)
        {
            throw new NotImplementedException();
        }

        public bool GetFeatureEnabled(string featureId)
        {
            throw new NotImplementedException();
        }

        public bool GetModuleInstalled(string moduleId, out Version version)
        {
            throw new NotImplementedException();
        }
    }
}
