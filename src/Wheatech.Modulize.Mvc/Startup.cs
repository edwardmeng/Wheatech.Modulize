﻿using System.Linq;
using System.Web.Mvc;

namespace Wheatech.Modulize.Mvc
{
    public class Startup
    {
        public void Configuration()
        {
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().SingleOrDefault();
            if (razorEngine != null)
            {
                ViewEngines.Engines.Remove(razorEngine);
            }
            ViewEngines.Engines.Add(new ModuleRazorViewEngine());
        }
    }
}
