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
    public class ModuleAjaxHelper
    {
        #region Fields

        private readonly AjaxHelper _ajaxHelper;

        private static readonly MethodInfo GenerateLinkMethod;
        private static readonly MethodInfo FormHelperMethod;

        #endregion

        #region Constructors

        static ModuleAjaxHelper()
        {
            GenerateLinkMethod = typeof(AjaxExtensions).GetMethod("GenerateLink", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                new[]
                {
                    typeof (AjaxHelper), typeof (string), typeof (string), typeof (AjaxOptions), typeof (IDictionary<string, object>)
                }, null);
            FormHelperMethod = typeof(AjaxExtensions).GetMethod("FormHelper", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null,
                new[]
                {
                    typeof (AjaxHelper), typeof (string), typeof (AjaxOptions), typeof (IDictionary<string, object>)
                }, null);
        }

        internal ModuleAjaxHelper(AjaxHelper ajaxHelper)
        {
            _ajaxHelper = ajaxHelper;
        }

        #endregion

        #region Reflection

        private string GenerateLink(string linkText, string targetUrl, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return (string)GenerateLinkMethod.Invoke(null, new object[] { _ajaxHelper, linkText, targetUrl, ajaxOptions, htmlAttributes });
        }

        private MvcForm FormHelper(string formAction, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return (MvcForm)FormHelperMethod.Invoke(null, new object[] { _ajaxHelper, formAction, ajaxOptions, htmlAttributes });
        }

        #endregion

        #region ActionLink

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, (string)null, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, (string)null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, module, null, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLink(linkText, actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>A new <see cref="MvcHtmlString"/> containing the anchor element.</returns>
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return ActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), ajaxOptions,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Returns an anchor element that contains the URL to the specified module action method; 
        /// when the action link is clicked, the action method is invoked asynchronously by using JavaScript.
        /// </summary>
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
        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (string.IsNullOrEmpty(linkText))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(linkText));
            }
            var targetUrl = ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues, _ajaxHelper.RouteCollection, _ajaxHelper.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(GenerateLink(linkText, targetUrl, ajaxOptions, htmlAttributes));
        }

        #endregion

        #region BeginForm

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, null, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(actionName, null, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, null, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
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
        public MvcForm BeginForm(string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(actionName, null, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, null, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, controllerName, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(actionName, controllerName, null, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, null, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
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
        public MvcForm BeginForm(string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
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
        public MvcForm BeginForm(string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(actionName, controllerName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, string module, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, module, null, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes for the element. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, string module, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <param name="htmlAttributes">
        /// An <see cref="System.Collections.Generic.IDictionary{String, Object}"/> that contains the HTML attributes for the element.
        /// </param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, string module,AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            return BeginForm(actionName, controllerName, module, null, ajaxOptions, htmlAttributes);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// A <see cref="RouteValueDictionary"/> that contains the parameters for a route.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
        /// <param name="actionName">The name of the action method that will handle the request.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="module">The ID of the module.</param>
        /// <param name="routeValues">
        /// An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. 
        /// The object is typically created by using object initializer syntax.
        /// </param>
        /// <param name="ajaxOptions">An object that provides options for the asynchronous request.</param>
        /// <returns>An opening &lt;form&gt; tag.</returns>
        public MvcForm BeginForm(string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions)
        {
            return BeginForm(actionName, controllerName, module, routeValues, ajaxOptions, null);
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
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
        public MvcForm BeginForm(string actionName, string controllerName, string module, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            return BeginForm(actionName, controllerName, module, new RouteValueDictionary(routeValues), ajaxOptions, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Writes an opening &lt;form&gt; tag to the response.
        /// </summary>
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
        public MvcForm BeginForm(string actionName, string controllerName, string module, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var formAction = ModuleHelper.GenerateUrl(null, actionName, controllerName, module, routeValues ?? new RouteValueDictionary(), _ajaxHelper.RouteCollection, _ajaxHelper.ViewContext.RequestContext, true);
            return FormHelper(formAction, ajaxOptions, htmlAttributes);
        }
    
        #endregion
    }
}
