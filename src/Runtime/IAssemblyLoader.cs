using System.Reflection;

namespace Wheatech.Modulize
{
    public interface IAssemblyLoader
    {
        bool TryRedirect(AssemblyIdentity identity, out AssemblyIdentity redirectedIdentity);

        bool TryDependencyLoad(ModuleDescriptor module, AssemblyIdentity identity, out Assembly assembly);

        bool TryInitiatedLoad(ModuleDescriptor module, out Assembly assembly);

        AssemblyIdentity CreateIdentity();

        AssemblyMatchResult Match(ref AssemblyIdentity assemblyIdentity);

        Assembly Load(ModuleDescriptor module);

        int Priority { get; }
    }
}
