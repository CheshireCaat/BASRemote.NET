

namespace BASApi.CSharp
{
    public class BasRemoteOptions
    {
        /// <summary>
        /// </summary>
        public string WorkingDir { get; set; }

        /// <summary>
        /// </summary>
        public string ScriptName { get; set; }

        /// <summary>
        ///     Password from a user account with access to the script.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Login from a user account with access to the script.
        /// </summary>
        public string Login { get; set; }
    }
}