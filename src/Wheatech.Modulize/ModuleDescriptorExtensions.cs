using System.Collections.Generic;
using System.Linq;

namespace Wheatech.Modulize
{
    public static class ModuleDescriptorExtensions
    {
        public static IEnumerable<ModuleDescriptor> GetDependingModules(this ModuleDescriptor module)
        {
            return module.Features.SelectMany(feature => feature.GetDependingFeatures()).Select(feature => feature.Module).Distinct().Except(new[] { module });
        }

        public static IEnumerable<ModuleDescriptor> GetDependedModules(this ModuleDescriptor module)
        {
            return module.Features.SelectMany(feature => feature.GetDependedFeatures()).Select(feature => feature.Module).Distinct().Except(new[] { module });
        }
    }
}
