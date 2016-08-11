using System;
using System.Web.Routing;

namespace Wheatech.Modulize.Mvc
{
    internal static class RouteCollectionExtensions
    {
        private static RouteCollection FilterRouteCollection(RouteCollection routes, string areaName, ModuleDescriptor module, out bool usingAreas, out bool usingModules)
        {
            if (areaName == null) areaName = string.Empty;
            usingAreas = false;
            usingModules = false;
            var filteredRoutes = new RouteCollection
            {
                AppendTrailingSlash = routes.AppendTrailingSlash,
                LowercaseUrls = routes.LowercaseUrls,
                RouteExistingFiles = routes.RouteExistingFiles
            };
            var areaFilteredRoutes = new RouteCollection
            {
                AppendTrailingSlash = routes.AppendTrailingSlash,
                LowercaseUrls = routes.LowercaseUrls,
                RouteExistingFiles = routes.RouteExistingFiles
            };
            var moduleFilteredRoutes = new RouteCollection
            {
                AppendTrailingSlash = routes.AppendTrailingSlash,
                LowercaseUrls = routes.LowercaseUrls,
                RouteExistingFiles = routes.RouteExistingFiles
            };
            using (routes.GetReadLock())
            {
                foreach (var route in routes)
                {
                    string routeAreaName = ModuleUtil.GetAreaName(route) ?? string.Empty;
                    usingAreas |= routeAreaName.Length > 0;
                    var routeModule = ModuleUtil.GetModule(route);
                    usingModules |= routeModule != null;
                    var sameAreaName = string.Equals(routeAreaName, areaName, StringComparison.OrdinalIgnoreCase);
                    var sameModule = module == routeModule;
                    if (sameModule)
                    {
                        moduleFilteredRoutes.Add(route);
                    }
                    if (sameAreaName)
                    {
                        areaFilteredRoutes.Add(route);
                    }
                    if (sameModule && sameAreaName)
                    {
                        filteredRoutes.Add(route);
                    }
                }
            }
            if (usingAreas && usingModules)
            {
                return filteredRoutes;
            }
            if (usingModules)
            {
                return moduleFilteredRoutes;
            }
            if (usingAreas)
            {
                return areaFilteredRoutes;
            }
            return routes;
        }

        internal static VirtualPathData GetVirtualPathForModuleArea(this RouteCollection routes, RequestContext requestContext, string name, RouteValueDictionary values, out bool usingAreas, out bool usingModules)
        {
            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }
            if (!string.IsNullOrEmpty(name))
            {
                usingAreas = false;
                usingModules = false;
                return routes.GetVirtualPath(requestContext, name, values);
            }
            string areaName = null;
            ModuleDescriptor moduleDescriptor = null;
            if (values != null)
            {
                object area, module;
                if (values.TryGetValue("area", out area))
                {
                    areaName = area as string;
                }
                else if (requestContext != null)
                {
                    areaName = ModuleUtil.GetAreaName(requestContext.RouteData);
                }
                if (values.TryGetValue("module", out module))
                {
                    moduleDescriptor = module as ModuleDescriptor;
                    if (moduleDescriptor == null)
                    {
                        var moduleId = module as string;
                        if (!string.IsNullOrEmpty(moduleId))
                        {
                            moduleDescriptor = Modulizer.GetModule(moduleId);
                        }
                    }
                }
                else if (requestContext != null)
                {
                    moduleDescriptor = ModuleUtil.GetModule(requestContext.RouteData);
                }
            }
            var dictionary = values;
            var filteredRoutes = FilterRouteCollection(routes, areaName, moduleDescriptor, out usingAreas, out usingModules);
            if (usingAreas)
            {
                dictionary = RouteValuesHelpers.GetRouteValues(dictionary);
                dictionary.Remove("area");
            }
            if (usingModules)
            {
                dictionary = RouteValuesHelpers.GetRouteValues(dictionary);
                dictionary.Remove("module");
            }
            return filteredRoutes.GetVirtualPath(requestContext, RouteValuesHelpers.GetRouteValues(dictionary));
        }

        public static VirtualPathData GetVirtualPathForModuleArea(this RouteCollection routes, RequestContext requestContext, string name, RouteValueDictionary values)
        {
            bool usingAreas, usingModules;
            return routes.GetVirtualPathForModuleArea(requestContext, name, values, out usingAreas,out usingModules);
        }

        public static VirtualPathData GetVirtualPathForModuleArea(this RouteCollection routes, RequestContext requestContext, RouteValueDictionary values)
        {
            return routes.GetVirtualPathForModuleArea(requestContext, null, values);
        }
    }
}
