using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.IO;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.SQLite
{
    /// <summary>
    /// The SQLitePersistProvider implements the methods to use SQLite as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class SQLitePersistProvider : IPersistProvider, IDisposable
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private string _connectionString;
        private SQLiteConnection _connection;
        private ConcurrentDictionary<string, Version> _modules = new ConcurrentDictionary<string, Version>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, bool> _features = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize new instance of <see cref="SQLitePersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration, a connection string or a physical path of database file. </param>
        public SQLitePersistProvider(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Initialize new instance of <see cref="SQLitePersistProvider"/>.
        /// </summary>
        public SQLitePersistProvider() : this(null)
        {
        }

        #endregion

        /// <summary>
        /// Saves the installed module by using the module ID and the installed version.
        /// </summary>
        /// <param name="moduleId">The module ID of the installed module.</param>
        /// <param name="version">The version of the installed module.</param>
        public void InstallModule(string moduleId, Version version)
        {
            Initialize();
            ValidateDisposed();
            lock (_modules)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "INSERT OR REPLACE INTO Modules(ID, Version) VALUES(@ID, @Version)";
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
            ValidateDisposed();
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
            ValidateDisposed();
            lock (_features)
            {
                ExecuteCommand(command =>
                {
                    command.CommandText = "INSERT OR REPLACE INTO Features(ID) VALUES(@ID)";
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
            ValidateDisposed();
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
            ValidateDisposed();
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
            ValidateDisposed();
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

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("SQLitePersistProvider");
            }
        }

        private void Initialize()
        {
            if (_initialized) return;
            if (string.IsNullOrEmpty(_nameOrConnectionString))
            {
                _connectionString = "Data Source=:memory:;Version=3;New=True;Pooling=True;";
            }
            else if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out _connectionString))
            {
                _connectionString = $"Data Source={PathUtils.ResolvePath(_nameOrConnectionString)};Version=3;Pooling=True;";
            }
            EnsureDatabaseFile(ref _connectionString);
            if (string.Equals(DbHelper.ExtractConnectionStringValue(_connectionString, "Data Source"), ":memory:", StringComparison.OrdinalIgnoreCase))
            {
                _connection = new SQLiteConnection(_connectionString);
                _connection.Open();
            }
            ExecuteCommand(command =>
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Modules(ID TEXT NOT NULL PRIMARY KEY, Version TEXT NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Features(ID TEXT NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            });
            _initialized = true;
        }

        private void ExecuteCommand(Action<SQLiteCommand> callback)
        {
            if (_connection == null)
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        callback(command);
                    }
                }
            }
            else
            {
                using (var command = new SQLiteCommand())
                {
                    command.Connection = _connection;
                    callback(command);
                }
            }
        }

        private void EnsureDatabaseFile(ref string connectionString)
        {
            connectionString = DbHelper.ReplaceConnectionStringValue(connectionString, (name, value) =>
            {
                if (string.Equals(name, "Data Source", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "DataSource", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "Data_Source", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(value, ":memory:", StringComparison.OrdinalIgnoreCase))
                    {
                        return PathUtils.ResolvePath(value);
                    }
                }
                return value;
            });
            string dataSource = DbHelper.ExtractConnectionStringValue(connectionString, "Data Source");
            if (!string.IsNullOrEmpty(dataSource) && !string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
            {
                var filePath = PathUtils.ResolvePath(dataSource);
                if (!File.Exists(filePath))
                {
                    var dirPath = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    SQLiteConnection.CreateFile(filePath);
                }
            }
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
            _modules = null;
            _features = null;
            _disposed = true;
        }
    }
}
