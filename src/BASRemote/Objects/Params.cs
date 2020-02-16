using System.Collections.Generic;

namespace BASRemote.Objects
{
    /// <summary>
    ///     Class that contains arguments for functions and messages.
    /// </summary>
    public sealed class Params : Dictionary<string, object>
    {
        /// <summary>
        ///     Create an empty instance of <see cref="Options" /> class.
        /// </summary>
        public static Params Empty => new Params();
    }
}