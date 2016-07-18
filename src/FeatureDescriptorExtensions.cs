using System.Collections.Generic;
using System.Linq;

namespace Wheatech.Modulize
{
    public static class FeatureDescriptorExtensions
    {
        public static IEnumerable<FeatureDescriptor> GetDependingFeatures(this FeatureDescriptor feature)
        {
            return GetDependingFeaturesImpl(feature).Distinct();
        }

        private static IEnumerable<FeatureDescriptor> GetDependingFeaturesImpl(FeatureDescriptor feature)
        {
            foreach (var depending in feature.Dependings)
            {
                yield return depending;
                foreach (var childDepending in GetDependingFeaturesImpl(depending))
                {
                    yield return childDepending;
                }
            }
        }

        public static IEnumerable<FeatureDescriptor> GetDependedFeatures(this FeatureDescriptor feature)
        {
            return GetDependedFeaturesImpl(feature).Distinct();
        }

        private static IEnumerable<FeatureDescriptor> GetDependedFeaturesImpl(FeatureDescriptor feature)
        {
            foreach (var dependency in feature.Dependencies)
            {
                yield return dependency.Feature;
                foreach (var dependencyFeature in GetDependedFeaturesImpl(dependency.Feature))
                {
                    yield return dependencyFeature;
                }
            }
        }
    }
}
