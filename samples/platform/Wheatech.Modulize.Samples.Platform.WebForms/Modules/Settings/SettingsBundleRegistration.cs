using Wheatech.Modulize.Web.Optimization;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class SettingsBundleRegistration : ModuleBundleRegistration
    {
        public override void RegisterBundles(ModuleBundleRegistrationContext context)
        {
            context.AddStyleBundle("~/settings/styles").IncludeDirectory("~/Styles", "*.css");
        }
    }
}