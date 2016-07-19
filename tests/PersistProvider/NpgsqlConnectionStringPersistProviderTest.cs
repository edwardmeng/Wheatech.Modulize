using System.Configuration;
using Wheatech.Modulize.Npgsql;

namespace Wheatech.Modulize.UnitTests
{
    public class NpgsqlConnectionStringPersistProviderTest: NpgsqlPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new NpgsqlPersistProvider(ConfigurationManager.ConnectionStrings["PostgreSql"].ConnectionString);
        }
    }
}
