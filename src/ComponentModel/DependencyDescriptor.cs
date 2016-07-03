using System;
using System.Globalization;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public sealed class DependencyDescriptor
    {
        public string FeatureId { get; internal set; }

        public IVersionComparator Version { get; internal set; }

        internal FeatureDescriptor Feature { get; set; }

        public static bool TryParse(string value, out DependencyDescriptor dependency)
        {
            dependency = null;
            if (string.IsNullOrWhiteSpace(value)) return false;
            string featureId, featureVersionText = null;
            var index = value.IndexOf(' ');
            if (index != -1)
            {
                featureId = value.Substring(0, index).Trim();
                featureVersionText = value.Substring(index + 1).Trim();
            }
            else
            {
                featureId = value;
            }
            if (string.IsNullOrEmpty(featureId)) return false;
            IVersionComparator featureVersion = null;
            if (!string.IsNullOrEmpty(featureVersionText) &&
                !VersionComparatorFactory.TryParse(featureVersionText, out featureVersion))
            {
                return false;
            }
            dependency = new DependencyDescriptor { FeatureId = featureId, Version = featureVersion };
            return true;
        }

        internal static bool TryParse(string value, string featureId, out DependencyDescriptor dependency)
        {
            dependency = null;
            if (string.IsNullOrWhiteSpace(value)) return false;
            IVersionComparator featureVersion = null;
            if (!string.IsNullOrEmpty(value) && !VersionComparatorFactory.TryParse(value, out featureVersion))
            {
                return false;
            }
            dependency = new DependencyDescriptor
            {
                FeatureId = featureId,
                Version = featureVersion
            };
            return true;
        }

        public static DependencyDescriptor Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(value));
            }
            DependencyDescriptor dependency;
            if (!TryParse(value, out dependency))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Invalid_Dependency, value), nameof(value));
            }
            return dependency;
        }

        public override string ToString()
        {
            return FeatureId + (Version == null ? null : " " + Version);
        }
    }
}
