using System;
using System.Data;
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
        private readonly string _nameOrConnectionString;
        private bool _initialized;
        private SqlConnection _connection;

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
            string connectionString;
            if (!DbHelper.TryGetConnectionString(_nameOrConnectionString, out connectionString))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ConnectionStringNotFound, _nameOrConnectionString));
            }
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            using (var command = new SqlCommand())
            {
                command.Connection = _connection;
                command.CommandType = CommandType.Text;

                command.CommandText = "IF OBJECT_ID(N'Modules',N'U') IS NULL CREATE TABLE Modules(ID VARCHAR(256) NOT NULL PRIMARY KEY, Version VARCHAR(256) NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "IF OBJECT_ID(N'Features',N'U') IS NULL CREATE TABLE Features(ID VARCHAR(256) NOT NULL PRIMARY KEY)";
                command.ExecuteNonQuery();
            }
            _initialized = true;
        }

        private void ValidateDisposed()
        {
            if (_connection == null)
            {
                throw new ObjectDisposedException("SqlServerPersistProvider");
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
            using (var command = new SqlCommand(
                "IF EXISTS(SELECT * FROM Modules WHERE ID=@ID)" + Environment.NewLine +
                "    UPDATE Modules SET Version=@Version WHERE ID=@ID" + Environment.NewLine +
                "ELSE" + Environment.NewLine +
                "    INSERT INTO Modules(ID, Version) VALUES(@ID, @Version)", _connection))
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
            using (var command = new SqlCommand("DELETE FROM Modules WHERE ID=@ID", _connection))
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
            using (var command = new SqlCommand("IF NOT EXISTS(SELECT * FROM Features WHERE ID=@ID) INSERT INTO Features(ID) VALUES(@ID)", _connection))
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
            using (var command = new SqlCommand("DELETE FROM Features WHERE ID=@ID", _connection))
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
            using (var command = new SqlCommand("SELECT COUNT(*) FROM Features WHERE ID=@ID", _connection))
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
            using (var command = new SqlCommand("SELECT Version FROM Modules WHERE ID=@ID", _connection))
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
    }
}
