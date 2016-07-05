using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Wheatech.Modulize
{
    public class ConfigAssemblyLoader : IAssemblyLoader
    {
        #region Nest Types

        private class BindingRedirectInformation
        {
            public IVersionComparator OldVersion { get; set; }

            public System.Version NewVersion { get; set; }
        }

        private class CodeBaseInformation
        {
            public System.Version Version { get; set; }

            public string Location { get; set; }
        }

        #endregion

        #region Properties

        private AssemblyIdentity Identity { get; set; }

        private BindingRedirectInformation BindingRedirect { get; set; }

        private CodeBaseInformation CodeBase { get; set; }

        public int Priority => CodeBase == null ? 10 : 9;

        #endregion

        #region Parse

        public static bool TryParse(XmlNode configNode, out ConfigAssemblyLoader assemblyInformation)
        {
            assemblyInformation = null;
            if (configNode == null) return false;
            AssemblyIdentity identity = null;
            BindingRedirectInformation bindingRedirect = null;
            CodeBaseInformation codebase = null;
            foreach (XmlNode childNode in configNode.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    switch (childNode.Name)
                    {
                        case "assemblyIdentity":
                            if (!TryParseAssemblyIdentity(childNode, out identity))
                            {
                                return false;
                            }
                            break;
                        case "bindingRedirect":
                            if (!TryParseBindingRedirect(childNode, out bindingRedirect))
                            {
                                return false;
                            }
                            break;
                        case "codeBase":
                            if (!TryParseCodeBase(childNode, out codebase))
                            {
                                return false;
                            }
                            break;
                    }
                }
            }
            if (identity == null || (bindingRedirect == null && codebase == null)) return false;
            assemblyInformation = new ConfigAssemblyLoader
            {
                Identity = identity,
                BindingRedirect = bindingRedirect,
                CodeBase = codebase
            };
            return true;
        }

        private static bool TryParseCodeBase(XmlNode configNode, out CodeBaseInformation codebase)
        {
            codebase = null;
            var versionText = GetAttribute(configNode, "version");
            var location = GetAttribute(configNode, "href");
            if (string.IsNullOrEmpty(versionText) || string.IsNullOrEmpty(location)) return false;
            System.Version version;
            if (!System.Version.TryParse(versionText, out version)) return false;
            codebase = new CodeBaseInformation
            {
                Version = version,
                Location = location
            };
            return true;
        }

        private static bool TryParseBindingRedirect(XmlNode configNode, out BindingRedirectInformation bindingRedirect)
        {
            bindingRedirect = null;
            var oldVersionText = GetAttribute(configNode, "oldVersion");
            var newVersionText = GetAttribute(configNode, "newVersion");
            if (string.IsNullOrEmpty(oldVersionText) || string.IsNullOrEmpty(newVersionText)) return false;
            IVersionComparator oldVersion;
            if (!VersionComparatorFactory.TryParse(oldVersionText, out oldVersion)) return false;
            System.Version newVersion;
            if (!System.Version.TryParse(newVersionText, out newVersion)) return false;
            bindingRedirect = new BindingRedirectInformation
            {
                OldVersion = oldVersion,
                NewVersion = newVersion
            };
            return true;
        }

        private static bool TryParseAssemblyIdentity(XmlNode configNode, out AssemblyIdentity identity)
        {
            identity = null;
            var name = GetAttribute(configNode, "name");
            if (string.IsNullOrEmpty(name)) return false;
            CultureInfo culture;
            if (!RuntimeHelper.TryParseCulture(GetAttribute(configNode, "culture"), out culture)) return false;
            byte[] publicKeyToken;
            if (!TryParsepPublicKeyToken(GetAttribute(configNode, "publicKeyToken"), out publicKeyToken)) return false;
            ProcessorArchitecture architecture;
            if (!TryParseArchitecture(GetAttribute(configNode, "processorArchitecture"), out architecture)) return false;
            identity = new AssemblyIdentity(name, null, culture, publicKeyToken, architecture);
            return true;
        }

        private static bool TryParsepPublicKeyToken(string publicKeyTokenText, out byte[] publicKeyToken)
        {
            publicKeyToken = null;
            if (string.IsNullOrEmpty(publicKeyTokenText) || string.Equals(publicKeyTokenText, "null", StringComparison.OrdinalIgnoreCase)) return true;
            if (publicKeyTokenText.Length % 2 != 0) return false;
            publicKeyToken = new byte[publicKeyTokenText.Length / 2];
            for (int i = 0; i < publicKeyToken.Length; i++)
            {
                byte byteValue;
                if (!byte.TryParse(publicKeyTokenText.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byteValue))
                {
                    return false;
                }
                publicKeyToken[i] = byteValue;
            }
            return true;
        }

        private static bool TryParseArchitecture(string processorArchitectureText, out ProcessorArchitecture architecture)
        {
            architecture = ProcessorArchitecture.None;
            return string.IsNullOrEmpty(processorArchitectureText) || Enum.TryParse(processorArchitectureText, true, out architecture);
        }

        private static string GetAttribute(XmlNode node, string name)
        {
            return node.Attributes?.GetNamedItem(name)?.Value;
        }

        #endregion

        #region Methods

        public AssemblyIdentity CreateIdentity()
        {
            return new AssemblyIdentity(Identity.ShortName, CodeBase?.Version ?? BindingRedirect?.NewVersion, Identity.Culture, Identity.PublicKeyToken, Identity.Architecture);
        }

        public AssemblyMatchResult Match(ref AssemblyIdentity assemblyIdentity)
        {
            if (!AssemblyIdentityComparer.ShortName.Equals(Identity, assemblyIdentity))
            {
                return AssemblyMatchResult.Failed;
            }
            if (Identity.Culture != null && !string.Equals(Identity.CultureName, assemblyIdentity.CultureName, StringComparison.OrdinalIgnoreCase))
            {
                return AssemblyMatchResult.Failed;
            }
            if (Identity.PublicKeyToken != null && (assemblyIdentity.PublicKeyToken == null || !Identity.PublicKeyToken.SequenceEqual(assemblyIdentity.PublicKeyToken)))
            {
                return AssemblyMatchResult.Failed;
            }
            if (Identity.Architecture != ProcessorArchitecture.None && Identity.Architecture != assemblyIdentity.Architecture)
            {
                return AssemblyMatchResult.Failed;
            }
            if (assemblyIdentity.Version != null)
            {
                if (BindingRedirect != null && !BindingRedirect.OldVersion.Match(assemblyIdentity.Version))
                {
                    return AssemblyMatchResult.Failed;
                }
                if (CodeBase != null && CodeBase.Version != assemblyIdentity.Version)
                {
                    return AssemblyMatchResult.Failed;
                }
            }
            if (BindingRedirect != null)
            {
                assemblyIdentity = new AssemblyIdentity(Identity.ShortName, BindingRedirect.NewVersion, Identity.Culture, Identity.PublicKeyToken, Identity.Architecture);
                return CodeBase != null ? AssemblyMatchResult.RedirectAndMatch : AssemblyMatchResult.Redirect;
            }
            return CodeBase != null ? AssemblyMatchResult.Success : AssemblyMatchResult.Failed;
        }

        public Assembly Load(ModuleDescriptor module)
        {
            if (CodeBase == null) return null;
            var location = CodeBase.Location;
            if (location.StartsWith("~/"))
            {
                location = Path.Combine(module.ShadowPath, location.Substring(2));
            }
            else if (!IsAbsolutePhysicalPath(location) && !IsUriPath(location))
            {
                location = Path.Combine(module.ShadowPath, location);
            }
            return Assembly.LoadFrom(location);
        }

        public static bool IsAbsolutePhysicalPath(string path)
        {
            if (path == null || path.Length < 3)
            {
                return false;
            }
            return path[1] == ':' && IsDirectorySeparatorChar(path[2]) || IsUncSharePath(path);
        }

        private static bool IsDirectorySeparatorChar(char ch)
        {
            return ch == '\\' || ch == '/';
        }

        private static bool IsUncSharePath(string path)
        {
            return path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]);
        }

        public static bool IsUriPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            var colonIndex = path.IndexOf(":", StringComparison.Ordinal);
            if (colonIndex == -1) return false;
            if (path.Length < colonIndex + 3 || path[colonIndex + 1] != '/' || path[colonIndex + 2] != '/') return false;
            var scheme = path.Substring(0, colonIndex).Trim().ToLower();
            return scheme == "http" || scheme == "https" || scheme == "ftp" || scheme == "file";
        }

        #endregion
    }
}
