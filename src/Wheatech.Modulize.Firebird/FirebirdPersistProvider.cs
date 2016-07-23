using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.Firebird
{
    /// <summary>
    /// The FirebirdPersistProvider implements the methods to use Firebird as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class FirebirdPersistProvider : IPersistProvider
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private string _connectionString;
        private readonly ConcurrentDictionary<string, Version> _modules = new ConcurrentDictionary<string, Version>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, bool> _features = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        #endregion

        /// <summary>
        /// Initialize new instance of <see cref="FirebirdPersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration or a connection string. </param>
        public FirebirdPersistProvider(string nameOrConnectionString)
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
            EnsureDatabaseFile(ref _connectionString);
            ExecuteCommand(command =>
            {
                command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                      "IF (NOT EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'MODULES'))" + Environment.NewLine +
                                      "THEN" + Environment.NewLine +
                                      "EXECUTE STATEMENT 'CREATE TABLE MODULES(ID VARCHAR(256) PRIMARY KEY, VERSION VARCHAR(256))';" + Environment.NewLine +
                                      "END";
                command.ExecuteNonQuery();

                command.CommandText = "EXECUTE BLOCK AS BEGIN" + Environment.NewLine +
                                      "IF (NOT EXISTS(SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = 'FEATURES'))" + Environment.NewLine +
                                      "THEN" + Environment.NewLine +
                                      "EXECUTE STATEMENT 'CREATE TABLE FEATURES(ID VARCHAR(256) PRIMARY KEY)';" + Environment.NewLine +
                                      "END";
                command.ExecuteNonQuery();
            });
            _initialized = true;
        }

        private void EnsureDatabaseFile(ref string connectionString)
        {
            connectionString = DbHelper.ReplaceConnectionStringValue(connectionString, (name, value) =>
            {
                if (string.Equals(name, "database", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "initial catalog", StringComparison.OrdinalIgnoreCase))
                {
                    return PathUtils.ResolvePath(value);
                }
                return value;
            });
            string dataSource = DbHelper.ExtractConnectionStringValue(connectionString, "database") ?? DbHelper.ExtractConnectionStringValue(connectionString, "initial catalog");
            if (!string.IsNullOrEmpty(dataSource) && !File.Exists(dataSource))
            {
                var dirPath = Path.GetDirectoryName(dataSource);
                if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                FbConnection.CreateDatabase(connectionString, true);
            }
        }

        private void ExecuteCommand(Action<FbCommand> callback)
        {
            using (var connection = new FbConnection(_connectionString))
            {
                connection.Open();
                using (var command = new FbCommand())
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
                    command.CommandText = "UPDATE OR INSERT INTO MODULES(ID, VERSION) VALUES(@ID, @Version)";
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
                    command.CommandText = "DELETE FROM MODULES WHERE ID=@ID";
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
                    command.CommandText = "UPDATE OR INSERT INTO FEATURES(ID) VALUES(@ID)";
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
                    command.CommandText = "DELETE FROM FEATURES WHERE ID=@ID";
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
                        command.CommandText = "SELECT COUNT(*) FROM FEATURES WHERE ID=@ID";
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
                        command.CommandText = "SELECT VERSION FROM MODULES WHERE ID=@ID";
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
