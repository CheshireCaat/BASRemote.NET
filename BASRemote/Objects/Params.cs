using System.Collections.Generic;
using Newtonsoft.Json;

namespace BASRemote.Objects
{
    public sealed class Params : Dictionary<object, object>
    {
        public static Params Empty => new Params();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}