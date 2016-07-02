using System.Globalization;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public static class AssemblyExtensions
    {
        public static void ValidateEntryAssembly(this ModuleDescriptor module)
        {
            if (module.EntryAssembly != null)
            {
                IAssemblyLoader matchEntryAssembly = null;
                foreach (var assembly in module.Assemblies)
                {
                    if (assembly.Match(module.EntryAssembly))
                    {
                        if (matchEntryAssembly == null)
                        {
                            matchEntryAssembly = assembly;
                        }
                        else
                        {
                            throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousModuleEntry, module.ModuleId));
                        }
                    }
                }
                if (matchEntryAssembly == null)
                {
                    throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_CannotFindModuleEntry, module.ModuleId));
                }
            }
            foreach (var feature in module.Features)
            {
                feature.ValidateEntryAssembly();
            }
        }

        public static void ValidateEntryAssembly(this FeatureDescriptor feature)
        {
            if (feature.EntryAssembly != null)
            {
                IAssemblyLoader matchEntryAssembly = null;
                foreach (var assembly in feature.Module.Assemblies)
                {
                    if (assembly.Match(feature.EntryAssembly))
                    {
                        if (matchEntryAssembly == null)
                        {
                            matchEntryAssembly = assembly;
                        }
                        else
                        {
                            throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousFeatureEntry, feature.FeatureId));
                        }
                    }
                }
                if (matchEntryAssembly == null)
                {
                    throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_CannotFindFeatureEntry, feature.FeatureId));
                }
            }
        }
    }
}
