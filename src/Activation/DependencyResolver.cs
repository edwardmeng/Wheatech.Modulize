using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal static class DependencyResolver
    {
        public static FeatureDescriptor[] Sort(this IEnumerable<FeatureDescriptor> features)
        {
            var featureArray = features.ToArray();
            var dictionary = featureArray.ToDictionary(feature => feature.FeatureId);
            FeatureDescriptor[] circleFeatures;
            var sortedFeatures = DependentNode<FeatureDescriptor>.SortValues(featureArray,
                feature => feature.Dependencies.Select(dependency =>
                {
                    FeatureDescriptor dependencyFeature;
                    return dictionary.TryGetValue(dependency.FeatureId, out dependencyFeature) ? dependencyFeature : null;
                }).Where(dependencyFeature => dependencyFeature != null).ToArray(), out circleFeatures);
            if (circleFeatures != null)
            {
                throw new ModuleActivationException(string.Format(CultureInfo.CurrentCulture, Strings.Activation_CircleDependency,
                    string.Join(", ", circleFeatures.Select(feature => feature.FeatureId))));
            }
            return sortedFeatures;
        }
    }
}
