using Wheatech.Modulize.Firebird;

namespace Wheatech.Modulize.UnitTests
{
    public class FirebirdConnectionNamePersistProviderTest : FirebirdPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new FirebirdPersistProvider("Firebird");
        }
    }
}
