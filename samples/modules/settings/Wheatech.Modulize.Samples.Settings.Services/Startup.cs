using Wheatech.Activation;
using Wheatech.Modulize.Samples.Caching;
using Wheatech.Modulize.Samples.Platform.Services;

namespace Wheatech.Modulize.Samples.Settings.Services
{
    public class Startup
    {
        public void Configuration(IActivatingEnvironment environment, IDatabaseService database, ICacheService caching)
        {
            environment.Use<ISettingsService>(new DefaultSettingsService(database, caching));
        }
    }
}
