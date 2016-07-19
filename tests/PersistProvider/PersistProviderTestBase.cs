using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using Wheatech.Activation;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class PersistProviderTestBase : ModulizeTestBase
    {
        private readonly IPersistProvider _persistProvider;

        protected PersistProviderTestBase()
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
            Modulizer.Configure().PersistWith(_persistProvider = CreatePersistProvider());
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

        protected abstract IPersistProvider CreatePersistProvider();

        [Fact]
        public void ModuleNotInstalled()
        {
            string moduleId = Guid.NewGuid().ToString();
            Version installVersion;
            Assert.False(_persistProvider.GetModuleInstalled(moduleId, out installVersion));
            Assert.Null(installVersion);
        }

        [Fact]
        public void ModuleInstalled()
        {
            string moduleId = Guid.NewGuid().ToString();
            _persistProvider.InstallModule(moduleId,new Version("1.0.5"));
            Version installVersion;
            Assert.True(_persistProvider.GetModuleInstalled(moduleId, out installVersion));
            Assert.Equal(new Version("1.0.5"), installVersion);
        }

        [Fact]
        public void FeatureNotEnabled()
        {
            var featureId = Guid.NewGuid().ToString();
            Assert.False(_persistProvider.GetFeatureEnabled(featureId));
        }

        [Fact]
        public void FeatureEnabled()
        {
            var featureId = Guid.NewGuid().ToString();
            _persistProvider.EnableFeature(featureId);
            Assert.True(_persistProvider.GetFeatureEnabled(featureId));
        }

        [Fact]
        public void FeatureDisabled()
        {
            var featureId = Guid.NewGuid().ToString();
            _persistProvider.EnableFeature(featureId);
            Assert.True(_persistProvider.GetFeatureEnabled(featureId));
            _persistProvider.DisableFeature(featureId);
            Assert.False(_persistProvider.GetFeatureEnabled(featureId));
        }
    }
}
