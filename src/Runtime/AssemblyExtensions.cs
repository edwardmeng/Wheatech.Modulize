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
                var assemblyIdentity = module.EntryAssembly;
                IAssemblyLoader matchEntryAssembly = null;
                foreach (var assembly in module.Assemblies)
                {
                    switch (assembly.Match(ref assemblyIdentity))
                    {
                        case AssemblyMatchResult.Success:
                        case AssemblyMatchResult.RedirectAndMatch:
                            if (matchEntryAssembly == null)
                            {
                                matchEntryAssembly = assembly;
                            }
                            else
                            {
                                throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousModuleEntry, module.ModuleId));
                            }
                            break;
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
                var assemblyIdentity = feature.EntryAssembly;
                foreach (var assembly in feature.Module.Assemblies)
                {
                    switch (assembly.Match(ref assemblyIdentity))
                    {
                        case AssemblyMatchResult.Success:
                        case AssemblyMatchResult.RedirectAndMatch:
                            if (matchEntryAssembly == null)
                            {
                                matchEntryAssembly = assembly;
                            }
                            else
                            {
                                throw new ModuleDiscoverException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousFeatureEntry, feature.FeatureId));
                            }
                            break;
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
