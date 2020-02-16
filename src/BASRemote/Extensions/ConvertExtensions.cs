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
                case JObject obj:
                    return obj.ToObject<T>();
                case JArray arr:
                    return arr.ToObject<T>();
                case JValue val:
                    return val.ToObject<T>();
            }

            return (T) System.Convert.ChangeType(source, typeof(T));
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