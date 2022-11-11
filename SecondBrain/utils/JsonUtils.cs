using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace SecondBrain.utils;

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
}