using Wheatech.Activation;
using Wheatech.Modulize.UnitTests;

[assembly:AssemblyActivator(typeof(UnitTestStartup))]

namespace Wheatech.Modulize.UnitTests
{
    public class UnitTestStartup
    {
        public static IActivatingEnvironment Environment { get; set; }

        public static void Configuration(IActivatingEnvironment environment)
        {
            Environment = environment;
        }
    }
}
