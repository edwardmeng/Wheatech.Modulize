using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wheatech.Modulize.Mvc.Properties;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Mvc
{
    public class ModuleRazorViewEngine : RazorViewEngine
    {
        #region ViewLocations

        private class ViewLocation
        {
            public ViewLocation(string virtualPathFormatString)
            {
                VirtualPathFormatString = virtualPathFormatString;
            }

            protected string VirtualPathFormatString { get; }

            public virtual string Format(string viewName, string controllerName, string areaName, string module)
            {
                return string.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName);
            }
        }

        private class AreaAwareViewLocation : ViewLocation
        {
            public AreaAwareViewLocation(string virtualPathFormatString)
                : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string module)
            {
                return String.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName, areaName);
            }
        }

        private class ModuleAwareViewLocation : ViewLocation
        {
            public ModuleAwareViewLocation(string virtualPathFormatString) : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string module)
            {
                return String.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName, module);
            }
        }

        private class ModuleAreaAwareViewLocation : ViewLocation
        {
            public ModuleAreaAwareViewLocation(string virtualPathFormatString) : base(virtualPathFormatString)
            {
            }

            public override string Format(string viewName, string controllerName, string areaName, string module)
            {
                return String.Format(CultureInfo.InvariantCulture, VirtualPathFormatString, viewName, controllerName, areaName, module);
            }
        }

        #endregion

        private const string CacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";
        private const string CacheKeyPrefixMaster = "Master";
        private const string CacheKeyPrefixPartial = "Partial";
        private const string CacheKeyPrefixView = "View";
        private readonly Func<string, string> GetExtensionThunk = VirtualPathUtility.GetExtension;

        public ModuleRazorViewEngine():this(null)
        {
        }

        public ModuleRazorViewEngine(IViewPageActivator viewPageActivator) : base(viewPageActivator)
        {
            ModuleAreaViewLocationFormats = new[]
            {
                "{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "{3}/Areas/{2}/Views/{1}/{0}.vbhtml",
                "{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "{3}/Areas/{2}/Views/Shared/{0}.vbhtml"
            };
            ModuleAreaMasterLocationFormats = ModuleAreaViewLocationFormats;
            ModuleAreaPartialLocationFormats = ModuleAreaViewLocationFormats;
            ModuleViewLocationFormats=new[]
            {
                "{2}/Views/{1}/{0}.cshtml",
                "{2}/Views/{1}/{0}.vbhtml",
                "{2}/Views/Shared/{0}.cshtml",
                "{2}/Views/Shared/{0}.vbhtml"
            };
            ModuleMasterLocationFormats = ModuleViewLocationFormats;
            ModulePartialViewLocationFormats = ModuleViewLocationFormats;
        }

        public string[] ModuleViewLocationFormats { get; set; }

        public string[] ModulePartialViewLocationFormats { get; set; }

        public string[] ModuleMasterLocationFormats { get; set; }

        public string[] ModuleAreaViewLocationFormats { get; set; }

        public string[] ModuleAreaMasterLocationFormats { get; set; }

        public string[] ModuleAreaPartialLocationFormats { get; set; }

        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return c == '~' || c == '/';
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string areaName, string module)
        {
            return string.Format(CultureInfo.InvariantCulture, CacheKeyFormat,
                                 GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName, module);
        }

        private static string AppendDisplayModeToCacheKey(string cacheKey, string displayMode)
        {
            return cacheKey + displayMode + ":";
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException(nameof(controllerContext));
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(viewName));
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string viewPath = GetPath(controllerContext, ViewLocationFormats, AreaViewLocationFormats, ModuleViewLocationFormats, ModuleAreaViewLocationFormats, viewName, controllerName, CacheKeyPrefixView, useCache, out viewLocationsSearched);
            string masterPath = GetPath(controllerContext, MasterLocationFormats, AreaMasterLocationFormats, ModuleMasterLocationFormats, ModuleAreaMasterLocationFormats, masterName, controllerName, CacheKeyPrefixMaster, useCache, out masterLocationsSearched);

            if (string.IsNullOrEmpty(viewPath) || (string.IsNullOrEmpty(masterPath) && !string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            }

            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException(nameof(controllerContext));
            }
            if (string.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(partialViewName));
            }

            string[] searched;
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string partialPath = GetPath(controllerContext, PartialViewLocationFormats, AreaPartialViewLocationFormats, ModulePartialViewLocationFormats, ModuleAreaPartialLocationFormats, partialViewName, controllerName, CacheKeyPrefixPartial, useCache, out searched);

            if (string.IsNullOrEmpty(partialPath))
            {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations, string[] moduleLocations, string[] moduleAreaLocations, string name, string controllerName,
            string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = new string[0];
            if (string.IsNullOrEmpty(name)) return string.Empty;
            var module = ModuleHelper.GetModule(controllerContext.RouteData);
            bool usingModule = !string.IsNullOrEmpty(module);
            var area = ModuleHelper.GetAreaName(controllerContext.RouteData);
            bool usingAreas = !string.IsNullOrEmpty(area);
            var viewLocations = GetViewLocations(locations, usingAreas ? areaLocations : null, usingModule ? moduleLocations : null,
                usingModule && usingAreas ? moduleAreaLocations : null);
            bool nameRepresentsPath = IsSpecificPath(name);
            string cacheKey = CreateCacheKey(cacheKeyPrefix, name, nameRepresentsPath ? string.Empty : controllerName, area, module);
            if (useCache)
            {
                // Only look at cached display modes that can handle the context.
                var possibleDisplayModes = DisplayModeProvider.GetAvailableDisplayModesForContext(controllerContext.HttpContext, controllerContext.DisplayMode);
                foreach (var displayMode in possibleDisplayModes)
                {
                    string cachedLocation = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, AppendDisplayModeToCacheKey(cacheKey, displayMode.DisplayModeId));
                    if (cachedLocation == null)
                    {
                        // If any matching display mode location is not in the cache, fall back to the uncached behavior, which will repopulate all of our caches.
                        return null;
                    }

                    // A non-empty cachedLocation indicates that we have a matching file on disk. Return that result.
                    if (cachedLocation.Length > 0)
                    {
                        if (controllerContext.DisplayMode == null)
                        {
                            controllerContext.DisplayMode = displayMode;
                        }

                        return cachedLocation;
                    }
                    // An empty cachedLocation value indicates that we don't have a matching file on disk. Keep going down the list of possible display modes.
                }
                // GetPath is called again without using the cache.
                return null;
            }
            return nameRepresentsPath
                ? GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations)
                : GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, area, module, cacheKey, ref searchedLocations);
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string moduleName, string cacheKey,
            ref string[] searchedLocations)
        {
            string result = string.Empty;
            searchedLocations = new string[locations.Count];
            var module = string.IsNullOrEmpty(moduleName) ? null : Modulizer.GetModule(moduleName);
            var modulePath = module == null ? null : PathHelper.ToAppRelativePath(controllerContext.HttpContext, module.ShadowPath);
            for (int i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                string virtualPath = location.Format(name, controllerName, areaName, modulePath);
                var virtualPathDisplayInfo = DisplayModeProvider.GetDisplayInfoForVirtualPath(virtualPath, controllerContext.HttpContext, path => FileExists(controllerContext, path), controllerContext.DisplayMode);

                if (virtualPathDisplayInfo != null)
                {
                    string resolvedVirtualPath = virtualPathDisplayInfo.FilePath;

                    searchedLocations = new string[0];
                    result = resolvedVirtualPath;
                    ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, AppendDisplayModeToCacheKey(cacheKey, virtualPathDisplayInfo.DisplayMode.DisplayModeId), result);

                    if (controllerContext.DisplayMode == null)
                    {
                        controllerContext.DisplayMode = virtualPathDisplayInfo.DisplayMode;
                    }

                    // Populate the cache for all other display modes. We want to cache both file system hits and misses so that we can distinguish
                    // in future requests whether a file's status was evicted from the cache (null value) or if the file doesn't exist (empty string).
                    foreach (var displayMode in DisplayModeProvider.Modes)
                    {
                        if (displayMode.DisplayModeId != virtualPathDisplayInfo.DisplayMode.DisplayModeId)
                        {
                            var displayInfoToCache = displayMode.GetDisplayInfo(controllerContext.HttpContext, virtualPath, path => FileExists(controllerContext, path));
                            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, AppendDisplayModeToCacheKey(cacheKey, displayMode.DisplayModeId), displayInfoToCache?.FilePath ?? string.Empty);
                        }
                    }
                    break;
                }

                searchedLocations[i] = virtualPath;
            }

            return result;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string result = name;

            if (!(FilePathIsSupported(name) && FileExists(controllerContext, name)))
            {
                result = string.Empty;
                searchedLocations = new[] { name };
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private bool FilePathIsSupported(string virtualPath)
        {
            if (FileExtensions == null)
            {
                // legacy behavior for custom ViewEngine that might not set the FileExtensions property
                return true;
            }
            else
            {
                // get rid of the '.' because the FileExtensions property expects extensions withouth a dot.
                string extension = GetExtensionThunk(virtualPath).TrimStart('.');
                return FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }
        }

        private static List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats, string[] moduleViewLocationFormats, string[] moduleAreaViewLocationFormats)
        {
            List<ViewLocation> allLocations = new List<ViewLocation>();
            if (moduleAreaViewLocationFormats != null)
            {
                allLocations.AddRange(moduleAreaViewLocationFormats.Select(locationFormat => new ModuleAreaAwareViewLocation(locationFormat)));
            }
            if (moduleViewLocationFormats != null)
            {
                allLocations.AddRange(moduleViewLocationFormats.Select(locationFormat => new ModuleAwareViewLocation(locationFormat)));
            }
            if (areaViewLocationFormats != null)
            {
                allLocations.AddRange(areaViewLocationFormats.Select(locationFormat => new AreaAwareViewLocation(locationFormat)));
            }
            if (viewLocationFormats != null)
            {
                allLocations.AddRange(viewLocationFormats.Select(locationFormat => new ViewLocation(locationFormat)));
            }
            return allLocations;
        }
    }
}
