using System;
using System.Globalization;
using MySql.Data.MySqlClient;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.MySql
{
    /// <summary>
    /// The MySQLPersistProvider implements the methods to use MySQL as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class MySqlPersistProvider : IPersistProvider, IDisposable
    {
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private MySqlConnection _connection;

        /// <summary>
        /// Initialize new instance of <see cref="MySqlPersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration or a connection string. </param>
        public MySqlPersistProvider(string nameOrConnectionString)
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
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            using (var command = new MySqlCommand())
            {
                command.Connection = _connection;

                command.CommandText = "CREATE TABLE IF NOT EXISTS Modules(ID VARCHAR(256) NOT NULL PRIMARY KEY, Version VARCHAR(256) NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Features(ID VARCHAR(256) NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            }
            _initialized = true;
        }

        private void ValidateDisposed()
        {
            if (_connection == null)
            {
                throw new ObjectDisposedException("MySqlPersistProvider");
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
            using (var command = new MySqlCommand("REPLACE INTO Modules(ID, Version) VALUES(@ID, @Version)", _connection))
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
            using (var command = new MySqlCommand("DELETE FROM Modules WHERE ID=@ID", _connection))
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
            using (var command = new MySqlCommand("REPLACE INTO Features SET ID=@ID", _connection))
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
            using (var command = new MySqlCommand("DELETE FROM Features WHERE ID=@ID", _connection))
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
            using (var command = new MySqlCommand("SELECT COUNT(*) FROM Features WHERE ID=@ID", _connection))
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
            using (var command = new MySqlCommand("SELECT Version FROM Modules WHERE ID=@ID", _connection))
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
