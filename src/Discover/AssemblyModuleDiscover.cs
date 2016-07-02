using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Wheatech.Modulize
{
    public class AssemblyModuleDiscover : IModuleDiscover
    {
        public IEnumerable<ModuleDescriptor> Discover(DiscoverContext context)
        {
            var path = PathUtils.ResolvePath(context.Location.Location);
            if (context.Location.DiscoverStrategy == DiscoverStrategy.Single)
            {
                var module = DiscoverModule(new FileInfo(path), context);
                if (module != null)
                {
                    yield return module;
                }
            }
            if (context.Location.DiscoverStrategy == DiscoverStrategy.Enumerate)
            {
                var dirInfo = new DirectoryInfo(path);
                if (dirInfo.Exists)
                {
                    foreach (var info in dirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                    {
                        var module = DiscoverModule(info, context);
                        if (module != null)
                        {
                            yield return module;
                        }
                    }
                }
            }
        }

        private ModuleDescriptor DiscoverModule(FileInfo fileInfo, DiscoverContext context)
        {
            if (fileInfo.Exists && fileInfo.Extension.ToLower() == ".dll")
            {
                var module = GetFileModule(fileInfo.FullName);
                if (module != null)
                {
                    if (context.Location.EnableShadow)
                    {
                        var shadowRoot = PathUtils.ResolvePath(context.ShadowPath);
                        var shadowDir = new DirectoryInfo(string.IsNullOrEmpty(context.Location.ModuleType) ? shadowRoot : Path.Combine(shadowRoot, context.Location.ModuleType));
                        if (!shadowDir.Exists)
                        {
                            shadowDir.Create();
                            shadowDir.Attributes |= FileAttributes.Hidden;
                        }
                        var shadowFile = fileInfo.CopyTo(Path.Combine(shadowDir.FullName, fileInfo.Name), true);
                        shadowFile.Attributes |= FileAttributes.Temporary;
                        module.ShadowPath = shadowFile.FullName;
                    }
                    else
                    {
                        module.ShadowPath = fileInfo.FullName;
                    }
                    module.ModuleType = context.Location.ModuleType;
                    module.ModulePath = fileInfo.FullName;
                    module.Assemblies = new IAssemblyLoader[] { FileAssemblyLoader.Create(module.ShadowPath) };
                    return module;
                }
            }
            return null;
        }

        private ModuleDescriptor GetFileModule(string filePath)
        {
            var assemblyInformation = FileAssemblyLoader.Create(filePath);
            if (assemblyInformation == null) return null;
            var assemblyIdentity = assemblyInformation.CreateIdentity();
            var productName = string.IsNullOrEmpty(assemblyInformation.ProductName) ? assemblyIdentity.ShortName : assemblyInformation.ProductName;
            var description = string.IsNullOrEmpty(assemblyInformation.Description) ? assemblyInformation.Comments : assemblyInformation.Description;
            var attributes = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(assemblyInformation.Trademarks))
            {
                attributes.Add("Trademarks", assemblyInformation.Trademarks);
            }
            if (!string.IsNullOrEmpty(assemblyInformation.Copyright))
            {
                attributes.Add("Copyright", assemblyInformation.Copyright);
            }
            var module = new ModuleDescriptor
            {
                ModuleId = assemblyIdentity.ShortName,
                ModuleName = productName,
                Description = description,
                ModuleVersion = new Version((assemblyInformation.FileVersion ?? assemblyInformation.ProductVersion) +
                                            (string.IsNullOrEmpty(assemblyInformation.SpecialBuild) ? null : "+" + assemblyInformation.SpecialBuild)),
                Authors = string.IsNullOrEmpty(assemblyInformation.CompanyName) ? null : new[] { assemblyInformation.CompanyName },
                Attributes = attributes,
                EntryAssembly = assemblyIdentity
            };
            var feature = new FeatureDescriptor
            {
                FeatureId = assemblyIdentity.ShortName,
                FeatureName = productName,
                Description = description,
                EntryAssembly = assemblyIdentity,
                Module = module
            };
            module.Features = new FeatureDescriptorCollection(feature);
            return module;
        }
    }
}
