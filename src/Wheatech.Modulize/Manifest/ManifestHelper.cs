using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal static class ManifestHelper
    {
        #region Fields

        private static readonly string[] ModuleIdPropertyNames = { "moduleid", "module-id", "module.id", "module id", "id", "module" };
        private static readonly string[] ModuleNamePropertyNames = { "modulename", "module-name", "module.name", "module name", "name" };
        private static readonly string[] ModuleDescriptionPropertyNames = { "moduledescription", "module-description", "module.description", "module description", "description" };
        private static readonly string[] ModuleVersionPropertyNames = { "moduleversion", "module-version", "module.version", "module version", "version" };
        private static readonly string[] ModuleWebSitePropertyNames = { "modulewebsite", "module-website", "module.website", "module website", "website" };

        private static readonly string[] ModuleLicensePropertyNames =
        {
            "modulelicense", "modulelicenseurl", "module-license", "module-license-url", "module-licenseurl", "module license", "module licenseurl",
            "module license url", "module.license", "module.licenseurl", "license", "licenseurl", "license url",
        };

        private static readonly string[] ModuleEntryAssemblyPropertyNames =
        {
            "moduleentryassembly", "module-entryassembly", "module-entry-assembly", "module.entryassembly",
            "module.entry.assembly", "module entry assembly", "moduleassembly", "module-assembly", "module.assembly", "module assembly", "entryassembly", "entry-assembly",
            "entry assembly", "assembly"
        };

        private static readonly string[] ModuleAuthorsPropertyNames =
        {
            "moduleauthors", "moduleauthor", "module-authors", "module-author", "module.authors", "module.author",
            "module authors", "module author", "authors", "author"
        };

        private static readonly string[] ModuleTagsPropertyNames =
        {
            "moduletags", "moduletag", "module-tags", "module-tag", "module.tags", "module.tag", "module tags", "module tag",
            "tags", "tag"
        };

        private static readonly string[] ModuleCategoryPropertyNames = { "modulecategory", "module-category", "module.category", "module category", "category" };

        private static readonly string[] ModuleHostVersionPropertyNames =
        {
            "platformversion", "hostversion", "platform-version", "host-version", "platform.version", "host.version",
            "platform version", "host version", "platform", "host"
        };

        private static readonly string[] FeatureIdPropertyNames = { "feature", "featureid", "feature-id", "feature.id", "feature id", "id" };
        private static readonly string[] FeatureNamePropertyNames = { "featurename", "feature-name", "feature.name", "feature name", "name" };

        private static readonly string[] FeatureDescriptionPropertyNames =
        {
            "featuredescription", "feature-description", "feature.description", "feature description", "description"
        };

        private static readonly string[] FeatureCategoryPropertyNames = { "featurecategory", "feature-category", "feature.category", "feature category", "category" };

        private static readonly string[] FeatureDependencyPropertyNames =
        {
            "featuredependencies", "feature-dependencies", "feature.dependencies", "feature dependencies", "featuredependency",
            "feature-dependency", "feature.dependency", "feature dependency", "dependencies", "dependency"
        };

        private static readonly string[] FeatureEntryAssemblyPropertyNames =
        {
            "featureentryassembly", "feature-entryassembly", "feature-entry-assembly", "feature.entryassembly",
            "feature.entry.assembly", "feature entry assembly", "featureassembly", "feature-assembly", "feature.assembly", "feature assembly", "entryassembly", "entry-assembly",
            "entry assembly", "assembly"
        };

        private static readonly string[] DependencyFeatureVersionPropertyNames = { "featureversion", "feature-version", "feature.version", "feature version", "version" };

        private static readonly string[] FeatureSectionNames = { "features", "feature" };

        private static readonly string[] FeaturePropertyNames;

        #endregion

        #region Constructor

        static ManifestHelper()
        {
            var propertyNames = new List<string>();
            propertyNames.AddRange(FeatureIdPropertyNames);
            propertyNames.AddRange(FeatureNamePropertyNames);
            propertyNames.AddRange(FeatureDescriptionPropertyNames);
            propertyNames.AddRange(FeatureCategoryPropertyNames);
            propertyNames.AddRange(FeatureDependencyPropertyNames);
            FeaturePropertyNames = propertyNames.ToArray();
        }

        #endregion

        #region Entry Point

        public static ModuleDescriptor BuildModule(Dictionary<string, object> properties, string defaultModuleId)
        {
            var moduleId = Convert.ToString(RemoveProperty(properties, ModuleIdPropertyNames));
            if (string.IsNullOrEmpty(moduleId))
            {
                moduleId = defaultModuleId;
            }
            var moduleName = Convert.ToString(RemoveProperty(properties, ModuleNamePropertyNames) ?? moduleId);
            if (string.IsNullOrEmpty(moduleName)) moduleName = moduleId;
            var description = Convert.ToString(RemoveProperty(properties, ModuleDescriptionPropertyNames));

            string propertyName;
            var moduleVersionText = Convert.ToString(RemoveProperty(properties, ModuleVersionPropertyNames, out propertyName));
            if (string.IsNullOrEmpty(moduleVersionText))
            {
                throw new ManifestParseException("The module version is required.");
            }
            Version moduleVersion;
            if (!Version.TryParse(moduleVersionText, out moduleVersion))
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidFormat, propertyName, moduleId));
            }
            var hostVersionText = Convert.ToString(RemoveProperty(properties, ModuleHostVersionPropertyNames, out propertyName));
            IVersionComparator hostVersion = null;
            if (!string.IsNullOrEmpty(hostVersionText) && !VersionComparatorFactory.TryParse(hostVersionText, out hostVersion))
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidFormat, propertyName, moduleId));
            }
            var authors = ParseArray(properties, ModuleAuthorsPropertyNames);
            var tags = ParseArray(properties, ModuleTagsPropertyNames);
            var website = Convert.ToString(RemoveProperty(properties, ModuleWebSitePropertyNames));
            var license = Convert.ToString(RemoveProperty(properties, ModuleLicensePropertyNames));
            var entryAssemblyText = Convert.ToString(RemoveProperty(properties, ModuleEntryAssemblyPropertyNames, out propertyName));
            AssemblyIdentity entryAssembly = null;
            if (!string.IsNullOrEmpty(entryAssemblyText) && !AssemblyIdentity.TryParse(entryAssemblyText, out entryAssembly))
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidFormat, propertyName, moduleId));
            }
            var module = new ModuleDescriptor
            {
                ModuleId = moduleId,
                ModuleName = moduleName,
                Description = description,
                ModuleVersion = moduleVersion,
                HostVersion = hostVersion,
                EntryAssembly = entryAssembly,
                License = license,
                Authors = authors != null && authors.Length == 0 ? null : authors,
                Tags = tags != null && tags.Length == 0 ? null : tags,
                WebSite = website
            };
            module.Features = ParseFeatures(properties, module);
            return module;
        }

        #endregion

        #region Parse Feature

        private static FeatureDescriptorCollection ParseFeatures(IDictionary<string, object> properties, ModuleDescriptor module)
        {
            var defaultCategory = Convert.ToString(RemoveProperty(properties, ModuleCategoryPropertyNames));
            var features = new FeatureDescriptorCollection(RemoveProperties(properties, FeatureSectionNames, (name, value) => ParseFeatures(value, defaultCategory, module)));
            if (features.Count == 0)
            {
                var featureId = Convert.ToString(RemoveProperty(properties, FeatureIdPropertyNames));
                features.Add(ParseOuterSingleFeature(properties, string.IsNullOrEmpty(featureId) ? module.ModuleId : featureId, defaultCategory, module));
            }
            features.SetReadOnly();
            return features;
        }

        private static IEnumerable<FeatureDescriptor> ParseFeatures(object value, string defaultCategory, ModuleDescriptor module)
        {
            var featuresProperties = value as IDictionary<string, object>;
            if (featuresProperties != null)
            {
                if (featuresProperties.Any(x => x.Value is IDictionary<string, object> && !FeaturePropertyNames.Contains(x.Key)))
                {
                    foreach (var featuresProperty in featuresProperties)
                    {
                        yield return
                            ParseMultileFeature(featuresProperty.Value as IDictionary<string, object> ?? new Dictionary<string, object>(), featuresProperty.Key, defaultCategory,
                                module);
                    }
                }
                else
                {
                    yield return ParseInnerSingleFeature(featuresProperties, module.ModuleId, defaultCategory, module);
                }
            }
            else
            {
                var featureString = value as string;
                if (!string.IsNullOrWhiteSpace(featureString))
                {
                    yield return ParseInnerSingleFeature(new Dictionary<string, object>(), featureString, defaultCategory, module);
                }
            }
        }

        private static FeatureDescriptor ParseMultileFeature(IDictionary<string, object> properties, string defaultFeatureId, string defaultCategory,
            ModuleDescriptor module)
        {
            var featureId = Convert.ToString(RemoveProperty(properties, FeatureIdPropertyNames));
            if (string.IsNullOrEmpty(featureId)) featureId = defaultFeatureId;
            var featureName = Convert.ToString(RemoveProperty(properties, FeatureNamePropertyNames));
            if (string.IsNullOrEmpty(featureName)) featureName = featureId;
            var description = Convert.ToString(RemoveProperty(properties, FeatureDescriptionPropertyNames));
            var category = Convert.ToString(RemoveProperty(properties, FeatureCategoryPropertyNames));
            if (string.IsNullOrEmpty(category)) category = defaultCategory;
            var entryAssembly = ParseFeatureEntryAssembly(properties, module.ModuleId);
            var dependencies = new DependencyDescriptorCollection(ParseDependencies(properties, module.ModuleId));
            dependencies.SetReadOnly();
            return new FeatureDescriptor
            {
                FeatureId = featureId,
                FeatureName = featureName,
                Description = description,
                Category = category,
                Dependencies = dependencies,
                Module = module,
                EntryAssembly = entryAssembly
            };
        }

        private static FeatureDescriptor ParseInnerSingleFeature(IDictionary<string, object> properties, string defaultFeatureId, string defaultCategory,
            ModuleDescriptor module)
        {
            var featureId = Convert.ToString(RemoveProperty(properties, FeatureIdPropertyNames));
            if (string.IsNullOrEmpty(featureId)) featureId = defaultFeatureId;
            var featureName = Convert.ToString(RemoveProperty(properties, FeatureNamePropertyNames));
            if (string.IsNullOrEmpty(featureName))
            {
                featureName = module.ModuleName == module.ModuleId ? featureId : module.ModuleName;
            }
            var description = Convert.ToString(RemoveProperty(properties, FeatureDescriptionPropertyNames));
            if (string.IsNullOrEmpty(description)) description = module.Description;
            var category = Convert.ToString(RemoveProperty(properties, FeatureCategoryPropertyNames));
            if (string.IsNullOrEmpty(category)) category = defaultCategory;
            var entryAssembly = ParseFeatureEntryAssembly(properties, module.ModuleId);
            var dependencies = new DependencyDescriptorCollection(ParseDependencies(properties, module.ModuleId));
            dependencies.SetReadOnly();
            return new FeatureDescriptor
            {
                FeatureId = featureId,
                FeatureName = featureName,
                Description = description,
                Category = category,
                Dependencies = dependencies,
                Module = module,
                EntryAssembly = entryAssembly ?? module.EntryAssembly
            };
        }

        private static FeatureDescriptor ParseOuterSingleFeature(IDictionary<string, object> properties, string featureId, string defaultCategory,
            ModuleDescriptor module)
        {
            var featureName = Convert.ToString(RemoveProperty(properties, FeatureNamePropertyNames));
            if (string.IsNullOrEmpty(featureName))
            {
                featureName = module.ModuleName == module.ModuleId ? featureId : module.ModuleName;
            }
            var description = Convert.ToString(RemoveProperty(properties, FeatureDescriptionPropertyNames));
            if (string.IsNullOrEmpty(description)) description = module.Description;
            var category = Convert.ToString(RemoveProperty(properties, FeatureCategoryPropertyNames));
            if (string.IsNullOrEmpty(category)) category = defaultCategory;
            var entryAssembly = ParseFeatureEntryAssembly(properties, module.ModuleId);
            var dependencies = new DependencyDescriptorCollection(ParseDependencies(properties, module.ModuleId));
            dependencies.SetReadOnly();
            return new FeatureDescriptor
            {
                FeatureId = featureId,
                FeatureName = featureName,
                Description = description,
                Category = category,
                Dependencies = dependencies,
                Module = module,
                EntryAssembly = entryAssembly ?? module.EntryAssembly
            };
        }

        private static AssemblyIdentity ParseFeatureEntryAssembly(IDictionary<string, object> properties, string moduleId)
        {
            string propertyName;
            var entryAssemblyText = Convert.ToString(RemoveProperty(properties, FeatureEntryAssemblyPropertyNames, out propertyName));
            AssemblyIdentity entryAssembly = null;
            if (!string.IsNullOrEmpty(entryAssemblyText) && !AssemblyIdentity.TryParse(entryAssemblyText, out entryAssembly))
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidFormat, propertyName, moduleId));
            }
            return entryAssembly;
        }

        #endregion

        #region Parse Dependency

        private static IEnumerable<DependencyDescriptor> ParseDependencies(IDictionary<string, object> properties, string moduleId)
        {
            return RemoveProperties(properties, FeatureDependencyPropertyNames, (name, value) => ParseDependencies(value, moduleId));
        }

        private static IEnumerable<DependencyDescriptor> ParseDependencies(object value, string moduleId)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                foreach (var dependencyText in stringValue.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
                {
                    DependencyDescriptor dependency;
                    if (!DependencyDescriptor.TryParse(dependencyText, out dependency))
                    {
                        throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidDependency, dependencyText, moduleId));
                    }
                    yield return dependency;
                }
                yield break;
            }
            var properties = value as IDictionary<string, object>;
            if (properties != null)
            {
                var featureId = Convert.ToString(RemoveProperty(properties, FeatureIdPropertyNames));
                if (!string.IsNullOrEmpty(featureId))
                {
                    yield return ParseDependency(properties, featureId, moduleId);
                    yield break;
                }
                foreach (var property in properties)
                {
                    var propertyValue = property.Value as IDictionary<string, object>;
                    if (propertyValue != null)
                    {
                        featureId = Convert.ToString(RemoveProperty(propertyValue, FeatureIdPropertyNames));
                        yield return ParseDependency(propertyValue, string.IsNullOrEmpty(featureId) ? property.Key : featureId, moduleId);
                        continue;
                    }
                    var dependencyText = property.Value as string;
                    if (!string.IsNullOrWhiteSpace(dependencyText))
                    {
                        DependencyDescriptor dependency;
                        if (!DependencyDescriptor.TryParse(dependencyText, property.Key, out dependency))
                        {
                            throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidDependency, property.Key + ": " + dependencyText, moduleId));
                        }
                        yield return dependency;
                    }
                }
                yield break;
            }

            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                foreach (var dependency in enumerable.OfType<object>().SelectMany(x => ParseDependencies(x, moduleId)))
                {
                    yield return dependency;
                }
            }
        }

        private static DependencyDescriptor ParseDependency(IDictionary<string, object> properties, string featureId, string moduleId)
        {
            var version = Convert.ToString(RemoveProperty(properties, DependencyFeatureVersionPropertyNames));
            IVersionComparator featureVersion = null;
            if (!string.IsNullOrEmpty(version) &&
                !VersionComparatorFactory.TryParse(version, out featureVersion))
            {
                throw new ManifestParseException(string.Format(CultureInfo.CurrentCulture, Strings.Manifest_InvalidDependencyVersion, version, moduleId));
            }
            return new DependencyDescriptor
            {
                FeatureId = featureId,
                Version = featureVersion
            };
        }

        #endregion

        #region Helpers

        private static string[] ParseArray(IDictionary<string, object> properties, string[] propertyNames)
        {
            return RemoveProperties(properties, propertyNames, (name, value) => ParseArray(value)).ToArray();
        }

        private static string[] ParseArray(object value)
        {
            if (value == null) return null;
            var stringArray = value as string[];
            if (stringArray != null) return stringArray;
            var stringValue = value as string;
            if (stringValue != null)
            {
                return stringValue.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }
            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.OfType<object>().SelectMany(obj => ParseArray(obj) ?? new string[0]).ToArray();
            }
            return new[] { Convert.ToString(value) };
        }

        private static IEnumerable<T> RemoveProperties<T>(IDictionary<string, object> properties, string[] propertyNames, Func<string, object, IEnumerable<T>> callback)
        {
            foreach (var name in propertyNames)
            {
                object value;
                if (properties.TryGetValue(name, out value))
                {
                    properties.Remove(name);
                    var result = callback?.Invoke(name, value);
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private static object RemoveProperty(IDictionary<string, object> properties, params string[] propertyNames)
        {
            string propertyName;
            return RemoveProperty(properties, propertyNames, out propertyName);
        }

        private static object RemoveProperty(IDictionary<string, object> properties, string[] propertyNames, out string propertyName)
        {
            propertyName = null;
            foreach (var name in propertyNames)
            {
                object propertyValue;
                if (properties.TryGetValue(name, out propertyValue))
                {
                    propertyName = name;
                    properties.Remove(name);
                    return propertyValue;
                }
            }
            return null;
        }

        #endregion
    }
}
