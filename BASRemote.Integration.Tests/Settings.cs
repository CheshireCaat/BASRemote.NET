using System.Configuration;
using System.Diagnostics;

namespace BASRemote.Integration.Tests
{
    public static class Settings
    {
        private const string Warning = "{0} not configured in app.config! Some tests may fail.";

        public static string Get(string name)
        {
            var value = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(value))
                Debug.WriteLine(Warning, name);
            return value;
        }
    }
}