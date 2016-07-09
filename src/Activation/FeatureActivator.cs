using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public sealed partial class FeatureDescriptor
    {
        private MethodInfo _enableMethod;
        private MethodInfo _disableMethod;
        private bool _enabled;
        private bool _refreshingErrors;

        internal void Initialize(IActivatingEnvironment environment)
        {
            Assembly entryAssembly = null;
            if (EntryAssembly != null)
            {
                if (!Module.TryLoadAssembly(environment, EntryAssembly, out entryAssembly))
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotLoadFeatureEntry, EntryAssembly, FeatureId));
                }
            }
            else if (Module.EntryAssembly != null)
            {
                Module.TryLoadAssembly(environment, Module.EntryAssembly, out entryAssembly);
            }
            if (entryAssembly != null)
            {
                var activatorType = FindActivatorType(entryAssembly);
                if (activatorType != null)
                {
                    _enableMethod = ActivationHelper.FindMethod(activatorType, "Enable", environment);
                    _disableMethod = ActivationHelper.FindMethod(activatorType, "Disable", environment);
                }
            }
        }

        private Type FindActivatorType(Assembly assembly)
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
            Type activatorType = null;
            var nonFeatureActivatorTypes = new List<Type>();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<FeatureActivatorAttribute>();
                if (attribute != null)
                {
                    var featureId = attribute.FeatureId;
                    if (featureId == FeatureId)
                    {
                        if (activatorType == null)
                        {
                            activatorType = type;
                        }
                        else
                        {
                            throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_AmbiguousExplicitFeatureActivator, FeatureId, assembly.FullName));
                        }
                    }
                    else if (string.IsNullOrEmpty(featureId))
                    {
                        nonFeatureActivatorTypes.Add(type);
                    }
                }
            }
            if (activatorType == null)
            {
                if (nonFeatureActivatorTypes.Count > 1)
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_AmbiguousImplictFeatureActivator, assembly.FullName));
                }
                if (nonFeatureActivatorTypes.Count == 1)
                {
                    activatorType = nonFeatureActivatorTypes[0];
                }
            }
            return activatorType;
        }

        internal void Enable(IActivatingEnvironment environment)
        {
            if (_enableMethod != null)
            {
                ActivationHelper.InvokeMethod(_enableMethod, environment);
            }
            _enabled = true;
            RefreshErrors(environment);
        }

        internal void Disable(IActivatingEnvironment environment)
        {
            if (_disableMethod != null)
            {
                ActivationHelper.InvokeMethod(_disableMethod, environment);
            }
            _enabled = false;
            RefreshErrors(environment);
        }

        internal void RefreshErrors(IActivatingEnvironment environment)
        {
            if (_refreshingErrors) return;
            _refreshingErrors = true;

            var dependencyFeatures = (from dependency in Dependencies
                                      where dependency.Feature != null
                                      select dependency.Feature).ToArray();
            if (dependencyFeatures.Any(feature => feature.Errors != FeatureErrors.None))
            {
                Errors |= FeatureErrors.ForbiddenDependency;
            }
            else
            {
                Errors &= ~FeatureErrors.ForbiddenDependency;
            }
            if (dependencyFeatures.Any(feature => feature.EnableState == FeatureEnableState.RequireEnable))
            {
                Errors |= FeatureErrors.DisabledDependency;
            }
            else
            {
                Errors &= ~FeatureErrors.DisabledDependency;
            }

            if (Module.InstallState == ModuleInstallState.RequireInstall)
            {
                Errors |= FeatureErrors.UninstallModule;
            }
            else
            {
                Errors &= ~FeatureErrors.UninstallModule;
            }

            Module.RefreshErrors(environment);
            if ((Module.Errors & ~ModuleErrors.ForbiddenFeatures) != ModuleErrors.None)
            {
                Errors |= FeatureErrors.ForbiddenModule;
            }
            else
            {
                Errors &= ~FeatureErrors.ForbiddenModule;
            }
            foreach (var dependingFeature in Dependings)
            {
                dependingFeature.RefreshErrors(environment);
            }
            _refreshingErrors = false;
        }

        internal void ValidateEntryAssembly()
        {
            if (EntryAssembly != null)
            {
                IAssemblyLoader matchEntryAssembly = null;
                var assemblyIdentity = EntryAssembly;
                foreach (var assembly in Module.Assemblies)
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
                                throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_AmbiguousFeatureEntry, FeatureId));
                            }
                            break;
                    }
                }
                if (matchEntryAssembly == null)
                {
                    throw new ModuleConfigurationException(string.Format(CultureInfo.CurrentCulture, Strings.Discover_CannotFindFeatureEntry, FeatureId));
                }
            }
        }

        public FeatureErrors Errors { get; internal set; }

        public FeatureEnableState EnableState
        {
            get
            {
                if (_enableMethod != null || _disableMethod != null)
                {
                    return _enabled ? FeatureEnableState.Enabled : FeatureEnableState.RequireEnable;
                }
                return FeatureEnableState.AutoEnable;
            }
        }
    }
}
