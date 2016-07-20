using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLiteConnectionNamePersistProviderTest: PersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider("SQLite");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                string filePath = PathUtils.ResolvePath("~/App_Data/modulize.db");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
