using System.Collections.Generic;
using System.IO;

namespace Wheatech.Modulize
{
    public class ManifestTable
    {
        private readonly IDictionary<string, IManifestParser> _parsers = new Dictionary<string, IManifestParser>();

        public void SetParser(string filename, IManifestParser parser)
        {
            _parsers[filename] = parser;
        }

        public void SetParser<TParser>(string filename)
            where TParser : IManifestParser, new()
        {
            SetParser(filename, new TParser());
        }

        public bool TryParse(string path, out ModuleDescriptor module)
        {
            module = null;
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists) return false;
            foreach (var parser in _parsers)
            {
                var fileInfo = new FileInfo(Path.Combine(path, parser.Key));
                if (fileInfo.Exists)
                {
                    using (var stream = fileInfo.OpenRead())
                    {
                        module = parser.Value.Parse(stream, dirInfo.Name);
                        if (module != null) return true;
                    }
                }
            }
            return false;
        }
    }
}
