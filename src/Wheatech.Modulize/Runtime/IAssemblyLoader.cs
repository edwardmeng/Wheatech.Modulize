using System.Reflection;

namespace Wheatech.Modulize
{
    public interface IAssemblyLoader
    {
        AssemblyIdentity CreateIdentity();

        AssemblyMatchResult Match(ref AssemblyIdentity assemblyIdentity);

        Assembly Load(ModuleDescriptor module);

        int Priority { get; }
    }
}
