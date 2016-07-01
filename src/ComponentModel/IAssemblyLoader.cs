using System.Reflection;

namespace Wheatech.Modulize
{
    public interface IAssemblyLoader
    {
        bool TryRedirect(AssemblyIdentity identity, out AssemblyIdentity redirectedIdentity);

        bool TryDependencyLoad(ModuleDescriptor module, AssemblyIdentity identity, out Assembly assembly);

        bool TryInitiatedLoad(ModuleDescriptor module, out Assembly assembly);

        AssemblyIdentity CreateIdentity();
    }
}
