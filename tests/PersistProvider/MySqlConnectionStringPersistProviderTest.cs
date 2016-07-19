using Wheatech.Modulize.MySql;

namespace Wheatech.Modulize.UnitTests
{
    public class MySqlConnectionStringPersistProviderTest: MySqlPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new MySqlPersistProvider("Server=localhost;Database=Modulize;Uid=root;Pwd=root;");
        }
    }
}
