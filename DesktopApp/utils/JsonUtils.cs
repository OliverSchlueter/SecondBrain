using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace DesktopApp.utils;

public static class JsonUtils
{
    public static JObject Deserialize(string json)
    {
        return (JObject) JsonConvert.DeserializeObject(json);
    }

    public static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }

    public static List<JProperty> JObject2JProperties(JObject jObject)
    {
        return jObject.Properties().ToList();
    }
}