using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wheatech.Modulize.Mvc.Properties;

namespace Wheatech.Modulize.Mvc
{
    public static class HtmlHelperExtensions
    {
        private static string GenerateLinkInternal(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string actionName, string controllerName, string module, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool includeImplicitMvcValues)
        {
            string str = ModuleHelper.GenerateUrl(routeName, actionName, controllerName, module, routeValues, routeCollection, requestContext, includeImplicitMvcValues);
            TagBuilder builder = new TagBuilder("a")
            {
                InnerHtml = !string.IsNullOrEmpty(linkText) ? HttpUtility.HtmlEncode(linkText) : string.Empty
            };
            builder.MergeAttributes(htmlAttributes);
            builder.MergeAttribute("href", str);
            return builder.ToString(TagRenderMode.Normal);
        }

        private static string GenerateLink(RequestContext requestContext, RouteCollection routeCollection, string linkText, string routeName, string actionName, string controllerName, string module, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return GenerateLinkInternal(requestContext, routeCollection, linkText, routeName, actionName, controllerName, module, routeValues, htmlAttributes, true);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(linkText));
            }
            return MvcHtmlString.Create(GenerateLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, null, actionName, controllerName, module, routeValues, htmlAttributes));
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string module, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, module, null, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, module, routeValues, null);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string module, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, module, new RouteValueDictionary(routeValues),
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string module)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, module, null, null);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, null, null, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, null);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, controllerName, null, null, null);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, null);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ModuleActionLink(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return htmlHelper.ModuleActionLink(linkText, actionName, null, null, null, null);
        }
    }
}
