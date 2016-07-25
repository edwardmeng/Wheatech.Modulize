using Wheatech.Modulize.Mvc;

namespace Wheatech.Modulize.Samples.Settings.Mvc
{
    public class SettingsModuleRegistration : ModuleRegistration
    {
        public override void RegisterModule(ModuleRegistrationContext context)
        {
            context.MapRoute("Settings.Default", "Settings/{controller}/{action}",
                new { controller = "Home", action = "Index" });
        }
    }
}