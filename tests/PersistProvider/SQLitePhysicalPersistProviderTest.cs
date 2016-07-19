using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLitePhysicalPersistProviderTest : PersistProviderTestBase
    {
        private readonly string _filePath = PathUtils.ResolvePath("~/App_Data/modulize.db");
        public SQLitePhysicalPersistProviderTest()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider("~/App_Data/modulize.db");
        }
    }
}
