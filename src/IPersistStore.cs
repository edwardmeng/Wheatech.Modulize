namespace Wheatech.Modulize
{
    public interface IPersistStore
    {
        void SaveModuleState(string moduleId, ModuleManageState state);

        void SaveModuleVersion(string moduleId, Version version);

        void SaveFeatureState(string featureId, FeatureManageState state);

        ModuleManageState? GetModuleState(string moduleId);

        Version GetModuleVersion(string moduleId);

        FeatureManageState? GetFeatureState(string featureId);
    }
}
