using System;
using System.Collections.Generic;
using System.IO;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class ManifestTable
    {
        private readonly IDictionary<string, IManifestParser> _parsers = new Dictionary<string, IManifestParser>();
        private bool _isReadOnly;

        private void ValidateReadOnly()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException(Strings.Collection_ReadOnly);
            }
        }

        public void SetParser(string filename, IManifestParser parser)
        {
            _parsers[filename] = parser;
        }

        public void SetParser<TParser>(string filename)
            where TParser : IManifestParser, new()
        {
            ValidateReadOnly();
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

        internal void SetReadOnly()
        {
            _isReadOnly = true;
        }
    }
}
