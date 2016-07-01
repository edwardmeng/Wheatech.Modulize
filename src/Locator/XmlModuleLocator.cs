using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Wheatech.Modulize
{
    public class XmlModuleLocator : IModuleLocator
    {
        private const string LocatorFilePath = "~/modules.config";

        public IEnumerable<ModuleLocation> GetLocations()
        {
            var document = new XmlDocument();
            document.Load(PathUtils.ResolvePath(LocatorFilePath));
            var rootElement = document.DocumentElement;
            if (rootElement == null)
            {
                yield break;
            }
            foreach (var location in ReadLocations(rootElement.ChildNodes, null))
            {
                yield return location;
            }
        }

        private IEnumerable<ModuleLocation> ReadLocations(IEnumerable nodes, string moduleType)
        {
            if (nodes == null) yield break;
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
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
                                    ModuleType = moduleType
                                };
                            }
                            break;
                        case "directionary":
                            var dirPath = node.Attributes?.GetNamedItem("path")?.Value;
                            if (!string.IsNullOrWhiteSpace(dirPath))
                            {
                                yield return new ModuleLocation()
                                {
                                    Location = dirPath,
                                    DiscoverStrategy = DiscoverStrategy.Enumerate,
                                    ModuleType = moduleType
                                };
                            }
                            break;
                        default:
                            foreach (var location in ReadLocations(node.ChildNodes, node.Name))
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
