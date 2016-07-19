namespace Wheatech.Modulize.SQLite
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Specifies the modulize engine to use the SQLite as backend persist provider.
        /// </summary>
        /// <param name="configuration">The <see cref="IModuleConfiguration"/> to register persist provider with.</param>
        /// <param name="nameOrConnectionString">Either the name of connection configuration name, a connection string or a physical path of database file. </param>
        /// <returns>The <see cref="IModuleConfiguration"/>.</returns>
        public static IModuleConfiguration PersistWithSQLite(this IModuleConfiguration configuration, string nameOrConnectionString)
        {
            return configuration.PersistWith(new SQLitePersistProvider(nameOrConnectionString));
        }
    }
}
