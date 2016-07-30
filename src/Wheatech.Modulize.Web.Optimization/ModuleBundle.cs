using System.Linq;
using System.Web.Optimization;
using Wheatech.Modulize.WebHelper;

namespace Wheatech.Modulize.Web.Optimization
{
    public class ModuleBundle
    {
        private readonly Bundle _bundle;
        private readonly ModuleDescriptor _module;

        internal ModuleBundle(ModuleDescriptor module, Bundle bundle)
        {
            _module = module;
            _bundle = bundle;
        }

        public virtual ModuleBundle Include(params string[] virtualPaths)
        {
            if (virtualPaths != null)
            {
                _bundle.Include(virtualPaths.Select(virtualPath => PathHelper.ToAppRelativePath(PathUtils.ResolvePath(_module.ShadowPath, virtualPath))).ToArray());
            }
            return this;
        }

        public virtual ModuleBundle Include(string virtualPath, params IItemTransform[] transforms)
        {
            if (!string.IsNullOrEmpty(virtualPath))
            {
                _bundle.Include(PathHelper.ToAppRelativePath(PathUtils.ResolvePath(_module.ShadowPath, virtualPath)), transforms);
            }
            return this;
        }

        public virtual ModuleBundle IncludeDirectory(string directoryVirtualPath, string searchPattern)
        {
            if (!string.IsNullOrEmpty(directoryVirtualPath))
            {
                _bundle.IncludeDirectory(PathHelper.ToAppRelativePath(PathUtils.ResolvePath(_module.ShadowPath, directoryVirtualPath)), searchPattern);
            }
            return this;
        }

        public virtual ModuleBundle IncludeDirectory(string directoryVirtualPath, string searchPattern, bool searchSubdirectories)
        {
            if (!string.IsNullOrEmpty(directoryVirtualPath))
            {
                _bundle.IncludeDirectory(PathHelper.ToAppRelativePath(PathUtils.ResolvePath(_module.ShadowPath, directoryVirtualPath)), searchPattern, searchSubdirectories);
            }
            return this;
        }
    }
}
