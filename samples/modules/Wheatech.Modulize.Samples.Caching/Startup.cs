using Wheatech.Activation;

namespace Wheatech.Modulize.Samples.Caching
{
    public class Startup
    {
        public Startup(IActivatingEnvironment environment)
        {
            environment.Use<ICacheService>(new DefaultCacheService());
        }
    }
}
