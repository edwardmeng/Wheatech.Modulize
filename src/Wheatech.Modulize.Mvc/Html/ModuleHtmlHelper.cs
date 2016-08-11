using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wheatech.Modulize.Mvc.Properties;

namespace Wheatech.Modulize.Mvc
{
    /// <summary>
    /// Represents support for HTML links in a module.
    /// </summary>
    public class ModuleHtmlHelper
    {
        private readonly HtmlHelper _htmlHelper;

        internal ModuleHtmlHelper(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        #region ActionLink

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

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(linkText));
            }
            return MvcHtmlString.Create(GenerateLink(_htmlHelper.ViewContext.RequestContext, _htmlHelper.RouteCollection, linkText, null, actionName, controllerName, module, routeValues, htmlAttributes));
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, module, null, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues)
        {
            return ActionLink(linkText, actionName, controllerName, module, routeValues, null);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, object routeValues, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, module, new RouteValueDictionary(routeValues),
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module)
        {
            return ActionLink(linkText, actionName, controllerName, module, null, null);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, null, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, null);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName)
        {
            return ActionLink(linkText, actionName, controllerName, null, null, null);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, null, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, null);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor (&lt;a&gt;) element that contains a URL path to the specified module action.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName)
        {
            return ActionLink(linkText, actionName, null, null, null, null);
        }

        #endregion
    }
}
