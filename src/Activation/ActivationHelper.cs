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

        public static bool MatchAssembly(Assembly assembly, AssemblyIdentity identity)
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
                if (module.TryLoadAssembly(identity, out assembly))
                {
                    return assembly;
                }
            }
            return null;
        }

        public static void InitializeModules(ModuleDescriptor[] modules, IActivatingEnvironment environment)
        {
            var duplicateModules = (from module in modules
                                    group module by module.ModuleId into g
                                    where g.Count() > 1
                                    select g.Key).ToArray();
            if (duplicateModules.Length > 0)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateModules, string.Join(", ", duplicateModules)));
            }

            var duplicateFeatures = (from module in modules
                                     from feature in module.Features
                                     group feature by feature.FeatureId into g
                                     where g.Count() > 1
                                     select g.Key).ToArray();
            if (duplicateFeatures.Length > 0)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_DuplicateFeatures, string.Join(", ", duplicateFeatures)));
            }

            // Load the assemblies in module configuration files or bin folder.
            // At this time the reference assemblies will not be requested by the framework.
            foreach (var module in modules)
            {
                if (module.HostVersion != null && !module.HostVersion.Match(environment.ApplicationVersion))
                {
                    module.RuntimeState |= ModuleRuntimeState.IncompatibleHost | ModuleRuntimeState.Forbidden;
                }
                else
                {
                    module.LoadAssemblies();
                }
            }
            var features = modules.SelectMany(module => module.Features).Sort();
            foreach (var module in features.Select(feature => feature.Module).Distinct())
            {
                if (!module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    module.Initialize(environment);
                }
            }
            var featuresDictionary = features.ToDictionary(feature => feature.FeatureId);
            foreach (var feature in features)
            {
                if (!feature.Module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    // Validate dependencies by feature ID and version.
                    foreach (var dependency in feature.Dependencies)
                    {
                        FeatureDescriptor dependentFeature;
                        if (!featuresDictionary.TryGetValue(dependency.FeatureId, out dependentFeature))
                        {
                            feature.RuntimeState |= FeatureRuntimeState.MissingDependency | FeatureRuntimeState.Forbidden;
                        }
                        else if (dependency.Version != null && !dependency.Version.Match(dependentFeature.Module.ModuleVersion))
                        {
                            feature.RuntimeState |= FeatureRuntimeState.IncompatibleDependency | FeatureRuntimeState.Forbidden;
                        }
                        else if (dependentFeature.RuntimeState.HasFlag(FeatureRuntimeState.Forbidden))
                        {
                            feature.RuntimeState |= FeatureRuntimeState.ForbiddenDependency | FeatureRuntimeState.Forbidden;
                        }
                    }
                    if (!feature.RuntimeState.HasFlag(FeatureRuntimeState.Forbidden))
                    {
                        feature.Initialize(environment);
                    }
                }
            }
        }

        public static void RecoverModules(ModuleDescriptor[] modules, IActivationStore store, IActivatingEnvironment environment)
        {
            var installedModules = new List<Tuple<ModuleDescriptor, Version>>();
            var enabledFeatures = new List<FeatureDescriptor>();
            foreach (var module in modules)
            {
                if (!module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    Version installVersion;
                    if (store.GetModuleInstalled(module.ModuleId, out installVersion))
                    {
                        installedModules.Add(Tuple.Create(module, installVersion));
                    }
                    foreach (var feature in module.Features)
                    {
                        if (!feature.RuntimeState.HasFlag(FeatureRuntimeState.Forbidden))
                        {
                            if (store.GetFeatureEnabled(feature.FeatureId))
                            {
                                enabledFeatures.Add(feature);
                            }
                        }
                    }
                }
            }
            var features = modules.SelectMany(module => module.Features).Sort();
            var featuresDictionary = features.ToDictionary(feature => feature.FeatureId);
            foreach (var module in features.Select(feature => feature.Module).Distinct())
            {
                if (!module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden) && module.ActivationState == ModuleActivationState.RequireInstall)
                {
                    var installedVersion = installedModules.FirstOrDefault(x => x.Item1 == module)?.Item2;
                    if (installedVersion != null)
                    {
                        if (module.ModuleVersion > installedVersion)
                        {
                            module.Upgrade(environment, installedVersion);
                            store.InstallModule(module.ModuleId, module.ModuleVersion);
                        }
                        else
                        {
                            module.AutoInstalled();
                        }
                    }
                }
            }
            foreach (var feature in features)
            {
                if (!feature.RuntimeState.HasFlag(FeatureRuntimeState.Forbidden) && !feature.Module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    foreach (var dependency in feature.Dependencies)
                    {
                        var dependentFeature = featuresDictionary[dependency.FeatureId];
                        if (dependentFeature.Module.ActivationState == ModuleActivationState.RequireInstall)
                        {
                            feature.RuntimeState |= FeatureRuntimeState.DisabledDependency | FeatureRuntimeState.Forbidden;
                        }
                        else if (dependentFeature.ActivationState == FeatureActivationState.RequireEnable && !enabledFeatures.Contains(dependentFeature))
                        {
                            feature.RuntimeState |= FeatureRuntimeState.DisabledDependency | FeatureRuntimeState.Forbidden;
                        }
                    }
                }
                if (!feature.RuntimeState.HasFlag(FeatureRuntimeState.Forbidden) && feature.Module.ActivationState != ModuleActivationState.RequireInstall &&
                    feature.ActivationState == FeatureActivationState.RequireEnable && enabledFeatures.Contains(feature))
                {
                    feature.Enable(environment);
                }
            }
        }

        public static void StartupModules(params ModuleDescriptor[] modules)
        {
            foreach (var module in modules)
            {
                if (!module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    ApplicationActivator.Startup(module.GetLoadedAssemblies());
                }
            }
        }

        public static void ShutdownModules(params ModuleDescriptor[] modules)
        {
            foreach (var module in modules)
            {
                if (!module.RuntimeState.HasFlag(ModuleRuntimeState.Forbidden))
                {
                    ApplicationActivator.Shutdown(module.GetLoadedAssemblies());
                }
            }
        }
    }
}
