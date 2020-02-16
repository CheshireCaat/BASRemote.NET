using System;
using System.IO;

namespace BASRemote.Helpers
{
    internal sealed class FileLock : IDisposable
    {
        private readonly FileStream _lock;

        public FileLock(string path)
        {
            _lock = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }

        public static bool IsLocked(string path)
        {
            try
            {
                using (new FileLock(path))
                {
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}