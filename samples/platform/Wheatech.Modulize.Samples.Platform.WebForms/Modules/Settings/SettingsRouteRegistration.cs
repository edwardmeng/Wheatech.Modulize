using Wheatech.Modulize.Web;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class SettingsRouteRegistration:ModuleRouteRegistration
    {
        public override void RegisterRoutes(ModuleRouteRegistrationContext context)
        {
            context.MapPageRoute("Settings.Default", "settings", "~/Settings.aspx");
        }
    }
}