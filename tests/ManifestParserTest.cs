using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ManifestParserTest
    {
        private static ModuleDescriptor ParseText(string manifest)
        {
            return new TextManifestParser().Parse(new MemoryStream(Encoding.UTF8.GetBytes(manifest)), null);
        }

        #region ParseModule

        [Theory]
        [InlineData("ModuleId")]
        [InlineData("module-id")]
        [InlineData("module.id")]
        [InlineData("Module ID")]
        [InlineData("ID")]
        [InlineData("Module")]
        public void ParseModuleId(string propertyName)
        {
            string manifestText = string.Format("{0}: ArchiveLater\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal("ArchiveLater", module.ModuleId);
            Assert.Equal(module.ModuleName, module.ModuleId);
        }

        [Fact]
        public void ParseDefaultModuleId()
        {
            var module = new TextManifestParser().Parse(new MemoryStream(Encoding.UTF8.GetBytes("Version: 1.9.2")), "ArchiveLater");
            Assert.Equal("ArchiveLater", module.ModuleId);
            Assert.Equal(module.ModuleName, module.ModuleId);
        }

        [Theory]
        [InlineData("ModuleName")]
        [InlineData("module-name")]
        [InlineData("module.name")]
        [InlineData("Module Name")]
        [InlineData("Name")]
        public void ParseModuleName(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: Archive Later\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal("Archive Later", module.ModuleName);
        }

        [Theory]
        [InlineData("ModuleDescription")]
        [InlineData("module-description")]
        [InlineData("module.description")]
        [InlineData("Module Description")]
        [InlineData("Description")]
        public void ParseModuleDescription(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: Archive Later\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal("Archive Later", module.Description);
        }

        [Theory]
        [InlineData("ModuleVersion")]
        [InlineData("module-version")]
        [InlineData("module.version")]
        [InlineData("Module Version")]
        [InlineData("Version")]
        public void ParseModuleVersion(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal(Version.Parse("1.9.2"), module.ModuleVersion);
        }

        [Theory]
        [InlineData("PlatformVersion")]
        [InlineData("HostVersion")]
        [InlineData("platform-version")]
        [InlineData("host-version")]
        [InlineData("platform.version")]
        [InlineData("host.version")]
        [InlineData("Platform Version")]
        [InlineData("Host Version")]
        [InlineData("Platform")]
        [InlineData("Host")]
        public void ParseHostVersion(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: 1.9\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal(VersionComparatorFactory.Parse("1.9"), module.HostVersion);
        }

        [Theory]
        [InlineData("moduleauthors")]
        [InlineData("moduleauthor")]
        [InlineData("module-authors")]
        [InlineData("module-author")]
        [InlineData("module.authors")]
        [InlineData("module.author")]
        [InlineData("module authors")]
        [InlineData("module author")]
        [InlineData("authors")]
        [InlineData("author")]
        public void ParseModuleAuthors(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: Edward Meng\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module.Authors);
            Assert.Equal(1, module.Authors.Length);
            Assert.Equal("Edward Meng", module.Authors[0]);

            manifestText = string.Format("ID: ArchiveLater\r\n{0}: Edward Meng, Leon Wu\r\nVersion: 1.9.2", propertyName);
            module = ParseText(manifestText);
            Assert.NotNull(module.Authors);
            Assert.Equal(2, module.Authors.Length);
            Assert.Equal("Edward Meng", module.Authors[0]);
            Assert.Equal("Leon Wu", module.Authors[1]);
        }

        [Theory]
        [InlineData("moduletags")]
        [InlineData("moduletag")]
        [InlineData("module-tags")]
        [InlineData("module-tag")]
        [InlineData("module.tags")]
        [InlineData("module.tag")]
        [InlineData("module tags")]
        [InlineData("module tag")]
        [InlineData("tags")]
        [InlineData("tag")]
        public void ParseModuleTags(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: Archive\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module.Tags);
            Assert.Equal(1, module.Tags.Length);
            Assert.Equal("Archive", module.Tags[0]);

            manifestText = string.Format("ID: ArchiveLater\r\n{0}: Archive, Later\r\nVersion: 1.9.2", propertyName);
            module = ParseText(manifestText);
            Assert.NotNull(module.Tags);
            Assert.Equal(2, module.Tags.Length);
            Assert.Equal("Archive", module.Tags[0]);
            Assert.Equal("Later", module.Tags[1]);
        }

        [Theory]
        [InlineData("modulewebsite")]
        [InlineData("module-website")]
        [InlineData("module.website")]
        [InlineData("module website")]
        [InlineData("website")]
        public void ParseModuleWebSite(string propertyName)
        {
            string manifestText = string.Format("ID: ArchiveLater\r\n{0}: http://www.wheatech.com\r\nVersion: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.Equal("http://www.wheatech.com", module.WebSite);
        }

        #endregion

        #region ParseOuterSingleFeature

        [Fact]
        public void ParseOuterSingleFeatureDefaultValue()
        {
            string manifestText =
@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2";
            var module = ParseText(manifestText);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal("ArchiveLater", feature.FeatureId);
            Assert.Equal("Archive Later", feature.FeatureName);
            Assert.Equal("Content", feature.Category);
            Assert.Equal("Scheduled archiving.", feature.Description);
        }

        [Theory]
        [InlineData("featurename")]
        [InlineData("feature-name")]
        [InlineData("feature.name")]
        [InlineData("feature name")]
        public void ParseOuterSingleFeature_FeatureName(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
{0}: Archive Feature", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Archive Feature", feature.FeatureName);
        }

        [Theory]
        [InlineData("featureid")]
        [InlineData("feature-id")]
        [InlineData("feature.id")]
        [InlineData("feature id")]
        public void ParseOuterSingleFeature_FeatureId(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
{0}: Archive.Feature", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Archive.Feature", feature.FeatureId);
        }

        [Theory]
        [InlineData("modulecategory")]
        [InlineData("module-category")]
        [InlineData("module.category")]
        [InlineData("module category")]
        [InlineData("featurecategory")]
        [InlineData("feature-category")]
        [InlineData("feature.category")]
        [InlineData("feature category")]
        [InlineData("category")]
        public void ParseOuterSingleFeature_Category(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
{0}: Content
Version: 1.9.2", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Content", feature.Category);
        }

        [Theory]
        [InlineData("featuredescription")]
        [InlineData("feature-description")]
        [InlineData("feature.description")]
        [InlineData("feature description")]
        public void ParseOuterSingleFeature_Description(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
{0}: Scheduled archiving.
Version: 1.9.2", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Scheduled archiving.", feature.Description);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseOuterSingleFeature_InlineDependency(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
{0}: Common 1.0.x, Scheduling 2.5.*
Version: 1.9.2", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal(2, feature.Dependencies.Count);
            Assert.Equal("Common", feature.Dependencies[0].FeatureId);
            Assert.Equal("Scheduling", feature.Dependencies[1].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), feature.Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), feature.Dependencies[1].Version);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseOuterSingleFeature_StructureDependency(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Version: 1.9.2
{0}
    Common
        Version: 1.0.x
    Scheduling: 2.5.*
    Email
        ID: Shopmate.Email
        Version: >=1.9.x
", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal(3, feature.Dependencies.Count);
            Assert.Equal("Common", feature.Dependencies[0].FeatureId);
            Assert.Equal("Scheduling", feature.Dependencies[1].FeatureId);
            Assert.Equal("Shopmate.Email", feature.Dependencies[2].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), feature.Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), feature.Dependencies[1].Version);
            Assert.Equal(VersionComparatorFactory.Parse(">=1.9.x"), feature.Dependencies[2].Version);
        }

        #endregion

        #region ParseInnerSingleFeature

        [Fact]
        public void ParseInnerSingleFeatureDefaultValue()
        {
            string manifestText =
@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
Feature
    Dependencies: Common, Scheduling";
            var module = ParseText(manifestText);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal("ArchiveLater", feature.FeatureId);
            Assert.Equal("Archive Later", feature.FeatureName);
            Assert.Equal("Content", feature.Category);
            Assert.Equal("Scheduled archiving.", feature.Description);
        }

        [Theory]
        [InlineData("feature")]
        [InlineData("featureid")]
        [InlineData("feature-id")]
        [InlineData("feature.id")]
        [InlineData("feature id")]
        [InlineData("id")]
        public void ParseInnerSingleFeature_FeatureId(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
Feature
    {0}: Archive.Feature", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Archive.Feature", feature.FeatureId);
        }

        [Theory]
        [InlineData("featurename")]
        [InlineData("feature-name")]
        [InlineData("feature.name")]
        [InlineData("feature name")]
        [InlineData("name")]
        public void ParseInnerSingleFeature_FeatureName(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
Feature
    {0}: Archive Feature", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Archive Feature", feature.FeatureName);
        }

        [Theory]
        [InlineData("featurecategory")]
        [InlineData("feature-category")]
        [InlineData("feature.category")]
        [InlineData("feature category")]
        [InlineData("category")]
        public void ParseInnerSingleFeature_Category(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: Scheduled archiving.
Category: Content
Version: 1.9.2
Feature
    {0}: Behavior", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Behavior", feature.Category);
        }

        [Theory]
        [InlineData("featuredescription")]
        [InlineData("feature-description")]
        [InlineData("feature.description")]
        [InlineData("feature description")]
        [InlineData("description")]
        public void ParseInnerSingleFeature_Description(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Description: The ArchiveLater module introduces scheduled archiving functionality.
Version: 1.9.2
Feature
    {0}: Scheduled archiving.", propertyName);

            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal("Scheduled archiving.", feature.Description);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseInnerSingleFeature_Dependency(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Version: 1.9.2
Feature
    {0}: Common, Scheduling", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal(2, feature.Dependencies.Count);
            Assert.Equal("Common", feature.Dependencies[0].FeatureId);
            Assert.Equal("Scheduling", feature.Dependencies[1].FeatureId);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseInnerSingleFeature_InlineDependency(string propertyName)
        {
            string manifestText =
string.Format(@"
ID: ArchiveLater
Name: Archive Later
Version: 1.9.2
Feature
    Name: Archive Feature
    {0}: Common 1.0.x, Scheduling 2.5.*
", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal(2, feature.Dependencies.Count);
            Assert.Equal("Common", feature.Dependencies[0].FeatureId);
            Assert.Equal("Scheduling", feature.Dependencies[1].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), feature.Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), feature.Dependencies[1].Version);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseInnerSingleFeature_StructureDependency(string propertyName)
        {
            string manifestText =
string.Format(@"ID: ArchiveLater
Name: Archive Later
Version: 1.9.2
Feature
    Name: Archive Feature
    {0}
        Common
            Version: 1.0.x
        Scheduling: 2.5.*
        Email
            ID: Shopmate.Email
            Version: >=1.9.x
", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);

            var feature = module.Features[0];
            Assert.Equal(3, feature.Dependencies.Count);
            Assert.Equal("Common", feature.Dependencies[0].FeatureId);
            Assert.Equal("Scheduling", feature.Dependencies[1].FeatureId);
            Assert.Equal("Shopmate.Email", feature.Dependencies[2].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), feature.Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), feature.Dependencies[1].Version);
            Assert.Equal(VersionComparatorFactory.Parse(">=1.9.x"), feature.Dependencies[2].Version);
        }

        #endregion

        #region ParseMultipleFeatures

        [Fact]
        public void ParseMultipleFeaturesDefaultValue()
        {
            string manifestText =
@"ID: AntiSpam
Version: 1.9.2
Category: Framework
Features
    Akismet.Filter
        Name: Akismet Anti-Spam Filter
    TypePad.Filter
        Name: TypePad Anti-Spam Filter";
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal("Framework", module.Features[0].Category);
            Assert.Equal("Framework", module.Features[1].Category);
        }

        [Theory]
        [InlineData("featureid")]
        [InlineData("feature-id")]
        [InlineData("feature.id")]
        [InlineData("feature id")]
        [InlineData("id")]
        public void ParseMultipleFeatures_FeatureId(string propertyName)
        {
            string manifestText =
string.Format(@"ID: AntiSpam
Version: 1.9.2
Features
    Akismet.Filter
        Name: Akismet Anti-Spam Filter
    Feature
        {0}: TypePad.Filter", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal("Akismet.Filter", module.Features[0].FeatureId);
            Assert.Equal("TypePad.Filter", module.Features[1].FeatureId);
        }

        [Theory]
        [InlineData("featurename")]
        [InlineData("feature-name")]
        [InlineData("feature.name")]
        [InlineData("feature name")]
        [InlineData("name")]
        public void ParseMultipleFeatures_FeatureName(string propertyName)
        {
            string manifestText =
string.Format(@"ID: AntiSpam
Version: 1.9.2
Features
    Akismet.Filter
        {0}: Akismet Anti-Spam Filter
    TypePad.Filter
        {0}: TypePad Anti-Spam Filter", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal("Akismet Anti-Spam Filter", module.Features[0].FeatureName);
            Assert.Equal("TypePad Anti-Spam Filter", module.Features[1].FeatureName);
        }

        [Theory]
        [InlineData("featurecategory")]
        [InlineData("feature-category")]
        [InlineData("feature.category")]
        [InlineData("feature category")]
        [InlineData("category")]
        public void ParseMultipleFeatures_Category(string propertyName)
        {
            string manifestText =
string.Format(@"ID: AntiSpam
Version: 1.9.2
Category: Framework
Features
    Akismet.Filter
        {0}: Behavior
    TypePad.Filter
        {0}: Content", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal("Behavior", module.Features[0].Category);
            Assert.Equal("Content", module.Features[1].Category);
        }

        [Theory]
        [InlineData("featuredescription")]
        [InlineData("feature-description")]
        [InlineData("feature.description")]
        [InlineData("feature description")]
        [InlineData("description")]
        public void ParseMultipleFeatures_Description(string propertyName)
        {
            string manifestText =
string.Format(@"ID: AntiSpam
Version: 1.9.2
Description: Provides anti-spam services to protect your content from malicious submissions.
Features
    Akismet.Filter
        {0}: Provides an anti-spam filter based on Akismet.
    TypePad.Filter
        {0}: Provides an anti-spam filter based on TypePad.", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);
            Assert.Equal("Provides an anti-spam filter based on Akismet.", module.Features[0].Description);
            Assert.Equal("Provides an anti-spam filter based on TypePad.", module.Features[1].Description);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseMultipleFeatures_InlineDependency(string propertyName)
        {
            string manifestText =
string.Format(@"
ID: AntiSpam
Version: 1.9.2
Feature
    Akismet.Filter
        {0}: Tokens 1.0.x, jQuery 2.5.*
    TypePad.Filter
        {0}: Akismet.Filter 2.0.x
", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal(2, module.Features[0].Dependencies.Count);
            Assert.Equal("Tokens", module.Features[0].Dependencies[0].FeatureId);
            Assert.Equal("jQuery", module.Features[0].Dependencies[1].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), module.Features[0].Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), module.Features[0].Dependencies[1].Version);

            Assert.Equal(1, module.Features[1].Dependencies.Count);
            Assert.Equal("Akismet.Filter", module.Features[1].Dependencies[0].FeatureId);
            Assert.Equal(VersionComparatorFactory.Parse("2.0.x"), module.Features[1].Dependencies[0].Version);
        }

        [Theory]
        [InlineData("featuredependencies")]
        [InlineData("feature-dependencies")]
        [InlineData("feature.dependencies")]
        [InlineData("feature dependencies")]
        [InlineData("featuredependency")]
        [InlineData("feature-dependency")]
        [InlineData("feature.dependency")]
        [InlineData("feature dependency")]
        [InlineData("dependencies")]
        [InlineData("dependency")]
        public void ParseMultipleFeatures_StructureDependency(string propertyName)
        {
            string manifestText =
string.Format(@"
ID: AntiSpam
Version: 1.9.2
Feature
    Akismet.Filter
        {0}
            Tokens
                Version: 1.0.x
            jQuery
                Version: 2.5.*
    TypePad.Filter
        {0}
            Akismet.Filter
                Version: 2.0.x
", propertyName);
            var module = ParseText(manifestText);
            Assert.NotNull(module);
            Assert.Equal(2, module.Features.Count);

            Assert.Equal(2, module.Features[0].Dependencies.Count);
            Assert.Equal("Tokens", module.Features[0].Dependencies[0].FeatureId);
            Assert.Equal("jQuery", module.Features[0].Dependencies[1].FeatureId);

            Assert.Equal(VersionComparatorFactory.Parse("1.0.x"), module.Features[0].Dependencies[0].Version);
            Assert.Equal(VersionComparatorFactory.Parse("2.5.*"), module.Features[0].Dependencies[1].Version);

            Assert.Equal(1, module.Features[1].Dependencies.Count);
            Assert.Equal("Akismet.Filter", module.Features[1].Dependencies[0].FeatureId);
            Assert.Equal(VersionComparatorFactory.Parse("2.0.x"), module.Features[1].Dependencies[0].Version);
        }

        #endregion

        private void AssertFullModule(ModuleDescriptor module)
        {
            Assert.NotNull(module);
            Assert.Equal("Wheatech.Email", module.ModuleId);
            Assert.Equal("Email Messaging", module.ModuleName);
            Assert.Equal("Apache", module.License);
            Assert.Equal("http://www.wheatech.net", module.WebSite);
            Assert.Equal(Version.Parse("1.9.2"), module.ModuleVersion);
            Assert.Equal(VersionComparatorFactory.Parse("1.9.x"), module.HostVersion);
            Assert.Equal("The Email Messaging module adds Email sending functionalities.", module.Description);

            Assert.NotNull(module.Authors);
            Assert.Equal(2, module.Authors.Length);
            Assert.True(module.Authors.SequenceEqual(new[] { "Edward Meng", "Leon Wu" }));

            Assert.NotNull(module.Tags);
            Assert.Equal(3, module.Tags.Length);
            Assert.True(module.Tags.SequenceEqual(new[] { "Email", "Messaging", "Workflow" }));

            Assert.Equal(2, module.Features.Count);
            Assert.Equal("Wheatech.Email", module.Features[0].FeatureId);
            Assert.Equal("Email Messaging", module.Features[0].FeatureName);
            Assert.Equal("Email Messaging services.", module.Features[0].Description);
            Assert.Equal("Messaging", module.Features[0].Category);

            Assert.Equal(2, module.Features[0].Dependencies.Count);
            Assert.Equal("Wheatech.Email", module.Features[1].Dependencies[0].FeatureId);
            Assert.Equal(VersionComparatorFactory.Parse("1.x"), module.Features[0].Dependencies[0].Version);
            Assert.Equal("Wheatech.Workflows", module.Features[0].Dependencies[1].FeatureId);
            Assert.Null(module.Features[0].Dependencies[1].Version);

            Assert.Equal("Wheatech.Email.Workflows", module.Features[1].FeatureId);
            Assert.Equal("Email Workflows Activities", module.Features[1].FeatureName);
            Assert.Equal("Provides email sending activities.", module.Features[1].Description);
            Assert.Equal("Workflows", module.Features[1].Category);

            Assert.Equal(2, module.Features[1].Dependencies.Count);
            Assert.Equal("Wheatech.Email", module.Features[1].Dependencies[0].FeatureId);
            Assert.Equal(VersionComparatorFactory.Parse("1.x"), module.Features[1].Dependencies[0].Version);
            Assert.Equal("Wheatech.Workflows", module.Features[1].Dependencies[1].FeatureId);
            Assert.Equal(VersionComparatorFactory.Parse("v1.5.x"), module.Features[1].Dependencies[1].Version);
        }

        [Fact]
        public void ParseJsonManifest()
        {
            var manifestText =
@"{
    ID: ""Wheatech.Email"",
    Name: ""Email Messaging"",
    License: ""Apache"",
    Authors: [""Edward Meng"", ""Leon Wu""],
    Tags: [""Email"", ""Messaging"", ""Workflow""],
    Website: ""http://www.wheatech.net"",
    Version: ""1.9.2"",
    HostVersion: ""1.9.x"",
    Description: ""The Email Messaging module adds Email sending functionalities."",
    Features:{
        ""Wheatech.Email"":{
            Name: ""Email Messaging"",
            Description: ""Email Messaging services."",
            Category: ""Messaging"",
            Dependencies: [""Wheatech.Email 1.x"", ""Wheatech.Workflows""]
        },
        ""Wheatech.Email.Workflows"":{
            Name: ""Email Workflows Activities"",
            Description: ""Provides email sending activities."",
            Category: ""Workflows"",
            Dependencies: {""Wheatech.Email"": ""1.x"", ""Wheatech.Workflows"": {Version: ""v1.5.x""}}
        }
    }
}";
            var module = new JsonManifestParser().Parse(new MemoryStream(Encoding.UTF8.GetBytes(manifestText)), null);
            AssertFullModule(module);
        }

        [Fact]
        public void ParseXmlManifest()
        {
            var manifestText =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<module id=""Wheatech.Email"" name=""Email Messaging"" version=""1.9.2"" host=""1.9.x"" license=""Apache"">
    <authors>Edward Meng, Leon Wu</authors>
    <tags>Email, Messaging, Workflow</tags>
    <website>http://www.wheatech.net</website>
    <description>The Email Messaging module adds Email sending functionalities.</description>
    <features>
        <Wheatech.Email name=""Email Messaging"" category=""Messaging"">
            <description>Email Messaging services.</description>
            <dependencies>Wheatech.Email 1.x, Wheatech.Workflows</dependencies>
        </Wheatech.Email>
        <Wheatech.Email.Workflows name=""Email Workflows Activities"" category=""Workflows"">
            <description>Provides email sending activities.</description>
            <dependencies>
                <Wheatech.Email>1.x</Wheatech.Email>
                <Wheatech.Workflows version=""v1.5.x""/>
            </dependencies>
        </Wheatech.Email.Workflows>
    </features>
</module>
";
            var module = new XmlManifestParser().Parse(new MemoryStream(Encoding.UTF8.GetBytes(manifestText)), null);
            AssertFullModule(module);
        }
    }
}
