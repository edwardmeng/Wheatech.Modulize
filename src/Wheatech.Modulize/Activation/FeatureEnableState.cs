namespace Wheatech.Modulize
{
    /// <summary>
    /// The enumeration for the enable state of <see cref="FeatureDescriptor"/>.
    /// </summary>
    public enum FeatureEnableState
    {
        /// <summary>
        /// The feature has declared enable method and the method has not been invoked.
        /// </summary>
        RequireEnable,
        /// <summary>
        /// The feature has declared enable method and the method has been invoked.
        /// </summary>
        Enabled,
        /// <summary>
        /// There is no enable method has been declared for the feature. And the feature can be enabled automatically without any invocation.
        /// </summary>
        AutoEnable
    }
}
