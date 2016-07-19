using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLiteMemoryPersistProviderTest : PersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider();
        }
    }
}
