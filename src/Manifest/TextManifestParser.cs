using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class TextManifestParser : IManifestParser
    {
        public ModuleDescriptor Parse(Stream stream, string moduleId)
        {
            using (var reader = new StreamReader(stream))
            {
                var lines = new List<string>();
                string lineText;
                while ((lineText = reader.ReadLine()) != null)
                {
                    lines.Add(lineText);
                }
                var linesArray = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                if (linesArray.Length == 0)
                {
                    throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_NoElement, moduleId));
                }
                int lineIndex = 0;
                return ManifestHelper.BuildModule(ReadToDictionary(linesArray, 0, moduleId, ref lineIndex), moduleId);
            }
        }

        private Dictionary<string, object> ReadToDictionary(string[] lines, int indent, string moduleId, ref int lineIndex)
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            while (lineIndex < lines.Length)
            {
                var lineText = lines[lineIndex];
                var currentIndent = GetIndents(lineText);
                if (currentIndent < indent) break;
                if (currentIndent > indent) throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidTextLine, lineIndex + 1, moduleId));

                lineText = lineText.Substring(indent);
                string name, value = null;
                var colonIndex = lineText.IndexOf(':');
                if (colonIndex == -1)
                {
                    name = lineText;
                }
                else
                {
                    name = lineText.Substring(0, colonIndex).Trim();
                    value = lineText.Substring(colonIndex + 1).Trim();
                    if (value.Length == 0) value = null;
                }
                values[name] = value;
                if (lineIndex < lines.Length - 1)
                {
                    var nextIndent = GetIndents(lines[lineIndex + 1]);
                    if (nextIndent > indent)
                    {
                        lineIndex++;
                        values[name] = ReadToDictionary(lines, nextIndent, moduleId, ref lineIndex);
                        continue;
                    }
                    if (nextIndent == indent)
                    {
                        lineIndex++;
                    }
                    else
                    {
                        lineIndex++;
                        break;
                    }
                }
                else
                {
                    lineIndex++;
                }
            }
            return values;
        }

        private int GetIndents(string lineText)
        {
            for (int i = 0; i < lineText.Length; i++)
            {
                if (!char.IsWhiteSpace(lineText, i))
                {
                    return i;
                }
            }
            return lineText.Length;
        }
    }
}
