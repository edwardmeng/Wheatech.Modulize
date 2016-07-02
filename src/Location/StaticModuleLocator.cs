using System.Collections.Generic;

namespace Wheatech.Modulize
{
    public class StaticModuleLocator : IModuleLocator
    {
        private readonly string _moduleType;
        private readonly string _path;
        private readonly DiscoverStrategy _strategy;
        private readonly bool _enableShadow;

        public StaticModuleLocator(string moduleType, string path, DiscoverStrategy strategy, bool enableShadow)
        {
            _moduleType = moduleType;
            _path = path;
            _strategy = strategy;
            _enableShadow = enableShadow;
        }

        public IEnumerable<ModuleLocation> GetLocations()
        {
            yield return new ModuleLocation
            {
                ModuleType = _moduleType,
                Location = PathUtils.ResolvePath(_path),
                DiscoverStrategy = _strategy,
                EnableShadow = _enableShadow
            };
        }
    }
}
