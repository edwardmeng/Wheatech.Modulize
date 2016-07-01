using System.Collections.Generic;

namespace Wheatech.Modulize
{
    public abstract class LocationModuleDiscover:IModuleDiscover
    {
        public abstract IEnumerable<ModuleDescriptor> Discover();
    }
}
