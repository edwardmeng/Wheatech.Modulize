using System;

namespace Wheatech.Modulize
{
    public static class ModuleConfigurationExtensions
    {
        public static IModuleConfiguration UseLocator<T>(this IModuleConfiguration configuration)
            where T : IModuleLocator, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseLocator(new T());
        }

        public static IModuleConfiguration UseManifest<T>(this IModuleConfiguration configuration, string fileName)
            where T : IManifestParser, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseManifest(fileName, new T());
        }

        public static IModuleConfiguration UseDiscover<T>(this IModuleConfiguration configuration)
            where T : IModuleDiscover, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.UseDiscover(new T());
        }

        public static IModuleConfiguration PersistWith<T>(this IModuleConfiguration configuration)
            where T:IPersistProvider,new ()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            return configuration.PersistWith(new T());
        }
    }
}
