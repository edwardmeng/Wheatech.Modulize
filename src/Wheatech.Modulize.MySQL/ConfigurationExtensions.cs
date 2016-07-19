namespace Wheatech.Modulize.MySql
{
    /// <summary>
    /// Extension methods for <see cref="IModuleConfiguration"/>. 
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Specifies the modulize engine to use the MySql as backend persist provider.
        /// </summary>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register persist provider with.</param>
        /// <param name="nameOrConnectionString">Either the name of connection configuration or a connection string. </param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration PersistWithMySql(this IModuleConfiguration configuration, string nameOrConnectionString)
        {
            return configuration.PersistWith(new MySqlPersistProvider(nameOrConnectionString));
        }
    }
}
