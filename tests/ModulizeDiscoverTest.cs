using System.Linq;
using Wheatech.Activation;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ModulizeDiscoverTest: ModulizeTestBase
    {
        [Fact]
        public void DiscoverNotExistDirectory()
        {
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(0, modules.Length);
        }

        [Fact]
        public void DiscoverSingleFileModule()
        {
            CreateAssembly(FolderPath + "Wheatech.Workflow.dll", "Wheatech Workflow", Version.Parse("1.5.3"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal("Wheatech.Workflow", modules[0].ModuleId);
            Assert.Equal("Wheatech Workflow", modules[0].ModuleName);
            Assert.Equal("Library", modules[0].ModuleType);
            Assert.Equal(Version.Parse("1.5.3"), modules[0].ModuleVersion);
        }

        [Fact]
        public void DiscoverJsonModule()
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
            Dependencies: [""Wheatech.Workflows""]
        },
        ""Wheatech.Email.Workflows"":{
            Name: ""Email Workflows Activities"",
            Description: ""Provides email sending activities."",
            Category: ""Workflows"",
            Dependencies: {""Wheatech.Email"": ""1.x"", ""Wheatech.Workflows"": {Version: ""v1.5.x""}}
        }
    }
}";
            CreateDirectoryModule(manifestText, "manifest.json", "Email");
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal("Library", modules[0].ModuleType);
            AssertFullModule(modules[0]);
        }

        [Fact]
        public void DiscoverXmlModule()
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
            <dependencies>Wheatech.Workflows</dependencies>
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
            CreateDirectoryModule(manifestText, "manifest.config", "Email");
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal("Library", modules[0].ModuleType);
            AssertFullModule(modules[0]);
        }

        [Fact]
        public void DiscoverTextModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Host Version: 1.9.x
License: Apache
Authors: Edward Meng, Leon Wu
Tags: Email, Messaging, Workflow
Website: http://www.wheatech.net
Description: The Email Messaging module adds Email sending functionalities.
Features
    Wheatech.Email
        Name: Email Messaging
        Category: Messaging
        Description: Email Messaging services.
        Dependencies: Wheatech.Workflows
    Wheatech.Email.Workflows
        Name: Email Workflows Activities
        Category: Workflows
        Description: Provides email sending activities.
        Dependencies: Wheatech.Email 1.x, Wheatech.Workflows v1.5.x
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal("Library", modules[0].ModuleType);
            AssertFullModule(modules[0]);
        }

        [Fact]
        public void DiscoverIncompitibleModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Host Version: 1.9.x
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("2.0"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.IncompatibleHost, modules[0].Errors);

            Assert.Equal(1, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.ForbiddenModule, modules[0].Features[0].Errors);
        }

        [Fact]
        public void DiscoverFeatureMissingDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
    Wheatech.Email.Workflows
        Dependencies: Wheatech.Email, Wheatech.Workflows
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);
            Assert.Equal(2, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.None, modules[0].Features[0].Errors);
            Assert.Equal(FeatureErrors.MissingDependency, modules[0].Features[1].Errors);
        }

        [Fact]
        public void DiscoverFeatureIncompatibleDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
        Version: 1.9.5
    Wheatech.Email.Workflows
        Dependencies: Wheatech.Email 1.2.x
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);
            Assert.Equal(2, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.None, modules[0].Features[0].Errors);
            Assert.Equal(FeatureErrors.IncompatibleDependency, modules[0].Features[1].Errors);
        }

        [Fact]
        public void DiscoverFeature_IncompatibleDependency_MissingDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
        Version: 1.9.5
    Wheatech.Email.Workflows
        Dependencies: Wheatech.Email 1.2.x, Wheatech.Workflows
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);
            Assert.Equal(2, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.None, modules[0].Features[0].Errors);
            Assert.Equal(FeatureErrors.MissingDependency | FeatureErrors.IncompatibleDependency, modules[0].Features[1].Errors);
        }

        [Fact]
        public void DiscoverRequiresInstallModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.dll", "Wheatech Email", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n"+
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public class EmailModuleInstaller\r\n" +
                "{\r\n" +
                CreateModuleInstaller(null) +
                "}");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleInstallState.RequireInstall, modules[0].InstallState);

            Assert.Equal(1, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.UninstallModule, modules[0].Features[0].Errors);
        }

        [Fact]
        public void DiscoverReflectModuleAssemblyFailed()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Assert.Throws<ModuleConfigurationException>(() => Modulizer.Start(UnitTestStartup.Environment));
        }

        [Theory]
        [InlineData("abstract class EmailModuleManager", null)]
        [InlineData("class EmailModuleManager<T>", null)]
        public void DiscoverIncorrectModuleManager(string className, string constructor)
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.dll", "Wheatech Email", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public " + className + "\r\n" +
                "{\r\n" +
                (string.IsNullOrEmpty(constructor) ? null : (
                "    public EmailModuleManager()\r\n" +
                "    {\r\n" +
                constructor +
                "    }\r\n")) +
                CreateModuleInstaller(null) +
                "}");
            Assert.Throws<ModuleActivationException>(() => Modulizer.Start(UnitTestStartup.Environment));
        }

        [Fact]
        public void DiscoverRequiresEnableFeature()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.dll", "Wheatech Email", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[FeatureActivator]\r\n" +
                "public class EmailFeatureActivator\r\n" +
                "{\r\n" +
                CreateFeatureActivator(null) +
                "}");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);

            Assert.Equal(1, modules[0].Features.Count);
            Assert.Equal(FeatureEnableState.RequireEnable, modules[0].Features[0].EnableState);
        }

        [Fact]
        public void DiscoverReflectFeatureAssemblyFailed()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Feature
    Wheatech.Email
        Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Assert.Throws<ModuleConfigurationException>(() => Modulizer.Start(UnitTestStartup.Environment));
        }

        [Theory]
        [InlineData("abstract class EmailFeatureActivator", null)]
        [InlineData("class EmailFeatureActivator<T>", null)]
        public void DiscoverIncorrectFeatureManager(string className, string constructor)
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Feature
    Wheatech.Email
        Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.dll", "Wheatech Email", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[FeatureActivator]\r\n" +
               "public " + className + "\r\n" +
               "{\r\n" +
                (string.IsNullOrEmpty(constructor) ? null : (
                "    public EmailFeatureActivator()\r\n" +
                "    {\r\n" +
                constructor +
                "    }\r\n")) +
               CreateFeatureActivator(null) +
               "}");
            Assert.Throws<ModuleActivationException>(() => Modulizer.Start(UnitTestStartup.Environment));
        }

        [Fact]
        public void DiscoverForbiddenFeatures_IncompatibleDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
        Version: 1.9.5
        Dependencies: Wheatech.Workflow 1.0.x
    Wheatech.Email.Workflows
        Dependencies: Wheatech.Email 1.2.x
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            manifestText = @"
ID: Wheatech.Workflow
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Workflow");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);

            var module = modules.FirstOrDefault(x => x.ModuleId == "Wheatech.Email");
            Assert.NotNull(module);
            Assert.Equal(ModuleErrors.ForbiddenFeatures, module.Errors);
            Assert.Equal(FeatureErrors.IncompatibleDependency, module.Features[0].Errors);
            Assert.Equal(FeatureErrors.IncompatibleDependency | FeatureErrors.ForbiddenDependency, module.Features[1].Errors);
        }

        [Fact]
        public void DiscoverForbiddenFeatures_MissingDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
        Dependencies: Wheatech.Core
    Wheatech.Email.Workflows
        Dependencies: Wheatech.Workflow
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.ForbiddenFeatures, modules[0].Errors);
            Assert.Equal(FeatureErrors.MissingDependency, modules[0].Features[0].Errors);
            Assert.Equal(FeatureErrors.MissingDependency, modules[0].Features[1].Errors);
        }

        [Fact]
        public void DiscoverForbiddenFeatures_ReflectionFailed()
        {
            var manifestText =
@"
ID: Wheatech.Email
Version: 1.9.2
Features
    Wheatech.Email
        Assembly: Wheatech.Core
    Wheatech.Email.Workflows
        Assembly: Wheatech.Email.Workflows
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.dll", "Wheatech Email", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
               "public abstract EmailFeatureManager\r\n" +
               "{\r\n" +
               CreateFeatureActivator(null) +
               "}");
            CreateAssembly(FolderPath + "Email/Wheatech.Email.Workflows.dll", "Wheatech Email Workflows", Version.Parse("1.9.2"),
                "using Wheatech.Modulize;\r\n" +
               "public class EmailWorkflowFeatureManager<T>\r\n" +
               "{\r\n" +
               CreateFeatureActivator(null) +
               "}");
            Assert.Throws<ModuleConfigurationException>(() => Modulizer.Start(UnitTestStartup.Environment));
        }

        [Fact]
        public void DiscoverAutoInstallModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);
            Assert.Equal(ModuleInstallState.Installed, modules[0].InstallState);
        }

        [Fact]
        public void DiscoverAutoEnableFeature()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);
            Assert.Equal(1, modules[0].Features.Count);
            Assert.Equal(FeatureEnableState.Enabled, modules[0].Features[0].EnableState);
        }

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

            Assert.Equal(1, module.Features[0].Dependencies.Count);
            Assert.Equal("Wheatech.Workflows", module.Features[0].Dependencies[0].FeatureId);
            Assert.Null(module.Features[0].Dependencies[0].Version);

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
    }
}
