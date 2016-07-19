using Wheatech.Modulize.Npgsql;

namespace Wheatech.Modulize.UnitTests
{
    public class NpgsqlConnectionNamePersistProviderTest: NpgsqlPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new NpgsqlPersistProvider("PostgreSql");
        }
    }
}
