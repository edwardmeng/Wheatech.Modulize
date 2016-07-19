using Wheatech.Modulize.SqlServer;

namespace Wheatech.Modulize.UnitTests
{
    public class SqlServerConnectionNamePersistProviderTest : SqlServerPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SqlServerPersistProvider("SqlServer");
        }
    }
}
