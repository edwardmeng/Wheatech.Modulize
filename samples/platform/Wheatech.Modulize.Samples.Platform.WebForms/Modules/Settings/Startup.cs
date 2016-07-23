using System.Web.Routing;
using Wheatech.Modulize.Web;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class Startup
    {
        public void Configuration()
        {
            RouteTable.Routes.MapModulePageRoute("Settings", "~/", "~/Settings.aspx");
        }
    }
}