using Wheatech.Activation;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.Samples.Platform.WebForms
{
    public class Startup
    {
        public void ConfigurationCompleted(IActivatingEnvironment environment, IModuleConfiguration configuration, IModuleContainer container)
        {
            configuration.PersistWithSQLite("SQLite");
            container.Start(environment);
        }
    }
}