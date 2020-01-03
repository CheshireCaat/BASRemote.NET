using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using BASRunner.CSharp.Exceptions;

namespace BASRunner.CSharp.Services
{
    public sealed class BasInstallService
    {
        private const string SoftwareName = "BrowserAutomationStudio";

        public event Action OnDownloadCompleted;

        public event Action OnDownloadError;

        public void Install(Version version, string folderPath)
        {
            var uri = new Uri("https://bablosoft.com/distr/" +
                              $"{SoftwareName}/{version.ToString(3)}/" +
                              $"{SoftwareName}InstallAllInOne.exe");
            InstallInternal(uri, Path.Combine(folderPath, $"{SoftwareName}Install.exe"));
        }

        public void Install(string version, string folderPath)
        {
            var uri = new Uri("https://bablosoft.com/distr/" +
                              $"{SoftwareName}/{version}/" +
                              $"{SoftwareName}InstallAllInOne.exe");
            InstallInternal(uri, Path.Combine(folderPath, $"{SoftwareName}Install.exe"));
        }

        private void InstallInternal(Uri address, string fileName)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadProgressChanged += ClientOnDownloadProgressChanged;
                    client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
                    client.DownloadFile(address, fileName);
                    OnDownloadCompleted?.Invoke();
                }
                catch (WebException)
                {
                    OnDownloadError?.Invoke();
                }
            }
        }

        private static void ClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void ClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public static bool IsProgramInstalled(string programDisplayName)
        {
            return true;
        }
    }
}