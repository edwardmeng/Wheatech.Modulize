using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.Activation;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal class FeatureManager
    {
        private readonly FeatureDescriptor _feature;
        private MethodInfo _enableMethod;
        private MethodInfo _disableMethod;

        public FeatureManager(FeatureDescriptor feature)
        {
            _feature = feature;
        }

        public void Initialize(IActivatingEnvironment environment)
        {
            Assembly entryAssembly = null;
            if (_feature.EntryAssembly != null)
            {
                if (!_feature.Module.ModuleManager.TryLoadAssembly(_feature.EntryAssembly, out entryAssembly))
                {
                    throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CannotLoadFeatureEntry, _feature.EntryAssembly, _feature.FeatureId));
                }
            }
            else if (_feature.Module.EntryAssembly!=null)
            {
                _feature.Module.ModuleManager.TryLoadAssembly(_feature.Module.EntryAssembly, out entryAssembly);
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
            var activatorTypes = ActivationHelper.FindTypes<IFeatureActivator>(assembly).ToArray();
            Type activatorType = null;
            var nonFeatureActivatorTypes = new List<Type>();
            foreach (var type in activatorTypes)
            {
                var featureId = GetFeatureId(type);
                if (featureId == _feature.FeatureId)
                {
                    if (activatorType == null)
                    {
                        activatorType = type;
                    }
                    else
                    {
                        throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_AmbiguousExplicitFeatureActivator, _feature.FeatureId, assembly.FullName));
                    }
                }
                else if (string.IsNullOrEmpty(featureId))
                {
                    nonFeatureActivatorTypes.Add(type);
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

        private string GetFeatureId(Type type)
        {
            var featureIdProperty = type.GetProperty("FeatureId");
            if (featureIdProperty == null) return null;
            if (!featureIdProperty.CanRead || featureIdProperty.GetMethod == null)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_PropertyMustCanRead, featureIdProperty.Name, TypeNameHelper.GetTypeDisplayName(type)));
            }
            if (featureIdProperty.PropertyType != typeof(string))
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_PropertyMustReturnString, featureIdProperty.Name, TypeNameHelper.GetTypeDisplayName(type)));
            }
            if (featureIdProperty.GetIndexParameters().Length > 0)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_PropertyCannotBeIndexer, featureIdProperty.Name, TypeNameHelper.GetTypeDisplayName(type)));
            }
            return (string)featureIdProperty.GetMethod.Invoke(featureIdProperty.GetMethod.IsStatic ? null : Activator.CreateInstance(type), new object[0]);
        }

        public void Enable(IActivatingEnvironment environment)
        {
            if (_enableMethod != null)
            {
                ActivationHelper.InvokeMethod(_enableMethod, environment);
            }
        }

        public void Disable(IActivatingEnvironment environment)
        {
            if (_disableMethod != null)
            {
                ActivationHelper.InvokeMethod(_disableMethod, environment);
            }
        }

        public bool RequiresEnable => _enableMethod != null || _disableMethod != null;
    }
}
