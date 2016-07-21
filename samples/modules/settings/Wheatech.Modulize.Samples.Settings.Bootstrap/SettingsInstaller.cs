using Wheatech.Modulize.Samples.Platform.Services;

namespace Wheatech.Modulize.Samples.Settings.Bootstrap
{
    [ModuleInstaller("1.0.0")]
    public class SettingsInstaller
    {
        public void Install(IDatabaseService database)
        {
            database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS \"Settings\"(\"KEY\" TEXT NOT NULL PRIMARY KEY, \"VALUE\" TEXT NOT NULL)");
        }

        public void Uninstall(IDatabaseService database)
        {
            database.ExecuteNonQuery("DROP TABLE IF EXISTS \"Settings\"");
        }
    }
}
