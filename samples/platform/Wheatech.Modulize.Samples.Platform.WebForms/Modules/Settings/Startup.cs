using System.Web.Routing;
using Wheatech.Modulize.Samples.Settings.Services;
using Wheatech.Modulize.Web;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class Startup
    {
        public Startup()
        {
            RouteTable.Routes.MapModulePageRoute("Settings", "~/", "~/Settings.aspx");
        }

        public void Configuration(ISettingsService service)
        {
            service.RegisterField(new SettingsField { Key = "DefaultPassword", Name = "Default Password" });
        }
    }
}