using System;
using System.Web.Mvc;

namespace Wheatech.Modulize.Mvc
{
    public static class ModulizeExtensions
    {
        public static ModuleAjaxHelper Module(this AjaxHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return new ModuleAjaxHelper(helper);
        }

        public static ModuleHtmlHelper Module(this HtmlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return new ModuleHtmlHelper(helper);
        }

        public static ModuleUrlHelper Module(this UrlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }
            return new ModuleUrlHelper(helper);
        }
    }
}
