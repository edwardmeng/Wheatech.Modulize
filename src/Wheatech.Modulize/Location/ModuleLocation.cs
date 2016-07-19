namespace Wheatech.Modulize
{
    /// <summary>
    /// Represents the location information returns from the module locators and provided to module discovers.
    /// </summary>
    public class ModuleLocation
    {
        /// <summary>
        /// The type of the module.
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets the strategy for the discover algorithms.
        /// </summary>
        public DiscoverStrategy DiscoverStrategy { get; set; }

        /// <summary>
        /// Gets a value to indicate whethear the shadow copy should be performed.
        /// </summary>
        public bool EnableShadow { get; set; }
    }
}
