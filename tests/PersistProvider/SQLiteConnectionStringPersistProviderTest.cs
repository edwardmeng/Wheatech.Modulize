using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLiteConnectionStringPersistProviderTest : PersistProviderTestBase
    {
        private readonly string _filePath = PathUtils.ResolvePath("~/App_Data/modulize.db");

        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider($"Data Source={_filePath};Version=3;");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
            }
        }
    }
}
