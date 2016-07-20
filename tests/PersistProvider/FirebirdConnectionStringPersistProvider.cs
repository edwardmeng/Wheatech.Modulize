using System.Configuration;
using Wheatech.Modulize.Firebird;

namespace Wheatech.Modulize.UnitTests
{
    public class FirebirdConnectionStringPersistProvider : FirebirdPersistProviderTestBase
    {
        protected override IPersistProvider CreatePersistProvider()
        {
            return new FirebirdPersistProvider(ConfigurationManager.ConnectionStrings["Firebird"].ConnectionString);
        }
    }
}
