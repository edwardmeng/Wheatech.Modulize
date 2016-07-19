using System;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    /// <summary>
    /// The attribute to mark the feature activator classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureActivatorAttribute : Attribute
    {
        /// <summary>
        /// Initialize new instance of <see cref="FeatureActivatorAttribute"/> with specified feature ID.
        /// </summary>
        /// <param name="featureId">The feature ID to specify which feature should be identified.</param>
        public FeatureActivatorAttribute(string featureId)
        {
            if (string.IsNullOrEmpty(featureId))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty,nameof(featureId));
            }
            FeatureId = featureId;
        }

        /// <summary>
        /// Initialize new instance of <see cref="FeatureActivatorAttribute"/>.
        /// </summary>
        public FeatureActivatorAttribute()
        {
        }

        /// <summary>
        /// Gets the feature ID to specify which feature should be identified.
        /// </summary>
        public string FeatureId { get; }
    }
}
