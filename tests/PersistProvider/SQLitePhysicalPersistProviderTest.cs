﻿using System.IO;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.UnitTests
{
    public class SQLitePhysicalPersistProviderTest : PersistProviderTestBase
    {
        private const string _filePath = "~/App_Data/modulize.db";
        protected override IPersistProvider CreatePersistProvider()
        {
            return new SQLitePersistProvider(_filePath);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                string filePath = PathUtils.ResolvePath(_filePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
