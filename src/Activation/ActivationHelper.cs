using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal static class ActivationHelper
    {
        public static IEnumerable<Type> FindTypes<T>(Assembly assembly)
        {
            IEnumerable<TypeInfo> types;
            try
            {
                types = assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
            }
            foreach (var type in types)
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        private static MethodInfo ValidateMethod(MethodInfo method)
        {
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                {
                    throw new ActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_InvalidParameter, parameter.Name, method.Name, TypeNameHelper.GetTypeDisplayName(method.DeclaringType)));
                }
            }
            return method;
        }

        public static MethodInfo FindMethod(Type type, string methodName, IActivatingEnvironment environment)
        {
            var genericMethodsWithEnv = new List<MethodInfo>();
            var nomalMethodsWithEnv = new List<MethodInfo>();
            var genericMethodsWithoutEnv = new List<MethodInfo>();
            var nomalMethodsWithoutEnv = new List<MethodInfo>();
            var methodNameWithEnv = methodName + environment;
            var methodNameWithoutEnv = methodName;
            foreach (var method in type.GetMethods())
            {
                if (method.Name == methodNameWithEnv)
                {
                    if (method.IsGenericMethodDefinition)
                    {
                        genericMethodsWithEnv.Add(method);
                    }
                    else
                    {
                        nomalMethodsWithEnv.Add(method);
                    }
                }
                if (method.Name == methodNameWithoutEnv)
                {
                    if (method.IsGenericMethodDefinition)
                    {
                        genericMethodsWithoutEnv.Add(method);
                    }
                    else
                    {
                        nomalMethodsWithoutEnv.Add(method);
                    }
                }
            }
            if (nomalMethodsWithEnv.Count > 1)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotMultipleMethod, methodNameWithEnv, TypeNameHelper.GetTypeDisplayName(type)));
            }
            if (nomalMethodsWithEnv.Count == 1)
            {
                return ValidateMethod(nomalMethodsWithEnv[0]);
            }
            if (nomalMethodsWithoutEnv.Count > 1)
            {
                throw new ActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotMultipleMethod, methodNameWithoutEnv, TypeNameHelper.GetTypeDisplayName(type)));
            }
            if (nomalMethodsWithoutEnv.Count == 1)
            {
                return ValidateMethod(nomalMethodsWithoutEnv[0]);
            }
            if (genericMethodsWithEnv.Count > 0)
            {
                throw new ActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotGenericMethod, methodNameWithEnv, TypeNameHelper.GetTypeDisplayName(type)));
            }
            if (genericMethodsWithoutEnv.Count > 0)
            {
                throw new ActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotGenericMethod, methodNameWithoutEnv, TypeNameHelper.GetTypeDisplayName(type)));
            }
            return null;
        }

        public static void InvokeMethod(MethodInfo method, IActivatingEnvironment environment)
        {
            var arguments = new List<object>();
            foreach (var parameter in method.GetParameters())
            {
                var value = environment.Get(parameter.ParameterType);
                if (value != null)
                {
                    arguments.Add(value);
                }
                else
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotFindParameter, parameter.Name, method.Name, TypeNameHelper.GetTypeDisplayName(method.DeclaringType)));
                }
            }
            method.Invoke(method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType), arguments.ToArray());
        }

        private static bool MatchAssembly(Assembly assembly, AssemblyIdentity identity)
        {
            var assemblyIdentity = new AssemblyIdentity(assembly.FullName);
            if (!identity.Equals(assemblyIdentity, AssemblyIdentityComparison.ShortName)) return false;
            if (identity.Version != null && !Equals(assemblyIdentity.Version, identity.Version)) return false;
            if (identity.Culture == null)
            {
                if (assemblyIdentity.Culture != null && !string.Equals(assemblyIdentity.CultureName, "neutral", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            else
            {
                if (!string.Equals(assemblyIdentity.CultureName, identity.CultureName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            if (identity.PublicKeyToken != null)
            {
                if (assemblyIdentity.PublicKeyToken == null) return false;
                if (!identity.PublicKeyToken.SequenceEqual(assemblyIdentity.PublicKeyToken)) return false;
            }
            if (identity.Architecture == ProcessorArchitecture.None) return true;
            var assemblyName = assembly.GetName();
            return assemblyName.ProcessorArchitecture == ProcessorArchitecture.MSIL || assemblyName.ProcessorArchitecture == identity.Architecture;
        }

        public static Assembly GetDomainAssembly(AssemblyIdentity assemblyIdentity)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => MatchAssembly(assembly, assemblyIdentity));
        }

        public static Assembly LoadAssembly(ModuleDescriptor[] modules, string assemblyName)
        {
            // If the argument name contains directory seperator, indicates it is an physical path or uri path.
            if (assemblyName.Contains('\\') || assemblyName.Contains('/'))
            {
                return Assembly.LoadFrom(PathUtils.ResolvePath(assemblyName));
            }
            AssemblyIdentity identity;
            if (!AssemblyIdentity.TryParse(assemblyName, out identity))
            {
                return Assembly.LoadFrom(assemblyName);
            }
            var assembly = GetDomainAssembly(identity);
            if (assembly != null) return assembly;
            if (modules == null) return null;
            foreach (var module in modules)
            {
                if (module.ModuleManager?.TryLoadAssembly(identity, out assembly) ?? false)
                {
                    return assembly;
                }
            }
            return null;
        }

        public static void InitializeModules(ModuleDescriptor[] modules,IActivatingEnvironment environment)
        {
            // Load the assemblies in module configuration files or bin folder.
            // At this time the reference assemblies will not be requested by the framework.
            foreach (var module in modules)
            {
                module.ModuleManager?.LoadAssemblies();
            }

            foreach (var module in modules)
            {
                module.ModuleManager?.Initialize(environment);
            }
        }
    }
}
