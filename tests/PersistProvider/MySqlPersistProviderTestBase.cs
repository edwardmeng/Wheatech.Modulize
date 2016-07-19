using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class MySqlPersistProviderTestBase: PersistProviderTestBase, IDisposable
    {
        public void Dispose()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DROP TABLE IF EXISTS Modules";
                    command.ExecuteNonQuery();

                    command.CommandText = "DROP TABLE IF EXISTS Features";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
