using System;
using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLiteConnectionNamePersistProviderTest: PersistProviderTestBase
    {
        private readonly string _filePath = PathUtils.ResolvePath("~/App_Data/modulize.db");

        public SQLiteConnectionNamePersistProviderTest()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider("SQLite");
        }
    }
}
