namespace BASRemote
{
    public sealed class BasRemoteOptions
    {
        /// <summary>
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        ///     Name of the selected private script. 
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