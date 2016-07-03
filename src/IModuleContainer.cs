using Wheatech.Activation;

namespace Wheatech.Modulize
{
    public interface IModuleContainer
    {
        ModuleDiscoverCollection Discovers { get; }

        ModuleLocatorCollection Locators { get; }

        ManifestTable Manifests { get; }

        IActivationProvider ActivationProvider { get; set; }

        ModuleDescriptor[] GetModules();

        FeatureDescriptor[] GetFeatures();

        void InstallModules(params string[] modules);

        void UninstallModules(params string[] modules);

        void EnableFeatures(params string[] features);

        void DisableFeatures(params string[] features);

        void Start(IActivatingEnvironment environment);
    }
}
