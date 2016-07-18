using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wheatech.Modulize.Properties;

namespace Wheatech.Modulize
{
    internal class JsonModuleLocator : IModuleLocator
    {
        private const string LocatorFilePath = "~/modules.json";

        public IEnumerable<ModuleLocation> GetLocations()
        {
            var fileInfo = new FileInfo(PathUtils.ResolvePath(LocatorFilePath));
            if (!fileInfo.Exists) yield break;
            using (var stream = fileInfo.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content)) yield break;
                    content = content.Trim();
                    var json = (JToken)JsonConvert.DeserializeObject(content);
                    if (json == null) yield break;
                    foreach (var location in ParseLocations(json, null))
                    {
                        yield return location;
                    }
                }
            }
        }

        private IEnumerable<ModuleLocation> ParseLocations(JToken token, string moduleType, DiscoverStrategy? forceStrategy = null, bool enableShadow = false)
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
                        DiscoverStrategy = forceStrategy ?? DiscoverStrategy.Enumerate,
                        ModuleType = moduleType,
                        EnableShadow = enableShadow
                    };
                    break;
                case JTokenType.Array:
                    foreach (var item in (JArray)token)
                    {
                        foreach (var location in ParseLocations(item, moduleType, DiscoverStrategy.Single, enableShadow))
                        {
                            yield return location;
                        }
                    }
                    break;
                case JTokenType.Object:
                    if (!string.IsNullOrEmpty(moduleType))
                    {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Strings.Locator_InvalidJsonToken, token.Type, LocatorFilePath));
                    }
                    var obj = (JObject)token;
                    var modules = obj.Property("modules");
                    if (modules != null)
                    {
                        var shadow = obj.Property("enableShadow")?.Value.Value<bool>() ?? enableShadow;
                        foreach (var location in ParseLocations(modules.Value, null,enableShadow: shadow))
                        {
                            yield return location;
                        }
                    }
                    else
                    {
                        foreach (var property in obj.Properties())
                        {
                            foreach (var location in ParseLocations(property.Value, property.Name, enableShadow: enableShadow))
                            {
                                yield return location;
                            }
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Strings.Locator_InvalidJsonToken, token.Type, LocatorFilePath));
            }
        }
    }
}
