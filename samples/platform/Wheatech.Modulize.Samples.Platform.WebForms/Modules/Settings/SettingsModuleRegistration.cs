using Wheatech.Modulize.Web;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class SettingsModuleRegistration:ModuleRegistration
    {
        public override void RegisterModule(ModuleRegistrationContext context)
        {
            context.MapPageRoute("Settings.Default", "settings", "~/Settings.aspx");
        }
    }
}