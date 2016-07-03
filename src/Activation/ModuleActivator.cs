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
        private bool _refreshingState;

        internal void Initialize(IActivatingEnvironment environment)
        {
            if (EntryAssembly != null)
            {
                Assembly entryAssembly;
                if (TryLoadAssembly(EntryAssembly, out entryAssembly))
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
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotLoadModuleEntry, EntryAssembly, ModuleId));
                }
            }
        }

        public ModuleActivationState ActivationState
        {
            get
            {
                if ((_installMethods != null && _installMethods.Length > 0) || (_uninstallMethods != null && _uninstallMethods.Length > 0))
                {
                    return _installed ? ModuleActivationState.Installed : ModuleActivationState.RequireInstall;
                }
                return ModuleActivationState.AutoInstall;
            }
        }

        public ModuleRuntimeState RuntimeState { get; internal set; }

        internal void AutoInstalled(IActivatingEnvironment environment)
        {
            _installed = true;
            RefreshRuntimeState(environment);
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
            RefreshRuntimeState(environment);
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
            RefreshRuntimeState(environment);
        }

        internal bool TryLoadAssembly(AssemblyIdentity assemblyIdentity, out Assembly assembly)
        {
            assembly = null;
            foreach (var assemblyLoader in Assemblies)
            {
                switch (assemblyLoader.Match(ref assemblyIdentity))
                {
                    case AssemblyMatchResult.Success:
                    case AssemblyMatchResult.RedirectAndMatch:
                        assembly = LoadAssembly(assemblyLoader);
                        if (!_loadedAssemblies.Contains(assembly))
                        {
                            _loadedAssemblies.Add(assembly);
                        }
                        return true;
                }
            }
            return false;
        }

        internal void LoadAssemblies()
        {
            foreach (var assemblyLoader in Assemblies)
            {
                var assembly = LoadAssembly(assemblyLoader);
                if (!_loadedAssemblies.Contains(assembly))
                {
                    _loadedAssemblies.Add(assembly);
                }
            }
        }

        private Assembly LoadAssembly(IAssemblyLoader assemblyLoader)
        {
            var assemblyIdentity = assemblyLoader.CreateIdentity();
            return _loadedAssemblies.FirstOrDefault(assembly => ActivationHelper.MatchAssembly(assembly, assemblyIdentity)) ??
                   ActivationHelper.GetDomainAssembly(assemblyIdentity) ?? assemblyLoader.Load(this);
        }

        internal void Startup()
        {
            ApplicationActivator.Startup();
        }

        internal Assembly[] GetLoadedAssemblies()
        {
            return _loadedAssemblies.ToArray();
        }

        internal void RefreshRuntimeState(IActivatingEnvironment environment)
        {
            if (_refreshingState) return;
            _refreshingState = true;
            if (HostVersion != null && !HostVersion.Match(environment.ApplicationVersion))
            {
                RuntimeState |= ModuleRuntimeState.IncompatibleHost;
            }
            else
            {
                RuntimeState &= ~ModuleRuntimeState.IncompatibleHost;
            }
            foreach (var feature in Features)
            {
                feature.RefreshRuntimeState(environment);
            }
            if (Features.All(feature => (feature.RuntimeState & ~FeatureRuntimeState.ForbiddenModule & ~FeatureRuntimeState.UninstallModule) != FeatureRuntimeState.None))
            {
                RuntimeState |= ModuleRuntimeState.ForbiddenFeatures;
            }
            else
            {
                RuntimeState &= ~ModuleRuntimeState.ForbiddenFeatures;
            }
            _refreshingState = false;
        }
    }
}
