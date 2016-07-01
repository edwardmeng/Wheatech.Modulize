using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class XmlManifestParser : IManifestParser
    {
        public ModuleDescriptor Parse(Stream stream, string moduleId)
        {
            var document = new XmlDocument();
            document.Load(stream);
            var rootElement = document.DocumentElement;
            if (rootElement == null)
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_NoElement, moduleId));
            }
            return ManifestHelper.BuildModule(ReadToDictionary(rootElement), moduleId);
        }

        private Dictionary<string, object> ReadToDictionary(XmlNode parentNode)
        {
            var attributes = parentNode.Attributes;
            var values = new Dictionary<string, object>();
            if (attributes != null)
            {
                foreach (XmlAttribute attribute in attributes)
                {
                    values[attribute.Name] = attribute.Value;
                }
            }
            if (parentNode.HasChildNodes)
            {
                foreach (XmlNode childNode in parentNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        values[childNode.Name] = ReadNodeValue(childNode);
                    }
                }
            }
            return values;
        }

        private object ReadNodeValue(XmlNode node)
        {
            object value = null;
            var attributes = node.Attributes;
            if (attributes != null && attributes.Count == 1)
            {
                var valueAttribute = attributes["value"];
                if (valueAttribute != null)
                {
                    value = valueAttribute.Value;
                }
            }
            if (node.HasChildNodes)
            {
                var childNode = node.FirstChild as XmlCharacterData;
                if (childNode != null)
                {
                    value = childNode.Value;
                }
            }
            return value ?? ReadToDictionary(node);
        }
    }
}
