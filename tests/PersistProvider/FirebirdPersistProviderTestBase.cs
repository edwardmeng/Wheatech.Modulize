using System;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class FirebirdPersistProviderTestBase : PersistProviderTestBase, IDisposable
    {
        public void Dispose()
        {
            using (var connection = new FbConnection(ConfigurationManager.ConnectionStrings["Firebird"].ConnectionString))
            {
                connection.Open();
                using (var command = new FbCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                          "IF (EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'MODULES'))" + Environment.NewLine +
                                          "THEN" + Environment.NewLine +
                                          "EXECUTE STATEMENT 'DROP TABLE MODULES';" + Environment.NewLine +
                                          "END";
                    command.ExecuteNonQuery();

                    command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                          "IF (EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'FEATURES'))" + Environment.NewLine +
                                          "THEN" + Environment.NewLine +
                                          "EXECUTE STATEMENT 'DROP TABLE FEATURES';" + Environment.NewLine +
                                          "END";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
