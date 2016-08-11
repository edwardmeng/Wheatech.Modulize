using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Wheatech.Modulize.Mvc.Properties;

namespace Wheatech.Modulize.Mvc
{
    /// <summary>
    /// Represents support for ASP.NET AJAX within an ASP.NET MVC module.
    /// </summary>
    public static class AjaxExtensions
    {
        #region Reflection

        private static readonly MethodInfo GenerateLinkMethod;
        private static readonly MethodInfo FormHelperMethod;

        static AjaxExtensions()
        {
            GenerateLinkMethod = typeof(System.Web.Mvc.Ajax.AjaxExtensions).GetMethod("GenerateLink", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                new[]
                {
                    typeof (AjaxHelper), typeof (string), typeof (string), typeof (AjaxOptions), typeof (IDictionary<string, object>)
                }, null);
            FormHelperMethod = typeof(System.Web.Mvc.Ajax.AjaxExtensions).GetMethod("FormHelper", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                new[]
                {
                    typeof (AjaxHelper), typeof (string), typeof (AjaxOptions), typeof (IDictionary<string, object>)
                }, null);
        }

        private static string GenerateLink(AjaxHelper ajaxHelper, string linkText, string targetUrl, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return (string)GenerateLinkMethod.Invoke(null, new object[] { ajaxHelper, linkText, targetUrl, ajaxOptions, htmlAttributes });
        }

        private static MvcForm FormHelper(this AjaxHelper ajaxHelper, string formAction, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return (MvcForm)FormHelperMethod.Invoke(null, new object[] { ajaxHelper, formAction, ajaxOptions, htmlAttributes });
        }

        #endregion

        #region ModuleActionLink

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, (string)null, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, (string)null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, module, null, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ModuleActionLink(ajaxHelper, linkText, actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ModuleActionLink(ajaxHelper, linkText, actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
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
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.ModuleActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), ajaxOptions,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public static MvcHtmlString ModuleActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(linkText));
            }
            var targetUrl = ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues, ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(ajaxHelper, linkText, targetUrl, ajaxOptions, htmlAttributes));
        }

        #endregion

        #region BeginModuleForm

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module,AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
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
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ajaxHelper.BeginModuleForm(actionName, controllerName, module, new RouteValueDictionary(routeValues), ajaxOptions, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="ajaxHelper">The AJAX helper.</param>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public static MvcForm BeginModuleForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var formAction = ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues ?? new RouteValueDictionary(), ajaxHelper.RouteCollection, ajaxHelper.ViewContext.RequestContext, true);
            return ajaxHelper.FormHelper(formAction, ajaxOptions, htmlAttributes);
        }
    
        #endregion
    }
}
