using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace Wheatech.Modulize
{
    internal class CompressionModuleDiscover : IModuleDiscover
    {
        public IEnumerable<ModuleDescriptor> Discover(DiscoverContext context)
        {
            var path = PathUtils.ResolvePath(context.Location.Location);
            if (context.Location.DiscoverStrategy == DiscoverStrategy.Single)
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    var module = DiscoverModule(fileInfo, context);
                    if (module!=null)
                    {
                        yield return module;
                    }
                }
            }
            else
            {
                var dirInfo = new DirectoryInfo(path);
                if (dirInfo.Exists)
                {
                    foreach (var info in dirInfo.GetFiles("*.zip", SearchOption.TopDirectoryOnly).Union(dirInfo.GetFiles("*.tar", SearchOption.TopDirectoryOnly)))
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
            var shadowDir = new DirectoryInfo(context.ShadowPath);
            switch (fileInfo.Extension.ToLower())
            {
                case ".zip":
                    shadowDir = DecompressZip(fileInfo, shadowDir);
                    break;
                case ".tar":
                    shadowDir = DecompressTar(fileInfo, shadowDir);
                    break;
                default:
                    return null;
            }
            ModuleDescriptor module;
            if (context.Manifest.TryParse(shadowDir.FullName, out module))
            {
                module.ModuleType = context.Location.ModuleType;
                module.ModulePath = fileInfo.FullName;
                module.ShadowPath = shadowDir.FullName;
                module.Assemblies = DirectoryModuleDiscover.FindAssemblies(shadowDir);
                module.ValidateEntryAssembly();
            }
            return module;
        }

        private DirectoryInfo DecompressZip(FileInfo fileToUnZip, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
                destination.Attributes |= FileAttributes.Hidden;
            }
            var moduleName = Path.GetFileNameWithoutExtension(fileToUnZip.Name);
            if (!string.IsNullOrEmpty(moduleName))
            {
                var moduleDir = destination.GetDirectories(moduleName, SearchOption.TopDirectoryOnly).SingleOrDefault();
                if (moduleDir == null)
                {
                    moduleDir = destination.CreateSubdirectory(moduleName);
                    moduleDir.Attributes |= FileAttributes.Hidden;
                }
                destination = moduleDir;
            }
            using (var inputStream = new ZipInputStream(fileToUnZip.OpenRead()))
            {
                ZipEntry zipEntry;
                while ((zipEntry = inputStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);

                    // create directory
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        directoryName = Path.Combine(destination.FullName, directoryName);
                        if (!Directory.Exists(directoryName))
                        {
                            var directory = Directory.CreateDirectory(directoryName);
                            directory.Attributes |= FileAttributes.Hidden;
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var filePath = Path.Combine(destination.FullName, zipEntry.Name);
                        using (var streamWriter = File.Create(filePath))
                        {
                            inputStream.CopyTo(streamWriter, 2048);
                        }
                        var fileInfo = new FileInfo(filePath);
                        fileInfo.Attributes |= FileAttributes.Temporary;
                    }
                }
            }
            return destination;
        }

        private DirectoryInfo DecompressTar(FileInfo fileToUnZip, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
                destination.Attributes |= FileAttributes.Hidden;
            }
            var moduleName = Path.GetFileNameWithoutExtension(fileToUnZip.Name);
            if (!string.IsNullOrEmpty(moduleName))
            {
                var moduleDir = destination.GetDirectories(moduleName, SearchOption.TopDirectoryOnly).SingleOrDefault();
                if (moduleDir == null)
                {
                    moduleDir = destination.CreateSubdirectory(moduleName);
                    moduleDir.Attributes |= FileAttributes.Hidden;
                }
                destination = moduleDir;
            }

            using (var inputStream = new TarInputStream(fileToUnZip.OpenRead()))
            {
                TarEntry zipEntry;
                while ((zipEntry = inputStream.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);

                    // create directory
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        directoryName = Path.Combine(destination.FullName, directoryName);
                        if (!Directory.Exists(directoryName))
                        {
                            var directory = Directory.CreateDirectory(directoryName);
                            directory.Attributes |= FileAttributes.Hidden;
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var filePath = Path.Combine(destination.FullName, zipEntry.Name);
                        using (var streamWriter = File.Create(filePath))
                        {
                            inputStream.CopyTo(streamWriter, 2048);
                        }
                        var fileInfo = new FileInfo(filePath);
                        fileInfo.Attributes |= FileAttributes.Temporary;
                    }
                }
            }
            return destination;
        }
    }
}
