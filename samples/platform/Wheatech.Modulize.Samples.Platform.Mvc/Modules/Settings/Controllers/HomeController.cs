using System.Linq;
using System.Web.Mvc;
using Wheatech.Modulize.Samples.Platform.Common;
using Wheatech.Modulize.Samples.Settings.Mvc.Model;
using Wheatech.Modulize.Samples.Settings.Services;

namespace Wheatech.Modulize.Samples.Settings.Mvc.Controllers
{
    public class HomeController:Controller
    {
        public ViewResult Index()
        {
            var service = this.GetService<ISettingsService>();
            return View(service.GetFields().Select(field => new SettingModel
            {
                Key = field.Key,
                Name = field.Name,
                Description = field.Description,
                Value = service.Get(field.Key)
            }).ToArray());
        }
    }
}