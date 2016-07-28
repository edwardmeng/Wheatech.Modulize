using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Wheatech.Modulize.Samples.Platform.Mvc.Controllers
{
    public class ModuleController : Controller
    {
        public ViewResult Index()
        {
            ViewBag.Modules = Modulizer.GetModules().OrderBy(module => module.ModuleName, StringComparer.CurrentCultureIgnoreCase);
            ViewBag.Features = Modulizer.GetFeatures().OrderBy(feature => feature.FeatureName, StringComparer.CurrentCultureIgnoreCase);
            return View();
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult InstallModule(string id)
        {
            Modulizer.InstallModules(id);
            Session["RequireRestart"] = true;
            return Json(true);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult UninstallModule(string id)
        {
            Modulizer.UninstallModules(id);
            Session["RequireRestart"] = true;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult EnableFeature(string id)
        {
            Modulizer.EnableFeatures(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult DisableFeature(string id)
        {
            Modulizer.DisableFeatures(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}