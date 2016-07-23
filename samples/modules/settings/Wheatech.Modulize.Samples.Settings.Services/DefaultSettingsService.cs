using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Wheatech.Modulize.PersistHelper;
using Wheatech.Modulize.Samples.Caching;
using Wheatech.Modulize.Samples.Platform.Services;

namespace Wheatech.Modulize.Samples.Settings.Services
{
    public class DefaultSettingsService : ISettingsService
    {
        private readonly IDatabaseService _database;
        private readonly ICacheService _caching;
        private readonly IOrderedDictionary _registerdFields = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);

        public DefaultSettingsService(IDatabaseService database, ICacheService caching)
        {
            _database = database;
            _caching = caching;
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty);
            }
            var value = _caching.Get<object>(key);
            if (value == DBNull.Value) return null;
            if (value == null)
            {
                value = _database.ExecuteScalar($"SELECT \"VALUE\" FROM \"Settings\" WHERE \"KEY\"='{key.Replace("'", "''")}'");
                _caching.Put(key, value ?? DBNull.Value);
                if (value == null) return null;
            }
            return Convert.ToString(value);
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Strings.Argument_Cannot_Be_Null_Or_Empty);
            }
            if (value == null)
            {
                _database.ExecuteNonQuery($"DELETE FROM \"Settings\" WHERE \"KEY\"='{key.Replace("'", "''")}'");
            }
            else
            {
                _database.ExecuteNonQuery($"INSERT OR REPLACE INTO \"Settings\"(\"KEY\", \"VALUE\") VALUES('{key.Replace("'", "''")}', '{value.Replace("'", "''")}')");
            }
            _caching.Put(key, (object)value ?? DBNull.Value);
        }

        public void RegisterField(SettingsField field)
        {
            _registerdFields.Add(field.Key, field);
        }

        public IEnumerable<SettingsField> GetFields()
        {
            return _registerdFields.Values.OfType<SettingsField>();
        }
    }
}
