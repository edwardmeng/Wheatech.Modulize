namespace Wheatech.Modulize
{
    public interface IModuleConfiguration
    {
        IModuleConfiguration UseLocator(IModuleLocator locator);

        IModuleConfiguration UseManifest(string fileName, IManifestParser parser);

        IModuleConfiguration UseDiscover(IModuleDiscover disover);

        IModuleConfiguration UseShadowPath(string shadowPath);

        IModuleConfiguration PersistWith(IPersistProvider provider);
    }
}
