using System;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class FirebirdPersistProviderTestBase : PersistProviderTestBase
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                using (var connection = new FbConnection(DbHelper.ReplaceConnectionStringValue(ConfigurationManager.ConnectionStrings["Firebird"].ConnectionString, (name, value) =>
                {
                    if (string.Equals(name, "database", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "initial catalog", StringComparison.OrdinalIgnoreCase))
                    {
                        return PathUtils.ResolvePath(value);
                    }
                    return value;
                })))
                {
                    connection.Open();
                    using (var command = new FbCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                              "IF (EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'MODULES'))" + Environment.NewLine +
                                              "THEN" + Environment.NewLine +
                                              "EXECUTE STATEMENT 'DELETE FROM MODULES';" + Environment.NewLine +
                                              "END";
                        command.ExecuteNonQuery();

                        command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                              "IF (EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'FEATURES'))" + Environment.NewLine +
                                              "THEN" + Environment.NewLine +
                                              "EXECUTE STATEMENT 'DELETE FROM FEATURES';" + Environment.NewLine +
                                              "END";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
