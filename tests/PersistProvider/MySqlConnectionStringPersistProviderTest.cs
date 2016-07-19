using System.Configuration;
using Wheatech.Modulize.MySql;

namespace Wheatech.Modulize.UnitTests
{
    public class MySqlConnectionStringPersistProviderTest: MySqlPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new MySqlPersistProvider(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString);
        }
    }
}
