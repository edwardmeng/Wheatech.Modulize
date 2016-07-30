using Wheatech.Activation;

namespace Wheatech.Modulize.Samples.Platform.Services
{
    public class Startup
    {
        public void Configuration(IActivatingEnvironment environment)
        {
            environment.Use<IDatabaseService>(new SQLiteDatabaseService("App_Data/sample.db"));
            //environment.Use<IDatabaseService>(new SqlServerDatabaseService("Modulize"));
        }
    }
}
