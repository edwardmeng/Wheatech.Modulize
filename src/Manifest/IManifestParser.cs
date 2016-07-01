using System.IO;

namespace Wheatech.Modulize
{
    public interface IManifestParser
    {
        ModuleDescriptor Parse(Stream stream, string moduleId);
    }
}
