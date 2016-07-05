using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    [Serializable]
    public sealed class FileAssemblyLoader : IAssemblyLoader
    {
        #region Properties

        public string CodeBase { get; internal set; }

        public string CompanyName { get; internal set; }

        public string Description { get; internal set; }

        public string Comments { get; internal set; }

        public System.Version ProductVersion { get; internal set; }

        public System.Version FileVersion { get; internal set; }

        public string Copyright { get; internal set; }

        public string ProductName { get; internal set; }

        public string Trademarks { get; internal set; }

        public string FileName { get; internal set; }

        public string SpecialBuild { get; internal set; }

        public CultureInfo Culture { get; internal set; }

        public string CultureName => Culture?.Name;

        public DateTime CreationTime { get; internal set; }

        public DateTime LastWriteTime { get; internal set; }

        public long Length { get; internal set; }

        public int Priority => 0;

        #endregion

        #region Methods

        public AssemblyIdentity CreateIdentity()
        {
            return new AssemblyIdentity(Path.GetFileNameWithoutExtension(FileName), FileVersion ?? ProductVersion, Culture);
        }

        public AssemblyMatchResult Match(ref AssemblyIdentity assemblyIdentity)
        {
            if (Path.GetFileNameWithoutExtension(FileName) != assemblyIdentity.ShortName)
            {
                return AssemblyMatchResult.Failed;
            }
            if (Culture != null && !string.Equals(Culture.Name, assemblyIdentity.CultureName, StringComparison.OrdinalIgnoreCase))
            {
                return AssemblyMatchResult.Failed;
            }
            return assemblyIdentity.Version == FileVersion || assemblyIdentity.Version == ProductVersion ? AssemblyMatchResult.Success : AssemblyMatchResult.Failed;
        }

        public Assembly Load(ModuleDescriptor module)
        {
            return Assembly.LoadFile(CodeBase);
        }

        private static System.Version ParseVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return null;
            var match = Regex.Match(version, @"^\d+(\.\d+){0,3}");
            if (match.Success)
            {
                return new System.Version(match.Value);
            }
            return null;
        }

        public static FileAssemblyLoader Create(string fileName, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(fileName));
            }
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) || extension.ToLower() != ".dll" || !File.Exists(fileName)) return null;
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            var fileInfo = new FileInfo(fileName);
            if (culture == null)
            {
                RuntimeHelper.TryParseCulture(versionInfo.Language, out culture);
            }
            return new FileAssemblyLoader
            {
                CodeBase = fileInfo.FullName,
                CompanyName = versionInfo.CompanyName,
                Description = versionInfo.FileDescription,
                Comments = versionInfo.Comments,
                FileVersion = ParseVersion(versionInfo.FileVersion),
                ProductVersion = ParseVersion(versionInfo.ProductVersion),
                Copyright = versionInfo.LegalCopyright,
                ProductName = versionInfo.ProductName,
                Trademarks = versionInfo.LegalTrademarks,
                FileName = Path.GetFileName(fileInfo.FullName),
                Culture = culture,
                CreationTime = fileInfo.CreationTime,
                LastWriteTime = fileInfo.LastWriteTime,
                Length = fileInfo.Length,
                SpecialBuild = versionInfo.SpecialBuild
            };
        }

        #endregion
    }
}
