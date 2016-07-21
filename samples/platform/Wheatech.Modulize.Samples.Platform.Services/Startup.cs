using Wheatech.Activation;

namespace Wheatech.Modulize.Samples.Platform.Services
{
    public class Startup
    {
        public void Configuration(IActivatingEnvironment environment)
        {
            environment.Use<IDatabaseService>(new SQLiteDatabaseService("SQLite"));
        }
    }
}
