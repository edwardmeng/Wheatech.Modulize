using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Wheatech.Activation;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class ModulizeTestBase
    {
        protected const string FolderPath = "~/modules/";

        static ModulizeTestBase()
        {
            ApplicationActivator.Startup();
        }

        protected ModulizeTestBase()
        {
            if (Directory.Exists(PathUtils.ResolvePath(FolderPath)))
            {
                Directory.Delete(PathUtils.ResolvePath(FolderPath), true);
            }
            if (!Directory.Exists(PathUtils.ResolvePath(FolderPath)))
            {
                Directory.CreateDirectory(PathUtils.ResolvePath(FolderPath));
            }
            Modulizer.Reset();
            Modulizer.Configure().UseLocator(new StaticModuleLocator("Library", FolderPath, DiscoverStrategy.Enumerate, false)).PersistWith<MockPersistProvider>();
        }

        protected static void CreateAssembly(string fileName, string productName, Version version, params string[] sources)
        {
            var tempFileName = PathUtils.ResolvePath("~/" + Path.GetFileName(fileName)).Replace("/", "\\");
            var result = new CSharpCodeProvider().CompileAssemblyFromSource(new CompilerParameters(new[] { "Wheatech.Activation.dll", "Wheatech.Modulize.dll" })
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = tempFileName
            }, sources.Concat(new[]
            {
                string.Format(
                    "using System.Reflection;\r\n" +
                    "[assembly: AssemblyVersion(\"{0}\")]\r\n" +
                    "[assembly: AssemblyFileVersion(\"{0}\")]\r\n" +
                    "[assembly: AssemblyProduct(\"{1}\")]", version, productName)
            }).ToArray());
            if (result.Errors.HasErrors)
            {
                foreach (CompilerError err in result.Errors)
                {
                    Console.Error.WriteLine(err.ErrorText);
                }
            }
            else
            {
                fileName = PathUtils.ResolvePath(fileName).Replace("/", "\\");
                if (!string.Equals(fileName, tempFileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    File.Copy(tempFileName, fileName);
                    File.Delete(tempFileName);
                }
            }
        }

        protected static void CreateDirectoryModule(string manifestText, string manifestName, string directoryName)
        {
            var directoryPath = PathUtils.ResolvePath(FolderPath + directoryName);
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
            Directory.CreateDirectory(directoryPath);
            using (var stream = File.OpenWrite(PathUtils.ResolvePath(FolderPath + directoryName + "/" + manifestName)))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(manifestText);
                }
            }
        }

        protected static string CreateModuleInstaller(IDictionary<string, string> methods)
        {
            var sb = new StringBuilder();

            Action<string> renderMethodBody = methodName =>
            {
                string methodBody;
                if (methods != null && methods.TryGetValue(methodName, out methodBody))
                {
                    sb.AppendLine(methodBody);
                }
            };

            sb.AppendLine("    public void Install(IActivatingEnvironment environment){");
            renderMethodBody("Install");
            sb.AppendLine("    }");

            sb.AppendLine("    public void Uninstall(IActivatingEnvironment environment){");
            renderMethodBody("Uninstall");
            sb.AppendLine("    }");

            return sb.ToString();
        }

        protected static string CreateFeatureActivator(IDictionary<string, string> methods)
        {
            var sb = new StringBuilder();

            Action<string> renderMethodBody = methodName =>
            {
                string methodBody;
                if (methods != null && methods.TryGetValue(methodName, out methodBody))
                {
                    sb.AppendLine(methodBody);
                }
            };
            sb.AppendLine("public void Enable(IActivatingEnvironment environment)");
            sb.AppendLine("{");
            renderMethodBody("Enable");
            sb.AppendLine("}");

            sb.AppendLine("public void Disable(IActivatingEnvironment environment)");
            sb.AppendLine("{");
            renderMethodBody("Disable");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
