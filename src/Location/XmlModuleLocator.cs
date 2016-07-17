using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Wheatech.Modulize
{
    public class XmlModuleLocator : IModuleLocator
    {
        private const string LocatorFilePath = "~/modules.config";

        public IEnumerable<ModuleLocation> GetLocations()
        {
            var filePath = PathUtils.ResolvePath(LocatorFilePath);
            if (!File.Exists(filePath)) yield break;
            var document = new XmlDocument();
            document.Load(filePath);
            var rootElement = document.DocumentElement;
            if (rootElement == null)
            {
                yield break;
            }
            foreach (var location in ReadLocations(rootElement.ChildNodes, null, GetEnableShadow(rootElement, false)))
            {
                yield return location;
            }
        }

        private bool GetEnableShadow(XmlNode node, bool defaultValue)
        {
            var shadowText = node.Attributes?.GetNamedItem("enableShadow")?.Value;
            bool nodeShadow;
            if (string.IsNullOrEmpty(shadowText) || !bool.TryParse(shadowText, out nodeShadow))
            {
                nodeShadow = defaultValue;
            }
            return nodeShadow;
        }

        private IEnumerable<ModuleLocation> ReadLocations(IEnumerable nodes, string moduleType, bool enableShadow)
        {
            if (nodes == null) yield break;
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    bool nodeShadow = GetEnableShadow(node, enableShadow);
                    switch (node.Name)
                    {
                        case "module":
                            var modulePath = node.Attributes?.GetNamedItem("path")?.Value;
                            if (!string.IsNullOrWhiteSpace(modulePath))
                            {
                                yield return new ModuleLocation
                                {
                                    Location = modulePath,
                                    DiscoverStrategy = DiscoverStrategy.Single,
                                    ModuleType = moduleType,
                                    EnableShadow = nodeShadow
                                };
                            }
                            break;
                        case "directory":
                            var dirPath = node.Attributes?.GetNamedItem("path")?.Value;
                            if (!string.IsNullOrWhiteSpace(dirPath))
                            {
                                yield return new ModuleLocation
                                {
                                    Location = dirPath,
                                    DiscoverStrategy = DiscoverStrategy.Enumerate,
                                    ModuleType = moduleType,
                                    EnableShadow = nodeShadow
                                };
                            }
                            break;
                        default:
                            foreach (var location in ReadLocations(node.ChildNodes, node.Name, nodeShadow))
                            {
                                yield return location;
                            }
                            break;
                    }
                }
            }
        }
    }
}
