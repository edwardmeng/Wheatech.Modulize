using System.Web;
using System.Web.Mvc;

namespace Wheatech.Modulize.Samples.Platform.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
