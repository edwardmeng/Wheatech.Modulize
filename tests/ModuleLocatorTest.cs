using System.IO;
using System.Linq;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ModuleLocatorTest
    {
        private const string JsonLocatorPath = "~/modules.json";
        private const string XmlLocatorPath = "~/modules.config";

        private void CreateTextFile(string filePath, string fileContent)
        {
            using (var stream = File.Open(PathUtils.ResolvePath(filePath), FileMode.Create))
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
        public void ParseJsonSimpleListLocation()
        {
            CreateTextFile(JsonLocatorPath, "['~/modules/email','~/modules/workflow']");
            try
            {
                var locations = new JsonModuleLocator().GetLocations().ToArray();
                Assert.Equal(2, locations.Length);
                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);
                Assert.False(locations[0].EnableShadow);

                Assert.Equal("~/modules/workflow", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[1].DiscoverStrategy);
                Assert.Null(locations[1].ModuleType);
                Assert.False(locations[1].EnableShadow);
            }
            finally
            {
                RemoveTextFile(JsonLocatorPath);
            }
        }

        [Fact]
        public void ParseJsonSimpleObjectLocation()
        {
            CreateTextFile(JsonLocatorPath, "{'library':'~/libraries','theme':'~/themes'}");
            try
            {
                var locations = new JsonModuleLocator().GetLocations().ToArray();
                Assert.Equal(2, locations.Length);
                Assert.Equal("~/libraries", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[0].DiscoverStrategy);
                Assert.Equal("library", locations[0].ModuleType);

                Assert.Equal("~/themes", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[1].DiscoverStrategy);
                Assert.Equal("theme", locations[1].ModuleType);
            }
            finally
            {
                RemoveTextFile(JsonLocatorPath);
            }
        }

        [Fact]
        public void ParseJsonComplexLocation()
        {
            CreateTextFile(JsonLocatorPath,
                @"
[
'~/modules/email',
{
    'library':'~/libraries',
    'theme':['~/themes/metronic', '~/themes/black']
},
'~/modules/workflow']
");
            try
            {
                var locations = new JsonModuleLocator().GetLocations().ToArray();
                Assert.Equal(5, locations.Length);
                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);

                Assert.Equal("~/libraries", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[1].DiscoverStrategy);
                Assert.Equal("library", locations[1].ModuleType);

                Assert.Equal("~/themes/metronic", locations[2].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[2].DiscoverStrategy);
                Assert.Equal("theme", locations[2].ModuleType);

                Assert.Equal("~/themes/black", locations[3].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[3].DiscoverStrategy);
                Assert.Equal("theme", locations[3].ModuleType);

                Assert.Equal("~/modules/workflow", locations[4].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[4].DiscoverStrategy);
                Assert.Null(locations[4].ModuleType);
            }
            finally
            {
                RemoveTextFile(JsonLocatorPath);
            }
        }

        [Fact]
        public void ParseJsonSettingsLocation()
        {
            CreateTextFile(JsonLocatorPath, @"
{
    enableShadow: true,
    modules:[
        '~/modules/email',
        '~/modules/workflow'
    ]
}
");
            try
            {
                var locations = new JsonModuleLocator().GetLocations().ToArray();
                Assert.Equal(2, locations.Length);
                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);
                Assert.True(locations[0].EnableShadow);

                Assert.Equal("~/modules/workflow", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[1].DiscoverStrategy);
                Assert.Null(locations[1].ModuleType);
                Assert.True(locations[1].EnableShadow);
            }
            finally
            {
                RemoveTextFile(JsonLocatorPath);
            }
        }

        [Fact]
        public void ParseXmlSimpleListLocation()
        {
            CreateTextFile(XmlLocatorPath, @"<?xml version=""1.0"" encoding=""utf-8""?>
<locations>
    <module path=""~/modules/email""/>
    <directory path=""~/library""/>
</locations>
");
            try
            {
                var locations = new XmlModuleLocator().GetLocations().ToArray();

                Assert.Equal(2, locations.Length);
                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);
                Assert.False(locations[0].EnableShadow);

                Assert.Equal("~/library", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[1].DiscoverStrategy);
                Assert.Null(locations[1].ModuleType);
                Assert.False(locations[1].EnableShadow);
            }
            finally
            {
                RemoveTextFile(XmlLocatorPath);
            }
        }

        [Fact]
        public void ParseXmlComplexLocation()
        {
            CreateTextFile(XmlLocatorPath, @"<?xml version=""1.0"" encoding=""utf-8""?>
<locations>
    <module path=""~/modules/email""/>
    <directory path=""~/library""/>
    <theme>
        <module path=""~/themes/metronic""/>
        <module path=""~/themes/black""/>
    </theme>
</locations>
");
            try
            {
                var locations = new XmlModuleLocator().GetLocations().ToArray();
                Assert.Equal(4, locations.Length);

                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);

                Assert.Equal("~/library", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[1].DiscoverStrategy);
                Assert.Null(locations[1].ModuleType);

                Assert.Equal("~/themes/metronic", locations[2].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[2].DiscoverStrategy);
                Assert.Equal("theme", locations[2].ModuleType);

                Assert.Equal("~/themes/black", locations[3].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[3].DiscoverStrategy);
                Assert.Equal("theme", locations[3].ModuleType);
            }
            finally
            {
                RemoveTextFile(XmlLocatorPath);
            }
        }

        [Fact]
        public void ParseXmlSettingsLocation()
        {
            CreateTextFile(XmlLocatorPath, @"<?xml version=""1.0"" encoding=""utf-8""?>
<locations enableShadow=""true"">
    <module path=""~/modules/email""/>
    <directory path=""~/library""/>
</locations>
");
            try
            {
                var locations = new XmlModuleLocator().GetLocations().ToArray();

                Assert.Equal(2, locations.Length);
                Assert.Equal("~/modules/email", locations[0].Location);
                Assert.Equal(DiscoverStrategy.Single, locations[0].DiscoverStrategy);
                Assert.Null(locations[0].ModuleType);
                Assert.True(locations[0].EnableShadow);

                Assert.Equal("~/library", locations[1].Location);
                Assert.Equal(DiscoverStrategy.Enumerate, locations[1].DiscoverStrategy);
                Assert.Null(locations[1].ModuleType);
                Assert.True(locations[1].EnableShadow);
            }
            finally
            {
                RemoveTextFile(XmlLocatorPath);
            }
        }
    }
}
