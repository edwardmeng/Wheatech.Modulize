namespace Wheatech.Modulize.SqlServer
{
    /// <summary>
    /// Extension methods for <see cref="IModuleConfiguration"/>. 
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Specifies the modulize engine to use the Microsoft SQL Server as backend persist provider.
        /// </summary>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register persist provider with.</param>
        /// <param name="nameOrConnectionString">Either the name of connection configuration or a connection string. </param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration PersistWithSqlServer(this IModuleConfiguration configuration, string nameOrConnectionString)
        {
            return configuration.PersistWith(new SqlServerPersistProvider(nameOrConnectionString));
        }
    }
}
