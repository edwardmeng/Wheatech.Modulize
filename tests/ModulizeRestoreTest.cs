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
    public class ModulizeRestoreTest : ModulizeTestBase
    {
        private readonly IPersistProvider _persistProvider;
        public ModulizeRestoreTest()
        {
            var manifestText =
@"ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            CreateEmailAssembly(null, null);
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));

            _persistProvider = ((ModuleConfiguration) Modulizer.Configure()).PersistProvider;
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

        [Fact]
        public void RestoreUninstalledModule()
        {
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email");
            Assert.NotNull(module);
            Assert.Equal(ModuleErrors.None, module.Errors);
            Assert.Equal(ModuleInstallState.RequireInstall, module.InstallState);
        }

        [Fact]
        public void RestoreInstalledModule()
        {
            _persistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email");
            Assert.NotNull(module);
            Assert.Equal(ModuleErrors.None, module.Errors);
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
        }

        [Fact]
        public void RestoreDisabledFeature()
        {
            _persistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email");
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);
            Assert.Equal(FeatureEnableState.RequireEnable, module.Features[0].EnableState);
        }

        [Fact]
        public void RestoreEnabledFeature()
        {
            _persistProvider.InstallModule("Wheatech.Email", new Version("1.9.2"));
            _persistProvider.EnableFeature("Wheatech.Email");
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email");
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);
            Assert.Equal(FeatureEnableState.Enabled, module.Features[0].EnableState);
        }

        [Fact]
        public void RestoreAutoInstallModule()
        {
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email.Sender");
            Assert.NotNull(module);
            Assert.Equal(ModuleErrors.None, module.Errors);
            Assert.Equal(ModuleInstallState.Installed, module.InstallState);
        }

        [Fact]
        public void RestoreAutoEnabledFeature()
        {
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            var module = modules.SingleOrDefault(x => x.ModuleId == "Wheatech.Email.Sender");
            Assert.NotNull(module);
            Assert.Equal(1, module.Features.Count);
            Assert.Equal(FeatureEnableState.Enabled, module.Features[0].EnableState);
        }
    }
}
