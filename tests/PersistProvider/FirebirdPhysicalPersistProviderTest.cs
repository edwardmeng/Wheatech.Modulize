using System.IO;
using Wheatech.Modulize.Firebird;

namespace Wheatech.Modulize.UnitTests
{
    public class FirebirdPhysicalPersistProviderTest: PersistProviderTestBase
    {
        private const string _filePath = "~/App_Data/modulize.fdb";
        protected override IPersistProvider CreatePersistProvider()
        {
            return new FirebirdPersistProvider(_filePath);
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
