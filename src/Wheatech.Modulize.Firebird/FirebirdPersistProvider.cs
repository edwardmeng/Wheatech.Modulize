using System;
using System.Globalization;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.Firebird
{
    /// <summary>
    /// The FirebirdPersistProvider implements the methods to use Firebird as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class FirebirdPersistProvider : IPersistProvider, IDisposable
    {
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private FbConnection _connection;

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
            string connectionString;
            if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out connectionString))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ConnectionStringNotFound, _nameOrConnectionString));
            }
            EnsureDatabaseFile(ref connectionString);
            _connection = new FbConnection(connectionString);
            _connection.Open();
            using (var command = new FbCommand())
            {
                command.Connection = _connection;
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
            }
            _initialized = true;
        }

        private void ValidateDisposed()
        {
            if (_connection == null)
            {
                throw new ObjectDisposedException("FirebirdPersistProvider");
            }
        }

        private void EnsureDatabaseFile(ref string connectionString)
        {
            connectionString = DbHelper.ReplaceConnectionStringValue(connectionString, (name, value) =>
            {
                if (string.Equals(name, "database",StringComparison.OrdinalIgnoreCase)||string.Equals(name, "initial catalog", StringComparison.OrdinalIgnoreCase))
                {
                    return PathUtils.ResolvePath(value);
                }
                return value;
            });
            string dataSource = DbHelper.ExtractConnectionStringValue(connectionString, "database") ?? DbHelper.ExtractConnectionStringValue(connectionString, "initial catalog");
            if (!string.IsNullOrEmpty(dataSource))
            {
                var filePath = PathUtils.ResolvePath(dataSource);
                if (!File.Exists(filePath))
                {
                    var dirPath = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    FbConnection.CreateDatabase(connectionString, true);
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
            ValidateDisposed();
            using (var command = new FbCommand("UPDATE OR INSERT INTO MODULES(ID, VERSION) VALUES(@ID, @Version)", _connection))
            {
                command.Parameters.AddWithValue("ID", moduleId);
                command.Parameters.AddWithValue("Version", version.ToString());
                command.ExecuteNonQuery();
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
            using (var command = new FbCommand("DELETE FROM MODULES WHERE ID=@ID", _connection))
            {
                command.Parameters.AddWithValue("ID", moduleId);
                command.ExecuteNonQuery();
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
            using (var command = new FbCommand("UPDATE OR INSERT INTO FEATURES(ID) VALUES(@ID)", _connection))
            {
                command.Parameters.AddWithValue("ID", featureId);
                command.ExecuteNonQuery();
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
            using (var command = new FbCommand("DELETE FROM FEATURES WHERE ID=@ID", _connection))
            {
                command.Parameters.AddWithValue("ID", featureId);
                command.ExecuteNonQuery();
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
            using (var command = new FbCommand("SELECT COUNT(*) FROM FEATURES WHERE ID=@ID", _connection))
            {
                command.Parameters.AddWithValue("ID", featureId);
                var result = command.ExecuteScalar();
                if (result == null || Convert.IsDBNull(result))
                {
                    return false;
                }
                return Convert.ToInt32(result) > 0;
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
            using (var command = new FbCommand("SELECT VERSION FROM MODULES WHERE ID=@ID", _connection))
            {
                command.Parameters.AddWithValue("ID", moduleId);
                var result = command.ExecuteScalar();
                if (result == null || Convert.IsDBNull(result))
                {
                    version = null;
                    return false;
                }
                version = new Version(Convert.ToString(result));
                return true;
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
        }
    }
}
