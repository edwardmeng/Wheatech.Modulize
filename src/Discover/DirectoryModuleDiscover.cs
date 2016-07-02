using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml;

namespace Wheatech.Modulize
{
    public class DirectoryModuleDiscover : IModuleDiscover
    {
        public IEnumerable<ModuleDescriptor> Discover(DiscoverContext context)
        {
            var dirInfo = new DirectoryInfo(PathUtils.ResolvePath(context.Location.Location));
            if (!dirInfo.Exists) yield break;
            if (context.Location.DiscoverStrategy == DiscoverStrategy.Enumerate)
            {
                foreach (var directory in dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    var module = DiscoverModule(directory, context);
                    if (module != null)
                    {
                        yield return module;
                    }
                }
            }
            else
            {
                var module = DiscoverModule(dirInfo, context);
                if (module != null)
                {
                    yield return module;
                }
            }
        }

        private ModuleDescriptor DiscoverModule(DirectoryInfo directory, DiscoverContext context)
        {
            ModuleDescriptor module;
            if (context.Manifest.TryParse(directory.FullName, out module))
            {
                var moduleShadow = directory;
                if (context.Location.EnableShadow)
                {
                    var shadowRoot = PathUtils.ResolvePath(context.ShadowPath);
                    var shadowDir = new DirectoryInfo(string.IsNullOrEmpty(context.Location.ModuleType) ? shadowRoot : Path.Combine(shadowRoot, context.Location.ModuleType));
                    if (!shadowDir.Exists)
                    {
                        shadowDir.Create();
                        shadowDir.Attributes |= FileAttributes.Hidden;
                    }
                    moduleShadow = shadowDir.GetDirectories(directory.Name, SearchOption.TopDirectoryOnly).SingleOrDefault();
                    if (moduleShadow == null)
                    {
                        moduleShadow = shadowDir.CreateSubdirectory(directory.Name);
                        moduleShadow.Attributes |= FileAttributes.Hidden;
                    }
                    CopyDirectory(directory, moduleShadow);
                }
                module.ModuleType = context.Location.ModuleType;
                module.ModulePath = directory.FullName;
                module.ShadowPath = moduleShadow.FullName;
                module.Assemblies = FindAssemblies(moduleShadow);
                module.ValidateEntryAssembly();
                return module;
            }
            return null;
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var childInfo in source.GetFileSystemInfos())
            {
                if (childInfo.Attributes.HasFlag(FileAttributes.Directory))
                {
                    var targetSubdir = target.GetDirectories(childInfo.Name, SearchOption.TopDirectoryOnly).SingleOrDefault();
                    if (targetSubdir == null)
                    {
                        targetSubdir = target.CreateSubdirectory(childInfo.Name);
                        targetSubdir.Attributes |= FileAttributes.Hidden;
                    }
                    CopyDirectory((DirectoryInfo)childInfo, targetSubdir);
                }
                else
                {
                    var fileName = Path.Combine(target.FullName, childInfo.Name);
                    var fileInfo = ((FileInfo)childInfo).CopyTo(fileName, true);
                    fileInfo.Attributes |= FileAttributes.Temporary;
                }
            }
        }

        internal static IAssemblyLoader[] FindAssemblies(DirectoryInfo info)
        {
            return FindConfigAssemblies(info).Union(FindFileAssemblies(info, null)).ToArray();
        }

        private static IEnumerable<IAssemblyLoader> FindConfigAssemblies(DirectoryInfo info)
        {
            var configFile =
                info.GetFileSystemInfos("web.config", SearchOption.TopDirectoryOnly).Union(info.GetFileSystemInfos("app.config", SearchOption.TopDirectoryOnly)).FirstOrDefault();
            if (configFile != null)
            {
                var document = new XmlDocument();
                document.Load(configFile.FullName);
                var dependentAssemblyNodes = document.SelectNodes("configuration/runtime/assemblyBinding/dependentAssembly");
                if (dependentAssemblyNodes != null)
                {
                    foreach (XmlNode dependentAssemblyNode in dependentAssemblyNodes)
                    {
                        if (dependentAssemblyNode.NodeType == XmlNodeType.Element)
                        {
                            ConfigAssemblyLoader assemblyInfo;
                            if (ConfigAssemblyLoader.TryParse(dependentAssemblyNode, out assemblyInfo))
                            {
                                yield return assemblyInfo;
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<IAssemblyLoader> FindFileAssemblies(DirectoryInfo info, CultureInfo defaultCulture)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToDictionary(c => c.Name);
            foreach (var childInfo in info.GetFileSystemInfos())
            {
                if (defaultCulture == null && childInfo.Attributes.HasFlag(FileAttributes.Directory) &&
                    (!HostingEnvironment.IsHosted || !string.Equals(childInfo.Name, "bin", StringComparison.OrdinalIgnoreCase)))
                {
                    CultureInfo culture;
                    if (cultures.TryGetValue(childInfo.Name, out culture))
                    {
                        foreach (var assembly in FindFileAssemblies((DirectoryInfo)childInfo, culture))
                        {
                            yield return assembly;
                        }
                    }
                }
                else if (!childInfo.Attributes.HasFlag(FileAttributes.Directory))
                {
                    var assembly = FileAssemblyLoader.Create(childInfo.FullName);
                    if (assembly != null)
                    {
                        yield return assembly;
                    }
                }
            }
            if (HostingEnvironment.IsHosted)
            {
                var bin = info.GetDirectories("bin", SearchOption.TopDirectoryOnly).SingleOrDefault();
                if (bin != null)
                {
                    foreach (var assembly in FindFileAssemblies(bin, defaultCulture))
                    {
                        yield return assembly;
                    }
                }
            }
        }
    }
}
