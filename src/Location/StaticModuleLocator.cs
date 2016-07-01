using System.Collections.Generic;

namespace Wheatech.Modulize
{
    public class StaticModuleLocator : IModuleLocator
    {
        private readonly string _moduleType;
        private readonly string _path;
        private readonly DiscoverStrategy _strategy;

        public StaticModuleLocator(string moduleType, string path, DiscoverStrategy strategy)
        {
            _moduleType = moduleType;
            _path = path;
            _strategy = strategy;
        }

        public IEnumerable<ModuleLocation> GetLocations()
        {
            yield return new ModuleLocation
            {
                ModuleType = _moduleType,
                Location = PathUtils.ResolvePath(_path),
                DiscoverStrategy = _strategy
            };
        }
    }
}
