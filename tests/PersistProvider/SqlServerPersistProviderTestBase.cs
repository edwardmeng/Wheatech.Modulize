using System.Configuration;
using System.Data.SqlClient;

namespace Wheatech.Modulize.UnitTests
{
    public abstract class SqlServerPersistProviderTestBase : PersistProviderTestBase
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "IF OBJECT_ID(N'Modules',N'U') IS NOT NULL DROP TABLE Modules";
                        command.ExecuteNonQuery();

                        command.CommandText = "IF OBJECT_ID(N'Features',N'U') IS NOT NULL DROP TABLE Features";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
