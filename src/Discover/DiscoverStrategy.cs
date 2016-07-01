namespace Wheatech.Modulize
{
    /// <summary>
    /// Enumeration of the strategy for the discover algorithms.
    /// </summary>
    public enum DiscoverStrategy
    {
        /// <summary>
        /// Enumerate all the subsequence directories and files.
        /// </summary>
        Enumerate,
        /// <summary>
        /// Only one module, this should be the directory or file path of the module.
        /// </summary>
        Single
    }
}
