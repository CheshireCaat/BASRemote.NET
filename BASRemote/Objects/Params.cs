using System.Collections.Generic;

namespace BASRemote.Objects
{
    public sealed class Params : Dictionary<string, object>
    {
        public static Params Empty => new Params();
    }
}