using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    public class JsonModuleLocator : IModuleLocator
    {
        private const string LocatorFilePath = "~/modules.json";

        public IEnumerable<ModuleLocation> GetLocations()
        {
            var fileInfo = new FileInfo(PathUtils.ResolvePath(LocatorFilePath));
            if (!fileInfo.Exists)
            {
                yield break;
            }
            using (var stream = fileInfo.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content)) yield break;
                    content = content.Trim();
                    var json = (JToken)JsonConvert.DeserializeObject(content);
                    if (json == null) yield break;
                    switch (json.Type)
                    {
                        case JTokenType.Object:
                            var obj = (JObject)json;
                            foreach (var property in obj.Properties())
                            {
                                foreach (var location in ParseLocations(property.Value))
                                {
                                    location.ModuleType = property.Name;
                                    yield return location;
                                }
                            }
                            break;
                        default:
                            foreach (var location in ParseLocations(json))
                            {
                                yield return location;
                            }
                            break;
                    }
                }
            }
        }

        private IEnumerable<ModuleLocation> ParseLocations(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.None:
                case JTokenType.Comment:
                case JTokenType.Null:
                case JTokenType.Undefined:
                    break;
                case JTokenType.String:
                case JTokenType.Uri:
                case JTokenType.Raw:
                    yield return new ModuleLocation
                    {
                        Location = Convert.ToString(((JValue)token).Value),
                        DiscoverStrategy = DiscoverStrategy.Enumerate
                    };
                    break;
                case JTokenType.Array:
                    foreach (var location in ParseArray((JArray)token))
                    {
                        yield return new ModuleLocation
                        {
                            Location = location,
                            DiscoverStrategy = DiscoverStrategy.Single
                        };
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format(Strings.Locator_InvalidJsonToken, token.Type, LocatorFilePath));
            }
        }

        private IEnumerable<string> ParseArray(JArray array)
        {
            foreach (var token in array)
            {
                switch (token.Type)
                {
                    case JTokenType.String:
                    case JTokenType.Uri:
                    case JTokenType.Raw:
                        yield return Convert.ToString(((JValue)token).Value);
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Strings.Locator_InvalidJsonToken, token.Type, LocatorFilePath));
                }
            }
        }
    }
}
