using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.Samples.Platform.Services
{
    public class SQLiteDatabaseService : IDatabaseService
    {
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private string _connectionString;
        private SQLiteConnection _connection;
        private bool _disposed;

        /// <summary>
        /// Initialize new instance of <see cref="SQLiteDatabaseService"/> by using the specified connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the name of connection configuration, a connection string or a physical path of database file. </param>
        public SQLiteDatabaseService(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
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

        private void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("SQLiteDatabaseService");
            }
        }

        private void EnsureDatabaseFile(ref string connectionString)
        {
            connectionString = DbHelper.ReplaceConnectionStringValue(connectionString, (name, value) =>
            {
                if (string.Equals(name, "Data Source", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "DataSource", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "Data_Source", StringComparison.OrdinalIgnoreCase))
                {
                    return PathUtils.ResolvePath(value);
                }
                return value;
            });
            var dataSource = DbHelper.ExtractConnectionStringValue(connectionString, "Data Source");
            if (!string.IsNullOrEmpty(dataSource) && !string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase) && !File.Exists(dataSource))
            {
                var dirPath = Path.GetDirectoryName(dataSource);
                if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                SQLiteConnection.CreateFile(dataSource);
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
            _disposed = true;
        }

        public int ExecuteNonQuery(string commandText)
        {
            Initialize();
            ValidateDisposed();
            int recordCount = 0;
            ExecuteCommand(command =>
            {
                command.CommandText = commandText;
                recordCount = command.ExecuteNonQuery();
            });
            return recordCount;
        }

        public IDataReader ExecuteReader(string commandText)
        {
            Initialize();
            ValidateDisposed();
            IDataReader reader = null;
            ExecuteCommand(command =>
            {
                command.CommandText = commandText;
                reader = command.ExecuteReader();
            });
            return reader;
        }

        public object ExecuteScalar(string commandText)
        {
            Initialize();
            ValidateDisposed();
            object result = null;
            ExecuteCommand(command =>
            {
                command.CommandText = commandText;
                result = command.ExecuteScalar();
            });
            return result;
        }
    }
}
