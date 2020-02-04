using Newtonsoft.Json;

namespace BASRemote.Extensions
{
    public static class ConvertExtensions
    {
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