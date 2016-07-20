using System.Configuration;
using Npgsql;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class NpgsqlPersistProviderTestBase : PersistProviderTestBase
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                using (var connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["PostgreSql"].ConnectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand())
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
}
