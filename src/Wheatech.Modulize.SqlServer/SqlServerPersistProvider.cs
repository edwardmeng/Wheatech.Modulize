using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Globalization;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.SqlServer
{
    /// <summary>
    /// The MySQLPersistProvider implements the methods to use Microsoft SQL Server as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class SqlServerPersistProvider : IPersistProvider
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private string _connectionString;
        private readonly ConcurrentDictionary<string, Version> _modules = new ConcurrentDictionary<string, Version>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, bool> _features = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        #endregion

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerPersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration or a connection string. </param>
        public SqlServerPersistProvider(string nameOrConnectionString)
        {
            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty, nameof(nameOrConnectionString));
            }
            _nameOrConnectionString = nameOrConnectionString;
        }

        private void Initialize()
        {
            if (_initialized) return;
            if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out _connectionString))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ConnectionStringNotFound, _nameOrConnectionString));
            }
            ExecuteCommand(command =>
            {
                command.CommandText = "IF OBJECT_ID(N'Modules',N'U') IS NULL CREATE TABLE Modules(ID VARCHAR(256) NOT NULL PRIMARY KEY, Version VARCHAR(256) NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "IF OBJECT_ID(N'Features',N'U') IS NULL CREATE TABLE Features(ID VARCHAR(256) NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            });
            _initialized = true;
        }

        private void ExecuteCommand(Action<SqlCommand> callback)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    callback(command);
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
            Initialize();
            lock (_modules)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "IF EXISTS(SELECT * FROM Modules WHERE ID=@ID)" + Environment.NewLine +
                                          "    UPDATE Modules SET Version=@Version WHERE ID=@ID" + Environment.NewLine +
                                          "ELSE" + Environment.NewLine +
                                          "    INSERT INTO Modules(ID, Version) VALUES(@ID, @Version)";
                    command.Parameters.AddWithValue("ID", moduleId);
                    command.Parameters.AddWithValue("Version", version.ToString());
                    command.ExecuteNonQuery();
                });
                _modules[moduleId] = version;
            }
        }

        /// <summary>
        /// Removes the installed module by using the module ID.
        /// </summary>
        /// <param name="moduleId">The module ID of the uninstalled module.</param>
        public void UninstallModule(string moduleId)
        {
            Initialize();
            lock (_modules)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "DELETE FROM Modules WHERE ID=@ID";
                    command.Parameters.AddWithValue("ID", moduleId);
                    command.ExecuteNonQuery();
                });
                _modules[moduleId] = null;
            }
        }

        /// <summary>
        /// Saves the enabled feature by using the feature ID.
        /// </summary>
        /// <param name="featureId">The feature ID of the enabled feature.</param>
        public void EnableFeature(string featureId)
        {
            Initialize();
            lock (_features)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "IF NOT EXISTS(SELECT * FROM Features WHERE ID=@ID) INSERT INTO Features(ID) VALUES(@ID)";
                    command.Parameters.AddWithValue("ID", featureId);
                    command.ExecuteNonQuery();
                });
                _features[featureId] = true;
            }
        }

        /// <summary>
        /// Removes the enabled feature by using the feature ID.
        /// </summary>
        /// <param name="featureId">The feature ID of the disabled feature.</param>
        public void DisableFeature(string featureId)
        {
            Initialize();
            lock (_features)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "DELETE FROM Features WHERE ID=@ID";
                    command.Parameters.AddWithValue("ID", featureId);
                    command.ExecuteNonQuery();
                });
                _features[featureId] = false;
            }
        }

        /// <summary>
        /// Gets a boolean value to indicate whether the feature has been enabled.
        /// </summary>
        /// <param name="featureId">The feature ID of the feature to be verified.</param>
        /// <returns><c>true</c> if the feature has been enabled; otherwise <c>false</c>.</returns>
        public bool GetFeatureEnabled(string featureId)
        {
            Initialize();
            lock (_features)
            {
                return _features.GetOrAdd(featureId, key =>
                {
                    object result = null;
                    ExecuteCommand(command =>
                    {
                        command.CommandText = "SELECT COUNT(*) FROM Features WHERE ID=@ID";
                        command.Parameters.AddWithValue("ID", key);
                        result = command.ExecuteScalar();
                    });
                    if (result == null || Convert.IsDBNull(result))
                    {
                        return false;
                    }
                    return Convert.ToInt32(result) > 0;
                });
            }
        }

        /// <summary>
        /// Gets a boolean value to indicate whether the module has been installed.
        /// </summary>
        /// <param name="moduleId">The module ID of the module to be verified.</param>
        /// <param name="version">The installed version of the module. If the module has not been installed, returns null.</param>
        /// <returns><c>true</c> if the module has been installed; otherwise <c>false</c>.</returns>
        public bool GetModuleInstalled(string moduleId, out Version version)
        {
            Initialize();
            lock (_modules)
            {
                version = _modules.GetOrAdd(moduleId, key =>
                {
                    object result = null;
                    ExecuteCommand(command =>
                    {
                        command.CommandText = "SELECT Version FROM Modules WHERE ID=@ID";
                        command.Parameters.AddWithValue("ID", key);
                        result = command.ExecuteScalar();
                    });
                    return result == null || Convert.IsDBNull(result) ? null : new Version(Convert.ToString(result));
                });
                return version != null;
            }
        }
    }
}
