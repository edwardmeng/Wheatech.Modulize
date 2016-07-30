using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Wheatech.Modulize.PersistHelper;

namespace Wheatech.Modulize.Samples.Platform.Services
{
    public class SqlServerDatabaseService: IDatabaseService
    {
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private string _connectionString;
        public SqlServerDatabaseService(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        private void Initialize()
        {
            if (_initialized) return;
            if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out _connectionString))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ConnectionStringNotFound, _nameOrConnectionString));
            }
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

        public int ExecuteNonQuery(string commandText)
        {
            Initialize();
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
