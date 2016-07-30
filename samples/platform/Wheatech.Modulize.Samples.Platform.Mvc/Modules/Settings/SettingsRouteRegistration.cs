using Wheatech.Modulize.Mvc;

namespace Wheatech.Modulize.Samples.Settings.Mvc
{
    public class SettingsRouteRegistration : ModuleRouteRegistration
    {
        public override void RegisterRoutes(ModuleRouteRegistrationContext context)
        {
            context.MapRoute("Settings.Default", "Settings/{controller}/{action}",
                new { controller = "Home", action = "Index" });
        }
    }
}