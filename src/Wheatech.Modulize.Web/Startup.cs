namespace Wheatech.Modulize.Web
{
    internal class Startup
    {
        public void Configuration(IModuleContainer container)
        {
            container.AddExtension(new ModuleWebAssemblyExtension());
        }
    }
}
