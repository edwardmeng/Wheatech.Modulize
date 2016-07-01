using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Wheatech.Modulize
{
    public class JsonManifestParser : IManifestParser
    {
        public ModuleDescriptor Parse(Stream stream, string moduleId)
        {
            using (var reader = new StreamReader(stream))
            {
                return ManifestHelper.BuildModule(ConvertToDictionary(JObject.Parse(reader.ReadToEnd())), moduleId);
            }
        }

        private Dictionary<string, object> ConvertToDictionary(JObject obj)
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (obj == null) return values;
            foreach (var property in obj.Properties())
            {
                values.Add(property.Name, ConvertJsonValue(property.Value));
            }
            return values;
        }

        private object ConvertJsonValue(JToken value)
        {
            switch (value.Type)
            {
                case JTokenType.Boolean:
                case JTokenType.String:
                case JTokenType.Date:
                case JTokenType.Float:
                case JTokenType.Guid:
                case JTokenType.Integer:
                case JTokenType.TimeSpan:
                case JTokenType.Uri:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    return ((JValue)value).Value;
                case JTokenType.Object:
                    return ConvertToDictionary((JObject)value);
                case JTokenType.Array:
                    var jarray = (JArray)value;
                    var array = new ArrayList();
                    foreach (var element in jarray)
                    {
                        array.Add(ConvertJsonValue(element));
                    }
                    return array.ToArray();
                default:
                    throw new NotSupportedException(string.Format("The json token {0} is not supported", value.Type));
            }
        }
    }
}
