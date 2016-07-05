using Wheatech.Activation;
using Wheatech.Modulize;

[assembly: AssemblyActivator(typeof(ModuleContainerStartup))]

namespace Wheatech.Modulize
{
    internal class ModuleContainerStartup
    {
        public ModuleContainerStartup(IActivatingEnvironment environment)
        {
            environment.Use(Modulizer.DefaultContainer);
            environment.Use(Modulizer.DefaultContainer.Configure());
        }
    }
}
