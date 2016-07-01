using System.Collections.Generic;

namespace Wheatech.Modulize
{
    public interface IModuleDiscover
    {
        IEnumerable<ModuleDescriptor> Discover();
    }
}
