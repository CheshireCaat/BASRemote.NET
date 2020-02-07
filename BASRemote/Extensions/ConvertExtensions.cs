using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BASRemote.Extensions
{
    internal static class ConvertExtensions
    {
        public static T Convert<T>(this object source)
        {
            switch (source)
            {
                case JObject jObject:
                    return jObject.ToObject<T>();
                case JArray jArray:
                    return jArray.ToObject<T>();
                case JValue jValue:
                    return jValue.ToObject<T>();
                default:
                    return (T) source;
            }
        }

        public static T FromJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static string ToJson(this object source)
        {
            return JsonConvert.SerializeObject(source);
        }
    }
}