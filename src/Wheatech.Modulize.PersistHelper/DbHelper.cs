using System;
using System.Configuration;

namespace Wheatech.Modulize.PersistHelper
{
    internal static class DbHelper
    {
        public static bool TryGetConnectionString(string nameOrConnectionString, out string connectionString)
        {
            string connectionStringName;
            connectionString = null;
            if (string.IsNullOrEmpty(nameOrConnectionString)) return false;
            if (TryGetConnectionName(nameOrConnectionString, out connectionStringName))
            {
                var appConfigConnection = ConfigurationManager.ConnectionStrings[connectionStringName];
                if (appConfigConnection != null)
                {
                    connectionString = appConfigConnection.ConnectionString;
                    return true;
                }
            }
            else if (nameOrConnectionString.IndexOf('=') > 0)
            {
                connectionString = nameOrConnectionString;
                return true;
            }
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

        public static string ExtractConnectionStringValue(string connectionString, string name)
        {
            foreach (var section in connectionString.Split(';'))
            {
                var index = section.IndexOf('=');
                if (index != -1)
                {
                    var sectionName = section.Substring(0, index).Trim();
                    if (string.Equals(sectionName, name, StringComparison.OrdinalIgnoreCase) || string.Equals(sectionName, name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(sectionName, name.Replace(' ', '_'), StringComparison.OrdinalIgnoreCase))
                    {
                        var value = section.Substring(index + 1).Trim();
                        if ((value[0] == '\'' && value[value.Length - 1] == '\'') || (value[0] == '"' && value[value.Length - 1] == '"'))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                        return value;
                    }
                }
            }
            return null;
        }
    }
}
