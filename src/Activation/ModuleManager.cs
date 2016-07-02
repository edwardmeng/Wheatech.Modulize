using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;

namespace Wheatech.Modulize
{
    internal class ModuleManager
    {
        private readonly ModuleDescriptor _module;
        private readonly List<Assembly> _loadedAssemblies = new List<Assembly>();
        private MethodInfo[] _installMethods;
        private MethodInfo[] _uninstallMethods;
        public ModuleManager(ModuleDescriptor module)
        {
            _module = module;
        }

        public void Initialize(IActivatingEnvironment environment)
        {
            if (_module.EntryAssembly != null)
            {
                Assembly entryAssembly;
                if (TryLoadAssembly(_module.EntryAssembly, out entryAssembly))
                {
                    var installerTypes = ActivationHelper.FindTypes<IModuleInstaller>(entryAssembly).ToArray();
                    var installMethods = new List<Tuple<Version, MethodInfo>>();
                    var uninstallMethods = new List<Tuple<Version, MethodInfo>>();
                    foreach (var installerType in installerTypes)
                    {
                        var installMethod = ActivationHelper.FindMethod(installerType, "Install", environment);
                        var uninstallMethod = ActivationHelper.FindMethod(installerType, "Uninstall", environment);
                        if (installMethod != null || uninstallMethod != null)
                        {
                            var version = ((IModuleInstaller)Activator.CreateInstance(installerType)).Version;
                            if (installMethod != null)
                            {
                                installMethods.Add(Tuple.Create(version, installMethod));
                            }
                            if (uninstallMethod != null)
                            {
                                uninstallMethods.Add(Tuple.Create(version, uninstallMethod));
                            }
                        }
                    }
                    _installMethods = (from method in installMethods
                                       orderby method.Item1
                                       select method.Item2).ToArray();

                    _uninstallMethods = (from method in uninstallMethods
                                         orderby method.Item1 descending
                                         select method.Item2).ToArray();
                }
            }
        }

        public bool RequiresInstall => (_installMethods != null && _installMethods.Length > 0) || (_uninstallMethods != null && _uninstallMethods.Length > 0);

        public void Install(IActivatingEnvironment environment)
        {
            if (_installMethods != null)
            {
                foreach (var method in _installMethods)
                {
                    ActivationHelper.InvokeMethod(method, environment);
                }
            }
        }

        public void Uninstall(IActivatingEnvironment environment)
        {
            if (_uninstallMethods != null)
            {
                foreach (var method in _uninstallMethods)
                {
                    ActivationHelper.InvokeMethod(method, environment);
                }
            }
        }

        public bool TryLoadAssembly(AssemblyIdentity assemblyIdentity, out Assembly assembly)
        {
            assembly = null;
            foreach (var assemblyLoader in _module.Assemblies)
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

        public void LoadAssemblies()
        {
            foreach (var assemblyLoader in _module.Assemblies)
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
            return ActivationHelper.GetDomainAssembly(assemblyLoader.CreateIdentity()) ?? assemblyLoader.Load(_module);
        }

        public void Startup()
        {
            ApplicationActivator.Startup();
        }
    }
}
