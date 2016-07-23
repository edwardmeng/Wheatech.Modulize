using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Wheatech.Modulize.Web
{
    internal class ModulePageRoute : RouteBase
    {
        #region Fields

        private readonly string _module;
        private string _url;
        private IRouteHandler _routeHandler;
        private object _parsedRoute;
        private Route _urlRoute;
        private RouteValueDictionary _defaults;
        private RouteValueDictionary _dataTokens;
        private RouteValueDictionary _constraints;

        private static readonly MethodInfo _routeParseMethod = typeof(Route).Assembly.GetType("System.Web.Routing.RouteParser")
            .GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly MethodInfo _routeMatchMethod = typeof(Route).Assembly.GetType("System.Web.Routing.ParsedRoute")
            .GetMethod("Match", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        #endregion

        #region Constructors

        public ModulePageRoute(string module, string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
        {
            _module = module;
            _url = url;
            _defaults = defaults;
            _dataTokens = dataTokens;
            _constraints = constraints;
            _routeHandler = routeHandler;
        }

        public ModulePageRoute(string module, string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : this(module, url, defaults, constraints, null, routeHandler)
        {
        }

        public ModulePageRoute(string module, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : this(module, url, defaults, null, routeHandler)
        {
        }

        public ModulePageRoute(string module, string url, IRouteHandler routeHandler)
            : this(module, url, null, routeHandler)
        {
        }

        #endregion

        #region Properties

        public string Url
        {
            get
            {
                return _url ?? string.Empty;
            }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    _parsedRoute = string.IsNullOrEmpty(value) ? null : _routeParseMethod.Invoke(null, new object[] { value });
                    _urlRoute = null;
                }
            }
        }

        public RouteValueDictionary Defaults
        {
            get { return _defaults; }
            set
            {
                if (_defaults != value)
                {
                    _urlRoute = null;
                    _defaults = value;
                }
            }
        }

        public RouteValueDictionary DataTokens
        {
            get { return _dataTokens; }
            set
            {
                if (_dataTokens != value)
                {
                    _dataTokens = value;
                    _urlRoute = null;
                }
            }
        }

        public RouteValueDictionary Constraints
        {
            get { return _constraints; }
            set
            {
                if (_constraints != value)
                {
                    _constraints = value;
                    _urlRoute = null;
                }
            }
        }

        public IRouteHandler RouteHandler
        {
            get { return _routeHandler; }
            set
            {
                if (_routeHandler != value)
                {
                    _routeHandler = value;
                    _urlRoute = null;
                }
            }
        }

        private Route UrlRoute => _urlRoute ?? (_urlRoute = string.IsNullOrEmpty(_url) ? null : new Route(_url, _defaults, _constraints, _dataTokens, _routeHandler));

        #endregion

        #region Methods

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            // Parse incoming URL (we trim off the first two chars since they're always "~/")
            var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;

            // Match the specified module.
            string module = RemoveFirstUrlSegment(ref virtualPath);
            if (!string.Equals(module, _module, StringComparison.OrdinalIgnoreCase)) return null;

            var routeData = new RouteData(this, _routeHandler);

            if (_parsedRoute != null)
            {
                // Match the url segments.
                var values = (RouteValueDictionary)_routeMatchMethod.Invoke(_parsedRoute, new object[] { virtualPath, _defaults });
                if (values == null) return null;

                // Validate the values using constraints.
                if (!ProcessConstraints(httpContext, values, RouteDirection.IncomingRequest))
                {
                    return null;
                }
                // Copy the matched values
                foreach (var value in values)
                {
                    routeData.Values.Add(value.Key, value.Value);
                }
            }
            // Copy the DataTokens from the Route to the RouteData
            if (_dataTokens != null)
            {
                foreach (var prop in _dataTokens)
                {
                    routeData.DataTokens[prop.Key] = prop.Value;
                }
            }
            routeData.DataTokens["Module_RelativePath"] = virtualPath;

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            if (string.IsNullOrEmpty(_url)) return null;
            var vpd = UrlRoute.GetVirtualPath(requestContext, values);
            if (vpd == null) return null;
            vpd.VirtualPath = "~/" + _module + "/" + vpd.VirtualPath;
            vpd.Route = this;
            return vpd;
        }

        private static string RemoveFirstUrlSegment(ref string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath)) return null;
            var index = virtualPath.IndexOfAny(new[] { '/', '\\' });
            string segment;
            if (index == -1)
            {
                segment = virtualPath;
                virtualPath = string.Empty;
            }
            else
            {
                segment = virtualPath.Substring(0, index);
                virtualPath = virtualPath.Substring(index + 1);
            }
            return segment;
        }

        private bool ProcessConstraints(HttpContextBase httpContext, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return _constraints == null || _constraints.All(constraintsItem => ProcessConstraint(httpContext, constraintsItem.Value, constraintsItem.Key, values, routeDirection));
        }

        protected virtual bool ProcessConstraint(HttpContextBase httpContext, object constraint, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var customConstraint = constraint as IRouteConstraint;
            if (customConstraint != null)
            {
                return customConstraint.Match(httpContext, UrlRoute, parameterName, values, routeDirection);
            }

            // If there was no custom constraint, then treat the constraint as a string which represents a Regex.
            string constraintsRule = constraint as string;
            if (constraintsRule == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentUICulture,
                    Strings.Route_ValidationMustBeStringOrCustomConstraint,
                    parameterName,
                    Url));
            }

            object parameterValue;
            values.TryGetValue(parameterName, out parameterValue);
            return Regex.IsMatch(Convert.ToString(parameterValue, CultureInfo.InvariantCulture), "^(" + constraintsRule + ")$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        #endregion
    }
}
