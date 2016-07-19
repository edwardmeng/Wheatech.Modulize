using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace Wheatech.Modulize.SQLite
{
    /// <summary>
    /// The SQLitePersistProvider implements the methods to use SQLite as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class SQLitePersistProvider : IPersistProvider
    {
        private string _connectionString;

        /// <summary>
        /// Initialize new instance of <see cref="SQLitePersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration name, a connection string or a physical path of database file. </param>
        public SQLitePersistProvider(string nameOrConnectionString)
        {
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                _connectionString = "Data Source=:memory:;Version=3;New=True;";
                return;
            }
            string connectionStringName;
            if (!TryGetConnectionName(nameOrConnectionString, out connectionStringName) || !TryInitializeFromAppConfig(connectionStringName))
            {
                if (!string.IsNullOrEmpty(nameOrConnectionString) && nameOrConnectionString.IndexOf('=') < 0)
                {
                    _connectionString = $"Data Source={PathUtils.ResolvePath(nameOrConnectionString)};Version=3;";
                }
                else
                {
                    _connectionString = nameOrConnectionString;
                }
            }
            ExecuteCommand(command =>
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Modules(ID TEXT NOT NULL PRIMARY KEY, Version TEXT)";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Features(ID TEXT NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Initialize new instance of <see cref="SQLitePersistProvider"/>.
        /// </summary>
        public SQLitePersistProvider() : this(null)
        {
        }

        private void ExecuteCommand(Action<SQLiteCommand> action)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandType = CommandType.Text;
                    action(command);
                }
            }
        }

        /// <summary>
        /// Saves the installed module by using the module ID and the installed version.
        /// </summary>
        /// <param name="moduleId">The module ID of the installed module.</param>
        /// <param name="version">The version of the installed module.</param>
        public void InstallModule(string moduleId, Version version)
        {
            ExecuteCommand(command =>
            {
                command.CommandText = "INSERT OR REPLACE INTO Modules(ID, Version) VALUES(@ID, @Version)";
                command.Parameters.AddWithValue("ID", moduleId);
                command.Parameters.AddWithValue("Version", version.ToString());
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Removes the installed module by using the module ID.
        /// </summary>
        /// <param name="moduleId">The module ID of the uninstalled module.</param>
        public void UninstallModule(string moduleId)
        {
            ExecuteCommand(command =>
            {
                command.CommandText = "DELETE FROM Modules WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", moduleId);
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Saves the enabled feature by using the feature ID.
        /// </summary>
        /// <param name="featureId">The feature ID of the enabled feature.</param>
        public void EnableFeature(string featureId)
        {
            ExecuteCommand(command =>
            {
                command.CommandText = "INSERT OR REPLACE INTO Features(ID) VALUES(@ID)";
                command.Parameters.AddWithValue("ID", featureId);
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Removes the enabled feature by using the feature ID.
        /// </summary>
        /// <param name="featureId">The feature ID of the disabled feature.</param>
        public void DisableFeature(string featureId)
        {
            ExecuteCommand(command =>
            {
                command.CommandText = "DELETE FROM Features WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", featureId);
                command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Gets a boolean value to indicate whether the feature has been enabled.
        /// </summary>
        /// <param name="featureId">The feature ID of the feature to be verified.</param>
        /// <returns><c>true</c> if the feature has been enabled; otherwise <c>false</c>.</returns>
        public bool GetFeatureEnabled(string featureId)
        {
            object result = null;
            ExecuteCommand(command =>
            {
                command.CommandText = "SELECT COUNT(*) FROM Features WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", featureId);
                result = command.ExecuteScalar();
            });
            if (result == null || Convert.IsDBNull(result))
            {
                return false;
            }
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Gets a boolean value to indicate whether the module has been installed.
        /// </summary>
        /// <param name="moduleId">The module ID of the module to be verified.</param>
        /// <param name="version">The installed version of the module. If the module has not been installed, returns null.</param>
        /// <returns><c>true</c> if the module has been installed; otherwise <c>false</c>.</returns>
        public bool GetModuleInstalled(string moduleId, out Version version)
        {
            object result = null;
            ExecuteCommand(command =>
            {
                command.CommandText = "SELECT Version FROM Modules WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", moduleId);
                result = command.ExecuteScalar();
            });
            if (result == null || Convert.IsDBNull(result))
            {
                version = null;
                return false;
            }
            version = new Version(Convert.ToString(result));
            return true;
        }

        private bool TryInitializeFromAppConfig(string name)
        {
            var appConfigConnection = ConfigurationManager.ConnectionStrings[name];
            if (appConfigConnection != null)
            {
                _connectionString = appConfigConnection.ConnectionString;
                return true;
            }
            return false;
        }

        private static bool TryGetConnectionName(string nameOrConnectionString, out string name)
        {
            int index = nameOrConnectionString.IndexOf('=');
            if (index < 0)
            {
                name = nameOrConnectionString;
                return true;
            }
            if (nameOrConnectionString.IndexOf('=', index + 1) >= 0)
            {
                name = null;
                return false;
            }
            if (nameOrConnectionString.Substring(0, index).Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                name = nameOrConnectionString.Substring(index + 1).Trim();
                return true;
            }
            name = null;
            return false;
        }
    }
}
