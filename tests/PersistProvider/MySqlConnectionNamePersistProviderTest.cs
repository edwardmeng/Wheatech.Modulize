using Wheatech.Modulize.MySql;

namespace Wheatech.Modulize.UnitTests
{
    public class MySqlConnectionNamePersistProviderTest: MySqlPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new MySqlPersistProvider("MySql");
        }
    }
}
