using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerSettings JsonOptions = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    public static T? FromJson<T>(this string json, T? defaultValue = default)
    {
        return string.IsNullOrWhiteSpace(json) ? defaultValue : JsonConvert.DeserializeObject<T>(json, JsonOptions);
    }

    public static string ToJson<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj, JsonOptions);
    }

    public static T? SelectValue<T>(this JObject source, string path, T? defaultValue = default)
    {
        var token = source.SelectToken(path);
        return token != null ? token.Value<T>() : defaultValue;
    }
}