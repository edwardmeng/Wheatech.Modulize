using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class TextModuleLocator : IModuleLocator
    {
        private const string LocatorFilePath = "~/modules.txt";

        public IEnumerable<ModuleLocation> GetLocations()
        {
            var fileInfo = new FileInfo(PathUtils.ResolvePath(LocatorFilePath));
            if (!fileInfo.Exists)
            {
                yield break;
            }
            using (var stream = fileInfo.OpenRead())
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
                    var lineIndex = 0;
                    while (lineIndex < linesArray.Length)
                    {
                        foreach (var location in RemoveRootLocations(linesArray, ref lineIndex))
                        {
                            yield return location;
                        }
                        foreach (var location in ReadChildLocations(linesArray,ref lineIndex))
                        {
                            yield return location;
                        }
                    }
                }
            }
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

        private IEnumerable<ModuleLocation> ReadChildLocations(string[] lines, ref int lineIndex)
        {
            string moduleType = null;
            if (lineIndex < lines.Length)
            {
                moduleType = lines[lineIndex].Trim();
                lineIndex++;
            }
            var indent = -1;
            var locations = new List<ModuleLocation>();
            while (lineIndex < lines.Length)
            {
                var lineText = lines[lineIndex];
                var currentIndent = GetIndents(lineText);
                if (currentIndent == 0)
                {
                    break;
                }
                if (indent == -1)
                {
                    indent = currentIndent;
                }
                else if (currentIndent != indent)
                {
                    throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Locator_InvalidTextLine, lineIndex + 1, LocatorFilePath));
                }
                lineText = lineText.Substring(indent).Trim();
                locations.Add(new ModuleLocation
                {
                    ModuleType = moduleType,
                    Location = lineText,
                    DiscoverStrategy = DiscoverStrategy.Single
                });
            }
            return locations;
        }

        private IEnumerable<ModuleLocation> RemoveRootLocations(string[] lines, ref int lineIndex)
        {
            var locations = new List<ModuleLocation>();
            while (lineIndex < lines.Length)
            {
                var lineText = lines[lineIndex];
                var currentIndent = GetIndents(lineText);
                if (currentIndent > 0)
                {
                    throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Locator_InvalidTextLine, lineIndex + 1, LocatorFilePath));
                }
                if (lineIndex + 1 < lines.Length)
                {
                    var nextIndent = GetIndents(lines[lineIndex + 1]);
                    if (nextIndent > 0) break;
                }
                var colonIndex = lineText.IndexOf(':');
                if (colonIndex != -1)
                {
                    locations.Add(new ModuleLocation
                    {
                        Location = lineText.Substring(colonIndex + 1).Trim(),
                        ModuleType = lineText.Substring(0, colonIndex).Trim(),
                        DiscoverStrategy = DiscoverStrategy.Enumerate
                    });
                }
                else
                {
                    locations.Add(new ModuleLocation
                    {
                        Location = lineText,
                        DiscoverStrategy = DiscoverStrategy.Single
                    });
                }
                lineIndex++;
            }
            return locations;
        }
    }
}
