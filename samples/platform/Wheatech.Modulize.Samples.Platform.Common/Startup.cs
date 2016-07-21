using Wheatech.Activation;

namespace Wheatech.Modulize.Samples.Platform.Common
{
    public class Startup
    {
        internal static IActivatingEnvironment Environment { get; private set; }

        public static void ConfigurationCompleted(IActivatingEnvironment environment)
        {
            Environment = environment;
        }
    }
}
