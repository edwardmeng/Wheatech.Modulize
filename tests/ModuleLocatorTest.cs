using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ModuleLocatorTest
    {
        private const string JsonLocatorPath = "~/modules.json";
        private const string TextLocatorPath = "~/modules.txt";
        private const string XmlLocatorPath = "~/modules.config";

        private void CreateTextFile(string filePath, string fileContent)
        {
            using (var stream = File.Open(PathUtils.ResolvePath(filePath),FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(fileContent);
                }
            }
        }

        private void RemoveTextFile(string filePath)
        {
            var fileInfo = new FileInfo(PathUtils.ResolvePath(filePath));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [Fact]
        public void ParseJsonSimpleList()
        {
            CreateTextFile(JsonLocatorPath,"['~/modules/email','~/modules/workflow']");
            try
            {
                var locations = new JsonModuleLocator().GetLocations().ToArray();
            }
            catch (Exception)
            {
                RemoveTextFile(JsonLocatorPath);
                throw;
            }
        }
    }
}
