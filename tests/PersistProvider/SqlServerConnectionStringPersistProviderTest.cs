using System.Configuration;
using Wheatech.Modulize.SqlServer;

namespace Wheatech.Modulize.UnitTests
{
    public class SqlServerConnectionStringPersistProviderTest : SqlServerPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SqlServerPersistProvider(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString);
        }
    }
}
