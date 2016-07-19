using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLiteConnectionStringPersistProviderTest : PersistProviderTestBase
    {
        private readonly string _filePath = PathUtils.ResolvePath("~/App_Data/modulize.db");
        public SQLiteConnectionStringPersistProviderTest()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider($"Data Source={_filePath};Version=3;");
        }
    }
}
