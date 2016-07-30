using Wheatech.Activation;
using Wheatech.Modulize.SqlServer;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.Samples.Platform.WebForms
{
    public class Startup
    {
        public void ConfigurationCompleted(IActivatingEnvironment environment, IModuleConfiguration configuration, IModuleContainer container)
        {
            configuration.PersistWithSQLite("App_Data/modulize.db");
            //configuration.PersistWithSqlServer("Modulize");
            container.Start(environment);
        }
    }
}