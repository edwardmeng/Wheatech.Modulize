using Wheatech.Modulize.Samples.Platform.Services;

namespace Wheatech.Modulize.Samples.Settings.Bootstrap
{
    [ModuleInstaller("1.0.0")]
    public class SettingsInstaller
    {
        public void Install(IDatabaseService database)
        {
            //database.ExecuteNonQuery("IF OBJECT_ID(N'Settings',N'U') IS NULL CREATE TABLE \"Settings\"(\"KEY\" VARCHAR(256) NOT NULL PRIMARY KEY, \"VALUE\" NVARCHAR(256) NOT NULL)");
            database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS \"Settings\"(\"KEY\" TEXT NOT NULL PRIMARY KEY, \"VALUE\" TEXT NOT NULL)");
        }

        public void Uninstall(IDatabaseService database)
        {
            //database.ExecuteNonQuery("IF OBJECT_ID(N'Settings',N'U') IS NOT NULL DROP TABLE \"Settings\"");
            database.ExecuteNonQuery("DROP TABLE IF EXISTS \"Settings\"");
        }
    }
}
