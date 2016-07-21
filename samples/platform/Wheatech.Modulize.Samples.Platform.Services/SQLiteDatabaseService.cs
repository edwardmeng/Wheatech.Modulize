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
        private SQLiteConnection _connection;

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
            string connectionString;
            if (string.IsNullOrEmpty(_nameOrConnectionString))
            {
                connectionString = "Data Source=:memory:;Version=3;New=True;";
            }
            else if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out connectionString))
            {
                connectionString = $"Data Source={PathUtils.ResolvePath(_nameOrConnectionString)};Version=3;";
            }
            EnsureDatabaseFile(connectionString);
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
            _initialized = true;
        }

        private void ValidateDisposed()
        {
            if (_connection == null)
            {
                throw new ObjectDisposedException("SQLiteDatabaseService");
            }
        }

        private void EnsureDatabaseFile(string connectionString)
        {
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
        }

        public int ExecuteNonQuery(string commandText)
        {
            Initialize();
            ValidateDisposed();
            using (var command = new SQLiteCommand(commandText, _connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        public IDataReader ExecuteReader(string commandText)
        {
            Initialize();
            ValidateDisposed();
            using (var command = new SQLiteCommand(commandText, _connection))
            {
                return command.ExecuteReader();
            }
        }

        public object ExecuteScalar(string commandText)
        {
            Initialize();
            ValidateDisposed();
            using (var command = new SQLiteCommand(commandText, _connection))
            {
                return command.ExecuteScalar();
            }
        }
    }
}
