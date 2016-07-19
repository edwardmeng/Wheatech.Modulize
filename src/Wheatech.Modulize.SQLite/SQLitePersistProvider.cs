﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Wheatech.Modulize.SQLite
{
    /// <summary>
    /// The SQLitePersistProvider implements the methods to use SQLite as backend of the modulize engine to persist or retrieve the modules and features activation state.
    /// </summary>
    public class SQLitePersistProvider : IPersistProvider, IDisposable
    {
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private SQLiteConnection _connection;

        /// <summary>
        /// Initialize new instance of <see cref="SQLitePersistProvider"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration name, a connection string or a physical path of database file. </param>
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

        /// <summary>
        /// Saves the installed module by using the module ID and the installed version.
        /// </summary>
        /// <param name="moduleId">The module ID of the installed module.</param>
        /// <param name="version">The version of the installed module.</param>
        public void InstallModule(string moduleId, Version version)
        {
            Initialize();
            ValidateDisposed();
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT OR REPLACE INTO Modules(ID, Version) VALUES(@ID, @Version)";
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
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "DELETE FROM Modules WHERE ID=@ID";
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
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT OR REPLACE INTO Features(ID) VALUES(@ID)";
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
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "DELETE FROM Features WHERE ID=@ID";
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
            object result;
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT COUNT(*) FROM Features WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", featureId);
                result = command.ExecuteScalar();
            }
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
            Initialize();
            ValidateDisposed();
            object result;
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT Version FROM Modules WHERE ID=@ID";
                command.Parameters.AddWithValue("ID", moduleId);
                result = command.ExecuteScalar();
            }
            if (result == null || Convert.IsDBNull(result))
            {
                version = null;
                return false;
            }
            version = new Version(Convert.ToString(result));
            return true;
        }

        private void ValidateDisposed()
        {
            if (_connection == null)
            {
                throw new ObjectDisposedException("SQLitePersistProvider");
            }
        }

        private void Initialize()
        {
            if (_initialized) return;
            string connectionString;
            if (string.IsNullOrEmpty(_nameOrConnectionString))
            {
                connectionString = "Data Source=:memory:;Version=3;New=True;";
            }
            else
            {
                string connectionStringName;
                if (!TryGetConnectionName(_nameOrConnectionString, out connectionStringName) || !TryInitializeFromAppConfig(connectionStringName, out connectionString))
                {
                    if (!string.IsNullOrEmpty(_nameOrConnectionString) && _nameOrConnectionString.IndexOf('=') < 0)
                    {
                        connectionString = $"Data Source={PathUtils.ResolvePath(_nameOrConnectionString)};Version=3;";
                    }
                    else
                    {
                        connectionString = _nameOrConnectionString;
                    }
                }
            }
            EnsureDatabaseFile(connectionString);
            _connection = new SQLiteConnection(connectionString);
            //SQLiteConnection.CreateFile();
            _connection.Open();
            using (var command = new SQLiteCommand(_connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "CREATE TABLE IF NOT EXISTS Modules(ID TEXT NOT NULL PRIMARY KEY, Version TEXT)";
                command.ExecuteNonQuery();

                command.CommandText = "CREATE TABLE IF NOT EXISTS Features(ID TEXT NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            }
            _initialized = true;
        }

        private void EnsureDatabaseFile(string connectionString)
        {
            string dataSource = null;
            foreach (var section in SQLiteConvert.Split(connectionString, ';'))
            {
                var index = section.IndexOf('=');
                if (index != -1)
                {
                    var sectionName = section.Substring(0, index).Trim();
                    if (string.Equals(sectionName, "Data Source", StringComparison.OrdinalIgnoreCase) || string.Equals(sectionName, "Data_Source", StringComparison.OrdinalIgnoreCase))
                    {
                        dataSource = section.Substring(index + 1).Trim();
                        if ((dataSource[0] == '\'' && dataSource[dataSource.Length - 1] == '\'') || (dataSource[0] == '"' && dataSource[dataSource.Length - 1] == '"'))
                        {
                            dataSource = dataSource.Substring(1, dataSource.Length - 2);
                        }
                        break;
                    }
                }
            }
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

        private bool TryInitializeFromAppConfig(string name, out string connectionString)
        {
            var appConfigConnection = ConfigurationManager.ConnectionStrings[name];
            if (appConfigConnection != null)
            {
                connectionString = appConfigConnection.ConnectionString;
                return true;
            }
            connectionString = null;
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
