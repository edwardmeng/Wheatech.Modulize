using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using Wheatech.Activation;
using Xunit;

namespace Wheatech.Modulize.UnitTests
{
    public class ModulizeReflectionTest : ModulizeTestBase
    {
        public static string GlobalState;

        [Fact]
        public void LoadInternalDependencyAssembly()
        {
            #region Prepare Module

            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            var emailSenderTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.Sender.dll").Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Modulize.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailSenderTempFileName
            },
            "namespace Wheatech.Email.Sender{\r\n" +
                "public interface IEmailSender{\r\n" +
                    "void Send();\r\n" +
                "}\r\n" +
            "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            var emailTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.dll").Replace("/", "\\");
            result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Activation.dll", "Wheatech.Modulize.dll", "Wheatech.Email.Sender.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailTempFileName
            },
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "using Wheatech.Email.Sender;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public class EmailModuleInstaller:IEmailSender\r\n" +
                "{\r\n" +
                CreateModuleInstaller(null) +
                "public void Send(){\r\n" +
                "}\r\n" +
                "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            var emailSenderFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.Sender.dll").Replace("/", "\\");
            if (File.Exists(emailSenderFileName))
            {
                File.Delete(emailSenderFileName);
            }
            File.Copy(emailSenderTempFileName, emailSenderFileName);
            File.Delete(emailSenderTempFileName);

            var emailFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.dll").Replace("/", "\\");
            if (File.Exists(emailFileName))
            {
                File.Delete(emailFileName);
            }
            File.Copy(emailTempFileName, emailFileName);
            File.Delete(emailTempFileName);

            #endregion

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(1, modules.Length);
            Assert.Equal(ModuleErrors.None, modules[0].Errors);

            Assert.Equal(1, modules[0].Features.Count);
            Assert.Equal(FeatureErrors.None, modules[0].Features[0].Errors);

            Assert.Equal(FeatureEnableState.Enabled, modules[0].Features[0].EnableState);
        }

        [Fact]
        public void LoadExternalModuleAssembly()
        {
            #region Prepare Modules

            var emailSenderTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.Sender.dll").Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Modulize.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailSenderTempFileName
            },
            "namespace Wheatech.Email.Sender{\r\n" +
                "public interface IEmailSender{\r\n" +
                    "void Send();\r\n" +
                "}\r\n" +
            "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            var emailTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.dll").Replace("/", "\\");
            result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Activation.dll", "Wheatech.Modulize.dll", "Wheatech.Email.Sender.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailTempFileName
            },
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "using Wheatech.Email.Sender;\r\n" +
                "[ModuleInstaller(\"1.9.2\")]\r\n" +
                "public class EmailModuleInstaller:IEmailSender\r\n" +
                "{\r\n" +
                CreateModuleInstaller(null) +
                "public void Send(){\r\n" +
                "}\r\n" +
                "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }


            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");

            var emailSenderFileName = PathUtils.ResolvePath(FolderPath + "Email.Sender/Wheatech.Email.Sender.dll").Replace("/", "\\");
            if (File.Exists(emailSenderFileName))
            {
                File.Delete(emailSenderFileName);
            }
            File.Copy(emailSenderTempFileName, emailSenderFileName);
            File.Delete(emailSenderTempFileName);

            var emailFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.dll").Replace("/", "\\");
            if (File.Exists(emailFileName))
            {
                File.Delete(emailFileName);
            }
            File.Copy(emailTempFileName, emailFileName);
            File.Delete(emailTempFileName);

            #endregion

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);

            for (int i = 0; i < modules.Length; i++)
            {
                Assert.Equal(ModuleErrors.None, modules[i].Errors);

                Assert.Equal(1, modules[i].Features.Count);
                Assert.Equal(FeatureErrors.None, modules[i].Features[0].Errors);

                Assert.Equal(FeatureEnableState.Enabled, modules[i].Features[0].EnableState);
            }
        }

        [Fact]
        public void LoadAssemblyPriority()
        {
            #region Prepare Modules

            var manifestText =
@"
ID: Wheatech.Email
Name: Email Messaging
Version: 1.9.2
Assembly: Wheatech.Email
Dependency: Wheatech.Email.Sender
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email");
            manifestText =
@"
ID: Wheatech.Email.Sender
Name: Email Messaging Sender
Version: 1.9.2
";
            CreateDirectoryModule(manifestText, "manifest.txt", "Email.Sender");

            var emailSenderTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.Sender.dll").Replace("/", "\\");
            var emailSenderFileName = PathUtils.ResolvePath(FolderPath + "Email.Sender/Wheatech.Email.Sender.dll").Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.dll", "Wheatech.Activation.dll", "Wheatech.Modulize.dll", "Wheatech.Modulize.UnitTests.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailSenderTempFileName
            },
            "namespace Wheatech.Email.Sender{\r\n" +
                "using Wheatech.Modulize.UnitTests;\r\n" +
                "public class EmailSender{\r\n" +
                    "public void Send(){\r\n" +
                        "ModulizeReflectionTest.GlobalState =\"Wheatech.Email.Sender\"; " +
                    "}\r\n" +
                "}\r\n" +
            "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }
            if (File.Exists(emailSenderFileName))
            {
                File.Delete(emailSenderFileName);
            }
            File.Copy(emailSenderTempFileName, emailSenderFileName);
            File.Delete(emailSenderTempFileName);

            emailSenderFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.Sender.dll").Replace("/", "\\");
            result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.dll", "Wheatech.Activation.dll", "Wheatech.Modulize.dll", "Wheatech.Modulize.UnitTests.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailSenderTempFileName
            },
            "namespace Wheatech.Email.Sender{\r\n" +
                "using Wheatech.Modulize.UnitTests;\r\n" +
                "public class EmailSender{\r\n" +
                    "public void Send(){\r\n" +
                        "ModulizeReflectionTest.GlobalState =\"Wheatech.Email\"; " +
                    "}\r\n" +
                "}\r\n" +
            "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }

            var emailTempFileName = PathUtils.ResolvePath("~/Wheatech.Email.dll").Replace("/", "\\");
            result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.dll", "Wheatech.Activation.dll", "Wheatech.Modulize.dll", "Wheatech.Email.Sender.dll", "Wheatech.Modulize.UnitTests.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = emailTempFileName
            },
                "using Wheatech.Modulize;\r\n" +
                "using Wheatech.Activation;\r\n" +
                "using Wheatech.Email.Sender;\r\n" +
                "[assembly: AssemblyActivator(typeof(EmailModuleActivator))]\r\n" +
                "public class EmailModuleActivator\r\n" +
                "{\r\n" +
                "    public void Configuration(IActivatingEnvironment environment)\r\n" +
                "    {\r\n"+
                "        new EmailSender().Send();\r\n " +
                "    }\r\n" +
                "}");
            if (result.Errors.HasErrors)
            {
                throw new InvalidOperationException(result.Errors[0].ErrorText);
            }

            if (File.Exists(emailSenderFileName))
            {
                File.Delete(emailSenderFileName);
            }
            File.Copy(emailSenderTempFileName, emailSenderFileName);
            File.Delete(emailSenderTempFileName);

            var emailFileName = PathUtils.ResolvePath(FolderPath + "Email/Wheatech.Email.dll").Replace("/", "\\");
            if (File.Exists(emailFileName))
            {
                File.Delete(emailFileName);
            }
            File.Copy(emailTempFileName, emailFileName);
            File.Delete(emailTempFileName);

            #endregion

            UnitTestStartup.Environment.UseApplicationVersion(System.Version.Parse("1.9.2"));
            Modulizer.Start(UnitTestStartup.Environment);
            Modulizer.InstallModules("Wheatech.Email", "Wheatech.Email.Sender");
            var modules = Modulizer.GetModules();
            Assert.Equal(2, modules.Length);

            for (int i = 0; i < modules.Length; i++)
            {
                Assert.Equal(ModuleErrors.None, modules[i].Errors);

                Assert.Equal(1, modules[i].Features.Count);
                Assert.Equal(FeatureErrors.None, modules[i].Features[0].Errors);

                Assert.Equal(FeatureEnableState.Enabled, modules[i].Features[0].EnableState);
            }

            Assert.Equal("Wheatech.Email.Sender", GlobalState);
        }
    }
}
