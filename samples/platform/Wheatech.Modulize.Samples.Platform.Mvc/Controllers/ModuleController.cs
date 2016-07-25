using System;
using System.Linq;
using System.Web.Mvc;

namespace Wheatech.Modulize.Samples.Platform.Mvc.Controllers
{
    public class ModuleController:Controller
    {
        public ViewResult Index()
        {
            ViewBag.Modules = Modulizer.GetModules().OrderBy(module => module.ModuleName, StringComparer.CurrentCultureIgnoreCase);
            ViewBag.Features = Modulizer.GetFeatures().OrderBy(feature => feature.FeatureName, StringComparer.CurrentCultureIgnoreCase);
            return View();
        }
    }
}