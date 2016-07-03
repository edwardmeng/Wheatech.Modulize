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
        private bool _refreshingState;

        internal void Initialize(IActivatingEnvironment environment)
        {
            Assembly entryAssembly = null;
            if (EntryAssembly != null)
            {
                if (!Module.TryLoadAssembly(EntryAssembly, out entryAssembly))
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotLoadFeatureEntry, EntryAssembly, FeatureId));
                }
            }
            else if (Module.EntryAssembly != null)
            {
                Module.TryLoadAssembly(Module.EntryAssembly, out entryAssembly);
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
            RefreshRuntimeState(environment);
        }

        internal void Disable(IActivatingEnvironment environment)
        {
            if (_disableMethod != null)
            {
                ActivationHelper.InvokeMethod(_disableMethod, environment);
            }
            _enabled = false;
            RefreshRuntimeState(environment);
        }

        internal void RefreshRuntimeState(IActivatingEnvironment environment)
        {
            if (_refreshingState) return;
            _refreshingState = true;

            var dependencyFeatures = (from dependency in Dependencies
                                      where dependency.Feature != null
                                      select dependency.Feature).ToArray();
            if (dependencyFeatures.Any(feature => feature.RuntimeState != FeatureRuntimeState.None))
            {
                RuntimeState |= FeatureRuntimeState.ForbiddenDependency;
            }
            else
            {
                RuntimeState &= ~FeatureRuntimeState.ForbiddenDependency;
            }
            if (dependencyFeatures.Any(feature => feature.ActivationState == FeatureActivationState.RequireEnable))
            {
                RuntimeState |= FeatureRuntimeState.DisabledDependency;
            }
            else
            {
                RuntimeState &= ~FeatureRuntimeState.DisabledDependency;
            }

            if (Module.ActivationState == ModuleActivationState.RequireInstall)
            {
                RuntimeState |= FeatureRuntimeState.UninstallModule;
            }
            else
            {
                RuntimeState &= ~FeatureRuntimeState.UninstallModule;
            }

            Module.RefreshRuntimeState(environment);
            if ((Module.RuntimeState & ~ModuleRuntimeState.ForbiddenFeatures) != ModuleRuntimeState.None)
            {
                RuntimeState |= FeatureRuntimeState.ForbiddenModule;
            }
            else
            {
                RuntimeState &= ~FeatureRuntimeState.ForbiddenModule;
            }
            foreach (var dependingFeature in Dependings)
            {
                dependingFeature.RefreshRuntimeState(environment);
            }
            _refreshingState = false;
        }

        public FeatureRuntimeState RuntimeState { get; internal set; }

        public FeatureActivationState ActivationState
        {
            get
            {
                if (_enableMethod != null || _disableMethod != null)
                {
                    return _enabled ? FeatureActivationState.Enabled : FeatureActivationState.RequireEnable;
                }
                return FeatureActivationState.AutoEnable;
            }
        }
    }
}
