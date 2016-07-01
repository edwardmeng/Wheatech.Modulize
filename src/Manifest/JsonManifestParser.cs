using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class JsonManifestParser : IManifestParser
    {
        public ModuleDescriptor Parse(Stream stream, string moduleId)
        {
            using (var reader = new StreamReader(stream))
            {
                return ManifestHelper.BuildModule(ConvertToDictionary(JObject.Parse(reader.ReadToEnd()), moduleId), moduleId);
            }
        }

        private Dictionary<string, object> ConvertToDictionary(JObject obj, string moduleId)
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (obj == null) return values;
            foreach (var property in obj.Properties())
            {
                values.Add(property.Name, ConvertJsonValue(property.Value, moduleId));
            }
            return values;
        }

        private object ConvertJsonValue(JToken value, string moduleId)
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
                    return ConvertToDictionary((JObject)value, moduleId);
                case JTokenType.Array:
                    var jarray = (JArray)value;
                    var array = new ArrayList();
                    foreach (var element in jarray)
                    {
                        array.Add(ConvertJsonValue(element, moduleId));
                    }
                    return array.ToArray();
                default:
                    throw new NotSupportedException(string.Format(Strings.Manifest_InvalidJsonToken, value.Type, moduleId));
            }
        }
    }
}
