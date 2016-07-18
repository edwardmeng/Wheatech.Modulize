using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public sealed partial class ModuleDescriptor
    {
        private readonly List<Assembly> _loadedAssemblies = new List<Assembly>();
        private Tuple<Version, MethodInfo>[] _installMethods;
        private MethodInfo[] _uninstallMethods;
        private bool _installed;
        private bool _refreshingErrors;

        internal void Initialize(IActivatingEnvironment environment)
        {
            if (EntryAssembly != null)
            {
                Assembly entryAssembly;
                if (TryLoadAssembly(environment, EntryAssembly, out entryAssembly))
                {
                    IEnumerable<TypeInfo> types;
                    try
                    {
                        types = entryAssembly.DefinedTypes;
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
                    }
                    var installMethods = new List<Tuple<Version, MethodInfo>>();
                    var uninstallMethods = new List<Tuple<Version, MethodInfo>>();
                    foreach (var type in types)
                    {
                        var attribute = type.GetCustomAttribute<ModuleInstallerAttribute>();
                        if (attribute != null)
                        {
                            if ((type.IsAbstract && !type.IsSealed) || type.IsGenericTypeDefinition)
                            {
                                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_InvalidModuleInstaller, TypeNameHelper.GetTypeDisplayName(type)));
                            }
                            var installMethod = ActivationHelper.FindMethod(type, "Install", environment);
                            if (installMethod != null)
                            {
                                installMethods.Add(Tuple.Create(attribute.Version, installMethod));
                            }
                            var uninstallMethod = ActivationHelper.FindMethod(type, "Uninstall", environment);
                            if (uninstallMethod != null)
                            {
                                uninstallMethods.Add(Tuple.Create(attribute.Version, uninstallMethod));
                            }
                        }
                    }
                    _installMethods = (from method in installMethods
                                       where method.Item1 <= ModuleVersion
                                       orderby method.Item1
                                       select method).ToArray();

                    _uninstallMethods = (from method in uninstallMethods
                                         where method.Item1 <= ModuleVersion
                                         orderby method.Item1 descending
                                         select method.Item2).ToArray();
                }
                else
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotLoadModuleEntry, ModuleId, EntryAssembly));
                }
            }
        }

        public ModuleInstallState InstallState
        {
            get
            {
                if (_installed) return ModuleInstallState.Installed;
                if ((_installMethods != null && _installMethods.Length > 0) || (_uninstallMethods != null && _uninstallMethods.Length > 0))
                {
                    return ModuleInstallState.RequireInstall;
                }
                return ModuleInstallState.AutoInstall;
            }
        }

        public ModuleErrors Errors { get; internal set; }

        internal void AutoInstalled(IActivatingEnvironment environment)
        {
            _installed = true;
            RefreshErrors(environment);
        }

        internal void Install(IActivatingEnvironment environment)
        {
            if (_installMethods != null)
            {
                foreach (var method in _installMethods)
                {
                    ActivationHelper.InvokeMethod(method.Item2, environment);
                }
            }
            _installed = true;
            RefreshErrors(environment);
        }

        internal void Upgrade(IActivatingEnvironment environment, Version installedVersion)
        {
            if (_installMethods != null)
            {
                foreach (var method in _installMethods)
                {
                    if (method.Item1 > installedVersion)
                    {
                        ActivationHelper.InvokeMethod(method.Item2, environment);
                    }
                }
            }
        }

        internal void Uninstall(IActivatingEnvironment environment)
        {
            if (_uninstallMethods != null)
            {
                foreach (var method in _uninstallMethods)
                {
                    ActivationHelper.InvokeMethod(method, environment);
                }
            }
            _installed = false;
            RefreshErrors(environment);
        }

        internal bool TryLoadAssembly(IActivatingEnvironment environment, AssemblyIdentity assemblyIdentity, out Assembly assembly)
        {
            assembly = null;
            foreach (var assemblyLoader in Assemblies)
            {
                switch (assemblyLoader.Match(ref assemblyIdentity))
                {
                    case AssemblyMatchResult.Success:
                    case AssemblyMatchResult.RedirectAndMatch:
                        assembly = LoadAssembly(environment, assemblyLoader);
                        if (!_loadedAssemblies.Contains(assembly))
                        {
                            _loadedAssemblies.Add(assembly);
                        }
                        return true;
                }
            }
            return false;
        }

        internal void LoadAssemblies(IActivatingEnvironment environment)
        {
            foreach (var assemblyLoader in Assemblies)
            {
                var assembly = LoadAssembly(environment, assemblyLoader);
                if (!_loadedAssemblies.Contains(assembly))
                {
                    _loadedAssemblies.Add(assembly);
                }
            }
        }

        private Assembly LoadAssembly(IActivatingEnvironment environment, IAssemblyLoader assemblyLoader)
        {
            var assemblyIdentity = assemblyLoader.CreateIdentity();
            return _loadedAssemblies.FirstOrDefault(assembly => ActivationHelper.MatchAssembly(assembly, assemblyIdentity)) ??
                   ActivationHelper.GetDomainAssembly(environment, assemblyIdentity) ?? assemblyLoader.Load(this);
        }

        internal void Startup()
        {
            ApplicationActivator.Startup();
        }

        internal Assembly[] GetLoadedAssemblies()
        {
            return _loadedAssemblies.ToArray();
        }

        internal void RefreshErrors(IActivatingEnvironment environment)
        {
            if (_refreshingErrors) return;
            _refreshingErrors = true;
            if (HostVersion != null && !HostVersion.Match(environment.ApplicationVersion))
            {
                Errors |= ModuleErrors.IncompatibleHost;
            }
            else
            {
                Errors &= ~ModuleErrors.IncompatibleHost;
            }
            foreach (var feature in Features)
            {
                feature.RefreshErrors(environment);
            }
            if (Features.All(feature => (feature.Errors & ~FeatureErrors.ForbiddenModule & ~FeatureErrors.UninstallModule) != FeatureErrors.None))
            {
                Errors |= ModuleErrors.ForbiddenFeatures;
            }
            else
            {
                Errors &= ~ModuleErrors.ForbiddenFeatures;
            }
            _refreshingErrors = false;
        }

        internal void ValidateEntryAssembly()
        {
            if (EntryAssembly != null)
            {
                var assemblyIdentity = EntryAssembly;
                IAssemblyLoader matchEntryAssembly = null;
                foreach (var assembly in Assemblies)
                {
                    switch (assembly.Match(ref assemblyIdentity))
                    {
                        case AssemblyMatchResult.Success:
                        case AssemblyMatchResult.RedirectAndMatch:
                            if (matchEntryAssembly == null)
                            {
                                matchEntryAssembly = assembly;
                            }
                            else
                            {
                                throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousModuleEntry, ModuleId));
                            }
                            break;
                    }
                }
                if (matchEntryAssembly == null)
                {
                    throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_CannotFindModuleEntry, ModuleId, EntryAssembly));
                }
            }
            foreach (var feature in Features)
            {
                feature.ValidateEntryAssembly();
            }
        }
    }
}
