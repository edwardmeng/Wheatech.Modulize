using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Wheatech.Activation;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ModulizeManageTest : ModulizeTestBase
    {
        public static string GlobalState;
        public readonly IPersistProvider PersistProvider = new MockPersistProvider();

        public ModulizeManageTest()
        {
            Modulizer.Configure().PersistWith(PersistProvider);
        }

        private void CreateEmailAssembly(IDictionary<string, string> moduleMethods, IDictionary<string, string> featureMethods)
        {
            var emailTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.dll").Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Activation.dll", "Wheatech.Modulize.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailTempFileName
            },
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public class EmailModuleInstaller\r\n" +
                "{\r\n" +
                CreateModuleInstaller(moduleMethods) +
                "}",
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[FeatureActivator(\"Wheatech.Email\")]\r\n" +
                "public class EmailFeatureActivator\r\n" +
                "{\r\n" +
                CreateFeatureActivator(featureMethods) +
                "}"
                );
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            var emailFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.dll").Replace("/", "\\");
            if (File.Exists(emailFileName))
            {
                File.Delete(emailFileName);
            }
            File.Copy(emailTempFileName, emailFileName);
            File.Delete(emailTempFileName);
        }

        private void CreateSenderAssembly(IDictionary<string, string> moduleMethods, IDictionary<string, string> featureMethods)
        {
            var emailTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.Sender.dll").Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Activation.dll", "Wheatech.Modulize.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailTempFileName
            },
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public class EmailSenderModuleActivator\r\n" +
                "{\r\n" +
                CreateModuleInstaller(moduleMethods) +
                "}",
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "[FeatureActivator(\"Wheatech.Email.Sender\")]" +
                "public class EmailSenderFeatureActivator\r\n" +
                "{\r\n" +
                CreateFeatureActivator(featureMethods) +
                "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            var emailFileName = PathUtils.ResolvePath(FolderPath + "Email.Sender/Wheatech.Email.Sender.dll").Replace("/", "\\");
            if (File.Exists(emailFileName))
            {
                File.Delete(emailFileName);
            }
            File.Copy(emailTempFileName, emailFileName);
            File.Delete(emailTempFileName);
        }

        [Fact]
        public void InstallModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            var module = modules[0];
            Assert.Equal(ModuleErrors.None, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureErrors.UninstallModule, feature.Errors);
            Modulizer.InstallModules("Wheatech.Email");
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
            Assert.Equal(FeatureEnableState.Enabled, feature.EnableState);
            Version installVersion;
            Assert.True(PersistProvider.GetModuleInstalled(module.ModuleId, out installVersion));
            Assert.Equal(new Version("1.9.2"), installVersion);
            Assert.True(PersistProvider.GetFeatureEnabled(feature.FeatureId));
        }

        [Fact]
        public void InstallForbiddenModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            var module = modules[0];
            Assert.Equal(ModuleErrors.ForbiddenFeatures, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureErrors.MissingDependency | FeatureErrors.UninstallModule, feature.Errors);
            Assert.Throws<ModuleActivationException>(() => Modulizer.InstallModules("Wheatech.Email"));
        }

        [Fact]
        public void InstallModule_UninstalledDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
Assembly: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");
            CreateSenderAssembly(null, null);

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);
            var module = modules.First(x => x.ModuleId == "Wheatech.Email");
            Assert.Equal(ModuleErrors.ForbiddenFeatures, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureErrors.UninstallModule | FeatureErrors.ForbiddenDependency, feature.Errors);
            Assert.Equal(FeatureEnableState.RequireEnable, feature.EnableState);
            Assert.Throws<ModuleActivationException>(() => Modulizer.InstallModules("Wheatech.Email"));
        }

        [Fact]
        public void InstallNotExistModule()
        {
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(0, modules.Length);
            Assert.Throws<ArgumentException>(() => Modulizer.InstallModules("Wheatech.Email"));
        }

        [Fact]
        public void UninstallNotExistModule()
        {
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(0, modules.Length);
            Assert.Throws<ArgumentException>(() => Modulizer.UninstallModules("Wheatech.Email"));
        }

        [Fact]
        public void UninstallModule_InstalledDependency()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
Assembly: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");
            CreateSenderAssembly(null, null);
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            Modulizer.InstallModules("Wheatech.Email", "Wheatech.Email.Sender");
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);
            var module = modules.First(x => x.ModuleId == "Wheatech.Email");
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureEnableState.Enabled, feature.EnableState);
            Assert.Throws<ModuleDependencyException>(() => Modulizer.UninstallModules("Wheatech.Email.Sender"));
        }

        [Fact]
        public void UninstallModule()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
Assembly: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");
            CreateSenderAssembly(null, null);
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            Modulizer.InstallModules("Wheatech.Email", "Wheatech.Email.Sender");
            Modulizer.UninstallModules("Wheatech.Email", "Wheatech.Email.Sender");
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);
            var module = modules.First(x => x.ModuleId == "Wheatech.Email.Sender");
            Assert.Equal(ModuleErrors.None, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureErrors.UninstallModule, feature.Errors);
            Assert.Equal(FeatureEnableState.RequireEnable, feature.EnableState);
        }

        [Fact]
        public void EnableNotExistFeature()
        {
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(0, modules.Length);
            Assert.Throws<ArgumentException>(() => Modulizer.EnableFeatures("Wheatech.Email"));
        }

        [Fact]
        public void EnableForbiddenFeature()
        {
            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            PersistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            PersistProvider.EnableFeature("Wheatech.Email");
            var modules = Modulizer.GetModules();
            var module = modules.First(x => x.ModuleId == "Wheatech.Email");
            Assert.Equal(ModuleErrors.ForbiddenFeatures, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureErrors.UninstallModule | FeatureErrors.MissingDependency, feature.Errors);
            Assert.Equal(FeatureEnableState.RequireEnable, feature.EnableState);

            Assert.Throws<ModuleActivationException>(() => Modulizer.EnableFeatures("Wheatech.Email"));
        }

        [Fact]
        public void EnableFeature_DisabledDependency()
        {
            PersistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            PersistProvider.InstallModule("Wheatech.Email.Sender", new Version("1.9.2"));

            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
Assembly: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");
            CreateSenderAssembly(null, null);

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);
            var module = modules.First(x => x.ModuleId == "Wheatech.Email");
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureEnableState.Enabled, feature.EnableState);

            Assert.Throws<ModuleDependencyException>(() => Modulizer.EnableFeatures("Wheatech.Email"));
        }

        [Fact]
        public void DisableFeature_EnabledDependency()
        {
            PersistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            PersistProvider.EnableFeature("Wheatech.Email");
            PersistProvider.InstallModule("Wheatech.Email.Sender", new Version("1.9.2"));
            PersistProvider.EnableFeature("Wheatech.Email.Sender");

            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
Assembly: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");
            CreateSenderAssembly(null, null);

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);

            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);
            var module = modules.First(x => x.ModuleId == "Wheatech.Email.Sender");
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
            Assert.Equal(1, module.Features.Count);
            var feature = module.Features[0];
            Assert.Equal(FeatureEnableState.Enabled, feature.EnableState);

            Assert.Throws<ModuleDependencyException>(() => Modulizer.DisableFeatures("Wheatech.Email.Sender"));
        }
    }
}
