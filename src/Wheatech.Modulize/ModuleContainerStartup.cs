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
            var configuration = Modulizer.DefaultContainer.Configure();
            environment.Use(configuration);
            var persistProvider = ((ModuleConfiguration)configuration).PersistProvider;
            if (persistProvider != null)
            {
                environment.Use(persistProvider);
            }
        }
    }
}
