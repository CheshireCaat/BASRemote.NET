using System;
using System.IO;

namespace BASRemote
{
    /// <summary>
    ///     Class that contains client settings.
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        ///     Location of the selected working folder.
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

        /// <summary>
        ///     Create an instance of <see cref="Options" /> class.
        /// </summary>
        public Options()
        {
            WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        }
    }
}