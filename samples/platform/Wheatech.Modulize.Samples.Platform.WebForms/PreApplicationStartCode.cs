using System.Web;
using Wheatech.Activation;
using Wheatech.Modulize.Samples.Platform.WebForms;

[assembly:PreApplicationStartMethod(typeof(PreApplicationStartCode), "Startup")]
namespace Wheatech.Modulize.Samples.Platform.WebForms
{
    public static class PreApplicationStartCode
    {
        public static void Startup()
        {
            ApplicationActivator.UseApplicationVersion(new System.Version("1.0.0")).UseStartupSteps("Configuration", "ConfigurationCompleted").Startup();
        }
    }
}