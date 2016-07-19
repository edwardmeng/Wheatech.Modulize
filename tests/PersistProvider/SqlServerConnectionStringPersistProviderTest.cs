using Wheatech.Modulize.SqlServer;

namespace Wheatech.Modulize.UnitTests
{
    public class SqlServerConnectionStringPersistProviderTest : SqlServerPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SqlServerPersistProvider("Server=localhost;Database=Modulize;User Id=sa;Password=sa;");
        }
    }
}
